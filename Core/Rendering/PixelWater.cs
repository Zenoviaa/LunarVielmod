using ReLogic.Content;
using Stellamod.Assets.ContentReader.Pal;
using Terraria.ModLoader;

namespace Stellamod.Core.Rendering;

public class PixelWater
{
    public PixelWater()
    {

    }

    public void SetDefaults()
    {
        StartGradientColor = Color.Aqua;
        EndGradientColor = Color.Lerp(Color.SeaGreen, Color.Black, 0.75f);
        BackgroundColor = Color.CornflowerBlue;
        CausticsColor = Color.SeaGreen * 0.75f;
        CausticsTexture = LoadTexture("WaterCaustics");
        NoiseTexture = LoadTexture("WaterNoise2");
        Palette = null;
        TilingMultiplier = Vector2.One;
        affectsLava = false;
        noLighting = false;
        vibrant = false;
        ignoreSkyColor = false;
        noReflection = false;
        reflectionAlpha = 1f;
        maxHeight = 32;
        levels = 10;
    }

    private Asset<Texture2D> LoadTexture(string fileName)
    {
        return ModContent.Request<Texture2D>($"Stellamod/Assets/NoiseTextures/{fileName}");
    }
    public Color StartGradientColor;
    public Color EndGradientColor;
    public Color BackgroundColor;
    public Color CausticsColor;
    public Vector2 TilingMultiplier;
    public Asset<Texture2D> NoiseTexture;
    public Asset<Texture2D> CausticsTexture;
    public Palette Palette;
    public bool noLighting;
    public bool vibrant;
    public bool ignoreSkyColor;
    public bool affectsLava;
    public bool noReflection;
    public float reflectionAlpha;
    public int maxHeight;
    public int levels;
}
