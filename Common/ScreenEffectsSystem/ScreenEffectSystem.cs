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

        if (!Main.gameMenu)
        {
            var gDevice = Main.spriteBatch.graphicsDevice;
            var src = Main.screenTarget;
            var dst = Main.screenTargetSwap;
            var finalSource = src;
            var drewEffects = 0;
            for (var i = 0; i < _effects.Length; i++)
            {
                var effect = _effects[i];
                if (!effect.isActive)
                    continue;

                drewEffects++;
                gDevice.SetRenderTarget(dst);
                gDevice.Clear(Color.Transparent);
                effect.Apply(Main.spriteBatch, src, dst);

                //Switch
                var temp = dst;
                finalSource = dst;
                dst = src;
                src = temp;

            }


            if (drewEffects != 0)
            {
                var sb = Main.spriteBatch;
                gDevice.SetRenderTarget(Main.screenTarget);
                gDevice.Clear(Color.Transparent);
                sb.Begin();
                sb.Draw(finalSource, Vector2.Zero, Color.White);
                sb.End();
            }

        }
        orig(self, finalTexture, screenTarget1, screenTarget2, clearColor);

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
