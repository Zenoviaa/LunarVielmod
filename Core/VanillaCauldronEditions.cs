using Stellamod.Content.CommonMaterials;
using Stellamod.Content.Items.Materials;
using Stellamod.Items;
using Stellamod.Items.Harvesting;
using Stellamod.Items.Materials;
using Stellamod.Items.Ores;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Core;

public class VanillaCauldronEditions : ModSystem
{
    public override void PostAddRecipes()
    {
        base.PostAddRecipes();
        Cauldron.SetMaterial(ModContent.ItemType<Mushroom>());
        Cauldron.VanillaBrew(result: ItemID.Aglet);
        Cauldron.VanillaBrew(result: ItemID.JellyfishNecklace);

        Cauldron.SetMaterial(ModContent.ItemType<Ivythorn>());
        Cauldron.VanillaBrew(result: ItemID.HermesBoots);
        Cauldron.VanillaBrew(result: ItemID.ShinyRedBalloon);
        Cauldron.VanillaBrew(result: ItemID.CloudinaBottle);

        Cauldron.SetMaterial(ModContent.ItemType<AlcadizScrap>());
        Cauldron.VanillaBrew(result: ItemID.PortableStool);
        Cauldron.VanillaBrew(result: ItemID.SunStone);

        Cauldron.SetMaterial(ModContent.ItemType<WinterbornShard>());
        Cauldron.VanillaBrew(result: ItemID.IceSkates);
        Cauldron.VanillaBrew(result: ItemID.BlizzardinaBottle);

        Cauldron.SetMaterial(ModContent.ItemType<MinersGold>());
        Cauldron.VanillaBrew(result: ItemID.RocketBoots);
        Cauldron.VanillaBrew(result: ItemID.ClimbingClaws);
        Cauldron.VanillaBrew(result: ItemID.LuckyHorseshoe);
        Cauldron.VanillaBrew(result: ItemID.ShoeSpikes);


        Cauldron.SetMaterial(ModContent.ItemType<TerrorFragments>());
        Cauldron.VanillaBrew(result: ItemID.BandofRegeneration);
        Cauldron.VanillaBrew(result: ItemID.PhilosophersStone);
        Cauldron.VanillaBrew(result: ItemID.FleshKnuckles);
        Cauldron.VanillaBrew(result: ItemID.PutridScent);
        Cauldron.VanillaBrew(result: ItemID.PanicNecklace);

        Cauldron.SetMaterial(ModContent.ItemType<GintzlMetal>());
        Cauldron.VanillaBrew(result: ItemID.AnkletoftheWind);
        Cauldron.VanillaBrew(result: ItemID.SandBoots);
        Cauldron.VanillaBrew(result: ItemID.SharkToothNecklace);
        Cauldron.VanillaBrew(result: ItemID.SandstorminaBottle);
        Cauldron.VanillaBrew(result: ItemID.Banana);

        Cauldron.SetMaterial(ModContent.ItemType<Cinderscrap>());
        Cauldron.VanillaBrew(result: ItemID.HellfireTreads);
        Cauldron.VanillaBrew(result: ItemID.LavaCharm);
        Cauldron.VanillaBrew(result: ItemID.EyeoftheGolem);
        Cauldron.VanillaBrew(result: ItemID.ObsidianRose);
        Cauldron.VanillaBrew(result: ItemID.ObsidianSkull);

        Cauldron.SetMaterial(ModContent.ItemType<HypnotizedSoul>());
        Cauldron.VanillaBrew(result: ItemID.BandofStarpower);

        Cauldron.SetMaterial(ModContent.ItemType<ConvulgingMater>());
        Cauldron.VanillaBrew(result: ItemID.CelestialMagnet);
        Cauldron.VanillaBrew(result: ItemID.RifleScope);

        Cauldron.SetMaterial(ModContent.ItemType<PearlescentScrap>());
        Cauldron.VanillaBrew(result: ItemID.CobaltShield);
        Cauldron.VanillaBrew(result: ItemID.StarCloak);
        Cauldron.VanillaBrew(result: ItemID.PaladinsShield);
        Cauldron.VanillaBrew(result: ItemID.MagicQuiver);

        Cauldron.SetMaterial(ModContent.ItemType<MarshScrap>());
        Cauldron.VanillaBrew(result: ItemID.FlowerBoots);
        Cauldron.VanillaBrew(result: ItemID.NaturesGift);
        Cauldron.VanillaBrew(result: ItemID.StaffofRegrowth);

        Cauldron.SetMaterial(ModContent.ItemType<MechanizedSoul>());
        Cauldron.VanillaBrew(result: ItemID.YoyoBag);
        Cauldron.VanillaBrew(result: ItemID.DiscountCard);
        Cauldron.VanillaBrew(result: ItemID.LuckyCoin);
        Cauldron.VanillaBrew(result: ItemID.Tabi);

        Cauldron.SetMaterial(ModContent.ItemType<KaleidoscopicInk>());
        Cauldron.VanillaBrew(result: ItemID.RainbowString);

        Cauldron.SetMaterial(ModContent.ItemType<IllurineScale>());
        Cauldron.VanillaBrew(result: ItemID.AngelWings);
        Cauldron.VanillaBrew(result: ItemID.FrozenWings);

        Cauldron.SetMaterial(ModContent.ItemType<MiracleThread>());
        Cauldron.VanillaBrew(result: ItemID.BlackBelt);
        Cauldron.VanillaBrew(result: ItemID.AnkhCharm);

        Cauldron.SetMaterial(ModContent.ItemType<EreshkinCandle>());
        Cauldron.VanillaBrew(result: ItemID.NecromanticScroll);
        Cauldron.VanillaBrew(result: ItemID.DemonWings);

        Cauldron.SetMaterial(ModContent.ItemType<RadiantNectar>());
        Cauldron.VanillaBrew(result: ItemID.LeafWings);
        Cauldron.VanillaBrew(result: ItemID.BeeWings);


        Cauldron.SetMaterial(ModContent.ItemType<AlcaricMush>());
        Cauldron.VanillaBrew(result: ItemID.TatteredFairyWings);
        Cauldron.VanillaBrew(result: ItemID.GhostWings);

        Cauldron.SetMaterial(ModContent.ItemType<FallenEyes>());
        Cauldron.VanillaBrew(result: ItemID.FishronWings);
        Cauldron.VanillaBrew(result: ItemID.MothronWings);
        Cauldron.VanillaBrew(result: ItemID.BoneWings);

        Cauldron.SetMaterial(ModContent.ItemType<MusicalHarmonise>());
        Cauldron.VanillaBrew(result: ItemID.AmphibianBoots);
        Cauldron.VanillaBrew(result: ItemID.FrogLeg);
        Cauldron.VanillaBrew(result: ItemID.Flipper);
        Cauldron.VanillaBrew(result: ItemID.DivingGear);
        Cauldron.VanillaBrew(result: ItemID.FloatingTube);
        Cauldron.VanillaBrew(result: ItemID.WaterWalkingBoots);
        Cauldron.VanillaBrew(result: ItemID.TsunamiInABottle);
        Cauldron.VanillaBrew(result: ItemID.BalloonPufferfish);
        Cauldron.VanillaBrew(result: ItemID.SailfishBoots);
    }
}
