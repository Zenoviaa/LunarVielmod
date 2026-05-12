using ReLogic.Content;
using Stellamod.Content.Areas.MoonspiralTower.VerliaBoss;
using Stellamod.Content.Biomes;
using Stellamod.Core.WallBackgroundSystem;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.MoonspiralTower
{
    public class MoonspiralTowerMaskedWallBackground : MaskedWallBackground
    {
        private Asset<Texture2D> _moonspiralTowerFrontPaneTextureAsset;
        private Asset<Texture2D> _moonspiralTowerFrontTextureAsset;
        private Asset<Texture2D> _moonspiralTowerFrontGlowBallTextureAsset;
        private Asset<Texture2D> _moonspiralTowerMidTextureAsset;
        private Asset<Texture2D> _moonspiralTowerBackTextureAsset;
        
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            _moonspiralTowerFrontGlowBallTextureAsset = ModContent.Request<Texture2D>("Stellamod/Assets/Textures/Backgrounds/MoonspiralTowerFrontGlowBall");
            _moonspiralTowerFrontPaneTextureAsset = ModContent.Request<Texture2D>("Stellamod/Assets/Textures/Backgrounds/MoonspiralTowerFrontPane");
            _moonspiralTowerFrontTextureAsset = ModContent.Request<Texture2D>("Stellamod/Assets/Textures/Backgrounds/MoonspiralTowerFront");
            _moonspiralTowerMidTextureAsset = ModContent.Request<Texture2D>("Stellamod/Assets/Textures/Backgrounds/MoonspiralTowerMid");
            _moonspiralTowerBackTextureAsset = ModContent.Request<Texture2D>("Stellamod/Assets/Textures/Backgrounds/MoonspiralTowerFar");
        }

        public override void Unload()
        {
            base.Unload();
            _moonspiralTowerFrontGlowBallTextureAsset = null;
            _moonspiralTowerFrontPaneTextureAsset = null;
            _moonspiralTowerFrontTextureAsset = null;
            _moonspiralTowerMidTextureAsset = null;
            _moonspiralTowerBackTextureAsset = null;
        }
        
        public override bool IsActive(Player player)
        {
            if (NPC.AnyDanger())
            {
                Color = Color.Lerp(Color, Color.Lerp(Color.White, Color.Black, 0.5f), 0.1f);
            }
            else
            {
                Color = Color.Lerp(Color, Color.White, 0.1f);
            }
            BiomePlayer biomePlayer = player.GetModPlayer<BiomePlayer>();
            return biomePlayer.ZoneMoonspiralTower;
        }

        public override void SetupDrawLayers()
        {
            base.SetupDrawLayers();
            DrawScale = 2.5f;
            DrawLayers[4].textureAsset = _moonspiralTowerFrontPaneTextureAsset;
            DrawLayers[4].parallax = new Vector2(0.1f);
            DrawLayers[4].additive = true;

            DrawLayers[3].textureAsset = _moonspiralTowerFrontTextureAsset;
            DrawLayers[3].parallax = new Vector2(0.1f);


            DrawLayers[2].textureAsset = _moonspiralTowerFrontGlowBallTextureAsset;
            DrawLayers[2].parallax = new Vector2(0.0135f);
            DrawLayers[2].additive = true;

            DrawLayers[1].textureAsset = _moonspiralTowerMidTextureAsset;
            DrawLayers[1].parallax = new Vector2(0.0135f);

            DrawLayers[0].textureAsset = _moonspiralTowerBackTextureAsset;
            DrawLayers[0].parallax = new Vector2(0.00075f);
        }
    }
}
