using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

namespace Common.SaveLoadSystem
{

    [InitializeOnLoad]
    static class HierarchyIcons
    {
        //This code is yoinked from the unity form https://answers.unity.com/questions/431952/how-to-show-an-icon-in-hierarchy-view.html
        static Texture2D texturePanel;
        static float hierarchyWindowWidth;

        static HierarchyIcons()
        {
            texturePanel = AssetDatabase.LoadAssetAtPath("Assets/SaveLoadTool/Icons/save_icon.png", typeof(Texture2D)) as Texture2D; //TODO: find the right path
            EditorApplication.hierarchyWindowItemOnGUI += HierarchyItem;
        }

        private static void HierarchyItem(int instanceID, Rect selectionRect)
        {
            GameObject go = EditorUtility.InstanceIDToObject(instanceID) as GameObject;

            //Check if scene
            if (go != null)
            {
                //Check it needs an icon
                if (go.GetComponent<SaveableIdentifier>())
                {
                    Rect r = new Rect(selectionRect);
                    r.x = hierarchyWindowWidth;
                    r.width = 20;

                    GUI.Label(r, texturePanel);
                }
            } else
            {
                //If scene then I will set the position of the icons
                hierarchyWindowWidth = selectionRect.width;
            }
        }
    }

    static class SaveCommand
    {
        [MenuItem("GameObject/Saveable", false, 17)]
        private static void MakeSaveable(MenuCommand command)
        {
            GameObject obj = (GameObject)command.context;
            Iidentifier identifier = obj.GetComponent<SaveableIdentifier>();

            if (identifier == null)
            {
                identifier = obj.AddComponent<SaveableIdentifier>();
                identifier.id = SaveLoadSystem.GetXMLid();
                SaveSpecificEditor.ShowWindow(obj);
            } else
            {
                Object.DestroyImmediate((SaveableIdentifier)identifier);
                SaveLoadSystem.Save();
            }
        }

        [MenuItem("GameObject/Save specific", false, 17)]
        private static void SaveWindow(MenuCommand command)
        {
            GameObject obj = (GameObject)command.context;
            SaveSpecificEditor.ShowWindow(obj);
        }

        [MenuItem("GameObject/Fancy save", false, 18)]
        private static void OpenFancySaveWindow(MenuCommand command)
        {
            GameObject obj = (GameObject)command.context;
            FancySaveWindow.ShowWindow(obj);
        }

        [MenuItem("GameObject/Save specific", true)]
        private static bool ValidateSaveWindow(MenuCommand command)
        {
            GameObject obj = (GameObject)command.context;
            return obj.GetComponent<SaveableIdentifier>() != null;
        }

        private static bool ValidateOpenFancySaveWindow(MenuCommand command)
        {
            GameObject obj = (GameObject)command.context;
            return obj.GetComponent<SaveableIdentifier>() != null;
        }

        [MenuItem("Edit/Save all saveables &s", false, 1)]
        private static void SaveObjectsToXML()
        {
            SaveLoadSystem.Save();
        }

        [MenuItem("Edit/Load all", false, 2)]
        private static void LoadObjects()
        {
            SaveLoadSystem.Load();
        }
    }
}