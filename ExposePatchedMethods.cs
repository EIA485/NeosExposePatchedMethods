using HarmonyLib;
using NeosModLoader;
using FrooxEngine;
using System.Reflection;
using System.Collections.Generic;

namespace ExposePatchedMethods
{
    public class ExposePatchedMethods : NeosMod
    {
        public override string Name => BuildInfo.Name;
        public override string Author => BuildInfo.Author;
        public override string Version => BuildInfo.Version;
        public override string Link => BuildInfo.Link;

        [AutoRegisterConfigKey]
        private static readonly ModConfigurationKey<bool> Key_Enable = new("enabled", "Enables this mod.", () => true);

        [AutoRegisterConfigKey]
        private static ModConfigurationKey<bool> Key_ExposeEverywhere = new("Show Everywhare", "Enables this texts to User in WorldSpace. That makes show everywhare.", () => false);

        private static ModConfiguration Config;

        static HashSet<string> HarmonyIds = new HashSet<string>();
        public override void OnEngineInit()
        {
            Config = GetConfiguration();
            Config.Save(true);
            if (!Config.GetValue(Key_Enable)) return;

            foreach (MethodBase method in Harmony.GetAllPatchedMethods())
            {
                foreach (string owner in Harmony.GetPatchInfo(method).Owners)
                {
                    HarmonyIds.Add(owner);
                }
            }
            Harmony harmony = new Harmony(BuildInfo.GUID);
            harmony.PatchAll();

        }

        [HarmonyPatch]
        class PatchEveryWhare
        {
            [HarmonyPostfix]
            [HarmonyPatch(typeof(Userspace), "OnAttach")]
            static void UserspaceOnAttachPostfix(Userspace __instance)
            {
                GenList(__instance.World.RootSlot);
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
