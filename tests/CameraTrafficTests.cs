using System;
using System.Collections;
using System.Reflection;
using GTA;
using GTA.Native;
using GTA.Math;

public static class CameraTrafficTests
{
    private static int count, context, mode = 4, rendering, created = 100;
    private static int setFovCalls, randomMods, colors, extras;
    private static void Set(object o, string name, object value)
    {
        o.GetType().GetField(name, BindingFlags.NonPublic | BindingFlags.Instance).SetValue(o, value);
    }
    private static object Get(object o, string name)
    {
        return o.GetType().GetField(name, BindingFlags.NonPublic | BindingFlags.Instance).GetValue(o);
    }
    private static void Call(object o, string name)
    {
        o.GetType().GetMethod(name, BindingFlags.NonPublic | BindingFlags.Instance).Invoke(o, null);
    }
    private static void Check(bool ok, string name)
    {
        count++;
        if (!ok)throw new Exception(name);
    }

    public static void Main()
    {
        TruthStoryPlus menu = new TruthStoryPlus();
        Function.Hook = delegate(Hash h, object[] p)
        {
            if (h == Hash.GET_CAM_ACTIVE_VIEW_MODE_CONTEXT)return context;
            if (h == Hash.GET_CAM_VIEW_MODE_FOR_CONTEXT)return mode;
            if (h == Hash.GET_RENDERING_CAM)return rendering;
            if (h == Hash.IS_GAMEPLAY_CAM_RENDERING)return rendering == 0;
            if (h == Hash.GET_GAMEPLAY_CAM_FOV)return 60f;
            if (h == Hash.GET_GAMEPLAY_CAM_COORD || h == Hash.GET_GAMEPLAY_CAM_ROT)return Vector3.Zero;
            if (h == Hash.CREATE_CAM)return ++created;
            if (h == Hash.RENDER_SCRIPT_CAMS)rendering = (bool)p[0] ? created : 0;
            if (h == Hash.SET_CAM_FOV)setFovCalls++;
            if (h == Hash.GET_NUM_VEHICLE_MODS)return 3;
            if (h == Hash.GET_VEHICLE_LIVERY_COUNT)return 4;
            if (h == Hash.GET_VEHICLE_MODEL_NUMBER_OF_SEATS)return 4;
            if (h == Hash.SET_VEHICLE_MOD)randomMods++;
            if (h == Hash.SET_VEHICLE_CUSTOM_PRIMARY_COLOUR || h == Hash.SET_VEHICLE_CUSTOM_SECONDARY_COLOUR)colors++;
            if (h == Hash.DOES_EXTRA_EXIST)return true;
            if (h == Hash.SET_VEHICLE_EXTRA)extras++;
            return null;
        };

        Set(menu, "firstPersonFov", true);
        Game.Player.Character.InVehicle = false;
        context = 0;
        mode = 4;
        Call(menu, "UpdateFov");
        Check(rendering != 0 && setFovCalls == 1, "on-foot first-person FOV renders");
        context = 1;
        Call(menu, "UpdateFov");
        Check(rendering == 0 && setFovCalls == 1, "vehicle context releases FOV and never changes it");
        context = 0;
        mode = 2;
        Call(menu, "UpdateFov");
        Check(rendering == 0 && setFovCalls == 1, "on-foot third person remains native");
        mode = 4;
        Game.Player.Character.InVehicle = true;
        context = 0;
        Call(menu, "UpdateFov");
        Check(rendering == 0 && setFovCalls == 1, "entering a vehicle releases FOV before camera context changes");

        Call(menu, "RandomCustomizedVehicle");
        IList spawned = (IList)Get(menu, "spawned");
        Check(spawned.Count == 1, "random customized vehicle spawned");
        Check(randomMods > 20, "supported random and performance mods applied");
        Check(colors == 2, "primary and secondary custom paint applied");
        Check(extras == 14, "supported extras randomized");

        for (int density = 0; density <= 500; density += 25)for (int limit = 10; limit <= 60; limit += 5)
            {
                int extra = TruthMenuLogic.TrafficTarget(density, false, limit);
                Check(extra >= 0 && extra <= limit, "traffic target stays capped");
                if (density <= 100)Check(extra == 0, "standard density needs no extra cars");
            }
        Function.Hook = null;
        Console.WriteLine("PASS: " + count + " on-foot FOV, random vehicle and traffic assertions.");
    }
}
