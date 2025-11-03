using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Assets;
using Stellamod.Content.Gores;
using Stellamod.Core.Camera;
using Stellamod.Core.Particles;
using Stellamod.Core.Shaders;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Projectiles.Paint;
using Stellamod.Trails;
using Stellamod.Visual.Particles;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Dungeon.BossesDG.Bisinine.Projectiles
{
    public class FallingBell : ModProjectile,
        IDrawOutlines
    {
        private int _textureIndex;
        private float _bounceDirection;
        private float _randScale;
        private float _alpha;
        private Vector2 _squishScale;
        private Color _outlineColor;
        private ref float Timer => ref Projectile.ai[0];
        private ref float BounceCount => ref Projectile.ai[1];
        private ref float BounceHeight => ref Projectile.ai[2];
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            ProjectileID.Sets.TrailCacheLength[Type] = 16;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 48;
            Projectile.height = 48;
            Projectile.penetrate = -1;
            Projectile.hostile = true;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            base.SendExtraAI(writer);
            writer.Write(_bounceDirection);
            writer.Write(_randScale);
            writer.Write(_textureIndex);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            base.ReceiveExtraAI(reader);
            _bounceDirection = reader.ReadSingle();
            _randScale = reader.ReadSingle();
            _textureIndex = reader.ReadInt32();
        }

        public override void AI()
        {
            base.AI();
 
            if(this.OwnedByLocalClient() && _randScale== 0f)
            {
                _randScale = Main.rand.NextFloat(0.6f, 1.6f);
                _textureIndex = Main.rand.Next(4);
                Projectile.netUpdate = true;
            }

            if(BounceCount >= 2)
            {
                Timer++;
                if(Timer >= 120)
                {
                    Projectile.Kill();
                }
                if(Timer >= 60)
                {
                    _outlineColor = Color.Lerp(_outlineColor, Color.Transparent, 0.1f);
                    Projectile.hostile = false;
                }
               
                _alpha = MathHelper.Lerp(1f, 0f, EasingFunction.InOutSine((Timer - 60f) / 60f));

                Projectile.velocity.Y += 1;
                Projectile.rotation += Projectile.velocity.X * 0.04f;
                Projectile.tileCollide = true;
            } else
            {
                _outlineColor = Color.Red;
                _alpha = 1f;
                Projectile.velocity.X = _bounceDirection * 5;
                if (Projectile.velocity.Y < 20)
                    Projectile.velocity.Y += 1;
                if (Projectile.Bottom.Y < BounceHeight)
                {
                    Projectile.tileCollide = false;
                }
                else
                {
                    Projectile.tileCollide = true;
                }
            }
                _squishScale = Vector2.Lerp(_squishScale, Vector2.One, 0.1f);

            if (Main.rand.NextBool(8))
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<GlowSparkleDust>(), newColor: Color.White, Scale: 0.5f);
            }
        
            Projectile.rotation += Projectile.velocity.Length() * 0.025f * _bounceDirection;
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {

            if (Projectile.velocity.Y != oldVelocity.Y && BounceCount < 2)
            {
                if (this.OwnedByLocalClient() && BounceCount == 0)
                {
                    Player target = PlayerHelper.FindClosestPlayer(Projectile.position, 2000);
                    if (target != null)
                    {
                        _bounceDirection = target.Center.X < Projectile.Center.X ? -1 : 1;
                    }
                    Projectile.netUpdate = true;
                }

                if(BounceCount == 0)
                    Projectile.velocity.Y = -oldVelocity.Y * 0.85f;
                BounceCount++;
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
                foreach (int g in gores)
                {
                    Gore.NewGore(Projectile.GetSource_FromThis(),
                        Projectile.Center,
                        -Vector2.UnitY.RotatedByRandom(MathHelper.ToRadians(20)) * Main.rand.NextFloat(5f, 15f), g, Main.rand.NextFloat(0f, 1f));
                }

                FXUtil.ShakeCamera(Projectile.position, 1024, 8);
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.SilverCoin);
                SoundStyle bellHitSound = Main.rand.NextBool(2) ? AssetRegistry.Sounds.Bishinine.BellHit1 :  AssetRegistry.Sounds.Bishinine.BellHit2;
                bellHitSound.PitchVariance = 0.3f;
                SoundEngine.PlaySound(bellHitSound, Projectile.position);
            }
        
            return false;
        }

        private Texture2D GetTexture()
        {
            switch (_textureIndex)
            {
                default:
                    return ModContent.Request<Texture2D>($"{Texture}_{_textureIndex}").Value;
                case 0:
                    return ModContent.Request<Texture2D>(Texture).Value;
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            Texture2D texture = GetTexture();
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            Vector2 drawOrigin = texture.Size() / 2f;
            float drawRotation = Projectile.rotation;
            Vector2 drawScale = _squishScale * Projectile.scale * _randScale;
            SpriteEffects spriteEffects = SpriteEffects.None;
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                Vector2 oldPos = Projectile.oldPos[i];
                Vector2 oldDrawPos = oldPos - Main.screenPosition;
                float f = i;
                float interpolant = f / (float)Projectile.oldPos.Length;
                Color fadeColor = Color.Lerp(Color.White, Color.Transparent, interpolant) * 0.03f;
                oldDrawPos += Projectile.Size / 2f;
                fadeColor *= _alpha;
                spriteBatch.Draw(texture, oldDrawPos, null, fadeColor, Projectile.oldRot[i], drawOrigin, drawScale, spriteEffects, 0f);
            }

            spriteBatch.Draw(texture, drawPos, null, Color.White.MultiplyRGB(lightColor) * _alpha, drawRotation, drawOrigin, drawScale, spriteEffects, 0f);
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
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
            Vector2 scale = _squishScale * Projectile.scale * _randScale;
            float rotation = Projectile.rotation;

            spriteBatch.Draw(texture, drawPos + left, drawFrame, outlineColor, rotation, drawOrigin, scale, spriteEffects, 0);
            spriteBatch.Draw(texture, drawPos + right, drawFrame, outlineColor, rotation, drawOrigin, scale, spriteEffects, 0);
            spriteBatch.Draw(texture, drawPos + up, drawFrame, outlineColor, rotation, drawOrigin, scale, spriteEffects, 0);
            spriteBatch.Draw(texture, drawPos + down, drawFrame, outlineColor, rotation, drawOrigin, scale, spriteEffects, 0);
        }
    }

    public class RisingScythe : ModProjectile,
        IDrawOutlines
    {
        private float _alpha;
        private bool _bounced;
        private float _bounceHeight;
        private ref float Timer => ref Projectile.ai[0];
        private ref float AttackNumber => ref Projectile.ai[2];
        private AIState State
        {
            get => (AIState)Projectile.ai[1];
            set => Projectile.ai[1] = (float)value;
        }
        private enum AIState
        {
            Flyup,
            FallingBells
        }
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            ProjectileID.Sets.TrailCacheLength[Type] = 8;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
              
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            base.SendExtraAI(writer);
            writer.Write(_bounceHeight);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            base.ReceiveExtraAI(reader);
            _bounceHeight = reader.ReadSingle();
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if(this.OwnedByLocalClient() && State != AIState.FallingBells)
            {
                Projectile.velocity.Y = -oldVelocity.Y * 0.5f;
                SwitchState(AIState.FallingBells);
            } else if (State == AIState.FallingBells)
            {
                if (!_bounced)
                {
                    Projectile.velocity.Y = -oldVelocity.Y;
                    _bounced = true;
                }
               
          
            }
            return false;
        }
        
        public override void AI()
        {
            base.AI();
            switch (State)
            {
                case AIState.Flyup:
                    AI_Flyup();
                    break;
                case AIState.FallingBells:
                    AI_FallingBells();
                    break;
            }
        }

        private void SwitchState(AIState state)
        {
            if (this.OwnedByLocalClient())
            {
                State = state;
                Timer = 0;
                Projectile.netUpdate = true;
            }
        }
        private void AI_Flyup()
        {
            _alpha = 1;
            Timer++;
            if(Timer == 1)
            {
                _bounceHeight = Projectile.Center.Y;
            }
            Projectile.rotation += 0.5f;
            if(Timer >= 60)
            {
                SwitchState(AIState.FallingBells);
            }
        }

        private void AI_FallingBells()
        {
            Timer++;
            if(Timer == 1)
            {
                int[] gores = AutoGoreLoader.FindGores("GrayRock");
                foreach (int g in gores)
                {
                    Gore.NewGore(Projectile.GetSource_FromThis(),
                        Projectile.Center,
                        -Vector2.UnitY.RotatedByRandom(MathHelper.ToRadians(20)) * Main.rand.NextFloat(5f, 15f), g, Main.rand.NextFloat(0f, 1f));
                }

                var p = Particle.NewBlackParticle<BlackSmokeParticle>(Projectile.Top, Vector2.Zero, Color.DarkGray);

                p.color *= 0.25f;
                p.fadeToColor = Color.Black;
                p.innerColor = Color.DarkGray;
                p.outerColor = Color.Black;


                var p2 = Particle.NewParticle<GlowDonutParticle>(Projectile.Top, -Vector2.UnitY);

                for(float f = 0; f < 8f; f++)
                {
                    Vector2 vel = -Vector2.UnitY;
                    vel = vel.RotatedByRandom(MathHelper.ToRadians(45));
                    vel *= Main.rand.NextFloat(5, 35);
                    FXUtil.GlowStretch(Projectile.Center, vel);
                }
                FXUtil.ShakeCamera(Projectile.position, 1024, 32);
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.SilverCoin);
                SoundStyle bellHitSound = AssetRegistry.Sounds.Bishinine.BishinineBellSmash;
                bellHitSound.PitchVariance = 0.3f;
                SoundEngine.PlaySound(bellHitSound, Projectile.position);

            }
            OffsetCameraModifier.FocusTargetOffset = new Vector2(0, -150);
            Projectile.velocity.X = MathHelper.Lerp(-5, 0, EasingFunction.InOutSine(Timer / 30f));
            if (!_bounced)
            {
                if (Projectile.velocity.Y < 20)
                    Projectile.velocity.Y += 1;
            }
            else
            {
                Projectile.velocity.Y *= 0.94f;
                Projectile.velocity.Y += 0.2f;
                _alpha *= 0.9f;
            }

                Projectile.rotation += Projectile.velocity.Length() * 0.05f;
            if(Timer % 30 == 0)
            {
                AttackNumber++;
                if (this.OwnedByLocalClient())
                {
                    Vector2 pos = Projectile.Center;
                    pos.Y -= 600;

                    Player target = PlayerHelper.FindClosestPlayer(Projectile.position, 2000);
                    if(target != null)
                    {
                        pos.X = target.Center.X + Main.rand.NextFloat(-1000, 1000);
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), pos, Vector2.Zero,
                            ModContent.ProjectileType<FallingBell>(), Projectile.damage, Projectile.knockBack, Projectile.owner, ai2: _bounceHeight);
                    }

                }
            }

            if(AttackNumber >= 7)
            {
                Projectile.Kill();
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            string texturePath = Texture;
            Texture2D texture = ModContent.Request<Texture2D>(texturePath).Value;
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
                Color fadeColor = Color.Lerp(Color.White, Color.Transparent, interpolant) * 0.06f;
                oldDrawPos += Projectile.Size / 2f;
                spriteBatch.Draw(texture, oldDrawPos, null, fadeColor * _alpha, Projectile.oldRot[i], drawOrigin, drawScale, spriteEffects, 0f);
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
                Color fadeColor = Color.Lerp(Color.White, Color.Blue, interpolant) * 0.12f;
                fadeColor *= (1.0f - interpolant);
                fadeColor.A = 0;
                oldDrawPos += Projectile.Size / 2f;
                spriteBatch.Draw(starTexture, oldDrawPos, null, fadeColor * _alpha, Projectile.oldRot[i], sdrawOrigin, Projectile.scale * 1.2f, SpriteEffects.None, 0f);
            }
            spriteBatch.Draw(texture, drawPos, null, Color.White.MultiplyRGB(lightColor) * _alpha, drawRotation, drawOrigin, drawScale, spriteEffects, 0f);
            return false;
        }
        
        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
        }

        public void DrawOutlines(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
        {
            this.OutlineNoRestart(Color.Red * _alpha, ref lightColor, Vector2.One);
        }
    }
}
