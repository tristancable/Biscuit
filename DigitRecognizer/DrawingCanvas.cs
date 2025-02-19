using Microsoft.Maui.Graphics;

public class DrawingCanvas : IDrawable
{
    private List<PointF> _points = new List<PointF>();

    public void AddPoint(PointF point)
    {
        _points.Add(point);
    }

    public void Clear()
    {
        _points.Clear();
    }

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        // Set stroke properties (color, width)
        canvas.StrokeColor = Colors.Black;
        canvas.StrokeSize = 2;

        // Draw each point as a small line segment
        if (_points.Count > 1)
        {
            for (int i = 0; i < _points.Count - 1; i++)
            {
                canvas.DrawLine(_points[i], _points[i + 1]);
            }
        }
    }
}
