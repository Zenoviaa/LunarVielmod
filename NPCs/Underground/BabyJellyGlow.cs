using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Helpers;
using Stellamod.Items.Accessories.Brooches;
using Stellamod.Items.Weapons.Mage;
using Stellamod.Items.Weapons.Ranged;
using Stellamod.Items.Weapons.Summon;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Underground
{
    internal class BabyJellyGlow : ModNPC
    {
        const float Movement_Osc_Time = 90;

        ref float Mommy => ref NPC.ai[0];
        ref float FollowingOffset => ref NPC.ai[1];
        float Timer;
        float TimerDirection;
        float PanicTimer;
        float PanicLength;
        Vector2 PanicDirection;
        bool IsOrphan;
        NPC Mother => Main.npc[(int)Mommy];

        public override void SendExtraAI(BinaryWriter writer)
        {
            base.SendExtraAI(writer);
            writer.WriteVector2(PanicDirection);
            writer.Write(PanicLength);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            base.ReceiveExtraAI(reader);
            PanicDirection = reader.ReadVector2();
            PanicLength = reader.ReadSingle();
        }

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 4;
        }

        public override void SetDefaults()
        {
            NPC.width = 24;
            NPC.height = 44;
            NPC.noTileCollide = true;
            NPC.noGravity = true;
            NPC.lifeMax = 66;
            NPC.knockBackResist = 0;
            NPC.HitSound = SoundID.NPCHit25; // The sound the NPC will make when being hit.
            NPC.DeathSound = new SoundStyle("Stellamod/Assets/Sounds/Morrowsc1");
            NPC.aiStyle = -1;
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter += 0.25f;
            NPC.frameCounter %= Main.npcFrameCount[NPC.type];
            int frame = (int)NPC.frameCounter;
            NPC.frame.Y = frame * frameHeight;
        }

        public override void AI()
        {
            if (!IsOrphan && !Mother.active)
            {
                IsOrphan = true;
            }

            if (IsOrphan)
            {
                NPC.dontTakeDamage = false;
                Panic();
            }
            else
            {
                NPC.dontTakeDamage = true;
                FollowMommy();
            }


            // Some visuals here
            Lighting.AddLight(NPC.Center, Color.White.ToVector3() * 0.28f);
        }


        private void WanderOff()
        {
            PanicTimer++;
            if (PanicTimer >= PanicLength && StellaMultiplayer.IsHost)
            {
                PanicLength = Main.rand.Next(30, 210);
                PanicDirection = Main.rand.NextVector2Circular(1, 1);
                PanicDirection = PanicDirection.SafeNormalize(Vector2.Zero);
                NPC.netUpdate = true;
            }

            //Get the direction to the player
            Vector2 targetVelocity = PanicDirection * 1f;
            NPC.velocity = Vector2.Lerp(NPC.velocity, targetVelocity, 0.1f);
        }

        private void FollowMommy()
        {
            float distance = Vector2.Distance(NPC.Center, Mother.Center);
            if (distance <= 256 * FollowingOffset)
            {
                WanderOff();
                return;
            }

            //Get the direction to the player
            Vector2 targetDirection = NPC.Center.DirectionTo(Mother.Center);
            Vector2 targetVelocity = targetDirection * 1.2f;

            //Move to the player
            NPC.velocity = Vector2.Lerp(NPC.velocity, targetVelocity, 0.1f);
        }

        private void Panic()
        {
            PanicTimer++;
            if(PanicTimer >= PanicLength && StellaMultiplayer.IsHost)
            {
                PanicLength = Main.rand.Next(30, 210);
                PanicDirection = Main.rand.NextVector2Circular(1, 1);
                PanicDirection = PanicDirection.SafeNormalize(Vector2.Zero);
                NPC.netUpdate = true;
            }

            //Get the direction to the player
            Vector2 targetVelocity = PanicDirection * 4f;
            NPC.velocity = Vector2.Lerp(NPC.velocity, targetVelocity, 0.1f);
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            target.AddBuff(BuffID.Electrified, 10);
        }


        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ItemID.Gel, 1, minimumDropped: 2, maximumDropped: 4));
        
        }


        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Vector3 huntrianColorXyz = DrawHelper.HuntrianColorOscillate(
                    Color.Purple.ToVector3(),
                    Color.Violet.ToVector3(),
                    new Vector3(2, 2, 2), 0);

            DrawHelper.DrawDimLight(NPC, huntrianColorXyz.X, huntrianColorXyz.Y, huntrianColorXyz.Z, Color.Purple, drawColor, 1);
            return base.PreDraw(spriteBatch, screenPos, drawColor);
        }


        Vector2 Drawoffset => new Vector2(0, NPC.gfxOffY) + Vector2.UnitX * NPC.spriteDirection * 0;
        public virtual string GlowTexturePath => Texture + "_Glow";
        private Asset<Texture2D> _glowTexture;
        public Texture2D GlowTexture => (_glowTexture ??= (ModContent.RequestIfExists<Texture2D>(GlowTexturePath, out var asset) ? asset : null))?.Value;
        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            float num108 = 4;
            float num107 = (float)Math.Cos((double)(Main.GlobalTimeWrappedHourly % 1.4f / 1.4f * 6.28318548f)) / 2f + 0.5f;
            float num106 = 0f;
            Color color1 = Color.White * num107 * .8f;
            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            spriteBatch.Draw(
                GlowTexture,
                NPC.Center - Main.screenPosition + Drawoffset,
                NPC.frame,
                color1,
                NPC.rotation,
                NPC.frame.Size() / 2,
                NPC.scale,
                effects,
                0
            );
            SpriteEffects spriteEffects3 = NPC.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Color color29 = new Color(127 - NPC.alpha, 127 - NPC.alpha, 127 - NPC.alpha, 0).MultiplyRGBA(Color.LightBlue);
            for (int num103 = 0; num103 < 1; num103++)
            {
                Color color28 = color29;
                color28 = NPC.GetAlpha(color28);
                color28 *= 1f - num107;
                Vector2 vector29 = NPC.Center + (num103 / (float)num108 * 6.28318548f + NPC.rotation + num106).ToRotationVector2() * (4f * num107 + 2f) - Main.screenPosition + Drawoffset - NPC.velocity * num103;
                Main.spriteBatch.Draw(GlowTexture, vector29, NPC.frame, color28, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, spriteEffects3, 0f);
            }


        }
    }
}
