using Stellamod.Core.Pixelation;
using Stellamod.Core.Utilities;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.PunkerTown.BossesPT.Gothivia.Projectiles;

public class BouncingRazorSuns : ModProjectile,
    IDrawToRenderTarget
{
    private LazyAsset<Texture2D> _altTexture;
    private LazyAsset<Texture2D> _auraTexture;
    public override void Unload()
    {
        base.Unload();
        _altTexture?.Unload();
        _auraTexture?.Unload();
    }

    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        if (!Main.dedServ)
        {
            _altTexture = new LazyAsset<Texture2D>($"{Texture}_Alt");
            _auraTexture = new LazyAsset<Texture2D>($"{Texture}_Aura");
        }
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
    }
    public override void AI()
    {
        base.AI();
    }
    public override bool PreDraw(ref Color lightColor)
    {
        return base.PreDraw(ref lightColor);
    }
    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
    }

    public void DrawToRenderTargets()
    {

    }
}
