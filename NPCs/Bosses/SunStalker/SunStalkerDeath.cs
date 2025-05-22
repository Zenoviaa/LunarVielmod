using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Bosses.SunStalker
{
    public class SunStalkerDeath : ModNPC
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Sun Stalker");
            NPCID.Sets.TrailCacheLength[NPC.type] = 15;
            NPCID.Sets.TrailingMode[NPC.type] = 0;
            Main.npcFrameCount[NPC.type] = 6;
        }
        //bool Dir;
        bool Dashing;
        //int moveSpeed = 0;
        //float DashSpeed = 9;
        //int moveSpeedY = 0;
        int Attack = 0;
        //float HomeY = 330f;
        bool Glow;
        public void Movement(Vector2 Player2, float PosX, float PosY, float Speed)
        {
            Player player = Main.player[NPC.target];
            Vector2 target = player.Center + new Vector2(PosX, PosY);
            base.NPC.velocity = Vector2.Lerp(base.NPC.velocity, VectorHelper.MovemontVelocity(base.NPC.Center, Vector2.Lerp(base.NPC.Center, target, 0.5f), base.NPC.Center.Distance(target) * Speed), 0.1f);
        }
        //bool IN;
        public override void SetDefaults()
        {
            NPC.width = 100;
            NPC.height = 116;
            NPC.damage = 28;
            NPC.defense = 14;
            NPC.lifeMax = 1600;
            NPC.HitSound = SoundID.NPCHit28;
            NPC.DeathSound = SoundID.NPCDeath42;
            NPC.value = 30f;
            NPC.buffImmune[BuffID.OnFire] = true;
            NPC.alpha = 0;
     
            NPC.knockBackResist = 0f;
            NPC.noTileCollide = true;
            NPC.noGravity = true;
            NPC.scale = 1f;
        }
        Vector2 targetPos;
        public override void OnKill()
        {
            Item.NewItem(NPC.GetSource_Death(), NPC.getRect(), ItemID.SlimeCrown, 1, false, 0, false, false);
            Item.NewItem(NPC.GetSource_Death(), NPC.getRect(), ItemID.Gel, Main.rand.Next(0, 2));
        }
        public override void AI()
        {
            NPC.dontTakeDamage = true;
            NPC.dontCountMe = true;
            if (Attack == 0)
            {

                NPC.spriteDirection = -NPC.direction;
                NPC.ai[0]++;
                if (Main.netMode != NetmodeID.Server)
                {
                    Dust dust = Dust.NewDustDirect(NPC.Center, NPC.width, NPC.height, DustID.CopperCoin);
                    dust.velocity /= 1f;
                    dust.scale *= .8f;
                    dust.noGravity = true;
                    Vector2 vector2_1 = new Vector2(Main.rand.Next(-80, 81), Main.rand.Next(-80, 81));
                    vector2_1.Normalize();
                    Vector2 vector2_2 = vector2_1 * (Main.rand.Next(50, 100) * 0.04f);
                    dust.velocity = vector2_2;
                    vector2_2.Normalize();
                    Vector2 vector2_3 = vector2_2 * 34f;
                    dust.position = NPC.Center - vector2_3;
                    NPC.netUpdate = true;
                }
                if (NPC.ai[0] == 2)
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
                    SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/SunStalker_Death_1"), NPC.position);
                    Glow = true;
                    Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(base.NPC.Center, 2048f, 124f);
                }
                if (NPC.ai[0] == 80)
                {
                    SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/SunStalker_Death_2"), NPC.position);
                }
                if (NPC.ai[0] == 60)
                {
                    Dashing = true;
                }
                if (NPC.ai[0] >= 60)
                {
                    NPC.alpha++;
                    if (Main.rand.NextBool(6))
                    {
                        var entitySource = NPC.GetSource_FromThis();
                        NPC.NewNPC(entitySource, (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<SunStalkerRayLight>());
                    }
                    Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(base.NPC.Center, 1024f, 12f);
                }
                if (NPC.ai[0] == 120)
                {
                    var EntitySource = NPC.GetSource_Death();
                    for (int i = 0; i < 14; i++)
                    {
                        Dust.NewDustPerfect(base.NPC.Center, DustID.GoldCoin, (Vector2.One * Main.rand.Next(1, 12)).RotatedByRandom(19.0), 0, default(Color), 4f).noGravity = true;
                    }
                    for (int i = 0; i < 200; i++)
                    {
                        Dust.NewDustPerfect(base.NPC.Center, DustID.GoldCoin, (Vector2.One * Main.rand.Next(1, 12)).RotatedByRandom(10.0), 0, default(Color), 1f).noGravity = false;
                    }
                    for (int i = 0; i < 40; i++)
                    {
                        Dust.NewDustPerfect(base.NPC.Center, DustID.CopperCoin, (Vector2.One * Main.rand.Next(4, 30)).RotatedByRandom(25.0), 0, default(Color), 6f).noGravity = true;
                    }
                    for (int i = 0; i < 20; i++)
                    {
                        Dust.NewDustPerfect(base.NPC.Center, DustID.CopperCoin, (Vector2.One * Main.rand.Next(4, 30)).RotatedByRandom(25.0), 0, default(Color), 2f).noGravity = false;
                    }
                    int Gore1 = ModContent.Find<ModGore>("Stellamod/SunStalker1").Type;
                    int Gore2 = ModContent.Find<ModGore>("Stellamod/SunStalker2").Type;
                    int Gore3 = ModContent.Find<ModGore>("Stellamod/SunStalker3").Type;
                    int Gore4 = ModContent.Find<ModGore>("Stellamod/SunStalker4").Type;
                    int Gore5 = ModContent.Find<ModGore>("Stellamod/SunStalker5").Type;
                    int Gore6 = ModContent.Find<ModGore>("Stellamod/SunStalker6").Type;
                    Gore.NewGore(EntitySource, NPC.position, NPC.velocity, Gore1);
                    Gore.NewGore(EntitySource, NPC.position, NPC.velocity, Gore2);
                    Gore.NewGore(EntitySource, NPC.position, NPC.velocity, Gore3);
                    Gore.NewGore(EntitySource, NPC.position, NPC.velocity, Gore4);
                    Gore.NewGore(EntitySource, NPC.position, NPC.velocity, Gore5);
                    Gore.NewGore(EntitySource, NPC.position, NPC.velocity, Gore6);
                    Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(base.NPC.Center, 2048f, 124f);
                    SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/SunStalker_Charge_Full_Note"), NPC.position);
                    NPC.life = 0;
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
                }

            }

            Player player = Main.player[NPC.target];
            targetPos = player.Center;
        }
        int frame = 0;
        public override void FindFrame(int frameHeight)
        {


            bool expertMode = Main.expertMode;
            Player player = Main.player[NPC.target];
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
                if (NPC.frameCounter >= 5)
                {
                    frame++;
                    NPC.frameCounter = 2;
                }
                if (frame >= 4)
                {
                    frame = 2;
                }
            }
            NPC.frame.Y = frameHeight * frame;

        }


        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
        {
            int spOff = NPC.alpha / 6;
            SpriteEffects Effects = ((base.NPC.spriteDirection != -1) ? SpriteEffects.FlipHorizontally : SpriteEffects.None);
            for (float j = -(float)Math.PI; j <= (float)Math.PI / 2f; j += (float)Math.PI / 2f)
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
        }
    }
}