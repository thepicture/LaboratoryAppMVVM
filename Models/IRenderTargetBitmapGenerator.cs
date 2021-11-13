using System.Windows.Media.Imaging;

namespace LaboratoryAppMVVM.Models
{
    public interface IRenderTargetBitmapGenerator
    {
        RenderTargetBitmap Generate(int width, int height);
    }
}
