using ReLogic.Content;
using Stellamod.Assets;
using Stellamod.Content.Biomes;
using Stellamod.Core.WallBackgroundSystem;
using Terraria;

namespace Stellamod.Backgrounds;

public partial class AegislavSurfaceBackground
{
    public class AegislavTempleBackground : MaskedWallBackground
    {
        private Asset<Texture2D> _moonspiralTowerMidTextureAsset;
        private Asset<Texture2D> _moonspiralTowerBackTextureAsset;

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            _moonspiralTowerMidTextureAsset = AssetManager.LoadBackground("BloodCathedral_Mid");
            _moonspiralTowerBackTextureAsset = AssetManager.LoadBackground("BloodCathedral_Far");
        }

        public override void Unload()
        {
            base.Unload();
            _moonspiralTowerMidTextureAsset = null;
            _moonspiralTowerBackTextureAsset = null;
        }

        public override bool IsActive(Player player)
        {
            BiomePlayer biomePlayer = player.GetModPlayer<BiomePlayer>();
            return biomePlayer.ZoneAegislavSurface;
        }

        public override void SetupDrawLayers()
        {
            base.SetupDrawLayers();
            DrawScale = 1f;

            DrawLayers[1].textureAsset = _moonspiralTowerMidTextureAsset;
            DrawLayers[1].parallax = new Vector2(0.0135f);

            DrawLayers[0].textureAsset = _moonspiralTowerBackTextureAsset;
            DrawLayers[0].parallax = new Vector2(0.00075f);
        }
    }

}
