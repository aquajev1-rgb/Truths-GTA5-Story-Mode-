using System;
using System.Drawing;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Windows.Forms;
namespace GTA.Math
{
public struct Vector3
{
    public float X, Y, Z;
    public Vector3(float x, float y, float z)
    {
        X = x;
        Y = y;
        Z = z;
    } public static Vector3 Zero
    {
        get
        {
            return new Vector3();
        }
    } public static Vector3 operator+(Vector3 a, Vector3 b)
    {
        return new Vector3(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
    } public static Vector3 operator*(Vector3 a, float b)
    {
        return new Vector3(a.X * b, a.Y * b, a.Z * b);
    }
}
}
namespace GTA
{
using GTA.Math;
public class Script
{
    public event EventHandler Tick, Aborted;
    public event KeyEventHandler KeyDown, KeyUp;
    public int Interval;
    public static void Yield() {}
}
public enum WeaponHash : uint {Pistol = 1, CarbineRifle = 2, PumpShotgun = 3, Unarmed = 4}
public enum PedHash : uint {Michael = 1, Franklin = 2602752943, Trevor = 2608926626}
public enum VehicleHash : uint {Adder = 1, SultanRS = 2, Banshee = 3, Comet = 4, TurismoR = 5}
public enum WindowTitle {EnterMessage60}
public class Entity
{
    public static Entity FromHandle(int h)
    {
        return new Ped();
    } public int Handle = 1;
    public bool IsPersistent, IsPositionFrozen;
    public bool IsInvincible, IsVisible = true;
    public Vector3 Position, Velocity, Rotation;
    public float Heading;
    public Vector3 ForwardVector
    {
        get
        {
            return new Vector3(0, 1, 0);
        }
    } public bool Alive = true, Released;
    public bool Exists()
    {
        return Alive;
    } public void Delete()
    {
        Alive = false;
    } public void MarkAsNoLongerNeeded()
    {
        Released = true;
    }
}
public class Ped: Entity
{
    public bool IsDead;
    public Model Model = new Model(1);
    public int Health = 180, MaxHealth = 200, Armor = 75;
    public bool InVehicle = true;
    public bool IsInVehicle()
    {
        return InVehicle;
    } public Vehicle CurrentVehicle = new Vehicle();
    public Ped Clone(bool b)
    {
        return new Ped();
    }
}
public class Vehicle: Entity
{
    public float Speed = 28f, DirtLevel, EngineHealth = 1000f, BodyHealth = 1000f, PetrolTankHealth = 1000f;
    public int PassengerCount = 0;
    public void Repair() {} public static VehicleHash[] GetAllModels()
    {
        return (VehicleHash[])Enum.GetValues(typeof(VehicleHash));
    }
}
public class Prop: Entity {}
public struct Model
{
    public int Hash;
    public Model(int h)
    {
        Hash = h;
    } public Model(string n)
    {
        Hash = 1;
    } public bool IsInCdImage
    {
        get
        {
            return true;
        }
    } public bool IsVehicle
    {
        get
        {
            return true;
        }
    } public bool IsPed
    {
        get
        {
            return true;
        }
    } public bool IsValid
    {
        get
        {
            return true;
        }
    } public bool IsPlane
    {
        get
        {
            return false;
        }
    } public bool IsHelicopter
    {
        get
        {
            return false;
        }
    } public bool IsBoat
    {
        get
        {
            return false;
        }
    } public bool Request(int n)
    {
        return true;
    } public void GetDimensions(out Vector3 a, out Vector3 b)
    {
        a = Vector3.Zero;
        b = new Vector3(2, 5, 2);
    } public void MarkAsNoLongerNeeded() {}
}
public class Player
{
    public Ped Character = new Ped();
    public int Handle = 1;
    public int Money = 1250000;
}
public static class Game
{
    public static Player Player = new Player();
    public static bool IsPaused = false;
    public static float LastFrameTime = .016f;
    public static string GetUserInput(WindowTitle w, string s, int n)
    {
        return s;
    } public static string GetLocalizedString(string s)
    {
        return s;
    }
}
public static class World
{
    public static Vehicle CreateVehicle(Model m, Vector3 v, float h)
    {
        return new Vehicle {Position = v};
    } public static Prop CreateProp(Model m, Vector3 v, bool a, bool b)
    {
        return new Prop();
    } public static Ped CreatePed(Model m, Vector3 v, float h)
    {
        return new Ped();
    }
}
}
namespace GTA.Native
{
public class OutputArgument
{
    public object Value;
    public T GetResult<T>()
    {
        return Value == null ? default(T) : (T)Value;
    }
}
public static class Function
{
    public static Func<Hash, object[], object> Hook;
    public static int NativeCalls;
    public static T Call<T>(Hash h, params object[] p)
    {
        NativeCalls++;
        if (Hook != null)
        {
            object custom = Hook(h, p);
            if (custom != null)return (T)custom;
        }
        object value = null;
        if (h == Hash.GET_ASPECT_RATIO)value = 16f / 9f;
        else if (h == Hash.END_TEXT_COMMAND_GET_SCREEN_WIDTH_OF_DISPLAY_TEXT)value = Canvas.TextWidth() / 1920f;
        else if (h == Hash.IS_USING_KEYBOARD_AND_MOUSE || h == Hash.IS_WEAPON_VALID)value = true;
        else if (h == Hash.GET_VEHICLE_CLASS_FROM_NAME)value = 7;
        else if (h == Hash.GET_DISPLAY_NAME_FROM_VEHICLE_MODEL)value = "Sultan RS";
        else if (h == Hash.GET_NUM_VEHICLE_MODS)value = 6;
        else if (h == Hash.GET_NUMBER_OF_PED_DRAWABLE_VARIATIONS)value = 30;
        if (value != null)return (T)value;
        return default(T);
    }
    public static void Call(Hash h, params object[] p)
    {
        NativeCalls++;
        if (Hook != null)Hook(h, p);
        if (h == Hash.DRAW_RECT)Canvas.Rect(p);
        else if (h == Hash.SET_SCRIPT_GFX_DRAW_ORDER)Canvas.Layer = Convert.ToInt32(p[0]);
        else if (h == Hash.SET_TEXT_SCALE)
        {
            Canvas.Scale = Convert.ToSingle(p[1]);
            Canvas.ScaleWasSet = true;
        }
        else if (h == Hash.SET_TEXT_COLOUR)Canvas.Color = Canvas.Rgba(p);
        else if (h == Hash.BEGIN_TEXT_COMMAND_DISPLAY_TEXT || h == Hash.BEGIN_TEXT_COMMAND_GET_SCREEN_WIDTH_OF_DISPLAY_TEXT)Canvas.Text = "";
        else if (h == Hash.ADD_TEXT_COMPONENT_SUBSTRING_PLAYER_NAME)Canvas.Text += (string)p[0];
        else if (h == Hash.END_TEXT_COMMAND_DISPLAY_TEXT)Canvas.WriteText(Convert.ToSingle(p[0]), Convert.ToSingle(p[1]));
    }
}
}
namespace GTA.UI
{
public class CustomSprite
{
    public string File;
    public PointF Position;
    public SizeF Size;
    public CustomSprite(string file, SizeF size, PointF pos, Color color)
    {
        File = file;
        Size = size;
        Position = pos;
    } public void Draw()
    {
        Canvas.Image(File, Position.X / 1280, Position.Y / 720, Size.Width / 1280, Size.Height / 720);
    }
}
}
public static class Canvas
{
    public static StringBuilder Svg = new StringBuilder();
    public static string Text = "", Color = "#aaffaa";
    public static float Scale;
    public static int Rectangles, Dropped, Layer, UnstyledText;
    public static bool ScaleWasSet;
    public static readonly List<float[]> RectangleBounds = new List<float[]>();
    public static readonly List<string> BackdropText = new List<string>();
    public static void BeginFrame()
    {
        Svg.Clear();
        Rectangles = Dropped = UnstyledText = 0;
        Layer = 4;
        ScaleWasSet = false;
        RectangleBounds.Clear();
        BackdropText.Clear();
    }
    public static string F(double n)
    {
        return n.ToString("0.###", System.Globalization.CultureInfo.InvariantCulture);
    }
    public static string E(string s)
    {
        return System.Security.SecurityElement.Escape(s);
    }
    public static string Rgba(object[] p)
    {
        return "rgba(" + p[0] + "," + p[1] + "," + p[2] + "," + F(Convert.ToDouble(p[3]) / 255.0) + ")";
    }
    public static void Rect(object[] p)
    {
        Rectangles++;
        if (Rectangles > 399)
        {
            Dropped++;
            return;
        }
        float nx = Convert.ToSingle(p[0]), ny = Convert.ToSingle(p[1]), nw = Convert.ToSingle(p[2]), nh = Convert.ToSingle(p[3]);
        RectangleBounds.Add(new float[] {nx - nw / 2, ny - nh / 2, nw, nh});
        float x = nx * 1920, y = ny * 1080, w = nw * 1920, h = nh * 1080;
        Svg.Append("<rect x='" + F(x - w / 2) + "' y='" + F(y - h / 2) + "' width='" + F(w) + "' height='" + F(h) + "' fill='" + Rgba(new object[] {p[4], p[5], p[6], p[7]})
                   + "'/>");
    }
    public static float TextWidth()
    {
        float units = 0;
        foreach (char c in Text)units += (" il.,:!'".IndexOf(c) >= 0?.26f : "MW@".IndexOf(c) >= 0?.85f : .56f);
        return units * Scale * 68f;
    }
    public static void WriteText(float x, float y)
    {
        if (!ScaleWasSet)UnstyledText++;
        if (Layer == 3)BackdropText.Add(Text);
        Svg.Append("<text x='" + F(x * 1920) + "' y='" + F(y * 1080 + Scale * 68f * .9f) + "' font-family='DejaVu Sans,sans-serif' font-size='" + F(
                       Scale * 68f) + "' fill='" + Color + "'>" + E(Text) + "</text>");
        ScaleWasSet = false;
        Scale = 1f;
    }
    public static void Image(string file, float x, float y, float w, float h)
    {
        Svg.Append("<image x='" + F(x * 1920) + "' y='" + F(y * 1080) + "' width='" + F(w * 1920) + "' height='" + F(
                       h * 1080) + "' href='data:image/png;base64," + Convert.ToBase64String(System.IO.File.ReadAllBytes(file)) + "'/>");
    }
    public static void Save(string path)
    {
        File.WriteAllText(path,
                          "<svg xmlns='http://www.w3.org/2000/svg' width='1160' height='1000' viewBox='0 0 1160 1000'><defs><pattern id='grid' width='30' height='30' patternUnits='userSpaceOnUse'><rect width='30' height='30' fill='#17231c'/><path d='M30 0H0V30' stroke='#24372a' fill='none'/></pattern></defs><rect width='1160' height='1000' fill='url(#grid)'/>"
                          + Svg + "</svg>");
        Svg.Clear();
    }
}
