
using Microsoft.Xna.Framework;
using Stellamod.Items.Materials;
using Stellamod.WorldG;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;

namespace Stellamod.NPCs.Overworld.IvythornSlime
{

    public class IvythornSlime : ModNPC
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Ivythorn Slime");
            Main.npcFrameCount[NPC.type] = Main.npcFrameCount[NPCID.BlueSlime];
        }

        public override void SetDefaults()
        {
            NPC.width = 32;
            NPC.height = 24;
            NPC.damage = 8;
            NPC.defense = 4;
            NPC.lifeMax = 46;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.value = 30f;
            NPC.buffImmune[BuffID.Poisoned] = true;
            NPC.buffImmune[BuffID.Venom] = true;
            NPC.alpha = 60;
            NPC.knockBackResist = .75f;
            NPC.aiStyle = 1;
            AIType = NPCID.BlueSlime;
            AnimationType = NPCID.BlueSlime;
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (EventWorld.Gintzing)
                return 0;
            if (!spawnInfo.Player.ZonePurity)
                return 0;
            return SpawnCondition.OverworldDaySlime.Chance;
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            base.ModifyNPCLoot(npcLoot);
            npcLoot.Add(ItemDropRule.Common(ItemID.Gel, minimumDropped: 1, maximumDropped: 2));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Ivythorn>(), minimumDropped: 1, maximumDropped: 4));
        }

        public override void AI()
        {
            NPC.spriteDirection = NPC.direction;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            if (Main.rand.NextBool(3))
            {
                target.AddBuff(BuffID.Poisoned, 180);
            }
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            var entitySource = NPC.GetSource_FromThis();
            int d = 193;
            for (int k = 0; k < 6; k++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, d, 2.5f * hit.HitDirection, -2.5f, 0, Color.Green, 0.7f);
                Dust.NewDust(NPC.position, NPC.width, NPC.height, d, 2.5f * hit.HitDirection, -2.5f, 0, Color.Green, 0.7f);
            }
            for (int j = 0; j < 2; j++)
            {
                int a = Gore.NewGore(entitySource, new Vector2(NPC.Center.X + Main.rand.Next(-10, 10), NPC.Center.Y + Main.rand.Next(-10, 10)), NPC.velocity, 911);
                Main.gore[a].timeLeft = 20;
                Main.gore[a].scale = Main.rand.NextFloat(.5f, 1f);
            }
            if (NPC.life <= 0)
            {
                for (int j = 0; j < 6; j++)
                {
                    int a = Gore.NewGore(entitySource, new Vector2(NPC.Center.X + Main.rand.Next(-10, 10), NPC.Center.Y + Main.rand.Next(-10, 10)), NPC.velocity, 911);
                    Main.gore[a].timeLeft = 20;
                    Main.gore[a].scale = Main.rand.NextFloat(.5f, 1f);
                }
            }
        }
    }
}