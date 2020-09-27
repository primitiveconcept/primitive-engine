namespace PrimitiveEngine.Unity
{
    using UnityEngine;


    public class EntityWrapper : MonoBehaviour
    {
        public void Awake()
        {
            WrapMonoBehaviours(this.gameObject);
            Destroy(this);
        }


        public static void WrapMonoBehaviours(GameObject gameObject)
        {
            Component[] components = gameObject.GetComponents<Component>();
            for (int i = 0; i < components.Length; i++)
            {
                Component component = components[i];
                if (component is BoxCollider2D
                    && gameObject.GetComponent<EntityBoxCollider2D>() == null)
                {
                    gameObject.AddComponent<EntityBoxCollider2D>();
                }
            }
        }
    }
}