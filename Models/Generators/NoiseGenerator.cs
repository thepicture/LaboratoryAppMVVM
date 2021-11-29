using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace LaboratoryAppMVVM.Models.Generators
{
    public class NoiseGenerator : IRenderTargetBitmapGenerator
    {
        private const int rectWidth = 1;
        private const int rectHeight = 1;
        private const int dpiX = 96;
        private const int dpiY = 96;

        public RenderTargetBitmap Generate(Size size)
        {
            DrawingVisual drawingVisual = new DrawingVisual();
            Random random = new Random();

            using (DrawingContext drawingContext = drawingVisual.RenderOpen())
            {
                DrawColoredRectangle(Convert.ToInt32(size.Width),
                                     Convert.ToInt32(size.Height),
                                     random,
                                     drawingContext);
            }

            RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap
                (
                    Convert.ToInt32(size.Width),
                    Convert.ToInt32(size.Height),
                    dpiX: dpiX,
                    dpiY: dpiY,
                    pixelFormat: PixelFormats.Pbgra32
                );
            renderTargetBitmap.Render(drawingVisual);
            return renderTargetBitmap;
        }

        private static void DrawColoredRectangle(int width,
                                                 int height,
                                                 Random random,
                                                 DrawingContext drawingContext)
        {
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    DrawRectangle(random, drawingContext, i, j);
                }
            }
        }

        private static void DrawRectangle(Random random,
                                          DrawingContext drawingContext,
                                          int i,
                                          int j)
        {
            drawingContext.DrawRectangle(new SolidColorBrush(Color.FromRgb(
                GetRandomByte(random),
                GetRandomByte(random),
                GetRandomByte(random))),
                                         null,
                                         new Rect(i, j, rectWidth, rectHeight));
        }

        private static byte GetRandomByte(Random random)
        {
            return Convert.ToByte(byte.MaxValue * random.NextDouble());
        }
    }
}
