using ReLogic.Threading;
using System;

namespace Stellamod.Helpers
{
    public static class CommonDrawing
    {
        public static Vector2[] InterpolateBetweenPoints(Vector2 start, Vector2 end, float numPoints = 64)
        {
            Vector2[] arr = new Vector2[(int)numPoints];
            for(float f = 0; f < numPoints; f++)
            {
                arr[(int)f] = Vector2.Lerp(start, end, f / numPoints);
            }
            return arr;
        }

        /// <summary>
        /// Performs a spline interpolation across an array of points, good for smoothing out trails
        /// </summary>
        /// <param name="oldPos"></param>
        /// <param name="numPoints"></param>
        /// <returns></returns>
        public static Vector2[] CatmullRomSplineInterpolation(Vector2[] oldPos, float numPoints = 128)
        {
            Vector2[] trailPoints = new Vector2[(int)numPoints];
            float trailLength = oldPos.Length;
            void InterpInner(int start, int end)
            {
                for (int i = start; i < end; i++)
                {
                    //Calculate the index in the oldpos array
                    float ratio = i / numPoints;
                    float weight = ratio * (trailLength) % 1f;
                    int oldPosIndex = (int)(ratio * (trailLength));

                    int leftmostIndex = oldPosIndex - 2;
                    int leftIndex = oldPosIndex - 1;
                    int rightIndex = oldPosIndex;
                    int rightmostIndex = oldPosIndex + 1;


                    leftIndex = Math.Clamp(leftIndex, 0, oldPos.Length - 1);
                    rightIndex = Math.Clamp(rightIndex, 0, oldPos.Length - 1);

                    Vector2 left = oldPos[leftIndex];
                    Vector2 right = oldPos[rightIndex];

                    Vector2 leftMost;
                    Vector2 rightMost;
                    if (rightmostIndex >= oldPos.Length)
                    {
                        //If we're outside the array try to predict the next position by using the left and right indices
                        Vector2 velocity = right - left;
                        rightMost = right + velocity;
                    }
                    else
                    {
                        rightMost = oldPos[rightmostIndex];
                    }

                    if (leftmostIndex < 0)
                    {
                        Vector2 velocity = left - right;
                        leftMost = left + velocity;
                    }
                    else
                    {
                        leftMost = oldPos[leftmostIndex];
                    }

                    Vector2 e = Vector2.CatmullRom(leftMost, left, right, rightMost, weight);
                    trailPoints[i] = e;
                }
            }
            InterpInner(0, trailPoints.Length);




            /*
            //Using parallel is probably actually slower here, readying several cores for a tiny array probably isn't actually a performance boost
            FastParallel.For(0, trailPoints.Length, delegate (int start, int end, object context) 
            {

            });*/

            return trailPoints;
        }

        /// <summary>
        /// Performs a spline interpolation across an array of points, good for smoothing out trails, takes an input trail points array instead of allocating a new one.
        /// </summary>
        /// <param name="oldPos"></param>
        /// <param name="numPoints"></param>
        /// <returns></returns>
        public static void CatmullRomSplineInterpolationNonAlloc(Vector2[] oldPos, Vector2[] trailPoints)
        {
            float trailLength = oldPos.Length;
            float numPoints = trailPoints.Length;
            FastParallel.For(0, trailPoints.Length, delegate (int start, int end, object context)
            {
                for (int i = start; i < end; i++)
                {
                    //Calculate the index in the oldpos array
                    float ratio = i / numPoints;
                    float weight = ratio * (trailLength) % 1f;
                    int oldPosIndex = (int)(ratio * (trailLength));

                    int leftmostIndex = oldPosIndex - 2;
                    int leftIndex = oldPosIndex - 1;
                    int rightIndex = oldPosIndex;
                    int rightmostIndex = oldPosIndex + 1;


                    leftIndex = Math.Clamp(leftIndex, 0, oldPos.Length - 1);
                    rightIndex = Math.Clamp(rightIndex, 0, oldPos.Length - 1);

                    Vector2 left = oldPos[leftIndex];
                    Vector2 right = oldPos[rightIndex];

                    Vector2 leftMost;
                    Vector2 rightMost;
                    if (rightmostIndex >= oldPos.Length)
                    {
                        //If we're outside the array try to predict the next position by using the left and right indices
                        Vector2 velocity = right - left;
                        rightMost = right + velocity;
                    }
                    else
                    {
                        rightMost = oldPos[rightmostIndex];
                    }

                    if (leftmostIndex < 0)
                    {
                        Vector2 velocity = left - right;
                        leftMost = left + velocity;
                    }
                    else
                    {
                        leftMost = oldPos[leftmostIndex];
                    }

                    Vector2 e = Vector2.CatmullRom(leftMost, left, right, rightMost, weight);
                    trailPoints[i] = e;
                }
            });
        }
    }
}
