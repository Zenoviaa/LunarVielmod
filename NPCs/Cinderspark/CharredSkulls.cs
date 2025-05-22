using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Assets.Biomes;
using Stellamod.Helpers;
using Stellamod.Items.Harvesting;
using Stellamod.Items.Materials;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Cinderspark
{
    internal class CharredSkulls : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 11; // The amount of frames the NPC has
        }

        public override void SetDefaults()
        {
            NPC.width = 70;
            NPC.height = 54;
            NPC.aiStyle = -1;
            NPC.damage = 45;
            NPC.defense = 42;
            NPC.lifeMax = 158;
            NPC.knockBackResist = 0f;
            NPC.npcSlots = 1;
            NPC.lavaImmune = true;
            NPC.HitSound = new SoundStyle("Stellamod/Assets/Sounds/Gintze_Hit") with { PitchVariance = 0.1f };
            NPC.DeathSound = new SoundStyle("Stellamod/Assets/Sounds/Gintze_Death") with { PitchVariance = 0.1f };
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter += 0.25f;
            NPC.frameCounter %= Main.npcFrameCount[NPC.type];
            int frame = (int)NPC.frameCounter;
            NPC.frame.Y = frame * frameHeight;
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.Player.InModBiome<CindersparkBiome>() && !spawnInfo.Player.ZoneUnderworldHeight)
            {
                return 0.6f;
            }

            //Else, the example bone merchant will not spawn if the above conditions are not met.
            return 0f;
        }

        public override void OnKill()
        {
            if (StellaMultiplayer.IsHost)
            {
                for(int i = 0; i < 1; i++)
                {
                    int radius = 8;
                    int x = (int)NPC.Center.X + Main.rand.Next(-radius, radius);
                    int y = (int)NPC.Center.Y + Main.rand.Next(-radius, radius);
                    NPC.NewNPC(NPC.GetSource_FromThis(), x, y, ModContent.NPCType<CharredSoul>());
                }
            }

            for (int i = 0; i < 16; i++)
            {
                float speedX = Main.rand.NextFloat(-1f, 1f);
                float speedY = Main.rand.NextFloat(-1f, 1f);
                float scale = Main.rand.NextFloat(0.66f, 1f);
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.InfernoFork,
                    speedX, speedY, Scale: scale);
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Vector3 huntrianColorXyz = DrawHelper.HuntrianColorOscillate(
                Color.OrangeRed.ToVector3(),
                Color.Red.ToVector3(),
                new Vector3(3, 3, 3), 0);

            DrawHelper.DrawDimLight(NPC, huntrianColorXyz.X, huntrianColorXyz.Y, huntrianColorXyz.Z, Color.OrangeRed, Color.White, 0);
            Lighting.AddLight(screenPos, Color.OrangeRed.ToVector3() * 1.0f * Main.essScale);
            return base.PreDraw(spriteBatch, screenPos, drawColor);
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Cinderscrap>(), chanceDenominator: 4, minimumDropped: 2, maximumDropped: 5));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<MoltenScrap>(), chanceDenominator: 2, minimumDropped: 1, maximumDropped: 3));
        }
    }
}
