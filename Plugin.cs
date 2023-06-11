using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Logging;
using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Networking;

namespace CharacterAnnouncer;

[BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
[HarmonyPatch]

public class Plugin : BaseUnityPlugin
{
	private static Dictionary<String, AudioClip> clips = new();
	private static AudioSource newsource = null;
	private static List<string> char_sounds = new() { "appaloosa", "beezerly", "kaizyle", "trixiebell", "meldor", "jermajesty", "hornlord", "soda", "polygon_char", "servant" };
	private static List<string> tromb_sounds = new() { "brass", "silver", "red", "blue", "green", "pink", "polygon", "champ" };
	private static List<string> sfx_sounds = new() { "trombone", "bass_trombone", "muted", "8-bit", "in_the_club", "gassy"};
	private static List<string> ui_sounds = new() { "choose" };
	private static int countdown = 3;
	private static string folderPath;
	internal static ManualLogSource Log;

	private void Awake()
	{
		folderPath = Path.Combine(Path.GetDirectoryName(this.Info.Location), "AnnouncerLines");

		Log = Logger;
		new Harmony(PluginInfo.PLUGIN_GUID).PatchAll();
	}

	private static void GetClips(string subfolder, List<string> files)
	{
		foreach (string s in files)
		{
			var dapath = Path.Combine(folderPath, subfolder, $"{s}.ogg");
			string censorpath = "";
			using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(dapath, AudioType.OGGVORBIS))
			{
				List<String> pathelements = dapath.Split(Path.DirectorySeparatorChar).ToList();
				string prev = "";
				int i = 0;
				foreach(string element in pathelements)
				{
					if (element == "plugins" && prev == "BepInEx")
					{
						censorpath = "./" + string.Join("/", pathelements.GetRange(i-1, pathelements.Count() - (i - 1)));
					}
					i++;
					prev = element;
				}
				www.SendWebRequest();

				while (!www.isDone)
				{ }

				if (www.isHttpError || www.isNetworkError)
				{
					Log.LogWarning($"Failed to load {s} from {censorpath}");
					Debug.Log(www.error);
					continue;
				}

				((DownloadHandlerAudioClip)www.downloadHandler).streamAudio = true;

				if (www.isNetworkError)
				{
					Log.LogWarning($"Failed to stream {s} from {censorpath}");
					Debug.Log(www.error);
				}

				else
				{
					AudioClip myClip = ((DownloadHandlerAudioClip)www.downloadHandler).audioClip;
					//Log.LogInfo($"Successfully loaded {s} from {censorpath}");
					clips[s] = myClip;
				}
			}
		}
	}

	[HarmonyPatch(typeof(CharSelectController_new))]
	class LoadSounds
	{
		[HarmonyPatch(nameof(CharSelectController_new.Awake))]
		private static void Postfix(CharSelectController_new __instance)
		{
			//Load Clips
			Log.LogInfo("Scene loaded. Getting AudioClips...");
			GetClips("Characters", char_sounds);
			GetClips("Colors", tromb_sounds);
			GetClips("Soundfonts", sfx_sounds);
			GetClips("User Interface", ui_sounds);
			//Reset Countdown 
			countdown = 3;
		}

	}

	[HarmonyPatch(typeof(CharSelectController_new))]
	class PatchCharacters
	{
		[HarmonyPatch(nameof(CharSelectController_new.chooseChar))]
		private static void Postfix(int puppet_choice, CharSelectController_new __instance)
		{
			Log.LogDebug("Selected character: " + char_sounds[puppet_choice]);
			PlaySound(char_sounds[puppet_choice], __instance);
		}

	}

	[HarmonyPatch(typeof(CharSelectController_new))]
	class PatchTrombones
	{
		[HarmonyPatch(nameof(CharSelectController_new.chooseTromb))]
		private static void Postfix(int tromb_choice, CharSelectController_new __instance)
		{
			Log.LogDebug("Selected Color: " + tromb_sounds[tromb_choice]);
			PlaySound(tromb_sounds[tromb_choice], __instance);
		}

	}

	[HarmonyPatch(typeof(CharSelectController_new))]
	class PatchSoundpacks
	{
		[HarmonyPatch(nameof(CharSelectController_new.chooseSoundPack))]
		private static void Postfix(int sfx_choice, CharSelectController_new __instance)
		{
			Log.LogDebug("Selected Soundfont: " + sfx_sounds[sfx_choice]);
			PlaySound(sfx_sounds[sfx_choice], __instance);
		}

	}
	private static void PlaySound(string name, CharSelectController_new __instance)
	{
		AudioClip clip;
		if (newsource == null) 
		{
			AudioSource refsource = __instance.all_sfx[0];
			AudioMixerGroup mixgroup = refsource.outputAudioMixerGroup;
			newsource = refsource.gameObject.AddComponent(typeof(AudioSource)) as AudioSource;
			newsource.outputAudioMixerGroup = mixgroup;
		}

		if (countdown == 3)
		{
			countdown += -1;
			clips.TryGetValue("choose", out clip);
			newsource.PlayOneShot(clip);
			return;
		}
		
		else if (countdown > 0)
		{
			countdown += -1;
			return;
		}

		clips.TryGetValue(name, out clip);
		newsource.PlayOneShot(clip);
	}
}
