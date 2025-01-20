using Il2CppTLD.Gear;

namespace WT
{
    internal static class Utility
    {

        public static Main.UEVersion IsUEAssemblyPresent()
        {
            MelonAssembly? assembly = MelonAssembly.LoadedAssemblies.FirstOrDefault(obj => obj.Assembly.GetName().Name.Contains("UnityExplorerTLD"));
            //MelonLogger.Msg(assembly);
            //MelonLogger.Msg(assembly?.Assembly.FullName);
            if (assembly == null)
            {
                assembly = MelonAssembly.LoadedAssemblies.FirstOrDefault(obj => obj.Assembly.GetName().Name.Contains("UnityExplorer.TLD"));
                if (assembly == null)
                {
                    assembly = MelonAssembly.LoadedAssemblies.FirstOrDefault(obj => obj.Assembly.GetName().Name.Contains("UnityExplorer"));
                    if (assembly == null)
                    {
                        return Main.UEVersion.NotPresent;
                    }
                    else return Main.UEVersion.yukieiji;
                }
                else return Main.UEVersion.STBlade;
            }
            else return Main.UEVersion.Digitalzombie;
        }

        public static bool IsScenePlayable()
        {
            return !(string.IsNullOrEmpty(GameManager.m_ActiveScene) || GameManager.m_ActiveScene.Contains("MainMenu") || GameManager.m_ActiveScene == "Boot" || GameManager.m_ActiveScene == "Empty");
        }

        public static bool IsScenePlayable(string scene)
        {
            return !(string.IsNullOrEmpty(scene) || scene.Contains("MainMenu") || scene == "Boot" || scene == "Empty");
        }

        public static bool IsMainMenu(string scene)
        {
            return !string.IsNullOrEmpty(scene) && scene.Contains("MainMenu");
        }

        public static void Log(ConsoleColor cc, string text)
        {
            if (Settings.options.doLog) MelonLogger.Msg(cc, text);
        }


        public static void ForceReload(this GunItem gi) => gi.m_RoundsInClip = gi.m_ClipSize;
        public static void ForceRefuel(this KeroseneLampItem kli) => kli.m_CurrentFuelLiters = kli.m_MaxFuel;

        private static string GetPath(Transform transform)
        {
            string path = transform.name;
            while (transform.parent != null)
            {
                transform = transform.parent;
                path = transform.name + "/" + path;
            }
            return path;
        }
    }
}
