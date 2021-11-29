using System;

namespace LaboratoryAppMVVM.Models
{
    public interface IPropertiesSearcher
    {
        /// <summary>
        /// Searches in properties 
        /// of the class <typeparamref name="T"/>
        /// for the given search text.
        /// </summary>
        /// <param name="text">The search text.</param>
        /// <returns>The function which takes type 
        /// and returns bool which determines 
        /// if a property of the object 
        /// satisfies the search text.</returns>
        Func<T, bool> Search<T>() where T : class;
    }
}
