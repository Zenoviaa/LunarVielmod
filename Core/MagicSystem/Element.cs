using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;

namespace Stellamod.Core.MagicSystem
{
    internal abstract class Element : ModItem
    {
        public override void SetDefaults()
        {

        }

        public virtual void AI(MagicProjectile mProj)
        {

        }

        public virtual void DrawForm(MagicProjectile mProj, SpriteBatch spriteBatch, ref Color lightColor)
        {

        }
    }
}
