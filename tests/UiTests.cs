using System;
using System.Collections;
using System.IO;
using System.Reflection;
using GTA;
using GTA.Native;

// Offline tests: the native adapter enforces a 399-rectangle frame budget.
// This exercises actual menu methods, but does not emulate GTA's font renderer.
public static class UiTests
{
    private const BindingFlags Private = BindingFlags.NonPublic | BindingFlags.Instance;
    private static int checks, frames, maximum;
    private static void Set(object menu, string name, object value)
    {
        menu.GetType().GetField(name, Private).SetValue(menu, value);
    }
    private static object Get(object menu, string name)
    {
        return menu.GetType().GetField(name, Private).GetValue(menu);
    }
    private static object Call(object menu, string name, params object[] args)
    {
        return menu.GetType().GetMethod(name, Private).Invoke(menu, args);
    }
    private static void Check(bool value, string message)
    {
        checks++;
        if (!value) throw new Exception(message);
    }

    public static void Main()
    {
        var menu = new TruthStoryPlus();
        // Direct mask validation catches truncated/invalid embedded data.
        int[] spans = (int[])typeof(TruthStoryPlus).GetField("LogoSpans", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null);
        Check(spans.Length == 36 * 4, "compact mask span count");
        for (int i = 0; i < spans.Length; i += 4)
            Check(spans[i] >= 0 && spans[i + 1] >= 0 && spans[i + 2] > 0 && spans[i + 3] > 0 && spans[i] + spans[i + 2] <= 32 &&
                  spans[i + 1] + spans[i + 3] <= 32, "logo mask stays inside badge");

        foreach (float ratio in new float[] { 4f / 3f, 16f / 9f, 21f / 9f, 32f / 9f })
            foreach (int scale in new int[] { 80, 100, 105 })
                foreach (bool right in new bool[] { false, true })
                    foreach (bool controller in new bool[] { false, true })
                        foreach (bool watermark in new bool[] { false, true })
                        {
                            Function.Hook = delegate(Hash hash, object[] args)
                            {
                                if (hash == Hash.GET_ASPECT_RATIO) return ratio;
                                if (hash == Hash.IS_USING_KEYBOARD_AND_MOUSE) return !controller;
                                return null;
                            };
                            Set(menu, "scalePercent", scale);
                            Set(menu, "rightSide", right);
                            Set(menu, "watermark", watermark);
                            Set(menu, "toast", "Validation notice");
                            for (int page = 0; page < 14; page++)
                            {
                                Set(menu, "page", page);
                                Set(menu, "cursor", 0);
                                Set(menu, "scroll", 0);
                                Call(menu, "Build");
                                IList options = (IList)Get(menu, "options");
                                foreach (int selected in new int[] { 0, options.Count - 1 })
                                {
                                    Set(menu, "cursor", selected);
                                    Call(menu, "FixScroll");
                                    Canvas.BeginFrame();
                                    Call(menu, "DrawMenu");
                        Call(menu, "DrawToast");
                        Check(Canvas.Svg.ToString().Contains(">Truths Story Mode+</text>"), "notification brand is complete and untruncated");
                                    frames++;
                                    maximum = Math.Max(maximum, Canvas.Rectangles);
                                    Check(Canvas.Rectangles <= 330, "UI leaves headroom under native rectangle limit");
                                    Check(Canvas.Dropped == 0, "footer and toast survive rectangle budget");
                                    Check(Canvas.UnstyledText == 0, "all text resets font scale, including each binary string");
                                    Check(Canvas.Layer == 4, "content and toast return to content layer");
                                    Check(Canvas.BackdropText.Count >= 18, "binary backdrop drawn");
                                    foreach (string text in Canvas.BackdropText)
                                        Check(text.Length <= 3 && text.Replace("0", "").Replace("1", "").Length == 0, "only small binary glyphs in background layer");
                                    foreach (float[] bounds in Canvas.RectangleBounds)
                                        Check(bounds[0] >= 0 && bounds[1] >= 0 && bounds[2] > 0 && bounds[3] > 0 && bounds[0] + bounds[2] <= 1.0001f &&
                                              bounds[1] + bounds[3] <= 1.0001f, "rectangle fits tested screen");
                                }
                            }
                        }
        // No glyph, rectangle or texture should depend on an external logo file.
        Canvas.BeginFrame();
        Call(menu, "Logo", 0f, 0f, 32f);
        Check(Canvas.Rectangles == 37, "complete logo uses one backdrop and 36 white spans");

        string directory = Path.Combine(Path.GetTempPath(), "truth-ui-test-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(directory);
        try
        {
            string log = Path.Combine(directory, "test.log");
            Set(menu, "dataDir", directory);
            Set(menu, "logPath", log);
            Call(menu, "LogException", "Validation", new IOException("PRIVATE_SENTINEL_DO_NOT_LOG"));
            string content = File.ReadAllText(log);
            Check(content.Contains("IOException") && !content.Contains("PRIVATE_SENTINEL"), "diagnostics omit private exception messages");
        }
        finally
        {
            Directory.Delete(directory, true);
        }
        Function.Hook = null;
        // Input callbacks only queue; game-thread processing retains F5 behavior.
        Set(menu, "opened", false);
        Call(menu, "OnKeyDown", null, new System.Windows.Forms.KeyEventArgs(System.Windows.Forms.Keys.F5));
        Check(!(bool)Get(menu, "opened"), "key callback does not execute menu actions");
        Call(menu, "ReadKeyboard");
        Check((bool)Get(menu, "opened"), "F5 opens on the script thread");
        Call(menu, "OnKeyDown", null, new System.Windows.Forms.KeyEventArgs(System.Windows.Forms.Keys.F5));
        Call(menu, "ReadKeyboard");
        Check((bool)Get(menu, "opened"), "held F5 does not repeat-toggle");
        Call(menu, "OnKeyUp", null, new System.Windows.Forms.KeyEventArgs(System.Windows.Forms.Keys.F5));
        Call(menu, "OnKeyDown", null, new System.Windows.Forms.KeyEventArgs(System.Windows.Forms.Keys.F5));
        Call(menu, "ReadKeyboard");
        Check(!(bool)Get(menu, "opened"), "second F5 press closes");
        Set(menu, "opened", true);
        Set(menu, "page", 0);
        Set(menu, "cursor", 0);
        Set(menu, "scroll", 0);
        Call(menu, "Build");
        Function.Hook = delegate(Hash hash, object[] args)
        {
            if (hash == Hash.IS_USING_KEYBOARD_AND_MOUSE) return false;
            if (hash == Hash.IS_DISABLED_CONTROL_JUST_PRESSED) return (int)args[1] == 201;
            return null;
        };
        Call(menu, "ReadPad");
        Check((int)Get(menu, "page") == 1, "controller apply activates the selected home shortcut");
        Function.Hook = delegate(Hash hash, object[] args)
        {
            if (hash == Hash.IS_USING_KEYBOARD_AND_MOUSE) return false;
            if (hash == Hash.IS_DISABLED_CONTROL_JUST_PRESSED) return (int)args[1] == 202;
            return null;
        };
        Call(menu, "ReadPad");
        Check(!(bool)Get(menu, "opened"), "controller back closes a root page");
        Function.Hook = null;
        Console.WriteLine("PASS: " + checks + " UI/privacy assertions over " + frames + " frames; maximum " + maximum +
                          " rectangles including toast.");
    }
}
