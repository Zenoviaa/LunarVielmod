
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Helpers;
using Stellamod.UI.Systems;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;


//By Al0n37
namespace Stellamod.NPCs.Event.Gintzearmy.BossGintze
{

    public class GintziaHand : ModNPC
    {
        public bool AtackCharge;
        public int JackFirerand = 0;
        public int PrevAtack;
        public float DrugRidus = 0;
        public int DrugAlpha = 0;
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 4;
            NPCID.Sets.TrailCacheLength[NPC.type] = 4;
            NPCID.Sets.TrailingMode[NPC.type] = 0;
            // DisplayName.SetDefault("Jack");
        }
        public override void SetDefaults()
        {
            NPC.aiStyle = 0;
            NPC.scale = 1.1f;
            NPC.alpha = 255;
            NPC.width = 30;
            NPC.height = 30;
            NPC.damage = 40;
            NPC.defense = 0;
            NPC.lifeMax = 60;
            NPC.HitSound = SoundID.NPCHit16;
            NPC.value = 60f;
            NPC.knockBackResist = 0.0f;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.alpha = 0;
        }

        int frame = 3;
        public override void FindFrame(int frameHeight)
        {
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
                ShakeModSystem.Shake = 8;
                var entitySource = NPC.GetSource_FromThis();
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.SilverCoin, 2.5f * hit.HitDirection, -2.5f, 0, default, 1.2f);
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.SilverCoin, 2.5f * hit.HitDirection, -2.5f, 0, default, 0.5f);
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.SilverCoin, 2.5f * hit.HitDirection, -2.5f, 0, default, 1.2f);
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.SilverCoin, 2.5f * hit.HitDirection, -2.5f, 0, default, 0.5f);
            }
            else
            {
                for (int k = 0; k < 7; k++)
                {

                    Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.SilverCoin, 2.5f * hit.HitDirection, -2.5f, 0, default, 1.2f);
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.SilverCoin, 2.5f * hit.HitDirection, -2.5f, 0, default, 0.5f);
                }
            }
        }
        Vector2 Drawoffset => new Vector2(0, NPC.gfxOffY) + Vector2.UnitX * NPC.spriteDirection * 0;
        public virtual string GlowTexturePath => Texture + "_Outline";
        private Asset<Texture2D> _glowTexture;
        public Texture2D GlowTexture => (_glowTexture ??= (RequestIfExists<Texture2D>(GlowTexturePath, out var asset) ? asset : null))?.Value;
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
        {
            Vector2 center = NPC.Center + new Vector2(0f, NPC.height * -0.1f);
            Lighting.AddLight(NPC.Center, Color.LightSkyBlue.ToVector3() * 0.25f * Main.essScale);
            // This creates a randomly rotated vector of length 1, which gets it's components multiplied by the parameters
            Vector2 direction = Main.rand.NextVector2CircularEdge(NPC.width * 0.6f, NPC.height * 0.6f);
            float distance = 0.3f + Main.rand.NextFloat() * 0.5f;
            Vector2 velocity = new Vector2(0f, -Main.rand.NextFloat() * 0.3f - 1.5f);
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;



            Vector2 frameOrigin = NPC.frame.Size();
            Vector2 offset = new Vector2(NPC.width - frameOrigin.X + -5, NPC.height - NPC.frame.Height + 2);
            Vector2 drawPos = NPC.position - screenPos + frameOrigin + offset;

            float time = Main.GlobalTimeWrappedHourly;
            float timer = Main.GlobalTimeWrappedHourly / 2f + time * 0.04f;

            time %= 4f;
            time /= 2f;

            if (time >= 1f)
            {
                time = 2f - time;
            }
            SpriteEffects Effects = NPC.spriteDirection != -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            time = time * 0.5f + 0.5f;
            if (AtackCharge)
            {
                for (float i = 0f; i < 1f; i += 0.25f)
                {
                    float radians = (i + timer) * MathHelper.TwoPi;

                    spriteBatch.Draw(texture, drawPos + new Vector2(0f, DrugRidus).RotatedBy(radians) * time, NPC.frame, new Color(100, 70, 255, 50), NPC.rotation, frameOrigin, NPC.scale, Effects, 0);
                }

                for (float i = 0f; i < 1f; i += 0.34f)
                {
                    float radians = (i + timer) * MathHelper.TwoPi;

                    spriteBatch.Draw(texture, drawPos + new Vector2(0f, DrugRidus * 2).RotatedBy(radians) * time, NPC.frame, new Color(140, 210, 255, 77), NPC.rotation, frameOrigin, NPC.scale, Effects, 0);
                }
            }






            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            var drawOrigin = new Vector2(TextureAssets.Npc[NPC.type].Width() * 0.5f, NPC.height * 0.5f);
            for (int k = 0; k < NPC.oldPos.Length; k++)
            {
                Vector2 DrawPos = NPC.oldPos[k] - Main.screenPosition + NPC.Size / 2 + new Vector2(0f, NPC.gfxOffY);
                Color color = NPC.GetAlpha(Color.Lerp(new Color(191, 165, 160), new Color(191, 59, 51), 1f / NPC.oldPos.Length * k) * (1f - 1f / NPC.oldPos.Length * k));
                spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, DrawPos, new Microsoft.Xna.Framework.Rectangle?(NPC.frame), color, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, Effects, 0f);
            }

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            return true;
        }
        Vector2 TPVector;
        public override void AI()
        {
            if (!NPC.AnyNPCs(ModContent.NPCType<CommanderGintzia>()))          
            {
                NPC.active = false;
            }
            if (AtackCharge)
            {
                DrugRidus = 4;
            }
            else
            {
                DrugRidus = 0;
            }
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

            if (NPC.ai[2] == 10)
            {
                if (NPC.alpha <= 255)
                {
                    NPC.alpha += 15;
                }
                NPC.ai[0]++;
                if (NPC.ai[0] == 50)
                {
                    NPC.ai[2] = 1;
                }
            }
          
            if (NPC.ai[2] == 1)
            {
                switch (NPC.ai[1])
                {
                    case 0:
                        NPC.noTileCollide = true;
                        NPC.alpha -= 50;
                        NPC.ai[0]++;
                        if (NPC.ai[0] > 20)
                        {
                            NPC.ai[0] = 0;
                            NPC.ai[1] = 1;
                        }
                        break;
                    case 1:
                        NPC.noTileCollide = true;
                        NPC.ai[0]++;
                        if (NPC.ai[0] >= 2)
                        {
                            NPC.ai[0] = 0;
                            NPC.ai[1] = Main.rand.Next(2, 5);
                            NPC.netUpdate = true;
                        }

                        break;
                    case 2:
                        NPC.noTileCollide = true;
                        frame = 2;
                        NPC.ai[0]++;
                        if (NPC.ai[0] <= 2)
                        {
                            if (NPC.position.X >= player.position.X)
                            {
                                TPVector.Y = player.Center.Y - 15;
                                TPVector.X = player.Center.X - 200;
                                NPC.position = TPVector;
                            }
                            else
                            {
                                TPVector.Y = player.Center.Y - 15;
                                TPVector.X = player.Center.X + 200;
                                NPC.position = TPVector;
                            }


                        }
                        if(NPC.ai[0] == 50)
                        {
                            AtackCharge = true;
                        }
                        if (NPC.ai[0] == 70)
                        {
                            if (NPC.position.X <= player.position.X)
                            {

                                NPC.velocity.X = 20;
                            }
                            else
                            {
                                NPC.velocity.X = -20;
                            }
                        }
                        if (NPC.ai[0] >= 90)
                        {
                            AtackCharge = false;
                            if (NPC.alpha <= 255)
                            {
                                NPC.alpha += 15;
                            }
                        }
                        if (NPC.ai[0] <= 100)
                        {
                            if (NPC.alpha >= 0)
                            {
                                NPC.alpha -= 15;
                            }


                        }
                        if (NPC.ai[0] == 140)
                        {
                            NPC.ai[1 ] = 1;
                        }

                        break;
                    case 3:
                        frame = 1;
                        NPC.noTileCollide = false;
                        NPC.ai[0]++;
                        if (NPC.ai[0] <= 2)
                        {
                            TPVector.Y = player.Center.Y - 200;
                            TPVector.X = player.Center.X;
                            NPC.position = TPVector;


                        }
                        if (NPC.ai[0] == 50)
                        {
                            AtackCharge = true;
                        }
                        if (NPC.ai[0] == 70)
                        {
                            NPC.velocity.Y = +20;
                        }
                        if (NPC.ai[0] >= 90)
                        {
                            AtackCharge = false;
                            if (NPC.alpha <= 255)
                            {
                                NPC.alpha += 15;
                            }
                        }
                        if (NPC.ai[0] <= 100)
                        {
                            if (NPC.alpha >= 0)
                            {
                                NPC.alpha -= 15;
                            }


                        }
                        if (NPC.ai[0] == 140)
                        {
                            NPC.ai[1] = 1;
                        }
                        break;
                    case 4:
                        frame = 0;
                        NPC.ai[0]++;
                        if (NPC.ai[0] <= 2)
                        {
                            if (NPC.position.X >= player.position.X)
                            {
                                TPVector.Y = player.Center.Y - 225;
                                TPVector.X = player.Center.X - 200;
                                NPC.position = TPVector;
                            }
                            else
                            {
                                TPVector.Y = player.Center.Y - 225;
                                TPVector.X = player.Center.X + 200;
                                NPC.position = TPVector;
                            }
                        }
                        if (NPC.ai[0] == 50)
                        {
                            AtackCharge = true;
                        }
                        if (NPC.ai[0] == 70)
                        {
                            Vector2 direction = Vector2.Normalize(Main.player[NPC.target].Center - NPC.Center) * 8.5f;

                            SoundEngine.PlaySound(SoundID.Item8, NPC.position);
                            SoundEngine.PlaySound(SoundID.Zombie53, NPC.position);
                            float offsetX = Main.rand.Next(-50, 50) * 0.01f;
                            float offsetY = Main.rand.Next(-50, 50) * 0.01f;
                            int damage = Main.expertMode ? 6 : 10;
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y, direction.X + offsetX, direction.Y + offsetY, ModContent.ProjectileType<Gintzianado>(), damage, 1, Main.myPlayer, 0, 0);
                        }
                        if (NPC.ai[0] >= 90)
                        {
                            AtackCharge = false;
                            if (NPC.alpha <= 255)
                            {
                                NPC.alpha += 15;
                            }
                        }
                        if (NPC.ai[0] <= 100)
                        {
                            if (NPC.alpha >= 0)
                            {
                                NPC.alpha -= 15;
                            }


                        }
                        if (NPC.ai[0] == 140)
                        {
                            NPC.ai[1] = 1;
                        }
                        break;
                }
            }
        }
    }
}