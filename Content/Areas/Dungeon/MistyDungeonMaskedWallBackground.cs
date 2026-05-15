using ReLogic.Content;
using Stellamod.Common.Shaders;
using Stellamod.Content.Biomes;
using Stellamod.Core.Pixelation;
using Stellamod.Core.WallBackgroundSystem;
using Stellamod.Helpers;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Dungeon;

public class MistyDungeonMaskedWallBackground : MaskedWallBackground
{
    private Asset<Texture2D> _mistyDungeonTextureAsset;
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        _mistyDungeonTextureAsset = ModContent.Request<Texture2D>("Stellamod/Assets/Textures/Backgrounds/MistyDungeon");
    }

    public override void Unload()
    {
        base.Unload();
        _mistyDungeonTextureAsset = null;
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
        return biomePlayer.ZoneMistyDungeonAnywhere;
    }

    public override bool UseCustomDrawing()
    {
        return true;
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        base.Draw(spriteBatch);
        var backgroundShader = MistyDungeonBackgroundShader.Instance;
        Vector2[] parallax = new Vector2[4];

        Vector2 cameraMovement = Main.Camera.Center - StartParallaxPosition;

        for (int i = 0; i < 4; i++)
        {
            parallax[i] = Vector2.Lerp(new Vector2(0.01f, 0f), Vector2.Zero, (float)i / 4f) * (cameraMovement) * 0.01f;
        }
        backgroundShader.Parallax = parallax;
        backgroundShader.FadeToColor = Color.Blue * 0.5f;
        spriteBatch.Begin(default,
            default,
            SamplerState.PointWrap,
            default,
            default,
            backgroundShader.Effect);

        Rectangle drawRect = new Rectangle(0, 0, Main.screenWidth, Main.screenHeight);
        for(int i = 3; i >= 0; i--)
        {
            SpritebatchDrawer drawer = SpritebatchDrawer.FromTextureAsset(_mistyDungeonTextureAsset, Main.screenPosition);
            drawer.drawOrigin = Vector2.Zero;
            drawer.scale = Vector2.One * 2;
            drawer.color = Color.White;
            drawer.VerticalFrame(i, 4);
            drawer.dstRect = drawRect;
            spriteBatch.Draw(drawer);
        }

        spriteBatch.End();
    }
}