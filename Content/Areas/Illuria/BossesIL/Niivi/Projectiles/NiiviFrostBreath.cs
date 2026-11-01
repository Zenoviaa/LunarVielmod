using Stellamod.Common;
using Stellamod.Common.Shaders;
using Stellamod.Core.Pixelation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Illuria.BossesIL.Niivi.Projectiles;

public class NiiviFrostBreath : ModProjectile
{
    ref float Timer => ref Projectile.ai[0];
    NPC Parent => Main.npc[(int)Projectile.ai[1]];
    ref float FrostBreathTime => ref Projectile.ai[2];
    public override string Texture => ModType.NoTexture;
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.tileCollide = false;
        Projectile.width = 2560;
        Projectile.height = 16;
        Projectile.penetrate = -1;
        Projectile.light = 0.6f;
        Projectile.timeLeft = 600;
    }
    public override bool ShouldUpdatePosition()
    {
        return false;
    }

    public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
    {
        return base.Colliding(projHitbox, targetHitbox);
    }
    public override void AI()
    {
        base.AI();
        Timer++;
        if(Timer >= FrostBreathTime)
        {
            Projectile.Kill();
        }
        Projectile.Center = Parent.Center;
        Projectile.velocity = Parent.rotation.ToRotationVector2() * 1256;
        if (!Parent.active)
            Projectile.Kill();
    }

    float GetTrailWidth(float ratio)
    {
        return MathHelper.SmoothStep(32, 256, ratio);
    }

    Color GetTrailColor(float ratio)
    {
        return Color.White;
    }

    void DrawPixelatedFrostBreath(GraphicsDevice gDevice)
    {
        var pass = AssetReferences.Effects.Niivi.NiiviFrostBreath.CreatePrimitivesPass();
        pass.Parameters.time = Main.GlobalTimeWrappedHourly;
        pass.Parameters.transformMatrix = TrailDrawer.WorldViewPoint2;
        pass.Apply();
        gDevice.Register(0, AssetReferences.Assets.LaserTextures.TexturedLaser.Asset.Value, SamplerState.LinearWrap);
        var points = DrawUtilities.InterpolateBetweenPoints(Projectile.Center, Projectile.Center + Projectile.velocity, 64);
        TrailDrawer.Draw(points, GetTrailColor, GetTrailWidth, pass.Shader);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        PixelationManager.QueuePrimitivesDrawAction(DrawPixelatedFrostBreath);
        return false;
        //return base.PreDraw(ref lightColor);
    }
    public override void OnHitPlayer(Player target, Player.HurtInfo info)
    {
        base.OnHitPlayer(target, info);
    }

    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
    }
}
