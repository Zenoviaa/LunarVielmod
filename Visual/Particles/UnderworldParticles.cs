using Stellamod.Core.Particles;
using Stellamod.Helpers;
using Terraria;

namespace Stellamod.Visual.Particles;


public class UnderworldFlameParticle : Particle<UnderworldFlameParticle>
{
    private Vector2 _parallax;
    private float _parallaxStrength;
    public bool ySlow;
    public override void OnSpawn()
    {
        ySlow = true;
        _parallax = Vector2.Zero;
        _parallaxStrength = Main.rand.NextFloat(0.3f, 0.6f);
    }

    public override void Update()
    {
        fadeIn++;
        if (fadeIn > 180)
            active = false;
        if (ySlow)
            Velocity.Y *= 0.98f;
        else
            Scale *= 0.99f;
        Velocity.X *= 0.999f;
        _parallax += (Main.screenPosition - Main.screenLastPosition) * -_parallaxStrength;
        Rotation = Velocity.X * 0.05f;
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        base.Draw(spriteBatch);
        float easeIn = EasingFunction.OutExpo(fadeIn / 80f);
        float easeOut = MathHelper.Lerp(1f, 0f, EasingFunction.InExpo(fadeIn / 180f));
        float alpha = easeIn * easeOut;
        Color glowColor = Color.White * easeIn;
        Vector2 centerPos = DrawPosition;
        var textureAsset = GetTexture();
        spriteBatch.Draw(textureAsset.Value, centerPos + _parallax, null, glowColor, Rotation, textureAsset.Value.Size() * 0.5f, Scale * easeOut, SpriteEffects.None, 0);
    }
}
public class UnderworldFlameParticle2 : Particle<UnderworldFlameParticle2>
{
    public override void OnSpawn()
    {

    }

    public override void Update()
    {
        fadeIn++;
        if (fadeIn > 180)
            active = false;
        Velocity.Y *= 0.98f;
        Velocity.X *= 0.999f;
        Rotation = Velocity.X * 0.05f;
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        base.Draw(spriteBatch);
        float easeIn = EasingFunction.OutExpo(fadeIn / 80f);
        float easeOut = MathHelper.Lerp(1f, 0f, EasingFunction.InExpo(fadeIn / 180f));
        float alpha = easeIn * easeOut;
        Color glowColor = Color.White * alpha;
        Vector2 centerPos = DrawPosition;
        var textureAsset = GetTexture();
        spriteBatch.Draw(textureAsset.Value, centerPos, null, glowColor * alpha, Rotation, textureAsset.Value.Size() * 0.5f, Scale, SpriteEffects.None, 0);
    }
}

public class UnderworldSmokeParticle : Particle<UnderworldSmokeParticle>
{
    public override void OnSpawn()
    {

    }

    public override void Update()
    {
        fadeIn++;
        if (fadeIn > 180)
            active = false;
        Velocity.Y *= 0.98f;
        Velocity.X *= 0.999f;
        Rotation = Velocity.X * 0.05f;
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        base.Draw(spriteBatch);
        float easeIn = EasingFunction.OutExpo(fadeIn / 80f);
        float easeOut = MathHelper.Lerp(1f, 0f, EasingFunction.InExpo(fadeIn / 180f));
        float alpha = easeIn * easeOut;
        Color glowColor = Color.White * alpha;
        Vector2 centerPos = DrawPosition;
        var textureAsset = GetTexture();
        spriteBatch.Draw(textureAsset.Value, centerPos, null, glowColor * alpha, Rotation, textureAsset.Value.Size() * 0.5f, Scale, SpriteEffects.None, 0);
    }
}