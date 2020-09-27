namespace PrimitiveEngine.Unity
{
    using System;
    using UnityEngine;

    
    public class SerializableTypeAttribute : PropertyAttribute
    {
        public readonly Type TypeFilter;


        public SerializableTypeAttribute(Type typeFilter)
        {
            this.TypeFilter = typeFilter;
        }
    }
}