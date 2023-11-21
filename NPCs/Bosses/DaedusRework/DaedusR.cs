
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Helpers;
using Stellamod.Items.Consumables;
using Stellamod.NPCs.Bosses.Daedus;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;


//By Al0n37
namespace Stellamod.NPCs.Bosses.DaedusRework
{
    [AutoloadBossHead]
    public class DaedusR : ModNPC
    {

        public int PrevAtack;
        int moveSpeed = 0;
        int moveSpeedY = 0;
        float DaedusDrug = 8;
        float HomeY = 330f;
        private bool p2 = false;
        private bool Solar = false;
        bool Attack;
        bool Flying;
        Vector2 DaedusPos;
        public override void SetStaticDefaults()
        {
            NPCID.Sets.TrailCacheLength[NPC.type] = 4;
            NPCID.Sets.TrailingMode[NPC.type] = 0;
            // DisplayName.SetDefault("Jack");
            Main.npcFrameCount[NPC.type] = 46;

            NPCID.Sets.BossBestiaryPriority.Add(Type);
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.OnFire] = true;


            // Influences how the NPC looks in the Bestiary
            NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                PortraitScale = 0.8f, // Portrait refers to the full picture when clicking on the icon in the bestiary
                PortraitPositionYOverride = 0f,
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
        {
            Vector2 center = NPC.Center + new Vector2(0f, NPC.height * -0.1f);
            Lighting.AddLight(NPC.Center, Color.LightBlue.ToVector3() * 1.25f * Main.essScale);
            // This creates a randomly rotated vector of length 1, which gets it's components multiplied by the parameters
            Vector2 direction = Main.rand.NextVector2CircularEdge(NPC.width * 0.6f, NPC.height * 0.6f);
            float distance = 0.3f + Main.rand.NextFloat() * 0.5f;
            Vector2 velocity = new Vector2(0f, -Main.rand.NextFloat() * 0.3f - 1.5f);
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;



            Vector2 frameOrigin = NPC.frame.Size();
            Vector2 offset = new Vector2(NPC.width - frameOrigin.X + (NPC.scale * 4), NPC.height - NPC.frame.Height + 0);
            Vector2 drawPos = NPC.position - screenPos + frameOrigin + offset;

            float time = Main.GlobalTimeWrappedHourly;
            float timer = Main.GlobalTimeWrappedHourly / 2f + time * 0.04f;

            time %= 4f;
            time /= 2f;

            if (time >= 1f)
            {
                time = 2f - time;
            }

            time = time * 0.5f + 0.5f;
            SpriteEffects Effects = NPC.spriteDirection != -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            for (float i = 0f; i < 1f; i += 0.25f)
            {
                float radians = (i + timer) * MathHelper.TwoPi;

                spriteBatch.Draw(texture, drawPos + new Vector2(0f, DaedusDrug).RotatedBy(radians) * time, NPC.frame, new Color(234, 132, 54, 50), NPC.rotation, frameOrigin, NPC.scale, Effects, 0);
            }

            for (float i = 0f; i < 1f; i += 0.34f)
            {
                float radians = (i + timer) * MathHelper.TwoPi;

                spriteBatch.Draw(texture, drawPos + new Vector2(0f, DaedusDrug * 2).RotatedBy(radians) * time, NPC.frame, new Color(254, 204, 72, 77), NPC.rotation, frameOrigin, NPC.scale, Effects, 0);
            }

            return true;
        }
        public override void SetDefaults()
        {
            NPC.alpha = 0;
            NPC.width = 230;
            NPC.height = 230;
            NPC.damage = 10;
            NPC.defense = 10;
            NPC.lifeMax = 2500;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.value = Item.buyPrice(gold: 40);
            NPC.boss = true;
            NPC.npcSlots = 10f;
            NPC.BossBar = ModContent.GetInstance<DaedusBossBar>();

            Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/Daedus");
        }
        int frame = 0;
        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter += 0.5f;

            if (Attack)
            {
                if (NPC.frameCounter >= 8)
                {
                    frame++;
                    NPC.frameCounter = 5;
                }
                if (frame >= 46)
                {
                    Attack = false;
                    frame = 0;
                }
                if (frame < 30)
                {
                    frame = 30;
                }
            }
            else
            {
                if (NPC.frameCounter >= 4)
                {
                    frame++;
                    NPC.frameCounter = 0;
                }
                if (frame >= 30)
                {
                    frame = 0;
                }
            }

            NPC.frame.Y = frameHeight * frame;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            // Sets the description of this NPC that is listed in the bestiary
            bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> {
                new MoonLordPortraitBackgroundProviderBestiaryInfoElement(), // Plain black background
				new FlavorTextBestiaryInfoElement("A puppet, a guardian, and a follower of Gothivia, one of her finest creations")
            });
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(attackCounter);
            writer.Write(timeBetweenAttacks);
            writer.WriteVector2(dashDirection);
            writer.Write(dashDistance);

        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            attackCounter = reader.ReadInt32();
            timeBetweenAttacks = reader.ReadInt32();


            dashDirection = reader.ReadVector2();
            dashDistance = reader.ReadSingle();

        }

        private int attackCounter;
        private int timeBetweenAttacks = 120;
        private Vector2 dashDirection = Vector2.Zero;
        private float dashDistance = 0f;

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0)
            {
                Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(NPC.Center, 2048f, 128f);
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Hay, 2.5f * hit.HitDirection, -2.5f, 0, default, 1.2f);
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Hay, 2.5f * hit.HitDirection, -2.5f, 0, default, 0.5f);
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Hay, 2.5f * hit.HitDirection, -2.5f, 0, default, 1.2f);
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Hay, 2.5f * hit.HitDirection, -2.5f, 0, default, 0.5f);
            }
            else
            {
                for (int k = 0; k < 7; k++)
                {

                    Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Hay, 2.5f * hit.HitDirection, -2.5f, 0, default, 1.2f);
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Hay, 2.5f * hit.HitDirection, -2.5f, 0, default, 0.5f);
                }
            }
        }
        public override void AI()
        {
            Player player = Main.player[NPC.target];
            bool expertMode = Main.expertMode;

            if (Flying)
            {
                if (NPC.Center.X >= player.Center.X && moveSpeed >= -120) // flies to players x position
                    moveSpeed--;
                else if (NPC.Center.X <= player.Center.X && moveSpeed <= 120)
                    moveSpeed++;

                NPC.velocity.X = moveSpeed * 0.10f;

                if (NPC.Center.Y >= player.Center.Y - HomeY && moveSpeedY >= -20) //Flies to players Y position
                {
                    moveSpeedY--;
                    HomeY = 200f;
                }
                else if (NPC.Center.Y <= player.Center.Y - HomeY && moveSpeedY <= 20)
                {
                    moveSpeedY++;
                }

                NPC.velocity.Y = moveSpeedY * 0.13f;
            }

            if (Attack)
            {
                NPC.velocity *= 0.82f;
                if (DaedusDrug <= 18)
                {
                    DaedusDrug += 0.1f;
                }
            }
            else
            {
                if (DaedusDrug >= 8)
                {
                    DaedusDrug -= 0.1f;
                }
            }
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
   
                NPC.ai[2] = 1;
            }
            p2 = NPC.life < NPC.lifeMax * 0.5f;
            if (p2)
            {
                if (!Solar)
                {
                    Vector2 GPos;
                    GPos.X = DaedusPos.X;
                    GPos.Y = NPC.Center.Y;
                    NPC.NewNPC(NPC.GetSource_FromThis(), (int)DaedusPos.X, (int)NPC.Center.Y, ModContent.NPCType<SolarSingularity>());
                    var entitySource = NPC.GetSource_FromThis();
                    Projectile.NewProjectile(entitySource, GPos, new Vector2(0, 0), Mod.Find<ModProjectile>("JackSpawnEffect").Type, NPC.damage / 9, 0);
                    Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(GPos, 1212f, 62f);
                    Solar = true;
                }
            }
           
            if (NPC.ai[2] == 1)
            {
                switch (NPC.ai[1])
                {
                    case 0:
 
                        NPC.ai[0]++;
                        if (NPC.ai[0] > 20)
                        {
                            DaedusPos = NPC.position;
                            NPC.noGravity = true;
                            NPC.noTileCollide = true;
                            Flying = true;
                            NPC.ai[0] = 0;
                            NPC.ai[1] = 1;
                        }
                        break;
                    case 1:
                        NPC.ai[0]++;
                        if (NPC.ai[0] >= 100)
                        {
                            int Atack = Main.rand.Next(2, 5);
                            if (Atack == PrevAtack)
                            {
                                Flying = true;
                                NPC.ai[0] = 1;
                            }
                            else
                            {
                                Flying = true;
                                NPC.ai[0] = 0;
                                NPC.ai[1] = Atack;
                            }
                        }

                        break;
                    case 2:
                        Vector2 DLightPos;
                        NPC.ai[0]++;
                        if (NPC.ai[0] == 20)
                        {
                            Attack = true;
                        }
                        if (NPC.ai[0] >= 130)
                        {
                            PrevAtack = 2;
                            NPC.ai[1] = 1;
                            NPC.ai[0] = 0;
                        }
                        if (NPC.ai[0] == 90)
                        {
                            DLightPos.Y = DaedusPos.Y + 230;
                            DLightPos.X = Main.rand.NextFloat(DaedusPos.X - 300, DaedusPos.X + 300);
                            NPC.NewNPC(NPC.GetSource_FromThis(), (int)DLightPos.X, (int)DLightPos.Y, ModContent.NPCType<DRay>());
                            DLightPos.X = Main.rand.NextFloat(DaedusPos.X - 300, DaedusPos.X + 300);
                            NPC.NewNPC(NPC.GetSource_FromThis(), (int)DLightPos.X, (int)DLightPos.Y, ModContent.NPCType<DRay>());
                            DLightPos.X = Main.rand.NextFloat(DaedusPos.X - 300, DaedusPos.X + 300);
                            NPC.NewNPC(NPC.GetSource_FromThis(), (int)DLightPos.X, (int)DLightPos.Y, ModContent.NPCType<DRay>());
                        }
                        break;
                    case 3:
  
                        NPC.ai[0]++;
                        if (NPC.ai[0] == 20)
                        {
                            Attack = true;
                        }
                        if (NPC.ai[0] >= 110)
                        {
                            PrevAtack = 3;
                            NPC.ai[1] = 1;
                            NPC.ai[0] = 0;
                        }
                        if (NPC.ai[0] == 90)
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + 140, NPC.position.Y + 65, 0, 0, ModContent.ProjectileType<FlameTornado>(), (int)(NPC.damage * 0f), 0f);
                            Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(base.NPC.Center, 1212f, 62f);
                        }
                        break;
                    case 4:

                        NPC.ai[0]++;
                        if (NPC.ai[0] == 20)
                        {
                            Attack = true;
                        }
                        if (NPC.ai[0] >= 110)
                        {
                            PrevAtack = 4;
                            NPC.ai[1] = 1;
                            NPC.ai[0] = 0;
                        }
                        if (NPC.ai[0] == 90)
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X, NPC.position.Y, 0, 0, ModContent.ProjectileType<BouncySword>(), (int)(NPC.damage * 1f), 0f);
                            Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(base.NPC.Center, 1212f, 62f);
                        }
                        break;
                }
            }
        }


        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            // Do NOT misuse the ModifyNPCLoot and OnKill hooks: the former is only used for registering drops, the latter for everything else

            // Add the treasure bag using ItemDropRule.BossBag (automatically checks for expert mode)
            //	npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<MinionBossBag>()));




            // ItemDropRule.MasterModeCommonDrop for the relic

            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Gambit>(), 1, 1, 2));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<GothiviasSeal>(), 1, 1, 1));
            npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<DaedusBag>()));
            // ItemDropRule.MasterModeDropOnAllPlayers for the pet
            //npcLoot.Add(ItemDropRule.MasterModeDropOnAllPlayers(ModContent.ItemType<MinionBossPetItem>(), 4));


            npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ModContent.ItemType<Items.Placeable.DaedusBossRel>()));


            // All our drops here are based on "not expert", meaning we use .OnSuccess() to add them into the rule, which then gets added
            LeadingConditionRule notExpertRule = new LeadingConditionRule(new Conditions.NotExpert());

            // Notice we use notExpertRule.OnSuccess instead of npcLoot.Add so it only applies in normal mode
            // Boss masks are spawned with 1/7 chance
            //notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<MinionBossMask>(), 7));

            // This part is not required for a boss and is just showcasing some advanced stuff you can do with drop rules to control how items spawn
            // We make 12-15 ExampleItems spawn randomly in all directions, like the lunar pillar fragments. Hereby we need the DropOneByOne rule,
            // which requires these parameters to be defined
            //int itemType = ModContent.ItemType<Gambit>();
            //var parameters = new DropOneByOne.Parameters()
            //{
            //	ChanceNumerator = 1,
            //	ChanceDenominator = 1,
            //	MinimumStackPerChunkBase = 1,
            //	MaximumStackPerChunkBase = 1,
            //	MinimumItemDropsCount = 1,
            //	MaximumItemDropsCount = 3,
            //};

            //notExpertRule.OnSuccess(new DropOneByOne(itemType, parameters));

            // Finally add the leading rule
            npcLoot.Add(notExpertRule);
        }
       


        public override void OnKill()
        {

            if (Main.netMode != NetmodeID.Server && Terraria.Graphics.Effects.Filters.Scene["Shockwave"].IsActive())
            {
                Terraria.Graphics.Effects.Filters.Scene["Shockwave"].Deactivate();
            }
            NPC.SetEventFlagCleared(ref DownedBossSystem.downedDaedusBoss, -1);
        }



    }
}