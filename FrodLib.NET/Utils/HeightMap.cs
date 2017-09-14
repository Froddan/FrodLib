using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;
using System.Diagnostics.CodeAnalysis;

/**
     * Code taken from http://www.float4x4.net/index.php/2010/06/generating-realistic-and-playable-terrain-height-maps/
     */
namespace FrodLib.Utils
{
    [ExcludeFromCodeCoverage]
    public class HeightMap
    {
        private float[,] heights;
        private PerlinGenerator perlin;
        public HeightMap(int size)
        {

            Size = size;
            heights = new float[size, size];

            perlin = new PerlinGenerator((int)DateTime.Now.Ticks);
        }

        public int Size { get; private set; }
        public float this[int x, int y]
        {
            get
            {
                return heights[x, y];
            }
            set
            {
                heights[x, y] = value;
            }
        }

        public void AddPerlinNoise(float f)
        {
            for (int i = 0; i < Size; i++)
            {
                for (int j = 0; j < Size; j++)
                {
                    heights[i, j] += perlin.Noise(f * i / (float)Size, f * j / (float)Size, 0);
                }
            }
        }

        public Bitmap CreateBitmap()
        {
            return CreateBitmap(1.0f);
        }

        public Bitmap CreateBitmap(float scale)
        {
            if (scale <= 0.0f)
            {
                throw new InvalidOperationException("Can't scale an image to zero or less");
            }

            Bitmap bmp = new Bitmap(Size, Size);

            for (int i = 0; i < Size; i++)
            {
                for (int j = 0; j < Size; j++)
                {
                    float height = heights[i, j];
                    if (height > 1) height = 1;
                    else if (height < -1) height = -1;
                    height += 1;
                    height *= 127.5f;
                    bmp.SetPixel((int)i, (int)j, Color.FromArgb((int)height, (int)height, (int)height));
                }
            }

            if (scale != 1.0f)
            {
                bmp = ScaleImage(bmp, scale);
            }

            return bmp;
        }

        public void Erode(float smoothness)
        {
            for (int i = 1; i < Size - 1; i++)
            {
                for (int j = 1; j < Size - 1; j++)
                {
                    float d_max = 0.0f;
                    int[] match = { 0, 0 };
                    for (int u = -1; u <= 1; u++)
                    {
                        for (int v = -1; v <= 1; v++)
                        {
                            if (Math.Abs(u) + Math.Abs(v) > 0)
                            {
                                float d_i = heights[i, j] - heights[i + u, j + v];
                                if (d_i > d_max)
                                {
                                    d_max = d_i;
                                    match[0] = u;
                                    match[1] = v;
                                }
                            }
                        }
                    }

                    if (0 < d_max && d_max <= (smoothness / (float)Size))
                    {
                        float d_h = 0.5f * d_max;
                        heights[i, j] -= d_h;
                        heights[i + match[0], j + match[1]] += d_h;
                    }
                }
            }
        }

        /// <summary>
        ///  Generate a heightmap using diamon square algorithm
        /// </summary>
        /// <param name="roughness">Surface 'roughness' of the map.Desiderable value are between 0.2 and 0.8</param>
        public void Generate(float roughness)
        {
            _SetMapValue(0, 0, (float)Size / 2);
            _SetMapValue(Size, 0, 0);
            _SetMapValue(Size, Size, (float)Size / 2);
            _SetMapValue(0, Size, Size);
            Random random = new Random((int)DateTime.Now.Ticks);
            Divide(Size, roughness, random);
        }

        public float[,] GetMap()
        {
            return heights;
        }


        public void Perturb(float f, float d)
        {
            int u, v;
            float[,] temp = new float[Size, Size];

            for (int i = 0; i < Size; i++)
            {
                for (int j = 0; j < Size; j++)
                {
                    u = i + (int)(perlin.Noise(f * i / (float)Size, f * j / (float)Size, 0) * d);
                    v = j + (int)(perlin.Noise(f * i / (float)Size, f * j / (float)Size, 1) * d);
                    if (u < 0)
                    {
                        u = 0;
                    }
                    else if (u >= Size)
                    {
                        u = Size - 1;
                    }

                    if (v < 0)
                    {
                        v = 0;
                    }
                    else if (v >= Size)
                    {
                        v = Size - 1;
                    }
                    temp[i, j] = heights[u, v];
                }
            }
            heights = temp;
        }

        public void SaveHeightMapToFile(string fileName)
        {
            SaveHeightMapToFile(fileName, 1);
        }

        /// <summary>
        /// Saves the height map to a file with the scale specified. 1 = current scale 
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="scale"></param>
        public void SaveHeightMapToFile(string fileName, float scale)
        {
            Bitmap bmp = CreateBitmap(scale);

            bmp.Save(fileName);
        }

        public void Smoothen()
        {
            for (int i = 1; i < Size - 1; i++)
            {
                for (int j = 1; j < Size - 1; j++)
                {
                    float total = 0.0f;
                    for (int u = -1; u <= 1; u++)
                    {
                        for (int v = -1; v <= 1; v++)
                        {
                            total += heights[i + u, j + v];
                        }
                    }
                    heights[i, j] = total / 9.0f;
                }
            }
        }

        private float _GetMapValue(int x, int y)
        {
            if (x < 0 || x > this.Size || y < 0 || y > this.Size) return -1;
            return heights[x, y];
        }

        private void _SetMapValue(int x, int y, float value)
        {
            heights[x, y] = value;
        }

        private float Average(float[] values)
        {
            float total = 0;
            float validLenght = 0;
            foreach (int dat in values)
            {
                if (dat != -1)
                {
                    total += dat;
                    validLenght++;
                }
            }

            return total / validLenght;
        }

        private void Diamond(int x, int y, int size, float offset)
        {
            float[] data = { _GetMapValue(x, y - size), _GetMapValue(x + size, y), _GetMapValue(x, y + size), _GetMapValue(x - size, y) };
            float ave = Average(data);
            _SetMapValue(x, y, ave + offset);
        }

        private void Divide(int size, float roughness, Random random)
        {
            var half = size / 2;
            var scale = roughness * size;
            if (half < 1) return;

            for (int y = half; y < Size; y += size)
            {
                for (int x = half; x < Size; x += size)
                {
                    Square(x, y, half, (float)(random.NextDouble() * scale * 2 - scale));
                }
            }
            for (int y = 0; y <= Size; y += half)
            {
                for (int x = (y + half) % size; x <= Size; x += size)
                {
                    Diamond(x, y, half, (float)(random.NextDouble() * scale * 2 - scale));
                }
            }
            Divide(size / 2, roughness, random);
        }

        /**
		 * <summary>
		 * Square phase averages four corner points before applying a random offset
		 * </summary>
		 *
		 * <param name="x">x position of the region</param>
		 * <param name="y">y position of the region</param>
		 * <param name="size">size of the region</param>
		 * <param name="offset">random offset</param>
		 */
        private Bitmap ScaleImage(Bitmap bmpToBeRescaled, float scale)
        {
            int scaleToSize = (int)(Size * scale);
            Bitmap scaledBitmap = new Bitmap(scaleToSize, scaleToSize);

            scaledBitmap.SetResolution(bmpToBeRescaled.HorizontalResolution, bmpToBeRescaled.VerticalResolution);

            using (Graphics graphics = Graphics.FromImage(scaledBitmap))
            {
                graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                graphics.DrawImage(bmpToBeRescaled, 0, 0, scaledBitmap.Width, scaledBitmap.Height);
            }

            return scaledBitmap;
        }

        private void Square(int x, int y, int size, float offset)
        {
            float[] data = { _GetMapValue(x - size, y - size), _GetMapValue(x + size, y - size), _GetMapValue(x + size, y + size), _GetMapValue(x - size, y + size) };
            float ave = Average(data);
            _SetMapValue(x, y, ave + offset);
        }

        public static explicit operator float[,] (HeightMap heightMap)
        {
            return heightMap.heights;
        }

    }
}
