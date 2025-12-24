using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Assets;
using Stellamod.Core.Particles;
using Stellamod.Core.Shaders;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Visual.Particles;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Dungeon.BossesDG.Bisinine.Projectiles
{
    public class BellBaseball : ModProjectile,
        IDrawOutlines
    {
        private Player _targetPlayer;
        private float _thrustDirection;
        private bool _hasHit;
        private bool _doHitEffects;
        private Vector2 _squishScale;
        private Color _outlineColor;
        private Color TargetOutlineColor;
        private ref float Timer => ref Projectile.ai[0];
        private ref float HitDirection => ref Projectile.ai[1];
        private ref float KillMyself => ref Projectile.ai[2];

        public bool IsReadyToHit { get; private set; }
        public override void SendExtraAI(BinaryWriter writer)
        {
            base.SendExtraAI(writer);
            writer.Write(_thrustDirection);
            writer.Write(_hasHit);
            writer.Write(_doHitEffects);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            base.ReceiveExtraAI(reader);
            _thrustDirection = reader.ReadSingle();
            _hasHit = reader.ReadBoolean();
            _doHitEffects = reader.ReadBoolean();
        }
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            ProjectileID.Sets.TrailCacheLength[Type] = 16;
            ProjectileID.Sets.TrailingMode[Type] = 3;
            ProjectileID.Sets.DrawScreenCheckFluff[Type] = 1000;

        }
        public override void SetDefaults()
        {
            base.SetDefaults();

            Projectile.width = 52;
            Projectile.height = 52;
            Projectile.tileCollide = false;
            Projectile.hostile = true;
        }

        public void HitEffects()
        {
            SoundStyle sound = AssetRegistry.Sounds.Bishinine.BishinineBellSmash;
            SoundEngine.PlaySound(sound, Projectile.position);

            LegacyParticle.NewParticle<GlowDonutParticle>(Projectile.Center, -Projectile.velocity.SafeNormalize(Vector2.Zero));
            var p = LegacyParticle.NewParticle<GlowDonutParticle>(Projectile.Center, -Projectile.velocity.SafeNormalize(Vector2.Zero) * 5);
            p.Scale *= 0.5f;
            for (float f = 0; f < 8; f++)
            {
                Vector2 vel = Projectile.velocity.SafeNormalize(Vector2.Zero);
                vel *= Main.rand.NextFloat(1f, 15f);
                vel = vel.RotatedByRandom(MathHelper.ToRadians(45));
                Vector2 position = Projectile.Center;
                position += Main.rand.NextVector2Circular(32, 32);
                Dust.NewDustPerfect(position, ModContent.DustType<GlowDust>(), vel, newColor: Color.Gray, Scale: Main.rand.NextFloat(0.2f, 2f));

                if (Main.rand.NextBool(4))
                {
                    FXUtil.GlowStretch(position, vel);
                }
            }


            SoundStyle bellHit = AssetRegistry.Sounds.Bishinine.BellHit1;
            bellHit.PitchVariance = 0.2f;
            SoundEngine.PlaySound(bellHit, Projectile.position);
            FXUtil.ShakeCamera(Projectile.position, 1024, 8);
            FXUtil.PunchCamera(Projectile.position, Projectile.velocity.SafeNormalize(Vector2.Zero), 8, 8, 8);
        }

        public override void AI()
        {
            base.AI();
            _targetPlayer = PlayerHelper.FindClosestPlayer(Projectile.position, 80000);
            _squishScale = Vector2.Lerp(_squishScale, Vector2.One, 0.1f);
            _outlineColor = Color.Lerp(_outlineColor, TargetOutlineColor, 0.1f);
            Timer++;
            if (_doHitEffects)
            {
                HitEffects();
                _doHitEffects = false;
            }

            if (this.OwnedByLocalClient() && HitDirection != 0)
            {
                _hasHit = true;
                _doHitEffects = true;
                _squishScale = new Vector2(0.8f, 1.2f);
                Timer = 0;


                Vector2 vel2 = (_targetPlayer.Center - Projectile.Center).SafeNormalize(Vector2.Zero);
                Vector2 velocity = vel2 * 22;
                velocity.Y -= 5;
                Projectile.velocity = velocity;
      
                HitDirection = 0;
                Projectile.netUpdate = true;
            }
      

            if(_targetPlayer != null)
            {
                Projectile.tileCollide = (Projectile.Top.Y + 32) > _targetPlayer.Top.Y;
                if (!_hasHit)
                {
                    if (Projectile.velocity.Y < 10)
                        Projectile.velocity.Y += 1;
                }
                else
                {
                    if (Projectile.velocity.Y < 5)
                        Projectile.velocity.Y += 0.5f;
                }
            }

 
            Projectile.rotation += Projectile.velocity.Length() * -0.05f;


            if (_hasHit)
            {
                if(Timer >= 15)
                {
                    Projectile.velocity *= 0.95f;
                }
            }
            if(Timer >= 35)
            {
                TargetOutlineColor = Color.Yellow;
                if (_hasHit)
                {
                    Projectile.velocity.X *= 0.5f;
                    Projectile.velocity.Y *= 0.5f;
                }
            }
            else
            {
                TargetOutlineColor = Color.Red;
                if(Timer % 5 == 0)
                {
                    Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 
                        ModContent.DustType<GlowSparkleDust>(), newColor: Color.Gray, Scale: Main.rand.NextFloat(0f, 1f));
                }
            }

            if(KillMyself > 0)
            {
                Projectile.Kill();
            }
            if (!NPC.AnyNPCs(ModContent.NPCType<Bishinine>()))
                Projectile.Kill();
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Timer >= 10 && Projectile.velocity.Y != oldVelocity.Y)
            {
                IsReadyToHit = true;
                _squishScale = new Vector2(1.2f, 0.8f);
                Projectile.velocity.Y = -oldVelocity.Y;
                for (float f = 0; f < 5f; f++)
                {
                    Vector2 pos = Projectile.Center;
                    pos += Main.rand.NextVector2Circular(16, 16);
                    Vector2 velocity = Main.rand.NextVector2Circular(16, 3);
                    var p = LegacyParticle.NewBlackParticle<BlackSmokeParticle>(pos, velocity, Color.DarkGray);
                    p.Scale *= 0.25f;
                    p.color *= 0.5f;
                    p.fadeToColor = Color.Black;
                    p.innerColor = Color.DarkGray;
                    p.outerColor = Color.Black;
                }

                FXUtil.ShakeCamera(Projectile.position, 1024, 8);
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.SilverCoin);
                SoundStyle bellHitSound = AssetRegistry.Sounds.Bishinine.BellHit2;
                bellHitSound.PitchVariance = 0.3f;
                SoundEngine.PlaySound(bellHitSound, Projectile.position);
            }
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
            var target = Projectile;
            for (int i = 0; i < 7; i++)
            {
                Dust.NewDustPerfect(target.Center, ModContent.DustType<GlowDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.White, 1f).noGravity = true;
            }
            for (int i = 0; i < 7; i++)
            {
                Dust.NewDustPerfect(target.Center, ModContent.DustType<TSmokeDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.LightGray, 1f).noGravity = true;
            }


            FXUtil.ShakeCamera(target.Center, 1024, 32);
            FXUtil.GlowCircleBoom(target.Center,
                innerColor: Color.Gray,
                glowColor: Color.Black,
                outerGlowColor: Color.Black, duration: 25, baseSize: 0.24f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            string texturePath = Texture;
            Texture2D texture = ModContent.Request<Texture2D>(texturePath).Value;
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            Vector2 drawOrigin = texture.Size() / 2f;
            float drawRotation = Projectile.rotation;
            Vector2 drawScale = _squishScale * Projectile.scale;
            SpriteEffects spriteEffects = Projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                Vector2 oldPos = Projectile.oldPos[i];
                Vector2 oldDrawPos = oldPos - Main.screenPosition;
                float f = i;
                float interpolant = f / (float)Projectile.oldPos.Length;
                Color fadeColor = Color.Lerp(Color.White, Color.Transparent, interpolant) * 0.25f;
                oldDrawPos += Projectile.Size / 2f;
                spriteBatch.Draw(texture, oldDrawPos, null, fadeColor, Projectile.oldRot[i], drawOrigin, drawScale, spriteEffects, 0f);
            }

            spriteBatch.Draw(texture, drawPos, null, Color.White.MultiplyRGB(lightColor), drawRotation, drawOrigin, drawScale, spriteEffects, 0f);
            return false;
        }
 

        public void DrawOutlines(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
        {
            string texturePath = Texture;
            Texture2D texture = ModContent.Request<Texture2D>(texturePath).Value;
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            Vector2 drawOrigin = texture.Size() / 2f;
            float drawRotation = Projectile.rotation;
            Vector2 drawScale = _squishScale * Projectile.scale;
            SpriteEffects spriteEffects = Projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;


            float outlineOffset = 2;
            Vector2 left = drawPos + Vector2.UnitX * -outlineOffset;
            Vector2 right = drawPos + Vector2.UnitX * outlineOffset;
            Vector2 up = drawPos + Vector2.UnitY * -outlineOffset;
            Vector2 down = drawPos + Vector2.UnitY * outlineOffset;
            Color outlineColor = _outlineColor;

            spriteBatch.Draw(texture, left, null, outlineColor, drawRotation, drawOrigin, drawScale, spriteEffects, 0f);
            spriteBatch.Draw(texture, right, null, outlineColor, drawRotation, drawOrigin, drawScale, spriteEffects, 0f);
            spriteBatch.Draw(texture, up, null, outlineColor, drawRotation, drawOrigin, drawScale, spriteEffects, 0f);
            spriteBatch.Draw(texture, down, null, outlineColor, drawRotation, drawOrigin, drawScale, spriteEffects, 0f);
        }

    }
}
