using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace Stellamod.Core.Grass
{
   
    [Autoload(Side = ModSide.Client)]
    public class GrassRenderer : ModSystem
    {
     
        public override void OnModLoad()
        {
            base.OnModLoad();
        }
        public override void OnModUnload()
        {
            base.OnModUnload();
        }
    }
}
