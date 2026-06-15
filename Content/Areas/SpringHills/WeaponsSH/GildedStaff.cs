using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Core.Bases;
using Stellamod.Core.Pixelation;
using Stellamod.Dusts;
using Stellamod.Helpers;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.SpringHills.WeaponsSH
{
    public class GildedStaff : ModItem
    {
        public override void SetDefaults()
        {
            Item.DefaultToArtifact();
            Item.damage = 13;
            Item.mana = 50;
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

            float rotOffset = 90f;
            if(Owner.direction == -1)
            {
                rotOffset += 90;
            }


            Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - MathHelper.ToRadians(rotOffset)); // set arm position (90 degree offset since arm starts lowered)
            Vector2 armPosition = Owner.GetFrontHandPosition(Player.CompositeArmStretchAmount.Full, Projectile.rotation - MathHelper.ToRadians(rotOffset)); // get position of hand

            armPosition.Y += Owner.gfxOffY;
            Projectile.Center = armPosition; // Set projectile to arm position
           // Owner.heldProj = Projectile.whoAmI;
            if (Projectile.spriteDirection == -1)
            {
                // Projectile.rotation += MathHelper.ToRadians(90);
            }


        }

        private void AI_Charge()
        {
            Timer++;
            if (Timer == 1)
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
            if (Timer == MaxChargeTime)
            {
                for (float f = 0; f < 7; f++)
                {
                    if (Main.rand.NextBool(2))
                    {
                        Dust.NewDustPerfect(EndPoint, ModContent.DustType<GlowSparkleDust>(), (Vector2.One * Main.rand.NextFloat(0.2f, 0.4f)).RotatedByRandom(19.0), 0, Color.White, Main.rand.NextFloat(0.5f, 1f)).noGravity = true;
                    }
                    else
                    {
                        Dust.NewDustPerfect(EndPoint, ModContent.DustType<GlyphDust>(), (Vector2.One * Main.rand.NextFloat(0.2f, 0.4f)).RotatedByRandom(19.0), 0, Color.White, Main.rand.NextFloat(0.5f, 1f)).noGravity = true;
                    }
                }
            }
            else if (Timer < MaxChargeTime)
            {
                if (Timer % 5 == 0)
                {
                    Vector2 spawnPos = EndPoint + Main.rand.NextVector2CircularEdge(64, 64);
                    Vector2 vel = (EndPoint - spawnPos).SafeNormalize(Vector2.Zero) * 4;
                    Dust.NewDustPerfect(spawnPos, ModContent.DustType<GlyphDust>(), vel, newColor: Color.White, Scale: Main.rand.NextFloat(0.25f, 0.66f));
                }
            }
            ChargeProgress = Timer / MaxChargeTime;
            ChargeProgress = MathHelper.Clamp(ChargeProgress, 0, 1);
            if (Main.myPlayer == Projectile.owner)
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
            if (Timer == 1)
            {
                if (Main.myPlayer == Projectile.owner)
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Owner.Center, Projectile.velocity,
                        ModContent.ProjectileType<GildedStaffBlast>(), (int)(Projectile.damage * ChargeProgress * 3f), Projectile.knockBack, Projectile.owner, ai1: ChargeProgress);
                }
                FXUtil.ShakeCamera(Projectile.position, 1024, 2);

            }
            if (Timer >= 4)
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

            Color glowColor = drawColor * ChargeProgress;
            glowColor.A = 0;
            spriteBatch.Draw(texture, drawPos + Projectile.velocity * 24, null, glowColor, drawRotation, drawOrigin, drawScale, spriteEffects, 0);
        }

        private void DrawPixelatedEnergyBall(SpriteBatch sb, Vector2 sp)
        {
            //Draw Code for the orb
            Texture2D texture = ModContent.Request<Texture2D>(TextureRegistry.EmptyGlowParticle).Value;
            Vector2 centerPos = Projectile.Center;
            Vector2 ballDrawPosition = centerPos + Projectile.velocity * 64;
            SpritebatchDrawer glowballDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.SimpleGlowCircle, ballDrawPosition);
            glowballDrawer.color = Color.Lerp(Color.Black, Color.White, EasingFunction.InOutSine(Timer / 30f));
            glowballDrawer.scale *= 0.5f * MathHelper.Lerp(0.2f, 0.4f, Timer / 30f) * 0.2f;
            glowballDrawer.color.A = 0;
            Main.spriteBatch.Draw(glowballDrawer);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            DrawStaff(ref lightColor);
            if (State == AIState.Charge)
            {
                PixelationManager.QueueSpritebatchDrawAction(DrawPixelatedEnergyBall);
            }
            return false;
        }
    }

    public class GildedStaffBlast : ModProjectile,
        IDrawToRenderTarget
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
            if (Timer == 1)
            {
                for (int i = 0; i < 7 * Charge; i++)
                {
                    Vector2 velocity = Projectile.velocity.RotatedByRandom(MathHelper.ToRadians(30)) * Main.rand.NextFloat(25f, 45f);
                    var particle = FXUtil.GlowStretch(Projectile.Center, velocity);
                    particle.InnerColor = Color.White;
                    particle.GlowColor = Color.LightCyan;
                    particle.OuterGlowColor = Color.Black;
                    particle.Duration = Main.rand.NextFloat(25, 50) * Charge;
                    particle.BaseSize = Main.rand.NextFloat(0.09f, 0.18f) * Charge;
                    particle.VectorScale *= 0.5f;
                }

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
            if (nearest != null)
            {
                Projectile.velocity = ProjectileHelper.SimpleHomingVelocity(Projectile, nearest.Center, 1);
            }
            Projectile.velocity *= 1.01f;
        }

        private void DrawPixelatedEnergyBall(SpriteBatch sb, Vector2 sp)
        {
            //Draw Code for the orb
            Texture2D texture = ModContent.Request<Texture2D>(TextureRegistry.EmptyGlowParticle).Value;
            Vector2 centerPos = Projectile.Center;
            Vector2 ballDrawPosition = centerPos;
            for(int i = 0; i < 4; i++)
            {
                SpritebatchDrawer glowballDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.SimpleGlowCircle, ballDrawPosition);
                glowballDrawer.color = Color.Lerp(Color.Black, Color.White, Charge);
                glowballDrawer.scale *= MathHelper.Lerp(0.2f, 0.5f, Charge) * 0.1f;
                glowballDrawer.color.A = 0;
                Main.spriteBatch.Draw(glowballDrawer);
            }

        }

        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
            for (float f = 0; f < 1 + MathHelper.Lerp(0, 5, Charge); f++)
            {
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlyphDust>(),
                    (Vector2.One * Main.rand.NextFloat(0.2f, 5f)).RotatedByRandom(19.0), 0, Color.White, Main.rand.NextFloat(1f, 3f)).noGravity = true;
            }
            for (int i = 0; i < 3 * Charge; i++)
            {
                //Old velocity is the velocity before this tick, so it won't be zero or whatever
                Vector2 velocity = Projectile.oldVelocity.RotatedByRandom(MathHelper.ToRadians(30)).SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(5, 15);

                //I love this particle type
                var particle = FXUtil.GlowStretch(Projectile.Center, velocity);
                particle.InnerColor = Color.White;
                particle.GlowColor = Color.LightCyan;
                particle.OuterGlowColor = Color.Black;
                particle.Duration = Main.rand.NextFloat(25, 50) * Charge;
                particle.BaseSize = Main.rand.NextFloat(0.09f, 0.18f) * Charge;
                particle.VectorScale *= 0.5f;
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
        private void DrawPixelatedEnergyTrail(GraphicsDevice gDevice)
        {
            var shader2 = RichLaserShader.Instance;
            shader2.LaserColor = Color.White;
            shader2.LaserTexture = TrailRegistry.StarTrail;
            shader2.InnerColor = Color.LightGray * 0.5f;
            shader2.OuterColor = Color.DarkGray;
            TrailDrawer.Draw(Main.spriteBatch, Projectile.oldPos, ColorFunction, WidthFunction, shader2, Projectile.Size * 0.5f);

            var bloom = BloomTrailShader.Instance;
            bloom.InnerColor = Color.LightGray * 0.5f;
            bloom.OuterColor = Color.DarkGray;
            TrailDrawer.Draw(Main.spriteBatch, Projectile.oldPos, ColorFunction, WidthFunction2, bloom, Projectile.Size * 0.5f);
        }

        private Color ColorFunction(float completionRatio)
        {
            Color inColor = Color.White;
            Color trailColor = Color.Lerp(Color.White, Color.Black, completionRatio);
            Color easeColor = Color.Lerp(inColor, trailColor, EasingFunction.InExpo(Timer / 60f));
            return easeColor;
        }

        private float WidthFunction(float completionRatio)
        {
            return MathHelper.SmoothStep(10, 2, completionRatio);
        }

        private float WidthFunction2(float completionRatio)
        {
            return WidthFunction(completionRatio) * 2f;
        }


        public void DrawToRenderTargets()
        {
            PixelationManager.QueuePrimitivesDrawAction(DrawPixelatedEnergyTrail);
            PixelationManager.QueueSpritebatchDrawAction(DrawPixelatedEnergyBall);
        }
    }
}









