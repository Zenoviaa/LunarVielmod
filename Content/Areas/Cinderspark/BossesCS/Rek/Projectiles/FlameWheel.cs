using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Core.Pixelation;
using Stellamod.Core.ProjectileHelpers;
using Stellamod.Effects.RoyalMagic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Cinderspark.BossesCS.Rek.Projectiles;

//Like fenix's think but fiery and really cool
//Could probably make this shader with similar methods to gothivia?
//or just use the same shader tbh
//I'll look at how it works
public class FlameWheel : ModProjectile
{
    private NPC Parent => Main.npc[(int)Projectile.ai[0]]; 
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        this.AddCommonDebuff(DebuffFlags.Burning_Serpent);
    }

    public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
    {
        float collisionRadius = 212;
        Vector2 centerPoint = targetHitbox.Center();
        Vector2 myPoint = projHitbox.Center();
        return Vector2.Distance(myPoint, centerPoint) <= collisionRadius;
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.tileCollide = false;
        Projectile.width = 200;
        Projectile.height = 200;
        Projectile.timeLeft = 600;
        Projectile.hostile = true;
    }
    public override void AI()
    {
        base.AI();
        if(Parent.ModNPC is not RekBoss || !Parent.active)
        {
            Projectile.active = false;
            return;
        }
        Projectile.Center = Parent.Center;
    }

    private float GetTrailWidth(float ratio)
    {
        return MathHelper.Lerp(150, 89, ratio);
    }
    private float GetTrailWidth2(float ratio)
    {
        return GetTrailWidth(ratio) * 1.5f;
    }
    private float GetTrailWidth3(float ratio)
    {
        return GetTrailWidth(ratio) * 2f;
    }

    private Color GetTrailColor(float ratio)
    {
        Color c = Color.Lerp(Color.White, Color.Lerp(Color.Pink, Color.Blue, ExtraMath.Osc(0f, 1f, speed: 16)), ratio)  * EasingFunction.QuadraticBump(ratio);// * EasingFunction.QuadraticBump(_swingTrailAlpha);
                                                                                                                                                                                // c.A = 0;
        return c;
    }
    private Color GetTrailColor2(float ratio)
    {
        Color c = Color.Lerp(Color.White, Color.Lerp(Color.Pink, Color.Blue, ExtraMath.Osc(0f, 1f, speed: 16)), ratio)  * 0.24f * EasingFunction.QuadraticBump(ratio);// * EasingFunction.QuadraticBump(_swingTrailAlpha);                                                                                                                     // c.A = 0;
        return c;
    }
    private void DrawFlameTrail(GraphicsDevice gDevice)
    {

        //FixedRichLaserShader shader = ShaderContent.GetInstance<FixedRichLaserShader>();
        AlcadSlashShader shader = ShaderContent.GetInstance<AlcadSlashShader>();
        shader.ScrollingLaser = TrailRegistry.Beamlight.Value;
        shader.Noise = AssetManager.Noise.Whirly.Value;
        shader.Slash = AssetManager.LaserTextures.CometTrail.Value;
        shader.BloomColor = Color.Purple;
        shader.Time = Main.GlobalTimeWrappedHourly * 24;
        shader.TransformMatrix = TrailDrawer.WorldViewPoint2;
        shader.Distortion = 0.15f;
        TrailDrawer.Draw(Projectile.oldPos, GetTrailColor, GetTrailWidth, shader, Projectile.Size * 0.5f);
        TrailDrawer.Draw(Projectile.oldPos, GetTrailColor, GetTrailWidth2, shader, Projectile.Size * 0.5f);

        FixedRichLaserShader shader2 = ShaderContent.GetInstance<FixedRichLaserShader>();
        shader2.LaserTexture = TrailRegistry.BeamTrail;
        TrailDrawer.Draw(Projectile.oldPos, GetTrailColor2, GetTrailWidth3, shader2, Projectile.Size * 0.5f);
    }
    private void DrawSlashEffect(GraphicsDevice gDevice)
    {
        /*
        Vector2[] position = new Vector2[128];
        float endPoint = _swingTrailEndRatio;
        float startPoint = endPoint - 0.35f;
        for (int i = 0; i < position.Length; i++)
        {
            float ratio = i / (float)position.Length;
            float interp = MathHelper.Lerp(endPoint, startPoint, ratio);
            Vector2 point = _startDashPoint + CalculateSwingOffset(_swingVelocity, interp);
            point += _swingVelocity.SafeNormalize(Vector2.Zero) * 200;
            position[i] = point;
        }

        //FixedRichLaserShader shader = ShaderContent.GetInstance<FixedRichLaserShader>();
        AlcadSlashShader shader = ShaderContent.GetInstance<AlcadSlashShader>();
        shader.ScrollingLaser = TrailRegistry.Beamlight.Value;
        shader.Noise = AssetManager.Noise.Whirly.Value;
        shader.Slash = AssetManager.GlowMask.SwordSlash.Value;
        shader.BloomColor = Color.Purple;
        shader.Time = Main.GlobalTimeWrappedHourly * 24;
        shader.TransformMatrix = TrailDrawer.WorldViewPoint2;
        shader.Distortion = 0.15f;
        TrailDrawer.Draw(position, GetTrailColor, GetTrailWidth, shader);
        TrailDrawer.Draw(position, GetTrailColor, GetTrailWidth2, shader);

        FixedRichLaserShader shader2 = ShaderContent.GetInstance<FixedRichLaserShader>();
        shader2.LaserTexture = TrailRegistry.BeamTrail;
        TrailDrawer.Draw(position, GetTrailColor2, GetTrailWidth3, shader2);*/
    }

    public override bool PreDraw(ref Color lightColor)
    {
       // PixelationManager.QueuePrimitivesDrawAction(DrawFlameTrail, DrawLayer.OverNPCsAdditive);
        return false;
    }
    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
    }
}
