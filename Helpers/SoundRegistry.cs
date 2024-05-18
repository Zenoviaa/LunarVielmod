using Terraria.Audio;

namespace Stellamod.Helpers
{
    internal static class SoundRegistry
    {
        private static string Path => "Stellamod/Assets/Sounds/";
        public static SoundStyle JugglerHit => new SoundStyle($"{Path}JugglerHit");



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
        public static SoundStyle Niivi_HeavyBreathing1 => new SoundStyle($"{Path}NiiviBreathing1");
        public static SoundStyle Niivi_HeavyBreathing2 => new SoundStyle($"{Path}NiiviBreathing2");
        public static SoundStyle Niivi_WingFlap => new SoundStyle($"{Path}NiiviWingFlap");
    }
}
