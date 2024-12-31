
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Helpers;
using System;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;


//By Al0n37
namespace Stellamod.NPCs.Bosses.DaedusRework
{

    public class SolarPortal : ModNPC
    {
        public int PrevAtack;
        int moveSpeed = 0;
        int moveSpeedY = 0;
        float DaedusDrug = 8;
        float HomeY = 330f;
        bool Attack;
        bool Flying;
        public override void SetStaticDefaults()
        {
            NPCID.Sets.TrailCacheLength[NPC.type] = 4;
            NPCID.Sets.TrailingMode[NPC.type] = 0;
            // DisplayName.SetDefault("Jack");
        }

        public override void SetDefaults()
        {
            NPC.scale = 0;
            NPC.alpha = 0;
            NPC.width = 50;
            NPC.height = 60;
            NPC.damage = 10;
            NPC.defense = 0;
            NPC.lifeMax = 650;
            NPC.HitSound = SoundID.NPCHit16;
            NPC.value = 60f;
            NPC.knockBackResist = 0.0f;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.dontCountMe = true;
            NPC.dontTakeDamage = true;
            NPC.boss = true;
            NPC.npcSlots = 10f;

            Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/Daedus");
        }

        Vector2 Drawoffset => new Vector2(0, NPC.gfxOffY) + Vector2.UnitX * NPC.spriteDirection * 0;
        public virtual string GlowTexturePath => Texture + "_Glow";
        private Asset<Texture2D> _glowTexture;
        public Texture2D GlowTexture => (_glowTexture ??= (RequestIfExists<Texture2D>(GlowTexturePath, out var asset) ? asset : null))?.Value;
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
            for (int num103 = 0; num103 < 1; num103++)
            {
                Color color28 = color29;
                color28 = NPC.GetAlpha(color28);
                color28 *= 1f - num107;
                Vector2 vector29 = NPC.Center + (num103 / (float)num108 * 6.28318548f + NPC.rotation + num106).ToRotationVector2() * (4f * num107 + 2f) - Main.screenPosition + Drawoffset - NPC.velocity * num103;
                Main.spriteBatch.Draw(GlowTexture, vector29, NPC.frame, color28, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, spriteEffects3, 0f);
            }
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Graveyard,
                new FlavorTextBestiaryInfoElement(LangText.Bestiary(this, "A scarecrow reanimated by the passion of wandering flames")),
            });
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

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
        {
           // Vector2 center = NPC.Center + new Vector2(0f, NPC.height * -0.1f);
            Lighting.AddLight(NPC.Center, Color.LightBlue.ToVector3() * 1.25f * Main.essScale);
            return true;
        }

        public Vector2 DaedusPosAdd;
        public override void AI()
        {
            Player player = Main.player[NPC.target];
            if (Flying)
            {
                if (NPC.Center.X >= DaedusPosAdd.X && moveSpeed >= -120) // flies to players x position
                    moveSpeed--;
                else if (NPC.Center.X <= DaedusPosAdd.X && moveSpeed <= 120)
                    moveSpeed++;

                NPC.velocity.X = moveSpeed * 0.10f;

                if (NPC.Center.Y >= DaedusPosAdd.Y - HomeY && moveSpeedY >= -20) //Flies to players Y position
                {
                    moveSpeedY--;
                    HomeY = 0f;
                }
                else if (NPC.Center.Y <= DaedusPosAdd.Y - HomeY && moveSpeedY <= 20)
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
           
            if (NPC.ai[2] == 1)
            {
                switch (NPC.ai[1])
                {
                    case 0:
 
                        NPC.ai[0]++;
                        if (NPC.ai[0] > 20)
                        {
                            NPC.rotation = NPC.DirectionTo(player.Center).ToRotation() - MathHelper.PiOver2;
                            NPC.ai[0] = 0;
                            NPC.ai[1] = 1;
                        }
                        break;
                    case 1:
                        NPC.ai[0]++;
                        if (NPC.ai[0] >= 100)
                        {
                            NPC.ai[1] = 2;
                        }

                        break;
                    case 2:
                        NPC.ai[0]++;
                        var entitySource = NPC.GetSource_FromThis();

                        if (NPC.ai[0] == 230 - 50)
                        {
                            NPC.rotation = NPC.DirectionTo(player.Center).ToRotation() - MathHelper.PiOver2;
                            Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(NPC.Center, 1212f, 62f);

                            if (StellaMultiplayer.IsHost)
                            {
                                Projectile.NewProjectile(entitySource, NPC.Center, new Vector2(0, 0), Mod.Find<ModProjectile>("JackSpawnEffect").Type, NPC.damage / 9, 0);
                                float speedX = NPC.velocity.X * Main.rand.NextFloat(.3f, .3f) + Main.rand.NextFloat(4f, 4f);
                                float speedY = NPC.velocity.Y * Main.rand.Next(-1, -1) * 0.0f + Main.rand.Next(-4, -4) * 0f;
                                Projectile.NewProjectile(NPC.GetSource_FromThis(),
                                    NPC.position.X + speedX + 10,
                                    NPC.position.Y, speedX * 0, speedY - 2 * 2, ModContent.ProjectileType<LanturnSpear>(), 12, 0f, Owner: Main.myPlayer);
                            }
                               
                            DaedusDrug = 10;
                        }

                        if (NPC.ai[0] == 250 - 50)
                        {
                            NPC.rotation = NPC.DirectionTo(player.Center).ToRotation() - MathHelper.PiOver2;
                            Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(NPC.Center, 1212f, 62f);

                            if (StellaMultiplayer.IsHost)
                            {
                                Projectile.NewProjectile(entitySource, NPC.Center, new Vector2(0, 0), Mod.Find<ModProjectile>("JackSpawnEffect").Type, NPC.damage / 9, 0);
                                float speedX = NPC.velocity.X * Main.rand.NextFloat(.3f, .3f) + Main.rand.NextFloat(4f, 4f);
                                float speedY = NPC.velocity.Y * Main.rand.Next(-1, -1) * 0.0f + Main.rand.Next(-4, -4) * 0f;
                                Projectile.NewProjectile(NPC.GetSource_FromThis(),
                                    NPC.position.X + speedX + 10,
                                    NPC.position.Y, speedX * 0, speedY - 2 * 2, ModContent.ProjectileType<LanturnSpear>(), 12, 0f, Owner: Main.myPlayer);
                            }
                            DaedusDrug = 10;
                        }

                        if (NPC.ai[0] == 270 - 50)
                        {
                            NPC.rotation = NPC.DirectionTo(player.Center).ToRotation() - MathHelper.PiOver2;
                            Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(NPC.Center, 1212f, 62f);
                            if (StellaMultiplayer.IsHost)
                            {
                                Projectile.NewProjectile(entitySource, NPC.Center, new Vector2(0, 0), Mod.Find<ModProjectile>("JackSpawnEffect").Type, NPC.damage / 9, 0);
                                float speedX = NPC.velocity.X * Main.rand.NextFloat(.3f, .3f) + Main.rand.NextFloat(4f, 4f);
                                float speedY = NPC.velocity.Y * Main.rand.Next(-1, -1) * 0.0f + Main.rand.Next(-4, -4) * 0f;
                                Projectile.NewProjectile(NPC.GetSource_FromThis(),
                                    NPC.position.X + speedX + 10,
                                    NPC.position.Y, speedX * 0, speedY - 2 * 2, ModContent.ProjectileType<LanturnSpear>(), 12, 0f, Owner: Main.myPlayer);
                            }
                            DaedusDrug = 10;
                        }
                        if (NPC.ai[0] == 290 - 50)
                        {
                            NPC.active = false;
                            NPC.rotation = NPC.DirectionTo(player.Center).ToRotation() - MathHelper.PiOver2;
                            Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(NPC.Center, 1212f, 62f);
                            if (StellaMultiplayer.IsHost)
                            {
                                Projectile.NewProjectile(entitySource, NPC.Center, new Vector2(0, 0), Mod.Find<ModProjectile>("JackSpawnEffect").Type, NPC.damage / 9, 0);
                                float speedX = NPC.velocity.X * Main.rand.NextFloat(.3f, .3f) + Main.rand.NextFloat(4f, 4f);
                                float speedY = NPC.velocity.Y * Main.rand.Next(-1, -1) * 0.0f + Main.rand.Next(-4, -4) * 0f;
                                Projectile.NewProjectile(NPC.GetSource_FromThis(), 
                                    NPC.position.X + speedX + 10, 
                                    NPC.position.Y, speedX * 0, speedY - 2 * 2, ModContent.ProjectileType<LanturnSpear>(), 12, 0f, Owner: Main.myPlayer);
                            }

                            DaedusDrug = 10;
                        }          
                      
                        if (NPC.scale <= 1)
                        {
                            NPC.rotation = NPC.DirectionTo(player.Center).ToRotation() - MathHelper.PiOver2;
                            NPC.scale += 0.04f;
                        }

                        break;
                }
            }
        }
    }
}