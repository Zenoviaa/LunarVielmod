
using Microsoft.Xna.Framework;
using Stellamod.Helpers;
using Stellamod.NPCs.Bosses.Jack;
using Stellamod.UI.Dialogue;
using Stellamod.Utilis;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;


//By Al0n37
namespace Stellamod.NPCs.Bosses.Zui.Projectiles
{

    public class ZuiDeath : ModNPC
    {
        public float DrugRidus = 0;
        public int DrugAlpha = 0;
        private bool p2 = false;
        float timer = 0;
        public override void SetStaticDefaults()
        {
            NPCID.Sets.TrailCacheLength[NPC.type] = 30;
            NPCID.Sets.TrailingMode[NPC.type] = 0;
            // DisplayName.SetDefault("Jack");
            Main.npcFrameCount[NPC.type] = 7;
        }
        public override void SetDefaults()
        {
            NPC.alpha = 255;
            NPC.width = 112;
            NPC.height = 70;
            NPC.damage = 0;
            NPC.defense = 6;
            NPC.lifeMax = 1150;
            NPC.HitSound = SoundID.NPCHit16;
            NPC.value = 60f;
            NPC.knockBackResist = 0.0f;
            NPC.noGravity = true;
            NPC.dontTakeDamage = true;
            NPC.dontCountMe = true;
            NPC.alpha = 255;
            NPC.BossBar = Main.BigBossProgressBar.NeverValid;
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
            if (frame >= 7)
            {
                frame = 0;
            }

            NPC.frame.Y = frameHeight * frame;
        }

        public bool DM = false;

        public override void AI()
        {
            NPC.damage = 0;
            timer++;
            if (timer == 1)
            {
                DialogueSystem dialogueSystem = ModContent.GetInstance<DialogueSystem>();

                //2. Create a new instance of your dialogue
                ZuiBeatDialogue exampleDialogue = new ZuiBeatDialogue();

                //3. Start it
                dialogueSystem.StartDialogue(exampleDialogue);

                Main.LocalPlayer.GetModPlayer<MyPlayer>().FocusOn(base.NPC.Center, 4f);
                CombatText.NewText(NPC.getRect(), Color.Gold, LangText.Misc("ZuiDeath.1"), true, false);
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


            if (DM)
            {
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
                        Dust dust = Dust.NewDustDirect(NPC.Center, NPC.width, NPC.height, DustID.GoldCoin);
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
                        CombatText.NewText(NPC.getRect(), Color.Gold, LangText.Misc("ZuiDeath.2"), true, false);
                        var EntitySource = NPC.GetSource_Death();
                        Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(base.NPC.Center, 2048f, 90f);
                        SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Crysalizer4"), NPC.position);
                        for (int i = 0; i < 14; i++)
                        {
                            Dust.NewDustPerfect(base.NPC.Center, DustID.GoldCoin, (Vector2.One * Main.rand.Next(1, 12)).RotatedByRandom(19.0), 0, default(Color), 4f).noGravity = true;
                        }
                        for (int i = 0; i < 14; i++)
                        {
                            Dust.NewDustPerfect(base.NPC.Center, DustID.Torch, (Vector2.One * Main.rand.Next(1, 12)).RotatedByRandom(19.0), 0, default(Color), 4f).noGravity = false;
                        }
                        for (int i = 0; i < 14; i++)
                        {
                            Dust.NewDustPerfect(base.NPC.Center, DustID.GoldFlame, (Vector2.One * Main.rand.Next(1, 12)).RotatedByRandom(19.0), 0, default(Color), 4f).noGravity = false;
                        }
                        for (int j = 0; j < 26; j++)
                        {

                            int a = Gore.NewGore(EntitySource, new Vector2(NPC.Center.X + Main.rand.Next(-10, 10), NPC.Center.Y + Main.rand.Next(-10, 10)), NPC.velocity, 911);
                            Main.gore[a].timeLeft = 20;
                            Main.gore[a].scale = Main.rand.NextFloat(.5f, 1f);
                        }
                        for (int i = 0; i < 40; i++)
                        {
                            Dust.NewDustPerfect(base.NPC.Center, DustID.GoldFlame, (Vector2.One * Main.rand.Next(1, 12)).RotatedByRandom(10.0), 0, default(Color), 1f).noGravity = false;
                        }

                        int Gore2 = ModContent.Find<ModGore>("Stellamod/ZuiHat").Type;
                        Gore.NewGore(EntitySource, NPC.position, NPC.velocity, Gore2);
                        Utilities.NewProjectileBetter(NPC.Center.X, NPC.Center.Y, 0, 0, ModContent.ProjectileType<ZuiSpawnEffect>(), 0, 0f, -1, 0, NPC.whoAmI);
                        NPC.active = false;
                    }
                }

            }
            
         
           
        }
    }
}