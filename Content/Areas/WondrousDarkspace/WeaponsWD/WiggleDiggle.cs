using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Gores;
using Stellamod.Helpers;
using Stellamod.Items;
using Stellamod.Items.Materials;
using Stellamod.Items.Materials.Molds;
using Stellamod.Trails;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.WondrousDarkspace.WeaponsWD
{
    public class WiggleDiggle : ClassSwapItem
    {
        public override DamageClass AlternateClass => DamageClass.Magic;
        public override void SetClassSwappedDefaults()
        {
            Item.damage = 40;
            Item.mana = 8;
        }

        public override void SetDefaults()
        {
            Item.damage = 80;
            Item.width = 96;
            Item.height = 42;
            Item.DamageType = DamageClass.Ranged;
            Item.useAnimation = 50;
            Item.useTime = 50;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.UseSound = SoundID.DD2_KoboldExplosion;
            Item.knockBack = 2;
            Item.rare = ItemRarityID.Lime;
            Item.shoot = ModContent.ProjectileType<WiggleDiggleProj>();
            Item.shootSpeed = 19;

        }

        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew<BlankGun, MiracleThread>();
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-8, 0);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {

            SoundStyle soundStyle = new SoundStyle("Stellamod/Assets/Sounds/ConfettiShot1");
            soundStyle.PitchVariance = 0.3f;
            soundStyle.Volume = 0.8f;
            SoundEngine.PlaySound(soundStyle, position);

            float rot = velocity.ToRotation();
            float spread = 0.4f;
            Vector2 offset = new Vector2(6, -0.1f * player.direction).RotatedBy(rot);

            //Funny Screenshake
            Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(player.Center, 1024f, 32f);
            int numProjectiles = Main.rand.Next(8, 15);
            float distance = 12;
            for (int p = 0; p < numProjectiles; p++)
            {
                //Particles and stuff
                Dust.NewDustPerfect(position + offset * distance, ModContent.DustType<Dusts.GlowDust>(), new Vector2(0, 0), 125, new Color(150, 80, 40), 1);
                Dust.NewDustPerfect(player.Center + offset * distance, ModContent.DustType<Dusts.TSmokeDust>(), Vector2.UnitY * -2 + offset.RotatedByRandom(spread), 150, new Color(60, 55, 50) * 0.5f, Main.rand.NextFloat(0.5f, 1));

                //Get a random velocity
                Vector2 startVelocity = velocity.RotatedByRandom(MathHelper.PiOver4 / 2);

                //Get a random
                float randScale = Main.rand.NextFloat(0.5f, 1.5f);

                // Rotate the velocity randomly by 30 degrees at max.
                Vector2 newVelocity = velocity.RotatedByRandom(MathHelper.ToRadians(15));
                newVelocity *= 1f - Main.rand.NextFloat(0.3f);
                Projectile.NewProjectileDirect(source, position, newVelocity, type, damage, knockback, player.whoAmI);
                for (int k = 0; k < Main.rand.Next(2, 7); k++)
                {
                    int[] goreTypes = new int[]
                    {
                        ModContent.GoreType<RibbonBlue>(),
                        ModContent.GoreType<RibbonPink>(),
                        ModContent.GoreType<RibbonWhite>(),
                        ModContent.GoreType<RibbonYellow>()
                    };

                    int goreType = goreTypes[Main.rand.Next(0, goreTypes.Length)];
                    Gore.NewGore(source, position + offset.RotatedByRandom(MathHelper.PiOver4) * distance * Main.rand.NextFloat(0.5f, 1f),
                        newVelocity.RotatedByRandom(MathHelper.PiOver4),
                      goreType);
                }
            }

            return base.Shoot(player, source, position, velocity, type, damage, knockback);
        }
    }

    public class WiggleDiggleProj : ModProjectile,
        IPixelPrimitiveDrawer
    {
        private int _color;
        private ref float Timer => ref Projectile.ai[0];
        public override string Texture => TextureRegistry.EmptyTexture;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 6;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = true;
        }

        public override void AI()
        {
            Timer++;
            if (Timer == 1)
            {
                _color = Main.rand.Next(4);
            }

            Projectile.velocity.Y += 0.15f;
            Projectile.rotation = Projectile.velocity.ToRotation();
        }

        public float WidthFunction(float completionRatio)
        {
            return 2;
        }

        public Color ColorFunction(float completionRatio)
        {
            switch (_color)
            {
                default:
                case 0:
                    return new Color(5, 7, 203);
                case 1:
                    return new Color(255, 51, 240);
                case 2:
                    return new Color(255, 229, 0);
                case 3:
                    return Color.White;
            }
        }

        public override void OnKill(int timeLeft)
        {
            int goreType;
            switch (_color)
            {
                default:
                case 0:
                    goreType = ModContent.GoreType<RibbonBlue>();
                    break;
                case 1:
                    goreType = ModContent.GoreType<RibbonPink>();
                    break;
                case 2:
                    goreType = ModContent.GoreType<RibbonYellow>();
                    break;
                case 3:
                    goreType = ModContent.GoreType<RibbonWhite>();
                    break;
            }
            for (int i = 0; i < 1; i++)
            {
                Vector2 velocity = Main.rand.NextVector2Circular(4, 4);
                Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.Center, velocity,
                  goreType);
            }
        }

        public PrimitiveTrail BeamDrawer;
        public Vector2[] TrailPos;
        public void DrawPixelPrimitives(SpriteBatch spriteBatch)
        {
            BeamDrawer ??= new PrimitiveTrail(WidthFunction, ColorFunction, null, true, TrailRegistry.LaserShader);
            BeamDrawer.SpecialShader = null;

            if (TrailPos == null)
            {
                TrailPos = new Vector2[Projectile.oldPos.Length];
                for (int i = 0; i < TrailPos.Length; i++)
                {
                    Projectile.oldPos[i] = Projectile.position;
                }
            }

            for (int i = 0; i < TrailPos.Length; i++)
            {
                TrailPos[i] = Projectile.oldPos[i];
                TrailPos[i] += new Vector2(VectorHelper.Osc(0, 16, offset: i));
            }
            BeamDrawer.DrawPixelated(TrailPos, -Main.screenPosition, 32);
            Main.spriteBatch.ExitShaderRegion();
        }
    }
}
