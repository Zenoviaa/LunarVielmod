using Stellamod.Common.Shaders;
using Stellamod.Core.Rendering.RTs;
using Terraria;
using Terraria.GameContent.UI.ResourceSets;
using Terraria.ModLoader;

namespace Stellamod.Common.WitchySystem;

[Autoload(Side = ModSide.Client)]
public class OffsetMinimapWithDisplaySet : ModSystem
{
    public override void Load()
    {
        base.Load();
        On_Main.GetInfoAccIconPosition += GetOffsetPosition;
        On_Main.UpdateMinimapAnchors += OffsetMinimapDown;
    }

    private void GetOffsetPosition(On_Main.orig_GetInfoAccIconPosition orig, int drawnCount, int StartX, out int X, out int Y)
    {
        orig(drawnCount, StartX, out X, out Y);

        //throw new NotImplementedException();
        Y += 160;
    }

    private void OffsetMinimapDown(On_Main.orig_UpdateMinimapAnchors orig)
    {
        orig();
        //   Main.NewText("whoas");
        //Main._minimapTopRightAnchorOffsetTowardsLeft += 190; 
        //Main._minimapTopRightAnchorOffsetTowardsLeft = 52 + (int)(240f * Main.MapScale);
        Main._minimapTopRightAnchorOffsetTowardsBottom += 160;
    }
}

public class PotionHealthDisplaySet : ModResourceDisplaySet
{
    PlayerStatsSnapshot _snapShot;
    PotionLiquidRenderer PotionLiquidRenderer => field ??= new();
    int TowardsLeft => 128;
    int TowardsBottom => 111;
    int BarOffsetTowardsLeft => 111;
    int BarOffsetTowardsBottom => 18;
    public override void PreDrawResources(PlayerStatsSnapshot snapshot)
    {
        base.PreDrawResources(snapshot);
        _snapShot = snapshot;
    }

    public override void DrawLife(SpriteBatch spriteBatch)
    {
        base.DrawLife(spriteBatch);
        PotionLiquidRenderer.waveAmplitude = 0.1f;
        PotionLiquidRenderer.waveFrequency = 0.03f;
        PotionLiquidRenderer.Simulate();


        void DrawMask()
        {
            var gradientDrawer = SpritebatchDrawer.FromTextureAsset(AssetReferences.Assets.Textures.UI.Witchy.PotionGuardBackingGradient.Asset, Main.screenPosition);
            gradientDrawer.color = Color.White;
            gradientDrawer.worldPosition.X += Main.screenWidth - TowardsLeft;
            gradientDrawer.worldPosition.Y += TowardsBottom;
            spriteBatch.Draw(gradientDrawer);

            var barDrawer = SpritebatchDrawer.FromTextureAsset(AssetReferences.Assets.Textures.UI.Witchy.PotionBar.Asset, Main.screenPosition);
            barDrawer.VerticalFrame(2, 3);
            barDrawer.color = Color.White;
            barDrawer.worldPosition.X += Main.screenWidth - TowardsLeft - BarOffsetTowardsLeft;
            barDrawer.worldPosition.Y += TowardsBottom + BarOffsetTowardsBottom;
            spriteBatch.Draw(barDrawer);


            var steps = (_snapShot.LifeMax - 200) / 20;
            var x = barDrawer.worldPosition.X;
            barDrawer.VerticalFrame(2, 3);
            barDrawer.color = Color.White;
            for (var i = 0; i < steps - 1; i++)
            {
                barDrawer.worldPosition.X -= 32;
                spriteBatch.Draw(barDrawer);
            }


            var barEdgeDrawer = SpritebatchDrawer.FromTextureAsset(AssetReferences.Assets.Textures.UI.Witchy.PotionBarEdge.Asset, Main.screenPosition);
            barEdgeDrawer.VerticalFrame(2, 3);
            barEdgeDrawer.color = Color.White;
            barEdgeDrawer.worldPosition.X = barDrawer.worldPosition.X - 32 - 2;
            barEdgeDrawer.worldPosition.Y += TowardsBottom + BarOffsetTowardsBottom ;
            spriteBatch.Draw(barEdgeDrawer);
        }


        var barDrawer = SpritebatchDrawer.FromTextureAsset(AssetReferences.Assets.Textures.UI.Witchy.PotionBar.Asset, Main.screenPosition);
        barDrawer.worldPosition.X += Main.screenWidth - TowardsLeft - BarOffsetTowardsLeft;
        barDrawer.worldPosition.Y += TowardsBottom + BarOffsetTowardsBottom;
        barDrawer.VerticalFrame(1, 3);
        barDrawer.color = Color.White;


        var steps = (_snapShot.LifeMax - 200) / 20;
        var x = barDrawer.worldPosition.X;
        barDrawer.worldPosition.X = x;
        spriteBatch.Draw(barDrawer);
        for (var i = 0; i < steps - 1; i++)
        {
            barDrawer.worldPosition.X -= 32;
            spriteBatch.Draw(barDrawer);
        }


        var barEdgeDrawer = SpritebatchDrawer.FromTextureAsset(AssetReferences.Assets.Textures.UI.Witchy.PotionBarEdge.Asset, Main.screenPosition);
        barEdgeDrawer.color = Color.White;
        barEdgeDrawer.worldPosition.X = barDrawer.worldPosition.X - 32 - 2;
        barEdgeDrawer.worldPosition.Y += TowardsBottom + BarOffsetTowardsBottom;
        barEdgeDrawer.VerticalFrame(1, 3);
        spriteBatch.Draw(barEdgeDrawer);



        DrawMask();


        var drawer = SpritebatchDrawer.FromTextureAsset(AssetReferences.Assets.Textures.UI.Witchy.PotionGuard.Asset, Main.screenPosition);
        drawer.color = Color.White;
        drawer.worldPosition.X += Main.screenWidth - TowardsLeft;
        drawer.worldPosition.Y += TowardsBottom;
        spriteBatch.Draw(drawer);

        using var liquidTarget = RT.Context(RenderTargets.ScreenTarget);
        spriteBatch.EndOut(out var oldParameters);



        using var potionTarget = RT.Context(RenderTargets.ScreenTarget);

        Main.graphics.PreferredDepthStencilFormat = DepthFormat.Depth24Stencil8;
        using (RT.Clear(potionTarget, Color.Transparent))
        {
            using (spriteBatch.Ctx(oldParameters with { matrix = Matrix.Identity}))
            {

                PotionLiquidRenderer.Render(spriteBatch, new Vector2(Main.screenWidth - 262, -54));
            }

        }
        using (RT.Clear(liquidTarget, Color.Transparent))
        {
            var white = SpriteWhiteShader.Instance;
            using(spriteBatch.Ctx(oldParameters with {  matrix = Matrix.Identity, sortMode = SpriteSortMode.Deferred, effect= white }))
            {
                DrawMask();
            }
            using (spriteBatch.Ctx(oldParameters with { matrix = Matrix.Identity, blendState = CustomBlendStates.Multiply }))
            {
                spriteBatch.Draw(potionTarget, Vector2.Zero, Color.White);

            }

        }
        spriteBatch.Begin(oldParameters);
        spriteBatch.Draw(liquidTarget, Vector2.Zero, Color.White);
 
    }
    public override void DrawMana(SpriteBatch spriteBatch)
    {
        base.DrawMana(spriteBatch);
    }

}
