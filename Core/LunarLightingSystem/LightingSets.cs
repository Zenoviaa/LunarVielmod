using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Core.LunarLightingSystem
{
    public class LightingSets : ModSystem 
    {
        public static Color[] EmissiveHeldItems;
        public static Color[] PointLitTiles;
        public static Color[] GlowingTiles;
        public override void SetupContent()
        {
            EmissiveHeldItems = ItemID.Sets.Factory.CreateCustomSet<Color>(Color.Black * 0);
            PointLitTiles = TileID.Sets.Factory.CreateCustomSet<Color>(Color.Black * 0);
            GlowingTiles = TileID.Sets.Factory.CreateCustomSet<Color>(Color.Black * 0);


            RegisterTorchColor(ItemID.Torch);
            RegisterTorchColor(ItemID.BlueTorch);
            RegisterTorchColor(ItemID.RedTorch);
            RegisterTorchColor(ItemID.GreenTorch);
            RegisterTorchColor(ItemID.PurpleTorch);
            RegisterTorchColor(ItemID.WhiteTorch);
            RegisterTorchColor(ItemID.YellowTorch);
            RegisterTorchColor(ItemID.DemonTorch);
            RegisterTorchColor(ItemID.CursedTorch);
            RegisterTorchColor(ItemID.IceTorch);
            RegisterTorchColor(ItemID.OrangeTorch);
            RegisterTorchColor(ItemID.IchorTorch);
            RegisterTorchColor(ItemID.UltrabrightTorch);
            RegisterTorchColor(ItemID.BoneTorch);
            RegisterTorchColor(ItemID.RainbowTorch);
            RegisterTorchColor(ItemID.PinkTorch);
            RegisterTorchColor(ItemID.DesertTorch);
            RegisterTorchColor(ItemID.CoralTorch);
            RegisterTorchColor(ItemID.CorruptTorch);
            RegisterTorchColor(ItemID.CrimsonTorch);
            RegisterTorchColor(ItemID.HallowedTorch);
            RegisterTorchColor(ItemID.JungleTorch);
            RegisterTorchColor(ItemID.MushroomTorch);
            RegisterTorchColor(ItemID.ShimmerTorch);
            GlowingTiles[TileID.MushroomGrass] = Color.LightBlue;
            for(int i = 0; i < TileID.Sets.Torch.Length; i++)
            {
                if (TileID.Sets.Torch[i])
                {
                    TileLoader.item
                    PointLitTiles[i] = Color.White;
                }

            }

            PointLitTiles[TileID.tor]
            base.SetupContent();
        }

        public static void RegisterTorchColor(int itemID)
        {
            Vector3 torchRGB = new Vector3();
            int torchID = TorchLightingHelper.TorchItemToTorchID(itemID);
            TorchID.TorchColor(itemID, out torchRGB.X, out torchRGB.Y, out torchRGB.Z);
            Color torchColor = new Color(torchRGB);
            EmissiveHeldItems[itemID] = torchColor;
        }
        public static Color GetTorchColor(int torchID)
        {
            Vector3 torchRGB = new Vector3();
            TorchID.TorchColor(torchID, out torchRGB.X, out torchRGB.Y, out torchRGB.Z);
            Color torchColor = new Color(torchRGB);
            return torchColor;
        }
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();


        }
    }
}
