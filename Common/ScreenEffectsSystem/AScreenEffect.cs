using Terraria.ModLoader;

namespace Stellamod.Common.ScreenEffectsSystem;

public abstract class AScreenEffect : ModType
{
    protected sealed override void Register()
    {
        ModTypeLookup<AScreenEffect>.Register(this);
    }

    public sealed override void SetupContent()
    {
        base.SetupContent();
        SetStaticDefaults();
    }

    public abstract ScreenEffectPriority Priority { get; }
    public abstract void Apply(SpriteBatch spriteBatch, RenderTarget2D src, RenderTarget2D dst);
    public bool isActive;
}
