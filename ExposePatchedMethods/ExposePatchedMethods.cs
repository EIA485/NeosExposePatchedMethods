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
		public override string Author => "eia485";
		public override string Version => "1.0.0";
		public override string Link => "https://github.com/EIA485/NeosExposePatchedMethods/";
		static HashSet<string> HarmonyIds = new HashSet<string>();
		public override void OnEngineInit()
		{
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

		[HarmonyPatch(typeof(Userspace), "OnAttach")]
		class ExposePatchedMethodsPatch
		{
			static void Postfix(Userspace __instance)
			{
				Slot list = __instance.World.RootSlot.AddSlot("Loaded mods",false);
				foreach (string harmonyId in HarmonyIds)
				{
					list.AddSlot(harmonyId);
				}

			}

		}
	}
}