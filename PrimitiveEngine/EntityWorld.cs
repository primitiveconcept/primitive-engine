namespace PrimitiveEngine
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;


    /// <summary>
    /// The Entity World Class.
    /// Main interface of the Entity System.
    /// </summary>
    public sealed class EntityWorld
    {
        private readonly Bag<Entity> deleted;
        private readonly Dictionary<string, IEntityTemplate> entityTemplates;
        private readonly Dictionary<Type, IComponentPool<ComponentPoolable>> pools;
        private readonly HashSet<Entity> refreshed;
        private readonly BlackBoard blackBoard;
        private DateTime dateTime;
        private int poolCleanupDelayCounter;
        private bool isInitialized = false;


        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="EntityWorld" /> class.
        /// </summary>
        /// <param name="isSortedEntities">if set to <c>true</c> [is sorted entities].</param>
        /// <param name="processAttributes">if set to <c>true</c> [process attributes].</param>
        /// <param name="initializeAll">if set to <c>true</c> [initialize all]. If you pass true here, there will be no need to call EntityWorld.InitializeAll() method</param>
        public EntityWorld(bool isSortedEntities = false, bool processAttributes = true, bool initializeAll = false)
        {
            this.IsSortedEntities = isSortedEntities;
            this.refreshed = new HashSet<Entity>();
            this.pools = new Dictionary<Type, IComponentPool<ComponentPoolable>>();
            this.entityTemplates = new Dictionary<string, IEntityTemplate>();
            this.deleted = new Bag<Entity>();
            this.EntityManager = new EntityManager(this);
            this.SystemManager = new SystemManager(this);
            this.TagManager = new TagManager();
            this.GroupManager = new GroupManager();
            this.blackBoard = new BlackBoard();

            this.PoolCleanupDelay = 10;
            this.dateTime = FastDateTime.Now;
            if (initializeAll)
                InitializeWithAllSystems(processAttributes);
        }
        #endregion


        #region Properties
        /// <summary>
        /// Gets or sets the black board.
        /// </summary>
        /// <value>The black board.</value>
        public BlackBoard BlackBoard
        {
            get { return this.blackBoard; }
        }


        /// <summary>
        /// Gets the current state of the entity world.
        /// </summary>
        /// <value>The state of the current.</value>
        public Dictionary<Entity, Bag<IEntityComponent>> CurrentState
        {
            get
            {
                Bag<Entity> entities = this.EntityManager.ActiveEntities;
                Dictionary<Entity, Bag<IEntityComponent>> currentState = new Dictionary<Entity, Bag<IEntityComponent>>();
                for (int index = 0, j = entities.Count; index < j; ++index)
                {
                    Entity entity = entities.Get(index);
                    if (entity != null)
                    {
                        Bag<IEntityComponent> components = entity.Components;
                        currentState.Add(entity, components);
                    }
                }

                return currentState;
            }
        }


        /// <summary>
        /// Gets the delta time since last game loop in ticks.
        /// </summary>
        /// <value>The delta in ticks.</value>
        public long Delta { get; private set; }


        /// <summary>
        /// Gets the entity manager.
        /// </summary>
        /// <value>The entity manager.</value>
        public EntityManager EntityManager { get; private set; }


        /// <summary>
        /// Gets the group manager.
        /// </summary>
        /// <value>The group manager.</value>
        public GroupManager GroupManager { get; private set; }


        /// <summary>
        /// Gets or sets the interval in FrameUpdates between pools cleanup. Default is 10.
        /// </summary>
        /// <value>The pool cleanup delay.</value>
        public int PoolCleanupDelay { get; set; }


        /// <summary>
        /// Gets the system manager.
        /// </summary>
        /// <value>The system manager.</value>
        public SystemManager SystemManager { get; private set; }


        /// <summary>
        /// Gets the tag manager.
        /// </summary>
        /// <value>The tag manager.</value>
        public TagManager TagManager { get; private set; }


        /// <summary>
        /// Gets a value indicating whether this instance is sorted entities.
        /// </summary>
        /// <value><see langword="true" /> if this instance is sorted entities; otherwise, <see langword="false" />.</value>
        internal bool IsSortedEntities { get; private set; }
        #endregion


        /// <summary>
        /// Clears this instance.
        /// </summary>
        public void Clear()
        {
            foreach (
                Entity activeEntity in this.EntityManager.ActiveEntities.Where(activeEntity => activeEntity != null))
            {
                activeEntity.Delete();
            }

            FixedUpdate();
        }


        /// <summary>
        /// Creates the entity.
        /// </summary>
        /// <param name="uniqueId">The desired unique id of this Entity. If null, <c>Artemis</c> will create a unique ID.
        /// This value can be accessed by using the property uniqueID of the Entity</param>
        /// <returns>A new entity.</returns>
        public Entity CreateEntity(int? uniqueId = null)
        {
            return this.EntityManager.Create(uniqueId);
        }


        /// <summary>
        /// Creates an entity from template.
        /// </summary>
        /// <param name="entityTemplateTag">The entity template tag.</param>
        /// <param name="templateArgs">The template arguments.</param>
        /// <returns>The created entity.</returns>
        public Entity CreateEntityFromTemplate(string entityTemplateTag, params object[] templateArgs)
        {
            return CreateEntityFromTemplate(null, entityTemplateTag, templateArgs);
        }

        
        /// <summary>
        /// Create an entity from a template.
        /// </summary>
        /// <param name="entityUniqueId">ID to give new entity (assigns a new ID if null).</param>
        /// <param name="entityTemplate">Template instance to create the entity from.</param>
        /// <param name="templateArgs">Additional template arguments.</param>
        /// <returns></returns>
        public Entity CreateEntityFromTemplate(
            int? entityUniqueId,
            IEntityTemplate entityTemplate,
            params object[] templateArgs)
        {
            if (entityTemplate == null)
                throw new MissingEntityTemplateException("Entity template was null.");
            
            Entity entity = entityUniqueId != null
                                ? this.EntityManager.Create(entityUniqueId.Value)
                                : this.EntityManager.Create();
            entity = entityTemplate.BuildEntity(entity, this, templateArgs);
            RefreshEntity(entity);
            
            return entity;
        }


        /// <summary>
        /// Deletes the entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        public void DeleteEntity(Entity entity)
        {
            Debug.Assert(entity != null, "Entity must not be null.");

            this.deleted.Add(entity);
        }


        /// <summary>
        /// Updates the EntityWorld.
        /// </summary>
        public void FixedUpdate()
        {
            long deltaTicks = (FastDateTime.Now - this.dateTime).Ticks;
            this.dateTime = FastDateTime.Now;
            FixedUpdate(deltaTicks);
        }


        /// <summary>
        /// Updates the EntityWorld.
        /// </summary>
        /// <param name="deltaTicks">The delta ticks.</param>
        public void FixedUpdate(long deltaTicks)
        {
            this.Delta = deltaTicks;

            this.EntityManager.RemoveMarkedComponents();

            ++this.poolCleanupDelayCounter;
            if (this.poolCleanupDelayCounter > this.PoolCleanupDelay)
            {
                this.poolCleanupDelayCounter = 0;
                foreach (Type item in this.pools.Keys)
                {
                    this.pools[item].CleanUp();
                }
            }

            if (!this.deleted.IsEmpty)
            {
                for (int index = this.deleted.Count - 1; index >= 0; --index)
                {
                    Entity entity = this.deleted.Get(index);
                    this.TagManager.Unregister(entity);
                    this.GroupManager.Remove(entity);
                    this.EntityManager.Remove(entity);
                    entity.DeletingState = false;
                }

                this.deleted.Clear();
            }

            bool isRefreshing = this.refreshed.Count > 0;

            if (isRefreshing)
            {
                foreach (Entity entity in this.refreshed)
                {
                    this.EntityManager.Refresh(entity);
                    entity.RefreshingState = false;
                }

                this.refreshed.Clear();
            }

            this.SystemManager.FixedUpdate();
        }


        /// <summary>
        /// Draws the EntityWorld.
        /// </summary>
        public void FrameUpdate()
        {
            this.SystemManager.FrameUpdate();
        }


        /// <summary>
        /// Gets a component from a pool.
        /// </summary>
        /// <param name="type">The type of the object to get.</param>
        /// <returns>The found component.</returns>
        /// <exception cref="Exception">There is no pool for the specified type</exception>
        public IEntityComponent GetComponentFromPool(Type type)
        {
            Debug.Assert(type != null, "Type must not be null.");

            if (!this.pools.ContainsKey(type))
                throw new Exception("There is no pool for the specified type " + type);

            return this.pools[type].New();
        }


        /// <summary>
        /// Gets the component from pool.
        /// </summary>
        /// <typeparam name="T">Type of the component</typeparam>
        /// <returns>The found component.</returns>
        /// <exception cref="Exception">There is no pool for the type  + type</exception>
        public T GetComponentFromPool<T>() where T : ComponentPoolable
        {
            return (T)GetComponentFromPool(typeof(T));
        }


        public Entity GetEntityById(int uniqueId)
        {
            return this.EntityManager.GetEntityById(uniqueId);
        }


        /// <summary>
        /// Gets the entity by its index in the entity collection.
        /// </summary>
        /// <param name="entityIndex">The entity index.</param>
        /// <returns>The specified entity.</returns>
        public Entity GetEntityByIndex(int entityIndex)
        {
            Debug.Assert(entityIndex >= 0, "Index must be at least 0.");
            return this.EntityManager.GetEntityByIndex(entityIndex);
        }


        /// <summary>
        /// Gets the pool for a Type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>The specified ComponentPool{ComponentPool-able}.</returns>
        public IComponentPool<ComponentPoolable> GetPool(Type type)
        {
            Debug.Assert(type != null, "Type must not be null.");

            return this.pools[type];
        }


        /// <summary>
        /// Gets an EntitySystem from the EntityWorld.
        /// </summary>
        /// <typeparam name="T">Type derived from EntitySystem.</typeparam>
        /// <returns>EntitySystem type from the EntityWorld.</returns>
        public T GetSystem<T>()
            where T : EntitySystem
        {
            return this.SystemManager.GetSystem<T>();
        }


        /// <summary>
        /// Initialize the EntityWorld.
        /// </summary>
        /// <param name="assembliesToScan">The assemblies to scan for data attributes.</param>
        public void InitializeWithAllSystems(params Assembly[] assembliesToScan)
        {
            if (!this.isInitialized)
            {
                bool processAttributes = assembliesToScan != null && assembliesToScan.Length > 0;
                this.SystemManager.InitializeAll(processAttributes, assembliesToScan);
                this.isInitialized = true;
            }
        }


        /// <summary>
        /// Initializes the EntityWorld using all assemblies in the current AppDomain.
        /// </summary>
        public void InitializeWithAllSystems()
        {
            InitializeWithAllSystems(AppDomain.CurrentDomain.GetAssemblies());
        }


        /// <summary>
        /// Initialize the EntityWorld.
        /// Call this if you dont pass true in the parameter called InitializedALL in entity world constructor
        /// </summary>
        /// <param name="processAttributes">if set to <see langword="true" /> [process attributes].</param>
        public void InitializeWithAllSystems(bool processAttributes)
        {
            if (!this.isInitialized)
            {
                this.SystemManager.InitializeAll(processAttributes);
                this.isInitialized = true;
            }
        }


        /// <summary>
        /// Loads the state of the entity.
        /// </summary>
        /// <param name="templateTag">The template tag. Can be null.</param>
        /// <param name="groupName">Name of the group. Can be null.</param>
        /// <param name="components">The components.</param>
        /// <param name="templateArgs">Parameters for entity template.</param>
        /// <returns>The <see cref="Entity" />.</returns>
        public Entity LoadEntityState(
            string templateTag,
            string groupName,
            IEnumerable<IEntityComponent> components,
            params object[] templateArgs)
        {
            Debug.Assert(components != null, "Components must not be null.");

            Entity entity;
            if (!string.IsNullOrEmpty(templateTag))
                entity = CreateEntityFromTemplate(templateTag, -1, templateArgs);
            else
                entity = CreateEntity();

            if (!string.IsNullOrEmpty(groupName))
                this.GroupManager.Set(groupName, entity);

            foreach (IEntityComponent comp in components)
            {
                entity.AddComponent(comp);
            }

            return entity;
        }


        /// <summary>
        /// Sets the entity template.
        /// </summary>
        /// <param name="entityTag">The entity tag.</param>
        /// <param name="entityTemplate">The entity template.</param>
        public void SetEntityTemplate(string entityTag, IEntityTemplate entityTemplate)
        {
            this.entityTemplates.Add(entityTag, entityTemplate);
        }


        /// <summary>
        /// Sets the pool for a specific type
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="pool">The pool.</param>
        public void SetPool(Type type, IComponentPool<ComponentPoolable> pool)
        {
            Debug.Assert(type != null, "Type must not be null.");
            Debug.Assert(pool != null, "Component pool must not be null.");

            this.pools.Add(type, pool);
        }


        /// <summary>
        /// Unloads the content.
        /// </summary>
        public void UnloadContent()
        {
            this.SystemManager.TerminateAll();
        }


        /// <summary>
        /// Creates an entity from template.
        /// </summary>
        /// <param name="entityUniqueId">The entity unique id.</param>
        /// <param name="entityTemplateTag">The entity template tag.</param>
        /// <param name="templateArgs">The template arguments.</param>
        /// <returns>The Entity.</returns>
        private Entity CreateEntityFromTemplate(
            int? entityUniqueId,
            string entityTemplateTag,
            params object[] templateArgs)
        {
            Debug.Assert(!string.IsNullOrEmpty(entityTemplateTag), "Entity template tag must not be null or empty.");
            this.entityTemplates.TryGetValue(entityTemplateTag, out IEntityTemplate entityTemplate);
            
            return CreateEntityFromTemplate(entityUniqueId, entityTemplate, templateArgs);
        }


        /// <summary>
        /// Refreshes the entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        internal void RefreshEntity(Entity entity)
        {
            Debug.Assert(entity != null, "Entity must not be null.");
            this.refreshed.Add(entity);
        }
    }
}