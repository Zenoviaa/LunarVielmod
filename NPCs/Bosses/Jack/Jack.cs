
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Helpers;
using Stellamod.Items.Accessories.Igniter;
using Stellamod.Items.Consumables;
using Stellamod.Items.Harvesting;
using Stellamod.Items.Materials;
using Stellamod.Items.Weapons.Mage;
using Stellamod.Items.Weapons.Melee;
using Stellamod.Items.Weapons.Ranged;
using Stellamod.Utilis;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;


//By Al0n37
namespace Stellamod.NPCs.Bosses.Jack
{
    [AutoloadBossHead]
    public class Jack : ModNPC
    {
        public int JackFirerand = 0;
        public int PrevAtack;
        public float DrugRidus = 0;
        public int DrugAlpha = 0;
        int moveSpeed = 0;
        int moveSpeedY = 0;
        float HomeY = 330f;
        private bool p2 = false;
        bool Chucking;
        bool Jumping;
        bool GVI;
        public override void SetStaticDefaults()
        {
            NPCID.Sets.TrailCacheLength[NPC.type] = 4;
            NPCID.Sets.TrailingMode[NPC.type] = 0;
            NPCID.Sets.MPAllowedEnemies[NPC.type] = true;
            Main.npcFrameCount[NPC.type] = 12;


            NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers();
            drawModifiers.CustomTexturePath = "Stellamod/NPCs/Bosses/Jack/JackBestiary";
            drawModifiers.PortraitScale = 1f; // Portrait refers to the full picture when clicking on the icon in the bestiary
            drawModifiers.PortraitPositionYOverride = 0f;
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);
        }

        public override void SetDefaults()
        {
            NPC.alpha = 255;
            NPC.width = 30;
            NPC.height = 75;
            NPC.damage = 32;
            NPC.defense = 6;         
            NPC.lifeMax = 1100;
            NPC.HitSound = SoundID.NPCHit16;
            NPC.value = Item.buyPrice(silver: 50);
            NPC.knockBackResist = 0.0f;
            NPC.noGravity = false;
            NPC.boss = true;
            NPC.alpha = 255;
            NPC.npcSlots = 10f;
            Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/Jack");
            NPC.aiStyle = 0;
            NPC.BossBar = ModContent.GetInstance<JackBossBar>();
        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * balance);
        }

        int frame = 0;
        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter += 0.5f;
            if (Jumping)
            {
                if (NPC.frameCounter >= 8)
                {
                    frame++;
                    NPC.frameCounter = 5;
                }
                if (frame >= 7)
                {
                    frame = 5;
                }
                if (frame < 5)
                {
                    frame = 5;
                }
            }
            else if(Chucking)
            {
                if (NPC.frameCounter >= 12)
                {
                    frame++;
                    NPC.frameCounter = 5;
                }
                if (frame >= 11)
                {
                    Chucking = false;
                }
                if (frame < 9)
                {
                    frame = 9;
                }
            }
            else
            {
                if (NPC.frameCounter >= 4)
                {
                    frame++;
                    NPC.frameCounter = 0;
                }
                if (frame >= 3)
                {
                    frame = 0;
                }
            }

            NPC.frame.Y = frameHeight * frame;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Graveyard,
                new FlavorTextBestiaryInfoElement(LangText.Bestiary(this, "A scarecrow reanimated by the passion of wandering flames, exploring out of the veil.")),
            });
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0)
            {
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Jack_Death1"), NPC.position);
                Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(base.NPC.Center, 2048f, 128f);
                var entitySource = NPC.GetSource_FromThis();
                if (StellaMultiplayer.IsHost)
                {
                    NPC.NewNPC(entitySource, (int)NPC.Center.X, (int)NPC.Center.Y,
                        ModContent.NPCType<JackDeath>());
                }

                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Hay, 2.5f * hit.HitDirection, -2.5f, 0, default(Color), 1.2f);
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Hay, 2.5f * hit.HitDirection, -2.5f, 0, default(Color), 0.5f);
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Hay, 2.5f * hit.HitDirection, -2.5f, 0, default(Color), 1.2f);
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Hay, 2.5f * hit.HitDirection, -2.5f, 0, default(Color), 0.5f);
            }
            else
            {
                for (int k = 0; k < 7; k++)
                {

                    Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Hay, 2.5f * hit.HitDirection, -2.5f, 0, default(Color), 1.2f);
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Hay, 2.5f * hit.HitDirection, -2.5f, 0, default(Color), 0.5f);
                }
            }
        }
        Vector2 Drawoffset => new Vector2(0, NPC.gfxOffY) + Vector2.UnitX * NPC.spriteDirection * 0;
        public virtual string GlowTexturePath => Texture + "_Outline";
        private Asset<Texture2D> _glowTexture;
        public Texture2D GlowTexture => (_glowTexture ??= (ModContent.RequestIfExists<Texture2D>(GlowTexturePath, out var asset) ? asset : null))?.Value;
        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (NPC.ai[0] >= 20 && NPC.ai[1] == 2)
            {
                float num108 = 4;
                float num107 = (float)Math.Cos((double)(Main.GlobalTimeWrappedHourly % 1.4f / 1.4f * 6.28318548f)) / 2f + 0.5f;
                float num106 = 0f;
                Color color1 = Color.LightBlue * num107 * .8f;
                var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
                spriteBatch.Draw(
                    GlowTexture,
                    NPC.Center - Main.screenPosition + Drawoffset,
                    NPC.frame,
                    color1,
                    NPC.rotation,
                    NPC.frame.Size() / 2,
                    NPC.scale,
                    effects,
                    0
                );

                SpriteEffects spriteEffects3 = (NPC.spriteDirection == 1) ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
                //Vector2 vector33 = new Vector2(NPC.Center.X, NPC.Center.Y) - Main.screenPosition + Drawoffset - NPC.velocity;
                Color color29 = new Color(127 - NPC.alpha, 127 - NPC.alpha, 127 - NPC.alpha, 0).MultiplyRGBA(Color.LightBlue);
                for (int num103 = 0; num103 < 4; num103++)
                {
                    Color color28 = color29;
                    color28 = NPC.GetAlpha(color28);
                    color28 *= 1f - num107;
                    Vector2 vector29 = NPC.Center + (num103 / (float)num108 * 6.28318548f + NPC.rotation + num106).ToRotationVector2() * (4f * num107 + 2f) - Main.screenPosition + Drawoffset - NPC.velocity * num103;
                    Main.spriteBatch.Draw(GlowTexture, vector29, NPC.frame, color28, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, spriteEffects3, 0f);
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
        {
            SpriteEffects Effects = NPC.spriteDirection != -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Lighting.AddLight(NPC.Center, Color.Orange.ToVector3() * 2.25f * Main.essScale);
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            //var drawOrigin = new Vector2(TextureAssets.Npc[NPC.type].Width() * 0.5f, NPC.height * 0.5f);
            for (int k = 0; k < NPC.oldPos.Length; k++)
            {
                Vector2 drawPos = NPC.oldPos[k] - Main.screenPosition + NPC.Size / 2 + new Vector2(0f, NPC.gfxOffY);
                Color color = NPC.GetAlpha(Color.Lerp(new Color(255, 255, 113), new Color(232, 111, 24), 1f / NPC.oldPos.Length * k) * (1f - 1f / NPC.oldPos.Length * k));
                spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, drawPos, new Microsoft.Xna.Framework.Rectangle?(NPC.frame), color, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, Effects, 0f);
            }

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            return true;
        }

        public float Spawner = 0;

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(JackFirerand);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            JackFirerand = reader.ReadInt32();
        }

        public override void AI()
        {
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

            if (NPC.ai[2] == 10)
            {
                if (NPC.alpha >= 0)
                {
                    NPC.alpha -= 5;
                }
                NPC.ai[0]++;
                if (NPC.ai[0] == 2)
                {
                    SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Jack_Spawn"), NPC.position);
                    Utilities.NewProjectileBetter(NPC.Center.X, NPC.Center.Y + 600, 0, -10, ModContent.ProjectileType<JackSpawnRay>(), 50, 0f, -1, 0, NPC.whoAmI);
                }
                if (NPC.ai[0] == 80)
                {
                    SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Jack_Laugh"), NPC.position);
                    Chucking = true;
                }
                if (NPC.ai[0] == 100)
                {
                    NPC.ai[2] = 1;
                }
            }
            if (p2)
            {
                NPC.noTileCollide = true;
                NPC.rotation = NPC.velocity.X * 0.07f;
                if (!GVI)
                {
                    if (Main.GraveyardVisualIntensity <= 0.4)
                    {
                        Main.GraveyardVisualIntensity += 0.02f;
                    }
                    else
                    {
                        GVI = true;
                    }
                }
                else
                {
                    Main.GraveyardVisualIntensity = 0.4f;
                }


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
            if (NPC.ai[2] == 1)
            {
                switch (NPC.ai[1])
                {
                    case 0:
                        NPC.alpha -= 50;
                        NPC.ai[0]++;
                        if (NPC.ai[0] > 20)
                        {
                            NPC.ai[0] = 0;
                            NPC.ai[1] = 3;
                        }
                        break;
                    case 1:
                        NPC.ai[0]++;
                        if (NPC.ai[0] >= 2)
                        {
                            if (StellaMultiplayer.IsHost)
                            {
                                int Atack = Main.rand.Next(2, 6 + 1);
                                if (Atack == PrevAtack)
                                {
                                    NPC.ai[0] = 1;
                                }
                                else
                                {
                                    NPC.ai[0] = 0;
                                    NPC.ai[1] = Atack;
                                }
                                NPC.netUpdate = true;
                            }
                        }

                        break;
                    case 2:
                        NPC.ai[0]++;
                        if (p2)
                        {
                            if (NPC.ai[0] == 20)
                            {
                                Vector2 direction = Vector2.Normalize(Main.player[NPC.target].Center - NPC.Center) * 8.5f;
                                if (StellaMultiplayer.IsHost)
                                {
                                    int amountOfProjectiles = Main.rand.Next(1, 3);
                                    for (int i = 0; i < amountOfProjectiles; ++i)
                                    {
                                        float offsetX = Main.rand.Next(-200, 200) * 0.01f;
                                        float offsetY = Main.rand.Next(-200, 200) * 0.01f;
                                        int damage = Main.expertMode ? 10 : 13;
                                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y, direction.X + offsetX, direction.Y + offsetY, 
                                            ModContent.ProjectileType<JackFire2>(), damage, 1, Owner: Main.myPlayer);
                                    }
                                }

                            }
                            NPC.velocity *= 0.96f;
                            if (NPC.ai[0] == 80)
                            {
                                Vector2 direction = Vector2.Normalize(Main.player[NPC.target].Center - NPC.Center) * 8.5f;
                                if (StellaMultiplayer.IsHost)
                                {
                                    int amountOfProjectiles = Main.rand.Next(1, 3);
                                    for (int i = 0; i < amountOfProjectiles; ++i)
                                    {
                                        float offsetX = Main.rand.Next(-200, 200) * 0.01f;
                                        float offsetY = Main.rand.Next(-200, 200) * 0.01f;
                                        int damage = Main.expertMode ? 9 : 12;
                                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y, direction.X + offsetX, direction.Y + offsetY, 
                                            ModContent.ProjectileType<JackFire2>(), damage, 1, Owner: Main.myPlayer);
                                    }
                                }

                            }
                            if (NPC.ai[0] >= 100)
                            {
                                NPC.netUpdate = true;
                                PrevAtack = 2;
                                NPC.ai[1] = 1;
                                NPC.ai[0] = 0;
                            }

                        }
                        else
                        {
                            if (NPC.ai[0] == 20)
                            {
                                if (StellaMultiplayer.IsHost)
                                {
                                    JackFirerand = Main.rand.Next(25, 40 + 1);
                                    NPC.netUpdate = true;
                                }
                          
                                NPC.noGravity = true;
                                NPC.noTileCollide = true;
                                NPC.velocity.Y -= 15;
                            }
                            if (NPC.ai[0] <= 330 && NPC.ai[0] >= 25)
                            {
                                NPC.velocity.Y *= 0.90f;
                            }

                            if (NPC.ai[0] >= 100 && NPC.ai[0] <= 280)
                            {
                                if (NPC.ai[0] % JackFirerand == 0)
                                {
                                    SoundEngine.PlaySound(SoundID.DD2_BetsyFireballShot, NPC.position);
                                    SoundEngine.PlaySound(SoundID.DD2_EtherianPortalSpawnEnemy, NPC.position);
                                    if (StellaMultiplayer.IsHost)
                                    {
                                        var entitySource = NPC.GetSource_FromThis();
                                        int OffSet = Main.rand.Next(-130, 130 + 1);
                                        Vector2 NukePos;
                                        NukePos.X = NPC.Center.X + OffSet;
                                        NukePos.Y = NPC.Center.Y;
                                        Projectile.NewProjectile(entitySource, NukePos, new Vector2(0, 0), 
                                            Mod.Find<ModProjectile>("JackSpawnEffect").Type, 0, 0, Owner: Main.myPlayer);
                                        Projectile.NewProjectile(entitySource, NukePos, new Vector2(0, 0), 
                                            Mod.Find<ModProjectile>("JackFire").Type, NPC.damage / 4, 0, Owner: Main.myPlayer);
                                    }
                                }
                            }

                            if (NPC.ai[0] == 280)
                            {
                                NPC.noGravity = false;
                                NPC.noTileCollide = false;
                            }

                            if (NPC.ai[0] >= 330)
                            {
                                PrevAtack = 2;
                                NPC.ai[1] = 1;
                                NPC.ai[0] = 0;
                            }
                        }

                        break;
                    case 3:
                        NPC.ai[0]++;
                        if (p2)
                        {
                            NPC.velocity *= 0.92f;
                            if (NPC.ai[0] == 20)
                            {
                                NPC.noTileCollide = true;
                                Jumping = true;
                                NPC.velocity.Y -= 10;
                                if (NPC.position.X <= player.position.X)
                                {
                                    NPC.velocity.X += 5;
                                }
                                else
                                {
                                    NPC.velocity.X -= 5;
                                }
                            }
                            if (NPC.ai[0] >= 20 && NPC.ai[0] <= 100)
                            {
                                NPC.noTileCollide = false;
                                if (NPC.ai[0] % 7 == 0)
                                {
                                    SoundEngine.PlaySound(SoundID.DD2_EtherianPortalSpawnEnemy, NPC.position);
                                    if (StellaMultiplayer.IsHost)
                                    {
                                        var entitySource = NPC.GetSource_FromThis();
                                        int OffSet = Main.rand.Next(-30, 30 + 1);
                                        Vector2 NukePos;
                                        NukePos.X = NPC.Center.X + OffSet;
                                        NukePos.Y = NPC.Center.Y;

                                        Projectile.NewProjectile(entitySource, NukePos, new Vector2(OffSet / 10, 6),
                                            Mod.Find<ModProjectile>("FallingHay").Type, 12, 0, Owner: Main.myPlayer);
                                    }
                                }
                            }

                            if (NPC.ai[0] >= 100)
                            {
                                Jumping = false;
                                PrevAtack = 3;
                                NPC.ai[1] = 1;
                                NPC.ai[0] = 0;
                            }
                        }
                        else
                        {
                            if (NPC.ai[0] == 20)
                            {
                                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Jack_Jump"), NPC.position);
                                Jumping = true;
                                NPC.velocity.Y -= 10;
                                if (NPC.position.X <= player.position.X)
                                {
                                    NPC.velocity.X += 5;
                                }
                                else
                                {
                                    NPC.velocity.X -= 5;
                                }
                            }

                            if (NPC.ai[0] <= 50)
                            {
                                NPC.velocity.X *= 1.10f;
                            }
                            if (NPC.ai[0] >= 50)
                            {
                                if (NPC.collideY || NPC.collideX)
                                {
                                    if (Jumping)
                                    {
                                        SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Jack_Land"), NPC.position);
                                        Jumping = false;
                                    }
                                    NPC.velocity.X *= 0.1f;
                                }
                                else
                                {
                                    NPC.velocity.X *= 1.05f;
                                }
                            }

                            if (NPC.ai[0] >= 100)
                            {
                                NPC.netUpdate = true;
                                PrevAtack = 3;
                                NPC.ai[1] = 1;
                                NPC.ai[0] = 0;
                            }
                        }

                        break;
                    case 4:
                        NPC.ai[0]++;
                        if (NPC.ai[0] == 2)
                        {
                            Chucking = true;
                        }
                        if (NPC.ai[0] == 30)
                        {
                            SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Jack_Throw"), NPC.position);
                            Vector2 direction = Vector2.Normalize(Main.player[NPC.target].Center - NPC.Center) * 8.5f;

                            for (int i = 0; i < 50; i++)
                            {
                                int num = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.FlameBurst, 0f, -2f, 0, default(Color), 1.5f);
                                Main.dust[num].noGravity = true;
                                Main.dust[num].position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
                                Main.dust[num].position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
                                {
                                    Main.dust[num].velocity = NPC.DirectionTo(Main.dust[num].position) * 6f;
                                }
                            }

                            if (StellaMultiplayer.IsHost)
                            {
                                int amountOfProjectiles = Main.rand.Next(1, 2);
                                for (int i = 0; i < amountOfProjectiles; ++i)
                                {
                                    float offsetX = Main.rand.Next(-200, 200) * 0.01f;
                                    float offsetY = Main.rand.Next(-200, 200) * 0.01f;
                                    int damage = Main.expertMode ? 8 : 11;
                                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y, direction.X + offsetX, direction.Y + offsetY - 8, 
                                        ModContent.ProjectileType<JackoBall>(), damage, 1, Owner: Main.myPlayer);
                                }
                            }

                        }
                        if (NPC.ai[0] >= 50)
                        {
                            NPC.netUpdate = true;
                            PrevAtack = 4;
                            NPC.ai[1] = 1;
                            NPC.ai[0] = 0;
                        }
                        break;
                    case 5:
                        NPC.ai[0]++;
                        if (NPC.ai[0] == 2)
                        {
                            Chucking = true;
                        }
                        if (NPC.ai[0] == 30)
                        {
                            SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Jack_Throw"), NPC.position);
                            Vector2 direction = Vector2.Normalize(Main.player[NPC.target].Center - NPC.Center) * 8.5f;

                            for (int i = 0; i < 50; i++)
                            {
                                int num = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Grass, 0f, -2f, 0, default(Color), 1.5f);
                                Main.dust[num].noGravity = true;
                                Main.dust[num].position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
                                Main.dust[num].position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
                                {
                                    Main.dust[num].velocity = NPC.DirectionTo(Main.dust[num].position) * 6f;
                                }
                            }

                            if (StellaMultiplayer.IsHost)
                            {
                                int amountOfProjectiles = Main.rand.Next(2, 4);
                                for (int i = 0; i < amountOfProjectiles; ++i)
                                {
                                    float offsetX = Main.rand.Next(-200, 200) * 0.01f;
                                    float offsetY = Main.rand.Next(-200, 200) * 0.01f;
                                    int damage = Main.expertMode ? 4 : 6;
                                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y, direction.X + offsetX, direction.Y + offsetY - 5, 
                                        ModContent.ProjectileType<MossBall>(), damage, 1, Owner: Main.myPlayer);
                                }
                            }
                        }
                        if (NPC.ai[0] >= 50)
                        {
                            PrevAtack = 5;
                            NPC.ai[1] = 1;
                            NPC.ai[0] = 0;
                        }
                        break;
                    case 6:
                        NPC.ai[0]++;
                        if (p2)
                        {
                            if (NPC.ai[0] >= 2)
                            {
                                PrevAtack = 6;
                                NPC.ai[1] = 1;
                                NPC.ai[0] = 0;
                            }
                        }
                        else
                        {
                            if (NPC.collideY || NPC.collideX)
                            {
                                NPC.velocity.X *= 0.1f;

                                if (NPC.ai[0] < 60 && NPC.ai[0] >= 30)
                                {
                                    SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Jack_Jump"), NPC.position);
                                    NPC.ai[0] = 60;
                                }
                                if (NPC.ai[0] < 100 && NPC.ai[0] >= 70)
                                {
                                    SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Jack_Jump"), NPC.position);
                                    NPC.ai[0] = 100;
                                }
                            }
                            else
                            {
                                NPC.velocity.X *= 1.05f;
                            }

                            NPC.velocity.Y += 0.1f;
                            if (NPC.ai[0] == 20)
                            {
                                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Jack_Jump"), NPC.position);
                                NPC.velocity.Y -= 7;
                                if (NPC.position.X <= player.position.X)
                                {
                                    NPC.velocity.X += 4;
                                }
                                else
                                {
                                    NPC.velocity.X -= 4;
                                }
                            }

                            if (NPC.ai[0] == 60)
                            {
                                NPC.velocity.Y -= 7;
                                if (NPC.position.X <= player.position.X)
                                {
                                    NPC.velocity.X += 4;
                                }
                                else
                                {
                                    NPC.velocity.X -= 4;
                                }
                            }

                            if (NPC.ai[0] == 100)
                            {
                                NPC.velocity.Y -= 7;
                                if (NPC.position.X <= player.position.X)
                                {
                                    NPC.velocity.X += 4;
                                }
                                else
                                {
                                    NPC.velocity.X -= 4;
                                }
                            }

                            if (NPC.ai[0] >= 150)
                            {
                                PrevAtack = 6;
                                NPC.ai[1] = 1;
                                NPC.ai[0] = 0;
                            }
                        }

                        break;
                }
            }
        }

        public override void OnKill()
        {
            NPC.SetEventFlagCleared(ref DownedBossSystem.downedJackBoss, -1);
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<JackoBag>()));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Gambit>(), 1, 1, 1));
            LeadingConditionRule notExpertRule = new LeadingConditionRule(new Conditions.NotExpert());
                notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<WanderingFlame>(), minimumDropped: 20, maximumDropped: 50));
                notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<JackoShot>(), chanceDenominator: 2));
                notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<StaffOFlame>(), chanceDenominator: 2));  
                notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<ScarecrowSaber>(), chanceDenominator: 2));
                notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<Cinderscrap>(), minimumDropped: 7, maximumDropped: 50));
                notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<AlcadizScrap>(), minimumDropped: 7, maximumDropped: 50));
                notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<TomedDustingFlames>(), chanceDenominator: 1));

            //Dunno if she should drop verlia brooch in classic mode or not
            npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ModContent.ItemType<Items.Placeable.JackBossRel>()));
            npcLoot.Add(notExpertRule);
        }
    }
}