using Microsoft.Xna.Framework;
using ParticleLibrary;
using Stellamod.Buffs;
using Stellamod.Helpers;
using Stellamod.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Summons.MiracleSoul
{
    public class MiracleSoulCollectibleProj : ModProjectile
    {
        private bool _homeToOwner;
        public Vector3 HuntrianColorXyz;
        public override void SetStaticDefaults()
        {
            // Sets the amount of frames this minion has on its spritesheet
            Main.projFrames[Projectile.type] = 7;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.tileCollide = false;
            Projectile.hostile = false;
            Projectile.friendly = false;
            Projectile.timeLeft = 1200;
        }

        public override void AI()
        {
            //If collide with owner, apply buff to them
            Player owner = Main.player[Projectile.owner];
            Rectangle myRect = Projectile.getRect();
            Rectangle ownerRect = owner.getRect();
            if(Projectile.Colliding(myRect, ownerRect) && Projectile.active)
            {
                //Add the buff for 12 seconds
                int miracleDuration = 720;
                owner.AddBuff(ModContent.BuffType<MiracleBoost>(), miracleDuration);
                owner.Heal(2);
                MiraclePlayer miraclePlayer = owner.GetModPlayer<MiraclePlayer>();
                miraclePlayer.miracleLevel++;
                miraclePlayer.miracleTimeLeft = miracleDuration;
                Projectile.Kill();
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/SwordOfGlactia1"), Projectile.position);
                for (int i = 0; i < 64; i++)
                {
                    Vector2 speed = Main.rand.NextVector2CircularEdge(4f, 4f);
                    Particle p = ParticleManager.NewParticle(Projectile.Center, speed, ParticleManager.NewInstance<VoidParticle>(),
                        default(Color), 1/3f);
                    p.layer = Particle.Layer.BeforeProjectiles;
                }

                int combatText = CombatText.NewText(miraclePlayer.Player.getRect(), Color.Magenta, miraclePlayer.miracleLevel, true);
                CombatText numText = Main.combatText[combatText];
                numText.lifeTime = 60;
            }

            //Visuals
            float hoverSpeed = 5;
            float yVelocity = VectorHelper.Osc(1, -1, hoverSpeed);
            if(Vector2.Distance(Projectile.position, owner.position) <= 128)
            {
                _homeToOwner = true;
            }

            Projectile.velocity = Vector2.Lerp(Projectile.velocity, new Vector2(0, yVelocity), 0.1f);
            if (_homeToOwner)
            {
                Projectile.velocity = VectorHelper.VelocityDirectTo(Projectile.position, owner.position, 10);
            }

            if (Main.rand.NextBool(6))
            {
                Vector2 vel = new Vector2(Main.rand.NextFloat(-1f, 1f), -1f);
                var dust = Dust.NewDustPerfect(Projectile.Center, DustID.GemAmethyst, vel, Scale: 1.5f);
                dust.noGravity = true;
            }

            HuntrianColorXyz = DrawHelper.HuntrianColorOscillate(
                new Vector3(217, 48, 228),
                new Vector3(117, 1, 187),
                new Vector3(3, 3, 3), 0);

            DrawHelper.AnimateTopToBottom(Projectile, 4);
            Lighting.AddLight(Projectile.Center, Color.White.ToVector3() * 0.28f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Vector3 huntrianColorXyz = DrawHelper.HuntrianColorOscillate(
                new Vector3(60, 0, 118),
                new Vector3(117, 1, 187),
                new Vector3(3, 3, 3), 0);

            DrawHelper.DrawDimLight(Projectile, huntrianColorXyz.X, huntrianColorXyz.Y, huntrianColorXyz.Z, ColorFunctions.MiracleVoid, lightColor, 1);
            DrawHelper.DrawAdditiveAfterImage(Projectile, new Color(217, 48, 228), new Color(117, 1, 187), ref lightColor);
            return true;
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 16; i++)
            {
                Vector2 speed = Main.rand.NextVector2CircularEdge(1f, 1f);
                var d = Dust.NewDustPerfect(Projectile.Center, DustID.GemAmethyst, speed, Scale: 3f);
                d.noGravity = true;
            }
        }
    }
}
