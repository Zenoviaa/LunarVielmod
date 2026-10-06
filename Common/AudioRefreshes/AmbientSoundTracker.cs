using Stellamod.Content.Biomes;
using Stellamod.Core;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Common.AudioRefreshes;

public class AmbientSoundTracker : ModSystem
{
    static List<AmbientSound> _ambientSoundsToAdd;
    static List<OneShotSound> _oneShotSoundsToAdd;
    static AmbientSound[] _ambientSounds;
    static OneShotSound[] _oneShotSounds;
    public static bool Enabled;
    public override void Load()
    {
        base.Load();
        _ambientSoundsToAdd = new();
        _oneShotSoundsToAdd = new();
    }

    public override void PostAddRecipes()
    {
        base.PostAddRecipes();
        RegisterAmbientSound(new(
            AssetReferences.Assets.Sounds.RefreshNAmbience.UnderwaterDeep.Asset with { Volume = 0.33f }, 
            () => Main.LocalPlayer.wet && Main.LocalPlayer.ZoneHarmonicCoralways));

        RegisterAmbientSound(new(
            AssetReferences.Assets.Sounds.RefreshNAmbience.UnderwaterShallow.Asset with { Volume = 0.33f },
            () => Main.LocalPlayer.wet && !Main.LocalPlayer.ZoneHarmonicCoralways));

        RegisterAmbientSound(new(
            AssetReferences.Assets.Sounds.RefreshNAmbience.ForestSpringHillAmbience.Asset with { Volume = 0.33f }, 
            () => (Main.LocalPlayer.GetModPlayer<BiomePlayer>().ZoneSpringHills || Main.LocalPlayer.ZonePurity) && Main.dayTime,
            VolumeOsc: true));
       
        RegisterAmbientSound(new(
            AssetReferences.Assets.Sounds.RefreshNAmbience.CampireAmbience.Asset, 
            () => Main.LocalPlayer.HasBuff(BuffID.Campfire), 
            FindCampfire));

        RegisterAmbientSound(new(
            AssetReferences.Assets.Sounds.RefreshNAmbience.WorldsEndAmbience.Asset, 
            () => Main.LocalPlayer.GetModPlayer<BiomePlayer>().ZoneWorldsEnd));

        _ambientSounds = _ambientSoundsToAdd.ToArray();
        _oneShotSounds = _oneShotSoundsToAdd.ToArray();
        _ambientSoundsToAdd = null;
        _oneShotSoundsToAdd = null;
    }

    int DistanceSquared(Point16 a, Point16 b)
    {
        var dx = (b.X - a.X);
        var dy = (b.Y - a.Y);
        return dx * dx + dy * dy;
    }

    Vector2 FindCampfire()
    {
        var campfirePoints = new List<Point16>();
        var rect = TileUtilities.CenterTileRectangle(Main.LocalPlayer.Center.ToTileCoordinates(), 100, 100);
        for(var x = rect.Left; x < rect.Right; x++)
        {
            for(var y = rect.Top; y < rect.Bottom; y++)
            {
                var tile = Main.tile[x, y];
                if (!tile.HasTile)
                    continue;
                if (!TileID.Sets.Campfire[tile.type])
                    continue;
                campfirePoints.Add(new(x, y));
            }
        }

        var playerPoint = Main.LocalPlayer.Center.ToTileCoordinates16();
        campfirePoints.Sort((x, y) => DistanceSquared(x, playerPoint).CompareTo(DistanceSquared(y, playerPoint)));
        if (campfirePoints.Count <= 0)
            return new Vector2(9999);
        return campfirePoints[0].ToWorldCoordinates();
    }

    public static void RegisterAmbientSound(AmbientSound ambientSound)
    {
        _ambientSoundsToAdd.Add(ambientSound);
    }

    public static void RegisterOneShotSound(OneShotSound oneShotSound)
    {
        _oneShotSoundsToAdd.Add(oneShotSound);
    }

    public override void PostUpdatePlayers()
    {
        base.PostUpdatePlayers();
        Update();
    }

    public void Update()
    {
        Enabled = ModContent.GetInstance<LunarVeilClientConfig>().Overhaul;
        for(var i = 0; i < _ambientSounds.Length; i++)
        {
            ref var ambientSound = ref _ambientSounds[i];
            ambientSound.Update(Enabled);
        }
        for (var i = 0; i < _oneShotSounds.Length; i++)
        {
            ref var oneShotSound = ref _oneShotSounds[i];
            oneShotSound.Update(Enabled);
        }
    }
}
