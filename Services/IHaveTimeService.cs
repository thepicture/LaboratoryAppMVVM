using System;

namespace LaboratoryAppMVVM.Services
{
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
