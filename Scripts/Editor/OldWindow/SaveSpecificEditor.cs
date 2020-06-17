using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Common.SaveLoadSystem
{
    public class SaveSpecificEditor : EditorWindow
    { 
        private static Iidentifier identifier;
        private static Vector2 scrollPosition;
        private static List<Component> components;
        private static List<ComponentSave> componentSaves;
        private static bool closeAllButton, saveButton = false;

        public static void ShowWindow(GameObject obj)
        {
            identifier = obj.GetComponent<Iidentifier>();
            components = new List<Component>();
            componentSaves = new List<ComponentSave>();
            foreach (Component c in obj.GetComponents<Component>())
            {
                if (c.GetType() != typeof(SaveableIdentifier))
                {
                    components.Add(c);
                }
            }
            GetWindow<SaveSpecificEditor>(true, "Save specific", true);

            //Shows a check button for every variable
            if (identifier.componentSaves == null)
            {
                for (int i = 0; i < components.Count; i++)
                {
                    if (components[i].GetType() != typeof(SaveableIdentifier))
                    {
                        List<bool> refVarList = new List<bool>();
                        for (int v = 0; v < ScriptReflector.GetVariableNames(components[i]).Count; v++)
                        {
                            refVarList.Add(false);
                        }
                        componentSaves.Add(new ComponentSave(components[i].GetType().Name, false, refVarList));
                    }
                }
            } else
            {
                componentSaves = identifier.componentSaves;
            }
        }

        private void OnGUI()
        {
            scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUIStyle.none);

            closeAllButton = GUILayout.Button("Close all");
            if (closeAllButton)
            {
                CloseAllFoldouts();
            }

            for (int i = 0; i < componentSaves.Count; i++)
            {
                if (components[i].GetType() != typeof(SaveableIdentifier))
                {
                    //TODO: save the variables
                    List<System.Tuple<string, string, string>> variables = ScriptReflector.GetVariableNames(components[i]);
                    ComponentSave tempList = componentSaves[i];
                    tempList.boolItem = EditorGUILayout.BeginFoldoutHeaderGroup(componentSaves[i].boolItem, components[i].GetType().Name);
                    componentSaves[i] = tempList;

                    if (componentSaves[i].boolItem == true)
                    {
                        GUILayout.Space(10);
                        EditorGUI.indentLevel += 5;
                        for (int v = 0; v < variables.Count; v++)
                        {
                            GUILayout.BeginHorizontal();
                            EditorGUILayout.LabelField(variables[v].Item1);
                            GUILayout.Space(-250);
                            componentSaves[i].boolList[v] = EditorGUILayout.Toggle(componentSaves[i].boolList[v]);
                            GUILayout.EndHorizontal();
                        }
                        EditorGUI.indentLevel -= 5;
                        GUILayout.Space(20);
                    }
                    EditorGUILayout.EndFoldoutHeaderGroup();
                }
            }

            GUILayout.FlexibleSpace();
            saveButton = GUILayout.Button("Save");
            if (saveButton)
            {
                SaveToIdentifier();
                Close();
            }
            GUILayout.EndScrollView();
        }

        private void CloseAllFoldouts()
        {
            for (int i = 0; i < componentSaves.Count; i++)
            {
                ComponentSave test = componentSaves[i];
                test.boolItem = false;
                componentSaves[i] = test;
            }
        }

        //Saves the information to the hidden identifier
        private void SaveToIdentifier()
        {
            for (int i = 0; i < componentSaves.Count; i++)
            {
                ComponentSave cSave = componentSaves[i];
                if (!componentSaves[i].boolList.Contains(true))
                {
                    cSave.boolItem = false;
                } else
                {
                    cSave.boolItem = true;
                }
                componentSaves[i] = cSave;
            }

            identifier.componentSaves = componentSaves;
            identifier.hasChanged = true;
        }
    }

}
