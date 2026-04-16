using Stellamod.Common.Shaders;
using Stellamod.Common.Shaders.MagicTrails;
using Stellamod.Content.Areas.Cinderspark.WeaponsCS;
using Stellamod.Content.CommonMaterials;
using Stellamod.Core.Bases;
using Stellamod.Core.Pixelation;
using Stellamod.Helpers;
using Stellamod.Items;
using Stellamod.Projectiles;
using Stellamod.Trails;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Riverside.WeaponsRS

{
    public class WaterSong : BaseCrossbowItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.damage = 8;
            Item.rare = ItemRarityID.Blue;
        }

        public override void ShootBow(Player player, EntitySource_ItemUse_WithAmmo source, ShootParams shootParams)
        {
            int Sound = Main.rand.Next(1, 3);
            SoundStyle shootSound;
            if (Sound == 1)
            {
                shootSound = new SoundStyle("Stellamod/Assets/Sounds/ArchariliteEnergyShot");
            }
            else
            {
                shootSound = new SoundStyle("Stellamod/Assets/Sounds/ArchariliteEnergyShot2");
            }

            shootSound.Volume = 0.5f;
            shootSound.PitchVariance = 0.25f;
            Vector2 position = shootParams.position;
            Vector2 velocity = shootParams.fireVelocity * 3;
            int damage = shootParams.damage;
            float knockback = shootParams.knockBack;
            SoundEngine.PlaySound(shootSound, position);

            float numberProjectiles = 3;
            float rotation = MathHelper.ToRadians(15);
            position += Vector2.Normalize(new Vector2(velocity.X, velocity.Y)) * 25f;

            for (int i = 0; i < numberProjectiles; i++)
            {
                Vector2 perturbedSpeed = new Vector2(velocity.X, velocity.Y).RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (numberProjectiles - 1))) * .4f; // This defines the projectile roatation and speed. .4f == projectile speed
                var crossshot = Projectile.NewProjectileDirect(source, position, perturbedSpeed, shootParams.projToShoot, damage, knockback, player.whoAmI);
                crossshot.GetGlobalProjectile<CrossbowGlobalProjectile>().isCrossbowShot = true;
            }
            Projectile.NewProjectile(source, shootParams.position, shootParams.fireVelocity * 6, ModContent.ProjectileType<Waterway>(), shootParams.damage, shootParams.knockBack, player.whoAmI, ai1: 1);
        }

        public override void StaminaShootBow(Player player, EntitySource_ItemUse_WithAmmo source, ShootParams shootParams)
        {
            Projectile.NewProjectile(source, shootParams.position, shootParams.fireVelocity * 6, ModContent.ProjectileType<Waterway>(), shootParams.damage, shootParams.knockBack, player.whoAmI);
        }

        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew(mold: ModContent.ItemType<BlankBow>(), material: ModContent.ItemType<MusicalHarmonise>());
        }
    }


    public class Waterway : ModProjectile
    {
        private int trailingMode;
        private ref float Timer => ref Projectile.ai[0];
        public override string Texture => TextureRegistry.EmptyTexture;

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return ProjectileHelper.OldPosColliding(Projectile.oldPos, projHitbox, targetHitbox, 32);
        }
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            ProjectileID.Sets.TrailCacheLength[Type] = 36;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.tileCollide = true;
            Projectile.friendly = true;
            Projectile.light = 1.5f;
            Projectile.timeLeft = 300;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 15;
        }
        public override void AI()
        {
            base.AI();
            if(Timer == 1)
            {
                if (Projectile.ai[1] == 0)
                {
                    foreach (var proj in Main.ActiveProjectiles)
                    {
                        if (proj.owner == Projectile.owner && proj.type == Projectile.type)
                            proj.ai[1] = 0;
                    }
                }
            }

            Timer++;
            if (Timer % 8 == 0)
            {
                var sp = SparkleParticle.Spawn(Projectile.Center, Vector2.Zero, Scale: 0.3f);
                sp.outerColor = Color.Aqua;
            }
            if (this.OwnedByLocalClient())
            {
                Player player = Main.player[Projectile.owner];
                Vector2 value = Main.MouseWorld;
                float interpMult = 0.6f;
                if (Projectile.ai[1] == 1)
                {
                    value = player.Center;
                    interpMult = 0.4f;
                }
             
                if (Vector2.Distance(Projectile.Center, Main.MouseWorld) >= 64f)
                {
                    Vector2 v = value - Projectile.Center;
                    Vector2 vector2 = v.SafeNormalize(Vector2.Zero);
                    float num8 = System.Math.Min(32, v.Length());
                    Vector2 value2 = vector2 * num8;
                    if (Projectile.velocity.Length() < 4f)
                    {
                        Projectile.velocity += Projectile.velocity.SafeNormalize(Vector2.Zero).RotatedBy(0.7853981852531433).SafeNormalize(Vector2.Zero) * 4f;
                    }
                    if (Projectile.velocity.HasNaNs())
                    {
                        Projectile.Kill();
                    }
                    Projectile.velocity = Vector2.Lerp(Projectile.velocity, value2, 0.05f * interpMult);
                }
                else
                {
                    Projectile.velocity *= 0.3f;
                    Projectile.velocity += (value - Projectile.Center) * 0.15f * interpMult;
                }
                Projectile.netUpdate = true;
            }

        }


        private Color ColorFunction(float completionRatio)
        {
            Color c = Color.Blue;
            switch (trailingMode)
            {
                default:
                case 0:
                    c = Color.Lerp(Color.White, Color.Black, completionRatio);
                    c.A = 0;
                    break;
                case 1:
                    c.A = 0;
                    break;
                case 2:
                    c.A = 0;
                    break;
            }

            return c;
        }

        private float WidthFunction(float completionRatio)
        {
            float baseWidth = 32;
            baseWidth *= MathHelper.SmoothStep(0f, 1f, Timer / 30f);
            baseWidth *= MathHelper.SmoothStep(0f, 1f, (float)Projectile.timeLeft / 30f);
            if (Projectile.ai[1] == 1)
                baseWidth *= 0.6f;
            float width = baseWidth * 1.3f;
            completionRatio = EasingFunction.QuadraticBump(completionRatio);
            switch (trailingMode)
            {
                default:
                case 0:
                    return MathHelper.Lerp(0, width, completionRatio);
                case 1:
                    return MathHelper.Lerp(0, width, completionRatio);
                case 2:
                    return MathHelper.Lerp(0, width + 12, completionRatio);
            }
        }


        private void DrawMainShader(Vector2[] oldPos)
        {
            trailingMode = 0;
            var shader = MagicRadianceOutlineShader.Instance;
            shader.PrimaryTexture = TrailRegistry.DottedTrail;
            shader.NoiseTexture = TrailRegistry.CloudsSmall;

            Color c = Color.LightSkyBlue;
            shader.PrimaryColor = c;
            shader.NoiseColor = Color.DarkBlue;
            shader.BlendState = BlendState.AlphaBlend;
            shader.SamplerState = SamplerState.PointWrap;
            shader.Speed = 0.8f;
            shader.Distortion = 0.25f;
            shader.Power = 0.25f;

            TrailDrawer.Draw(Main.spriteBatch, oldPos, Projectile.oldRot, ColorFunction, WidthFunction, shader);
            /*
            trailingMode = 0;
            var shader = MagicRadianceShader.Instance;
            shader.PrimaryTexture = TrailRegistry.BeamTrail;
            shader.NoiseTexture = TrailRegistry.CloudsSmall;
            shader.OutlineTexture = TrailRegistry.DottedTrailOutline;
            shader.PrimaryColor = Color.Lerp(Color.White, new Color(255, 207, 79), 0.5f);
            shader.NoiseColor = Color.Transparent;
            shader.OutlineColor = Color.Transparent;
            shader.BlendState = BlendState.Additive;
            shader.SamplerState = SamplerState.PointWrap;
            shader.Speed = 0.8f;
            shader.Distortion = 0.25f;
            shader.Power = 0.5f;
         //   shader.Threshold = 0.2f;
            //This just applis the shader changes
            TrailDrawer.Draw(Main.spriteBatch, oldPos, Projectile.oldRot, ColorFunction, WidthFunction, shader);
            */
        }

        private void DrawOutlineShader(Vector2[] oldPos)
        {
            trailingMode = 1;
            var shader = MagicRadianceOutlineShader.Instance;
            shader.PrimaryTexture = TrailRegistry.DottedTrailOutline;
            shader.NoiseTexture = TrailRegistry.CloudsSmall;

            Color c = new Color(38, 204, 255);
            shader.PrimaryColor = c;
            shader.NoiseColor = c;
            shader.BlendState = BlendState.AlphaBlend;
            shader.SamplerState = SamplerState.PointWrap;
            shader.Speed = 0.8f;
            shader.Distortion = 0.25f;
            shader.Power = 2.5f;

            TrailDrawer.Draw(Main.spriteBatch, oldPos, Projectile.oldRot, ColorFunction, WidthFunction, shader);
        }

        private void DrawOutlineShader2(Vector2[] oldPos)
        {
            trailingMode = 2;
            var shader = MagicRadianceOutlineShader.Instance;
            shader.PrimaryTexture = TrailRegistry.DottedTrailOutline;
            shader.NoiseTexture = TrailRegistry.CloudsSmall;

            Color c = Color.White;
            shader.PrimaryColor = c;
            shader.NoiseColor = c;
            shader.BlendState = BlendState.AlphaBlend;
            shader.SamplerState = SamplerState.PointWrap;
            shader.Speed = 0.8f;
            shader.Distortion = 0.25f;
            shader.Power = 3.5f;

            TrailDrawer.Draw(Main.spriteBatch, oldPos, Projectile.oldRot, ColorFunction, WidthFunction, shader);
        }

        private void RenderPixelatedWater(GraphicsDevice gDevice)
        {
            Vector2[] oldPos = Projectile.oldPos;
            DrawMainShader(oldPos);
          //  DrawOutlineShader(oldPos);
            DrawOutlineShader2(oldPos);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            PixelationManager.QueuePrimitivesDrawAction(RenderPixelatedWater);
            return false;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.velocity.X != oldVelocity.X)
                Projectile.velocity.X = -oldVelocity.X;
            if (Projectile.velocity.Y != oldVelocity.Y)
                Projectile.velocity.Y = -oldVelocity.Y;
            return false;
        }
        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
        }
    }
}