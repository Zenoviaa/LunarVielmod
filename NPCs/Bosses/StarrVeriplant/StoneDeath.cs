
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Events;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;
using System;
using System.Collections.Generic;
using Steamworks;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.GameContent;
using Stellamod.Utilis;
using Stellamod.NPCs.Acidic;
using Stellamod.NPCs.Bosses.INest.IEagle;
using Stellamod.NPCs.Bosses.INest;
using Stellamod.NPCs.Bosses.SunStalker;
using Terraria.GameContent.ItemDropRules;
using Stellamod.Items.Materials;
using Stellamod.Items.Weapons.Ranged;
using Stellamod.Items.Weapons.Melee;
using Stellamod.Items.Weapons.Mage;
using Stellamod.Items.Consumables;
using Stellamod.NPCs.Bosses.Jack;


//By Al0n37
namespace Stellamod.NPCs.Bosses.StarrVeriplant
{

    public class StoneDeath : ModNPC
    {


        public float DrugRidus = 0;
        public int DrugAlpha = 0;
        int moveSpeed = 0;
        int moveSpeedY = 0;
        float HomeY = 330f;
        private bool p2 = false;
        bool Chucking;

        float timer = 0;
        public override void SetStaticDefaults()
        {
            NPCID.Sets.TrailCacheLength[NPC.type] = 30;
            NPCID.Sets.TrailingMode[NPC.type] = 0;
            // DisplayName.SetDefault("Jack");
            Main.npcFrameCount[NPC.type] = 30;
        }
        public override void SetDefaults()
        {
            NPC.alpha = 255;
            NPC.width = 72;
            NPC.height = 66;
            NPC.damage = 0;
            NPC.defense = 6;
            NPC.lifeMax = 1150;
            NPC.HitSound = SoundID.NPCHit16;
            NPC.value = 60f;
            NPC.knockBackResist = 0.0f;
            NPC.noGravity = true;
            NPC.boss = true;
            NPC.dontTakeDamage = true;
            NPC.dontCountMe = true;
            NPC.alpha = 255;
        }
        int frame = 0;
        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter += 1f;

            if (NPC.frameCounter >= 3)
            {
                frame++;
                NPC.frameCounter = 0;
            }
            if (frame >= 29)
            {
                frame = 0;
            }

            NPC.frame.Y = frameHeight * frame;
        }
        
        public override void AI()
        {
            NPC.damage = 0;
            timer++;

                if (timer == 1)
            {
                Main.LocalPlayer.GetModPlayer<MyPlayer>().FocusOn(base.NPC.Center, 4f);
            }
            Player player = Main.player[NPC.target];
            bool expertMode = Main.expertMode;
            if (!NPC.HasPlayerTarget)
            {
                NPC.TargetClosest(false);
                Player player1 = Main.player[NPC.target];

                if (!NPC.HasPlayerTarget || NPC.Distance(player1.Center) > 3000f)
                {
                    return;
                }
            }
            Player playerT = Main.player[NPC.target];
            int distance = (int)(NPC.Center - playerT.Center).Length();
            if (distance > 3000f || playerT.dead)
            {
                NPC.ai[0] = 0;
                NPC.ai[3]++;
                NPC.position.Y = player.Center.Y + -800;
                if (NPC.ai[3] >= 80)
                {
                    NPC.active = false;
                }
            }
            if (NPC.ai[2] == 0)
            {
                NPC.ai[2] = 10;
            }
            p2 = NPC.life < NPC.lifeMax * 0.5f;
            Main.GraveyardVisualIntensity = 0.4f;
            if (NPC.ai[2] == 10)
            {
                if (NPC.alpha >= 0)
                {
                    NPC.alpha = 0;
                }
                NPC.ai[0]++;
                if (Main.netMode != NetmodeID.Server)
                {
                    Dust dust = Dust.NewDustDirect(NPC.Center, NPC.width, NPC.height, DustID.Grass);
                    dust.velocity *= -1f;
                    dust.scale *= .8f;
                    dust.noGravity = true;
                    Vector2 vector2_1 = new Vector2(Main.rand.Next(-80, 81), Main.rand.Next(-80, 81));
                    vector2_1.Normalize();
                    Vector2 vector2_2 = vector2_1 * (Main.rand.Next(50, 100) * 0.04f);
                    dust.velocity = vector2_2;
                    vector2_2.Normalize();
                    Vector2 vector2_3 = vector2_2 * 34f;
                    dust.position = NPC.Center - vector2_3;
                    NPC.netUpdate = true;
                }
                if (NPC.ai[0] == 110)
                {
                    var EntitySource = NPC.GetSource_Death();
                    Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(base.NPC.Center, 2048f, 90f);
                    SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Kaboom"), NPC.position);
                    for (int i = 0; i < 14; i++)
                    {
                        Dust.NewDustPerfect(base.NPC.Center, DustID.Stone, (Vector2.One * Main.rand.Next(1, 12)).RotatedByRandom(19.0), 0, default(Color), 4f).noGravity = true;
                    }
                    for (int i = 0; i < 14; i++)
                    {
                        Dust.NewDustPerfect(base.NPC.Center, DustID.Tungsten, (Vector2.One * Main.rand.Next(1, 12)).RotatedByRandom(19.0), 0, default(Color), 4f).noGravity = false;
                    }
                    for (int i = 0; i < 14; i++)
                    {
                        Dust.NewDustPerfect(base.NPC.Center, DustID.SilverCoin, (Vector2.One * Main.rand.Next(1, 12)).RotatedByRandom(19.0), 0, default(Color), 4f).noGravity = false;
                    }
                    for (int j = 0; j < 26; j++)
                    {

                        int a = Gore.NewGore(EntitySource, new Vector2(NPC.Center.X + Main.rand.Next(-10, 10), NPC.Center.Y + Main.rand.Next(-10, 10)), NPC.velocity, 911);
                        Main.gore[a].timeLeft = 20;
                        Main.gore[a].scale = Main.rand.NextFloat(.5f, 1f);
                    }
                    for (int i = 0; i < 40; i++)
                    {
                        Dust.NewDustPerfect(base.NPC.Center, DustID.Stone, (Vector2.One * Main.rand.Next(1, 12)).RotatedByRandom(10.0), 0, default(Color), 1f).noGravity = false;
                    }

                    int Gore2 = ModContent.Find<ModGore>("Stellamod/Stone1").Type;
                    Gore.NewGore(EntitySource, NPC.position, NPC.velocity, Gore2);
                    Utilities.NewProjectileBetter(NPC.Center.X, NPC.Center.Y, 0, 0, ModContent.ProjectileType<JackSpawnEffect>(), 50, 0f, -1, 0, NPC.whoAmI);
                    NPC.active = false;
                }
            }


        }
    }
}