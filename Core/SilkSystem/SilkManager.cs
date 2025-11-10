using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Content.Areas.WondrousDarkspace.TilesWD;
using Stellamod.Core.Particles;
using Stellamod.Core.Shaders;
using Stellamod.Core.Shaders.MagicTrails;
using Stellamod.Helpers;
using Stellamod.Items.Materials;
using Stellamod.Systems.MiscellaneousMath;
using Stellamod.Trails;
using Stellamod.Visual.Particles;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.DataStructures;
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
        public void Update()
        {
            if (Main.rand.NextBool(100))
            {
                Vector2 spawnPoint = GetRandomPoint();
                Particle.NewParticle<SilkParticle>(spawnPoint, Vector2.Zero, Color.Transparent);
            }
        }

        public Vector2 GetRandomPoint()
        {
            Vector2[] positions = GetWorldPoints();
            Vector2 spawnPoint = positions[Main.rand.Next(0, positions.Length)];
            return spawnPoint;
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
            if (worldPoints == null)
            {
                InitTrailCache();

            }
            return worldPoints;
        }
        public float[] GetWorldRot()
        {
            if (worldRot == null)
            {
                InitTrailCache();
            }
            return worldRot;
        }
        public bool IsConnectedToTile(int i, int j)
        {
            return (tile1.X == i && tile1.Y == j) || (tile2.X == i && tile2.Y == j);
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
        public void NetSend(BinaryWriter writer)
        {

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
            if (type == ModContent.TileType<SilkTile>())
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
        public const int Max_Silk_Count = 100;
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

        public override void ClearWorld()
        {
            base.ClearWorld();
            _silkStrings.Clear();
        }

        public override void NetSend(BinaryWriter writer)
        {
            base.NetSend(writer);
            writer.Write(_silkStrings.Count);
            for (int i = 0; i < _silkStrings.Count; i++)
            {
                var str = _silkStrings[i];
                writer.Write(str.tile1.X);
                writer.Write(str.tile1.Y);
                writer.Write(str.tile2.X);
                writer.Write(str.tile2.Y);
                writer.Write(str.width);
            }
        }
        public override void NetReceive(BinaryReader reader)
        {
            base.NetReceive(reader);
            int silkStringCount = reader.ReadInt32();
            _silkStrings.Clear();
            for (int s = 0; s < silkStringCount; s++)
            {
                int x1 = reader.ReadInt32();
                int y1 = reader.ReadInt32();
                int x2 = reader.ReadInt32();
                int y2 = reader.ReadInt32();
                float width = reader.ReadSingle();
                SilkString str = new SilkString(new Point(x1, y1), new Point(x2, y2), width);
                _silkStrings.Add(str);
            }
        }
        public override void PostUpdateDusts()
        {
            base.PostUpdateDusts();
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
                    silkString.Update();
                }
            }

        
        }
        public static void DestroySilk(int i, int j)
        {


            if (MultiplayerHelper.IsHost || Main.netMode == NetmodeID.SinglePlayer)
            {

                SilkString connectedString = _silkStrings.Find(x => x.IsConnectedToTile(i, j));
                if (connectedString == null)
                    return;
                _silkStrings.Remove(connectedString);
                int numThreads = Main.rand.Next(3, 8);
                for (int n = 0; n < numThreads; n++)
                {

                    Vector2 point = connectedString.GetRandomPoint();
                    int itemIndex = Item.NewItem(new EntitySource_TileBreak(i, j), point,
                              ModContent.ItemType<MiracleThread>(), 1);
                    NetMessage.SendData(MessageID.SyncItem, -1, -1, null, itemIndex, 1f);

                    for (int s = 0; s < 15; s++)
                    {
                        Vector2 spawnPoint = point + Main.rand.NextVector2Circular(32, 32);
                        Particle.NewParticle<SilkParticle>(spawnPoint, Vector2.Zero, Color.Transparent);
                    }
                }
                NetMessage.SendData(MessageID.WorldData);
            }
            else if (!MultiplayerHelper.IsHost)
            {
                Stellamod.WriteToPacket(Stellamod.Instance.GetPacket(), (byte)MessageType.BreakString, i, j).Send(-1);
            }

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
            SpriteBatch spriteBatch = Main.spriteBatch;
            spriteBatch.Begin(SpriteSortMode.Deferred,
                BlendState.Additive,
                SamplerState.PointWrap,
                DepthStencilState.None,
                RasterizerState.CullCounterClockwise);
            for (int i = 0; i < _silkStrings.Count; i++)
            {
                SilkString silkString = _silkStrings[i];
                if (silkString.ShouldRender())
                {
                    DrawSilkString(silkString);
                }
            }
            spriteBatch.End();
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
            if (_silkStrings.Count >= Max_Silk_Count)
                return;

            int neededAir = j - 3;
            Tile groundTile = Main.tile[i, j];
            Tile airTile = Main.tile[i, neededAir];
            if (!airTile.HasTile && groundTile.HasTile)
            {
                int i2 = i;
                int j2 = neededAir;
                Tile currentTile = Main.tile[i2, j2];
                while ((!currentTile.HasTile && j2 > 0) || (currentTile.TileType == TileID.MinecartTrack))
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

            Asset<Texture2D> silkEnd = TrailRegistry.SilkEnd;
            Vector2 startPoint = silkString.tile1.ToWorldCoordinates();
            Vector2 endPoint = silkString.tile2.ToWorldCoordinates();

            float drawRotation = (endPoint - startPoint).ToRotation();
            Vector2 drawPoint = startPoint - Main.screenPosition;
            Color drawColor = Color.White.MultiplyRGB(lightColor) * 0.75f;
            Vector2 origin = silkEnd.Size() / 2f;
            Vector2 drawScale = Vector2.One;


            spriteBatch.Draw(silkEnd.Value, drawPoint, null, drawColor, drawRotation, origin, drawScale, SpriteEffects.None, 0);

            Vector2 drawPoint2 = endPoint - Main.screenPosition;
            drawPoint2 += (startPoint - endPoint).SafeNormalize(Vector2.Zero) * 32;
            float drawRotation2 = (startPoint - endPoint).ToRotation();
            spriteBatch.Draw(silkEnd.Value, drawPoint2, null, drawColor, drawRotation2, origin, drawScale, SpriteEffects.None, 0);
        }
    }
}
