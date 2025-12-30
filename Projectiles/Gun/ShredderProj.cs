using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Common.Shaders;
using Stellamod.Common.Shaders.MagicTrails;
using Stellamod.Helpers;
using Stellamod.Trails;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Gun
{
    public class ShredderProj : ModProjectile,
        IPixelPrimitiveDrawer
    {
        private float _rotationSpeed;

        public override void OnSpawn(IEntitySource source)
        {
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                Projectile.oldPos[i] = Projectile.position;
            }
        }

        public override void SetStaticDefaults()
        {
            // Sets the amount of frames this minion has on its spritesheet
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 18;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.timeLeft = 180;
            Projectile.penetrate = -1;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 12;

        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.velocity = -Projectile.velocity;
            return false;
        }
        public override void AI()
        {
            float rotationDirection = Projectile.ai[0];
            Projectile.velocity = Projectile.velocity.RotatedBy(MathHelper.ToRadians(_rotationSpeed * rotationDirection));
            _rotationSpeed += 0.15f;

            //Dunno if this is needed but whatever
            Projectile.rotation = Projectile.velocity.ToRotation();
        }

        public float WidthFunction(float completionRatio)
        {
            float baseWidth = Projectile.scale * 4;
            return MathHelper.SmoothStep(baseWidth, 3.5f, completionRatio);
        }

        public Color ColorFunction(float completionRatio)
        {
            Color startColor = Color.Lerp(Color.Black, Color.Black, ExtraMath.Osc(0f, 1f, speed: 12));
            Color endColor = Color.Lerp(Color.Cyan, Color.Purple, ExtraMath.Osc(0f, 1f, speed: 12, offset: 4));
            return Color.Lerp(startColor, endColor, completionRatio);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            //This damages everything in the trail
            Vector2[] positions = Projectile.oldPos;
            float collisionPoint = 0;
            for (int i = 1; i < positions.Length; i++)
            {
                Vector2 position = positions[i];
                Vector2 previousPosition = positions[i - 1];
                if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), position, previousPosition, 6, ref collisionPoint))
                    return true;
            }
            return base.Colliding(projHitbox, targetHitbox);
        }

        public PrimitiveTrail BeamDrawer;
        public void DrawPixelPrimitives(SpriteBatch spriteBatch)
        {
            var shader = MagicNormalShader.Instance;
            shader.PrimaryTexture = TrailRegistry.SilkTrail;
            shader.NoiseTexture = TrailRegistry.CausticTrail;
            shader.BlendState = BlendState.Additive;
            shader.SamplerState = SamplerState.PointWrap;
            shader.Speed = 0.5f;
            shader.Repeats = 1f;
            //This just applis the shader changes
            TrailDrawer.Draw(Main.spriteBatch, Projectile.oldPos, Projectile.oldRot, ColorFunction, WidthFunction, shader, offset: Projectile.Size / 2);
        }
    }
}
