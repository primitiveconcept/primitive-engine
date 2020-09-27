namespace PrimitiveEngine
{
	using System;
	using System.Collections.Generic;
	using System.Threading.Tasks;


	/// <summary>
	/// Class ParallelEntityProcessingSystem.
	/// </summary>
	public abstract class ParallelEntityProcessingSystem : EntitySystem
	{
		private readonly TaskFactory factory;


		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="ParallelEntityProcessingSystem" /> class.
		/// </summary>
		/// <param name="aspect">The aspect.</param>
		protected ParallelEntityProcessingSystem(Aspect aspect)
			: base(aspect)
		{
			this.factory = new TaskFactory(TaskScheduler.Default);
		}
		#endregion


		/// <summary>
		/// Processes the specified entity.
		/// </summary>
		/// <param name="entity">The entity.</param>
		public abstract void Process(Entity entity);


		/// <summary>
		/// Processes the entities.
		/// </summary>
		/// <param name="entities">The entities.</param>
		protected override void ProcessEntities(IDictionary<int, Entity> entities)
		{
			float simultaneous = 2;

			int perThread = (int)Math.Ceiling(entities.Values.Count / simultaneous);
			Entity[] threadEntities = new Entity[entities.Values.Count];
			entities.Values.CopyTo(threadEntities, 0);
			int numberOfEntities = entities.Values.Count - 1;
			List<Task> tasks = new List<Task>();

			for (int processorIndex = 0; processorIndex < simultaneous; ++processorIndex)
			{
				int initial = numberOfEntities;
				tasks.Add(
					this.factory.StartNew(
						() =>
							{
								for (int spartialIndex = initial; spartialIndex > initial - perThread && spartialIndex >= 0; --spartialIndex)
								{
									Process(threadEntities[spartialIndex]);
								}
							}));
				numberOfEntities -= perThread;
			}

			Task.WaitAll(tasks.ToArray());
		}
	}
}