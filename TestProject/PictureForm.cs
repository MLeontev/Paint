using System.Windows.Forms;
using System.Drawing;
using System.IO;
using System.Reflection;
using System;
using PluginInterface;
using System.Collections.Generic;
using System.Device.Location;
using System.Threading.Tasks;
using System.Threading;

namespace TestProject
{
    public partial class PictureForm : Form
    {
        Dictionary<string, IPlugin> plugins = new Dictionary<string, IPlugin>();

        public PictureForm()
        {
            InitializeComponent();
            FindPlugins();
            CreatePluginsMenu();
        }

        void FindPlugins()
        {
            string folder = AppDomain.CurrentDomain.BaseDirectory;

            string[] files = Directory.GetFiles(folder, "*.dll");

            foreach (string file in files)
            {
                try
                {
                    Assembly assembly = Assembly.LoadFile(file);

                    foreach (Type type in assembly.GetTypes())
                    {
                        Type iface = type.GetInterface("PluginInterface.IPlugin");

                        if (iface != null)
                        {
                            IPlugin plugin = (IPlugin)Activator.CreateInstance(type);
                            plugins.Add(plugin.Name, plugin);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка загрузки плагина\n" + ex.Message);
                }
            }
        }

        private void CreatePluginsMenu()
        {
            foreach (var p in plugins)
            {
                var item = filtersToolStripMenuItem.DropDownItems.Add(p.Value.Name);
                item.Click += OnPluginClick;
            }
        }

        private void OnPluginClick(object sender, EventArgs args)
        {
            IPlugin plugin = plugins[((ToolStripMenuItem)sender).Text];
            plugin.Filtered += (s, e) =>
            {
                pictureBox.Refresh();
            };
            plugin.Transform((Bitmap)pictureBox.Image);
        }



        private void button1_Click(object sender, EventArgs e)
        {
            Bitmap bitmap = (Bitmap)pictureBox.Image;

            using (Graphics g = Graphics.FromImage(pictureBox.Image))
            {
                string date = DateTime.Now.ToString("d");

                
                PointF dateLocation = new PointF(bitmap.Width - 150, bitmap.Height - 45);
                g.DrawString(date, new Font("Arial", 12), Brushes.DarkOrange, dateLocation);

                GeoCoordinateWatcher watcher = new GeoCoordinateWatcher();
                watcher.StatusChanged += Watcher_StatusChanged;
                watcher.Start();
            }

            pictureBox.Refresh();
        }

        private void Watcher_StatusChanged(object sender, GeoPositionStatusChangedEventArgs e)
        {
            Bitmap bitmap = (Bitmap)pictureBox.Image;
            GeoCoordinateWatcher watcher = (GeoCoordinateWatcher)sender;

            using (Graphics g = Graphics.FromImage(pictureBox.Image))
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

            pictureBox.Refresh();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Bitmap bitmap = (Bitmap)pictureBox.Image;

            using (Graphics g = Graphics.FromImage(bitmap))
            {
                string date = DateTime.Now.ToString("d");
                PointF dateLocation = new PointF(bitmap.Width - 150, bitmap.Height - 45);
                g.DrawString(date, new Font("Arial", 12), Brushes.DarkOrange, dateLocation);
            }

            GeoCoordinateWatcher watcher = new GeoCoordinateWatcher();
            watcher.StatusChanged += (wsender, we) =>
            {
                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    if (we.Status == GeoPositionStatus.Ready)
                    {
                        GeoCoordinate coord = watcher.Position.Location;
                        if (!coord.IsUnknown)
                        {
                            PointF coordLocation = new PointF(bitmap.Width - 150, bitmap.Height - 25);
                            g.DrawString($"{coord.Latitude}; {coord.Longitude}", new Font("Arial", 12), Brushes.DarkOrange, coordLocation);
                        }
                    }
                }
            };

            watcher.Start();

            pictureBox.Refresh();
        }
    }
}
