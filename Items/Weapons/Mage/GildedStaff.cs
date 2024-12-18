using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Common.Shaders;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Items.Materials;
using Stellamod.Items.Weapons.Ranged.GunSwapping;
using Stellamod.Projectiles;
using Stellamod.Trails;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Mage
{
    internal class GildedStaff : ClassSwapItem
    {
        public int dir;
        public override DamageClass AlternateClass => DamageClass.Ranged;

        public override void SetClassSwappedDefaults()
        {
            Item.damage = 6;
            Item.mana = 6;
        }
        public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Gilded Staff");
			// Tooltip.SetDefault("Shoots two spinning pieces of spiritual magic at your foes!\nThe fabric is super magical, it turned wood into something like a flamethrower! :>");
		}
		public override void SetDefaults()
		{
			Item.damage = 15;
			Item.mana = 5;
			Item.width = 40;
			Item.height = 40;
			Item.useTime = 32;
			Item.useAnimation = 32;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.staff[Item.type] = true;
			Item.noMelee = true;
            Item.noUseGraphic = true;
			Item.knockBack = 2f;
			Item.DamageType = DamageClass.Magic;
			Item.value = Item.sellPrice(silver: 10);
			Item.rare = ItemRarityID.Blue;
	
	
			Item.shoot = ModContent.ProjectileType<GildedStaffHold>();
			Item.shootSpeed = 8f;
            Item.channel = true;
			Item.autoReuse = false;
			Item.crit = 22;
		}
        public override bool CanUseItem(Player player)
        {
            return player.ownedProjectileCounts[Item.shoot] < 1;
        }

    }

	public class GildedStaffHold : ModProjectile
	{
		private enum AIState
		{
			Charge,
			Release
		}
		private AIState State
		{
			get => (AIState)Projectile.ai[0];
			set => Projectile.ai[0] = (float)value;
		}

		private float MaxChargeTime => 60;

		private ref float Timer => ref Projectile.ai[1];
		private ref float ChargeProgress => ref Projectile.ai[2];
		public override string Texture => this.PathHere() + "/GildedStaff";
		private Player Owner => Main.player[Projectile.owner];
        private Vector2 EndPoint => Projectile.Center + Projectile.velocity * 64;
        public override void SetDefaults()
        {
            base.SetDefaults();
			Projectile.width = 8;
			Projectile.height = 8;
			Projectile.penetrate = -1;
			Projectile.tileCollide = false;
			Projectile.friendly = false;
			Projectile.timeLeft = int.MaxValue;
        }

        public override void AI()
        {
            base.AI();
			switch (State)
			{
				case AIState.Charge:
					AI_Charge();
					break;
				case AIState.Release:
					AI_Release();
					break;
			}


            Owner.heldProj = Projectile.whoAmI;
            Owner.itemTime = 2;
            Owner.itemAnimation = 2;
            Owner.itemRotation = (float)Math.Atan2(Projectile.velocity.Y * Projectile.direction, Projectile.velocity.X * Projectile.direction);

        }

        private void SwitchState(AIState state)
		{
			State = state;
			Timer = 0;
			Projectile.netUpdate = true;
		}

		private void SetHoldPosition()
        {
            if (Main.myPlayer == Projectile.owner)
            {
               // Projectile.spriteDirection = (int)Main.MouseWorld.X > Owner.MountedCenter.X ? 1 : -1;
    
                Projectile.netUpdate = true;
            }

      
            if (Main.myPlayer == Projectile.owner)
            {
                Owner.direction = Main.MouseWorld.X > Owner.MountedCenter.X ? 1 : -1;
            }

            Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - MathHelper.ToRadians(90f)); // set arm position (90 degree offset since arm starts lowered)
            Vector2 armPosition = Owner.GetFrontHandPosition(Player.CompositeArmStretchAmount.Full, Projectile.rotation - (float)Math.PI / 2); // get position of hand

            armPosition.Y += Owner.gfxOffY;
            Projectile.Center = armPosition; // Set projectile to arm position
            Owner.heldProj = Projectile.whoAmI;
            if (Projectile.spriteDirection == -1)
            {
               // Projectile.rotation += MathHelper.ToRadians(90);
            }


        }

        private void AI_Charge()
		{
            Timer++;
            if(Timer == 1)
            {
                SoundStyle mySound = new SoundStyle("Stellamod/Assets/Sounds/StormKnight_Rechage");
                mySound.PitchVariance = 0.3f;
                SoundEngine.PlaySound(mySound, Projectile.position);

            }
            if (Main.myPlayer == Projectile.owner)
            {

                Projectile.velocity = Owner.Center.DirectionTo(Main.MouseWorld);
                Projectile.netUpdate = true;
            }
            if(Timer == MaxChargeTime)
			{
                for (float f = 0; f < 7; f++)
                {
					if (Main.rand.NextBool(2))
					{
                        Dust.NewDustPerfect(EndPoint, ModContent.DustType<GlowSparkleDust>(), (Vector2.One * Main.rand.NextFloat(0.2f, 0.4f)).RotatedByRandom(19.0), 0, Color.White, Main.rand.NextFloat(0.5f, 1f)).noGravity = true;
                    }
					else
					{
                        Dust.NewDustPerfect(EndPoint, ModContent.DustType<GlyphDust>(),  (Vector2.One * Main.rand.NextFloat(0.2f, 0.4f)).RotatedByRandom(19.0), 0, Color.White, Main.rand.NextFloat(0.5f, 1f)).noGravity = true;
                    }
                }
            }
            else if (Timer < MaxChargeTime)
			{
				if(Timer % 5 == 0)
				{
					Vector2 spawnPos = EndPoint + Main.rand.NextVector2CircularEdge(64, 64);
					Vector2 vel = (EndPoint - spawnPos).SafeNormalize(Vector2.Zero) * 4;
					Dust.NewDustPerfect(spawnPos, ModContent.DustType<GlyphDust>(), vel, newColor: Color.White, Scale: Main.rand.NextFloat(0.5f, 1.5f));
				}
			}
			ChargeProgress = Timer / MaxChargeTime;
			ChargeProgress = MathHelper.Clamp(ChargeProgress, 0, 1);	
			if(Main.myPlayer == Projectile.owner)
			{
				if (!Owner.channel)
				{
                    SwitchState(AIState.Release);
				}
			}
			Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(45);
            Lighting.AddLight((Projectile.Center + Projectile.velocity * 64), Color.LightCyan.ToVector3() * 1.5f);
			SetHoldPosition();
        }

		private void AI_Release()
        {
            Timer++;
			if(Timer == 1)
			{
				if(Main.myPlayer == Projectile.owner)
				{
					Projectile.NewProjectile(Projectile.GetSource_FromThis(), Owner.Center, Projectile.velocity, ModContent.ProjectileType<GildedStaffBlast>(), (int)(Projectile.damage * ChargeProgress * 2f), Projectile.knockBack, Projectile.owner, ai1: ChargeProgress);
				}
                FXUtil.ShakeCamera(Projectile.position, 1024, 2);
              
			}
            if(Timer >= 4)
            {
                Projectile.Kill();
            }
            SetHoldPosition();
        }

		private void DrawStaff(ref Color lightColor)
		{
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            SpriteBatch spriteBatch = Main.spriteBatch;
            Color drawColor = Color.White.MultiplyRGB(lightColor);
            Vector2 drawOrigin = texture.Size() / 2f;
            float drawRotation = Projectile.rotation;
            float drawScale = 1f;
            SpriteEffects spriteEffects = Projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            spriteBatch.Draw(texture, drawPos + Projectile.velocity * 24, null, drawColor, drawRotation, drawOrigin, drawScale, spriteEffects, 0);
            spriteBatch.Restart(blendState: BlendState.Additive);

            for(int i =0;i < 6; i++)
            {
                spriteBatch.Draw(texture, drawPos + Projectile.velocity * 24, null, drawColor * ChargeProgress, drawRotation, drawOrigin, drawScale, spriteEffects, 0);
            }
            spriteBatch.RestartDefaults();
        }

		private void DrawEnergyBall(ref Color lightColor)
		{
            //Draw Code for the orb
            Texture2D texture = ModContent.Request<Texture2D>(TextureRegistry.EmptyGlowParticle).Value;
            Vector2 centerPos = Projectile.Center - Main.screenPosition;
            GlowCircleShader shader = GlowCircleShader.Instance;

            //How quickly it lerps between the colors
            shader.Speed = 10f;

            //This effects the distribution of colors
            shader.BasePower = 2.5f;

            //Radius of the circle
            shader.Size = MathHelper.Lerp(0f, 0.06f, Easing.OutCubic(ChargeProgress));


            //Colors
            Color startInner = Color.White;
            Color startGlow = Color.Lerp(Color.LightGoldenrodYellow, Color.LightBlue, VectorHelper.Osc(0f, 1f, speed: 32));
            Color startOuterGlow = Color.Lerp(Color.Black, Color.Black, VectorHelper.Osc(0f, 1f, speed: 64));

            shader.InnerColor = startInner;
            shader.GlowColor = startGlow;
            shader.OuterGlowColor = startOuterGlow;

            //Idk i just included this to see how it would look
            //Don't go above 0.5;
            shader.Pixelation = 0.005f;

            //This affects the outer fade
            shader.OuterPower = 3.5f;
            shader.Apply();


            SpriteBatch spriteBatch = Main.spriteBatch;
            spriteBatch.Restart(blendState: BlendState.Additive, effect: shader.Effect);
            for (int i = 0; i < 2; i++)
            {
                spriteBatch.Draw(texture, centerPos + Projectile.velocity * 64, null, Color.White, Projectile.rotation, texture.Size() / 2f, 1f, SpriteEffects.None, 0);
            }

            spriteBatch.RestartDefaults();
            Texture2D texture2D4 = ModContent.Request<Texture2D>("Stellamod/Effects/Masks/DimLight").Value;
            Color glowColor = Color.White;
            glowColor.A = 0;
            glowColor *= Timer / 30f;
            for (int i = 0; i < 2; i++)
            {
                Main.spriteBatch.Draw(texture2D4, centerPos + Projectile.velocity * 64, null, glowColor, Projectile.rotation, new Vector2(32, 32), 0.17f * (7 + 0.6f) * ChargeProgress * VectorHelper.Osc(0.75f, 1f, speed: 3), SpriteEffects.None, 0f);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
			DrawStaff(ref lightColor);
            if(State == AIState.Charge)
			    DrawEnergyBall(ref lightColor);
            return false;
        }
    }

	public class GildedStaffBlast : ModProjectile
	{
		private ref float Timer => ref Projectile.ai[0];
        private ref float Charge => ref Projectile.ai[1];
		public override string Texture => TextureRegistry.EmptyTexture;
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
			ProjectileID.Sets.TrailCacheLength[Type] = 16;
			ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
			Projectile.width = 16;
			Projectile.height = 16;
			Projectile.friendly = true;
			Projectile.timeLeft = 240;
        }
        public override void AI()
        {
            base.AI();
			Timer++;
            if(Timer == 1)
            {
                SoundStyle mySound = new SoundStyle("Stellamod/Assets/Sounds/Starblast");
                mySound.PitchVariance = 0.3f;
                SoundEngine.PlaySound(mySound, Projectile.position);

                mySound = new SoundStyle("Stellamod/Assets/Sounds/StarFlower1");
                mySound.PitchVariance = 0.3f;
                SoundEngine.PlaySound(mySound, Projectile.position);
                Projectile.velocity *= 12;
            }
            if (Main.rand.NextBool(8))
            {
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlyphDust>(), (Vector2.One * Main.rand.NextFloat(0.2f, 5f)).RotatedByRandom(19.0), 0, Color.White, Main.rand.NextFloat(0.2f, 1f)).noGravity = true;
            }
            if (Main.rand.NextBool(8))
            {
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlyphDust>(), (Vector2.One * Main.rand.NextFloat(0.2f, 5f)).RotatedByRandom(19.0), 0, Color.LightCyan, Main.rand.NextFloat(0.2f, 1f)).noGravity = true;
            }
            NPC nearest = ProjectileHelper.FindNearestEnemy(Projectile.position, 367);
            if(nearest != null)
            {
                Projectile.velocity = ProjectileHelper.SimpleHomingVelocity(Projectile, nearest.Center, 1);
            }
            Projectile.velocity *= 1.01f;
        }


        public PrimDrawer TrailDrawer { get; private set; } = null;
        public float WidthFunction(float completionRatio)
        {
            float baseWidth = Projectile.scale * Projectile.width * MathHelper.Lerp(1f, 3f, Charge);
            return MathHelper.SmoothStep(baseWidth, 3.5f, completionRatio);
        }

        public Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(Color.White, Color.Transparent, completionRatio) * 0.7f;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D4 = ModContent.Request<Texture2D>("Stellamod/Effects/Masks/DimLight").Value;
            Color drawColor = Color.White;
            drawColor.A = 0;
            SpriteBatch spriteBatch = Main.spriteBatch;
            for (int i = 0; i < 2; i++)
                spriteBatch.Draw(texture2D4, Projectile.Center - Main.screenPosition, null, drawColor, Projectile.rotation, new Vector2(32, 32), 1f + Charge, SpriteEffects.None, 0f);

            TrailDrawer ??= new PrimDrawer(WidthFunction, ColorFunction, GameShaders.Misc["VampKnives:BasicTrail"]);
            GameShaders.Misc["VampKnives:BasicTrail"].SetShaderTexture(TrailRegistry.BeamTrail);
            TrailDrawer.DrawPrims(Projectile.oldPos, Projectile.Size * 0.5f - Main.screenPosition, 155);

            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            DrawEnergyBall(ref lightColor);
            return false;
        }

        private void DrawEnergyBall(ref Color lightColor)
        {
            //Draw Code for the orb
            Texture2D texture = ModContent.Request<Texture2D>(TextureRegistry.EmptyGlowParticle).Value;
            Vector2 centerPos = Projectile.Center - Main.screenPosition;
            GlowCircleShader shader = GlowCircleShader.Instance;

            //How quickly it lerps between the colors
            shader.Speed = 10f;

            //This effects the distribution of colors
            shader.BasePower = 2.5f;

            //Radius of the circle
            shader.Size = MathHelper.Lerp(0f, 0.09f, Charge);


            //Colors
            Color startInner = Color.White;
            Color startGlow = Color.Lerp(Color.White, Color.LightBlue, VectorHelper.Osc(0f, 1f, speed: 3f));
            Color startOuterGlow = Color.Lerp(Color.LightGoldenrodYellow, Color.Blue, VectorHelper.Osc(0f, 1f, speed: 3f));

            shader.InnerColor = startInner;
            shader.GlowColor = startGlow;
            shader.OuterGlowColor = startOuterGlow;

            //Idk i just included this to see how it would look
            //Don't go above 0.5;
            shader.Pixelation = 0.005f;

            //This affects the outer fade
            shader.OuterPower = 13.5f;
            shader.Apply();


            SpriteBatch spriteBatch = Main.spriteBatch;
            spriteBatch.Restart(blendState: BlendState.Additive, effect: shader.Effect);
            for (int i = 0; i < 2; i++)
            {
                spriteBatch.Draw(texture, centerPos, null, Color.White, Projectile.rotation, texture.Size() / 2f, 1f, SpriteEffects.None, 0);
            }

            spriteBatch.RestartDefaults();
        }

        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
            for (float f = 0; f < 1 + MathHelper.Lerp(0, 5, Charge); f++)
            {
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlyphDust>(),
                    (Vector2.One * Main.rand.NextFloat(0.2f, 5f)).RotatedByRandom(19.0), 0, Color.White, Main.rand.NextFloat(1f, 3f)).noGravity = true;
            }
            for (float i = 0; i < 4; i++)
            {
                float progress = i / 4f;
                float rot = progress * MathHelper.ToRadians(360);
                rot += Main.rand.NextFloat(-0.5f, 0.5f);
                Vector2 offset = rot.ToRotationVector2() * 24;
                var particle = FXUtil.GlowCircleDetailedBoom1(Projectile.Center,
                    innerColor: Color.White,
                    glowColor: Color.LightGoldenrodYellow,
                    outerGlowColor: Color.Black,
                    duration: Main.rand.NextFloat(6, 12),
                    baseSize: Main.rand.NextFloat(0.01f, 0.05f) * MathHelper.Lerp(1f, 2f, Charge));
                particle.Rotation = rot + MathHelper.ToRadians(45);
            }
        }
    }
}









