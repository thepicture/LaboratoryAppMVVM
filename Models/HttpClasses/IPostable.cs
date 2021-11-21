namespace LaboratoryAppMVVM.Models.HttpClasses
{
    /// <summary>
    /// Defines a method for posting a request.
    /// </summary>
    public interface IPostable
    {
        /// <summary>
        /// Posts a request 
        /// and gets the response 
        /// as the byte array if it exists, 
        /// otherwise returns null.
        /// </summary>
        /// <returns>The response byte array 
        /// if it exists, 
        /// otherwise null.</returns>
        byte[] Post();
    }
}
