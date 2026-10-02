using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using GTA;
using GTA.Math;
using GTA.Native;

// SPDX-License-Identifier: PolyForm-Noncommercial-1.0.0
// Truth's GTA5 Story Mode+ 1.0.0 | RELEASE | Enhanced API v3
// F5 opens. All game/native work executes on the script thread, never in key callbacks.
public class TruthStoryPlus : Script
{
    private const string Version = "1.0.0 / RELEASE";
    private readonly string[] pages =
    {
        "HOME",
        "PLAYER",
        "CASH",
        "SPAWNER",
        "VEHICLE",
        "PERFORMANCE",
        "CUSTOM SHOP",
        "WEAPONS",
        "WARDROBE",
        "TELEPORT",
        "WORLD",
        "PLAYGROUND",
        "SCENARIOS",
        "INTERFACE"
    };
    private readonly string[] pageHints =
    {
        "Your private Los Santos sandbox.",
        "Survival, movement and wanted controls.",
        "Edit the active Story Mode character's balance.",
        "Every vehicle model reported by your game.",
        "Care for and control your current vehicle.",
        "Power, torque and a speed limiter in MPH.",
        "Bodywork, paint, wheels, lights and plates.",
        "Browse your API's weapon catalog.",
        "Clothing, accessories and saved looks.",
        "Favorite places and saved coordinates.",
        "Weather, clock, traffic and camera effects.",
        "A little chaos, on your terms.",
        "Give your character something to do.",
        "Make the menu feel like yours."
    };
    private readonly string[] weatherNames =
    {
        "CLEAR",
        "EXTRASUNNY",
        "CLOUDS",
        "OVERCAST",
        "RAIN",
        "THUNDER",
        "FOGGY",
        "SMOG",
        "CLEARING",
        "NEUTRAL",
        "SNOW",
        "BLIZZARD",
        "SNOWLIGHT",
        "XMAS",
        "HALLOWEEN"
    };
    private readonly string[] componentNames = { "Face", "Mask", "Hair", "Torso", "Legs", "Bags", "Shoes", "Accessories", "Undershirt", "Armor", "Decals", "Jacket" };
    private readonly string[] propNames = { "Hat", "Glasses", "Ears", "Watch", "Bracelet" };
    private readonly int[] propIds = { 0, 1, 2, 6, 7 };
    private readonly string[] modNames =
    {
        "Spoilers",
        "Front bumpers",
        "Rear bumpers",
        "Side skirts",
        "Exhausts",
        "Chassis",
        "Grilles",
        "Hoods",
        "Left fenders",
        "Right fenders",
        "Roofs",
        "Engine",
        "Brakes",
        "Transmission",
        "Horns",
        "Suspension",
        "Armor",
        "Front wheels",
        "Rear wheels",
        "Plate holders",
        "Trim",
        "Ornaments",
        "Dials",
        "Seats",
        "Steering wheels",
        "Shifters",
        "Plaques",
        "Trunk",
        "Hydraulics",
        "Engine blocks",
        "Air filters",
        "Strut braces",
        "Arch covers",
        "Aerials",
        "Trim design",
        "Fuel tanks",
        "Windows",
        "Liveries"
    };
    private readonly int[] modIds =
    {
        0,
        1,
        2,
        3,
        4,
        5,
        6,
        7,
        8,
        9,
        10,
        11,
        12,
        13,
        14,
        15,
        16,
        23,
        24,
        25,
        27,
        28,
        30,
        32,
        33,
        34,
        35,
        37,
        38,
        39,
        40,
        41,
        42,
        43,
        44,
        45,
        46,
        48
    };
    private readonly string[] classNames =
    {
        "All classes",
        "Compacts",
        "Sedans",
        "SUVs",
        "Coupes",
        "Muscle",
        "Sports classics",
        "Sports",
        "Super",
        "Motorcycles",
        "Off-road",
        "Industrial",
        "Utility",
        "Vans",
        "Cycles",
        "Boats",
        "Helicopters",
        "Planes",
        "Service",
        "Emergency",
        "Military",
        "Commercial",
        "Trains",
        "Open wheel"
    };
    private readonly string[] scenarioNames =
    {
        "Dance / party",
        "Street musician",
        "Human statue",
        "Push-ups",
        "Sit-ups",
        "Yoga",
        "Meditate / picnic",
        "Drink coffee",
        "Smoke",
        "Use binoculars",
        "Fishing",
        "Jog in place",
        "Film with phone",
        "Tourist camera",
        "Stand guard",
        "Weld",
        "Hammer",
        "Golf swing"
    };
    private readonly string[] scenarioIds =
    {
        "WORLD_HUMAN_PARTYING",
        "WORLD_HUMAN_MUSICIAN",
        "WORLD_HUMAN_HUMAN_STATUE",
        "WORLD_HUMAN_PUSH_UPS",
        "WORLD_HUMAN_SIT_UPS",
        "WORLD_HUMAN_YOGA",
        "WORLD_HUMAN_PICNIC",
        "WORLD_HUMAN_DRINKING",
        "WORLD_HUMAN_SMOKING",
        "WORLD_HUMAN_BINOCULARS",
        "WORLD_HUMAN_STAND_FISHING",
        "WORLD_HUMAN_JOG_STANDING",
        "WORLD_HUMAN_MOBILE_FILM_SHOCKING",
        "WORLD_HUMAN_TOURIST_MOBILE",
        "WORLD_HUMAN_GUARD_STAND",
        "WORLD_HUMAN_WELDING",
        "WORLD_HUMAN_HAMMERING",
        "WORLD_HUMAN_GOLF_PLAYER"
    };
    private readonly Queue<Keys> keyQueue = new Queue<Keys>();
    private readonly HashSet<Keys> heldKeys = new HashSet<Keys>();
    private readonly Stopwatch clock = Stopwatch.StartNew();
    private readonly Random random = new Random();
    private readonly List<Option> options = new List<Option>();
    private readonly List<VehicleEntry> vehicles = new List<VehicleEntry>();
    private readonly List<WeaponEntry> weapons = new List<WeaponEntry>();
    private readonly HashSet<int> favorites = new HashSet<int>();
    private readonly List<Entity> toys = new List<Entity>();
    private readonly List<Vehicle> spawned = new List<Vehicle>();
    private readonly Dictionary<string, string> prefs = new Dictionary<string, string>();
    private readonly int[] remembered = new int[14];
    private string dataDir, logPath, query = "", toast = "", confirm = "", view = "main";
    private long toastUntil, confirmUntil, nextRepeat, nextErrorLog, nextRainbow, nextHop;
    private int page, cursor, scroll, lastPad = -1, lastKey = -1, errors;
    private bool opened, initialized, ready, busy, sound = true, binary = true, hints = true, watermark = true, rightSide, speedometer;
    private int opacity = 185, scalePercent = 100, visibleRows = 9;
    private float aspect = 16f / 9f, left, top = .045f, uiScale = 1f;
    // Native monochrome logo: independent of Enhanced's external texture hook.
    // 32 x 32 mask, merged white runs: 36 spans instead of 200 gray spans.
    // Keep both logos inside the shared native rectangle budget (see tests).
    private const int LogoGrid = 32;
    private static readonly int[] LogoSpans =
    {
        13, 4, 5, 1, 10, 5, 11, 1, 8, 6, 15, 1, 7, 7, 5, 1,
        18, 7, 6, 1, 6, 8, 5, 1, 19, 8, 6, 1, 6, 9, 4, 1,
        20, 9, 6, 2, 5, 10, 5, 1, 5, 11, 4, 1, 14, 11, 2, 1,
        19, 11, 1, 1, 21, 11, 6, 2, 4, 12, 5, 2, 14, 12, 3, 2,
        18, 12, 2, 1, 19, 13, 1, 1, 21, 13, 7, 2, 4, 14, 6, 3,
        20, 15, 8, 4, 4, 17, 7, 2, 4, 19, 8, 1, 19, 19, 9, 1,
        4, 20, 7, 1, 19, 20, 8, 2, 4, 21, 6, 1, 5, 22, 6, 1,
        20, 22, 6, 2, 6, 23, 5, 1, 7, 24, 3, 1, 20, 24, 5, 1,
        8, 25, 2, 1, 20, 25, 4, 1, 9, 26, 1, 1, 20, 26, 2, 1
    };
    private bool trafficControl, trafficOnly, trafficFpsGuard = true;
    private int trafficDensity = 100, trafficType, trafficLimit = 60;
    private float trafficFrameTime = 1f / 60f;
    private string trafficStatus = "OFF";
    private long nextTraffic, trafficPendingUntil, nextTrafficCleanup, trafficLastFrame;
    private int trafficPendingHash;
    private readonly Model trafficDriverModel = new Model("a_m_y_business_01");
    private readonly List<TrafficPair> trafficCars = new List<TrafficPair>();
    private readonly string[] trafficNames = {"Normal road mix", "Supercars", "Sports", "Muscle", "Off-road / SUVs", "Motorcycles", "Commercial", "Chosen spawner model"};
    private readonly string[][] trafficModels =
    {
        new string[]{"asea", "blista", "primo", "sultan", "baller", "buffalo", "fugitive", "stanier"},
        new string[]{"adder", "zentorno", "turismor", "t20", "osiris", "infernus"},
        new string[]{"sultan", "comet2", "elegy2", "banshee", "jester", "carbonizzare"},
        new string[]{"dominator", "gauntlet", "ruiner", "vigero", "sabregt", "dukes"},
        new string[]{"rebel2", "sandking", "dubsta", "baller", "mesa", "bifta"},
        new string[]{"bati", "akuma", "double", "sanchez", "daemon", "bagger"},
        new string[]{"benson", "mule", "pounder", "boxville", "burrito3", "bus"}
    };
    private sealed class TrafficPair
    {
        public Vehicle Car;
        public Ped Driver;
    }
    private bool firstPersonFov;
    private int fovPercent = 115, fovCamera;

    private bool god, neverWanted, stamina, superJump, invisible, noRagdoll, policeIgnore, everyoneIgnore;
    private int wanted, runIndex, swimIndex;
    private readonly float[] movement = { 1f, 1.15f, 1.3f, 1.49f };
    private int moneyAmount = 100000, vehicleClass;
    private bool favoritesOnly, warpSpawn = true;
    private int selectedVehicleHash;
    private string selectedVehicleName = "Choose a vehicle";
    private bool carGod, autoRepair, seatbelt, drift, rainbow, bunny, hornBoost;
    private int powerPercent, torquePercent = 100, mphCap, launchMph = 60;
    private int modSlot, primaryPaint, secondaryPaint, wheelType, tint, livery, extraId = 1, neonStyle;
    private string plate = "TRUTH";
    private bool infiniteAmmo, infiniteClip, explosiveAmmo, fireAmmo, explosiveMelee;
    private int damagePercent = 100, weaponTint;
    private uint selectedWeapon = (uint)WeaponHash.Pistol;
    private string selectedWeaponName = "Pistol";
    private readonly HashSet<uint> infiniteAmmoWeapons = new HashSet<uint>();
    private int clothingSlot = 11, propSlot, outfitSlot, teleportSlot, hour = 12, minute, weatherIndex;
    private bool freezeClock, blackout, noTraffic, noPeds, nightVision, thermal, hideHud, hideRadar;
    private int timePercent = 100, gravityMode;
    private PedState pedState;
    private CarState carState;
    private Vector3? previousPosition;
    private float previousHeading;
    private readonly string[] placeNames =
    {
        "Los Santos Airport",
        "Vespucci Beach",
        "Del Perro Pier",
        "Observatory",
        "Vinewood sign",
        "Sandy Shores airfield",
        "Mount Chiliad summit",
        "Paleto Bay",
        "Grapeseed airstrip",
        "Golf club",
        "Los Santos Customs",
        "Casino exterior"
    };
    private readonly Vector3[] places =
    {
        new Vector3(-1034, -2730, 20),
        new Vector3(-1195, -1509, 4.4f),
        new Vector3(-1640, -1010, 13),
        new Vector3(-438, 1075, 353),
        new Vector3(698, 1193, 325),
        new Vector3(1738, 3283, 41),
        new Vector3(501, 5604, 797),
        new Vector3(-111, 6469, 31.6f),
        new Vector3(2129, 4805, 41),
        new Vector3(-1368, 56, 54),
        new Vector3(-365, -130, 38.7f),
        new Vector3(921, 47, 81)
    };

    private sealed class Option
    {
        public string Name, Help, Kind;
        public Func<string> Value;
        public Action Run;
        public Action<int> Change;
        public Func<bool> Enabled;
    }
    private sealed class VehicleEntry
    {
        public int Hash, Class;
        public string Name, Model;
    }
    private sealed class WeaponEntry
    {
        public uint Hash;
        public string Name;
    }
    private sealed class PedState
    {
        public Ped Ped;
        public bool Invincible, Visible, Ragdoll;
        public bool TouchedGod, TouchedVisible, TouchedRagdoll, TouchedAmmo;
    }
    private sealed class CarState
    {
        public Vehicle Car;
        public bool Invincible;
        public bool TouchedGod, TouchedPower, TouchedGrip, TouchedRainbow;
        public int AppliedPower, Primary, Secondary, Pr, Pg, Pb, Sr, Sg, Sb;
        public bool CustomPrimary, CustomSecondary;
    }
    private Ped Hero
    {
        get
        {
            return Game.Player.Character;
        }
    }
    private Vehicle Ride
    {
        get
        {
            Ped p = Hero;
            return Valid(p) && p.IsInVehicle() ? p.CurrentVehicle : null;
        }
    }
    private static bool Valid(Entity e)
    {
        return e != null && e.Exists();
    }
    private bool HasCar()
    {
        return Valid(Ride);
    }
    private long Now
    {
        get
        {
            return clock.ElapsedMilliseconds;
        }
    }
    private static bool Story()
    {
        return !Function.Call<bool>(Hash.NETWORK_IS_SESSION_ACTIVE) && !Function.Call<bool>(Hash.NETWORK_IS_GAME_IN_PROGRESS);
    }

    // Lifecycle and input: event callbacks enqueue, the script thread executes.
    public TruthStoryPlus()
    {
        dataDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "scripts", "TruthStoryPlus");
        logPath = Path.Combine(dataDir, "TruthStoryPlus.log");
        Interval = 0;
        LoadPrefs();
        Tick += OnTick;
        KeyDown += OnKeyDown;
        KeyUp += OnKeyUp;
        Aborted += OnAbort;
        Log("Starting " + Version + " | API " + typeof(Script).Assembly.GetName().Version);
    }
    private void OnKeyDown(object sender, KeyEventArgs e)
    {
        lock (keyQueue)
        {
            if (heldKeys.Add(e.KeyCode) && keyQueue.Count < 64) keyQueue.Enqueue(e.KeyData);
        }
    }
    private void OnKeyUp(object sender, KeyEventArgs e)
    {
        lock (keyQueue) heldKeys.Remove(e.KeyCode);
    }
    private void Initialize()
    {
        Log("Embedded monochrome logo ready; native rectangle renderer.");
        foreach (string n in Enum.GetNames(typeof(WeaponHash)))
        {
            uint h = (uint)(WeaponHash)Enum.Parse(typeof(WeaponHash), n);
            if (n != "Unarmed" && Function.Call<bool>(Hash.IS_WEAPON_VALID, h)) weapons.Add(new WeaponEntry { Hash = h, Name = SplitName(n) });
        }
        weapons.Sort(delegate(WeaponEntry a, WeaponEntry b)
        {
            return StringComparer.OrdinalIgnoreCase.Compare(a.Name, b.Name);
        });
        initialized = true;
        Build();
        Tell("Loaded v1.0.0. Press F5 to open.");
        Log("Ready. F5 registered. " + weapons.Count + " valid weapon entries.");
    }
    private void OnTick(object sender, EventArgs e)
    {
        try
        {
            if (!Story())
            {
                opened = false;
                if (ready)
                {
                    ResetEffects();
                    ready = false;
                }
                lock (keyQueue)
                {
                    keyQueue.Clear();
                    heldKeys.Clear();
                }
                return;
            }
            Ped p = Hero;
            if (!Valid(p)) return;
            ready = true;
            if (!initialized) Initialize();
            if (confirm.Length > 0 && Now >= confirmUntil) confirm = "";
            UpdatePed(p);
            UpdateCar(Ride);
            ApplyWorld();
            UpdateTraffic();
            UpdateFov();
            ReadKeyboard();
            if (Game.IsPaused) return;
            if (opened)
            {
                Function.Call(Hash.DISABLE_ALL_CONTROL_ACTIONS, 0);
                Function.Call(Hash.DISABLE_ALL_CONTROL_ACTIONS, 2);
                ReadPad();
                if (opened) DrawMenu();
            }
            else if (speedometer && HasCar()) DrawSpeed();
            if (toast.Length > 0 && Now < toastUntil) DrawToast();
        }
        catch (Exception ex)
        {
            Guard(StopFov);
            firstPersonFov = false;
            trafficControl = false;
            Guard(ClearTraffic);
            opened = false; // Restore gameplay input even when an unexpected native fails.
            if (Now >= nextErrorLog)
            {
                LogException("Tick", ex);
                nextErrorLog = Now + 3000;
                errors++;
                Tell("An option failed. Details: scripts/TruthStoryPlus/TruthStoryPlus.log");
            }
            if (errors >= 3)
            {
                ResetEffects();
                errors = 0;
            }
        }
    }
    private void ReadKeyboard()
    {
        List<Keys> input = new List<Keys>();
        lock (keyQueue)
        {
            while (keyQueue.Count > 0) input.Add(keyQueue.Dequeue());
        }
        foreach (Keys data in input)
        {
            Keys k = data & Keys.KeyCode;
            if (k == Keys.F4)
            {
                opened = false;
                continue;
            }
            if (k == Keys.F5)
            {
                opened = !opened;
                confirm = "";
                Click();
                continue;
            }
            if (!opened || busy) continue;
            if (k == Keys.Escape || k == Keys.Back) Back();
            else if (k == Keys.Tab) SwitchPage((data & Keys.Shift) != 0 ? -1 : 1);
            else if (k == Keys.Up) Move(-1);
            else if (k == Keys.Down) Move(1);
            else if (k == Keys.Left) Change(-1);
            else if (k == Keys.Right) Change(1);
            else if (k == Keys.Enter || k == Keys.NumPad5) Activate();
            else if (k == Keys.F6) Search();
            else if (k == Keys.PageDown) Move(visibleRows);
            else if (k == Keys.PageUp) Move(-visibleRows);
        }
        if (!opened)
        {
            lastKey = -1;
            return;
        }
        int held = -1;
        lock (keyQueue)
        {
            if (heldKeys.Contains(Keys.Up)) held = 0;
            else if (heldKeys.Contains(Keys.Down)) held = 1;
            else if (heldKeys.Contains(Keys.Left)) held = 2;
            else if (heldKeys.Contains(Keys.Right)) held = 3;
        }
        if (held != lastKey)
        {
            lastKey = held;
            nextRepeat = Now + 350;
        }
        else if (held >= 0 && Now >= nextRepeat)
        {
            if (held < 2) Move(held == 0 ? -1 : 1);
            else Change(held == 2 ? -1 : 1);
            nextRepeat = Now + 110;
        }
    }
    private static bool PadPress(int n)
    {
        return Function.Call<bool>(Hash.IS_DISABLED_CONTROL_JUST_PRESSED, 2, n);
    }
    private static bool PadHold(int n)
    {
        return Function.Call<bool>(Hash.IS_DISABLED_CONTROL_PRESSED, 2, n);
    }
    private long padRepeat;
    private void ReadPad()
    {
        if (busy || Function.Call<bool>(Hash.IS_USING_KEYBOARD_AND_MOUSE, 2)) return;
        if (PadPress(202))
        {
            Back();
            return;
        }
        if (PadPress(205))
        {
            SwitchPage(-1);
            return;
        }
        if (PadPress(206))
        {
            SwitchPage(1);
            return;
        }
        if (PadPress(201))
        {
            Activate();
            return;
        }
        if (PadPress(203))
        {
            Search();
            return;
        }
        int control = PadHold(172) ? 172 : PadHold(173) ? 173 : PadHold(174) ? 174 : PadHold(175) ? 175 : -1;
        if (control == -1)
        {
            lastPad = -1;
            return;
        }
        if (control != lastPad || Now >= padRepeat)
        {
            if (control == 172) Move(-1);
            else if (control == 173) Move(1);
            else Change(control == 174 ? -1 : 1);
            padRepeat = Now + (control != lastPad ? 350 : 110);
            lastPad = control;
        }
    }
    private void SwitchPage(int step)
    {
        if (view == "main") remembered[page] = cursor;
        page = TruthMenuLogic.Wrap(page + step, pages.Length);
        view = "main";
        query = "";
        cursor = remembered[page];
        scroll = 0;
        confirm = "";
        Build();
        Click();
    }
    private void Go(int target)
    {
        SwitchPage(target - page);
    }
    private void Browse(string target)
    {
        view = target;
        cursor = 0;
        scroll = 0;
        confirm = "";
        Build();
    }
    private void Back()
    {
        if (confirm.Length > 0)
        {
            confirm = "";
            return;
        }
        if (view != "main")
        {
            view = "main";
            cursor = remembered[page];
            scroll = 0;
            Build();
        }
        else opened = false;
        Click();
    }
    private void Move(int step)
    {
        cursor = TruthMenuLogic.Wrap(cursor + step, options.Count);
        confirm = "";
        FixScroll();
        Click();
    }
    private void FixScroll()
    {
        cursor = Math.Max(0, Math.Min(cursor, options.Count - 1));
        scroll = TruthMenuLogic.Scroll(cursor, scroll, visibleRows);
        scroll = Math.Max(0, Math.Min(scroll, Math.Max(0, options.Count - visibleRows)));
    }
    private void Change(int d)
    {
        if (options.Count == 0) return;
        Option o = options[cursor];
        if (o.Enabled != null && !o.Enabled())
        {
            Tell("This option needs a current vehicle or valid selection.");
            return;
        }
        if (o.Change != null)
        {
            Guard(delegate
            {
                o.Change(d);
            });
            confirm = "";
            Click();
        }
    }
    private void Activate()
    {
        if (options.Count == 0 || busy) return;
        Option o = options[cursor];
        if (o.Enabled != null && !o.Enabled())
        {
            Tell("This option needs a current vehicle or valid selection.");
            return;
        }
        Guard(delegate
        {
            if (o.Run != null) o.Run();
            else if (o.Change != null) o.Change(1);
        });
        Click();
    }
    private void Guard(Action a)
    {
        try
        {
            a();
        }
        catch (Exception ex)
        {
            LogException("Action", ex);
            Tell("Action could not complete. See TruthStoryPlus.log.");
        }
    }
    private void Confirm(string id, Action a)
    {
        if (confirm == id && Now <= confirmUntil)
        {
            confirm = "";
            a();
        }
        else
        {
            confirm = id;
            confirmUntil = Now + 5000;
            Tell("Press APPLY again within 5 seconds to confirm: " + id);
        }
    }
    private void Search()
    {
        if (page == 3 || page == 7)
        {
            string s = Input("Search", query, 40);
            if (s == null) return;
            query = s.Trim();
            Browse(page == 3 ? "vehicles" : "weapons");
        }
        else if (page == 2) EnterMoney();
        else Tell("Search: Spawner / Weapons. Exact number entry: Cash.");
    }
    private string Input(string title, string initial, int max)
    {
        busy = true;
        try
        {
            return Game.GetUserInput(WindowTitle.EnterMessage60, initial, max);
        }
        finally
        {
            busy = false;
            lock (keyQueue)
            {
                keyQueue.Clear();
                heldKeys.Clear();
            }
            lastKey = lastPad = -1;
        }
    }
    private void Click()
    {
        if (sound) Function.Call(Hash.PLAY_SOUND_FRONTEND, -1, "NAV_UP_DOWN", "HUD_FRONTEND_DEFAULT_SOUNDSET", true);
    }
    private void Tell(string message)
    {
        toast = message;
        toastUntil = Now + 5000;
    }
    private void LogException(string operation, Exception error)
    {
        // Exception messages/stack traces can expose account names and local paths.
        // Record a useful category without serializing user input or filesystem data.
        Log(operation + ": " + error.GetType().Name + " (0x" + error.HResult.ToString("X8", CultureInfo.InvariantCulture) + ")");
    }
    private void Log(string message)
    {
        try
        {
            Directory.CreateDirectory(dataDir);
            using (FileStream f = new FileStream(logPath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
                using (StreamWriter w = new StreamWriter(f)) w.WriteLine(DateTime.Now.ToString("s") + " " + message);
        }
        catch
        {
            System.Diagnostics.Debug.WriteLine(message);
        }
    }
    private static string SplitName(string name)
    {
        System.Text.StringBuilder b = new System.Text.StringBuilder();
        for (int i = 0; i < name.Length; i++)
        {
            if (i > 0 && char.IsUpper(name[i]) && char.IsLower(name[i - 1])) b.Append(' ');
            b.Append(name[i]);
        }
        return b.ToString();
    }
    private void Add(string name, string help, Action run, Func<string> value, Action<int> change, string kind, Func<bool> enabled)
    {
        options.Add(new Option { Name = name, Help = help, Run = run, Value = value, Change = change, Kind = kind, Enabled = enabled });
    }
    private void Act(string n, string h, Action a)
    {
        Add(n, h, a, null, null, "APPLY", null);
    }
    private void CarAct(string n, string h, Action a)
    {
        Add(n, h, a, null, null, "APPLY", HasCar);
    }
    private void Link(string n, string h, Action a, Func<string> v)
    {
        Add(n, h, a, v, null, "OPEN", null);
    }
    private void Toggle(string n, string h, Func<bool> get, Action<bool> set)
    {
        Add(n, h, delegate
        {
            set(!get());
        }, delegate
        {
            return get() ? "ON" : "OFF";
        }, delegate(int d)
        {
            set(!get());
        }, "TOGGLE", null);
    }
    private void Slider(string n, string h, Func<string> get, Action<int> set, Func<bool> enabled)
    {
        Add(n, h, null, get, set, "VALUE", enabled);
    }
    // Menu definitions: row factories share one navigation and drawing path.
    private void Build()
    {
        options.Clear();
        if (view == "vehicles")
        {
            VehicleRows();
            FixScroll();
            return;
        }
        if (view == "weapons")
        {
            WeaponRows();
            FixScroll();
            return;
        }
        if (view == "places")
        {
            for (int n = 0; n < places.Length; n++)
            {
                int j = n;
                Act(placeNames[j], "Travel with your occupied vehicle. Backtrack is available on the Teleport page.", delegate
                {
                    Teleport(places[j], 0);
                });
            }
            FixScroll();
            return;
        }
        switch (page)
        {
        case 0:
            Link("Player controls", "Health, protection, movement and wanted level.", delegate
            {
                Go(1);
            }, delegate
            {
                return god ? "PROTECTED" : "NORMAL";
            });
            Link("Garage & vehicle catalog", "Browse every installed vehicle model detected by the Enhanced API.", delegate
            {
                Go(3);
            }, delegate
            {
                return "BROWSE";
            });
            Link("Performance workshop", "Power boost, torque and speed cap with explicit reset controls.", delegate
            {
                Go(5);
            }, delegate
            {
                return "TUNE";
            });
            Link("Playground", "Funny vehicle effects, physics toys, props and character antics.", delegate
            {
                Go(11);
            }, delegate
            {
                return "HAVE FUN";
            });
            Act("Heal + armor + repair", "Restore your character and repair the vehicle you occupy.", delegate
            {
                Heal();
                if (HasCar()) Repair();
            });
            Act("Clear wanted level", "Remove the active wanted level.", delegate
            {
                SetWanted(0);
                Tell("Wanted level cleared.");
            });
            Act("Reset active effects", "Turn off this menu's continuous effects and restore the states it tracked.", delegate
            {
                ResetEffects();
                Tell("Active effects reset.");
            });
            Act("Clean up spawned toys", "Delete only props and NPCs created by this menu. Spawned vehicles have separate cleanup.", CleanToys);
            Link("Interface preferences", "Transparency, scale, placement, binary rain, sounds and the driving HUD.", delegate
            {
                Go(13);
            }, delegate
            {
                return "CUSTOMIZE";
            });
            break;
        case 1:
            Toggle("Invincibility", "Protect the active character; the original state is restored when disabled.", delegate
            {
                return god;
            }, delegate(bool b)
            {
                god = b;
            });
            Act("Heal & full armor", "Restore maximum health, fill armor and remove visible injuries.", Heal);
            Toggle("Never wanted", "Continuously keep your wanted level at zero.", delegate
            {
                return neverWanted;
            }, delegate(bool b)
            {
                neverWanted = b;
            });
            Slider("Wanted stars", "Choose 0-5 stars, then use Apply wanted level.", delegate
            {
                return wanted + " / 5";
            }, delegate(int d)
            {
                wanted = TruthMenuLogic.Wrap(wanted + d, 6);
            }, null);
            Act("Apply wanted level", "Disables Never wanted when applying a nonzero level.", delegate
            {
                if (wanted > 0)neverWanted = false;
                SetWanted(wanted);
            });
            Toggle("Infinite stamina", "Refill stamina each frame while enabled.", delegate
            {
                return stamina;
            }, delegate(bool b)
            {
                stamina = b;
            });
            Toggle("Super jump", "Higher jumps while on foot.", delegate
            {
                return superJump;
            }, delegate(bool b)
            {
                superJump = b;
            });
            Slider("Run multiplier", "Uses the game's supported 1.00-1.49 sprint multiplier.", delegate
            {
                return movement[runIndex].ToString("0.00") + "x";
            }, delegate(int d)
            {
                runIndex = TruthMenuLogic.Wrap(runIndex + d, movement.Length);
            }, null);
            Slider("Swim multiplier", "Uses the game's supported 1.00-1.49 swim multiplier.", delegate
            {
                return movement[swimIndex].ToString("0.00") + "x";
            }, delegate(int d)
            {
                swimIndex = TruthMenuLogic.Wrap(swimIndex + d, movement.Length);
            }, null);
            Toggle("Invisible", "Hide your character. Collision and damage rules remain active.", delegate
            {
                return invisible;
            }, delegate(bool b)
            {
                invisible = b;
            });
            Toggle("Prevent ragdoll", "Disable involuntary ragdoll. Playground ragdoll requires this to be off.", delegate
            {
                return noRagdoll;
            }, delegate(bool b)
            {
                noRagdoll = b;
            });
            Toggle("Police ignore player", "Ask police to ignore your character.", delegate
            {
                return policeIgnore;
            }, delegate(bool b)
            {
                policeIgnore = b;
                Function.Call(Hash.SET_POLICE_IGNORE_PLAYER, Game.Player.Handle, b);
            });
            Toggle("Everyone ignores player", "Ask NPCs to ignore your character.", delegate
            {
                return everyoneIgnore;
            }, delegate(bool b)
            {
                everyoneIgnore = b;
                Function.Call(Hash.SET_EVERYONE_IGNORE_PLAYER, Game.Player.Handle, b);
            });
            Act("Refill special ability", "Fill the current protagonist's ability meter when available.", delegate
            {
                Function.Call(Hash.SPECIAL_ABILITY_FILL_METER, Game.Player.Handle, true, 0);
            });
            Act("Clean character", "Remove blood, wetness and visible damage.", CleanPed);
            Act("Dry clothes", "Clear the wetness effect on your character.", delegate
            {
                Function.Call(Hash.CLEAR_PED_WETNESS, Hero.Handle);
            });
            Act("Soak clothes", "Apply a visible wet clothing effect.", delegate
            {
                Function.Call(Hash.SET_PED_WETNESS_HEIGHT, Hero.Handle, 2f);
            });
            Act("Give parachute", "Equip a parachute for your next jump.", GiveParachute);
            break;
        case 2:
            Slider("Amount", "Adjust in steps of $10,000. Exact entry is available below or with F6 / X.", delegate
            {
                return Cash(moneyAmount);
            }, delegate(int d)
            {
                moneyAmount = TruthMenuLogic.ClampMoney((long)moneyAmount + d * 10000L);
            }, null);
            Act("Enter exact amount", "Enter a whole number from 0 to 2,000,000,000.", EnterMoney);
            Act("Add amount", "Add the selected amount to the active Story protagonist.", delegate
            {
                EditMoney(1);
            });
            Act("Subtract amount", "Subtract the selected amount; the result cannot go below zero.", delegate
            {
                EditMoney(-1);
            });
            Act("Set exact balance", "Replace your character's balance with the selected amount. Apply twice to confirm.", delegate
            {
                Confirm("Set balance", delegate
                {
                    EditMoney(0);
                });
            });
            Act("Clear balance", "Set the active character's cash to zero. Apply twice to confirm.", delegate
            {
                Confirm("Clear balance", delegate
                {
                    moneyAmount = 0;
                    EditMoney(0);
                });
            });
            int[] amounts = {1000, 10000, 100000, 1000000, 10000000, 100000000};
            foreach (int n in amounts)
            {
                int a = n;
                Act("Preset: " + Cash(a), "Select this amount without changing your balance yet.", delegate
                {
                    moneyAmount = a;
                    Tell("Amount selected: " + Cash(a));
                });
            }
            break;
        case 3:
            Link("Browse all vehicles", "Catalog is built from loaded game model metadata. Models stream only when selected.", delegate
            {
                EnsureVehicles();
                Browse("vehicles");
            }, delegate
            {
                return vehicles.Count == 0 ? "LOAD CATALOG" : vehicles.Count + " MODELS";
            });
            Act("Search models", "Search display names, enum model names or hashes. F6 / X also opens search.", Search);
            Slider("Vehicle class", "Filter the catalog by vehicle class.", delegate
            {
                return classNames[vehicleClass];
            }, delegate(int d)
            {
                vehicleClass = TruthMenuLogic.Wrap(vehicleClass + d, classNames.Length);
            }, null);
            Toggle("Favorites only", "Limit the catalog to your saved favorite models.", delegate
            {
                return favoritesOnly;
            }, delegate(bool b)
            {
                favoritesOnly = b;
            });
            Toggle("Enter spawned vehicle", "Place your character in the driver seat after spawning.", delegate
            {
                return warpSpawn;
            }, delegate(bool b)
            {
                warpSpawn = b;
            });
            Act("Spawn exact model name", "Enter a model such as adder, sultanrs or an installed add-on model name.", delegate
            {
                string s = Input("Vehicle model", "adder", 40);
                if (!string.IsNullOrWhiteSpace(s))Spawn(new Model(s.Trim()), s.Trim());
            });
            Act("Spawn random road vehicle", "Choose an available car or motorcycle from the detected catalog.", RandomVehicle);
            Act("Random customized vehicle",
                "Spawn a random road vehicle with randomized body parts, wheels, paint, lights, extras and maximum supported performance upgrades.",
                RandomCustomizedVehicle);
            Act("Repeat last spawn", "Spawn the last model you selected.", delegate
            {
                if (selectedVehicleHash != 0)Spawn(new Model(selectedVehicleHash), selectedVehicleName);
                else Tell("Choose a vehicle first.");
            });
            Act("Favorite last selected model", "Save or remove the last selected vehicle in your favorites.", ToggleFavorite);
            Act("Clear catalog filters", "Show all classes and reset the search and favorites filter.", delegate
            {
                query = "";
                vehicleClass = 0;
                favoritesOnly = false;
            });
            Act("Remove last spawned vehicle", "Only removes a vehicle created by this menu. Exit it first.", DeleteLastCar);
            Act("Clean up spawned vehicles", "Delete unoccupied vehicles created by this menu; occupied vehicles are kept.", delegate
            {
                Confirm("Clean spawned vehicles", CleanCars);
            });
            break;
        case 4:
            CarAct("Repair vehicle", "Restore body, engine and fuel tank health.", Repair);
            CarAct("Wash vehicle", "Remove dirt and decals.", delegate
            {
                Ride.DirtLevel = 0;
                Function.Call(Hash.WASH_DECALS_FROM_VEHICLE, Ride.Handle, 1f);
            });
            Toggle("Vehicle invincibility", "Protect the vehicle you occupy, restoring its prior state when you leave.", delegate
            {
                return carGod;
            }, delegate(bool b)
            {
                carGod = b;
            });
            Toggle("Automatic repair", "Restores health while moving; full body repair when nearly stopped. Preserves driving momentum.", delegate
            {
                return autoRepair;
            }, delegate(bool b)
            {
                autoRepair = b;
            });
            Toggle("Seatbelt", "Disable windscreen ejection while in a vehicle.", delegate
            {
                return seatbelt;
            }, delegate(bool b)
            {
                seatbelt = b;
            });
            CarAct("Engine on", "Start the engine.", delegate
            {
                Function.Call(Hash.SET_VEHICLE_ENGINE_ON, Ride.Handle, true, true, false);
            });
            CarAct("Engine off", "Stop the engine without changing vehicle ownership.", delegate
            {
                Function.Call(Hash.SET_VEHICLE_ENGINE_ON, Ride.Handle, false, true, true);
            });
            CarAct("Upright vehicle", "Remove roll and pitch, then settle the vehicle onto the ground.", delegate
            {
                Vehicle v = Ride;
                v.Rotation = new Vector3(0, 0, v.Heading);
                Function.Call(Hash.SET_VEHICLE_ON_GROUND_PROPERLY, v.Handle, 5f);
            });
            CarAct("Open all doors", "Open supported doors, including hood and trunk.", delegate
            {
                for (int i = 0; i < 6; i++)Function.Call(Hash.SET_VEHICLE_DOOR_OPEN, Ride.Handle, i, false, false);
            });
            CarAct("Close all doors", "Close supported doors.", delegate
            {
                Function.Call(Hash.SET_VEHICLE_DOORS_SHUT, Ride.Handle, false);
            });
            CarAct("Roll windows down", "Lower all supported windows.", delegate
            {
                Function.Call(Hash.ROLL_DOWN_WINDOWS, Ride.Handle);
            });
            CarAct("Roll windows up", "Raise all supported windows.", delegate
            {
                for (int i = 0; i < 4; i++)Function.Call(Hash.ROLL_UP_WINDOW, Ride.Handle, i);
            });
            CarAct("Fix all tires", "Repair the standard tire indices on your current vehicle.", delegate
            {
                foreach (int i in new int[]
            {
                0, 1, 2, 3, 4, 5, 45, 47
            })Function.Call(Hash.SET_VEHICLE_TYRE_FIXED, Ride.Handle, i);
            });
            CarAct("Bulletproof tires", "Prevent standard tire punctures on this vehicle.", delegate
            {
                Function.Call(Hash.SET_VEHICLE_TYRES_CAN_BURST, Ride.Handle, false);
            });
            CarAct("Normal tires", "Allow standard tire punctures on this vehicle.", delegate
            {
                Function.Call(Hash.SET_VEHICLE_TYRES_CAN_BURST, Ride.Handle, true);
            });
            CarAct("Lock doors", "Lock vehicle doors.", delegate
            {
                Function.Call(Hash.SET_VEHICLE_DOORS_LOCKED, Ride.Handle, 2);
            });
            CarAct("Unlock doors", "Unlock vehicle doors.", delegate
            {
                Function.Call(Hash.SET_VEHICLE_DOORS_LOCKED, Ride.Handle, 1);
            });
            CarAct("Siren on", "Enable a siren if your vehicle has one.", delegate
            {
                Function.Call(Hash.SET_VEHICLE_SIREN, Ride.Handle, true);
            });
            CarAct("Siren off", "Disable the siren.", delegate
            {
                Function.Call(Hash.SET_VEHICLE_SIREN, Ride.Handle, false);
            });
            break;
        case 5:
            Slider("Engine power boost", "Additional power in percent. Zero returns to stock power.", delegate
            {
                return "+" + powerPercent + "%";
            }, delegate(int d)
            {
                powerPercent = Clamp(powerPercent + d * 25, 0, 1000);
            }, null);
            Slider("Torque multiplier", "100% is stock; applied each frame while driving.", delegate
            {
                return (torquePercent / 100f).ToString("0.00") + "x";
            }, delegate(int d)
            {
                torquePercent = Clamp(torquePercent + d * 25, 100, 500);
            }, null);
            Slider("MPH speed cap", "Set 0 for OFF. Smooth horizontal limiter preserves airborne motion; braking into a lower cap is gradual.",
                   delegate
            {
                return mphCap == 0 ? "OFF" : mphCap + " MPH";
            }, delegate(int d)
            {
                mphCap = Clamp(mphCap + d * 5, 0, 400);
            }, null);
            Act("Enter exact MPH cap", "Enter 0-400. Zero turns the cap off.", delegate
            {
                string s = Input("MPH cap", mphCap.ToString(), 3);
                int n;
                if (s != null && int.TryParse(s, out n))mphCap = Clamp(n, 0, 400);
            });
            Slider("Launch speed", "Select the one-time forward speed used by Apply launch speed.", delegate
            {
                return launchMph + " MPH";
            }, delegate(int d)
            {
                launchMph = Clamp(launchMph + d * 5, 0, 300);
            }, null);
            CarAct("Apply launch speed", "Set forward vehicle speed to the selected MPH value.", delegate
            {
                Function.Call(Hash.SET_VEHICLE_FORWARD_SPEED, Ride.Handle, TruthMenuLogic.MphToMps(launchMph));
            });
            Toggle("Drift / reduced grip", "Use the game's reduced-grip mode for your current vehicle.", delegate
            {
                return drift;
            }, delegate(bool b)
            {
                drift = b;
            });
            CarAct("Max performance upgrades", "Apply the highest supported engine, brake, transmission, suspension and armor mods, plus turbo.",
                   MaxPerformance);
            CarAct("Stock performance upgrades", "Remove performance mods and turn turbo off.", delegate
            {
                SetModKit();
                foreach (int i in new int[] {11, 12, 13, 15, 16})Function.Call(Hash.SET_VEHICLE_MOD, Ride.Handle, i, -1, false);
                Function.Call(Hash.TOGGLE_VEHICLE_MOD, Ride.Handle, 18, false);
            });
            CarAct("Toggle turbo", "Toggle installed turbo on vehicles supporting it.", delegate
            {
                SetModKit();
                Function.Call(Hash.TOGGLE_VEHICLE_MOD, Ride.Handle, 18, !Function.Call<bool>(Hash.IS_TOGGLE_MOD_ON, Ride.Handle, 18));
            });
            Act("Reset performance effects", "Reset power, torque, MPH cap and reduced grip.", delegate
            {
                powerPercent = 0;
                torquePercent = 100;
                mphCap = 0;
                drift = false;
            });
            Toggle("Driving speedometer", "Show a compact branded MPH display when the menu is closed.", delegate
            {
                return speedometer;
            }, delegate(bool b)
            {
                speedometer = b;
            });
            break;
        case 6:
            ShopRows();
            break;
        case 7:
            WeaponsPage();
            break;
        case 8:
            WardrobeRows();
            break;
        case 9:
            Link("Landmarks", "Twelve outdoor locations across Los Santos and Blaine County.", delegate
            {
                Browse("places");
            }, delegate
            {
                return places.Length + " PLACES";
            });
            Act("Waypoint teleport", "Probe ground at your map waypoint. Cancel safely if a ground surface cannot be found.", Waypoint);
            Act("Backtrack", "Return to the position before your last teleport.", delegate
            {
                if (previousPosition.HasValue)Teleport(previousPosition.Value, previousHeading);
                else Tell("No previous teleport position yet.");
            });
            Slider("Saved position slot", "Choose one of five slots stored on disk.", delegate
            {
                return (teleportSlot + 1) + " / 5";
            }, delegate(int d)
            {
                teleportSlot = TruthMenuLogic.Wrap(teleportSlot + d, 5);
            }, null);
            Act("Save current position", "Store your position and heading in the selected slot.", SavePosition);
            Act("Load saved position", "Teleport to the selected slot, including your occupied vehicle.", LoadPosition);
            Act("Step forward 5 meters", "Move a short distance ahead. Check that the space is clear first.", delegate
            {
                Entity e = Controlled();
                Teleport(e.Position + e.ForwardVector * 5f, e.Heading);
            });
            Act("Move up 3 meters", "Raise your character or current vehicle three meters.", delegate
            {
                Entity e = Controlled();
                Teleport(e.Position + new Vector3(0, 0, 3), e.Heading);
            });
            Act("Skydiving jump", "On foot: give a parachute and teleport 650 meters above your current position.", Skydive);
            break;
        case 10:
            WorldRows();
            break;
        case 11:
            FunRows();
            break;
        case 12:
            Act("Stop current scenario", "Clear the player's current scenario task.", delegate
            {
                Function.Call(Hash.CLEAR_PED_TASKS, Hero.Handle);
            });
            for (int n = 0; n < scenarioNames.Length; n++)
            {
                int j = n;
                Act(scenarioNames[j], "On foot only. Game chooses an animation variation supported by your current character.", delegate
                {
                    Scenario(scenarioIds[j]);
                });
            }
            break;
        case 13:
            Slider("Panel opacity", "Adjust the black translucent panels from 90 to 235.", delegate
            {
                return opacity + " / 255";
            }, delegate(int d)
            {
                opacity = Clamp(opacity + d * 5, 90, 235);
            }, null);
            Slider("Menu scale", "Scale the entire menu from 80% to 105%; narrow screens are constrained to fit.", delegate
            {
                return scalePercent + "%";
            }, delegate(int d)
            {
                scalePercent = Clamp(scalePercent + d * 5, 80, 105);
            }, null);
            Toggle("Right side placement", "Anchor the menu to the right side of the screen.", delegate
            {
                return rightSide;
            }, delegate(bool b)
            {
                rightSide = b;
            });
            Toggle("Animated binary backdrop", "Original subtle binary columns behind the interface boxes.", delegate
            {
                return binary;
            }, delegate(bool b)
            {
                binary = b;
            });
            Toggle("Navigation sounds", "Play a quiet menu movement sound.", delegate
            {
                return sound;
            }, delegate(bool b)
            {
                sound = b;
            });
            Toggle("Option descriptions", "Display help for the selected option in the lower panel.", delegate
            {
                return hints;
            }, delegate(bool b)
            {
                hints = b;
            });
            Toggle("Footer brand mark", "Show the small secondary logo in the footer; the title remains in the header.", delegate
            {
                return watermark;
            }, delegate(bool b)
            {
                watermark = b;
            });
            Toggle("Driving speedometer", "Small green-and-black MPH display when the menu is closed.", delegate
            {
                return speedometer;
            }, delegate(bool b)
            {
                speedometer = b;
            });
            Act("Save interface preferences", "Save appearance and navigation preferences for the next launch. Gameplay cheats start off.", delegate
            {
                SavePrefs();
                Tell("Interface preferences saved.");
            });
            Act("Reset interface appearance", "Restore the default green transparent design.", delegate
            {
                opacity = 185;
                scalePercent = 100;
                rightSide = false;
                binary = true;
                sound = true;
                hints = true;
                watermark = true;
            });
            Act("Reset all active effects", "Disable continuous effects from this menu and restore tracked states.", delegate
            {
                ResetEffects();
                Tell("Active effects reset.");
            });
            break;
        }
        FixScroll();
    }
    private void ShopRows()
    {
        Slider("Modification category", "Choose a supported part category, then change Part index.", delegate
        {
            return modNames[modSlot];
        }, delegate(int d)
        {
            modSlot = TruthMenuLogic.Wrap(modSlot + d, modIds.Length);
        }, null);
        Slider("Part index", "Stock is -1. Unavailable categories remain unchanged.", delegate
        {
            return ModValue();
        }, delegate(int d)
        {
            ChangeMod(d);
        }, HasCar);
        CarAct("Max performance package", "Highest supported performance upgrades and turbo.", MaxPerformance);
        Slider("Primary paint index", "GTA paint index from 0-159. Apply using the row below.", delegate
        {
            return primaryPaint.ToString();
        }, delegate(int d)
        {
            primaryPaint = TruthMenuLogic.Wrap(primaryPaint + d, 160);
        }, null);
        Slider("Secondary paint index", "GTA paint index from 0-159. Apply using the row below.", delegate
        {
            return secondaryPaint.ToString();
        }, delegate(int d)
        {
            secondaryPaint = TruthMenuLogic.Wrap(secondaryPaint + d, 160);
        }, null);
        CarAct("Apply indexed paint", "Clear custom RGB paint and apply the selected indexed colors.", delegate
        {
            StopRainbow();
            Function.Call(Hash.CLEAR_VEHICLE_CUSTOM_PRIMARY_COLOUR, Ride.Handle);
            Function.Call(Hash.CLEAR_VEHICLE_CUSTOM_SECONDARY_COLOUR, Ride.Handle);
            Function.Call(Hash.SET_VEHICLE_COLOURS, Ride.Handle, primaryPaint, secondaryPaint);
        });
        CarAct("Truth signature paint", "Black paint with green accents and green underglow.", delegate
        {
            StopRainbow();
            Function.Call(Hash.SET_VEHICLE_CUSTOM_PRIMARY_COLOUR, Ride.Handle, 4, 9, 6);
            Function.Call(Hash.SET_VEHICLE_CUSTOM_SECONDARY_COLOUR, Ride.Handle, 25, 230, 70);
            SetNeon(1);
        });
        CarAct("Custom primary RGB", "Enter three numbers separated by spaces, such as 20 200 60.", delegate
        {
            EnterRgb(true);
        });
        CarAct("Custom secondary RGB", "Enter three numbers separated by spaces, such as 5 12 8.", delegate
        {
            EnterRgb(false);
        });
        Slider("Wheel type", "Cycle wheel categories 0-12. Availability depends on the vehicle.", delegate
        {
            return wheelType.ToString();
        }, delegate(int d)
        {
            wheelType = TruthMenuLogic.Wrap(wheelType + d, 13);
        }, null);
        CarAct("Apply wheel type", "Apply wheel type, then use the Front wheels modification category.", delegate
        {
            SetModKit();
            Function.Call(Hash.SET_VEHICLE_WHEEL_TYPE, Ride.Handle, wheelType);
        });
        Slider("Window tint", "0=none, 1=pure black, 2=dark, 3=light, 4=stock, 5=limo, 6=green.", delegate
        {
            return tint.ToString();
        }, delegate(int d)
        {
            tint = TruthMenuLogic.Wrap(tint + d, 7);
        }, null);
        CarAct("Apply window tint", "Apply the selected tint to supported windows.", delegate
        {
            Function.Call(Hash.SET_VEHICLE_WINDOW_TINT, Ride.Handle, tint);
        });
        Slider("Livery index", "Choose a livery index. Apply checks the vehicle's native livery count.", delegate
        {
            return livery.ToString();
        }, delegate(int d)
        {
            livery = Clamp(livery + d, 0, 100);
        }, null);
        CarAct("Apply native livery", "Some vehicles use modification slot 48 instead; select Liveries above for those.", delegate
        {
            int n = Function.Call<int>(Hash.GET_VEHICLE_LIVERY_COUNT, Ride.Handle);
            if (n > 0)Function.Call(Hash.SET_VEHICLE_LIVERY, Ride.Handle, Math.Min(livery, n - 1));
            else Tell("Use the Liveries modification category for this vehicle, if available.");
        });
        Slider("Extra ID", "Select an extra attachment ID from 0-20.", delegate
        {
            return extraId.ToString();
        }, delegate(int d)
        {
            extraId = TruthMenuLogic.Wrap(extraId + d, 21);
        }, null);
        CarAct("Toggle selected extra", "Toggle only if the extra exists on this vehicle.", delegate
        {
            int h = Ride.Handle;
            if (Function.Call<bool>(Hash.DOES_EXTRA_EXIST, h, extraId))Function.Call(Hash.SET_VEHICLE_EXTRA, h, extraId,
                        Function.Call<bool>(Hash.IS_VEHICLE_EXTRA_TURNED_ON, h, extraId));
            else Tell("That extra is unavailable on this vehicle.");
        });
        Slider("Underglow style", "Off, green, lime, emerald or mint.", delegate
        {
            return new string[]
            {"OFF", "GREEN", "LIME", "EMERALD", "MINT"
            }[neonStyle];
        }, delegate(int d)
        {
            neonStyle = TruthMenuLogic.Wrap(neonStyle + d, 5);
        }, null);
        CarAct("Apply underglow", "Enable or disable neon strips on supported vehicles.", delegate
        {
            SetNeon(neonStyle);
        });
        CarAct("Toggle xenon lights", "Toggle xenon headlights on supported vehicles.", delegate
        {
            SetModKit();
            Function.Call(Hash.TOGGLE_VEHICLE_MOD, Ride.Handle, 22, !Function.Call<bool>(Hash.IS_TOGGLE_MOD_ON, Ride.Handle, 22));
        });
        CarAct("Green tire smoke", "Enable custom tire smoke and apply signature green.", delegate
        {
            SetModKit();
            Function.Call(Hash.TOGGLE_VEHICLE_MOD, Ride.Handle, 20, true);
            Function.Call(Hash.SET_VEHICLE_TYRE_SMOKE_COLOR, Ride.Handle, 30, 255, 70);
        });
        Act("Edit license plate", "Enter up to eight characters, then apply to your vehicle.", delegate
        {
            string s = Input("Plate text", plate, 8);
            if (s != null)plate = s.Trim();
        });
        Add("Apply license plate", "Write the chosen text to this vehicle's plate.", delegate
        {
            Function.Call(Hash.SET_VEHICLE_NUMBER_PLATE_TEXT, Ride.Handle, plate);
        }, delegate
        {
            return plate;
        }, null, "APPLY", HasCar);
    }
    private void WeaponsPage()
    {
        Link("Browse weapons", "List of valid weapon hashes in your installed Enhanced API. Select to equip.", delegate
        {
            Browse("weapons");
        }, delegate
        {
            return weapons.Count + " ENTRIES";
        });
        Act("Search weapons", "Search the weapon catalog by name. F6 / X also opens search.", Search);
        Add("Give selected weapon", "Equip your selection with 500 rounds.", delegate
        {
            GiveWeapon(selectedWeapon, 500);
        }, delegate
        {
            return selectedWeaponName;
        }, null, "APPLY", null);
        Act("Basic loadout", "Pistol, carbine rifle, shotgun and a parachute.", delegate
        {
            GiveWeapon((uint)WeaponHash.Pistol, 250);
            GiveWeapon((uint)WeaponHash.CarbineRifle, 500);
            GiveWeapon((uint)WeaponHash.PumpShotgun, 200);
            GiveParachute();
        });
        Act("Refill equipped ammo", "Add 500 rounds to the currently equipped weapon.", delegate
        {
            Function.Call(Hash.ADD_AMMO_TO_PED, Hero.Handle, CurrentWeaponHash(), 500);
        });
        Act("Give parachute", "Add a parachute to your inventory.", GiveParachute);
        Toggle("Infinite reserve ammo", "Apply infinite ammo to each weapon equipped while enabled; clear those flags when disabled.", delegate
        {
            return infiniteAmmo;
        }, delegate(bool b)
        {
            infiniteAmmo = b;
        });
        Toggle("Infinite clip", "Skip clip depletion while enabled.", delegate
        {
            return infiniteClip;
        }, delegate(bool b)
        {
            infiniteClip = b;
        });
        Toggle("Explosive bullets", "Explosive projectiles for supported weapons; disables fire bullets.", delegate
        {
            return explosiveAmmo;
        }, delegate(bool b)
        {
            explosiveAmmo = b;
            if (b)fireAmmo = false;
        });
        Toggle("Fire bullets", "Incendiary projectiles for supported weapons; disables explosive bullets.", delegate
        {
            return fireAmmo;
        }, delegate(bool b)
        {
            fireAmmo = b;
            if (b)explosiveAmmo = false;
        });
        Toggle("Explosive melee", "Explosive hits from supported melee attacks.", delegate
        {
            return explosiveMelee;
        }, delegate(bool b)
        {
            explosiveMelee = b;
        });
        Slider("Damage multiplier", "100% is normal. Applies to the player's weapon damage.", delegate
        {
            return (damagePercent / 100f).ToString("0.0") + "x";
        }, delegate(int d)
        {
            damagePercent = Clamp(damagePercent + d * 50, 100, 1000);
        }, null);
        Slider("Weapon tint index", "Tint range depends on the equipped weapon. Apply clamps to the supported range.", delegate
        {
            return weaponTint.ToString();
        }, delegate(int d)
        {
            weaponTint = Clamp(weaponTint + d, 0, 31);
        }, null);
        Act("Apply equipped weapon tint", "Color the equipped weapon using a supported tint index.", delegate
        {
            uint h = CurrentWeaponHash();
            int n = Function.Call<int>(Hash.GET_WEAPON_TINT_COUNT, h);
            if (n > 0)Function.Call(Hash.SET_PED_WEAPON_TINT_INDEX, Hero.Handle, h, Math.Min(weaponTint, n - 1));
        });
        Act("Remove equipped weapon", "Remove the currently held weapon. Apply twice to confirm.", delegate
        {
            Confirm("Remove equipped weapon", delegate
            {
                Function.Call(Hash.REMOVE_WEAPON_FROM_PED, Hero.Handle, CurrentWeaponHash());
            });
        });
        Act("Remove all weapons", "Clear the player's weapon inventory. Apply twice to confirm.", delegate
        {
            Confirm("Remove all weapons", delegate
            {
                Function.Call(Hash.REMOVE_ALL_PED_WEAPONS, Hero.Handle, true);
            });
        });
    }
    private void WardrobeRows()
    {
        Slider("Clothing component", "Availability depends on the character model.", delegate
        {
            return componentNames[clothingSlot];
        }, delegate(int d)
        {
            clothingSlot = TruthMenuLogic.Wrap(clothingSlot + d, componentNames.Length);
        }, null);
        Slider("Clothing drawable", "Cycle every drawable supported by the selected component.", delegate
        {
            return Function.Call<int>(Hash.GET_PED_DRAWABLE_VARIATION, Hero.Handle, clothingSlot) + " / " + Math.Max(0,
                    Function.Call<int>(Hash.GET_NUMBER_OF_PED_DRAWABLE_VARIATIONS, Hero.Handle, clothingSlot) - 1);
        }, delegate(int d)
        {
            ChangeClothes(d, false);
        }, null);
        Slider("Clothing texture", "Cycle every texture for the current drawable.", delegate
        {
            return Function.Call<int>(Hash.GET_PED_TEXTURE_VARIATION, Hero.Handle, clothingSlot).ToString();
        }, delegate(int d)
        {
            ChangeClothes(d, true);
        }, null);
        Slider("Accessory slot", "Choose hats, glasses, ears, watches or bracelets.", delegate
        {
            return propNames[propSlot];
        }, delegate(int d)
        {
            propSlot = TruthMenuLogic.Wrap(propSlot + d, propIds.Length);
        }, null);
        Slider("Accessory drawable", "-1 removes the selected accessory.", delegate
        {
            return Function.Call<int>(Hash.GET_PED_PROP_INDEX, Hero.Handle, propIds[propSlot], 0).ToString();
        }, delegate(int d)
        {
            ChangeProp(d, false);
        }, null);
        Slider("Accessory texture", "Cycle the textures supported by the current accessory.", delegate
        {
            return Function.Call<int>(Hash.GET_PED_PROP_TEXTURE_INDEX, Hero.Handle, propIds[propSlot]).ToString();
        }, delegate(int d)
        {
            ChangeProp(d, true);
        }, null);
        Act("Clear selected accessory", "Remove the accessory from the selected slot.", delegate
        {
            Function.Call(Hash.CLEAR_PED_PROP, Hero.Handle, propIds[propSlot], 0);
        });
        Act("Clear all accessories", "Remove hats, glasses and other props.", delegate
        {
            Function.Call(Hash.CLEAR_ALL_PED_PROPS, Hero.Handle, 0);
        });
        Act("Randomize clothing", "Select random component variations supported by this character.", delegate
        {
            Function.Call(Hash.SET_PED_RANDOM_COMPONENT_VARIATION, Hero.Handle, 0);
        });
        Act("Default outfit", "Restore this character model's default component variations.", delegate
        {
            Function.Call(Hash.SET_PED_DEFAULT_COMPONENT_VARIATION, Hero.Handle);
        });
        Slider("Saved outfit slot", "Five disk-backed outfit slots. A saved outfit loads only on the same character model.", delegate
        {
            return (outfitSlot + 1) + " / 5";
        }, delegate(int d)
        {
            outfitSlot = TruthMenuLogic.Wrap(outfitSlot + d, 5);
        }, null);
        Act("Save current outfit", "Save components, textures and accessories to this slot.", SaveOutfit);
        Act("Load saved outfit", "Validate the character model and all saved component indices before applying.", LoadOutfit);
    }
    private void WorldRows()
    {
        Toggle("On-foot first-person FOV",
               "Opt-in camera overlay for first person while on foot. It releases immediately in vehicles, while aiming or during cutscenes.", delegate
        {
            return firstPersonFov;
        }, delegate(bool b)
        {
            firstPersonFov = b;
            if (!b)StopFov();
        });
        Slider("On-foot FOV scale", "Scales the on-foot first-person gameplay FOV, clamped to 45-110 degrees. Vehicle cameras are never modified.",
               delegate
        {
            return fovPercent + "%";
        }, delegate(int d)
        {
            fovPercent = Clamp(fovPercent + d * 5, 80, 160);
        }, null);
        Toggle("Traffic controller", "Enable custom density and extra AI drivers. Extra cars spawn gradually on roads outside your view.", delegate
        {
            return trafficControl;
        }, delegate(bool b)
        {
            trafficControl = b;
            if (!b)ClearTraffic();
        });
        Slider("Traffic density",
               "0-100% adjusts ambient traffic. Above 100% adds up to 60 extra AI cars at 500%, subject to the extra-car limit and FPS guard.", delegate
        {
            return trafficDensity + "%";
        }, delegate(int d)
        {
            trafficDensity = Clamp(trafficDensity + d * 25, 0, 500);
        }, null);
        Slider("Extra traffic type", "Type applies to menu-created traffic. Existing game cars are never replaced.", delegate
        {
            return trafficNames[trafficType];
        }, delegate(int d)
        {
            trafficType = TruthMenuLogic.Wrap(trafficType + d, trafficNames.Length);
            CancelTrafficModel();
        }, null);
        Toggle("Selected traffic only",
               "Suppress new ambient cars and populate with the chosen type instead. Existing traffic fades naturally; mission vehicles remain.", delegate
        {
            return trafficOnly;
        }, delegate(bool b)
        {
            trafficOnly = b;
        });
        Slider("Extra traffic limit", "Maximum active menu-created cars, independent of existing traffic. More AI vehicles can reduce FPS.",
               delegate
        {
            return trafficLimit + " CARS";
        }, delegate(int d)
        {
            trafficLimit = Clamp(trafficLimit + d * 5, 10, 60);
        }, null);
        Toggle("Traffic FPS guard", "Pause new traffic spawns below approximately 30 FPS. This does not guarantee a frame rate.", delegate
        {
            return trafficFpsGuard;
        }, delegate(bool b)
        {
            trafficFpsGuard = b;
        });
        Act("Traffic status", "Shows managed car count and spawn state. Density is a target, not an exact count multiplier.", delegate
        {
            Tell(trafficCars.Count + " managed cars / " + trafficStatus);
        });
        Act("Clear added traffic", "Disable the controller and remove only its cars/drivers. A car occupied by you is kept.", delegate
        {
            trafficControl = false;
            ClearTraffic();
            Tell("Added traffic cleared; normal traffic restored.");
        });
        Slider("Weather preset", "Select a weather type, then apply. Weather remains forced until released.", delegate
        {
            return weatherNames[weatherIndex];
        }, delegate(int d)
        {
            weatherIndex = TruthMenuLogic.Wrap(weatherIndex + d, weatherNames.Length);
        }, null);
        Act("Apply weather", "Apply the selected weather persistently.", delegate
        {
            Function.Call(Hash.SET_WEATHER_TYPE_NOW_PERSIST, weatherNames[weatherIndex]);
            weatherOwned = true;
        });
        Act("Release weather override", "Allow the game's normal weather system to resume.", ReleaseWeather);
        Slider("Hour", "Select 0-23, then Apply time.", delegate
        {
            return hour.ToString("00");
        }, delegate(int d)
        {
            hour = TruthMenuLogic.Wrap(hour + d, 24);
        }, null);
        Slider("Minute", "Select 0-59, then Apply time.", delegate
        {
            return minute.ToString("00");
        }, delegate(int d)
        {
            minute = TruthMenuLogic.Wrap(minute + d, 60);
        }, null);
        Act("Apply time", "Set the clock to your chosen hour and minute.", delegate
        {
            Function.Call(Hash.SET_CLOCK_TIME, hour, minute, 0);
        });
        Act("Golden hour", "Set the clock to 18:30 and apply clear weather.", delegate
        {
            hour = 18;
            minute = 30;
            Function.Call(Hash.SET_CLOCK_TIME, hour, minute, 0);
            Function.Call(Hash.SET_WEATHER_TYPE_NOW_PERSIST, "CLEAR");
            weatherOwned = true;
        });
        Act("Midnight rain", "Set the clock to midnight and apply rain.", delegate
        {
            hour = 0;
            minute = 0;
            Function.Call(Hash.SET_CLOCK_TIME, 0, 0, 0);
            Function.Call(Hash.SET_WEATHER_TYPE_NOW_PERSIST, "RAIN");
            weatherOwned = true;
        });
        Toggle("Freeze clock", "Pause game-clock progression until disabled.", delegate
        {
            return freezeClock;
        }, delegate(bool b)
        {
            freezeClock = b;
            Function.Call(Hash.PAUSE_CLOCK, b);
        });
        Slider("Time speed", "100% is normal; 25-100% gives slow motion.", delegate
        {
            return timePercent + "%";
        }, delegate(int d)
        {
            timePercent = Clamp(timePercent + d * 5, 25, 100);
        }, null);
        Slider("Gravity", "World gravity: normal, low or very low. Applies to the whole session.", delegate
        {
            return new string[]
            {"NORMAL", "LOW", "VERY LOW"
            }[gravityMode];
        }, delegate(int d)
        {
            gravityMode = TruthMenuLogic.Wrap(gravityMode + d, 3);
            Function.Call(Hash.SET_GRAVITY_LEVEL, gravityMode);
            gravityOwned = true;
        }, null);
        Toggle("City blackout", "Switch off artificial city lighting until disabled.", delegate
        {
            return blackout;
        }, delegate(bool b)
        {
            blackout = b;
            Function.Call(Hash.SET_ARTIFICIAL_LIGHTS_STATE, b);
        });
        Toggle("Suppress new traffic", "Reduce ambient vehicle spawning each frame; existing vehicles remain.", delegate
        {
            return noTraffic;
        }, delegate(bool b)
        {
            noTraffic = b;
        });
        Toggle("Suppress new pedestrians", "Reduce ambient pedestrian spawning each frame; existing NPCs remain.", delegate
        {
            return noPeds;
        }, delegate(bool b)
        {
            noPeds = b;
        });
        Toggle("Night vision", "Enable night vision; switches off thermal vision.", delegate
        {
            return nightVision;
        }, delegate(bool b)
        {
            nightVision = b;
            if (b)
            {
                thermal = false;
                Function.Call(Hash.SET_SEETHROUGH, false);
            }
            Function.Call(Hash.SET_NIGHTVISION, b);
        });
        Toggle("Thermal vision", "Enable thermal vision; switches off night vision.", delegate
        {
            return thermal;
        }, delegate(bool b)
        {
            thermal = b;
            if (b)
            {
                nightVision = false;
                Function.Call(Hash.SET_NIGHTVISION, false);
            }
            Function.Call(Hash.SET_SEETHROUGH, b);
        });
        Toggle("Hide game HUD", "Hide the game's HUD each frame for screenshots.", delegate
        {
            return hideHud;
        }, delegate(bool b)
        {
            hideHud = b;
        });
        Toggle("Hide minimap", "Hide the radar while enabled.", delegate
        {
            return hideRadar;
        }, delegate(bool b)
        {
            hideRadar = b;
        });
    }
    private void FunRows()
    {
        Toggle("Rainbow paint", "Cycle custom vehicle colors. Restore the vehicle's original colors when disabled or exited.", delegate
        {
            return rainbow;
        }, delegate(bool b)
        {
            rainbow = b;
        });
        Toggle("Bunny-hop vehicle", "Give your grounded vehicle a hop every 1.5 seconds. Works while the menu is closed.", delegate
        {
            return bunny;
        }, delegate(bool b)
        {
            bunny = b;
        });
        Toggle("Horn boost", "Hold the normal vehicle horn control to accelerate. Works while the menu is closed.", delegate
        {
            return hornBoost;
        }, delegate(bool b)
        {
            hornBoost = b;
        });
        CarAct("Vehicle hop", "Add upward velocity to your vehicle for a single jump.", delegate
        {
            HopCar(7f);
        });
        CarAct("Send vehicle skyward", "Launch your occupied vehicle vertically with an upward speed of 25 m/s.", delegate
        {
            HopCar(25f);
        });
        CarAct("Instant handbrake stop", "Set your current vehicle's velocity to zero.", delegate
        {
            Ride.Velocity = Vector3.Zero;
        });
        CarAct("Turn vehicle around", "Rotate your vehicle 180 degrees and stop its movement.", delegate
        {
            Ride.Heading += 180;
            Ride.Velocity = Vector3.Zero;
        });
        Act("Ragdoll flop", "On foot: make your character fall over. Requires Prevent ragdoll to be off.", delegate
        {
            if (HasCar() || noRagdoll)
            {
                Tell("Exit your vehicle and disable Prevent ragdoll first.");
                return;
            }
            Function.Call(Hash.SET_PED_TO_RAGDOLL, Hero.Handle, 2500, 2500, 0, false, false, false);
        });
        Act("Superhero leap", "On foot: launch up and forward with a parachute.", delegate
        {
            if (HasCar())
            {
                Tell("Exit your vehicle first.");
                return;
            }
            GiveParachute();
            Hero.Velocity = Hero.ForwardVector * 16f + new Vector3(0, 0, 20);
        });
        Act("Skydiving jump", "On foot: parachute from 650 meters above your current location.", Skydive);
        Act("Clone my character", "Create one copy of your character nearby. Tracked for cleanup.", CloneMe);
        Act("Clown bodyguard", "Spawn a clown who joins your player's group.", delegate
        {
            SpawnPed("s_m_y_clown_01", true);
        });
        Act("Mime bodyguard", "Spawn a mime who joins your player's group.", delegate
        {
            SpawnPed("s_m_y_mime", true);
        });
        Act("Chimp cameo", "Spawn a chimp nearby. Tracked for cleanup.", delegate
        {
            SpawnPed("a_c_chimp", false);
        });
        Act("Poodle cameo", "Spawn a poodle nearby. Tracked for cleanup.", delegate
        {
            SpawnPed("a_c_poodle", false);
        });
        Act("Beach ball", "Drop a physical beach ball a short distance ahead.", delegate
        {
            SpawnProp("prop_beachball_02");
        });
        Act("Traffic cone", "Drop a physical cone a short distance ahead.", delegate
        {
            SpawnProp("prop_roadcone02a");
        });
        Act("Barrel", "Drop a physical barrel a short distance ahead.", delegate
        {
            SpawnProp("prop_barrel_02a");
        });
        Act("Street party", "Start the partying scenario on your character.", delegate
        {
            Scenario("WORLD_HUMAN_PARTYING");
        });
        Act("Surprise me", "Randomly select a harmless menu action: heal, outfit shuffle, party, parachute or a prop.", delegate
        {
            switch (random.Next(5))
            {
            case 0:
                Heal();
                break;
            case 1:
                Function.Call(Hash.SET_PED_RANDOM_COMPONENT_VARIATION, Hero.Handle, 0);
                break;
            case 2:
                Scenario("WORLD_HUMAN_PARTYING");
                break;
            case 3:
                GiveParachute();
                break;
            default:
                SpawnProp("prop_beachball_02");
                break;
            }
        });
        Act("Clean up toys & guests", "Delete only the props and NPCs created by this menu.", CleanToys);
        Act("Stop playground effects", "Disable rainbow paint, bunny hops and horn boost.", delegate
        {
            rainbow = false;
            bunny = false;
            hornBoost = false;
        });
    }
    // Runtime catalogs and searchable views.
    private void EnsureVehicles()
    {
        if (vehicles.Count > 0)return;
        HashSet<int> seen = new HashSet<int>();
        VehicleHash[] all;
        try
        {
            all = Vehicle.GetAllModels();
        }
        catch (Exception ex)
        {
            LogException("Catalog fallback", ex);
            all = (VehicleHash[])Enum.GetValues(typeof(VehicleHash));
        }
        int processed = 0;
        foreach (VehicleHash h in all)
        {
            int raw = unchecked((int)h);
            if (!seen.Add(raw))continue;
            Model m = new Model(raw);
            if (!m.IsInCdImage || !m.IsVehicle)continue;
            string enumName = Enum.GetName(typeof(VehicleHash), h);
            string key = Function.Call<string>(Hash.GET_DISPLAY_NAME_FROM_VEHICLE_MODEL, raw);
            string label = string.IsNullOrEmpty(key) ? "" : Game.GetLocalizedString(key);
            if (string.IsNullOrWhiteSpace(label) || label == "NULL")label = enumName ?? ("Model " + unchecked((uint)raw).ToString("X8"));
            int cls = Function.Call<int>(Hash.GET_VEHICLE_CLASS_FROM_NAME, raw);
            vehicles.Add(new VehicleEntry {Hash = raw, Class = cls, Name = label, Model = enumName ?? key ?? ""});
            if (++processed % 50 == 0)Script.Yield();
        }
        vehicles.Sort(delegate(VehicleEntry a, VehicleEntry b)
        {
            int c = StringComparer.OrdinalIgnoreCase.Compare(a.Name, b.Name);
            return c != 0 ? c : a.Hash.CompareTo(b.Hash);
        });
        Log("Vehicle catalog: " + vehicles.Count + " models.");
    }
    private void VehicleRows()
    {
        EnsureVehicles();
        foreach (VehicleEntry e in vehicles)
        {
            if (vehicleClass > 0 && e.Class != vehicleClass - 1)continue;
            if (favoritesOnly && !favorites.Contains(e.Hash))continue;
            if (!TruthMenuLogic.Matches(e.Name + " " + e.Model + " " + unchecked((uint)e.Hash).ToString("X8"), query))continue;
            VehicleEntry pick = e;
            string cls = e.Class >= 0 && e.Class + 1 < classNames.Length ? classNames[e.Class + 1] : "Other";
            Add(e.Name, "Model: " + e.Model + " | " + cls + ". Apply to spawn. Left/right toggles favorite.", delegate
            {
                Spawn(new Model(pick.Hash), pick.Name);
            }, delegate
            {
                return favorites.Contains(pick.Hash) ? "FAVORITE" : "SPAWN";
            }, delegate(int d)
            {
                selectedVehicleHash = pick.Hash;
                selectedVehicleName = pick.Name;
                ToggleFavorite();
            }, "SPAWN", null);
        }
        if (options.Count == 0)Act("No matching vehicles", "Use F6 / X to change the search. Go Back, then Clear catalog filters to reset filters.",
                                       Search);
    }
    private void WeaponRows()
    {
        foreach (WeaponEntry e in weapons)
        {
            if (!TruthMenuLogic.Matches(e.Name, query))continue;
            WeaponEntry pick = e;
            Act(e.Name, "Equip this weapon and add 500 rounds.", delegate
            {
                selectedWeapon = pick.Hash;
                selectedWeaponName = pick.Name;
                GiveWeapon(pick.Hash, 500);
            });
        }
        if (options.Count == 0)Act("No matching weapons", "Use F6 / X to change the search.", Search);
    }
    private static int Clamp(int n, int lo, int hi)
    {
        return Math.Max(lo, Math.Min(hi, n));
    }
    private static string Cash(int n)
    {
        return "$" + n.ToString("N0", CultureInfo.InvariantCulture);
    }
    private uint CurrentWeaponHash()
    {
        return Function.Call<uint>(Hash.GET_SELECTED_PED_WEAPON, Hero.Handle);
    }
    private void SetWanted(int n)
    {
        Function.Call(Hash.SET_PLAYER_WANTED_LEVEL, Game.Player.Handle, n, false);
        Function.Call(Hash.SET_PLAYER_WANTED_LEVEL_NOW, Game.Player.Handle, false);
    }
    private void Heal()
    {
        Hero.Health = Hero.MaxHealth;
        Hero.Armor = 100;
        CleanPed();
        Tell("Health and armor restored.");
    }
    private void CleanPed()
    {
        Function.Call(Hash.CLEAR_PED_BLOOD_DAMAGE, Hero.Handle);
        Function.Call(Hash.RESET_PED_VISIBLE_DAMAGE, Hero.Handle);
        Function.Call(Hash.CLEAR_PED_WETNESS, Hero.Handle);
    }
    private void Repair()
    {
        if (!HasCar())return;
        RepairMoving(Ride);
        Ride.DirtLevel = 0;
        Tell("Vehicle repaired and cleaned.");
    }
    private void RepairMoving(Vehicle v)
    {
        // Whole-vehicle repair can rebuild physics. Only do that while practically stopped.
        if (v.Speed < 1f)
        {
            v.Repair();
            return;
        }
        v.EngineHealth = 1000f;
        v.BodyHealth = 1000f;
        v.PetrolTankHealth = 1000f;
    }
    private void GiveParachute()
    {
        Function.Call(Hash.GIVE_WEAPON_TO_PED, Hero.Handle, 0xFBAB5776u, 1, false, false);
    }
    private void GiveWeapon(uint h, int ammo)
    {
        if (!Function.Call<bool>(Hash.IS_WEAPON_VALID, h))
        {
            Tell("That weapon is unavailable.");
            return;
        }
        Function.Call(Hash.GIVE_WEAPON_TO_PED, Hero.Handle, h, ammo, false, true);
        Tell("Weapon equipped.");
    }
    private void EnterMoney()
    {
        string s = Input("Amount", moneyAmount.ToString(CultureInfo.InvariantCulture), 15);
        int n;
        if (s == null)return;
        if (!TruthMenuLogic.TryMoney(s, out n))
        {
            Tell("Enter a whole number from 0 to 2,000,000,000.");
            return;
        }
        moneyAmount = n;
        Tell("Amount selected: " + Cash(n));
    }
    private void EditMoney(int mode)
    {
        int h = Hero.Model.Hash;
        if (h != unchecked((int)PedHash.Michael) && h != unchecked((int)PedHash.Franklin) && h != unchecked((int)PedHash.Trevor))
        {
            Tell("Cash editing requires Michael, Franklin or Trevor.");
            return;
        }
        long next = mode == 0 ? moneyAmount : (long)Game.Player.Money + mode * (long)moneyAmount;
        Game.Player.Money = TruthMenuLogic.ClampMoney(next);
        Tell("New balance: " + Cash(Game.Player.Money));
    }
    private void Spawn(Model model, string label)
    {
        spawned.RemoveAll(delegate(Vehicle entry)
        {
            return !Valid(entry);
        });
        if (spawned.Count >= 15)
        {
            Tell("15 spawned vehicles are tracked. Clean up some before spawning more.");
            return;
        }
        if (!model.IsInCdImage || !model.IsVehicle)
        {
            Tell("Vehicle model is unavailable: " + label);
            return;
        }
        if (Function.Call<bool>(Hash.IS_THIS_MODEL_A_TRAIN, model.Hash))
        {
            Tell("Train models require a rail mission and cannot use the regular vehicle spawner.");
            return;
        }
        selectedVehicleHash = model.Hash;
        selectedVehicleName = label;
        Vehicle made = null;
        try
        {
            if (!model.Request(1500))
            {
                Tell("Model did not finish streaming. Try again.");
                return;
            }
            // Spawn ahead; leave enough clearance for longer models.
            Vector3 low, high;
            model.GetDimensions(out low, out high);
            float clearance = Math.Max(8f, (high.Y - low.Y) * .6f + 5f);
            made = World.CreateVehicle(model, Hero.Position + Hero.ForwardVector * clearance + new Vector3(0, 0, .5f), Hero.Heading);
            if (!Valid(made))
            {
                Tell("The game could not create this vehicle.");
                return;
            }
            made.IsPersistent = true;
            spawned.Add(made);
            if (!model.IsPlane && !model.IsHelicopter && !model.IsBoat)Function.Call(Hash.SET_VEHICLE_ON_GROUND_PROPERLY, made.Handle, 5f);
            if (warpSpawn &&
                    Function.Call<int>(Hash.GET_VEHICLE_MODEL_NUMBER_OF_SEATS, model.Hash) > 0)Function.Call(Hash.SET_PED_INTO_VEHICLE, Hero.Handle,
                                made.Handle, -1);
            Tell("Spawned " + label + ".");
        }
        finally
        {
            model.MarkAsNoLongerNeeded();
        }
    }
    private void RandomVehicle()
    {
        EnsureVehicles();
        List<VehicleEntry> road = vehicles.FindAll(delegate(VehicleEntry v)
        {
            return v.Class >= 0 && v.Class <= 8;
        });
        if (road.Count == 0)
        {
            Tell("No road vehicles found.");
            return;
        }
        VehicleEntry pick = road[random.Next(road.Count)];
        Spawn(new Model(pick.Hash), pick.Name);
    }
    // Random builds retain the existing supported-part checks.
    private void RandomCustomizedVehicle()
    {
        EnsureVehicles();
        List<VehicleEntry> road = vehicles.FindAll(delegate(VehicleEntry entry)
        {
            return entry.Class >= 0 && entry.Class <= 8;
        });
        if (road.Count == 0)
        {
            Tell("No road vehicles found.");
            return;
        }
        spawned.RemoveAll(delegate(Vehicle entry)
        {
            return !Valid(entry);
        });
        int before = spawned.Count;
        VehicleEntry pick = road[random.Next(road.Count)];
        Spawn(new Model(pick.Hash), pick.Name);
        if (spawned.Count <= before)return;
        Vehicle vehicle = spawned[spawned.Count - 1];
        if (!Valid(vehicle))return;
        RandomizeVehicle(vehicle);
        Tell("Random build complete: " + pick.Name + ".");
    }
    private void RandomizeVehicle(Vehicle vehicle)
    {
        int h = vehicle.Handle;
        Function.Call(Hash.SET_VEHICLE_MOD_KIT, h, 0);
        Function.Call(Hash.SET_VEHICLE_WHEEL_TYPE, h, random.Next(13));
        foreach (int slot in modIds)
        {
            int count = Function.Call<int>(Hash.GET_NUM_VEHICLE_MODS, h, slot);
            if (count <= 0)continue;
            bool performance = slot == 11 || slot == 12 || slot == 13 || slot == 15 || slot == 16;
            int choice = performance ? count - 1 : random.Next(-1, count);
            Function.Call(Hash.SET_VEHICLE_MOD, h, slot, choice, slot == 23 || slot == 24 ? random.Next(2) == 0 : false);
        }
        Function.Call(Hash.TOGGLE_VEHICLE_MOD, h, 18, true);
        bool smoke = random.Next(2) == 0, xenon = random.Next(2) == 0;
        Function.Call(Hash.TOGGLE_VEHICLE_MOD, h, 20, smoke);
        Function.Call(Hash.TOGGLE_VEHICLE_MOD, h, 22, xenon);
        int pr = random.Next(20, 256), pg = random.Next(20, 256), pb = random.Next(20, 256);
        int sr = random.Next(20, 256), sg = random.Next(20, 256), sb = random.Next(20, 256);
        Function.Call(Hash.SET_VEHICLE_CUSTOM_PRIMARY_COLOUR, h, pr, pg, pb);
        Function.Call(Hash.SET_VEHICLE_CUSTOM_SECONDARY_COLOUR, h, sr, sg, sb);
        Function.Call(Hash.SET_VEHICLE_WINDOW_TINT, h, random.Next(7));
        Function.Call(Hash.SET_VEHICLE_NUMBER_PLATE_TEXT_INDEX, h, random.Next(6));
        string[] plates = {"TRUTH", "RANDOM", "LUCKY", "WILD", "PLUS"};
        Function.Call(Hash.SET_VEHICLE_NUMBER_PLATE_TEXT, h, plates[random.Next(plates.Length)]);
        for (int extra = 1; extra <= 14; extra++)
            if (Function.Call<bool>(Hash.DOES_EXTRA_EXIST, h, extra))Function.Call(Hash.SET_VEHICLE_EXTRA, h, extra, random.Next(2) == 0);
        int liveries = Function.Call<int>(Hash.GET_VEHICLE_LIVERY_COUNT, h);
        if (liveries > 0)Function.Call(Hash.SET_VEHICLE_LIVERY, h, random.Next(liveries));
        bool neon = random.Next(3) != 0;
        for (int side = 0; side < 4; side++)Function.Call(Hash.SET_VEHICLE_NEON_ENABLED, h, side, neon && random.Next(4) != 0);
        Function.Call(Hash.SET_VEHICLE_NEON_COLOUR, h, random.Next(256), random.Next(256), random.Next(256));
        Function.Call(Hash.SET_VEHICLE_TYRE_SMOKE_COLOR, h, random.Next(256), random.Next(256), random.Next(256));
        Function.Call(Hash.SET_VEHICLE_TYRES_CAN_BURST, h, false);
        vehicle.DirtLevel = 0;
    }
    private void ToggleFavorite()
    {
        if (selectedVehicleHash == 0)
        {
            Tell("Choose a vehicle first.");
            return;
        }
        if (!favorites.Add(selectedVehicleHash))
        {
            favorites.Remove(selectedVehicleHash);
            Tell("Favorite removed.");
        }
        else Tell("Favorite saved.");
        SaveData();
    }
    private void DeleteLastCar()
    {
        spawned.RemoveAll(delegate(Vehicle entry)
        {
            return !Valid(entry);
        });
        if (spawned.Count == 0)
        {
            Tell("No tracked vehicle to remove.");
            return;
        }
        Vehicle v = spawned[spawned.Count - 1];
        if (Function.Call<bool>(Hash.IS_VEHICLE_SEAT_FREE, v.Handle, -1, false) && v.PassengerCount == 0)
        {
            v.Delete();
            spawned.RemoveAt(spawned.Count - 1);
            Tell("Vehicle removed.");
        }
        else Tell("Exit the vehicle and clear its passengers first.");
    }
    private void CleanCars()
    {
        int removed = 0;
        for (int i = spawned.Count - 1; i >= 0; i--)
        {
            Vehicle v = spawned[i];
            if (!Valid(v))
            {
                spawned.RemoveAt(i);
                continue;
            }
            if (!Function.Call<bool>(Hash.IS_VEHICLE_SEAT_FREE, v.Handle, -1, false) || v.PassengerCount > 0)continue;
            v.Delete();
            spawned.RemoveAt(i);
            removed++;
        }
        Tell(removed + " spawned vehicles removed; occupied vehicles kept.");
    }
    private void SetModKit()
    {
        Function.Call(Hash.SET_VEHICLE_MOD_KIT, Ride.Handle, 0);
    }
    private string ModValue()
    {
        if (!HasCar())return "ENTER VEHICLE";
        int h = Ride.Handle;
        int count = Function.Call<int>(Hash.GET_NUM_VEHICLE_MODS, h, modIds[modSlot]);
        return count <= 0 ? "UNAVAILABLE" : Function.Call<int>(Hash.GET_VEHICLE_MOD, h, modIds[modSlot]) + " / " + (count - 1);
    }
    private void ChangeMod(int d)
    {
        SetModKit();
        int h = Ride.Handle, slot = modIds[modSlot];
        int n = Function.Call<int>(Hash.GET_NUM_VEHICLE_MODS, h, slot);
        if (n <= 0)
        {
            Tell("This vehicle has no parts for that category.");
            return;
        }
        int current = Function.Call<int>(Hash.GET_VEHICLE_MOD, h, slot);
        Function.Call(Hash.SET_VEHICLE_MOD, h, slot, TruthMenuLogic.Wrap(current + 1 + d, n + 1) - 1, false);
    }
    private void MaxPerformance()
    {
        if (!HasCar())
        {
            Tell("Enter a vehicle first.");
            return;
        }
        SetModKit();
        int h = Ride.Handle;
        foreach (int i in new int[] {11, 12, 13, 15, 16})
        {
            int count = Function.Call<int>(Hash.GET_NUM_VEHICLE_MODS, h, i);
            if (count > 0)Function.Call(Hash.SET_VEHICLE_MOD, h, i, count - 1, false);
        }
        Function.Call(Hash.TOGGLE_VEHICLE_MOD, h, 18, true);
        Tell("Supported performance upgrades installed.");
    }
    private void EnterRgb(bool primary)
    {
        string s = Input("RGB", "20 220 65", 15);
        if (s == null)return;
        int[] rgb;
        if (!TruthMenuLogic.TryRgb(s, out rgb))
        {
            Tell("Use three values from 0-255, such as 20 220 65.");
            return;
        }
        StopRainbow();
        Function.Call(primary ? Hash.SET_VEHICLE_CUSTOM_PRIMARY_COLOUR : Hash.SET_VEHICLE_CUSTOM_SECONDARY_COLOUR, Ride.Handle, rgb[0], rgb[1],
                      rgb[2]);
    }
    private void SetNeon(int style)
    {
        if (!HasCar())return;
        int h = Ride.Handle;
        for (int i = 0; i < 4; i++)Function.Call(Hash.SET_VEHICLE_NEON_ENABLED, h, i, style != 0);
        int[,] colors = {{0, 0, 0}, {30, 255, 70}, {150, 255, 20}, {0, 180, 90}, {100, 255, 175}};
        Function.Call(Hash.SET_VEHICLE_NEON_COLOUR, h, colors[style, 0], colors[style, 1], colors[style, 2]);
    }
    private void ChangeClothes(int d, bool texture)
    {
        int h = Hero.Handle, slot = clothingSlot;
        int draw = Function.Call<int>(Hash.GET_PED_DRAWABLE_VARIATION, h, slot);
        int count = texture ? Function.Call<int>(Hash.GET_NUMBER_OF_PED_TEXTURE_VARIATIONS, h, slot,
                    draw) : Function.Call<int>(Hash.GET_NUMBER_OF_PED_DRAWABLE_VARIATIONS, h, slot);
        if (count <= 0)
        {
            Tell("No variations available for this slot.");
            return;
        }
        int tex = 0;
        if (texture)tex = TruthMenuLogic.Wrap(Function.Call<int>(Hash.GET_PED_TEXTURE_VARIATION, h, slot) + d, count);
        else draw = TruthMenuLogic.Wrap(draw + d, count);
        if (Function.Call<bool>(Hash.IS_PED_COMPONENT_VARIATION_VALID, h, slot, draw, tex))Function.Call(Hash.SET_PED_COMPONENT_VARIATION, h, slot,
                    draw, tex, 0);
    }
    private void ChangeProp(int d, bool texture)
    {
        int h = Hero.Handle, slot = propIds[propSlot];
        int draw = Function.Call<int>(Hash.GET_PED_PROP_INDEX, h, slot, 0), tex = 0;
        if (texture)
        {
            if (draw < 0)return;
            int count = Function.Call<int>(Hash.GET_NUMBER_OF_PED_PROP_TEXTURE_VARIATIONS, h, slot, draw);
            if (count <= 0)return;
            tex = TruthMenuLogic.Wrap(Function.Call<int>(Hash.GET_PED_PROP_TEXTURE_INDEX, h, slot) + d, count);
        }
        else
        {
            int count = Function.Call<int>(Hash.GET_NUMBER_OF_PED_PROP_DRAWABLE_VARIATIONS, h, slot);
            draw = TruthMenuLogic.Wrap(draw + 1 + d, count + 1) - 1;
            if (draw < 0)
            {
                Function.Call(Hash.CLEAR_PED_PROP, h, slot, 0);
                return;
            }
        }
        Function.Call(Hash.SET_PED_PROP_INDEX, h, slot, draw, tex, true, 0);
    }
    private void SaveOutfit()
    {
        List<string> lines = new List<string>();
        int h = Hero.Handle;
        lines.Add(Hero.Model.Hash.ToString(CultureInfo.InvariantCulture));
        for (int i = 0; i < 12;
                i++)lines.Add(Function.Call<int>(Hash.GET_PED_DRAWABLE_VARIATION, h, i) + "," + Function.Call<int>(Hash.GET_PED_TEXTURE_VARIATION, h,
                                  i) + "," + Function.Call<int>(Hash.GET_PED_PALETTE_VARIATION, h, i));
        foreach (int i in propIds)lines.Add(Function.Call<int>(Hash.GET_PED_PROP_INDEX, h, i,
                                                0) + "," + Function.Call<int>(Hash.GET_PED_PROP_TEXTURE_INDEX, h, i));
        Directory.CreateDirectory(dataDir);
        File.WriteAllLines(Path.Combine(dataDir, "outfit-" + (outfitSlot + 1) + ".txt"), lines.ToArray());
        Tell("Outfit saved to slot " + (outfitSlot + 1) + ".");
    }
    private void LoadOutfit()
    {
        string file = Path.Combine(dataDir, "outfit-" + (outfitSlot + 1) + ".txt");
        if (!File.Exists(file))
        {
            Tell("This outfit slot is empty.");
            return;
        }
        string[] lines = File.ReadAllLines(file);
        int model;
        if (lines.Length != 18 || !int.TryParse(lines[0], out model) || model != Hero.Model.Hash)
        {
            Tell("This outfit belongs to a different character or has invalid data.");
            return;
        }
        int h = Hero.Handle;
        int[][] values = new int[17][];
        // Validate the full saved outfit before modifying any component.
        for (int i = 0; i < 17; i++)
        {
            string[] parts = lines[i + 1].Split(',');
            int need = i < 12 ? 3 : 2;
            if (parts.Length != need)
            {
                Tell("Invalid outfit file.");
                return;
            }
            values[i] = new int[need];
            for (int j = 0; j < need; j++)if (!int.TryParse(parts[j], out values[i][j]))
                {
                    Tell("Invalid outfit file.");
                    return;
                }
            int d = values[i][0], t = values[i][1];
            if (i < 12)
            {
                if (d < 0 || t < 0 || values[i][2] < 0 || values[i][2] > 3 || !Function.Call<bool>(Hash.IS_PED_COMPONENT_VARIATION_VALID, h, i, d, t))
                {
                    Tell("A saved clothing component is unavailable.");
                    return;
                }
            }
            else if (d >= 0)
            {
                int id = propIds[i - 12];
                if (d >= Function.Call<int>(Hash.GET_NUMBER_OF_PED_PROP_DRAWABLE_VARIATIONS, h, id) || t < 0 ||
                        t >= Function.Call<int>(Hash.GET_NUMBER_OF_PED_PROP_TEXTURE_VARIATIONS, h, id, d))
                {
                    Tell("A saved accessory is unavailable.");
                    return;
                }
            }
        }
        for (int i = 0; i < 12; i++)Function.Call(Hash.SET_PED_COMPONENT_VARIATION, h, i, values[i][0], values[i][1], values[i][2]);
        for (int i = 12; i < 17; i++)
        {
            int id = propIds[i - 12];
            if (values[i][0] < 0)Function.Call(Hash.CLEAR_PED_PROP, h, id, 0);
            else Function.Call(Hash.SET_PED_PROP_INDEX, h, id, values[i][0], values[i][1], true, 0);
        }
        Tell("Outfit loaded.");
    }
    private Entity Controlled()
    {
        Vehicle v = Ride;
        return Valid(v) ? (Entity)v : Hero;
    }
    private void Teleport(Vector3 target, float heading)
    {
        Entity e = Controlled();
        Vector3 from = e.Position;
        float facing = e.Heading;
        Function.Call(Hash.REQUEST_COLLISION_AT_COORD, target.X, target.Y, target.Z);
        e.Position = target;
        e.Heading = heading;
        e.Velocity = Vector3.Zero;
        previousPosition = from;
        previousHeading = facing;
        Tell("Teleported. Backtrack is available.");
    }
    private void Waypoint()
    {
        int blip = Function.Call<int>(Hash.GET_FIRST_BLIP_INFO_ID, 8);
        if (!Function.Call<bool>(Hash.DOES_BLIP_EXIST, blip))
        {
            Tell("Set a waypoint on the map first.");
            return;
        }
        Vector3 pos = Function.Call<Vector3>(Hash.GET_BLIP_INFO_ID_COORD, blip);
        Entity e = Controlled();
        Vector3 original = e.Position;
        bool wasFrozen = e.IsPositionFrozen;
        bool success = false;
        float ground = 0;
        busy = true;
        try
        {
            Function.Call(Hash.FREEZE_ENTITY_POSITION, e.Handle, true);
            // Move the frozen entity temporarily so far-away map collision can stream.
            e.Position = new Vector3(pos.X, pos.Y, 1000);
            long deadline = Now + 2200;
            while (Now < deadline)
            {
                Function.Call(Hash.REQUEST_COLLISION_AT_COORD, pos.X, pos.Y, 1000f);
                OutputArgument z = new OutputArgument();
                if (Function.Call<bool>(Hash.GET_GROUND_Z_FOR_3D_COORD, pos.X, pos.Y, 1000f, z, false, false))
                {
                    ground = z.GetResult<float>();
                    success = true;
                    break;
                }
                Script.Yield();
            }
            e.Position = original;
            if (success)Teleport(new Vector3(pos.X, pos.Y, ground + 1.2f), e.Heading);
            else Tell("Ground was not found. Kept your original position.");
        }
        finally
        {
            if (Valid(e))
            {
                if (!success)e.Position = original;
                Function.Call(Hash.FREEZE_ENTITY_POSITION, e.Handle, wasFrozen);
            }
            busy = false;
        }
    }
    private void SavePosition()
    {
        Entity e = Controlled();
        Vector3 p = e.Position;
        string s = p.X.ToString("R", CultureInfo.InvariantCulture) + "," + p.Y.ToString("R", CultureInfo.InvariantCulture) + "," + p.Z.ToString("R",
                   CultureInfo.InvariantCulture) + "," + e.Heading.ToString("R", CultureInfo.InvariantCulture);
        prefs["Location" + teleportSlot] = s;
        SaveData();
        Tell("Position saved to slot " + (teleportSlot + 1) + ".");
    }
    private void LoadPosition()
    {
        string s;
        if (!prefs.TryGetValue("Location" + teleportSlot, out s))
        {
            Tell("This position slot is empty.");
            return;
        }
        float[] v;
        if (!TruthMenuLogic.TryPosition(s, out v))
        {
            Tell("Saved position is invalid.");
            return;
        }
        Teleport(new Vector3(v[0], v[1], v[2]), v[3]);
    }
    private void Skydive()
    {
        if (HasCar())
        {
            Tell("Exit your vehicle first.");
            return;
        }
        GiveParachute();
        Teleport(Hero.Position + new Vector3(0, 0, 650), Hero.Heading);
    }
    private void Scenario(string id)
    {
        if (HasCar())
        {
            Tell("Exit your vehicle before starting a scenario.");
            return;
        }
        Function.Call(Hash.CLEAR_PED_TASKS, Hero.Handle);
        Function.Call(Hash.TASK_START_SCENARIO_IN_PLACE, Hero.Handle, id, 0, true);
        Tell("Scenario started. Use Stop current scenario to finish.");
    }
    private bool ToySpace()
    {
        toys.RemoveAll(delegate(Entity e)
        {
            return !Valid(e);
        });
        if (toys.Count < 20)return true;
        Tell("20 toys are tracked. Clean up toys before adding more.");
        return false;
    }
    private void SpawnProp(string name)
    {
        if (!ToySpace())return;
        Model m = new Model(name);
        if (!m.IsInCdImage || !m.IsValid)
        {
            Tell("Prop model unavailable.");
            return;
        }
        try
        {
            if (!m.Request(1200))
            {
                Tell("Prop streaming timed out.");
                return;
            }
            Prop p = World.CreateProp(m, Hero.Position + Hero.ForwardVector * 4f, true, true);
            if (Valid(p))
            {
                p.IsPersistent = true;
                toys.Add(p);
                Tell("Toy spawned.");
            }
        }
        finally
        {
            m.MarkAsNoLongerNeeded();
        }
    }
    private void SpawnPed(string name, bool guard)
    {
        if (!ToySpace())return;
        Model m = new Model(name);
        if (!m.IsInCdImage || !m.IsPed)
        {
            Tell("Character model unavailable.");
            return;
        }
        try
        {
            if (!m.Request(1200))
            {
                Tell("Character streaming timed out.");
                return;
            }
            Ped p = World.CreatePed(m, Hero.Position + Hero.ForwardVector * 3f, Hero.Heading);
            if (!Valid(p))
            {
                Tell("The game could not spawn this character.");
                return;
            }
            p.IsPersistent = true;
            toys.Add(p);
            if (guard)
            {
                int group = Function.Call<int>(Hash.GET_PLAYER_GROUP, Game.Player.Handle);
                Function.Call(Hash.SET_PED_AS_GROUP_MEMBER, p.Handle, group);
                Function.Call(Hash.SET_PED_NEVER_LEAVES_GROUP, p.Handle, true);
                Function.Call(Hash.SET_PED_RELATIONSHIP_GROUP_HASH, p.Handle, Function.Call<uint>(Hash.GET_PED_RELATIONSHIP_GROUP_HASH, Hero.Handle));
                Function.Call(Hash.GIVE_WEAPON_TO_PED, p.Handle, (uint)WeaponHash.Pistol, 250, false, true);
            }
            Tell(guard ? "Bodyguard joined your group." : "Guest spawned.");
        }
        finally
        {
            m.MarkAsNoLongerNeeded();
        }
    }
    private void CloneMe()
    {
        if (!ToySpace())return;
        Ped p = Hero.Clone(false);
        if (!Valid(p))
        {
            Tell("The game could not clone this character.");
            return;
        }
        p.Position = Hero.Position + Hero.ForwardVector * 3;
        p.IsPersistent = true;
        toys.Add(p);
        Tell("Your double has arrived.");
    }
    private void CleanToys()
    {
        foreach (Entity e in toys)if (Valid(e))e.Delete();
        toys.Clear();
        Tell("Toys and guests removed.");
    }
    private void HopCar(float height)
    {
        if (!HasCar())return;
        Vehicle v = Ride;
        Vector3 vel = v.Velocity;
        v.Velocity = new Vector3(vel.X, vel.Y, height);
    }
    private bool runOwned, swimOwned, damageOwned, timeOwned, weatherOwned, gravityOwned, radarOwned, radarWasHidden;
    private bool seatbeltOwned, seatbeltOriginal;
    private long nextRepair;
    private void UpdatePed(Ped p)
    {
        if (pedState == null || !Valid(pedState.Ped) || pedState.Ped.Handle != p.Handle)
        {
            RestorePed();
            pedState = new PedState {Ped = p};
        }
        PedState s = pedState;
        if (god)
        {
            if (!s.TouchedGod)
            {
                s.Invincible = p.IsInvincible;
                s.TouchedGod = true;
            }
            p.IsInvincible = true;
        }
        else if (s.TouchedGod)
        {
            p.IsInvincible = s.Invincible;
            s.TouchedGod = false;
        }
        if (invisible)
        {
            if (!s.TouchedVisible)
            {
                s.Visible = p.IsVisible;
                s.TouchedVisible = true;
            }
            p.IsVisible = false;
        }
        else if (s.TouchedVisible)
        {
            p.IsVisible = s.Visible;
            s.TouchedVisible = false;
        }
        if (noRagdoll)
        {
            if (!s.TouchedRagdoll)
            {
                s.Ragdoll = Function.Call<bool>(Hash.CAN_PED_RAGDOLL, p.Handle);
                s.TouchedRagdoll = true;
            }
            Function.Call(Hash.SET_PED_CAN_RAGDOLL, p.Handle, false);
        }
        else if (s.TouchedRagdoll)
        {
            Function.Call(Hash.SET_PED_CAN_RAGDOLL, p.Handle, s.Ragdoll);
            s.TouchedRagdoll = false;
        }
        if (seatbelt && HasCar())
        {
            if (!seatbeltOwned)
            {
                seatbeltOriginal = Function.Call<bool>(Hash.GET_PED_CONFIG_FLAG, p.Handle, 32, true);
                seatbeltOwned = true;
            }
            Function.Call(Hash.SET_PED_CONFIG_FLAG, p.Handle, 32, false);
        }
        else if (seatbeltOwned)
        {
            Function.Call(Hash.SET_PED_CONFIG_FLAG, p.Handle, 32, seatbeltOriginal);
            seatbeltOwned = false;
        }
        if (neverWanted)SetWanted(0);
        if (stamina)Function.Call(Hash.RESTORE_PLAYER_STAMINA, Game.Player.Handle, 1f);
        if (superJump)Function.Call(Hash.SET_SUPER_JUMP_THIS_FRAME, Game.Player.Handle);
        if (runIndex > 0 || runOwned)
        {
            Function.Call(Hash.SET_RUN_SPRINT_MULTIPLIER_FOR_PLAYER, Game.Player.Handle, movement[runIndex]);
            runOwned = runIndex > 0;
        }
        if (swimIndex > 0 || swimOwned)
        {
            Function.Call(Hash.SET_SWIM_MULTIPLIER_FOR_PLAYER, Game.Player.Handle, movement[swimIndex]);
            swimOwned = swimIndex > 0;
        }
        if (infiniteAmmo)
        {
            uint h = CurrentWeaponHash();
            Function.Call(Hash.SET_PED_INFINITE_AMMO, p.Handle, true, h);
            infiniteAmmoWeapons.Add(h);
        }
        else if (infiniteAmmoWeapons.Count > 0)
        {
            foreach (uint h in infiniteAmmoWeapons)Function.Call(Hash.SET_PED_INFINITE_AMMO, p.Handle, false, h);
            infiniteAmmoWeapons.Clear();
        }
        if (infiniteClip || s.TouchedAmmo)
        {
            Function.Call(Hash.SET_PED_INFINITE_AMMO_CLIP, p.Handle, infiniteClip);
            s.TouchedAmmo = infiniteClip;
        }
        if (explosiveAmmo)Function.Call(Hash.SET_EXPLOSIVE_AMMO_THIS_FRAME, Game.Player.Handle);
        if (fireAmmo)Function.Call(Hash.SET_FIRE_AMMO_THIS_FRAME, Game.Player.Handle);
        if (explosiveMelee)Function.Call(Hash.SET_EXPLOSIVE_MELEE_THIS_FRAME, Game.Player.Handle);
        if (damagePercent != 100 || damageOwned)
        {
            Function.Call(Hash.SET_PLAYER_WEAPON_DAMAGE_MODIFIER, Game.Player.Handle, damagePercent / 100f);
            damageOwned = damagePercent != 100;
        }
    }
    private void RestorePed()
    {
        if (pedState != null && Valid(pedState.Ped))
        {
            PedState s = pedState;
            Ped p = s.Ped;
            if (s.TouchedGod)p.IsInvincible = s.Invincible;
            if (s.TouchedVisible)p.IsVisible = s.Visible;
            if (s.TouchedRagdoll)Function.Call(Hash.SET_PED_CAN_RAGDOLL, p.Handle, s.Ragdoll);
            if (s.TouchedAmmo)Function.Call(Hash.SET_PED_INFINITE_AMMO_CLIP, p.Handle, false);
            if (seatbeltOwned)Function.Call(Hash.SET_PED_CONFIG_FLAG, p.Handle, 32, seatbeltOriginal);
            foreach (uint h in infiniteAmmoWeapons)Function.Call(Hash.SET_PED_INFINITE_AMMO, p.Handle, false, h);
        }
        infiniteAmmoWeapons.Clear();
        seatbeltOwned = false;
        pedState = null;
    }
    private void UpdateCar(Vehicle v)
    {
        if (carState != null && (!Valid(v) || !Valid(carState.Car) || v.Handle != carState.Car.Handle))
        {
            RestoreCar();
        }
        if (!Valid(v))return;
        if (carState == null)carState = new CarState {Car = v};
        CarState s = carState;
        int h = v.Handle;
        if (carGod)
        {
            if (!s.TouchedGod)
            {
                s.Invincible = v.IsInvincible;
                s.TouchedGod = true;
            }
            v.IsInvincible = true;
        }
        else if (s.TouchedGod)
        {
            v.IsInvincible = s.Invincible;
            s.TouchedGod = false;
        }
        if (s.AppliedPower != powerPercent)
        {
            Function.Call(Hash.MODIFY_VEHICLE_TOP_SPEED, h, (float)powerPercent);
            s.AppliedPower = powerPercent;
        }
        if (torquePercent != 100 || s.TouchedPower)
            Function.Call(Hash.SET_VEHICLE_CHEAT_POWER_INCREASE, h, torquePercent / 100f);
        s.TouchedPower = powerPercent > 0 || torquePercent != 100;
        // Smooth horizontal limiter: never resets the vehicle's physics or vertical velocity.
        if (mphCap > 0)
        {
            Vector3 vel = v.Velocity;
            float speed = (float)Math.Sqrt(vel.X * vel.X + vel.Y * vel.Y);
            float limited = TruthMenuLogic.LimitedSpeed(speed, mphCap, Game.LastFrameTime);
            if (limited < speed && speed > 0.01f)v.Velocity = new Vector3(vel.X * limited / speed, vel.Y * limited / speed, vel.Z);
        }
        if (drift || s.TouchedGrip)
        {
            Function.Call(Hash.SET_VEHICLE_REDUCE_GRIP, h, drift);
            s.TouchedGrip = drift;
        }
        if (autoRepair && Now >= nextRepair)
        {
            if (v.EngineHealth < 999f || v.BodyHealth < 999f || v.PetrolTankHealth < 999f)RepairMoving(v);
            nextRepair = Now + 1000;
        }
        if (rainbow)
        {
            if (!s.TouchedRainbow)
            {
                CapturePaint(s);
                s.TouchedRainbow = true;
            }
            if (Now >= nextRainbow)
            {
                double t = Now / 900.0;
                int r = (int)(128 + 127 * Math.Sin(t)), g = (int)(128 + 127 * Math.Sin(t + 2.094)), b = (int)(128 + 127 * Math.Sin(t + 4.188));
                Function.Call(Hash.SET_VEHICLE_CUSTOM_PRIMARY_COLOUR, h, r, g, b);
                Function.Call(Hash.SET_VEHICLE_CUSTOM_SECONDARY_COLOUR, h, b, r, g);
                nextRainbow = Now + 60;
            }
        }
        else if (s.TouchedRainbow)
        {
            RestorePaint(s);
            s.TouchedRainbow = false;
        }
        if (!opened && bunny && Now >= nextHop && Function.Call<bool>(Hash.IS_VEHICLE_ON_ALL_WHEELS, h))
        {
            HopCar(5f);
            nextHop = Now + 1500;
        }
        if (!opened && hornBoost && Function.Call<bool>(Hash.IS_CONTROL_PRESSED, 0, 86))
        {
            Vector3 velocity = v.Velocity;
            float speed = (float)Math.Sqrt(velocity.X * velocity.X + velocity.Y * velocity.Y);
            float gain = TruthMenuLogic.BoostGain(speed, mphCap, Game.LastFrameTime);
            if (gain > 0f)
            {
                Vector3 forward = v.ForwardVector;
                v.Velocity = new Vector3(velocity.X + forward.X * gain, velocity.Y + forward.Y * gain, velocity.Z);
            }
        }
    }
    private void CapturePaint(CarState s)
    {
        int h = s.Car.Handle;
        OutputArgument p = new OutputArgument(), q = new OutputArgument();
        Function.Call(Hash.GET_VEHICLE_COLOURS, h, p, q);
        s.Primary = p.GetResult<int>();
        s.Secondary = q.GetResult<int>();
        s.CustomPrimary = Function.Call<bool>(Hash.GET_IS_VEHICLE_PRIMARY_COLOUR_CUSTOM, h);
        s.CustomSecondary = Function.Call<bool>(Hash.GET_IS_VEHICLE_SECONDARY_COLOUR_CUSTOM, h);
        OutputArgument r = new OutputArgument(), g = new OutputArgument(), b = new OutputArgument();
        Function.Call(Hash.GET_VEHICLE_CUSTOM_PRIMARY_COLOUR, h, r, g, b);
        s.Pr = r.GetResult<int>();
        s.Pg = g.GetResult<int>();
        s.Pb = b.GetResult<int>();
        Function.Call(Hash.GET_VEHICLE_CUSTOM_SECONDARY_COLOUR, h, r, g, b);
        s.Sr = r.GetResult<int>();
        s.Sg = g.GetResult<int>();
        s.Sb = b.GetResult<int>();
    }
    private void RestorePaint(CarState s)
    {
        if (!Valid(s.Car))return;
        int h = s.Car.Handle;
        Function.Call(Hash.SET_VEHICLE_COLOURS, h, s.Primary, s.Secondary);
        if (s.CustomPrimary)Function.Call(Hash.SET_VEHICLE_CUSTOM_PRIMARY_COLOUR, h, s.Pr, s.Pg, s.Pb);
        else Function.Call(Hash.CLEAR_VEHICLE_CUSTOM_PRIMARY_COLOUR, h);
        if (s.CustomSecondary)Function.Call(Hash.SET_VEHICLE_CUSTOM_SECONDARY_COLOUR, h, s.Sr, s.Sg, s.Sb);
        else Function.Call(Hash.CLEAR_VEHICLE_CUSTOM_SECONDARY_COLOUR, h);
    }
    private void StopRainbow()
    {
        rainbow = false;
        if (carState != null && carState.TouchedRainbow)
        {
            RestorePaint(carState);
            carState.TouchedRainbow = false;
        }
    }
    private void RestoreCar()
    {
        if (carState != null && Valid(carState.Car))
        {
            CarState s = carState;
            int h = s.Car.Handle;
            if (s.TouchedGod)s.Car.IsInvincible = s.Invincible;
            if (s.TouchedPower)
            {
                Function.Call(Hash.MODIFY_VEHICLE_TOP_SPEED, h, 0f);
                Function.Call(Hash.SET_VEHICLE_CHEAT_POWER_INCREASE, h, 1f);
            }
            if (s.TouchedGrip)Function.Call(Hash.SET_VEHICLE_REDUCE_GRIP, h, false);
            if (s.TouchedRainbow)RestorePaint(s);
        }
        carState = null;
    }
    // Opt-in on-foot camera overlay. Never take ownership of vehicle cameras.
    private void StopFov()
    {
        if (fovCamera == 0)return;
        int camera = fovCamera;
        fovCamera = 0;
        // Do not stop a camera owned by another script or a mission.
        if (Function.Call<int>(Hash.GET_RENDERING_CAM) == camera)
            Function.Call(Hash.RENDER_SCRIPT_CAMS, false, false, 0, true, false, 0);
        Function.Call(Hash.DESTROY_CAM, camera, false);
    }
    private void UpdateFov()
    {
        if (!firstPersonFov)
        {
            StopFov();
            return;
        }
        if (Game.IsPaused || Hero.IsDead || HasCar() || Function.Call<bool>(Hash.IS_CUTSCENE_ACTIVE))
        {
            StopFov();
            return;
        }
        int context = Function.Call<int>(Hash.GET_CAM_ACTIVE_VIEW_MODE_CONTEXT);
        // Context zero is on foot. Vehicle, bike, boat and aircraft cameras are untouched.
        if (context != 0)
        {
            StopFov();
            return;
        }
        int mode = Function.Call<int>(Hash.GET_CAM_VIEW_MODE_FOR_CONTEXT, context);
        if (mode != 4 || Function.Call<bool>(Hash.IS_PLAYER_FREE_AIMING, Game.Player.Handle) ||
                Function.Call<bool>(Hash.IS_FIRST_PERSON_AIM_CAM_ACTIVE))
        {
            StopFov();
            return;
        }
        if (fovCamera != 0 && Function.Call<int>(Hash.GET_RENDERING_CAM) != fovCamera)
        {
            StopFov();
            return;
        }
        if (fovCamera == 0 && !Function.Call<bool>(Hash.IS_GAMEPLAY_CAM_RENDERING))return;
        Vector3 pos = Function.Call<Vector3>(Hash.GET_GAMEPLAY_CAM_COORD);
        Vector3 rot = Function.Call<Vector3>(Hash.GET_GAMEPLAY_CAM_ROT, 2);
        float fov = Function.Call<float>(Hash.GET_GAMEPLAY_CAM_FOV);
        bool created = fovCamera == 0;
        if (created)fovCamera = Function.Call<int>(Hash.CREATE_CAM, "DEFAULT_SCRIPTED_CAMERA", true);
        if (fovCamera == 0)return;
        Function.Call(Hash.SET_CAM_COORD, fovCamera, pos.X, pos.Y, pos.Z);
        Function.Call(Hash.SET_CAM_ROT, fovCamera, rot.X, rot.Y, rot.Z, 2);
        Function.Call(Hash.SET_CAM_FOV, fovCamera, Math.Max(45f, Math.Min(110f, fov * fovPercent / 100f)));
        Function.Call(Hash.SET_SCRIPTED_CAMERA_IS_FIRST_PERSON_THIS_FRAME, true);
        if (created)Function.Call(Hash.RENDER_SCRIPT_CAMS, true, false, 0, true, false, 0);
    }
    private static float DistanceSquared(Vector3 a, Vector3 b)
    {
        float x = a.X - b.X, y = a.Y - b.Y, z = a.Z - b.Z;
        return x * x + y * y + z * z;
    }
    // Bounded traffic controller: model streaming and owned-entity cleanup.
    private void CancelTrafficModel()
    {
        if (trafficPendingHash == 0)return;
        new Model(trafficPendingHash).MarkAsNoLongerNeeded();
        trafficPendingHash = 0;
        trafficDriverModel.MarkAsNoLongerNeeded();
    }
    private void ReleaseTrafficPair(TrafficPair pair)
    {
        // Never delete the player's current ride, including when riding as a passenger.
        if (Valid(pair.Car) && Valid(Hero) && Function.Call<bool>(Hash.IS_PED_IN_VEHICLE, Hero.Handle, pair.Car.Handle, false))
        {
            pair.Car.MarkAsNoLongerNeeded();
            if (Valid(pair.Driver))pair.Driver.MarkAsNoLongerNeeded();
            return;
        }
        if (Valid(pair.Driver))pair.Driver.Delete();
        if (Valid(pair.Car))pair.Car.Delete();
    }
    private void ClearTraffic()
    {
        CancelTrafficModel();
        foreach (TrafficPair pair in trafficCars)try
            {
                ReleaseTrafficPair(pair);
            }
            catch (Exception ex)
            {
                LogException("Traffic cleanup", ex);
            }
        trafficCars.Clear();
        trafficLastFrame = 0;
        trafficStatus = "OFF";
    }
    private bool RoadTrafficModel(Model model)
    {
        if (!model.IsInCdImage || !model.IsVehicle)return false;
        int cls = Function.Call<int>(Hash.GET_VEHICLE_CLASS_FROM_NAME, model.Hash);
        return (cls >= 0 && cls <= 12 || cls == 17 || cls == 18 || cls == 20) &&
               Function.Call<int>(Hash.GET_VEHICLE_MODEL_NUMBER_OF_SEATS, model.Hash) > 0;
    }
    private void UpdateTraffic()
    {
        if (!trafficControl)return;
        int target = noTraffic ? 0 : TruthMenuLogic.TrafficTarget(trafficDensity, trafficOnly, trafficLimit);
        long frameNow = Now;
        float frameSeconds = trafficLastFrame == 0 ? Game.LastFrameTime : (frameNow - trafficLastFrame) / 1000f;
        trafficLastFrame = frameNow;
        trafficFrameTime = trafficFrameTime * .97f + Math.Max(.001f, Math.Min(.1f, frameSeconds)) * .03f;
        if (Now >= nextTrafficCleanup)
        {
            nextTrafficCleanup = Now + 1000;
            for (int i = trafficCars.Count - 1; i >= 0; i--)
            {
                TrafficPair pair = trafficCars[i];
                bool exists = Valid(pair.Car);
                bool occupied = exists && Function.Call<bool>(Hash.IS_PED_IN_VEHICLE, Hero.Handle, pair.Car.Handle, false);
                bool remove = !exists || occupied;
                if (exists && !occupied)
                {
                    Vector3 pos = pair.Car.Position;
                    float distance = DistanceSquared(pos, Hero.Position);
                    remove = distance > 300f * 300f || ((trafficCars.Count > target || !Valid(pair.Driver) || pair.Driver.IsDead) && distance > 60f * 60f &&
                                                            !Function.Call<bool>(Hash.IS_SPHERE_VISIBLE, pos.X, pos.Y, pos.Z, 6f));
                }
                if (remove)
                {
                    ReleaseTrafficPair(pair);
                    trafficCars.RemoveAt(i);
                }
            }
        }
        if (Game.IsPaused || Hero.IsDead || Function.Call<bool>(Hash.IS_CUTSCENE_ACTIVE) || busy)
        {
            trafficStatus = "SUSPENDED";
            return;
        }
        if (trafficFpsGuard && trafficFrameTime > 1f / 30f)
        {
            trafficStatus = "FPS GUARD";
            CancelTrafficModel();
            return;
        }
        if (trafficCars.Count >= target)
        {
            trafficStatus = target == 0 ? "AMBIENT ONLY" : "AT TARGET";
            CancelTrafficModel();
            return;
        }
        if (Now < nextTraffic)return;
        if (trafficPendingHash == 0)
        {
            Model model;
            if (trafficType == 7)model = new Model(selectedVehicleHash);
            else
            {
                string[] pool = trafficModels[trafficType];
                model = new Model(pool[random.Next(pool.Length)]);
            }
            if (!RoadTrafficModel(model))
            {
                trafficStatus = "CHOOSE ROAD MODEL";
                nextTraffic = Now + 2000;
                return;
            }
            trafficPendingHash = model.Hash;
            trafficPendingUntil = Now + 5000;
            Function.Call(Hash.REQUEST_MODEL, model.Hash);
            Function.Call(Hash.REQUEST_MODEL, trafficDriverModel.Hash);
            trafficStatus = "STREAMING";
            return;
        }
        if (!Function.Call<bool>(Hash.HAS_MODEL_LOADED, trafficPendingHash) || !Function.Call<bool>(Hash.HAS_MODEL_LOADED, trafficDriverModel.Hash))
        {
            if (Now > trafficPendingUntil)
            {
                CancelTrafficModel();
                nextTraffic = Now + 1000;
                trafficStatus = "STREAM TIMEOUT";
            }
            return;
        }
        // One road query and at most one spawn per interval. Never wait/yield for model streaming.
        nextTraffic = Now + 650;
        double angle = random.NextDouble() * Math.PI * 2;
        float radius = 100f + random.Next(80);
        Vector3 center = Hero.Position;
        OutputArgument node = new OutputArgument(), heading = new OutputArgument(), lanes = new OutputArgument();
        if (!Function.Call<bool>(Hash.GET_NTH_CLOSEST_VEHICLE_NODE_WITH_HEADING, center.X + (float)Math.Cos(angle)*radius,
                                 center.Y + (float)Math.Sin(angle)*radius, center.Z, 1, node, heading, lanes, 1, 3f, 0f))
        {
            trafficStatus = "LOOKING FOR ROAD";
            return;
        }
        Vector3 position = node.GetResult<Vector3>();
        if (DistanceSquared(position, center) < 75f * 75f || DistanceSquared(position, center) > 230f * 230f ||
                Math.Abs(position.Z - center.Z) > 45f ||
                Function.Call<bool>(Hash.IS_SPHERE_VISIBLE, position.X, position.Y, position.Z, 8f) ||
                Function.Call<bool>(Hash.IS_ANY_VEHICLE_NEAR_POINT, position.X, position.Y, position.Z, 12f))
        {
            trafficStatus = "WAITING FOR SPACE";
            return;
        }
        Vehicle car = null;
        Ped driver = null;
        bool tracked = false;
        try
        {
            car = World.CreateVehicle(new Model(trafficPendingHash), position, heading.GetResult<float>());
            if (!Valid(car))
            {
                trafficStatus = "SPAWN FAILED";
                return;
            }
            car.IsPersistent = true;
            Function.Call(Hash.SET_VEHICLE_ON_GROUND_PROPERLY, car.Handle, 5f);
            int handle = Function.Call<int>(Hash.CREATE_PED_INSIDE_VEHICLE, car.Handle, 4, trafficDriverModel.Hash, -1, false, true);
            if (handle == 0)
            {
                trafficStatus = "DRIVER FAILED";
                return;
            }
            driver = Entity.FromHandle(handle) as Ped;
            if (!Valid(driver))
            {
                trafficStatus = "DRIVER FAILED";
                return;
            }
            driver.IsPersistent = true;
            Function.Call(Hash.SET_DRIVER_ABILITY, handle, .85f);
            Function.Call(Hash.SET_PED_KEEP_TASK, handle, true);
            Function.Call(Hash.TASK_VEHICLE_DRIVE_WANDER, handle, car.Handle, 20f, 786603);
            trafficCars.Add(new TrafficPair {Car = car, Driver = driver});
            tracked = true;
            trafficStatus = "ADDING TRAFFIC";
        }
        finally
        {
            if (!tracked)
            {
                if (Valid(driver))driver.Delete();
                if (Valid(car))car.Delete();
            }
            CancelTrafficModel();
        }
    }
    private void ApplyWorld()
    {
        if (timePercent != 100 || timeOwned)
        {
            Function.Call(Hash.SET_TIME_SCALE, timePercent / 100f);
            timeOwned = timePercent != 100;
        }
        if (noTraffic || trafficControl)
        {
            float density = noTraffic || trafficOnly ? 0f : Math.Min(1f, trafficDensity / 100f);
            Function.Call(Hash.SET_VEHICLE_DENSITY_MULTIPLIER_THIS_FRAME, density);
            Function.Call(Hash.SET_RANDOM_VEHICLE_DENSITY_MULTIPLIER_THIS_FRAME, density);
            Function.Call(Hash.SET_PARKED_VEHICLE_DENSITY_MULTIPLIER_THIS_FRAME, density);
        }
        if (noPeds)
        {
            Function.Call(Hash.SET_PED_DENSITY_MULTIPLIER_THIS_FRAME, 0f);
            Function.Call(Hash.SET_SCENARIO_PED_DENSITY_MULTIPLIER_THIS_FRAME, 0f, 0f);
        }
        if (hideHud)Function.Call(Hash.HIDE_HUD_AND_RADAR_THIS_FRAME);
        if (hideRadar)
        {
            if (!radarOwned)
            {
                radarWasHidden = Function.Call<bool>(Hash.IS_RADAR_HIDDEN);
                radarOwned = true;
            }
            Function.Call(Hash.DISPLAY_RADAR, false);
        }
        else if (radarOwned)
        {
            Function.Call(Hash.DISPLAY_RADAR, !radarWasHidden);
            radarOwned = false;
        }
    }
    private void ReleaseWeather()
    {
        if (weatherOwned)
        {
            Function.Call(Hash.CLEAR_OVERRIDE_WEATHER);
            Function.Call(Hash.CLEAR_WEATHER_TYPE_PERSIST);
            weatherOwned = false;
        }
    }
    private void ResetEffects()
    {
        firstPersonFov = false;
        Guard(StopFov);
        trafficControl = false;
        Guard(ClearTraffic);
        Guard(RestorePed);
        Guard(RestoreCar);
        int player = Game.Player.Handle;
        if (runOwned)Function.Call(Hash.SET_RUN_SPRINT_MULTIPLIER_FOR_PLAYER, player, 1f);
        if (swimOwned)Function.Call(Hash.SET_SWIM_MULTIPLIER_FOR_PLAYER, player, 1f);
        if (damageOwned)Function.Call(Hash.SET_PLAYER_WEAPON_DAMAGE_MODIFIER, player, 1f);
        if (policeIgnore)Function.Call(Hash.SET_POLICE_IGNORE_PLAYER, player, false);
        if (everyoneIgnore)Function.Call(Hash.SET_EVERYONE_IGNORE_PLAYER, player, false);
        if (freezeClock)Function.Call(Hash.PAUSE_CLOCK, false);
        if (blackout)Function.Call(Hash.SET_ARTIFICIAL_LIGHTS_STATE, false);
        if (nightVision)Function.Call(Hash.SET_NIGHTVISION, false);
        if (thermal)Function.Call(Hash.SET_SEETHROUGH, false);
        if (timeOwned)Function.Call(Hash.SET_TIME_SCALE, 1f);
        if (gravityOwned)Function.Call(Hash.SET_GRAVITY_LEVEL, 0);
        if (radarOwned)Function.Call(Hash.DISPLAY_RADAR, !radarWasHidden);
        ReleaseWeather();
        god = neverWanted = stamina = superJump = invisible = noRagdoll = policeIgnore = everyoneIgnore = false;
        carGod = autoRepair = seatbelt = drift = rainbow = bunny = hornBoost = false;
        infiniteAmmo = infiniteClip = explosiveAmmo = fireAmmo = explosiveMelee = false;
        freezeClock = blackout = noTraffic = noPeds = nightVision = thermal = hideHud = hideRadar = false;
        runOwned = swimOwned = damageOwned = timeOwned = gravityOwned = radarOwned = false;
        runIndex = swimIndex = powerPercent = mphCap = gravityMode = 0;
        torquePercent = damagePercent = timePercent = 100;
    }
    private void OnAbort(object sender, EventArgs e)
    {
        try
        {
            ResetEffects();
        }
        catch (Exception ex)
        {
            LogException("Reset during shutdown", ex);
        }
        foreach (Entity toy in toys)try
            {
                if (Valid(toy))toy.Delete();
            }
            catch {}
        foreach (Vehicle v in spawned)try
            {
                if (Valid(v))v.MarkAsNoLongerNeeded();
            }
            catch {}
        Log("Stopped.");
    }
    // Local persistence. Runtime state is excluded from source releases.
    private void LoadPrefs()
    {
        try
        {
            string file = Path.Combine(dataDir, "preferences.ini");
            if (File.Exists(file))foreach (string l in File.ReadAllLines(file))
                {
                    int i = l.IndexOf('=');
                    if (i > 0)prefs[l.Substring(0, i)] = l.Substring(i + 1);
                }
            opacity = PrefInt("Opacity", 185, 90, 235);
            scalePercent = PrefInt("Scale", 100, 80, 105);
            rightSide = PrefBool("RightSide", false);
            binary = PrefBool("Binary", true);
            sound = PrefBool("Sound", true);
            hints = PrefBool("Hints", true);
            watermark = PrefBool("Watermark", true);
            speedometer = PrefBool("Speedometer", false);
            string f;
            if (prefs.TryGetValue("Favorites", out f))foreach (string s in f.Split(','))
                {
                    int n;
                    if (int.TryParse(s, out n))favorites.Add(n);
                }
        }
        catch (Exception ex)
        {
            LogException("Preferences", ex);
        }
    }
    private int PrefInt(string k, int def, int min, int max)
    {
        string s;
        int n;
        return prefs.TryGetValue(k, out s) && int.TryParse(s, out n) ? Clamp(n, min, max) : def;
    }
    private bool PrefBool(string k, bool def)
    {
        string s;
        bool b;
        return prefs.TryGetValue(k, out s) && bool.TryParse(s, out b) ? b : def;
    }
    private void SavePrefs()
    {
        prefs["Opacity"] = opacity.ToString();
        prefs["Scale"] = scalePercent.ToString();
        prefs["RightSide"] = rightSide.ToString();
        prefs["Binary"] = binary.ToString();
        prefs["Sound"] = sound.ToString();
        prefs["Hints"] = hints.ToString();
        prefs["Watermark"] = watermark.ToString();
        prefs["Speedometer"] = speedometer.ToString();
        SaveData();
    }
    private void SaveData()
    {
        List<string> fs = new List<string>();
        foreach (int h in favorites)fs.Add(h.ToString(CultureInfo.InvariantCulture));
        prefs["Favorites"] = string.Join(",", fs.ToArray());
        Directory.CreateDirectory(dataDir);
        List<string> lines = new List<string>();
        foreach (KeyValuePair<string, string> p in prefs)lines.Add(p.Key + "=" + p.Value);
        string file = Path.Combine(dataDir, "preferences.ini"), temp = file + ".tmp";
        File.WriteAllLines(temp, lines.ToArray());
        if (File.Exists(file))File.Replace(temp, file, null);
        else File.Move(temp, file);
    }
    // Virtual canvas: 870 x 750, scaled from a 900-high reference with aspect correction.
    private void Layout()
    {
        aspect = Function.Call<float>(Hash.GET_ASPECT_RATIO, false);
        if (aspect < 1f || aspect > 4f)aspect = 16f / 9f;
        uiScale = Math.Min(scalePercent / 100f, Math.Min(.90f * 900f / 750f, .94f * 900f * aspect / 870f));
        left = rightSide ? 1f - .025f - 870f * uiScale / (900f * aspect) : .025f;
        top = .045f;
    }
    private float X(float x)
    {
        return left + x * uiScale / (900f * aspect);
    }
    private float Y(float y)
    {
        return top + y * uiScale / 900f;
    }
    private void Rect(float x, float y, float w, float h, int r, int g, int b, int a)
    {
        Function.Call(Hash.DRAW_RECT, X(x + w * .5f), Y(y + h * .5f), w * uiScale / (900f * aspect), h * uiScale / 900f, r, g, b, a, false);
    }
    private void Box(float x, float y, float w, float h, bool active, int fill)
    {
        Rect(x, y, w, h, 1, 12, 6, fill);
        int alpha = active ? 255 : 170;
        int green = active ? 255 : 205;
        Rect(x, y, w, 1, 30, green, 65, alpha);
        Rect(x, y + h - 1, w, 1, 30, green, 65, alpha);
        Rect(x, y, 1, h, 30, green, 65, alpha);
        Rect(x + w - 1, y, 1, h, 30, green, 65, alpha);
        if (active)
        {
            for (int band = 0; band < 3; band++)Rect(x + 1, y + 1 + band * (h - 2) / 3f, w - 2, (h - 2) / 3f, 10, 145 - band * 28, 46 - band * 10, 190);
            Rect(x, y, 4, h, 126, 255, 168, 255);
            Rect(x, y, w, 2, 120, 255, 158, 255);
        }
    }
    private void Txt(string value, float x, float y, float size, int r, int g, int b, int a, float width)
    {
        string s = (value ?? "").Replace("~", " ");
        float scale = size * uiScale;
        float maxWidth = width * uiScale / (900f * aspect);
        Function.Call(Hash.SET_TEXT_FONT, 0);
        Function.Call(Hash.SET_TEXT_SCALE, 0f, scale);
        if (TextWidth(s) > maxWidth)
        {
            int lo = 0, hi = s.Length;
            while (lo < hi)
            {
                int mid = (lo + hi + 1) / 2;
                if (TextWidth(s.Substring(0, mid) + "...") <= maxWidth)lo = mid;
                else hi = mid - 1;
            }
            s = s.Substring(0, lo) + "...";
        }
        Function.Call(Hash.SET_TEXT_COLOUR, r, g, b, a);
        Function.Call(Hash.SET_TEXT_CENTRE, false);
        Function.Call(Hash.SET_TEXT_RIGHT_JUSTIFY, false);
        Function.Call(Hash.SET_TEXT_WRAP, X(x), X(x + width));
        Function.Call(Hash.BEGIN_TEXT_COMMAND_DISPLAY_TEXT, "STRING");
        Function.Call(Hash.ADD_TEXT_COMPONENT_SUBSTRING_PLAYER_NAME, s);
        Function.Call(Hash.END_TEXT_COMMAND_DISPLAY_TEXT, X(x), Y(y), 0);
    }
    private float TextWidth(string s)
    {
        Function.Call(Hash.BEGIN_TEXT_COMMAND_GET_SCREEN_WIDTH_OF_DISPLAY_TEXT, "STRING");
        Function.Call(Hash.ADD_TEXT_COMPONENT_SUBSTRING_PLAYER_NAME, s);
        return Function.Call<float>(Hash.END_TEXT_COMMAND_GET_SCREEN_WIDTH_OF_DISPLAY_TEXT, true);
    }
    private void Logo(float x, float y, float size)
    {
        Rect(x, y, size, size, 0, 0, 0, 255);
        float pixel = size / LogoGrid;
        for (int i = 0; i < LogoSpans.Length; i += 4)
        {
            Rect(x + LogoSpans[i]*pixel, y + LogoSpans[i + 1]*pixel, LogoSpans[i + 2]*pixel, LogoSpans[i + 3]*pixel, 255, 255, 255, 255);
        }
    }
    // Keep menu + toast below the regression-tested 330-rectangle budget.
    private void DrawMenu()
    {
        Layout();
        // Binary glyphs belong to the backdrop, below ALL panel/text commands.
        Function.Call(Hash.SET_SCRIPT_GFX_DRAW_ORDER, 3);
        // Bright layered emerald frame around transparent black glass.
        Rect(-7, -7, 884, 764, 0, 255, 82, 22);
        Rect(-4, -4, 878, 758, 0, 255, 82, 48);
        Rect(-2, -2, 874, 754, 48, 255, 105, 200);
        Box(0, 0, 870, 750, false, opacity);
        for (int band = 0; band < 8; band++)Rect(1, 1 + band * 13.5f, 868, 13.5f, 0, 180 - band * 18, 55 - band * 6, 60);
        Rect(0, 0, 870, 3, 147, 255, 182, 255);
        Rect(0, 747, 870, 3, 20, 255, 90, 255);
        if (binary)
        {
            int phase = (int)(Now / 170);
            for (int col = 0; col < 18; col++)
            {
                float xx = 18 + col * 47;
                float yy = 95 + ((phase * 5 + col * 71) % 560);
                Txt(((phase + col) % 2 == 0) ? "101" : "010", xx, yy, .18f, 45, 255, 100, 110, 40);
                if (yy > 125)Txt("01", xx, yy - 26, .16f, 15, 205, 65, 65, 35);
            }
        }
        Function.Call(Hash.SET_SCRIPT_GFX_DRAW_ORDER, 4);
        Box(10, 10, 850, 79, false, Math.Min(220, opacity));
        for (int band = 0; band < 6; band++)Rect(11, 11 + band * (76f / 6f), 848, 76f / 6f, 0, 125 - band * 18, 38 - band * 6, 100);
        Rect(10, 87, 850, 2, 80, 255, 135, 255);
        Box(22, 21, 55, 55, true, 210);
        Logo(25, 24, 49);
        Txt("TRUTH'S GTA5", 94, 19, .43f, 235, 255, 240, 255, 410);
        Txt("STORY MODE+", 95, 52, .28f, 67, 232, 104, 255, 325);
        Box(666, 22, 180, 23, false, 100);
        Txt("ENHANCED / SINGLE PLAYER", 676, 24, .18f, 103, 222, 134, 255, 165);
        Txt(Version, 666, 55, .19f, 83, 171, 107, 255, 180);
        Rect(10, 94, 850, 2, 51, 255, 100, 155);
        Txt("CONTROL CENTER", 23, 111, .20f, 73, 160, 96, 255, 159);
        for (int i = 0; i < pages.Length; i++)
        {
            float y = 143 + i * 33;
            Box(20, y, 164, 29, i == page, i == page ? 215 : 100);
            Txt((i + 1).ToString("00"), 28, y + 5, .18f, 57, 151, 82, 255, 23);
            Txt(pages[i], 57, y + 4, .218f, i == page ? 154 : 82, i == page ? 255 : 184, i == page ? 177 : 111, 255, 118);
        }
        Box(20, 614, 164, 58, false, 140);
        Txt("STORY MODE+", 31, 623, .22f, 84, 232, 116, 255, 143);
        Txt("F5 TO CLOSE", 31, 647, .18f, 69, 158, 93, 255, 143);
        string title = view == "vehicles" ? "VEHICLE CATALOG" : view == "weapons" ? "WEAPON CATALOG" : view == "places" ? "LANDMARKS" : pages[page];
        Txt(title, 206, 106, .33f, 150, 255, 175, 255, 440);
        Txt((options.Count == 0 ? 0 : cursor + 1).ToString("00") + " / " + options.Count.ToString("00"), 750, 111, .23f, 73, 219, 111, 255, 96);
        string sub = query.Length > 0 && (page == 3 || page == 7) ? "SEARCH: " + query : pageHints[page];
        Txt(sub, 207, 142, .19f, 79, 174, 106, 255, 635);
        int end = Math.Min(options.Count, scroll + visibleRows);
        for (int i = scroll; i < end; i++)
        {
            Option o = options[i];
            bool active = i == cursor;
            bool enabled = o.Enabled == null || o.Enabled();
            float y = 174 + (i - scroll) * 46;
            Box(203, y, 644, 42, active, active ? Math.Min(245, opacity + 20) : Math.Max(75, opacity - 55));
            Txt(o.Name, 219, y + 10, .27f, enabled ? (active ? 160 : 118) : 60, enabled ? (active ? 255 : 214) : 114,
                enabled ? (active ? 187 : 142) : 75, 255, 385);
            string val = o.Value == null ? o.Kind : o.Value();
            if (!enabled)val = "ENTER VEHICLE";
            if (confirm.Length > 0 && Now < confirmUntil && active)val = "APPLY TO CONFIRM";
            Box(616, y + 7, 218, 28, false, active ? 200 : 110);
            if (o.Change != null)Txt("<", 624, y + 12, .20f, 76, 219, 111, 255, 14);
            Txt(val, 642, y + 11, .215f, enabled ? 92 : 54, enabled ? 242 : 126, enabled ? 134 : 77, 255, 171);
            if (o.Change != null)Txt(">", 817, y + 12, .20f, 76, 219, 111, 255, 12);
        }
        if (options.Count > visibleRows)
        {
            Rect(851, 174, 3, 410, 22, 63, 35, 180);
            float thumb = Math.Max(20, 410f * visibleRows / options.Count);
            float offset = (410 - thumb) * scroll / Math.Max(1, options.Count - visibleRows);
            Rect(851, 174 + offset, 3, thumb, 64, 234, 108, 220);
        }
        Box(203, 598, 644, 74, false, Math.Max(150, opacity));
        Txt("SELECTED OPTION", 217, 607, .16f, 46, 164, 82, 255, 610);
        string help = hints && options.Count > 0 ? options[cursor].Help : "TRUTH'S GTA5 STORY MODE+ / Your session. Your rules.";
        DrawHelp(help, 217, 628, 616);
        Rect(10, 684, 850, 1, 30, 164, 65, 135);
        if (watermark)
        {
            Box(21, 697, 38, 38, false, 220);
            Logo(24, 700, 32);
        }
        Txt("TRUTH'S GTA5 STORY MODE+", watermark ? 69 : 23, 696, .18f, 106, 238, 140, 255, watermark ? 231 : 277);
        bool pad = !Function.Call<bool>(Hash.IS_USING_KEYBOARD_AND_MOUSE, 2);
        Txt(pad ? "DPAD  SELECT / EDIT     A / CROSS  APPLY     B / CIRCLE  BACK" : "ARROWS  SELECT / EDIT     ENTER  APPLY     ESC  BACK", 310,
            696, .18f, 101, 206, 125, 255, 531);
        Txt(pad ? "LB / RB  CATEGORY     X / SQUARE  SEARCH / INPUT     F5  CLOSE" :
            "TAB / SHIFT+TAB  CATEGORY     F6  SEARCH / INPUT     F5  CLOSE", 310, 722, .175f, 63, 157, 87, 255, 531);
        Vehicle v = Ride;
        string status = Valid(v) ? (v.Speed * 2.236936f).ToString("0") + " MPH" : "ON FOOT";
        Txt(status + "  /  " + Cash(Game.Player.Money), watermark ? 69 : 23, 722, .17f, 76, 185, 104, 255, watermark ? 231 : 277);
    }
    private void DrawHelp(string message, float x, float y, float width)
    {
        // Wrap using native glyph widths; no word is hidden behind a value box.
        string[] words = message.Split(' ');
        string first = "", second = "";
        Function.Call(Hash.SET_TEXT_FONT, 0);
        Function.Call(Hash.SET_TEXT_SCALE, 0f, .205f * uiScale);
        float available = width * uiScale / (900f * aspect);
        bool lineTwo = false;
        foreach (string word in words)
        {
            if (!lineTwo && TextWidth((first.Length > 0 ? first + " " : "") + word) > available)lineTwo = true;
            if (lineTwo)second += (second.Length > 0 ? " " : "") + word;
            else first += (first.Length > 0 ? " " : "") + word;
        }
        Txt(first, x, y, .205f, 105, 202, 128, 255, width);
        if (second.Length > 0)Txt(second, x, y + 22, .205f, 105, 202, 128, 255, width);
    }
    private void DrawToast()
    {
        Function.Call(Hash.SET_SCRIPT_GFX_DRAW_ORDER, 4);
        Layout();
        float oldLeft = left, oldTop = top;
        left = .025f;
        top = .905f;
        // Separate full-width lines keep the brand and status from clipping.
        Box(0, 0, 870, 58, false, 220);
        Txt("Truths Story Mode+", 13, 5, .23f, 110, 255, 145, 255, 844);
        Txt(toast, 13, 31, .20f, 133, 226, 155, 255, 844);
        left = oldLeft;
        top = oldTop;
    }
    private void DrawSpeed()
    {
        Function.Call(Hash.SET_SCRIPT_GFX_DRAW_ORDER, 4);
        Layout();
        left = .025f;
        top = .815f;
        Box(0, 0, 236, 74, false, 185);
        Txt("TRUTH'S / STORY MODE+", 12, 10, .17f, 85, 218, 119, 255, 211);
        Txt((Ride.Speed * 2.236936f).ToString("0") + " MPH", 12, 31, .37f, 149, 255, 178, 255, 211);
    }
}

// Pure helpers are also exercised by the packaged validation harness.
public static class TruthMenuLogic
{
    public static int TrafficTarget(int density, bool only, int limit)
    {
        return Math.Min(Math.Max(0, limit), Math.Max(0, only ? density * 12 / 100 : (density - 100) * 15 / 100));
    }
    public static int Wrap(int n, int count)
    {
        return count <= 0 ? 0 : (n % count + count) % count;
    }
    public static int Scroll(int selected, int current, int rows)
    {
        if (selected < current)return selected;
        if (selected >= current + rows)return selected - rows + 1;
        return current;
    }
    public static int ClampMoney(long n)
    {
        return (int)Math.Max(0L, Math.Min(2000000000L, n));
    }
    public static float MphToMps(int n)
    {
        return n / 2.236936f;
    }
    public static float LimitedSpeed(float speed, int cap, float dt)
    {
        if (cap <= 0 || speed <= MphToMps(cap))return speed;
        return Math.Max(MphToMps(cap), speed - 12f * Math.Max(0f, Math.Min(.05f, dt)));
    }
    public static float BoostGain(float speed, int cap, float dt)
    {
        float gain = 20f * Math.Max(0f, Math.Min(.05f, dt));
        return cap <= 0 ? gain : Math.Max(0f, Math.Min(gain, MphToMps(cap) - speed));
    }
    public static bool Matches(string text, string query)
    {
        return string.IsNullOrWhiteSpace(query) || text.IndexOf(query.Trim(), StringComparison.OrdinalIgnoreCase) >= 0;
    }
    public static bool TryMoney(string value, out int amount)
    {
        amount = 0;
        if (value == null)return false;
        string s = value.Trim().Replace(",", "").Replace("$", "");
        long n;
        if (!long.TryParse(s, NumberStyles.None, CultureInfo.InvariantCulture, out n) || n < 0 || n > 2000000000L)return false;
        amount = (int)n;
        return true;
    }
    public static bool TryRgb(string value, out int[] rgb)
    {
        rgb = new int[3];
        if (value == null)return false;
        string[] p = value.Split(new char[] {' ', ','}, StringSplitOptions.RemoveEmptyEntries);
        if (p.Length != 3)return false;
        for (int i = 0; i < 3; i++)if (!int.TryParse(p[i], out rgb[i]) || rgb[i] < 0 || rgb[i] > 255)return false;
        return true;
    }
    public static bool TryPosition(string value, out float[] pos)
    {
        pos = new float[4];
        if (value == null)return false;
        string[] p = value.Split(',');
        if (p.Length != 4)return false;
        for (int i = 0; i < 4; i++)if (!float.TryParse(p[i], NumberStyles.Float, CultureInfo.InvariantCulture, out pos[i]) || float.IsNaN(pos[i]) ||
                                           float.IsInfinity(pos[i]))return false;
        return Math.Abs(pos[0]) < 20000 && Math.Abs(pos[1]) < 20000 && pos[2] > -2000 && pos[2] < 10000;
    }
}
