using Microsoft.Xna.Framework;
using Stellamod.Helpers;
using Stellamod.Trails;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Spears
{
    internal class TheIrradiaspearSparkProj : ModProjectile
    {
        public override string Texture => TextureRegistry.EmptyTexture;
        public PrimDrawer TrailDrawer { get; private set; } = null;
        private float LifeTime => 45;
        private ref float Timer => ref Projectile.ai[0];
        private ref float RandOffset => ref Projectile.ai[1];
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 12;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.light = 0.2f;
        }

        public override void AI()
        {
            Timer++;
            float maxDetectDistance = 512;
            NPC closestNpc = NPCHelper.FindClosestNPC(Projectile.position, maxDetectDistance);
            if (closestNpc != null && Timer > (LifeTime + RandOffset) / 2)
            {
                Vector2 targetVelocity = Projectile.Center.DirectionTo(closestNpc.Center) * 25;
                Vector2 velocity = Vector2.Lerp(Projectile.velocity, targetVelocity, 0.08f);
                Projectile.velocity = velocity;
            }

            Projectile.velocity *= 0.98f;
            Projectile.rotation = Projectile.velocity.ToRotation() + Projectile.velocity.Length() * 0.05f;

            float lifeTime = LifeTime + RandOffset;
            if(Timer >= lifeTime)
            {
                Projectile.Kill();
            }
        }

        public float WidthFunction(float completionRatio)
        {
            float baseWidth = Projectile.scale * Projectile.width * 0.3f;
            return MathHelper.SmoothStep(baseWidth, 3.5f, completionRatio);
        }

        public Color ColorFunction(float completionRatio)
        {
            float progress = Timer / LifeTime;
            float minParticleSpawnSpeed = 8;
            float maxParticleSpawnSpeed = 2;
            int particleSpawnSpeed = (int)MathHelper.Lerp(minParticleSpawnSpeed, maxParticleSpawnSpeed, progress);
            if (Timer % particleSpawnSpeed == 0)
            {
                return Color.Lerp(ColorFunctions.AcidFlame, Color.Transparent, completionRatio);
            }
            else
            {
                return Color.Lerp(Color.White, Color.Transparent, completionRatio);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            TrailDrawer ??= new PrimDrawer(WidthFunction, ColorFunction, GameShaders.Misc["VampKnives:BasicTrail"]);
            GameShaders.Misc["VampKnives:BasicTrail"].SetShaderTexture(TrailRegistry.SimpleTrail);
            TrailDrawer.DrawPrims(Projectile.oldPos, Projectile.Size * 0.5f - Main.screenPosition, 155);
            return base.PreDraw(ref lightColor);
        }

        public override void OnKill(int timeLeft)
        {
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero,
                ModContent.ProjectileType<TheIrradiaspearExplosionProj>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
        }
    }
}
