public class DrawingCanvas : IDrawable
{
    private List<List<PointF>> _lines = new List<List<PointF>>(); // Stores multiple lines
    private List<PointF> _currentLine = new List<PointF>(); // Stores current line

    public void AddPoint(PointF point)
    {
        _currentLine.Add(point);
    }

    public void StartNewLine()
    {
        _currentLine = new List<PointF>(); // Start a new line
        _lines.Add(_currentLine); // Add the new line to the list of lines
    }

    public void Clear()
    {
        _lines.Clear(); // Clear all lines
    }

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        canvas.FillColor = Colors.Black;
        float brushSize = 5; // Adjust size as needed

        foreach (var line in _lines)
        {
            for (int i = 1; i < line.Count; i++)
            {
                var p1 = line[i - 1];
                var p2 = line[i];

                float distance = (float)Math.Sqrt(Math.Pow(p2.X - p1.X, 2) + Math.Pow(p2.Y - p1.Y, 2));
                int steps = Math.Max(1, (int)(distance / (brushSize * 0.5))); // Ensures at least 1 step

                for (int j = 0; j <= steps; j++)
                {
                    float t = j / (float)steps;
                    float x = p1.X + (p2.X - p1.X) * t;
                    float y = p1.Y + (p2.Y - p1.Y) * t;

                    canvas.FillCircle(x, y, brushSize);
                }
            }
        }
    }
}