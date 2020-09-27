namespace PrimitiveEngine
{
    using System;
	using System.Collections.Generic;
	using System.Linq;


    /// <summary>
    /// Class BlackBoard.
    /// </summary>
    public class BlackBoard
    {
        private readonly object entryLock;
        private readonly Dictionary<string, object> intelligence;
        private readonly Dictionary<string, List<Trigger>> triggers;


        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="BlackBoard"/> class.
        /// </summary>
        public BlackBoard()
        {
            this.triggers = new Dictionary<string, List<Trigger>>();
            this.intelligence = new Dictionary<string, object>();
            this.entryLock = new object();
        }
        #endregion


        /// <summary>
        /// Adds the trigger.
        /// </summary>
        /// <param name="trigger">The trigger.</param>
        /// <param name="evaluateNow">if set to <see langword="true" /> [evaluate now].</param>
        public void AddTrigger(Trigger trigger, bool evaluateNow = false)
        {
            lock (this.entryLock)
            {
                trigger.BlackBoard = this;
                foreach (string intelName in trigger.WorldPropertiesMonitored)
                {
                    if (this.triggers.ContainsKey(intelName))
                        this.triggers[intelName].Add(trigger);
                    else
                        this.triggers[intelName] = new List<Trigger>{ trigger };
                }

                if (evaluateNow)
                {
                    if (trigger.IsFired == false)
                        trigger.Fire(TriggerStateType.TriggerAdded);
                }
            }
        }


        /// <summary>
        /// Atomics the operate on entry.
        /// </summary>
        /// <param name="operation">The operation.</param>
        public void AtomicOperateOnEntry(Action<BlackBoard> operation)
        {
            lock (this.entryLock)
            {
                operation(this);
            }
        }


        /// <summary>
        /// Gets the entry.
        /// </summary>
		/// <typeparam name="T">The <see langword="Type"/> T.</typeparam>
		/// <returns>The specified element.</returns>
		public T GetEntry<T>()
		{
			string name = typeof(T).Name;
			object ret = GetEntry(name);
			return ret == null ? default(T) : (T)ret;
		}


        /// <summary>
        /// Gets the entry.
        /// </summary>
        /// <typeparam name="T">The <see langword="Type"/> T.</typeparam>
        /// <param name="name">The name.</param>
        /// <returns>The specified element.</returns>
        public T GetEntry<T>(string name)
        {
            object ret = GetEntry(name);
            return ret == null ? default(T) : (T)ret;
        }


        /// <summary>
        /// Gets the entry.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>The specified element.</returns>
        public object GetEntry(string name)
        {
            object entry;
            this.intelligence.TryGetValue(name, out entry);
            return entry;
        }


        /// <summary>
        /// Removes the entry.
        /// </summary>
        /// <param name="name">The name.</param>
        public void RemoveEntry(string name)
        {
            lock (this.entryLock)
            {
                this.intelligence.Remove(name);

                if (!this.triggers.ContainsKey(name))
                    return;

                for (int index = 0; index < this.triggers[name].Count; index++)
                {
                    Trigger item = this.triggers[name][index];
                    if (item.IsFired == false)
                    {
                        item.Fire(TriggerStateType.ValueRemoved);
                    }
                }
            }
        }


        /// <summary>
        /// Removes the trigger.
        /// </summary>
        /// <param name="trigger">The trigger.</param>
        public void RemoveTrigger(Trigger trigger)
        {
            lock (this.entryLock)
            {
                foreach (string intelName in trigger.WorldPropertiesMonitored)
                {
                    this.triggers[intelName].Remove(trigger);
                }
            }
        }


        /// <summary>
        /// Sets the entry.
        /// </summary>
        /// <typeparam name="T">The <see langword="Type"/> T.</typeparam>
        /// <param name="name">The name.</param>
        /// <param name="intel">The intel.</param>
        public void SetEntry<T>(string name, T intel)
        {
            lock (this.entryLock)
            {
                TriggerStateType triggerStateType = this.intelligence.ContainsKey(name) ? TriggerStateType.ValueChanged : TriggerStateType.ValueAdded;
                this.intelligence[name] = intel;

                if (this.triggers.ContainsKey(name))
                {
                    foreach (Trigger item in this.triggers[name].Where(item => item.IsFired == false))
                    {
                        item.Fire(triggerStateType);
                    }
                }
            }
        }


        /// <summary>
        /// Sets the entry using intel's type name as key.
        /// </summary>
		/// <typeparam name="T">The <see langword="Type"/> T.</typeparam>
		/// <param name="intel">The intel.</param>
		public void SetEntry<T>(T intel)
		{
			lock (this.entryLock)
			{
				string name = typeof(T).Name;
				TriggerStateType triggerStateType = this.intelligence.ContainsKey(name) 
                                                        ? TriggerStateType.ValueChanged 
                                                        : TriggerStateType.ValueAdded;
				this.intelligence[name] = intel;

                if (!this.triggers.ContainsKey(name))
                    return;
                
                foreach (Trigger item in this.triggers[name]
                    .Where(item => item.IsFired == false))
                {
                    item.Fire(triggerStateType);
                }
            }
		}


        /// <summary>
        /// Get a list of all related triggers.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>A List{Trigger} of appropriated triggers.</returns>
        public List<Trigger> TriggerList(string name)
        {
            return this.triggers[name];
        }
    }
}