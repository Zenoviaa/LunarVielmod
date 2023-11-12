using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Runtime.CompilerServices;
using Terraria;

namespace Stellamod
{
    public static class VectorHelper
    {
        public static Color Alpha(this Color c, float alpha)
        {
            return new Color(c.R, c.G, c.B, (int)(255f * MathHelper.Clamp(alpha, 0f, 1f)));
        }

        public static Color MultiplyAlpha(this Color c, float alpha)
        {
            return new Color(c.R, c.G, c.B, (int)(c.A / 255f * MathHelper.Clamp(alpha, 0f, 1f) * 255f));
        }

        public static Vector2 Right => new Vector2(1f, 0f);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 GetPosition(this Rectangle rect)
        {
            return new Vector2(rect.X, rect.Y);
        }

        public static Player GetNearestPlayerDirect(Vector2 Point, bool Alive = false)
        {
            float NearestPlayerDist = -1f;
            Player NearestPlayer = null;
            Player[] player2 = Main.player;
            foreach (Player player in player2)
            {
                if ((!Alive || (player.active && !player.dead)) && (NearestPlayerDist == -1f || player.Distance(Point) < NearestPlayerDist))
                {
                    NearestPlayerDist = player.Distance(Point);
                    NearestPlayer = player;
                }
            }
            return NearestPlayer;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Osc(float from, float to, float speed = 1f, float offset = 0f)
        {
            float dif = (to - from) / 2f;
            return from + dif + dif * (float)Math.Sin(Main.GlobalTimeWrappedHourly * speed + offset);
        }
        public static class Numbers
        {
            /// <summary>
            /// Cycles a number from 0 to 1 to 0. <br/>
            /// Mode 1 = -1 to 1.
            /// Mode 2 = 0 to 1.
            /// <summary>
            public static float ZerotoOne(float multiplier = 1f, int mode = 0, float differentiator = 0)
            {
                double rad = mode == 0 ? Math.PI : mode == 1 ? Math.PI * 2 : Math.PI / 2;
                return (float)Math.Sin((Main.GlobalTimeWrappedHourly + differentiator * multiplier) % rad);
            }
        }
        public static Vector2 MovemontVelocity(Vector2 A, Vector2 B, float speed)
        {
            Vector2 move = B - A;
            move *= speed / move.Length();
            if (!move.HasNaNs())
            {
                return move;
            }
            return Vector2.Zero;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 RandomPointInArea(Vector2 A, Vector2 B)
        {
            return new Vector2(Main.rand.Next((int)A.X, (int)B.X) + 1, Main.rand.Next((int)A.Y, (int)B.Y) + 1);
        }
        public static void Navigate(this Projectile p, Vector2 to, float speed, float smooth)
        {
            Vector2 move = to - p.position;
            Vector2 vel = move * (speed / move.Length());
            p.velocity += (vel - p.velocity) / smooth;
        }


        public static void NPCNavigate(this NPC p, Vector2 to, float speed, float smooth)
        {
            Vector2 move = to - p.position;
            Vector2 vel = move * (speed / move.Length());
            p.velocity += (vel - p.velocity) / smooth;
        }
        public static Vector2 DirectionTo(this Vector2 origin, Vector2 target)
        {
            return Vector2.Normalize(target - origin);
        }
        public static Color ToColor(this Vector3 vec, float alpha = 1f)
        {
            return new Color(vec.X, vec.Y, vec.Z, alpha);
        }
        public static void BeginBlendState(this SpriteBatch spriteBatch, BlendState state, SamplerState samplerState = null, bool ui = false)
        {
            spriteBatch.End();
            spriteBatch.Begin((!ui) ? SpriteSortMode.Immediate : SpriteSortMode.Deferred, state, samplerState ?? Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, ui ? Main.UIScaleMatrix : Main.GameViewMatrix.TransformationMatrix);
        }

        public static void EndBlendState(this SpriteBatch spriteBatch, bool ui = false)
        {
            spriteBatch.End();
            spriteBatch.Begin((!ui) ? SpriteSortMode.Immediate : SpriteSortMode.Deferred, BlendState.AlphaBlend, ui ? SamplerState.AnisotropicClamp : Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, ui ? Main.UIScaleMatrix : Main.GameViewMatrix.TransformationMatrix);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 Normalized(this Vector2 vec)
        {
            return vec.SafeNormalize(Vector2.Zero);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Distance(this Vector2 v, Vector2 To)
        {
            float dist = Vector2.Distance(v, To);
            if (!float.IsNaN(dist))
            {
                return dist;
            }
            return 0f;
        }
    }
}