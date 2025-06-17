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
        private Hook _hook2;

        public void Awake()
        {
            Log.Init(Logger);

            Instance = this;

            var defaultValue = 4;
            PlayerCount = Config.Bind("Gameplay Tweaks", "PlayerCount", defaultValue, "Overrides the game's player count with a fixed value.");
            PlayerCount.SettingChanged += PlayerCountOnSettingChanged;
            ModSettingsManager.AddOption(new IntFieldOption(PlayerCount));

            var allFlags = (BindingFlags)(-1);
            var hookConfig = new HookConfig { ManualApply = true };
            _hook = new Hook(
                typeof(Run).GetProperty(nameof(Run.participatingPlayerCount), allFlags).GetGetMethod(true),
                OverrideParticipatingPlayerCount,
                hookConfig);
            _hook2 = new Hook(
                typeof(Run).GetProperty(nameof(Run.livingPlayerCount), allFlags).GetGetMethod(true),
                OverrideLivingPlayerCount,
                hookConfig);
        }

        private void PlayerCountOnSettingChanged(object sender, EventArgs e) => LogPlayerCount();

        private static int OverrideParticipatingPlayerCount(Func<Run, int> orig, Run self)
        {
            return Instance.PlayerCount.Value;
        }

        private static int OverrideLivingPlayerCount(Func<Run, int> orig, Run self)
        {
            var originalCount = orig(self);
            var myCount = Instance.PlayerCount.Value;
            if (originalCount > 0 && myCount > 0 && myCount < originalCount)
            {
                return myCount;
            }

            return originalCount;
        }

        public void OnEnable()
        {
            _hook?.Apply();
            _hook2?.Apply();
        }

        public void OnDisable()
        {
            _hook?.Undo();
            _hook2?.Undo();
        }

        private static void LogPlayerCount()
        {
            Log.Info("New PlayerCount: " + Instance.PlayerCount.Value);
        }
    }
}