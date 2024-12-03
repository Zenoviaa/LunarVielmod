using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Common.Shaders;
using Stellamod.Helpers;
using Stellamod.Trails;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Common.Bases
{
    public abstract class BaseSafunaiProjectile : ModProjectile
    {
        private Vector2[] _oldSwingPos;
        private bool _synced;
        protected ref float Timer => ref Projectile.ai[0];
        protected BaseSafunaiItem Safunai
        {
            get
            {
                return Owner.HeldItem.ModItem as BaseSafunaiItem;
            }
        }

        private Player Owner => Main.player[Projectile.owner];
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            _oldSwingPos = new Vector2[32];
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

        private int slamTimer = 0;

        public override void AI()
        {
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

            Lighting.AddLight(CurrentBase, new Color(254, 204, 72).ToVector3());
            Projectile.timeLeft = 2;

            if (Slam)
                Owner.itemTime = Owner.itemAnimation = 25;
            else if (PreSlam)
                Owner.itemTime = Owner.itemAnimation = 10;

            ThrowOutAI();

            if (!Slam)
                Owner.itemRotation = MathHelper.WrapAngle(Owner.AngleTo(Projectile.Center) - (Owner.direction < 0 ? MathHelper.Pi : 0));
            else
                Owner.itemRotation = MathHelper.WrapAngle(Owner.AngleTo(Main.MouseWorld) - (Owner.direction < 0 ? MathHelper.Pi : 0));
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {


        }

        private Vector2 GetSwingPosition(float progress)
        {
            if (Slam)
            {
                progress = Easing.InOutCubic(progress);
                progress = Easing.InBack(progress);
            }
            else
            {
                progress = Easing.InOutCubic(progress);

            }
         
            //Starts at owner center, goes to peak range, then returns to owner center
            float distance = MathHelper.Clamp(SwingDistance, ThrowRange * 0.1f, ThrowRange) * MathHelper.Lerp((float)Math.Sin(progress * MathHelper.Pi), 1, 0.04f);
            distance = Math.Max(distance, 100); //Dont be too close to player

            float angleMaxDeviation = MathHelper.Pi / 1.2f;
            float angleOffset = Owner.direction * (Flip ? -1 : 1) * MathHelper.Lerp(-angleMaxDeviation, angleMaxDeviation, progress); //Moves clockwise if player is facing right, counterclockwise if facing left
            return Projectile.velocity.RotatedBy(angleOffset) * distance;
        }

        public virtual void ThrowOutAI()
        {
            Projectile.rotation = Projectile.AngleFrom(Owner.Center);
            Vector2 position = Owner.MountedCenter;
            float progress = ++Timer / SwingTime; //How far the projectile is through its swing


            if (slamTimer == 5)
                SoundEngine.PlaySound(SoundID.NPCDeath7, Projectile.Center);

            Vector2[] swingPos = new Vector2[60];
            for(int i = 0; i < swingPos.Length; i++)
            {
                float l = swingPos.Length;
                //Lerp between the points
                float progressOnTrail = i / l;

                //Calculate starting lerp value
                float startTrailLerpValue = MathHelper.Clamp(progress - 0.5f, 0, 1);
                float startTrailProgress = startTrailLerpValue;
                startTrailProgress = startTrailLerpValue;


                //Calculate ending lerp value
                float endTrailLerpValue = progress;
                float endTrailProgress = endTrailLerpValue;
                endTrailProgress = endTrailLerpValue;

                //Lerp in between points
                float smoothedTrailProgress = MathHelper.Lerp(startTrailProgress, endTrailProgress, progressOnTrail);
                Vector2 centerPos = position + GetSwingPosition(smoothedTrailProgress);
                swingPos[i] = centerPos;
            }
            _oldSwingPos = swingPos;

            Projectile.Center = position + GetSwingPosition(progress);
            Projectile.direction = Projectile.spriteDirection = -Owner.direction * (Flip ? -1 : 1);

            if (Timer >= SwingTime)
                Projectile.Kill();
        }

        protected virtual float WidthFunction(float completionRatio)
        {
            float t = Timer / 60f;
            t = MathHelper.Clamp(t, 0f, 1f);
            return MathHelper.Lerp(0f, 32, completionRatio) * 1f - t;
        }

        protected virtual Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(Color.Transparent,Color.Purple,completionRatio);
        }

        public PrimDrawer TrailDrawer { get; private set; } = null;
        protected virtual void DrawSlashTrail()
        {
            Vector2 drawOffset = -Main.screenPosition;
            TrailDrawer ??= new PrimDrawer(WidthFunction, ColorFunction, GameShaders.Misc["VampKnives:SuperSimpleTrail"]);
            TrailDrawer.Shader = GameShaders.Misc["VampKnives:SuperSimpleTrail"];
            GameShaders.Misc["VampKnives:SuperSimpleTrail"].SetShaderTexture(TrailRegistry.LightningTrail2);
            TrailDrawer.DrawPrims(_oldSwingPos, drawOffset, 155);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            DrawSlashTrail();
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
            float scaleMult = Easing.SpikeOutCirc(Timer / SwingTime);
            scaleMult = scaleMult * 1.25f;
            spriteBatch.Draw(projTexture, projBottom - Main.screenPosition, null, lightColor, newRotation, origin, Projectile.scale * scaleMult, flip, 0);

            spriteBatch.Restart(blendState: BlendState.Additive);
            float glowProgress = Easing.SpikeOutCirc(Timer / SwingTime);
            for(int i = 0; i < 1; i++)
            {
                spriteBatch.Draw(glowTexture, projBottom - Main.screenPosition, null, Color.White * glowProgress, newRotation, origin, Projectile.scale * scaleMult, flip, 0);
            }
            spriteBatch.RestartDefaults();

            CurrentBase = projBottom + (newRotation - 1.57f).ToRotationVector2() * (projTexture.Height / 2);
            if (!Slam)
                return false;

            Texture2D whiteTexture = ModContent.Request<Texture2D>(Texture + "_White").Value;
            if (slamTimer < 20 && slamTimer > 5)
            {
                float progress = (slamTimer - 5) / 15f;
                float transparency = (float)Math.Pow(1 - progress, 2);
                float scale = 1 + progress;
                Main.spriteBatch.Draw(whiteTexture, projBottom - Main.screenPosition, null, Color.White * transparency, newRotation, origin, Projectile.scale * scale, flip, 0);
            }

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
                progress = EaseFunction.EaseCubicInOut.Ease(progress);
            else
                progress = EaseFunction.EaseQuadOut.Ease(progress);

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
