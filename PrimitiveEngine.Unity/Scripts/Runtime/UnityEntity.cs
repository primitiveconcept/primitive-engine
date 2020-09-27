namespace PrimitiveEngine.Unity
{
	using System;
	using System.Collections.Generic;
	using UnityEngine;


	public partial class UnityEntity : MonoBehaviour
	{
		[ReorderableList(
			elementHeaderFormat = "Entity Components",
			disableAdding = true)]
		[SerializeReference]
		private List<IEntityComponent> components = new List<IEntityComponent>();

		private Entity entity;


		/// <summary>
		/// Add a pre-instantiated IEntityComponent to the entity.
		/// </summary>
		/// <param name="component">Instantiated component to add.</param>
		public void AddComponent(IEntityComponent component)
		{
			this.entity.AddComponent(component);
		}


		/// <summary>
		/// Create and add a new IEntityComponent to the entity. 
		/// </summary>
		/// <typeparam name="T">Type of component to add.</typeparam>
		/// <returns>New component instance that was added.</returns>
		public IEntityComponent AddComponent<T>()
			where T : IEntityComponent
		{
			return AddComponent(typeof(T));
		}


		public void Awake()
		{
			this.entity = Game.EntityWorld.CreateEntity(this.gameObject.GetInstanceID());

			for (int i = 0; i < this.components.Count; i++)
			{
				this.entity.AddComponent(this.components[i]);
			}
		}


		/// <summary>
		/// Remove a component from the entity.
		/// </summary>
		/// <typeparam name="T">Type of component to remove.</typeparam>
		/// <returns>True if component was found and removed.</returns>
		public bool RemoveComponent<T>()
			where T : IEntityComponent
		{
			return RemoveComponent(typeof(T));
		}


		private IEntityComponent AddComponent(Type componentType)
		{
			IEntityComponent instance = (IEntityComponent)Activator.CreateInstance(componentType);
			this.components.Add(instance);
			
			if (Application.isPlaying)
				this.entity.AddComponent(instance);
			
			
			return instance;
		}


		private bool RemoveComponent(Type componentType)
		{
			for (int i = 0; i < this.components.Count; i++)
			{
				if (this.components[i].GetType() == componentType)
				{
					this.components.RemoveAt(i);
					this.entity.RemoveComponent(componentType);
					return true;
				}
			}

			return false;
		}
	}
}