using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Common.MagicSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stellamod.Items.Elements
{
    internal class Radiant : Element
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
        }


        public override void DrawForm(MagicProjectile mProj, SpriteBatch spriteBatch, ref Color lightColor)
        {
            base.DrawForm(mProj, spriteBatch, ref lightColor);

        }
    }
}
