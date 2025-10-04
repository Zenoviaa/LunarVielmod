using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Dusts;
using Stellamod.Helpers;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Special
{
    internal abstract class BaseChest : ModNPC
    {
        protected int _frame;
        protected ref float Timer => ref NPC.ai[0];
        protected Color ChestColor;
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            NPCID.Sets.TrailCacheLength[Type] = 16;
            NPCID.Sets.TrailingMode[Type] = 3;
            NPCID.Sets.ImmuneToAllBuffs[Type] = true;
            Main.npcFrameCount[Type] = 33;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            NPC.width = 32;
            NPC.height = 32;
            NPC.lifeMax = 10;
            NPC.damage = 1;
            NPC.defense = 9999;
            NPC.noGravity = true;
            NPC.HitSound = SoundID.DD2_CrystalCartImpact;
            NPC.DeathSound = SoundID.NPCDeath44;
            NPC.knockBackResist = 0f;
            ChestColor = Color.White;
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            //No Contact Damage
            return false;
        }

        public override void FindFrame(int frameHeight)
        {
            base.FindFrame(frameHeight);
            NPC.frameCounter += 0.5f;
            if (NPC.frameCounter >= 1f)
            {
                _frame++;
                NPC.frameCounter = 0f;
            }
            if (_frame >= Main.npcFrameCount[Type])
                _frame = 0;
            NPC.frame.Y = frameHeight * _frame;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = TextureAssets.Npc[Type].Value;
            Vector2 drawPos = NPC.Center - Main.screenPosition;
            Rectangle frame = NPC.frame;
            Vector2 drawOrigin = frame.Size() / 2f;

            //Draw Trail or Glow or Somethin'
            spriteBatch.Restart(blendState: BlendState.Additive);
            for (float f = 0f; f < 4f; f++)
            {
                float progress = f / 4f;
                float rot = progress * MathHelper.TwoPi;
                rot += Timer * 0.1f;
                Vector2 offset = rot.ToRotationVector2() * VectorHelper.Osc(4f, 6f);
                spriteBatch.Draw(texture, drawPos + offset, frame, drawColor.MultiplyRGB(ChestColor), NPC.rotation, drawOrigin, NPC.scale, SpriteEffects.None, 0f);
            }
            spriteBatch.RestartDefaults();
            spriteBatch.Draw(texture, drawPos, frame, drawColor, NPC.rotation, drawOrigin, NPC.scale, SpriteEffects.None, 0f);
            return false;
        }

        public override void AI()
        {
            base.AI();
            Timer++;
            NPC.dontTakeDamage = Timer < 30;
            if (Timer == 1)
            {
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/StarFlower3"), NPC.position);
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/MorrowExp"), NPC.position);
                for (float f = 0; f < 32; f++)
                {
                    Dust.NewDustPerfect(NPC.Center, DustID.Fireworks,
                        (Vector2.One * Main.rand.NextFloat(0.2f, 5f)).RotatedByRandom(19.0), 0, ChestColor, Main.rand.NextFloat(1f, 3f)).noGravity = true;
                }
            }
            if (Timer % 6 == 0)
            {
                int d = Dust.NewDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<GlyphDust>(),
                    newColor: ChestColor,
                    Scale: Main.rand.NextFloat(0.2f, 0.75f));
                Main.dust[d].velocity *= 0.3f;
            }

            NPC.velocity.Y = MathF.Sin(Timer * 0.01f) * 0.5f;
            Lighting.AddLight(NPC.position, ChestColor.ToVector3() * 0.78f);
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            base.HitEffect(hit);
            float num = 2f;
            for (float f = 0; f < num; f++)
            {
                float progress = f / num;
                float rot = progress * MathHelper.ToRadians(360);
                rot += Main.rand.NextFloat(-0.5f, 0.5f);
                Vector2 velocity = rot.ToRotationVector2() * Main.rand.NextFloat(4f, 25f);
                var particle = FXUtil.GlowStretch(NPC.Center, velocity);
                particle.InnerColor = Color.White;
                particle.GlowColor = Color.LightCyan;
                particle.OuterGlowColor = Color.Black;
                particle.Duration = Main.rand.NextFloat(6, 12);
                particle.BaseSize = Main.rand.NextFloat(0.09f, 0.18f);
                particle.VectorScale *= 0.5f;
            }

            if (NPC.life <= 0)
            {
                FXUtil.ShakeCamera(NPC.position, 1024, 8);
                for (float f = 0; f < num * 3; f++)
                {
                    float progress = f / num * 3;
                    float rot = progress * MathHelper.ToRadians(360);
                    rot += Main.rand.NextFloat(-0.5f, 0.5f);
                    Vector2 velocity = rot.ToRotationVector2() * Main.rand.NextFloat(4f, 25f);
                    var particle = FXUtil.GlowStretch(NPC.Center, velocity);
                    particle.InnerColor = Color.White;
                    particle.GlowColor = ChestColor;
                    particle.OuterGlowColor = Color.Black;
                    particle.Duration = Main.rand.NextFloat(12, 25);
                    particle.BaseSize = Main.rand.NextFloat(0.09f, 0.18f);
                    particle.VectorScale *= 0.5f;
                }

                for (float f = 0; f < 32; f++)
                {
                    Dust.NewDustPerfect(NPC.Center, ModContent.DustType<GlowSparkleDust>(),
                        (Vector2.One * Main.rand.NextFloat(0.2f, 5f)).RotatedByRandom(19.0), 0, ChestColor, Main.rand.NextFloat(1f, 3f)).noGravity = true;
                }
            }
        }
    }
}
