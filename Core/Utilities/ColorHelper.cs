using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stellamod.Core.Utilities
{
    public static class ColorHelper
    {
        public static void BlurPass(Vector3[] input, Vector3[] output, int width, int height, int samples)
        {
          
            int GetIndex(int x, int y)
            {
                return y * height + x;
            }

            bool IsInRange(int index)
            {
                return index >= 0 && index < input.Length;
            }

            Vector3 GetColorSafely(int x, int y)
            {
                int index = GetIndex(x, y);
                if (!IsInRange(index))
                    return Vector3.Zero;
                return input[index];
            }
            
            for(int x = 0; x < width; x++)
            {
                for(int y = 0; y < height; y++)
                {
                    Vector3 avgColor = GetColorSafely(x - 1, y) + GetColorSafely(x + 1, y) + GetColorSafely(x, y - 1) + GetColorSafely(x, y + 1);
                    avgColor /= 4f;
                    int index = GetIndex(x, y);
                    output[index] = avgColor;
                }
            }
        }
    }
}
