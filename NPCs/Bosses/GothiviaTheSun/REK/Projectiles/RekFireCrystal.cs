using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Dusts;
using Stellamod.Helpers;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Animations;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Bosses.GothiviaTheSun.REK.Projectiles
{
    internal class RekFireCrystal : ModNPC
    {
        private ref float Timer => ref NPC.ai[0];
        private Vector2 LaserDirection
        {
            get
            {
                Player target = Main.player[NPC.target];
                Vector2 direction = NPC.Center.DirectionTo(target.Center);
                direction = direction.RotatedBy(MathHelper.PiOver2);
                return direction;
            }
        }

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 1;
            NPCID.Sets.TrailCacheLength[Type] = 18;
            NPCID.Sets.TrailingMode[Type] = 4;
        }

        public override void SetDefaults()
        {
            NPC.width = 54;
            NPC.height = 122;
            NPC.lifeMax = 18000;
            NPC.knockBackResist = 0f;
            NPC.damage = 150;
            NPC.defense = 70;
            NPC.noGravity = true;
            NPC.HitSound = SoundID.DD2_WitherBeastCrystalImpact;
            NPC.DeathSound = SoundID.DD2_WitherBeastDeath;
           
        }

        public override void AI()
        {
            NPC.TargetClosest();
 
            Timer++;
            if(Timer == 1)
            {
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/RekSummon"), NPC.position);
            }

            float frequency = 0.2f;
            float amplitude = 1;
            NPC.velocity = new Vector2(0, MathF.Sin(Timer * frequency) * amplitude);
            if(Timer == 1)
            {
                for (int i = 0; i < 8; i++)
                {
                    int dustType = ModContent.DustType<GlowDust>();
                    Vector2 velocity = Main.rand.NextVector2CircularEdge(8, 8);
                    Dust.NewDustPerfect(NPC.Center, dustType, velocity, Alpha: 0, newColor: Color.White);
                }

                if (StellaMultiplayer.IsHost)
                {
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero,
                    ModContent.ProjectileType<RekFireCrystalCircleProj>(), 0, 0, Main.myPlayer);
                }
            }

            if(Timer == 540)
            {
                int damage = 150;
                int knockback = 2;
                if (StellaMultiplayer.IsHost)
                {
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, -LaserDirection,
                        ModContent.ProjectileType<RekFireCrystalLaserProj>(), damage, knockback, Main.myPlayer, ai1: NPC.whoAmI);
                }
            }

            if(Main.rand.NextBool(30))
            {
                int dustType = ModContent.DustType<TSmokeDust>();
                Vector2 velocity = -Vector2.UnitX * 3;
                Dust.NewDustPerfect(
                    NPC.Center + Main.rand.NextVector2Circular(64, 64), 
                    dustType, 
                    velocity, 
                    Alpha: 0, 
                    newColor: Color.OrangeRed);
            }

            // Some visuals here
            Lighting.AddLight(NPC.Center, Color.White.ToVector3() * 0.78f);
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
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            Texture2D texture = TextureAssets.Npc[NPC.type].Value;
            int projFrames = Main.npcFrameCount[NPC.type];
            int frameHeight = texture.Height / projFrames;
            int startY = frameHeight * NPC.frame.Y;

            Rectangle sourceRectangle = new Rectangle(0, startY, texture.Width, frameHeight);
            Vector2 drawOrigin = sourceRectangle.Size() / 2f;
            //drawOrigin.X = projectile.spriteDirection == 1 ? sourceRectangle.Width - offsetX : offsetX;
            for (int k = 0; k < NPC.oldPos.Length; k++)
            {
                Vector2 drawPos = NPC.oldPos[k] - Main.screenPosition + drawOrigin;// + new Vector2(0f, projectile.gfxOffY);
                Color color = NPC.GetAlpha(Color.Lerp(Color.OrangeRed, Color.Transparent, 1f / NPC.oldPos.Length * k) * (1f - 1f / NPC.oldPos.Length * k));
                Main.spriteBatch.Draw(texture, drawPos, sourceRectangle, color, NPC.oldRot[k], drawOrigin, NPC.scale, SpriteEffects.None, 0f);
            }

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);


            if(Timer < 540)
            {
                float progress = Timer / 540f;
                Texture2D lineTexture = ModContent.Request<Texture2D>("Stellamod/Effects/Masks/Extra_47").Value;
                Color lineDrawColor = (Color)GetLineAlpha(drawColor);
                lineDrawColor *= progress;


                Vector2 lineDrawOrigin = texture.Size() / 2;
                float lineDrawScale = NPC.scale;
                float lineDrawRotation = LaserDirection.ToRotation() + MathHelper.PiOver2;
                Main.spriteBatch.Draw(lineTexture, NPC.Center - Main.screenPosition, null,
                    lineDrawColor,
                    lineDrawRotation,
                    lineDrawOrigin,
                    lineDrawScale, SpriteEffects.None, 0);
            }

            return base.PreDraw(spriteBatch, screenPos, drawColor);
        }

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D glowTexture = ModContent.Request<Texture2D>(Texture + "_Glow").Value;
            Vector2 drawPosition = NPC.Center - screenPos;
            float drawRotation = NPC.rotation;
            Vector2 drawOrigin = NPC.Size / 2;
            float drawScale = NPC.scale;
            float osc = VectorHelper.Osc(0, 1, speed: 6);

            for (float j = 0f; j < 1f; j += 0.25f)
            {
                float radians = (j + osc) * MathHelper.TwoPi;
                spriteBatch.Draw(glowTexture, drawPosition + new Vector2(0f, 8f).RotatedBy(radians) * osc,
                    null, Color.White * osc * 0.3f, drawRotation, drawOrigin, drawScale, SpriteEffects.None, 0);
            }


            float progress = Timer / 30f;
            float easedProgress = Easing.OutExpo(1f - progress);
            Color whiteDrawColor = Color.White * easedProgress;
            Texture2D whiteTexture = ModContent.Request<Texture2D>(Texture + "_White").Value;
            spriteBatch.Draw(
                whiteTexture, 
                drawPosition,
                null,
                whiteDrawColor, 
                drawRotation, 
                drawOrigin, 
                drawScale, SpriteEffects.None, 0);
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            for(int i = 0; i < 4; i++)
            {
                int dustType = ModContent.DustType<GlowDust>();
                Vector2 velocity = Main.rand.NextVector2Circular(8, 8);
                Dust.NewDustPerfect(NPC.Center, dustType, velocity, Alpha: 0, newColor: Color.OrangeRed);
            }

            if(NPC.lifeMax <= 0)
            {
                //Death explosion thing
            }
        }
    }
}
