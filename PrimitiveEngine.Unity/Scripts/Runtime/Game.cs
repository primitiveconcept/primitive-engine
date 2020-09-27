namespace PrimitiveEngine.Unity
{
	using System;
	using UnityEngine;


	public class Game : MonoBehaviour
	{
		public SystemsConfig SystemsConfig;

		private static Game _instance;
		private EntityWorld entityWorld;


		#region Properties
		public static EntityWorld EntityWorld
		{
			get { return Instance.entityWorld; }
		}


		public static bool IsInitialized
		{
			get { return Instance != null; }
		}


		private static Game Instance
		{
			get
			{
				if (_instance != null)
					return _instance;
				
				_instance = FindObjectOfType<Game>();
				if (_instance == null)
				{
					Debug.LogError("No Game instance found in scene.");	
				}
					
				_instance.InitializeMainEntityWorld();
				return _instance;
			}
		}
		#endregion


		public static Entity GetEntity<T>(T component)
			where T: MonoBehaviour, IEntityComponent
		{
			return EntityWorld.GetEntityById(component.gameObject.GetInstanceID());
		}


		public virtual void Awake()
		{
			if (_instance == null)
			{
				_instance = this;
				InitializeMainEntityWorld();
			}

			foreach (EntitySystem system in this.entityWorld.SystemManager.Systems)
			{
				Debug.Log($"Added system: {system.GetType().Name}");
			}
		}


		public virtual void FixedUpdate()
		{
			this.entityWorld.FixedUpdate();
		}


		public virtual void Update()
		{
			this.entityWorld.FrameUpdate();
		}


		protected virtual void InitializeMainEntityWorld()
		{
			this.entityWorld = new EntityWorld();

			if (this.SystemsConfig != null)
			{
				for (int systemOrder = 0; systemOrder < this.SystemsConfig.Systems.Length; systemOrder++)
				{
					SystemsConfig.Entry entry = this.SystemsConfig.Systems[systemOrder];
					Type type = Type.GetType(entry.SystemTypeName);
					this.entityWorld.SystemManager.InitializeSystem(
						type: type, 
						updateType: entry.UpdateType, 
						order: systemOrder, 
						executionType: entry.ExecutionType);
				}
			}
			else
			{
				this.entityWorld.InitializeWithAllSystems();	
			}
		}
	}
}

#if UNITY_EDITOR
namespace PrimitiveEngine.Unity
{
	using UnityEditor;


	[CustomEditor(typeof(Game))]
	class GameEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();
			EditorGUILayout.LabelField($"Entities: {Game.EntityWorld.EntityManager.ActiveEntities.Count}");
		}
	}
}
#endif