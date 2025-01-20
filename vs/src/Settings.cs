using ModSettings;

namespace WT
{
    internal static class Settings
    {
        public static void OnLoad()
        {
            Settings.options = new WTSettings();
            Settings.options.AddToModSettings("Waltz Toolz");
        }

        public static WTSettings options;
    }

    internal class WTSettings : JsonModSettings
    {
        [Section("Controls")]

        [Name("God")]
        [Description("Toggle god mode + infinite carry weight")]
        public KeyCode godKey = KeyCode.F3;

        [Name("Fly")]
        [Description("Toggle fly + ghost mode")]
        public KeyCode flyKey = KeyCode.F4;

        [Name("Light")]
        [Description("Toggle dev light")]
        public KeyCode lightKey = KeyCode.F5;

        [Name("Light default range")]
        [Description("---")]
        [Slider(10, 100, 10)]
        public int lightrange = 10;

        [Name("More light")]
        [Description("---")]
        public KeyCode moreLightKey = KeyCode.Equals;

        [Name("Less light")]
        [Description("---")]
        public KeyCode lessLightKey = KeyCode.Minus;

        [Name("Info HUD")]
        [Description("---")]
        public KeyCode infoHUD = KeyCode.F6;

        [Name("Inspect")]
        [Description("Open object under crosshair in UnityExplorer\n\nBy default filters out unnecessary layers like Player or FX, hold sprint key to disable filter")]
        public KeyCode inspectKey = KeyCode.U;

        [Name("Teleport to crosshair")]
        [Description("---")]
        public KeyCode gotoKey = KeyCode.None;

        [Name("Load mesh bundles")]
        [Description("Load bundles from WaltzToolz/meshInsert and start placing first loaded mesh. Use , and . to scroll through available meshes")]
        public KeyCode meshKey = KeyCode.Slash;

        [Section("Misc")]
        /*
        [Name("Hide UE on start")]
        [Description("Actually hide Unity Explorer on game start, for real")]
        public bool hideUE = false;
        */
        [Name("Skip menus")]
        [Description("Load last sandbox save when game starts, skipping all the menus")]
        public bool skipMenus = false;

        [Name("Enable console messages")]
        [Description("---")]
        public bool doLog = true;

        /*
        [Name("Load model")]
        [Description("Loads all meshes from all assetbundles from Mods/QML/ folder\n\nStands for Quick Mesh Load")]
        [Slider(1, 5)]
        public KeyCode loadMeshKey = KeyCode.L;
        */


        protected override void OnConfirm()
        {



            base.OnConfirm();
        }
    }
}
