﻿using SkiaSharp;

public class DrawingCanvas : IDrawable
{
    private List<Line> _lines = new List<Line>();
    private List<Circle> _circles = new List<Circle>();
    private Line _currentLine;
    private bool isEraser = false;
    private int currentBrushSize = 30;
    private bool isCircleMode = false;

    // Undo/Redo stacks
    private Stack<UndoAction> undoStack = new Stack<UndoAction>();
    private Stack<UndoAction> redoStack = new Stack<UndoAction>();

    public class Line
    {
        public List<PointF> Points { get; set; } = new List<PointF>();
        public int BrushSize { get; set; }
        public bool IsEraser { get; set; }

        public Line(int brushSize, bool isEraser)
        {
            BrushSize = brushSize;
            IsEraser = isEraser;
        }
    }

    public class Circle
    {
        public PointF Center { get; set; }
        public int Radius { get; set; }
        public int BrushSize { get; set; }
        public bool IsEraser { get; set; }

        public Circle(PointF center, int radius, int brushSize, bool isEraser)
        {
            Center = center;
            Radius = radius;
            BrushSize = brushSize;
            IsEraser = isEraser;
        }
    }

    public void SetDrawingMode(bool drawing)
    {
        isEraser = !drawing;
        isCircleMode = false;
    }

    public void SetCircleMode()
    {
        isCircleMode = true;
    }

    public void SetBrushSize(int size)
    {
        currentBrushSize = size;
    }

    public void StartNewLine(PointF startPoint)
    {
        if (!isCircleMode)
        {
            _currentLine = new Line(currentBrushSize, isEraser);
            _currentLine.Points.Add(startPoint);
        }
    }

    public void FinishLine()
    {
        if (_currentLine != null && _currentLine.Points.Count > 0)
        {
            _lines.Add(_currentLine);
            undoStack.Push(new UndoAction { ActionType = ActionType.Line, Line = _currentLine });
            _currentLine = null;
        }
    }

    public void AddPoint(PointF point)
    {
        if (!isCircleMode)
        {
            _currentLine?.Points.Add(point);

            if (isEraser)
            {
                EraseCircle(point);
            }
        }
        else
        {
            if (isEraser)
            {
                EraseCircle(point);
            }
            else
            {
                DrawDot(point);
            }
        }
    }

    private void EraseCircle(PointF point)
    {
        var radius = currentBrushSize / 2;

        _circles.RemoveAll(c =>
            Math.Sqrt(Math.Pow(c.Center.X - point.X, 2) + Math.Pow(c.Center.Y - point.Y, 2)) < radius);
    }

    public void DrawCircle(PointF center)
    {
        var circle = new Circle(center, currentBrushSize, currentBrushSize, isEraser);
        _circles.Add(circle);

        undoStack.Push(new UndoAction { ActionType = ActionType.Circle, Circle = circle });
    }

    public void DrawDot(PointF point)
    {
        var circle = new Circle(point, currentBrushSize, currentBrushSize, isEraser);
        _circles.Add(circle);

        undoStack.Push(new UndoAction { ActionType = ActionType.Circle, Circle = circle });
    }

    public void Clear()
    {
        _lines.Clear();
        _circles.Clear();
        undoStack.Clear();
        redoStack.Clear();
    }

    public void Undo()
    {
        if (undoStack.Count > 0)
        {
            var lastAction = undoStack.Pop();

            if (lastAction.ActionType == ActionType.Line)
            {
                _lines.Remove(lastAction.Line);
            }
            else if (lastAction.ActionType == ActionType.Circle)
            {
                _circles.Remove(lastAction.Circle);
            }

            redoStack.Push(lastAction);
        }
    }

    public void Redo()
    {
        if (redoStack.Count > 0)
        {
            var lastAction = redoStack.Pop();

            if (lastAction.ActionType == ActionType.Line)
            {
                _lines.Add(lastAction.Line);
            }
            else if (lastAction.ActionType == ActionType.Circle)
            {
                _circles.Add(lastAction.Circle);
            }

            undoStack.Push(lastAction);
        }
    }

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        foreach (var line in _lines)
        {
            if (line.Points.Count < 2) continue;

            canvas.StrokeSize = line.BrushSize;
            canvas.StrokeColor = line.IsEraser ? Colors.LightGray : Colors.Black;
            canvas.StrokeLineCap = LineCap.Round;

            for (int i = 1; i < line.Points.Count; i++)
            {
                canvas.DrawLine(line.Points[i - 1].X, line.Points[i - 1].Y, line.Points[i].X, line.Points[i].Y);
            }
        }

        foreach (var circle in _circles)
        {
            canvas.FillColor = circle.IsEraser ? Colors.LightGray : Colors.Black;
            canvas.FillEllipse(circle.Center.X - (circle.Radius / 2), circle.Center.Y - (circle.Radius / 2),
                   circle.Radius, circle.Radius);
        }

        if (_currentLine != null && _currentLine.Points.Count > 1)
        {
            canvas.StrokeSize = _currentLine.BrushSize;
            canvas.StrokeColor = _currentLine.IsEraser ? Colors.LightGray : Colors.Black;
            canvas.StrokeLineCap = LineCap.Round;

            for (int i = 1; i < _currentLine.Points.Count; i++)
            {
                canvas.DrawLine(_currentLine.Points[i - 1].X, _currentLine.Points[i - 1].Y,
                               _currentLine.Points[i].X, _currentLine.Points[i].Y);
            }
        }
    }

    public class UndoAction
    {
        public ActionType ActionType { get; set; }
        public Line Line { get; set; }
        public Circle Circle { get; set; }
    }

    public enum ActionType
    {
        Line,
        Circle
    }
    public double[] GetPixelData(int width, int height)
    {
        // Create an SKBitmap with the specified width and height
        using (var skBitmap = new SKBitmap(width, height))
        {
            // Create a SkiaSharp canvas to draw on the bitmap
            using (var skCanvas = new SKCanvas(skBitmap))
            {
                // Clear the canvas with white background
                skCanvas.Clear(SKColors.White);

                // Set up SkiaSharp paint for lines
                var paint = new SKPaint
                {
                    IsAntialias = true,
                    StrokeCap = SKStrokeCap.Round,
                    StrokeJoin = SKStrokeJoin.Round
                };

                // Render lines
                foreach (var line in _lines)
                {
                    if (line.Points.Count < 2) continue;

                    paint.StrokeWidth = line.BrushSize;
                    paint.Color = line.IsEraser ? SKColors.LightGray : SKColors.Black;

                    var points = line.Points.Select(p => new SKPoint(p.X, p.Y)).ToArray();
                    skCanvas.DrawPoints(SKPointMode.Polygon, points, paint);
                }

                // Render circles (dots)
                foreach (var circle in _circles)
                {
                    paint.Color = circle.IsEraser ? SKColors.LightGray : SKColors.Black;
                    skCanvas.DrawCircle(circle.Center.X, circle.Center.Y, circle.Radius / 2, paint);
                }
            }
            var resizedBitmap = skBitmap.Resize(new SKImageInfo(28, 28), SKFilterQuality.Medium);

            // Now, we extract pixel data as grayscale values
            var pixelData = new double[28 * 28];
            for (int y = 0; y < 28; y++)
            {
                for (int x = 0; x < 28; x++)
                {
                    // Get the color at the current pixel
                    SKColor color = resizedBitmap.GetPixel(x, y);

                    // Convert the color to grayscale using the luminosity formula
                    double grayscale = 0.2989 * color.Red / 255.0 + 0.5870 * color.Green / 255.0 + 0.1140 * color.Blue / 255.0;

                    // Store the grayscale value in the array
                    pixelData[y * 28 + x] = 1 - grayscale;
                }
            }

            return pixelData;
        }
    }
}