namespace PrimitiveEngine.Unity
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using UnityEditor;
    using UnityEditorInternal;
    using UnityEngine;

    [CustomPropertyDrawer(typeof(ReorderableListAttribute))]
    public class ReorderableListDrawer : ArrayDrawer
    {
        private static readonly ReorderableListAttribute
        defaultAttribute = new ReorderableListAttribute();

        private static readonly GetArrayOrListElementTypeDelegate
        GetArrayOrListElementType =
            (GetArrayOrListElementTypeDelegate)
            Delegate.CreateDelegate(
                typeof(GetArrayOrListElementTypeDelegate),
                null,
                typeof(PropertyDrawer)
                .Assembly
                .GetType("UnityEditor.EditorExtensionMethods")
                .GetMethod(
                    "GetArrayOrListElementType",
                    BindingFlags.NonPublic |
                    BindingFlags.Static
                )
            );

        private static readonly GetDrawerTypeForTypeDelegate
        GetDrawerTypeForType =
            (GetDrawerTypeForTypeDelegate)
            Delegate.CreateDelegate(
                typeof(GetDrawerTypeForTypeDelegate),
                null,
                typeof(PropertyDrawer)
                .Assembly
                .GetType("UnityEditor.ScriptAttributeUtility")
                .GetMethod(
                    "GetDrawerTypeForType",
                    BindingFlags.NonPublic |
                    BindingFlags.Static
                )
            );

        private readonly ReorderableListMap
        m_ReorderableListMap = new ReorderableListMap();

        private ReorderableListOfValues
        m_MostRecentReorderableList;

        private string
        m_MostRecentPropertyPath;


        public delegate void BackgroundColorDelegate(
            SerializedProperty array,
            int index,
            ref Color backgroundColor);


        public delegate void ElementDelegate(SerializedProperty array, int index);


        private delegate Type GetArrayOrListElementTypeDelegate(Type listType);


        private delegate Type GetDrawerTypeForTypeDelegate(Type type);


        public static event BackgroundColorDelegate onBackgroundColor;

        public static event ElementDelegate onElementSelected;


        #region Properties
        public new ReorderableListAttribute attribute
        {
            get
            {
                ReorderableListAttribute attribute = (ReorderableListAttribute)base.attribute;
                return attribute ?? defaultAttribute;
            }
        }
        #endregion


        public struct BackgroundColorScope : IDisposable
        {
            private readonly BackgroundColorDelegate m_callback;

            public BackgroundColorScope(BackgroundColorDelegate callback)
            {
                this.m_callback = callback;
                onBackgroundColor += this.m_callback;
            }

            public void Dispose()
            {
                onBackgroundColor -= this.m_callback;
            }
        }


        public override bool CanCacheInspectorGUI(SerializedProperty property)
        {
            return true;
        }


        public struct ElementSelectionScope : IDisposable
        {
            private readonly ElementDelegate m_callback;

            public ElementSelectionScope(ElementDelegate callback)
            {
                this.m_callback = callback;
                onElementSelected += this.m_callback;
            }

            public void Dispose()
            {
                onElementSelected -= this.m_callback;
            }
        }


        public override float GetPropertyHeight(
            SerializedProperty property,
            GUIContent label)
        {
            ReorderableListOfValues reorderableListOfValues =
                GetReorderableList(
                    this.attribute,
                    this.fieldInfo,
                    property);

            Debug.Assert(
                reorderableListOfValues.serializedProperty.propertyPath ==
                property.propertyPath);

            try {
                return reorderableListOfValues.GetHeight(label);
            }
            catch (Exception ex) {
                Debug.LogException(ex);
                return 0f;
            }
        }


        public override void OnGUI(
            Rect position,
            SerializedProperty property,
            GUIContent label)
        {
            ReorderableListOfValues reorderableList =
                GetReorderableList(
                    this.attribute,
                    this.fieldInfo,
                    property);
            reorderableList.UpdateLabel(label);
            reorderableList.onBackgroundColor = onBackgroundColor;
            reorderableList.onSelectCallback += OnSelectCallback;
            reorderableList.DoGUI(position);
            reorderableList.onSelectCallback -= OnSelectCallback;
            reorderableList.onBackgroundColor = null;
        }


        protected ReorderableListOfValues GetReorderableList(
            ReorderableListAttribute attribute,
            FieldInfo fieldInfo,
            SerializedProperty property)
        {
            string propertyPath = property.propertyPath;

            if (this.m_MostRecentReorderableList != null)
            {
                if (this.m_MostRecentPropertyPath == propertyPath)
                {
                    this.m_MostRecentReorderableList.serializedProperty = property;
                    return this.m_MostRecentReorderableList;
                }
            }

            this.m_MostRecentReorderableList = this.m_ReorderableListMap
                .Find(propertyPath);

            if (this.m_MostRecentReorderableList == null)
            {
                ReorderableListOfValues reorderableList =
                    CreateReorderableList(
                        attribute,
                        fieldInfo,
                        property);

                this.m_ReorderableListMap.Add(propertyPath, reorderableList);

                this.m_MostRecentReorderableList = reorderableList;
            }
            else
            {
                this.m_MostRecentReorderableList.serializedProperty = property;
            }

            this.m_MostRecentPropertyPath = propertyPath;

            return this.m_MostRecentReorderableList;
        }


        protected void OnSelectCallback(ReorderableList list)
        {
            SerializedProperty array = list.serializedProperty;
            int index = list.index;
            if (onElementSelected != null)
                onElementSelected.Invoke(array, index);
        }


        private ReorderableListOfValues
        CreateReorderableList(
            ReorderableListAttribute attribute,
            FieldInfo fieldInfo,
            SerializedProperty property)
        {
            Type listType = fieldInfo.FieldType;

            Type elementType = GetArrayOrListElementType(listType);

            bool elementIsValue =
                elementType.IsEnum ||
                elementType.IsPrimitive ||
                elementType == typeof(string) ||
                elementType == typeof(Color) ||
                elementType == typeof(LayerMask) ||
                elementType == typeof(Vector2) ||
                elementType == typeof(Vector3) ||
                elementType == typeof(Vector4) ||
                elementType == typeof(Rect) ||
                elementType == typeof(AnimationCurve) ||
                elementType == typeof(Bounds) ||
                elementType == typeof(Gradient) ||
                elementType == typeof(Quaternion) ||
                elementType == typeof(Vector2Int) ||
                elementType == typeof(Vector3Int) ||
                elementType == typeof(RectInt) ||
                elementType == typeof(BoundsInt);

            if (elementIsValue)
            {
                return
                    new ReorderableListOfValues(
                        attribute,
                        property,
                        listType,
                        elementType
                    );
            }

            bool elementIsUnityEngineObject =
                typeof(UnityEngine.Object)
                .IsAssignableFrom(elementType);

            if (elementIsUnityEngineObject)
            {
                bool elementsAreSubassets =
                    elementIsUnityEngineObject &&
                    attribute != null &&
                    attribute.elementsAreSubassets;

                if (elementsAreSubassets)
                {
                    Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

                    IEnumerable<Type> types = assemblies.SelectMany(a => a.GetTypes());

                    Type[] subassetTypes =
                        types.Where(t =>
                            t.IsAbstract == false &&
                            t.IsGenericTypeDefinition == false &&
                            elementType.IsAssignableFrom(t)
                        )
                        .ToArray();

                    return
                        new ReorderableListOfSubassets(
                            attribute,
                            property,
                            listType,
                            elementType,
                            subassetTypes
                        );
                }
                else
                {
                    return
                        new ReorderableListOfValues(
                            attribute,
                            property,
                            listType,
                            elementType
                        );
                }
            }

            Type elementPropertyDrawerType = GetDrawerTypeForType(elementType);
            if (elementPropertyDrawerType == null)
            {
                bool elementIsStruct =
                    elementType.IsValueType &&
                    elementType.IsEnum == false &&
                    elementType.IsPrimitive == false;

                bool elementIsClass =
                    elementType.IsClass;

                if (elementIsStruct || elementIsClass)
                {
                    return
                        new ReorderableListOfStructures(
                            attribute,
                            property,
                            listType,
                            elementType
                        );
                }
            }

            return
                new ReorderableListOfValues(
                    attribute,
                    property,
                    listType,
                    elementType
                );

        }


        private class ReorderableListMap
        : Dictionary<string, ReorderableListOfValues>
        {
            public ReorderableListOfValues Find(string key)
            {
                ReorderableListOfValues reorderableList = default(ReorderableListOfValues);
                TryGetValue(key, out reorderableList);
                return reorderableList;
            }
        }
    }

}
