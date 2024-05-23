using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Helpers;
using Stellamod.Projectiles.Visual;
using Stellamod.Trails;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Bosses.GothiviaTheSun.GOS.Projectiles
{
    internal class GothLightBlastProj : ModProjectile
    {
        internal PrimitiveTrail BeamDrawer;
        public override string Texture => TextureRegistry.EmptyTexture;
        //Ai
        private ref float Timer => ref Projectile.ai[0];
        private float LifeTime => 60f;
        private float BlowtorchDistance => 3096;

        //Draw Code
        private Vector2[] LinePos;
        public override void SetDefaults()
        {
            Projectile.width = 412;
            Projectile.height = 16;
            Projectile.hostile = false;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = (int)LifeTime;
            Projectile.hide = true;
            LinePos = new Vector2[5];
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 5;
        }
        public override void AI()
        {
            Timer++;
            if(Timer == LifeTime / 2)
            {
                if (StellaMultiplayer.IsHost)
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero,
                        ModContent.ProjectileType<GothCircleExpand>(), 0, 0, Projectile.owner);
                }


                //Effects
                SoundEngine.PlaySound(SoundID.DD2_BetsyFireballShot, Projectile.position);
                SoundEngine.PlaySound(SoundID.DD2_EtherianPortalSpawnEnemy, Projectile.position);
                ScreenShaderSystem screenShaderSystem = ModContent.GetInstance<ScreenShaderSystem>();
                screenShaderSystem.TintScreen(Color.White, 0.05f, timer: 30f);
                screenShaderSystem.DistortScreen(TextureRegistry.NormalNoise1, new Vector2(0.5f, 0.5f), timer: 15);
                Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(Projectile.Center, 1024, 16f);
            }

            if (Projectile.scale < 1f || Timer <= 1)
            {
                Projectile.scale = MathF.Sin(Timer / 600f * MathHelper.Pi) * 3f;
                if (Projectile.scale > 1f)
                    Projectile.scale = 1f;
            }

            if(Timer > LifeTime / 2)
            {
                float progress = (Timer - LifeTime / 2) / LifeTime;
                float easedProgress = Easing.OutExpo(progress);
                List<Vector2> points = new();
                for (int i = 0; i <= 8; i++)
                {
                    points.Add(Vector2.Lerp(Projectile.Center, Projectile.Center + Projectile.velocity.SafeNormalize(Vector2.Zero) * easedProgress
                        * BlowtorchDistance, i / 8f));
                }
                LinePos = points.ToArray();
            }

        }

        public override bool ShouldUpdatePosition()
        {
            //Returning false makes velocity not move the projectile
            return false;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(BuffID.OnFire3, 60);
        }

        public float WidthFunction(float completionRatio)
        {
            float mult = 1;
            if (Projectile.timeLeft < LifeTime / 3f)
            {
                mult = Projectile.timeLeft / (float)LifeTime / 3f;
            }
            return Projectile.width * Projectile.scale * 1.3f * mult * MathF.Sin((1f - completionRatio) * 0.5f);
        }

        public Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(Color.Black, Color.LightGoldenrodYellow, completionRatio);
        }


        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            //This damages everything in the trail
            Vector2[] positions = LinePos;
            float collisionPoint = 0;
            for (int i = 1; i < positions.Length; i++)
            {
                Vector2 position = positions[i];
                Vector2 previousPosition = positions[i - 1];
                if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), position, previousPosition, 64, ref collisionPoint))
                    return true;
            }

            //Return false to not use default collision
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if(Timer < LifeTime / 2f)
            {
                float progress = Timer / (LifeTime / 2f);
                Texture2D lineTexture = ModContent.Request<Texture2D>("Stellamod/Effects/Masks/Extra_47").Value;
                Color lineDrawColor = new Color(
                    Color.White.R,
                    Color.White.G,
                    Color.White.B, 0) * (1f - Projectile.alpha / 50f);
                lineDrawColor *= progress;

                Vector2 lineDrawOrigin = lineTexture.Size();
                float lineDrawScale = 1f;
                float lineDrawRotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
                Main.spriteBatch.Draw(lineTexture, Projectile.Center - Main.screenPosition, null,
                    lineDrawColor,
                    lineDrawRotation,
                    lineDrawOrigin,
                    lineDrawScale, SpriteEffects.None, 0);

            }

            BeamDrawer ??= new PrimitiveTrail(WidthFunction, ColorFunction, null, true, TrailRegistry.FireVertexShader);
            BeamDrawer.SpecialShader = TrailRegistry.FireVertexShader;
            BeamDrawer.SpecialShader.UseColor(Color.Lerp(Color.Black, Color.White, 0.3f));
            BeamDrawer.SpecialShader.SetShaderTexture(TrailRegistry.BeamTrail);
            BeamDrawer.DrawPixelated(LinePos, -Main.screenPosition, 32);
            Main.spriteBatch.ExitShaderRegion();
            return false;
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            base.DrawBehind(index, behindNPCsAndTiles, behindNPCs, behindProjectiles, overPlayers, overWiresUI);
            behindNPCs.Add(index);
        }
    }
}
