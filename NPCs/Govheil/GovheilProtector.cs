using Microsoft.Xna.Framework;
using Stellamod.Assets.Biomes;
using Stellamod.Helpers;
using Stellamod.Items.Harvesting;
using Stellamod.Items.Materials;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;

namespace Stellamod.NPCs.Govheil
{

    public class GovheilProtector : ModNPC
    {
        public int moveSpeed = 0;
        public int moveSpeedY = 0;
        public int counter;
        public bool dash = false;
        public short npcCounter = 0;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Shadow Wraith");
            NPCID.Sets.TrailingMode[NPC.type] = 0;
            NPCID.Sets.TrailCacheLength[NPC.type] = 15;
            Main.npcFrameCount[NPC.type] = 9;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.NightTime,
                new FlavorTextBestiaryInfoElement(LangText.Bestiary(this, "The last protectors of the govheil, and they are pretty formidable..."))
            });
        }
      
        public override void SetDefaults()
        {
            NPC.width = 98;
            NPC.height = 90;
            NPC.damage = 40;
            NPC.defense = 10;
            NPC.lifeMax = 350;
            NPC.HitSound = SoundID.Tink;
            NPC.DeathSound = SoundID.Tink;
            NPC.value = 30f;
            NPC.buffImmune[BuffID.ShadowFlame] = true;
            NPC.knockBackResist = .2f;
            NPC.alpha = 0;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
        }

        int frame = 0;
        int games = 0;
        public override void AI()
        {
            games++; 
            if (games <= 760)
            {
                NPC.velocity *= 0.99f;
                NPC.aiStyle = 22;
                AIType = NPCID.IchorSticker;

            }

            if (games >= 1280)
            {
                games = 0;
            }

            if (games >= 1200)
            {

                float speedXB = NPC.velocity.X * Main.rand.NextFloat(-1f, 1f);
                float speedY = NPC.velocity.Y * Main.rand.Next(0, 0) * 0.0f + Main.rand.Next(-4, 4) * 0f;
                if (StellaMultiplayer.IsHost)
                {
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X, NPC.position.Y, speedXB * 3, speedY,
                        ProjectileID.GreekFire3, 12, 0f, Owner: Main.myPlayer);
                }


            }
            if (games >= 761)
            {
                NPC.aiStyle = -1;


                if (NPC.target < 0 || NPC.target == 255 || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
                {
                    NPC.TargetClosest();
                }
                if (counter == 0)
                {
                    if (npcCounter >= 4)
                    {
                        npcCounter = 0;
                        NPC.ai[0] = 150;
                    }
                }
                counter++;
                Player player = Main.player[NPC.target];
                NPC.rotation = NPC.velocity.X * 0.1f;
                if (NPC.Center.X >= player.Center.X && moveSpeed >= -60)
                {
                    moveSpeed--;
                }

                if (NPC.Center.X <= player.Center.X && moveSpeed <= 60)
                {
                    moveSpeed++;
                }

                NPC.velocity.X = moveSpeed * 0.07f;

                if (NPC.Center.Y >= player.Center.Y - NPC.ai[0] && moveSpeedY >= -50)
                {
                    moveSpeedY--;
                    NPC.ai[0] = 150f;
                }

                if (NPC.Center.Y <= player.Center.Y - NPC.ai[0] && moveSpeedY <= 50)
                {
                    moveSpeedY++;
                }

                NPC.velocity.Y = moveSpeedY * 0.07f;
                if (counter >= 110 && counter < 140)
                {
                    dash = true;
                    NPC.velocity *= 0.5f;
                }

                if (counter == 140)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Vector2 direction = player.Center - NPC.Center;
                        direction.Normalize();
                        direction.X *= 4f;
                        direction.Y *= 4f;
                        NPC.velocity = direction;
                    }
                }
                if (counter == 180)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                        NPC.ai[0] += -25f;
                    NPC.velocity = Vector2.Zero;
                    counter = 0;
                    dash = false;
                }

            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<AlcadizMetal>(), 3, 1, 5));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Cinderscrap>(), 2, 1, 5));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<LostScrap>(), 1, 1, 7));
            


        }

        public override void FindFrame(int frameHeight)
        {


            NPC.frameCounter += 0.5f;

            if (NPC.frameCounter >= 3)
            {
                frame++;
                NPC.frameCounter = 0;
            }
            if (frame >= 9)
            {
                frame = 0;
            }
            NPC.frame.Y = frameHeight * frame;

        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {


            if (spawnInfo.Player.InModBiome<GovheilCastle>())
            {
                return 0.5f;
            }


            return 0f;
        }
    }
}