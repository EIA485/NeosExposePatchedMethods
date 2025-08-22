using BepInExResoniteShim;
using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Configuration;
using FrooxEngine;
using HarmonyLib;
using System.Collections.Generic;
using System.Reflection;
using BepInEx.NET.Common;

namespace ExposePatchedMethods
{
    [ResonitePlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION, MyPluginInfo.PLUGIN_AUTHORS, MyPluginInfo.PLUGIN_REPOSITORY_URL)]
    [BepInDependency("ResoniteModding.BepInExResoniteShim")]
    public class ExposePatchedMethods : BasePlugin
    {

        static private ConfigEntry<bool> ExposeUserspace, ExposeEverywhere;

        static private ConfigEntry<NameMetadata> UserSpaceModNamesMeta, EverywhereModNamesMeta;

        static private HashSet<string> harmonyIds;
        static public HashSet<string> HarmonyIds
        {
            get
            {
                if (harmonyIds == null)
                {
                    harmonyIds = new HashSet<string>();
                    foreach (MethodBase method in Harmony.GetAllPatchedMethods())
                    {
                        foreach (string owner in Harmony.GetPatchInfo(method).Owners)
                        {
                            HarmonyIds.Add(owner);
                        }
                    }
                }
                return harmonyIds;
            }
        }

        [Flags]
        private enum NameMetadata
        {
            None = 0,
            Guid = 1,
            Version = 2,
            Author = 4,
            Link = 8,
            All = 15
        }

        public override void Load()
        {
            ExposeUserspace = Config.Bind(MyPluginInfo.PLUGIN_NAME, "Show in userspace", true, "Enables showing harmony ids under the root of UserSpace.");
            ExposeEverywhere = Config.Bind(MyPluginInfo.PLUGIN_NAME, "Show Everywhere", false, "Enables showing harmony ids in WorldSpace. They show under your user root");

            UserSpaceModNamesMeta = Config.Bind(MyPluginInfo.PLUGIN_NAME,"Show Names in userspace w/ metadata", NameMetadata.All, "Enables showing mod names under the root of UserSpace. also allows you to select what metadata to show");
            EverywhereModNamesMeta = Config.Bind(MyPluginInfo.PLUGIN_NAME,"Show Names Everywhere w/ metadata", NameMetadata.All, "Enables showing mod names in WorldSpace. They show under your user root. also allows you to select what metadata to show");

            HarmonyInstance.PatchAll();
        }

        [HarmonyPatch]
        class Patches
        {
            [HarmonyPostfix]
            [HarmonyPatch(typeof(Userspace), "OnAttach")]
            static void UserspaceOnAttachPostfix(Userspace __instance)
            {
                if (ExposeUserspace.Value) GenList(__instance.World.RootSlot);
                if (UserSpaceModNamesMeta.Value != NameMetadata.None) GenList(__instance.World.RootSlot, UserSpaceModNamesMeta.Value);
            }

            [HarmonyPostfix]
            [HarmonyPatch(typeof(World), "LocalUser", MethodType.Setter)]
            static void LocalUserSetterPostfix(User value)
            {
                value.World.RunInUpdates(0, () =>
                {
                    LinkRef<UserRoot> userRoot = (LinkRef<UserRoot>)AccessTools.Field(typeof(User), "userRoot").GetValue(value);
                    userRoot.OnTargetChange += OnLocalUserRootChanged;
                    OnLocalUserRootChanged(userRoot);
                });
            }
        }

        static void OnLocalUserRootChanged(SyncRef<UserRoot> userRoot)
        {
            if (userRoot.Target == null) return;
            userRoot.Target.Slot.ChildAdded += Slot_ChildAdded;
        }

        private static void Slot_ChildAdded(Slot slot, Slot child)
        {
            slot.ChildAdded -= Slot_ChildAdded;
            slot.RunInUpdates(1, () =>
            {
                if (ExposeEverywhere.Value) GenList(slot);
                if (EverywhereModNamesMeta.Value != NameMetadata.None) GenList(slot, EverywhereModNamesMeta.Value);
            });
        }

        static void GenList(Slot slot, NameMetadata metadata)
        {
            Slot list = slot.AddSlot("Loaded mod names", false);
            foreach (var plugin in NetChainloader.Instance.Plugins.Values)
            {
                Slot s = list.AddSlot(plugin.Metadata.Name);
                if (metadata != NameMetadata.None) s.AttachComponent<DynamicVariableSpace>().SpaceName.Value = "modMetaData";
                if (metadata.HasFlag(NameMetadata.Version)) s.CreateVariable("modMetaData/Version", plugin.Metadata.Version.ToString());
                if (metadata.HasFlag(NameMetadata.Guid)) s.CreateVariable("modMetaData/GUID", plugin.Metadata.GUID);

                if (plugin.Instance.GetType().GetCustomAttribute(typeof(ResonitePlugin)) is ResonitePlugin resoPlugin)
                {
                    if (metadata.HasFlag(NameMetadata.Author)) s.CreateVariable("modMetaData/Author", resoPlugin.Author);
                    if (metadata.HasFlag(NameMetadata.Link)) s.CreateVariable("modMetaData/Link", resoPlugin.Link);
                }
            }
        }

        static void GenList(Slot slot)
        {
            Slot list = slot.AddSlot("Loaded mods", false);
            foreach (string harmonyId in HarmonyIds) list.AddSlot(harmonyId);
        }
    }
}
