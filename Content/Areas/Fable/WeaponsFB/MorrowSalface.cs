using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Core.Bases;
using Stellamod.Core.Shaders;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Items;
using Stellamod.Items.Materials;
using Stellamod.Items.Materials.Molds;
using Stellamod.Trails;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Fable.WeaponsFB
{
    public class MorrowSalface : BaseCrossbowItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.damage = 13;
        }

        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew(
                mold: ModContent.ItemType<BlankBow>(),
                material: ModContent.ItemType<AlcadizScrap>());
        }

        public override void ShootBow(Player player, EntitySource_ItemUse_WithAmmo source, ShootParams shootParams)
        {
            Vector2 fireVelocity = shootParams.velocity * shootParams.speed;
            fireVelocity *= 3;
            fireVelocity *= shootParams.chargeStrength;
            if (shootParams.projToShoot == ProjectileID.WoodenArrowFriendly)
                shootParams.projToShoot = ModContent.ProjectileType<MorrowShotArrow>();
            float bowDamage = shootParams.damage * shootParams.chargeStrength;
            Projectile crossShot = Projectile.NewProjectileDirect(source, shootParams.position, fireVelocity,
                shootParams.projToShoot, (int)bowDamage, shootParams.knockBack, player.whoAmI, ai0: shootParams.projToShoot);
            crossShot.GetGlobalProjectile<CrossbowGlobalProjectile>().CrossbowShot = true;
        }

        public override void StaminaShootBow(Player player, EntitySource_ItemUse_WithAmmo source, ShootParams shootParams)
        {
            base.StaminaShootBow(player, source, shootParams);
            float bowDamage = shootParams.damage * shootParams.chargeStrength;
            Vector2 position = shootParams.position;
            Vector2 velocity = shootParams.velocity * shootParams.chargeStrength * 4;
            FunctionRepeatHelper.Repeat(() =>
            {
                Projectile crossShot = Projectile.NewProjectileDirect(source, position, velocity, ModContent.ProjectileType<MorrowShot>(), (int)bowDamage, 0, player.whoAmI);
            //    crossShot.GetGlobalProjectile<CrossbowGlobalProjectile>().CrossbowShot = true;
            }, repeats: 3, rate: 4);
        }
    }






    public class MorrowShotArrow : ModProjectile
    {
        private ref float Timer => ref Projectile.ai[0];
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 1;
            ProjectileID.Sets.TrailCacheLength[Type] = 8;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.light = 1.5f;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.maxPenetrate = 1;
        }

        public override void AI()
        {
            base.AI();
            Timer++;
            if (Timer % 6 == 0)
            {
                if (Main.rand.NextBool(2))
                    Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<SalfaceDust>(), Vector2.Zero, newColor: Color.White, Scale: Main.rand.NextFloat(0.5f, 1f));
                else
                    Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlyphDust>(), Vector2.Zero, newColor: Color.CadetBlue, Scale: Main.rand.NextFloat(0.5f, 1f));
            }

            float maxDetectDistance = 196;
            NPC closest = ProjectileHelper.FindNearestEnemy(Projectile.position, maxDetectDistance);
            if (closest != null)
            {
                Projectile.velocity = ProjectileHelper.SimpleHomingVelocity(Projectile, closest.Center, 3);
            }

            Projectile.velocity *= 1.005f;
            Projectile.rotation = Projectile.velocity.ToRotation();
        }


        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, new Vector2(texture.Width / 2, texture.Height / 2), 1f, Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
            return false;
        }

        public override void PostDraw(Color lightColor)
        {
            Texture2D texture2D4 = ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/DimLight").Value;
            Color glowColor = Color.CadetBlue;
            glowColor.A = 0;
            glowColor *= Timer / 30f;
            for (int i = 0; i < 3; i++)
            {
                Main.spriteBatch.Draw(texture2D4, Projectile.Center - Main.screenPosition, null, glowColor, Projectile.rotation, new Vector2(32, 32), 0.17f * (7 + 0.6f), SpriteEffects.None, 0f);
            }
        }

        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
            FXUtil.GlowCircleBoom(Projectile.Center,
                innerColor: Color.White,
                glowColor: Color.CadetBlue,
                outerGlowColor: Color.Black, duration: 25, baseSize: Main.rand.NextFloat(0.06f, 0.12f));

            for (float i = 0; i < 8; i++)
            {
                float progress = i / 4f;
                float rot = progress * MathHelper.ToRadians(360);
                rot += Main.rand.NextFloat(-0.5f, 0.5f);
                Vector2 offset = rot.ToRotationVector2() * 24;
                var particle = FXUtil.GlowCircleDetailedBoom1(Projectile.Center,
                    innerColor: Color.White,
                    glowColor: Color.CadetBlue,
                    outerGlowColor: Color.Black,
                    baseSize: Main.rand.NextFloat(0.04f, 0.12f),
                    duration: Main.rand.NextFloat(15, 25));
                particle.Rotation = rot + MathHelper.ToRadians(45);
            }
        }
    }







    public class MorrowShot : ModProjectile
    {
        private ref float Timer => ref Projectile.ai[0];
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.friendly = true;
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.extraUpdates = 3;
            Projectile.timeLeft = 300;
            Projectile.penetrate = 3;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
        }

        public override void AI()
        {
            base.AI();
            Timer++;
            if (Timer % 3 == 0)
            {
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowSparkleDust>(), newColor: Color.White, Scale: Main.rand.NextFloat(0.25f, 0.5f));
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlyphDust>(), newColor: Color.White, Scale: Main.rand.NextFloat(0.5f, 1f));
            }
            if (Timer < 100)
            {
                if (Projectile.velocity.Length() < 15)
                    Projectile.velocity *= 1.05f;
            }

            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;

            //Setup the shader
            MotionBlurShader shader = MotionBlurShader.Instance;
            float maxSpeed = 0.4f;
            float speed = MathHelper.Clamp(Projectile.velocity.Length() * 0.02f, 0f, maxSpeed);

            //This is gonna make it like stretch itself as it moves faster
            Vector2 scale = Vector2.Lerp(Vector2.One, new Vector2(2f, 0.18f), Easing.InOutCubic(speed));

            shader.Velocity = Vector2.UnitY * speed;

            //This just affects the opacity of the blur, prob don't need to change this number
            shader.BlurStrength = 2f;
            shader.Apply();

            //Draw the texture
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            Rectangle frame = Projectile.Frame();
            Vector2 drawOrigin = frame.Size() / 2f;

            float rotation = Projectile.rotation;
            Color finalColor = Color.White.MultiplyRGB(lightColor);
            spriteBatch.Draw(texture, drawPos, frame, finalColor, rotation, drawOrigin, scale, SpriteEffects.None, 0);

            //Draw the blurring on top
            spriteBatch.Restart(effect: shader.Effect);
            spriteBatch.Draw(texture, drawPos, frame, finalColor * 0.5f, rotation, drawOrigin, scale, SpriteEffects.None, 0);
            spriteBatch.RestartDefaults();
            return false;
        }


        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
            for (float f = 0; f < 3; f++)
            {
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowSparkleDust>(), Velocity: Projectile.oldVelocity.RotatedByRandom(MathHelper.ToRadians(30)) * Main.rand.NextFloat(0.3f, 0.6f), newColor: Color.White, Scale: Main.rand.NextFloat(0.25f, 0.5f));
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlyphDust>(), Velocity: Projectile.oldVelocity.RotatedByRandom(MathHelper.ToRadians(30)) * Main.rand.NextFloat(0.3f, 0.6f), newColor: Color.White, Scale: Main.rand.NextFloat(0.5f, 1f));
            }
            FXUtil.GlowCircleBoom(Projectile.Center, Color.White, Color.LightBlue, Color.Black);
        }
    }
}
