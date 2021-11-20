using System;

namespace LaboratoryAppMVVM.Services
{
    /// <summary>
    /// Defines methods for classes which handles the tick event of a timer.
    /// </summary>
    public interface IHaveTimeService
    {
        /// <summary>
        /// Raises to update dependency properties.
        /// </summary>
        event Action TickChanged;
        /// <summary>
        /// Stops the current timer. 
        /// </summary>
        void Stop();
        /// <summary>
        /// Starts the current timer.
        /// </summary>
        void Start();
    }
}
