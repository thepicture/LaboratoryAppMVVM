namespace LaboratoryAppMVVM.Models
{
    /// <summary>
    /// Defines a method to validate that the values 
    /// are in consonance with each other.
    /// </summary>
    public interface IValidator
    {
        /// <summary>
        /// Validates that the values are in consonance 
        /// with each other and returns true if they are 
        /// in consonance, and false otherwise.
        /// </summary>
        /// <param name="values">The given values to validate.</param>
        /// <returns>True if the given values are 
        /// in consonance, and false otherwise.</returns>
        bool IsValidated(params object[] values);
    }
}
