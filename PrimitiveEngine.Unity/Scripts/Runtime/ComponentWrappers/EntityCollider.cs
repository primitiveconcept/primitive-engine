namespace PrimitiveEngine.Unity
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Events;


    public abstract class EntityCollider : EntityMonoBehaviour
    {
        public ColliderEvent CollisionEntered;
        public ColliderEvent CollisionStayed;
        public ColliderEvent CollisionExited;
        
        public ColliderEvent TriggerEntered;
        public ColliderEvent TriggerStayed;
        public ColliderEvent TriggerExited;

        public List<Collider> CollisionList { get; set; } = new List<Collider>();
        public List<Collider> TriggerList { get; set; } = new List<Collider>();
        
        private new Collider collider;

        public Collider Collider
        {
            get
            {
                if (this.collider == null)
                    this.collider = GetComponent<Collider>();
                return this.collider;
            }
            set { this.collider = value; }
        }
    }
    
    [Serializable]
    public class ColliderEvent : UnityEvent<Collider>
    {}
}