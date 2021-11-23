namespace LaboratoryAppMVVM.Models
{
    /// <summary>
    /// Defines a method to calculate something 
    /// and return a value.
    /// </summary>
    public interface ICalculator
    {
        /// <summary>
        /// Calculates something and returns a value.
        /// </summary>
        /// <param name="values">The values 
        /// presented in calculations.</param>
        /// <returns>A calculated value.</returns>
        object Calculate(params object[] values);
    }
}
