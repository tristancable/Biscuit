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


                int numImages = imageData.Length / (imageSize * imageSize); // Assuming grayscale, 28x28 images
                int numLabels = labelData.Length; // Each label is 1 byte
                if (numLabels > numImages)
                {
                    int excess = numLabels - numImages;

                    // Shift the labels forward by trimming the first `excess` labels
                    byte[] trimmedLabels = new byte[numImages];
                    Array.Copy(labelData, excess, trimmedLabels, 0, numImages);

                    labelData = trimmedLabels; // Replace label data with shifted labels
                    numLabels = numImages;

                    Console.WriteLine($"⚠️ Trimmed first {excess} labels. New label count: {numLabels}");
                }

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


            if (numLabels > numImages)
            {
                int excess = numLabels - numImages;

                // Shift the labels forward by trimming the first `excess` labels
                byte[] trimmedLabels = new byte[numImages];
                Array.Copy(labelData, excess, trimmedLabels, 0, numImages);

                labelData = trimmedLabels;
                numLabels = numImages;

                Console.WriteLine($"Trimmed first {excess} labels. New label count: {numLabels}");
            }


            if (numImages != numLabels)
            {
                throw new InvalidOperationException($"Number of images ({numImages}) doesn't match number of labels ({numLabels})");
            }

            List<Image> images = new List<Image>();

            double[] allPixelValues = new double[imageData.Length];
            for (int i = 0; i < imageData.Length; i++)
            {
                allPixelValues[i] = imageData[i] / 255.0;
            }

            for (int i = 0; i < numImages; i++)
            {
                int byteOffset = i * bytesPerImage;
                double[] pixelValues = new double[bytesPerImage];
                Array.Copy(allPixelValues, byteOffset, pixelValues, 0, bytesPerImage);

                images.Add(new Image(imageSize, greyscale, pixelValues, labelData[i]));
            }

            return images.ToArray();
        }

        [Serializable]
        public struct DataFile
        {
            public string imageFilePath;
            public string labelFilePath;
        }
    }
}
