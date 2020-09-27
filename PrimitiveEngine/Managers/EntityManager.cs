namespace PrimitiveEngine
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;


    /// <summary>
    /// The Entity Manager.
    /// </summary>
    public sealed class EntityManager
    {
        private readonly Bag<Bag<IEntityComponent>> componentsByType;
        private readonly EntityWorld entityWorld;
        private readonly Bag<Entity> removedAndAvailable;
        private readonly Bag<int> identifierPool;
        private readonly Dictionary<int, Entity> uniqueIdToEntities;
        private readonly HashSet<Tuple<Entity, ComponentType>> componentsToBeRemoved =
            new HashSet<Tuple<Entity, ComponentType>>();

        private int nextAvailableId;


        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="EntityManager" /> class.
        /// </summary>
        /// <param name="entityWorld">The entity world.</param>
        public EntityManager(EntityWorld entityWorld)
        {
            Debug.Assert(entityWorld != null, "EntityWorld must not be null.");

            this.uniqueIdToEntities = new Dictionary<int, Entity>();
            this.removedAndAvailable = new Bag<Entity>();
            this.componentsByType = new Bag<Bag<IEntityComponent>>();
            this.ActiveEntities = new Bag<Entity>();
            this.identifierPool = new Bag<int>(4);
            this.RemovedEntitiesRetention = 100;
            this.entityWorld = entityWorld;
            RemovedComponentEvent += EntityManagerRemovedComponentEvent;
        }
        #endregion


        /// <summary>
        /// Occurs when [added component event].
        /// </summary>
        public event AddedComponentHandler AddedComponentEvent;


        /// <summary>
        /// Occurs when [added entity event].
        /// </summary>
        public event AddedEntityHandler AddedEntityEvent;


        /// <summary>
        /// Occurs when [removed component event].
        /// </summary>
        public event RemovedComponentHandler RemovedComponentEvent;


        /// <summary>
        /// Occurs when [removed entity event].
        /// </summary>
        public event RemovedEntityHandler RemovedEntityEvent;


        #region Properties
        /// <summary>
        /// Gets all active Entities.
        /// </summary>
        /// <value>The active entities.</value>
        /// <returns>Bag of active entities.</returns>
        public Bag<Entity> ActiveEntities { get; private set; }


        /// <summary>
        /// Gets or sets the removed entities retention.
        /// </summary>
        /// <value>The removed entities retention.</value>
        public int RemovedEntitiesRetention { get; set; }
        #endregion


        /// <summary>
        /// Create a new, "blank" entity
        /// </summary>
        /// <param name="entityId">The entity instance id.</param>
        /// <param name="uniqueId">The unique id.</param>
        public Entity Create(int? uniqueId = null)
        {
            int id = uniqueId.HasValue 
                          ? uniqueId.Value 
                          : BitConverter.ToInt32(Guid.NewGuid().ToByteArray(), 0);

            Entity result = this.removedAndAvailable.RemoveLast();
            if (result == null)
            {
                int entityId = this.identifierPool.IsEmpty
                                   ? this.nextAvailableId++
                                   : this.identifierPool.RemoveLast();

                result = new Entity(this.entityWorld, entityId);
            }
            else
            {
                result.Reset();
            }

            result.Id = id;
            this.uniqueIdToEntities[result.Id] = result;
            this.ActiveEntities.Set(result.Index, result);

            if (AddedEntityEvent != null)
            {
                AddedEntityEvent(result);
            }

            return result;
        }


        /// <summary>
        /// Get all components assigned to an entity.
        /// </summary>
        /// <param name="entity">Entity for which you want the components.</param>
        /// <returns>Bag of components</returns>
        public Bag<IEntityComponent> GetComponents(Entity entity)
        {
            Debug.Assert(entity != null, "Entity must not be null.");
            //Debug.Assert(entity.entityManager == this, "");  // TODO

            Bag<IEntityComponent> entityComponents = new Bag<IEntityComponent>();
            int entityId = entity.Index;
            for (int index = 0, b = this.componentsByType.Count; b > index; ++index)
            {
                Bag<IEntityComponent> components = this.componentsByType.Get(index);
                if (components != null
                    && entityId < components.Count)
                {
                    IEntityComponent component = components.Get(entityId);
                    if (component != null)
                        entityComponents.Add(component);
                }
            }

            return entityComponents;
        }


        /// <summary>
        /// Gets the entities.
        /// </summary>
        /// <param name="aspect">The aspect.</param>
        /// <returns>The filled Bag{Entity}.</returns>
        public Bag<Entity> GetEntities(Aspect aspect)
        {
            Bag<Entity> entitiesBag = new Bag<Entity>();
            for (int index = 0; index < this.ActiveEntities.Count; ++index)
            {
                Entity entity = this.ActiveEntities.Get(index);
                if (entity != null
                    && aspect.Interests(entity))
                    entitiesBag.Add(entity);
            }

            return entitiesBag;
        }


        /// <summary>
        /// Gets the entity by unique ID.
        /// </summary>
        /// <param name="entityUniqueId">The entity unique id.</param>
        /// <returns>The Entity.</returns>
        public Entity GetEntityById(int entityUniqueId)
        {
            this.uniqueIdToEntities.TryGetValue(entityUniqueId, out Entity entity);
            return entity;
        }


        /// <summary>
        /// Get the entity for the given entityId
        /// </summary>
        /// <param name="entityIndex">Desired EntityId</param>
        /// <returns>The specified Entity.</returns>
        public Entity GetEntityByIndex(int entityIndex)
        {
            Debug.Assert(entityIndex >= 0, "Id must be at least 0.");

            return this.ActiveEntities.Get(entityIndex);
        }


        /// <summary>
        /// Check if this entity is active, or has been deleted, within the framework.
        /// </summary>
        /// <param name="entityIndex">The entity id.</param>
        /// <returns><see langword="true" /> if the specified entity is active; otherwise, <see langword="false" />.</returns>
        public bool IsActive(int entityIndex)
        {
            return this.ActiveEntities.Get(entityIndex) != null;
        }


        /// <summary>
        /// Remove an entity from the entityWorld.
        /// </summary>
        /// <param name="entity">Entity you want to remove.</param>
        public void Remove(Entity entity)
        {
            Debug.Assert(entity != null, "Entity must not be null.");

            this.ActiveEntities.Set(entity.Index, null);
            RemoveComponentsOfEntity(entity);

            if (this.removedAndAvailable.Count < this.RemovedEntitiesRetention)
                this.removedAndAvailable.Add(entity);
            else
                this.identifierPool.Add(entity.Index);

            if (RemovedEntityEvent != null)
                RemovedEntityEvent(entity);

            this.uniqueIdToEntities.Remove(entity.Id);
        }


        /// <summary>Check if a component on a given entity can reset.</summary>
        /// <param name="entity">The entity for which you want to get the component</param>
        /// <param name="componentType">The desired component type</param>
        internal bool CanReset(Entity entity, ComponentType componentType)
        {
            Debug.Assert(entity != null, "Entity must not be null.");
            Debug.Assert(componentType != null, "Component type must not be null.");

            IResettableComponent component = GetComponent (entity, componentType) as IResettableComponent;
            if (component != null) {
                return true;
            } 

            return false;
        }


        /// <summary>Resets the component of the given component type for the given entity by overwriting it with
        /// a new one.</summary>
        /// <param name="entity">The entity for which you want to get the component</param>
        /// <param name="componentType">The desired component type</param>
        /// <param name="args">The initialize parameters</param>
        internal void ResetComponent(
            Entity entity, 
            ComponentType componentType, 
            params object[] args)
        {
            Debug.Assert(entity != null, "Entity must not be null.");
            Debug.Assert(componentType != null, "Component type must not be null.");

            IResettableComponent component =
                GetComponent(entity, componentType) as IResettableComponent;
            if (component != null) {
                component.Reset (args);
            }
        }


        /// <summary>
        /// Adds the component.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="component">The component.</param>
        /// <param name="type">The type.</param>
        internal void AddComponent(Entity entity, IEntityComponent component, ComponentType type)
        {
            if (type.Id >= this.componentsByType.Capacity)
                this.componentsByType.Set(type.Id, null);

            Bag<IEntityComponent> components = this.componentsByType.Get(type.Id);
            if (components == null)
            {
                components = new Bag<IEntityComponent>();
                this.componentsByType.Set(type.Id, components);
            }

            components.Set(entity.Index, component);

            entity.AddTypeBit(type.Bit);
            if (AddedComponentEvent != null)
                AddedComponentEvent(entity, component);

            this.entityWorld.RefreshEntity(entity);
        }


        /// <summary>
        /// Entities the manager removed component event.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="component">The component.</param>
        internal void EntityManagerRemovedComponentEvent(Entity entity, IEntityComponent component)
        {
            ComponentPoolable componentPoolable = component as ComponentPoolable;
            if (componentPoolable != null)
            {
                if (componentPoolable.PoolId < 0)
                    return;

                IComponentPool<ComponentPoolable> pool = this.entityWorld.GetPool(component.GetType());
                if (pool != null)
                    pool.ReturnObject(componentPoolable);
            }
        }


        /// <summary>
        /// Strips all components from the given entity.
        /// </summary>
        /// <param name="entity">Entity for which you want to remove all components</param>
        internal void RemoveComponentsOfEntity(Entity entity)
        {
            Debug.Assert(entity != null, "Entity must not be null.");

            entity.TypeBits = 0;
            this.entityWorld.RefreshEntity(entity);

            int entityId = entity.Index;
            for (int index = this.componentsByType.Count - 1; index >= 0; --index)
            {
                Bag<IEntityComponent> components = this.componentsByType.Get(index);
                if (components != null
                    && entityId < components.Count)
                {
                    IEntityComponent componentToBeRemoved = components.Get(entityId);
                    if (RemovedComponentEvent != null
                        && componentToBeRemoved != null)
                        RemovedComponentEvent(entity, componentToBeRemoved);

                    components.Set(entityId, null);
                }
            }
        }


        /// <summary>
        /// Add the given component to the given entity.
        /// </summary>
        /// <param name="entity">Entity for which you want to add the component.</param>
        /// <param name="component">Component you want to add.</param>
        internal void AddComponent(Entity entity, IEntityComponent component)
        {
            Debug.Assert(entity != null, "Entity must not be null.");
            Debug.Assert(component != null, "Component must not be null.");

            ComponentType type = ComponentTypeManager.GetTypeFor(component.GetType());

            AddComponent(entity, component, type);
        }


        /// <summary>
        /// Add a component to the given entity.
        /// If the component's type does not already exist,
        /// add it to the bag of available component types.
        /// </summary>
        /// <typeparam name="T">Component type you want to add.</typeparam>
        /// <param name="entity">The entity to which you want to add the component.</param>
        /// <param name="component">The component instance you want to add.</param>
        internal void AddComponent<T>(Entity entity, IEntityComponent component) where T : IEntityComponent
        {
            Debug.Assert(entity != null, "Entity must not be null.");
            Debug.Assert(component != null, "Component must not be null.");

            ComponentType type = ComponentTypeManager.GetTypeFor<T>();

            AddComponent(entity, component, type);
        }


        /// <summary>
        /// Get the component instance of the given component type for the given entity.
        /// </summary>
        /// <param name="entity">The entity for which you want to get the component</param>
        /// <param name="componentType">The desired component type</param>
        /// <returns>Component instance</returns>
        internal IEntityComponent GetComponent(Entity entity, ComponentType componentType)
        {
            Debug.Assert(entity != null, "Entity must not be null.");
            Debug.Assert(componentType != null, "Component type must not be null.");

            if (componentType.Id >= this.componentsByType.Capacity)
                return null;

            int entityId = entity.Index;
            Bag<IEntityComponent> bag = this.componentsByType.Get(componentType.Id);

            if (bag != null
                && entityId < bag.Capacity)
                return bag.Get(entityId);

            return null;
        }


        /// <summary>
        /// Ensure the any changes to components are synced up with the entity - ensure systems "see" all components.
        /// </summary>
        /// <param name="entity">The entity whose components you want to refresh</param>
        internal void Refresh(Entity entity)
        {
            SystemManager systemManager = this.entityWorld.SystemManager;
            Bag<EntitySystem> systems = systemManager.Systems;
            for (int index = 0, s = systems.Count; s > index; ++index)
            {
                systems.Get(index).OnEntityChanged(entity);
            }
        }


        /// <summary>
        /// Removes the given component from the given entity.
        /// </summary>
        /// <typeparam name="T">The type of the component you want to remove.</typeparam>
        /// <param name="entity">The entity for which you are removing the component.</param>
        internal void RemoveComponent<T>(Entity entity) where T : IEntityComponent
        {
            RemoveComponent(entity, ComponentType<T>.CType);
        }


        /// <summary>
        /// Removes the given component type from the given entity.
        /// </summary>
        /// <param name="entity">The entity for which you want to remove the component.</param>
        /// <param name="componentType">The component type you want to remove.</param>
        internal void RemoveComponent(Entity entity, ComponentType componentType)
        {
            Debug.Assert(entity != null, "Entity must not be null.");
            Debug.Assert(componentType != null, "Component type must not be null.");

            Tuple<Entity, ComponentType> pair = new Tuple<Entity, ComponentType>(entity, componentType);
            this.componentsToBeRemoved.Add(pair);
        }


        /// <summary>
        /// Removes components marked for removal.
        /// </summary>
        internal void RemoveMarkedComponents()
        {
            foreach (Tuple<Entity, ComponentType> pair in this.componentsToBeRemoved)
            {
                Entity entity = pair.Item1;
                ComponentType componentType = pair.Item2;

                int entityId = entity.Index;
                Bag<IEntityComponent> components = this.componentsByType.Get(componentType.Id);

                if (components != null
                    && entityId < components.Count)
                {
                    IEntityComponent componentToBeRemoved = components.Get(entityId);
                    if (RemovedComponentEvent != null
                        && componentToBeRemoved != null)
                        RemovedComponentEvent(entity, componentToBeRemoved);

                    entity.RemoveTypeBit(componentType.Bit);
                    this.entityWorld.RefreshEntity(entity);
                    components.Set(entityId, null);
                }
            }
            this.componentsToBeRemoved.Clear();
        }
    }
}