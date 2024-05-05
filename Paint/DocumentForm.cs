using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;

namespace Paint
{
    public partial class DocumentForm : Form
    {
        private int x, y;
        public Bitmap bitmap;

        private bool isDrawing;
        public bool changed;

        private Point startPoint;

        public int Width { get; set; }
        public int Height { get; set; }
        public double Scale { get; set; }


        public DocumentForm()
        {
            InitializeComponent();

            changed = false;

            Width = 500;
            Height = 300;

            Color background = Color.White;

            bitmap = new Bitmap(Width, Height);
            Graphics g = Graphics.FromImage(bitmap);
            g.Clear(background);

            isDrawing = false;

            Scale = 1;
        }

        private void DocumentForm_Load(object sender, EventArgs e)
        {
            Text = GetFileName();
        }

        private string GetFileName()
        {
            if (string.IsNullOrEmpty(path))
            {
                return "Новое изображение";
            }
            else
            {
                return Path.GetFileNameWithoutExtension(path);
            }
        }

        private void DocumentForm_MouseDown(object sender, MouseEventArgs e)
        {
            x = e.X;
            y = e.Y;

            startPoint = new Point(e.X, e.Y);

            isDrawing = true;
        }

        private void DocumentForm_MouseUp(object sender, MouseEventArgs e)
        {
            isDrawing = false;

            switch (MainForm.CurrentTool)
            {
                case Tool.Line:
                    {
                        Graphics g = Graphics.FromImage(bitmap);
                        g.DrawLine(new Pen(MainForm.Color, (float)(MainForm.Thickness * Scale)), startPoint, new Point(e.X, e.Y));
                        Invalidate();
                        changed = true;
                        break;
                    }
                case Tool.Ellipse:
                    {
                        int width = Math.Abs(e.X - startPoint.X);
                        int height = Math.Abs(e.Y - startPoint.Y);
                        int x = Math.Min(startPoint.X, e.X);
                        int y = Math.Min(startPoint.Y, e.Y);
                        Graphics g = Graphics.FromImage(bitmap);
                        g.DrawEllipse(new Pen(MainForm.Color, (float)(MainForm.Thickness * Scale)), x, y, width, height);
                        Invalidate();
                        changed = true;
                        break;
                    }
                case Tool.Star:
                    {
                        Graphics g = Graphics.FromImage(bitmap);

                        float d = (float)Math.Sqrt(Math.Pow(e.X - startPoint.X, 2) + Math.Pow(e.Y - startPoint.Y, 2));
                        float R = d / 2f;
                        float r = R * MainForm.RadiusRatio;

                        DrawStar(g, new Pen(MainForm.Color, (float)(MainForm.Thickness * Scale)), startPoint, MainForm.n, R, r);

                        Invalidate();
                        changed = true;
                        break;
                    }
                default:
                    break;
            }
        }

        private void DrawStar(Graphics g, Pen pen, Point center, int n, float R, float r)
        {
            PointF[] points = new PointF[n * 2 + 1];
            double angle = 0;
            double k;

            for (int i = 0; i < 2 * n + 1; i++)
            {
                k = i % 2 == 0 ? r : R;
                points[i] = new PointF((float)(center.X + k * Math.Cos(angle)),
                                       (float)(center.Y + k * Math.Sin(angle)));
                angle += Math.PI / n;
            }

            g.DrawPolygon(pen, points);
        }


        private void DocumentForm_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDrawing)
            {
                Graphics g = Graphics.FromImage(bitmap);

                switch (MainForm.CurrentTool)
                {
                    case Tool.Pen:
                        {
                            g.DrawLine(new Pen(MainForm.Color, (float)(MainForm.Thickness * Scale)), x, y, e.X, e.Y);
                            break;
                        }
                    case Tool.Eraser:
                        {
                            g.DrawLine(new Pen(Color.White, (float)(MainForm.Thickness * Scale)), x, y, e.X, e.Y);
                            break;
                        }
                    default:
                        break;
                }

                Invalidate();
                x = e.X;
                y = e.Y;
                changed = true;
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            e.Graphics.DrawImage(bitmap, 0, 0);
        }

        public void ChangeSize(int width, int height)
        {
            Width = width; Height = height;

            int prevWidth = bitmap.Width;
            int prevHeight = bitmap.Height;

            Bitmap newBitmap = new Bitmap(width, height);

            Graphics g = Graphics.FromImage(newBitmap);
            g.Clear(Color.White);
            g.DrawImage(bitmap, 0, 0, prevWidth, prevHeight);

            bitmap = newBitmap;
            Invalidate();

            changed = true;
        }


        private string path;

        public void SaveAs()
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.AddExtension = true;
            dlg.Filter = "Windows Bitmap (*.bmp)|*.bmp| Файлы JPEG (*.jpeg, *.jpg)|*.jpeg;*.jpg";
            ImageFormat[] imageFormats = { ImageFormat.Bmp, ImageFormat.Jpeg };

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                ChangeSize(Width, Height);
                path = dlg.FileName;
                bitmap.Save(path, imageFormats[dlg.FilterIndex - 1]);
                Text = Path.GetFileNameWithoutExtension(path);
                changed = false;
            }
        }

        public void Save()
        {
            if (string.IsNullOrEmpty(path))
            {
                SaveAs();
            }
            else
            {
                ChangeSize(Width, Height);

                ImageFormat format;

                string extension = Path.GetExtension(path);
                switch (extension)
                {
                    case ".bmp":
                        format = ImageFormat.Bmp;
                        break;
                    case ".jpg":
                        format = ImageFormat.Jpeg;
                        break;
                    case ".jpeg":
                        format = ImageFormat.Jpeg;
                        break;
                    default:
                        throw new Exception("Формат файла не поддерживается.");
                }

                using (FileStream fileStream = new FileStream(path, FileMode.Create))
                {
                    bitmap.Save(fileStream, format);
                    changed = false;
                }
            }
        }

        public void Open()
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Windows Bitmap (*.bmp)|*.bmp| Файлы JPEG (*.jpeg, *.jpg)|*.jpeg;*.jpg";

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                path = dlg.FileName;
                using (FileStream fileStream = new FileStream(path, FileMode.Open))
                {
                    bitmap = new Bitmap(fileStream);
                    Width = bitmap.Width;
                    Height = bitmap.Height;
                }
                Invalidate();
            }
        }

        private void DocumentForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (changed)
            {
                string name = GetFileName();

                DialogResult res = MessageBox.Show($"Сохранить изменения в файле {name}?", "Paint", MessageBoxButtons.YesNoCancel);
                
                if (res == DialogResult.Yes)
                {
                    if (string.IsNullOrEmpty(path))
                    {
                        SaveAs();
                        e.Cancel = string.IsNullOrEmpty(path);
                    }
                    else
                    {
                        Save();
                    }
                }
                else if (res == DialogResult.Cancel)
                {
                    e.Cancel = true;
                }
            }
        }


        public void ZoomIn()
        {
            Scale += 0.1;
            Zoom();
        }

        public void ZoomOut()
        {
            if (Scale > 0.1)
            {
                Scale -= 0.1;
                Zoom();
            }
        }

        private void Zoom()
        {
            int newWidth = (int)(Width * Scale);
            int newHeight = (int)(Height * Scale);

            Bitmap newBitmap = new Bitmap(newWidth, newHeight);
            Graphics g = Graphics.FromImage(newBitmap);
            g.Clear(Color.White);
            g.DrawImage(bitmap, 0, 0, newWidth, newHeight);

            bitmap = newBitmap;
            Invalidate();
        }
    }
}
