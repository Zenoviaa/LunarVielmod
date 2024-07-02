
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Helpers;
using Stellamod.Items.Accessories;
using Stellamod.Items.Accessories.Brooches;
using Stellamod.Items.Consumables;
using Stellamod.Items.Materials;
using Stellamod.Items.Weapons.Igniters;
using Stellamod.Items.Weapons.Ranged;
using Stellamod.Items.Weapons.Thrown;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
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
        float moveSpeed = 0;
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
            NPCID.Sets.MPAllowedEnemies[NPC.type] = true;
            NPCID.Sets.BossBestiaryPriority.Add(Type);
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.OnFire] = true;


            // Influences how the NPC looks in the Bestiary

            // Influences how the NPC looks in the Bestiary
            NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                CustomTexturePath = "Stellamod/NPCs/Bosses/DaedusRework/DaedusBestiary",
                PortraitScale = 0.8f, // Portrait refers to the full picture when clicking on the icon in the bestiary
                PortraitPositionYOverride = 0f,
            };

            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
        {
            Lighting.AddLight(NPC.Center, Color.LightBlue.ToVector3() * 1.25f * Main.essScale);
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
            NPC.damage = 14;
            NPC.defense = 10;
            NPC.lifeMax = 2600;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = new SoundStyle("Stellamod/Assets/Sounds/StormDragon_Bomb");
            NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.value = Item.buyPrice(gold: 1);
            NPC.boss = true;
            NPC.npcSlots = 10f;
            NPC.takenDamageMultiplier = 0.9f;
            NPC.BossBar = ModContent.GetInstance<DaedusBossBar>();

            NPC.aiStyle = 0;
            Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/Daedus");
        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * balance);
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            return false;
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
            // We can use AddRange instead of calling Add multiple times in order to add multiple items at once
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				// Sets the preferred biomes of this town NPC listed in the bestiary.
				// With Town NPCs, you usually set this to what biome it likes the most in regards to NPC happiness.
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Graveyard,

				// Sets your NPC's flavor text in the bestiary.
				new FlavorTextBestiaryInfoElement(LangText.Bestiary(this, "One who followed power, exchanging their freedom for a taste of a singularity. Trapped forever guarding whats left of the Govheil.")),

				// You can add multiple elements if you really wanted to
				// You can also use localization keys (see Localization/en-US.lang)
				new FlavorTextBestiaryInfoElement(LangText.Bestiary(this, "The aspiring forgotten puppet", "2"))
            });
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(Flying);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Flying = reader.ReadBoolean();
        }

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
            NPC.damage = 0;

            bool lineOfSight = Collision.CanHitLine(NPC.position, NPC.width, NPC.height, player.position, player.width, player.height);
            if (Flying)
            {
                float acceleration = 0.4f;
                float deceleration = 0.2f;
                float outOfSightAcceleration = 4;
                float maxSpeed = 20;
                if (NPC.Center.X >= player.Center.X && moveSpeed >= -maxSpeed)
                {
                    if(moveSpeed >= -maxSpeed)
                    {
                        if (lineOfSight)
                        {
                            moveSpeed -= acceleration;
                        }
                        else
                        {
                            moveSpeed -= outOfSightAcceleration;
                        }
                    } else if(moveSpeed < -maxSpeed)
                    {
                        moveSpeed+= deceleration;
                    }
                }           
                else if (NPC.Center.X <= player.Center.X)
                {
                    if (moveSpeed <= maxSpeed)
                    {
                        if (lineOfSight)
                        {
                            moveSpeed+= acceleration;
                        }
                        else
                        {
                            moveSpeed += outOfSightAcceleration;
                        }
                    }
                    else if (moveSpeed > maxSpeed)
                    {
                        moveSpeed-= deceleration;
                    }
                }

                NPC.velocity.X = moveSpeed * 0.18f;

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
                NPC.velocity *= 0.80f;
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
                    if (StellaMultiplayer.IsHost)
                    {
                        NPC.NewNPC(NPC.GetSource_FromThis(), (int)DaedusPos.X, (int)NPC.Center.Y, 
                            ModContent.NPCType<SolarSingularity>());
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), GPos, new Vector2(0, 0), Mod.Find<ModProjectile>("JackSpawnEffect").Type, 10 / 9, 0, Main.myPlayer);
                    }
                  
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
                        //Check that there is a line to attack before attacking
                        if (Collision.CanHitLine(NPC.position, NPC.width, NPC.height, playerT.position, playerT.width, playerT.height))
                        {
                            NPC.ai[0]++;
                            if (NPC.ai[0] >= 100)
                            {
                                if (StellaMultiplayer.IsHost)
                                {

                                    int attack = Main.rand.Next(2, 5);
                                    if (attack == PrevAtack)
                                    {
                                        NPC.ai[0] = 1;
                                    }
                                    else
                                    {
                                        NPC.ai[0] = 0;
                                        NPC.ai[1] = attack;
                                    }

                                    Flying = true;
                                    NPC.netUpdate = true;
                                }
                            }
                        }
                
                        break;
                    case 2:
              
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
                            Vector2 DLightPos;
                            DLightPos.Y = DaedusPos.Y + 230;

                            if (StellaMultiplayer.IsHost)
                            {
                                DLightPos.X = Main.rand.NextFloat(DaedusPos.X - 300, DaedusPos.X + 300);
                                NPC.NewNPC(NPC.GetSource_FromThis(), (int)DLightPos.X, (int)DLightPos.Y, 
                                    ModContent.NPCType<DRay>());
                                DLightPos.X = Main.rand.NextFloat(DaedusPos.X - 300, DaedusPos.X + 300);
                                NPC.NewNPC(NPC.GetSource_FromThis(), (int)DLightPos.X, (int)DLightPos.Y, 
                                    ModContent.NPCType<DRay>());
                                DLightPos.X = Main.rand.NextFloat(DaedusPos.X - 300, DaedusPos.X + 300);
                                NPC.NewNPC(NPC.GetSource_FromThis(), (int)DLightPos.X, (int)DLightPos.Y, 
                                    ModContent.NPCType<DRay>());
                            }                     
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
                            if (StellaMultiplayer.IsHost)
                            {
                                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + 140, NPC.position.Y + 65, 0, 0, 
                                    ModContent.ProjectileType<FlameTornado>(), (int)(NPC.damage * 0f), 0f, Main.myPlayer);
                            }
                            
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
                            if (StellaMultiplayer.IsHost)
                            {
                                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X, NPC.position.Y, 0, 0, 
                                    ModContent.ProjectileType<BouncySword>(), (int)(40 * 1f), 0f, Main.myPlayer);
                            }
                           
                            Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(base.NPC.Center, 1212f, 62f);
                        }
                        break;
                }
            }
        }


        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Gambit>(), 1, 1, 2));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<GothiviasSeal>(), 1, 1, 1));
            npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<DaedusBag>()));
            npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ModContent.ItemType<Items.Placeable.DaedusBossRel>()));


            // All our drops here are based on "not expert", meaning we use .OnSuccess() to add them into the rule, which then gets added
            LeadingConditionRule notExpertRule = new LeadingConditionRule(new Conditions.NotExpert());
            int numResults = 5;


            notExpertRule.OnSuccess(ItemDropRule.AlwaysAtleastOneSuccess(
                ItemDropRule.Common(ModContent.ItemType<BearBroochA>(), chanceDenominator: numResults),
                ItemDropRule.Common(ModContent.ItemType<VixedBroochA>(), chanceDenominator: numResults),
                ItemDropRule.Common(ModContent.ItemType<HeatGlider>(), chanceDenominator: numResults),
                 ItemDropRule.Common(ModContent.ItemType<DaedCard>(), chanceDenominator: numResults),
                ItemDropRule.Common(ModContent.ItemType<DaedussSunSheid>(), chanceDenominator: numResults)));

            notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<Plate>(), minimumDropped: 200, maximumDropped: 1300));
            notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<AlcadizScrap>(), minimumDropped: 4, maximumDropped: 55));
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

            for(int i = 0; i < 48; i++)
            {
                Vector2 velocity = Main.rand.NextVector2Circular(12, 12);
                Dust.NewDustPerfect(NPC.Center, DustID.Hay, velocity);
            }

            if (StellaMultiplayer.IsHost)
            {
                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero,
                    ModContent.ProjectileType<DaedusDeath>(), 0, 0, Main.myPlayer,  ai0: -NPC.direction);
            }
        }
    }
}