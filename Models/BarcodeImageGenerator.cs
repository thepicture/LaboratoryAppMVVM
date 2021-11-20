using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace LaboratoryAppMVVM.Models
{
    public class BarcodeImageGenerator : IRenderTargetBitmapGenerator
    {
        private readonly string _barcode;
        private DrawingVisual _drawingVisual;
        private RenderTargetBitmap _resultImage;
        private double _currentCaret;
        private Size _size;
        private const double _dpiX = 96;
        private const double _dpiY = 96;
        private const double rectWidthCoefficient = 0.15 + 1;
        private const double emptySpaceLength = 1.35;
        private const double barcodeRectHeightMargin = 22.85;
        private const int pixelsPerDip = 96;
        private const int emSize = 10;

        public BarcodeImageGenerator(string barcode)
        {
            _barcode = barcode;
        }

        public RenderTargetBitmap Generate(Size size)
        {
            _size = size;
            if (AreSizesBad())
            {
                ThrowBadSizeException();
            }
            else
            {
                DoActionsForCreatingRenderTargetBitmap();
            }
            return _resultImage;
        }

        private void DoActionsForCreatingRenderTargetBitmap()
        {
            OpenDrawingVisual();
            GenerateBarcode();
            RenderBarcodeToResultImage();
        }

        private void RenderBarcodeToResultImage()
        {
            _resultImage = new RenderTargetBitmap
              (
                  Convert.ToInt32(_size.Width),
                  Convert.ToInt32(_size.Height),
                  _dpiX,
                  _dpiY,
                  PixelFormats.Pbgra32
              );
            _resultImage.Render(_drawingVisual);
        }

        private void GenerateBarcode()
        {
            using (DrawingContext drawingContext = _drawingVisual.RenderOpen())
            {
                DrawBarcodeImage(drawingContext);
            }
        }

        private void DrawBarcodeImage(DrawingContext drawingContext)
        {
            foreach (char charOfBarcode in _barcode)
            {
                double widthOfRect = Convert.ToInt32(charOfBarcode.ToString())
                                     * rectWidthCoefficient;
                FormattedText formattedText = GetFormattedText(charOfBarcode);
                if (charOfBarcode != '0')
                {
                    DrawBarcodeRectangle(drawingContext, widthOfRect);
                }
                DrawBarcodeText(drawingContext, formattedText);
                ShiftCaret(widthOfRect, formattedText);
            }
        }

        private void ShiftCaret(double widthOfRect, FormattedText formattedText)
        {
            _currentCaret += widthOfRect + emptySpaceLength + formattedText.Width;
        }

        private void DrawBarcodeRectangle(DrawingContext drawingContext,
                                          double widthOfRect)
        {
            drawingContext.DrawRectangle
                (
                    Brushes.Black,
                    null,
                    new Rect(x: _currentCaret,
                             y: 0,
                             widthOfRect,
                             _size.Height - barcodeRectHeightMargin)
                );
        }

        private void DrawBarcodeText(DrawingContext drawingContext,
                                     FormattedText formattedText)
        {
            drawingContext.DrawText(formattedText,
                                     new Point(_currentCaret,
                                     _size.Height - 22.85));
        }

        private static FormattedText GetFormattedText(char charOfBarcode)
        {
            return new FormattedText
                (
                    textToFormat: charOfBarcode.ToString(),
                    culture: System.Globalization.CultureInfo.InvariantCulture,
                    flowDirection: FlowDirection.LeftToRight,
                    typeface: new Typeface("Comic Sans MS"),
                    emSize: emSize,
                    foreground: Brushes.Black,
                    pixelsPerDip: pixelsPerDip
                );
        }

        private void OpenDrawingVisual()
        {
            _drawingVisual = new DrawingVisual();
        }

        private bool AreSizesBad()
        {
            return _size.Width <= 0 || _size.Height <= 0;
        }

        private void ThrowBadSizeException()
        {
            throw new ArgumentOutOfRangeException(nameof(_size),
                                "Expected size "
                                + "as a positive "
                                + "double value, "
                                + "actual are "
                                + "width="
                                + _size.Width
                                + ", height="
                                + _size.Height);
        }
    }
}
