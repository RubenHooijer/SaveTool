using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Common.SaveLoadSystem
{
    //IN PROGRESS
    //A drag & drop window for saving
    public class FancySaveWindow : EditorWindow
    {
        private static Iidentifier identifier;
        private static Vector2 scrollPosition;
        private static List<Component> components;
        private static List<ComponentSave> componentSaves;

        List<EditorDraggableObject> draggableObjects = new List<EditorDraggableObject>();

        private void OnEnable()
        {
            draggableObjects.Add(new EditorDraggableObject());
        }

        private void Update()
        {
            Repaint();
        }

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
            GetWindow<FancySaveWindow>(true, "Save Window", true);

            
        }

        private void OnGUI()
        {
            foreach (var dragObj in draggableObjects)
            {
                dragObj.OnGUI();
            }
        }
    } 
}
