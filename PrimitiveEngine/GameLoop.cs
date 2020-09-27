namespace PrimitiveEngine
{
	public abstract class GameLoop
	{
		private bool running;
		private GameTime gameTime;
		

		#region Properties		
		/// <summary>
		/// Timeline of the game loop.
		/// </summary>
		public GameTime GameTime
		{
			get { return this.gameTime; }
		}
		#endregion


		/// <summary>
		/// Game logic to run on each game loop iteration.
		/// </summary>
		/// <param name="deltaTime">Ticks since the last Update().</param>
		public abstract void Update(long deltaTime);

		
		/// <summary>
		/// Starts the game loop.
		/// </summary>
		public void Start()
		{
			this.running = true;
			this.gameTime = new GameTime();
			while (this.running)
			{
				Update(this.gameTime.ElapsedTime);
				this.gameTime.Update();
			}
		}
		

		/// <summary>
		/// Stops the game loop.
		/// </summary>
		public void Stop()
		{
			this.running = false;
		}
	}
}