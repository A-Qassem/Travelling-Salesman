using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace TSP
{
    public class Visualization
    {
        private Color _backgroundColor = Color.FromArgb(240, 248, 255);
        private Color _improvedColor = Color.FromArgb(230, 255, 230);
        private Color _cityColor = Color.OrangeRed;
        private Color _pathColor = Color.MediumSeaGreen;
        private Color _currentPathColor = Color.FromArgb(150, 70, 130, 180);

        public bool ShowCityLabels { get; set; } = true;
        public bool ShowCurrentPath { get; set; } = true;
        public bool Improved { get; set; } = false;

        public void DrawTSP(Graphics g, List<City> cities, List<int> bestPath, List<int> currentBestPath)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Draw grid background
            using (Pen gridPen = new Pen(Color.FromArgb(20, 0, 0, 0)))
            {
                int gridSize = 25;
                for (int x = 0; x < 2000; x += gridSize) // Use a large enough width
                    g.DrawLine(gridPen, x, 0, x, 2000);  // Use a large enough height

                for (int y = 0; y < 2000; y += gridSize)
                    g.DrawLine(gridPen, 0, y, 2000, y);
            }

            // Draw current generation best path if enabled and available
            if (ShowCurrentPath && currentBestPath.Count > 0 && cities.Count > 0)
            {
                using (Pen pathPen = new Pen(_currentPathColor, 1.5f))
                {
                    Point prev = cities[0].Position;
                    foreach (var idx in currentBestPath)
                    {
                        if (idx < cities.Count) // Safety check
                        {
                            Point next = cities[idx].Position;
                            g.DrawLine(pathPen, prev, next);
                            prev = next;
                        }
                    }
                    g.DrawLine(pathPen, prev, cities[0].Position);
                }
            }

            // Draw best overall path if available
            if (bestPath.Count > 0 && cities.Count > 0)
            {
                using (Pen pathPen = new Pen(_pathColor, 2.5f))
                {
                    Point prev = cities[0].Position;
                    foreach (var idx in bestPath)
                    {
                        if (idx < cities.Count) // Safety check
                        {
                            Point next = cities[idx].Position;
                            g.DrawLine(pathPen, prev, next);
                            // Draw an arrow to show direction
                            DrawArrow(g, prev, next, pathPen);
                            prev = next;
                        }
                    }
                    g.DrawLine(pathPen, prev, cities[0].Position);
                    DrawArrow(g, prev, cities[0].Position, pathPen);
                }
            }

            // Draw cities
            for (int i = 0; i < cities.Count; i++)
            {
                City city = cities[i];
                Point position = city.Position;
                int size = i == 0 ? 14 : 12;
                Color fillColor = i == 0 ? Color.Blue : _cityColor;

                g.FillEllipse(new SolidBrush(fillColor), position.X - size / 2, position.Y - size / 2, size, size);
                g.DrawEllipse(new Pen(Color.Black, 1.5f), position.X - size / 2, position.Y - size / 2, size, size);

                // Draw city labels if enabled
                if (ShowCityLabels)
                {
                    string label = i.ToString();
                    Font labelFont = new Font("Arial", 9, FontStyle.Bold);
                    SizeF textSize = g.MeasureString(label, labelFont);

                    // Draw semi-transparent white background for text
                    g.FillRectangle(
                        new SolidBrush(Color.FromArgb(180, 255, 255, 255)),
                        position.X - textSize.Width / 2,
                        position.Y - size / 2 - textSize.Height - 2,
                        textSize.Width,
                        textSize.Height
                    );

                    g.DrawString(
                        label,
                        labelFont,
                        Brushes.Black,
                        position.X - textSize.Width / 2,
                        position.Y - size / 2 - textSize.Height - 2
                    );
                }
            }
        }

        private void DrawArrow(Graphics g, Point start, Point end, Pen pen)
        {
            // Only draw arrows if cities are far enough apart
            double distance = Math.Sqrt(Math.Pow(end.X - start.X, 2) + Math.Pow(end.Y - start.Y, 2));
            if (distance < 50) return;

            // Calculate arrow position (at 70% of the way from start to end)
            float ratio = 0.7f;
            Point arrowPos = new Point(
                (int)(start.X + (end.X - start.X) * ratio),
                (int)(start.Y + (end.Y - start.Y) * ratio)
            );

            // Calculate angle
            double angle = Math.Atan2(end.Y - start.Y, end.X - start.X);

            // Create arrow head
            Point[] arrowHead = new Point[3];
            int arrowSize = 8;
            arrowHead[0] = arrowPos;
            arrowHead[1] = new Point(
                (int)(arrowPos.X - arrowSize * Math.Cos(angle - Math.PI / 6)),
                (int)(arrowPos.Y - arrowSize * Math.Sin(angle - Math.PI / 6))
            );
            arrowHead[2] = new Point(
                (int)(arrowPos.X - arrowSize * Math.Cos(angle + Math.PI / 6)),
                (int)(arrowPos.Y - arrowSize * Math.Sin(angle + Math.PI / 6))
            );

            // Draw arrow head
            g.FillPolygon(new SolidBrush(pen.Color), arrowHead);
        }

        public Color GetBackgroundColor(bool improved)
        {
            return improved ? _improvedColor : _backgroundColor;
        }
    }
}