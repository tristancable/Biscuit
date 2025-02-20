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
        static int img_h = 28;
        static int img_w = 28;
        static int img_size_flat = img_w * img_h;
        static int n_classes = 10;
        static int n_channels = 1;
        public static void PrepareData()
        {
        }
    }
}