using Stellamod.Common.GunSystem;
using Stellamod.Content.CommonMaterials;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Items;
using Stellamod.Projectiles.Gun;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Cinderspark.WeaponsCS
{
    public class BrokenWrath : BaseGun
    {
        private int _combo;
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.damage = 16;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 40;
            Item.height = 40;
            Item.useTime = 32;
            Item.useAnimation = 32;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 6;
            Item.value = Item.sellPrice(0, 0, 20, 0);
            Item.rare = ItemRarityID.Blue;
            Item.UseSound = SoundID.Item5;
            Item.autoReuse = true;
            Item.shoot = ProjectileID.Bullet;
            Item.shootSpeed = 4f;
            Item.useAmmo = AmmoID.Bullet;
            Item.noMelee = true;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(8, 0);
        }

        public override bool GunShot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (type == ProjectileID.Bullet)
                type = ModContent.ProjectileType<BrokenMissile>();
            Vector2 Offset = Vector2.Normalize(new Vector2(velocity.X, velocity.Y - 1)) * 20f;
            if (Collision.CanHit(position, 0, 0, position + Offset, 0, 0))
            {
                position += Offset;
            }

            velocity = velocity.RotatedByRandom(MathHelper.ToRadians(5));
            Projectile proj = Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, player.whoAmI, ai1: _combo);
            _combo++;
            if (_combo >= 2)
                _combo = 0;

            for (int index1 = 0; index1 < 19; ++index1)
            {
                int index2 = Dust.NewDust(new Vector2(position.X, position.Y), Item.width - 20, Item.height - 45, DustID.CopperCoin, velocity.X, velocity.Y, byte.MaxValue, new Color(), Main.rand.Next(6, 10) * 0.1f);
                Main.dust[index2].noGravity = true;
                Main.dust[index2].velocity *= 0.5f;
                Main.dust[index2].scale *= 1.2f;
            }
            damage /= 2;

            //generate the remaining projectiles
            int Sound = Main.rand.Next(1, 3);
            SoundStyle shootSound;
            if (Sound == 1)
            {
                shootSound = new SoundStyle("Stellamod/Assets/Sounds/BrokenWrath2");
            }
            else
            {
                shootSound = new SoundStyle("Stellamod/Assets/Sounds/BrokenWrath1");

            }
            shootSound.PitchVariance = 0.3f;
            shootSound.Volume = 0.5f;
            SoundEngine.PlaySound(shootSound, player.position);


            Vector2 origVect = new Vector2(velocity.X, velocity.Y);
            Vector2 newVect = origVect.RotatedBy(System.Math.PI / (Main.rand.Next(72, 1300) / 23));
            Projectile.NewProjectile(source, position, newVect, ModContent.ProjectileType<BTech1>(), damage, knockback, player.whoAmI, 0f, 0f);
            newVect = origVect.RotatedBy(System.Math.PI / (Main.rand.Next(72, 1300) / 23));
            Projectile.NewProjectile(source, position, newVect, ModContent.ProjectileType<BTech2>(), damage, knockback, player.whoAmI, 0f, 0f);
            newVect = origVect.RotatedBy(System.Math.PI / (Main.rand.Next(72, 1300) / 23));
            Projectile.NewProjectile(source, position, newVect, ModContent.ProjectileType<BTech3>(), damage, knockback, player.whoAmI, 0f, 0f);
            newVect = origVect.RotatedBy(System.Math.PI / (Main.rand.Next(72, 1300) / 13));
            return base.GunShot(player, source, position, velocity, type, damage, knockback);
        }

        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew(
                mold: ModContent.ItemType<BlankGun>(),
                material: ModContent.ItemType<Cinderscrap>());
        }
    }

    public class BrokenMissile : ModProjectile
    {
        private ref float Style => ref Projectile.ai[1];
        private Color MainColor
        {
            get
            {
                if (Style == 1)
                {
                    return Color.Cyan;
                }
                return Color.Orange;
            }
        }

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.Bullet);
            AIType = ProjectileID.Bullet;
            Projectile.penetrate = 1;
            Projectile.width = 15;
            Projectile.height = 15;
            Projectile.extraUpdates += 1;
        }

        public override void AI()
        {
            Projectile.velocity *= 1.01f;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            Vector2 drawOrigin = new Vector2(TextureAssets.Projectile[Projectile.type].Value.Width * 0.5f, Projectile.height * 0.5f);
            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Color color = Projectile.GetAlpha(Color.Lerp(MainColor, Color.Transparent, 1f / Projectile.oldPos.Length * k) * (1f - 1f / Projectile.oldPos.Length * k));
                color.A = 0;
                spriteBatch.Draw(TextureAssets.Projectile[Projectile.type].Value, drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0f);
            }
      
            return false;
        }
        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item14, Projectile.position);
            for (float f = 0; f < 12; f++)
            {
                Vector2 velocity = Main.rand.NextVector2Circular(5, 5);
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlyphDust>(), velocity, 0, MainColor, Main.rand.NextFloat(1f, 3f)).noGravity = true;
            }

            for (float i = 0; i < 4; i++)
            {
                float progress = i / 4f;
                float rot = progress * MathHelper.ToRadians(360);
                Vector2 offset = rot.ToRotationVector2() * 24;
                var particle = FXUtil.GlowCircleDetailedBoom1(Projectile.Center,
                    innerColor: Color.White,
                    glowColor: MainColor,
                    outerGlowColor: Color.Black,
                    baseSize: Main.rand.NextFloat(0.02f, 0.16f),
                    duration: Main.rand.NextFloat(12, 24));
                particle.Rotation = rot + MathHelper.ToRadians(45);
            }
        }
    }

    public abstract class BTechBase : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.Shuriken);
            AIType = ProjectileID.Shuriken;
            Projectile.penetrate = 1;
            Projectile.width = 15;
            Projectile.height = 15;
        }
        public override void AI()
        {
            Projectile.velocity /= 0.99f;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Vector2 drawOrigin = new Vector2(TextureAssets.Projectile[Projectile.type].Value.Width * 0.5f, Projectile.height * 0.5f);
            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Color color = Projectile.GetAlpha(Color.Lerp(new Color(253, 255, 31), new Color(182, 83, 38), 1f / Projectile.oldPos.Length * k) * (1f - 1f / Projectile.oldPos.Length * k));
                Main.spriteBatch.Draw(TextureAssets.Projectile[Projectile.type].Value, drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0f);
            }
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
            for (float f = 0; f < 4; f++)
            {
                Dust.NewDustPerfect(Projectile.Center, DustID.CopperCoin,
                    (Vector2.One * Main.rand.NextFloat(0.2f, 5f)).RotatedByRandom(19.0), 0, Color.White, Main.rand.NextFloat(1f, 3f)).noGravity = true;
            }
        }
    }

    public class BTech1 : BTechBase
    {

    }

    public class BTech2 : BTechBase
    {

    }

    public class BTech3 : BTechBase
    {

    }
}