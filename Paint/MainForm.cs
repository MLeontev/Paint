using PluginInterface;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace Paint
{
    public partial class MainForm : Form
    {
        public static Tool CurrentTool { get; set; }
        public static Color Color { get; set; }
        public static int Thickness { get; set; }

        public static int n { get; set; }
        public static float RadiusRatio { get; set; }

        Dictionary<string, IPlugin> plugins = new Dictionary<string, IPlugin>();

        public MainForm()
        {
            InitializeComponent();
            Color = Color.Black;
            Thickness = 3;
            CurrentTool = Tool.Pen;
            threePxToolStripMenuItem.Checked = true;
            пероToolStripMenuItem.Checked = true;

            n = 5;
            RadiusRatio = 0.4f;

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
                var item = фильтрыToolStripMenuItem.DropDownItems.Add(p.Value.Name);
                item.Click += OnPluginClick;
            }
        }

        private void OnPluginClick(object sender, EventArgs args)
        {
            DocumentForm activeDocumentForm = ActiveMdiChild as DocumentForm;

            IPlugin plugin = plugins[((ToolStripMenuItem)sender).Text];
            plugin.Filtered += (s, e) =>
            {
                activeDocumentForm.Invalidate();
                activeDocumentForm.changed = true;
            };
            plugin.Transform(activeDocumentForm.bitmap);
        }

        private void выходToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void оПрограммеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var aboutForm = new AboutForm();
            aboutForm.ShowDialog();
        }

        private void новыйToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var documentForm = new DocumentForm();
            documentForm.MdiParent = this;
            documentForm.Show();
        }

        private void рисунокToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            размерХолстаToolStripMenuItem.Enabled = !(ActiveMdiChild == null);
            масштабToolStripMenuItem.Enabled = !(ActiveMdiChild == null);
            масштабToolStripMenuItem1.Enabled = !(ActiveMdiChild == null);
        }

        private void файлToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            сохранитьToolStripMenuItem.Enabled = !(ActiveMdiChild == null);
            сохранитьКакToolStripMenuItem.Enabled = !(ActiveMdiChild == null);
        }

        private void красныйToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Color = Color.Red;
        }

        private void синийToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Color = Color.Blue;
        }

        private void зеленыйToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Color = Color.Green;
        }

        private void другойToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ColorDialog cd = new ColorDialog();
            if (cd.ShowDialog() == DialogResult.OK)
                Color = cd.Color;
        }

        private void размерХолстаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DocumentForm activeDocumentForm = ActiveMdiChild as DocumentForm;

            var sizeForm = new CanvasSizeForm(activeDocumentForm);
            bool ok = false;
            do
            {
                sizeForm.ShowDialog();

                if (sizeForm.DialogResult == DialogResult.OK)
                {
                    int newWidth, newHeight;

                    if (int.TryParse(sizeForm.GetWidth(), out newWidth) && int.TryParse(sizeForm.GetHeight(), out newHeight))
                    {
                        activeDocumentForm.ChangeSize(newWidth, newHeight);
                        ok = true;
                    }
                    else
                    {
                        MessageBox.Show("Введены некорректные значения для ширины и высоты. Попробуйте снова.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        ok = false;
                    }
                }
                else if (sizeForm.DialogResult == DialogResult.Cancel)
                {
                    ok = true;
                }
            } while (!ok);
        }

        private void каскадомToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.Cascade);
        }

        private void слеваНаправоToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.TileVertical);
        }

        private void сверхуВнизToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.TileHorizontal);
        }

        private void упорядочитьЗначкиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.ArrangeIcons);
        }

        private void сохранитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DocumentForm activeDocumentForm = ActiveMdiChild as DocumentForm;
            activeDocumentForm.Save();
        }

        private void сохранитьКакToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DocumentForm activeDocumentForm = ActiveMdiChild as DocumentForm;
            activeDocumentForm.SaveAs();
        }

        private void открытьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var documentForm = new DocumentForm();
            documentForm.MdiParent = this;
            documentForm.Open();
            documentForm.Show();
        }

        private void onePxToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Thickness = 1;

            onePxToolStripMenuItem.Checked = true;
            foreach (ToolStripMenuItem item in (toolStripDropDownButton2).DropDownItems)
            {
                if (item != onePxToolStripMenuItem)
                {
                    item.Checked = false;
                }
            }
        }

        private void threePxToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Thickness = 3;

            threePxToolStripMenuItem.Checked = true;
            foreach (ToolStripMenuItem item in (toolStripDropDownButton2).DropDownItems)
            {
                if (item != threePxToolStripMenuItem)
                {
                    item.Checked = false;
                }
            }
        }

        private void fivePxToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Thickness = 5;
            

            fivePxToolStripMenuItem.Checked = true;
            foreach (ToolStripMenuItem item in (toolStripDropDownButton2).DropDownItems)
            {
                if (item != fivePxToolStripMenuItem)
                {
                    item.Checked = false;
                }
            }
        }

        private void eightPxToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Thickness = 8;

            eightPxToolStripMenuItem.Checked = true;
            foreach (ToolStripMenuItem item in (toolStripDropDownButton2).DropDownItems)
            {
                if (item != eightPxToolStripMenuItem)
                {
                    item.Checked = false;
                }
            }
        }

        private void пероToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CurrentTool = Tool.Pen;

            пероToolStripMenuItem.Checked = true;
            foreach (ToolStripMenuItem item in (toolStripDropDownButton3).DropDownItems)
            {
                if (item != пероToolStripMenuItem)
                {
                    item.Checked = false;
                }
            }
            toolStripButton1.Enabled = false;
        }

        private void линияToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CurrentTool = Tool.Line;

            линияToolStripMenuItem.Checked = true;
            foreach (ToolStripMenuItem item in (toolStripDropDownButton3).DropDownItems)
            {
                if (item != линияToolStripMenuItem)
                {
                    item.Checked = false;
                }
            }
            toolStripButton1.Enabled = false;
        }

        private void эллипсToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CurrentTool = Tool.Ellipse;

            эллипсToolStripMenuItem.Checked = true;
            foreach (ToolStripMenuItem item in (toolStripDropDownButton3).DropDownItems)
            {
                if (item != эллипсToolStripMenuItem)
                {
                    item.Checked = false;
                }
            }
            toolStripButton1.Enabled = false;
        }

        private void ластикToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CurrentTool = Tool.Eraser;

            ластикToolStripMenuItem.Checked = true;
            foreach (ToolStripMenuItem item in (toolStripDropDownButton3).DropDownItems)
            {
                if (item != ластикToolStripMenuItem)
                {
                    item.Checked = false;
                }
            }
            toolStripButton1.Enabled = false;
        }

        private void звездаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CurrentTool = Tool.Star;

            звездаToolStripMenuItem.Checked = true;
            foreach (ToolStripMenuItem item in (toolStripDropDownButton3).DropDownItems)
            {
                if (item != звездаToolStripMenuItem)
                {
                    item.Checked = false;
                }
            }

            toolStripButton1.Enabled = true;
        }

        private void масштабToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DocumentForm activeDocumentForm = ActiveMdiChild as DocumentForm;
            activeDocumentForm.ZoomIn();
        }

        private void масштабToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            DocumentForm activeDocumentForm = ActiveMdiChild as DocumentForm;
            activeDocumentForm.ZoomOut();
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            var starForm = new StarParametrsForm();
            bool ok = false;
            do
            {
                starForm.ShowDialog();

                if (starForm.DialogResult == DialogResult.OK)
                {
                    float newRatio;
                    int newN;

                    if (int.TryParse(starForm.GetNumber(), out newN) && newN > 0 && newN <= 10 &&
                        float.TryParse(starForm.GetRatio(), out newRatio) && newRatio > 0)
                    {
                        ok = true;
                        n = newN;
                        RadiusRatio = (float)newRatio;
                    }
                    else
                    {
                        MessageBox.Show("Неверные параметры. Попробуйте снова.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        ok = false;
                    }
                }
                else if (starForm.DialogResult == DialogResult.Cancel)
                {
                    ok = true;
                }
            } while (!ok);
        }

        private void окноToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            int childFormCount = 0;
            foreach (Form childForm in MdiChildren)
            {
                childFormCount++;
            }

            bool enabled = childFormCount >= 2;

            каскадомToolStripMenuItem.Enabled = enabled;
            сверхуВнизToolStripMenuItem.Enabled = enabled;
            слеваНаправоToolStripMenuItem.Enabled = enabled;
            упорядочитьЗначкиToolStripMenuItem.Enabled = enabled;
        }
    }
}
