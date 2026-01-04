using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static WT.Main;

namespace WT
{
    internal class UEThings
    {
        public static void Inspect(object go)
        {
            switch (isUnityExplorerPresent)
            {
                case UEVersion.STBlade:
                    //InspectST(go);
                    break;
                case UEVersion.Digitalzombie:
                    InspectDZ(go);
                    break;
                case UEVersion.yukieiji:
                    //InspectYJ(go);
                    break;
            }
        }
        //public static void InspectST(object go) => UEST.InspectorManager.Inspect(go);
        public static void InspectDZ(object go) => UEDZ.InspectorManager.Inspect(go);
        //public static void InspectYJ(object go) => UEYJ.InspectorManager.Inspect(go);


        public static KeyCode UEGetDefaultKey()
        {
            switch (isUnityExplorerPresent)
            {
                case UEVersion.STBlade:
                    //return UEGetDefaultKeyST();
                case UEVersion.Digitalzombie:
                    return UEGetDefaultKeyDZ();
                case UEVersion.yukieiji:
                    //return UEGetDefaultKeyYJ();
                default:
                    return KeyCode.F7;
            }
        }


        public static void UEHideOnStartup()
        {
            switch (isUnityExplorerPresent)
            {
                case UEVersion.STBlade:
                    //UEHideOnStartupST();
                    break;
                case UEVersion.Digitalzombie:
                    UEHideOnStartupDZ();
                    break;
                case UEVersion.yukieiji:
                    //UEHideOnStartupYJ();
                    break;
            }
        }

        public static void UERestoreSettings()
        {
            switch (isUnityExplorerPresent)
            {
                case UEVersion.STBlade:
                    //UERestoreSettingsST();
                    break;
                case UEVersion.Digitalzombie:
                    UERestoreSettingsDZ();
                    break;
                case UEVersion.yukieiji:
                    //UERestoreSettingsYJ();
                    break;
            }
        }

        public static bool UEIsUIEnabled()
        {
            switch (isUnityExplorerPresent)
            {
                case UEVersion.STBlade:
                    //return UEIsUIEnabledST();
                    break;
                case UEVersion.Digitalzombie:
                    return UEIsUIEnabledDZ();
                case UEVersion.yukieiji:
                    //return UEIsUIEnabledYJ();
                    break;
            }
            return false;
        }


        //public static void UEHideOnStartupST() => UEST.Config.ConfigManager.Hide_On_Startup.Value = true;
        public static void UEHideOnStartupDZ() => UEDZ.Config.ConfigManager.Hide_On_Startup.Value = true;
        //public static void UEHideOnStartupYJ() => UEYJ.Config.ConfigManager.Hide_On_Startup.Value = true;

        //public static KeyCode UEGetDefaultKeyST() => UEST.Config.ConfigManager.Master_Toggle.Value;
        public static KeyCode UEGetDefaultKeyDZ() => UEDZ.Config.ConfigManager.Master_Toggle.Value;
        //public static KeyCode UEGetDefaultKeyYJ() => UEYJ.Config.ConfigManager.Master_Toggle.Value;

        /*
        public static void UERestoreSettingsST()
        {
            foreach (UEST.UI.UIManager.Panels e in Enum.GetValues(typeof(UEST.UI.UIManager.Panels)))
            {
                UEST.UI.UIManager.GetPanel(e).ApplySaveData();
            }
        }
        */
        public static void UERestoreSettingsDZ()
        {
            foreach (UEDZ.UI.UIManager.Panels e in Enum.GetValues(typeof(UEDZ.UI.UIManager.Panels)))
            {
                UEDZ.UI.UIManager.GetPanel(e).ApplySaveData();
            }
        }
        /*
        public static void UERestoreSettingsYJ()
        {
            foreach (UEYJ.UI.UIManager.Panels e in Enum.GetValues(typeof(UEYJ.UI.UIManager.Panels)))
            {
                UEYJ.UI.UIManager.GetPanel(e).ApplySaveData();
            }
        }
        */


        //public static bool UEIsUIEnabledST() => UEST.UI.UIManager.ShowMenu;
        public static bool UEIsUIEnabledDZ() => UEDZ.UI.UIManager.ShowMenu;
        //public static bool UEIsUIEnabledYJ() => UEYJ.UI.UIManager.ShowMenu;



    }
}
