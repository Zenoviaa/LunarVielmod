using System;
using System.Linq;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Common.ScreenEffectsSystem;

[Autoload(Side = ModSide.Client)]
public class ScreenEffectSystem : ModSystem
{
    private AScreenEffect[] _effects;
    public override void Load()
    {
        base.Load();
        On_FilterManager.EndCapture += ApplyScreenEffects;
        On_Main.Draw += DeActivateScreenEffects;
    }

    private void DeActivateScreenEffects(On_Main.orig_Draw orig, Main self, GameTime gameTime)
    {
        orig(self, gameTime);
        if (Main.gameMenu)
            return;
        for (var i = 0; i < _effects.Length; i++)
        {
            _effects[i].isActive = false;
        }
    }

    private void ApplyScreenEffects(On_FilterManager.orig_EndCapture orig, FilterManager self, RenderTarget2D finalTexture, RenderTarget2D screenTarget1, RenderTarget2D screenTarget2, Color clearColor)
    {

       
        orig(self, finalTexture, screenTarget1, screenTarget2, clearColor);
        if (!Main.gameMenu)
        {
            var gDevice = Main.spriteBatch.graphicsDevice;
            var src = screenTarget1;
            var dst = screenTarget2;

            var drewEffects = 0;
            var srcIndex = 0;
            var nextIndex = 1;
            for (var i = 0; i < _effects.Length; i++)
            {
                var effect = _effects[i];
                if (!effect.isActive)
                    continue;

                drewEffects++;

                var drawSource = srcIndex == 0 ? src : dst;
                var drawTarget = nextIndex == 0 ? src : dst;

                gDevice.SetRenderTarget(drawTarget);
                gDevice.Clear(Color.Transparent);
                effect.Apply(Main.spriteBatch, drawSource, drawTarget);

                srcIndex++;
                nextIndex++;
                srcIndex %= 2;
                nextIndex %= 2;
            }


            if (drewEffects != 0)
            {
                var lastSource = srcIndex == 0 ? src : dst;
                var sb = Main.spriteBatch;
                gDevice.SetRenderTarget(finalTexture);
                gDevice.Clear(Color.Transparent);
                sb.Begin();
                sb.Draw(lastSource, Vector2.Zero, Color.White);
                sb.End();
            }

        }
    }

    public override void PostAddRecipes()
    {
        base.PostAddRecipes();
        _effects = ModContent.GetContent<AScreenEffect>().ToArray();
        Array.Sort(_effects, (x, y) => x.Priority.CompareTo(y.Priority));
    }

    public override void Unload()
    {
        base.Unload();
        _effects = null;
    }
}
