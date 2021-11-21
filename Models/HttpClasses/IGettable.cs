namespace LaboratoryAppMVVM.Models.HttpClasses
{
    /// <summary>
    /// Defines a method to send a GET request.
    /// </summary>
    public interface IGettable
    {
        /// <summary>
        /// Sends a GET request.
        /// </summary>
        /// <returns>The response as a byte array.</returns>
        byte[] Get();
    }
}
