using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Helpers;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Bosses.DreadMire.Heart
{
    [AutoloadBossHead]
    public class DreadMiresHeart : ModNPC
    {
        int moveSpeed = 0;
        int moveSpeedY = 0;
        float HomeY = 150f;
        int Dashes = 0;

        public bool Dir = false;
        public int AtackNum = 4;
        public bool Barf = false;
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 11;
            NPCID.Sets.TrailCacheLength[NPC.type] = 15;
            NPCID.Sets.TrailingMode[NPC.type] = 0;
            NPCID.Sets.MPAllowedEnemies[NPC.type] = true;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Events.BloodMoon,
                new FlavorTextBestiaryInfoElement(LangText.Bestiary(this, "The heart of the beast brought on by devilish intent\r\n(How the hell does that thing fit inside her chest?)")),
            });
        }

        public override void SetDefaults()
        {
            NPC.aiStyle = 0;
            NPC.noGravity = true;
            NPC.lifeMax = 800;
            NPC.defense = 10;
            NPC.damage = 60;
            NPC.value = 65f;
            NPC.knockBackResist = 0f;
            NPC.width = 60;
            NPC.height = 50;
            NPC.scale = 1f;
            NPC.lavaImmune = false;
            NPC.noTileCollide = false;
            NPC.alpha = 0;
            NPC.noTileCollide = true;
            NPC.dontTakeDamage = false;
            NPC.HitSound = SoundID.NPCDeath19;
            NPC.DeathSound = SoundID.NPCDeath23;
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;
            if (!Barf)
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
            if (Barf)
            {
                if (NPC.frameCounter >= 12)
                {
                    frame++;
                    NPC.frameCounter = 9;
                }
                if (frame >= 11)
                {
                    frame = 9;
                }
                if (frame < 9)
                {
                    frame = 9;
                }
            }

            NPC.frame.Y = frameHeight * frame;
        }

        private void Disappear()
        {
            Player obj = Main.player[NPC.target];
            NPC.velocity.Y += 0.5f;
            if (Vector2.Distance(obj.position, NPC.position) > 1000f)
            {
                NPC.active = false;
            }
        }

        public void Movement(Vector2 Player2, float PosX, float PosY, float Speed)
        {
            Player player = Main.player[NPC.target];
            Vector2 target = player.Center + new Vector2(PosX, PosY);
            NPC.velocity = Vector2.Lerp(NPC.velocity, VectorHelper.MovemontVelocity(NPC.Center, Vector2.Lerp(NPC.Center, target, 0.5f), NPC.Center.Distance(target) * Speed), 0.1f);
        }

        public int previousAttack;

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(HomeY);
            writer.Write(moveSpeed);
            writer.Write(moveSpeedY);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            HomeY = reader.ReadSingle();
            moveSpeed = reader.ReadInt32();
            moveSpeedY = reader.ReadInt32();
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
                    NPC.velocity.Y -= 1f;
                    if (NPC.timeLeft > 30)
                        NPC.timeLeft = 30;
                    return;
                }
            }
            Player playerT = Main.player[NPC.target];
            int distance = (int)(NPC.Center - playerT.Center).Length();
            if (distance > 3000f || playerT.dead || Main.dayTime)
            {
                NPC.ai[2] = 2;
                Disappear();
            }
            NPC.rotation = NPC.velocity.X * 0.05f;
            Lighting.AddLight(NPC.Center, Color.DarkRed.ToVector3() * 2.25f * Main.essScale);
            if (NPC.ai[2] == 0)
            {
                NPC.ai[0]++;
                if (NPC.ai[0] >= 1)
                {
                    var entitySource = NPC.GetSource_FromThis();
                    if (StellaMultiplayer.IsHost)
                    {
                        NPC.NewNPC(entitySource, (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<DreadMirePentagram>());
                    }

                    SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/DMHeart__Spawn"), NPC.position);
                    Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(NPC.Center, 2048f, 124f);

                    NPC.ai[0] = 0;
                    NPC.ai[2] = 1;
                }
            }
            if (NPC.ai[2] == 1)
            {
                switch (NPC.ai[1])
                {
                    case 0:
                        NPC.ai[1] = Main.rand.Next(1, 1);
                        NPC.ai[0] = 0;
                        break;
                    case 1:
                        NPC.velocity *= 1.82f;
                        if (StellaMultiplayer.IsHost)
                        {
                            if (NPC.Center.X >= player.Center.X && moveSpeed >= Main.rand.Next(-60, -40)) // flies to players x position
                            {
                                moveSpeed--;
                                NPC.netUpdate = true;
                            }

                            if (NPC.Center.X <= player.Center.X && moveSpeed <= Main.rand.Next(40, 60))
                            {
          
                                moveSpeed++;
                                NPC.netUpdate = true;
                            }

                            if (NPC.Center.Y >= player.Center.Y - HomeY && moveSpeedY >= Main.rand.Next(-40, -30)) //Flies to players Y position
                            {
                                moveSpeedY--;
                                HomeY = Main.rand.NextFloat(160f, 180f);
                                NPC.netUpdate = true;
                            }

                            if (NPC.Center.Y <= player.Center.Y - HomeY && moveSpeedY <= Main.rand.Next(30, 40))
                            {                 
                                moveSpeedY++;
                                NPC.netUpdate = true;
                            }
                        }
              

                        NPC.velocity.X = moveSpeed * 0.13f;
                        NPC.velocity.Y = moveSpeedY * 0.1f;
                        NPC.ai[0]++;
                        if (NPC.ai[0] == 60 || NPC.ai[0] == 160 || NPC.ai[0] == 220)
                        {
                            int Sound = Main.rand.Next(1, 3);
                            if (Sound == 1)
                            {
                                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/DMHeart__Shot2"), NPC.position);
                            }
                            else
                            {
                                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/DMHeart__Shot1"), NPC.position);
                            }

                            float Speed = Main.rand.Next(3, 7);
                            float offsetRandom = Main.rand.Next(0, 50);
                            Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(base.NPC.Center, 2048f, 32f);

                            float spread = 45f * 0.0174f;
                            double startAngle = Math.Atan2(1, 0) - spread / 2;
                            double deltaAngle = spread / 8f;
                            double offsetAngle;
                            for (int i = 0; i < 4; i++)
                            {
                                if (StellaMultiplayer.IsHost)
                                {
                                    offsetAngle = (startAngle + deltaAngle * (i + i * i) / 2f) + 32f * i + offsetRandom;
                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y, (float)(Math.Sin(offsetAngle) * Speed), (float)(Math.Cos(offsetAngle) * Speed),
                                        ModContent.ProjectileType<DreadMiresHeartEye>(), 16, 0, Owner: Main.myPlayer);
                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y, (float)(-Math.Sin(offsetAngle) * Speed), (float)(-Math.Cos(offsetAngle) * Speed),
                                        ModContent.ProjectileType<DreadMiresHeartEye>(), 16, 0, Owner: Main.myPlayer);
                                }
                            }
                        }

                        if (NPC.ai[0] == 290)
                        {
                            if (StellaMultiplayer.IsHost)
                            {
                                NPC.ai[1] = Main.rand.Next(2, 4);
                                NPC.ai[0] = 0;
                                NPC.netUpdate = false;
                            }
                        }

                        break;

                    case 2:
                        NPC.ai[0]++;
                        if (NPC.ai[0] <= 50)
                        {
                            NPC.velocity *= 0.97f;
                        }
                        if (NPC.ai[0] >= 50)
                        {
                            NPC.velocity.Y *= 0.98f;
                            NPC.velocity.X *= 1.05f;
                        }
                        if (NPC.alpha >= 0)
                        {
                            NPC.alpha -= 1;
                        }
  
                        if (NPC.ai[0] <= 1)
                        {
                            SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/DMHeart__PreDash"), NPC.position);
                        }
                        if (NPC.ai[0] == 50)
                        {
                            if (StellaMultiplayer.IsHost)
                            {
                                var entitySource = NPC.GetSource_FromThis();
                                NPC.NewNPC(entitySource, (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<DreadMirePentagramSmall>());
                            }
    
                            Barf = true;
                            Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(base.NPC.Center, 2048f, 32f);
                            int Sound = Main.rand.Next(1, 3);
                            if (Sound == 1)
                            {
                                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/DMHeart__Dash"), NPC.position);
                            }
                            else
                            {
                                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/DMHeart__Dash2"), NPC.position);
                            }

                            if (StellaMultiplayer.IsHost)
                            {
                                Vector2 direction = Main.player[NPC.target].Center - NPC.Center;
                                direction.Normalize();
                                direction.X = direction.X * Main.rand.Next(25, 28);
                                direction.Y = direction.Y * Main.rand.Next(25, 28);
                                NPC.alpha = 60;
                                NPC.velocity.X = direction.X;
                                NPC.velocity.Y = direction.Y;
                                NPC.netUpdate = true;
                            }
 
                            for (int i = 0; i < 20; i++)
                            {
                                int num = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.DungeonSpirit, 0f, -2f, 0, default, .8f);
                                Main.dust[num].noGravity = true;
                                Main.dust[num].position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
                                Main.dust[num].position.Y += Main.rand.Next(-50, 51) * .05f - 1.5f;
                                if (Main.dust[num].position != NPC.Center)
                                    Main.dust[num].velocity = NPC.DirectionTo(Main.dust[num].position) * 6f;
                            }

                        }
                        if (NPC.ai[0] == 100)
                        {
                            Barf = false;
                            NPC.ai[0] = 0;
                            Dashes += 1;
                            if (Dashes == 3)
                            {
                                NPC.velocity *= 0.97f;
                                Dashes = 0;
                                NPC.ai[1] = 1;
                                NPC.ai[0] = 0;
                            }
                        }
                        break;
                    case 3:
                        if (NPC.ai[0] <= 98)
                        {
                            NPC.velocity.Y *= 0.98f;
                            Dust dust = Dust.NewDustDirect(NPC.Center, NPC.width, NPC.height, DustID.CopperCoin);
                            dust.velocity *= -1f;
                            dust.scale *= .8f;
                            dust.noGravity = true;

                            Vector2 vector2_1 = new Vector2(Main.rand.Next(-180, 181), Main.rand.Next(-180, 181));
                            vector2_1.Normalize();
                            Vector2 vector2_2 = vector2_1 * (Main.rand.Next(50, 200) * 0.04f);
                            dust.velocity = vector2_2;
                            vector2_2.Normalize();
                            Vector2 vector2_3 = vector2_2 * 34f;
                            dust.position = NPC.Center - vector2_3;
                        }
                        NPC.ai[0]++;
                        if (NPC.ai[0] <= 50)
                        {
                            NPC.velocity *= 0.97f;
                        }
                        if (NPC.ai[0] > 10 && NPC.ai[0] < 30)
                        {
                            NPC.alpha -= 2;
                        }
                        if (NPC.ai[0] > 30 && NPC.ai[0] < 60)
                        {
                            NPC.alpha += 2;
                        }
                        if (NPC.ai[0] == 100)
                        {
                            var entitySource = NPC.GetSource_FromThis();
                            if (StellaMultiplayer.IsHost)
                            {
                                NPC.NewNPC(entitySource, (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<DreadMirePentagramSmall>());
                            }
                
                            SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/DMHeart__Open"), NPC.position);
                            Barf = true;
                        }
                        if (NPC.ai[0] >= 100 && NPC.alpha >= 0)
                        {
                            NPC.alpha -= 2;
                        }
                        if (NPC.ai[0] >= 120 && NPC.ai[0] <= 180)
                        {
                            if (Main.rand.NextBool(3) && StellaMultiplayer.IsHost)
                            {
                                NPC.alpha = 20;
                                int Sound = Main.rand.Next(1, 4);
                                if (Sound == 1)
                                {
                                    SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/DMHeart__Vomit2"), NPC.position);
                                }
                                if (Sound == 2)
                                {
                                    SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/DMHeart__Vomit3"), NPC.position);
                                }
                                if (Sound == 3)
                                {
                                    SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/DMHeart__Vomit1"), NPC.position);
                                }

                                SoundEngine.PlaySound(SoundID.Item34, NPC.Center);
                                Vector2 direction = Vector2.Normalize(Main.player[NPC.target].Center - NPC.Center) * 8.5f;
                                int amountOfProjectiles = Main.rand.Next(0, 2);
                                for (int i = 0; i < amountOfProjectiles; ++i)
                                {
                                    float offsetX = Main.rand.Next(-200, 200) * 0.01f;
                                    float offsetY = Main.rand.Next(-200, 200) * 0.01f;
                                    int damage = Main.expertMode ? 14 : 20;
                                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y, direction.X + offsetX, direction.Y + offsetY, 
                                        ModContent.ProjectileType<DreadMiresHeartVomit1>(), damage, 1, Owner: Main.myPlayer);
                                }
                            }
                        }
                        if (NPC.ai[0] == 200)
                        {
                            NPC.alpha = 0;
                            Barf = false;
                            NPC.ai[1] = 1;
                            NPC.ai[0] = 0;
                        }
                        break;

                }
            }

        }


        int frame = 0;
        Vector2 Drawoffset => new Vector2(0, NPC.gfxOffY) + Vector2.UnitX * NPC.spriteDirection * 0;
        public virtual string GlowTexturePath => Texture + "_Glow";
        private Asset<Texture2D> _glowTexture;
        public Texture2D GlowTexture => (_glowTexture ??= (ModContent.RequestIfExists<Texture2D>(GlowTexturePath, out var asset) ? asset : null))?.Value;
        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
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
            Vector2 vector33 = new Vector2(NPC.Center.X, NPC.Center.Y) - Main.screenPosition + Drawoffset - NPC.velocity;
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

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0)
            {
                Player player = Main.player[NPC.target];
                if (StellaMultiplayer.IsHost)
                {
                    var entitySource = NPC.GetSource_FromThis();
                    NPC.NewNPC(entitySource, (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<DreadMirePentagramSmall>());
                }

                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, 2.5f * hit.HitDirection, -2.5f, 0, default, 1.2f);
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, 2.5f * hit.HitDirection, -2.5f, 0, default, 0.5f);
            }
            else
            {
                for (int k = 0; k < 7; k++)
                {
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, 2.5f * hit.HitDirection, -2.5f, 0, default, 1.2f);
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, 2.5f * hit.HitDirection, -2.5f, 0, default, 0.5f);
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
        {
            Lighting.AddLight(NPC.Center, Color.DarkRed.ToVector3() * 2.25f * Main.essScale);
            SpriteEffects Effects = NPC.spriteDirection != -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            int spOff = NPC.alpha / 6;
            for (float j = -(float)Math.PI; j <= (float)Math.PI / 3f; j += (float)Math.PI / 3f)
            {
                spriteBatch.Draw((Texture2D)TextureAssets.Npc[NPC.type], NPC.Center + new Vector2(0f, -2f) + new Vector2(4f + NPC.alpha * 0.25f + spOff, 0f).RotatedBy(NPC.rotation + j) - Main.screenPosition, NPC.frame, Color.FromNonPremultiplied(255 + spOff * 2, 255 + spOff * 2, 255 + spOff * 2, 100 - NPC.alpha), NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, Effects, 0f);
            }
            spriteBatch.Draw((Texture2D)TextureAssets.Npc[NPC.type], NPC.Center - Main.screenPosition, NPC.frame, NPC.GetAlpha(lightColor), NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, Effects, 0f);
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            for (int k = 0; k < NPC.oldPos.Length; k++)
            {
                Vector2 drawPos = NPC.oldPos[k] - Main.screenPosition + NPC.Size / 2 + new Vector2(0f, NPC.gfxOffY);
                Color color = NPC.GetAlpha(Color.Lerp(new Color(255, 8, 55), new Color(99, 39, 51), 1f / NPC.oldPos.Length * k) * (1f - 1f / NPC.oldPos.Length * k));
                spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, drawPos, new Microsoft.Xna.Framework.Rectangle?(NPC.frame), color, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, Effects, 0f);
            }

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            return true;
        }
    }

}