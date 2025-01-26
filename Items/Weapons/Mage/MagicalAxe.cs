using Microsoft.Xna.Framework;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Items.Materials;
using Stellamod.Items.Materials.Molds;
using Stellamod.Projectiles.IgniterExplosions;
using Stellamod.UI.Systems;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Mage
{
    internal class MagicalAxe : ClassSwapItem
    {
        public override DamageClass AlternateClass => DamageClass.Summon;

        public override void SetClassSwappedDefaults()
        {
            Item.damage = 30;
            Item.mana = 6;
        }

        public override void SetDefaults()
        {
            Item.damage = 70;
            Item.mana = 30;
            Item.width = 40;
            Item.height = 40;
            Item.useTime = 44;
            Item.useAnimation = 44;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.noMelee = true;
            Item.knockBack = 0f;
            Item.DamageType = DamageClass.Magic;
            Item.value = 15000;
            Item.rare = ItemRarityID.LightPurple;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<MagicalAxeBoomer>();
            Item.autoReuse = true;
            Item.noUseGraphic = true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, Main.MouseWorld, Vector2.Zero, type, damage, knockback, player.whoAmI, 0f, 0f);
            return false;

        }
        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew(mold: ModContent.ItemType<BlankStaff>(), material: ModContent.ItemType<ConvulgingMater>());
        }
    }

    public class MagicalAxeBoomer : ModProjectile
    {
        private ref float Timer => ref Projectile.ai[0];
        public override string Texture => TextureRegistry.EmptyTexture;
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 1;
            Projectile.height = 1;
            Projectile.timeLeft = 180;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            base.AI();
            Timer++;
            if (Timer == 1)
            {
                for (float f = 0; f < 4; f++)
                {
                    Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlyphDust>(),
                        (Vector2.One * Main.rand.NextFloat(0.2f, 5f)).RotatedByRandom(19.0), 0, Color.Blue, Main.rand.NextFloat(1f, 3f)).noGravity = true;
                }
                for (float f = 0; f < 4; f++)
                {
                    Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowDust>(),
                        (Vector2.One * Main.rand.NextFloat(0.2f, 5f)).RotatedByRandom(19.0), 0, Color.Blue, Main.rand.NextFloat(1f, 3f)).noGravity = true;
                }
                for (float i = 0; i < 4; i++)
                {
                    float progress = i / 4f;
                    float rot = progress * MathHelper.ToRadians(360);
                    rot += Main.rand.NextFloat(-0.5f, 0.5f);
                    Vector2 offset = rot.ToRotationVector2() * 24;
                    var particle = FXUtil.GlowCircleDetailedBoom1(Projectile.Center,
                        innerColor: Color.White,
                        glowColor: Color.Blue,
                        outerGlowColor: Color.Black,
                        duration: Main.rand.NextFloat(6, 12),
                        baseSize: Main.rand.NextFloat(0.01f, 0.05f));
                    particle.Rotation = rot + MathHelper.ToRadians(45);
                }

                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/StarFlower1"), Projectile.position);
            }

            if (Timer == 10)
            {
                for (float f = 0; f < 4; f++)
                {
                    Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlyphDust>(),
                        (Vector2.One * Main.rand.NextFloat(0.2f, 5f)).RotatedByRandom(19.0), 0, Color.Blue, Main.rand.NextFloat(1f, 3f)).noGravity = true;
                }
                for (float f = 0; f < 4; f++)
                {
                    Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowDust>(),
                        (Vector2.One * Main.rand.NextFloat(0.2f, 5f)).RotatedByRandom(19.0), 0, Color.Blue, Main.rand.NextFloat(1f, 3f)).noGravity = true;
                }
                if (Main.myPlayer == Projectile.owner)
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero,
                        ModContent.ProjectileType<SparklyBoom>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                }

                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/StarFlower3"), Projectile.position);
                ShakeModSystem.Shake = 3;
                FXUtil.ShakeCamera(Projectile.position, 1024, 4);

            }
            if (Timer == 30)
            {
                for (float f = 0; f < 4; f++)
                {
                    Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlyphDust>(),
                        (Vector2.One * Main.rand.NextFloat(0.2f, 5f)).RotatedByRandom(19.0), 0, Color.Blue, Main.rand.NextFloat(1f, 3f)).noGravity = true;
                }
                for (float f = 0; f < 4; f++)
                {
                    Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowDust>(),
                        (Vector2.One * Main.rand.NextFloat(0.2f, 5f)).RotatedByRandom(19.0), 0, Color.Blue, Main.rand.NextFloat(1f, 3f)).noGravity = true;
                }
                if (Main.myPlayer == Projectile.owner)
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero,
                        ModContent.ProjectileType<BlossomBoom>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                }
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/StarSheith"), Projectile.position);
                FXUtil.ShakeCamera(Projectile.position, 1024, 4);
                Projectile.Kill();
            }

        }
    }
}
