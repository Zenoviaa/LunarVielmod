﻿
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary;
using Stellamod.Particles;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Slashers.Reavestor
{
    public class ReavestorSwordProj2 : ModProjectile
    {
        public static bool swung = false;
        public int SwingTime = 15;
        public float holdOffset = 30f;
        public int combowombo;
        private bool ParticleSpawned;
        private bool _initialized;
        private int timer;

        public override void SetDefaults()
        {
            Projectile.damage = 30;
            Projectile.timeLeft = SwingTime;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.height = 90;
            Projectile.width = 90;
            Projectile.friendly = true;
            Projectile.scale = 1f;
        }

        public float Timer
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        public virtual float Lerp(float val)
        {
            return val == 1f ? 1f : (val == 1f ? 1f : (float)Math.Pow(2, val * 10f - 10f) / 2f);
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            if (!_initialized && Main.myPlayer == Projectile.owner)
            {
                timer++;

                SwingTime = (int)(30 / player.GetAttackSpeed(DamageClass.Melee));
                Projectile.alpha = 255;
                Projectile.timeLeft = SwingTime;
                _initialized = true;
                Projectile.damage -= 9999;
                //Projectile.netUpdate = true;

            }
            else if (_initialized)
            {
                Projectile.alpha = 0;
                if (!player.active || player.dead || player.CCed || player.noItems)
                {
                    return;
                }
                if (timer == 1)
                {
                    Projectile.damage += 9999;
                    Projectile.damage *= 4;

                    timer++;
                }
                Vector3 RGB = new Vector3(1.28f, 0f, 1.28f);
                float multiplier = 1;
                float max = 2.25f;
                float min = 1.0f;
                RGB *= multiplier;
                if (RGB.X > max)
                {
                    multiplier = 0.5f;
                }
                if (RGB.X < min)
                {
                    multiplier = 1.5f;
                }
                Lighting.AddLight(Projectile.position, RGB.X, RGB.Y, RGB.Z);
                Projectile.usesLocalNPCImmunity = true;
                Projectile.localNPCHitCooldown = 10000;
                int dir = (int)Projectile.ai[1];
                if (!ParticleSpawned)
                {

                        ParticleManager.NewParticle(player.Center, player.DirectionTo(Main.MouseWorld), ParticleManager.NewInstance<FabledParticle5>(), Color.Purple, 0.7f, Projectile.whoAmI, Projectile.whoAmI);

                
                    ParticleManager.NewParticle(player.Center, player.DirectionTo(Main.MouseWorld), ParticleManager.NewInstance<AuroranSlashParticle>(), Color.Purple, 0.7f, Projectile.whoAmI, Projectile.whoAmI);
                    ParticleSpawned = true;
                }


                for (int i = 0; i < 5; i++)
                {
                    Dust dust = Dust.NewDustDirect(Projectile.position - Projectile.velocity, Projectile.width, Projectile.height, DustID.GoldCoin, 0, 0, 100, Color.Violet, 1f);
                    dust.noGravity = true;
                    dust.velocity *= 2f;
                    dust = Dust.NewDustDirect(Projectile.position - Projectile.velocity, Projectile.width, Projectile.height, DustID.GoldCoin, 0f, 0f, 1000, Color.Violet, 1f);
                }
                player.statDefense += 10;

             
                float swingProgress = Lerp(Utils.GetLerpValue(0f, SwingTime, Projectile.timeLeft));
                // the actual rotation it should have
                float defRot = Projectile.velocity.ToRotation();
                // starting rotation
                float endSet = ((MathHelper.PiOver2) / 0.2f);
                float start = defRot - endSet;

                // ending rotation
                float end = defRot + endSet;
                // current rotation obv
                float rotation = dir == 1 ? start.AngleLerp(end, swingProgress) : start.AngleLerp(end, 1f - swingProgress);
                // offsetted cuz sword sprite
                Vector2 position = player.RotatedRelativePoint(player.MountedCenter);
                position += rotation.ToRotationVector2() * holdOffset;
                Projectile.Center = position;
                Projectile.rotation = (position - player.Center).ToRotation() + MathHelper.PiOver4;

                player.heldProj = Projectile.whoAmI;
                player.ChangeDir(Projectile.velocity.X < 0 ? -1 : 1);
                player.itemRotation = rotation * player.direction;
                player.itemTime = 2;
                player.itemAnimation = 2;
                //Projectile.netUpdate = true;
            }

        }

        public override bool ShouldUpdatePosition() => false;


        public override void OnKill(int timeLeft)
        {
            Player player = Main.player[Projectile.owner];
            player.statDefense -= 10;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Ichor, 600);
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = (Texture2D)ModContent.Request<Texture2D>(Texture);

            int frameHeight = texture.Height / Main.projFrames[Projectile.type];
            int startY = frameHeight * Projectile.frame;

            Rectangle sourceRectangle = new Rectangle(0, startY, texture.Width, frameHeight);
            Vector2 origin = sourceRectangle.Size() / 2f;
            Color drawColor = Projectile.GetAlpha(lightColor);

            Main.EntitySpriteDraw(texture,
                Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY),
                sourceRectangle, drawColor, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);




            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            Main.instance.LoadProjectile(Projectile.type);


            // Redraw the projectile with the color not influenced by light
            Vector2 drawOrigin = new Vector2(texture.Width * 0.5f, Projectile.height * 0.5f);
            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = (Projectile.oldPos[k] - Main.screenPosition) + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Color color = Projectile.GetAlpha(Color.Lerp(new Color(255, 215, 13), new Color(140, 80, 13), 1f / Projectile.oldPos.Length * k) * (1f - 1f / Projectile.oldPos.Length * k));
                Main.EntitySpriteDraw(texture, drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            }
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 215, 12, 0) * (1f - Projectile.alpha / 255f);
        }
    }
}