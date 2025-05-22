using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Trails;
using Stellamod.Helpers;

namespace Stellamod.Projectiles.Test
{
    internal class CircleTestProj : ModProjectile, 
        IPixelPrimitiveDrawer
    {
        public enum ActionState
        {
            Start,
            Shrink,
            Out
        }

        //Max radius of the circle
        public const float Max_Circle_Radius = 600;

        //Min radius of the circle
        public const float Min_Circle_Radius = 256;

        //How long the circle lives
        public const float Circle_Lifetime = 360;
        public const float Circle_Shrink_Delay = 60;
        public const float Circle_Enclose_Time = 60;
        public const float Circle_Out_Time = 120;

        public const float Circle_Width = 128;
        public const int Circle_Points = 128;

        public Vector2[] CirclePos;

        ref float Timer => ref Projectile.ai[0];
        public ActionState State
        {
            get => (ActionState)Projectile.ai[1];
            set => Projectile.ai[1] = (float)value;
        }

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.timeLeft = 360;
            Projectile.tileCollide = false;

            //Points on the circle
            CirclePos = new Vector2[Circle_Points];
        }

        public override bool? CanDamage()
        {
            return State != ActionState.Start;
        }

        public override void AI()
        {
   
            switch (State)
            {
                case ActionState.Start:
                    Start();
                    break;
                case ActionState.Shrink:
                    Shrink();
                    break;
                case ActionState.Out:
                    Out();
                    break;
            }

        }

        private void Start()
        {
            Timer++;
            float circleProgress = Timer / Circle_Shrink_Delay;
            float circleEasedProgress = Easing.InOutExpo(circleProgress);
            Projectile.scale = MathHelper.Lerp(0f, 1f, circleEasedProgress);
            DrawCircle(Max_Circle_Radius);
            if (Timer >= Circle_Shrink_Delay)
            {
                State = ActionState.Shrink;
                Timer = 0;
            }
        }

        private void Shrink()
        {
            Timer++;
            float circleProgress = Timer / Circle_Enclose_Time;
            float circleEasedProgress = Easing.InOutExpo(circleProgress);
            float circleRadius = MathHelper.Lerp(Max_Circle_Radius, Min_Circle_Radius, circleEasedProgress);
            DrawCircle(circleRadius);
            if(Timer >= Circle_Enclose_Time)
            {
                Timer = 0;
                State = ActionState.Out;
            }
        }

        private void Out()
        {
            Timer++;
            float outProgress = Timer / Circle_Out_Time;
            float outEasedProgress = Easing.InOutExpo(outProgress);
            Projectile.scale = MathHelper.Lerp(1f, 0f, outEasedProgress);
            DrawCircle(Min_Circle_Radius);
            if(Timer >= Circle_Out_Time)
            {
                Timer = 0;
                Projectile.Kill();
            }
        }

        private void DrawCircle(float radius)
        {
            Vector2 startDirection = Vector2.UnitY;
            for (int i = 0; i < CirclePos.Length; i++)
            {
                float circleProgress = i / (float)(CirclePos.Length);
                float radiansToRotateBy = circleProgress * (MathHelper.TwoPi + MathHelper.PiOver4/2);
                CirclePos[i] = Projectile.Center + startDirection.RotatedBy(radiansToRotateBy) * radius;
            }
 
        }

        public float WidthFunction(float completionRatio)
        {
            return Projectile.scale * Circle_Width;
        }

        public Color ColorFunction(float completionRatio)
        {
            Color color = Color.Lerp(new Color(93, 203, 243), ColorFunctions.MiracleVoid, 0.65f);
            return color * Projectile.Opacity * MathF.Pow(Utils.GetLerpValue(0f, 0.1f, completionRatio, true), 3f);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            //This damages everything in the trail
            Vector2[] positions = CirclePos;
            float collisionPoint = 0;
            for (int i = 1; i < positions.Length; i++)
            {
                Vector2 position = positions[i];
                Vector2 previousPosition = positions[i - 1];
                if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), position, previousPosition, 6, ref collisionPoint))
                    return true;
            }
            return false;
        }

        internal PrimitiveTrail BeamDrawer;
        public void DrawPixelPrimitives(SpriteBatch spriteBatch)
        {
            BeamDrawer ??= new PrimitiveTrail(WidthFunction, ColorFunction, null, true, TrailRegistry.LaserShader);

            TrailRegistry.LaserShader.UseColor(Color.LightCyan);
            TrailRegistry.LaserShader.SetShaderTexture(TrailRegistry.BeamTrail);

            BeamDrawer.DrawPixelated(CirclePos, -Main.screenPosition, CirclePos.Length);
            Main.spriteBatch.ExitShaderRegion();
        }
    }
}
