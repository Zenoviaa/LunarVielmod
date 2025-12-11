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
                public static Asset<Texture2D> ShimmerWaterCaustics = ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/ShimmerWaterCaustics");
                public static Asset<Texture2D> IceWaterCaustics = ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/IceWaterCaustics");

                public static Asset<Texture2D> BasicGlow = ModContent.Request<Texture2D>("Stellamod/Assets/Textures/Noise/BasicGlow");
                public static Asset<Texture2D> Clouds3 = ModContent.Request<Texture2D>("Stellamod/Assets/Textures/Noise/Clouds3");
                public static Asset<Texture2D> CloudsSmall = ModContent.Request<Texture2D>("Stellamod/Assets/Textures/Noise/SmallClouds");
                public static Asset<Texture2D> Perlin = ModContent.Request<Texture2D>("Stellamod/Assets/Textures/Noise/PerlinNoise");
                public static Asset<Texture2D> CandleFlame = ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/CandleFlame");
                public static Asset<Texture2D> CartoonyStar = ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/CartoonyStar");
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

                public static Asset<Texture2D> BulbyTrail = ModContent.Request<Texture2D>("Stellamod/Assets/Textures/Trails/BulbyTrail");
            }
        }

        public static class Sounds
        {
            private static string Path => "Stellamod/Assets/Sounds/";
            public static class Illuria
            {
                public static SoundStyle IceImpact1 = new SoundStyle($"{Path}Illuria/IceImpact1");
                public static SoundStyle IceImpact2 = new SoundStyle($"{Path}Illuria/IceImpact2");
                public static SoundStyle MagicalIce = new SoundStyle($"{Path}Illuria/MagicalIce");
            }

            public static class SteamPunking
            {
                public static SoundStyle MechSteaming = new SoundStyle($"{Path}SteamPunking/MechSteaming");
                public static SoundStyle MechSupercharge = new SoundStyle($"{Path}SteamPunking/MechSupercharge");
                public static SoundStyle MechSawRevLoop = new SoundStyle($"{Path}SteamPunking/MechSawRevLoop");
                public static SoundStyle MechSawRevUp = new SoundStyle($"{Path}SteamPunking/MechSawRevUp");
                public static SoundStyle MechShoot1 = new SoundStyle($"{Path}SteamPunking/MechShoot1");
                public static SoundStyle MechSaw = new SoundStyle($"{Path}SteamPunking/MechSaw");
                public static SoundStyle MechMove = new SoundStyle($"{Path}SteamPunking/MechMove");
                public static SoundStyle MechTurn = new SoundStyle($"{Path}SteamPunking/MechTurn");
            }

            public static class SanguineSingularity
            {
                public static SoundStyle ChangeTheWorldo = new SoundStyle($"{Path}SanguineSingularity/ChangeTheWorldo");
                public static SoundStyle SanguineCyst = new SoundStyle($"{Path}SanguineSingularity/SanguineCyst");
                public static SoundStyle BloodyExplosion = new SoundStyle($"{Path}SanguineSingularity/BloodyExplosion");
                public static SoundStyle BloodyGrab = new SoundStyle($"{Path}SanguineSingularity/BloodyGrab");
                public static SoundStyle BloodyHit = new SoundStyle($"{Path}SanguineSingularity/BloodyHit");
                public static SoundStyle BloodyDeath = new SoundStyle($"{Path}SanguineSingularity/BloodyDeath");
                public static SoundStyle SanguineCry2 = new SoundStyle($"{Path}SanguineSingularity/SanguineCry2");
                public static SoundStyle SanguineCry = new SoundStyle($"{Path}SanguineSingularity/SanguineCry");
                public static SoundStyle SanguineLaugh = new SoundStyle($"{Path}SanguineSingularity/SanguineLaugh");
                public static SoundStyle SanguineCharge = new SoundStyle($"{Path}SanguineSingularity/SanguineCharge");
                public static SoundStyle SanguineSpawn = new SoundStyle($"{Path}SanguineSingularity/SanguineSpawn");
                public static SoundStyle SanguineBurst = new SoundStyle($"{Path}SanguineSingularity/SanguineBurst");
                public static SoundStyle SanguineBurstReady = new SoundStyle($"{Path}SanguineSingularity/SanguineBurstReady");
                public static SoundStyle SanguinePreBurst = new SoundStyle($"{Path}SanguineSingularity/SanguinePreBurst");
                public static SoundStyle SanguineDeath = new SoundStyle($"{Path}SanguineSingularity/SanguineDeath");
                public static SoundStyle SanguineDash = new SoundStyle($"{Path}SanguineSingularity/SanguineDash");
            }
            public static class STARBOMBER
            {
                public static SoundStyle Ommove5 = new SoundStyle($"{Path}STARBOMBER/Ommove5");
                public static SoundStyle Ommove4 = new SoundStyle($"{Path}STARBOMBER/Ommove4");
                public static SoundStyle Ommove3 = new SoundStyle($"{Path}STARBOMBER/Ommove3");
                public static SoundStyle Ommove2 = new SoundStyle($"{Path}STARBOMBER/Ommove2");
                public static SoundStyle Ommove1 = new SoundStyle($"{Path}STARBOMBER/Ommove1");
                public static SoundStyle Heavyspin = new SoundStyle($"{Path}STARBOMBER/Heavyspin");
                public static SoundStyle HeavyCrush = new SoundStyle($"{Path}STARBOMBER/HeavyCrush");
                public static SoundStyle STARRAILGUN = new SoundStyle($"{Path}STARBOMBER/STARRAILGUN");
                public static SoundStyle STARWALK = new SoundStyle($"{Path}STARBOMBER/STARWALK");
                public static SoundStyle STARSTEP = new SoundStyle($"{Path}STARBOMBER/STARSTEP");
            }
            public static class Minerva
            {
                public static SoundStyle MinervaVoice1 = new SoundStyle($"{Path}Minerva/MinervaVoice1");
                public static SoundStyle MinervaVoice2 = new SoundStyle($"{Path}Minerva/MinervaVoice2");
                public static SoundStyle MinervaVoice3 = new SoundStyle($"{Path}Minerva/MinervaVoice3");
                public static SoundStyle MinervaLaugh = new SoundStyle($"{Path}Minerva/MinervaLaugh");
                public static SoundStyle MinervaSpin = new SoundStyle($"{Path}Minerva/MinervaSpin");
                public static SoundStyle MinervaDeath = new SoundStyle($"{Path}Minerva/MinervaDeath");
                public static SoundStyle Stunned = new SoundStyle($"{Path}Minerva/Stunned");
            }

            public static class Bishinine
            {
                public static SoundStyle Bigballchargepart = new SoundStyle($"{Path}Bishinine/Bigballchargepart");
                public static SoundStyle BigBallready = new SoundStyle($"{Path}Bishinine/BigBallready");
                public static SoundStyle BishinineChargeBell = new SoundStyle($"{Path}Bishinine/BishinineChargeBell");
                public static SoundStyle BishinineCometfallbegin = new SoundStyle($"{Path}Bishinine/BishinineCometfallbegin");
                public static SoundStyle BishinineFastfall = new SoundStyle($"{Path}Bishinine/BishinineFastfall");
                public static SoundStyle BigBellGroundhit = new SoundStyle($"{Path}Bishinine/BigBellGroundhit");
                public static SoundStyle BishinineBellSmash = new SoundStyle($"{Path}Bishinine/BishinineBellSmash");
                public static SoundStyle BishinineSound1 = new SoundStyle($"{Path}Bishinine/BishinineSound1");
                public static SoundStyle BishinineSound2 = new SoundStyle($"{Path}Bishinine/BishinineSound2");
                public static SoundStyle BishinineLaugh = new SoundStyle($"{Path}Bishinine/Bishininelaugh");
                public static SoundStyle FallingBell = new SoundStyle($"{Path}Bishinine/FallingBell");
                public static SoundStyle BellHit1 = new SoundStyle($"{Path}Bishinine/BellHit1");
                public static SoundStyle BellHit2 = new SoundStyle($"{Path}Bishinine/BellHit2");
                public static SoundStyle Comet1 = new SoundStyle($"{Path}Bishinine/Comet1");
                public static SoundStyle Comet2 = new SoundStyle($"{Path}Bishinine/Comet2");
            }
            public static class Nature
            {
                public static SoundStyle LeafRustle1 = new SoundStyle($"{Path}Nature/LeafRustle1");
                public static SoundStyle LeafRustle2 = new SoundStyle($"{Path}Nature/LeafRustle2");
            }
            public static class Stars
            {
                public static SoundStyle Starsingle1 = new SoundStyle($"{Path}Stars/Starsingle1");
                public static SoundStyle Starsingle2 = new SoundStyle($"{Path}Stars/Starsingle2");
                public static SoundStyle Starsingle3 = new SoundStyle($"{Path}Stars/Starsingle3");
                public static SoundStyle Starsingle4 = new SoundStyle($"{Path}Stars/Starsingle4");
                public static SoundStyle Starsingle5 = new SoundStyle($"{Path}Stars/Starsingle5");
            }
            public static class Bow
            {
                public static SoundStyle Aim = new SoundStyle($"{Path}Bow/Aim");
                public static SoundStyle CrossbowPull = new SoundStyle($"{Path}Bow/CrossbowPull");
            }
            public static class Gun
            {
                public static SoundStyle ShockLineShock = new SoundStyle($"{Path}Gun/ShockLineShock");
                public static SoundStyle ShockLineShoot = new SoundStyle($"{Path}Gun/ShockLineShoot");
                public static SoundStyle GrappleWindWhoosh = new SoundStyle($"{Path}Gun/GrappleWindWhoosh");
                public static SoundStyle GrappleWindUpStart = new SoundStyle($"{Path}Gun/GrappleWindUpStart");
                public static SoundStyle GrappleShoot = new SoundStyle($"{Path}Gun/GrappleShoot");
                public static SoundStyle GrappleCharge = new SoundStyle($"{Path}Gun/GrappleCharge");
                public static SoundStyle GunJam = new SoundStyle($"{Path}Gun/GunJam");
                public static SoundStyle GunReload = new SoundStyle($"{Path}Gun/GunReload");
                public static SoundStyle GunToss = new SoundStyle($"{Path}Gun/GunToss");
            }

            public static class Ravager
            {
                public static SoundStyle RavagerAngry = new SoundStyle($"{Path}Ravager/RavagerAngry");
                public static SoundStyle RavagerRoar = new SoundStyle($"{Path}Ravager/RavagerRoar");
                public static SoundStyle RavagerRockSlide1 = new SoundStyle($"{Path}Ravager/RavagerRockSlide1");
                public static SoundStyle RavagerRockSlide2 = new SoundStyle($"{Path}Ravager/RavagerRockSlide2");
                public static SoundStyle RavagerRockSmash1 = new SoundStyle($"{Path}Ravager/RavagerRockSmash1");
                public static SoundStyle RavagerRockSmash2 = new SoundStyle($"{Path}Ravager/RavagerRockSmash2");
                public static SoundStyle RavagerRockSummon1 = new SoundStyle($"{Path}Ravager/RavagerRockSummon1");
                public static SoundStyle RavagerRockSummon2 = new SoundStyle($"{Path}Ravager/RavagerRockSummon2");
                public static SoundStyle RavagerRockSummon3 = new SoundStyle($"{Path}Ravager/RavagerRockSummon3");
            }
            public static class Jiitas
            {
                public static SoundStyle JiitasGunShot = new SoundStyle($"{Path}Jiitas/JiitasGunShot");
                public static SoundStyle JiitasKnifeThrow = new SoundStyle($"{Path}Jiitas/JiitasKnifeThrow");
                public static SoundStyle JiitasKnifeSlash = new SoundStyle($"{Path}Jiitas/JiitasKnifeSlash");
                public static SoundStyle JiitasBombThrow = new SoundStyle($"{Path}Jiitas/JiitasBombThrow");
                public static SoundStyle JiitasBombFuse = new SoundStyle($"{Path}Jiitas/JiitasBombFuse");
                public static SoundStyle JiitasLaugh = new SoundStyle($"{Path}Jiitas/JiitasLaugh");
                public static SoundStyle JiitasSit = new SoundStyle($"{Path}Jiitas/JiitasSit");
                public static SoundStyle JiitasSummon = new SoundStyle($"{Path}Jiitas/JiitasSummon");
                public static SoundStyle JiitasLightSpin = new SoundStyle($"{Path}Jiitas/JiitasLightSpin");
                public static SoundStyle JiitasReload = new SoundStyle($"{Path}Jiitas/JiitasReload");
                public static SoundStyle JiitasSadWah = new SoundStyle($"{Path}Jiitas/JiitasSadWah");
            }

            public static class MagicWand
            {
                public static SoundStyle EnchantmentGrab = new SoundStyle($"{Path}MagicWand/EnchantmentGrab");
                public static SoundStyle EnchantmentPlace = new SoundStyle($"{Path}MagicWand/EnchantmentPlace");
                public static SoundStyle BasicCharge = new SoundStyle($"{Path}MagicWand/BasicCharge");
                public static SoundStyle BloodletCharge = new SoundStyle($"{Path}MagicWand/BloodletCharge");
                public static SoundStyle BloodletChargeShot = new SoundStyle($"{Path}MagicWand/BloodletChargeShot");
                public static SoundStyle DeeyaCharge = new SoundStyle($"{Path}MagicWand/DeeyaCharge");
                public static SoundStyle DeeyaChargeShot = new SoundStyle($"{Path}MagicWand/DeeyaChargeShot");
                public static SoundStyle FireCharge = new SoundStyle($"{Path}MagicWand/FireCharge");
                public static SoundStyle FireChargeShot = new SoundStyle($"{Path}MagicWand/FireChargeShot");
                public static SoundStyle GuutCharge = new SoundStyle($"{Path}MagicWand/GuutCharge");
                public static SoundStyle GuutChargeShot = new SoundStyle($"{Path}MagicWand/GuutChargeShot");
                public static SoundStyle HexCharge = new SoundStyle($"{Path}MagicWand/HexCharge");
                public static SoundStyle HexChargeShot = new SoundStyle($"{Path}MagicWand/HexChargeShot");
                public static SoundStyle NatureCharge = new SoundStyle($"{Path}MagicWand/NatureCharge");
                public static SoundStyle NatureChargeShot = new SoundStyle($"{Path}MagicWand/NatureChargeShot");
                public static SoundStyle PhantasmalCharge = new SoundStyle($"{Path}MagicWand/PhantasmalCharge");
                public static SoundStyle PhantasmalChargeShot = new SoundStyle($"{Path}MagicWand/PhantasmalChargeShot");
                public static SoundStyle UvilisCharge = new SoundStyle($"{Path}MagicWand/UvilisCharge");
                public static SoundStyle UvilisChargeShot = new SoundStyle($"{Path}MagicWand/UvilisChargeShot");
            }
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

                public static SoundStyle RadiantCast1 = new SoundStyle($"{Path}Magic/RadianceCast1");
                public static SoundStyle RadianceHit1 = new SoundStyle($"{Path}Magic/RadianceHit1");

                public static SoundStyle WindCast1 = new SoundStyle($"{Path}Magic/WindCast1");
                public static SoundStyle WindCast2 = new SoundStyle($"{Path}Magic/WindCast2");
                public static SoundStyle WindHit1 = new SoundStyle($"{Path}Magic/WindHit1");
                public static SoundStyle WindHit2 = new SoundStyle($"{Path}Magic/WindHit2");

                public static SoundStyle VineWrap = new SoundStyle($"{Path}Magic/VineWrap");
            }

            public static class Melee
            {
                public static SoundStyle ScytheBigSlash = new SoundStyle($"{Path}Melee/ScytheBigSlash");
                public static SoundStyle ScytheBladeSlash1 = new SoundStyle($"{Path}Melee/ScytheBladeSlash1");
                public static SoundStyle ScytheHit1 = new SoundStyle($"{Path}Melee/ScytheHit1");
                public static SoundStyle ScytheHit2 = new SoundStyle($"{Path}Melee/ScytheHit2");
                public static SoundStyle ScytheHit3 = new SoundStyle($"{Path}Melee/ScytheHit3");
                public static SoundStyle ScythePull = new SoundStyle($"{Path}Melee/ScythePull");
                public static SoundStyle ScytheWindSlash1 = new SoundStyle($"{Path}Melee/ScytheWindSlash1");
                public static SoundStyle ScytheWindSlash2 = new SoundStyle($"{Path}Melee/ScytheWindSlash2");
                public static SoundStyle ScytheWindSlash3 = new SoundStyle($"{Path}Melee/ScytheWindSlash3");
                public static SoundStyle ScytheWindSlash4 = new SoundStyle($"{Path}Melee/ScytheWindSlash4");


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


                public static SoundStyle CrystalHit1 = new SoundStyle($"{Path}Melee/CrystalHit1");

                public static SoundStyle SpearSlash1 = new SoundStyle($"{Path}Melee/SpearSlash1");
                public static SoundStyle SpearSlash2 = new SoundStyle($"{Path}Melee/SpearSlash2");
            }
        }
    }
}
