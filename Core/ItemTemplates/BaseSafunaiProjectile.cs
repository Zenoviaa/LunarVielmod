﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoMod.Core.Utils;
using Stellamod.Core.Effects;
using Stellamod.Core.Effects.Trails;
using Stellamod.Core.Helpers;
using Stellamod.Core.Helpers.Math;
using Stellamod.Core.ItemTemplates;
using Stellamod.Core.SwingSystem;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Urdveil.Common.Bases
{
    public abstract class BaseSafunaiProjectile : ModProjectile
    {
        private bool _initialized;
        private bool _synced;
        private OvalSwing _oval;
        protected ref float Timer => ref Projectile.ai[0];
        protected BaseSafunaiItem Safunai
        {
            get
            {
                return Owner.HeldItem.ModItem as BaseSafunaiItem;
            }
        }
        public ITrailer Trailer { get; set; }
        private Player Owner => Main.player[Projectile.owner];
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.Size = new Vector2(85, 85);
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ownerHitCheck = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.extraUpdates = 16;
        }

        public int SwingTime;
        public float SwingDistance;
        public float Curvature;

        public float ThrowRange = 320;

        public bool Flip = false;
        public bool Slam = false;
        public bool PreSlam = false;

        public Vector2 CurrentBase = Vector2.Zero;

        public Vector2[] swingTrailCache;
        private int slamTimer = 0;

        public override void AI()
        {
            if (!_initialized)
            {
                _oval = new OvalSwing();
                swingTrailCache = new Vector2[32];
                Trailer = new SlashTrailer();
                OnInitialize();
                _initialized = true;
            }

            if (Projectile.timeLeft > 2) //Initialize chain control points on first tick, in case of projectile hooking in on first tick
            {
                _chainMidA = Projectile.Center;
                _chainMidB = Projectile.Center;
            }
            if (Main.myPlayer == Projectile.owner)
            {
                _synced = true;
                Projectile.netUpdate = true;
            }
            else if (!_synced)
                return;


            Projectile.timeLeft = 2;
            Owner.itemTime = 2;
            Owner.heldProj = Projectile.whoAmI;
            ThrowOutAI();

            if (!Slam)
                Owner.itemRotation = MathHelper.WrapAngle(Owner.AngleTo(Projectile.Center) - (Owner.direction < 0 ? MathHelper.Pi : 0));
            else
                Owner.itemRotation = MathHelper.WrapAngle(Owner.AngleTo(Main.MouseWorld) - (Owner.direction < 0 ? MathHelper.Pi : 0));
            Lighting.AddLight(CurrentBase, new Color(254, 204, 72).ToVector3());
        }

        public virtual void OnInitialize()
        {

        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {


        }

        private Vector2 GetSwingPosition(float progress)
        {
            _oval.SetDirection(Flip ? 1 : -1);
            Vector2 velocity = Projectile.velocity;
            _oval.Easing = EasingFunction.InOutCirc;
            if (Slam)
            {
                _oval.Easing = EasingFunction.InOutExpo;
            }


            float endDistance = Math.Min(SwingDistance, 252);
            float distance = MathHelper.Lerp(0, endDistance, EasingFunction.OutExpo(progress));
            distance = MathHelper.Lerp(distance, 0, EasingFunction.InExpo(progress));
            _oval.XSwingRadius = distance;
            _oval.YSwingRadius = distance / 2;
            _oval.UpdateSwing(progress, Projectile.position, velocity, out Vector2 offset);
            return offset;
        }

        public virtual void ThrowOutAI()
        {
            Projectile.rotation = Projectile.AngleFrom(Owner.Center);
            Vector2 position = Owner.MountedCenter;
            float progress = ++Timer / SwingTime; //How far the projectile is through its swing
            if (slamTimer == 5)
                SoundEngine.PlaySound(SoundID.NPCDeath7, Projectile.Center);

            Projectile.Center = position + GetSwingPosition(progress);
            _oval.CalculateTrailingPoints(progress, Projectile.velocity, ref swingTrailCache);
            _oval.TrailOffset = 1f;
            //Now we transform the points
            //Calculating points locally and then translating it is a bit simpler.
            for (int t = 0; t < swingTrailCache.Length; t++)
            {
                Matrix translationMatrix = Matrix.CreateTranslation(new
                    Vector3(Owner.Center.X, Owner.Center.Y, 0));
                swingTrailCache[t] = Vector2.Transform(swingTrailCache[t], translationMatrix);
            }

            Projectile.direction = Projectile.spriteDirection = -Owner.direction * (Flip ? -1 : 1);

            if (Timer >= SwingTime)
                Projectile.Kill();
        }

        protected virtual float WidthFunction(float completionRatio)
        {
            float t = Timer / 60f;
            t = MathHelper.Clamp(t, 0f, 1f);
            return MathHelper.Lerp(0f, 8f, completionRatio) * 1f - t;
        }

        protected virtual Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(Color.Transparent, Color.Purple, completionRatio);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Trailer?.DrawTrail(ref lightColor, swingTrailCache);
            Texture2D projTexture = TextureAssets.Projectile[Projectile.type].Value;
            Texture2D glowTexture = ModContent.Request<Texture2D>(Texture + "_Glow").Value;

            //End control point for the chain
            Vector2 projBottom = Projectile.Center + new Vector2(0, projTexture.Height / 2).RotatedBy(Projectile.rotation + MathHelper.PiOver2) * 0.75f;
            DrawChainCurve(Main.spriteBatch, projBottom, out Vector2[] chainPositions);

            //Adjust rotation to face from the last point in the bezier curve
            float newRotation = (projBottom - chainPositions[chainPositions.Length - 2]).ToRotation() + MathHelper.PiOver2;

            //Draw from bottom center of texture
            Vector2 origin = new Vector2(projTexture.Width / 2, projTexture.Height);
            SpriteEffects flip = (Projectile.spriteDirection < 0) ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            lightColor = Lighting.GetColor((int)(Projectile.Center.X / 16f), (int)(Projectile.Center.Y / 16f));



            SpriteBatch spriteBatch = Main.spriteBatch;
            float scaleMult = EasingFunction.QuadraticBump(Timer / SwingTime);
            scaleMult = scaleMult * 1.25f;
            spriteBatch.Draw(projTexture, projBottom - Main.screenPosition, null, lightColor, newRotation, origin, Projectile.scale * scaleMult, flip, 0);

            spriteBatch.Restart(blendState: BlendState.Additive);
            float glowProgress = EasingFunction.QuadraticBump(Timer / SwingTime);
            for (int i = 0; i < 1; i++)
            {
                spriteBatch.Draw(glowTexture, projBottom - Main.screenPosition, null, Color.White * glowProgress, newRotation, origin, Projectile.scale * scaleMult, flip, 0);
            }
            spriteBatch.RestartDefaults();

            CurrentBase = projBottom + (newRotation - 1.57f).ToRotationVector2() * (projTexture.Height / 2);
            if (!Slam)
                return false;

            Texture2D whiteTexture = ModContent.Request<Texture2D>(Texture + "_White").Value;
            float transparency = glowProgress * 0.5f;
            float scale = 1 + glowProgress * 0.5f;

            spriteBatch.Restart(blendState: BlendState.Additive);
            spriteBatch.Draw(whiteTexture, projBottom - Main.screenPosition, null, Color.White * transparency, newRotation, origin, Projectile.scale * scale, flip, 0);
            for(int i = 0; i < 2; i++)
                spriteBatch.Draw(projTexture, projBottom - Main.screenPosition, null, lightColor, newRotation, origin, Projectile.scale * scaleMult, flip, 0);

            spriteBatch.RestartDefaults();
            return false;
        }

        //Control points for drawing chain bezier, update slowly when hooked in
        private Vector2 _chainMidA;
        private Vector2 _chainMidB;
        protected void DrawChainCurve(SpriteBatch spriteBatch, Vector2 projBottom, out Vector2[] chainPositions)
        {
            Texture2D chainTex = ModContent.Request<Texture2D>(Texture + "_Chain").Value;

            float progress = Timer / SwingTime;

            if (Slam)
                progress = EasingFunction.InOutCubic(progress);
            else
                progress = EasingFunction.OutQuad(progress);

            float angleMaxDeviation = MathHelper.Pi * 0.85f;
            float angleOffset = Owner.direction * (Flip ? -1 : 1) * MathHelper.Lerp(angleMaxDeviation, -angleMaxDeviation / 4, progress);

            _chainMidA = Owner.MountedCenter + GetSwingPosition(progress).RotatedBy(angleOffset) * Curvature;
            _chainMidB = Owner.MountedCenter + GetSwingPosition(progress).RotatedBy(angleOffset / 2) * Curvature * 2.5f;

            Curvature curve = new Curvature(new Vector2[] { Owner.MountedCenter, _chainMidA, _chainMidB, projBottom });

            int numPoints = 30;
            chainPositions = curve.GetPoints(numPoints).ToArray();

            //Draw each chain segment, skipping the very first one, as it draws partially behind the player
            for (int i = 1; i < numPoints; i++)
            {
                Vector2 position = chainPositions[i];

                float rotation = (chainPositions[i] - chainPositions[i - 1]).ToRotation() - MathHelper.PiOver2; //Calculate rotation based on direction from last point
                float yScale = Vector2.Distance(chainPositions[i], chainPositions[i - 1]) / chainTex.Height; //Calculate how much to squash/stretch for smooth chain based on distance between points

                Vector2 scale = new Vector2(1, yScale); // Stretch/Squash chain segment
                Color chainLightColor = Lighting.GetColor((int)position.X / 16, (int)position.Y / 16); //Lighting of the position of the chain segment
                Vector2 origin = new Vector2(chainTex.Width / 2, chainTex.Height); //Draw from center bottom of texture
                spriteBatch.Draw(chainTex, position - Main.screenPosition, null, chainLightColor, rotation, origin, scale, SpriteEffects.None, 0);
            }

            if (Slam)
            {
                spriteBatch.Restart(blendState: BlendState.Additive);
                float glowProgress = EasingFunction.QuadraticBump(Timer / SwingTime);
                int glowPoints = (int)MathHelper.Lerp(0, numPoints, glowProgress);
                for (int i = 1; i < glowPoints; i++)
                {
                    Vector2 position = chainPositions[i];

                    float rotation = (chainPositions[i] - chainPositions[i - 1]).ToRotation() - MathHelper.PiOver2; //Calculate rotation based on direction from last point
                    float yScale = Vector2.Distance(chainPositions[i], chainPositions[i - 1]) / chainTex.Height; //Calculate how much to squash/stretch for smooth chain based on distance between points

                    Vector2 scale = new Vector2(1, yScale); // Stretch/Squash chain segment
                    Color chainLightColor = Lighting.GetColor((int)position.X / 16, (int)position.Y / 16); //Lighting of the position of the chain segment
                    Vector2 origin = new Vector2(chainTex.Width / 2, chainTex.Height); //Draw from center bottom of texture
                    spriteBatch.Draw(chainTex, position - Main.screenPosition, null, chainLightColor, rotation, origin, scale, SpriteEffects.None, 0);
                    spriteBatch.Draw(chainTex, position - Main.screenPosition, null, chainLightColor, rotation, origin, scale, SpriteEffects.None, 0);
                }
                spriteBatch.RestartDefaults();
            }
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Curvature curve = new Curvature(new Vector2[] { Owner.MountedCenter, _chainMidA, _chainMidB, Projectile.Center });

            int numPoints = 32;
            Vector2[] chainPositions = curve.GetPoints(numPoints).ToArray();
            float collisionPoint = 0;
            for (int i = 1; i < numPoints; i++)
            {
                Vector2 position = chainPositions[i];
                Vector2 previousPosition = chainPositions[i - 1];
                if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), position, previousPosition, 6, ref collisionPoint))
                    return true;
            }
            return base.Colliding(projHitbox, targetHitbox);
        }
        public virtual void DrawSwingTrail(ref Color lightColor, Vector2[] swingTrailCache)
        {
            //I think it makes the most sense to abstract our trails out to a trailer and shader cache,
            //so we can just replace the trailer for different trails!
            //So much simpler, and we can just make new trailers

            Trailer?.DrawTrail(ref lightColor, swingTrailCache);
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(SwingTime);
            writer.Write(SwingDistance);
            writer.Write(Flip);
            writer.Write(Slam);
            writer.Write(Curvature);
            writer.Write(PreSlam);
            writer.Write(_synced);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            SwingTime = reader.ReadInt32();
            SwingDistance = reader.ReadSingle();
            Flip = reader.ReadBoolean();
            Slam = reader.ReadBoolean();
            Curvature = reader.ReadSingle();
            PreSlam = reader.ReadBoolean();
            _synced = reader.ReadBoolean();
        }
    }
}
