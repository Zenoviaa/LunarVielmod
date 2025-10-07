using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Core.Shaders;
using Stellamod.Core.Shaders.MagicTrails;
using Stellamod.Helpers;
using Stellamod.Systems.MiscellaneousMath;
using Stellamod.Trails;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.Utilities;

namespace Stellamod.Core.SilkSystem
{
    public class SilkString
    {
        public SilkString(Point tile1, Point tile2, float width = 1f)
        {
            this.tile1 = tile1;
            this.tile2 = tile2;
            this.width = width;
        }
        public Point tile1;
        public Point tile2;
        public float width;
        public Vector2[] worldPoints;
        public float[] worldRot;
        public float GetWidth(float completionRatio)
        {

            float startWidth = width * 64;
            float midWidth = width * 16;
            float ease = EasingFunction.QuadraticBump(completionRatio);
            return MathHelper.Lerp(startWidth, midWidth, ease);
        }

        private void InitTrailCache()
        {
            Vector2 drawPos1 = tile1.ToWorldCoordinates();
            Vector2 drawPos2 = tile2.ToWorldCoordinates();
            Vector2[] trailPoints = new Vector2[2];
            trailPoints[0] = drawPos1;
            trailPoints[1] = drawPos2;
            MathUtil.LerpTrailPoints(trailPoints, out worldPoints, smoothFactor: 32);

            worldRot = new float[worldPoints.Length];
        }
        public Vector2[] GetWorldPoints()
        {
            if(worldPoints == null)
            {
                InitTrailCache();

            }
            return worldPoints;
        }
        public float[] GetWorldRot()
        {
            if(worldRot == null)
            {
                InitTrailCache();
            }
            return worldRot;
        }
        public Color GetColor(float completionRatio)
        {
            return Color.White;
        }

        public bool ShouldRender()
        {
            Player player = Main.LocalPlayer;
            Vector2 worldPos1 = tile1.ToWorldCoordinates();
            Vector2 worldPos2 = tile2.ToWorldCoordinates();

            float distance1 = Vector2.Distance(player.Center, worldPos1);
            float distance2 = Vector2.Distance(player.Center, worldPos2);
            float minDistance = MathF.Min(distance1, distance2);
            if (minDistance <= 1000)
                return true;
            return false;
        }
    }

    public class SilkSerializer : TagSerializer<SilkString, TagCompound>
    {
        public override SilkString Deserialize(TagCompound tag)
        {
            Point tile1 = tag.Get<Point>("tile1");
            Point tile2 = tag.Get<Point>("tile2");
            float width = tag.GetFloat("width");
            return new SilkString(tile1, tile2, width);
        }

        public override TagCompound Serialize(SilkString value)
        {
            return new TagCompound
            {
                ["tile1"] = value.tile1,
                ["tile2"] = value.tile2,
                ["width"] = value.width
            };
        }
    }
    public class SilkGlobalTile : GlobalTile
    {
        public override void RandomUpdate(int i, int j, int type)
        {
            base.RandomUpdate(i, j, type);
            if (type == TileID.GraniteBlock)
            {
                if (Main.rand.NextBool(16))
                {
                    SilkManager.GrowSilk(i, j, Main.rand);
                }

            }
        }
    }
    public class SilkManager : ModSystem
    {
        private static List<SilkString> _silkStrings = new List<SilkString>();
        public override void OnModLoad()
        {
            base.OnModLoad();
            _silkStrings = new List<SilkString>();
            On_Main.DrawDust += DrawStrings;
        }


        public override void OnModUnload()
        {
            base.OnModUnload();
            On_Main.DrawDust -= DrawStrings;
        }

        private void DrawStrings(On_Main.orig_DrawDust orig, Main self)
        {
            orig(self);
            Player localPlayer = Main.LocalPlayer;
            MyPlayer myPlayer = localPlayer.GetModPlayer<MyPlayer>();
            if (!myPlayer.ZoneWonder)
            {
                return;
            }

            for (int i = 0; i < _silkStrings.Count; i++)
            {
                SilkString silkString = _silkStrings[i];
                if (silkString.ShouldRender())
                {
                    DrawSilkString(silkString);
                }
            }
        }

        public override void SaveWorldData(TagCompound tag)
        {
            base.SaveWorldData(tag);
            tag["silkStrings"] = _silkStrings;
        }

        public override void LoadWorldData(TagCompound tag)
        {
            base.LoadWorldData(tag);
            _silkStrings = new List<SilkString>();
            _silkStrings = tag.Get<List<SilkString>>("silkStrings");
        }

        public static void GrowSilk(int i, int j, UnifiedRandom rand)
        {
            int neededAir = j - 3;
            Tile groundTile = Main.tile[i, j];
            Tile airTile = Main.tile[i, neededAir];
            if (!airTile.HasTile && groundTile.HasTile)
            {
                int i2 = i;
                int j2 = neededAir;
                Tile currentTile = Main.tile[i2, j2];
                while (!currentTile.HasTile && j2 > 0)
                {
                    i2 += rand.Next(-1, 2);
                    j2--;
                    currentTile = Main.tile[i2, j2];
                }

            
                Point bottomTile = new Point(i, j);
                Point topTile = new Point(i2, j2);
                float width = rand.NextFloat(0.5f, 1f);
                SilkString silkString = new SilkString(bottomTile, topTile, width);
                _silkStrings.Add(silkString);
            }
        }

        private void DrawSilkString(SilkString silkString)
        {

            //  drawPos1 -= Main.screenPosition;
            // drawPos2 -= Main.screenPosition;


            SpriteBatch spriteBatch = Main.spriteBatch;

            SimpleTrailShader trailShader = SimpleTrailShader.Instance;
            trailShader.TrailingTexture = TrailRegistry.SilkTrail;
            trailShader.SecondaryTrailingTexture = TrailRegistry.StarTrail;
            trailShader.TertiaryTrailingTexture = TrailRegistry.SilkTrail;
            trailShader.BlendState = BlendState.AlphaBlend;

            Color lightColor = Lighting.GetColor(silkString.tile1.X, silkString.tile1.Y);

            Color rgbColor = Color.Lerp(Color.White, Color.Pink, MathUtil.Osc(0f, 1f, speed: 1));
            rgbColor = rgbColor.MultiplyRGB(lightColor);
            trailShader.PrimaryColor = rgbColor;
            trailShader.SecondaryColor = rgbColor * 0.5f;
            TrailDrawer.Draw(spriteBatch, silkString.GetWorldPoints(), silkString.GetWorldRot(), silkString.GetColor, silkString.GetWidth, trailShader);
        }
    }
}
