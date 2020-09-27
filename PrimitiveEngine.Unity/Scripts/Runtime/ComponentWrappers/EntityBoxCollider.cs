namespace PrimitiveEngine.Unity
{
    using UnityEngine;


    [RequireComponent(typeof(BoxCollider))]
    public class EntityBoxCollider : EntityCollider
    {
        private BoxCollider boxCollider;

        public BoxCollider BoxCollider
        {
            get
            {
                if (this.boxCollider == null)
                    this.boxCollider = GetComponent<BoxCollider>();
                if (this.boxCollider == null)
                    this.boxCollider = this.gameObject.AddComponent<BoxCollider>();
                return this.boxCollider;
            }
            set { this.boxCollider = value; }
        }
    }
}