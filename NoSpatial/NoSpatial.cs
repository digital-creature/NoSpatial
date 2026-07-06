using Awwdio;
using HarmonyLib;
using ResoniteModLoader;
using System;
using System.Reflection;

namespace NoSpatial;
//More info on creating mods can be found https://github.com/resonite-modding-group/ResoniteModLoader/wiki/Creating-Mods
public partial class NoSpatial : ResoniteMod {
	internal const string VERSION_CONSTANT = "1.0.0"; //Changing the version here updates it in all locations needed
	public override string Name => "NoSpatial";
	public override string Author => "digital-creature";
	public override string Version => VERSION_CONSTANT;
	public override string Link => "https://github.com/resonite-modding-group/ExampleMod/";

	private static ModConfiguration Config;

	public override void OnEngineInit() {
		Harmony harmony = new("bleat.goat.NoSpatial");
        Config = GetConfiguration();
        Config?.Save(true);
		harmony.PatchAll();
	}

	[HarmonyPatch]
	class NoSpatial_DisableSpatial_Patch {
		static readonly Type AudioContextReflection = AccessTools.TypeByName("AudioOutputListenerContext");
		static readonly PropertyInfo OutputProperty = AccessTools.Property(AudioContextReflection, "Output");
		static readonly MethodInfo SpatializeSetter = AccessTools.PropertySetter(typeof(AudioOutput), "Spatialize");
		static readonly Action<AudioOutput, bool> SetSpatialization =
			AccessTools.MethodDelegate<Action<AudioOutput, bool>>(SpatializeSetter);

		public static MethodBase TargetMethod()
		{
			return AccessTools.FirstMethod(AudioContextReflection, method => method.Name.Contains("ConsumeProcessAndMix"));
		}

		public static void Prefix(object __instance)
		{
			if(!Config.GetValue(Enabled)) return; // No need to do any work here if the effect is disabled.

			var output = (AudioOutput)OutputProperty.GetValue(__instance);
			if(output is null) return;

			SetSpatialization(output, false);
		}
	}
}
