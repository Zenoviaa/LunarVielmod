using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Core.Pixelation;
using Stellamod.Effects.RoyalMagic;
using Stellamod.Helpers;
using Stellamod.Trails;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using static Terraria.GameContent.Animations.Actions.Sprites;

namespace Stellamod.Content.Areas.RoyalCapital.BossesRC.RoyalFox.Projectiles;

public class FenixSaw : ModProjectile,
    IDrawToRenderTarget
{
    private float _swingTrailAlpha;
    private ref float Timer => ref Projectile.ai[0];
    private NPC Parent => Main.npc[(int)Projectile.ai[1]];

    private ref float SpinTimer => ref Projectile.ai[2];
    public override string Texture => TextureRegistry.EmptyTexture;
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 333;
        Projectile.height = 333;
        Projectile.hostile = false;
        Projectile.timeLeft = 600;
        Projectile.penetrate = -1;
        Projectile.tileCollide = false;
    }

    public override void AI()
    {
        base.AI();
        //Main.NewText("E");
        if(SpinTimer > 100 && this.OwnedByLocalClient())
        {
            SpinTimer = 45;
            Projectile.netUpdate = true;
        }
        if (!Parent.active)
            Projectile.active = false;

        if(SpinTimer > 0)
        {
            SpinTimer--;
        }

        Timer++;
        Projectile.velocity = (Parent.Center - Projectile.Center);
        Projectile.rotation -= (0.35f + MathHelper.Lerp(0f, 0.35f, SpinTimer / 45));
        float inAlpha = EasingFunction.InOutSine(Timer / 130);
        float outAlpha = EasingFunction.InOutSine((float)Projectile.timeLeft / 30f);
        float alpha = inAlpha * outAlpha;
        _swingTrailAlpha = alpha;
        if(Timer >= 100)
        {
            Projectile.hostile = true;
        }
    }
  
    private float GetTrailWidth(float ratio)
    {
        return MathHelper.Lerp(150, 89, ratio) * 0.8f ;
    }
    private float GetTrailWidth2(float ratio)
    {
        return GetTrailWidth(ratio) * 1.5f;
    }
    private float GetTrailWidth3(float ratio)
    {
        return GetTrailWidth(ratio) * 2f;
    }
    private float GetTrailWidth4(float ratio)
    {
        return GetTrailWidth(ratio) * 1.45f * MathHelper.SmoothStep(1f, 0, ratio);
    }


    private Color GetTrailColor(float ratio)
    {

        Color inbetweenColor = Color.Blue;
        inbetweenColor = Color.Lerp(inbetweenColor, Color.Pink, ExtraMath.Osc(0f, 1f, speed: 9));
        Color c1 = Color.Lerp(Color.White, inbetweenColor, ratio);
        Color c2 = Color.Lerp(inbetweenColor, Color.Pink, ratio);
        c2 = Color.Lerp(c2, Color.Pink, ExtraMath.Osc(0f, 1f, speed: 16));
        Color c3 = Color.Lerp(c1, c2, ratio);
        c3 *= 0.5f;
        c3 *= _swingTrailAlpha;
       // c3.A = 0;
        return c3;
    }

    private Color GetTrailColor2(float ratio)
    {
        Color c = GetTrailColor(ratio) * 0.24f * EasingFunction.QuadraticBump(ratio);// * EasingFunction.QuadraticBump(_swingTrailAlpha);     
        // c.A = 0;
        c *= _swingTrailAlpha;
        return c;
    }
    private Color GetTrailColor3(float ratio)
    {
        Color c = Color.White;
        c = Color.Lerp(Color.Blue, c, ratio);
        c = Color.Lerp(c, Color.Pink, ExtraMath.Osc(0f, 1f, speed: 9));
        c *= _swingTrailAlpha ;// * EasingFunction.QuadraticBump(_swingTrailAlpha);                                                                                                                     // c.A = 0;
        return c;
    }

    private void DrawSwordSlash(SpriteBatch sb, Vector2 sp)
    {
        Vector2 point = Parent.Center + CalculateSwingOffset(1f);
        SpritebatchDrawer glowSword = SpritebatchDrawer.FromTextureAsset(
            ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/GlowSword_Sword"), point);
        glowSword.rotation = (point - Parent.Center).ToRotation() + MathHelper.PiOver2;
        Color startColor = Color.White;
        Color glowColor = Color.Lerp(Color.Blue, Color.Pink, ExtraMath.Osc(0f, 1f, speed: 3));
        Color mixColor = Color.Lerp(startColor, glowColor, ExtraMath.Osc(0f, 1f, speed: 6));
        glowSword.color = mixColor * ExtraMath.Osc(0.8f, 1f, speed: 3) * _swingTrailAlpha;
        glowSword.color.A = 0;
        glowSword.scale *= 1.6f;

        for(int i = 0; i < 16; i++)
        {
            float ratio = (float)i / 16f;
            glowSword.worldPosition = Parent.Center + CalculateSwingOffset(MathHelper.Lerp(0.5f, 1f, ratio));
            glowSword.rotation = (glowSword.worldPosition - Parent.Center).ToRotation() + MathHelper.PiOver2;
            glowSword.color = Color.Lerp(Color.White, Color.Transparent, ratio) * 0.15f * _swingTrailAlpha;
            glowSword.color.A = 0;
            sb.Draw(glowSword);
        }
        glowSword.worldPosition = Parent.Center + CalculateSwingOffset(1f);
        glowSword.rotation = (point - Parent.Center).ToRotation()  + MathHelper.PiOver2;
        glowSword.color = mixColor * ExtraMath.Osc(0.8f, 1f, speed: 3) * _swingTrailAlpha;
        glowSword.color.A = 0;
        sb.Draw(glowSword);
        sb.Draw(glowSword);

    }

    private Vector2 CalculateSwingOffset(float interpolant)
    {
        Vector2 v = Vector2.UnitY;
        v = v.RotatedBy(MathHelper.TwoPi * interpolant * MathHelper.Lerp(1f, 1.5f, EasingFunction.InOutSine(Timer / 90)));
        v = v.RotatedBy(Projectile.rotation);
        v *= 144;
        return v;
    }
    private Vector2 CalculateSwingOffset(float interpolant, float dist)
    {
        Vector2 v = Vector2.UnitY;
        v = v.RotatedBy(MathHelper.TwoPi * interpolant * MathHelper.Lerp(1f, 1.5f, EasingFunction.InOutSine(Timer / 90)));
        v = v.RotatedBy(Projectile.rotation);
        v *= dist;
        return v;
    }

    private void DrawSlashEffect(GraphicsDevice gDevice)
    {
        Vector2[] position = new Vector2[128];
        for (int i = 0; i < position.Length; i++)
        {
            //Here we use parent.center cause projectil.center might be the wrong spottt
            float ratio = (float)i / (float)position.Length;
            Vector2 v = CalculateSwingOffset(ratio);
            v = v.RotatedBy(MathHelper.ToRadians(180));
            Vector2 point = Parent.Center + v;
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
        shader2.LaserColor = Color.Lerp(Color.Cyan, Color.Pink, ExtraMath.Osc(0f, 1f, speed: 8));
         TrailDrawer.Draw(position, GetTrailColor2, GetTrailWidth3, shader2);

        for (int i = 0; i < position.Length; i++)
        {
            //Here we use parent.center cause projectil.center might be the wrong spottt
            float ratio = (float)i / (float)position.Length;
            Vector2 v = CalculateSwingOffset(MathHelper.Lerp(0.75f, 1f, ratio), 244);
            v = v.RotatedBy(MathHelper.ToRadians(180 - 45));
            Vector2 point = Parent.Center + v;
            position[i] = point;
        }

        BasicLaserAlphaShader alphaShader = ShaderContent.GetInstance<BasicLaserAlphaShader>();
        alphaShader.LaserTexture = TrailRegistry.GlowTrailNoBlack;
        alphaShader.InnerColor = Color.White;
        alphaShader.OuterColor = Color.White;
        TrailDrawer.Draw(position, GetTrailColor3, GetTrailWidth4, alphaShader);


    }

    public void DrawToRenderTargets()
    {
        PixelationManager.QueuePrimitivesDrawAction(DrawSlashEffect, DrawLayer.OverPlayers);
        PixelationManager.QueueSpritebatchDrawAction(DrawSwordSlash, DrawLayer.OverPlayers);
        //throw new NotImplementedException();
    }
}
