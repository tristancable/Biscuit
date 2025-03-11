using SkiaSharp;

namespace DigitRecognizer.Services


{
    public class Image
    {
        public readonly int size;
        public readonly int numPixels;
        public readonly bool greyscale;

        public readonly double[] pixelValues;
        public readonly int label;

        public Image(int size, bool greyscale, double[] pixelValues, int label)
        {
            this.size = size;
            this.numPixels = size * size;
            this.greyscale = greyscale;
            this.pixelValues = pixelValues;
            this.label = label;
        }

        public SKBitmap ToBitmap()
        {
            SKBitmap bitmap = new SKBitmap(size, size);
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    int index = y * size + x;
                    byte pixel = (byte)(pixelValues[index] * 255); // Scale back to 0-255
                    SKColor color = new SKColor(pixel, pixel, pixel); // Grayscale
                    bitmap.SetPixel(x, y, color);
                }
            }
            return bitmap;
        }
    }
}