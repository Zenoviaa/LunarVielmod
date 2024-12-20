using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Common.Bases;
using Stellamod.Common.MaskingShaderSystem;
using Stellamod.Common.Particles;
using Stellamod.Common.Shaders;
using Stellamod.Common.Shaders.MagicTrails;
using Stellamod.Helpers;
using Stellamod.Trails;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Mage.Tomes
{
    internal class MothlightStar : BaseMagicTomeItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.shoot = ModContent.ProjectileType<MothlightStarTome>();
            Item.shootSpeed = 9;
        }
    }

    internal class MothlightStarTome : BaseMagicTomeProjectile
    {
        private float _dustTimer;
        public override string Texture => this.PathHere() + "/MothlightStar";
        public override void SetDefaults()
        {
            base.SetDefaults();
            //How often it shoots
            AttackRate = 12;

            //How fast it drains mana, better to change the mana use in the item instead of this tho
            ManaConsumptionRate = 4;

            //How far the tome is held from the player
            HoldDistance = 36;

            //The glow effect around it
            GlowDistanceOffset = 4;
            GlowRotationSpeed = 0.05f;
        }

        public override void AI()
        {
            base.AI();
            _dustTimer++;
            if (_dustTimer % 16 == 0)
            {
                Color color = Color.Lerp(Color.White, Color.LightBlue, Main.rand.NextFloat(0f, 1f));
                color.A = 0;
                Vector2 velocity = Main.rand.NextVector2Circular(2, 2);
                Vector2 spawnPoint = Projectile.Center + Main.rand.NextVector2Circular(4, 4);
                Particle.NewBlackParticle<GlowParticle>(spawnPoint, velocity, color, Scale: 0.33f);
            }
        }
        protected override void Shoot(Player player, IEntitySource source, Vector2 position, Vector2 velocity, int damage, float knockback)
        {
            base.Shoot(player, source, position, velocity, damage, knockback);
            Projectile.NewProjectile(source, position, velocity,
                ModContent.ProjectileType<MothlightStarBall>(), damage, knockback, Projectile.owner,
                ai1: Projectile.whoAmI);
            SoundStyle soundStyle = SoundRegistry.MothlightStarCast;
            soundStyle.PitchVariance = 0.12f;
            SoundEngine.PlaySound(soundStyle, position);
        }
    }

    internal class MothlightStarBall : ModProjectile,
        IPreDrawMaskShader,
        IDrawMaskShader
    {
        private Vector2 _drawScale;
        private float _scale;
        private float _scaleMult;
        private float _scaleOutMult;
        private bool _hasHitWall;
        private float _scaleOutTimer;
        private ref float Timer => ref Projectile.ai[0];
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            ProjectileID.Sets.TrailCacheLength[Type] = 24;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            _drawScale = Vector2.Zero;
            _scaleMult = Main.rand.NextFloat(0.8f, 1f);
            _scaleOutMult = 1f;
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 120;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 20;
        }

        public override void AI()
        {
            base.AI();
            Timer++;
            if (Timer % 4 == 0)
            {
                Color color = Color.Lerp(Color.White, Color.LightBlue, Main.rand.NextFloat(0f, 1f));
                color.A = 0;
                Vector2 velocity = Main.rand.NextVector2Circular(2, 2);
                Vector2 spawnPoint = Projectile.Center + Main.rand.NextVector2Circular(4, 4);
                //Dust.NewDustPerfect(spawnPoint, ModContent.DustType<MothlightDust>(), Velocity: velocity, Scale: 1f);
            }


            if (_hasHitWall)
            {
                _drawScale.X = MathHelper.Lerp(_drawScale.X, 0.2f * _scaleMult, 0.08f);
                _scale = _drawScale.X;
                Projectile.velocity *= 0.9f;
            }
            else
            {
                _drawScale = Vector2.Lerp(_drawScale, Vector2.One * 0.5f * _scaleMult, 0.04f);
                _scale = _drawScale.X;
                float maxHomingDetectDistance = 256f;
                float maxDegreesRotate = 10;
                NPC npcToChase = ProjectileHelper.FindNearestEnemy(Projectile.Center, maxHomingDetectDistance);
                if (npcToChase != null)
                    Projectile.velocity = ProjectileHelper.SimpleHomingVelocity(Projectile, npcToChase.Center, degreesToRotate: maxDegreesRotate);
                Projectile.rotation = Projectile.velocity.ToRotation();
            }

            if (Projectile.timeLeft <= 30)
            {
                _scaleOutTimer++;
                float scaleProgress = _scaleOutTimer / 30f;
                float easedProgress = Easing.InOutCubic(scaleProgress);
                _scaleOutMult = MathHelper.Lerp(1f, 0f, easedProgress);
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            _hasHitWall = true;
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }

        public MiscShaderData GetMaskDrawShader()
        {
            var shader = MagicMothlightDistortionShader.Instance;
            return shader.Data;
        }

        public void PreDrawMask(SpriteBatch spriteBatch)
        {
            var shader = MagicMothlightDistortionShader.Instance;
            shader.Data.Apply();
            shader.Apply();

            Texture2D texture = ModContent.Request<Texture2D>(Texture + "_Outline").Value;
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            Color drawColor = Color.White;
            Vector2 drawOrigin = texture.Size() / 2f;
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer,
                    shader.Effect, Main.GameViewMatrix.TransformationMatrix);

            spriteBatch.Draw(texture, drawPos, null, drawColor, Projectile.rotation, drawOrigin, _drawScale * _scaleOutMult, SpriteEffects.None, 0f);
            spriteBatch.End();
            spriteBatch.Begin();
        }

        private Color ColorFunc(float p)
        {
            return Color.White;
        }

        private float WidthFunc(float p)
        {
            return MathHelper.Lerp(64 * _scale * _scaleMult * _scaleOutMult, 0, Easing.OutExpo(p, 5));
        }

        public void DrawMask(SpriteBatch spriteBatch)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            Color drawColor = Color.White;
            Vector2 drawOrigin = texture.Size() / 2f;

            spriteBatch.Draw(texture, drawPos, null, drawColor, Projectile.rotation, drawOrigin, _drawScale * _scaleOutMult, SpriteEffects.None, 0f);

            var trailShader = SimpleTrailShader.Instance;
            trailShader.TrailingTexture = TrailRegistry.GlowTrailNoBlack;
            trailShader.SecondaryTrailingTexture = TrailRegistry.GlowTrailNoBlack;
            trailShader.TertiaryTrailingTexture = TrailRegistry.GlowTrailNoBlack;
            TrailDrawer.Draw(spriteBatch, Projectile.oldPos, Projectile.oldRot, ColorFunc, WidthFunc, trailShader, offset: Projectile.Size / 2f);
        }


    }
}
