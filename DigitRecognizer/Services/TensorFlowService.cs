using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tensorflow;
using Tensorflow.NumPy;
using static Tensorflow.Binding;
namespace DigitRecognizer.Services
{
    public static class TensorFlowService
    {
        private static Tensor x, y;
        private static Tensor logits, loss, optimizer;
        private static Session sess;
        public static void Initialize()
        {
            BuildGraph();
            sess = tf.Session();
            sess.run(tf.global_variables_initializer());
        }
        private static void BuildGraph()
        {
            // Placeholders
            x = tf.placeholder(TF_DataType.TF_FLOAT, new Shape(-1, 28, 28));
            y = tf.placeholder(TF_DataType.TF_INT32, new Shape(-1));
            // Flatten
            var inputLayer = tf.reshape(x, new Shape(-1, 784));
            // First Dense Layer
            var W1 = tf.Variable(tf.random.truncated_normal(new Shape(784, 128), 0.1f));
            var b1 = tf.Variable(tf.zeros(new Shape(128)));
            var hiddenLayer = tf.nn.relu(tf.matmul(inputLayer, W1) + b1);
            // Output
            var W2 = tf.Variable(tf.random.truncated_normal(new Shape(128, 10), 0.1f));
            var b2 = tf.Variable(tf.zeros(new Shape(10)));
            logits = tf.matmul(hiddenLayer, W2) + b2;
            // Loss
            loss = tf.reduce_mean(tf.nn.sparse_softmax_cross_entropy_with_logits(y, logits));
            // Optimizer
            var learningRate = 0.01f;
            optimizer = tf.train.AdamOptimizer(learningRate).minimize(loss);
        }
        public static void Train(NDArray xTrain, NDArray yTrain, int epochs, int batchSize)
        {
            long numBatches = xTrain.shape[0] / batchSize;
            for (int epoch = 0; epoch < epochs; epoch++)
            {
                for (int i = 0; i < numBatches; i++)
                {
                    var (batchX, batchY) = GetBatch(xTrain, yTrain, batchSize, i);
                    sess.run(optimizer, new FeedItem(x, batchX), new FeedItem(y, batchY));
                }
                // Loss after epoch
                var trainLoss = sess.run(loss, new FeedItem(x, xTrain), new FeedItem(y, yTrain));
                Console.WriteLine($"Epoch {epoch + 1}/{epochs}, Loss: {trainLoss}");
            }
        }
        public static NDArray Predict(NDArray input)
        {
            var predictions = sess.run(tf.nn.softmax(logits), new FeedItem(x, input));
            return np.argmax(predictions, 1);
        }
        private static (NDArray, NDArray) GetBatch(NDArray images, NDArray labels, int batchSize, int batchIndex)
        {
            long start = batchIndex * batchSize;
            long end = Math.Min(start + batchSize, images.shape[0]);
            return (images[$"{start}:{end}"], labels[$"{start}:{end}"]);
        }
    }
}