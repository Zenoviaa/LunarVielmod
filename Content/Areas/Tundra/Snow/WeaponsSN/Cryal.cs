using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Content.CommonMaterials;
using Stellamod.Core;
using Stellamod.Core.Bases;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Items;
using Stellamod.Projectiles.Bow;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Tundra.Snow.WeaponsSN
{
    public class Cryal : BaseCrossbowItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.damage = 6;
            Item.rare = ItemRarityID.Green;
            staminaCost = 1;
        }

        public override void ShootBow(Player player, EntitySource_ItemUse_WithAmmo source, ShootParams shootParams)
        {
            Vector2 fireVelocity = shootParams.velocity * shootParams.speed;
            fireVelocity *= 3;
            fireVelocity *= shootParams.chargeStrength;

            float bowDamage = shootParams.damage * shootParams.chargeStrength;
            Projectile crossShot1 = Projectile.NewProjectileDirect(source, shootParams.position, fireVelocity.RotatedBy(-0.06f),
                shootParams.projToShoot, (int)bowDamage, shootParams.knockBack, player.whoAmI, ai0: shootParams.projToShoot);
            crossShot1.GetGlobalProjectile<CrossbowGlobalProjectile>().isCrossbowShot = true;
            Projectile crossShot2 = Projectile.NewProjectileDirect(source, shootParams.position, fireVelocity.RotatedBy(0.06f),
                shootParams.projToShoot, (int)bowDamage, shootParams.knockBack, player.whoAmI, ai0: shootParams.projToShoot);
            crossShot2.GetGlobalProjectile<CrossbowGlobalProjectile>().isCrossbowShot = true;
        }
        public override void StaminaShootBow(Player player, EntitySource_ItemUse_WithAmmo source, ShootParams shootParams)
        {
            base.StaminaShootBow(player, source, shootParams);
            float bowDamage = shootParams.damage * shootParams.chargeStrength * 2;
            Vector2 bulletVelocity = shootParams.velocity * shootParams.chargeStrength * 32;
            Projectile.NewProjectile(source, shootParams.position, bulletVelocity, 
                ModContent.ProjectileType<IcingIc>(), (int)bowDamage, shootParams.knockBack, player.whoAmI);
        }

        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew(
                mold: ModContent.ItemType<BlankBow>(),
                material: ModContent.ItemType<WinterbornShard>());
        }
    }
    public class IcingIc : ScarletProjectile
    {
        public override string Texture => TextureRegistry.EmptyTexture;
        private ref float Timer => ref Projectile.ai[0];
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Frostburn, 180);
            Projectile.velocity *= 0.3f;
        }
        public override void SetDefaults()
        {
            TrailCacheLength = 16;
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.friendly = true;
            Projectile.extraUpdates = 1;
            Projectile.penetrate = -1;

        }
        public override void AI()
        {
            base.AI();
            Timer++;
            Projectile.velocity *= 0.96f;
            if (Projectile.velocity.Length() <= 0.3f)
            {
                Projectile.Kill();
            }
            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.rotation += MathHelper.PiOver2;
            Lighting.AddLight(Projectile.Center, Color.Yellow.ToVector3() * 1.0f * Main.essScale);
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.velocity.X != oldVelocity.X)
                Projectile.velocity.X = -oldVelocity.X;
            if (Projectile.velocity.Y != oldVelocity.Y)
                Projectile.velocity.Y = -oldVelocity.Y;
            return false;
        }

       
        public override void PostDraw(Color lightColor)
        {
            Texture2D texture2D4 = ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/DimLight").Value;
            for (int i = 0; i < 3; i++)
            {
                Main.spriteBatch.Draw(texture2D4, Projectile.Center - Main.screenPosition, null,
                    new Color((int)(5f * 1), (int)(45f * 1), (int)(85f * 1), 0), Projectile.rotation, new Vector2(32, 32), 0.17f * (7 + 0.6f), SpriteEffects.None, 0f);
            }
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 3; i++)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, -Vector2.UnitY.RotatedByRandom(0.5f) * 7, ProjectileID.FrostDaggerfish, Projectile.damage, Projectile.knockBack, Projectile.owner);
            }
            SoundStyle parendineHitSound = AssetRegistry.Sounds.Melee.Parendine;
            parendineHitSound.PitchVariance = 0.2f;
            SoundEngine.PlaySound(parendineHitSound, Projectile.Center);

            for (int i = 0; i < 7; i++)
            {
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.DeepSkyBlue, 1f).noGravity = true;
            }
            for (int i = 0; i < 7; i++)
            {
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<TSmokeDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.LightSkyBlue, 1f).noGravity = true;
            }
            FXUtil.ShakeCamera(Projectile.Center, 1024, 32);
            float boomSize = Main.rand.NextFloat(0.025f, 0.08f);
            FXUtil.GlowCircleBoom(Projectile.Center,
                innerColor: Color.White,
                glowColor: Color.LightBlue,
                outerGlowColor: Color.Blue, duration: 25, baseSize: boomSize);

            var boom = FXUtil.GlowCircleBoom(Projectile.Center,
                 innerColor: Color.White,
                 glowColor: Color.LightBlue,
                 outerGlowColor: Color.Blue, duration: 25, baseSize: 0.12f);
            boom.Scale *= 2;
            for (float i = 0; i < 4; i++)
            {
                float progress = i / 4f;
                float rot = progress * MathHelper.ToRadians(360);
                rot += Main.rand.NextFloat(-0.5f, 0.5f);
                Vector2 offset = rot.ToRotationVector2() * 24;
                var particle = FXUtil.GlowCircleLongBoom(Projectile.Center,
                    innerColor: Color.White,
                    glowColor: Color.LightBlue,
                    outerGlowColor: Color.DarkBlue,
                    baseSize: Main.rand.NextFloat(0.1f, 0.2f),
                    duration: Main.rand.NextFloat(15, 25));
                particle.Rotation = rot + MathHelper.ToRadians(45);
            }
        }
        public float WidthFunction(float completionRatio)
        {
            float w = MathHelper.SmoothStep(5, 10, EasingFunction.QuadraticBump(completionRatio));
            w = MathHelper.Lerp(w, 0f, EasingFunction.InOutSine((Timer - 200) / 40f));
            return w;
        }

        public Color ColorFunction(float completionRatio)
        {
            Color tipColor = Color.Lerp(Color.LightSkyBlue, Color.Blue, completionRatio);
            Color finalColor = Color.Lerp(Color.Cyan, tipColor, EasingFunction.QuadraticBump(MathF.Pow(completionRatio, 0.5f)));
            Color finalColor2 = Color.Lerp(Color.Transparent, finalColor, EasingFunction.QuadraticBump(completionRatio));
            finalColor2 = Color.Lerp(finalColor2, Color.Blue, (Timer - 200) / 40f);
            return finalColor2;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            BlackFireShader blackFireShader = BlackFireShader.Instance;
            blackFireShader.Time = Main.GlobalTimeWrappedHourly * 2;
            blackFireShader.InnerColor = Color.LightCyan;
            blackFireShader.OuterColor = Color.Purple;
            blackFireShader.BackColor = Color.DarkBlue;
            blackFireShader.PrimaryTexture2 = TrailRegistry.BeamTrail;
            blackFireShader.NoiseTexture = TrailRegistry.WhispyTrail;
            blackFireShader.Distortion = 0.15f;
            TrailDrawer.Draw(Main.spriteBatch, OldCenterPos, OldCenterRot, ColorFunction, WidthFunction, blackFireShader, Vector2.Zero);

            SparkyShader sparkyShader = SparkyShader.Instance;
            sparkyShader.InnerColor = Color.White;
            sparkyShader.OuterColor = Color.Cyan;
            sparkyShader.Time = Timer * 0.3f;
            sparkyShader.Distortion = -0.15f;
            SpriteBatch spriteBatch = Main.spriteBatch;
            spriteBatch.Restart(effect: sparkyShader.Effect, blendState: BlendState.Additive);

            Texture2D texture = ModContent.Request<Texture2D>(TextureRegistry.ZuiEffect).Value;
            spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null,
                Color.Lerp(Color.Cyan, Color.White, EasingFunction.InExpo(Timer / 90f)),
                Timer * 0.005f, texture.Size() / 2f,
                MathHelper.Lerp(0.2f, 1, EasingFunction.InExpo(Timer / 120f)), SpriteEffects.None, 0);

            return false;
        }

    }
}
