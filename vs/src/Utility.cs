namespace DT
{
    internal class Utility
    {

        public static DTMain.UEVersion IsUEAssemblyPresent()
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
                        return DTMain.UEVersion.NotPresent;
                    }
                    else return DTMain.UEVersion.yukieiji;
                }
                else return DTMain.UEVersion.STBlade;
            }
            else return DTMain.UEVersion.Digitalzombie;
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
    }
}
