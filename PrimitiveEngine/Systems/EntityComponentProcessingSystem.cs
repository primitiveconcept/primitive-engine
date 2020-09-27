namespace PrimitiveEngine
{
    using System.Collections.Generic;
	
    
    /// <summary>
    /// System which processes entities calling Process(Entity entity, T component)
    /// every update.
    /// Automatically extracts the components specified by the type parameters.
    /// </summary>
    /// <typeparam name="T">The component.</typeparam>
    public abstract class EntityComponentProcessingSystem<T> : EntitySystem
        where T : IEntityComponent
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="EntityComponentProcessingSystem{T}"/> class
        /// with an aspect which processes entities which have all the specified component types.
        /// </summary>
        protected EntityComponentProcessingSystem()
            : this(Aspect.Empty())
        {
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="EntityComponentProcessingSystem{T}"/> class
        /// with an aspect which processes entities which have all the specified component types
        /// as well as the any additional constraints specified by the aspect.
        /// </summary>
        /// <param name="aspect">The aspect specifying the additional constraints.</param>
        protected EntityComponentProcessingSystem(Aspect aspect)
            : base(aspect.GetAll(typeof(T)))
        {
        }
        #endregion


        /// <summary>
        /// Called for every entity in this system with the components
        /// automatically passed as arguments.
        /// </summary>
        /// <param name="entity">The entity that is processed </param>
        /// <param name="component">The component.</param>
        public abstract void Process(Entity entity, T component);


        /// <summary>
        /// Processes the entities.
        /// </summary>
        /// <param name="entities">The entities.</param>
        protected override void ProcessEntities(IDictionary<int, Entity> entities)
        {
            foreach (Entity entity in entities.Values)
            {
                Process(entity, entity.GetComponent<T>());
            }
        }
    }

    /// <summary>
    /// System which processes entities calling Process(Entity entity, T1 component1, T2 component2)
    /// every update. 
    /// Automatically extracts the components specified by the type parameters.
    /// </summary>
    /// <typeparam name="T1">The first component.</typeparam>
    /// <typeparam name="T2">The second component.</typeparam>
    public abstract class EntityComponentProcessingSystem<T1, T2> : EntitySystem
        where T1 : IEntityComponent
        where T2 : IEntityComponent
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="EntityComponentProcessingSystem{T1, T2}"/> class 
        /// with an aspect which processes entities which have all the specified component types.
        /// </summary>
        protected EntityComponentProcessingSystem()
            : this(Aspect.Empty())
        {
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="EntityComponentProcessingSystem{T1, T2}"/> class 
        /// with an aspect which processes entities which have all the specified component types as well as 
        /// the any additional constraints specified by the aspect.
        /// </summary>
        /// <param name="aspect"> The aspect specifying the additional constraints. </param>
        protected EntityComponentProcessingSystem(Aspect aspect)
            : base(aspect.GetAll(typeof(T1), typeof(T2)))
        {
        }
        #endregion


        /// <summary>
        /// Called every for every entity in this system with the components automatically passed as arguments.
        /// </summary>
        /// <param name="entity">The entity that is processed </param>
        /// <param name="component1>The first component.</param>
        /// <param name="component2">The second component.</param>
        public abstract void Process(Entity entity, T1 component1, T2 component2);


        /// <summary>
        /// Processes the entities.
        /// </summary>
        /// <param name="entities">The entities.</param>
        protected override void ProcessEntities(IDictionary<int, Entity> entities)
        {
            foreach (Entity entity in entities.Values)
            {
                Process(entity, entity.GetComponent<T1>(), entity.GetComponent<T2>());
            }
        }
    }

    /// <summary>
    /// System which processes entities calling Process(Entity entity, T1 component1, T2 component2, T3 component3) every update.
    /// Automatically extracts the components specified by the type parameters.
    /// </summary>
    /// <typeparam name="T1">The first component.</typeparam>
    /// <typeparam name="T2">The second component.</typeparam>
    /// <typeparam name="T3">The third component.</typeparam>
    public abstract class EntityComponentProcessingSystem<T1, T2, T3> : EntitySystem
        where T1 : IEntityComponent
        where T2 : IEntityComponent
        where T3 : IEntityComponent
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="EntityComponentProcessingSystem{T1, T2, T3}"/> class 
        /// with an aspect which processes entities which have all the specified component types.
        /// </summary>
        protected EntityComponentProcessingSystem()
            : this(Aspect.Empty())
        {
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="EntityComponentProcessingSystem{T1, T2, T3}"/> class 
        /// with an aspect which processes entities which have all the specified component types
        /// as well as the any additional constraints specified by the aspect.
        /// </summary>
        /// <param name="aspect">The aspect specifying the additional constraints.</param>
        protected EntityComponentProcessingSystem(Aspect aspect)
            : base(aspect.GetAll(typeof(T1), typeof(T2), typeof(T3)))
        {
        }
        #endregion


        /// <summary>
        /// Called for every entity in this system with the components automatically passed as arguments.
        /// </summary>
        /// <param name="entity">The entity that is processed</param>
        /// <param name="component1">The first component.</param>
        /// <param name="component2">The second component.</param>
        /// <param name="component3">The third component.</param>
        public abstract void Process(Entity entity, T1 component1, T2 component2, T3 component3);


        /// <summary>
        /// Processes the entities.
        /// </summary>
        /// <param name="entities">The entities.</param>
        protected override void ProcessEntities(IDictionary<int, Entity> entities)
        {
            foreach (Entity entity in entities.Values)
            {
                Process(entity, entity.GetComponent<T1>(), entity.GetComponent<T2>(), entity.GetComponent<T3>());
            }
        }
    }

    /// <summary>
    /// System which processes entities calling Process(Entity entity, T1 t1, T2 t2, T3 t3, T4 t4) every update.
    /// Automatically extracts the components specified by the type parameters.
    /// </summary>
    /// <typeparam name="T1">The first component.</typeparam>
    /// <typeparam name="T2">The second component.</typeparam>
    /// <typeparam name="T3">The third component.</typeparam>
    /// <typeparam name="T4">The fourth component.</typeparam>
    public abstract class EntityComponentProcessingSystem<T1, T2, T3, T4> : EntitySystem
        where T1 : IEntityComponent
        where T2 : IEntityComponent
        where T3 : IEntityComponent
        where T4 : IEntityComponent
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="EntityComponentProcessingSystem{T1, T2, T3, T4}"/> class 
        /// with an aspect which processes entities which have all the specified component types.
        /// </summary>
        protected EntityComponentProcessingSystem()
            : this(Aspect.Empty())
        {
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="EntityComponentProcessingSystem{T1, T2, T3, T4}"/> class 
        /// with an aspect which processes entities which have all the specified component types
        /// as well as the any additional constraints specified by the aspect.
        /// </summary>
        /// <param name="aspect">The aspect specifying the additional constraints.</param>
        protected EntityComponentProcessingSystem(Aspect aspect)
            : base(aspect.GetAll(typeof(T1), typeof(T2), typeof(T3), typeof(T4)))
        {
        }
        #endregion


        /// <summary>
        /// Called every for every entity in this system with the components automatically passed as arguments.
        /// </summary>
        /// <param name="entity">The entity that is processed</param>
        /// <param name="component1">The first component.</param>
        /// <param name="component2">The second component.</param>
        /// <param name="component3">The third component.</param>
        /// <param name="component4">The fourth component.</param>
        public abstract void Process(Entity entity, T1 component1, T2 component2, T3 component3, T4 component4);


        /// <summary>
        /// Processes the entities.
        /// </summary>
        /// <param name="entities">The entities.</param>
        protected override void ProcessEntities(IDictionary<int, Entity> entities)
        {
            foreach (Entity entity in entities.Values)
            {
                Process(entity, entity.GetComponent<T1>(), entity.GetComponent<T2>(), entity.GetComponent<T3>(), entity.GetComponent<T4>());
            }
        }
    }

    /// <summary>
    /// System which processes entities calling Process(Entity entity, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5) every update.
    /// Automatically extracts the components specified by the type parameters.
    /// </summary>
    /// <typeparam name="T1">The first component.</typeparam>
    /// <typeparam name="T2">The second component.</typeparam>
    /// <typeparam name="T3">The third component.</typeparam>
    /// <typeparam name="T4">The fourth component.</typeparam>
    /// <typeparam name="T5">The fifth component.</typeparam>
    public abstract class EntityComponentProcessingSystem<T1, T2, T3, T4, T5> : EntitySystem
        where T1 : IEntityComponent
        where T2 : IEntityComponent
        where T3 : IEntityComponent
        where T4 : IEntityComponent
        where T5 : IEntityComponent
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="EntityComponentProcessingSystem{T1, T2, T3, T4, T5}"/> class 
        /// with an aspect which processes entities which have all the specified component types.
        /// </summary>
        protected EntityComponentProcessingSystem()
            : this(Aspect.Empty())
        {
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="EntityComponentProcessingSystem{T1, T2, T3, T4, T5}"/> class 
        /// with an aspect which processes entities which have all the specified component types
        /// as well as the any additional constraints specified by the aspect.
        /// </summary>
        /// <param name="aspect">The aspect specifying the additional constraints.</param>
        protected EntityComponentProcessingSystem(Aspect aspect)
            : base(aspect.GetAll(typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5)))
        {
        }
        #endregion


        /// <summary>
        /// Called every for every entity in this system with the components automatically passed as arguments.
        /// </summary>
        /// <param name="entity">The entity that is processed</param>
        /// <param name="component1">The first component.</param>
        /// <param name="component2">The second component.</param>
        /// <param name="component3">The third component.</param>
        /// <param name="component4">The fourth component.</param>
        /// <param name="component5">The fifth component.</param>
        public abstract void Process(Entity entity, T1 component1, T2 component2, T3 component3, T4 component4, T5 component5);


        /// <summary>
        /// Processes the entities.
        /// </summary>
        /// <param name="entities">The entities.</param>
        protected override void ProcessEntities(IDictionary<int, Entity> entities)
        {
            foreach (Entity entity in entities.Values)
            {
                Process(entity, entity.GetComponent<T1>(), entity.GetComponent<T2>(), entity.GetComponent<T3>(), entity.GetComponent<T4>(), entity.GetComponent<T5>());
            }
        }
    }
}
