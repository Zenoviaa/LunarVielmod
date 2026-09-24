using Stellamod.Common.Shaders;
using Stellamod.Common.WeaponUpgrade;
using Stellamod.Core.LunarLightingSystem;
using Stellamod.Core.Pixelation;
using Stellamod.Core.Rendering.RTs;
using Stellamod.Core.Utilities;
using System;
using System.Reflection;
using Terraria;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Cinderspark.AccCS;




[Autoload(Side = ModSide.Client)]
public class InfraredRenderer : ModSystem
{
    public bool IsActive => !Main.gameMenu && Main.LocalPlayer.GetModPlayer<HeatGogglesPlayer>().hasHeatGoggles;
    public override void Load()
    {
        base.Load();
        On_Main.DrawPlayers_AfterProjectiles += DrawToScreen;
    }

    private void DrawToScreen(On_Main.orig_DrawPlayers_AfterProjectiles orig, Main self)
    {
        orig(self);
        if (!IsActive)
            return;

        SpriteBatch spriteBatch = Main.spriteBatch;
        using var screenTarget = RT.Context(RenderTargets.ScreenTarget);
        using(RT.Clear(screenTarget, Color.Transparent))
        {
            var target = Main.instance.tileTarget;

            var outlineShader = ShaderContent.GetInstance<WhiteOutlineShader>();
            outlineShader.TexelSize = Vector2.One / target.Size();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer, effect: outlineShader.Effect, Main.GameViewMatrix.TransformationMatrix);
            spriteBatch.Draw(target, Main.sceneTilePos - Main.screenPosition, null, Color.Orange, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
            spriteBatch.End();


            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            Main.instance.DrawNPCs();
            spriteBatch.End();
        }

        SpriteWhiteShader whiteShader = SpriteWhiteShader.Instance;
        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer, effect: whiteShader.Effect);
        spriteBatch.Draw(screenTarget, Vector2.Zero, null, Color.Orange * ExtraMath.Osc(0.5f, 1f, speed: 3), 0, Vector2.Zero, 1, SpriteEffects.None, 0);
        spriteBatch.End();
    }
}

public class HeatGogglesPlayer : ModPlayer
{
    public bool hasHeatGoggles;
    public override void ResetEffects()
    {
        base.ResetEffects();
        hasHeatGoggles = false;
    }
}

public class HeatGoggles : ModItem
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.DefaultToAccessory();
        Item.rare = ModContent.RarityType<CinderscrapRarity>();
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        base.UpdateAccessory(player, hideVisual);
        player.GetModPlayer<HeatGogglesPlayer>().hasHeatGoggles = true;
       
    }
}


