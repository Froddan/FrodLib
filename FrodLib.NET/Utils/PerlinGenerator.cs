using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace FrodLib.Utils
{
    [ExcludeFromCodeCoverage]
    public class PerlinGenerator
    {
        private const int GradientSizeTable = 256;
        private readonly Random _random;
        private readonly float[] _gradients = new float[GradientSizeTable * 3];

        private readonly byte[] _perm = new byte[GradientSizeTable];

        public PerlinGenerator(int seed)
        {
            _random = new Random(seed);
            _random.NextBytes(_perm);
            InitGradients();

        }

        public float Noise(float x, float y, float z)
        {
            int ix = (int)Math.Floor(x);
            float fx0 = x - ix;
            float fx1 = fx0 - 1;
            float wx = Smooth(fx0);

            int iy = (int)Math.Floor(y);
            float fy0 = y - iy;
            float fy1 = fy0 - 1;
            float wy = Smooth(fy0);

            int iz = (int)Math.Floor(z);
            float fz0 = z - iz;
            float fz1 = fz0 - 1;
            float wz = Smooth(fz0);

            float vx0 = Lattice(ix, iy, iz, fx0, fy0, fz0);
            float vx1 = Lattice(ix + 1, iy, iz, fx1, fy0, fz0);
            float vy0 = Lerp(wx, vx0, vx1);

            vx0 = Lattice(ix, iy + 1, iz, fx0, fy1, fz0);
            vx1 = Lattice(ix + 1, iy + 1, iz, fx1, fy1, fz0);
            float vy1 = Lerp(wx, vx0, vx1);

            float vz0 = Lerp(wz, vy0, vy1);

            vx0 = Lattice(ix, iy, iz + 1, fx0, fy0, fz1);
            vx1 = Lattice(ix + 1, iy, iz + 1, fx1, fy0, fz1);
            vy0 = Lerp(wx, vx0, vx1);

            vx0 = Lattice(ix, iy + 1, iz + 1, fx0, fy1, fz1);
            vx1 = Lattice(ix + 1, iy + 1, iz + 1, fx1, fy1, fz1);
            vy1 = Lerp(wx, vx0, vx1);

            float vz1 = Lerp(wz, vy0, vy1);
            return Lerp(wz, vz0, vz1);
        }

        private void InitGradients()
        {
            for (int i = 0; i < GradientSizeTable; i++)
            {
                float z = 1f - 2f * (float)_random.NextDouble();
                float r = (float)Math.Sqrt(1f - z * z);
                float theta = 2 * (float)Math.PI * (float)_random.NextDouble();

                _gradients[i * 3] = r * (float)Math.Cos(theta);
                _gradients[i * 3 + 1] = r * (float)Math.Sin(theta);
                _gradients[i * 3 + 2] = z;
            }
        }

        private int Permute(int x)
        {
            const int mask = GradientSizeTable - 1;
            return _perm[x & mask];
        }

        private int Index(int ix, int iy, int iz)
        {
            return Permute(ix + Permute(iy + Permute(iz)));
        }

        private float Lattice(int ix, int iy, int iz, float fx, float fy, float fz)
        {
            int index = Index(ix, iy, iz);
            int g = index * 3;
            return _gradients[g] * fx + _gradients[g + 1] * fy + _gradients[g + 2] * fz;
        }

        private float Lerp(float t, float value0, float value1)
        {
            return value0 + t * (value1 - value0);
        }

        private float Smooth(float x)
        {
            return x * x * (3 - 2 * x);
        }
    }
}
