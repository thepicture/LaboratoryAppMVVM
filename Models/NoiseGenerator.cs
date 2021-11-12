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

            using (DrawingContext drawingContext = drawingVisual.RenderOpen())
            {
                for (int i = 0; i < height; i++)
                {
                    for (int j = 0; j < width; j++)
                    {
                        drawingContext.DrawRectangle(DateTime.Now.Ticks % 2 == 0 ? Brushes.Black : Brushes.White,
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
