namespace PrimitiveEngine.Unity
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEditor;
    using UnityEngine;


    [CustomPropertyDrawer(typeof(SerializableTypeAttribute))]
    public class SerializableTypeDrawer : PropertyDrawer
    {
        private List<string> typeNames;
        private List<string> assemblyQualifiedNames;
        
        public override void OnGUI (
            Rect position, 
            SerializedProperty property, 
            GUIContent label) 
        {
            SerializableTypeAttribute serializableTypeAttribute = attribute as SerializableTypeAttribute;

            if (this.typeNames == null
                || this.assemblyQualifiedNames == null)
            {
                Debug.Log("Populating types");
                this.typeNames = new List<string>();
                this.assemblyQualifiedNames = new List<string>();
                
                Type typeFilter = serializableTypeAttribute.TypeFilter;
                IEnumerable<Type> types = AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(s => s.GetTypes())
                    .Where(p => typeFilter.IsAssignableFrom(p) && !p.IsAbstract);
                foreach (Type type in types)
                {
                    this.typeNames.Add(type.Name);
                    this.assemblyQualifiedNames.Add(type.AssemblyQualifiedName);
                }
            }

            int index = Mathf.Max (0, this.assemblyQualifiedNames.IndexOf(property.stringValue));
            index = EditorGUI.Popup (position, property.displayName, index, this.typeNames.ToArray());
            property.stringValue = this.assemblyQualifiedNames [index];
        }
    }
}