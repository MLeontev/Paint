using PluginInterface;
using System;
using System.Drawing;

namespace MedianFilter
{
    public class MedianFilter : IPlugin
    {
        public string Name => "Матричный медианный фильтр";

        public string Author => "Леонтьев Максим";

        public void Transform(Bitmap bitmap)
        {
            int windowWidth = 5;
            int windowHeight = 5;

            int width = bitmap.Width;
            int height = bitmap.Height;

            int edgex = windowWidth / 2;
            int edgey = windowHeight / 2;

            Color[,] newPoints = new Color[width, height];

            for (int x = edgex; x < width - edgex; x++)
            {
                for (int y = edgey; y < height - edgey; y++)
                {
                    int[] window = new int[windowWidth * windowHeight];

                    int i = 0;

                    for (int fx = 0; fx < windowWidth; fx++)
                    {
                        for (int fy = 0; fy < windowHeight; fy++)
                        {
                            int pixelX = x + fx - edgex;
                            int pixelY = y + fy - edgey;

                            Color pixelColor = bitmap.GetPixel(pixelX, pixelY);
                            int pixelValue = pixelColor.ToArgb();

                            window[i] = pixelValue;
                            i++;
                        }
                    }

                    Array.Sort(window);

                    int medianPixelValue = window[windowWidth * windowHeight / 2];
                    Color medianPixelColor = Color.FromArgb(medianPixelValue);

                    newPoints[x, y] = medianPixelColor;
                }
            }

            for (int x = edgex; x < width - edgex; x++)
            {
                for (int y = edgey; y < height - edgey; y++)
                {
                    bitmap.SetPixel(x, y, newPoints[x, y]);
                }
            }
        }
    }
}
