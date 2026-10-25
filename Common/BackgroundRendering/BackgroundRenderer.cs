using System.Linq;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.ModLoader;

namespace Stellamod.Common.BackgroundRendering;

[Autoload(Side = ModSide.Client)]
public class BackgroundRenderer : ModSystem
{
    private ABackground[] _backgrounds;
    private BackgroundDrawing[] _backgroundDrawData;
    public override void OnModLoad()
    {
        base.OnModLoad();
        On_OverlayManager.Draw += DrawBackgrounds;
    }

    private void DrawBackgrounds(On_OverlayManager.orig_Draw orig, OverlayManager self, SpriteBatch spriteBatch, RenderLayers layer, bool beginSpriteBatch)
    {
        if (layer == RenderLayers.Background)
        {
            for (var i = 0; i < _backgrounds.Length; i++)
            {
                ref var data = ref _backgroundDrawData[i];
                if (data.opacity <= 0)
                    continue;
                var background = _backgrounds[i];
                data.areaRectangle = new Rectangle(0, 0, Main.screenWidth, Main.screenHeight);
                data.cameraMovement += (Main.screenPosition - Main.screenLastPosition);
                data.lightColor = Main.ColorOfTheSkies;
                background.Draw(spriteBatch, data);
            }
        }
        orig(self, spriteBatch, layer, beginSpriteBatch);

    }


    public override void OnModUnload()
    {
        base.OnModUnload();
    }

    public override void PostAddRecipes()
    {
        base.PostAddRecipes();
        _backgrounds = ModContent.GetContent<ABackground>().ToArray();
        _backgroundDrawData = new BackgroundDrawing[_backgrounds.Length];
    }

    public override void PostUpdateEverything()
    {
        base.PostUpdateEverything();
        for (var i = 0; i < _backgrounds.Length; i++)
        {
            var background = _backgrounds[i];
            ref var data = ref _backgroundDrawData[i];
            if (background.IsActive())
            {
                data.opacity += 0.05f;
            }
            else
            {
                data.opacity -= 0.05f;
                if (data.opacity <= 0)
                    data.cameraMovement = Vector2.Zero;
            }
            data.opacity = MathHelper.Clamp(data.opacity, 0f, 1f);
        }
    }
}

public abstract class ABackground : ModType
{
    public sealed override void SetupContent()
    {
        base.SetupContent();
        SetStaticDefaults();
    }

    protected sealed override void Register()
    {
        ModTypeLookup<ABackground>.Register(this);
    }

    public abstract void Draw(SpriteBatch spriteBatch, in BackgroundDrawing draw);
    public abstract bool IsActive();
}


public struct BackgroundDrawing
{
    public Rectangle areaRectangle;
    public Color lightColor;
    public Vector2 cameraMovement;
    public float opacity;
    public int CalculateXLoops(float width)
    {
        return (int)((areaRectangle.Width / width) + 1);
    }

    public int CalculateYLoops(float height)
    {
        return (int)((areaRectangle.Height / height) + 1);
    }
}
