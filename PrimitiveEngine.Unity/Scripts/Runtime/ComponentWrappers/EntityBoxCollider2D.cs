namespace PrimitiveEngine.Unity.ComponentWrappers
{
    using UnityEngine;


    [RequireComponent(typeof(BoxCollider2D))]
    public class EntityBoxCollider2D : EntityCollider2D
    {
        private BoxCollider2D boxCollider2D;

        public BoxCollider2D BoxCollider2D
        {
            get
            {
                if (this.boxCollider2D == null)
                    this.boxCollider2D = GetComponent<BoxCollider2D>();
                if (this.boxCollider2D == null)
                    this.boxCollider2D = this.gameObject.AddComponent<BoxCollider2D>();
                return this.boxCollider2D;
            }
            set { this.boxCollider2D = value; }
        }
    }
}