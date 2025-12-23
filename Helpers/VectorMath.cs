using Microsoft.Xna.Framework;

namespace Stellamod.Helpers
{

    public static class VectorMath
    {

        public static void FillArr(Vector2[] arr, Vector2 start, Vector2 end)
        {
            for (int i = 0; i < arr.Length; i++)
            {
                float completionRatio = (float)i / (float)arr.Length;
                Vector2 position = Vector2.Lerp(start, end, completionRatio);
                arr[i] = position;
            }
        }
    }
}
