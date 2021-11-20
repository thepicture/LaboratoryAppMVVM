using System.Windows;
using System.Windows.Media.Imaging;

namespace LaboratoryAppMVVM.Models.Generators
{
    /// <summary>
    /// Defines a method 
    /// for generating a picture 
    /// as a RenderTargetBitmap.
    /// </summary>
    public interface IRenderTargetBitmapGenerator
    {
        /// <summary>
        /// Generates a picture as a RenderTargetBitmap.
        /// </summary>
        /// <param name="size">The size of a picture.</param>
        /// <returns>The RenderTargetBitmap with a picture.</returns>
        RenderTargetBitmap Generate(Size size);
    }
}
