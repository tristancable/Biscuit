﻿using System.Diagnostics;
using DigitRecognizer.Services;
using SkiaSharp;
namespace DigitRecognizer
{
    public partial class MainPage : ContentPage
    {
        public static DrawingCanvas DrawableInstance = new DrawingCanvas();
        public static NeuralNetwork network;
        private string currentDirectory = $"{AppContext.BaseDirectory}\\Resources\\MNIST Model\\Mnist Net.json";

        public MainPage()
        {
            InitializeComponent();
            CanvasView.Drawable = (Microsoft.Maui.Graphics.IDrawable)DrawableInstance;
            network = NetworkSaveData.LoadNetworkFromFile(currentDirectory);
        }

        private PointF _lastPoint;
        private bool _isDragging = false;

        private void OnUndoClicked(object sender, EventArgs e)
        {
            DrawableInstance.Undo();
            CanvasView.Invalidate();
            double[] data = DrawableInstance.GetPixelData((int)CanvasView.Width, (int)CanvasView.Height);
            (int prediction, double[] outputs) = network.Classify(data);
            NeuralNetwork.Text = prediction + "\n";
            outputs.ToList().ForEach(o => NeuralNetwork.Text += o + "\n");
        }

        private void OnRedoClicked(object sender, EventArgs e)
        {
            DrawableInstance.Redo();
            CanvasView.Invalidate();
            double[] data = DrawableInstance.GetPixelData((int)CanvasView.Width, (int)CanvasView.Height);
            (int prediction, double[] outputs) = network.Classify(data);
            NeuralNetwork.Text = prediction + "\n";
            outputs.ToList().ForEach(o => NeuralNetwork.Text += o + "\n");
        }


        // Start Interaction - Touch Pressed
        private void OnCanvasPointerPressed(object sender, TouchEventArgs e)
        {
            var touchPoint = e.Touches.FirstOrDefault();
            if (touchPoint != null)
            {
                var point = new PointF((float)touchPoint.X, (float)touchPoint.Y);
                _lastPoint = point;
                _isDragging = false;

                DrawableInstance.StartNewLine(point);
                DrawableInstance.DrawDot(point);
                CanvasView.Invalidate();
            }
            double[] data = DrawableInstance.GetPixelData((int)CanvasView.Width, (int)CanvasView.Height);
            (int prediction, double[] outputs) = network.Classify(data);
            NeuralNetwork.Text = prediction + "\n";
            outputs.ToList().ForEach(o => NeuralNetwork.Text += o + "\n");
        }

        private void OnCanvasPointerMoved(object sender, TouchEventArgs e)
        {
            var touchPoint = e.Touches.FirstOrDefault();
            if (touchPoint != null)
            {
                var point = new PointF((float)touchPoint.X, (float)touchPoint.Y);

                if (!_isDragging && (Math.Abs(point.X - _lastPoint.X) > 5 || Math.Abs(point.Y - _lastPoint.Y) > 5))
                {
                    _isDragging = true;
                }

                if (_isDragging)
                {
                    DrawableInstance.AddPoint(point);
                    CanvasView.Invalidate();
                }
            }
        }

        private void OnCanvasPointerReleased(object sender, TouchEventArgs e)
        {
            if (_isDragging)
            {
                DrawableInstance.FinishLine();
            }

            CanvasView.Invalidate();
            double[] data = DrawableInstance.GetPixelData((int)CanvasView.Width, (int)CanvasView.Height);
            (int prediction, double[] outputs) = network.Classify(data);
            NeuralNetwork.Text = prediction + "\n";
            outputs.ToList().ForEach(o => NeuralNetwork.Text += o + "\n");
        }

        public void OnClearClicked(object sender, EventArgs e)
        {
            DrawableInstance.Clear();
            CanvasView.Invalidate();
        }

        public void OnBrushSizeChanged(object sender, EventArgs e)
        {
            var slider = (Slider)sender;
            DrawableInstance.SetBrushSize((int)slider.Value);
        }

        private void OnPencilClicked(object sender, EventArgs e)
        {
            DrawableInstance.SetDrawingMode(true);
        }

        private void OnEraserClicked(object sender, EventArgs e)
        {
            DrawableInstance.SetDrawingMode(false);
        }
    }
}