using System;
public static class TruthTests
{
    private static int count;
    private static void Check(bool ok, string name)
    {
        count++;
        if (!ok)throw new Exception(name);
    }
    public static void Main()
    {
        Check(TruthMenuLogic.Wrap(-1, 14) == 13, "previous category wraps");
        Check(TruthMenuLogic.Wrap(14, 14) == 0, "next category wraps");
        Check(TruthMenuLogic.Wrap(-3, 0) == 0, "empty catalog is safe");
        Check(TruthMenuLogic.Scroll(0, 20, 9) == 0, "scroll resets on shorter page");
        Check(TruthMenuLogic.Scroll(9, 0, 9) == 1, "next item scrolls into view");
        Check(TruthMenuLogic.Scroll(8, 0, 9) == 0, "ninth item stays on first page");
        Check(TruthMenuLogic.Scroll(888, 0, 9) == 880, "large catalog selection visible");
        Check(TruthMenuLogic.ClampMoney(long.MaxValue) == 2000000000, "cash addition overflow protected");
        Check(TruthMenuLogic.ClampMoney(long.MinValue) == 0, "cash subtraction floor protected");
        Check(Math.Abs(TruthMenuLogic.MphToMps(60) - 26.8224f) < .001f, "60 MPH conversion");
        Check(TruthMenuLogic.Matches("Sultan RS", "sUlTaN"), "case insensitive search");
        Check(TruthMenuLogic.Matches("Sultan RS", ""), "empty search shows catalog");
        Check(!TruthMenuLogic.Matches("Sultan RS", "Adder"), "nonmatch filtered");
        int n;
        Check(TruthMenuLogic.TryMoney("$1,234,567", out n) && n == 1234567, "formatted cash input");
        Check(TruthMenuLogic.TryMoney("2000000000", out n) && n == 2000000000, "maximum cash input");
        Check(!TruthMenuLogic.TryMoney("2000000001", out n), "out of range cash rejected");
        Check(!TruthMenuLogic.TryMoney("-1", out n), "negative cash rejected");
        Check(!TruthMenuLogic.TryMoney("1.5", out n), "fractional cash rejected");
        Check(!TruthMenuLogic.TryMoney("garbage", out n), "bad cash rejected");
        int[] rgb;
        Check(TruthMenuLogic.TryRgb("0, 255, 55", out rgb) && rgb[1] == 255, "RGB input");
        Check(!TruthMenuLogic.TryRgb("0 256 20", out rgb), "RGB upper bound");
        Check(!TruthMenuLogic.TryRgb("-1 5 20", out rgb), "RGB lower bound");
        Check(!TruthMenuLogic.TryRgb("10 20", out rgb), "RGB channel count");
        float[] p;
        Check(TruthMenuLogic.TryPosition("-1034,-2730,20,180", out p) && p[2] == 20, "saved location");
        Check(!TruthMenuLogic.TryPosition("NaN,0,0,0", out p), "NaN location rejected");
        Check(!TruthMenuLogic.TryPosition("Infinity,0,0,0", out p), "infinite location rejected");
        Check(!TruthMenuLogic.TryPosition("50000,0,0,0", out p), "invalid coordinates rejected");
        Check(!TruthMenuLogic.TryPosition("0,0,0", out p), "incomplete location rejected");
        foreach (int mph in new int[] {179, 180, 181, 200, 250, 400})
            foreach (int cap in new int[] {0, 5, 180, 400})
                foreach (float dt in new float[] {0f, 1f / 144f, 1f / 60f, 1f / 30f, 1f})
                {
                    float speed = TruthMenuLogic.MphToMps(mph);
                    float limited = TruthMenuLogic.LimitedSpeed(speed, cap, dt);
                    Check(limited > 0f, "high-speed cap never stops car");
                    Check(limited <= speed && limited >= speed - .6001f, "bounded gradual deceleration");
                    if (cap == 0 || mph <= cap)Check(limited == speed, "off/below cap preserves momentum");
                    float boost = TruthMenuLogic.BoostGain(speed, cap, dt);
                    Check(boost >= 0f && boost <= 1.0001f, "boost never brakes and clamps stalled frames");
                    if (cap > 0 && mph >= cap)Check(boost == 0f, "boost respects cap without snapping speed");
                }
        float over = TruthMenuLogic.MphToMps(250);
        for (int frame = 0; frame < 600; frame++)over = TruthMenuLogic.LimitedSpeed(over, 180, 1f / 60f);
        Check(Math.Abs(over - TruthMenuLogic.MphToMps(180)) < .001f, "limiter converges to selected MPH");
        for (int rows = 1; rows < 12; rows++)for (int selected = 0; selected < 1500; selected++)
            {
                int offset = TruthMenuLogic.Scroll(selected, 0, rows);
                Check(offset >= 0 && selected >= offset && selected < offset + rows, "catalog row visibility");
            }
        Console.WriteLine("PASS: " + count + " assertions across navigation, parsing, bounds and units.");
    }
}
