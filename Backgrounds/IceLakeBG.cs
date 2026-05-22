using Microsoft.Xna.Framework;
using Stellamod.Content.Areas;
using Stellamod.Core.Backgrounds;
using Stellamod.Core.Effects;
using Terraria;
using Terraria.ModLoader;
namespace Stellamod.Backgrounds
{
    public class IceBackgroundRemove : ModBiome
    {
        public override int Music
        {
            get
            {
                if (!Main.dayTime)
                {
                    return MusicLoader.GetMusicSlot(Mod, "Assets/Music/CountingStars");
                }
                return -1;
            }
        }
        public override SceneEffectPriority Priority => SceneEffectPriority.BiomeMedium;
        public override ModSurfaceBackgroundStyle SurfaceBackgroundStyle => ModContent.GetInstance<NoBackgroundStyle>();
        public override bool IsBiomeActive(Player player)
        {
       //     Main.NewText(player.ZoneSnow);

            return player.ZoneSnow;
        }
    }

    public class IceLakeBG : CustomBG
    {
        public CustomBGLayer bleedLayer;
        public CustomBGLayer vanillaLayer;
        public CustomBGLayer backLayer;
        public CustomBGLayer back2Layer;
        public CustomBGLayer midLayer;
        public CustomBGLayer frontLayer;
        public CustomBGLayer front2Layer;
        private void AddBackmostLayer()
        {
            vanillaLayer = new CustomBGLayer();
            vanillaLayer.Texture = ModContent.Request<Texture2D>("Terraria/Images/Background_98");
            vanillaLayer.DrawOffset = Vector2.Zero;
            AddLayer(vanillaLayer);
        }

        private void AddBackmost2Layer()
        {
            backLayer = new CustomBGLayer();
            backLayer.Texture = ModContent.Request<Texture2D>("Terraria/Images/Background_101");
            backLayer.DrawOffset = Vector2.Zero;
            AddLayer(backLayer);
        }

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            DrawScale = 1.5f;
            DrawOffset = new Vector2(0, -100);

            float startParallax = 0.15f;

            AddBackmostLayer();
            AddBackmost2Layer();

            //Guh
            CustomBGLayer backFogLayer = new CustomBGLayer();
            backFogLayer.SetTexture("Assets/Textures/Backgrounds/IceGradient");
            backFogLayer.Parallax = startParallax;
            backFogLayer.DrawOffset = Vector2.Zero;

            MistShader mistShader = new MistShader();
            mistShader.StartColor = Color.White * 0.5f;
            mistShader.EndColor = Color.Transparent;
            backFogLayer.Shader = mistShader;
            AddLayer(backFogLayer);

            back2Layer = new CustomBGLayer();
            back2Layer.SetTexture("Assets/Textures/Backgrounds/IceBack2");
            back2Layer.Parallax = startParallax;
            back2Layer.DrawOffset = Vector2.Zero;
            AddLayer(back2Layer);


            midLayer = new CustomBGLayer();
            midLayer.SetTexture("Assets/Textures/Backgrounds/IceMiddle");
            midLayer.Parallax = startParallax + 0.02f;
            midLayer.DrawOffset = Vector2.Zero;
            AddLayer(midLayer);

            CustomBGLayer midFogLayer = new CustomBGLayer();
            midFogLayer.SetTexture("Assets/Textures/Backgrounds/IceGradient");
            midFogLayer.Parallax = startParallax + 0.02f;
            midFogLayer.DrawOffset = Vector2.Zero;

            MistShader midMistShader = new MistShader();
            midMistShader.StartColor = Color.White * 0.5f;
            midMistShader.EndColor = Color.Transparent;
            midFogLayer.Shader = midMistShader;
            AddLayer(midFogLayer);

            frontLayer = new CustomBGLayer();
            frontLayer.SetTexture("Assets/Textures/Backgrounds/IceFront");
            frontLayer.Parallax = startParallax + 0.05f;
            frontLayer.DrawOffset = Vector2.Zero;
            AddLayer(frontLayer);

            front2Layer = new CustomBGLayer();
            front2Layer.SetTexture("Assets/Textures/Backgrounds/IceFront2");
            front2Layer.Parallax = startParallax + 0.09f;
            front2Layer.DrawOffset = Vector2.Zero;
            AddLayer(front2Layer);

            bleedLayer = new CustomBGLayer();
            bleedLayer.SetTexture("Assets/Textures/Backgrounds/IceUnderground");
            bleedLayer.Parallax = startParallax;
            bleedLayer.DrawOffset = new Vector2(0, bleedLayer.Texture.Size().Y * DrawScale * 2);
            AddLayer(bleedLayer);
        }
        public override void SetDrawDefaults()
        {
            base.SetDrawDefaults();
            DrawScale = 1.5f;
            LocalParallaxSpeed = 1 / 1.5f;
  
            backLayer.DrawScale = 1f / 1.5f;
            vanillaLayer.DrawScale = 1f / 1.5f;

            backLayer.DrawOffset = new Vector2(0, 500);
            vanillaLayer.DrawOffset = new Vector2(0, 500);


            vanillaLayer.Parallax = 0.04f;
            backLayer.Parallax = 0.06f;
            back2Layer.Parallax = 0.075f;
            midLayer.Parallax = 0.09f;
            frontLayer.Parallax = 0.12f;
            front2Layer.Parallax = 0.18f;

            frontLayer.DrawOffset = front2Layer.DrawOffset = new Vector2(0, 0);
            bleedLayer.DrawOffset = new Vector2(0, 1590);
            bleedLayer.Parallax = front2Layer.Parallax;
            DrawOffset = new Vector2(0, 280);
        }

        public override bool IsActive()
        {
            return Main.LocalPlayer.ZoneSnow && Main.LocalPlayer.ZoneOverworldHeight;
        }
    }
}
