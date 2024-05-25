using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Helpers;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Underground
{
    internal class AbandonedMinor : ModNPC
    {
        private int _lastDirection;
        private float _waitTimer;
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 16;
        }

        public override void SetDefaults()
        {
            NPC.width = 34;
            NPC.height = 40;
            NPC.aiStyle = 3;
            NPC.damage = 34;
            NPC.defense = 8;
            NPC.lifeMax = 140;
            NPC.HitSound = SoundID.NPCHit48;
            NPC.DeathSound = SoundID.DD2_SkeletonDeath;
            NPC.value = 563f;
            NPC.knockBackResist = .45f;
            NPC.aiStyle = 3;
            AIType = NPCID.SnowFlinx;
        }

        public override void AI()
        {
            _waitTimer++;
            if (_waitTimer < 120)
            {
                NPC.velocity.X *= 0;
                NPC.spriteDirection = _lastDirection;
            }
            else
            {
                NPC.spriteDirection = -NPC.direction;
                _lastDirection = NPC.spriteDirection;
            }

            if(_waitTimer >= 400)
            {
                _waitTimer = 0;
            }
        }

        public override void FindFrame(int frameHeight)
        {
            if(_waitTimer < 120)
            {
                //Idle
                NPC.frameCounter += 0.2f;
                NPC.frameCounter %= Main.npcFrameCount[NPC.type];
                if (NPC.frameCounter >= 6)
                    NPC.frameCounter = 0;
                int frame = (int)NPC.frameCounter;
                NPC.frame.Y = frame * frameHeight;
            }
            else
            {
                //Moving
                NPC.frameCounter += 0.2f;
                NPC.frameCounter %= Main.npcFrameCount[NPC.type];
                if (NPC.frameCounter < 6)
                    NPC.frameCounter = 6;
                int frame = (int)NPC.frameCounter;
                NPC.frame.Y = frame * frameHeight;
            }
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Vector3 huntrianColorXyz = DrawHelper.HuntrianColorOscillate(
                Color.White.ToVector3(),
                Color.WhiteSmoke.ToVector3(),
                new Vector3(3, 3, 3), 0);

            DrawHelper.DrawDimLight(NPC, huntrianColorXyz.X, huntrianColorXyz.Y, huntrianColorXyz.Z, Color.White, Color.WhiteSmoke, 0);
            Lighting.AddLight(screenPos, Color.White.ToVector3() * 1.0f * Main.essScale);
            return base.PreDraw(spriteBatch, screenPos, drawColor);
        }


        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ItemID.SpelunkerGlowstick, minimumDropped: 1, maximumDropped: 3));
            npcLoot.Add(ItemDropRule.Common(ItemID.Coal, chanceDenominator: 20, minimumDropped: 1, maximumDropped: 3));
            npcLoot.Add(ItemDropRule.Common(ItemID.MiningPants, chanceDenominator: 20, minimumDropped: 1, maximumDropped: 1));
            npcLoot.Add(ItemDropRule.Common(ItemID.MiningShirt, chanceDenominator: 20, minimumDropped: 1, maximumDropped: 1));
            npcLoot.Add(ItemDropRule.Common(ItemID.MiningHelmet, chanceDenominator: 20, minimumDropped: 1, maximumDropped: 1));
        }
    }
}
