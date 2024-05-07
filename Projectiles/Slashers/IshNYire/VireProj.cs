using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Buffs;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Trails;
using Stellamod.UI.Systems;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Slashers.IshNYire
{
	public class VireProj : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Halhurish");
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
			ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
		}

		public override void SetDefaults()
		{
			Projectile.friendly = true;
			Projectile.Size = new Vector2(120, 120);
			Projectile.tileCollide = false;
			Projectile.ownerHitCheck = true;
			Projectile.ignoreWater = true;
			Projectile.penetrate = -1;
			Projectile.usesLocalNPCImmunity = true;

		}

		private Player Owner => Main.player[Projectile.owner];

		public int SwingTime;
		public float SwingDistance;
		public float Curvature;

		public ref float Timer => ref Projectile.ai[0];
		public ref float AiState => ref Projectile.ai[1];


		private Vector2 returnPosOffset; //The position of the projectile when it starts returning to the player from being hooked
		private Vector2 npcHookOffset = Vector2.Zero; //Used to determine the offset from the hooked npc's center
		private float npcHookRotation; //Stores the projectile's rotation when hitting an npc
		private NPC hookNPC; //The npc the projectile is hooked into

		public const float THROW_RANGE = 320; //Peak distance from player when thrown out, in pixels
		public const float HOOK_MAXRANGE = 700; //Maximum distance between owner and hooked enemies before it automatically rips out
		public const int HOOK_HITTIME = 20; //Time between damage ticks while hooked in
		public const int RETURN_TIME = 6; //Time it takes for the projectile to return to the owner after being ripped out

		private int _flashTime;

		public bool Flip = false;
		public bool Slam = false;
		public bool PreSlam = false;

		private List<float> oldRotation = new List<float>();
		private List<Vector2> oldBase = new List<Vector2>();

		public Vector2 CurrentBase = Vector2.Zero;

		private int slamTimer = 0;

		public override void AI()
		{
			if (Projectile.timeLeft > 2) //Initialize chain control points on first tick, in case of projectile hooking in on first tick
			{
				_chainMidA = Projectile.Center;
				_chainMidB = Projectile.Center;


			}

			Lighting.AddLight(CurrentBase, new Color(254, 204, 72).ToVector3());
			Projectile.timeLeft = 2;

			if (Slam)
				Owner.itemTime = ((Owner.itemAnimation = 20) / 2);
			else if (PreSlam)
				Owner.itemTime = ((Owner.itemAnimation = 5) / 2);

			ThrowOutAI();

			if (!Slam)
				Owner.itemRotation = MathHelper.WrapAngle(Owner.AngleTo(Projectile.Center) - (Owner.direction < 0 ? MathHelper.Pi : 0));
			else
				Owner.itemRotation = MathHelper.WrapAngle(Owner.AngleTo(Main.MouseWorld) - (Owner.direction < 0 ? MathHelper.Pi : 0));
			_flashTime = Math.Max(_flashTime - 1, 0);




		}
		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{

			ShakeModSystem.Shake = 4;
			SoundEngine.PlaySound(new SoundStyle($"{nameof(Stellamod)}/Assets/Sounds/MorrowExp"));
			float speedX = Projectile.velocity.X * Main.rand.NextFloat(.2f, .3f) + Main.rand.NextFloat(-4f, 4f);
			float speedY = Projectile.velocity.Y * Main.rand.Next(20, 35) * 0.01f + Main.rand.Next(-10, 11) * 0.2f;

			if (Main.rand.NextBool(10))
				target.AddBuff(ModContent.BuffType<SigfriedsInsanity>(), 180);


			if (Slam)
			{

				/*
				Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position.X + speedX, Projectile.position.Y + speedY, speedX, speedY, ProjectileID.WandOfSparkingSpark, (int)(Projectile.damage * 1.5), 0f, Projectile.owner, 0f, 0f);
				Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position.X + speedX, Projectile.position.Y + speedY, speedX, speedY * 3, ProjectileID.WandOfSparkingSpark, (int)(Projectile.damage * 1.5), 0f, Projectile.owner, 0f, 0f);
				Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position.X + speedX, Projectile.position.Y + speedY, speedX * 0.4f, speedY, ProjectileID.WandOfSparkingSpark, (int)(Projectile.damage * 1.5), 0f, Projectile.owner, 0f, 0f);
				Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position.X + speedX, Projectile.position.Y + speedY, speedX, speedY, ProjectileID.WandOfSparkingSpark, (int)(Projectile.damage * 1.5), 0f, Projectile.owner, 0f, 0f);
				Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position.X + speedX, Projectile.position.Y + speedY, speedX * 0.5f, speedY, ProjectileID.WandOfSparkingSpark, (int)(Projectile.damage * 1.5), 0f, Projectile.owner, 0f, 0f);
				Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position.X + speedX, Projectile.position.Y + speedY, speedX, speedY, ProjectileID.WandOfSparkingSpark, (int)(Projectile.damage * 1.5), 0f, Projectile.owner, 0f, 0f);
				Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position.X + speedX, Projectile.position.Y + speedY, speedX * 0.25f, speedY * 2, ProjectileID.WandOfSparkingSpark, (int)(Projectile.damage * 1.5), 0f, Projectile.owner, 0f, 0f);
				Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position.X + speedX, Projectile.position.Y + speedY, speedX * 5, speedY * 3, ProjectileID.WandOfSparkingSpark, (int)(Projectile.damage * 1.5), 0f, Projectile.owner, 0f, 0f);
				Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position.X + speedX, Projectile.position.Y + speedY, speedX * 2, speedY, ProjectileID.WandOfSparkingSpark, (int)(Projectile.damage * 1.5), 0f, Projectile.owner, 0f, 0f);
				*/
				target.SimpleStrikeNPC(Projectile.damage * 15, 1, crit: false, Projectile.knockBack);

				Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(Projectile.Center, 1024f, 32f);
				SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Vinger2"));

				for (int i = 0; i < 14; i++)
				{
					Dust.NewDustPerfect(target.Center, ModContent.DustType<GlowDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.Yellow, 1f).noGravity = true;
				}
				for (int i = 0; i < 14; i++)
				{
					Dust.NewDustPerfect(target.Center, ModContent.DustType<TSmokeDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.Orange, 1f).noGravity = true;
				}

			}
			else
			{
				SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Vinger"));
				ShakeModSystem.Shake = 4;
				for (int i = 0; i < 14; i++)
				{
					Dust.NewDustPerfect(target.Center, ModContent.DustType<GlowDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.Orange, 0.5f).noGravity = true;
				}
				for (int i = 0; i < 4; i++)
				{
					Dust.NewDustPerfect(target.Center, ModContent.DustType<TSmokeDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.DarkGray, 0.5f).noGravity = true;
				}
			}
		}
		private Vector2 GetSwingPosition(float progress)
		{
			//Starts at owner center, goes to peak range, then returns to owner center
			float distance = MathHelper.Clamp(SwingDistance, THROW_RANGE * 0.1f, THROW_RANGE) * MathHelper.Lerp((float)Math.Sin(progress * MathHelper.Pi), 1, 0.04f);
			distance = Math.Max(distance, 100); //Dont be too close to player

			float angleMaxDeviation = MathHelper.Pi / 1.2f;
			float angleOffset = Owner.direction * (Flip ? -1 : 1) * MathHelper.Lerp(-angleMaxDeviation, angleMaxDeviation, progress); //Moves clockwise if player is facing right, counterclockwise if facing left
			return Projectile.velocity.RotatedBy(angleOffset) * distance;
		}

		private void ThrowOutAI()
		{
			Projectile.rotation = Projectile.AngleFrom(Owner.Center);
			Vector2 position = Owner.MountedCenter;
			float progress = ++Timer / SwingTime; //How far the projectile is through its swing


			if (slamTimer == 5)
				SoundEngine.PlaySound(SoundID.NPCDeath7, Projectile.Center);

			Projectile.Center = position + GetSwingPosition(progress);
			Projectile.direction = Projectile.spriteDirection = -Owner.direction * (Flip ? -1 : 1);

			if (Timer >= SwingTime)
				Projectile.Kill();
		}

		public TrailRenderer SwordSlash;
		public TrailRenderer SwordSlash2;
		public TrailRenderer SwordSlash3;
		public TrailRenderer SwordSlash4;
		public override bool PreDraw(ref Color lightColor)
		{
			Main.spriteBatch.End();

			var TrailTex = ModContent.Request<Texture2D>("Stellamod/Effects/Primitives/Trails/WhispyTrail").Value;
			var TrailTex2 = ModContent.Request<Texture2D>("Stellamod/Effects/Primitives/Trails/WhispyTrail").Value;
			var TrailTex3 = ModContent.Request<Texture2D>("Stellamod/Effects/Primitives/Trails/WaterTrail").Value;
			var TrailTex4 = ModContent.Request<Texture2D>("Stellamod/Effects/Primitives/Trails/WaterTrail").Value;
			Color color = Color.Multiply(new(1.50f, 1.75f, 3.5f, 0), 200);



			/*
            Texture2D Texture2 = TextureAssets.Projectile[Projectile.type].Value;
            Main.spriteBatch.Draw(Texture2, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, new Vector2(Texture2.Width / 2, Texture2.Height / 2), 1f, Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
            TrailDrawer ??= new PrimDrawer(WidthFunction, ColorFunction, GameShaders.Misc["VampKnives:BasicTrail"]);
            GameShaders.Misc["VampKnives:BasicTrail"].SetShaderTexture(TrailRegistry.SmallWhispyTrail);
            TrailDrawer.DrawPrims(Projectile.oldPos, Projectile.Size * 0.5f - Main.screenPosition, 155);*/
			if (SwordSlash == null)
			{
				SwordSlash = new TrailRenderer(TrailTex, TrailRenderer.DefaultPass, (p) => new Vector2(20f), (p) => new Color(100, 100, 90, 50) * (1f - p));
				SwordSlash.drawOffset = Projectile.Size / 1.8f;
			}
			if (SwordSlash2 == null)
			{
				SwordSlash2 = new TrailRenderer(TrailTex2, TrailRenderer.DefaultPass, (p) => new Vector2(50f), (p) => new Color(150, 150, 150, 50) * (1f - p));
				SwordSlash2.drawOffset = Projectile.Size / 1.9f;

			}
			if (SwordSlash3 == null)
			{
				SwordSlash3 = new TrailRenderer(TrailTex3, TrailRenderer.DefaultPass, (p) => new Vector2(80f), (p) => new Color(100, 50, 50, 100));
				SwordSlash3.drawOffset = Projectile.Size / 2f;

			}

			if (SwordSlash4 == null)
			{
				SwordSlash4 = new TrailRenderer(TrailTex3, TrailRenderer.DefaultPass, (p) => new Vector2(40f), (p) => new Color(50, 50, 50, 30) * (1f - p));
				SwordSlash4.drawOffset = Projectile.Size / 2.2f;

			}
			Main.spriteBatch.Begin(SpriteSortMode.Texture, null, null, null, null, null, Main.GameViewMatrix.ZoomMatrix);


			float[] rotation = new float[Projectile.oldRot.Length];
			for (int i = 0; i < rotation.Length; i++)
			{
				rotation[i] = Projectile.oldRot[i] + MathHelper.ToRadians(45);
			}


			SwordSlash.Draw(Projectile.oldPos, rotation);
			SwordSlash2.Draw(Projectile.oldPos, rotation);
			SwordSlash3.Draw(Projectile.oldPos, rotation);
			SwordSlash4.Draw(Projectile.oldPos, rotation);


			if (Projectile.timeLeft > 2)
				return false;

			Texture2D projTexture = TextureAssets.Projectile[Projectile.type].Value;
			Texture2D glowTexture = ModContent.Request<Texture2D>(Texture + "_Glow").Value;

			//End control point for the chain
			Vector2 projBottom = Projectile.Center + new Vector2(0, projTexture.Height / 2).RotatedBy(Projectile.rotation + MathHelper.PiOver2) * 0.75f;
			DrawChainCurve(Main.spriteBatch, projBottom, out Vector2[] chainPositions);

			//Adjust rotation to face from the last point in the bezier curve
			float newRotation = (projBottom - chainPositions[chainPositions.Length - 2]).ToRotation() + MathHelper.PiOver2;

			//Draw from bottom center of texture
			Vector2 origin = new Vector2(projTexture.Width / 2, projTexture.Height);
			SpriteEffects flip = (Projectile.spriteDirection < 0) ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

			lightColor = Lighting.GetColor((int)(Projectile.Center.X / 16f), (int)(Projectile.Center.Y / 16f));

			Main.spriteBatch.Draw(projTexture, projBottom - Main.screenPosition, null, lightColor, newRotation, origin, Projectile.scale, flip, 0);
			Main.spriteBatch.Draw(glowTexture, projBottom - Main.screenPosition, null, Color.White, newRotation, origin, Projectile.scale, flip, 0);

			CurrentBase = projBottom + (newRotation - 1.57f).ToRotationVector2() * (projTexture.Height / 2);

			oldBase.Add(projBottom - Main.screenPosition);

			if (oldBase.Count > 8)
				oldBase.RemoveAt(0);

			if (!Slam)
				return false;

			Texture2D whiteTexture = ModContent.Request<Texture2D>(Texture + "_White").Value;
			if (slamTimer < 20 && slamTimer > 5)
			{
				float progress = (slamTimer - 5) / 15f;
				float transparency = (float)Math.Pow(1 - progress, 2);
				float scale = 1 + progress;
				Main.spriteBatch.Draw(whiteTexture, projBottom - Main.screenPosition, null, Color.White * transparency, newRotation, origin, Projectile.scale * scale, flip, 0);
			}
			return false;
		}

		//Control points for drawing chain bezier, update slowly when hooked in
		private Vector2 _chainMidA;
		private Vector2 _chainMidB;
		private void DrawChainCurve(SpriteBatch spriteBatch, Vector2 projBottom, out Vector2[] chainPositions)
		{
			Texture2D chainTex = ModContent.Request<Texture2D>(Texture + "_Chain").Value;

			float progress = Timer / SwingTime;

			if (Slam)
				progress = EaseFunction.EaseCubicInOut.Ease(progress);
			else
				progress = EaseFunction.EaseQuadOut.Ease(progress);

			float angleMaxDeviation = MathHelper.Pi * 0.85f;
			float angleOffset = Owner.direction * (Flip ? -1 : 1) * MathHelper.Lerp(angleMaxDeviation, -angleMaxDeviation / 4, progress);

			_chainMidA = Owner.MountedCenter + GetSwingPosition(progress).RotatedBy(angleOffset) * Curvature;
			_chainMidB = Owner.MountedCenter + GetSwingPosition(progress).RotatedBy(angleOffset / 2) * Curvature * 2.5f;

			Curvature curve = new Curvature(new Vector2[] { Owner.MountedCenter, _chainMidA, _chainMidB, projBottom });

			int numPoints = 30;
			chainPositions = curve.GetPoints(numPoints).ToArray();

			//Draw each chain segment, skipping the very first one, as it draws partially behind the player
			for (int i = 1; i < numPoints; i++)
			{
				Vector2 position = chainPositions[i];

				float rotation = (chainPositions[i] - chainPositions[i - 1]).ToRotation() - MathHelper.PiOver2; //Calculate rotation based on direction from last point
				float yScale = Vector2.Distance(chainPositions[i], chainPositions[i - 1]) / chainTex.Height; //Calculate how much to squash/stretch for smooth chain based on distance between points

				Vector2 scale = new Vector2(1, yScale); // Stretch/Squash chain segment
				Color chainLightColor = Lighting.GetColor((int)position.X / 16, (int)position.Y / 16); //Lighting of the position of the chain segment
				Vector2 origin = new Vector2(chainTex.Width / 2, chainTex.Height); //Draw from center bottom of texture
				spriteBatch.Draw(chainTex, position - Main.screenPosition, null, chainLightColor, rotation, origin, scale, SpriteEffects.None, 0);
			}
		}

		public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
		{
			Curvature curve = new Curvature(new Vector2[] { Owner.MountedCenter, _chainMidA, _chainMidB, Projectile.Center });

			int numPoints = 32;
			Vector2[] chainPositions = curve.GetPoints(numPoints).ToArray();
			float collisionPoint = 0;
			for (int i = 1; i < numPoints; i++)
			{
				Vector2 position = chainPositions[i];
				Vector2 previousPosition = chainPositions[i - 1];
				if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), position, previousPosition, 6, ref collisionPoint))
					return true;
			}
			return base.Colliding(projHitbox, targetHitbox);
		}



		public override void SendExtraAI(BinaryWriter writer)
		{
			writer.Write(SwingTime);
			writer.Write(SwingDistance);
			writer.WriteVector2(returnPosOffset);
			writer.WriteVector2(npcHookOffset);
			writer.Write(npcHookRotation);
			writer.Write(Flip);
			writer.Write(Slam);
			writer.Write(Curvature);

			if (hookNPC == default(NPC)) //Write a -1 instead if the npc isnt set
				writer.Write(-1);
			else
				writer.Write(hookNPC.whoAmI);
		}

		public override void ReceiveExtraAI(BinaryReader reader)
		{
			SwingTime = reader.ReadInt32();
			SwingDistance = reader.ReadSingle();
			returnPosOffset = reader.ReadVector2();
			npcHookOffset = reader.ReadVector2();
			npcHookRotation = reader.ReadSingle();
			Flip = reader.ReadBoolean();
			Slam = reader.ReadBoolean();
			Curvature = reader.ReadSingle();

			int whoAmI = reader.ReadInt32(); //Read the whoami value sent
			if (whoAmI == -1) //If its a -1, sync that the npc hasn't been set yet
				hookNPC = default;
			else
				hookNPC = Main.npc[whoAmI];
		}
	}
}
