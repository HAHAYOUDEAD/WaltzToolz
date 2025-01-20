using Il2Cpp;
using Il2CppTLD.IntBackedUnit;
using Il2CppTLD.Placement;

namespace WT
{
    internal class UnbreakablePatches
    {

        public static Vector3 gravityVector = new Vector3(0, -9.8f, 0);


        [HarmonyPatch(typeof(GameManager), nameof(GameManager.Awake))]
        private static class GetGameManagerForInspection
        {
            internal static void Prefix(ref GameManager __instance)
            {
                Main.gm = __instance;
            }
        }

        /*
        [HarmonyPatch(typeof(UEYJ.UI.Panels.UEPanel), nameof(UEYJ.UI.Panels.UEPanel.SetActive))]
        private static class gffg
        {
            internal static void Postfix(ref UEYJ.UI.Panels.UEPanel __instance, ref bool active)
            {
                MelonLogger.Msg(CC.Red, $"{__instance.PanelType} - {active}");
            }
        }


        [HarmonyPatch(typeof(UEYJ.UI.Panels.UEPanel), nameof(UEYJ.UI.Panels.UEPanel.ApplySaveData), [typeof(string)])]
        private static class tes1
        {
            internal static void Prefix(ref UEYJ.UI.Panels.UEPanel __instance, ref string data)
            {
                MelonLogger.Msg(__instance.PanelType);
                MelonLogger.Msg(data);
            }
        }
        */
        
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
                if (Main.showCursor || Overlay.showCursorForHUD)
                {
                    __result = true;
                }

            }
        }

        [HarmonyPatch(typeof(PlayerManager), nameof(PlayerManager.GetGearPlacePoint))]
        private static class DisableCookingPlacementPoints
        {
            internal static bool Prefix()
            {
                return Main.allowCookPlacement;
            }
        }

        [HarmonyPatch(typeof(PlayerManager), nameof(PlayerManager.ExitMeshPlacement))]
        private static class EndPlacement
        {
            internal static void Postfix()
            {
                if (Main.isPlacingCustom) Main.isPlacingCustom = false;
            }
        }

        [HarmonyPatch(typeof(PlayerManager), nameof(PlayerManager.CancelPlaceMesh))]
        private static class CancelPlacement
        {
            internal static void Postfix(ref PlayerManager __instance)
            {
                if (Main.isPlacingCustom)
                {
                    Main.isPlacingCustom = false;
                    GameObject.Destroy(__instance.m_ObjectToPlace);  
                }
            }
        }

        [HarmonyPatch(typeof(FlyMode), nameof(FlyMode.Enter))]
        private static class GhostWhenFlyOn
        {
            internal static void Postfix()
            {
                GameManager.GetPlayerManagerComponent().m_Ghost = true;
                GameManager.m_vpFPSPlayer.Controller.m_FallSpeed = 0;
                GameManager.m_vpFPSPlayer.Controller.PhysicsGravityModifier = 0;
            }
        }

        [HarmonyPatch(typeof(FlyMode), nameof(FlyMode.Exit))]
        private static class GhostWhenFlyOff
        {
            internal static void Postfix()
            {
                GameManager.GetPlayerManagerComponent().m_Ghost = false;
                GameManager.m_vpFPSPlayer.Controller.PhysicsGravityModifier = 0.2f;
            }
        }
                
        [HarmonyPatch(typeof(FlyMode), nameof(FlyMode.TeleportPlayerAndExit))]
        private static class GhostWhenFlyOff2
        {
            internal static void Postfix()
            {
                GameManager.GetPlayerManagerComponent().m_Ghost = false;
                GameManager.m_vpFPSPlayer.Controller.PhysicsGravityModifier = 0.2f;
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
 