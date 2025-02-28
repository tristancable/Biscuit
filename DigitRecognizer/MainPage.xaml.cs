using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Controls.Xaml;
using System;
using System.Linq;

namespace DigitRecognizer
{
    public partial class MainPage : ContentPage
    {
        public static DrawingCanvas DrawableInstance = new DrawingCanvas();

        public MainPage()
        {
            InitializeComponent();
            CanvasView.Drawable = (Microsoft.Maui.Graphics.IDrawable)DrawableInstance;
        }

        private PointF _lastPoint; // Track the last touch point
        private bool _isDragging = false; // Track if the user is dragging

        // Start Interaction - Touch Pressed
        private void OnCanvasPointerPressed(object sender, TouchEventArgs e)
        {
            var touchPoint = e.Touches.FirstOrDefault();
            if (touchPoint != null)
            {
                var point = new PointF((float)touchPoint.X, (float)touchPoint.Y);
                _lastPoint = point; // Save the first point when the user clicks
                _isDragging = false; // Initially not dragging

                // Start a new line or draw a dot
                DrawableInstance.StartNewLine(point); // Start the line at the clicked position

                // Draw a dot if we are not dragging (just clicking)
                DrawableInstance.DrawDot(point); // Draw a dot at the clicked position
                //DrawableInstance.AddPoint(point);
                CanvasView.Invalidate(); // Redraw immediately
            }
        }

        // Drag Interaction - Touch Moved (Drawing while dragging)
        private void OnCanvasPointerMoved(object sender, TouchEventArgs e)
        {
            var touchPoint = e.Touches.FirstOrDefault();
            if (touchPoint != null)
            {
                var point = new PointF((float)touchPoint.X, (float)touchPoint.Y);

                // Check if the user has moved a sufficient distance to start drawing a line
                if (!_isDragging && (Math.Abs(point.X - _lastPoint.X) > 5 || Math.Abs(point.Y - _lastPoint.Y) > 5))
                {
                    _isDragging = true; // User has started dragging
                }

                // If dragging, add points to the line
                if (_isDragging)
                {
                    DrawableInstance.AddPoint(point); // Add the point to the current line
                }

                CanvasView.Invalidate(); // Redraw continuously while dragging
            }
        }

        // End Interaction - Touch Released (Finish drawing when released)
        private void OnCanvasPointerReleased(object sender, TouchEventArgs e)
        {
            if (_isDragging)
            {
                DrawableInstance.FinishLine(); // Finish the current line if we were dragging
            }

            CanvasView.Invalidate(); // Redraw after touch is released
        }

        // Clear the canvas
        public void OnClearClicked(object sender, EventArgs e)
        {
            DrawableInstance.Clear();
            CanvasView.Invalidate();
        }

        // Handle brush size changes
        public void OnBrushSizeChanged(object sender, EventArgs e)
        {
            var slider = (Slider)sender;
            DrawableInstance.SetBrushSize((int)slider.Value);
        }

        // Activate the pencil tool
        private void OnPencilClicked(object sender, EventArgs e)
        {
            DrawableInstance.SetDrawingMode(true);
        }

        // Activate the eraser tool
        private void OnEraserClicked(object sender, EventArgs e)
        {
            DrawableInstance.SetDrawingMode(false);
        }
    }
}