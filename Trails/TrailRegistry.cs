using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace Stellamod.Trails
{
    static class TrailRegistry
    {
        private static string BasePath => "Stellamod/Effects/Primitives/Trails/";
        public static MiscShaderData LaserShader => GameShaders.Misc["Stellamod:LaserShader"];

        public static Asset<Texture2D> GlowTrail =>
            ModContent.Request<Texture2D>($"{BasePath}GlowTrail");
        public static Asset<Texture2D> GlowTrailNoBlack =>
            ModContent.Request<Texture2D>($"{BasePath}GlowTrailNoBlack");

        public static Asset<Texture2D> GlowTrailNoBlackOutline =>
            ModContent.Request<Texture2D>($"{BasePath}GlowTrailNoBlackOutline");
        public static Asset<Texture2D> IceTrail => ModContent.Request<Texture2D>(BasePath + "IceTrail");

        public static Asset<Texture2D> IceTrailFlat => ModContent.Request<Texture2D>(BasePath + "IceTrailFlat");
        public static Asset<Texture2D> IceTrailSpiked => ModContent.Request<Texture2D>(BasePath + "IceTrailSpiked");

        public static Asset<Texture2D> QuickClouds => ModContent.Request<Texture2D>(BasePath + "QuickClouds");
        public static Asset<Texture2D> NoiseTextureLeaves => ModContent.Request<Texture2D>(BasePath + "Leaves");
        public static Asset<Texture2D> CloudsSmall => ModContent.Request<Texture2D>(BasePath + "SmallClouds");
        public static Asset<Texture2D> DottedTrail => ModContent.Request<Texture2D>(BasePath + "DottedTrail");
        public static Asset<Texture2D> DottedTrailOutline => ModContent.Request<Texture2D>(BasePath + "DottedTrailOutline");
        public static Asset<Texture2D> DreadTrail => ModContent.Request<Texture2D>("Stellamod/Effects/Primitives/Trails/DreadTrail");
        public static Asset<Texture2D> Clouds3 => ModContent.Request<Texture2D>("Stellamod/Effects/Primitives/Trails/Clouds3");
        public static Asset<Texture2D> BeamTrail => ModContent.Request<Texture2D>("Stellamod/Trails/Beam");
        public static Asset<Texture2D> BeamTrail2 => ModContent.Request<Texture2D>("Stellamod/Trails/BeamTrail");
        public static Asset<Texture2D> BulbTrail => ModContent.Request<Texture2D>("Stellamod/Trails/BulbTrail");

        public static Asset<Texture2D> Dashtrail => ModContent.Request<Texture2D>("Stellamod/Effects/Primitives/Trails/DashTrail");

        public static Asset<Texture2D> SlicingTrail => ModContent.Request<Texture2D>("Stellamod/Trails/Slice");

        public static Asset<Texture2D> STARTRAIL => ModContent.Request<Texture2D>("Stellamod/Trails/BeamTrail");
        public static Asset<Texture2D> STARTRAIL2 => ModContent.Request<Texture2D>("Stellamod/Trails/Beamlight");
        public static MiscShaderData GenericLaserVertexShader => GameShaders.Misc["VampKnives:GenericLaserShader"];
        public static MiscShaderData LightBeamVertexShader => GameShaders.Misc["VampKnives:LightBeamVertexShader"];
        
        public static MiscShaderData FireVertexShader => GameShaders.Misc["VampKnives:Fire"];
        public static MiscShaderData FireWhiteVertexShader => GameShaders.Misc["VampKnives:FireWhite"];


        public static Asset<Texture2D> StarTrail => ModContent.Request<Texture2D>("Stellamod/Effects/Primitives/Trails/StarTrail");
        public static Asset<Texture2D> StringTrail => ModContent.Request<Texture2D>("Stellamod/Effects/Primitives/Trails/StringTrail");
        public static Asset<Texture2D> SmallWhispyTrail => ModContent.Request<Texture2D>("Stellamod/Effects/Primitives/Trails/SmallWhispyTrail");
        public static Asset<Texture2D> WaveTrail => ModContent.Request<Texture2D>("Stellamod/Effects/Primitives/Trails/WaveTrail");
        public static Asset<Texture2D> WaterTrail => ModContent.Request<Texture2D>("Stellamod/Effects/Primitives/Trails/WaterTrail");

        public static Asset<Texture2D> CrystalTrail => ModContent.Request<Texture2D>("Stellamod/Effects/Primitives/Trails/CrystalTrail");
        public static Asset<Texture2D> VortexTrail => ModContent.Request<Texture2D>("Stellamod/Effects/Primitives/Trails/VortexTrail");
        public static Asset<Texture2D> WhispyTrail => ModContent.Request<Texture2D>("Stellamod/Effects/Primitives/Trails/WhispyTrail");
        public static Asset<Texture2D> CorkscrewTrail => ModContent.Request<Texture2D>("Stellamod/Effects/Primitives/Trails/CorkscrewTrail");
        public static Asset<Texture2D> TwistingTrail => ModContent.Request<Texture2D>("Stellamod/Effects/Primitives/Trails/TwistingTrail");
        public static Asset<Texture2D> TwistingTrailSmall => ModContent.Request<Texture2D>("Stellamod/Effects/Primitives/Trails/TwistingTrailSmall");
        public static Asset<Texture2D> FadedStreak => ModContent.Request<Texture2D>("Stellamod/Effects/Primitives/Trails/FadedStreak");
        public static Asset<Texture2D> TerraTrail => ModContent.Request<Texture2D>("Stellamod/Effects/Primitives/Trails/TerraTrail");
        public static Asset<Texture2D> DNATrail => ModContent.Request<Texture2D>("Stellamod/Effects/Primitives/Trails/DNAHelixTrail");
        public static Asset<Texture2D> SpikyTrail1 => ModContent.Request<Texture2D>("Stellamod/Effects/Primitives/Trails/SpikyTrail1");
        public static Asset<Texture2D> SpikyTrail2 => ModContent.Request<Texture2D>("Stellamod/Effects/Primitives/Trails/SpikyTrail2");
        public static Asset<Texture2D> LightningTrail => ModContent.Request<Texture2D>("Stellamod/Effects/Primitives/Trails/LightningTrail");
        public static Asset<Texture2D> SimpleTrail => ModContent.Request<Texture2D>("Stellamod/Effects/Primitives/Trails/SimpleTrail");
        public static Asset<Texture2D> SmoothTrailInverted => ModContent.Request<Texture2D>("Stellamod/Effects/Primitives/Trails/SmoothTrailInverted");


        public static Asset<Texture2D> CrystalNoise => ModContent.Request<Texture2D>("Stellamod/Effects/Masks/Noise/Crystals");
        public static Asset<Texture2D> CausticTrail => ModContent.Request<Texture2D>("Stellamod/Effects/Primitives/Trails/CausticTrail");


        public static Asset<Texture2D> LightningTrail2 =>
           ModContent.Request<Texture2D>($"Stellamod/Effects/Primitives/Trails/LightningTrail2");

        public static Asset<Texture2D> LightningTrail2Outline =>
           ModContent.Request<Texture2D>($"Stellamod/Effects/Primitives/Trails/LightningTrail2Outline");

        public static Asset<Texture2D> LightningTrail3 =>
           ModContent.Request<Texture2D>($"Stellamod/Effects/Primitives/Trails/LightningTrail3");

        public static Asset<Texture2D> LoveTrail =>
   ModContent.Request<Texture2D>($"Stellamod/Effects/Primitives/Trails/LoveTrail");


    }
}
