
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Helpers;
using Stellamod.NPCs.Bosses.singularityFragment;
using Stellamod.Trails;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Stellamod.NPCs.Bosses.SupernovaFragment
{

    public class SupernovaOrb : ModNPC
    {
        private int _frameCounter;
        private int _frameTick;
        private Vector2[] _chainPos = new Vector2[16];

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Dread Fire");
            NPCID.Sets.TrailingMode[NPC.type] = 0;
            NPCID.Sets.TrailCacheLength[NPC.type] = 28;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
        {
            Lighting.AddLight(NPC.Center, Color.Orange.ToVector3() * 1.25f * Main.essScale);
            SpriteEffects Effects = NPC.spriteDirection != -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            Texture2D texture = Request<Texture2D>(Texture).Value;

            Vector2 frameOrigin = NPC.frame.Size() /2f;
            Vector2 drawPos = NPC.Center - screenPos;

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
                    spriteBatch.Draw(texture, drawPos + new Vector2(0f, 4 / 2).RotatedBy(radians) * time, NPC.frame, new Color(49, 39, 124, 0), 0, frameOrigin, NPC.scale, Effects, 0);
                }

                for (float i = 0f; i < 1f; i += 0.34f)
                {
                    float radians = (i + timer) * MathHelper.TwoPi;
                    spriteBatch.Draw(texture, drawPos + new Vector2(0f, 6).RotatedBy(radians) * time, NPC.frame, new Color(50, 74, 255, 0), 0, frameOrigin, NPC.scale, Effects, 0);
                }
            }

            Texture2D texture2 = TextureAssets.Projectile[NPC.type].Value;
            Main.spriteBatch.Draw(texture2, NPC.Center - Main.screenPosition, null, Color.White, NPC.rotation, new Vector2(texture2.Width / 2, texture2.Height / 2), 1f, NPC.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
            TrailDrawer ??= new PrimDrawer(WidthFunction, ColorFunction, GameShaders.Misc["VampKnives:BasicTrail"]);
            GameShaders.Misc["VampKnives:BasicTrail"].SetShaderTexture(TrailRegistry.LightningTrail);
            TrailDrawer.DrawPrims(NPC.oldPos, frameOrigin - Main.screenPosition, 155);

            Texture2D chainTexture = ModContent.Request<Texture2D>("Stellamod/Projectiles/Chains/SupernovaChainCircle").Value;
            int frameCount = 15;
            int frameTime = 2;
            float alpha = NPC.alpha / 255f;
            alpha = 1f - alpha;
            Rectangle animationFrame = chainTexture.AnimationFrame(
                ref _frameCounter, ref _frameTick, frameTime, frameCount, true);
            DrawHelper.DrawSupernovaChains(chainTexture, _chainPos, animationFrame, alpha);
            return true;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }

        public PrimDrawer TrailDrawer { get; private set; } = null;
        public float WidthFunction(float completionRatio)
        {
            float baseWidth = NPC.scale * NPC.width * 0.5f;
            return MathHelper.SmoothStep(baseWidth, baseWidth * 0.1f, completionRatio);
        }

        public Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(Color.OrangeRed, Color.DeepPink, completionRatio) * 0.7f;
        }


        Vector2 Drawoffset => new Vector2(0, NPC.gfxOffY) + Vector2.UnitX * NPC.spriteDirection * 0;
        public virtual string GlowTexturePath => Texture + "_Glow";
        private Asset<Texture2D> _glowTexture;
        public Texture2D GlowTexture => (_glowTexture ??= (RequestIfExists<Texture2D>(GlowTexturePath, out var asset) ? asset : null))?.Value;
        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            Texture2D texture = TextureAssets.Npc[NPC.type].Value;
            for(int i = 0; i  < 8; i++)
            {
                spriteBatch.Draw(texture, NPC.Center - screenPos, new Microsoft.Xna.Framework.Rectangle?(NPC.frame), drawColor, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, SpriteEffects.None, 0f);
            }
          
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
        }

        public override void SetDefaults()
        {
            NPC.aiStyle = 0;
            NPC.width = 54;
            NPC.height = 54;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.damage = 60;
            NPC.alpha = 255;
            NPC.dontCountMe = false;
            NPC.dontTakeDamage = false;
            NPC.lifeMax = 2000;
            NPC.DeathSound = new SoundStyle("Stellamod/Assets/Sounds/VoidDead2") with { PitchVariance = 0.1f };
            NPC.HitSound = new SoundStyle("Stellamod/Assets/Sounds/VoidHit") with { PitchVariance = 0.1f };
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            int d = 206;
            int d1 = DustID.BlueCrystalShard;
            for (int k = 0; k < 30; k++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, d, 2.5f * hit.HitDirection, -2.5f, 0, Color.White, 0.7f);
                Dust.NewDust(NPC.position, NPC.width, NPC.height, d1, 2.5f * hit.HitDirection, -2.5f, 0, default, .74f);
            }
            if (NPC.life <= 0)
            {
                Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(base.NPC.Center, 2048f, 124f);
                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y, 0, 0,
                           ModContent.ProjectileType<SupernovaExplosionSmall>(), NPC.damage, 1, Owner: Main.myPlayer);
                SupernovaFragment.SingularityOrbs -= 1;
                for (int i = 0; i < 20; i++)
                {
                    int num = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.SomethingRed, 0f, -2f, 0, default, .8f);
                    Main.dust[num].noGravity = true;
                    Main.dust[num].position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
                    Main.dust[num].position.Y += Main.rand.Next(-50, 51) * .05f - 1.5f;
                    if (Main.dust[num].position != NPC.Center)
                        Main.dust[num].velocity = NPC.DirectionTo(Main.dust[num].position) * 6f;
                }
            }
        }
        public override bool PreAI()
        {
            NPC.TargetClosest(true);
            NPC.ai[3]++;

            int parent = (int)NPC.ai[0];
            if (parent < 0 || parent >= Main.maxNPCs || !Main.npc[parent].active || Main.npc[parent].type != NPCType<SupernovaFragment>())
            {
                NPC.active = false;
                return false;
            }

            NPC.Center = Main.npc[parent].Center + NPC.ai[2] * NPC.ai[1].ToRotationVector2();
            for(int i = 0; i < _chainPos.Length; i++)
            {
                float f = i;
                float l = _chainPos.Length;
                float p = f / l;
                _chainPos[i] = Vector2.Lerp(NPC.Center, Main.npc[parent].Center, p);
            }

            if (NPC.ai[3] <= 60)
            {
                if (NPC.alpha >= 0)
                {
                    NPC.alpha -= 5;
                }
                else
                {

                }
            }

            NPC.ai[1] += .005f;
            return false;
        }

        public override bool CheckActive()
        {
            return false;
        }
    }
}