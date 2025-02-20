using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NumSharp;
using Tensorflow;
namespace DigitRecognizer.Services
{
    public static class TensorFlowService
    {
        public static void Start()
        {
            var (xTrain, yTrain, xTest, yTest) = LoadMNIST();
        }
        private static (NDArray, NDArray, NDArray, NDArray) LoadMNIST()
        {
            string dataDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MNIST_TestData");
            string trainImagesPath = Path.Combine(dataDir, "train-images-idx3-ubyte");
            string trainLabelsPath = Path.Combine(dataDir, "train-labels-idx1-ubyte");
            string testImagesPath = Path.Combine(dataDir, "t10k-images-idx3-ubyte");
            string testLabelsPath = Path.Combine(dataDir, "t10k-labels-idx1-ubyte");
            if (!File.Exists(trainImagesPath) || !File.Exists(trainLabelsPath) || !File.Exists(testImagesPath) || !File.Exists(testLabelsPath))
            {
                throw new FileNotFoundException("MNIST dataset files are missing in MNIST_TestData.");
            }
            var xTrain = ReadImages(trainImagesPath);
            var yTrain = ReadLabels(trainLabelsPath);
            var xTest = ReadImages(testImagesPath);
            var yTest = ReadLabels(testLabelsPath);
            return (xTrain, yTrain, xTest, yTest);
        }
        private static NDArray ReadImages(string path)
        {
            using (var stream = new FileStream(path, FileMode.Open))
            using (var reader = new BinaryReader(stream))
            {
                int numImages = ReverseBytes(reader.ReadInt32());
                int rows = ReverseBytes(reader.ReadInt32());
                int columns = ReverseBytes(reader.ReadInt32());
                byte[] buffer = reader.ReadBytes(numImages * rows * columns);
                return np.array(buffer).reshape(numImages, rows, columns).astype(NPTypeCode.Float) / 255.0f;
            }
        }
        private static NDArray ReadLabels(string path)
        {
            using (var stream = new FileStream(path, FileMode.Open))
            using (var reader = new BinaryReader(stream))
            {
                int numLabels = ReverseBytes(reader.ReadInt32());
                byte[] buffer = reader.ReadBytes(numLabels);
                return np.array(buffer).astype(NPTypeCode.Int32);
            }
        }
        private static int ReverseBytes(int value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            Array.Reverse(bytes);
            return BitConverter.ToInt32(bytes, 0);
        }
    }
}