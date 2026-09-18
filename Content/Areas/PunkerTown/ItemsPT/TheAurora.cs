using Stellamod.Common.Shaders;
using Stellamod.Content.CommonMaterials;
using Stellamod.Content.Dusts;
using Stellamod.Core.Particles;
using Stellamod.Core.Pixelation;
using Stellamod.Items;
using Stellamod.Visual.Particles;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Stellamod.Content.Areas.PunkerTown.ItemsPT
{
    public class TheAurora : ModItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.noMelee = true;
            Item.mana = 12;
            Item.damage = 19;
            Item.DamageType = DamageClass.Magic;
            Item.width = 40;
            Item.height = 40;
            Item.useTime = 6;
            Item.useAnimation = 12;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 4;
            Item.value = Item.sellPrice(0, 1, 20, 0);
            Item.rare = ItemRarityID.LightRed;
            Item.autoReuse = true;
            Item.shoot = ProjectileType<AuroraStar>();
            Item.shootSpeed = 15f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            //Dust Burst Towards Mouse

            int Sound = Main.rand.Next(1, 3);
            SoundStyle shootSound = new SoundStyle("Stellamod/Assets/Sounds/MiniPistol");
            if (Sound == 1)
            {


            }
            else
            {
                shootSound = new SoundStyle("Stellamod/Assets/Sounds/MiniPistol3");
            }
            shootSound.PitchVariance = 0.1f;
            shootSound.Volume = 0.3f;
            SoundEngine.PlaySound(shootSound, position);

            shootSound = new SoundStyle("Stellamod/Assets/Sounds/Starblast");
            shootSound.PitchVariance = 0.2f;
            SoundEngine.PlaySound(shootSound, position);
            float rot = velocity.ToRotation();
            float spread = 0.4f;

            Vector2 offset = new Vector2(1, 0f).RotatedBy(rot);
            for (int k = 0; k < 2; k++)
            {
                Vector2 direction = offset.RotatedByRandom(spread);
                Dust.NewDustPerfect(position + offset * 80, ModContent.DustType<Dusts.GlowDust>(), direction * Main.rand.NextFloat(8), 125, Color.Goldenrod, Main.rand.NextFloat(0.2f, 0.5f));
            }
            Dust.NewDustPerfect(position + offset * 80, ModContent.DustType<Dusts.GlowDust>(), new Vector2(0, 0), 125, Color.Goldenrod, 1);
            for (int k = 0; k < 2; k++)
            {
                Projectile.NewProjectile(source, position, -velocity.RotatedByRandom(MathHelper.ToRadians(65)), type, damage, knockback, player.whoAmI);
            }

            return false;

        }
        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew(
                mold: ModContent.ItemType<BlankGun>(),
                material: ModContent.ItemType<MarshScrap>());
        }
    }

    public class AuroraStar : ModProjectile
    {
        private ref float Timer => ref Projectile.ai[0];
        private Vector2 TargetPosition;
        private Color MainColor
        {
            get
            {
                return Color.Goldenrod;
            }
        }
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            ProjectileID.Sets.TrailCacheLength[Type] = 24;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 4;
            Projectile.height = 4;
            Projectile.friendly = true;
            Projectile.timeLeft = 360;
            Projectile.tileCollide = false;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            base.SendExtraAI(writer);
            writer.WriteVector2(TargetPosition);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            base.ReceiveExtraAI(reader);
            TargetPosition = reader.ReadVector2();
        }
        public override void AI()
        {
            base.AI();

            Timer++;
            if (Timer > 120)
            {
                Projectile.tileCollide = true;
            }

            if (Timer % 36 == 0)
            {
                //  Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlyphDust>(), Projectile.velocity * 0.1f, 0, MainColor, Main.rand.NextFloat(1f, 3f)).noGravity = true;
            }
            if (Main.myPlayer == Projectile.owner && TargetPosition == Vector2.Zero)
            {
                TargetPosition = Main.MouseWorld;
                Projectile.netUpdate = true;
            }

            float maxDegreesRotate = MathHelper.Lerp(0.2f, 16f, Timer / 30f);

            Projectile.extraUpdates = (int)MathHelper.Lerp(1, 3f, Timer / 30f);
            Projectile.velocity = ProjectileHelper.SimpleHomingVelocity(Projectile, TargetPosition, maxDegreesRotate);
            Projectile.rotation = Projectile.velocity.ToRotation() + Timer * 0.05f;
        }

        public float GetTrailWidth(float completionRatio)
        {
            float baseWidth = Projectile.scale * Projectile.width;
            return MathHelper.SmoothStep(12, 0.5f, completionRatio);
        }

        public Color GetTrailColor(float completionRatio)
        {
            return Color.Lerp(Color.Goldenrod, Color.CadetBlue, completionRatio) * 0.7f;
        }

        private void DrawPixelatedTrail(GraphicsDevice graphicsDevice)
        {
            var laserShader = BasicLaserShader.Instance;
            TrailDrawer.Draw(Main.spriteBatch, Projectile.oldPos, GetTrailColor, GetTrailWidth, laserShader, Projectile.Size / 2f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            PixelationManager.QueuePrimitivesDrawAction(DrawPixelatedTrail, DrawLayer.OverNPCsWithOutline);
            Texture2D texture2D4 = ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/DimLight").Value;
            Color drawColor = MainColor;
            drawColor.A = 0;
            SpriteBatch spriteBatch = Main.spriteBatch;
            for (int i = 0; i < 2; i++)
                spriteBatch.Draw(texture2D4, Projectile.Center - Main.screenPosition, null, drawColor, Projectile.rotation, new Vector2(32, 32), 0.5f, SpriteEffects.None, 0f);

            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, new Vector2(texture.Width / 2, texture.Height / 2), Projectile.scale, Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
            for (float f = 0; f < 1; f++)
            {
                DustParticle dp = Particle<DustParticle>.Spawn(Projectile.Center, Vector2.UnitY.RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(12f, 15f), Color.White);
                dp.gravity = 0;
                dp.dampening = 0.2f;
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlyphDust>(),
                    (Vector2.One * Main.rand.NextFloat(0.2f, 5f)).RotatedByRandom(19.0), 0, MainColor, Main.rand.NextFloat(1f, 3f)).noGravity = true;
            }
            for (float i = 0; i < 4; i++)
            {
                float progress = i / 4f;
                float rot = progress * MathHelper.ToRadians(360);
                rot += Main.rand.NextFloat(-0.5f, 0.5f);
                Vector2 offset = rot.ToRotationVector2() * 24;
                var particle = FXUtil.GlowCircleDetailedBoom1(Projectile.Center,
                    innerColor: Color.White,
                    glowColor: MainColor,
                    outerGlowColor: Color.Black,
                    duration: Main.rand.NextFloat(6, 12),
                    baseSize: Main.rand.NextFloat(0.01f, 0.05f));
                particle.Rotation = rot + MathHelper.ToRadians(45);
            }
        }
    }
}