using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Core.Pixelation;
using Stellamod.Core.Shaders;
using Stellamod.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Stellamod.Core.RibbonSystem
{
    public enum RibbonWandType : byte
    {
        Red,
        Blue,
        Green,
        Purple,
        Multicolor
    }
    public class RibbonSerializer : TagSerializer<Ribbon, TagCompound>
    {
        public override Ribbon Deserialize(TagCompound tag)
        {
            Point tile1 = tag.Get<Point>("tile1");
            Point tile2 = tag.Get<Point>("tile2");
            float length = tag.GetFloat("length");
            int style = tag.Get<int>("style");
            return new Ribbon(tile1.ToWorldCoordinates(), tile2.ToWorldCoordinates(), length, (RibbonWandType)style);
        }

        public override TagCompound Serialize(Ribbon value)
        {
            return new TagCompound
            {
                ["tile1"] = value.GetStartTile(),
                ["tile2"] = value.GetEndTile(),
                ["length"] = value.ribbonLength,
                ["style"] = (int)value.style
            };
        }
    }
    /// <summary>
    /// Represents a long trail of flag like things basically
    /// </summary>
    public class Ribbon
    {
        public const float Ribbon_Length = 16;
        public static Color RibbonColor => Color.DarkRed;
        private float _windOffset;
        public VertexPositionColor[] vertices;
        public Vector2[] originalPositions;
        public Vector2[] linePoints;
        public Vector2 startPosition;
        public Vector2 endPosition;
        public float ribbonLength;
        public float ribbonPadding;
        public RibbonWandType style;
        public Ribbon(Vector2 startPosition, Vector2 endPosition, float ribbonLength, RibbonWandType style)
        {
            this.startPosition = startPosition;
            this.endPosition = endPosition;
            this.ribbonLength = ribbonLength;
            this.style = style;
            this.ribbonPadding = 16;
            CalculateVertices();
        }

        //Get the start and ending tiles for the save data of the ribbons
        public Point GetStartTile()
        {
            return startPosition.ToTileCoordinates();
        }

        public Point GetEndTile()
        {
            return endPosition.ToTileCoordinates();
        }

        public void SimulateWind()
        {
            //Simulate the movement on the flags
            float windSpeed = Main.windSpeedCurrent;
            float windMove = windSpeed * 4;
            _windOffset += windSpeed * 0.1f;
            Parallel.For(0, vertices.Length, i =>
            {
                float xWind = ExtraMath.Osc(0f, 1f, speed: 0.8f, offset: originalPositions[i].Y * 0.02f + _windOffset) * windSpeed * 12;
                float yWind = ExtraMath.Osc(0f, 1f, speed: 0.8f, offset: originalPositions[i].Y * 0.02f + _windOffset) * windSpeed * 4;
                Vector3 windOffset = new Vector3(new Vector2(xWind, yWind), 0);
                Vector2 originalPosition = originalPositions[i];
                vertices[i].Position = new Vector3(originalPosition.X, originalPosition.Y, 0) + windOffset;
            });
        }
        public bool IsConnectedToTile(int i, int j)
        {
            Point tile1 = startPosition.ToTileCoordinates();
            Point tile2 = endPosition.ToTileCoordinates();
            return (tile1.X == i && tile1.Y == j) || (tile2.X == i && tile2.Y == j);
        }
        private Color GetColor(int offset)
        {
            switch (style)
            {
                default:
                case RibbonWandType.Red:
                    return Color.DarkRed;
                case RibbonWandType.Blue:
                    return Color.Lerp(Color.Blue, Color.White, 0.25f);
                case RibbonWandType.Green:
                    return Color.ForestGreen;
                case RibbonWandType.Purple:
                    return Color.MediumPurple;
                case RibbonWandType.Multicolor:
                    int i = offset % 4;
                    switch (i)
                    {
                        default:
                        case 0:
                            return Color.DarkRed;
                        case 1:
                            return Color.Lerp(Color.Blue, Color.White, 0.25f);
                        case 2:
                            return Color.ForestGreen;
                        case 3:
                            return Color.MediumPurple;
                    }
            }
        }

        public void CalculateVertices()
        {
            List<Vector2> ribbonOriginalPositions = new List<Vector2>();
            List<VertexPositionColor> ribbonVertices = new List<VertexPositionColor>();
            Vector2 current = startPosition;
            Vector2 normalVelocity = (endPosition - startPosition).SafeNormalize(Vector2.Zero);

            float length = ribbonLength;
            float paddedLength = length + ribbonPadding;

            float distance = Vector2.Distance(startPosition, endPosition);
            float steps = MathF.Ceiling(distance / paddedLength);
            linePoints = new Vector2[(int)steps];

            float maxSlack = Vector2.Distance(startPosition, endPosition) / 64f;
            for (int r = 0; r < linePoints.Length; r++)
            {
                float completionRatio = (float)r / steps;

                Vector2 linePoint = Vector2.Lerp(startPosition, endPosition, completionRatio);

                float slack = EasingFunction.QuadraticBump(completionRatio);
                slack *= maxSlack;



                Vector2 velocity = normalVelocity * length;
                Vector2 perpVector = velocity.RotatedBy(MathHelper.PiOver2);
                if (perpVector.Y < 0)
                    perpVector = velocity.RotatedBy(-MathHelper.PiOver2);
                Vector2 newStart = linePoint + perpVector * slack;
                linePoints[r] = newStart;
            }


            for (int i = 0; i < steps - 1; i++)
            {
                float completionRatio = (float)i / steps;
                Vector2 point1 = linePoints[i];

                Vector2 point2 = linePoints[i + 1];

                //Get the end point of this ribbon, going towards the next point
                Vector2 ribbonStart = point1;
                Vector2 ribbonNormalVelocity = (point2 - point1).SafeNormalize(Vector2.Zero);
                Vector2 ribbonEnd = ribbonStart + ribbonNormalVelocity * ribbonLength;

                Vector2 ribbonMiddle = (ribbonStart + ribbonEnd) / 2f;

                //Clamp the velocity to the length of the ribbon
                //Calculate the perpendicular velocity
                Vector2 vel = (ribbonEnd - ribbonStart);
                vel = vel.SafeNormalize(Vector2.Zero) * ribbonLength;
                Vector2 perpVector = vel.RotatedBy(MathHelper.PiOver2);



                //There's probably a better way to do this but eh
                if (perpVector.Y < 0)
                    perpVector = vel.RotatedBy(-MathHelper.PiOver2);

                Vector2 ribbonBottom = ribbonMiddle + Vector2.UnitY * 8;

                Color color = GetColor(i);
                VertexPositionColor v1 = new VertexPositionColor(new Vector3(ribbonStart, 0), color);
                VertexPositionColor v2 = new VertexPositionColor(new Vector3(ribbonEnd, 0), color);
                VertexPositionColor v3 = new VertexPositionColor(new Vector3(ribbonBottom, 0), color);

                ribbonVertices.Add(v1);
                ribbonOriginalPositions.Add(ribbonStart);

                ribbonVertices.Add(v2);
                ribbonOriginalPositions.Add(ribbonEnd);

                ribbonVertices.Add(v3);
                ribbonOriginalPositions.Add(ribbonBottom);
            }
            vertices = ribbonVertices.ToArray();
            originalPositions = ribbonOriginalPositions.ToArray();
        }
    }

    /// <summary>
    /// Renders strands of ribbonbs
    /// </summary>
    public class RibbonRenderer : ModSystem
    {
        private Point _oldScreenSize;
        private RenderTarget2D _pixelatedRibbonRT;
        private RenderTarget2D _pixelScreenRenderRT;
        private List<Ribbon> _ribbons;
        private VertexPositionColor[] _vertexBufferArr;
        private int _vertexIndex;

        private Vector2[] _linesBufferArr;
        private int _lineIndex;

        public int DownSamples => 2;
        public const int Max_Ribbon_Vertex_Count = 3 * 500;
        public const int Max_Line_Count = 500;
        public override void Load()
        {
            base.Load();
            ResizeRenderTargets();
        }

        public override void ClearWorld()
        {
            base.ClearWorld();
            _ribbons.Clear();

        }
        public override void PostUpdateEverything()
        {
            base.PostUpdateEverything();
            ResizeRenderTargets();
            for (int i = 0; i < _ribbons.Count; i++)
            {
                Ribbon ribbon = _ribbons[i];
                ribbon.SimulateWind();
            }
        }

        public void AddRibbon(Ribbon ribbon)
        {
            _ribbons.Add(ribbon);
        }

        public void RemoveRibbon(Ribbon ribbon)
        {
            _ribbons.Remove(ribbon);
        }

        public override void SaveWorldData(TagCompound tag)
        {
            base.SaveWorldData(tag);
            tag["ribbons"] = _ribbons;
        }

        public override void LoadWorldData(TagCompound tag)
        {
            base.LoadWorldData(tag);
            _ribbons = new List<Ribbon>();
            _ribbons = tag.Get<List<Ribbon>>("ribbons");
        }

        public override void NetSend(BinaryWriter writer)
        {
            base.NetSend(writer);
            writer.Write(_ribbons.Count);
            for (int i = 0; i < _ribbons.Count; i++)
            {
                var str = _ribbons[i];
                writer.WriteVector2(str.startPosition);
                writer.WriteVector2(str.endPosition);
                writer.Write(str.ribbonLength);
                writer.Write((int)str.style);
            }
        }
        public override void NetReceive(BinaryReader reader)
        {
            base.NetReceive(reader);
            int silkStringCount = reader.ReadInt32();
            _ribbons.Clear();
            for (int s = 0; s < silkStringCount; s++)
            {
                Vector2 startPosition = reader.ReadVector2();
                Vector2 endPosition = reader.ReadVector2();
                float length = reader.ReadSingle();
                int style = reader.ReadInt32();

                Ribbon str = new Ribbon(startPosition, endPosition, length, (RibbonWandType)style);
                _ribbons.Add(str);
            }
        }
        public override void OnModLoad()
        {
            base.OnModLoad();
            _ribbons = new List<Ribbon>(100);
            _vertexBufferArr = new VertexPositionColor[Max_Ribbon_Vertex_Count];
            _linesBufferArr = new Vector2[Max_Line_Count];
            On_Main.CheckMonoliths += RenderToPixelationRT;
            On_Main.DoDraw_DrawNPCsBehindTiles += DrawPixelRTToScreen;
        }

        public override void OnModUnload()
        {
            base.OnModUnload();
            On_Main.CheckMonoliths -= RenderToPixelationRT;
            On_Main.DoDraw_DrawNPCsBehindTiles -= DrawPixelRTToScreen;
        }
        public override void PostDrawTiles()
        {
            base.PostDrawTiles();
            Player localPlayer = Main.LocalPlayer;
            if (localPlayer.HeldItem.type == ModContent.ItemType<RibbonWand>() || localPlayer.HeldItem.type == ModContent.ItemType<RibbonScissors>())
            {
                Vector2 mouseWorld = Main.MouseWorld;
                int i = (int)(mouseWorld.X / 16f);
                int j = (int)(mouseWorld.Y / 16f);

                Texture2D wallTexture = ModContent.Request<Texture2D>(TextureRegistry.ZuiEffect).Value;
                Vector2 drawOrigin = wallTexture.Size() / 2f;

                Rectangle frame = new Rectangle(0, 0, 16, 16);
                SpriteBatch spriteBatch = Main.spriteBatch;
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

     
                Vector2 cameraCenterWorld = Main.Camera.Center;
                Vector2 cameraTopLeft = cameraCenterWorld - new Vector2(Main.screenWidth, Main.screenHeight) / 2;
                Vector2 cameraBottomRight = cameraCenterWorld + new Vector2(Main.screenWidth, Main.screenHeight) / 2;
                Rectangle cameraRectangle = new Rectangle((int)cameraTopLeft.X, (int)cameraTopLeft.Y, (int)(cameraBottomRight.X - cameraTopLeft.X), (int)(cameraBottomRight.Y - cameraTopLeft.Y));

                foreach (var ribbon in _ribbons)
                {
                    Vector2 startDrawPos = ribbon.startPosition.ToTileCoordinates().ToWorldCoordinates(0,0);
                    Vector2 endDrawPos = ribbon.endPosition.ToTileCoordinates().ToWorldCoordinates(0,0);
                    Color drawColor = Color.Red;

                    if (cameraRectangle.Contains(startDrawPos.ToPoint()) || cameraRectangle.Contains(endDrawPos.ToPoint()))
                    {
                        spriteBatch.Draw(TextureAssets.Tile[0].Value, startDrawPos - Main.screenPosition, frame, drawColor, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
                        spriteBatch.Draw(TextureAssets.Tile[0].Value, endDrawPos - Main.screenPosition, frame, drawColor, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
                    }

                }
                Vector2 tilePos = new Vector2(i, j) * 16;
                spriteBatch.Draw(TextureAssets.Tile[0].Value, tilePos - Main.screenPosition, frame, Color.Green * 0.5f, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);



                spriteBatch.End();
            }
        }

        public void ReceiveBreakRibbonSync(int i, int j)
        {
            Ribbon connectedString = _ribbons.Find(x => x.IsConnectedToTile(i, j));
            if (connectedString == null)
                return;
            _ribbons.Remove(connectedString);
        }

        public void ReceivePlaceRibbonSync(Vector2 startPosition, Vector2 endPosition, RibbonWandType style)
        {
            Ribbon ribbon = new Ribbon(startPosition, endPosition, 16, style);
            AddRibbon(ribbon);
        }

        public void PlaceRibbon(Vector2 startPosition, Vector2 endPosition, RibbonWandType style)
        {
            ReceivePlaceRibbonSync(startPosition, endPosition, style);
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                int ignoreClient = Main.LocalPlayer.whoAmI;
                Stellamod.WriteToPacket(Stellamod.Instance.GetPacket(), (byte)MessageType.PlaceRibbon,
                    startPosition.X,
                    startPosition.Y,
                    endPosition.X,
                    endPosition.Y,
                    (int)style).Send(ignoreClient);
            }
        }

        public void BreakRibbon(int i, int j)
        {
            ReceiveBreakRibbonSync(i, j);
            if(Main.netMode == NetmodeID.MultiplayerClient)
            {
                int ignoreClient = Main.LocalPlayer.whoAmI;
                Stellamod.WriteToPacket(Stellamod.Instance.GetPacket(), (byte)MessageType.BreakRibbon, i, j).Send(ignoreClient);
            }

        }

        public bool TryBreakRibbon(Vector2 position)
        {
            Point tilePoint = position.ToTileCoordinates();
            Ribbon connectedString = _ribbons.Find(x => x.IsConnectedToTile(tilePoint.X, tilePoint.Y));
            if (connectedString != null)
            {
                BreakRibbon(tilePoint.X, tilePoint.Y);
                return true;
            }
            return false;
        }

        private void DrawPixelRTToScreen(On_Main.orig_DoDraw_DrawNPCsBehindTiles orig, Main self)
        {
            GatherRibbonVertices();
            if (ShouldRender() && !Main.gameMenu)
            {
                SpriteBatch spriteBatch = Main.spriteBatch;
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone);

                float scale = DownSamples;

                //Draw the outline for the ribbonbs
                float outlineOffset = 2;
                Vector2 v = Vector2.UnitY * outlineOffset;
                Vector2 h = Vector2.UnitX * outlineOffset;
                spriteBatch.Draw(_pixelatedRibbonRT, Vector2.Zero + v, null, Color.Black, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
                spriteBatch.Draw(_pixelatedRibbonRT, Vector2.Zero - v, null, Color.Black, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
                spriteBatch.Draw(_pixelatedRibbonRT, Vector2.Zero + h, null, Color.Black, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
                spriteBatch.Draw(_pixelatedRibbonRT, Vector2.Zero - h, null, Color.Black, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);


                spriteBatch.Draw(_pixelatedRibbonRT, Vector2.Zero, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
                spriteBatch.End();
            }

            orig(self);



        }

        private bool ShouldRender()
        {
            return _vertexIndex >= 3;
        }


        private void GatherRibbonVertices()
        {
            _vertexIndex = 0;
            _lineIndex = 0;
            Vector2 cameraCenterWorld = Main.Camera.Center;
            Vector2 cameraTopLeft = cameraCenterWorld - new Vector2(Main.screenWidth, Main.screenHeight) / 2;
            Vector2 cameraBottomRight = cameraCenterWorld + new Vector2(Main.screenWidth, Main.screenHeight) / 2;
            Rectangle cameraRectangle = new Rectangle((int)cameraTopLeft.X, (int)cameraTopLeft.Y, (int)(cameraBottomRight.X - cameraTopLeft.X), (int)(cameraBottomRight.Y - cameraTopLeft.Y));
            for (int i = 0; i < _ribbons.Count; i++)
            {
                Ribbon ribbon = _ribbons[i];
                Vector2 start = ribbon.startPosition;
                Vector2 end = ribbon.endPosition;

                if (cameraRectangle.Contains(start.ToPoint()) || cameraRectangle.Contains(end.ToPoint()))
                {
                    for (int j = 0; j < ribbon.vertices.Length && _vertexIndex < _vertexBufferArr.Length; j++)
                    {
                        _vertexBufferArr[_vertexIndex] = ribbon.vertices[j];
                        _vertexIndex++;
                    }

                    for (int j = 0; j < ribbon.linePoints.Length && _lineIndex < _linesBufferArr.Length; j++)
                    {
                        _linesBufferArr[_lineIndex] = ribbon.linePoints[j];
                        _lineIndex++;
                    }
                }
            }
        }

        private void RenderToPixelationRT(On_Main.orig_CheckMonoliths orig)
        {
            orig();
            if (ShouldRender() && !Main.gameMenu)
            {
                SpriteBatch spriteBatch = Main.spriteBatch;
                GraphicsDevice graphicsDevice = Main.graphics.GraphicsDevice;
                graphicsDevice.SetRenderTarget(_pixelScreenRenderRT);
                graphicsDevice.Clear(Color.Transparent);

                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);


                Texture2D ribbonLineTexture = ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/Line").Value;
                Vector2 drawOrigin = new Vector2(0, ribbonLineTexture.Height / 2);
                for (int i = 0; i < _lineIndex - 1; i++)
                {
                    Vector2 position = _linesBufferArr[i];
                    Vector2 nextPosition = _linesBufferArr[i + 1];
                    float rotation = (nextPosition - position).ToRotation();

                    position -= Main.screenPosition;

                    Vector2 drawScale = new Vector2(0.1f, 1f);

                    spriteBatch.Draw(ribbonLineTexture, position, null, Color.White * 0.5f, rotation, drawOrigin, drawScale, SpriteEffects.None, 0);
                }

                spriteBatch.End();


                //Apply the flag shader :p 
                var flagShader = FlagShader.Instance;
                flagShader.ApplyPasses();

                //We can get all the ribbons in a single draw call
                graphicsDevice.BlendState = BlendState.AlphaBlend;
                graphicsDevice.SamplerStates[0] = SamplerState.PointClamp;
                graphicsDevice.DrawUserPrimitives(
                      PrimitiveType.TriangleList, _vertexBufferArr, 0, _vertexIndex / 3);


                //Now we take that output and downscale it to the pixel RT
                graphicsDevice.SetRenderTarget(_pixelatedRibbonRT);
                graphicsDevice.Clear(Color.Transparent);

                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
                float denom = DownSamples;
                float scale = 1f / denom;


                spriteBatch.Draw(_pixelScreenRenderRT, Vector2.Zero, null, Color.White, 0, Vector2.Zero, scale, SpriteEffects.None, 0);
                spriteBatch.End();
            }
        }

        private void ResizeRenderTargets()
        {
            if (Main.netMode == NetmodeID.Server)
                return;

            if (Main.dedServ)
                return;

            Point screenSize = Main.ScreenSize;
            if (_oldScreenSize != screenSize && Main.netMode != NetmodeID.Server)
            {
                Main.QueueMainThreadAction(() =>
                {
                    _pixelatedRibbonRT.Release();
                    _pixelatedRibbonRT = new RenderTarget2D(Main.graphics.GraphicsDevice, screenSize.X / DownSamples, screenSize.Y / DownSamples);


                    _pixelScreenRenderRT.Release();
                    _pixelScreenRenderRT = new RenderTarget2D(Main.graphics.GraphicsDevice, screenSize.X, screenSize.Y);


                });
                _oldScreenSize = screenSize;
            }
        }
    }
}
