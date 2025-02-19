using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using System;

namespace DigitRecognizer
{
    public partial class MainPage : ContentPage
    {
        public static DrawingCanvas DrawableInstance = new DrawingCanvas();

        public MainPage()
        {
            InitializeComponent();
            CanvasView.Drawable = DrawableInstance;

            // Subscribe to the interaction events for the canvas
            CanvasView.StartInteraction += OnCanvasPointerPressed;
            CanvasView.DragInteraction += OnCanvasPointerMoved;
            CanvasView.EndInteraction += OnCanvasPointerReleased;
        }

        private void OnCanvasPointerPressed(object sender, TouchEventArgs e)
        {
            // Handle touch start
            var touchPoint = e.Touches[0];
            if (touchPoint != null)
            {
                DrawableInstance.AddPoint(new PointF((float)touchPoint.X, (float)touchPoint.Y));
                CanvasView.Invalidate(); // Refresh the drawing
            }
        }

        private void OnCanvasPointerMoved(object sender, TouchEventArgs e)
        {
            // Handle touch move
            var touchPoint = e.Touches[0];
            if (touchPoint != null)
            {
                DrawableInstance.AddPoint(new PointF((float)touchPoint.X, (float)touchPoint.Y));
                CanvasView.Invalidate(); // Update the drawing as user moves finger
            }
        }

        private void OnCanvasPointerReleased(object sender, TouchEventArgs e)
        {
            // Optionally handle pointer release here if needed
        }

        private void OnClearClicked(object sender, EventArgs e)
        {
            DrawableInstance.Clear();
            CanvasView.Invalidate(); // Clear the canvas
        }
    }
}
