namespace PrimitiveEngine
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using System.Reflection;
	using System.Threading.Tasks;


	/// <summary>
	/// Class SystemManager.
	/// </summary>
	public sealed class SystemManager
	{
		private readonly EntityWorld entityWorld;
		private readonly IDictionary<Type, IList> systems;
		private readonly SystemBitManager systemBitManager;
		private readonly Bag<EntitySystem> mergedBag;
		private IDictionary<int, SystemLayer> fixedUpdateLayers;
		private IDictionary<int, SystemLayer> frameUpdateLayers;


		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="SystemManager" /> class.
		/// </summary>
		/// <param name="entityWorld">The entity world.</param>
		internal SystemManager(EntityWorld entityWorld)
		{
			this.mergedBag = new Bag<EntitySystem>();
			this.frameUpdateLayers = new Dictionary<int, SystemLayer>();
			this.fixedUpdateLayers = new Dictionary<int, SystemLayer>();
			this.systemBitManager = new SystemBitManager();
			this.systems = new Dictionary<Type, IList>();
			this.entityWorld = entityWorld;
		}
		#endregion


		#region Properties
		/// <summary>
		/// Gets the systems.
		/// </summary>
		/// <value>The systems.</value>
		public Bag<EntitySystem> Systems
		{
			get { return this.mergedBag; }
		}
		#endregion


		/// <summary>
		/// Gets the system.
		/// </summary>
		/// <typeparam name="T">The EntitySystem</typeparam>
		/// <returns>The system instance</returns>
		/// <exception cref="InvalidOperationException">There are more or none systems of the type passed</exception>
		public T GetSystem<T>() where T : EntitySystem
		{
			this.systems.TryGetValue(typeof(T), out IList foundSystems);

			if (foundSystems == null)
				return null;
			
			if (foundSystems.Count > 1)
			{
				throw new InvalidOperationException(
					$"System list contains more than one element of type {typeof(T)}");
			}
			
			return (T)foundSystems[0];
		}


		/// <summary>
		/// Gets the systems.
		/// </summary>
		/// <typeparam name="T">The EntitySystem</typeparam>
		/// <returns>A List of System Instances</returns>
		public List<T> GetSystems<T>() where T : EntitySystem
		{
			this.systems.TryGetValue(typeof(T), out IList system);

			return (List<T>)system;
		}


		/// <summary>
		/// Sets the system.
		/// </summary>
		/// <typeparam name="T">The <see langword="Type" /> T.</typeparam>
		/// <param name="system">The system.</param>
		/// <param name="updateType">Type of the game loop.</param>
		/// <param name="layer">The layer.</param>
		/// <param name="executionType">Type of the execution.</param>
		/// <returns>The set system.</returns>
		public T SetSystem<T>(
			T system,
			UpdateType updateType,
			int layer = 0,
			ExecutionType executionType = ExecutionType.Synchronous) where T : EntitySystem
		{
			return (T)SetSystem(system.GetType(), system, updateType, layer, executionType);
		}


		/// <summary>
		/// Creates the instance.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <returns>The specified ComponentPool-able instance.</returns>
		private static ComponentPoolable CreateInstance(Type type)
		{
			return (ComponentPoolable)Activator.CreateInstance(type);
		}


		/// <summary>
		/// Processes the specified systems to process.
		/// </summary>
		/// <param name="systemsToProcess">The systems to process.</param>
		private static void Process(IDictionary<int, SystemLayer> systemsToProcess)
		{
			foreach (int item in systemsToProcess.Keys)
			{
				if (systemsToProcess[item].SynchronousSystems.Count > 0)
					ProcessBagSynchronous(systemsToProcess[item].SynchronousSystems);
				if (systemsToProcess[item].AsynchronousSystems.Count > 0)
					ProcessBagAsynchronous(systemsToProcess[item].AsynchronousSystems);
			}
		}


		/// <summary>
		/// Updates the bag asynchronous.
		/// </summary>
		/// <param name="entitySystems">The entity systems.</param>
		private static void ProcessBagAsynchronous(IEnumerable<EntitySystem> entitySystems)
		{
			Parallel.ForEach(
				source: entitySystems,
				body: (EntitySystem entitySystem) =>
				{
					entitySystem.Process();
				});
		}


		/// <summary>
		/// Updates the bag synchronous.
		/// </summary>
		/// <param name="entitySystems">The entitySystems.</param>
		private static void ProcessBagSynchronous(Bag<EntitySystem> entitySystems)
		{
			for (int index = 0, j = entitySystems.Count; index < j; ++index)
			{
				entitySystems.Get(index).Process();
			}
		}


		/// <summary>
		/// Sets the system.
		/// </summary>
		/// <param name="layers">The layers.</param>
		/// <param name="system">The system.</param>
		/// <param name="layer">The layer.</param>
		/// <param name="executionType">Type of the execution.</param>
		private static void SetSystem(
			ref IDictionary<int, SystemLayer> layers,
			EntitySystem system,
			int layer,
			ExecutionType executionType)
		{
			if (!layers.ContainsKey(layer))
				layers[layer] = new SystemLayer();

			Bag<EntitySystem> updateBag = layers[layer][executionType];

			if (!updateBag.Contains(system))
				updateBag.Add(system);

			layers = (from d in layers orderby d.Key ascending select d).ToDictionary(keySelector: pair => pair.Key, elementSelector: pair => pair.Value);
		}


		/// <summary>
		/// Creates the pool.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <param name="attributes">The attributes.</param>
		/// <exception cref="NullReferenceException">propertyComponentPool is null.</exception>
		private void CreatePool(Type type, IEnumerable<Attribute> attributes)
		{
			ComponentPoolAttribute propertyComponentPool = null;

			foreach (ComponentPoolAttribute artemisComponentPool in attributes.OfType<ComponentPoolAttribute>())
			{
				propertyComponentPool = artemisComponentPool;
			}

			MethodInfo[] methods = type.GetMethods();
			IEnumerable<MethodInfo> methodInfos = from methodInfo in methods
												let methodAttributes = methodInfo.GetCustomAttributes(false)
												from attribute in methodAttributes.OfType<ComponentCreate>()
												select methodInfo;

			Func<Type, ComponentPoolable> create = null;

			foreach (MethodInfo methodInfo in methodInfos)
			{
				create = (Func<Type, ComponentPoolable>)Delegate.CreateDelegate(typeof(Func<Type, ComponentPoolable>), methodInfo);
			}

			if (create == null)
				create = CreateInstance;

			IComponentPool<ComponentPoolable> pool;

			if (propertyComponentPool == null)
				throw new NullReferenceException("propertyComponentPool is null.");

			if (!propertyComponentPool.IsSupportMultiThread)
			{
				pool = new ComponentPool<ComponentPoolable>(
					propertyComponentPool.InitialSize,
					propertyComponentPool.ResizeSize,
					propertyComponentPool.IsResizable,
					create,
					type);
			}
			else
			{
				pool = new ComponentPoolMultiThread<ComponentPoolable>(
					propertyComponentPool.InitialSize,
					propertyComponentPool.ResizeSize,
					propertyComponentPool.IsResizable,
					create,
					type);
			}

			this.entityWorld.SetPool(type, pool);
		}


		/// <summary>
		/// Sets the system.
		/// </summary>
		/// <param name="systemType">Type of the system.</param>
		/// <param name="system">The system.</param>
		/// <param name="updateType">Type of the game loop.</param>
		/// <param name="layer">The layer.</param>
		/// <param name="executionType">Type of the execution.</param>
		/// <returns>The EntitySystem.</returns>
		private EntitySystem SetSystem(
			Type systemType,
			EntitySystem system,
			UpdateType updateType,
			int layer = 0,
			ExecutionType executionType = ExecutionType.Synchronous)
		{
			system.EntityWorld = this.entityWorld;

			if (this.systems.ContainsKey(systemType))
				this.systems[systemType].Add(system);
			else
			{
				Type genericType = typeof(List<>);
				Type listType = genericType.MakeGenericType(systemType);
				this.systems[systemType] = (IList)Activator.CreateInstance(listType);
				this.systems[systemType].Add(system);
			}

			switch (updateType)
			{
				case UpdateType.FrameUpdate:
					SetSystem(ref this.frameUpdateLayers, system, layer, executionType);
					break;

				case UpdateType.FixedUpdate:
					SetSystem(ref this.fixedUpdateLayers, system, layer, executionType);
					break;
			}

			if (!this.mergedBag.Contains(system))
				this.mergedBag.Add(system);

			system.Bit = this.systemBitManager.GetBitFor(system);

			return system;
		}


		/// <summary>
		/// The system layer class.
		/// </summary>
		private sealed class SystemLayer
		{
			/// <summary>
			/// The synchronous systems.
			/// </summary>
			public readonly Bag<EntitySystem> SynchronousSystems;

			/// <summary>
			/// The asynchronous systems.
			/// </summary>
			public readonly Bag<EntitySystem> AsynchronousSystems;


			#region Constructors
			/// <summary>Initializes a new instance of the <see cref="SystemLayer"/> class.</summary>
			public SystemLayer()
			{
				this.AsynchronousSystems = new Bag<EntitySystem>();
				this.SynchronousSystems = new Bag<EntitySystem>();
			}
			#endregion


			#region Properties
			/// <summary>
			/// Gets the <see cref="Bag{EntitySystem}"/> with the specified execution type.
			/// </summary>
			/// <param name="executionType">Type of the execution.</param>
			/// <returns>The Bag{EntitySystem}.</returns>
			public Bag<EntitySystem> this[ExecutionType executionType]
			{
				get
				{
					switch (executionType)
					{
						case ExecutionType.Synchronous:
							return this.SynchronousSystems;

						case ExecutionType.Asynchronous:
							return this.AsynchronousSystems;

						default:
							throw new ArgumentOutOfRangeException(nameof(executionType));
					}
				}
			}
			#endregion
		}


		/// <summary>
		/// Initialize a system.
		/// </summary>
		/// <param name="type">Type of the system, inheriting from EntitySystem.</param>
		/// <param name="updateType">Update type of the system (default: FrameUpdate).</param>
		/// <param name="order">Order the system processes (default: 0).</param>
		/// <param name="executionType">System execution type (default: Synchronous).</param>
		public void InitializeSystem(
			Type type, 
			UpdateType updateType = UpdateType.FrameUpdate,
			int order = 0,
			ExecutionType executionType = ExecutionType.Synchronous)
		{
			if (typeof(EntitySystem).IsAssignableFrom(type))
			{
				EntitySystem instance = (EntitySystem)Activator.CreateInstance(type);
				SetSystem(
					instance, 
					updateType, 
					order, 
					executionType);
				instance.LoadContent();
			}
		}
		

		/// <summary>
		/// Initializes all systems.
		/// </summary>
		/// <param name="processAttributes">if set to <see langword="true" /> [process attributes].</param>
		/// <param name="assembliesToScan">The assemblies to scan.</param>
		/// <exception cref="Exception">propertyComponentPool is null.</exception>
		internal void InitializeAll(
			bool processAttributes, 
			IEnumerable<Assembly> assembliesToScan = null)
		{
			if (processAttributes)
			{
				IDictionary<Type, List<Attribute>> types;
				if (assembliesToScan == null)
					types = AttributesProcessor.Process(
						supportedAttributes: AttributesProcessor.SupportedAttributes, 
						assembliesToScan: null);
				else
					types = AttributesProcessor.Process(
						supportedAttributes: AttributesProcessor.SupportedAttributes, 
						assembliesToScan: assembliesToScan);

				foreach (KeyValuePair<Type, List<Attribute>> item in types)
				{
					Type type = item.Key;
					List<Attribute> attributes = item.Value;
					if (typeof(EntitySystem).IsAssignableFrom(type))
					{
						EntitySystemAttribute entitySystemAttribute = (EntitySystemAttribute)attributes[0];
						EntitySystem instance = (EntitySystem)Activator.CreateInstance(type);
						SetSystem(
							instance, 
							entitySystemAttribute.UpdateType, 
							entitySystemAttribute.Layer, 
							entitySystemAttribute.ExecutionType);
					}

					else if (typeof(IEntityTemplate).IsAssignableFrom(type))
					{
						EntityTemplateAttribute entitySystemAttribute = (EntityTemplateAttribute)attributes[0];
						IEntityTemplate instance = (IEntityTemplate)Activator.CreateInstance(type);
						this.entityWorld.SetEntityTemplate(entitySystemAttribute.Name, instance);
					}

					else if (typeof(ComponentPoolable).IsAssignableFrom(type))
						CreatePool(type, attributes);
				}
			}

			for (int index = 0, j = this.mergedBag.Count; index < j; ++index)
			{
				this.mergedBag.Get(index).LoadContent();
			}
		}


		/// <summary>
		/// Terminates all systems.
		/// </summary>
		internal void TerminateAll()
		{
			for (int index = 0; index < this.Systems.Count; ++index)
			{
				EntitySystem entitySystem = this.Systems.Get(index);
				entitySystem.UnloadContent();
			}

			this.Systems.Clear();
		}


		/// <summary>
		/// Updates the specified execution type.
		/// </summary>
		internal void FixedUpdate()
		{
			Process(this.fixedUpdateLayers);
		}


		/// <summary>
		/// Updates the specified execution type.
		/// </summary>
		internal void FrameUpdate()
		{
			Process(this.frameUpdateLayers);
		}
	}
}