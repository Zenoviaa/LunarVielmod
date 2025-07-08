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
            public static Asset<Texture2D> EmptyBigTexture = ModContent.Request<Texture2D>("Stellamod/Assets/Textures/EmptyBigTexture");
            public static class Noise
            {
                public static Asset<Texture2D> BasicGlow = ModContent.Request<Texture2D>("Stellamod/Assets/Textures/Noise/BasicGlow");
                public static Asset<Texture2D> Clouds3 = ModContent.Request<Texture2D>("Stellamod/Assets/Textures/Noise/Clouds3");
                public static Asset<Texture2D> CloudsSmall = ModContent.Request<Texture2D>("Stellamod/Assets/Textures/Noise/SmallClouds");
            }
            public static class Trails
            {
                public static Asset<Texture2D> BasicSlash_Wide1 = ModContent.Request<Texture2D>("Stellamod/Assets/Textures/Trails/BasicSlash_Wide1");
                public static Asset<Texture2D> BasicSlash_Wide2 = ModContent.Request<Texture2D>("Stellamod/Assets/Textures/Trails/BasicSlash_Wide2");
                public static Asset<Texture2D> BasicSlash_Wide3 = ModContent.Request<Texture2D>("Stellamod/Assets/Textures/Trails/BasicSlash_Wide3");
                public static Asset<Texture2D> BasicSlash_Wide4 = ModContent.Request<Texture2D>("Stellamod/Assets/Textures/Trails/BasicSlash_Wide4");

                public static Asset<Texture2D> StringySlash1 = ModContent.Request<Texture2D>("Stellamod/Assets/Textures/Trails/StringySlash1");
                public static Asset<Texture2D> StringySlash2 = ModContent.Request<Texture2D>("Stellamod/Assets/Textures/Trails/StringySlash2");
                public static Asset<Texture2D> StringySlash3 = ModContent.Request<Texture2D>("Stellamod/Assets/Textures/Trails/StringySlash3");
                public static Asset<Texture2D> StringySlash4 = ModContent.Request<Texture2D>("Stellamod/Assets/Textures/Trails/StringySlash4");
            }
        }

        public static class Sounds
        {
            private static string Path => "Stellamod/Assets/Sounds/";
            public static class Magic
            {
                public static SoundStyle AutomationCast1 = new SoundStyle($"{Path}Magic/AutomationCast1");
                public static SoundStyle AutomationCast2 = new SoundStyle($"{Path}Magic/AutomationCast2");
                public static SoundStyle AutomationHit1 = new SoundStyle($"{Path}Magic/AutomationHit1");
                public static SoundStyle AutomationHit2 = new SoundStyle($"{Path}Magic/AutomationHit2");

                public static SoundStyle BasicMagicHit1 = new SoundStyle($"{Path}Magic/BasicMagicHit1");
                public static SoundStyle BasicMagicHit2 = new SoundStyle($"{Path}Magic/BasicMagicHit2");

                public static SoundStyle BloodletCast1 = new SoundStyle($"{Path}Magic/BloodletCast1");
                public static SoundStyle BloodletHit1 = new SoundStyle($"{Path}Magic/BloodletHit1");
                public static SoundStyle BloodletHit2 = new SoundStyle($"{Path}Magic/BloodletHit2");

                public static SoundStyle DeeyaCast1 = new SoundStyle($"{Path}Magic/DeeyaCast1");
                public static SoundStyle DeeyaCast2 = new SoundStyle($"{Path}Magic/DeeyaCast2");
                public static SoundStyle DeeyaHit1 = new SoundStyle($"{Path}Magic/DeeyaHit1");
                public static SoundStyle DeeyaHit2 = new SoundStyle($"{Path}Magic/DeeyaHit2");

                public static SoundStyle GuutCast1 = new SoundStyle($"{Path}Magic/GuutCast1");
                public static SoundStyle GuutCast2 = new SoundStyle($"{Path}Magic/GuutCast2");
                public static SoundStyle GuutHit1 = new SoundStyle($"{Path}Magic/GuutHit1");
                public static SoundStyle GuutHit2 = new SoundStyle($"{Path}Magic/GuutHit2");

                public static SoundStyle HolyCast1 = new SoundStyle($"{Path}Magic/HolyCast1");
                public static SoundStyle HolyCast2 = new SoundStyle($"{Path}Magic/HolyCast2");
                public static SoundStyle HolyHit1 = new SoundStyle($"{Path}Magic/HolyHit1");
                public static SoundStyle HolyHit2 = new SoundStyle($"{Path}Magic/HolyHit2");

                public static SoundStyle MothlightStarCast1 = new SoundStyle($"{Path}Magic/MothlightStarCast1");
                public static SoundStyle MothlightStarCast2 = new SoundStyle($"{Path}Magic/MothlightStarCast2");
                public static SoundStyle MothlightStarCast3 = new SoundStyle($"{Path}Magic/MothlightStarCast3");

                public static SoundStyle NaturalCast1 = new SoundStyle($"{Path}Magic/NaturalCast1");
                public static SoundStyle NaturalCast2 = new SoundStyle($"{Path}Magic/NaturalCast2");
                public static SoundStyle NaturalHit1 = new SoundStyle($"{Path}Magic/NaturalHit1");
                public static SoundStyle NaturalHit2 = new SoundStyle($"{Path}Magic/NaturalHit2");

                public static SoundStyle PrimeMagicCast1 = new SoundStyle($"{Path}Magic/PrimeMagicCast1");
                public static SoundStyle PrimeMagicCast2 = new SoundStyle($"{Path}Magic/PrimeMagicCast2");
                public static SoundStyle PrimeMagicHit1 = new SoundStyle($"{Path}Magic/PrimeMagicHit1");
                public static SoundStyle PrimeMagicHit2 = new SoundStyle($"{Path}Magic/PrimeMagicHit2");

                public static SoundStyle RadiantCast1 = new SoundStyle($"{Path}Magic/RadiantCast1");
                public static SoundStyle RadianceHit1 = new SoundStyle($"{Path}Magic/RadianceHit1");

                public static SoundStyle WindCast1 = new SoundStyle($"{Path}Magic/WindCast1");
                public static SoundStyle WindCast2 = new SoundStyle($"{Path}Magic/WindCast2");
                public static SoundStyle WindHit1 = new SoundStyle($"{Path}Magic/WindHit1");
                public static SoundStyle WindHit2 = new SoundStyle($"{Path}Magic/WindHit2");
            }

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

                public static SoundStyle Safunais1 = new SoundStyle($"{Path}Melee/Safunais");
                public static SoundStyle Safunais2 = new SoundStyle($"{Path}Melee/Safunais2");
                public static SoundStyle Safunais3 = new SoundStyle($"{Path}Melee/Safunais3");

                public static SoundStyle Vinger = new SoundStyle($"{Path}Melee/Vinger");
                public static SoundStyle Vinger2 = new SoundStyle($"{Path}Melee/Vinger2");
                public static SoundStyle MorrowExp = new SoundStyle($"{Path}Melee/MorrowExp");

                public static SoundStyle Parendine = new SoundStyle($"{Path}Melee/Parendine");
                public static SoundStyle Parendine2 = new SoundStyle($"{Path}Melee/Parendine2");
            }
        }
    }
}
