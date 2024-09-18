using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.NPCs.Bosses.DreadMire.Heart;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Bosses.DreadMire
{

    public class DreadFireCircle : ModNPC
    {
        private float CircleTimer;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Dread Fire");
            NPCID.Sets.TrailingMode[NPC.type] = 0;
            NPCID.Sets.TrailCacheLength[NPC.type] = 5;
            Main.npcFrameCount[NPC.type] = 4;
        }

        float Size = 0;
        int frame = 0;
        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter += 0.5f;
            if (NPC.frameCounter >= 5)
            {
                frame++;
                NPC.frameCounter = 0;
            }
            if (frame >= 4)
            {
                frame = 0;
            }
            NPC.frame.Y = frameHeight * frame;
        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,
                new FlavorTextBestiaryInfoElement(LangText.Bestiary(this, "A poisonous slime mutated from normal green slimes")),
            });
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
        {
            SpriteEffects Effects = NPC.spriteDirection != -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);



            Vector2 center = NPC.Center + new Vector2(0f, NPC.height * -0.1f);
            // This creates a randomly rotated vector of length 1, which gets it's components multiplied by the parameters
            Vector2 direction = Main.rand.NextVector2CircularEdge(NPC.width * 0.6f, NPC.height * 0.6f);
            float distance = 0.3f + Main.rand.NextFloat() * 0.5f;
            Vector2 velocity = new Vector2(0f, -Main.rand.NextFloat() * 0.3f - 1.5f);
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;



            Vector2 frameOrigin = NPC.frame.Size();
            Vector2 offset = new Vector2(NPC.width - frameOrigin.X - 15, NPC.height - NPC.frame.Height + 8);
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

            if (NPC.alpha == 0)
            {
                for (float i = 0f; i < 1f; i += 0.25f)
                {
                    float radians = (i + timer) * MathHelper.TwoPi;

                    spriteBatch.Draw(texture, DrawPos + new Vector2(0f, Size / 2).RotatedBy(radians) * time, NPC.frame, new Color(99, 39, 51, 0), 0, frameOrigin, NPC.scale, Effects, 0);
                }

                for (float i = 0f; i < 1f; i += 0.34f)
                {
                    float radians = (i + timer) * MathHelper.TwoPi;

                    spriteBatch.Draw(texture, DrawPos + new Vector2(0f, Size).RotatedBy(radians) * time, NPC.frame, new Color(255, 8, 55, 0), 0, frameOrigin, NPC.scale, Effects, 0);
                }
            }


            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            var effects = NPC.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            for (int k = 0; k < NPC.oldPos.Length; k++)
            {
                Vector2 drawPos = NPC.oldPos[k] - Main.screenPosition + NPC.Size / 2 + new Vector2(0f, NPC.gfxOffY);
                Color color = NPC.GetAlpha(Color.Lerp(new Color(255, 8, 55), new Color(99, 39, 51), 1f / NPC.oldPos.Length * k) * (1f - 1f / NPC.oldPos.Length * k));
                spriteBatch.Draw((Texture2D)TextureAssets.Npc[NPC.type], drawPos, new Microsoft.Xna.Framework.Rectangle?(NPC.frame), color, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0f);
            }
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            return true;

        }
        public override void SetDefaults()
        {
            NPC.width = 42;
            NPC.height = 42;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.damage = 15;
            NPC.alpha = 255;
            NPC.dontCountMe = true;
            NPC.dontTakeDamage = true;
            NPC.lifeMax = 2700;
            NPC.aiStyle = -1;
        }

        public override bool PreAI()
        {
            bool expertMode = Main.expertMode;
            if (Main.rand.NextBool(29))
            {
                int dust = Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, ModContent.DustType<RuneDust>(), NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].scale = 1.5f;
            }

            if (Main.rand.NextBool(29))
            {
                int dust3 = Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, ModContent.DustType<RuneDust>(), NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f);
                Main.dust[dust3].noGravity = true;
                Main.dust[dust3].scale = 1.5f;
            }
            if (Main.rand.NextBool(45))
            {
                NPC.netUpdate = false;
            }
            NPC.TargetClosest(true);
            Vector2 direction = Main.player[NPC.target].Center - NPC.Center;
            direction.Normalize();
            direction *= 9f;
            int parent = (int)NPC.ai[0];
            if (parent < 0 || parent >= Main.maxNPCs || !Main.npc[parent].active || Main.npc[parent].type != ModContent.NPCType<DreadMireR>())
            {
                NPC.netUpdate = true;
                NPC.active = false;
                return false;
            }
            if (NPC.AnyNPCs(ModContent.NPCType<DreadMiresHeart>()))
            {
                NPC.netUpdate = true;
                int Sound = Main.rand.Next(1, 3);
                if (Sound == 1)
                {
                    SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Dreadmire_BoneSpawn1"), NPC.position);
                }
                else
                {
                    SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Dreadmire_BoneSpawn2"), NPC.position);
                }
                for (int i = 0; i < 14; i++)
                {
                    Dust.NewDustPerfect(base.NPC.Center, DustID.RedTorch, (Vector2.One * Main.rand.Next(1, 12)).RotatedByRandom(19.0), 0, default(Color), 4f).noGravity = true;
                }
                for (int i = 0; i < 40; i++)
                {
                    Dust.NewDustPerfect(base.NPC.Center, DustID.RedTorch, (Vector2.One * Main.rand.Next(1, 12)).RotatedByRandom(10.0), 0, default(Color), 1f).noGravity = false;
                }
                SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode, NPC.position);
                SoundEngine.PlaySound(SoundID.DD2_BetsysWrathImpact, NPC.position);
                Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(base.NPC.Center, 2048f, 64f);
                var entitySource = NPC.GetSource_FromThis();
                NPC.NewNPC(entitySource, (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<DreadFireBomb>());
                NPC.active = false;
                NPC.active = false;
                return false;
            }

            NPC.Center = Main.npc[parent].Center + NPC.ai[2] * NPC.ai[1].ToRotationVector2();
            NPC.ai[3]++;



            if (NPC.ai[3] <= 60)
            {
                if(NPC.alpha >= 0)
                {
                    NPC.alpha -= 5;
                }
                else
                {
                    Size += 0.6f;
                }
                
            }
            if (NPC.ai[3] >= 250)
            {
                NPC.netUpdate = true;
                int Sound = Main.rand.Next(1, 3);
                if (Sound == 1)
                {
                    SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Dreadmire_BoneSpawn1"), NPC.position);
                }
                else
                {
                    SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Dreadmire_BoneSpawn2"), NPC.position);
                }
                for (int i = 0; i < 14; i++)
                {
                    Dust.NewDustPerfect(base.NPC.Center, DustID.RedTorch, (Vector2.One * Main.rand.Next(1, 12)).RotatedByRandom(19.0), 0, default(Color), 4f).noGravity = true;
                }
                for (int i = 0; i < 40; i++)
                {
                    Dust.NewDustPerfect(base.NPC.Center, DustID.RedTorch, (Vector2.One * Main.rand.Next(1, 12)).RotatedByRandom(10.0), 0, default(Color), 1f).noGravity = false;
                }
                SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode, NPC.position);
                SoundEngine.PlaySound(SoundID.DD2_BetsysWrathImpact, NPC.position);
                Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(base.NPC.Center, 2048f, 64f);
                var entitySource = NPC.GetSource_FromThis();
                NPC.NewNPC(entitySource, (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<DreadFireBomb>());
                NPC.active = false;
            }
            NPC.ai[1] += .03f;

            CircleTimer++;
            if(CircleTimer % 125 == 0 && StellaMultiplayer.IsHost)
            {
                Vector2 directionToTarget = NPC.Center.DirectionTo(Main.player[NPC.target].Center);
                Vector2 velocity = directionToTarget * 8;
                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, velocity,
                    ModContent.ProjectileType<DreadFireHand>(), 28, 1, Main.myPlayer);
            }
            return false;
        }


        public override bool CheckActive()
        {
            return false;
        }
    }
}