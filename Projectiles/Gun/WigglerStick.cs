using Microsoft.Xna.Framework;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Projectiles.IgniterExplosions;
using Stellamod.UI.Systems;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Gun
{
    internal class WigglerStick : ModProjectile
    {
        private float _lighting;
        private bool _setOffset;
        private Vector2 _offset;
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 7;
        }

        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 60 * 10;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 25;
        }

        public override void AI()
        {
            int targetNpc = (int)Projectile.ai[0];
            NPC target = Main.npc[targetNpc];
            if (target.active && !_setOffset)
            {
                _offset = (target.position - Projectile.position) + new Vector2(0.001f, 0.001f);
                _setOffset = true;
            }
            else if (!target.active)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity,
                    ModContent.ProjectileType<WigglerShot>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                Projectile.Kill();
            }
            else
            {
                Vector2 targetPos = target.position - _offset + new Vector2(0.001f, 0.001f);
                Vector2 directionToTarget = Projectile.position.DirectionTo(targetPos);
                float dist = Vector2.Distance(Projectile.position, targetPos);
                Projectile.velocity = (directionToTarget * dist) + new Vector2(0.001f, 0.001f);
            }

            bool detonate = Projectile.ai[2] == 1;
            if (detonate)
            {
                ref float detonationTimer = ref Projectile.ai[1];
                detonationTimer--;
                if (detonationTimer == 10)
                {
                    SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/CombusterReady"), Projectile.position);
                    ExplodeEffects();
                }

                if (detonationTimer < 0)
                {
                    ShakeModSystem.Shake = 3;
                    SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Kaboom"), Projectile.position);
                    Boom();
                    Projectile.Kill();
                }

            }
            Projectile.rotation = Projectile.velocity.ToRotation();
            Visuals();
        }

        private void Boom()
        {
            if (Main.myPlayer == Projectile.owner)
            {
                int projType;
                switch (Main.rand.Next(3))
                {
                    default:
                    case 0:
                        projType = ModContent.ProjectileType<SparklyBoom>();
                        break;
                    case 1:
                        projType = ModContent.ProjectileType<BongoBoom>();
                        break;
                    case 2:
                        projType = ModContent.ProjectileType<SparklyBoom>();
                        break;
                }
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, projType, Projectile.damage * 3, Projectile.knockBack, Projectile.owner);
            }
        }
        private void ExplodeEffects()
        {
            for (float f = 0; f < 12; f++)
            {
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlyphDust>(),
                    (Vector2.One * Main.rand.NextFloat(0.2f, 5f)).RotatedByRandom(19.0), 0, Color.Blue, Main.rand.NextFloat(1f, 3f)).noGravity = true;
            }

            for (float i = 0; i < 4; i++)
            {
                float progress = i / 4f;
                float rot = progress * MathHelper.ToRadians(360);
                Vector2 offset = rot.ToRotationVector2() * 24;
                var particle = FXUtil.GlowCircleDetailedBoom1(Projectile.Center,
                    innerColor: Color.White,
                    glowColor: Color.Blue,
                    outerGlowColor: Color.Black,
                    baseSize: Main.rand.NextFloat(0.1f, 0.2f),
                    duration: Main.rand.NextFloat(12, 24f));
                particle.Rotation = rot + MathHelper.ToRadians(45);
            }
        }

        private void Visuals()
        {
            DrawHelper.AnimateTopToBottom(Projectile, 5);
        }

        public override void PostDraw(Color lightColor)
        {
            bool detonate = Projectile.ai[2] == 1;
            if (detonate)
            {
                _lighting += 0.01f;
                Lighting.AddLight(Main.screenPosition - Projectile.position, Color.White.ToVector3() * _lighting * Main.essScale); ;

            }
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 8; i++)
            {
                Vector2 speed = Main.rand.NextVector2CircularEdge(4f, 4f);
                var d = Dust.NewDustPerfect(Projectile.Center, DustID.GemSapphire, speed * 4);
                d.noGravity = true;
            }
        }
    }
}
