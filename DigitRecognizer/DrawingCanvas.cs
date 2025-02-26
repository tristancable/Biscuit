public class DrawingCanvas : IDrawable
{
    private List<Line> _lines = new List<Line>();
    private List<Circle> _circles = new List<Circle>(); // Store circles
    private Line _currentLine;
    private bool isEraser = false;
    private int currentBrushSize = 5;
    public bool isCircleMode = false; // New flag for circle mode

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

    // Set the drawing mode (pencil, eraser, or circle)
    public void SetDrawingMode(bool drawing)
    {
        isEraser = !drawing;
        isCircleMode = false; // Reset circle mode if switching to pencil/eraser
    }

    // Set circle mode
    public void SetCircleMode()
    {
        isCircleMode = true;
    }

    // Set brush size
    public void SetBrushSize(int size)
    {
        currentBrushSize = size;
    }

    // Start a new line
    public void StartNewLine(PointF startPoint)
    {
        if (!isCircleMode) // Only start new line if not in circle mode
        {
            _currentLine = new Line(currentBrushSize, isEraser);
            _currentLine.Points.Add(startPoint);
        }
    }

    // Add a point to the current line
    public void AddPoint(PointF point)
    {
        if (!isCircleMode)
        {
            _currentLine?.Points.Add(point);
        }
    }

    // Finish the line and add it to the list of lines
    public void FinishLine()
    {
        if (_currentLine != null && _currentLine.Points.Count > 0)
        {
            _lines.Add(_currentLine);
            _currentLine = null; // Reset for the next line
        }
    }

    // Draw a circle at the clicked position
    public void DrawCircle(PointF center)
    {
        _circles.Add(new Circle(center, currentBrushSize, currentBrushSize, isEraser));
    }

    // Draw a dot at the clicked position
    public void DrawDot(PointF point)
    {
        // Draw the dot with the same size as the brush size
        _circles.Add(new Circle(point, currentBrushSize, currentBrushSize, isEraser));
    }


    // Clear all lines and circles
    public void Clear()
    {
        _lines.Clear();
        _circles.Clear();
    }

    // Draw the lines and circles on the canvas
    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        // Draw all lines
        foreach (var line in _lines)
        {
            if (line.Points.Count < 2) continue; // Ensure at least two points to draw

            canvas.StrokeSize = line.BrushSize;
            canvas.StrokeColor = line.IsEraser ? Colors.LightGray : Colors.Black;
            canvas.StrokeLineCap = LineCap.Round;

            for (int i = 1; i < line.Points.Count; i++)
            {
                canvas.DrawLine(line.Points[i - 1].X, line.Points[i - 1].Y, line.Points[i].X, line.Points[i].Y);
            }
        }

        // Draw all circles
        foreach (var circle in _circles)
        {
            canvas.FillColor = circle.IsEraser ? Colors.LightGray : Colors.Black;  // Set fill color
            canvas.FillEllipse(circle.Center.X - (circle.Radius / 2), circle.Center.Y - (circle.Radius / 2),
                   circle.Radius, circle.Radius);
        }

        // Optionally, draw the current line while the user is dragging (if there is one)
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
}