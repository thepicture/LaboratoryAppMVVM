using System;

namespace LaboratoryAppMVVM.Services
{
    /// <summary>
    /// Defines methods for classes which handles the tick event of a timer. 
    /// This class cannot be instantiated.
    /// </summary>
    public abstract class HaveTimeServiceBase
    {
        /// <summary>
        /// Raises to update dependency properties.
        /// </summary>
        public event Action TickChanged;
        /// <summary>
        /// Stops the current timer. 
        /// </summary>
        protected TimeSpan _totalTimeLeft;
        public TimeSpan TotalTimeLeft
        {
            get => _totalTimeLeft; set
            {
                _totalTimeLeft = value;
                OnTickChanged();
            }
        }
        protected void OnTickChanged()
        {
            TickChanged?.Invoke();
        }
        public abstract void Stop();
        /// <summary>
        /// Starts the current timer.
        /// </summary>
        public abstract void Start();
    }
}
