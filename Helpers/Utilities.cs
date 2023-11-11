
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Trails;
using System;
using System.Collections.Generic;
using System.Reflection;
using Terraria;
using Terraria.Chat;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.Localization;


namespace Stellamod.Helpers.Separate
{
    public static partial class Utilities
    {
        private static readonly FieldInfo shaderTextureField = typeof(MiscShaderData).GetField("_uImage1", BindingFlags.NonPublic | BindingFlags.Instance);

        private static readonly FieldInfo shaderTextureField2 = typeof(MiscShaderData).GetField("_uImage2", BindingFlags.NonPublic | BindingFlags.Instance);

        private static readonly FieldInfo shaderTextureField3 = typeof(MiscShaderData).GetField("_uImage3", BindingFlags.NonPublic | BindingFlags.Instance);


        /// <summary>
        /// Uses reflection to set the _uImage1. Its underlying data is private and the only way to change it publicly is via a method that only accepts paths to vanilla textures.
        /// </summary>
        /// <param name="shader">The shader</param>
        /// <param name="texture">The texture to use</param>
        public static void SetShaderTexture(this MiscShaderData shader, Asset<Texture2D> texture) => shaderTextureField.SetValue(shader, texture);

        /// <summary>
        /// Uses reflection to set the _uImage2. Its underlying data is private and the only way to change it publicly is via a method that only accepts paths to vanilla textures.
        /// </summary>
        /// <param name="shader">The shader</param>
        /// <param name="texture">The texture to use</param>
        public static void SetShaderTexture2(this MiscShaderData shader, Asset<Texture2D> texture) => shaderTextureField2.SetValue(shader, texture);

        /// <summary>
        /// Uses reflection to set the _uImage3. Its underlying data is private and the only way to change it publicly is via a method that only accepts paths to vanilla textures.
        /// </summary>
        /// <param name="shader">The shader</param>
        /// <param name="texture">The texture to use</param>
        public static void SetShaderTexture3(this MiscShaderData shader, Asset<Texture2D> texture) => shaderTextureField3.SetValue(shader, texture);

        /// <summary>
        /// Prepares a <see cref="SpriteBatch"/> for shader-based drawing.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch.</param>
        public static void EnterShaderRegion(this SpriteBatch spriteBatch)
        {
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
        }

        /// <summary>
        /// Ends changes to a <see cref="SpriteBatch"/> based on shader-based drawing in favor of typical draw begin states.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch.</param>
        public static void ExitShaderRegion(this SpriteBatch spriteBatch)
        {
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
        }

        /// <summary>
        /// Sets a <see cref="SpriteBatch"/>'s <see cref="BlendState"/> arbitrarily.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch.</param>
        /// <param name="blendState">The blend state to use.</param>
        public static void SetBlendState(this SpriteBatch spriteBatch, BlendState blendState)
        {
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, blendState, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
        }

        /// <summary>
        /// Reset's a <see cref="SpriteBatch"/>'s <see cref="BlendState"/> based to a typical <see cref="BlendState.AlphaBlend"/>.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch.</param>
        /// <param name="blendState">The blend state to use.</param>
        public static void ResetBlendState(this SpriteBatch spriteBatch) => spriteBatch.SetBlendState(BlendState.AlphaBlend);

        /// <summary>
        /// Restarts a given <see cref="SpriteBatch"/> such that it enforces a rectangular area where pixels outside of said area are not drawn.<br></br>
        /// This is incredible convenient for UI sections where you need to ensure things only appear inside a box panel.<br></br>
        /// This method should be followed by a call to <see cref="ReleaseCutoffRegion(SpriteBatch, Matrix, SpriteSortMode)"/> once you're ready to flush the contents drawn under these conditions.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch to enforce the cutoff region on.</param>
        /// <param name="cutoffRegion">The cutoff region. This should be in screen coordinates.</param>
        /// <param name="perspective">The perspective matrix that should be used across drawn contents.</param>
        /// <param name="sortMode">The sort mode that should be used across drawn contents. Use <see cref="SpriteSortMode.Immediate"/> if you additionally need to draw shaders.</param>
        /// <param name="newBlendState">The blend state that should be used across drawn contents. This defaults to <see cref="BlendState.AlphaBlend"/>.</param>
        public static void EnforceCutoffRegion(this SpriteBatch spriteBatch, Rectangle cutoffRegion, Matrix perspective, SpriteSortMode sortMode = SpriteSortMode.Deferred, BlendState newBlendState = null)
        {
            var rasterizer = Main.Rasterizer;
            rasterizer.ScissorTestEnable = true;

            spriteBatch.End();
            spriteBatch.Begin(sortMode, newBlendState ?? BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, rasterizer, null, perspective);
            spriteBatch.GraphicsDevice.ScissorRectangle = cutoffRegion;
        }

        /// <summary>
        /// Flushes contents drawn under restrictions enforced by the <see cref="EnforceCutoffRegion(SpriteBatch, Rectangle, Matrix, SpriteSortMode, BlendState)"/> method and returns the <see cref="SpriteBatch"/> to a more typical state.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch to flush the contents of.</param>
        /// <param name="perspective">The perspective matrix that was used before the cutoff region was enforced. Take care to ensure that this has the correct input.</param>
        /// <param name="sortMode">The sort mode that should be used across drawn contents. Use <see cref="SpriteSortMode.Immediate"/> if you additionally need to draw shaders.</param>
        public static void ReleaseCutoffRegion(this SpriteBatch spriteBatch, Matrix perspective, SpriteSortMode sortMode = SpriteSortMode.Deferred)
        {
            int width = spriteBatch.GraphicsDevice.Viewport.Width;
            int height = spriteBatch.GraphicsDevice.Viewport.Height;

            spriteBatch.End();
            spriteBatch.Begin(sortMode, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, perspective);
            spriteBatch.GraphicsDevice.ScissorRectangle = new(-1, -1, width + 2, height + 2);
        }

        /// <summary>
        /// Draws a line significantly more efficiently than <see cref="Utils.DrawLine(SpriteBatch, Vector2, Vector2, Color, Color, float)"/> using just one scaled line texture. Positions are automatically converted to screen coordinates.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch by which the line should be drawn.</param>
        /// <param name="start">The starting point of the line in world coordinates.</param>
        /// <param name="end">The ending point of the line in world coordinates.</param>
        /// <param name="color">The color of the line.</param>
        /// <param name="width">The width of the line.</param>
        public static void DrawLineBetter(this SpriteBatch spriteBatch, Vector2 start, Vector2 end, Color color, float width)
        {
            // Draw nothing if the start and end are equal, to prevent division by 0 problems.
            if (start == end)
                return;

            start -= Main.screenPosition;
            end -= Main.screenPosition;

            Texture2D line = StellasTextureRegistry.Line.Value;
            float rotation = (end - start).ToRotation();
            Vector2 scale = new(Vector2.Distance(start, end) / line.Width, width);

            spriteBatch.Draw(line, start, null, color, rotation, line.Size() * Vector2.UnitY * 0.5f, scale, SpriteEffects.None, 0f);
        }

        public static void DrawBloomLine(this SpriteBatch spriteBatch, Vector2 start, Vector2 end, Color color, float width)
        {
            // Draw nothing if the start and end are equal, to prevent division by 0 problems.
            if (start == end)
                return;

            start -= Main.screenPosition;
            end -= Main.screenPosition;

            Texture2D line = StellasTextureRegistry.BloomLine.Value;
            float rotation = (end - start).ToRotation() + MathHelper.PiOver2;
            Vector2 scale = new Vector2(width, Vector2.Distance(start, end)) / line.Size();
            Vector2 origin = new(line.Width / 2f, line.Height);

            spriteBatch.Draw(line, start, null, color, rotation, origin, scale, SpriteEffects.None, 0f);
        }

        /// <summary>
        /// Creates a generic dust explosion at a given position.
        /// </summary>
        /// <param name="spawnPosition">The place to spawn dust at.</param>
        /// <param name="dustType">The dust ID to use.</param>
        /// <param name="dustPerBurst">The amount of dust to spawn per burst.</param>
        /// <param name="burstSpeed">The speed of the dust when exploding.</param>
        /// <param name="baseScale">The scale of the dust</param>
        public static void CreateGenericDustExplosion(Vector2 spawnPosition, int dustType, int dustPerBurst, float burstSpeed, float baseScale)
        {
            // Generate a dust explosion
            float burstDirectionVariance = 3;
            for (int j = 0; j < 10; j++)
            {
                burstDirectionVariance += j * 2;
                for (int k = 0; k < dustPerBurst; k++)
                {
                    Dust burstDust = Dust.NewDustPerfect(spawnPosition, dustType);
                    burstDust.scale = baseScale * Main.rand.NextFloat(0.8f, 1.2f);
                    burstDust.position = spawnPosition + Main.rand.NextVector2Circular(10f, 10f);
                    burstDust.velocity = Main.rand.NextVector2Square(-burstDirectionVariance, burstDirectionVariance).SafeNormalize(Vector2.UnitY) * burstSpeed;
                    burstDust.noGravity = true;
                }
                burstSpeed += 3f;
            }
        }

       

        public static void CreateFireExplosion(Vector2 topLeft, Vector2 area, Vector2 force)
        {
            // Sparks and such
            for (int i = 0; i < 40; i++)
            {
                int idx = Dust.NewDust(topLeft, (int)area.X, (int)area.Y, DustID.Smoke, 0f, 0f, 100, default, 2f);
                Main.dust[idx].velocity *= 3f;
                if (Main.rand.NextBool(2))
                {
                    Main.dust[idx].scale = 0.5f;
                    Main.dust[idx].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                    Main.dust[idx].velocity += force.RotatedByRandom(0.4f) * Main.rand.NextFloat(0.8f, 1.2f);
                }
            }
            for (int i = 0; i < 70; i++)
            {
                int idx = Dust.NewDust(topLeft, (int)area.X, (int)area.Y, DustID.Torch, 0f, 0f, 100, default, 3f);
                Main.dust[idx].noGravity = true;
                Main.dust[idx].velocity *= 5f;
                Main.dust[idx].velocity += force.RotatedByRandom(0.4f) * Main.rand.NextFloat(0.8f, 1.2f);

                idx = Dust.NewDust(topLeft, (int)area.X, (int)area.Y, DustID.Torch, 0f, 0f, 100, default, 2f);
                Main.dust[idx].velocity *= 2f;
                Main.dust[idx].velocity += force.RotatedByRandom(0.4f) * Main.rand.NextFloat(0.8f, 1.2f);
            }

            // Smoke, which counts as a Gore
            if (Main.netMode != NetmodeID.Server)
            {
                int goreAmt = 3;
                Vector2 center = topLeft + area * 0.5f;
                Vector2 source = new(center.X - 24f, center.Y - 24f);
                for (int goreIndex = 0; goreIndex < goreAmt; goreIndex++)
                {
                    float velocityMult = 0.33f;
                    if (goreIndex < (goreAmt / 3))
                        velocityMult = 0.66f;
                    if (goreIndex >= (2 * goreAmt / 3))
                        velocityMult = 1f;

                    int type = Main.rand.Next(61, 64);
                    int smoke = Gore.NewGore(new EntitySource_WorldEvent(), source, default, type, 1f);
                    Gore gore = Main.gore[smoke];
                    gore.velocity *= velocityMult;
                    gore.velocity.X += 1f;
                    gore.velocity.Y += 1f;
                    gore.velocity += force.RotatedByRandom(0.4f) * Main.rand.NextFloat(0.8f, 1.2f);

                    type = Main.rand.Next(61, 64);
                    smoke = Gore.NewGore(new EntitySource_WorldEvent(), source, default, type, 1f);
                    gore = Main.gore[smoke];
                    gore.velocity *= velocityMult;
                    gore.velocity.X -= 1f;
                    gore.velocity.Y += 1f;
                    gore.velocity += force.RotatedByRandom(0.4f) * Main.rand.NextFloat(0.8f, 1.2f);

                    type = Main.rand.Next(61, 64);
                    smoke = Gore.NewGore(new EntitySource_WorldEvent(), source, default, type, 1f);
                    gore = Main.gore[smoke];
                    gore.velocity *= velocityMult;
                    gore.velocity.X += 1f;
                    gore.velocity.Y -= 1f;
                    gore.velocity += force.RotatedByRandom(0.4f) * Main.rand.NextFloat(0.8f, 1.2f);

                    type = Main.rand.Next(61, 64);
                    smoke = Gore.NewGore(new EntitySource_WorldEvent(), source, default, type, 1f);
                    gore = Main.gore[smoke];
                    gore.velocity *= velocityMult;
                    gore.velocity.X -= 1f;
                    gore.velocity.Y -= 1f;
                    gore.velocity += force.RotatedByRandom(0.4f) * Main.rand.NextFloat(0.8f, 1.2f);
                }
            }
        }

        /// <summary>
        /// Draws a projectile as a series of afterimages. The first of these afterimages is centered on the center of the projectile's hitbox.<br />
        /// This function is guaranteed to draw the projectile itself, even if it has no afterimages and/or the Afterimages config option is turned off.
        /// </summary>
        /// <param name="proj">The projectile to be drawn.</param>
        /// <param name="mode">The type of afterimage drawing code to use. Vanilla Terraria has three options: 0, 1, and 2.</param>
        /// <param name="lightColor">The light color to use for the afterimages.</param>
        /// <param name="typeOneIncrement">If mode 1 is used, this controls the loop increment. Set it to more than 1 to skip afterimages.</param>
        /// <param name="texture">The texture to draw. Set to <b>null</b> to draw the projectile's own loaded texture.</param>
        /// <param name="drawCentered">If <b>false</b>, the afterimages will be centered on the projectile's position instead of its own center.</param>
        

        public static void DisplayText(string text, Color? color = null)
        {
            if (Main.netMode == NetmodeID.SinglePlayer)
                Main.NewText(text, color ?? Color.White);
            else if (Main.netMode == NetmodeID.Server)
                ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral(text), color ?? Color.White);
        }

        public static void GetCircleVertices(int sideCount, float radius, Vector2 center, out List<short> triangleIndices, out List<PrimitiveTrail.VertexPosition2DColor> vertices)
        {
            vertices = new();
            triangleIndices = new();

            // Use the law of cosines to determine the side length of the triangles that compose the inscribed shape.
            float sideAngle = MathHelper.TwoPi / sideCount;
            float sideLength = MathF.Sqrt(2f - MathF.Cos(sideAngle) * 2f) * radius;

            // Calculate vertices by approximating a circle with a bunch of triangles.
            for (int i = 0; i < sideCount; i++)
            {
                float completionRatio = i / (float)(sideCount - 1f);
                float nextCompletionRatio = (i + 1) / (float)(sideCount - 1f);
                Vector2 orthogonal = (MathHelper.TwoPi * completionRatio + MathHelper.PiOver2).ToRotationVector2();
                Vector2 radiusOffset = (MathHelper.TwoPi * completionRatio).ToRotationVector2() * radius;
                Vector2 leftEdgeInner = center;
                Vector2 rightEdgeInner = center;
                Vector2 leftEdge = leftEdgeInner + radiusOffset + orthogonal * sideLength * -0.5f;
                Vector2 rightEdge = rightEdgeInner + radiusOffset + orthogonal * sideLength * 0.5f;

                vertices.Add(new(leftEdge - Main.screenPosition, Color.White, new(completionRatio, 1f)));
                vertices.Add(new(rightEdge - Main.screenPosition, Color.White, new(nextCompletionRatio, 1f)));
                vertices.Add(new(rightEdgeInner - Main.screenPosition, Color.White, new(nextCompletionRatio, 0f)));
                vertices.Add(new(leftEdgeInner - Main.screenPosition, Color.White, new(completionRatio, 0f)));

                triangleIndices.Add((short)(i * 4));
                triangleIndices.Add((short)(i * 4 + 1));
                triangleIndices.Add((short)(i * 4 + 2));
                triangleIndices.Add((short)(i * 4));
                triangleIndices.Add((short)(i * 4 + 2));
                triangleIndices.Add((short)(i * 4 + 3));
            }
        }

       

       

     

        public static void EmptyDrawCache(this List<DrawData> drawCache)
        {
            // WHAT THE FUCK NO ABORT ABORT ABORT
            if (drawCache.Count >= 10000 || Main.mapFullscreen)
                drawCache.Clear();

            Vector2 topLeft = Vector2.One * -200f;
            Vector2 bottomRight = new Vector2(Main.screenWidth, Main.screenHeight) - topLeft;
            while (drawCache.Count > 0)
            {
                if (drawCache[0].position.Length() > 10000f && drawCache[0].position.Between(topLeft, bottomRight))
                    drawCache[0] = drawCache[0] with { position = drawCache[0].position - Main.screenPosition };
                drawCache[0].Draw(Main.spriteBatch);
                drawCache.RemoveAt(0);
            }
        }

       
        public static void SwapToRenderTarget(this RenderTarget2D renderTarget, Color? flushColor = null)
        {
            // Local variables for convinience.
            GraphicsDevice graphicsDevice = Main.graphics.GraphicsDevice;
            SpriteBatch spriteBatch = Main.spriteBatch;

            // If we are in the menu, a server, or any of these are null, return.
            if (Main.gameMenu || Main.dedServ || renderTarget is null || graphicsDevice is null || spriteBatch is null)
                return;

            // Otherwise set the render target.
            graphicsDevice.SetRenderTarget(renderTarget);

            // "Flush" the screen, removing any previous things drawn to it.
            flushColor ??= Color.Transparent;
            graphicsDevice.Clear(flushColor.Value);
        }

       

      

        /// <summary>
        /// Return a matrix suitable for use when resetting spritebatches in CustomSkys for shader work.
        /// </summary>
        /// <returns></returns>
        public static Matrix GetCustomSkyBackgroundMatrix()
        {
            Matrix transformationMatrix = Main.BackgroundViewMatrix.TransformationMatrix;
            transformationMatrix.Translation -= Main.BackgroundViewMatrix.ZoomMatrix.Translation *
                new Vector3(1f, Main.BackgroundViewMatrix.Effects.HasFlag(SpriteEffects.FlipVertically) ? (-1f) : 1f, 1f);
            return transformationMatrix;
        }
    }
}
