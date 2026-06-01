
//using System.IO;
using System;
using System.Collections.Generic;
using System.Linq;

using HarmonyLib;
using MelonLoader;
using UnityEngine;
using UnityEngine.UI;


//using static System.Runtime.CompilerServices.RuntimeHelpers;

[assembly: MelonInfo(typeof(NoTimeEditorAdditorMod.EditorAdditorMod), "notime.kastuk.editaddit", "Editor Additor Mod", "2026.02.23")]


namespace NoTimeEditorAdditorMod
{
     public class EditorAdditorMod : MelonMod
    {
        //public static ManualLogSource InstanceLog;
        public override void OnInitializeMelon()
        {
            //EditorAdditorMod.InstanceLog = base.Log;
            //Harmony harmony = new Harmony("notime.kastuk.editaddit"); //no need for Melon Loader
            //harmony.PatchAll(typeof(EditorAdditorMod.ObjectsEditorMods));
            MelonLogger.Msg("!!! Editor Additor LOADED !!!");
            ModConfig.Bind();

        }



        public static class ModConfig
        {
            public static MelonPreferences_Entry<KeyCode> Mod1Key;
            public static MelonPreferences_Entry<KeyCode> Mod2Key;
            public static MelonPreferences_Entry<float> Mod1;
            public static MelonPreferences_Entry<float> Mod2;

            public static MelonPreferences_Entry<KeyCode> RotatKey;
            public static MelonPreferences_Entry<KeyCode> HeigScalKey;

            public static MelonPreferences_Entry<KeyCode> CamKey;

            public static MelonPreferences_Category Modifiers;
            public static MelonPreferences_Category OtherCon;

            public static void Bind()
            {
                // 2. Create the category (shows up in UserLibs/MelonPreferences.cfg)
                Modifiers = MelonPreferences.CreateCategory("Modifiers");
                OtherCon = MelonPreferences.CreateCategory("Other Controls");

                // 3. Create the entry with a default KeyCode (e.g., KeyCode.K)
                //myKeybind = Modifiers.CreateEntry<KeyCode>("ToggleKey", KeyCode.K, "The key to press for action");



                ModConfig.Mod1Key = Modifiers.CreateEntry<KeyCode>("Modifier 1 key", KeyCode.RightControl, "Modifier 1 key");       
                ModConfig.Mod1 = Modifiers.CreateEntry<float>("Modifier 1 value", 0.2f, "Modifier 1 value (usually for tiny adjust)");
                ModConfig.Mod2Key = Modifiers.CreateEntry<KeyCode>("Modifier 2 key", KeyCode.RightShift, "Modifier 2 key");
                ModConfig.Mod2 = Modifiers.CreateEntry<float>("Modifier 2 value", 5f, "Modifier 2 value (usually for bigger moves)");

                ModConfig.RotatKey = OtherCon.CreateEntry<KeyCode>("Rotation key", KeyCode.Slash, "Key to hold with arrows pressing for rotating by Y (left-right) and Z (up-down) axis");
                ModConfig.HeigScalKey = OtherCon.CreateEntry<KeyCode>("Height+Scale key", KeyCode.Space, "Key to hold with arrows pressing for changing height (up-down) and scale (left-right)");

                ModConfig.CamKey = OtherCon.CreateEntry<KeyCode>("Camera key", KeyCode.End, "Key move camera towards highlighted object");

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

            public static Transform cam;
            //public static InputField[] allinputfields;
            //public static RectTransform rect;
            //public static Vector3[] corners = new Vector3[4];
            public static bool listOrdered = false;

            [HarmonyPostfix]
            [HarmonyPatch(typeof(LevelEditor), nameof(LevelEditor.Start))]
            public static void LevEdStartPostfix(ref LevelEditor __instance)
            {
                if (__instance.water != null)
                {
                    __instance.water.GetComponent<Collider>().enabled = false; //let click other objects under water surface
                    //Adjust visual level of water in editor
                    __instance.water.transform.position = new Vector3(__instance.water.transform.position.x, __instance.water.transform.position.y - 1f, __instance.water.transform.position.z);
                }
				//listOrdered = false;
                
                //check and reorder all objects in the list by parents on top

                //LevelObjectCatalogue spawnlist = __instance.catalogue.spawned;
                //Dictionary <string, List <string>> Parented = new Dictionary<string, List<string>>();
                //foreach (var obj in __instance.catalogue.spawned)
                //{
                //	if (obj.parentID != null)
                //	{
                //		if (!Parented.ContainsKey(obj.parentID))
                //                    {
                //                        Parented[obj.parentID] = new List<string>();
                //                    }
                //                    else
                //                    {
                //                        Parented[obj.parentID].Add(obj.uniqueID);
                //                    }
                //	}	
                //}
                //            foreach (var par in __instance.catalogue.spawned)
                //            {
                //               if (Parented.ContainsKey(par.uniqueID))
                //                {
                //                    int order = __instance.catalogue.spawned.IndexOf(par); // MUST BE IN FIRST foreach and saved into dictionary key alongside id, then splitted out, when need... index of parent object in the list
                //                    for (int i = 0; i < Parented[par.uniqueID].Count; i++)
                //                    {

                //                    }
                //                }
                //            }



                //GameObject gam = GameObject.Find("Menu n Camera"); //PROBLEM - moving of this camera into coordinates will break common keys movement
                //if (gam != null)
                //{
                //	cam = gam.transform.Find("Camera");
                //}

                //__instance.UpdateHierarchy(); //try to reallign parented objects in the list after edit of the file outside of editor
                //__instance.UpdateParented(); //NOT WORKS

                //allinputfields = __instance.inspector.GetComponentsInChildren<InputField>();//UnityEngine.Object.FindObjectsOfType<InputField>();
                //rect = __instance.inspector.GetComponent<RectTransform>();
                //rect.GetWorldCorners(corners);
                //recover camera position after load (where to save previous position numbers? level description?)
                //__instance.camSpawn.position = new Vector3(x,y,z);
                //__instance.camSpawn.eulerAngles = new Vector3(x,y,z);
            }

            [HarmonyPatch(typeof(LevelEditor), nameof(LevelEditor.OpenObjectCatalogue))]
            [HarmonyPrefix]
            public static void LevEditOpObjCatPrefix(ref LevelEditor __instance)
            {
                __instance.water.GetComponent<Collider>().enabled = false;
				
              //  if (listOrdered == false)
                //{
                //    __instance.catalogue.spawned = ReorderObjects(__instance.catalogue.spawned);
               //     listOrdered = true;
               //     InstanceLog.LogError("List of spawned objects is reordered: "+ __instance.catalogue.spawned.ToString());
               // }
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

            //[HarmonyPatch(typeof(LevelEditor), nameof(LevelEditor.SetDefaultName))]
            //[HarmonyPostfix]
            //public static void SetDefNamePostfix(ref LevelObjectCatalogue __refe)
            //{ //NOT WORKS
            //    if (__refe.inherit.Contains("Empty GameObject"))
            //    {
            //        __refe.scale = new Vector3(1.0f, 1.0f, 1.0f); //set default scale for object, which usually used for childs container to evade autoscaling of childs later
            //    }
            //}

            //public static bool cammove = false;
            [HarmonyPatch(typeof(LevelEditor), nameof(LevelEditor.LateUpdate))]
            [HarmonyPostfix]
            public static void LevEditLUpdatePostfix(ref LevelEditor __instance)
            {
                if (__instance.tobeplaced != null && __instance.tobeplaced.name == "_PrefEmpty")
                { //make every new placed Empty GameObject to be properly scaled to 1.0, so any children will not autoscale after saveload
                    __instance.tobeplaced.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                } //well, any other objects will still have a problem as parents, so only that ones can be used for groups of objects



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

                if (Input.GetKeyDown(ModConfig.CamKey))// || cammove)
                {
                    //cammove = true;
                    cam.position = gb.transform.position + new Vector3(0f, 3f, -3f);//Vector3.MoveTowards(cam.position, (gb.transform.position + new Vector3(0f, 10f, -10f)), 10f * Time.deltaTime);
                    //Vector3 dir = gb.transform.position - cam.position;
                    //Quaternion targrot = Quaternion.LookRotation(dir);
                    cam.LookAt(cam.position, gb.transform.position);

                    //Quaternion.Slerp(cam.rotation, targrot, 10f * Time.deltaTime);

                    //if (Vector3.Distance(cam.position, gb.transform.position) < 5f)
                    //{
                    //	cammove = false;
                    //}
                }
                //__instance.RefreshObjectCatalogue(); //NOTWORKS need to update values in Inspector window

                //gb = null; //wrong idea to nulify catched selected object in same update of every frame
            }



            private static Il2CppSystem.Collections.Generic.List<LevelObjectCatalogue> ReorderObjects(Il2CppSystem.Collections.Generic.List<LevelObjectCatalogue> sourceList)
            {
                InstanceLog.LogError("SPawned objects list to be reordered: "+ sourceList.ToString());
                var preresult = new List<LevelObjectCatalogue>();
                foreach (var obj in sourceList) //conversion to common List
                {
                    preresult.Add(obj);
                }
                // 1. Separate roots and children
                var roots = preresult.Where(o => o.parentID == null || o.parentID == null).ToList();
                var children = preresult.Where(o => o.parentID != null && o.parentID != null)
                                         .GroupBy(o => o.parentID)
                                         .ToDictionary(g => g.Key, g => g.ToList());


                var result = new Il2CppSystem.Collections.Generic.List<LevelObjectCatalogue>();

                // 2. Iterate through roots, adding them and their children in order
                foreach (var root in roots)
                {
                    preresult.Add(root);

                    // Add children of this root
                    if (children.TryGetValue(root.uniqueID, out var childList))
                    {
                        preresult.AddRange(childList);
                    }
                }

                // 3. Optional: Add orphaned children (if any) that weren't attached to a root
                var attachedIds = new HashSet<string>(preresult.Select(o => o.uniqueID));
                foreach (var childGroup in children)
                {
                    foreach (var child in childGroup.Value)
                    {
                        if (!attachedIds.Contains(child.uniqueID))
                        {
                            preresult.Add(child);
                        }
                    }
                }

                foreach (var item in preresult) //conversion back
                {
                    result.Add(item);
                }
                return result;
            }


        }
    }
}
