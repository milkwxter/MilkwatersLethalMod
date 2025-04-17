using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using LobbyCompatibility.Attributes;
using LobbyCompatibility.Enums;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace MilkwatersLethalMod
{
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    [BepInDependency("BMX.LobbyCompatibility", BepInDependency.DependencyFlags.HardDependency)]
    [LobbyCompatibility(CompatibilityLevel.Everyone, VersionStrictness.None)]
    [BepInDependency(LethalLib.Plugin.ModGUID)]
    public class MilkwatersLethalMod : BaseUnityPlugin
    {
        public static MilkwatersLethalMod Instance { get; private set; } = null!;
        internal new static ManualLogSource Logger { get; private set; } = null!;
        internal static Harmony? Harmony { get; set; }
        public static AssetBundle MyCustomAssets;

        private void Awake()
        {
            Logger = base.Logger;
            Instance = this;

            Patch();

            Logger.LogInfo($"{MyPluginInfo.PLUGIN_GUID} v{MyPluginInfo.PLUGIN_VERSION} has loaded!");

            string sAssemblyLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            MyCustomAssets = AssetBundle.LoadFromFile(Path.Combine(sAssemblyLocation, "milkwaters_lethal_bundle"));
            if (MyCustomAssets == null)
            {
                Logger.LogError("Failed to load custom assets.");
                return;
            }

            int iRarity = 30;
            Item scrapCrystal = MyCustomAssets.LoadAsset<Item>("Assets/Milkwater/crystal.asset");
            LethalLib.Modules.NetworkPrefabs.RegisterNetworkPrefab(scrapCrystal.spawnPrefab);
            LethalLib.Modules.Items.RegisterScrap(scrapCrystal, iRarity, LethalLib.Modules.Levels.LevelTypes.All);
        }

        internal static void Patch()
        {
            Harmony ??= new Harmony(MyPluginInfo.PLUGIN_GUID);

            Logger.LogDebug("Patching...");

            Harmony.PatchAll();

            Logger.LogDebug("Finished patching!");
        }

        internal static void Unpatch()
        {
            Logger.LogDebug("Unpatching...");

            Harmony?.UnpatchSelf();

            Logger.LogDebug("Finished unpatching!");
        }
    }
}
