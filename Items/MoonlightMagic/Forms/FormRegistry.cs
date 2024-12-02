using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.ModLoader;

namespace Stellamod.Items.MoonlightMagic.Forms
{
    internal static class FormRegistry
    {
        public static string AssetDirectory => "Stellamod/Items/MoonlightMagic/Forms/";
        public static Asset<Texture2D> Circle => ModContent.Request<Texture2D>(AssetDirectory + "Circle");
        public static Asset<Texture2D> FourPointedStar => ModContent.Request<Texture2D>(AssetDirectory + "FourPointedStar");

        public static Asset<Texture2D> FourPointedDiamond => ModContent.Request<Texture2D>(AssetDirectory + "FourPointedDiamond");

        public static Asset<Texture2D> Hat => ModContent.Request<Texture2D>(AssetDirectory + "Hat");
        public static Asset<Texture2D> Skull => ModContent.Request<Texture2D>(AssetDirectory + "Skull");
        public static Asset<Texture2D> Bow => ModContent.Request<Texture2D>(AssetDirectory + "Bow");
        public static Asset<Texture2D> Lantern => ModContent.Request<Texture2D>(AssetDirectory + "Lantern");
        public static Asset<Texture2D> Squid => ModContent.Request<Texture2D>(AssetDirectory + "Squid");
        public static Asset<Texture2D> BowingArrow => ModContent.Request<Texture2D>(AssetDirectory + "BowingArrow");
        public static Asset<Texture2D> FourPointedClover => ModContent.Request<Texture2D>(AssetDirectory + "FourPointedClover");

        public static Asset<Texture2D> Spine => ModContent.Request<Texture2D>(AssetDirectory + "Spine");
        public static Asset<Texture2D> Triangle => ModContent.Request<Texture2D>(AssetDirectory + "Triangle");
        public static Asset<Texture2D> Snowglobe => ModContent.Request<Texture2D>(AssetDirectory + "Snowglobe");
        public static Asset<Texture2D> Vase => ModContent.Request<Texture2D>(AssetDirectory + "Vase");
        public static Asset<Texture2D> Octopus => ModContent.Request<Texture2D>(AssetDirectory + "Octopus");
        public static Asset<Texture2D> Pickaxe => ModContent.Request<Texture2D>(AssetDirectory + "Pickaxe");
        public static Asset<Texture2D> Alien => ModContent.Request<Texture2D>(AssetDirectory + "Alien");
        public static Asset<Texture2D> Fairy => ModContent.Request<Texture2D>(AssetDirectory + "Fairy");

        public static Asset<Texture2D> Snake => ModContent.Request<Texture2D>(AssetDirectory + "Snake");
        public static Asset<Texture2D> Runic => ModContent.Request<Texture2D>(AssetDirectory + "Runic");
        public static Asset<Texture2D> Swirl => ModContent.Request<Texture2D>(AssetDirectory + "Swirl");
        public static Asset<Texture2D> DoubleArrow => ModContent.Request<Texture2D>(AssetDirectory + "DoubleArrow");
        public static Asset<Texture2D> Arrow => ModContent.Request<Texture2D>(AssetDirectory + "Arrow");
        public static Asset<Texture2D> Hammer => ModContent.Request<Texture2D>(AssetDirectory + "Hammer");
        public static Asset<Texture2D> SmallKnife => ModContent.Request<Texture2D>(AssetDirectory + "SmallKnife");
        public static Asset<Texture2D> Sunbug => ModContent.Request<Texture2D>(AssetDirectory + "Sunbug");
        public static Asset<Texture2D> Aztec => ModContent.Request<Texture2D>(AssetDirectory + "Aztec");

        public static Asset<Texture2D> Coin => ModContent.Request<Texture2D>(AssetDirectory + "Coin");
        public static Asset<Texture2D> Crescent => ModContent.Request<Texture2D>(AssetDirectory + "Crescent");
        public static Asset<Texture2D> Quadle => ModContent.Request<Texture2D>(AssetDirectory + "Quadle");
        public static Asset<Texture2D> Box => ModContent.Request<Texture2D>(AssetDirectory + "Box");
        public static Asset<Texture2D> Smile => ModContent.Request<Texture2D>(AssetDirectory + "Smile");
        public static Asset<Texture2D> Projector => ModContent.Request<Texture2D>(AssetDirectory + "Projector");
        public static Asset<Texture2D> Tickler => ModContent.Request<Texture2D>(AssetDirectory + "Tickler");

        public static Asset<Texture2D> Ship => ModContent.Request<Texture2D>(AssetDirectory + "Ship");

        public static Asset<Texture2D> Lamp => ModContent.Request<Texture2D>(AssetDirectory + "Lamp");
    }
}
