using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using System;

namespace DigitRecognizer
{
    public partial class MainPage : ContentPage
    {
        public static DrawingCanvas DrawableInstance = new DrawingCanvas();
        private bool isDrawing = false; // Track if we are in the middle of a drawing session

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
            // Handle touch start: start a new line without clearing the canvas
            var touchPoint = e.Touches[0];
            if (touchPoint != null)
            {
                // If we're not already drawing, start a new line
                if (!isDrawing)
                {
                    isDrawing = true; // Mark that we are now drawing
                    DrawableInstance.StartNewLine(); // Start a new line (modify this method as needed)
                }

                // Add the first point of the new line
                DrawableInstance.AddPoint(new PointF((float)touchPoint.X, (float)touchPoint.Y));
                CanvasView.Invalidate(); // Refresh the drawing
            }
        }

        private void OnCanvasPointerMoved(object sender, TouchEventArgs e)
        {
            // Handle touch move: continue the current line
            var touchPoint = e.Touches[0];
            if (touchPoint != null && isDrawing)
            {
                DrawableInstance.AddPoint(new PointF((float)touchPoint.X, (float)touchPoint.Y));
                CanvasView.Invalidate(); // Update the drawing as user moves finger
            }
        }

        private void OnCanvasPointerReleased(object sender, TouchEventArgs e)
        {
            // Handle pointer release: finish the current line
            var touchPoint = e.Touches[0];
            if (touchPoint != null)
            {
                // Add the final point to the current line
                DrawableInstance.AddPoint(new PointF((float)touchPoint.X, (float)touchPoint.Y));
                isDrawing = false; // Mark that the drawing session is finished
                CanvasView.Invalidate(); // Finalize the drawing
            }
        }

        private void OnClearClicked(object sender, EventArgs e)
        {
            DrawableInstance.Clear();
            CanvasView.Invalidate(); // Clear the canvas
        }
    }
}
