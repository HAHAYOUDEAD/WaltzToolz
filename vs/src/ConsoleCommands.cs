using Il2CppTLD.Gear;
using Il2CppTLD.Scenes;
using UnityEngine.ResourceManagement.ResourceLocations;

namespace WT
{
    class ConsoleCommands
    {

        public static string loadingSceneSafely = string.Empty;

        [HarmonyPatch(typeof(ConsoleManager), nameof(ConsoleManager.Initialize))]
        private static class AddCommands
        {
            internal static void Postfix()
            {
                if (!uConsole.CommandAlreadyRegistered("weather_clear"))
                {
                    uConsole.RegisterCommand("weather_clear", new Action(CONSOLE_ClearWeather));
                    uConsole.RegisterCommand("weather_lightsnow", new Action(CONSOLE_LightSnowWeather));
                    uConsole.RegisterCommand("sansara_character_reset", new Action(CONSOLE_SansaraSetup));
                    uConsole.RegisterCommand("reload", new Action(CONSOLE_Reload));
                    uConsole.RegisterCommand("load", new Action(CONSOLE_Load));
                    //uConsole.RegisterCommand("scene_safe", new Action(CONSOLE_SafeLoadScene));
                    //uConsole.RegisterCommand("reload_but_save_first", new Action(CONSOLE_Reload));


                }

                foreach (uConsoleCommandParameterSet ccps in uConsoleAutoComplete.m_CommandParameterSets)
                {
                    if (ccps.m_Commands.Contains("scene"))
                    {
                        //ccps.m_Commands.Add("scene_safe");
                        return;
                    }
                }

            }
        }
        
        [HarmonyPatch(typeof(GameManager), nameof(GameManager.LoadSaveGameSlot), [typeof(SaveSlotInfo)] )]
        private static class SafeLoadScene
        {
            internal static void Prefix(ref SaveSlotInfo ssi)
            {
                if (!string.IsNullOrEmpty(loadingSceneSafely))
                {
                    Weather.m_RegionFromSaveSlot = loadingSceneSafely;
                    ssi.m_Region = loadingSceneSafely;
                    loadingSceneSafely = string.Empty;
                }
            }
        }

        public static void CONSOLE_ClearWeather()
        {
            float normalizedTime = 0f;
            if (Utils.TryParseTOD("8", out normalizedTime))
            {
                //MelonLogger.Msg("Set time to: " + normalizedTime);
                GameManager.GetTimeOfDayComponent().SetNormalizedTime(normalizedTime);
            }

            GameManager.GetWindComponent().StartPhaseImmediate(WindDirection.North, WindStrength.Calm);

            GameManager.GetWeatherTransitionComponent().ActivateWeatherSet(WeatherStage.Clear);
            WeatherTransition.m_WeatherTransitionTimeScalar = 1f;
        }

        public static void CONSOLE_LightSnowWeather()
        {
            float normalizedTime = 0f;
            if (Utils.TryParseTOD("8", out normalizedTime))
            {
                //MelonLogger.Msg("Set time to: " + normalizedTime);
                GameManager.GetTimeOfDayComponent().SetNormalizedTime(normalizedTime);
            }

            GameManager.GetWindComponent().StartPhaseImmediate(WindDirection.North, WindStrength.Calm);

            GameManager.GetWeatherTransitionComponent().ActivateWeatherSet(WeatherStage.LightSnow);
            WeatherTransition.m_WeatherTransitionTimeScalar = 1f;
        }

        public static void CONSOLE_Reload()
        {

            if (!InterfaceManager.GetPanel<Panel_PauseMenu>() || !IsScenePlayable(GameManager.m_ActiveScene))
            {
                uConsoleLog.Add("This command only works in Sandbox");
                return;
            }

            //loadingSceneSafely = true;

            MelonCoroutines.Start(SaveThenLoad());
            uConsole.TurnOff();

        }
        
        public static void CONSOLE_SafeLoadScene()
        {

            if (!InterfaceManager.GetPanel<Panel_PauseMenu>() || !IsScenePlayable(GameManager.m_ActiveScene))
            {
                uConsoleLog.Add("This command only works in Sandbox");
                return;
            }
            string name = uConsole.GetString();
            if (string.IsNullOrEmpty(name))
            {
                uConsoleLog.Add("Specify scene name");
                return;
            }

            loadingSceneSafely = name;

            CONSOLE_Reload();
            uConsole.TurnOff();

        }

        public static IEnumerator SaveThenLoad()
        {
            ConsoleManager.CONSOLE_save();
            yield return new WaitForEndOfFrame();
            while (SaveGameSystem.IsAsyncSaveRunning())
            {
                yield return new WaitForEndOfFrame();
            }
            InterfaceManager.GetPanel<Panel_PauseMenu>().DoQuitGame();

            MelonCoroutines.Start(Main.WaitForSaveSlotsAndLoad());
            yield break;
        }

        public static void CONSOLE_Load()
        {
            Panel_PauseMenu ppm = InterfaceManager.GetPanel<Panel_PauseMenu>();

            if (!ppm || !IsScenePlayable(GameManager.m_ActiveScene))
            {
                uConsoleLog.Add("This command only works in Sandbox");
                return;
            }

            uConsole.TurnOff();
            ppm.DoQuitGame();

            MelonCoroutines.Start(Main.WaitForSaveSlotsAndLoad());
        }

           

        public static void CONSOLE_SansaraSetup()
        {
            GearItem clothing;
            GearItem weapon;
            GearItem lantern;
            PlayerManager pm = GameManager.GetPlayerManagerComponent();
            GameManager.GetInventoryComponent().DestroyAllGear();

            pm.AddItemCONSOLE("GEAR_TechnicalBackpack", 1);
            pm.AddItemCONSOLE("GEAR_TechnicalBackpack", 1);
            pm.AddItemCONSOLE("GEAR_TechnicalBackpack", 1);
            pm.AddItemCONSOLE("GEAR_TechnicalBackpack", 1);
            pm.AddItemCONSOLE("GEAR_TechnicalBackpack", 1);
            pm.AddItemCONSOLE("GEAR_TechnicalBackpack", 1);

            clothing = pm.AddItemCONSOLE("GEAR_BasicWinterCoat", 1);
            clothing = pm.AddItemCONSOLE("GEAR_WolfSkinCape", 1);
            pm.PutOnClothingItem(clothing, ClothingLayer.Top);
            clothing = pm.AddItemCONSOLE("GEAR_FishermanSweater", 1);
            pm.PutOnClothingItem(clothing, ClothingLayer.Base);
            clothing = pm.AddItemCONSOLE("GEAR_LongUnderwearWool", 1);
            pm.PutOnClothingItem(clothing, ClothingLayer.Base);
            clothing = pm.AddItemCONSOLE("GEAR_CombatBoots", 1);
            pm.PutOnClothingItem(clothing, ClothingLayer.Top);

            weapon = pm.AddItemCONSOLE("GEAR_Rifle_Barbs", 1);
            weapon.GetComponent<GunItem>().ForceReload();
            pm.AddItemCONSOLE("GEAR_RifleAmmoBox", 10);
            weapon = pm.AddItemCONSOLE("GEAR_RevolverStubNosed", 1);
            weapon.GetComponent<GunItem>().ForceReload();
            pm.AddItemCONSOLE("GEAR_RevolverAmmoBox", 5);
            pm.AddItemCONSOLE("GEAR_Stone", 12);
            pm.AddItemCONSOLE("GEAR_WoodMatches", 2);
            pm.AddItemCONSOLE("GEAR_Accelerant", 5);
            pm.AddItemCONSOLE("GEAR_Firelog", 5);
            pm.AddItemCONSOLE("GEAR_Tinder", 5);
            pm.AddItemCONSOLE("GEAR_Firestriker", 1);
            lantern = pm.AddItemCONSOLE("GEAR_KeroseneLamp_Spelunkers", 1);
            lantern.GetComponent<KeroseneLampItem>().ForceRefuel();
            lantern.GetComponent<KeroseneLampItem>().m_FuelBurnPerHour = new Il2CppTLD.IntBackedUnit.ItemLiquidVolume(0);
            pm.AddItemCONSOLE("GEAR_Bow_Manufactured", 1);
            pm.AddItemCONSOLE("GEAR_ArrowManufactured", 12);
            pm.AddItemCONSOLE("GEAR_Prybar", 1);
            pm.AddItemCONSOLE("GEAR_Hatchet", 1);

        }
    }
}
