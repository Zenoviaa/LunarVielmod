using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Trails;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace Stellamod.Helpers
{
    public static class DrawHelper
    {
		public static void DrawDimLight(Vector2 pos, Color color, float rotation, float scale)
		{
            SpriteBatch spriteBatch = Main.spriteBatch;
            Texture2D texture = ModContent.Request<Texture2D>("Stellamod/Effects/Masks/DimLight").Value;
            Color drawColor = new Color(color.R, color.G, color.B, 0);
			Vector2 drawOrigin = texture.Size() / 2;
            spriteBatch.Draw(texture, pos, null, drawColor, rotation, drawOrigin, scale, SpriteEffects.None, 0f);
        }

		public static void DrawCircle(Vector2 center, float radius, Vector2[] circlePos, float offset = 0)
		{
            Vector2 startDirection = Vector2.UnitY;
            for (int i = 0; i < circlePos.Length; i++)
            {
                float circleProgress = i / (float)(circlePos.Length);
                float radiansToRotateBy = (circleProgress * (MathHelper.TwoPi + MathHelper.PiOver4 / 2)) + offset;
                circlePos[i] = center + startDirection.RotatedBy(radiansToRotateBy) * radius;
            }
        }
        public static void DrawFlowerChains(Texture2D chainTexture, Vector2[] oldPos, Rectangle animationFrame, float alpha)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;

            float time = Main.GlobalTimeWrappedHourly;
            float timer = Main.GlobalTimeWrappedHourly / 2f + time * 0.04f;
            float rotationOffset = VectorHelper.Osc(1f, 2f, 5);
            time %= 4f;
            time /= 2f;

            if (time >= 1f)
            {
                time = 2f - time;
            }

            time = time * 0.5f + 0.5f;

            for (float k = 0f; k < 1f; k += 0.25f)
            {
                float radians = (k + timer) * MathHelper.TwoPi;
                for (int i = 1; i < oldPos.Length; i++)
                {
                    //Draw from center bottom of texture
                    Vector2 frameSize = new Vector2(animationFrame.Width, animationFrame.Height);
                    Vector2 origin = new Vector2(frameSize.X / 2, frameSize.Y);

                    Vector2 position = oldPos[i] + new Vector2(0f, 8f * rotationOffset).RotatedBy(radians) * time;

                    float rotation = (oldPos[i] - oldPos[i - 1]).ToRotation() - MathHelper.PiOver2; //Calculate rotation based on direction from last point
                    float yScale = Vector2.Distance(oldPos[i], oldPos[i - 1]) / frameSize.Y; //Calculate how much to squash/stretch for smooth chain based on distance between points

                    Vector2 scale = new Vector2(1, yScale); // Stretch/Squash chain segment
                    Color chainLightColor = Lighting.GetColor((int)position.X / 16, (int)position.Y / 16); //Lighting of the position of the chain segment
                    chainLightColor = chainLightColor.MultiplyAlpha(alpha);

                    for (int j = 0; j < 2; j++)
                    {
                        spriteBatch.Draw(chainTexture, position - Main.screenPosition, animationFrame,
                            chainLightColor * 0.2f, rotation, origin, scale, SpriteEffects.None, 0);
                    }
                }
            }

            for (int i = 1; i < oldPos.Length; i++)
            {
                //Draw from center bottom of texture
                Vector2 frameSize = new Vector2(8, 16);
                Vector2 origin = new Vector2(frameSize.X / 2, frameSize.Y);

                Vector2 position = oldPos[i];

                float rotation = (oldPos[i] - oldPos[i - 1]).ToRotation() - MathHelper.PiOver2; //Calculate rotation based on direction from last point
                float yScale = Vector2.Distance(oldPos[i], oldPos[i - 1]) / frameSize.Y; //Calculate how much to squash/stretch for smooth chain based on distance between points

                Vector2 scale = new Vector2(1, yScale); // Stretch/Squash chain segment
                Color chainLightColor = Lighting.GetColor((int)position.X / 16, (int)position.Y / 16); //Lighting of the position of the chain segment
                chainLightColor = chainLightColor.MultiplyAlpha(alpha);

                for (int j = 0; j < 3; j++)
                {
                    spriteBatch.Draw(chainTexture, position - Main.screenPosition, animationFrame,
                        chainLightColor, rotation, origin, scale, SpriteEffects.None, 0);
                }
            }
        }
		public static void DrawSupernovaChains(Texture2D chainTexture, Vector2 startPos, Vector2 endPos, Rectangle animationFrame, float alpha)
		{
			SpriteBatch spriteBatch = Main.spriteBatch;

			float time = Main.GlobalTimeWrappedHourly;
			float timer = Main.GlobalTimeWrappedHourly / 2f + time * 0.04f;
			float rotationOffset = VectorHelper.Osc(1f, 2f, 5);
			time %= 4f;
			time /= 2f;

			if (time >= 1f)
			{
				time = 2f - time;
			}

			time = time * 0.5f + 0.5f;

			for (float k = 0f; k < 1f; k += 0.25f)
			{
				float radians = (k + timer) * MathHelper.TwoPi;
				//Draw from center bottom of texture
				Vector2 frameSize = animationFrame.Size();
				Vector2 origin = new Vector2(frameSize.X / 2, frameSize.Y);

				Vector2 position = endPos + new Vector2(0f, 8f * rotationOffset).RotatedBy(radians) * time;

				float rotation = (endPos - startPos).ToRotation() - MathHelper.PiOver2; //Calculate rotation based on direction from last point
				float yScale = Vector2.Distance(endPos, startPos) / frameSize.Y; //Calculate how much to squash/stretch for smooth chain based on distance between points

				Vector2 scale = new Vector2(1, yScale); // Stretch/Squash chain segment
				Color chainLightColor = Lighting.GetColor((int)position.X / 16, (int)position.Y / 16); //Lighting of the position of the chain segment
				chainLightColor = chainLightColor.MultiplyAlpha(alpha);

				for (int j = 0; j < 2; j++)
				{
					spriteBatch.Draw(chainTexture, position - Main.screenPosition, animationFrame,
						chainLightColor * 0.2f, rotation, origin, scale, SpriteEffects.None, 0);
				}
			}

			{
				//Draw from center bottom of texture
				Vector2 frameSize = animationFrame.Size();
				Vector2 origin = new Vector2(frameSize.X / 2, frameSize.Y);

				Vector2 position = endPos;

				float rotation = (endPos - startPos).ToRotation() - MathHelper.PiOver2; //Calculate rotation based on direction from last point
				float yScale = Vector2.Distance(endPos, startPos) / frameSize.Y; //Calculate how much to squash/stretch for smooth chain based on distance between points

				Vector2 scale = new Vector2(1, yScale); // Stretch/Squash chain segment
				Color chainLightColor = Lighting.GetColor((int)position.X / 16, (int)position.Y / 16); //Lighting of the position of the chain segment
				chainLightColor = chainLightColor.MultiplyAlpha(alpha);

				for (int j = 0; j < 3; j++)
				{
					spriteBatch.Draw(chainTexture, position - Main.screenPosition, animationFrame,
						chainLightColor, rotation, origin, scale, SpriteEffects.None, 0);
				}
			}
        }
        public static void DrawSupernovaChains(Texture2D chainTexture, Vector2[] oldPos, Rectangle animationFrame, float alpha)
		{
            SpriteBatch spriteBatch = Main.spriteBatch;

            float time = Main.GlobalTimeWrappedHourly;
            float timer = Main.GlobalTimeWrappedHourly / 2f + time * 0.04f;
            float rotationOffset = VectorHelper.Osc(1f, 2f, 5);
            time %= 4f;
            time /= 2f;

            if (time >= 1f)
            {
                time = 2f - time;
            }

            time = time * 0.5f + 0.5f;

            for (float k = 0f; k < 1f; k += 0.25f)
            {
                float radians = (k + timer) * MathHelper.TwoPi;
                for (int i = 1; i < oldPos.Length; i++)
                {
					//Draw from center bottom of texture
					Vector2 frameSize = animationFrame.Size();
                    Vector2 origin = new Vector2(frameSize.X / 2, frameSize.Y);

                    Vector2 position = oldPos[i] + new Vector2(0f, 8f * rotationOffset).RotatedBy(radians) * time;

                    float rotation = (oldPos[i] - oldPos[i - 1]).ToRotation() - MathHelper.PiOver2; //Calculate rotation based on direction from last point
                    float yScale = Vector2.Distance(oldPos[i], oldPos[i - 1]) / frameSize.Y; //Calculate how much to squash/stretch for smooth chain based on distance between points

                    Vector2 scale = new Vector2(1, yScale); // Stretch/Squash chain segment
                    Color chainLightColor = Lighting.GetColor((int)position.X / 16, (int)position.Y / 16); //Lighting of the position of the chain segment
                    chainLightColor = chainLightColor.MultiplyAlpha(alpha);

                    for (int j = 0; j < 2; j++)
                    {
                        spriteBatch.Draw(chainTexture, position - Main.screenPosition, animationFrame,
                            chainLightColor * 0.2f, rotation, origin, scale, SpriteEffects.None, 0);
                    }
                }
            }

            for (int i = 1; i < oldPos.Length; i++)
            {
				//Draw from center bottom of texture
				Vector2 frameSize = animationFrame.Size();
                Vector2 origin = new Vector2(frameSize.X / 2, frameSize.Y);

                Vector2 position = oldPos[i];

                float rotation = (oldPos[i] - oldPos[i - 1]).ToRotation() - MathHelper.PiOver2; //Calculate rotation based on direction from last point
                float yScale = Vector2.Distance(oldPos[i], oldPos[i - 1]) / frameSize.Y; //Calculate how much to squash/stretch for smooth chain based on distance between points

                Vector2 scale = new Vector2(1, yScale); // Stretch/Squash chain segment
                Color chainLightColor = Lighting.GetColor((int)position.X / 16, (int)position.Y / 16); //Lighting of the position of the chain segment
				chainLightColor = chainLightColor.MultiplyAlpha(alpha);

                for (int j = 0; j < 3; j++)
                {
                    spriteBatch.Draw(chainTexture, position - Main.screenPosition, animationFrame,
                        chainLightColor, rotation, origin, scale, SpriteEffects.None, 0);
                }
            }
        }

		public static void DrawChainOval(Vector2 center, float xRadius, float yRadius, float angle, float rotation, ref Vector2[] oldPos)
        {
            for (int i = 0; i < oldPos.Length; i++)
            {
                float ovalProgress = (float)(i / (float)oldPos.Length);
                float xOffset = xRadius * MathF.Cos(ovalProgress * angle);
                float yOffset = yRadius * MathF.Sin(ovalProgress * angle);
                Vector2 pointOnOval = center + new Vector2(xOffset, yOffset).RotatedBy(rotation);
                oldPos[i] = pointOnOval;
            }
        }

		/// <summary>
		/// Oscillates between two colors
		/// </summary>
		/// <param name="from"></param>
		/// <param name="to"></param>
		/// <param name="speed"></param>
		/// <returns></returns>
		public static Color Oscillate(Color from, Color to, float speed)
        {
			float t = VectorHelper.Osc(0, 1, speed);
			return Color.Lerp(from, to, t);
        }


		/// <summary>
		/// Oscillates between two colors, but Huntrian
		/// <br>See Firefly Staff for example usage</br>
		/// </summary>
		/// <param name="from"></param>
		/// <param name="to"></param>
		/// <param name="speed"></param>
		/// <param name="colorOffset"></param>
		/// <returns></returns>
		public static Vector3 HuntrianColorOscillate(Vector3 from, Vector3 to, Vector3 speed, float colorOffset)
		{
			Vector3 xyz;
			xyz.X = VectorHelper.Osc(from.X, to.X, speed.X, colorOffset);
			xyz.Y = VectorHelper.Osc(from.Y, to.Y, speed.Y, colorOffset);
			xyz.Z = VectorHelper.Osc(from.Z, to.Z, speed.Z, colorOffset);
			return xyz;
		}


		public static PrimDrawer TrailDrawer { get; private set; } = null;

        public static void DrawLineTelegraph(Vector2 drawPos, Color drawColor, Vector2 velocity, float drawScale = 1f, SpriteEffects spriteEffects = SpriteEffects.None)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            Texture2D lineTexture = ModContent.Request<Texture2D>("Stellamod/Effects/Masks/Extra_47").Value;
            Vector2 drawOrigin = lineTexture.Size() / 2;
			float rotation = velocity.ToRotation() + MathHelper.PiOver2;
            spriteBatch.Draw(lineTexture, drawPos, null, drawColor, rotation, drawOrigin, drawScale, spriteEffects, 0);
        }


        public static void DrawLineTelegraph(Vector2 drawPos, Color drawColor, float rotation, float drawScale = 1f, SpriteEffects spriteEffects = SpriteEffects.None)
		{
			SpriteBatch spriteBatch = Main.spriteBatch;
            Texture2D lineTexture = ModContent.Request<Texture2D>("Stellamod/Effects/Masks/Extra_47").Value;
            Vector2 drawOrigin = lineTexture.Size() / 2;
            spriteBatch.Draw(lineTexture, drawPos, null, drawColor, rotation, drawOrigin, drawScale, spriteEffects, 0);
        }


		/// <summary>
		/// Draws a simple trail using "VampKnives:BasicTrail"
		/// <br></br>Don't forget to set the trailing cache and trailing modes on your projectile!
		/// </summary>
		/// <param name="projectile"></param>
		/// <param name="widthFunction"></param>
		/// <param name="colorFunction"></param>
		/// <param name="trailTexture"></param>
		public static void DrawSimpleTrail(Projectile projectile, 
			PrimDrawer.WidthTrailFunction widthFunction, 
			PrimDrawer.ColorTrailFunction colorFunction, 
			Asset<Texture2D> trailTexture,
			Vector2? frameSize = null,
			Vector2? offset = null)
		{
            if (TrailDrawer == null)
            {
				TrailDrawer = new PrimDrawer(widthFunction, colorFunction, GameShaders.Misc["VampKnives:BasicTrail"]);
            }

			TrailDrawer.WidthFunc = widthFunction;
			TrailDrawer.ColorFunc = colorFunction;
			GameShaders.Misc["VampKnives:BasicTrail"].SetShaderTexture(trailTexture);

			Vector2 trailOffset;
			if(frameSize != null)
			{
                trailOffset = (Vector2)frameSize * 0.5f - Main.screenPosition;
            }
			else
			{
                trailOffset = projectile.Size * 0.5f - Main.screenPosition;
            }

			if(offset != null)
			{
				trailOffset += (Vector2)offset;
			}

            TrailDrawer.DrawPrims(projectile.oldPos, trailOffset, 155);
        }


        public static void DrawSimpleTrail(NPC npc, PrimDrawer.WidthTrailFunction widthFunction, PrimDrawer.ColorTrailFunction colorFunction, Asset<Texture2D> trailTexture)
		{
			if (TrailDrawer == null)
			{
				TrailDrawer = new PrimDrawer(widthFunction, colorFunction, GameShaders.Misc["VampKnives:BasicTrail"]);
			}

			TrailDrawer.WidthFunc = widthFunction;
			TrailDrawer.ColorFunc = colorFunction;
			GameShaders.Misc["VampKnives:BasicTrail"].SetShaderTexture(trailTexture);
			TrailDrawer.DrawPrims(npc.oldPos, npc.Size * 0.5f - Main.screenPosition, 155);
		}

		/// <summary>
		/// Draws an after image for the projectile, this should be called in PreDraw
		/// <br>Don't forget to set defaults for ProjectileID.Sets.TrailCacheLength and ProjectileID.Sets.TrailingMode on your projectile otherwise this will not work</br>
		/// </summary>
		/// <param name="projectile"></param>
		/// <param name="startColor"></param>
		/// <param name="endColor"></param>
		/// <param name="lightColor"></param>
		public static void DrawAdditiveAfterImage(Projectile projectile, Color startColor, Color endColor, ref Color lightColor)
		{
			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

			Texture2D texture = TextureAssets.Projectile[projectile.type].Value;
			int projFrames = Main.projFrames[projectile.type];
			int frameHeight = texture.Height / projFrames;
			int startY = frameHeight * projectile.frame;

			Rectangle sourceRectangle = new Rectangle(0, startY, texture.Width, frameHeight);
			Vector2 drawOrigin = sourceRectangle.Size() / 2f;
			//drawOrigin.X = projectile.spriteDirection == 1 ? sourceRectangle.Width - offsetX : offsetX;
			for (int k = 0; k < projectile.oldPos.Length; k++)
			{
				Vector2 drawPos = projectile.oldPos[k] - Main.screenPosition + drawOrigin;// + new Vector2(0f, projectile.gfxOffY);
				Color color = projectile.GetAlpha(Color.Lerp(startColor, endColor, 1f / projectile.oldPos.Length * k) * (1f - 1f / projectile.oldPos.Length * k));
				Main.spriteBatch.Draw(texture, drawPos, sourceRectangle, color, projectile.oldRot[k], drawOrigin, projectile.scale, SpriteEffects.None, 0f);
			}

			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
		}
        public static void DrawAdditiveAfterImage(NPC npc, Color startColor, Color endColor)
        {
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            Texture2D texture = TextureAssets.Npc[npc.type].Value;
			Rectangle sourceRectangle = npc.frame;
            Vector2 drawOrigin = sourceRectangle.Size() / 2f;
            //drawOrigin.X = projectile.spriteDirection == 1 ? sourceRectangle.Width - offsetX : offsetX;
            for (int k = 0; k < npc.oldPos.Length; k++)
            {
                Vector2 drawPos = npc.oldPos[k] - Main.screenPosition + drawOrigin;// + new Vector2(0f, projectile.gfxOffY);
                Color color = npc.GetAlpha(Color.Lerp(startColor, endColor, 1f / npc.oldPos.Length * k) * (1f - 1f / npc.oldPos.Length * k));
                Main.spriteBatch.Draw(texture, drawPos, sourceRectangle, color, npc.oldRot[k], drawOrigin, npc.scale, SpriteEffects.None, 0f);
            }

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
        }

        public static void DrawGlowInInventory(Item item, SpriteBatch spriteBatch, Vector2 position, Color glowColor)
        {
			float sizeLimit = 34;
			int numberOfCloneImages = 6;
			Main.DrawItemIcon(spriteBatch, item, position, Color.White * 0.7f, sizeLimit);
			for (float i = 0; i < 1; i += 1f / numberOfCloneImages)
			{
				float cloneImageDistance = MathF.Cos(Main.GlobalTimeWrappedHourly / 2.4f * MathF.Tau / 2f) + 0.5f;
				cloneImageDistance = MathHelper.Max(cloneImageDistance, 0.1f);
				Color color = glowColor * 0.4f;
				color *= 1f - cloneImageDistance * 0.2f;
				color.A = 0;
				cloneImageDistance *= 3;
				Vector2 drawPos = position + (i * MathF.Tau).ToRotationVector2() * (cloneImageDistance + 2f);
				Main.DrawItemIcon(spriteBatch, item, drawPos, color, sizeLimit);
			}
		}

		public static void DrawAdvancedBroochGlow(Item item, SpriteBatch spriteBatch, Vector2 position, Color glowColor)
		{
			float sizeLimit = 34;
			int numberOfCloneImages = 3;
			Main.DrawItemIcon(spriteBatch, item, position, Color.White * 0.2f, sizeLimit);
			for (float i = 0; i < 1; i += 1f / numberOfCloneImages)
			{
				float cloneImageDistance = MathF.Cos(Main.GlobalTimeWrappedHourly / 2.4f * MathF.Tau / 2f) + 0.5f;
				cloneImageDistance = MathHelper.Max(cloneImageDistance, 0.05f);
				Color color = glowColor * 0.4f;
				color *= 1f - cloneImageDistance * 0.2f;
				color.A = 0;
				cloneImageDistance *= 3;
				Vector2 drawPos = position + (i * MathF.Tau).ToRotationVector2() * (cloneImageDistance + 2f);
				Main.DrawItemIcon(spriteBatch, item, drawPos, color, sizeLimit);
			}
		}

		public static void DrawGlow2InWorld(Item item, SpriteBatch spriteBatch, ref float rotation, ref float scale, int whoAmI)
        {
			// Draw the periodic glow effect behind the item when dropped in the world (hence PreDrawInWorld)
			Texture2D texture = TextureAssets.Item[item.type].Value;
			Rectangle frame;
			if (Main.itemAnimations[item.type] != null)
			{
				// In case this item is animated, this picks the correct frame
				frame = Main.itemAnimations[item.type].GetFrame(texture, Main.itemFrameCounter[whoAmI]);
			}
			else
			{
				frame = texture.Frame();
			}

			Vector2 frameOrigin = frame.Size() / 2f;
			Vector2 offset = new Vector2(item.width / 2 - frameOrigin.X, item.height - frame.Height);
			Vector2 drawPos = item.position - Main.screenPosition + frameOrigin + offset;

			float time = Main.GlobalTimeWrappedHourly;
			float timer = item.timeSinceItemSpawned / 240f + time * 0.04f;

			time %= 4f;
			time /= 2f;

			if (time >= 1f)
			{
				time = 2f - time;
			}

			time = time * 0.5f + 0.5f;
			for (float i = 0f; i < 1f; i += 0.25f)
			{
				float radians = (i + timer) * MathHelper.TwoPi;
				spriteBatch.Draw(texture, drawPos + new Vector2(0f, 8f).RotatedBy(radians) * time, frame, new Color(90, 70, 255, 50), rotation, frameOrigin, scale, SpriteEffects.None, 0);
			}

			for (float i = 0f; i < 1f; i += 0.34f)
			{
				float radians = (i + timer) * MathHelper.TwoPi;
				spriteBatch.Draw(texture, drawPos + new Vector2(0f, 4f).RotatedBy(radians) * time, frame, new Color(140, 120, 255, 77), rotation, frameOrigin, scale, SpriteEffects.None, 0);
			}
		}

		/// <summary>
		/// Draws a dim light effect, call this in any draw function
		/// </summary>
		/// <param name="projectile"></param>
		/// <param name="dimLightX"></param>
		/// <param name="dimLightY"></param>
		/// <param name="dimLightZ"></param>
		public static void DrawDimLight(Projectile projectile, float dimLightX, float dimLightY, float dimLightZ, Color worldLightingColor, Color lightColor, int glowCount = 4)
        {
			Texture2D texture = ModContent.Request<Texture2D>("Stellamod/Effects/Masks/DimLight").Value;
			for (int i = 0; i < glowCount; i++)
			{
				Main.spriteBatch.Draw(texture, projectile.Center - Main.screenPosition, null, new Color((int)(dimLightX * 1), (int)(dimLightY * 1), (int)(dimLightZ * 1), 0), projectile.rotation, new Vector2(32, 32), 0.17f * (7 + 0.6f), SpriteEffects.None, 0f);
			}

			Main.spriteBatch.Draw(texture, projectile.Center - Main.screenPosition, null, new Color((int)(dimLightX * 1), (int)(dimLightY * 1), (int)(dimLightZ * 1), 0), projectile.rotation, new Vector2(32, 32), 0.07f * (7 + 0.6f), SpriteEffects.None, 0f);
			Lighting.AddLight(projectile.Center, worldLightingColor.ToVector3() * 1.0f * Main.essScale);
		}

		public static void DrawDimLight(NPC npc, float dimLightX, float dimLightY, float dimLightZ, Color worldLightingColor, Color lightColor, int glowCount = 4)
		{
			Texture2D texture = ModContent.Request<Texture2D>("Stellamod/Effects/Masks/DimLight").Value;
			for (int i = 0; i < glowCount; i++)
			{
				Main.spriteBatch.Draw(texture, npc.Center - Main.screenPosition, null, new Color((int)(dimLightX * 1), (int)(dimLightY * 1), (int)(dimLightZ * 1), 0), npc.rotation, new Vector2(32, 32), 0.17f * (7 + 0.6f), SpriteEffects.None, 0f);
			}

			Main.spriteBatch.Draw(texture, npc.Center - Main.screenPosition, null, new Color((int)(dimLightX * 1), (int)(dimLightY * 1), (int)(dimLightZ * 1), 0), npc.rotation, new Vector2(32, 32), 0.07f * (7 + 0.6f), SpriteEffects.None, 0f);
			Lighting.AddLight(npc.Center, worldLightingColor.ToVector3() * 1.0f * Main.essScale);
		}

		/// <summary>
		/// Animates the projectile from top to bottom
		/// </summary>
		/// <param name="projectile"></param>
		/// <param name="frameSpeed"></param>
		public static void AnimateTopToBottom(Projectile projectile, int frameSpeed)
		{           
			// This is a simple "loop through all frames from top to bottom" animation
			projectile.frameCounter++;
			if (projectile.frameCounter >= frameSpeed)
			{
				projectile.frameCounter = 0;
				projectile.frame++;

				if (projectile.frame >= Main.projFrames[projectile.type])
				{
					projectile.frame = 0;
				}
			}
		}

        public static void UpdateFrame(ref float frame, float speed, int minFrame, int maxFrame)
        {
            frame += speed;
            if (frame < minFrame)
            {
                frame = minFrame;
            }
            if (frame > maxFrame)
            {
                frame = minFrame;
            }
        }


        public static Rectangle FrameGrid(float frame, int columns, int frameWidth, int frameHeight)
		{
            Rectangle rect = new Rectangle(0, 0, frameWidth, frameHeight);
            rect.X = ((int)frame % columns) * rect.Width;
            rect.Y = (((int)frame - ((int)frame % columns)) / columns) * rect.Height;
			return rect;
        }
    }
}
