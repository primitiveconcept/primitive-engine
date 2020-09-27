namespace PrimitiveEngine.Unity
{
    using System;
    using UnityEngine;


    [CreateAssetMenu]
    public class SystemsConfig : ScriptableObject
    {
        [ReorderableList]
        public SystemsConfig.Entry[] Systems;


        [Serializable]
        public class Entry
        {
            [SerializableType(typeof(EntitySystem))]
            public string SystemTypeName;
            public UpdateType UpdateType = UpdateType.FrameUpdate;
            public ExecutionType ExecutionType = ExecutionType.Synchronous;
        }
    }
}