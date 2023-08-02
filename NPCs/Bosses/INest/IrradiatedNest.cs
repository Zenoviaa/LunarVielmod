
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
using Stellamod.Items.Consumables;
using Stellamod.Items.Materials;
using Stellamod.Items.Weapons.Ranged;
using Stellamod.Items.Weapons.Mage;
using Stellamod.Items.Weapons.Melee;
using Terraria.GameContent.ItemDropRules;
using Stellamod.Items.Weapons.Melee;
using Stellamod.Items.Weapons.Ranged;
using Stellamod.Items.Weapons.Melee.Spears;


//By Al0n37
namespace Stellamod.NPCs.Bosses.INest
{
    [AutoloadBossHead]
    public class IrradiatedNest : ModNPC
    {
        private bool HasHitGround = false;
        public float DrugRidus = 0;
        public int DrugAlpha = 0;
        private bool Spawned = false;
        private bool p2 = false;
        bool ToFar;
        bool Nukeing;
        bool Nukes;

        public override void SetStaticDefaults()
        {
            NPCID.Sets.TrailCacheLength[NPC.type] = 14;
            NPCID.Sets.TrailingMode[NPC.type] = 0;
            // DisplayName.SetDefault("Irradiated Nest");
            Main.npcFrameCount[NPC.type] = 20;
        }
        public override void SetDefaults()
        {
            NPC.alpha = 255;
            NPC.width = 150;
            NPC.height = 60;
            NPC.damage = 30;
            NPC.defense = 15;
            NPC.lifeMax = 3650;
            NPC.HitSound = SoundID.NPCHit42;
            NPC.value = 60f;
            NPC.knockBackResist = 0.0f;
            NPC.noGravity = false;
        }
        int frame = 0;
        public override void FindFrame(int frameHeight)
        {


            bool expertMode = Main.expertMode;
            Player player = Main.player[NPC.target];
            NPC.frameCounter++;

            if (!Nukeing)
            {
                if (Nukes)
                {
                    if (frame <= 16)
                    {
                        frame = 17;
                    }
                    if (NPC.frameCounter >= 21)
                    {
                        Nukes = false;
                        frame++;
                        NPC.frameCounter = 0;
                    }
                    if (frame >= 21)
                    {
                        Nukes = false;
                        frame = 0;
                    }
                }
                else
                {
                    if (NPC.frameCounter >= 5)
                    {
                        frame++;
                        NPC.frameCounter = 0;
                    }
                    if (frame >= 4)
                    {
                        frame = 0;
                    }
                }

            }

            if (Nukeing)
            {

                if (frame <= 4)
                {
                    Nukes = true;
                    frame = 5;
                }


                if (NPC.frameCounter >= 17)
                {
                    frame++;
                    NPC.frameCounter = 13;
                }
                if (frame >= 16)
                {
                    frame = 13;
                }
            }
            NPC.frame.Y = frameHeight * frame;

        }


        bool CutScene;
        bool CutScene2;
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<StaffOFlame>(), 2, 1, 3));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<IrradiatedGreatBlade>(), 2, 1, 3));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<IrradiatedGreatBlade>(), 2, 1, 3));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<TheIrradiaspear>(), 2, 1, 3));
            npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<NestBag>()));

        }
        public virtual string GlowTexturePath => Texture + "_Glow";
        private Asset<Texture2D> _glowTexture;
        public Texture2D GlowTexture => (_glowTexture ??= (ModContent.RequestIfExists<Texture2D>(GlowTexturePath, out var asset) ? asset : null))?.Value;
        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (GlowTexture is not null)
            {
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
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
        {


            SpriteEffects Effects = NPC.spriteDirection != -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;


            Vector2 center = NPC.Center + new Vector2(0f, NPC.height * -0.1f);
            // This creates a randomly rotated vector of length 1, which gets it's components multiplied by the parameters
            Vector2 direction = Main.rand.NextVector2CircularEdge(NPC.width * 0.6f, NPC.height * 0.6f);
            float distance = 0.3f + Main.rand.NextFloat() * 0.5f;
            Vector2 velocity = new Vector2(0f, -Main.rand.NextFloat() * 0.3f - 1.5f);
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

                spriteBatch.Draw(texture, DrawPos + new Vector2(0f, DrugRidus).RotatedBy(radians) * time, NPC.frame, new Color(53, 107, 112, DrugAlpha), NPC.rotation, frameOrigin, NPC.scale, Effects, 0);
            }

            for (float i = 0f; i < 1f; i += 0.34f)
            {
                float radians = (i + timer) * MathHelper.TwoPi;

                spriteBatch.Draw(texture, DrawPos + new Vector2(0f, DrugRidus).RotatedBy(radians) * time, NPC.frame, new Color(152, 208, 113, DrugAlpha), NPC.rotation, frameOrigin, NPC.scale, Effects, 0);
            }


            Lighting.AddLight(NPC.Center, Color.GreenYellow.ToVector3() * 2.25f * Main.essScale);
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            var drawOrigin = new Vector2(TextureAssets.Npc[NPC.type].Width() * 0.5f, NPC.height * 0.5f);
            for (int k = 0; k < NPC.oldPos.Length; k++)
            {
                Vector2 drawPos = NPC.oldPos[k] - Main.screenPosition + NPC.Size / 2 + new Vector2(0f, NPC.gfxOffY);
                Color color = NPC.GetAlpha(Color.Lerp(new Color(152, 208, 113), new Color(53, 107, 112), 1f / NPC.oldPos.Length * k) * (1f - 1f / NPC.oldPos.Length * k));
                spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, drawPos, new Microsoft.Xna.Framework.Rectangle?(NPC.frame), color, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, Effects, 0f);
            }

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            return true;
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
            if (distance > 3000f || playerT.dead || !playerT.ZoneAcid())
            {
                player.GetModPlayer<MyPlayer>().IrradiatedKilled = 0;
                NPC.ai[0] = 0;
                NPC.ai[3]++;
                NPC.position.Y = player.Center.Y + -800;
                if(NPC.ai[3] >= 80)
                {
                    NPC.active = false;
                }
            }
            if (DrugRidus >= 0)
            {
                DrugRidus -= 1.5f;
            }
            p2 = NPC.life < NPC.lifeMax * 0.5f;
            if (!p2)
            {
                Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/Irradiated_Nest");
            }
            if (p2 && !CutScene)
            {
                NPC.alpha -= 30;
                NPC.dontTakeDamage = true;
                NPC.dontCountMe = true;
                NPC.ai[0] = 0;
                NPC.ai[3]++;
                if (NPC.ai[3] == 420 - 150)
                {
                    Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/Irradiated_Cutscene");
                    CombatText.NewText(new Rectangle((int)NPC.position.X, (int)NPC.position.Y, NPC.width, NPC.height), new Color(152, 208, 113, 44), "Significant damage detected...");
                }
                if (NPC.ai[3] == 1)
                {
                    Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/Irradiated_Cutscene");
                    Nukeing = false;
                }
                if (NPC.ai[3] == 560 - 150)
                {
                    Nukeing = true;
                    CombatText.NewText(new Rectangle((int)NPC.position.X, (int)NPC.position.Y, NPC.width, NPC.height), new Color(152, 208, 113, 44), "deploying Communication transmission device");
                }
                if (NPC.ai[3] == 730 - 150)
                {
                    DrugRidus = 50;
                    SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/IrradiatedNest_ComuicationRay"));
                    Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(base.NPC.Center, 2048f, 512f);
                    Utilities.NewProjectileBetter(NPC.Center.X, NPC.Center.Y, 0, -10, ModContent.ProjectileType<CommunicationRay>(), 50, 0f, -1, 0, NPC.whoAmI);

                }
                if (NPC.ai[3] == 780 - 150)
                {

                    SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/IrradiatedNest_Comuicating"));

                }
                if (NPC.ai[3] == 820 - 150)
                {
                    Nukeing = false;


                }
                if (NPC.ai[3] == 1170 - 150)
                {
                    player.GetModPlayer<MyPlayer>().IrradiatedKilled = 0;
                    var entitySource = NPC.GetSource_FromThis();
                    NPC.NewNPC(entitySource, (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<Irradieagle>());
                    CombatText.NewText(new Rectangle((int)NPC.position.X, (int)NPC.position.Y, NPC.width, NPC.height), new Color(152, 208, 113, 44), "Transmission successful!");
                    Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(base.NPC.Center, 2048f, 512f);
                    NPC.netUpdate = true;
                }
                if (NPC.ai[3] == 1250 - 150)
                {
                    Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/Irradieagle_Wrath");
                    NPC.defense = 24;
                    NPC.ai[3] = 0;
                    NPC.ai[1] = 0;
                    NPC.ai[0] = 0;
                    CutScene = true;
                    NPC.life = 300;
                    NPC.netUpdate = true;

                }
            }
            if (!Spawned)
            {
                Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/Irradiated_Nest");
                Spawned = true;
                NPC.ai[2] = 1;
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/IrradiatedNest_Land"));
                Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(base.NPC.Center, 2048f, 512f);
                NPC.netUpdate = true;
                NPC.alpha = 0;
            }

            if (playerT.GetModPlayer<MyPlayer>().IrradiatedKilled == 1 && !CutScene2)
            {
                NPC.alpha -= 30;
                NPC.ai[0] = 0;
                NPC.ai[3]++;
                if (NPC.ai[3] == 20)
                {
                    NPC.dontTakeDamage = true;
                    NPC.dontCountMe = true;
                    CombatText.NewText(new Rectangle((int)NPC.position.X, (int)NPC.position.Y, NPC.width, NPC.height), new Color(152, 208, 113, 44), "Back up vessel destroyed...");
                    NPC.netUpdate = true;
                }
                if (NPC.ai[3] == 1)
                {
                    NPC.dontTakeDamage = true;
                    NPC.dontCountMe = true;
                    Nukeing = false;
                }
                if (NPC.ai[3] == 160)
                {
                    NPC.dontTakeDamage = true;
                    NPC.dontCountMe = true;
                    Nukeing = true;
                    CombatText.NewText(new Rectangle((int)NPC.position.X, (int)NPC.position.Y, NPC.width, NPC.height), new Color(152, 208, 113, 44), "Proceed with D. S. D. P!");
                    NPC.netUpdate = true;
                }

                if (NPC.ai[3] == 200)
                {
                    NPC.dontTakeDamage = false;
                    NPC.dontCountMe = false;
                    Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/Irradieagle_Wrath");
                    NPC.defense = 30;
                    NPC.ai[3] = 0;
                    NPC.ai[1] = 4;
                    NPC.ai[0] = 0;
                    CutScene2 = true;
                    NPC.life = 1000;
                    NPC.netUpdate = true;
                }
            }
            Vector2 targetPos;
            if (distance > 1000f && !ToFar)
            {
                ToFar = true;
                NPC.ai[1] = 3;
                NPC.ai[0] = 0;

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
                        if (NPC.ai[0] >= 100 && NPC.ai[0] <= 400)
                        {
                            if (NPC.ai[0] % 100 == 0)
                            {
                                DrugRidus = 50;
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    var entitySource = NPC.GetSource_FromThis();
                                    Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(base.NPC.Center, 512f, 32f);
                                    int EggForce = Main.rand.Next(-10, 10 + 1);
                                    SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/IrradiatedNest_Egg_Shot"));
                                    Projectile.NewProjectile(entitySource, NPC.Center, new Vector2(EggForce, -5), Mod.Find<ModProjectile>("AEgg").Type, NPC.damage / 9, 0);
                                }
                            }
                        }
                        if (NPC.ai[0] == 480)
                        {

                            NPC.ai[1] = Main.rand.Next(2, 3 + 1);
                            NPC.ai[0] = 0;
                            NPC.netUpdate = true;
                        }
                        break;
                        
                    case 2:
                        NPC.ai[0]++;
                        if (NPC.ai[0] == 50)
                        {

                            SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/IrradiatedNest_Missile_Shots"));
                        }
                        if (NPC.ai[0] == 20)
                        {
                            Nukeing = true;

                        }
                        if (NPC.ai[0] >= 80 && NPC.ai[0] <= 230)
                        {
                            if (NPC.ai[0] % 3 == 0)
                            {
                                DrugRidus = 20;
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    var entitySource = NPC.GetSource_FromThis();


                                    Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(base.NPC.Center, 512f, 32f);
                                    int OffSet = Main.rand.Next(-40, 40 + 1);
                                    Vector2 NukePos;
                                    NukePos.X = NPC.Center.X + OffSet;
                                    NukePos.Y = NPC.Center.Y;

                                    Projectile.NewProjectile(entitySource, NukePos, new Vector2(0, -50), Mod.Find<ModProjectile>("ToxicNuke").Type, 26, 0);
                                }
                            }
                        }
                        if (NPC.ai[0] == 280)
                        {
              
                            Nukeing = false;
                            NPC.ai[0] = 0;
                            int Chance2 = Main.rand.Next(1, 3 + 1);
                            if (Chance2 == 1)
                            {
                                NPC.ai[1] = 3;
                            }
                            else
                            {
                                NPC.ai[1] = 1;
                            }
                            NPC.netUpdate = true;

                        }
                        break;
                    case 3:
                        NPC.ai[0]++;
                        if (NPC.ai[0] == 50)
                        {
                            SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/IrradiatedNest_Teleport"));
                        }
                        if (NPC.ai[0] >= 50 && NPC.ai[0] <= 100)
                        {
                            NPC.alpha += 30;

                        }
                        if (NPC.ai[0] == 100)
                        {
                            ToFar = false;
                            NPC.alpha = 0;
                            NPC.position.X = player.Center.X;
                            NPC.position.Y = player.Center.Y + -800;
                            NPC.netUpdate = true;
                        }
                        if (!HasHitGround && NPC.ai[0] >= 110)
                        {
                            if (NPC.collideY || NPC.collideX)
                            {
                                NPC.boss = true;
                                DrugRidus = 50;
                                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/IrradiatedNest_Land"));
                                Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(base.NPC.Center, 2048f, 256f);
                                HasHitGround = true;
                                NPC.ai[0] = 1000;
                            }
                        } 

                        if (NPC.ai[0] == 1000)
                        {
                            int Chance2 = Main.rand.Next(1, 3 + 1);
                            if (Chance2 == 1)
                            {
                                NPC.ai[1] = 2;
                            }
                            else
                            {
                                NPC.ai[1] = 1;
                            }
                            NPC.ai[0] = 0;
                            NPC.netUpdate = true;
                            HasHitGround = false;

                        }
                        break;
                    case 4:
                        NPC.ai[0]++;
                        NPC.noGravity = true;
                        if (NPC.ai[0] <= 60)
                        {
                            targetPos = player.Center;
                            Movement(targetPos, 0f, -400f, 0.18f);
                            NPC.netUpdate = true;
                        }
                        if (NPC.ai[0] >= 60)
                        {
                            NPC.velocity *= 0.90f;
                        }
                        if (NPC.ai[0] == 70)
                        {
                            DrugRidus = 50;
                            SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/IrradiatedNest_ComuicationRay"));
                            Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(base.NPC.Center, 2048f, 512f);
                            Utilities.NewProjectileBetter(NPC.Center.X, NPC.Center.Y, 0, 10, ModContent.ProjectileType<DesperationRay>(), 100, 0f, -1, 0, NPC.whoAmI);
                            NPC.dontTakeDamage = false;
                            NPC.dontCountMe = false;
                            NPC.netUpdate = true;
                        }
                        if (NPC.ai[0] >= 70)
                        {
                            DrugRidus = 30;


                        }
                        if (NPC.ai[0] == 150)
                        {

                            NPC.ai[0] = 0;
                            NPC.netUpdate = true;
                        }
                        break;
                }

            }


        }
        public override void HitEffect(NPC.HitInfo hit)
        {
            int d = 74;
            int d1 = DustID.CursedTorch;
            for (int k = 0; k < 30; k++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, d, 2.5f * hit.HitDirection, -2.5f, 0, Color.White, 0.7f);
                Dust.NewDust(NPC.position, NPC.width, NPC.height, d1, 2.5f * hit.HitDirection, -2.5f, 0, default(Color), .74f);
            }
            if (NPC.life <= 0)
            {
                Player player = Main.player[NPC.target];
                player.GetModPlayer<MyPlayer>().IrradiatedKilled = 0;
                var entitySource = NPC.GetSource_FromThis();
                NPC.NewNPC(entitySource, (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<IrradiatedNestDeath>());
                for (int i = 0; i < 20; i++)
                {
                    int num = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.CursedTorch, 0f, -2f, 0, default(Color), .8f);
                    Main.dust[num].noGravity = true;
                    Main.dust[num].position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
                    Main.dust[num].position.Y += Main.rand.Next(-50, 51) * .05f - 1.5f;
                    if (Main.dust[num].position != NPC.Center)
                        Main.dust[num].velocity = NPC.DirectionTo(Main.dust[num].position) * 6f;
                }
            }
        }
        public void Movement(Vector2 Player2, float PosX, float PosY, float Speed)
        {
            Player player = Main.player[NPC.target];
            Vector2 target = Player2 + new Vector2(PosX, PosY);
            base.NPC.velocity = Vector2.Lerp(base.NPC.velocity, VectorHelper.MovemontVelocity(base.NPC.Center, Vector2.Lerp(base.NPC.Center, target, 0.5f), base.NPC.Center.Distance(target) * Speed), 0.1f);
        }
    }
}