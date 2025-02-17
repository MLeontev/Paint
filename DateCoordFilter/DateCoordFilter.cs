﻿using PluginInterface;
using System;
using System.Drawing;
using System.Device.Location;

namespace DateCoordFilter
{
    [Version(1, 0)]
    public class DateCoordFilter : IPlugin
    {
        public string Name => "Добавить дату и координаты";

        public string Author => "Леонтьев Максим";

        public event EventHandler Filtered;

        public void Transform(Bitmap bitmap)
        {
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                string date = DateTime.Now.ToString("d");
                PointF dateLocation = new PointF(bitmap.Width - 150, bitmap.Height - 45);
                g.DrawString(date, new Font("Arial", 12), Brushes.DarkOrange, dateLocation);
                Filtered?.Invoke(this, EventArgs.Empty);
            }

            GeoCoordinateWatcher watcher = new GeoCoordinateWatcher();
            watcher.StatusChanged += (sender, e) =>
            {
                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    if (e.Status == GeoPositionStatus.Ready)
                    {
                        GeoCoordinate coord = watcher.Position.Location;
                        if (!coord.IsUnknown)
                        {
                            PointF coordLocation = new PointF(bitmap.Width - 150, bitmap.Height - 25);
                            g.DrawString($"{coord.Latitude}; {coord.Longitude}", new Font("Arial", 12), Brushes.DarkOrange, coordLocation);
                        }
                    }
                }
                Filtered?.Invoke(this, EventArgs.Empty);
            };

            watcher.Start();
        }
    }
}
