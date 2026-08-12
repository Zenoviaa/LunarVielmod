using Stellamod.Content.Areas.Terror.TilesTR;
using Stellamod.Core.Pixelation;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.UI.Chat;

namespace Stellamod.Common.Particles;

//Helper methods for spawning particles
[Autoload(Side = ModSide.Client)]
public sealed class Particles : ModSystem
{
    private List<IParticleUpdater> _particleUpdaters;

    public static BitDust BitDust;
    public override void Load()
    {
        base.Load();
        On_Main.DrawDust += DrawParticles;
        BitDust = new();
        _particleUpdaters = new List<IParticleUpdater>
        {
            BitDust
        };
       
        for (int i = 0; i < _particleUpdaters.Count; i++)
        {
            if (_particleUpdaters[i] is ILoadable loadable)
            {
                loadable.Load(Mod);
            }
        }
    }

    public override void Unload()
    {
        base.Unload();
        if (_particleUpdaters == null)
            return;

        for (int i = 0; i < _particleUpdaters.Count; i++)
        {
            if (_particleUpdaters[i] is ILoadable loadable)
            {
                loadable.Unload();
            }
        }
        _particleUpdaters.Clear();
        _particleUpdaters = null;
    }

    private void DrawParticles(On_Main.orig_DrawDust orig, Main self)
    {
        orig(self);
        for (int i = 0; i < _particleUpdaters.Count; i++)
        {
            IParticleUpdater particleUpdater = _particleUpdaters[i];
            if (particleUpdater.PixelationDrawLayer != DrawLayer.None)
            {
                PixelationManager.QueueSpritebatchDrawAction(_particleUpdaters[i].Draw, particleUpdater.PixelationDrawLayer);
            }
            else
            {
                _particleUpdaters[i].Draw(Main.spriteBatch, Main.screenPosition);
            }
        }
    }

    public override void PostUpdateDusts()
    {
        base.PostUpdateDusts();
        for (int i = 0; i < _particleUpdaters.Count; i++)
        {
            _particleUpdaters[i].Update();
        }

        /*
        if (Main.mouseLeft && Main.mouseLeftRelease)
        {
            BitDustFactory factory = BitDustFactory.Default;
            factory.position = Main.MouseWorld;
            for (int i = 0; i < 15_000; i++)
            {

     
                factory.velocity = Main.rand.NextVector2Circular(16, 16);
                BitDust.Spawn(factory);
            }
        }*/

    }

    public override void PostDrawTiles()
    {
        base.PostDrawTiles();
        
        //Just for testing the atlas
        /*
        Main.spriteBatch.Begin();
        var time = BitDust.elapsedString;
        ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, FontAssets.MouseText.Value, time, Main.Camera.Center - Main.screenPosition + new Vector2(-144, -128), Color.White, 0, Vector2.Zero, Vector2.One * 1.2f);
        Main.spriteBatch.End();*/
    }
}
