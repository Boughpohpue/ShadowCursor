using System;
using System.Collections.Generic;
using System.Windows;

namespace ShadowCursor
{
    public class LineSegment
    {
        private readonly Point _p1;
        private readonly Point _p2;
        private readonly double _length = -1.0;

        public Point P1 => _p1;
        public Point P2 => _p2;
        public double Length => _length;

        public LineSegment(Point p1, Point p2)
        {
            _p1 = p1;
            _p2 = p2;
            _length = Math.Sqrt(Math.Pow(p2.X - p1.X, 2) + Math.Pow(p2.Y - p1.Y, 2));
        }

        public Point GetSegmentPointAtDistance(double distance)
        {
            return new Point(
                (int)(P1.X + (distance * (P2.X - P1.X) / Length)),
                (int)(P1.Y + (distance * (P2.Y - P1.Y) / Length))
            );
        }

        public List<Point> GetSegmentPointsAtDistance(double d)
        {
            if (d > Length)
                throw new InvalidOperationException();

            var count = (int)(Length / d);
            var points = new List<Point> { P1 };
            for (var i = 1; i <= count; i++)
            {
                points.Add(GetSegmentPointAtDistance(d * i));
            }
            if (!points.Contains(P2)) points.Add(P2);
            return points;
        }
    }
}
