using System;

namespace LaboratoryAppMVVM.Models.Exports
{
    /// <summary>
    /// Defines a method to get a drawing context 
    /// and release its resources when it is necessary. 
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    public interface IDrawingContext : IDisposable
    {
        /// <summary>
        /// Gets a drawing context.
        /// </summary>
        /// <returns>A drawing context.</returns>
        object GetContext();
    }
}