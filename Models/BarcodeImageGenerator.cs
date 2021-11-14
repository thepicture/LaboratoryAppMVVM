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
            double currentCaret = 0;

            using (DrawingContext drawingContext = drawingVisual.RenderOpen())
            {
                foreach (char charOfBarcode in _barcode)
                {
                    double widthOfRect = Convert.ToInt32(charOfBarcode.ToString())
                        * (0.15 + 1);
                    FormattedText formattedText = new FormattedText
                        (
                            textToFormat: charOfBarcode.ToString(),
                            culture: System.Globalization.CultureInfo.InvariantCulture,
                            flowDirection: FlowDirection.LeftToRight,
                            typeface: new Typeface("Comic Sans MS"),
                            emSize: 10,
                            foreground: Brushes.Black,
                            pixelsPerDip: 96
                        );

                    if (charOfBarcode == '0')
                    {
                        drawingContext
                            .DrawText(
                                        formattedText,
                                        origin: new Point(currentCaret, height - 22.85)
                                     );
                    }
                    else
                    {
                        drawingContext.DrawRectangle
                            (
                                Brushes.Black,
                                null,
                                new Rect(
                                    currentCaret,
                                    0,
                                    widthOfRect,
                                    height - 22.85)
                            );
                        drawingContext
                            .DrawText
                            (
                                formattedText,
                                new Point(currentCaret, height - 22.85)
                            );
                    }
                    currentCaret += widthOfRect + 1.35 + formattedText.Width;
                }
            }
            RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap
                (
                    width,
                    height,
                    96,
                    96,
                    PixelFormats.Pbgra32
                );
            renderTargetBitmap.Render(drawingVisual);
            return renderTargetBitmap;
        }
    }
}
