using Microsoft.Xna.Framework;
using Stellamod.Assets.Biomes;
using Stellamod.Helpers;
using Stellamod.Items.Harvesting;
using Stellamod.Items.Materials;
using Stellamod.Items.Placeable;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Govheil
{
    public class GovheilProtector : ModNPC
    {
        private ref float Timer => ref NPC.ai[1];
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
        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            return false;
        }
        public override void SetDefaults()
        {
            NPC.width = 98;
            NPC.height = 90;
            NPC.damage = 40;
            NPC.defense = 10;
            NPC.lifeMax = 250;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = new SoundStyle("Stellamod/Assets/Sounds/CorsageRune1");
            NPC.value = 30f;
            NPC.buffImmune[BuffID.ShadowFlame] = true;
            NPC.knockBackResist = .2f;
            NPC.alpha = 0;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
        }

        int frame = 0;

        public override void AI()
        {
            NPC.aiStyle = -1;
            if (!NPC.HasValidTarget || Timer == 1)
                NPC.TargetClosest();
            if (counter == 0)
            {
                if (npcCounter >= 4)
                {
                    npcCounter = 0;
                    NPC.ai[0] = 150;
                }
            }

            Timer++;
            Player player = Main.player[NPC.target];
            NPC.spriteDirection = player.Center.X < NPC.Center.X ? 1 : -1;
            NPC.rotation = NPC.velocity.X * 0.1f;
            if (NPC.Center.X >= player.Center.X && moveSpeed >= -60)
            {
                moveSpeed--;
            }

            if (NPC.Center.X <= player.Center.X && moveSpeed <= 60)
            {
                moveSpeed++;
            }



            if (NPC.Center.Y >= player.Center.Y - NPC.ai[0] && moveSpeedY >= -50)
            {
                moveSpeedY--;
                NPC.ai[0] = 150f;
            }

            if (NPC.Center.Y <= player.Center.Y - NPC.ai[0] && moveSpeedY <= 50)
            {
                moveSpeedY++;
            }


            if (Timer >= 110 && Timer < 140)
            {
                dash = true;
                NPC.velocity *= 0.5f;
            }

            if (Timer == 140)
            {
                Vector2 direction = player.Center - NPC.Center;
                direction = direction.SafeNormalize(Vector2.Zero);
                direction.X *= 4f;
                direction.Y *= 4f;
                NPC.velocity = direction;
            }
            if (Timer >= 180)
            {
                float distance = 128;
                float particleSpeed = 8;

                Vector2 position = NPC.Center + Main.rand.NextVector2CircularEdge(distance, distance);
                Vector2 speed = (NPC.Center - position).SafeNormalize(Vector2.Zero) * particleSpeed;
                var d = Dust.NewDustPerfect(position, DustID.GemTopaz, speed, Scale: 1f);
                d.noGravity = true;
                NPC.velocity *= 0.98f;
            }
            else
            {
                NPC.velocity.X = moveSpeed * 0.07f;
                NPC.velocity.Y = moveSpeedY * 0.07f;
            }
            if (Timer == 240)
            {
                if (StellaMultiplayer.IsHost)
                {
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, (player.Center - NPC.Center).SafeNormalize(Vector2.Zero) * 7, 
                        ModContent.ProjectileType<GovheilGear>(), 16, 1, Main.myPlayer);
                }
                NPC.ai[0] += -25f;
                NPC.velocity = Vector2.Zero;
                Timer = 0;
                dash = false;
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<AlcadizMetal>(), 3, 1, 5));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<LostScrap>(), 1, 1, 7));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<MorrowChestKey>(), 5, 1, 1));


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