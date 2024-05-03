using PluginInterface;
using System;
using System.Drawing;

namespace GrayScaleFilter
{
    [Version(1, 0)]
    public class GrayScaleFilter : IPlugin
    {
        public string Name => "Черно-белый фильтр";

        public string Author => "Леонтьев Максим";

        public void Transform(Bitmap bitmap)
        {
            for (int i = 0; i < bitmap.Width; i++)
            {
                for (int j = 0; j < bitmap.Height; j++)
                {
                    Color c = bitmap.GetPixel(i, j);
                    byte gray = (byte)(.299 * c.R + .587 * c.G + .114 * c.B);
                    bitmap.SetPixel(i, j, Color.FromArgb(gray, gray, gray));
                }
            }
        }
    }
}
