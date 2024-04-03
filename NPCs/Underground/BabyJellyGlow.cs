using Microsoft.Xna.Framework;
using Stellamod.Helpers;
using Stellamod.Items.Accessories.Brooches;
using Stellamod.Items.Weapons.Mage;
using Stellamod.Items.Weapons.Ranged;
using Stellamod.Items.Weapons.Summon;
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
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<JellyBow>(), 75));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<JellyStaff>(), 75));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<JellyTome>(), 75));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<MorrowedJelliesBroochA>(), 75));
        }
    }
}
