using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using Stellamod.Assets;
using Stellamod.Content.Gores;
using Stellamod.Core.Particles;
using Stellamod.Core.Shaders;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Visual.Particles;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Dungeon.BossesDG.Bisinine.Projectiles
{
    public class BigBell : ModProjectile,
        IDrawOutlines
    {
        private float _targetScale;
        private float _bounceDirection;
        private bool _hasBounced;
        private float _bounceCount;
        private Vector2 _squishScale;
        private bool _playChargeSound;
        private Color _outlineColor;
        private int _sizeIndex;
        private ref float Timer => ref Projectile.ai[0];
        private ref float ShouldGrow => ref Projectile.ai[1];

        private enum AIState
        {
            Hold,
            Grow,
            Throw
        }

        private AIState State
        {
            get => (AIState)Projectile.ai[2];
            set => Projectile.ai[2] = (float)value;
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            base.SendExtraAI(writer);
            writer.Write(_sizeIndex);
            writer.Write(_bounceDirection);
            writer.Write(_hasBounced);
            writer.Write(_bounceCount);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            base.ReceiveExtraAI(reader);
            _sizeIndex= reader.ReadInt32();
            _bounceDirection = reader.ReadSingle();
            _hasBounced = reader.ReadBoolean();
            _bounceCount = reader.ReadSingle();
        }

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            ProjectileID.Sets.TrailCacheLength[Type] = 16;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 64;
            Projectile.height = 64;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
        }


        public override void AI()
        {
            base.AI();
            if (!_playChargeSound)
            {
                SoundStyle chargeSound = AssetRegistry.Sounds.Bishinine.BishinineChargeBell;
                SoundEngine.PlaySound(chargeSound, Projectile.position);
                _playChargeSound = true;
            }
            _squishScale = Vector2.Lerp(_squishScale, Vector2.One, 0.1f);
            if (Main.rand.NextBool(8))
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<GlowSparkleDust>(), newColor: Color.White, Scale: 0.5f);
            }
            Projectile.scale = MathHelper.Lerp(Projectile.scale, _targetScale, 0.1f);
            switch (State)
            {
                case AIState.Hold:
                    AI_Hold();
                    break;
                case AIState.Grow:
                    AI_Grow();
                    break;
                case AIState.Throw:
                    AI_Throw();
                    break;
            }
        }

        private void SwitchState(AIState state)
        {
            if (this.OwnedByLocalClient())
            {
                Timer = 0;
                State = state;
                Projectile.netUpdate = true;
            }
        }

        private void AI_Hold()
        {
            _outlineColor = Color.Yellow;
            Timer++;
            if(ShouldGrow == 1)
            {
                SwitchState(AIState.Grow);
            } else if (ShouldGrow == 2)
            {
                SwitchState(AIState.Throw);
            }
        }

        private void AI_Grow()
        {
            _targetScale = MathHelper.Lerp(0.5f, 1f, _sizeIndex / 3f);
            ShouldGrow = 0;
            _outlineColor = Color.Yellow;
            if(_sizeIndex < 3)
            {
     
                _sizeIndex++;
            }
            if (_sizeIndex == 3)
            {
                SoundStyle bigbellReady = AssetRegistry.Sounds.Bishinine.BigBallready;
                SoundEngine.PlaySound(bigbellReady, Projectile.position);
            }

            SwitchState(AIState.Hold);
        }

        private void AI_Throw()
        {
            _outlineColor = Color.Red;
            Timer++;
            if(Timer == 1)
            {
                if (this.OwnedByLocalClient())
                {
                    Player target = PlayerHelper.FindClosestPlayer(Projectile.position, 2000);
                    if (target != null)
                    {
                        _bounceDirection = target.Center.X < Projectile.Center.X ? -1 : 1;
                    }
                    _bounceCount++;
                    Projectile.velocity.X = _bounceDirection * 18;
                    Projectile.velocity.Y = -8;
                    Projectile.netUpdate = true;

                 
                }
                var p = Particle.NewParticle<GlowDonutParticle>(Projectile.Center, -Projectile.velocity);
                var p2 = Particle.NewParticle<GlowDonutParticle>(Projectile.Center, -Projectile.velocity * 2);
                p2.Scale *= 0.5f;
            }

            if (_hasBounced)
            {
                Projectile.velocity.X = _bounceDirection * 5;
            }
            if (_bounceCount >= 12)
                Projectile.Kill();
            if (Projectile.velocity.Y < 20)
                Projectile.velocity.Y += 1;
            Projectile.rotation += Projectile.velocity.X * 0.05f;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.velocity.Y != oldVelocity.Y)
            {
                if (this.OwnedByLocalClient() )
                {
                    Player target = PlayerHelper.FindClosestPlayer(Projectile.position, 4000);
                    if (target != null)
                    {
                        _bounceDirection = target.Center.X < Projectile.Center.X ? -2 : 2;
                        _bounceDirection *= Main.rand.NextFloat(0.5f, 1f);
                        _hasBounced = true;
                    }
                    _bounceCount++;
                    Projectile.netUpdate = true;
                }

                Projectile.velocity.Y = -oldVelocity.Y * 1;
                _squishScale = new Vector2(0.8f, 1.2f);

                for (float f = 0; f < 5f; f++)
                {
                    Vector2 pos = Projectile.Center;
                    pos += Main.rand.NextVector2Circular(16, 16);
                    Vector2 velocity = Main.rand.NextVector2Circular(16, 3);
                    var p = Particle.NewBlackParticle<BlackSmokeParticle>(pos, velocity, Color.DarkGray);
                    p.Scale *= 0.25f;
                    p.color *= 0.5f;
                    p.fadeToColor = Color.Black;
                    p.innerColor = Color.DarkGray;
                    p.outerColor = Color.Black;
                }
                int[] gores = AutoGoreLoader.FindGores("SilverBell");
                for(int i = 0; i < 2; i++)
                {
                    foreach (int g in gores)
                    {
                        Gore.NewGore(Projectile.GetSource_FromThis(),
                            Projectile.Center,
                            -Vector2.UnitY.RotatedByRandom(MathHelper.ToRadians(20)) * Main.rand.NextFloat(5f, 15f), g, Main.rand.NextFloat(0f, 1f));
                    }
                }

                var p3 = Particle.NewParticle<GlowDonutParticle>(Projectile.Center, Vector2.UnitY);

                FXUtil.ShakeCamera(Projectile.position, 1024, 24);
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.SilverCoin);
                SoundStyle bellHitSound = Main.rand.NextBool(2) ? AssetRegistry.Sounds.Bishinine.BellHit1 : AssetRegistry.Sounds.Bishinine.BellHit2;
                bellHitSound.PitchVariance = 0.3f;
                SoundEngine.PlaySound(bellHitSound, Projectile.position);

                SoundStyle sound = AssetRegistry.Sounds.Bishinine.BigBellGroundhit;
                SoundEngine.PlaySound(sound, Projectile.position);
            }

            return false;
        }
        private Texture2D GetTexture()
        {
            string texturePath = Texture;
            int index = _sizeIndex - 1;

            if (index < 0)
                return ModContent.Request<Texture2D>(TextureRegistry.EmptyTexture).Value;
            switch (index)
            {
                default:
                    texturePath = $"{Texture}_{index}";
                    break;
                case 0:
                    texturePath = Texture;
                    break;
    
            }
            //test
            return ModContent.Request<Texture2D>(texturePath).Value;
        }


        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
            for (int i = 0; i < 7; i++)
            {
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.White, 1f).noGravity = true;
            }
            for (int i = 0; i < 7; i++)
            {
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<TSmokeDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.LightGray, 1f).noGravity = true;
            }


            FXUtil.ShakeCamera(Projectile.Center, 1024, 32);
            FXUtil.GlowCircleBoom(Projectile.Center,
                innerColor: Color.White,
                glowColor: Color.Black,
                outerGlowColor: Color.Black, duration: 25, baseSize: 0.24f);
          
        }

        public override bool PreDraw(ref Color lightColor)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            Texture2D texture = GetTexture();
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            Vector2 drawOrigin = texture.Size() / 2f;
            float drawRotation = Projectile.rotation;
            Vector2 drawScale = Vector2.One * Projectile.scale;
            SpriteEffects spriteEffects = SpriteEffects.None;
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                Vector2 oldPos = Projectile.oldPos[i];
                Vector2 oldDrawPos = oldPos - Main.screenPosition;
                float f = i;
                float interpolant = f / (float)Projectile.oldPos.Length;
                Color fadeColor = Color.Lerp(Color.White, Color.Transparent, interpolant) * 0.07f;
                oldDrawPos += Projectile.Size / 2f;
                spriteBatch.Draw(texture, oldDrawPos, null, fadeColor, Projectile.oldRot[i], drawOrigin, drawScale, spriteEffects, 0f);
            }
            Texture2D starTexture = ModContent.Request<Texture2D>(TextureRegistry.ZuiEffect).Value;
            Vector2 sdrawOrigin = starTexture.Size() / 2f;
            Color cometColor = Color.GhostWhite;
            cometColor.A = 0;

            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                Vector2 oldPos = Projectile.oldPos[i];
                Vector2 oldDrawPos = oldPos - Main.screenPosition;
                float f = i;
                float interpolant = f / (float)Projectile.oldPos.Length;
                Color fadeColor = Color.Lerp(Color.White, Color.Blue, interpolant) * 0.1f;
                fadeColor *= (1.0f - interpolant);
                fadeColor.A = 0;
                oldDrawPos += Projectile.Size / 2f;
                spriteBatch.Draw(starTexture, oldDrawPos, null, fadeColor, Projectile.oldRot[i], sdrawOrigin, Projectile.scale * 1.5f, SpriteEffects.None, 0f);
            }
            spriteBatch.Draw(texture, drawPos, null, Color.White.MultiplyRGB(lightColor), drawRotation, drawOrigin, drawScale, spriteEffects, 0f);
            return false;
        }

        public void DrawOutlines(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
        {
            Texture2D texture = GetTexture();
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            float outlineOffset = 2;
            Vector2 left = Vector2.UnitX * -outlineOffset;
            Vector2 right = Vector2.UnitX * outlineOffset;
            Vector2 up = Vector2.UnitY * -outlineOffset;
            Vector2 down = Vector2.UnitY * outlineOffset;
            SpriteEffects spriteEffects = SpriteEffects.None;
            SpriteWhiteShader whiteShader = SpriteWhiteShader.Instance;

            Color outlineColor = _outlineColor;
            Rectangle? drawFrame = null;
            Vector2 drawOrigin = texture.Size() / 2;
            Vector2 scale = Vector2.One * Projectile.scale;
            float rotation = Projectile.rotation;

            spriteBatch.Draw(texture, drawPos + left, drawFrame, outlineColor, rotation, drawOrigin, scale, spriteEffects, 0);
            spriteBatch.Draw(texture, drawPos + right, drawFrame, outlineColor, rotation, drawOrigin, scale, spriteEffects, 0);
            spriteBatch.Draw(texture, drawPos + up, drawFrame, outlineColor, rotation, drawOrigin, scale, spriteEffects, 0);
            spriteBatch.Draw(texture, drawPos + down, drawFrame, outlineColor, rotation, drawOrigin, scale, spriteEffects, 0);
        }
    }
}
