using HarmonyLib;
using NeosModLoader;
using FrooxEngine;
using System.Reflection;
using System.Collections.Generic;

namespace ExposePatchedMethods
{
    public class ExposePatchedMethods : NeosMod
    {
        public override string Name => "ExposePatchedMethods";
        public override string Author => "eia485, kazu0617";
        public override string Version => "2.0.0";
        public override string Link => "https://github.com/EIA485/NeosExposePatchedMethods";

        [AutoRegisterConfigKey]
        private static readonly ModConfigurationKey<bool> Key_ExposeUserspace = new("Show in userspace", "Enables this texts to User under a root in the root of UserSpace.", () => true);

        [AutoRegisterConfigKey]
        private static ModConfigurationKey<bool> Key_ExposeEverywhere = new("Show Everywhare", "Enables this texts to User in WorldSpace. That makes show everywhare.", () => false);

        private static ModConfiguration Config;

        static HashSet<string> HarmonyIds = new HashSet<string>();
        public override void OnEngineInit()
        {
            Config = GetConfiguration();
            Config.Save(true);

            foreach (MethodBase method in Harmony.GetAllPatchedMethods())
            {
                foreach (string owner in Harmony.GetPatchInfo(method).Owners)
                {
                    HarmonyIds.Add(owner);
                }
            }
            Harmony harmony = new Harmony("net.eia485.ExposePatchedMethods");
            harmony.PatchAll();

        }

        [HarmonyPatch]
        class PatchEveryWhare
        {
            [HarmonyPostfix]
            [HarmonyPatch(typeof(Userspace), "OnAttach")]
            static void UserspaceOnAttachPostfix(Userspace __instance)
            {
                if (Config.GetValue(Key_ExposeUserspace)) GenList(__instance.World.RootSlot);
            }
            [HarmonyPostfix]
            [HarmonyPatch(typeof(User), "Root", MethodType.Setter)]
            static void UserRootSetterPostfix(User __instance)
            {
                if (Config.GetValue(Key_ExposeEverywhere)) GenList(__instance.Root.Slot);
            }
        }

        static void GenList(Slot slot)
        {
            Slot list = slot.AddSlot("Loaded mods", false);
            foreach (string harmonyId in HarmonyIds)
            {
                list.AddSlot(harmonyId);
            }
        }
    }
}
