namespace PrimitiveEngine
{
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Numerics;


	/// <summary>
	/// Base of all Entity Systems.
	/// Provide basic functionalities.
	/// </summary>
	public abstract class EntitySystem
	{
		protected EntityWorld entityWorld;

		private readonly Aspect aspect;

		private IDictionary<int, Entity> actives;


		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="EntitySystem" /> class.
		/// </summary>
		protected EntitySystem()
		{
			this.Bit = 0;
			this.aspect = Aspect.Empty();
			this.IsEnabled = true;
		}


		/// <summary>
		/// Initializes a new instance of the <see cref="EntitySystem"/> class.
		/// </summary>
		/// <param name="aspect">The aspect.</param>
		protected EntitySystem(Aspect aspect)
			: this()
		{
			Debug.Assert(aspect != null, "Aspect must not be null.");
			this.aspect = aspect;
		}
		#endregion


		#region Properties
		/// <summary>
		/// Gets all active Entities for this system.
		/// </summary>
		public IEnumerable<Entity> ActiveEntities
		{
			get { return this.actives.Values; }
		}


		/// <summary>
		/// Gets the aspect.
		/// </summary>
		public Aspect Aspect
		{
			get { return this.aspect; }
		}


		/// <summary>
		/// Gets or sets the black board.
		/// </summary>
		/// <value>The black board.</value>
		public BlackBoard BlackBoard
		{
			get { return this.entityWorld.BlackBoard; }
		}


		/// <summary>
		/// Gets or sets the entity world.
		/// </summary>
		/// <value>The entity world.</value>
		public EntityWorld EntityWorld
		{
			get { return this.entityWorld; }

			protected internal set
			{
				this.entityWorld = value;
				if (this.EntityWorld.IsSortedEntities)
					this.actives = new SortedDictionary<int, Entity>();
				else
					this.actives = new Dictionary<int, Entity>();
			}
		}


		/// <summary>
		/// Gets or sets a value indicating whether this instance is enabled.
		/// </summary>
		/// <value><see langword="true" /> if this instance is enabled; otherwise, <see langword="false" />.</value>
		public bool IsEnabled { get; set; }


		/// <summary>
		/// Gets or sets the system bit. (Setter only).
		/// </summary>
		/// <value>The system bit.</value>
		internal BigInteger Bit { get; set; }
		#endregion


		/// <summary>
		/// Override to implement code that gets executed when systems are initialized.
		/// </summary>
		public virtual void LoadContent() {}


		/// <summary>
		/// Called when [added].
		/// </summary>
		/// <param name="entity">The entity.</param>
		public virtual void OnEntityAdded(Entity entity) {}


		/// <summary>
		/// Called when [change].
		/// </summary>
		/// <param name="entity">The entity.</param>
		public virtual void OnEntityChanged(Entity entity)
		{
			Debug.Assert(entity != null, "Entity must not be null.");

			bool contains = (this.Bit & entity.SystemBits) == this.Bit;
			////bool interest = (this.typeFlags & entity.TypeBits) == this.typeFlags;
			bool interest = this.Aspect.Interests(entity);

			if (interest && !contains)
				Add(entity);
			else if (!interest && contains)
				Remove(entity);
			else if (interest
					&& contains
					&& entity.IsEnabled)
				Enable(entity);
			else if (interest
					&& contains
					&& !entity.IsEnabled)
				Disable(entity);
		}


		/// <summary>
		/// Called when [disabled].
		/// </summary>
		/// <param name="entity">The entity.</param>
		public virtual void OnEntityDisabled(Entity entity) {}


		/// <summary>
		/// Called when [enabled].
		/// </summary>
		/// <param name="entity">The entity.</param>
		public virtual void OnEntityEnabled(Entity entity) {}


		/// <summary>
		/// Called when [removed].
		/// </summary>
		/// <param name="entity">The entity.</param>
		public virtual void OnEntityRemoved(Entity entity) {}


		/// <summary>
		/// Processes this instance.
		/// </summary>
		public virtual void Process()
		{
			if (CheckProcessing())
			{
				Begin();
				ProcessEntities(this.actives);
				End();
			}
		}


		/// <summary>
		/// Toggles this instance.
		/// </summary>
		public void Toggle()
		{
			this.IsEnabled = !this.IsEnabled;
		}


		/// <summary>
		/// Override to implement code that gets executed when systems are terminated.
		/// </summary>
		public virtual void UnloadContent() {}


		/// <summary>
		/// Adds the specified entity.
		/// </summary>
		/// <param name="entity">The entity.</param>
		protected void Add(Entity entity)
		{
			Debug.Assert(entity != null, "Entity must not be null.");

			entity.AddSystemBit(this.Bit);
			if (entity.IsEnabled)
				Enable(entity);

			OnEntityAdded(entity);
		}


		/// <summary>
		/// Begins this instance processing.
		/// </summary>
		protected virtual void Begin() {}


		/// <summary>
		/// Checks the processing.
		/// </summary>
		/// <returns><see langword="true" /> if this instance is enabled, <see langword="false" /> otherwise</returns>
		protected virtual bool CheckProcessing()
		{
			return this.IsEnabled;
		}


		/// <summary>
		/// Ends this instance processing.
		/// </summary>
		protected virtual void End() {}


		/// <summary>
		/// Interests in the specified entity.
		/// </summary>
		/// <param name="entity">The entity.</param>
		/// <returns><see langword="true" /> if any interests in entity, <see langword="false" /> otherwise</returns>
		protected virtual bool Interests(Entity entity)
		{
			return this.Aspect.Interests(entity);
		}


		/// <summary>
		/// Processes the entities.
		/// </summary>
		/// <param name="entities">The entities.</param>
		protected virtual void ProcessEntities(IDictionary<int, Entity> entities) {}


		/// <summary>
		/// Removes the specified entity.
		/// </summary>
		/// <param name="entity">The entity.</param>
		protected void Remove(Entity entity)
		{
			Debug.Assert(entity != null, "Entity must not be null.");

			entity.RemoveSystemBit(this.Bit);
			if (entity.IsEnabled)
				Disable(entity);

			OnEntityRemoved(entity);
		}


		/// <summary>
		/// Disables the specified entity.
		/// </summary>
		/// <param name="entity">The entity.</param>
		private void Disable(Entity entity)
		{
			Debug.Assert(entity != null, "Entity must not be null.");

			if (!this.actives.ContainsKey(entity.Index))
				return;

			this.actives.Remove(entity.Index);
			OnEntityDisabled(entity);
		}


		/// <summary>
		/// Enables the specified entity.
		/// </summary>
		/// <param name="entity">The entity.</param>
		public void Enable(Entity entity)
		{
			Debug.Assert(entity != null, "Entity must not be null.");

			if (this.actives.ContainsKey(entity.Index))
				return;

			this.actives.Add(entity.Index, entity);
			OnEntityEnabled(entity);
		}
	}
}