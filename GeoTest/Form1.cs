using System;
using System.Device.Location;
using System.Windows.Forms;

namespace GeoTest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private GeoCoordinateWatcher Watcher = null;

        private void button1_Click(object sender, EventArgs e)
        {
            Watcher = new GeoCoordinateWatcher();

            Watcher.StatusChanged += Watcher_StatusChanged;

            Watcher.Start();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }

        private void Watcher_StatusChanged(object sender, GeoPositionStatusChangedEventArgs e)
        {
            MessageBox.Show(Watcher.Status.ToString());

            if (e.Status == GeoPositionStatus.Ready)
            {
                if (Watcher.Position.Location.IsUnknown)
                {
                    tb1.Text = "Cannot find location data";
                }
                else
                {
                    tb1.Text = Watcher.Position.Location.Latitude.ToString();
                    tb2.Text = Watcher.Position.Location.Longitude.ToString();
                }
            }
        }
    }
}
