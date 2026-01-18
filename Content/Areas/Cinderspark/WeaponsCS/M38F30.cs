using Microsoft.Xna.Framework;
using Stellamod.Common.GunSystem;
using Stellamod.Content.CommonMaterials;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Items;
using Stellamod.Items.Harvesting;
using Stellamod.Projectiles.Gun;
using Stellamod.Trails;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Creative;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Cinderspark.WeaponsCS
{
    public class M38F30 : BaseGun
    {
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.damage = 22;
            Item.width = 50;
            Item.height = 50;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 4;
            Item.value = Item.sellPrice(0, 1, 1, 29);
            Item.rare = ItemRarityID.Green;

            Item.shootSpeed = 20;
            Item.autoReuse = true;
            Item.DamageType = DamageClass.Ranged;
            Item.shoot = ProjectileID.PurificationPowder;
            Item.shootSpeed = 20f;
            Item.useAmmo = AmmoID.Bullet;
            Item.UseSound = SoundID.Item98;
            Item.useAnimation = 34;
            Item.useTime = 34;
            Item.consumeAmmoOnLastShotOnly = true;
            Item.noMelee = true;
        }

        public override void SetMagazine(ref GunReloadParams fireParams)
        {
            base.SetMagazine(ref fireParams);
            fireParams.reloadWindow = 60;
            fireParams.maxAmmo = 12;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-5f, 0f);
        }

        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew(mold: ModContent.ItemType<BlankGun>(), material: ModContent.ItemType<Cinderscrap>());
        }

        public override void ShootEffects(Vector2 position, Vector2 velocity)
        {
            base.ShootEffects(position, velocity);
        }

        public override bool GunShot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Vector2 origVect = new Vector2(velocity.X, velocity.Y);
            //generate the remaining projectiles


            Vector2 newVelocity = velocity;
            newVelocity.Y -= 2;
            newVelocity.X *= 0.5f;
            Projectile.NewProjectile(source, position, newVelocity, ModContent.ProjectileType<M38F30Rocks>(), damage / 2, knockback, player.whoAmI, 0, 0f);

            for (int index1 = 0; index1 < 19; ++index1)
            {
                int index2 = Dust.NewDust(new Vector2(position.X, position.Y), Item.width - 20, Item.height - 45, DustID.CopperCoin, velocity.X, velocity.Y, byte.MaxValue, new Color(), Main.rand.Next(2, 10) * 0.2f);
                Main.dust[index2].noGravity = true;
                Main.dust[index2].velocity *= 0.7f;
                Main.dust[index2].scale *= 1.2f;
            }
            for (int index1 = 0; index1 < 19; ++index1)
            {
                int index2 = Dust.NewDust(new Vector2(position.X, position.Y), Item.width - 20, Item.height - 45, DustID.DynastyWood, velocity.X, velocity.Y, byte.MaxValue, new Color(), Main.rand.Next(2, 10) * 0.2f);
                Main.dust[index2].noGravity = false;
                Main.dust[index2].velocity *= 0.1f;
                Main.dust[index2].scale *= 2.2f;
            }
            return base.GunShot(player, source, position, velocity, type, damage, knockback);
        }
    }

    public class M38F30Rocks : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailingMode[Type] = 1;
            ProjectileID.Sets.TrailCacheLength[Type] = 8;
        }

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.Shuriken);
            AIType = ProjectileID.Shuriken;
            Projectile.penetrate = 1;
            Projectile.width = 8;
            Projectile.height = 12;
            Projectile.timeLeft = 700;
            Projectile.alpha = 0;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            Projectile.ai[1]++;
            if (Projectile.ai[1] >= 10)
            {
                Projectile.tileCollide = true;
            }

            if (Projectile.ai[1] == 2)
            {
                SoundStyle shootSound = new SoundStyle("Stellamod/Assets/Sounds/M38F30");
                shootSound.PitchVariance = 0.2f;
                shootSound.Volume = 0.66f;
                SoundEngine.PlaySound(shootSound, Projectile.position);
            }

            if (Main.rand.NextBool(7))
            {
                int dustnumber = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.CopperCoin, 0f, 0f, 150, Color.White, 1f);
                Main.dust[dustnumber].noGravity = false;
                Main.dust[dustnumber].velocity *= 0.3f;
            }
            Projectile.velocity.Y -= 0.01f;
            Lighting.AddLight(Projectile.Center, Color.Brown.ToVector3() * 1.75f * Main.essScale);
        }
        public override void OnKill(int timeLeft)
        {
            for (float f = 0; f < 9; f++)
            {
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlyphDust>(),
                    (Vector2.One * Main.rand.NextFloat(0.2f, 5f)).RotatedByRandom(19.0), 0, Color.Orange, Main.rand.NextFloat(1f, 3f)).noGravity = true;
            }

            for (float i = 0; i < 4; i++)
            {
                float progress = i / 4f;
                float rot = progress * MathHelper.ToRadians(360);
                Vector2 offset = rot.ToRotationVector2() * 24;
                var particle = FXUtil.GlowCircleDetailedBoom1(Projectile.Center,
                    innerColor: Color.White,
                    glowColor: Color.Orange,
                    outerGlowColor: Color.Black,
                    baseSize: Main.rand.NextFloat(0.1f, 0.2f),
                    duration: Main.rand.NextFloat(12, 24));
                particle.Rotation = rot + MathHelper.ToRadians(45);
            }

            SoundStyle sound;
            if (Main.rand.Next(1, 3) == 1)
            {
                sound = new SoundStyle("Stellamod/Assets/Sounds/M38F30Bomb2");
            }
            else
            {
                sound = new SoundStyle("Stellamod/Assets/Sounds/M38F30Bomb1");
            }
            sound.Volume = 0.66f;
            sound.PitchVariance = 0.3f;
            SoundEngine.PlaySound(sound, Projectile.position);

            if (this.OwnedByLocalClient())
            {
                for (int i = 0; i < 5; i++)
                {
                    Vector2 velocity = new Vector2(Main.rand.Next(-8, 8), Main.rand.Next(-8, -5));
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity,
                        ModContent.ProjectileType<M38F30Rocks2>(), Projectile.damage / 3, 1, Projectile.owner, 0, 0);
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, new Vector2(texture.Width / 2, texture.Height / 2), 1f, Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
            return false;
        }
    }
    public class M38F30Rocks2 : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Rock");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 20;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 1;
        }

        public override void PostDraw(Color lightColor)
        {

        }

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.Shuriken);
            AIType = ProjectileID.Shuriken;
            Projectile.penetrate = 1;
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.timeLeft = 700;
            Projectile.alpha = 0;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            Projectile.ai[1]++;
            if (Projectile.ai[1] >= 10)
            {
                Projectile.tileCollide = true;
            }

            Projectile.velocity.Y -= 0.01f;
            Lighting.AddLight(Projectile.Center, Color.Brown.ToVector3() * 1.75f * Main.essScale);
        }

        public override void OnKill(int timeLeft)
        {
            SoundStyle sound;
            if (Main.rand.Next(1, 3) == 1)
            {
                sound = new SoundStyle("Stellamod/Assets/Sounds/M38F30Bomb3");
            }
            else
            {
                sound = new SoundStyle("Stellamod/Assets/Sounds/M38F30Bomb4");
            }
            sound.Volume = 0.66f;
            sound.PitchVariance = 0.3f;
            SoundEngine.PlaySound(sound, Projectile.position);

            for (int i = 0; i < 10; i++)
            {
                int num1 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.CopperCoin, 0f, -2f, 0, default, .8f);
                Main.dust[num1].noGravity = true;
                Main.dust[num1].position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
                Main.dust[num1].position.Y += Main.rand.Next(-50, 51) * .05f - 1.5f;
                if (Main.dust[num1].position != Projectile.Center)
                    Main.dust[num1].velocity = Projectile.DirectionTo(Main.dust[num1].position) * 6f;
                int num = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.CopperCoin, 0f, -2f, 0, default, .8f);
                Main.dust[num].noGravity = true;
                Main.dust[num].position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
                Main.dust[num].position.Y += Main.rand.Next(-50, 51) * .05f - 1.5f;
                if (Main.dust[num].position != Projectile.Center)
                    Main.dust[num].velocity = Projectile.DirectionTo(Main.dust[num].position) * 6f;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, new Vector2(texture.Width / 2, texture.Height / 2), 1f, Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
            return false;
        }
    }
}
