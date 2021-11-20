namespace LaboratoryAppMVVM.Models.Entities
{
    /// <summary>
    /// Represents a barcode for exporting.
    /// </summary>
    public class Barcode
    {
        public string ImagePath { get; set; }

        public Barcode(string imagePath)
        {
            ImagePath = imagePath;
        }
    }
}
