using System;
using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using MonoMod.RuntimeDetour;
using RiskOfOptions;
using RiskOfOptions.Options;
using RoR2;

namespace ForcePlayerCount
{
    [BepInDependency("com.rune580.riskofoptions")]
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    public class ForcePlayerCount : BaseUnityPlugin
    {
        public const string PluginGUID = PluginAuthor + "." + PluginName;
        public const string PluginAuthor = "iDeathHD";
        public const string PluginName = "ForcePlayerCount";
        public const string PluginVersion = "1.0.0";

        public static ForcePlayerCount Instance { get; private set; }
        public ConfigEntry<int> PlayerCount { get; private set; }

        private Hook _hook;

        public void Awake()
        {
            Log.Init(Logger);

            Instance = this;

            var defaultValue = 4;
            PlayerCount = Config.Bind("Gameplay Tweaks", "PlayerCount", defaultValue, "Overrides the game's player count with a fixed value.");
            PlayerCount.SettingChanged += PlayerCountOnSettingChanged;

            var allFlags = (BindingFlags)(-1);
            var hookConfig = new HookConfig { ManualApply = true };
            _hook = new Hook(
                typeof(Run).GetProperty(nameof(Run.participatingPlayerCount), allFlags).GetGetMethod(true),
                OverrideParticipatingPlayerCount,
                hookConfig);
            ModSettingsManager.AddOption(new IntFieldOption(PlayerCount));
        }

        private void PlayerCountOnSettingChanged(object sender, EventArgs e) => LogPlayerCount();

        private static int OverrideParticipatingPlayerCount(Func<Run, int> orig, Run self) => Instance.PlayerCount.Value;

        public void OnEnable()
        {
            _hook?.Apply();
        }

        public void OnDisable()
        {
            _hook?.Undo();
        }

        private static void LogPlayerCount()
        {
            Log.Info("New PlayerCount: " + Instance.PlayerCount.Value);
        }
    }
}