using System;
using Terraria;

namespace Stellamod.WorldG;

public partial class VeilGen
{
    public static void SetCircle(Action<int, int> setFunction, Point centerTile, float radius)
    {
        var left = centerTile.X - radius;
        var right = centerTile.X + radius;
        var top = centerTile.Y - radius;
        var bottom = centerTile.Y + radius;
        var sqrRadius = radius * radius;
        for(var x = left; x < right; x++)
        {
            for(var y = top; y < bottom; y++)
            {
                var sqrDistance = Vector2.DistanceSquared(new Vector2(x, y), new Vector2(centerTile.X, centerTile.Y));
                if (sqrDistance > sqrRadius)
                    continue;
                setFunction((int)x, (int)y);
            }
        }
    }
    
    /// <summary>
    /// Takes a starting point and moves and creates circles to try and make a tentacle
    /// </summary>
    /// <param name="fnl"></param>
    /// <param name="setFunction"></param>
    /// <param name="startTile"></param>
    /// <param name="initialVelocity"></param>
    /// <param name="steps"></param>
    /// <param name="startRadius"></param>
    /// <param name="endRadius"></param>
    public static void SetTentacle(FastNoiseLite fnl,
        Action<int, int> setFunction,
        Point startTile, 
        Vector2 initialVelocity, 
        int steps, 
        float startRadius, 
        float endRadius)
    {
        Vector2 currentPosition = startTile.ToWorldCoordinates();
        Vector2 currentVelocity = initialVelocity;
        for(var k = 0; k < steps; k++)
        {
            var ratio = (float)k / (float)steps;
            var radius = MathHelper.SmoothStep(startRadius, endRadius, ratio);
            SetCircle(setFunction, currentPosition.ToTileCoordinates(), radius);
            var noise = fnl.GetNoise(startTile.X + k, 0);
            currentVelocity = currentVelocity.RotatedBy(noise * 0.04F);
            currentPosition += currentVelocity * radius;
        }
    }

    public static Action<int, int, Color> RedSetTileManipulator(Action<int, int> setFunction)
    {
        void ManipulatorFunc(int x, int y, Color c)
        {
            if(c.R > 125)
                setFunction(x, y);
        }
        return ManipulatorFunc;
    }

    public static void SprayPaint(int x, int y, Action<int, int, Color> manipulator)
    {
        string[] prefabs = new string[]
        {
            "SprayPaint_1",
            "SprayPaint_2",
            "SprayPaint_3",
            "SprayPaint_4"
        };
        GenerationTextureManager generationTextureManager = GenerationTextureManager.Instance;
        var prefabPath = prefabs[MoonUtils.Rand.Next(prefabs.Length)];
        var prefab = generationTextureManager.GetPrefab(prefabPath);
        prefab.PasteErase(new Point(x, y), PrefabPlacementType.FromCenter, manipulator);
    }
}
