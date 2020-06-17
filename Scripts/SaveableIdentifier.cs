using System.Collections.Generic;
using UnityEngine;

namespace Common.SaveLoadSystem
{
    //Hidden identifier
    public class SaveableIdentifier : MonoBehaviour, Iidentifier
    {
        public int _id;
        public int id 
        {
            get { return _id; }
            set 
            { 
                _id = (_id != 0) ? _id : value; 
            }
        }

        public List<ComponentSave> _componentSave;
        public List<ComponentSave> componentSaves 
        { 
            get { return _componentSave; } 
            set { _componentSave = value; }
        }

        public bool _hasChanged;
        public bool hasChanged 
        { 
            get { return _hasChanged; } 
            set { _hasChanged = value; }
        }
    }
}
