using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;

namespace Stellamod.Common.Shaders.MagicTrails
{
    internal class LightningTrail
    {
        private Vector2[] _offsets;
        public LightningTrail()
        {

        }

        public float LightningRandomOffsetRange = 32;
        public float LightningRandomExpand = 8;
        public void RandomPositions(Vector2[] oldPos)
        {
            Vector2[] offsets = new Vector2[oldPos.Length];
            offsets[0] = Vector2.Zero;
            for (int i = 1; i < offsets.Length - 1; i++)
            {

                Vector2 prevPosition = oldPos[i - 1];
                Vector2 nextPosition = oldPos[i + 1];
                if (prevPosition == Vector2.Zero || nextPosition == Vector2.Zero)
                {
                    offsets[i] = Vector2.Zero;
                    continue;
                }

                Vector2 normalDir = (prevPosition - nextPosition).SafeNormalize(Vector2.One).RotatedBy(MathHelper.PiOver2);
                float length = Main.rand.NextFromList(-1, 1) * Main.rand.NextFloat(LightningRandomOffsetRange, LightningRandomOffsetRange);
                offsets[i] = normalDir * length + Main.rand.NextVector2Circular(LightningRandomExpand, LightningRandomExpand);
            }
            offsets[offsets.Length - 1] = Vector2.Zero;
            _offsets = offsets;
        }

        public void Draw(SpriteBatch spriteBatch, Vector2[] oldPos, float[] oldRot, Func<float, Color> colorFunc, Func<float, float> widthFunc, BaseShader shader, Vector2? offset = null)
        {
            if (_offsets == null)
                return;
            Vector2[] lightningTrailPos = new Vector2[oldPos.Length];
            for (int i = 0; i < lightningTrailPos.Length && i < _offsets.Length; i++)
            {
                lightningTrailPos[i] = oldPos[i] + _offsets[i];
            }

            TrailDrawer.Draw(spriteBatch, lightningTrailPos, oldRot, colorFunc, widthFunc, shader, offset);
        }
    }
}
