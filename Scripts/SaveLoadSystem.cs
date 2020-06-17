using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;
using System.Xml;

namespace Common.SaveLoadSystem
{
    //Head class of the save & load system
    [InitializeOnLoad]
    public class SaveLoadSystem
    {
        #region Private vars
        private static SaveableIdentifier[] allIdentifiersInScene;
        private static XmlDocument xmlDocument = new XmlDocument();
        private static XmlNode rootNode;
        private static string saveFileName = "Save.xml";
        private static string rootNodeName = "saveables";
        private static string idPrefix = "id";
        #endregion

        static SaveLoadSystem()
        {
            try
            {
                xmlDocument.Load(saveFileName);
                rootNode = xmlDocument.FirstChild;
            } catch (FileNotFoundException)
            {
                CreateNewFile();
            }
        }

        #region Load functions
        /// <summary>
        /// Instantiates/overwrites all currently saved objects
        /// </summary>
        public static void Load()
        {
            allIdentifiersInScene = Object.FindObjectsOfType<SaveableIdentifier>();

            for (int i = 1; i < GetXMLid(false) + 1; i++)
            {
                LoadObject(i);
            }
        }

        private static void LoadObject(int id)
        {
            if (GetIdNode(id) == null)
            {
                return;
            }

            GameObject obj = null;
            for (int i = 0; i < allIdentifiersInScene.Length; i++)
            {
                if (allIdentifiersInScene[i].id == id)
                {
                    obj = allIdentifiersInScene[i].gameObject;
                }
            }

            if (obj == null)
            {
                obj = new GameObject();
            }

            XmlNode idNode = GetIdNode(id);

            //Adds the component with correct values
            foreach (XmlNode cNode in idNode.ChildNodes)
            {
                string cName = cNode.Attributes[0].Value;
                System.Type cType = System.Type.GetType(cName);
                Component currentComponent = (obj.GetComponent(cType)) ? obj.GetComponent(cType) : obj.AddComponent(cType);

                foreach (XmlNode vNode in cNode.ChildNodes)
                {
                    ScriptReflector.SetComponentVarTo(
                        currentComponent,
                        vNode.Name,
                        ConvertString.ThisType(vNode.InnerText, System.Type.GetType(vNode.Attributes[0].Value)));
                }
            }
        }
        #endregion

        #region Save functions
        /// <summary>
        /// Saves all saveable objects in the current open scenes
        /// </summary>
        public static void Save()
        {
            CreateNewFile();
            ConvertAllObjectsToXML(true);
        }

        private static void ConvertAllObjectsToXML(bool checkForChange = false)
        {
            allIdentifiersInScene = Object.FindObjectsOfType<SaveableIdentifier>();

            for (int i = 0; i < allIdentifiersInScene.Length; i++)
            {
                if (allIdentifiersInScene[i].hasChanged || checkForChange)
                {
                    AddToSave(allIdentifiersInScene[i].gameObject, allIdentifiersInScene[i].id);
                    allIdentifiersInScene[i].hasChanged = false;
                }
            }

            xmlDocument.Save(saveFileName);
        }

        private static void AddToSave(GameObject obj, int id)
        {
            XmlNode idNode = GetCreateIdNode(id);
            Iidentifier identifier = obj.GetComponent<Iidentifier>();
            List<ComponentSave> cSave = identifier.componentSaves;

            foreach (Component component in obj.GetComponents<Component>())
            {
                ComponentSave componentSave = cSave.Find(x => x.componentTypeName == component.GetType().Name);

                if (
                    component.GetType() != typeof(SaveableIdentifier) &&
                    (GetChildNodeFromName(idNode, component.GetType().AssemblyQualifiedName) == null || identifier.hasChanged) &&
                    componentSave.boolItem == true
                    )
                {
                    MakeVariableNodes(component, MakeComponentNode(component, idNode), componentSave);
                }
            }
        }

        private static XmlNode MakeComponentNode(Component component, XmlNode parentNode)
        {
            XmlElement componentNode = GetChildNodeFromName(parentNode, component.GetType().AssemblyQualifiedName);
            if (componentNode == null)
            {
                componentNode = xmlDocument.CreateElement("component");
                componentNode.SetAttribute("name", component.GetType().AssemblyQualifiedName);
                parentNode.AppendChild(componentNode);
            }

            return componentNode;
        }

        private static void MakeVariableNodes(Component component, XmlNode componentNode, ComponentSave componentSave)
        {
            List<System.Tuple<string, string, string>> componentFields = ScriptReflector.GetVariableNames(component);

            for (int i = 0; i < componentFields.Count; i++)
            {
                XmlElement fieldNode = GetChildNodeFromName(componentNode, componentFields[i].Item1);
                if (componentSave.boolList[i])
                {

                    if (fieldNode == null)
                    {
                        fieldNode = xmlDocument.CreateElement(componentFields[i].Item1);
                    }

                    fieldNode.SetAttribute("type", componentFields[i].Item2);
                    fieldNode.InnerText = componentFields[i].Item3;
                    componentNode.AppendChild(fieldNode);

                } else if (fieldNode != null)
                {
                    componentNode.RemoveChild(fieldNode);
                }
            }
        }


        #endregion

        #region General functions
        public static int GetXMLid(bool forSaveable = true)
        {
            string id = rootNode.Attributes["id"].Value;
            int newValue = int.Parse(id);
            if (forSaveable)
            {
                newValue += 1;
                rootNode.Attributes["id"].Value = newValue.ToString();
                xmlDocument.Save(saveFileName);
            }
            return newValue;
        }

        private static XmlNode GetCreateIdNode(int id)
        {
            if (xmlDocument.SelectSingleNode("saveables/" + idPrefix + id.ToString() + "[1]") == null)
            {
                XmlNode idNode = xmlDocument.CreateElement(idPrefix + id.ToString());
                rootNode.AppendChild(idNode);

                return idNode;
            }

            return xmlDocument.SelectSingleNode("saveables/" + idPrefix + id.ToString() + "[1]") ;
        }

        private static XmlNode GetIdNode(int id) => xmlDocument.SelectSingleNode("saveables/" + idPrefix + id.ToString() + "[1]");

        private static XmlElement GetChildNodeFromName(XmlNode parentNode, string nodeName)
        {
            foreach (XmlElement cNode in parentNode.ChildNodes)
            {
                if (cNode.Attributes["name"] != null)
                {
                    if (cNode.Attributes["name"].Value == nodeName)
                    {
                        return cNode;
                    }
                } else if (cNode.Name == nodeName)
                {
                    return cNode;
                }
            }

            return null;
        }

        private static void CreateNewFile()
        {
            if (rootNode == null)
            {
                XmlElement rootElement = xmlDocument.CreateElement(rootNodeName);
                rootElement.SetAttribute(idPrefix, Object.FindObjectsOfType<SaveableIdentifier>().Length.ToString());
                xmlDocument.AppendChild(rootElement);
            } else
            {
                string idValue = rootNode.Attributes[idPrefix].Value;
                rootNode.RemoveAll();
                XmlElement rootelement = rootNode as XmlElement;
                rootelement.SetAttribute(idPrefix, idValue);
            }

            xmlDocument.Save(saveFileName);
            xmlDocument.Load(saveFileName);
            rootNode = xmlDocument.FirstChild;
        }
        #endregion
    }
}
