using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Content.Areas.Cinderspark.WeaponsCS;
using Stellamod.Content.CommonMaterials;
using Stellamod.Core.Pixelation;
using Stellamod.Helpers;
using Stellamod.Items;
using Stellamod.Projectiles;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Fable.WeaponsFB
{
    public class BurningFlask : ModItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.width = 24;
            Item.height = 24;
            Item.damage = 9;
            Item.DamageType = DamageClass.Ranged;
            Item.noUseGraphic = true;
            Item.height = 40;
            Item.useTime = 70;
            Item.useAnimation = 70;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 6;
            Item.value = 10000;
            Item.rare = ItemRarityID.LightPurple;
            Item.UseSound = SoundID.DD2_GhastlyGlaivePierce;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<HornetLob>();
            Item.shootSpeed = 13f;
        }

        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew(
                mold: ModContent.ItemType<BlankJuggler>(), 
                material: ModContent.ItemType<AlcadizScrap>());
        }
    }

    public class HornetLob : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 16;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.aiStyle = ProjAIStyleID.ThrownProjectile;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = true;
        }

        public override void AI()
        {
            Vector3 RGB = new(1.00f, 0.37f, 0.30f);

            // The multiplication here wasn't doing anything
            Lighting.AddLight(Projectile.position, RGB.X, RGB.Y, RGB.Z);
        }

        public float WidthFunction(float completionRatio)
        {
            float baseWidth = Projectile.scale * Projectile.width * 0.5f;
            return MathHelper.SmoothStep(baseWidth, 3.5f, completionRatio);
        }

        public Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(Color.DimGray, Color.Transparent, completionRatio);
        }

        private void DrawFlamingTrail(GraphicsDevice graphicsDevice)
        {
            BlackFireShader blackFireShader = BlackFireShader.Instance;
            TrailDrawer.Draw(Main.spriteBatch, Projectile.oldPos, ColorFunction, WidthFunction, blackFireShader, Projectile.Size * 0.5f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            PixelationManager.QueuePrimitivesDrawAction(DrawFlamingTrail);
            DrawHelper.DrawDimLight(Projectile, 70, 30, 10, Color.OrangeRed, lightColor, 2);
            DrawHelper.DrawAdditiveAfterImage(Projectile, Color.OrangeRed, Color.Transparent, ref lightColor);
            return base.PreDraw(ref lightColor);
        }


        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
            float numDust = 5;
            for (float n = 0; n < numDust; n++)
            {
                Vector2 inverseVelocity = -Projectile.oldVelocity;
                inverseVelocity = inverseVelocity.RotatedByRandom(1.5f) * Main.rand.NextFloat(0.5f, 1f);
                var dp = DustParticle.Spawn(Projectile.Center, inverseVelocity);
                dp.dampening = 0.1f;
                dp.Scale *= 0.5f;
                dp.innerColor = Color.OrangeRed;
                dp.outerColor = Color.DarkRed;
            }FXUtil.GlowCircleBoom(Projectile.Center, Color.Yellow, Color.OrangeRed, Color.DarkRed);

            if (this.OwnedByLocalClient())
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero,
                    ModContent.ProjectileType<HornetKaboom>(), Projectile.damage, 0f, Projectile.owner, 0f, 0f);
            }

            SoundStyle flameUp = AssetManager.GetSound("flameup");
            flameUp.PitchVariance = 0.3f;
            SoundEngine.PlaySound(flameUp, Projectile.position);
        }
    }
}