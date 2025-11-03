using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Assets;
using Stellamod.Core.Particles;
using Stellamod.Core.Shaders;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Dungeon.BossesDG.Bisinine.Projectiles
{
    public class BellBaseball : ModNPC,
        IDrawOutlines
    {
        private float _bounceTimer;
        private Vector2 _squishScale;
        private Color _outlineColor;
        private Color TargetOutlineColor;
        private ref float Timer => ref NPC.ai[0];
        private ref float HitDirection => ref NPC.ai[1];
        private ref float KillMyself => ref NPC.ai[2];
        private ref float ReadyToHit => ref NPC.ai[3];
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            NPCID.Sets.TrailCacheLength[Type] = 16;
            NPCID.Sets.TrailingMode[Type] = 3;
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            NPC.width = 52;
            NPC.height = 52;
            NPC.noGravity = false;
            NPC.noTileCollide = false;
            NPC.dontTakeDamage = true;
            NPC.dontTakeDamageFromHostiles = true;
            NPC.lifeMax = 100;
            NPC.damage = 40;
        }
        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            return base.CanHitPlayer(target, ref cooldownSlot) && Timer < 35;
        }

        public override void AI()
        {
            base.AI();
            if (!NPC.HasValidTarget)
                NPC.TargetClosest();
            _squishScale = Vector2.Lerp(_squishScale, Vector2.One, 0.1f);
            _outlineColor = Color.Lerp(_outlineColor, TargetOutlineColor, 0.1f);
            Timer++;
            if(HitDirection != 0)
            {
                _bounceTimer = 0;
                _squishScale = new Vector2(0.8f, 1.2f);
                Timer = 0;
                Vector2 velocity = HitDirection.ToRotationVector2() * 17;
                velocity.Y -= 5;
                NPC.velocity = velocity;
                HitDirection = 0;
            }

            NPC.noTileCollide = (NPC.Top.Y + 32) < Main.player[NPC.target].Top.Y; 
            NPC.rotation += NPC.velocity.Length() * -0.05f;
            if(NPC.collideX && Timer >= 10)
            {
                NPC.velocity.X = -NPC.velocity.X;
            }

            _bounceTimer++;
            if (Timer >= 10 && NPC.collideY && NPC.velocity.Y > 0.3f)
            {
                _bounceTimer = 0;
                _squishScale = new Vector2(1.2f, 0.8f);
                NPC.velocity.Y = -NPC.velocity.Y;
                for(float f = 0; f < 5f; f++)
                {
                    Vector2 pos = NPC.Center;
                    pos += Main.rand.NextVector2Circular(16, 16);
                    Vector2 velocity = Main.rand.NextVector2Circular(16, 3);
                    var p = Particle.NewBlackParticle<BlackSmokeParticle>(pos, velocity, Color.DarkGray);
                    p.Scale *= 0.25f;
                    p.color *= 0.5f;
                    p.fadeToColor = Color.Black;
                    p.innerColor = Color.DarkGray;
                    p.outerColor = Color.Black;
                }

                FXUtil.ShakeCamera(NPC.position, 1024, 8);
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.SilverCoin);
                SoundStyle bellHitSound = AssetRegistry.Sounds.Bishinine.BellHit2;
                bellHitSound.PitchVariance = 0.3f;
                SoundEngine.PlaySound(bellHitSound, NPC.position);
            }

            if(Timer >= 35)
            {
                TargetOutlineColor = Color.Yellow;
                NPC.velocity.X *= 0.94f;
            }
            else
            {
                TargetOutlineColor = Color.Red;
                if(Timer % 5 == 0)
                {
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<GlowSparkleDust>(), newColor: Color.White, Scale: Main.rand.NextFloat(0f, 1f));
                }
            }
            if (NPC.collideY)
            {
                ReadyToHit = 1;
            }

            if(KillMyself > 0)
            {
                NPC.Kill();
            }
            if (!NPC.AnyNPCs(ModContent.NPCType<Bishinine>()))
                NPC.Kill();
        }

        public override void OnKill()
        {
            base.OnKill();
            var target = NPC;
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
                innerColor: Color.White,
                glowColor: Color.Black,
                outerGlowColor: Color.Black, duration: 25, baseSize: 0.24f);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            string texturePath = Texture;
            Texture2D texture = ModContent.Request<Texture2D>(texturePath).Value;
            Vector2 drawPos = NPC.Center - screenPos;
            Vector2 drawOrigin = NPC.frame.Size() / 2f;
            float drawRotation = NPC.rotation;
            Vector2 drawScale = _squishScale * NPC.scale;
            SpriteEffects spriteEffects = NPC.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            for (int i = 0; i < NPC.oldPos.Length; i++)
            {
                Vector2 oldPos = NPC.oldPos[i];
                Vector2 oldDrawPos = oldPos - Main.screenPosition;
                float f = i;
                float interpolant = f / (float)NPC.oldPos.Length;
                Color fadeColor = Color.Lerp(Color.White, Color.Transparent, interpolant) * 0.25f;
                oldDrawPos += NPC.Size / 2f;
                spriteBatch.Draw(texture, oldDrawPos, NPC.frame, fadeColor, NPC.oldRot[i], drawOrigin, drawScale, spriteEffects, 0f);
            }

            spriteBatch.Draw(texture, drawPos, NPC.frame, drawColor, drawRotation, drawOrigin, drawScale, spriteEffects, 0f);
            return false;
        }

        public void DrawOutlines(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
        {
            string texturePath = Texture;
            Texture2D texture = ModContent.Request<Texture2D>(texturePath).Value;
            Vector2 drawPos = NPC.Center - Main.screenPosition;
            Vector2 drawOrigin = NPC.frame.Size() / 2f;
            float drawRotation = NPC.rotation;
            Vector2 drawScale = _squishScale * NPC.scale;
            SpriteEffects spriteEffects = NPC.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;


            float outlineOffset = 2;
            Vector2 left = drawPos + Vector2.UnitX * -outlineOffset;
            Vector2 right = drawPos + Vector2.UnitX * outlineOffset;
            Vector2 up = drawPos + Vector2.UnitY * -outlineOffset;
            Vector2 down = drawPos + Vector2.UnitY * outlineOffset;
            Color outlineColor = _outlineColor;

            spriteBatch.Draw(texture, left, NPC.frame, outlineColor, drawRotation, drawOrigin, drawScale, spriteEffects, 0f);
            spriteBatch.Draw(texture, right, NPC.frame, outlineColor, drawRotation, drawOrigin, drawScale, spriteEffects, 0f);
            spriteBatch.Draw(texture, up, NPC.frame, outlineColor, drawRotation, drawOrigin, drawScale, spriteEffects, 0f);
            spriteBatch.Draw(texture, down, NPC.frame, outlineColor, drawRotation, drawOrigin, drawScale, spriteEffects, 0f);
        }

    }
}
