using System.Collections.Generic;
using UnityEngine;

namespace Common.SaveLoadSystem
{
    //Saves the positions of the variable inside the component, you want to save
    //FIX: update when a variable changes position
    [System.Serializable]
    public struct ComponentSave
    {
        public string componentTypeName;
        public bool boolItem;
        public List<bool> boolList;

        public ComponentSave(string typeName, bool boolItem, List<bool> boolList)
        {
            componentTypeName = typeName;
            this.boolItem = boolItem;
            this.boolList = boolList;
        }
    }
}
