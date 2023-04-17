using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System;

namespace CharacterAnnouncer;

[BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
[HarmonyPatch]

public class CharacterAnnouncer : BaseUnityPlugin
{
	internal static ManualLogSource Log;

	private void Awake()
	{
		Log = Logger;
		new Harmony(PluginInfo.PLUGIN_GUID).PatchAll();
	}

	// I have no idea what I'm doing and it probably shows

	[HarmonyPatch(typeof(CharSelectController))]
	class Patch0
	{
		[HarmonyPatch(nameof(CharSelectController.clickColor0))]
		private static void Postfix(CharSelectController __instance)
		{
			PlaySound(__instance, 0);
		}

	}
	[HarmonyPatch(typeof(CharSelectController))]
	class Patch1
	{
		[HarmonyPatch(nameof(CharSelectController.clickColor1))]
		private static void Postfix(CharSelectController __instance)
		{
			PlaySound(__instance, 1);
		}

	}
	// Don't repeat yourself my ass
	[HarmonyPatch(typeof(CharSelectController))]
	class Patch2
	{
		[HarmonyPatch(nameof(CharSelectController.clickColor2))]
		private static void Postfix(CharSelectController __instance)
		{
			PlaySound(__instance, 2);
		}

	}
	[HarmonyPatch(typeof(CharSelectController))]
	class Patch3
	{
		[HarmonyPatch(nameof(CharSelectController.clickColor3))]
		private static void Postfix(CharSelectController __instance)
		{
			PlaySound(__instance, 3);
		}

	}
	[HarmonyPatch(typeof(CharSelectController))]
	class Patch4
	{
		[HarmonyPatch(nameof(CharSelectController.clickColor4))]
		private static void Postfix(CharSelectController __instance)
		{
			PlaySound(__instance, 4);
		}

	}
	[HarmonyPatch(typeof(CharSelectController))]
	class Patch5
	{
		[HarmonyPatch(nameof(CharSelectController.clickColor5))]
		private static void Postfix(CharSelectController __instance)
		{
			PlaySound(__instance, 5);
		}

	}
	[HarmonyPatch(typeof(CharSelectController))]
	class Patch6
	{
		[HarmonyPatch(nameof(CharSelectController.clickColor6))]
		private static void Postfix(CharSelectController __instance)
		{
			PlaySound(__instance, 6);
		}

	}
	[HarmonyPatch(typeof(CharSelectController))]
	class Patch7
	{
		[HarmonyPatch(nameof(CharSelectController.clickColor7))]
		private static void Postfix(CharSelectController __instance)
		{
			PlaySound(__instance, 7);
		}

	}
	// It's horrendous
	private static void PlaySound(CharSelectController __instance, int index)
	{
		bool[] port = __instance.selectedportrait;
		if (port[0] || port[1])
		{
			__instance.sfx_charnames.Stop();
			__instance.sfx_charnames.clip = __instance.allcharnames[index];
			__instance.sfx_charnames.Play();
		}
	}
}
