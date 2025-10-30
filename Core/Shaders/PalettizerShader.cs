using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Assets;
using Stellamod.Trails;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace Stellamod.Core.Shaders
{
    public class PalettizerShader : BaseShader
    {
        private static PalettizerShader _instance;
        public static PalettizerShader Instance
        {
            get
            {
                _instance ??= new();
                _instance.SetDefaults();
                return _instance;
            }
        }


        public Asset<Texture2D> PaletteTexture
        {
            set
            {
                Data.UseImage1(value);
            }
        }
       
        public override void SetDefaults()
        {
            base.SetDefaults();
        }
    }
}
