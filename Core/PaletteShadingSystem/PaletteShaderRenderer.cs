
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Content.Areas.Ishtar.BossesIS.SanguineSingularity;
using Stellamod.Content.Biomes;
using Stellamod.Core.Palettes;
using Stellamod.Core.Shaders;
using Stellamod.Helpers;
using System;
using System.Formats.Tar;
using System.Linq;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace Stellamod.Core.PaletteShadingSystem
{
    public enum PalettePriority : byte
    {
        Low,
        Medium,
        High,
        Highest
    }

    public enum PaletteType : byte
    {
        VanillaShader,
        LunarShader
    }
    public class AbyssPaletteEffect : PaletteEffect
    {
        public override PaletteType PaletteType => PaletteType.LunarShader;
        public override PalettePriority Priority => PalettePriority.Medium;
        public override bool IsActive(Player player)
        {
            MyPlayer myPlayer = player.GetModPlayer<MyPlayer>();
            if (myPlayer.ZoneAbyss)
                return true;
            if(myPlayer.ZoneAurelus)
                return true;

            return false;
        }
    }
    public class VilepipesNGardenPaletteEffect : PaletteEffect
    {
        public override PalettePriority Priority => PalettePriority.Medium;
        public override PaletteType PaletteType => PaletteType.LunarShader;
        public override bool IsActive(Player player)
        {
            MyPlayer myPlayer = player.GetModPlayer<MyPlayer>();
            if (myPlayer.ZoneGovheil)
                return true;
            if (myPlayer.ZoneAcid)
                return true;

            return false;
        }
    }

    public class HellPaletteEffect : PaletteEffect
    {
        public override PaletteType PaletteType => PaletteType.LunarShader;
        public override bool IsActive(Player player)
        {
            MyPlayer myPlayer = player.GetModPlayer<MyPlayer>();
            if (myPlayer.ZoneCinder)
                return true;
            if (myPlayer.ZoneDrakonic)
                return true;
            if (player.ZoneUnderworldHeight)
                return true;

            return false;
        }
    }

    public class RoyalCapitalPaletteEffect : PaletteEffect
    {
        public override PalettePriority Priority => PalettePriority.Medium;
        public override PaletteType PaletteType => PaletteType.LunarShader;

        public override bool IsActive(Player player)
        {
            MyPlayer myPlayer = player.GetModPlayer<MyPlayer>();
            if (myPlayer.ZoneAlcadzia)
                return true;
            return false;
        }
    }

    public class DungeonPaletteEffect : PaletteEffect
    {
        public override PalettePriority Priority => PalettePriority.Low;
        public override PaletteType PaletteType => PaletteType.VanillaShader;
        public override bool IsActive(Player player)
        {
            if (player.ZoneDungeon)
                return true;
            return false;
        }
    }


    public class DesertPaletteEffect : PaletteEffect
    {
        public override PalettePriority Priority => PalettePriority.Low;
        public override PaletteType PaletteType => PaletteType.VanillaShader;
        public override bool IsActive(Player player)
        {
            if (player.ZoneUndergroundDesert)
                return true;
            return false;
        }
    }

    public class DesertTopPaletteEffect : PaletteEffect
    {
        public override PalettePriority Priority => PalettePriority.Low;
        public override PaletteType PaletteType => PaletteType.VanillaShader;

        public override bool IsActive(Player player)
        {
            MyPlayer myPlayer = player.GetModPlayer<MyPlayer>();
            if (myPlayer.ZoneAshotiTemple)
                return true;
            if (player.ZoneUndergroundDesert)
                return false;
            if (player.ZoneDesert)
                return true;
            return false;
        }
    
    }

    public class FablePaletteEffect : PaletteEffect
    {
        public override PalettePriority Priority => PalettePriority.Medium;
        public override PaletteType PaletteType => PaletteType.LunarShader;
        public override bool IsActive(Player player)
        {
            MyPlayer myPlayer = player.GetModPlayer<MyPlayer>();
            if (myPlayer.ZoneFable)
                return true;
            return false;
        }
    }

    public class IllurianMistyDungeonPaletteEffect : PaletteEffect
    {
        public override PalettePriority Priority => PalettePriority.Medium;
        public override PaletteType PaletteType => PaletteType.LunarShader;
        public override bool IsActive(Player player)
        {
            return player.GetModPlayer<BiomePlayer>().ZoneMistyDungeon;
        }
    }

    public class BloodHoundPaletteEffect : PaletteEffect
    {
        public override PalettePriority Priority => PalettePriority.Medium;
        public override PaletteType PaletteType => PaletteType.LunarShader;
        public override bool IsActive(Player player)
        {
            MyPlayer myPlayer = player.GetModPlayer<MyPlayer>();
            if (Main.dayTime)
                return false;
            if (myPlayer.ZoneBloodCathedral)
                return true;
            return false;
        }
    }

    public class SanguineSingularityPaletteEffect : PaletteEffect
    {
        public override PalettePriority Priority => PalettePriority.Highest;
        public override PaletteType PaletteType => PaletteType.LunarShader;
        public override bool IsActive(Player player)
        {
            return base.IsActive(player) && NPC.AnyNPCs(ModContent.NPCType<SanguineSingularity>());
        }
    }

    /// <summary>
    /// A palette shader effect, these don't use terraria's normal post-processing because we don't want it over EVERY single layer
    /// </summary>
    public abstract class PaletteEffect : ModType
    {
        public virtual PaletteType PaletteType { get; }
        public virtual PalettePriority Priority { get; }
        public sealed override void SetupContent()
        {
            base.SetupContent();
            SetStaticDefaults();
        }
        public float fade;
        protected override void Register()
        {
            ModTypeLookup<PaletteEffect>.Register(this);
        }

        public virtual string GetPaletteFile()
        {
            return this.GetType().Name.Replace("PaletteEffect", ".pal");
        }

        public Effect GetShader()
        {
            string palFile = GetPaletteFile();
            PalettizerShader palettizerShader = PalettizerShader.Instance;
            palettizerShader.PaletteTexture = PaletteHelper.GetColorSpectrum(palFile);
            palettizerShader.Progress = fade;
            return palettizerShader.Effect;
        }

        public virtual bool IsActive(Player player)
        {
            return true;
        }
    }


    /// <summary>
    /// Handles applying a palette shader effect to the screen
    /// </summary>
    [Autoload(Side = ModSide.Client)]
    public class PaletteShaderRenderer : ModSystem
    {
        private PaletteEffect[] _paletteEffects;
        private RenderTarget2D _paletteRenderRT;
        private Vector2 _previousScreenSize;
        public override void Load()
        {
            ResizeRenderTarget(true);
            On_OverlayManager.Draw += DrawPalette;
        }
        public override void Unload()
        {
            base.Unload();
            On_OverlayManager.Draw -= DrawPalette;
        }
        public override void OnModLoad()
        {
            base.OnModLoad();
            _paletteEffects = ModContent.GetContent<PaletteEffect>().ToArray();
        }
      

        private void UpdatePaletteEffects()
        {
            for(int i = 0; i < _paletteEffects.Length; i++)
            {
                PaletteEffect paletteEffect = _paletteEffects[i];
                if (paletteEffect.IsActive(Main.LocalPlayer))
                {
                    paletteEffect.fade += 0.01f;
                }
                else
                {
                    paletteEffect.fade -= 0.01f;
                }
                paletteEffect.fade = MathHelper.Clamp(paletteEffect.fade, 0f, 1f);
          
            }
        }

        public override void PostUpdateEverything()
        {
            base.PostUpdateEverything();
            ResizeRenderTarget(false);
            UpdatePaletteEffects();
        }

        private void DrawPalette(On_OverlayManager.orig_Draw orig, OverlayManager self, SpriteBatch spriteBatch, RenderLayers layer, bool beginSpriteBatch)
        {
            if (layer == RenderLayers.All && beginSpriteBatch)
            {
                DrawToScreen();
            }
            orig(self, spriteBatch, layer, beginSpriteBatch);
        }

        private void DrawToScreen()
        {
            Effect paletteEffect = null;
            float fade = 0f;
            for(int i = 0; i < _paletteEffects.Length; i++)
            {
                PaletteEffect pEffect = _paletteEffects[i];
                if(pEffect.fade > 0 && pEffect.fade >= fade)
                {
                    fade = pEffect.fade;
                    paletteEffect = pEffect.GetShader();
               
                }
            }

            if (paletteEffect == null)
                return;
  
            SpriteBatch spriteBatch = Main.spriteBatch;
            GraphicsDevice graphicsDevice = spriteBatch.GraphicsDevice;
            graphicsDevice.SetRenderTarget(_paletteRenderRT);
            graphicsDevice.Clear(Color.Transparent);

            
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, Main.Rasterizer, null);
            spriteBatch.Draw(Main.screenTarget, Vector2.Zero, null, Color.White, 0f, Vector2.Zero, 1, SpriteEffects.None, 0f);
            spriteBatch.End();

            graphicsDevice.SetRenderTarget(Main.screenTarget);
            graphicsDevice.Clear(Color.Transparent);

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, Main.Rasterizer, paletteEffect);
            spriteBatch.Draw(_paletteRenderRT, Vector2.Zero, null, Color.White, 0f, Vector2.Zero, 1, SpriteEffects.None, 0f);
            spriteBatch.End();
        }

        private void ResizeRenderTarget(bool load)
        {
            if (!Main.gameMenu && !Main.dedServ || load && !Main.dedServ)
            {
                Vector2 currentScreenSize = new(Main.screenWidth, Main.screenHeight);
                if (currentScreenSize != _previousScreenSize)
                {
                    Main.QueueMainThreadAction(() =>
                    {
                        if (_paletteRenderRT != null && !_paletteRenderRT.IsDisposed)
                            _paletteRenderRT.Dispose();


                        _paletteRenderRT = new RenderTarget2D(Main.graphics.GraphicsDevice, Main.screenWidth, Main.screenHeight, false,
                            SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);

                    });
                }

                _previousScreenSize = currentScreenSize;
            }
        }
    }
}
