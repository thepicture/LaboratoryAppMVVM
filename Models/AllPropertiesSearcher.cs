using System;
using System.Reflection;

namespace LaboratoryAppMVVM.Models
{
    /// <summary>
    /// Implements methods for 
    /// searching search text 
    /// in any properties of the instance.
    /// </summary>
    public class AllPropertiesSearcher : IPropertiesSearcher
    {
        private readonly string _searchText;

        /// <summary>
        /// Creates a instance of all properties searcher 
        /// with the given search text.
        /// </summary>
        /// <param name="searchText"></param>
        public AllPropertiesSearcher(string searchText)
        {
            _searchText = searchText;
        }

        public Func<T, bool> Search<T>() where T : class
        {
            return instance =>
            {
                PropertyInfo[] properties = instance.GetType().GetProperties();
                bool isSatisfies = false;
                isSatisfies = IsAnyPropertyInObjectContainsValue(_searchText,
                                                          instance,
                                                          properties,
                                                          isSatisfies);
                return isSatisfies;
            };
        }

        private static bool IsAnyPropertyInObjectContainsValue<T>(string searchText,
                                                        T instance,
                                                        PropertyInfo[] properties,
                                                        bool isPropertySatisfiesSearchText)
        {
            foreach (PropertyInfo property in properties)
            {
                if (isPropertySatisfiesSearchText)
                {
                    break;
                }
                object valueOfProperty = property.GetValue(instance);
                if (valueOfProperty == null)
                {
                    continue;
                }
                isPropertySatisfiesSearchText = IsValueContainsSearchText(searchText, valueOfProperty);
            }

            return isPropertySatisfiesSearchText;
        }

        private static bool IsValueContainsSearchText(string searchText,
                                                      object valueOfObject)
        {
            return valueOfObject.ToString().ToLower().Contains(searchText.ToLower());
        }
    }
}
