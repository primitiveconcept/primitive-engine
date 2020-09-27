namespace PrimitiveEngine.Unity
{
	using PrimitiveEngine;
	using UnityEngine;


	public abstract partial class EntityMonoBehaviour : MonoBehaviour, 
														IEntityComponent
	{
		#region Properties
		public Entity Entity { get; set; }
		#endregion


		public virtual void Awake()
		{
			this.Entity = AttachToEntity();
		}


		protected Entity AttachToEntity()
		{
			Entity entity = null;
			if (Game.IsInitialized)
			{
				entity = Game.GetEntity(this);
				if (entity == null)
					entity = Game.EntityWorld.CreateEntity(this.gameObject.GetInstanceID()); 
			
				entity.AddComponent(this);	
			}
			else
			{
				Debug.Log("Game not initialized");
			}

			return entity;
		}
	}
}

#if UNITY_EDITOR
namespace PrimitiveEngine.Unity
{
	using UnityEditor;
	using UnityEngine;


	partial class EntityMonoBehaviour
	{
		[CustomEditor(typeof(EntityMonoBehaviour), editorForChildClasses: true)]
		public class UnityEntityComponentEditor : Editor
		{
			public override void OnInspectorGUI()
			{
				base.OnInspectorGUI();

				if (Application.isPlaying)
				{
					EditorGUILayout.Space();
					EntityMonoBehaviour entityMonoBehaviour = this.target as EntityMonoBehaviour;
					// ReSharper disable once PossibleNullReferenceException
					if (entityMonoBehaviour.Entity != null)
						EditorGUILayout.LabelField($"Entity ID: {entityMonoBehaviour.Entity.Id}");
				}
			}
		}
	}
}
#endif