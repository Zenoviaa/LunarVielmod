
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Helpers;
using Stellamod.Items.Consumables;
using Stellamod.Items.Harvesting;
using Stellamod.Items.Materials;
using Stellamod.Items.Weapons.Mage;
using Stellamod.Items.Weapons.Melee;
using Stellamod.Items.Weapons.Ranged;
using Stellamod.NPCs.Bosses.DreadMire.Heart;
using Stellamod.NPCs.Bosses.Jack;
using Stellamod.Utilis;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;


//By Al0n37
namespace Stellamod.NPCs.Bosses.Caeva
{

    public class Caeva : ModNPC
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
            // DisplayName.SetDefault("Jack");
            Main.npcFrameCount[NPC.type] = 8;
        }

        public override void SetDefaults()
        {
            NPC.alpha = 255;
            NPC.width = 70;
            NPC.height = 120;
            //NPC.damage = 90;
            //NPC.defense = 42;
            //NPC.lifeMax = 80000;
            NPC.damage = 0;
            NPC.defense = 0;
            NPC.lifeMax = 0;
            NPC.HitSound = SoundID.NPCHit16;
            NPC.value = 60f;
            NPC.knockBackResist = 0.0f;
            NPC.noGravity = true;
            NPC.boss = true;
            NPC.noTileCollide = true;
            NPC.alpha = 255;
            NPC.npcSlots = 10f;
            Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/Caeva");
        }

        int frame = 0;
        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter += 0.5f;

            if (NPC.frameCounter >= 4)
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
                var entitySource = NPC.GetSource_FromThis();
     
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.WaterCandle, 2.5f * hit.HitDirection, -2.5f, 0, default, 1.2f);
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.WaterCandle, 2.5f * hit.HitDirection, -2.5f, 0, default, 0.5f);
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.WaterCandle, 2.5f * hit.HitDirection, -2.5f, 0, default, 1.2f);
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.WaterCandle, 2.5f * hit.HitDirection, -2.5f, 0, default, 0.5f);
            }
            else
            {
                for (int k = 0; k < 7; k++)
                {

                    Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.WaterCandle, 2.5f * hit.HitDirection, -2.5f, 0, default, 1.2f);
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.WaterCandle, 2.5f * hit.HitDirection, -2.5f, 0, default, 0.5f);
                }
            }
        }
        Vector2 Drawoffset => new Vector2(0, NPC.gfxOffY) + Vector2.UnitX * NPC.spriteDirection * 0;
        public virtual string GlowTexturePath => Texture + "_Glow";
        private Asset<Texture2D> _glowTexture;
        public Texture2D GlowTexture => (_glowTexture ??= (ModContent.RequestIfExists<Texture2D>(GlowTexturePath, out var asset) ? asset : null))?.Value;
        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            float num108 = 4;
            float num107 = (float)Math.Cos((double)(Main.GlobalTimeWrappedHourly % 1.4f / 1.4f * 6.28318548f)) / 2f + 0.5f;
            float num106 = 0f;
            Color color1 = Color.Yellow * num107 * .8f;
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

            SpriteEffects spriteEffects3 = NPC.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            //Vector2 vector33 = new Vector2(NPC.Center.X, NPC.Center.Y) - Main.screenPosition + Drawoffset - NPC.velocity;
            Color color29 = new Color(127 - NPC.alpha, 127 - NPC.alpha, 127 - NPC.alpha, 0).MultiplyRGBA(Color.Yellow);
            for (int num103 = 0; num103 < 4; num103++)
            {
                Color color28 = color29;
                color28 = NPC.GetAlpha(color28);
                color28 *= 1f - num107;
                Vector2 vector29 = NPC.Center + (num103 / (float)num108 * 6.28318548f + NPC.rotation + num106).ToRotationVector2() * (4f * num107 + 2f) - Main.screenPosition + Drawoffset - NPC.velocity * num103;
                Main.spriteBatch.Draw(GlowTexture, vector29, NPC.frame, color28, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, spriteEffects3, 0f);
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
        {
            SpriteEffects Effects = NPC.spriteDirection != -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Lighting.AddLight(NPC.Center, Color.Blue.ToVector3() * 2.25f * Main.essScale);
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            //var drawOrigin = new Vector2(TextureAssets.Npc[NPC.type].Width() * 0.5f, NPC.height * 0.5f);
            for (int k = 0; k < NPC.oldPos.Length; k++)
            {
                Vector2 drawPos = NPC.oldPos[k] - Main.screenPosition + NPC.Size / 2 + new Vector2(0f, NPC.gfxOffY);
                Color color = NPC.GetAlpha(Color.Lerp(new Color(106, 255, 255), new Color(151, 46, 175), 1f / NPC.oldPos.Length * k) * (1f - 1f / NPC.oldPos.Length * k));
                spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, drawPos, new Microsoft.Xna.Framework.Rectangle?(NPC.frame), color, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, Effects, 0f);
            }

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            return true;
        }
        public void Movement(Vector2 Player2, float PosX, float PosY, float Speed)
        {
            Player player = Main.player[NPC.target];
            Vector2 target = player.Center + new Vector2(PosX, PosY);
            base.NPC.velocity = Vector2.Lerp(base.NPC.velocity, VectorHelper.MovemontVelocity(base.NPC.Center, Vector2.Lerp(base.NPC.Center, target, 0.5f), base.NPC.Center.Distance(target) * Speed), 0.1f);
        }
        Vector2 targetPos;
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
            targetPos = player.Center;
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
            NPC.rotation = NPC.velocity.X * 0.07f;
            p2 = NPC.life < NPC.lifeMax * 0.5f;

            if (NPC.ai[2] == 10)
            {
                if (NPC.alpha >= 0)
                {
                    NPC.alpha -= 5;
                }
                NPC.ai[0]++;

                if (NPC.ai[0] >= 0)
                {
                    NPC.ai[2] = 1;
                }
            }
            if (p2)
            {
              
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
                            NPC.ai[1] = 1;
                        }
                        break;
                    case 1:
                        NPC.ai[0]++;
                        if (NPC.ai[0] >= 2)
                        {

                            int Atack = Main.rand.Next(2, 4);
                            if (Atack == PrevAtack)
                            {
                                NPC.ai[0] = 1;
                            }
                            else
                            {
                                NPC.ai[0] = 0;
                                NPC.ai[1] = Atack;
                            }

                        }

                        break;
                    case 2:

                        NPC.ai[0]++;
                        if (NPC.position.X >= player.position.X)
                        {
                            Movement(targetPos, 400f, -200f, 0.05f);
                        }
                        else
                        {
                            Movement(targetPos, -400f, -200f, 0.05f);
                        }


                        if (NPC.ai[0] == 100)
                        {
                            NPC.alpha = 40;
                            NPC.ai[0] = 0;
                            Vector2 direction = Vector2.Normalize(Main.player[NPC.target].Center - NPC.Center) * 8.5f;

                            SoundEngine.PlaySound(SoundID.Item8, NPC.position);
                            SoundEngine.PlaySound(SoundID.Zombie53, NPC.position);
                            float offsetX = Main.rand.Next(-50, 50) * 0.01f;
                            float offsetY = Main.rand.Next(-50, 50) * 0.01f;
                            int damage = Main.expertMode ? 4 : 7;
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y, direction.X + offsetX, direction.Y + offsetY, ModContent.ProjectileType<CaevaBubble>(), damage, 1, Main.myPlayer, 0, 0);
                        }


                        if (NPC.ai[0] == 130)
                        {
                            PrevAtack = 2;
                            NPC.ai[1] = 1;
                            NPC.ai[0] = 0;
                        }
                        break;
                    case 3:

                        NPC.ai[0]++;
                        if (NPC.position.X >= player.position.X)
                        {
                            Movement(targetPos, 400f, -000f, 0.05f);
                        }
                        else
                        {
                            Movement(targetPos, -400f, -000f, 0.05f);
                        }

                        if (NPC.ai[0] == 100)
                        {
                            NPC.alpha = 40;
                            NPC.ai[0] = 0;
                            Vector2 direction = Vector2.Normalize(Main.player[NPC.target].Center - NPC.Center) * 8.5f;


                            float offsetX = Main.rand.Next(-50, 50) * 0.01f;
                            float offsetY = Main.rand.Next(-50, 50) * 0.01f;

                            var entitySource = NPC.GetSource_FromThis();
                            NPC.NewNPC(entitySource, (int)NPC.Center.X - 60, (int)NPC.Center.Y, ModContent.NPCType<CaevaDeathRow>());
;
                            NPC.NewNPC(entitySource, (int)NPC.Center.X - 60, (int)NPC.Center.Y + 300, ModContent.NPCType<CaevaDeathRowR>());

                            NPC.NewNPC(entitySource, (int)NPC.Center.X - 60, (int)NPC.Center.Y - 300, ModContent.NPCType<CaevaDeathRow>());
                        }
                        if (NPC.ai[0] == 330)
                        {
                            PrevAtack = 3;
                            NPC.ai[1] = 1;
                            NPC.ai[0] = 0;
                        }
                        break;
                }

            }


        }


    }
}