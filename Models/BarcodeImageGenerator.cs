using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace LaboratoryAppMVVM.Models
{
    public class BarcodeImageGenerator : IRenderTargetBitmapGenerator
    {
        private readonly string _barcode;

        public BarcodeImageGenerator(string barcode)
        {
            _barcode = barcode;
        }

        public RenderTargetBitmap Generate(int width, int height)
        {
            DrawingVisual drawingVisual = new DrawingVisual();
            Random random = new Random();

            using (DrawingContext drawingContext = drawingVisual.RenderOpen())
            {
                for (int i = 0; i < width; i++)
                {
                    drawingContext.DrawRectangle(random.NextDouble() > 0.5
                        ? Brushes.Black
                        : Brushes.White,
                        null,
                        new Rect(i, 0, random.Next(0, 9 + 1) * 2, height - 22.85));
                }
                drawingContext.DrawText(
                    new FormattedText(_barcode,
                                      System.Globalization.CultureInfo.InvariantCulture,
                                      FlowDirection.LeftToRight,
                                      new Typeface("Comic Sans MS"),
                                      10,
                                      Brushes.Black,
                                      96),
                    new Point(0, height - 22.85)
                    );

            }
            RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap(width, height, 96, 96, PixelFormats.Pbgra32); ;
            renderTargetBitmap.Render(drawingVisual);
            return renderTargetBitmap;
        }
    }
}
