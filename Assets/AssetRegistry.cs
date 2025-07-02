using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.Audio;
using Terraria.ModLoader;

namespace Stellamod.Assets
{
    public static class AssetRegistry
    {
        public static class Textures
        {
            public static class Trails
            {
                public static Asset<Texture2D> BasicSlash_Wide1 = ModContent.Request<Texture2D>("Stellamod/Assets/Textures/Trails/BasicSlash_Wide1");
                public static Asset<Texture2D> BasicSlash_Wide2 = ModContent.Request<Texture2D>("Stellamod/Assets/Textures/Trails/BasicSlash_Wide2");
                public static Asset<Texture2D> BasicSlash_Wide3 = ModContent.Request<Texture2D>("Stellamod/Assets/Textures/Trails/BasicSlash_Wide3");
                public static Asset<Texture2D> BasicSlash_Wide4 = ModContent.Request<Texture2D>("Stellamod/Assets/Textures/Trails/BasicSlash_Wide4");
            }
        }

        public static class Sounds
        {
            private static string Path => "Stellamod/Assets/Sounds/";
            public static class Melee
            {
                public static SoundStyle SpearHit1 = new SoundStyle($"{Path}Melee/SpearHit1");
                public static SoundStyle SwordSpin1 = new SoundStyle($"{Path}Melee/SwordSpin1");
                public static SoundStyle HammerHit1 = new SoundStyle($"{Path}Melee/HammerHit1");
                public static SoundStyle HammerHit2 = new SoundStyle($"{Path}Melee/HammerHit2");

                public static SoundStyle HammerSmash1 = new SoundStyle($"{Path}Melee/HammerSmash1");
                public static SoundStyle HammerSmash2 = new SoundStyle($"{Path}Melee/HammerSmash2");
                public static SoundStyle HammerSmash3 = new SoundStyle($"{Path}Melee/HammerSmash3");
                public static SoundStyle HammerSmashLightning1 = new SoundStyle($"{Path}Melee/HammerSmashLightning1");

                public static SoundStyle HeavySwordSlash1 = new SoundStyle($"{Path}Melee/HeavySwordSlash1");
                public static SoundStyle HeavySwordSlash2 = new SoundStyle($"{Path}Melee/HeavySwordSlash2");

                public static SoundStyle NormalSwordHit1 = new SoundStyle($"{Path}Melee/NormalSwordHit1");
                public static SoundStyle NormalSwordSlash1 = new SoundStyle($"{Path}Melee/NormalSwordSlash1");
                public static SoundStyle NormalSwordSlash2 = new SoundStyle($"{Path}Melee/NormalSwordSlash2");
            }
        }
    }
}
