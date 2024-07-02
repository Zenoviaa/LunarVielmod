using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Helpers;
using Stellamod.Items.Accessories;
using Stellamod.Items.Consumables;
using Stellamod.Items.Weapons.Mage;
using Stellamod.Items.Weapons.Melee.Spears;
using Stellamod.Items.Weapons.Ranged;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Bosses.SunStalker
{
    [AutoloadBossHead]
    public class SunStalker : ModNPC
    {
        private bool _invincible;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Sun Stalker");
            NPCID.Sets.TrailCacheLength[NPC.type] = 15;
            NPCID.Sets.TrailingMode[NPC.type] = 0;
            NPCID.Sets.MPAllowedEnemies[NPC.type] = true;
            Main.npcFrameCount[NPC.type] = 6;


            NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers();
            drawModifiers.CustomTexturePath = "Stellamod/NPCs/Bosses/SunStalker/SunStalkerBestiary";
            drawModifiers.PortraitScale = 1f; // Portrait refers to the full picture when clicking on the icon in the bestiary
            drawModifiers.PortraitPositionYOverride = 0f;
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);
        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            // We can use AddRange instead of calling Add multiple times in order to add multiple items at once
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				// Sets the preferred biomes of this town NPC listed in the bestiary.
				// With Town NPCs, you usually set this to what biome it likes the most in regards to NPC happiness.
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Desert,

				// Sets your NPC's flavor text in the bestiary.
				new FlavorTextBestiaryInfoElement(LangText.Bestiary(this, "Former flyer and born where all big birds were born, in the cinderspark.")),

				// You can add multiple elements if you really wanted to
				// You can also use localization keys (see Localization/en-US.lang)
				new FlavorTextBestiaryInfoElement(LangText.Bestiary(this, "SunStalker, the Last Talon"))
            });
        }

        bool Intro;
        bool Dir;
        bool Dashing;
        int moveSpeed = 0;
        float DashSpeed = 9;
        int moveSpeedY = 0;
        int Attack = 0;
        int Attacks = 6;
        float HomeY = 330f;
        float DrugRidus = 0;
        float PrevAttack = 0;
        bool Glow;
        bool TPChance;

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(TPChance);
            writer.Write(Attack);
            writer.Write(_invincible);
            writer.WriteVector2(targetPos);
            writer.Write(moveSpeed);
            writer.Write(moveSpeedY);
            writer.Write(HomeY);
            writer.Write(PrevAttack);
            writer.Write(Attacks);
            writer.Write(DashSpeed);
            writer.Write(Dashing);
            writer.Write(Dir);
            writer.Write(Intro);
            writer.Write(DrugRidus);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            TPChance = reader.ReadBoolean();
            Attack = reader.ReadInt32();
            _invincible = reader.ReadBoolean();
            targetPos = reader.ReadVector2();
            moveSpeed = reader.ReadInt32();
            moveSpeedY = reader.ReadInt32();
            HomeY = reader.ReadSingle();
            PrevAttack = reader.ReadSingle();
            Attacks = reader.ReadInt32();
            DashSpeed = reader.ReadSingle();
            Dashing = reader.ReadBoolean();
            Dir = reader.ReadBoolean();
            Intro = reader.ReadBoolean();
            DrugRidus = reader.ReadSingle();
        }


        public void Movement(Vector2 Player2, float PosX, float PosY, float Speed)
        {
            Player player = Main.player[NPC.target];
            Vector2 target = player.Center + new Vector2(PosX, PosY);
            base.NPC.velocity = Vector2.Lerp(base.NPC.velocity, VectorHelper.MovemontVelocity(base.NPC.Center, Vector2.Lerp(base.NPC.Center, target, 0.5f), base.NPC.Center.Distance(target) * Speed), 0.1f);
        }

        public override void SetDefaults()
        {
            NPC.width = 100;
            NPC.height = 116;
            NPC.damage = 50;
            NPC.defense = 12;
            NPC.lifeMax = 1300;
            NPC.HitSound = SoundID.NPCHit28;
            NPC.DeathSound = SoundID.NPCDeath42;
            NPC.value = Item.buyPrice(silver: 25);
            NPC.buffImmune[BuffID.OnFire] = true;
            NPC.alpha = 255;
            NPC.boss = true;
            NPC.knockBackResist = 0f;
            NPC.noTileCollide = true;
            NPC.npcSlots = 10f;
            NPC.noGravity = true;
            NPC.scale = 1f;
            NPC.aiStyle = 0;

            NPC.BossBar = ModContent.GetInstance<SunStalkerBossBar>();
            if (!Main.dedServ)
            {
                Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/SunStalker");
            }
     
        }
        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * balance);
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            return Spawner > 30;
        }

        Vector2 targetPos;
        public override void OnKill()
        {
            NPC.SetEventFlagCleared(ref DownedBossSystem.downedSunsBoss, -1);
        }

        public float Spawner = 0;

        public override void AI()
        {
            Spawner++;
            if (NPC.life < NPC.lifeMax / 2)
            {
                Attacks = 6;
            }
            else
            {
                Attacks = 5;
            }

            Player player = Main.player[NPC.target];
            NPC.dontTakeDamage = _invincible;
            NPC.dontCountMe = _invincible;
            if (Attack == 0)
            {

                NPC.spriteDirection = -NPC.direction;
                NPC.ai[0]++;
           
                if(NPC.ai[1] == 1)
                {
                    NPC.position.X = player.position.X - 00;
                    NPC.position.Y = player.position.Y - 300;
                    if (NPC.alpha >= 0)
                    {
                        NPC.alpha -= 4;

                    }
                }
                else
                {
                    if (NPC.alpha >= 0)
                    {
                        NPC.alpha -= 2;
                    }
                }

                if (NPC.alpha >= 5 && NPC.ai[1] == 0)
                {
                    Dust dust = Dust.NewDustDirect(NPC.Center, NPC.width, NPC.height, DustID.CopperCoin);
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
                    NPC.velocity.Y -= 0.01f;
                }
                if (NPC.ai[0] == 2)
                {
                    if (StellaMultiplayer.IsHost)
                    {
                        _invincible = true;
                        NPC.netUpdate = true;
                    }
                    SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/SunStalker_Charge"), NPC.position);
                }
                if (NPC.alpha <= 5 && NPC.ai[1] == 0)
                {
                    for (int i = 0; i < 50; i++)
                    {
                        int num = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.GoldCoin, 0f, -2f, 0, default(Color), 1.5f);
                        Main.dust[num].noGravity = true;
                        Main.dust[num].position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
                        Main.dust[num].position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
                        {
                            Main.dust[num].velocity = NPC.DirectionTo(Main.dust[num].position) * 6f;
                        }
                    }
                    for (int i = 0; i < 50; i++)
                    {
                        int num = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.CopperCoin, 0f, -2f, 0, default(Color), 1.5f);
                        Main.dust[num].noGravity = true;
                        Main.dust[num].position.X += Main.rand.Next(-50, 51) * .05f - 2.5f;
                        Main.dust[num].position.X += Main.rand.Next(-50, 51) * .05f - 2.5f;
                        {
                            Main.dust[num].velocity = NPC.DirectionTo(Main.dust[num].position) * 6f;
                        }
                    }

                    if (StellaMultiplayer.IsHost)
                    {
                        _invincible = false;
                        NPC.netUpdate = true;
                    }

                    Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(base.NPC.Center, 512f, 32f);
                    NPC.velocity.Y = 0f;
                    Glow = true;
                    NPC.ai[1] = 1;

                    SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/SunStalker_Charge_Full"), NPC.position);
                }
                if (NPC.alpha <= 0 && NPC.ai[1] == 1)
                {
                    SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/SunStalker_Charge_Full_Note"), NPC.position);
                    NPC.ai[1] = 2;
                    NPC.ai[0] = 5000;
                    NPC.alpha = 40;
                }
                if (NPC.ai[0] >= 5050 && NPC.ai[1] == 2)
                {

                    Intro = true;
                    if (StellaMultiplayer.IsHost)
                    {
                        _invincible = false;
                        NPC.netUpdate = true;
                    }

                    NPC.ai[0] = 0;
                    Attack = 5;
                    NPC.ai[2] = 1;
                }

            }
            targetPos = player.Center;
            if (NPC.ai[2] == 1)
            {
                switch (NPC.ai[3])
                {
                    case 0:
                        NPC.spriteDirection = -NPC.direction;
                        NPC.ai[0]++;

                        if (NPC.ai[0] <= 100)
                        {
                            NPC.rotation = NPC.velocity.X * 0.07f;



                            if (NPC.Center.X >= player.Center.X && moveSpeed >= -120) // flies to players x position
                                moveSpeed--;
                            else if (NPC.Center.X <= player.Center.X && moveSpeed <= 120)
                                moveSpeed++;

                            NPC.velocity.X = moveSpeed * 0.10f;

                            if (NPC.Center.Y >= player.Center.Y - HomeY && moveSpeedY >= -20) //Flies to players Y position
                            {
                                moveSpeedY--;
                                HomeY = 250f;
                            }
                            else if (NPC.Center.Y <= player.Center.Y - HomeY && moveSpeedY <= 20)
                            {
                                moveSpeedY++;
                            }

                            NPC.velocity.Y = moveSpeedY * 0.13f;
                        }
                        else
                        {
                            NPC.rotation = NPC.velocity.X * 0.07f;
                            NPC.velocity *= 0.90f;
                        }
                        if (NPC.ai[0] == 100)
                        {
                            int Sound = Main.rand.Next(1, 3);
                            if (Sound == 1)
                            {
                                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/SunStalker_Attack"), NPC.position);
                            }
                            else
                            {
                                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/SunStalker_Attack2"), NPC.position);
                            }

                        }
                        if (NPC.ai[0] >= 100 && NPC.ai[0] <= 125)
                        {
                            NPC.alpha += 1;
                        }
                        if (NPC.ai[0] >= 125 && NPC.ai[0] <= 165)
                        {
                            Dust dust = Dust.NewDustDirect(NPC.Center, NPC.width, NPC.height, DustID.CopperCoin);
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
                            if (NPC.alpha > 0)
                            {
                                NPC.alpha -= 1;
                            }


                        }
                        if (NPC.ai[0] == 150 || NPC.ai[0] == 170 || NPC.ai[0] == 190)
                        {
                            int Sound = Main.rand.Next(1, 3);
                            if (Sound == 1)
                            {
                                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/SunStalker_Rock"), NPC.position);
                            }
                            else
                            {
                                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/SunStalker_Rock2"), NPC.position);
                            }
                 
                            if (StellaMultiplayer.IsHost)
                            {
                                Vector2 RockPos;
                                RockPos.Y = NPC.Center.Y;
                                RockPos.X = Main.rand.NextFloat(player.Center.X + 600, player.Center.X - 600 + 1);
                                int damage = Main.expertMode ? 16 : 20;
                                Projectile.NewProjectile(NPC.GetSource_FromThis(), RockPos, Vector2.Zero, 
                                    ModContent.ProjectileType<SunRock>(), damage, 0, Owner: Main.myPlayer);
                            }
                        }
                        if (NPC.ai[0] == 210)
                        {
                            NPC.alpha = 0;
                            if (StellaMultiplayer.IsHost)
                            {
                                NPC.ai[3] = Main.rand.Next(1, Attacks);
                                NPC.netUpdate = true;
                            }
                
                            NPC.ai[0] = 0;
                        }
                        break;
                    case 1:
                        NPC.spriteDirection = -NPC.direction;
                        NPC.ai[0]++;

                        NPC.rotation = NPC.velocity.X * 0.07f;


                        if (NPC.Center.X >= player.Center.X && moveSpeed >= -120) // flies to players x position
                            moveSpeed--;
                        else if (NPC.Center.X <= player.Center.X && moveSpeed <= 120)
                            moveSpeed++;

                        NPC.velocity.X = moveSpeed * 0.10f;

                        if (NPC.Center.Y >= player.Center.Y - HomeY && moveSpeedY >= -20) //Flies to players Y position
                        {
                            moveSpeedY--;
                            HomeY = 250f;
                        }
                        else if (NPC.Center.Y <= player.Center.Y - HomeY && moveSpeedY <= 20)
                        {

                            moveSpeedY++;
                        }

                        NPC.velocity.Y = moveSpeedY * 0.13f;

                        if (NPC.ai[0] >= 75)
                        {
                            if (NPC.alpha > 0)
                            {
                                NPC.alpha -= 1;
                            }

                        }
                        if (NPC.ai[0] == 100 || NPC.ai[0] == 160 || NPC.ai[0] == 220)
                        {
                            NPC.alpha += 30;
                            Vector2 direction = Vector2.Normalize(Main.player[NPC.target].Center - NPC.Center) * 8.5f;
 

                            int Sound = Main.rand.Next(1, 3);
                            if (Sound == 1)
                            {
                                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/SunStalker_Feather"), NPC.position);
                            }
                            else
                            {
                                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/SunStalker_Feather2"), NPC.position);
                            }
                            for (int i = 0; i < 50; i++)
                            {
                                int num = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.GoldCoin, 0f, -2f, 0, default(Color), 1.5f);
                                Main.dust[num].noGravity = true;
                                Main.dust[num].position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
                                Main.dust[num].position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
                                {
                                    Main.dust[num].velocity = NPC.DirectionTo(Main.dust[num].position) * 6f;
                                }
                            }
                            int amountOfProjectiles = Main.rand.Next(1, 4);
                            for (int i = 0; i < amountOfProjectiles; ++i)
                            {
                                if (StellaMultiplayer.IsHost)
                                {
                                    float offsetX = Main.rand.Next(-200, 200) * 0.01f;
                                    float offsetY = Main.rand.Next(-200, 200) * 0.01f;
                                    int damage = Main.expertMode ? 16 : 22;
                                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y, direction.X + offsetX, direction.Y + offsetY,
                                       ModContent.ProjectileType<SunFeather>(), damage, 1, Owner: Main.myPlayer);
                                }              
                            }
                        }
                        if (NPC.ai[0] == 280)
                        {
                            PrevAttack = 1;
                            NPC.ai[3] = 10;
                        }
                        break;
                    case 2:
                        NPC.ai[0]++;
                        if (NPC.ai[0] <= 50)
                        {
                            NPC.ai[0] = 80;
                        }
                        if (NPC.ai[0] <= 150)
                        {
                            NPC.rotation = NPC.velocity.X * 0.07f;

                            Dust dust = Dust.NewDustDirect(NPC.Center, NPC.width, NPC.height, DustID.CopperCoin);
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
                            if (NPC.position.X >= player.position.X)
                            {
                                Movement(targetPos, 400f, 0f, 0.05f);
                            }
                            else
                            {
                                Movement(targetPos, -400f, 0f, 0.05f);
                            }
                        }
                        else
                        {
                            NPC.velocity *= 0.90f;
                        }
                        if (NPC.ai[0] == 150)
                        {
                            Dashing = true;
                            NPC.alpha = 40;
                            if (NPC.position.X >= player.position.X)
                            {
                                Dir = true;
                            }
                            else
                            {
                                Dir = false;
                            }
                            for (int i = 0; i < 50; i++)
                            {
                                int num = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.GoldCoin, 0f, -2f, 0, default(Color), 1.5f);
                                Main.dust[num].noGravity = true;
                                Main.dust[num].position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
                                Main.dust[num].position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
                                {
                                    Main.dust[num].velocity = NPC.DirectionTo(Main.dust[num].position) * 6f;
                                }
                            }
                            int Sound = Main.rand.Next(1, 3);
                            if (Sound == 1)
                            {
                                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/SunStalker_Dash"), NPC.position);
                            }
                            else
                            {
                                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/SunStalker_Dash2"), NPC.position);
                            }
                        }
                        if (NPC.ai[0] >= 150)
                        {

                            NPC.rotation = NPC.velocity.X * 0.07f;
                            NPC.spriteDirection = -NPC.direction;
                            if (NPC.alpha > 0)
                            {
                                NPC.alpha -= 1;
                            }
                            if (Dir)
                            {
                                NPC.velocity.X = -DashSpeed;
                                if (NPC.position.X >= player.position.X)
                                {
                                    NPC.alpha = 40;

                                    NPC.ai[0] = 151;
                                }
                                else
                                {
                                    DashSpeed -= 0.1f;
                                }

                            }
                            else
                            {
                                NPC.velocity.X = DashSpeed;
                                if (NPC.position.X <= player.position.X)
                                {
                                    NPC.alpha = 40;
                                    NPC.ai[0] = 151;
                                }
                                else
                                {
                                    DashSpeed -= 0.1f;
                                }

                            }

                        }
                        else
                        {
                            NPC.spriteDirection = -NPC.direction;
                        }
                        if (NPC.ai[0] == 260)
                        {
                            DashSpeed = 9;
                            Dashing = false;
                            PrevAttack = 2;
                            NPC.ai[3] = 10;

                        }
                        break;
                    case 3:
                        NPC.spriteDirection = -NPC.direction;
                        NPC.ai[0]++;

                        if (NPC.ai[0] <= 100)
                        {
                            NPC.rotation = NPC.velocity.X * 0.07f;
                            if (NPC.Center.X >= player.Center.X && moveSpeed >= -120) // flies to players x position
                                moveSpeed--;
                            else if (NPC.Center.X <= player.Center.X && moveSpeed <= 120)
                                moveSpeed++;

                            NPC.velocity.X = moveSpeed * 0.10f;

                            if (NPC.Center.Y >= player.Center.Y - HomeY && moveSpeedY >= -20) //Flies to players Y position
                            {
                                moveSpeedY--;
                                HomeY = 250f;
                            }
                            else if (NPC.Center.Y <= player.Center.Y - HomeY && moveSpeedY <= 20)
                            {

                                moveSpeedY++;
                            }

                            NPC.velocity.Y = moveSpeedY * 0.13f;
                        }
                        else
                        {
                            if (NPC.ai[0] == 100)
                            {
                                DrugRidus = 1;
                            }

                            if (NPC.ai[0] >= 100 && NPC.ai[0] <= 150)
                            {
                                if (DrugRidus <= 26)
                                {
                                    DrugRidus += 1.08f;
                                }
                            }
                            if (NPC.ai[0] >= 190 && NPC.ai[0] <= 210)
                            {
                                if (DrugRidus >= 0)
                                {
                                    DrugRidus -= 0.95f;
                                }
                            }

                            NPC.rotation = NPC.velocity.X * 0.07f;
                            NPC.velocity *= 0.90f;
                        }
                        if (NPC.ai[0] == 100)
                        {
                            int Sound = Main.rand.Next(1, 3);
                            if (Sound == 1)
                            {
                                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/SunStalker_Attack"), NPC.position);
                            }
                            else
                            {
                                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/SunStalker_Attack2"), NPC.position);
                            }

                        }
                        if (NPC.ai[0] >= 100 && NPC.ai[0] <= 125)
                        {
                            NPC.alpha += 1;
                        }
                        if (NPC.ai[0] >= 125 && NPC.ai[0] <= 165)
                        {
                            Dust dust = Dust.NewDustDirect(NPC.Center, NPC.width, NPC.height, DustID.CopperCoin);
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
                            if (NPC.alpha > 0)
                            {
                                NPC.alpha -= 1;
                            }
                        }
                        if (NPC.ai[0] == 150 || NPC.ai[0] == 170 || NPC.ai[0] == 190)
                        {
                            int Sound = Main.rand.Next(1, 3);
                            if (Sound == 1)
                            {
                                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/SunStalker_Bomb"), NPC.position);
                            }
                            else
                            {
                                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/SunStalker_Bomb_2"), NPC.position);
                            }

                            if (StellaMultiplayer.IsHost)
                            {
                                Vector2 RockPos;
                                RockPos.Y = Main.rand.NextFloat(player.Center.Y + 300, player.Center.Y - 300 + 1);
                                RockPos.X = Main.rand.NextFloat(player.Center.X + 300, player.Center.X - 300 + 1);
                                int damage = Main.expertMode ? 15 : 19;
                                Projectile.NewProjectile(NPC.GetSource_FromThis(), RockPos, Vector2.Zero,
                                    ModContent.ProjectileType<SunBomb>(), damage, 0, Owner: Main.myPlayer);
                            }
                        }
                        if (NPC.ai[0] == 210)
                        {
                            DrugRidus = 0;
                            NPC.alpha = 0;
                            PrevAttack = 3;
                            NPC.ai[3] = 10;
                        }
                        break;

                    case 4:
                        NPC.ai[0]++;
                        if (NPC.ai[0] <= 10)
                        {
                            SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/SunStalker_Charge_TP_Out"), NPC.position);
                        }
                        if (NPC.ai[0] <= 50)
                        {
                            if (StellaMultiplayer.IsHost)
                            {
                                _invincible = true;
                                NPC.netUpdate = true;
                            }

                            NPC.rotation = NPC.velocity.X * 0.07f;
                            Glow = false;
                            if (NPC.alpha <= 255)
                            {
                                NPC.alpha += 18;
                            }


                            if (NPC.Center.X >= player.Center.X && moveSpeed >= -120) // flies to players x position
                                moveSpeed--;
                            else if (NPC.Center.X <= player.Center.X && moveSpeed <= 120)
                                moveSpeed++;

                            NPC.velocity.X = moveSpeed * 0.10f;

                            if (NPC.Center.Y >= player.Center.Y - HomeY && moveSpeedY >= -20) //Flies to players Y position
                            {
                                moveSpeedY--;
                                HomeY = 250f;
                            }
                            else if (NPC.Center.Y <= player.Center.Y - HomeY && moveSpeedY <= 20)
                            {

                                moveSpeedY++;
                            }

                            NPC.velocity.Y = moveSpeedY * 0.13f;
                        }
                        else
                        {
                            if (NPC.ai[0] == 51)
                            {
                                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/SunStalker_Charge_TP_In"), NPC.position);
                                if (NPC.position.X >= player.position.X)
                                {
                                    NPC.position.X = player.position.X - 200;
                                    NPC.position.Y = player.position.Y - 300;
                                }
                                else
                                {
                                    NPC.position.X = player.position.X + 200;
                                    NPC.position.Y = player.position.Y - 300;
                                }
                            }

                            if (NPC.ai[0] >= 52)
                            {
                                if (NPC.alpha >= 0)
                                {
                                    if (NPC.Center.X >= player.Center.X && moveSpeed >= -120) // flies to players x position
                                        moveSpeed--;
                                    else if (NPC.Center.X <= player.Center.X && moveSpeed <= 120)
                                        moveSpeed++;

                                    NPC.velocity.X = moveSpeed * 0.10f;

                                    if (NPC.Center.Y >= player.Center.Y - HomeY && moveSpeedY >= -20) //Flies to players Y position
                                    {
                                        moveSpeedY--;
                                        HomeY = 250f;
                                    }
                                    else if (NPC.Center.Y <= player.Center.Y - HomeY && moveSpeedY <= 20)
                                    {

                                        moveSpeedY++;
                                    }

                                    NPC.velocity.Y = moveSpeedY * 0.13f;
                                    NPC.alpha -= 18;
                                }
                            }
                        }

                        if (NPC.ai[0] == 90)
                        {
                            if (StellaMultiplayer.IsHost)
                            {
                                _invincible = false;
                                NPC.netUpdate = true;
                            }
    
                            Glow = true;
                            NPC.alpha = 0;
                            PrevAttack = 4;
                            NPC.ai[3] = 10;
                        }
                        break;
                    case 5:
                        NPC.ai[0]++;
                        if (StellaMultiplayer.IsHost)
                        {
                            if (Main.rand.NextBool(6) && NPC.ai[0] <= 240)
                            {
                                NPC.NewNPC(NPC.GetSource_FromThis(), (int)NPC.Center.X, (int)NPC.Center.Y, 
                                    ModContent.NPCType<SunStalkerRayLightBig>());
                            }
                        }
        
                        if (NPC.ai[0] <= 240)
                        {
                            NPC.rotation = NPC.velocity.X * 0.07f;
                            NPC.velocity *= 0.90f;
                            if (NPC.alpha <= 255)
                            {
                                Glow = false;
                                NPC.alpha += 6;
                            }
                            if (alphaCounter <= 10)
                            {
                                alphaCounter += 0.1f;
                            }
                        }
                        if (NPC.ai[0] == 20)
                        {
                            if (StellaMultiplayer.IsHost)
                            {
                                _invincible = true;
                                NPC.netUpdate = true;
                            }
                      
                            SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/SunStalker_Sun_Start"), NPC.position);
                        }
                        if (NPC.ai[0] == 250)
                        {
                            if (StellaMultiplayer.IsHost)
                            {
                                _invincible = false;
                                NPC.netUpdate = true;
                            }
                            SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/SunStalker_Sun_End"), NPC.position);
                        }
                        if (NPC.ai[0] >= 240)
                        {
                            if (NPC.alpha >= 0)
                            {
                                Glow = true;
                                NPC.alpha -= 6;
                            }
                            if (alphaCounter >= 0)
                            {
                                alphaCounter -= 0.1f;
                            }
                        }
                        if (NPC.ai[0] == 380)
                        {
                            PrevAttack = 5;
                            NPC.ai[3] = 10;
                        }

                        if (NPC.ai[0] == 240 || NPC.ai[0] == 180 || NPC.ai[0] == 60 || NPC.ai[0] == 120)
                        {
                            int Sound = Main.rand.Next(1, 3);
                            if (Sound == 1)
                            {
                                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/SunStalker_Sun_Shot1"), NPC.position);
                            }
                            else
                            {
                                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/SunStalker_Sun_Shot2"), NPC.position);
                            }
                            float offsetRandom = Main.rand.Next(0, 50);
                            Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(base.NPC.Center, 2048f, 124f);
                            for (int i = 0; i < 20; i++)
                            {
                                Dust.NewDustPerfect(NPC.Center, DustID.CopperCoin, (Vector2.One * Main.rand.Next(1, 12)).RotatedByRandom(25.0), 0, default(Color), 2f).noGravity = false;
                            }

                            float spread = 45f * 0.0174f;
                            double startAngle = Math.Atan2(1, 0) - spread / 2;
                            double deltaAngle = spread / 8f;
                            double offsetAngle;
                            for (int i = 0; i < 4; i++)
                            {
                                offsetAngle = (startAngle + deltaAngle * (i + i * i) / 2f) + 32f * i + offsetRandom;
                                if (StellaMultiplayer.IsHost)
                                {
                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y, (float)(Math.Sin(offsetAngle) * 9f), (float)(Math.Cos(offsetAngle) * 9f),
                                        ModContent.ProjectileType<SunDeath>(), 16, 0, Main.myPlayer);
                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y, (float)(-Math.Sin(offsetAngle) * 9f), (float)(-Math.Cos(offsetAngle) * 9f),
                                        ModContent.ProjectileType<SunDeath>(), 16, 0, Main.myPlayer);
                                }
                            }
                        }

                        break;
                    case 10:
                        NPC.ai[0]++;

                        if (NPC.ai[0] >= 2 && StellaMultiplayer.IsHost)
                        {
                            if (Main.rand.NextBool(2))
                            {
                                if(TPChance)
                                {
                                    NPC.ai[0] = 0;
                                    Attack = 5;
                                }
                                else
                                {
                                    NPC.ai[0] = 0;
                                }
             
                            }
                            else
                            {

                                if (NPC.ai[3] == PrevAttack)
                                {
                                    TPChance = false;
                                    NPC.ai[0] = 0;
                                }
                                else
                                {
                                    TPChance = true;
                                    NPC.ai[0] = 0;
                                    NPC.ai[3] = Main.rand.Next(1, Attacks);
                                }
                            }

                            NPC.netUpdate = true;
                        }

                        break;
                }
            }

            if (Main.rand.NextBool(2))
            {
                int dustnumber = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.GoldCoin, 0f, 0f, 150, Color.White, 3f);
                Main.dust[dustnumber].velocity *= 0.3f;
                Main.dust[dustnumber].noGravity = true;
            }

            if (Intro)
            {
                if (!player.active || player.dead || !Main.dayTime || !player.ZoneDesert && !player.ZoneBeach) //despawns when player is ded
                {
                    NPC.spriteDirection = NPC.direction;
                    NPC.ai[0] = 0;
                    NPC.ai[2]++;
                    Attack = -1;

                    if (StellaMultiplayer.IsHost)
                    {
                        _invincible = true;
                        NPC.netUpdate = true;
                    }

                    if (NPC.ai[2] <= 30)
                    {
                        NPC.rotation = NPC.velocity.X * 0.07f;
                        NPC.velocity *= 0.90f;
                    }
                    if (NPC.ai[2] >= 40)
                    {
                        NPC.alpha += 7;

                        if (NPC.alpha >= 200)
                        {
                            NPC.active = false;
                        }
                        NPC.velocity.Y -= 0.90f;
                    }
                    if (NPC.ai[2] == 40)
                    {
                        SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/SunStalker_Charge_TP_Out"), NPC.position);
                    }
                }
            }
        }

        int frame = 0;
        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;
            if (!Dashing)
            {
                if (NPC.frameCounter >= 7)
                {
                    frame++;
                    NPC.frameCounter = 0;
                }
                if (frame >= 6)
                {
                    frame = 0;
                }
            }

            if (Dashing)
            {
                frame = 5;
            }
            NPC.frame.Y = frameHeight * frame;
        }


        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
        {
            int spOff = NPC.alpha / 6;
            SpriteEffects Effects = ((base.NPC.spriteDirection != -1) ? SpriteEffects.FlipHorizontally : SpriteEffects.None);

            if (NPC.ai[3] == 3 && NPC.ai[0] >= 100)
            {
                Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
                Vector2 frameOrigin = NPC.frame.Size();
                Vector2 offset = new Vector2(NPC.width - frameOrigin.X + 10, NPC.height - NPC.frame.Height + 0);
                Vector2 DrawPos = NPC.position - screenPos + frameOrigin + offset;

                float time = Main.GlobalTimeWrappedHourly;
                float timer = Main.GlobalTimeWrappedHourly / 2f + time * 0.04f;

                time %= 4f;
                time /= 2f;

                if (time >= 1f)
                {
                    time = 2f - time;
                }

                time = time * 0.5f + 0.5f;
                for (float i = 0f; i < 1f; i += 0.25f)
                {
                    float radians = (i + timer) * MathHelper.TwoPi;

                    spriteBatch.Draw(texture, DrawPos + new Vector2(0f, DrugRidus).RotatedBy(radians) * time, NPC.frame, new Color(255, 216, 121, 0), NPC.rotation, frameOrigin, NPC.scale, Effects, 0);
                }

                for (float i = 0f; i < 1f; i += 0.34f)
                {
                    float radians = (i + timer) * MathHelper.TwoPi;

                    spriteBatch.Draw(texture, DrawPos + new Vector2(0f, DrugRidus).RotatedBy(radians) * time, NPC.frame, new Color(220, 126, 58, 0), NPC.rotation, frameOrigin, NPC.scale, Effects, 0);
                }



            }



            for (float j = -(float)Math.PI; j <= (float)Math.PI / 3f; j += (float)Math.PI / 3f)
            {
                spriteBatch.Draw((Texture2D)TextureAssets.Npc[base.NPC.type], base.NPC.Center + new Vector2(0f, -2f) + new Vector2(4f + NPC.alpha * 0.25f + spOff, 0f).RotatedBy(base.NPC.rotation + j) - Main.screenPosition, base.NPC.frame, Color.FromNonPremultiplied(255 + spOff * 2, 255 + spOff * 2, 255 + spOff * 2, 100 - base.NPC.alpha), base.NPC.rotation, base.NPC.frame.Size() / 2f, base.NPC.scale, Effects, 0f);
            }
            spriteBatch.Draw((Texture2D)TextureAssets.Npc[base.NPC.type], base.NPC.Center - Main.screenPosition, base.NPC.frame, base.NPC.GetAlpha(lightColor), base.NPC.rotation, base.NPC.frame.Size() / 2f, base.NPC.scale, Effects, 0f);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            for (int k = 0; k < NPC.oldPos.Length; k++)
            {
                Vector2 drawPos = NPC.oldPos[k] - Main.screenPosition + NPC.Size / 2 + new Vector2(0f, NPC.gfxOffY);
                Color color = NPC.GetAlpha(Color.Lerp(new Color(248, 228, 171), new Color(220, 126, 58), 1f / NPC.oldPos.Length * k) * (1f - 1f / NPC.oldPos.Length * k));
                spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, drawPos, new Microsoft.Xna.Framework.Rectangle?(NPC.frame), color, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, Effects, 0f);
            }

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            return true;
        }
        public virtual string GlowTexturePath => Texture + "_Glow";
        private Asset<Texture2D> _glowTexture;
        public Texture2D GlowTexture => (_glowTexture ??= (ModContent.RequestIfExists<Texture2D>(GlowTexturePath, out var asset) ? asset : null))?.Value;
        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (GlowTexture is not null && Glow)
            {
                Lighting.AddLight(NPC.Center, Color.LightGoldenrodYellow.ToVector3() * 1.75f * Main.essScale);
                SpriteEffects spriteEffects = SpriteEffects.None;
                if (NPC.spriteDirection == 1)
                {
                    spriteEffects = SpriteEffects.FlipHorizontally;
                }
                Vector2 halfSize = new Vector2(GlowTexture.Width / 2, GlowTexture.Height / Main.npcFrameCount[NPC.type] / 2);
                spriteBatch.Draw(
                    GlowTexture,
                    new Vector2(NPC.position.X - screenPos.X + (NPC.width / 2) - GlowTexture.Width * NPC.scale / 2f + halfSize.X * NPC.scale, NPC.position.Y - screenPos.Y + NPC.height - GlowTexture.Height * NPC.scale / Main.npcFrameCount[NPC.type] + 4f + halfSize.Y * NPC.scale + Main.NPCAddHeight(NPC) + NPC.gfxOffY),
                    NPC.frame,
                    Color.White,
                    NPC.rotation,
                    halfSize,
                    NPC.scale,
                    spriteEffects,
                0);
            }
            Texture2D texture2D4 = ModContent.Request<Texture2D>("Stellamod/Effects/Masks/DimLight").Value;
            Main.spriteBatch.Draw(texture2D4, (NPC.Center - Main.screenPosition), null, new Color((int)(55f * alphaCounter), (int)(55f * alphaCounter), (int)(45f * alphaCounter), 0), NPC.rotation, new Vector2(64 / 2, 64 / 2), 0.2f * (counter + 0.3f), SpriteEffects.None, 0f);


            Texture2D texture2D5 = ModContent.Request<Texture2D>("Stellamod/Effects/Masks/DimLight").Value;
            Main.spriteBatch.Draw(texture2D5, (NPC.Center - Main.screenPosition), null, new Color((int)(55f * alphaCounter), (int)(55f * alphaCounter), (int)(15f * alphaCounter), 0), NPC.rotation, new Vector2(64 / 2, 64 / 2), 0.2f * (counter + 0.3f * 2), SpriteEffects.None, 0f);

            Texture2D texture2D6 = ModContent.Request<Texture2D>("Stellamod/Effects/Masks/DimLight").Value;
            Main.spriteBatch.Draw(texture2D6, (NPC.Center - Main.screenPosition), null, new Color((int)(55f * alphaCounter), (int)(55f * alphaCounter), (int)(15f * alphaCounter), 0), NPC.rotation, new Vector2(64 / 2, 64 / 2), 0.2f * (counter + 0.3f * 6), SpriteEffects.None, 0f);

            Texture2D texture2D7 = ModContent.Request<Texture2D>("Stellamod/Effects/Masks/DimLight").Value;
            Main.spriteBatch.Draw(texture2D7, (NPC.Center - Main.screenPosition), null, new Color((int)(55f * alphaCounter), (int)(55f * alphaCounter), (int)(15f * alphaCounter), 0), NPC.rotation, new Vector2(64 / 2, 64 / 2), 0.2f * (counter + 0.3f * 7), SpriteEffects.None, 0f);
        }
        float alphaCounter = 0;
        float counter = 12;
        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0)
            {
                var entitySource = NPC.GetSource_FromThis();
                NPC.NewNPC(entitySource, (int)NPC.Center.X, (int)NPC.Center.Y + 50, ModContent.NPCType<SunStalkerDeath>());
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.CopperCoin, 2.5f * hit.HitDirection, -2.5f, 0, default(Color), 1.2f);
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.CopperCoin, 2.5f * hit.HitDirection, -2.5f, 0, default(Color), 0.5f);

            }
            else
            {
                for (int k = 0; k < 27; k++)
                {
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.CopperCoin, 2.5f * hit.HitDirection, -2.5f, 0, default(Color), 1.2f);
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.CopperCoin, 2.5f * hit.HitDirection, -2.5f, 0, default(Color), 0.5f);
                }
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<SunStalkerBag>()));
            npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ModContent.ItemType<Items.Placeable.SunsBossRel>()));
            LeadingConditionRule notExpertRule = new LeadingConditionRule(new Conditions.NotExpert());
            notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<WingedFury>(), 2));
            notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<SunGlyph>()));
            notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<StalkersTallon>(), 2));
            notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<SunBlastStaff>(), 2));
            npcLoot.Add(notExpertRule);
        }
    }
}