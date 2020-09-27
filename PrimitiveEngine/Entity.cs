namespace PrimitiveEngine
{
	using System;
	using System.Diagnostics;
	using System.Numerics;


	/// <summary>
	/// Basic unity of this entity system.
	/// </summary>
	public sealed class Entity
	{
		private readonly EntityManager entityManager;
		private readonly EntityWorld entityWorld;
		private int id;


		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="Entity"/> class.
		/// </summary>
		/// <param name="entityWorld">The entity world.</param>
		/// <param name="index">The id.</param>
		internal Entity(EntityWorld entityWorld, int index)
		{
			this.SystemBits = 0;
			this.TypeBits = 0;
			this.IsEnabled = true;
			this.entityWorld = entityWorld;
			this.entityManager = entityWorld.EntityManager;
			this.Index = index;
		}
		#endregion


		#region Properties
		/// <summary>
		///   Gets all components belonging to this entity.
		///   Warning: Use only for debugging purposes, it is dead slow.
		///   The returned bag is only valid until this method is called
		///   again, then it is overwritten.
		/// </summary>
		/// <value>All components of this entity.</value>
		public Bag<IEntityComponent> Components
		{
			get { return this.entityManager.GetComponents(this); }
		}


		/// <summary>
		/// Gets or sets a value indicating whether [deleting state].
		/// </summary>
		/// <value>
		/// <see langword="true" /> if [deleting state]; otherwise, <see langword="false" />.
		/// </value>
		public bool DeletingState { get; set; }


		/// <summary>
		/// Gets the entity world.
		/// </summary>
		/// <value>The entity world.</value>
		public EntityWorld EntityWorld
		{
			get { return this.entityWorld; }
		}


		/// <summary>
		/// Gets or sets the group.
		/// </summary>
		/// <value>The group.</value>
		public string Group
		{
			get { return this.entityWorld.GroupManager.GetGroupOf(this); }

			set { this.entityWorld.GroupManager.Set(value, this); }
		}


		/// <summary>
		/// Gets the unique id.
		/// </summary>
		/// <value>The unique id.</value>
		public int Id
		{
			get { return this.id; }

			internal set
			{
				this.id = value;
			}
		}


		/// <summary>
		/// Gets the internal index for this entity within the framework.
		/// No other entity will have the same index,
		/// but indexes are however reused so another entity may acquire
		/// this index if the previous entity was deleted.
		/// </summary>
		/// <value>The index.</value>
		public int Index { get; private set; }


		/// <summary>
		/// Gets a value indicating whether this instance is active.
		/// </summary>
		/// <value>
		/// <see langword="true" /> if this instance is active; otherwise, <see langword="false" />.
		/// </value>
		public bool IsActive
		{
			get { return this.entityManager.IsActive(this.Index); }
		}


		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="Entity" /> is enabled.
		/// </summary>
		/// <value>
		/// <see langword="true" /> if enabled; otherwise, <see langword="false" />.
		/// </value>
		public bool IsEnabled { get; set; }


		/// <summary>
		/// Gets or sets a value indicating whether [refreshing state].
		/// </summary>
		/// <value>
		/// <see langword="true" /> if [refreshing state]; otherwise, <see langword="false" />.
		/// </value>
		public bool RefreshingState { get; set; }


		/// <summary>
		/// Gets or sets the tag.
		/// </summary>
		/// <value>The tag.</value>
		public string Tag
		{
			get { return this.entityWorld.TagManager.GetTagOfEntity(this); }

			set
			{
				string oldTag = this.entityWorld.TagManager.GetTagOfEntity(this);
				if (value != oldTag)
				{
					if (oldTag != null)
						this.entityWorld.TagManager.Unregister(this);

					if (value != null)
						this.entityWorld.TagManager.Register(value, this);
				}
			}
		}


		/// <summary>
		/// Gets or sets the system bits.
		/// </summary>
		/// <value>The system bits.</value>
		internal BigInteger SystemBits { get; set; }


		/// <summary>
		/// Gets or sets the type bits.
		/// </summary>
		/// <value>The type bits.</value>
		internal BigInteger TypeBits { get; set; }
		#endregion


		/// <summary>
		/// Adds the component.
		/// </summary>
		/// <param name="component">The component.</param>
		public void AddComponent(IEntityComponent component)
		{
			Debug.Assert(component != null, "Component must not be null.");

			this.entityManager.AddComponent(this, component);
		}


		/* TODO: This doesn't use the static ComponentType field of the most child class T. 
		/// <summary>
		/// Adds the component.
		/// </summary>
		/// <typeparam name="T">The <see langword="Type"/> T.</typeparam>
		/// <param name="component">The component.</param>
		public void AddComponent<T>(T component) where T : IEntityComponent
		{
			Debug.Assert(component != null, "Component must not be null.");
			this.entityManager.AddComponent<T>(this, component);
		}
		*/


		/// <summary>
		/// Adds the component from pool.
		/// </summary>
		/// <typeparam name="T">The <see langword="Type"/> T.</typeparam>
		/// <returns>The added component.</returns>
		public T AddComponentFromPool<T>() where T : ComponentPoolable
		{
			T component = this.entityWorld.GetComponentFromPool<T>();
			this.entityManager.AddComponent<T>(this, component);
			return component;
		}


		/// <summary>
		/// Gets the component from pool, runs init delegate, then adds the components to the entity.
		/// </summary>
		/// <typeparam name="T">The <see langword="Type"/> T.</typeparam>
		/// <returns>The added component.</returns>
		public void AddComponentFromPool<T>(Action<T> init) where T : ComponentPoolable
		{
			Debug.Assert(init != null, "Init delegate must not be null.");

			T component = this.entityWorld.GetComponentFromPool<T>();
			init(component);
			this.entityManager.AddComponent<T>(this, component);
		}


		/// <summary>
		/// Checks whether the given component can be reset.
		/// </summary>
		/// <typeparam name="T">the component type.</typeparam>
		public bool CanReset<T>() where T : IEntityComponent
		{
			return this.entityManager.CanReset (this, ComponentType<T>.CType);
		}


		/// <summary>
		/// Checks whether the given component can be reset.
		/// </summary>
		/// <param name="componentType">Type of the component.</param>
		public bool CanReset(Type componentType) 
		{
			return this.entityManager.CanReset (this, ComponentTypeManager.GetTypeFor(componentType));
		}


		/// <summary>
		/// Deletes this instance.
		/// </summary>
		public void Delete()
		{
			if (this.DeletingState)
				return;

			this.entityWorld.DeleteEntity(this);
			this.DeletingState = true;
		}


		/// <summary>
		/// Gets the component.
		/// Slower retrieval of components from this entity.
		/// Minimize usage of this, but is fine to use e.g. when
		/// creating new entities and setting data in components.
		/// </summary>
		/// <param name="componentType">Type of the component.</param>
		/// <returns>component that matches, or null if none is found.</returns>
		public IEntityComponent GetComponent(ComponentType componentType)
		{
			Debug.Assert(componentType != null, "Component type must not be null.");

			return this.entityManager.GetComponent(this, componentType);
		}


		/// <summary>
		/// Gets the component.
		/// This is the preferred method to use when
		/// retrieving a component from a entity.
		/// It will provide good performance.
		/// </summary>
		/// <typeparam name="T">the expected return component type.</typeparam>
		/// <returns>component that matches, or null if none is found.</returns>
		public T GetComponent<T>() where T : IEntityComponent
		{
			return (T)this.entityManager.GetComponent(this, ComponentType<T>.CType);
		}


		/// <summary>
		/// Determines whether this instance has a specific component.
		/// </summary>
		/// <typeparam name="T">The <see langword="Type"/> T.</typeparam>
		/// <returns><see langword="true" /> if this instance has a specific component; otherwise, <see langword="false" />.</returns>
		public bool HasComponent<T>() where T : IEntityComponent
		{
			return !Equals((T)this.entityManager.GetComponent(this, ComponentType<T>.CType), default(T));
		}


		/// <summary>
		///   Refreshes this instance.
		///   Refresh all changes to components for this entity.
		///   After adding or removing components,
		///   you must call this method.
		///   It will update all relevant systems.
		///   It is typical to call this after adding components
		///   to a newly created entity.
		/// </summary>
		public void Refresh()
		{
			if (this.RefreshingState)
				return;

			this.entityWorld.RefreshEntity(this);
			this.RefreshingState = true;
		}


		/// <summary>
		/// Marks the component to remove. The actual removal is deferred and will happen in the next EntityWorld update.
		/// </summary>
		/// <typeparam name="T">Component Type.</typeparam>
		public void RemoveComponent<T>() where T : IEntityComponent
		{
			this.entityManager.RemoveComponent(this, ComponentTypeManager.GetTypeFor<T>());
		}


		/// <summary>
		///   Marks the component to remove. The actual removal is deferred and will happen in the next EntityWorld update.
		///   Faster removal of components from a entity.
		/// </summary>
		/// <param name="componentType">The type.</param>
		public void RemoveComponent(Type componentType)
		{
			this.entityManager.RemoveComponent(this, ComponentTypeManager.GetTypeFor(componentType));
		}


		/// <summary>
		///   Marks the component to remove. The actual removal is deferred and will happen in the next EntityWorld update.
		///   Faster removal of components from a entity.
		/// </summary>
		/// <param name="componentType">The type.</param>
		public void RemoveComponent(ComponentType componentType)
		{
			Debug.Assert(componentType != null, "Component type must not be null.");

			this.entityManager.RemoveComponent(this, componentType);
		}


		/// <summary>
		/// Resets this instance.
		/// </summary>
		public void Reset()
		{
			this.SystemBits = 0;
			this.TypeBits = 0;
			this.IsEnabled = true;
		}


		/// <summary>
		/// Resets the component.
		/// This is the preferred method to use when
		/// resetting a component to its original state when first added.
		/// </summary>
		/// <typeparam name="T">The component base type.</typeparam>
		/// <param name="args"></param>
		public void ResetComponent<T>(params object[] args) where T : IEntityComponent
		{
			this.entityManager.ResetComponent(this, ComponentType<T>.CType, args);
		}


		/// <summary>
		/// Resets the component.
		/// This is the preferred method to use when
		/// resetting a component to its original state when first added.
		/// </summary>
		/// <param name="componentType">The component base type.</param>
		/// <param name="args"></param>
		public void ResetComponent(ComponentType componentType, params object[] args)
		{
			this.entityManager.ResetComponent(this, componentType);
		}


		/// <summary>
		/// Returns a <see cref="string" /> that represents this instance.
		/// </summary>
		/// <returns>A <see cref="string" /> that represents this instance.</returns>
		public override string ToString()
		{
			return string.Format("Entity{{{0}}}", this.Index);
		}


		/// <summary>
		/// Adds the system bit.
		/// </summary>
		/// <param name="bit">The bit.</param>
		internal void AddSystemBit(BigInteger bit)
		{
			this.SystemBits |= bit;
		}


		/// <summary>
		/// Adds the type bit.
		/// </summary>
		/// <param name="bit">The bit.</param>
		internal void AddTypeBit(BigInteger bit)
		{
			this.TypeBits |= bit;
		}


		/// <summary>
		/// Removes the system bit.
		/// </summary>
		/// <param name="bit">The bit.</param>
		internal void RemoveSystemBit(BigInteger bit)
		{
			this.SystemBits &= ~bit;
		}


		/// <summary>
		/// Removes the type bit.
		/// </summary>
		/// <param name="bit">The bit.</param>
		internal void RemoveTypeBit(BigInteger bit)
		{
			this.TypeBits &= ~bit;
		}
	}
}