using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.ModLoader;

// This is in the base namespace for convience with how often textures are used with newer visuals.
// Please maintain Type -> Alphabetical order when adding new textures to the registry.
namespace Stellamod.Helpers.Separate
{
    public static class StellasTextureRegistry
    {
       

        public static Asset<Texture2D> BloomLine => ModContent.Request<Texture2D>("Stellamod/Textures/BloomLine");

        public static Asset<Texture2D> BloomLineSmall => ModContent.Request<Texture2D>("Stellamod/Textures/BloomLineSmall");

        public static Asset<Texture2D> Invisible => ModContent.Request<Texture2D>("Stellamod/Textures/Invisible");

     
        public static Asset<Texture2D> LaserCircle => ModContent.Request<Texture2D>("Stellamod/Textures/LaserCircle");

        public static Asset<Texture2D> Line => ModContent.Request<Texture2D>("IStellamod/Textures/Line");

        public static string InvisPath => "IStellamod/Textures/Invisible";
    }
}
