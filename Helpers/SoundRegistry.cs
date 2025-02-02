﻿using Terraria.Audio;

namespace Stellamod.Helpers
{
    internal static class SoundRegistry
    {
        private static string Path => "Stellamod/Assets/Sounds/";
        public static SoundStyle CrystalHit1 => new SoundStyle($"{Path}CrystalHit1");

        public static SoundStyle FireHit1 => new SoundStyle($"{Path}Cinder");
        public static SoundStyle NSwordSlash1 => new SoundStyle($"{Path}NormalSwordSlash1");

        public static SoundStyle HeavySwordSlash1 => new SoundStyle($"{Path}HeavySwordSlash1");

        public static SoundStyle HeavySwordSlash2 => new SoundStyle($"{Path}HeavySwordSlash2");

        public static SoundStyle NSwordSlash2 => new SoundStyle($"{Path}NormalSwordSlash2");

        public static SoundStyle NSwordHit1 => new SoundStyle($"{Path}NormalSwordHit1");

        public static SoundStyle NSwordSpin1 => new SoundStyle($"{Path}SwordSpin1");

        public static SoundStyle CurveSwordSlash1 => new SoundStyle($"{Path}CurveSwordSlash1");

        public static SoundStyle SpearSlash1 => new SoundStyle($"{Path}SpearSlash1");

        public static SoundStyle SpearSlash2 => new SoundStyle($"{Path}SpearSlash2");

        public static SoundStyle SpearHit1 => new SoundStyle($"{Path}SpearHit1");

        public static SoundStyle HammerHit1 => new SoundStyle($"{Path}HammerHit1");

        public static SoundStyle HammerHit2 => new SoundStyle($"{Path}HammerHit2");

        public static SoundStyle HammerSmash1 => new SoundStyle($"{Path}HammerSmash1");

        public static SoundStyle HammerSmash2 => new SoundStyle($"{Path}HammerSmash2");

        public static SoundStyle HammerSmash3 => new SoundStyle($"{Path}HammerSmash3");

        public static SoundStyle HammerSmashLightning1 => new SoundStyle($"{Path}HammerSmashLightning1");

        public static SoundStyle MagicShockwaveExplosion => new SoundStyle($"{Path}MagicalShockwave");

        public static SoundStyle FireShockwaveExplosion => new SoundStyle($"{Path}FireShockwave");

        public static SoundStyle GaseousShockwaveExplosion => new SoundStyle($"{Path}GaseousShockwave");

        public static SoundStyle LightShockwaveExplosion => new SoundStyle($"{Path}MagicalShockwave2");

        public static SoundStyle GunShootLight1 => new SoundStyle($"{Path}Gunshot1");

        public static SoundStyle MusicChord1 => new SoundStyle($"{Path}MusicChord1");

        public static SoundStyle HeavyExplosion1 => new SoundStyle($"{Path}HeavyExplosion1");

        public static SoundStyle GunShootHeavy1 => new SoundStyle($"{Path}GunShot2");

        public static SoundStyle BowHolding => new SoundStyle($"{Path}BowHolding");

        public static SoundStyle CastTickSummon1 => new SoundStyle($"{Path}Summoner1");

        public static SoundStyle MotorcycleSlash1 => new SoundStyle($"{Path}MotoSlash");

        public static SoundStyle MotorcycleSlash2 => new SoundStyle($"{Path}MotoSlash2");

        public static SoundStyle MotorcycleDrive => new SoundStyle($"{Path}MotoMot");

        public static SoundStyle RadianceCast1 => new SoundStyle($"{Path}RadianceCast1");

        public static SoundStyle RadianceHit1 => new SoundStyle($"{Path}RadianceHit1");

        public static SoundStyle WindCast => new SoundStyle($"{Path}WindCast",
            variantSuffixesStart: 1,
            numVariants: 2);

        public static SoundStyle WindHit => new SoundStyle($"{Path}WindHit",
            variantSuffixesStart: 1,
            numVariants: 2);

        public static SoundStyle UvilisCast => new SoundStyle($"{Path}UvilisCast",
            variantSuffixesStart: 1,
            numVariants: 2);

        public static SoundStyle UvilisHit => new SoundStyle($"{Path}UvilisHit",
            variantSuffixesStart: 1,
            numVariants: 2);

        public static SoundStyle PrismaticHit => new SoundStyle($"{Path}PrismaticHit1");

        public static SoundStyle BloodletCast => new SoundStyle($"{Path}BloodletCast1");

        public static SoundStyle BloodletHit => new SoundStyle($"{Path}BloodletHit",
            variantSuffixesStart: 1,
            numVariants: 2);

        public static SoundStyle AutomationCast => new SoundStyle($"{Path}AutomationCast",
            variantSuffixesStart: 1,
            numVariants: 2);

        public static SoundStyle AutomationHit => new SoundStyle($"{Path}AutomationHit",
            variantSuffixesStart: 1,
            numVariants: 2);
        public static SoundStyle NatureCast => new SoundStyle($"{Path}NaturalCast",
            variantSuffixesStart: 1,
            numVariants: 2);

        public static SoundStyle NatureHit => new SoundStyle($"{Path}NaturalHit",
            variantSuffixesStart: 1,
            numVariants: 2);

        public static SoundStyle PrimeMagicCast => new SoundStyle($"{Path}PrimeMagicCast",
            variantSuffixesStart: 1,
            numVariants: 2);

        public static SoundStyle PrimeMagicHit => new SoundStyle($"{Path}PrimeMagicHit",
            variantSuffixesStart: 1,
            numVariants: 2);

        public static SoundStyle GuutCast => new SoundStyle($"{Path}GuutCast",
            variantSuffixesStart: 1,
            numVariants: 2);

        public static SoundStyle GuutHit => new SoundStyle($"{Path}GuutHit",
            variantSuffixesStart: 1,
            numVariants: 2);


        public static SoundStyle DeeyaCast => new SoundStyle($"{Path}DeeyaCast",
            variantSuffixesStart: 1,
            numVariants: 2);

        public static SoundStyle DeeyaHit => new SoundStyle($"{Path}DeeyaHit",
            variantSuffixesStart: 1,
            numVariants: 2);

        public static SoundStyle HolyCast => new SoundStyle($"{Path}HolyCast",
            variantSuffixesStart: 1,
            numVariants: 2);

        public static SoundStyle HolyHit => new SoundStyle($"{Path}HolyHit",
            variantSuffixesStart: 1,
            numVariants: 2);

        public static SoundStyle BasicMagicHit => new SoundStyle($"{Path}BasicMagicHit",
            variantSuffixesStart: 1,
            numVariants: 2);

        public static SoundStyle BallSwing => new SoundStyle($"{Path}BallSwing");

        public static SoundStyle MothlightStarCast => new SoundStyle($"{Path}MothlightStarCast",
            variantSuffixesStart: 1,
            numVariants: 3);
        public static SoundStyle JugglerPong => new SoundStyle($"{Path}JugglerPong", 1, 2);
        public static SoundStyle JugglerHit => new SoundStyle($"{Path}JugglerHit");
        public static SoundStyle JugglerHitMax => new SoundStyle($"{Path}JugglerHitMax");
        public static SoundStyle BeeBuzz1 => new SoundStyle($"{Path}BeeBuzz1");
        public static SoundStyle BeeBuzz2 => new SoundStyle($"{Path}BeeBuzz2");
        public static SoundStyle BeeDeath1 => new SoundStyle($"{Path}BeeDeath1");
        public static SoundStyle BeeDeath2 => new SoundStyle($"{Path}BeeDeath2");
        public static SoundStyle StingBoom1 => new SoundStyle($"{Path}StingBomb1");
        public static SoundStyle StingBoom2 => new SoundStyle($"{Path}StingBomb2");
        public static SoundStyle FanHit1 => new SoundStyle($"{Path}FanHit1");
        public static SoundStyle FanHit2 => new SoundStyle($"{Path}FanHit2");
        public static SoundStyle OrbSmash => new SoundStyle($"{Path}OrbSmash");
        public static SoundStyle OrbSummon1 => new SoundStyle($"{Path}OrbSummon1");
        public static SoundStyle OrbSummon2 => new SoundStyle($"{Path}OrbSummon2");
        public static SoundStyle BubbleIn => new SoundStyle($"{Path}BubbleIn");
        public static SoundStyle CombusterBoom => new SoundStyle($"{Path}CombusterBoom");
        public static SoundStyle LaserChannel => new SoundStyle($"{Path}LaserChannel");
        public static SoundStyle ExplosionCrystalShard => new SoundStyle($"{Path}ExplosionCrystalShard");
        public static SoundStyle QuickHit => new SoundStyle($"{Path}QuickHit");
        public static SoundStyle IceyWind => new SoundStyle($"{Path}IceyWind");
        public static SoundStyle Lightning2 => new SoundStyle($"{Path}Lighting2");

        public static SoundStyle JellyBow => new SoundStyle($"{Path}JellyBow");
        public static SoundStyle JellyTome => new SoundStyle($"{Path}JellyTome");
        public static SoundStyle JellyStaff => new SoundStyle($"{Path}JellyStaff");
        public static SoundStyle JellyLance => new SoundStyle($"{Path}JellyLance");
        public static SoundStyle IridineRevive => new SoundStyle($"{Path}IridineRevive");
        //Niivi Sounds
        public static SoundStyle Niivi_LaserBlastReady => new SoundStyle($"{Path}DreamCharge");
        public static SoundStyle Niivi_LaserBlast1 => new SoundStyle($"{Path}NStarblast");
        public static SoundStyle Niivi_LaserBlast2 => new SoundStyle($"{Path}NStarblast2");
        public static SoundStyle Niivi_CrystalSummon => new SoundStyle($"{Path}CrystalSumms");
        public static SoundStyle Niivi_StarSummon => new SoundStyle($"{Path}Starrer");
        public static SoundStyle Niivi_StarSummon2 => new SoundStyle($"{Path}StarBounce");
        public static SoundStyle Niivi_StarringDeath => new SoundStyle($"{Path}StarringDeath");
        public static SoundStyle Niivi_PrismaticCharge => new SoundStyle($"{Path}PrimCharge");
        public static SoundStyle Niivi_BigCharge => new SoundStyle($"{Path}BiggerCharge");
        public static SoundStyle Niivi_PrimGrow1 => new SoundStyle($"{Path}PrimGrow1");
        public static SoundStyle Niivi_PrimGrow2 => new SoundStyle($"{Path}PrimGrow2");
        public static SoundStyle Niivi_Starence => new SoundStyle($"{Path}Starence");
        public static SoundStyle Niivi_Voidfield => new SoundStyle($"{Path}Voidfield");
        public static SoundStyle Niivi_Voidence => new SoundStyle($"{Path}Voidence");
        public static SoundStyle Niivi_PrimAm => new SoundStyle($"{Path}PrimAm");
        public static SoundStyle Niivi_PrimRay => new SoundStyle($"{Path}PrimRay");
        public static SoundStyle Niivi_PrimBomb => new SoundStyle($"{Path}PrimBomb");
        public static SoundStyle Niivi_HeavyBreathing1 => new SoundStyle($"{Path}NiiviHeavyBreathing1");
        public static SoundStyle Niivi_HeavyBreathing2 => new SoundStyle($"{Path}NiiviHeavyBreathing2");
        public static SoundStyle Niivi_WingFlap => new SoundStyle($"{Path}NiiviWingFlap");
        public static SoundStyle Niivi_Tired => new SoundStyle($"{Path}NiiviTired");
        public static SoundStyle Niivi_Death => new SoundStyle($"{Path}NiiviDeath");
        public static SoundStyle VeilCast => new SoundStyle($"{Path}VeilCast",
            variantSuffixesStart: 1,
            numVariants: 2);

        public static SoundStyle VeilHit => new SoundStyle($"{Path}VeilHit",
            variantSuffixesStart: 1,
            numVariants: 2);


        public static SoundStyle RoyalMagicCast => new SoundStyle($"{Path}RoyalMagicCast",
            variantSuffixesStart: 1,
            numVariants: 2);

        public static SoundStyle RoyalMagicHit => new SoundStyle($"{Path}RoyalMagicHit",
            variantSuffixesStart: 1,
            numVariants: 2);
    }
}
