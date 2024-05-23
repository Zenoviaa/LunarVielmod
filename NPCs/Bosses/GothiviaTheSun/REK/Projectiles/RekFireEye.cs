
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Helpers;
using Stellamod.Trails;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Bosses.GothiviaTheSun.REK.Projectiles
{
    internal class RekFireEye : ModNPC
    {
        public PrimDrawer TrailDrawer { get; private set; } = null;
        private ref float Timer => ref NPC.ai[0];
        private NPC Owner
        {
            get => Main.npc[(int)NPC.ai[1]];
            set => NPC.ai[1] = value.whoAmI;
        }
        private ref float AttackTimer => ref NPC.ai[2];
        private ref float OrbitTimer => ref NPC.ai[3];

        private bool killYoSelf;
        private Vector2 LaserDirection;
        private Player Target => Main.player[NPC.target];
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 32;
            NPCID.Sets.TrailCacheLength[Type] = 16;
            NPCID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            NPC.width = 200;
            NPC.height = 200;
            NPC.lifeMax = 1000;
            NPC.damage = 200;
            NPC.defense = 70;
            NPC.dontTakeDamage = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.scale = 0f;
            NPC.boss = true;
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            return false;
        }

        public override void AI()
        {
            NPC.scale = MathHelper.Lerp(NPC.scale, 1f, 0.1f);
            NPC.TargetClosest();
            if (Owner.life <= 1)
            {
                killYoSelf = true;
            }

            if (killYoSelf || !NPC.AnyNPCs(ModContent.NPCType<RekSnake>()))
            {
                NPC.Kill();
            }

            AI_Orbit();
            AI_Attack();
        }

        private void AI_Attack()
        {
            switch (AttackTimer)
            {
                case 0:
                    Timer++;
                    if(Timer >= 360)
                    {
                        LaserDirection = NPC.Center.DirectionTo(Target.Center);
                        LaserDirection = LaserDirection.RotatedBy(MathHelper.PiOver2);
                      
                        AttackTimer++;
                        Timer = 0;
                    }
                    break;
                case 1:
                    Timer++;
                    NPC.Center = Vector2.Lerp(NPC.Center, Target.Center + new Vector2(0, -384), 0.1f);
                    if(Timer >= 60)
                    {
                        if (StellaMultiplayer.IsHost)
                        {
                            int damage = 50;
                            int knockback = 1;
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, LaserDirection,
                                ModContent.ProjectileType<RekFireEyeLaserProj>(), damage, knockback, Main.myPlayer, ai1: NPC.whoAmI);
                        }
                        AttackTimer++;
                        Timer = 0;
                    }
                    break;
                case 2:
                    Timer++;
                    if(Timer >= 90)
                    {
                        AttackTimer = 0;
                        Timer = 0;
                    }
                    break;
            }
        }

        private void AI_Orbit()
        {
            OrbitTimer++;
            float radiusProgress = MathHelper.Clamp(OrbitTimer / 60f, 0, 1);
            float easedRadiusProgress = Easing.OutCubic(radiusProgress);
            float radius = 252 * easedRadiusProgress;

            Vector2 offset = -Vector2.UnitY;
            float progress = Timer / 240f;
            if(AttackTimer <= 0)
            {
                Vector2 targetCenter = Owner.Center + offset.RotatedBy(progress * MathHelper.TwoPi) * radius;
                NPC.Center = Vector2.Lerp(NPC.Center, targetCenter, 0.05f);
            }
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter += 0.6f;
            NPC.frameCounter %= Main.npcFrameCount[NPC.type];
            int frame = (int)NPC.frameCounter;
            NPC.frame.Y = frame * frameHeight;
        }

        public float WidthFunction(float completionRatio)
        {
            return NPC.width * NPC.scale * (1f - completionRatio) * 2f;
        }

        public Color ColorFunction(float completionRatio)
        {
            return Color.Orange * (1f - completionRatio);
        }

        public Color? GetLineAlpha(Color lightColor)
        {
            return new Color(
                Color.White.R,
                Color.White.G,
                Color.White.B, 0) * (1f - NPC.alpha / 50f);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (TrailDrawer == null)
            {
                TrailDrawer = new PrimDrawer(WidthFunction, ColorFunction, GameShaders.Misc["VampKnives:BasicTrail"]);
            }

            GameShaders.Misc["VampKnives:BasicTrail"].SetShaderTexture(TrailRegistry.FadedStreak);
            Vector2 frameSize = new Vector2(200, 200);
            TrailDrawer.DrawPrims(NPC.oldPos, frameSize * 0.5f - screenPos, 155);

            //Draw Telegraph Line like on the axe
            if(AttackTimer == 1)
            {
                float progress = Timer / 60f;
                Texture2D lineTexture = ModContent.Request<Texture2D>("Stellamod/Effects/Masks/Extra_47").Value;
                Color lineDrawColor = (Color)GetLineAlpha(drawColor);
                lineDrawColor *= progress;

                Vector2 lineDrawOrigin = lineTexture.Size();
                float lineDrawScale = NPC.scale;
                float lineDrawRotation = LaserDirection.ToRotation() + MathHelper.PiOver2;
                spriteBatch.Draw(lineTexture, NPC.Center - Main.screenPosition, null,
                    lineDrawColor,
                    lineDrawRotation,
                    lineDrawOrigin,
                    lineDrawScale, SpriteEffects.None, 0);
            }


            SpriteEffects effects = NPC.direction == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            spriteBatch.Draw(texture, new Vector2(NPC.Center.X, NPC.Center.Y) - Main.screenPosition + new Vector2(0, NPC.gfxOffY), NPC.frame, new Color(255, 255, 255, 0) * (1f - NPC.alpha / 80f), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);
            return false;
        }
    }
}
