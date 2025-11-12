using Terraria.ID;

namespace Stellamod.Core.LunarLightingSystem
{

    public static class TorchLightingHelper
    {
        public static int TorchItemToTorchID(int itemID)
        {
            switch (itemID)
            {
                default:
                    return -1;
                case ItemID.Torch:
                    return 0;
                case ItemID.BlueTorch:
                    return 1;
                case ItemID.RedTorch:
                    return 2;
                case ItemID.GreenTorch:
                    return 3;
                case ItemID.PurpleTorch:
                    return 4;
                case ItemID.WhiteTorch:
                    return 5;
                case ItemID.YellowTorch:
                    return 6;
                case ItemID.DemonTorch:
                    return 7;
                case ItemID.CursedTorch:
                    return 8;
                case ItemID.IceTorch:
                    return 9;
                case ItemID.OrangeTorch:
                    return 10;
                case ItemID.IchorTorch:
                    return 11;
                case ItemID.UltrabrightTorch:
                    return 12;
                case ItemID.BoneTorch:
                    return 13;
                case ItemID.RainbowTorch:
                    return 14;
                case ItemID.PinkTorch:
                    return 15;
                case ItemID.DesertTorch:
                    return 16;
                case ItemID.CoralTorch:
                    return 17;
                case ItemID.CorruptTorch:
                    return 18;
                case ItemID.CrimsonTorch:
                    return 19;
                case ItemID.HallowedTorch:
                    return 20;
                case ItemID.JungleTorch:
                    return 21;
                case ItemID.MushroomTorch:
                    return 22;
                case ItemID.ShimmerTorch:
                    return 23;
            }
        }
    }
}
