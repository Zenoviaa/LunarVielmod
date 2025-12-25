using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace Stellamod.Content.Areas.Illuria.WeaponsIL
{
    public class IcicleSystem
    {
        private int _numIcicles;
        private Icicle[] _icicles;
        public IcicleSystem(int numIcicles, int steps = 32)
        {
            _numIcicles = numIcicles;
            _icicles = new Icicle[_numIcicles];
            for (int i = 0; i < _icicles.Length; i++)
            {
                _icicles[i] = new Icicle(steps);
            }
        }

        public void Update(Vector2 initialPosition, Vector2 initialVelocity, float time, float offsetPerIcicle = 0f)
        {
            for (int i = 0; i < _icicles.Length; i++)
            {
                Icicle icicle = _icicles[i];
                icicle.initialPosition = initialPosition;
                icicle.initialVelocity = initialVelocity;//.RotatedByRandom(0.5f);
                icicle.initialVelocity = icicle.initialVelocity.RotatedBy(offsetPerIcicle * i);
                icicle.time = time;
                icicle.Update();
            }
        }
        public void Draw(SpriteBatch spriteBatch, Vector2 screenPos)
        {
            for (int i = 0; i < _icicles.Length; i++)
            {
                Icicle icicle = _icicles[i];
                icicle.Draw(spriteBatch, screenPos);
            }
        }
    }
}
