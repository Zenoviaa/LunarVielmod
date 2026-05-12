using Stellamod.Common.Shaders;
using Stellamod.Core.Palettes;
using Stellamod.Core.Particles;
using Stellamod.Core.Utilities;
using Stellamod.Helpers;
using Stellamod.Trails;
using Stellamod.Visual.Particles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.WaterSide.BossesWS.Projectiles;

public class LeviathanBite : ModProjectile
{
    private ref float Timer => ref Projectile.ai[0];
    private NPC Parent => Main.npc[(int)Projectile.ai[1]];
    private ref float Style => ref Projectile.ai[2];
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 200;
        Projectile.height = 200;
        Projectile.hostile = false;
        Projectile.tileCollide = false;
        Projectile.penetrate = -1;
        Projectile.ignoreWater = true;
        Projectile.timeLeft = 60;
    }
    public override bool ShouldUpdatePosition()
    {
        return false;
    }

    public override void AI()
    {
        base.AI();
        Timer++;
        if(Timer == 1)
        {
   

        }
   
        if(Timer == 30)
        {
            ShakeScreenPosition.Shake = 4;
            FXUtil.ShakeCamera(Projectile.Center, 1024, 16);
            for (float f = 0; f < 16; f++)
            {
                Vector2 velocity = (Projectile.rotation-MathHelper.TwoPi).ToRotationVector2();
                if (Main.rand.NextBool(2))
                    velocity *= -1;
                var spawnParams = DustParticleSpawnParams.Default;
                spawnParams.outerColor = Color.Blue;
                var dp = DustParticle.Spawn(Projectile.Center, velocity, spawnParams);
                dp.Scale *= 0.6f;
            }
        }
        SpecialEffectsPlayer effectsPlayer = Main.LocalPlayer.GetModPlayer<SpecialEffectsPlayer>();
        effectsPlayer.darknessCurve = MathHelper.Lerp(0f, 0.5f, EasingFunction.QuadraticBump(Timer / 60f));
        if (Timer > 8)
        {
            Projectile.hostile = true;
        }
        if(Style == 1)
        {
            Projectile.Center = Parent.Center;
            Projectile.velocity = Parent.velocity;
        }
        if (Style == 2)
        {
            Projectile.Center = Parent.Center;
            float dp = Vector2.Dot(Projectile.velocity.SafeNormalize(Vector2.Zero), Parent.velocity.SafeNormalize(Vector2.Zero));
            if(dp > 0)
            {
                Projectile.velocity = Parent.velocity;

            }
            //
        }
        Projectile.rotation = Projectile.velocity.ToRotation();
    }
    public override bool PreDraw(ref Color lightColor)
    {
        Vector2 up = (Projectile.rotation - MathHelper.PiOver2).ToRotationVector2() * 128;

        float time = 60f;
        float ratio = Timer / time;
        float ease = EasingFunction.QuadraticBump(ratio);
        float ease2 = EasingFunction.InExpo(ratio / 0.5f);
        float ease3 = EasingFunction.InOutSine(ratio);
        float radiansOffset = MathHelper.ToRadians(135);
        float alpha = 0.05f;

        void Draw(Vector2 offset)
        {
            Vector2 scale = Vector2.Lerp(Vector2.One * 1.8f, Vector2.One, ease3) * 2;
            Color biteColor = Color.DarkBlue;
            SpritebatchDrawer biteDrawer = SpritebatchDrawer.FromProjectile(Projectile);
            biteDrawer.color = Color.Lerp(Color.Black, biteColor, ease) * alpha;
            biteDrawer.color.A = 0;
            biteDrawer.rotation -= MathHelper.Lerp(radiansOffset, 0f, ease2);
            biteDrawer.BottomLeftOrigin();
            biteDrawer.scale = scale;
            biteDrawer.worldPosition += offset;
            biteDrawer.worldPosition += up * MathHelper.Lerp(1f, -0.35f, ease2);
            Main.spriteBatch.Draw(biteDrawer);

            biteDrawer = SpritebatchDrawer.FromProjectile(Projectile);
            biteDrawer.color = Color.Lerp(Color.Black, biteColor, ease) * alpha;
            biteDrawer.color.A = 0;
            biteDrawer.rotation += MathHelper.Lerp(radiansOffset, 0f, ease2);
            biteDrawer.spriteEffects = SpriteEffects.FlipVertically;
            biteDrawer.BottomLeftOrigin();
            biteDrawer.drawOrigin.Y = biteDrawer.sourceRect.Value.Height - biteDrawer.drawOrigin.Y;
            biteDrawer.scale = scale;
            biteDrawer.worldPosition += offset;
            biteDrawer.worldPosition -= up * MathHelper.Lerp(1f, -0.35f, ease2);
            Main.spriteBatch.Draw(biteDrawer);
        }

        GlowingSwordMaskShader shader = GlowingSwordMaskShader.Instance;
        shader.TrailTexture = TrailRegistry.BulbTrail;
        shader.Distortion = 0.02f;
        shader.DistortionTexture = TrailRegistry.WhispyTrail;
        shader.Time = Main.GlobalTimeWrappedHourly * 16;
        shader.Bloom = ease;
        shader.Tiling = Vector2.One * 0.75f;
        shader.InnerColor = Color.Lerp(Color.LightGreen, Color.DarkTurquoise, ExtraMath.Osc(0f, 1f, 12));
        shader.OuterColor = Color.DarkTurquoise;
        for (float f = 0; f < MathHelper.TwoPi; f += 0.1f)
        {
            Vector2 off = (f + Main.GlobalTimeWrappedHourly).ToRotationVector2();
            Draw(off * 4);
        }
        for (int i = 0; i < Projectile.oldPos.Length; i++)
        {
            alpha = MathHelper.Lerp(1f, 0f, (float)i / (float)Projectile.oldPos.Length);
            Vector2 pos = Projectile.oldPos[i] + Projectile.Size * 0.5f;
            Draw(pos - Projectile.Center);
        }
        Main.spriteBatch.Restart(effect: shader.Effect);

    

        alpha = 1f;

        Draw(Main.rand.NextVector2Circular(1,1));

        Main.spriteBatch.RestartDefaults();
        return false;
    //    return base.PreDraw(ref lightColor);
    }
    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
    }
}
