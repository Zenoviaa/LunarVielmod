
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Core.Bases;
using Stellamod.Core.Effects.Trails;
using Stellamod.Core.SwingSystem;
using Stellamod.Helpers;
using Stellamod.Items;
using Stellamod.Items.Harvesting;
using Stellamod.Items.Materials.Molds;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.SpringHills.WeaponsSH
{
    public class DualSpoons : BaseSwingItemV2
    {
        public override void SetDefaults2()
        {
            base.SetDefaults2();
            Item.useAnimation = 16;
            Item.useTime = 16;
            Item.damage = 2;
            Item.shoot = ModContent.ProjectileType<DualSpoonsSlash>();
            staminaProjectileShoot = ModContent.ProjectileType<DualSpoonThrow>();
            meleeWeaponType = MeleeWeaponType.Knives;
            staminaCost = 1;
        }

        public override void ShootSwingStamina(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            float radians = MathHelper.ToRadians(15);
            damage *= 3;
            Projectile.NewProjectile(source, position, velocity.RotatedBy(-radians), type, damage, knockback,
                player.whoAmI);
            Projectile.NewProjectile(source, position, velocity.RotatedBy(radians), type, damage, knockback,
                player.whoAmI);
        }

        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew<Mushroom, BlankSword>();
        }
    }


    public class DualSpoonsSlash : BaseSwingProjectileV2
    {
        private bool _hasSpawnedSecondKnife;
        public override void DefineCombo()
        {
            base.DefineCombo();
            var SlashEffect = new SlashEffect()
            {
                BaseColor = Color.White,
                WindColor = Color.LightGray,
                LightColor = Color.Gray,
                RimHighlightColor = Color.White,
                BlendState = Microsoft.Xna.Framework.Graphics.BlendState.Additive
            };
            var SlashTrailer = new SlashTrailer();
            SlashTrailer.TrailWidthFunction = GetTrailWidth;
            SlashTrailer.Shader = SlashEffect;
            Trailer = SlashTrailer;
            SwingV2Helper.AddKnivesSwingStyle(this);
            useAfterImage = true;
        }

        private float GetTrailWidth(float interpolant)
        {
            return EasingFunction.QuadraticBump(interpolant) * 8;
        }

        public override void AI()
        {
            base.AI();
            if (!_hasSpawnedSecondKnife && ComboIndex != ComboCount - 1 && Interpolant >= 0.9f)
            {
                CloneProjectile();
                _hasSpawnedSecondKnife = true;
            }

            growScale = MathHelper.Lerp(0f, 0.3f, EasingFunction.QuadraticBump(Interpolant));
        }


        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            base.ModifyHitNPC(target, ref modifiers);
            SoundStyle spearHit = SoundRegistry.SpearHit1;
            spearHit.PitchVariance = 0.5f;
            SoundEngine.PlaySound(spearHit, Projectile.position);
            if (ComboIndex == 5)
            {
                StatModifier statModifier = new StatModifier(0.5f, 1f);
                modifiers.FinalDamage.CombineWith(statModifier);
            }
        }
    }

    public class DualSpoonThrow : ModProjectile
    {
        private ref float Timer => ref Projectile.ai[0];

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.friendly = true;
            Projectile.timeLeft = 300;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 30;
            Projectile.penetrate = 4;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            Projectile.velocity.X = -Projectile.velocity.X;
            Projectile.velocity.Y += -5;
        }

        public override void AI()
        {
            base.AI();
            Timer++;
            if (Timer % 10 == 0)
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.SilverCoin);
            }

            if (Timer < 30)
            {
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(45);
            }
            else
            {
                Projectile.velocity.Y += 0.5f;
                Projectile.rotation += 0.5f;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            SpriteBatch spriteBatch = Main.spriteBatch;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            float rotation = Projectile.rotation;
            Vector2 drawOrigin = texture.Size() / 2f;
            Vector2 drawScale = Vector2.One;
            spriteBatch.Draw(texture, drawPosition, null, lightColor, rotation, drawOrigin, drawScale, SpriteEffects.None, 0);
            return false;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.penetrate--;
            if (Projectile.penetrate <= 0)
                return true;
            if (Projectile.velocity.X != oldVelocity.X)
                Projectile.velocity.X = -oldVelocity.X;
            if (Projectile.velocity.Y != oldVelocity.Y)
                Projectile.velocity.Y = -oldVelocity.Y;
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
            FXUtil.GlowCircleBoom(Projectile.Center, Color.White, Color.LightGray, Color.DarkGray, baseSize: 0.06f);
        }
    }
}