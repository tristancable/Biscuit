using System.Diagnostics;

namespace DigitRecognizer.Services
{
    public class ImageLoader
    {
        int imageSize = 28;
        bool greyscale = true;
        DataFile[] dataFiles;
        string[] labelNames = Array.Empty<string>();
        Image[] images = Array.Empty<Image>();

        public int NumImages => images?.Length ?? 0;
        public int InputSize => imageSize * imageSize * (greyscale ? 1 : 3);
        public int OutputSize => labelNames.Length;
        public string[] LabelNames => labelNames;

        DataFile[] files = new[]  {
            new DataFile {imageFilePath = "MNIST_TestData/t10k-images-idx3-ubyte", labelFilePath = "MNIST_TestData/t10k-labels-idx1-ubyte" }
        };

        public ImageLoader()
        {
            dataFiles = files ?? throw new ArgumentNullException(nameof(files));
            images = LoadImages();
        }

        public Image GetImage(int i)
        {
            if (images.Length == 0) throw new InvalidOperationException("No images have been loaded.");
            if (i < 0 || i >= images.Length) throw new IndexOutOfRangeException("Image index out of range.");
            return images[i];
        }

        public DataPoint[] GetAllData()
        {
            return images.Select(DataFromImage).ToArray();
        }

        private DataPoint DataFromImage(Image image)
        {
            return new DataPoint(image.pixelValues, image.label, OutputSize);
        }

        private Image[] LoadImages()
        {
            List<Image> allImages = new List<Image>();

            foreach (var file in dataFiles)
            {
                if (!File.Exists(file.imageFilePath) || !File.Exists(file.labelFilePath))
                    throw new FileNotFoundException($"File not found: {file.imageFilePath} or {file.labelFilePath}");

                byte[] imageData = File.ReadAllBytes(file.imageFilePath);
                byte[] labelData = File.ReadAllBytes(file.labelFilePath);
                allImages.AddRange(LoadImages(imageData, labelData));
            }

            return allImages.ToArray();
        }

        private Image[] LoadImages(byte[] imageData, byte[] labelData)
        {
            int numChannels = greyscale ? 1 : 3;
            int bytesPerImage = imageSize * imageSize * numChannels;
            int numImages = imageData.Length / bytesPerImage;
            int numLabels = labelData.Length;

            if (numImages != numLabels)
                throw new InvalidOperationException($"Number of images ({numImages}) doesn't match number of labels ({numLabels}).");

            int dataSetSize = Math.Min(numImages, numLabels);
            var images = new Image[dataSetSize];

            double[] allPixelValues = new double[imageData.Length];
            Parallel.For(0, imageData.Length, i => allPixelValues[i] = imageData[i] / 255.0);

            Parallel.For(0, dataSetSize, imageIndex =>
            {
                int byteOffset = imageIndex * bytesPerImage;
                double[] pixelValues = new double[bytesPerImage];
                Array.Copy(allPixelValues, byteOffset, pixelValues, 0, bytesPerImage);

                images[imageIndex] = new Image(imageSize, greyscale, pixelValues, labelData[imageIndex]);
            });

            return images;
        }

        [Serializable]
        public struct DataFile
        {
            public string imageFilePath;
            public string labelFilePath;
        }
    }
}
