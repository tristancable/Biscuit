using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitRecognizer
{
    internal class Canvas : IDrawable
    {
        private List<PointF> _points = new List<PointF>();

        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            canvas.StrokeColor = Colors.Black;
            canvas.StrokeSize = 3;

            for (int i = 1; i < _points.Count; i++)
            {
                canvas.DrawLine(_points[i - 1].X, _points[i - 1].Y, _points[i].X, _points[i].Y);
            }
        }

        public void AddPoint(PointF point)
        {
            _points.Add(point);
        }

        public void Clear()
        {
            _points.Clear();
        }
    }
}
