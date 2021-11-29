namespace LaboratoryAppMVVM.Models.Entities
{
    /// <summary>
    /// Describes the progress state of the research status of an analyzer.
    /// </summary>
    public class ProgressState
    {
        /// <summary>
        /// The value is 0. The research is preparing.
        /// </summary>
        public static readonly int Preparing = 0;
        /// <summary>
        /// The value is 100. The research if fulfilled.
        /// </summary>
        public static readonly int? Fulfilled = null;
    }
}
