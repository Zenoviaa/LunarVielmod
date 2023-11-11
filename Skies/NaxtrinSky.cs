using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.Graphics.Effects;
using Terraria.ModLoader;

namespace Stellamod.Skies
{
    public class NaxtrinSky : CustomSky
	{
		public override void Activate(Vector2 position, params object[] args)
		{
			worldSurface = (float)Main.worldSurface * 16f;
			_active = true;
			pointOfInterest = position;
			strengthTarget = 1f;
		}

		public override void Deactivate(params object[] args)
		{
			_active = false;
			strengthTarget = 0;
		}

		public override bool IsActive() => _strength > 0.001f && !Main.gameMenu;

		public override void Reset()
		{
			_active = false;
			strengthTarget = 0;
		}

		private bool _active;
		private float _strength;
		private float _windSpeed;
		private float _brightness;
		public static float strengthTarget;
		public static Color lightColor;
		public static Vector2 pointOfInterest;
		public static float worldSurface;

		public float Strength { get => _strength; }

		public static float? forceStrength;

		public override Color OnTileColor(Color inColor)
		{
			float inColorStrength = (inColor.R + inColor.G + inColor.B) / 3f / 255f;
			if (inColorStrength > 0.05f)
			{
				float fastStrength = Math.Clamp(_strength * 2f, 0, 1f);
				inColor = inColor.MultiplyRGBA(Color.Lerp(Color.White, lightColor, fastStrength));
				Main.ColorOfTheSkies = Color.Lerp(Main.ColorOfTheSkies, new Color(8, 0, 3), fastStrength);
			}
			return inColor;
		}

		public override float GetCloudAlpha() => 1f - _strength;

		public override void Update(GameTime gameTime)
		{
			if (!_active)
			{
				_strength = Math.Max(0f, _strength - 0.01f);
			}
			else if (strengthTarget != 0f)
			{
				if (_active && _strength < strengthTarget)
				{
					_strength = Math.Min(strengthTarget, _strength + 0.01f);
				}
				else
				{
					_strength = Math.Max(0, _strength - 0.01f);
				}

				if (forceStrength.HasValue)
				{
					_strength = forceStrength.Value;
					forceStrength = null;
				}


				else
				{
					pointOfInterest = Vector2.Lerp(pointOfInterest, Main.screenPosition + Main.ScreenSize.ToVector2() / 2f, 0.05f);
					worldSurface = MathHelper.Lerp(worldSurface, (float)Main.worldSurface * 16, 0.1f);
				}
			}

			_brightness = MathHelper.Lerp(_brightness, 0.15f, 0.08f);
			_windSpeed += 0.0025f;// Main.WindForVisuals * 0.005f;
			_windSpeed = _windSpeed % 10f;
		}

		public static Effect Neffect = ModContent.Request<Effect>("Stellamod/Effects/Primitives/NaxtrinSky", AssetRequestMode.ImmediateLoad).Value;

		public override void Draw(SpriteBatch spriteBatch, float minDepth, float maxDepth)
		{
			lightColor = Color.Lerp(Color.Crimson, Color.LightCoral, 0.5f);

			SkyManager.Instance["Ambience"].Deactivate();

			if (maxDepth >= float.MaxValue && minDepth < float.MaxValue)
			{
				//removing the sky
				spriteBatch.Draw(TextureAssets.BlackTile.Value, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.Black * (float)Math.Sqrt(_strength));
			}

			if (maxDepth >= 2 && minDepth < 2)
			{

				Vector2 offset = (Main.screenPosition + Main.ScreenSize.ToVector2() / 2f - pointOfInterest) / 10f;
				if (offset.Length() > 0)
					offset = Vector2.Lerp(offset, Vector2.Zero, 0.01f + offset.Length() * 0.001f);
				if (offset.Length() > 200)
					offset = offset.SafeNormalize(Vector2.Zero) * 200;

				Effect storm = Neffect;
				storm.Parameters["uScreenSize"].SetValue(Main.ScreenSize.ToVector2());
				storm.Parameters["uTexture0"].SetValue(ModContent.Request<Texture2D>("Stellamod/Textures/Noise1").Value);
				storm.Parameters["uTexture1"].SetValue(ModContent.Request<Texture2D>("Stellamod/Textures/Noise2").Value);
				storm.Parameters["uOffsetPosition"].SetValue(offset);
				storm.Parameters["uWorldPosition"].SetValue((Main.screenPosition + Main.ScreenSize.ToVector2() / 2f) / 5.5f);
				storm.Parameters["uTime"].SetValue(_windSpeed / 2f);
				storm.Parameters["uCurveFactor"].SetValue(1.2f);
				storm.Parameters["uColorMap"].SetValue(ModContent.Request<Texture2D>("Stellamod/Textures/ColorMap").Value);
				storm.Parameters["uColorMapSection"].SetValue(0.2f);
				storm.Parameters["uStrength"].SetValue(0.8f);
				storm.Parameters["uPower"].SetValue(2.2f);

				spriteBatch.End();
				spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, storm, Main.BackgroundViewMatrix.TransformationMatrix);

				spriteBatch.Draw(TextureAssets.BlackTile.Value, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.Pink * (float)Math.Sqrt(_strength));

				spriteBatch.End();
				spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.BackgroundViewMatrix.TransformationMatrix);


				//glowing up the sky
				Texture2D backGlow = (ModContent.Request<Texture2D>("Stellamod/Textures/Backglow").Value);
				float glowYPos = worldSurface - Utils.GetLerpValue(-worldSurface * 2, worldSurface, Main.screenPosition.Y) * worldSurface + backGlow.Height / 3 * 2;
				Vector2 glowPosition = new Vector2(Main.screenWidth / 2, glowYPos); //creating vertical parallax
				Rectangle glowFrame = new Rectangle(0, 0, Main.screenWidth, backGlow.Height);
				Vector2 glowOrigin = new Vector2(Main.screenWidth / 2, backGlow.Height);
				Vector2 glowScale = new Vector2(1f, 1.1f + MathF.Sin(Main.GlobalTimeWrappedHourly * 0.75f % MathHelper.TwoPi) * 0.5f);

				spriteBatch.Draw(backGlow, glowPosition, glowFrame, new Color(90, 20, 60, 30) * _strength, 0, glowOrigin, glowScale, 0, 0);
				spriteBatch.Draw(backGlow, glowPosition, glowFrame, new Color(100, 20, 70, 0) * _strength, 0, glowOrigin, glowScale * new Vector2(1f, 0.5f), 0, 0);
			}

			if (maxDepth >= 2 && minDepth < 2)
			{
				Vector2 offset = (Main.screenPosition + Main.ScreenSize.ToVector2() / 2f - pointOfInterest) / 6f;
				if (offset.Length() > 0)
					offset = Vector2.Lerp(offset, Vector2.Zero, 0.01f + offset.Length() * 0.001f);
				if (offset.Length() > 200)
					offset = offset.SafeNormalize(Vector2.Zero) * 200;

				Effect storm = Neffect;
				storm.Parameters["uScreenSize"].SetValue(Main.ScreenSize.ToVector2());
				storm.Parameters["uTexture0"].SetValue(ModContent.Request<Texture2D>("Stellamod/Textures/Noise1").Value);
				storm.Parameters["uTexture1"].SetValue(ModContent.Request<Texture2D>("Stellamod/Textures/Noise2").Value);
				storm.Parameters["uOffsetPosition"].SetValue(offset);
				storm.Parameters["uWorldPosition"].SetValue((Main.screenPosition + Main.ScreenSize.ToVector2() / 2f) / 2.5f);
				storm.Parameters["uTime"].SetValue(_windSpeed);
				storm.Parameters["uCurveFactor"].SetValue(1.6f);
				storm.Parameters["uColorMap"].SetValue(ModContent.Request<Texture2D>("Stellamod/Textures/ColorMap").Value);
				storm.Parameters["uColorMapSection"].SetValue(0.2f);
				storm.Parameters["uStrength"].SetValue(1.6f);
				storm.Parameters["uPower"].SetValue(5f);

				spriteBatch.End();
				spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, storm, Main.BackgroundViewMatrix.TransformationMatrix);

				spriteBatch.Draw(TextureAssets.BlackTile.Value, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.White * (float)Math.Sqrt(_strength));

				spriteBatch.End();
				spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.BackgroundViewMatrix.TransformationMatrix);
			}

		}
	}
}