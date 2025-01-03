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
global using static DT.Utility;
global using UEST = UnityExplorerST::UnityExplorer;
global using UEDZ = UnityExplorerDZ::UnityExplorer;
global using UEYJ = UnityExplorerYJ::UnityExplorer;
using Il2CppVLB;
using Il2CppRewired.HID;

namespace DT
{
    public class DTMain : MelonMod
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

        public bool showInfoHUD;
        public bool forceShowInfoHUD;
        public bool showLookatPoint;

        public static GameObject rayCube;

        public static bool showCursor;
        public static bool showCursorForHUD;
        public static bool HUDControlReset;
        public static PlayerControlMode? prevControlMode;

        public static bool allowCookPlacement = true;
        public static bool allowPhysicsCheck = true;

        public static string HUDPosition;
        public static string HUDLookatName;
        public static Vector3 HUDHitPosition;
        public static GameObject HUDHit;
        public static bool HUDIsPartOfStaticBatch;


        public static int HUDBlockWidth = 160;
        public static int HUDBlockHeight = 140;
        public static int HUDOuterOffset = 10;
        public static int HUDInnerOffset = 5;
        public static int HUDEntryHeight = 20;
        public static int HUDSmallButton = 40;

        public static UEVersion isUnityExplorerPresent = UEVersion.NotPresent;

        public const string unityExplorerAssemblyName = "UnityExplorer";

        public static readonly string modFolderName = "waltzToolz/";
        public static readonly string bundleFolderName = modFolderName + "meshInsert/";

        public static List<AssetBundle> loadedBundles = new List<AssetBundle>();
        public static List<GameObject> loadedMeshes = new List<GameObject>();

        public static bool isPlacingCustom;

        public static int currentCustom;

        public static GameManager gm;


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
        }


        public override void OnSceneWasLoaded(int level, string scene)
        {
            Application.runInBackground = true;

            Log(CC.Yellow, "Loaded " + scene + " #" + level);

            if (scene == "Boot")
            {
                if (isUnityExplorerPresent != UEVersion.NotPresent && Settings.options.hideUE)
                {
                    UEHideOnStartup();
                }
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

        public static void UEHideOnStartup()
        {
            switch (isUnityExplorerPresent)
            {
                case UEVersion.STBlade:
                    UEHideOnStartupST();
                    break;
                case UEVersion.Digitalzombie:
                    UEHideOnStartupDZ();
                    break;
                case UEVersion.yukieiji:
                    UEHideOnStartupYJ();
                    break;
            }
        }

        public static void UEHideOnStartupST()
        {
            UEST.Config.ConfigManager.Hide_On_Startup.Value = true;
        }
        public static void UEHideOnStartupDZ()
        {
            UEDZ.Config.ConfigManager.Hide_On_Startup.Value = true;
        }
        public static void UEHideOnStartupYJ()
        {
            UEYJ.Config.ConfigManager.Hide_On_Startup.Value = true;
            UEYJ.UI.UIManager.GetPanel(UEYJ.UI.UIManager.Panels.Clipboard).ApplySaveData();
        }

        public static bool UEIsUIEnabled()
        {
            switch (isUnityExplorerPresent)
            {
                case UEVersion.STBlade:
                    return UEIsUIEnabledST();
                case UEVersion.Digitalzombie:
                    return UEIsUIEnabledDZ();
                case UEVersion.yukieiji:
                    return UEIsUIEnabledYJ();
            }
            return false;
        }

        public static bool UEIsUIEnabledST() => UEST.UI.UIManager.ShowMenu;
        public static bool UEIsUIEnabledDZ() => UEDZ.UI.UIManager.ShowMenu;
        public static bool UEIsUIEnabledYJ() => UEYJ.UI.UIManager.ShowMenu;

        public static IEnumerator WaitForSaveSlotsAndLoad()
        {
            while (!InterfaceManager.GetPanel<Panel_Sandbox>().isActiveAndEnabled)
            {
                InterfaceManager.GetPanel<Panel_MainMenu>().OnSandbox();
                yield return new WaitForEndOfFrame();
            }
            InterfaceManager.GetPanel<Panel_Sandbox>().OnClickResume();
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

        //TESTmain.staicaseAsset2 = TESTmain.loadBundle1.LoadAsset<GameObject>("staircase2");
        // TESTmain.testingPlatformAsset = TESTmain.loadBundle1.LoadAsset<GameObject>("testing");



        // TESTmain.staircase2 = GameObject.Instantiate(TESTmain.staicaseAsset2);
        //  TESTmain.testingPlatform = GameObject.Instantiate(TESTmain.testingPlatformAsset);

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


        public static void Inspect(object go)
        {
            switch (isUnityExplorerPresent)
            {
                case UEVersion.STBlade:
                    InspectST(go);
                    break;
                case UEVersion.Digitalzombie:
                    InspectDZ(go); 
                    break;
                case UEVersion.yukieiji:
                    InspectYJ(go);
                    break;
            }
        }
        public static void InspectST(object go) => UEST.InspectorManager.Inspect(go);
        public static void InspectDZ(object go) => UEDZ.InspectorManager.Inspect(go);
        public static void InspectYJ(object go) => UEYJ.InspectorManager.Inspect(go);


        public static void DrawPoint(bool draw, Vector3 pos, float width = 1f)
        {
            if (!draw)
            {
                if (rayCube) GameObject.Destroy(rayCube);
                return;
            }

            if (!rayCube)
            {
                LineRenderer line;
                rayCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                rayCube.name = "WaltzToolsHelper";
                GameObject.Destroy(rayCube.GetComponent<Collider>());
                GameObject.Destroy(rayCube.GetComponent<Renderer>());

                rayCube.transform.localScale = new Vector3(1f, 1f, 1f);

                line = rayCube.AddComponent<LineRenderer>();
                line.material = new Material(Shader.Find("Sprites/Default"));
                line.material.renderQueue = 2999;
                line.startWidth = 0f;
                line.endWidth = 0.035f;
                
                Color c1 = new Color(1.1f, 0.7f, 0.25f);
                Color c2 = new Color(0f, 0f, 0f);
                Gradient gradient = new Gradient();
                gradient.SetKeys(
                    new GradientColorKey[] { new GradientColorKey(c1, 0.0f), new GradientColorKey(c1, 1.0f) },
                    new GradientAlphaKey[] { new GradientAlphaKey(0f, 0.0f), new GradientAlphaKey(1f, 0.5f), new GradientAlphaKey(1f, 1.0f) }
                );
                line.colorGradient = gradient;

                GameObject point = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                point.name = "Pin";
                GameObject.Destroy(point.GetComponent<Collider>());
                
                point.transform.SetParent(rayCube.transform);
                //point.transform.localPosition = Vector3.up * 0.5f;
                //if (point.GetComponent<MeshRenderer>)point.AddComponent<MeshRenderer>();
                point.GetComponent<Renderer>().material = new Material(Shader.Find("Sprites/Default"));
                point.GetComponent<Renderer>().material.color = c1;
                point.GetComponent<Renderer>().material.renderQueue = 3999;

                GameObject textObj = new GameObject("Text");


                TextMeshPro text = textObj.AddComponent<TextMeshPro>();
                text.font = GameManager.GetFontManager().m_LatinTMPFont;
                text.color = c1;
                text.fontSize = 10f;
                text.alignment = TextAlignmentOptions.Top;
                text.lineSpacing = -30f;
                //text.fontSize = 50;

                textObj.transform.SetParent(point.transform);
                textObj.GetComponent<Renderer>().material.renderQueue = 4000;

            }
            else
            {
                float mult = Mathf.Pow(width + 1.5f, 1.1f);

                LineRenderer line = rayCube.GetComponent<LineRenderer>();
                line.widthMultiplier = mult;
                //GameObject point = rayCube.
                rayCube.transform.position = pos;
                line.SetPosition(0, rayCube.transform.position);
                line.SetPosition(1, rayCube.transform.position + Vector3.up * 0.1f * mult);
                Transform pin = rayCube.transform.FindChild("Pin");
                pin.localScale = Vector3.one * 0.035f * mult;
                pin.localPosition = Vector3.up * 0.1f * mult;
                TextMeshPro text = pin.GetComponentInChildren<TextMeshPro>();
                text.text = "H: " + (pos.y - GameManager.GetPlayerObject().transform.position.y).ToString("0.00") + "\nD: " + width.ToString("0.00");
                text.m_fontScaleMultiplier = mult;
                text.transform.rotation = Quaternion.LookRotation(text.transform.position - GameManager.GetVpFPSCamera().transform.position);
            }

        }

        public enum HUDe
        { 
            FullLine,
            SplitLeft,
            SplitRight,
            SmallButton,
            BackgroundBox,
            DoubleLineEnd,
        }


        public Rect MakeRect(HUDe type, int num, bool left)
        {
            switch (type)
            {
                default:
                    return new Rect();
                case HUDe.FullLine:
                    return new Rect(left ? HUDInnerOffset : HUDInnerOffset + HUDBlockWidth, HUDEntryHeight * num, HUDBlockWidth - HUDOuterOffset, HUDEntryHeight);
                case HUDe.DoubleLineEnd:
                    return new Rect(left ? HUDInnerOffset : HUDInnerOffset + HUDBlockWidth, HUDEntryHeight * num, HUDBlockWidth - HUDOuterOffset, HUDEntryHeight * 2);
                case HUDe.SplitLeft:
                    return new Rect(left ? HUDInnerOffset : HUDInnerOffset + HUDBlockWidth, HUDEntryHeight * num, (HUDBlockWidth - HUDOuterOffset) / 2 - HUDInnerOffset, HUDEntryHeight);
                case HUDe.SplitRight:
                    return new Rect(left ? (HUDBlockWidth - HUDOuterOffset) / 2 + HUDInnerOffset : HUDBlockWidth + (HUDBlockWidth - HUDOuterOffset) / 2 + HUDInnerOffset, HUDEntryHeight * num, (HUDBlockWidth - HUDOuterOffset) / 2, HUDEntryHeight);
                case HUDe.SmallButton:
                    return new Rect(left ? HUDBlockWidth - HUDSmallButton - HUDInnerOffset : HUDBlockWidth * 2 - HUDSmallButton - HUDInnerOffset, HUDEntryHeight * num, HUDSmallButton, HUDEntryHeight);
                case HUDe.BackgroundBox:
                    return new Rect(left ? 0 : HUDBlockWidth, HUDEntryHeight, HUDBlockWidth, HUDBlockHeight - HUDEntryHeight);
            }

                
        }

        public override void OnGUI()
        {

            if (!showInfoHUD) return;

            if (showInfoHUD && InterfaceManager.DetermineIfOverlayIsActive() && !forceShowInfoHUD)
            {
                showInfoHUD = !showInfoHUD;
                return;
            }

            if (!InterfaceManager.DetermineIfOverlayIsActive()) forceShowInfoHUD = false;

            int groupTopScreenOffset = 0;
            if (isUnityExplorerPresent != UEVersion.NotPresent && UEIsUIEnabled())
            {
                groupTopScreenOffset = 35;
            }
                /* general section

                current scenes?


                */

                /* weather section

                [toggle aurora]
                [freeze time] (add snowflake near time to visualize)
                current time [+][-]
                current weather [clear][snow]
                current wind [+][-]


                */

            if (Input.GetKey(KeyCode.RightAlt))
            {

               
                if (prevControlMode == null) prevControlMode = GameManager.GetPlayerManagerComponent().GetControlMode();
                GameManager.GetPlayerManagerComponent().SetControlMode(PlayerControlMode.Locked);
                showCursorForHUD = true;
                HUDControlReset = true;
                
            }
            else 
            {
                if (HUDControlReset)
                {
                    if (prevControlMode != null) GameManager.GetPlayerManagerComponent().SetControlMode((PlayerControlMode)prevControlMode);
                    showCursorForHUD = false;
                    HUDControlReset = false;
                    prevControlMode = null;
                }

            }


            GUIStyle title = new GUIStyle("box") { fontStyle = FontStyle.Bold };
            GUIStyle alignLeft = new GUIStyle("label") { alignment = TextAnchor.MiddleLeft, fontStyle = FontStyle.Bold };
            GUIStyle alignLeftItalic = new GUIStyle("label") { alignment = TextAnchor.MiddleLeft, fontStyle = FontStyle.Italic };
            //GUIStyle alignLeftLong = new GUIStyle("label") { alignment = TextAnchor.UpperLeft };
            GUIStyle alignRight = new GUIStyle("label") { alignment = TextAnchor.MiddleRight };
            GUIStyle alignMiddle = new GUIStyle("label") { alignment = TextAnchor.UpperCenter };
            GUIStyle alignMiddleColored = new GUIStyle("label") { alignment = TextAnchor.UpperCenter };
            alignMiddleColored.normal.textColor = Color.gray;
            GUIStyle tick = new GUIStyle("toggle") { alignment = TextAnchor.MiddleLeft, fontStyle = FontStyle.Bold };
            GUIStyle inactiveButton = new GUIStyle("button");
            inactiveButton.normal.textColor = Color.gray;
            int entry = 0;
            bool isLeftSide;

            Regex posPattern = new Regex("[()]");
            //string pos;
            //string lookatObject;

            // camera section
            GUI.BeginGroup(new Rect(HUDOuterOffset, HUDOuterOffset + groupTopScreenOffset, HUDBlockWidth*2, HUDBlockHeight));
            GUI.Box(new Rect(0, 0, HUDBlockWidth * 2, HUDEntryHeight), "Camera", title);
            isLeftSide = true;
            GUI.Box(MakeRect(HUDe.BackgroundBox, entry, isLeftSide),"");
            entry = 1;
            GUI.Label(MakeRect(HUDe.SplitLeft, entry, isLeftSide), "Pitch", alignLeft);
            GUI.Label(MakeRect(HUDe.SplitLeft, entry, isLeftSide), GameManager.GetVpFPSCamera()?.m_CurrentPitch.ToString("0.00"), alignRight);
            GUI.Label(MakeRect(HUDe.SplitRight, entry, isLeftSide), "| Yaw", alignLeft);
            GUI.Label(MakeRect(HUDe.SplitRight, entry, isLeftSide), GameManager.GetVpFPSCamera()?.m_CurrentYaw.ToString("0.00"), alignRight);
            entry = 2;
            GUI.Label(MakeRect(HUDe.FullLine, entry, isLeftSide), "Camera position", alignLeft);
            HUDPosition = GameManager.GetVpFPSCamera() ? posPattern.Replace(GameManager.GetVpFPSCamera().transform.position.ToString("0.00"), "") : "Null";
            if (GUI.Button(MakeRect(HUDe.SmallButton, entry, isLeftSide), "Copy"))
            {
                if (GameManager.GetVpFPSCamera())
                {
                    GUIUtility.systemCopyBuffer = posPattern.Replace(GameManager.GetVpFPSCamera().transform.position.ToString(), "");
                }
            }
            entry = 3;
            GUI.Label(MakeRect(HUDe.FullLine, entry, isLeftSide), HUDPosition, alignMiddle);
            entry = 4;
            GUI.Label(MakeRect(HUDe.FullLine, entry, isLeftSide), "Cam mode", alignLeft);
            GUI.Label(MakeRect(HUDe.FullLine, entry, isLeftSide), GameManager.GetPlayerAnimationComponent()?.m_CameraBasedHandPositioningMode.ToString(), alignRight);
            entry = 5;
            GUI.Label(MakeRect(HUDe.FullLine, entry, isLeftSide), "Ctrl mode", alignLeft);
            GUI.Label(MakeRect(HUDe.FullLine, entry, isLeftSide), GameManager.GetPlayerManagerComponent().GetControlMode().ToString(), alignRight);
            entry = 6;
            GUI.Label(MakeRect(HUDe.FullLine, entry, isLeftSide), "Arms FOV", alignLeft);
            GUI.Label(MakeRect(HUDe.FullLine, entry, isLeftSide), GameManager.GetVpFPSCamera()?.m_WeaponCamera?.GetComponent<Camera>().fieldOfView.ToString("0.00"), alignRight);



            isLeftSide = false;
            GUI.Box(MakeRect(HUDe.BackgroundBox, entry, isLeftSide), "");
            entry = 1;
            if (GUI.Button(MakeRect(HUDe.FullLine, entry, isLeftSide), "Toggle look-at point"))
            {
                showLookatPoint = !showLookatPoint;
            }
            entry = 2;
            GUI.Label(MakeRect(HUDe.FullLine, entry, isLeftSide), "Look-at position", alignLeft);
            if (!GameManager.m_IsPaused && !showCursorForHUD)
            {
                HUDPosition = "Null";
                HUDLookatName = "Null";
            }
            if (GameManager.GetMainCamera() && !GameManager.m_IsPaused && !showCursorForHUD)
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
                    if (showLookatPoint)
                    {
                        DrawPoint(true, hit.point, Vector3.Distance(hit.point, GameManager.GetPlayerObject().transform.position));
                    }
                    else
                    {
                        DrawPoint(false, Vector3.zero);
                    }
                    HUDPosition = GameManager.GetVpFPSCamera() ? posPattern.Replace(hit.point.ToString("0.00"), "") : "Null";
                    HUDLookatName = hit.transform.gameObject.name;
                    HUDHit = hit.transform.gameObject;
                    HUDHitPosition = hit.point;

                    int s = 0;
                    Renderer[] renderers = hit.transform.GetComponentsInChildren<Renderer>().ToArray();

                    foreach (Renderer r in renderers)
                    {
                        if (r && r.isPartOfStaticBatch)
                        {
                            s++;
                        }
                    }
                    if (s == renderers.Length)
                    {
                        HUDIsPartOfStaticBatch = true;
                    }
                    else
                    {
                        HUDIsPartOfStaticBatch = false;
                    }

                }
                else
                {
                    HUDPosition = "Null";
                    HUDLookatName = "Null";
                    HUDIsPartOfStaticBatch = false;
                }
            }
            if (GUI.Button(MakeRect(HUDe.SmallButton, entry, isLeftSide), "Copy"))
            {
                GUIUtility.systemCopyBuffer = posPattern.Replace(HUDHitPosition.ToString(), "");
            }
            entry = 3;
            GUI.Label(MakeRect(HUDe.FullLine, entry, isLeftSide), HUDPosition, alignMiddle);


            entry = 4;
            GUI.Label(MakeRect(HUDe.FullLine, entry, isLeftSide), "Look-at object name", alignLeft);
            entry = 5;
            GUI.Label(MakeRect(HUDe.DoubleLineEnd, entry, isLeftSide), HUDLookatName, HUDIsPartOfStaticBatch ? alignMiddleColored : alignMiddle);

            GUI.EndGroup();






            GUI.BeginGroup(new Rect(HUDOuterOffset * 2 + HUDBlockWidth * 2, HUDOuterOffset + groupTopScreenOffset, HUDBlockWidth * 2, HUDBlockHeight));
            // weather section left
            GUI.Box(new Rect(0, 0, HUDBlockWidth * 2, HUDEntryHeight), "Environment", title);
            isLeftSide = true;
            GUI.Box(MakeRect(HUDe.BackgroundBox, entry, isLeftSide), "");
            entry = 1;
            GUI.Label(MakeRect(HUDe.FullLine, entry, isLeftSide), "Current time", alignLeft);
            GUI.Label(MakeRect(HUDe.FullLine, entry, isLeftSide), GameManager.GetTimeOfDayComponent().FormatTime(GameManager.GetTimeOfDayComponent().GetHour(), GameManager.GetTimeOfDayComponent().GetMinutes()), alignRight);
            entry = 2;
            GameManager.GetTimeOfDayComponent().SetNormalizedTime(GUI.HorizontalSlider(MakeRect(HUDe.FullLine, entry, isLeftSide), GameManager.GetTimeOfDayComponent().GetNormalizedTime(), 0f, 1f));



            entry = 5;
            Container c = HUDHit.GetComponentInChildren<Container>();
            if (c)
            {
                if (GUI.Button(MakeRect(HUDe.FullLine, entry, isLeftSide), "Copy LootTable"))
                {
                    if (c.m_LootTableData)
                    {
                        string lt = "loottable=" + c.m_LootTableData?.name.Replace("LootTable", "");
                        Dictionary<string, int> weights = new();
                        {
                            foreach (Il2CppTLD.Gameplay.RandomTableDataEntry<AssetReferenceGearItem> argi in c.m_LootTableData.m_BaseEntries)
                            {
                                lt += $"\nitem={argi.m_Item.GetOrLoadAsset().name} w={argi.m_Weight}";
                            }
                            MelonLogger.Msg(lt);
                            GUIUtility.systemCopyBuffer = lt;
                            HUDMessage.AddMessage($"Copied to clipboard and sent to console: {c.m_LootTableData?.name.Replace("LootTable", "")}", false, true);
                        }

                    }
                    else
                    {
                        HUDMessage.AddMessage("No Loot Table found", false, true);
                    }
                }
            }
            else
            {
                GUI.Button(MakeRect(HUDe.FullLine, entry, isLeftSide), "Copy LootTable", inactiveButton);
            }
            entry = 6;
            if (c)
            {
                if (GUI.Button(MakeRect(HUDe.FullLine, entry, isLeftSide), "Repopulate container"))
                {
                    c.m_Inspected = false;
                    c.m_NotPopulated = true;
                    c.PopulateContents();
                }
            }
            else
            {
                GUI.Button(MakeRect(HUDe.FullLine, entry, isLeftSide), "Repopulate container", inactiveButton);
            }

            // weather section right
            isLeftSide = false;
            GUI.Box(MakeRect(HUDe.BackgroundBox, entry, isLeftSide), "");
            entry = 1;
            GUI.Toggle(MakeRect(HUDe.FullLine, entry, isLeftSide), GameManager.GetWeatherComponent().IsIndoorEnvironment(), " Indoor environment", tick);
            entry = 2;
            GUI.Toggle(MakeRect(HUDe.FullLine, entry, isLeftSide), GameManager.GetWeatherComponent().IsIndoorScene(), " Indoor scene", tick);
            
            
            




            /*
             * Il2Cpp.GameManager.GetWeatherTransitionComponent().m_CurrentWeatherSet.ForceTransitionToStep(1);

    Log("Set Name: " + Il2Cpp.GameManager.GetWeatherTransitionComponent().m_CurrentWeatherSet.name);            

Log("Weather Name: " + Il2Cpp.GameManager.GetWeatherTransitionComponent().m_CurrentWeatherSet.m_WeatherStages[Il2Cpp.GameManager.GetWeatherTransitionComponent().m_CurrentWeatherSet.m_CurrentIndex].m_WeatherType
);

Log("Set #: " + Il2Cpp.GameManager.GetWeatherTransitionComponent().m_CurrentWeatherSet.m_WeatherStages.Length
);

*/


            GUI.EndGroup();

            
            if (isUnityExplorerPresent != UEVersion.NotPresent)
            {
                
                GUI.BeginGroup(new Rect(HUDOuterOffset * 3 + HUDBlockWidth * 4, HUDOuterOffset + groupTopScreenOffset, HUDBlockWidth * 2, HUDBlockHeight));
                GUI.Box(new Rect(0, 0, HUDBlockWidth * 2, HUDEntryHeight), "Inspect in UE", title);
                // inspect section left
                isLeftSide = true;
                GUI.Box(MakeRect(HUDe.BackgroundBox, entry, isLeftSide), "");
                entry = 1;
                if (GUI.Button(MakeRect(HUDe.FullLine, entry, isLeftSide), "vp_FPSPlayer"))
                {
                    Inspect(GameManager.GetVpFPSPlayer());
                }
                entry = 2;
                if (GUI.Button(MakeRect(HUDe.FullLine, entry, isLeftSide), "vp_FPSCamera"))
                {
                    Inspect(GameManager.GetVpFPSCamera());
                }
                entry = 3;
                if (GUI.Button(MakeRect(HUDe.FullLine, entry, isLeftSide), "WeaponCamera"))
                {
                    Inspect(GameManager.GetWeaponCamera());
                }
                entry = 4;
                if (GUI.Button(MakeRect(HUDe.FullLine, entry, isLeftSide), "PlayerManager"))
                {
                    Inspect(GameManager.GetPlayerManagerComponent());
                }
                entry = 5;
                if (GUI.Button(MakeRect(HUDe.FullLine, entry, isLeftSide), "FPHands"))
                {
                    Inspect(GameManager.GetPlayerAnimationComponent().transform.parent);
                }
                entry = 6;
                /*
                if (GameManager.GetPlayerManagerComponent().m_Inspect)
                {
                    if (GUI.Button(MakeRect(HUDe.FullLine, entry, isLeftSide), "Currently inspected"))
                    {
                        Inspect(GameManager.GetPlayerManagerComponent().m_Inspect);
                    }
                }
                else
                {
                    GUI.Button(MakeRect(HUDe.FullLine, entry, isLeftSide), "Currently inspected", inactiveButton);
                }
                */



                // inspect section right
                isLeftSide = false;
                GUI.Box(MakeRect(HUDe.BackgroundBox, entry, isLeftSide), "");
                entry = 1;
                if (GUI.Button(MakeRect(HUDe.FullLine, entry, isLeftSide), "GameManager"))
                {
                    Inspect(gm);
                }
                entry = 2;
                if (GUI.Button(MakeRect(HUDe.FullLine, entry, isLeftSide), "Inventory"))
                {
                    Inspect(GameManager.GetInventoryComponent());
                }
                entry = 3;
                if (GUI.Button(MakeRect(HUDe.FullLine, entry, isLeftSide), "InterfaceManager"))
                {
                    Inspect(InterfaceManager.GetInstance());
                }
                entry = 4;
                if (GUI.Button(MakeRect(HUDe.FullLine, entry, isLeftSide), "WeatherComponent"))
                {
                    Inspect(GameManager.GetWeatherComponent());
                }
                entry = 5;

                entry = 6;
                //GUI.Box(new Rect(160, 20, 160, 200), "");

                GUI.EndGroup();
            }
            

            GUI.Label(new Rect(HUDOuterOffset, HUDBlockHeight + HUDOuterOffset + groupTopScreenOffset, HUDBlockWidth * 2, HUDEntryHeight), "Hold Right Alt to enable mouse", alignLeftItalic);


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

        public override void OnUpdate()
        {
            if (InputManager.GetKeyDown(InputManager.m_CurrentContext, Settings.options.infoHUD))
            {
                showInfoHUD = !showInfoHUD;
                if (InterfaceManager.DetermineIfOverlayIsActive() && showInfoHUD) forceShowInfoHUD = true;
                 
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
                ConsoleManager.CONSOLE_ghost();
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



            if (InputManager.GetKeyDown(InputManager.m_CurrentContext, KeyCode.F7))
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
            }
        }
    }
}




