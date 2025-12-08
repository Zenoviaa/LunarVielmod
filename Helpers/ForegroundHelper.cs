using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Helpers;

public class ForegroundHelper : ModSystem
{
    private static readonly List<ForegroundItem> _foregroundItemsToRemove = new List<ForegroundItem>();
    public static readonly List<ForegroundItem> Items = new List<ForegroundItem>();
    public static readonly List<ForegroundItem> PlayerLayerItems = new();

    public override void Load()
    {
        On_Main.DrawProjectiles += PlayerLayerHook;
        On_Main.DoUpdateInWorld += On_Main_DoUpdateInWorld;
    }

    public override void Unload()
    {
        On_Main.DrawProjectiles -= PlayerLayerHook;
        On_Main.DoUpdateInWorld -= On_Main_DoUpdateInWorld;
        Items.Clear();
        PlayerLayerItems.Clear();
    }

    private static void On_Main_DoUpdateInWorld(On_Main.orig_DoUpdateInWorld orig, Main self, System.Diagnostics.Stopwatch sw)
    {
        orig(self, sw);

        if (Main.PlayerLoaded && !Main.gameMenu)
            Update();
    }

    private static void PlayerLayerHook(On_Main.orig_DrawProjectiles orig, Main self)
    {
        orig(self);
        if (PlayerLayerItems.Count > 0)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);

            foreach (var val in PlayerLayerItems)
                val.Draw();

            spriteBatch.End();
        }

    }


    public static void Draw()
    {
        Rectangle screen = new((int)Main.screenPosition.X - Main.screenWidth, (int)Main.screenPosition.Y - Main.screenHeight, Main.screenWidth * 3, Main.screenHeight * 3);
        foreach (var val in Items)
        {
            if (screen.Contains(new Rectangle((int)val.position.X, (int)val.position.Y, val.Texture.Width(), val.Texture.Height())))
                val.Draw();
        }
    }

    public static void Update()
    {
        UpdateSet(PlayerLayerItems);
        UpdateSet(Items);
    }

    private static void UpdateSet(List<ForegroundItem> set)
    {
        _foregroundItemsToRemove.Clear();
        foreach (var val in set)
        {
            if (!Main.gamePaused)
                val.Update();

            if (val.killMe)
                _foregroundItemsToRemove.Add(val);
        }

        foreach (var item in _foregroundItemsToRemove)
            set.Remove(item);
    }


    public static int AddItem(ForegroundItem item, bool forced = false, bool playerLayer = false)
    {
        if (!forced) //Skip if option is turned off
            return -1;

        if (playerLayer)
        {
            PlayerLayerItems.Add(item);
            return PlayerLayerItems.IndexOf(item);
        }
        else
        {
            Items.Add(item);
            return Items.IndexOf(item);
        }
    }
}
