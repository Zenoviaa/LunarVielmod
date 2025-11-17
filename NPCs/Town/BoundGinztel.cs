using Microsoft.Xna.Framework;
using Stellamod.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;

namespace Stellamod.NPCs.Town
{
    public class BoundGinztel : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 1;
            NPCID.Sets.TownCritter[NPC.type] = true;
        }

        public override void SetDefaults()
        {
            NPC.friendly = true;
            NPC.townNPC = true;
            NPC.dontTakeDamage = true;
            NPC.width = 32;
            NPC.height = 48;
            NPC.aiStyle = 0;
            NPC.damage = 0;
            NPC.defense = 25;
            NPC.DeathSound = new SoundStyle($"Stellamod/Assets/Sounds/ItemHarvested");  // The sound the NPC will make when it dies.
            NPC.knockBackResist = 0f;
            NPC.rarity = 1;
            NPC.value = 250f; // How many copper coins the NPC will drop when killed.

            NPC.friendly = true;
            NPC.lifeMax = 250;
            NPC.dontTakeDamageFromHostiles = true;

        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            return 0f;
        }

        // Our AI here makes our NPC sit waiting for a player to enter range, jumps to attack, flutter mid-fall to stay afloat a little longer, then falls to the ground. Note that animation should happen in FindFrame
        public override void AI()
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                NPC.homeless = false;
                NPC.homeTileX = -1;
                NPC.homeTileY = -1;
                NPC.netUpdate = true;
            }

            if (NPC.wet)
            {
                NPC.life = 250;
            }
            foreach (var player in Main.player)
            {
                if (!player.active)
                    continue;
                if (player.talkNPC == NPC.whoAmI)
                {
                    Rescue();
                    return;
                }
            }
        }


        public override void OnKill()
        {
            CombatText.NewText(NPC.getRect(), Color.White, LangText.Misc("BoundGinztel"), true, false);
            base.OnKill();
        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            // We can use AddRange instead of calling Add multiple times in order to add multiple items at once
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
				// Sets the description of this NPC that is listed in the bestiary.
				new FlavorTextBestiaryInfoElement(LangText.Bestiary(this, "A bound scientist lurking in the Virulent"))
            });
        }


        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position) => false;

        public override string GetChat() => LangText.Chat(this, "Basic");


        public void Rescue()
        {

            NPC.Transform(ModContent.NPCType<Ginztel>());
            NPC.dontTakeDamage = false;
        }
    }
}
