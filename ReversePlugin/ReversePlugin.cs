using PluginInterface;
using System.Drawing;
using System.Windows.Forms;

namespace Plugins
{
    public class ReversePlugin : IPlugin
    {
        public string Name => "Переворот изображения";

        public string Author => "Леонтьев Максим";

        public void Transform(Bitmap bitmap)
        {
            MessageBox.Show("Применяем фильтр");
            for (int i = 0; i < bitmap.Width; i++)
            {
                for (int j = 0; j < bitmap.Height; j++)
                {
                    bitmap.SetPixel(i, j, Color.Black);
                }
            }
            MessageBox.Show("Применили фильтр");
        }
    }
}
