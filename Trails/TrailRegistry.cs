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


        public static MiscShaderData GenericLaserVertexShader => GameShaders.Misc["VampKnives:GenericLaserShader"];
        public static MiscShaderData LightBeamVertexShader => GameShaders.Misc["VampKnives:LightBeamVertexShader"];
        public static MiscShaderData ArtemisLaserShader => GameShaders.Misc["VampKnives:ArtemisLaserShader"];
        public static MiscShaderData FireVertexShader => GameShaders.Misc["VampKnives:Fire"];



        public static Asset<Texture2D> StarTrail => ModContent.Request<Texture2D>("Stellamod/Effects/Primitives/Trails/StarTrail");
        public static Asset<Texture2D> StringTrail => ModContent.Request<Texture2D>("Stellamod/Effects/Primitives/Trails/StringTrail");
        public static Asset<Texture2D> SmallWhispyTrail => ModContent.Request<Texture2D>("Stellamod/Effects/Primitives/Trails/SmallWhispyTrail");
        public static Asset<Texture2D> WaveTrail => ModContent.Request<Texture2D>("Stellamod/Effects/Primitives/Trails/WaveTrail");
        public static Asset<Texture2D> WaterTrail => ModContent.Request<Texture2D>("Stellamod/Effects/Primitives/Trails/WaterTrail");

        public static Asset<Texture2D> CrystalTrail => ModContent.Request<Texture2D>("Stellamod/Effects/Primitives/Trails/CrystalTrail");
        public static Asset<Texture2D> VortexTrail => ModContent.Request<Texture2D>("Stellamod/Effects/Primitives/Trails/VortexTrail");
        public static Asset<Texture2D> WhispyTrail => ModContent.Request<Texture2D>("Stellamod/Effects/Primitives/Trails/WhispyTrail");
        public static Asset<Texture2D> CorkscrewTrail => ModContent.Request<Texture2D>("Stellamod/Effects/Primitives/Trails/CorkscrewTrail");
        public static Asset<Texture2D> FadedStreak => ModContent.Request<Texture2D>("Stellamod/Effects/Primitives/Trails/FadedStreak");
        public static Asset<Texture2D> TerraTrail => ModContent.Request<Texture2D>("Stellamod/Effects/Primitives/Trails/TerraTrail");
        public static Asset<Texture2D> DNATrail => ModContent.Request<Texture2D>("Stellamod/Effects/Primitives/Trails/DNAHelixTrail");
        public static Asset<Texture2D> SpikyTrail1 => ModContent.Request<Texture2D>("Stellamod/Effects/Primitives/Trails/SpikyTrail1");
        public static Asset<Texture2D> SpikyTrail2 => ModContent.Request<Texture2D>("Stellamod/Effects/Primitives/Trails/SpikyTrail2");
        public static Asset<Texture2D> LightningTrail => ModContent.Request<Texture2D>("Stellamod/Effects/Primitives/Trails/LightningTrail");



        public static Asset<Texture2D> CrystalNoise => ModContent.Request<Texture2D>("Stellamod/Effects/Masks/Noise/Crystals");


    }
}
