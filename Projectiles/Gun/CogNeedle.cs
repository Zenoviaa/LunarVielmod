using Microsoft.Xna.Framework;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Trails;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;


namespace Stellamod.Projectiles.Gun
{
    internal class CogNeedle : ModProjectile
    {
        private int _targetNpc = -1;
        private Vector2 _targetOffset;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 40;
            Projectile.height = 8;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = true;
            Projectile.penetrate = 7;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 45;
        }

        public float WidthFunction(float completionRatio)
        {
            float baseWidth = Projectile.scale * Projectile.width * 0.5f;
            return MathHelper.SmoothStep(baseWidth, 3.5f, completionRatio);
        }

        public Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(Color.OrangeRed, Color.Transparent, completionRatio);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if(_targetNpc == -1)
            {
                DrawHelper.DrawSimpleTrail(Projectile, WidthFunction, ColorFunction, TrailRegistry.SpikyTrail1);
                DrawHelper.DrawAdditiveAfterImage(Projectile, Color.OrangeRed, Color.Transparent, ref lightColor);
            }

            return base.PreDraw(ref lightColor);
        }

        public override void AI()
        {
            if(_targetNpc != -1)
            {
                NPC target = Main.npc[_targetNpc];
                if (!target.active)
                {
                    Projectile.Kill();
                }

                Vector2 targetPos = target.position - _targetOffset;
                Vector2 directionToTarget = Projectile.position.DirectionTo(targetPos);
                float dist = Vector2.Distance(Projectile.position, targetPos);
                Projectile.velocity = (directionToTarget * dist) + new Vector2(0.001f, 0.001f);
            }
            else
            {
                Projectile.velocity *= 1.01f;
                Projectile.rotation = Projectile.velocity.ToRotation();
            }

           
      
            Vector3 RGB = new(1.00f, 0.37f, 0.30f);
            // The multiplication here wasn't doing anything
            Lighting.AddLight(Projectile.position, RGB.X, RGB.Y, RGB.Z);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Bleeding, 180);       
            target.AddBuff(BuffID.Poisoned, 180);
            if(_targetNpc == -1)
            {
                _targetNpc = target.whoAmI;
                _targetOffset = (target.position - Projectile.position) + new Vector2(0.001f, 0.001f); 
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero,
                    ModContent.ProjectileType<NailKaboom>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
            }
        }

        public override void OnKill(int timeLeft)
        {

            float spread = 0.4f;

            Vector2 direction = Projectile.Center.RotatedByRandom(spread);
            for (int i = 0; i <= 10; i++)
            {
                Dust.NewDust(base.Projectile.position, 22, 22, ModContent.DustType<GlowLineDust>(), base.Projectile.velocity.X * 0.5f, base.Projectile.velocity.Y * 0.5f);
                Dust.NewDust(base.Projectile.position, 22, 22, ModContent.DustType<GlowDust>(), 0f, 0f, 0, new Color(150, 80, 40), 0.3f);
            }
        }
    }
}
