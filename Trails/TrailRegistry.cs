using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace Stellamod.Trails
{
    static class TrailRegistry
    {
        public static MiscShaderData LaserShader => GameShaders.Misc["Stellamod:LaserShader"];
        public static Asset<Texture2D> BeamTrail => ModContent.Request<Texture2D>("Stellamod/Trails/Beam");
        public static Asset<Texture2D> BeamTrail2 => ModContent.Request<Texture2D>("Stellamod/Trails/BeamTrail");
        public static Asset<Texture2D> BulbTrail => ModContent.Request<Texture2D>("Stellamod/Trails/BulbTrail");

    }
}
