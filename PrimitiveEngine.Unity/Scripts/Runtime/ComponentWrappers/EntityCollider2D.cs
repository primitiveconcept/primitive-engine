namespace PrimitiveEngine.Unity
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Events;


    public abstract class EntityCollider2D : EntityMonoBehaviour
    {
        public Collider2DEvent CollisionEntered;
        public Collider2DEvent CollisionStayed;
        public Collider2DEvent CollisionExited;
        
        public Collider2DEvent TriggerEntered;
        public Collider2DEvent TriggerStayed;
        public Collider2DEvent TriggerExited;

        public List<Collider2D> CollisionList { get; set; } = new List<Collider2D>();
        public List<Collider2D> TriggerList { get; set; } = new List<Collider2D>();
        
        private new Collider2D collider2D;

        public Collider2D Collider2D
        {
            get
            {
                if (this.collider2D == null)
                    this.collider2D = GetComponent<Collider2D>();
                return this.collider2D;
            }
            set { this.collider2D = value; }
        }
    }
    
    [Serializable]
    public class Collider2DEvent : UnityEvent<Collider2D>
    {}
}