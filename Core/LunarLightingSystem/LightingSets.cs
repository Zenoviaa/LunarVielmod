using Microsoft.Xna.Framework;
using Stellamod.Content.Areas.SpecialTiles.EffectTiles;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Core.LunarLightingSystem
{
    public class LightingSets : ModSystem 
    {
        public static Color[] EmissiveHeldItems = ItemID.Sets.Factory.CreateCustomSet<Color>(Color.Black * 0);
        public static Color[] PointLitTiles = ItemID.Sets.Factory.CreateCustomSet<Color>(Color.Black * 0);
        public static Color[] GlowingTiles = ItemID.Sets.Factory.CreateCustomSet<Color>(Color.Black * 0);
        public static bool[] NoShadows = TileID.Sets.Factory.CreateBoolSet();
        public override void SetupContent()
        {
            EmissiveHeldItems = ItemID.Sets.Factory.CreateCustomSet<Color>(Color.Black * 0);
            PointLitTiles = TileID.Sets.Factory.CreateCustomSet<Color>(Color.Black * 0);
            GlowingTiles = TileID.Sets.Factory.CreateCustomSet<Color>(Color.Black * 0);
            NoShadows = TileID.Sets.Factory.CreateBoolSet();
            NoShadows[ModContent.TileType<BossBarrierBlock>()] = true;
            NoShadows[ModContent.TileType<STARBOMBERBarrierBlock>()] = true;
            NoShadows[ModContent.TileType<StarrVeriplantBarrierBlock>()] = true;

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
            for (int i = 0; i < TileID.Sets.Torch.Length; i++)
            {
                if (TileID.Sets.Torch[i])
                {
                    PointLitTiles[i] = Color.White;
                }

            }
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
