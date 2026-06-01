using System;
using System.Collections.Generic;
//using System.IO;
using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
//using Il2CppInterop.Runtime.InteropTypes.Arrays;
//using MoonSharp.Interpreter.Tree.Expressions;
using UnityEngine;
//using UnityEngine.UI;

namespace NoTimeEditorAdditorMod
{

    [BepInPlugin("notime.kastuk.editaddit", "Editor Additor Mod", "2026.06.01")]
    public class EditorAdditorMod : BasePlugin
    {
        public static ManualLogSource InstanceLog;
        public override void Load()
        {
            EditorAdditorMod.InstanceLog = base.Log;
            Harmony harmony = new Harmony("com.notime.editaddit");
            harmony.PatchAll(typeof(EditorAdditorMod.ObjectsEditorMods));
            InstanceLog.LogFatal("!!! Editor Additor LOADED !!!");
            ModConfig.Bind(this);
        }

        

        public static class ModConfig
        {
            public static KeyCode Mod1Key;
            public static KeyCode Mod2Key;
            public static float Mod1;
            public static float Mod2;

            public static KeyCode RotatKey;
            public static KeyCode HeigScalKey;

            public static KeyCode CamKey;

            public static bool WaterLevelFix;
            public static int WLevel;

            public static void Bind(EditorAdditorMod eda)
            {

                ModConfig.Mod1Key = eda.Config.Bind<KeyCode>("Modifiers", "Modifier 1 key", KeyCode.RightControl, "Modifier 1 key").Value;
                ModConfig.Mod1 = eda.Config.Bind<float>("Modifiers", "Modifier 1 value", 0.2f, "Modifier 1 value (usually for tiny adjust)").Value;
                ModConfig.Mod2Key = eda.Config.Bind<KeyCode>("Modifiers", "Modifier 2 key", KeyCode.RightShift, "Modifier 2 key").Value;
                ModConfig.Mod2 = eda.Config.Bind<float>("Modifiers", "Modifier 2 value", 5f, "Modifier 2 value (usually for bigger moves)").Value;
                
                ModConfig.RotatKey = eda.Config.Bind<KeyCode>("Other Controls", "Rotation key", KeyCode.Slash, "Key to hold with arrows pressing for rotating by Y (left-right) and Z (up-down) axis").Value;
                ModConfig.HeigScalKey = eda.Config.Bind<KeyCode>("Other Controls", "Height+Scale key", KeyCode.Space, "Key to hold with arrows pressing for changing height (up-down) and scale (left-right)").Value;
                
                ModConfig.CamKey = eda.Config.Bind<KeyCode>("Other Controls", "Camera key", KeyCode.End, "Key to move camera to highlighted object").Value;

                ModConfig.WaterLevelFix = eda.Config.Bind<bool>("Experimentable", "Water fix switch", false, "Will attempt to adjust water level in editor").Value;
                ModConfig.WLevel = eda.Config.Bind<int>("Experimentable", "Water fix Level", -2, "Added value").Value;
            }
        }


        //TODO

        //MAIN FEATURES
        //DONE*move by holding arrow keys too, but need limitation of moves per second - Time.deltaTime
        //DONE*select object not only by click on mesh, but at name in objects list too!
        //DONE*update inspector values
        //DONE*clean last selected object at deselection to not continue adjustments?

        //GAME FIXES
        //DONE?*water level, adjusting the visual level (-2 m)
        //DONE*remove collider of water surface to let select objects under it

        //DONE* TRY AUTOSCALE EMPTY GAME OBJECT to 1.0 at creation, and use only them for grouping, as autoscaling at world loading is what scale all child objects I suppose (order of applying params to childs after parent?)
        //disable trees (palm, pine) animations?
        //fix scale of nature spawn sprites
        //duplicate: fixing of the not shown copied objects outline HIGHLIGHTER - its just transform location is shifted by axis Y to 100f lower!
        //duplicate: bug with not changing of values of copied object (untill saveload)
        //duplicate: bugged relative scaling at level load (double scaling at own scale values, then at parent scale value, so childs become so tiny)
        //fix scaling of childs related to parent scale at saveload
        //duplicate: wrong placing copies of child under parent (relative to worlds origin instead of ex parent coords)

        //OTHER STUFF
        //scroll by mousewheel in objects window
        //proper reorder of hierarchy of parented objects at load of level - list insert!
        //autoparenting of new placed objects after last parent selected with some switch (key pressed) + autohierarchy in list

        //tiny adjusting of sliders at terrain editor (numerical values shown/enter?)

        //recover camera position after editor load - may place empty gameobject at exit and use it coords at load
        // resort workshop list that CUstom category going up, or just move the scroll to down


        [HarmonyPatch]
        public static class ObjectsEditorMods
        {
            public static GameObject gb;

            private const float timelimit = 0.8f;

            private static float curtime = 0f;

            //public static Transform cam;

            //public static InputField[] allinputfields;
            //public static RectTransform rect;
            //public static Vector3[] corners = new Vector3[4];

            [HarmonyPostfix]
            [HarmonyPatch(typeof(LevelEditor), nameof(LevelEditor.Start))]
            public static void LevEdStartPostfix(ref LevelEditor __instance)
            {
                if (__instance.water != null && ModConfig.WaterLevelFix)
                {
                    //__instance.water.GetComponent<Collider>().enabled = false; //let click other objects under water surface
                    //Adjust visual level of water in editor
                    __instance.water.transform.position = new Vector3(__instance.water.transform.position.x, __instance.water.transform.position.y + ModConfig.WLevel, __instance.water.transform.position.z);
                }

                //allinputfields = __instance.inspector.GetComponentsInChildren<InputField>();//UnityEngine.Object.FindObjectsOfType<InputField>();
                //rect = __instance.inspector.GetComponent<RectTransform>();
                //rect.GetWorldCorners(corners);
                //recover camera position after load (where to save previous position numbers? level description?)
                //__instance.camSpawn.position = new Vector3(x,y,z);
                //__instance.camSpawn.eulerAngles = new Vector3(x,y,z);
            }

            [HarmonyPatch(typeof(LevelEditor), nameof(LevelEditor.OpenObjectCatalogue))]
            [HarmonyPostfix]
            public static void LevEditOpObjCatPostfix(ref LevelEditor __instance)
            {
                __instance.water.GetComponent<Collider>().enabled = false;
            }

            [HarmonyPatch(typeof(LevelEditor), nameof(LevelEditor.CloseObjectCatalogue))]
            [HarmonyPostfix]
            public static void LevEditClsObjCatPostfix(ref LevelEditor __instance)
            {
                __instance.water.GetComponent<Collider>().enabled = true;
                gb = null;
            }


            [HarmonyPostfix]
            [HarmonyPatch(typeof(LevelEditor), nameof(LevelEditor.FindSelectedSpawned))]
            public static void FindSelSpawnPostfix(ref GameObject g)
            {
                if (g != null)
                {
                    //InstanceLog.LogError("Return selected object: " + g.ToString());
                    gb = g; //works only at selection at mesh click, not at list of objects
                }
            }

            [HarmonyPostfix]
            [HarmonyPatch(typeof(LevelEditor), nameof(LevelEditor.SelectSpawned))]
            public static void SelectSpawnPostfix(ref LevelEditor __instance, ref int i)
            {
                    //gb = __instance.catalogueSpawned[i]; //OUTOFRANGE as at oblist
					gb = __instance.catalogue.spawned[i].pref;
                    //InstanceLog.LogError("Return selected object at list index: " + i + ", " + gb.name.ToString());
            }

            

                [HarmonyPatch(typeof(LevelEditor), nameof(LevelEditor.LateUpdate))]
            [HarmonyPostfix]
            public static void LevEditLUpdatePostfix(ref LevelEditor __instance)
            {
                if (__instance.tobeplaced != null && __instance.tobeplaced.name == "_PrefEmpty")
                {
                    __instance.tobeplaced.transform.localScale = new Vector3(1f, 1f, 1f);
                } //make new placed emptygameobject be at stable scale to not jam any children grouped objects under it at saveload


                if (gb == null || !__instance.inspector.active)//) && !Input.anyKeyDown)
                {
                    //InstanceLog.LogError("no selected gameobject");
					//gb = null;
                    return;
                }

                
                //foreach (var input in allinputfields)
                //{
                //    if (input != null && input.isFocused)
                //    {
                //        return;
                //    }
                //}

                //Vector3 mousepos = Input.mousePosition; //nope, it stop all work, as ui rectangle is quite big
                //if (mousepos.x >= corners[0].x && mousepos.x <= corners[2].x && mousepos.y >= corners[0].y && mousepos.y <= corners[2].y)
                //{
                //    return;
               //}

                //GameObject gb = __instance.selectedCatalogueObject.pref; //not works

                curtime += Time.deltaTime;
                if (curtime < timelimit)
                {
                    return;
                }
                //InstanceLog.LogError("gameobject selected: " + gb.ToString());
                Vector3 posit = gb.transform.position;
                Vector3 rotat = gb.transform.eulerAngles;
                Vector3 scale = gb.transform.localScale;
                //InstanceLog.LogError("gameobject position: " + posit.ToString());

                float mod = 1f;

                if (Input.GetKey(ModConfig.Mod1Key))
                {
                    //InstanceLog.LogError("RCtrl pressed");
                    mod = ModConfig.Mod1;
                }
                if (Input.GetKey(ModConfig.Mod2Key))
                {
                    //InstanceLog.LogError("RShift pressed");
                    mod = ModConfig.Mod2;
                }

                if (Input.GetKey(ModConfig.HeigScalKey))
                {
                    //InstanceLog.LogError("RAlt pressed"); //bad key, ALt+Ctrl+arrows is rotation screen
                    if (Input.GetKey(KeyCode.DownArrow))
                    {
                        //Height by Y axis
                        gb.transform.position = posit + new Vector3(0f, -0.02f * mod, 0f);
                    } //todo try to move by hold key too, but need limitation of moves per second
                    if (Input.GetKey(KeyCode.UpArrow))
                    {
                        gb.transform.position = posit + new Vector3(0f, 0.02f * mod, 0f);
                    }
                    //scaling
                    if (Input.GetKey(KeyCode.LeftArrow))
                    {
                        gb.transform.localScale = new Vector3(scale.x - 0.02f * mod, scale.y - 0.02f * mod, scale.z - 0.02f * mod);
                    }
                    if (Input.GetKey(KeyCode.RightArrow))
                    {
                        gb.transform.localScale = new Vector3(scale.x + 0.02f * mod, scale.y + 0.02f * mod, scale.z + 0.02f * mod);
                    }
                }

                else if (Input.GetKey(ModConfig.RotatKey))
                {
                    //Rotating by Y axis
                    if (Input.GetKey(KeyCode.LeftArrow))
                    {
                        gb.transform.eulerAngles = new Vector3(rotat.x, rotat.y - 2f * mod, rotat.z);
                    }
                    if (Input.GetKey(KeyCode.RightArrow))
                    {
                        gb.transform.eulerAngles = new Vector3(rotat.x, rotat.y + 2f * mod, rotat.z);
                    }
					//Rotating by Z axis
					if (Input.GetKey(KeyCode.UpArrow))
                    {
                        gb.transform.eulerAngles = new Vector3(rotat.x, rotat.y, rotat.z + 2f * mod);
                    }
                    if (Input.GetKey(KeyCode.DownArrow))
                    {
                        gb.transform.eulerAngles = new Vector3(rotat.x, rotat.y, rotat.z - 2f * mod);
                    }
                }
                else
                {
					//just moving in plane by X and Z axis
                    if (Input.GetKey(KeyCode.DownArrow))
                    {
                        gb.transform.position = posit + new Vector3(0f, 0f, 0.02f * mod);
                    }
                    if (Input.GetKey(KeyCode.UpArrow))
                    {
                        gb.transform.position = posit + new Vector3(0f, 0f, -0.02f * mod);
                    }
                    if (Input.GetKey(KeyCode.LeftArrow))
                    {
                        gb.transform.position = posit + new Vector3(0.02f * mod, 0f, 0f);
                    }
                    if (Input.GetKey(KeyCode.RightArrow))
                    {
                        gb.transform.position = posit + new Vector3(-0.02f * mod, 0f, 0f);
                    }
                }

                if (curtime >= timelimit && 
                    Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.DownArrow)
                    )
                {
                    __instance.UpdateInspector();
                }

                if (Input.GetKeyDown(ModConfig.CamKey))
                {
                    if (Camera.main != null)
                    {
                        Camera.main.transform.position = gb.transform.position + new Vector3(0f, 3f, -5f);
                        Camera.main.transform.LookAt(gb.transform.position);
                    }
                }

                //__instance.RefreshObjectCatalogue(); //NOTWORKS need to update values in Inspector window

                //gb = null; //wrong idea to nulify catched selected object in same update of every frame
            }
        }
    }
}
