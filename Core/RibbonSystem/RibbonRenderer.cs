using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Core.Pixelation;
using Stellamod.Core.Shaders;
using Stellamod.Helpers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Stellamod.Core.RibbonSystem
{
    public class RibbonWand : ModItem
    {
        public Vector2? startPosition;
        public Vector2? endPosition;
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.width = 16;
            Item.height = 16;
            Item.rare = ItemRarityID.Green;
            Item.useTime = 2;
            Item.useAnimation = 2;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.autoReuse = false;
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }
        public override bool? UseItem(Player player)
        {
            int mouseX = (int)(Main.MouseWorld.X);
            int mouseY = (int)(Main.MouseWorld.Y);
            if (startPosition == null)
            {

                startPosition = new Vector2(mouseX, mouseY);
                Main.NewText(startPosition);
            }
            else if (endPosition == null)
            {
                endPosition = new Vector2(mouseX, mouseY);
                Main.NewText(endPosition);
            }

            if (startPosition != null && endPosition != null)
            {
                Vector2 start = startPosition.Value;
                Vector2 end = endPosition.Value;
                Vector2 temp = start;



                RibbonRenderer ribbonRenderer = ModContent.GetInstance<RibbonRenderer>();
                Ribbon ribbon = new Ribbon(start, end, 16, Color.DarkRed);
                ribbonRenderer.AddRibbon(ribbon);
                startPosition = null;
                endPosition = null;
            }

            return true;
        }
    }

    /// <summary>
    /// Represents a long trail of flag like things basically
    /// </summary>
    public class Ribbon
    {
        private float _windOffset;
        public VertexPositionColor[] vertices;
        public Vector2[] originalPositions;
        public Color ribbonColor;
        public Vector2 startPosition;
        public Vector2 endPosition;
        public float ribbonLength;
   

        public Ribbon(Vector2 startPosition, Vector2 endPosition, float ribbonLength, Color ribbonColor)
        {
            this.startPosition = startPosition;
            this.endPosition = endPosition;
            this.ribbonLength = ribbonLength;
            this.ribbonColor = ribbonColor;
            CalculateVertices();
        }

        public void SimulateWind()
        {
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

        public void CalculateVertices()
        {
            List<Vector2> ribbonOriginalPositions = new List<Vector2>();
            List<VertexPositionColor> ribbonVertices = new List<VertexPositionColor>();
            Vector2 current = startPosition;
            Vector2 normalVelocity = (endPosition - startPosition).SafeNormalize(Vector2.Zero);
            Vector2 velocity = normalVelocity * ribbonLength;
            float distance = Vector2.Distance(startPosition, endPosition);
            float steps = MathF.Ceiling(distance / ribbonLength);
            Vector2[] ribbonPositions = new Vector2[(int)steps];

            float maxSlack = Vector2.Distance(startPosition, endPosition) / 64f;
            for(int r = 0; r < ribbonPositions.Length; r++)
            {
                float completionRatio = (float)r / steps;
                current += velocity;

                float slack = EasingFunction.QuadraticBump(completionRatio);
                slack *= maxSlack;

                Vector2 next = current + velocity;
                Vector2 midPosition = (current + next) / 2f;
                Vector2 perpVector = velocity.RotatedBy(MathHelper.PiOver2);
                if (perpVector.Y < 0)
                    perpVector = velocity.RotatedBy(-MathHelper.PiOver2);
                Vector2 newStart = current + perpVector * slack;
                ribbonPositions[r] = newStart;
            }
            for (int i = 0; i < steps - 1; i++)
            {
                float completionRatio = (float)i / steps;
                Vector2 point1 = ribbonPositions[i];
                Vector2 point2 = ribbonPositions[i + 1];
                Vector2 mid = (point1 + point2) / 2f;
                Vector2 vel = (point2 - point1);
                Vector2 perpVector = vel.RotatedBy(MathHelper.PiOver2);
                if (perpVector.Y < 0)
                    perpVector = velocity.RotatedBy(-MathHelper.PiOver2);
                Vector2 point3 = mid + perpVector / 2f;

                VertexPositionColor v1 = new VertexPositionColor(new Vector3(point1, 0), ribbonColor);
                VertexPositionColor v2 = new VertexPositionColor(new Vector3(point2, 0), ribbonColor);
                VertexPositionColor v3 = new VertexPositionColor(new Vector3(point3, 0), ribbonColor);

                ribbonVertices.Add(v1);
                ribbonOriginalPositions.Add(point1);

                ribbonVertices.Add(v2);
                ribbonOriginalPositions.Add(point2);

                ribbonVertices.Add(v3);
                ribbonOriginalPositions.Add(point3);
            }
            vertices = ribbonVertices.ToArray();
            originalPositions = ribbonOriginalPositions.ToArray();
        }
    }

    /// <summary>
    /// Renders strands of ribbonbs
    /// </summary>
    [Autoload(Side = ModSide.Client)]
    public class RibbonRenderer : ModSystem
    {
        private Point _oldScreenSize;
        private RenderTarget2D _pixelatedRibbonRT;
        private RenderTarget2D _pixelScreenRenderRT;
        private List<Ribbon> _ribbons;
        private List<VertexPositionColor> _vertexBufferArr;
        public int DownSamples => 2;
        public override void OnModLoad()
        {
            base.OnModLoad();
            _ribbons = new List<Ribbon>(100);
            _vertexBufferArr = new List<VertexPositionColor>();
            On_Main.CheckMonoliths += RenderToPixelationRT;
            On_Main.DoDraw_DrawNPCsOverTiles += DrawPixelRTToScreen;
        }

        public override void Load()
        {
            base.Load();
            ResizeRenderTargets();
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
        }
        public override void LoadWorldData(TagCompound tag)
        {
            base.LoadWorldData(tag);
        }

        public override void OnModUnload()
        {
            base.OnModUnload();
            On_Main.CheckMonoliths -= RenderToPixelationRT;
            On_Main.DoDraw_DrawNPCsOverTiles -= DrawPixelRTToScreen;
        }

        private bool ShouldRender()
        {
            return _ribbons.Count >= 1;
        }

        private void RenderToPixelationRT(On_Main.orig_CheckMonoliths orig)
        {
            orig();
            if (Main.gameMenu)
                return;
            if (!ShouldRender())
                return;
     
            SpriteBatch spriteBatch = Main.spriteBatch;
            GraphicsDevice graphicsDevice = Main.graphics.GraphicsDevice;
            graphicsDevice.SetRenderTarget(_pixelScreenRenderRT);
            graphicsDevice.Clear(Color.Transparent);

            //Apply the flag shader :p 
            var flagShader = FlagShader.Instance;
            flagShader.ApplyPasses();

            _vertexBufferArr.Clear();
            for (int i = 0; i < _ribbons.Count; i++)
            {
                Ribbon ribbon = _ribbons[i];
                _vertexBufferArr.AddRange(ribbon.vertices);
            }

            graphicsDevice.BlendState = BlendState.AlphaBlend;
            graphicsDevice.SamplerStates[0] = SamplerState.PointClamp;
            graphicsDevice.DrawUserPrimitives(
                  PrimitiveType.TriangleList, _vertexBufferArr.ToArray(), 0, _vertexBufferArr.Count / 3);

            //Now we take that output and downscale it to the pixel RT
            graphicsDevice.SetRenderTarget(_pixelatedRibbonRT);
            graphicsDevice.Clear(Color.Transparent);


            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
            float denom = DownSamples;
            float scale = 1f / denom;


            spriteBatch.Draw(_pixelScreenRenderRT, Vector2.Zero, null, Color.White, 0, Vector2.Zero, scale, SpriteEffects.None, 0);
            spriteBatch.End();
        }


        private void DrawPixelRTToScreen(On_Main.orig_DoDraw_DrawNPCsOverTiles orig, Main self)
        {
            orig(self);
            if (Main.gameMenu)
                return;
            if (!ShouldRender())
                return;
         
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

        private void ResizeRenderTargets()
        {
            Point screenSize = Main.ScreenSize;
            if (_oldScreenSize != screenSize)
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
