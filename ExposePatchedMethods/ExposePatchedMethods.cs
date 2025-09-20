using HarmonyLib;
using ResoniteModLoader;
using FrooxEngine;
using System;
using System.Reflection;
using System.Collections.Generic;

namespace ExposePatchedMethods
{
	public class ExposePatchedMethods : ResoniteMod
	{
		public override string Name => "ExposePatchedMethods";
		public override string Author => "eia485, kazu0617, rampa3, Nytra";
		public override string Version => "5.0.0";
		public override string Link => "https://github.com/Nytra/ResoniteExposePatchedMethods";

		#region config
		//old keys to be read for the new keys
		[AutoRegisterConfigKey]
		static private readonly ModConfigurationKey<bool> Key_UserSpaceModNames = new("Show Names in userspace", "old, this is now internal", () => false, true);
		[AutoRegisterConfigKey]
		static private readonly ModConfigurationKey<bool> Key_EverywhereModNames = new("Show Names Everywhere", "old, this is now internal", () => false, true);



		[AutoRegisterConfigKey]
		static private readonly ModConfigurationKey<bool> Key_HarmonyidsOnStart = new("Harmonyids on start", "Generates the list of Harmonyids when the mod is loaded", () => harmonyidsOnStart);

		[AutoRegisterConfigKey]
		static private readonly ModConfigurationKey<bool> Key_ExposeUserspace = new("Show in userspace", "Enables showing harmony ids under the root of UserSpace.", () => true);

		[AutoRegisterConfigKey]
		static private readonly ModConfigurationKey<bool> Key_ExposeEverywhere = new("Show Everywhere", "Enables showing harmony ids in WorldSpace. They show under your user root", () => false);





		[AutoRegisterConfigKey]
		static private readonly ModConfigurationKey<NameMetadata?> Key_UserSpaceModNamesMeta = new("Show Names in userspace w/ metadata", "Enables showing mod names under the root of UserSpace. also allows you to select what metadata to show", () => updateFromOld ? (Config.GetValue(Key_UserSpaceModNames) ? NameMetadata.None : null) : NameMetadata.Name | NameMetadata.Author | NameMetadata.Version | NameMetadata.Link);
		[AutoRegisterConfigKey]
		static private readonly ModConfigurationKey<NameMetadata?> Key_EverywhereModNamesMeta = new("Show Names Everywhere w/ metadata", "Enables showing mod names in WorldSpace. They show under your user root. also allows you to select what metadata to show", () => updateFromOld ? (Config.GetValue(Key_EverywhereModNames) ? NameMetadata.None : null) : NameMetadata.Name | NameMetadata.Author | NameMetadata.Version | NameMetadata.Link);

		private static ModConfiguration Config;

		static bool harmonyidsOnStart;
		static bool updateFromOld;
		public override void DefineConfiguration(ModConfigurationDefinitionBuilder builder) => builder.Version(new Version(4, 0, 0, 0));

		public override IncompatibleConfigurationHandlingOption HandleIncompatibleConfigurationVersions(Version serializedVersion, Version definedVersion)
		{
			harmonyidsOnStart = serializedVersion > new Version(1, 0, 0, 0);
			updateFromOld = serializedVersion == new Version(3, 0, 0, 0);
			return IncompatibleConfigurationHandlingOption.FORCELOAD;
		}
		#endregion


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
			Name = 1,
			Author = 2,
			Version = 4,
			Link = 8
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
				if (Config.GetValue(Key_ExposeUserspace)) GenList(__instance.World.RootSlot);
				if (Config.GetValue(Key_UserSpaceModNamesMeta).HasValue) GenList(__instance.World.RootSlot, Config.GetValue(Key_UserSpaceModNamesMeta).Value);
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
				if (Config.GetValue(Key_ExposeEverywhere)) GenList(slot);
				if (Config.GetValue(Key_EverywhereModNamesMeta).HasValue) GenList(slot, Config.GetValue(Key_EverywhereModNamesMeta).Value);
			});
		}

		static void GenList(Slot slot, NameMetadata metadata)
		{
			var list = slot.Children.FirstOrDefault(s=>s.Name == "Loaded mod names")
 				?? slot.AddSlot("Loaded mod names", false);
			foreach (ResoniteModBase mod in ModLoader.Mods())
			{
				Slot s = list.AddSlot(mod.Name);
				if (metadata != NameMetadata.None) s.AttachComponent<DynamicVariableSpace>().SpaceName.Value = "modMetaData";
				if (metadata.HasFlag(NameMetadata.Name)) s.CreateVariable("modMetaData/Name", mod.Name);
				if (metadata.HasFlag(NameMetadata.Author)) s.CreateVariable("modMetaData/Author", mod.Author);
				if (metadata.HasFlag(NameMetadata.Version)) s.CreateVariable("modMetaData/Version", mod.Version);
				if (metadata.HasFlag(NameMetadata.Link)) s.CreateVariable("modMetaData/Link", mod.Link);
			}
		}

		static void GenList(Slot slot)
		{
            Slot list = slot.Children.FirstOrDefault(s => s.Name == "Loaded mods") ?? slot.AddSlot("Loaded mods", false);
			foreach (string harmonyId in HarmonyIds) list.AddSlot(harmonyId);
		}
	}
}
