namespace PrimitiveEngine
{
    using System.Collections.Generic;
	using System.Diagnostics;


    /// <summary>
    /// Class GroupManager.
    /// </summary>
    public sealed class GroupManager
    {
        private readonly Bag<Entity> emptyBag;
        private readonly Dictionary<string, Bag<Entity>> entitiesByGroup;
        private readonly Bag<string> groupByEntity;


        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="GroupManager"/> class.
        /// </summary>
        internal GroupManager()
        {
            this.groupByEntity = new Bag<string>();
            this.entitiesByGroup = new Dictionary<string, Bag<Entity>>();
            this.emptyBag = new Bag<Entity>();
        }
        #endregion


        /// <summary>
        /// Gets the entities.
        /// </summary>
        /// <param name="group">The group.</param>
        /// <returns>All entities related to the specified group in a Bag{Entity}.</returns>
        public Bag<Entity> GetEntities(string group)
        {
            Debug.Assert(!string.IsNullOrEmpty(group), "Group must not be null or empty.");

            Bag<Entity> bag;
            if (!this.entitiesByGroup.TryGetValue(group, out bag))
            {
                return this.emptyBag;
            }

            return bag;
        }


        /// <summary>
        /// Gets the group of.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>The group name.</returns>
        public string GetGroupOf(Entity entity)
        {
            Debug.Assert(entity != null, "Entity must not be null.");

            int entityId = entity.Index;
            if (entityId < this.groupByEntity.Capacity)
            {
                return this.groupByEntity.Get(entityId);
            }

            return null;
        }


        /// <summary>
        /// Determines whether the specified entity is grouped.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns><see langword="true" /> if the specified entity is grouped; otherwise, <see langword="false" />.</returns>
        public bool IsGrouped(Entity entity)
        {
            Debug.Assert(entity != null, "Entity must not be null.");

            return GetGroupOf(entity) != null;
        }


        /// <summary>
        /// Removes an entity from the group it is assigned to, if any.
        /// </summary>
        /// <param name="entity">The entity to be removed</param>
        internal void Remove(Entity entity)
        {
            Debug.Assert(entity != null, "Entity must not be null.");

            int entityId = entity.Index;
            if (entityId < this.groupByEntity.Capacity)
            {
                string group = this.groupByEntity.Get(entityId);
                if (group != null)
                {
                    this.groupByEntity.Set(entityId, null);

                    Bag<Entity> entities;
                    if (this.entitiesByGroup.TryGetValue(group, out entities))
                    {
                        entities.Remove(entity);
                    }
                }
            }
        }


        /// <summary>
        /// Sets the specified group.
        /// </summary>
        /// <param name="group">The group.</param>
        /// <param name="entity">The entity.</param>
        internal void Set(string group, Entity entity)
        {
            Debug.Assert(!string.IsNullOrEmpty(group), "Group must not be null or empty.");
            Debug.Assert(entity != null, "Entity must not be null.");

            // Entity can only belong to one group.
            Remove(entity);

            Bag<Entity> entities;
            if (!this.entitiesByGroup.TryGetValue(group, out entities))
            {
                entities = new Bag<Entity>();
                this.entitiesByGroup.Add(group, entities);
            }

            entities.Add(entity);

            this.groupByEntity.Set(entity.Index, group);
        }
    }
}