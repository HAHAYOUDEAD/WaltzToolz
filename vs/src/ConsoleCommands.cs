namespace DT
{
    class ConsoleCommands
    {
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

        public static void CONSOLE_SansaraSetup()
        {
            GearItem clothing;
            PlayerManager pm = GameManager.GetPlayerManagerComponent();
            GameManager.GetInventoryComponent().DestroyAllGear();

            //clothing = pm.AddItemCONSOLE("GEAR_BasicWinterCoat", 1);
            clothing = pm.AddItemCONSOLE("GEAR_WolfSkinCape", 1);
            pm.PutOnClothingItem(clothing, ClothingLayer.Top);
            clothing = pm.AddItemCONSOLE("GEAR_FishermanSweater", 1);
            pm.PutOnClothingItem(clothing, ClothingLayer.Base);
            clothing = pm.AddItemCONSOLE("GEAR_LongUnderwearWool", 1);
            pm.PutOnClothingItem(clothing, ClothingLayer.Base);
            clothing = pm.AddItemCONSOLE("GEAR_CombatBoots", 1);
            pm.PutOnClothingItem(clothing, ClothingLayer.Top);

            pm.AddItemCONSOLE("GEAR_Rifle_Barbs", 1);
            pm.AddItemCONSOLE("GEAR_RifleAmmoBox", 10);
            pm.AddItemCONSOLE("GEAR_RevolverStubNosed", 1);
            pm.AddItemCONSOLE("GEAR_RevolverAmmoBox", 10);
            pm.AddItemCONSOLE("GEAR_Stone", 20);
            pm.AddItemCONSOLE("GEAR_WoodMatches", 2);
            pm.AddItemCONSOLE("GEAR_Accelerator", 5);
            pm.AddItemCONSOLE("GEAR_Firelog", 5);
            pm.AddItemCONSOLE("GEAR_Tinder", 5);
            pm.AddItemCONSOLE("GEAR_Firestriker", 1);
            pm.AddItemCONSOLE("GEAR_KeroseneLamp_Spelunkers", 1);
            pm.AddItemCONSOLE("GEAR_Bow_Manufactured", 1);
            pm.AddItemCONSOLE("GEAR_ArrowManufactured", 12);
            pm.AddItemCONSOLE("GEAR_Prybar", 1);
            pm.AddItemCONSOLE("GEAR_Hatchet", 1);

        }
    }
}
