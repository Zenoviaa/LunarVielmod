
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Utilis;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;


//By Al0n37
namespace Stellamod.NPCs.Bosses.Jack
{

    public class JackDeath : ModNPC
    {
        public float DrugRidus = 0;
        public int DrugAlpha = 0;
        private bool p2 = false;
        public override void SetStaticDefaults()
        {
            NPCID.Sets.TrailCacheLength[NPC.type] = 4;
            NPCID.Sets.TrailingMode[NPC.type] = 0;
            // DisplayName.SetDefault("Jack");
            Main.npcFrameCount[NPC.type] = 4;
        }

        public override void SetDefaults()
        {
            NPC.alpha = 255;
            NPC.width = 30;
            NPC.height = 75;
            NPC.damage = 10;
            NPC.defense = 6;         
            NPC.lifeMax = 1150;
            NPC.HitSound = SoundID.NPCHit16;
            NPC.value = 60f;
            NPC.knockBackResist = 0.0f;
            NPC.noGravity = true;
            NPC.boss = true;
            NPC.dontTakeDamage = true;
            NPC.dontCountMe = true;
            NPC.alpha = 255;
        }

        int frame = 0;
        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter += 1f;

            if (NPC.frameCounter >= 4)
            {
                frame++;
                NPC.frameCounter = 0;
            }
            if (frame >= 3)
            {
                frame = 0;
            }

            NPC.frame.Y = frameHeight * frame;
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
            Main.GraveyardVisualIntensity = 0.4f;
            if (NPC.ai[2] == 10)
            {
                if (NPC.alpha >= 0)
                {
                    NPC.alpha = 0;
                }
                NPC.ai[0]++;
                if (Main.netMode != NetmodeID.Server)
                {
                    Dust dust = Dust.NewDustDirect(NPC.Center, NPC.width, NPC.height, DustID.FlameBurst);
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
                    NPC.netUpdate = true;
                }
                if (NPC.ai[0] == 110)
                {
                    var EntitySource = NPC.GetSource_Death();
                    Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(base.NPC.Center, 2048f, 128f);
                    SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Jack_Death2"), NPC.position);
                    for (int i = 0; i < 14; i++)
                    {
                        Dust.NewDustPerfect(base.NPC.Center, DustID.Torch, (Vector2.One * Main.rand.Next(1, 12)).RotatedByRandom(19.0), 0, default(Color), 4f).noGravity = true;
                    }
                    for (int i = 0; i < 14; i++)
                    {
                        Dust.NewDustPerfect(base.NPC.Center, DustID.Hay, (Vector2.One * Main.rand.Next(1, 12)).RotatedByRandom(19.0), 0, default(Color), 4f).noGravity = false;
                    }
                    for (int i = 0; i < 14; i++)
                    {
                        Dust.NewDustPerfect(base.NPC.Center, DustID.Grass, (Vector2.One * Main.rand.Next(1, 12)).RotatedByRandom(19.0), 0, default(Color), 4f).noGravity = false;
                    }

                    for (int j = 0; j < 26; j++)
                    {
                        int a = Gore.NewGore(EntitySource, new Vector2(NPC.Center.X + Main.rand.Next(-10, 10), NPC.Center.Y + Main.rand.Next(-10, 10)), NPC.velocity, 911);
                        Main.gore[a].timeLeft = 20;
                        Main.gore[a].scale = Main.rand.NextFloat(.5f, 1f);
                    }
                    for (int i = 0; i < 40; i++)
                    {
                        Dust.NewDustPerfect(base.NPC.Center, DustID.Torch, (Vector2.One * Main.rand.Next(1, 12)).RotatedByRandom(10.0), 0, default(Color), 1f).noGravity = false;
                    }

                    int Gore2 = ModContent.Find<ModGore>("Stellamod/Jack1").Type;
                    Gore.NewGore(EntitySource, NPC.position, NPC.velocity, Gore2);
                    if (StellaMultiplayer.IsHost)
                    {
                        Utilities.NewProjectileBetter(NPC.Center.X, NPC.Center.Y, 0, 0, ModContent.ProjectileType<JackSpawnEffect>(), 50, 0f, -1, 0, NPC.whoAmI);
                    }

                    NPC.active = false;
                }
            }
        }
    }
}