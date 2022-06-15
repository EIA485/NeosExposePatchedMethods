using HarmonyLib;
using NeosModLoader;
using FrooxEngine;
using System;
using System.Reflection;
using System.Collections.Generic;

namespace ExposePatchedMethods
{
	public class ExposePatchedMethods : NeosMod
	{
		public override string Name => "ExposePatchedMethods";
		public override string Author => "eia485, kazu0617, rampa3";
		public override string Version => "3.0.0";
		public override string Link => "https://github.com/EIA485/NeosExposePatchedMethods";

		#region config
		[AutoRegisterConfigKey]
		static private readonly ModConfigurationKey<bool> Key_HarmonyidsOnStart = new("Harmonyids on start", "Generates the list of Harmonyids when the mod is loaded", () => harmonyidsOnStart);

		[AutoRegisterConfigKey]
		static private readonly ModConfigurationKey<bool> Key_ExposeUserspace = new("Show in userspace", "Enables showing harmony ids under the root of UserSpace.", () => true);

		[AutoRegisterConfigKey]
		static private readonly ModConfigurationKey<bool> Key_ExposeEverywhere = new("Show Everywhere", "Enables showing harmony ids in WorldSpace. They show under your user root", () => false);

		[AutoRegisterConfigKey]
		static private readonly ModConfigurationKey<bool> Key_UserSpaceModNames = new("Show Names in userspace", "Enables showing mod names under the root of UserSpace.", () => false);

		[AutoRegisterConfigKey]
		static private readonly ModConfigurationKey<bool> Key_EverywhereModNames = new("Show Names Everywhere", "Enables showing mod names in WorldSpace. They show under your user root.", () => false);

		private static ModConfiguration Config;

		static bool harmonyidsOnStart;
		public override void DefineConfiguration(ModConfigurationDefinitionBuilder builder) => builder.Version(new Version(3, 0, 0, 0));

		public override IncompatibleConfigurationHandlingOption HandleIncompatibleConfigurationVersions(Version serializedVersion, Version definedVersion)
		{
			harmonyidsOnStart = serializedVersion > new Version(1, 0, 0, 0);
			return IncompatibleConfigurationHandlingOption.FORCE_LOAD;
		}
		#endregion


		static private HashSet<string> harmonyIds;
		static public HashSet<string> HarmonyIds
		{
			get 
			{ 
				if(harmonyIds == null)
				{
					harmonyIds = new HashSet<string>();
					foreach(MethodBase method in Harmony.GetAllPatchedMethods())
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

		public override void OnEngineInit()
		{
			Config = GetConfiguration();

			if (Config.GetValue(Key_HarmonyidsOnStart))
				_ = HarmonyIds;
			Config.Save(true);

			Harmony harmony = new Harmony("net.eia485.ExposePatchedMethods");
			harmony.PatchAll();

		}

		[HarmonyPatch]
		class Patches
		{
			[HarmonyPostfix]
			[HarmonyPatch(typeof(Userspace), "OnAttach")]
			static void UserspaceOnAttachPostfix(Userspace __instance)
			{
				if (Config.GetValue(Key_ExposeUserspace)) GenList(__instance.World.RootSlot, false);
				if (Config.GetValue(Key_UserSpaceModNames)) GenList(__instance.World.RootSlot, true);
			}

			[HarmonyPostfix]
			[HarmonyPatch(typeof(World), "LocalUser", MethodType.Setter)]
			static void LocalUserSetterPostfix(User value)
			{
				value.World.RunInUpdates(0, () => {
					LinkRef<UserRoot> userRoot = (LinkRef<UserRoot>)AccessTools.Field(typeof(User), "userRoot").GetValue(value);
					userRoot.OnTargetChange += OnLocalUserRootChanged;
					OnLocalUserRootChanged(userRoot);
				});
			}
		}
		
		static void OnLocalUserRootChanged(SyncRef<UserRoot> userRoot)
		{
			if(userRoot.Target == null) return;
            userRoot.Target.Slot.ChildAdded += Slot_ChildAdded;
		}

        private static void Slot_ChildAdded(Slot slot, Slot child)
        {
			slot.ChildAdded -= Slot_ChildAdded;
			slot.RunInUpdates(1, () =>
			{
				if (Config.GetValue(Key_ExposeEverywhere)) GenList(slot, false);
				if (Config.GetValue(Key_EverywhereModNames)) GenList(slot, true);
			});
		}

        static void GenList(Slot slot, bool UseNames)
		{
			if (UseNames)
			{
				Slot list = slot.AddSlot("Loaded mod names", false);
				foreach (NeosModBase mod in ModLoader.Mods())
				{
					list.AddSlot(mod.Name);
				}
			}
			else
			{
				Slot list = slot.AddSlot("Loaded mods", false);
				foreach (string harmonyId in HarmonyIds)
				{
					list.AddSlot(harmonyId);
				}
			}
		}
	}
}
