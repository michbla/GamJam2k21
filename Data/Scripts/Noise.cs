using System;
using System.Linq;
using OpenTK.Mathematics;
using System.Threading.Tasks;

namespace GamJam2k21
{
    public static class Noise2D
    {
        private static readonly Random _random = new Random();

        private static int[] _permutation;

        private static Vector2[] _gradients;

        static Noise2D()
        {
            _permutation = calculatePermutation();
            _gradients = calculateGradients();
        }

        public static float[,] GenerateNoiseMap(int width, int height, int octaves, float freq)
        {
            var data = new float[height, width];

            var min = float.MaxValue;
            var max = float.MinValue;

            reseed();

            var frequency = freq;
            var amplitude = 1f;

            for (var octave = 0; octave < octaves; octave++)
            {
                Parallel.For(0, width * height, (offset) =>
                {
                    var j = offset % width;
                    var i = offset / width;
                    var noise = generateNoise(i * frequency * 1f / width, j * frequency * 1f / height);
                    noise = data[i, j] += noise * amplitude;

                    min = MathHelper.Min(min, noise);
                    max = MathHelper.Max(max, noise);
                }
                );
                frequency *= 4;
                amplitude /= 10;
            }
            float[,] result = new float[width, height];
            for (var i = 0; i < height; i++)
                for (var j = 0; j < width; j++)
                    result[j, i] = MathHelper.Clamp((data[i, j] - min) / (max - min), 0.0f, 1.0f);

            return result;
        }

        private static int[] calculatePermutation()
        {
            int[] permutation = Enumerable.Range(0, 256).ToArray();

            for (var i = 0; i < permutation.Length; i++)
            {
                var source = _random.Next(permutation.Length);

                var temp = permutation[i];
                permutation[i] = permutation[source];
                permutation[source] = temp;
            }
            return permutation;
        }

        public static void reseed()
        {
            _permutation = calculatePermutation();
        }

        private static Vector2[] calculateGradients()
        {
            Vector2[] gradients = new Vector2[256];
            Vector2 gradient;

            for (var i = 0; i < gradients.Length; i++)
            {
                do
                    gradient = new Vector2(
                        (float)(_random.NextDouble() * 2 - 1),
                        (float)(_random.NextDouble() * 2 - 1));
                while (gradient.LengthSquared >= 1);

                gradient.Normalize();

                gradients[i] = gradient;
            }
            return gradients;
        }

        private static float drop(float t)
        {
            t = Math.Abs(t);
            return 1f - t * t * t * (t * (t * 6 - 15) + 10);
        }

        private static float q(float u, float v)
        {
            return drop(u) * drop(v);
        }

        public static float generateNoise(float x, float y)
        {
            var cell = new Vector2((float)Math.Floor(x), (float)Math.Floor(y));

            var total = 0f;

            var corners = new[] { 
                new Vector2(0, 0), 
                new Vector2(0, 1), 
                new Vector2(1, 0), 
                new Vector2(1, 1) };

            foreach (var corner in corners)
            {
                var ij = cell + corner;
                var uv = new Vector2(x - ij.X, y - ij.Y);

                var index = _permutation[(int)ij.X % _permutation.Length];
                index = _permutation[(index + (int)ij.Y) % _permutation.Length];

                var grad = _gradients[index % _gradients.Length];

                total += q(uv.X, uv.Y) * Vector2.Dot(grad, uv);
            }

            return Math.Max(Math.Min(total, 1f), -1f);
        }

    }
}
