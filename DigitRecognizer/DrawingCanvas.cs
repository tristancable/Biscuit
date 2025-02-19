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
        foreach (var line in _lines)
        {
            if (line.Count > 1)
            {
                // Create a PathF to draw the line
                var path = new PathF();
                path.MoveTo(line[0].X, line[0].Y); // Start at the first point

                // Add all points to the path
                for (int i = 1; i < line.Count; i++)
                {
                    path.LineTo(line[i].X, line[i].Y);
                }

                // Draw the path
                canvas.StrokeColor = Colors.Black;
                canvas.StrokeSize = 10;
                canvas.DrawPath(path);
            }
        }
    }
}