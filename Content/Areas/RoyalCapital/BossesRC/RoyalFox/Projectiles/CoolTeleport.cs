using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Core.Palettes;
using Stellamod.Core.Pixelation;
using Stellamod.Core.Utilities;
using Stellamod.Effects.RoyalMagic;
using Stellamod.Helpers;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.RoyalCapital.BossesRC.RoyalFox.Projectiles;

public class CoolTeleport : ModProjectile,
    IDrawToRenderTarget
{
    private float Time => 25;
    private ref float Timer => ref Projectile.ai[0];
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 128;
        Projectile.height = 64;
        Projectile.timeLeft = (int)Time;
        Projectile.penetrate = -1;
        Projectile.tileCollide = false;
        Projectile.ignoreWater = true;
    }

    public override void AI()
    {
        base.AI();
        Timer++;
        if (Timer == 1)
        {
            RoyalFox.PlayAirbounceSuond(Projectile.position);
            PixelPrimitiveCircleFactory.CreateGenericInBoom(Projectile.Center, Color.White, Color.Transparent, 45, 384);
            for (float f = 0; f < 4; f++)
            {
                Vector2 vel = Vector2.UnitX * Main.rand.NextFloat(-30f, 30f);
                var d = DustParticle.Spawn(Projectile.Center, vel);
                d.noTileCollide = true;
                d.dampening = 0.1f;
                d.gravity = 0;
                d.outerColor = Color.Blue;
            }
            for(int i = 0; i < 16; i++)
            {
                Vector2 pos = Projectile.Center + Main.rand.NextVector2Circular(64, 256);
                var sp = RoyalMagicSwordParticle.Spawn(pos, -Vector2.UnitY * 4);
                sp.color = Color.Blue;
            }
        }
        ShakeScreenPosition.Shake = MathHelper.Lerp(2, 0, EasingFunction.InExpo(Timer / Time));
        if (ModContent.GetInstance<LunarVeilClientConfig>().DramaticEffects)
        {
            SpecialEffectsPlayer effectsPlayer = Main.LocalPlayer.GetModPlayer<SpecialEffectsPlayer>();
           // effectsPlayer.darknessCurve = MathHelper.Lerp(0.6f, 0f, EasingFunction.InExpo(Timer / Time));
        }
        if (Main.rand.NextBool(8))
        {
            var d = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(64, 64), DustID.GemDiamond);
            d.noGravity = true;
        }
    }

    private void DrawCoolTeleport(SpriteBatch sb, Vector2 sp)
    {
        var teleportFadeShader = ShaderContent.GetInstance<TeleportFadeShader>();
        sb.Restart(effect: teleportFadeShader.Effect);
        SpritebatchDrawer drawer = SpritebatchDrawer.FromProjectile(Projectile);
        drawer.color = Color.Blue;
        drawer.scale = Vector2.Lerp(Vector2.One, new Vector2(1f, 0f), EasingFunction.InOutExpo(Timer / Time)) * new Vector2(MathHelper.Lerp(10f, 3f, EasingFunction.OutExpo(Timer / Time)), 1f);

        sb.Draw(drawer);

        drawer.rotation = MathHelper.PiOver2;
        drawer.scale.Y *= MathHelper.Lerp(4f, 2f, EasingFunction.OutExpo(Timer / Time));
        drawer.scale.X *= MathHelper.Lerp(4f, 1f, EasingFunction.OutExpo(Timer / Time));
        sb.Draw(drawer);
        sb.RestartDefaults();

        SpritebatchDrawer flashDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.StarFlare2, Projectile.Center);
        flashDrawer.color = Color.White;
        flashDrawer.color.A = 0;
        flashDrawer.scale = Vector2.Lerp(Vector2.One * 1.5f, Vector2.Zero, EasingFunction.OutExpo(Timer / Time));
        sb.Draw(flashDrawer);


    }

    public void DrawToRenderTargets()
    {
        PixelationManager.QueueSpritebatchDrawAction(DrawCoolTeleport, DrawLayer.OverPlayers);
    }
}
