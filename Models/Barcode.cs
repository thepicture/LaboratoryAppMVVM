namespace LaboratoryAppMVVM.Models
{
    /// <summary>
    /// Represents a barcode for exporting.
    /// </summary>
    class Barcode
    {
        public string ImagePath { get; set; }

        public Barcode(string imagePath)
        {
            ImagePath = imagePath;
        }
    }
}
