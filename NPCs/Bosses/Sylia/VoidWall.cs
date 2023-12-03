using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary;
using Stellamod.Helpers;
using Stellamod.Particles;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Bosses.Sylia
{
    internal class VoidWall : ModProjectile
    {
        private int _particleCounter = 0;
        private float _projSpeed = 2;

        private const int Body_Radius = 64;
        private const int Body_Particle_Count = 8;
        private const int Body_Particle_Rate = 2;
        public override void SetDefaults()
        {
            Projectile.width = Body_Radius;
            Projectile.height = Body_Radius * 16;
            Projectile.tileCollide = false;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.timeLeft = 30000;
		}

        private int _frameCounter;
        private int _frameTick;

        //Visual Stuffs
        public override bool PreDraw(ref Color lightColor)
        {
            //Draw The Body
            Vector3 huntrianColorXyz = DrawHelper.HuntrianColorOscillate(
                new Vector3(60, 0, 118),
                new Vector3(117, 1, 187),
                new Vector3(3, 3, 3), 0);

            DrawHelper.DrawDimLight(Projectile, huntrianColorXyz.X, huntrianColorXyz.Y, huntrianColorXyz.Z, new Color(60, 0, 118), lightColor, 1);
            SpriteEffects effects = SpriteEffects.None;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            Vector2 origin = new Vector2(58 / 2, 88 / 2);

            Texture2D voidMouthTexture = ModContent.Request<Texture2D>("Stellamod/NPCs/Bosses/Sylia/Projectiles/VoidMouth").Value;
            int frameSpeed = 2;
            int frameCount = 6;
            Main.EntitySpriteDraw(voidMouthTexture, drawPosition,
                voidMouthTexture.AnimationFrame(ref _frameCounter, ref _frameTick, frameSpeed, frameCount, true),
                Color.White, 0, origin, 1f, effects, 0f);

            return base.PreDraw(ref lightColor);
        }

        public override void AI()
        {
            //Movement Code
            //Wanna just home into enemies and then explode or something
            //On second thought, maybe have ai similar to charging type minions like optic staff
            //hmmm
            Player playerToHomeTo = Main.player[Main.myPlayer];
            float closestDistance = Vector2.Distance(Projectile.position, playerToHomeTo.position);
            for (int i = 0; i < Main.maxPlayers; i++)
            {
                Player player = Main.player[i];
                float distanceToPlayer = Vector2.Distance(Projectile.position, player.position);
                if (distanceToPlayer < closestDistance)
                {
                    closestDistance = distanceToPlayer;
                    playerToHomeTo = player;
                }
            }

            Vector2 homingVelocity = (playerToHomeTo.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * _projSpeed;
            Projectile.velocity = new Vector2(_projSpeed, homingVelocity.Y);
            Projectile.rotation = Projectile.velocity.ToRotation();
            Visuals();

            if (!NPC.AnyNPCs(ModContent.NPCType<Sylia>()) && Projectile.active)
            {
                Projectile.Kill();
            }
        }


        private void Visuals()
        {
            _particleCounter++;
            if (_particleCounter > Body_Particle_Rate)
            {
                for (int i = 0; i < Body_Particle_Count; i++)
                {
                    Vector2 position = Projectile.Center + Main.rand.NextVector2Circular(Body_Radius / 2, Body_Radius / 2);
                    float size = Main.rand.NextFloat(0.75f, 1f);
                    Particle p = ParticleManager.NewParticle(position, Vector2.Zero, ParticleManager.NewInstance<VoidParticle>(),
                        default(Color), size);

                    p.layer = Particle.Layer.BeforeNPCs;
                    Particle tearParticle = ParticleManager.NewParticle(position, Vector2.Zero, ParticleManager.NewInstance<VoidTearParticle>(),
                        default(Color), size + 0.025f);

                    tearParticle.layer = Particle.Layer.BeforePlayersBehindNPCs;
                }

                float halfWidth = Projectile.Size.X / 2;
                float halfHeight = Projectile.Size.Y / 2;
                for (int i = 0; i < Body_Particle_Count; i++)
                {
                    float x = Main.rand.NextFloat(-halfWidth, halfWidth);
                    float y = Main.rand.NextFloat(-halfHeight, halfHeight);
                    Vector2 position = Projectile.Center + new Vector2(x, y);
                    float size = Main.rand.NextFloat(0.75f, 1f);
                    Particle p = ParticleManager.NewParticle(position, Vector2.Zero, ParticleManager.NewInstance<VoidParticle>(),
                        default(Color), size);

                    p.layer = Particle.Layer.BeforeNPCs;
                }

                _particleCounter = 0;
            }
        }
    }
}
