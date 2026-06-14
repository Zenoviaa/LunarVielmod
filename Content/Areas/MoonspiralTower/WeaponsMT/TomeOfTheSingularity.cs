using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Content.CommonMaterials;
using Stellamod.Core.Bases;
using Stellamod.Core.Particles;
using Stellamod.Core.Pixelation;
using Stellamod.Helpers;
using Stellamod.Items;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.MoonspiralTower.WeaponsMT
{
    public class SingularitySparkProjG : ModProjectile
    {
        private ref float Timer => ref Projectile.ai[0];
        private bool IsSmall => Projectile.ai[1] == 1;
        private bool Bounced
        {
            get
            {
                return Projectile.ai[2] == 1;
            }
            set
            {
                Projectile.ai[2] = value ? 1 : 0;   
            }
        }
        private float ScaleVariance;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Shadow Hand");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 24;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.timeLeft = 240;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
        }

        public override void AI()
        {
            Timer++;
            if (Timer == 1)
            {
                SoundStyle softSummon = new SoundStyle("Stellamod/Assets/Sounds/SoftSummon2");
                softSummon.PitchVariance = 0.3f;
                if (IsSmall)
                    softSummon.Volume = 0.5f;
                SoundEngine.PlaySound(softSummon, Projectile.position);
                ScaleVariance = Main.rand.NextFloat(0.8f, 1f);
            }
            if (IsSmall)
            {
                Projectile.velocity.Y += 0.1f;
                Projectile.extraUpdates = 1;
            }
            Projectile.velocity *= 0.98f;
            Projectile.rotation += Projectile.velocity.Length() * 0.05f;
            Projectile.rotation += 0.01f;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (!Bounced)
            {
                if (Projectile.velocity.X != oldVelocity.X)
                    Projectile.velocity.X = -oldVelocity.X;
                if (Projectile.velocity.Y != oldVelocity.Y)
                    Projectile.velocity.Y = -oldVelocity.Y;
                Bounced = true;
                return false;
            }

            return base.OnTileCollide(oldVelocity);
        }
        public float GetTrailWidth(float completionRatio)
        {
            float w = MathHelper.SmoothStep(32, 0, completionRatio);
            if (IsSmall)
                w *= 0.5f;
            return w;
        }

        public Color GetTrailColor(float completionRatio)
        {
            return Color.Lerp(Color.LightBlue, Color.DarkBlue, completionRatio);
        }

        private void DrawPixelatedTrail(GraphicsDevice graphicsDevice)
        {
            var shader = BasicLaserShader.Instance;
            shader.InnerColor = Color.LightBlue;
            shader.OuterColor = Color.White;
            shader.LaserTexture = TrailRegistry.StarTrail;
            TrailDrawer.Draw(Main.spriteBatch, Projectile.oldPos, GetTrailColor, GetTrailWidth, shader, Projectile.Size / 2f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            PixelationManager.QueuePrimitivesDrawAction(DrawPixelatedTrail, DrawLayer.OverNPCsWithOutline);

            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Color drawColor = Color.White;
            drawColor = drawColor.MultiplyRGB(lightColor);
            Vector2 scale = Vector2.One * ScaleVariance;
            if (IsSmall)
                scale *= 0.5f;
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, new Vector2(texture.Width / 2, texture.Height / 2), scale, Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
            if (!IsSmall)
            {
                if (this.OwnedByLocalClient())
                {
                    for (int i = 0; i < 2; i++)
                    {
                        Vector2 vel = -Vector2.UnitY * Main.rand.NextFloat(4, 8);
                        vel = vel.RotatedByRandom(MathHelper.ToRadians(30));
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, vel, Type, Projectile.damage / 2, Projectile.knockBack, Projectile.owner, ai1: 1);
                    }
                }

                for (int i = 0; i < 2; i++)
                {
                    DustParticle dp = Particle<DustParticle>.Spawn(Projectile.Center, Main.rand.NextVector2Circular(2, 2), Color.White, Scale: Main.rand.NextFloat(0.3f, 1.5f));
                    dp.outerColor = Color.Blue;
                }
            }
            DustParticle dp2 = Particle<DustParticle>.Spawn(Projectile.Center, Main.rand.NextVector2Circular(2, 2), Color.White, Scale: Main.rand.NextFloat(0.3f, 1.5f));
            dp2.outerColor = Color.Blue;

        }
    }

    public class TomeOfTheSingularity : AbstractMagicTome
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Void's Grasp");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults2()
        {
            base.SetDefaults2();
            Item.damage = 24;
            Item.width = 50;
            Item.height = 50;
            Item.shoot = ModContent.ProjectileType<SingularitySparkProjG>();
            Item.shootSpeed = 15f;
            Item.mana = 12;
            Item.useTime = 10;
            Item.useAnimation = 10;
        }

        public override Color GetTomeHintColor()
        {
            return Color.SkyBlue;
        }

        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew(mold: ModContent.ItemType<BlankStaff>(), material: ModContent.ItemType<PearlescentScrap>());
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            base.ModifyShootStats(player, ref position, ref velocity, ref type, ref damage, ref knockback);
            velocity = velocity.RotatedByRandom(MathHelper.ToRadians(22));
        }
    }
}
