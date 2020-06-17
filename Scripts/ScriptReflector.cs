using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System.Linq;

namespace Common.SaveLoadSystem
{
    public class ScriptReflector
    {
        /// <summary>
        /// returns Name, AssemblyName, Value
        /// </summary>
        /// <param name="component"></param>
        /// <returns>Name, AssemblyName, Value</returns>
        public static List<System.Tuple<string, string, string>> GetVariableNames(Component component)
        {
            List<System.Tuple<string, string, string>> variableNamesValues = new List<System.Tuple<string, string, string>>();

            System.Type componentType = component.GetType();
            FieldInfo[] fields = componentType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            PropertyInfo[] properties = componentType.GetProperties(BindingFlags.Instance | BindingFlags.Public);

            if (properties.Length > 0)
            {
                variableNamesValues.AddRange(from property in properties
                                        where property.CanRead && property.CanWrite && property.GetValue(component) != null
                                        select System.Tuple.Create(property.Name, property.PropertyType.AssemblyQualifiedName, property.GetValue(component).ToString()));
            }

            if (fields.Length > 0)
            {
                variableNamesValues.AddRange(from field in fields
                                        where field.GetValue(component) != null && !(field.FieldType is object)
                                        select System.Tuple.Create(field.Name, field.FieldType.AssemblyQualifiedName, field.GetValue(component).ToString()));;
            }

            return variableNamesValues;
        }

        public static void SetComponentVarTo(Component currentComponent, string varName, object value)
        {
            //Assert.IsNotNull(value);
            if (currentComponent.GetType().GetProperty(varName) != null)
            {
                currentComponent.GetType().GetProperty(varName).SetValue(currentComponent, value);
            }

            if (currentComponent.GetType().GetField(varName) != null)
            {
                currentComponent.GetType().GetField(varName).SetValue(currentComponent, value);
            }
        }
    }
}
