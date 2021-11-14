using System;

namespace LaboratoryAppMVVM.Models
{
    /// <summary>
    /// Class for searching in all properties of the instance of a class.
    /// </summary>
    /// <typeparam name="T">The type of the class.</typeparam>
    public class AllPropertiesSearcher<T> where T : class
    {
        /// <summary>
        /// Search in all instances properties for the given search text.
        /// </summary>
        /// <param name="text">The search text.</param>
        /// <returns>The function which takes type 
        /// and returns bool which determines 
        /// if the object satisfies the search text.</returns>
        public Func<T, bool> Search(string searchText)
        {
            return instance =>
            {
                System.Reflection.PropertyInfo[] properties = instance.GetType()
                .GetProperties();
                bool isSatisfies = false;
                foreach (System.Reflection.PropertyInfo property in properties)
                {
                    if (isSatisfies)
                    {
                        break;
                    }
                    object valueOfPatient = property.GetValue(instance);
                    if (valueOfPatient == null)
                    {
                        continue;
                    }
                    isSatisfies = valueOfPatient.ToString().ToLower().Contains(searchText.ToLower());
                }
                return isSatisfies;
            };
        }
    }
}
