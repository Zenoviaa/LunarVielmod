using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Assets;
using Stellamod.Content.CommonMaterials;
using Stellamod.Core.Bases;
using Stellamod.Helpers;
using Stellamod.Items;
using Stellamod.Trails;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Fable.WeaponsFB
{
    public class JackoShot : BaseCrossbowItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.damage = 14;
        }


        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-2f, 0f);
        }
        public override void StaminaShootBow(Player player, EntitySource_ItemUse_WithAmmo source, ShootParams shootParams)
        {
            base.StaminaShootBow(player, source, shootParams);
            float bowDamage = shootParams.damage * shootParams.chargeStrength * 2;
            Vector2 bulletVelocity = shootParams.velocity * shootParams.chargeStrength * 32;
            Projectile.NewProjectile(source, shootParams.position, bulletVelocity,
                ModContent.ProjectileType<JackoShotBombArrow>(), (int)bowDamage, shootParams.knockBack, player.whoAmI);
        }

        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew<AlcadizScrap, BlankBow>();
        }

    }









    public class JackoShotBombArrow : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8; // The length of old position to be recorded
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2; // The recording mode
        }

        public override void SetDefaults()
        {
            Projectile.width = 32; // The width of projectile hitbox
            Projectile.height = 20; // The height of projectile hitbox
            Projectile.friendly = true; // Can the projectile deal damage to enemies?
            Projectile.hostile = false; // Can the projectile deal damage to the player?
            Projectile.DamageType = DamageClass.Ranged; // Is the projectile shoot by a ranged weapon?
            Projectile.timeLeft = 600; // The live time for the projectile (60 = 1 second, so 600 is 10 seconds)
            Projectile.light = 0.5f; // How much light emit around the projectile
            Projectile.ignoreWater = true; // Does the projectile's speed be influenced by water?
            Projectile.tileCollide = true; // Can the projectile collide with tiles?
            AIType = 1;
        }

        public override void AI()
        {
            base.AI();
            Projectile.rotation = Projectile.velocity.ToRotation();
            Lighting.AddLight(Projectile.Center, Color.Yellow.ToVector3() * 0.78f * Main.essScale);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);


        }

        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
            Vector2 velocity = Main.rand.NextVector2Circular(16, 16);
            for (int i = 0; i < 3; i++)
            {
                float progress = (float)i / 3f;
                float rot = progress * MathHelper.TwoPi;
                Vector2 startVel = Vector2.UnitY.RotatedBy(rot);
                startVel *= 8;
                startVel.Y -= 16;
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, startVel,
                    ModContent.ProjectileType<JackoShotBombArrowFire>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
            }
            SoundEngine.PlaySound(SoundID.DD2_BetsysWrathImpact, Projectile.position);
        }


        public float WidthFunction(float completionRatio)
        {
            float baseWidth = Projectile.scale * Projectile.width * 1.2f;
            return MathHelper.SmoothStep(baseWidth, 3.5f, completionRatio);
        }

        public Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(Color.LightGoldenrodYellow * 0.1361f, Color.Transparent, completionRatio);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            DrawHelper.DrawSimpleTrail(Projectile, WidthFunction, ColorFunction, TrailRegistry.StarTrail);
            SpriteBatch spriteBatch = Main.spriteBatch;
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawOrigin = texture.Size() / 2f;
            Color drawColor = Color.White.MultiplyRGB(lightColor);
            float drawRotation = Projectile.rotation;
            float drawScale = 1f;

            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            spriteBatch.Draw(texture, drawPos, Projectile.Frame(), drawColor, drawRotation, Projectile.Frame().Size() / 2f, drawScale, SpriteEffects.None, 0);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            for (int i = 0; i < 4; i++)
            {
                float rot = (float)i / 4f;
                Vector2 vel = rot.ToRotationVector2() * VectorHelper.Osc(0f, 4f, speed: 16);
                Vector2 flameDrawPos = drawPos + vel + Main.rand.NextVector2Circular(2, 2);
                flameDrawPos -= Vector2.UnitY * 4;
                spriteBatch.Draw(texture, flameDrawPos, Projectile.Frame(), drawColor, drawRotation, Projectile.Frame().Size() / 2f, drawScale, SpriteEffects.None, 0);
            }

            for (int i = 0; i < 4; i++)
            {
                Vector2 flameDrawPos = drawPos + Main.rand.NextVector2Circular(2, 2);
                spriteBatch.Draw(texture, flameDrawPos, Projectile.Frame(), drawColor, drawRotation, Projectile.Frame().Size() / 2f, drawScale, SpriteEffects.None, 0);
            }

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            return false;
        }
    }









    public class JackoShotBombArrowFire : ModProjectile
    {
        private ref float Timer => ref Projectile.ai[0];

        private float _scale;
        private Vector2 InitialVelocity;
        private Vector2 TargetVelocity;

        public override void SendExtraAI(BinaryWriter writer)
        {
            base.SendExtraAI(writer);
            writer.WriteVector2(InitialVelocity);
            writer.WriteVector2(TargetVelocity);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            base.ReceiveExtraAI(reader);
            InitialVelocity = reader.ReadVector2();
            TargetVelocity = reader.ReadVector2();
        }

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            ProjectileID.Sets.TrailCacheLength[Type] = 16;
            ProjectileID.Sets.TrailingMode[Type] = 2;
            Main.projFrames[Type] = 4;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.friendly = false;
            Projectile.light = 0.278f;
            Projectile.timeLeft = 180;
            Projectile.tileCollide = false;
            Projectile.penetrate = 1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }


        public override void AI()
        {
            base.AI();
            Timer++;
            if (Timer == 1)
            {
                InitialVelocity = Projectile.velocity;
                SoundEngine.PlaySound(SoundID.DD2_BetsyFireballShot, Projectile.position);
            }
            if (Timer % 12 == 0)
            {
                Vector2 vel = Vector2.Zero;
                Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.Torch, vel, Scale: 1);
                d.noGravity = true;
            }
            if (Timer % 6 == 0)
            {
                Vector2 vel = Vector2.Zero;
                Dust d = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(8, 8), DustID.Torch, vel, Scale: 1);
                d.noGravity = true;
            }
            if (Timer <= 15)
            {
                _scale = MathHelper.Lerp(0f, Main.rand.NextFloat(0.15f, 0.66f), Easing.InCubic(Timer / 5f));
                Projectile.velocity *= 0.75f;
            }

            if (Timer == 5)
            {
                //Ping Sound
                SoundStyle soundStyle = new SoundStyle("Stellamod/Assets/Sounds/Jack_FirePing");
                soundStyle.PitchVariance = 0.1f;
                SoundEngine.PlaySound(soundStyle, Projectile.position);
            }

            if (Timer > 60)
            {
                Projectile.friendly = true;
                float maxHomingDistance = 512;
                NPC npcToChase = ProjectileHelper.FindNearestEnemy(Projectile.Center, maxHomingDistance);

                if (npcToChase != null)
                {
                    Projectile.velocity = Projectile.velocity.MoveTowards((npcToChase.Center-Projectile.Center).SafeNormalize(Vector2.Zero) * 10, 0.5f);
                    Projectile.velocity *= 1.05f;
                }


            }

            Projectile.rotation = Projectile.velocity.X * 0.05f;
            DrawHelper.AnimateTopToBottom(Projectile, 4);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            Projectile.Kill();
        }

        public override bool PreDraw(ref Color lightColor)
        {
            DrawUtilities.DrawSpriteAfterImage(Main.spriteBatch, Projectile, Color.Red, Color.Transparent, alpha: 0.05f);
            SpritebatchDrawer flameDrawer = SpritebatchDrawer.FromProjectile(Projectile);
            Main.spriteBatch.Draw(flameDrawer);

            SpritebatchDrawer glintDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.ShootingStarGlint, Projectile.Center);
            glintDrawer.color = Color.Lerp(Color.Yellow, Color.Red, ExtraMath.Osc(0f, 1f, speed: 16));
            glintDrawer.color.A = 0;
            glintDrawer.rotation = MathHelper.Lerp(1.54f, 0f, EasingFunction.InOutCirc(Timer / 30f));
            glintDrawer.scale *= MathHelper.Lerp(6, 0f, EasingFunction.InOutSine(Timer / 60f));
            Main.spriteBatch.Draw(glintDrawer);
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 24; i++)
            {
                int num = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.FlameBurst, 0f, -2f, 0, default(Color), 1.5f);
                Dust dust = Main.dust[num];
                dust.noGravity = true;
                dust.position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
                dust.position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
                dust.velocity = Projectile.DirectionTo(dust.position) * 6f;
            }

            SoundEngine.PlaySound(SoundID.DD2_BetsyFireballImpact, Projectile.position);
        }

        public override void PostDraw(Color lightColor)
        {
            base.PostDraw(lightColor);
            Texture2D dimLightTexture = ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/DimLight").Value;
            float drawScale = 1f;
            SpriteBatch spriteBatch = Main.spriteBatch;
            for (int i = 0; i < 3; i++)
            {
                Color glowColor = new Color(85, 45, 15) * 0.5f;
                glowColor.A = 0;
                spriteBatch.Draw(dimLightTexture, Projectile.Center - Main.screenPosition, null, glowColor,
                    Projectile.rotation, dimLightTexture.Size() / 2f, drawScale * VectorHelper.Osc(0.75f, 1f, speed: 32, offset: Projectile.whoAmI), SpriteEffects.None, 0f);
            }
        }
    }
}
