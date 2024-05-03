using System.Windows.Forms;
using System.Drawing;
using System.IO;
using System.Reflection;
using System;
using PluginInterface;
using System.Collections.Generic;

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
            plugin.Transform((Bitmap)pictureBox.Image);
            pictureBox.Refresh();
        }
    }
}
