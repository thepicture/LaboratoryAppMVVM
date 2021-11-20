namespace LaboratoryAppMVVM.Services
{
    /// <summary>
    /// Defines methods to show feedback messages 
    /// to an user.
    /// </summary>
    public interface IMessageService
    {
        /// <summary>
        /// Shows a question 
        /// with the ability 
        /// to give an answer.
        /// </summary>
        /// <param name="message">The question message.</param>
        /// <returns></returns>
        bool ShowQuestion(string message);
        /// <summary>
        /// Shows an error.
        /// </summary>
        /// <param name="message">The error message.</param>
        void ShowError(string message);
        /// <summary>
        /// Shows an information.
        /// </summary>
        /// <param name="message">The information message.</param>
        void ShowInformation(string message);
    }
}
