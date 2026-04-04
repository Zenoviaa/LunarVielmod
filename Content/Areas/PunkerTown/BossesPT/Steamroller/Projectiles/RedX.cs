using Stellamod.Assets;
using Stellamod.Core.Pixelation;
using Stellamod.Helpers;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.PunkerTown.BossesPT.Steamroller.Projectiles;

public class RedX : ModProjectile
{
    private ref float Timer => ref Projectile.ai[0];
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 16;
        Projectile.height = 16;
        Projectile.friendly = false;
        Projectile.tileCollide = false;
        Projectile.penetrate = -1;
        Projectile.timeLeft = 90;
    }

    public override void AI()
    {
        base.AI();
        Timer++;
        if (Timer == 1)
        {
            SoundStyle warningSound = AssetRegistry.Sounds.SteamPunking.MechSawRevUp;
            warningSound.PitchVariance = 0.3f;
            warningSound.Volume = 0.3f;
            SoundEngine.PlaySound(warningSound, Projectile.position);
            float numDust = 8;
            for (float n = 0; n < numDust; n++)
            {
                float radians = (n / numDust) * MathHelper.TwoPi;
                Vector2 offset = radians.ToRotationVector2();
                offset *= 64;
                Vector2 pos = Projectile.Center + offset;
                DustParticleSpawnParams spawnParams = DustParticleSpawnParams.Default;
                var dp = DustParticle.Spawn(pos, Vector2.Zero, spawnParams);
                dp.noTileCollide = true;
                dp.fast = true;
                dp.dampening = 0.1f;
                dp.gravity = 0;
            }
        }
    }


    private void DrawPixelatedX(SpriteBatch sb, Vector2 screenPos)
    {
        float easeIn = EasingFunction.InOutSine(Timer / 30f);
        float easeOut = EasingFunction.InOutSine((float)Projectile.timeLeft / 30f);
        SpritebatchDrawer drawer = SpritebatchDrawer.FromProjectile(Projectile);
        drawer.color = Color.Yellow * 0.65f * easeIn * easeOut;
        drawer.color.A = 0;


        Vector2 scale = Vector2.Lerp(Vector2.One * 0.5f, Vector2.One, easeIn * easeOut);
        drawer.scale = scale;

        SpritebatchDrawer glowDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.SimpleGlowCircle, Projectile.Center);
        glowDrawer.color = Color.Yellow * 0.25f * easeIn * easeOut;
        glowDrawer.color.A = 0;
        glowDrawer.scale = scale * ExtraMath.Osc(0.6f, 1f, speed: 8);
        Main.spriteBatch.Draw(drawer);
        Main.spriteBatch.Draw(glowDrawer);

        Vector2 offset = Vector2.Lerp(Vector2.Zero, -Vector2.UnitX * 64, easeOut);
        SpritebatchDrawer lineDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.GradientPillar, Projectile.Center + offset);
        lineDrawer.color = Color.Yellow * 0.65f * easeIn * easeOut;
        lineDrawer.color.A = 0;
        lineDrawer.scale.X *= 0.04f;
        lineDrawer.scale.Y *= 4;

        Main.spriteBatch.Draw(lineDrawer);
        lineDrawer.worldPosition = Projectile.Center - offset;
        Main.spriteBatch.Draw(lineDrawer);
    }
    public override bool PreDraw(ref Color lightColor)
    {
        PixelationManager.QueueSpritebatchDrawAction(DrawPixelatedX);
        return false;
    }

    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
    }
}
