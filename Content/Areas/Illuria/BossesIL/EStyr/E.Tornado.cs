using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Core.Shaders;
using Stellamod.Helpers;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Illuria.BossesIL.EStyr
{
    public class BlackTornadoDebris : ModProjectile,
        IDrawOutlines
    {
        private ref float Timer => ref Projectile.ai[0];
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 300;
        }
        public override void AI()
        {
            base.AI();
            Timer++;
            if(Timer % 10 == 0)
            {
                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(16, 16), DustID.Dirt, Vector2.Zero, 
                    newColor: Color.White,
                    Scale: 1);
            }
            float outScale = (float)Projectile.timeLeft / 15f;
            outScale = EasingFunction.InOutSine(outScale);
            Projectile.scale = 1f * outScale;
            Projectile.rotation += 0.01f * MathF.Sign(Projectile.velocity.X);
            Projectile.rotation += Projectile.velocity.Length() * 0.05f;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            DrawSprite(Main.spriteBatch, Projectile.Center - Main.screenPosition, lightColor);
            return false;
        }


        private void DrawSprite(SpriteBatch spriteBatch, Vector2 drawPosition, Color drawColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawOrigin = texture.Size() / 2f;
            spriteBatch.Draw(texture, drawPosition, null, drawColor, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
        }

        public void DrawOutlines(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
        {
            float outlineOffset = 2;
            Vector2 v = Vector2.UnitY * outlineOffset;
            Vector2 h = Vector2.UnitX * outlineOffset;
            DrawSprite(spriteBatch, Projectile.Center + v - screenPos, Color.Red);
            DrawSprite(spriteBatch, Projectile.Center - v - screenPos, Color.Red);
            DrawSprite(spriteBatch, Projectile.Center + h - screenPos, Color.Red);
            DrawSprite(spriteBatch, Projectile.Center - h - screenPos, Color.Red);
        }

        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
        }
    }


    public class BlackTornado : ModProjectile
    {
        private class BlackTornadoParticleManager
        {
            private float _timer;
            private Vector2[] _particles;
            public BlackTornadoParticleManager(int particleCount)
            {
                _particles = new Vector2[particleCount];
            }

    
            public void Update()
            {
                _timer++;
                for(int i = 0; i < _particles.Length; i++)
                {
                    float initialX = -100;
                    float initialY = 0;
                    float initialZ = -100;
                    Vector3 initialPosition = new Vector3(initialX, initialY, initialZ);

                    float radians = _timer + i * 0.02f;
                    Quaternion rotation = Quaternion.CreateFromAxisAngle(new Vector3(0, 0.75f, 0), radians);
                    Matrix rotationMatrix = Matrix.CreateFromQuaternion(rotation);
                    Vector3 rotatedPosition = Vector3.Transform(initialPosition, rotationMatrix);

                    //Set the new position
                    _particles[i] = new Vector2(rotatedPosition.X, rotatedPosition.Y);
                }
            }
        }
        private ref float Timer => ref Projectile.ai[0];
        private NPC Parent
        {
            get => Main.npc[(int)Projectile.ai[1]];

        }
        private BlackTornadoParticleManager _particleManagerBackingField;
        private BlackTornadoParticleManager ParticleManager
        {
            get
            {
                _particleManagerBackingField ??= new BlackTornadoParticleManager(100);
                return _particleManagerBackingField;
            }
        }
        public override string Texture => TextureRegistry.EmptyTexture;
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 64;
            Projectile.height = 64;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            base.AI();
            //For this projectile what we're going to need to do is create a tornado visual with projectiles coming outward and then coming in
            //Gustbeak has a tornado but it's meh
            //Gintzia's winds look a bit better and should look fine when combined with swirling particles
            //So make a new particle manager for this
            Timer++;
            ParticleManager.Update();
            Projectile.Center = Parent.Center;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }
    }
    public class TornadoSuckPlayer : ModPlayer
    {
        public Vector2? TornadoCenter;
        public float TornadoPullStrength;
        public override void PreUpdateMovement()
        {
            base.PreUpdateMovement();
            if (TornadoCenter.HasValue)
            {

                Vector2 tornadoCenter = TornadoCenter.Value;
                Vector2 tornadoPullDirection = tornadoCenter - Player.Center;
                tornadoPullDirection = tornadoPullDirection.SafeNormalize(Vector2.Zero);

                Player.velocity += tornadoPullDirection * TornadoPullStrength;
                TornadoCenter = null;
            }
        }
    }
    public partial class E
    {
        private int TornadoDamage => 45;

        /// <summary>
        /// Sucks in all players to him
        /// </summary>
        /// <param name="strength"></param>
        private void SuckAllPlayers(float strength)
        {
            foreach (var player in Main.ActivePlayers)
            {
                TornadoSuckPlayer tornadoSuckPlayer = player.GetModPlayer<TornadoSuckPlayer>();
                tornadoSuckPlayer.TornadoCenter = NPC.Center;
                tornadoSuckPlayer.TornadoPullStrength = strength;
            }
        }

        private void AI_TornadoStart()
        {
            Timer++;
            if (Timer == 1)
            {
                NPC.TargetClosest();
                TargetVector = NPC.velocity;
            }

            float startupTime = 60;
            float completionRatio = Timer / startupTime;
            float ease = EasingFunction.InOutSine(completionRatio);
            Vector2 positionToMoveTo = MyTarget.Center - new Vector2(0, 252);
            Vector2 targetVelocity = positionToMoveTo - NPC.Center;
            Vector2 interpolatedVelocity = Vector2.Lerp(TargetVector, targetVelocity, ease);
            NPC.velocity = interpolatedVelocity;
            if (Timer >= startupTime)
            {
                SwitchState(AIState.Tornado_PreSpin);
            }
        }

        private void AI_TornadoPreSpin()
        {
            //In this state, he'll slowly start speeding up and then creating the tornado, 
            //The earlier startup state is just to get him into the position
            //This is mostly done with a sound and animation, so not much happens here
            Timer++;

            float prespinTime = 30f;
            NPC.velocity *= 0.9f;
            if(Timer >= prespinTime)
            {
                SwitchState(AIState.Tornado_Spin);
            }
        }

        private void AI_TornadoSpin()
        {
            //Here the tornado projectile will actually spawn and we'll begin sucking in all of the players
            //At the same time we'll slowly move towards our target
            Timer++;
            if(Timer == 1)
            {
                if (MultiplayerHelper.IsHost)
                {
                    Projectile.NewProjectile(SourceFromThis, NPC.Center, Vector2.Zero, 
                        ModContent.ProjectileType<BlackTornado>(), TornadoDamage, 1, Main.myPlayer, ai1: NPC.whoAmI);
                }
            }

            float tornadoTime = 600;
            Vector2 targetNormal = (MyTarget.Center - NPC.Center).SafeNormalize(Vector2.Zero);
            float distanceToTarget = Vector2.Distance(NPC.Center, MyTarget.Center);
            if(distanceToTarget >= 500)
            {
                Vector2 targetVelocity = targetNormal * 20;
                NPC.velocity = Vector2.Lerp(NPC.velocity, targetVelocity, 0.1f);
            }
            else
            {
                NPC.velocity = Vector2.Lerp(NPC.velocity, targetNormal * 3, 0.1f);
            }

            //Unsure how strong this should actually be so make sure to balance this number properly
            float tornadoStrength = 3f;
            SuckAllPlayers(tornadoStrength);
            if(Timer >= tornadoTime)
            {
                SwitchState(AIState.Tornado_End);
            }
        }

        private void AI_TornadoEnd()
        {
            Timer++;
            if(Timer >= 15)
            {
                SwitchState(AIState.Idle);
            }
        }
    }
}
