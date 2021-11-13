using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace LaboratoryAppMVVM.Models
{
    public class NoiseGenerator
    {
        public RenderTargetBitmap Generate(int width, int height)
        {
            DrawingVisual drawingVisual = new DrawingVisual();
            Random random = new Random();

            using (DrawingContext drawingContext = drawingVisual.RenderOpen())
            {
                for (int i = 0; i < width; i++)
                {
                    for (int j = 0; j < height; j++)
                    {
                        drawingContext.DrawRectangle(new SolidColorBrush(Color.FromRgb(Convert.ToByte(255 * random.NextDouble()),
                            Convert.ToByte(255 * random.NextDouble()),
                            Convert.ToByte(255 * random.NextDouble()))),
                                                     null,
                                                     new Rect(i, j, 1, 1));
                    }
                }
            }

            RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap(width, height, 96, 96, PixelFormats.Pbgra32);
            renderTargetBitmap.Render(drawingVisual);
            return renderTargetBitmap;
        }
    }
}
