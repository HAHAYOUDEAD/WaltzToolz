extern alias UnityExplorerST;
extern alias UnityExplorerDZ;
extern alias UnityExplorerYJ;
global using CC = System.ConsoleColor;
global using System;
global using MelonLoader;
global using HarmonyLib;
global using UnityEngine;
global using System.Reflection;
global using UnityEngine.UI;
global using Il2CppTMPro;
global using System.Collections;
global using System.Collections.Generic;
global using UnityEngine.SceneManagement;
global using Il2Cpp;
global using System.Linq;
global using System.Text.RegularExpressions;
global using static WT.Utility;
global using static WT.UEThings;
global using UEST = UnityExplorerST::UnityExplorer;
global using UEDZ = UnityExplorerDZ::UnityExplorer;
global using UEYJ = UnityExplorerYJ::UnityExplorer;
using Il2CppVLB;
using Il2CppRewired.HID;
using static Il2Cpp.BaseAi;
using System.Text;

namespace WT
{
    public class Main : MelonMod
    {

        public bool isLoaded = false;

        public float lastFrameWeight;

        public bool enableAutoRefill = false;

        public string itemListFileName = "unlimitedItemList.txt";
        public static string modsPath;
        public List<string> itemNames = new List<string>();

        public GameObject lightGameObject;
        public Light lightComp;

        public static Scene currentSandbox;

        public bool isFirstStart = true;



        public static GameObject rayCube;

        public static bool showCursor;

        public static PlayerControlMode? prevControlMode;

        public static bool allowCookPlacement = true;
        public static bool allowPhysicsCheck = true;



        public static UEVersion isUnityExplorerPresent = UEVersion.NotPresent;

        public const string unityExplorerAssemblyName = "UnityExplorer";

        public static readonly string modFolderName = "waltzToolz/";
        public static readonly string bundleFolderName = modFolderName + "meshInsert/";
        public static readonly string commandsFolderName = modFolderName + "commandSets/";

        public static List<AssetBundle> loadedBundles = new List<AssetBundle>();
        public static List<GameObject> loadedMeshes = new List<GameObject>();

        public static bool isPlacingCustom;

        public static int currentCustom;

        public static GameManager gm;

        public static bool UEConfigLoaded;


        public enum UEVersion
        { 
            NotPresent,
            STBlade,
            Digitalzombie,
            yukieiji
        }

        public override void OnInitializeMelon()
        {
            
            modsPath = Path.GetFullPath(typeof(MelonMod).Assembly.Location + "/../../../Mods/");

            isUnityExplorerPresent = IsUEAssemblyPresent();

            Settings.OnLoad();

            Overlay.RefreshCommandSets();
        }


        public override void OnSceneWasLoaded(int level, string scene)
        {
            Application.runInBackground = true;

            Log(CC.Yellow, "Loaded " + scene + " #" + level);

            if (scene == "Boot")
            {
                /*
                if (isUnityExplorerPresent != UEVersion.NotPresent && Settings.options.hideUE)
                {
                    UEHideOnStartup();
                }
                */
            }

            
            if (IsMainMenu(scene))
            {

            }
            


            if (IsScenePlayable(scene))
            {
                if (lightGameObject == null)
                {
                    lightGameObject = new GameObject("devLight");

                    //lightGameObject.transform.Translate(Vector3.down * 100f);
                    lightComp = lightGameObject.AddComponent<Light>();
                    lightGameObject.AddComponent<LightTracking>();
                    //lightGameObject.GetComponent<LightTracking>().m_WasLightingWeaponCamera = true;
                }


                lightComp.enabled = false;

                
            }
        }

        public override void OnSceneWasInitialized(int level, string scene)
        {
            Log(CC.Green, "Initialized " + scene + " #" + level);

            if (IsMainMenu(scene) && isFirstStart)
            {
                if (Settings.options.skipMenus) MelonCoroutines.Start(WaitForSaveSlotsAndLoad());

                isFirstStart = false;
            }


            if (IsScenePlayable())
            {
                isLoaded = true;


                if (scene.EndsWith("_SANDBOX"))
                {
                    currentSandbox = UnityEngine.SceneManagement.SceneManager.GetSceneByName(scene);

                    
                }
            }
        }

        public static IEnumerator WaitForSaveSlotsAndLoad()
        {

            while (InterfaceManager.GetPanel<Panel_Loading>()?.isActiveAndEnabled == true)
            {
                //MelonLogger.Msg("waiting for loadscreen to end");
                yield return new WaitForEndOfFrame();
            }
            while (InterfaceManager.GetPanel<Panel_Sandbox>()?.isActiveAndEnabled != true)
            {
                if (InterfaceManager.GetPanel<Panel_MainMenu>()) InterfaceManager.GetPanel<Panel_MainMenu>().OnSandbox();
                //MelonLogger.Msg("waiting for sandbox");
                yield return new WaitForEndOfFrame();

            }
            while (InterfaceManager.GetPanel<Panel_Loading>()?.isActiveAndEnabled != true)
            {
                if (InterfaceManager.GetPanel<Panel_Confirmation>()?.isActiveAndEnabled == true)
                {
                    InterfaceManager.GetPanel<Panel_Confirmation>().ForceQuit();
                }
                InterfaceManager.GetPanel<Panel_Sandbox>().OnClickResume();
                //MelonLogger.Msg("waiting for loadscreen");
                yield return new WaitForEndOfFrame();
            }
            
            yield break;
        }


        public static bool IsAnyOverlayActive()
        {

            if (InterfaceManager.IsOverlayActiveCached()) return true;

            if (uConsole.m_On) return true;

            if (isUnityExplorerPresent != UEVersion.NotPresent && UEIsUIEnabled()) return true;

            return false;
        }

        public override void OnSceneWasUnloaded(int level, string scene)
        {
            Log(CC.DarkGray, "Unloaded " + scene + " #" + level);

            isLoaded = false;

        }

        public void GetPrevCustomIndex()
        {
            int max = loadedMeshes.Count() - 1;
            if (currentCustom + 1 <= max) currentCustom ++;
            else currentCustom = 0;
        }

        public void GetNextCustomIndex()
        {
            int max = loadedMeshes.Count() - 1;
            if (currentCustom - 1 >= 0) currentCustom --;
            else currentCustom = max;
        }


        public override void OnGUI()
        {
            Overlay.DrawOverlay();
        }
        public override void OnUpdate()
        {
            if (InputManager.GetKeyDown(InputManager.m_CurrentContext, Settings.options.infoHUD))
            {
                Overlay.showInfoHUD = !Overlay.showInfoHUD;
                if (InterfaceManager.DetermineIfOverlayIsActive() && Overlay.showInfoHUD) Overlay.forceShowInfoHUD = true;
                 
            }

            if (!isLoaded) return;

            if (InputManager.GetKeyDown(InputManager.m_CurrentContext, Settings.options.inspectKey) )
            {

                if (isUnityExplorerPresent != UEVersion.NotPresent)
                {
                    if (GameManager.GetPlayerManagerComponent().IsInspectModeActive() && GameManager.GetPlayerManagerComponent().m_Inspect)
                    {
                        Inspect(GameManager.GetPlayerManagerComponent().m_Inspect.gameObject);
                        HUDMessage.m_HUDMessageQueue.Dequeue();
                        HUDMessage.AddMessage("Inspecting: " + GameManager.GetPlayerManagerComponent().m_Inspect.name, true, true);
                        return;
                    }

                    if (IsAnyOverlayActive()) return;

                    Ray ray = GameManager.GetMainCamera().ScreenPointToRay(Input.mousePosition);
                    int layerMask = Physics.AllLayers;

                    if (!InputManager.GetSprintDown(InputManager.m_CurrentContext))
                    {
                        layerMask ^= (1 << vp_Layer.Player);
                        layerMask ^= (1 << vp_Layer.TransparentFX);
                        layerMask ^= (1 << vp_Layer.UI);
                        layerMask ^= (1 << vp_Layer.Weapon);
                        layerMask ^= (1 << vp_Layer.TriggerIgnoreRaycast);
                        layerMask ^= (1 << vp_Layer.TriggerReverb);
                    }

                    //layerMask ^= (1 << layerToExclude);
                    //layerMask |= (1 << layerToInclude);

                    if (Physics.Raycast(ray, out RaycastHit hit, 5000, layerMask))
                    {
                        HUDMessage.m_HUDMessageQueue.Dequeue();
                        HUDMessage.AddMessage("Hit: " + hit.transform.gameObject.name);
                        Inspect(hit.transform.gameObject);
                    }
                    else
                    {
                        HUDMessage.m_HUDMessageQueue.Dequeue();
                        HUDMessage.AddMessage("No hit");
                    }
                }
            }

            if (InputManager.GetKeyDown(InputManager.m_CurrentContext, Settings.options.gotoKey))
            {
                Ray ray = GameManager.GetMainCamera().ScreenPointToRay(Input.mousePosition);
                int layerMask = Physics.AllLayers;

                if (!InputManager.GetSprintDown(InputManager.m_CurrentContext))
                {
                    layerMask ^= (1 << vp_Layer.Player);
                    layerMask ^= (1 << vp_Layer.TransparentFX);
                    layerMask ^= (1 << vp_Layer.UI);
                    layerMask ^= (1 << vp_Layer.Weapon);
                    layerMask ^= (1 << vp_Layer.TriggerIgnoreRaycast);
                    layerMask ^= (1 << vp_Layer.TriggerReverb);
                }

                if (Physics.Raycast(ray, out RaycastHit hit, 5000, layerMask))
                {
                    GameManager.GetPlayerManagerComponent().TeleportPlayer(hit.point + Vector3.up, GameManager.m_vpFPSCamera.transform.rotation);
                }

            }

            if (InputManager.GetKeyDown(InputManager.m_CurrentContext, Settings.options.godKey))
            {

                ConsoleManager.CONSOLE_god();

            }

            if (InputManager.GetKeyDown(InputManager.m_CurrentContext, Settings.options.flyKey))
            {
                ConsoleManager.CONSOLE_fly();
                //ConsoleManager.CONSOLE_ghost();
            }   
            

            if (InputManager.GetKeyDown(InputManager.m_CurrentContext, Settings.options.meshKey))
            {
                loadedMeshes.Clear();
                DirectoryInfo d = new DirectoryInfo(modsPath + bundleFolderName);
                FileInfo[] Files = d.GetFiles();

                foreach (AssetBundle ab in loadedBundles)
                {
                    ab.Unload(true);
                }

                loadedBundles.Clear();

                foreach (FileInfo file in Files)
                {
                    string n = file.Name;
                    AssetBundle ab = AssetBundle.LoadFromFile("Mods/" + bundleFolderName + n);

                    if (ab && !loadedBundles.Contains(ab))
                    {
                        loadedBundles.Add(ab);
                    }
                }

                foreach (AssetBundle ab in loadedBundles)
                {
                    GameObject[] gos = ab.LoadAllAssets<GameObject>();

                    if (gos.Length > 0)
                    {
                        loadedMeshes.AddRange(gos.ToList());
                    }
                }

                if (loadedMeshes.Count > 0)
                {
                    GameManager.GetPlayerManagerComponent().StartPlaceMesh(GameObject.Instantiate(loadedMeshes[0]), PlaceMeshFlags.None);
                    isPlacingCustom = true;
                }
            }

            if (InputManager.GetKeyDown(InputManager.m_CurrentContext, KeyCode.Comma))
            {
                if (!isPlacingCustom) return;
                GetPrevCustomIndex();
                GameObject.Destroy(GameManager.GetPlayerManagerComponent().m_ObjectToPlace);
                GameManager.GetPlayerManagerComponent().m_ObjectToPlace = GameObject.Instantiate(loadedMeshes[currentCustom]);
                GameManager.GetPlayerManagerComponent().m_ObjectToPlace.layer = vp_Layer.InteractivePropNoCollidePlayer;
                //GameManager.GetPlayerManagerComponent().CancelPlaceMesh();
                //GameManager.GetPlayerManagerComponent().StartPlaceMesh(GameObject.Instantiate(loadedMeshes[currentCustom]), PlaceMeshFlags.None);
                HUDMessage.AddMessage("Placing " + loadedMeshes[currentCustom].name);
            }

            if (InputManager.GetKeyDown(InputManager.m_CurrentContext, KeyCode.Period))
            {
                if (!isPlacingCustom) return;
                GetNextCustomIndex();
                GameObject.Destroy(GameManager.GetPlayerManagerComponent().m_ObjectToPlace);
                GameManager.GetPlayerManagerComponent().m_ObjectToPlace = GameObject.Instantiate(loadedMeshes[currentCustom]);
                GameManager.GetPlayerManagerComponent().m_ObjectToPlace.layer = vp_Layer.InteractivePropNoCollidePlayer;
                //GameManager.GetPlayerManagerComponent().CancelPlaceMesh();
                //GameManager.GetPlayerManagerComponent().StartPlaceMesh(GameObject.Instantiate(loadedMeshes[currentCustom]), PlaceMeshFlags.None);
                HUDMessage.AddMessage("Placing " + loadedMeshes[currentCustom].name);
            }

            if (InputManager.GetKeyDown(InputManager.m_CurrentContext, Settings.options.lightKey))
            {
                if (!lightComp.enabled)
                {
                    lightGameObject.GetComponent<LightTracking>().m_WasLightingWeaponCamera = true;
                    lightGameObject.transform.position = GameObject.Find("/CHARACTER_FPSPlayer/WorldView/FPSCamera").transform.position;
                    lightGameObject.transform.SetParent(GameObject.Find("/CHARACTER_FPSPlayer/WorldView/FPSCamera").transform);
                    lightComp.range = Settings.options.lightrange;
                    HUDMessage.m_HUDMessageQueue.Dequeue();
                    HUDMessage.AddMessage("Let there be light");
                }
                else
                {
                    //lightGameObject.transform.Translate(Vector3.down * 100f);
                    HUDMessage.m_HUDMessageQueue.Dequeue();
                    HUDMessage.AddMessage("Let there be dark");
                }

                lightComp.enabled = !lightComp.enabled;
            }

            if (InputManager.GetKeyDown(InputManager.m_CurrentContext, Settings.options.lessLightKey))
            {
                if (!lightComp.enabled) return;
                if (lightComp.range <= 10)
                {
                    lightComp.range = 10;
                }
                else
                {
                    lightComp.range -= 10;
                }
                HUDMessage.m_HUDMessageQueue.Dequeue();
                HUDMessage.AddMessage("Dev light dimmed to " + lightComp.range.ToString(), 0.2f);
            }

            if (InputManager.GetKeyDown(InputManager.m_CurrentContext, Settings.options.moreLightKey))
            {
                if (!lightComp.enabled) return;
                lightComp.range += 10;
                HUDMessage.m_HUDMessageQueue.Dequeue();
                HUDMessage.AddMessage("Dev light brightened to " + lightComp.range.ToString(), 0.2f);
            }

            /*
            if (InputManager.GetKeyDown(InputManager.m_CurrentContext, Settings.options.loadMeshKey))
            {
                
            }
            */



            if (InputManager.GetKeyDown(InputManager.m_CurrentContext, UEGetDefaultKey()))
            {
                if (GameManager.m_IsPaused)
                {
                    GameManager.m_IsPaused = false;
                    showCursor = false;
                }
                
                else
                {
                    GameManager.m_IsPaused = true;
                    showCursor = true;
                }

                //if (!UEConfigLoaded) MelonCoroutines.Start(WaitForUEAndRestoreConfig());
            }
        }
        /*
        public static IEnumerator WaitForUEAndRestoreConfig()
        {
            while (!UEIsUIEnabled())
            {
                MelonLogger.Msg("waiting");
                yield return new WaitForEndOfFrame();
            }
            UERestoreSettings();

            UEConfigLoaded = true;

            yield break;
        }
        */
    }
}




