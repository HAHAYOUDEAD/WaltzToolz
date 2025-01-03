using Il2Cpp;
using Il2CppTLD.IntBackedUnit;

namespace DT
{
    internal class UnbreakablePatches
    {

        public static Vector3 gravityVector = new Vector3(0, -9.8f, 0);


        [HarmonyPatch(typeof(GameManager), nameof(GameManager.Awake))]
        private static class GetGameManagerForInspection
        {
            internal static void Prefix(ref GameManager __instance)
            {
                DTMain.gm = __instance;
            }
        }
        
        [HarmonyPatch(typeof(ConsoleManager), nameof(ConsoleManager.CONSOLE_god))]
        private static class ToggleInfiniteCarry
        {
            internal static void Postfix()
            {
                Encumber encumberComp = GameManager.GetEncumberComponent();
                if (GameManager.GetPlayerManagerComponent().m_God)
                {
                    HUDMessage.m_HUDMessageQueue.Dequeue();
                    HUDMessage.AddMessage("God mode activated");

                    float carryAdd = 9999f;

                    encumberComp.m_MaxCarryCapacity = ItemWeight.FromKilograms(30f + carryAdd);
                    encumberComp.m_MaxCarryCapacityWhenExhausted = ItemWeight.FromKilograms(15f + carryAdd);
                    encumberComp.m_NoSprintCarryCapacity = ItemWeight.FromKilograms(40f + carryAdd);
                    encumberComp.m_NoWalkCarryCapacity = ItemWeight.FromKilograms(60f + carryAdd);
                    encumberComp.m_EncumberLowThreshold = ItemWeight.FromKilograms(31f + carryAdd);
                    encumberComp.m_EncumberMedThreshold = ItemWeight.FromKilograms(40f + carryAdd);
                    encumberComp.m_EncumberHighThreshold = ItemWeight.FromKilograms(60f + carryAdd);
                }
                else
                {
                    HUDMessage.m_HUDMessageQueue.Dequeue();
                    HUDMessage.AddMessage("God mode deactivated");

                    encumberComp.m_MaxCarryCapacity = ItemWeight.FromKilograms(30f);
                    encumberComp.m_MaxCarryCapacityWhenExhausted = ItemWeight.FromKilograms(15f);
                    encumberComp.m_NoSprintCarryCapacity = ItemWeight.FromKilograms(40f);
                    encumberComp.m_NoWalkCarryCapacity = ItemWeight.FromKilograms(60f);
                    encumberComp.m_EncumberLowThreshold = ItemWeight.FromKilograms(31f);
                    encumberComp.m_EncumberMedThreshold = ItemWeight.FromKilograms(40f);
                    encumberComp.m_EncumberHighThreshold = ItemWeight.FromKilograms(60f);
                }
            }
        }

        [HarmonyPatch(typeof(InterfaceManager), nameof(InterfaceManager.ShouldEnableMousePointer))]
        private static class ToggleCursor
        {
            internal static void Postfix(ref bool __result)
            {
                if (DTMain.showCursor || DTMain.showCursorForHUD)
                {
                    __result = true;
                }

            }
        }

        [HarmonyPatch(typeof(PlayerManager), "GetGearPlacePoint")]
        private static class DisableCookingPlacementPoints
        {
            internal static bool Prefix()
            {
                return DTMain.allowCookPlacement;
            }
        }

        [HarmonyPatch(typeof(PlayerManager), nameof(PlayerManager.ExitMeshPlacement))]
        private static class EndPlacement
        {
            internal static void Postfix()
            {
                if (DTMain.isPlacingCustom) DTMain.isPlacingCustom = false;
            }
        }

        [HarmonyPatch(typeof(PlayerManager), nameof(PlayerManager.CancelPlaceMesh))]
        private static class CancelPlacement
        {
            internal static void Postfix(ref PlayerManager __instance)
            {
                if (DTMain.isPlacingCustom)
                {
                    DTMain.isPlacingCustom = false;
                    GameObject.Destroy(__instance.m_ObjectToPlace);  
                }
            }
        }



        [HarmonyPatch(typeof(FlyMode), "LateUpdate")]
        private static class DisableGravityWhenInFlyMode
        {
            internal static bool Prefix()
            {
                if (CameraDebugMode.IsFly)
                {
                    
                    if (Physics.gravity != Vector3.zero)
                    {
                        Physics.gravity = Vector3.zero; // doesn't work for whatever reason
                    }
                    
                }
                else
                {
                    if (Physics.gravity == Vector3.zero)
                    {
                        Physics.gravity = gravityVector;
                    }
                }

                

                return true;
            }
        }
        
        /*
        [HarmonyPatch(typeof(Utils), "SetIsKinematic")]
        private static class DisablePhysicsCheck
        {
            internal static bool Prefix(ref Rigidbody rb)
            {
                //rb.isKinematic = true;
                //rb.isKinematic = false;
                //rb.velocity = Vector3.zero;
                //MelonLogger.Msg("SetIsKinematic");
                return DTMain.allowPhysicsCheck;
            }
        }
        */

        /*
        [HarmonyPatch(typeof(PlayerManager), "ExitMeshPlacement")]
        private static class DisablePhysicsCheck2
        {
            internal static void Prefix(PlayerManager __instance)
            {
                if (!DTMain.allowPhysicsCheck)
                {
                    if (__instance.m_ObjectToPlaceGearItem.gameObject.GetComponent<Rigidbody>())
                    {
                        __instance.m_ObjectToPlaceGearItem.gameObject.GetComponent<Rigidbody>().isKinematic = false;
                        __instance.m_ObjectToPlaceGearItem.gameObject.GetComponent<Rigidbody>().interpolation = RigidbodyInterpolation.Interpolate;
                        __instance.m_ObjectToPlaceGearItem.gameObject.GetComponent<Rigidbody>().collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
                        __instance.m_ObjectToPlaceGearItem.gameObject.GetComponent<Rigidbody>().solverIterations = 30;
                    }
                    else
                    {
                        if (__instance.m_ObjectToPlaceGearItem.gameObject.GetComponent<MeshCollider>())
                        {
                            __instance.m_ObjectToPlaceGearItem.gameObject.GetComponent<MeshCollider>().convex = true;
                        }
                        __instance.m_ObjectToPlaceGearItem.gameObject.AddComponent<Rigidbody>();
                        __instance.m_ObjectToPlaceGearItem.gameObject.GetComponent<Rigidbody>().interpolation = RigidbodyInterpolation.Interpolate;
                        __instance.m_ObjectToPlaceGearItem.gameObject.GetComponent<Rigidbody>().collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
                        __instance.m_ObjectToPlaceGearItem.gameObject.GetComponent<Rigidbody>().solverIterations = 30;
                    }
                }



            }

        }
        */

        [HarmonyPatch(typeof(ConsoleManager), "Initialize")]
        private static class AddCommands
        {
            internal static void Postfix()
            {
                uConsole.RegisterCommand("weather_clear", new Action(ConsoleCommands.CONSOLE_ClearWeather));
                uConsole.RegisterCommand("weather_lightsnow", new Action(ConsoleCommands.CONSOLE_LightSnowWeather));
                uConsole.RegisterCommand("sansara_character_reset", new Action(ConsoleCommands.CONSOLE_SansaraSetup));
            }
        }
        /*
        public static bool lostFocus;

        [HarmonyPatch(typeof(GameManager), "OnApplicationFocus")]
        private static class blablatest2
        {
            public static void Postfix(ref bool focusStatus)
            {
                lostFocus = !focusStatus;
                GameManager.m_IsPaused = false;
            }
        }
        */


    }
}
 