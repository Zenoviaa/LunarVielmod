using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Illuria.BossesIL.CrumblingTowerOfIlluria
{
    public class TowerHeart : ModNPC
    {
        private ref float Timer => ref NPC.ai[0];
        private NPC Parent
        {
            get => Main.npc[(int)NPC.ai[1]];
        }
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Main.npcFrameCount[NPC.type] = 1;
            NPCID.Sets.TrailCacheLength[NPC.type] = 16;
            NPCID.Sets.TrailingMode[Type] = 3;
            NPCID.Sets.MPAllowedEnemies[NPC.type] = true;
            NPCID.Sets.BossBestiaryPriority.Add(Type);
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            NPC.width = 32;
            NPC.height = 32;
            NPC.damage = 100;
            NPC.defense = 33;
            NPC.lifeMax = 500;
            NPC.scale = 1f;
            NPC.aiStyle = -1;

            NPC.value = Item.buyPrice(gold: 5);
            NPC.knockBackResist = 0f;
       
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.npcSlots = 30f;
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            return false;
        }
        public override void AI()
        {
            base.AI();
        }

        public override void OnKill()
        {
            base.OnKill();

        }
    }
}
