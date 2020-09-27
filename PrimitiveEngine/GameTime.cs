namespace PrimitiveEngine
{
	using System;

	
	/// <summary>
	/// Represents a timeline through which elapsed time may be measured.
	/// </summary>
	public class GameTime
	{
		private long totalGameTime;
		private long previousUpdateTime;
		private bool paused;


		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="GameTime"/> class.
		/// </summary>
		public GameTime()
		{
			this.totalGameTime = 0;
			this.previousUpdateTime = DateTime.Now.Ticks;
		}
		#endregion


		#region Properties
		/// <summary>
		/// Returns number of ticks since the last time Update() was called.
		/// </summary>
		public long ElapsedTime
		{
			get
			{
				if (!this.paused)
					return DateTime.Now.Ticks - this.previousUpdateTime;
				return 0;
			}
		}


		public bool Paused
		{
			get { return this.paused; }
		}


		/// <summary>
		/// Time when Update() was last called.
		/// </summary>
		public long PreviousUpdateTime
		{
			get { return this.previousUpdateTime; }
		}


		public long TotalGameTime
		{
			get { return this.totalGameTime; }
		}
		#endregion


		/// <summary>
		/// Pause time.
		/// Until Unpause is called, ElapsedTime will return 0, and Time will not accumulate.
		/// </summary>
		public void Pause()
		{
			this.paused = true;
		}


		/// <summary>
		/// Unpase time.
		/// </summary>
		public void Unpause()
		{
			this.paused = false;
		}


		/// <summary>
		/// Update Time and PreviousUpdateTime.
		/// </summary>
		public void Update()
		{
			if (!this.paused)
			{
				this.totalGameTime += this.ElapsedTime;
				this.previousUpdateTime = DateTime.Now.Ticks;
			}
		}
	}
}