using Stellamod.Common.ArmorReforge;
using Stellamod.Core.Utilities;
using System.Reflection;
using Terraria;
using Terraria.Graphics.Light;
using Terraria.ID;

namespace Stellamod.Core.LunarLightingSystem;

public struct Light
{
    public Color color;
    public Vector2 position;
    public float diameter;
}

public enum ShadowQuality
{
    Ultra_Low,
    Low,
    Medium,
    High,
    Very_High
}

public class PointLights
{
    public PointLights(int maxLights)
    {
        Lights = new Light[maxLights];
    }
    public readonly Light[] Lights;
    public int UsedLightCount { get; private set; }
    public void Clear()
    {
        UsedLightCount = 0;
    }
    public float GetPlayerLightRadius(Player player)
    {
        Item heldItem = player.HeldItem;
        float lightRadius;
        if (LightingSets.EmissiveHeldItems[heldItem.type].A > 0)
        {
            lightRadius = 400;
        }
        else
        {
            lightRadius =  50;
        }

        ShiningPlayer shiningPlayer = player.GetModPlayer<ShiningPlayer>();
        lightRadius *= 1.0f + shiningPlayer.extraLight;
        return lightRadius;
    }
    public Vector3 GetPlayerLightColor(Player player)
    {
        if (player.dead)
            return Vector3.Zero;

        Item heldItem = player.HeldItem;
        if (LightingSets.EmissiveHeldItems[heldItem.type].A > 0)
        {

            int c = TorchLightingHelper.TorchItemToTorchID(heldItem.type);
            if (c != -1)
            {
                TorchID.TorchColor(c, out float r, out float g, out float b);
                Color myColor = new Color(r, g, b);
                return myColor.ToVector3();
            }


            Vector3 color = LightingSets.EmissiveHeldItems[heldItem.type].ToVector3();
            return color;

        }
        else
        {
            return Vector3.Zero;
        }
    }
    public void GatherLights()
    {
        (Point topLeft, Point bottomRight) = TileUtilities.CameraTileBounds(384);
        LightingEngine lightingEngine = typeof(Lighting).GetField("_activeEngine", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null) as LightingEngine;
        TileLightScanner tileScanner = typeof(LightingEngine).GetField("_tileScanner", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(lightingEngine) as TileLightScanner;
        LightMap lightMap = typeof(LightingEngine).GetField("_activeLightMap", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(lightingEngine) as LightMap;
        foreach (Player player in Main.ActivePlayers)
        {
            if (UsedLightCount >= Lights.Length)
                return;
            Vector3 playerLightColor = GetPlayerLightColor(player);
            int r = (int)(playerLightColor.X * 255);
            int g = (int)(playerLightColor.Y * 255);
            int b = (int)(playerLightColor.Z * 255);

            int lightIndex = UsedLightCount;
            ref Light light = ref Lights[lightIndex];
            light.color = new Color(r, g, b, lightIndex);
            light.position = player.Center;
            light.diameter = GetPlayerLightRadius(player);
            UsedLightCount++;
        }
        for (int x = topLeft.X; x < bottomRight.X; x++)
        {
            for (int y = topLeft.Y; y < bottomRight.Y; y++)
            {
                //Return out of all loops if we run out of lights
                if (UsedLightCount >= Lights.Length)
                    return;
                Point lightTilePoint = new Point(x, y);
                Tile tile = Main.tile[lightTilePoint];
                if (!tile.HasTile)
                    continue;
                if (Main.tileSolid[tile.TileType])
                    continue;
                if (Main.tileSolidTop[tile.TileType])
                    continue;
                if (!Main.tileLighted[tile.TileType])
                    continue;

                Vector3 lightColor;
                tileScanner.GetTileLight(x, y, out lightColor);


                //Only bright lights should cast shadows
                float brightness = lightColor.X + lightColor.Y + lightColor.Z;
                brightness /= 3f;
                if (brightness <= 0.35f)
                    continue;



                Vector2 position = lightTilePoint.ToWorldCoordinates();

                int r = (int)(lightColor.X * 255);
                int g = (int)(lightColor.Y * 255);
                int b = (int)(lightColor.Z * 255);
                int lightIndex = UsedLightCount;
      

                //Add the light to our gathered lights
                ref Light light = ref Lights[lightIndex];
                light.color = new Color(r, g, b, lightIndex);
                light.position = position;
                light.diameter = LunarLightingRenderer.POINT_LIGHT_DIAMETER;
                UsedLightCount++;
                //    spriteBatch.Draw(heightTile, position - Main.screenPosition, drawColor);
            }
        }




    }
    public Light this[int i]
    {
        get => Lights[i];
    }
}
