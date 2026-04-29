using ReLogic.Content;
using Stellamod.Assets;
using Stellamod.Core.Camera;
using Stellamod.Core.Pixelation;
using Stellamod.Helpers;
using Stellamod.UI.Systems;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.MoonspiralTower.VerliaBoss.Projectiles;

public class VerliaDesperationMoon : ModProjectile
{
    private ref float Timer => ref Projectile.ai[0];
    private float _scale;
    private float _flashAlpha;
    private Asset<Texture2D> _outlineTextureAsset;
    private Asset<Texture2D> _scrollingMoonTextureAsset;
    private Asset<Texture2D> _shadowMoonTextureAsset;
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
        _flashAlpha = 1f;
        Projectile.width = 192;
        Projectile.height = 192;
        Projectile.hostile = true;
        Projectile.timeLeft = 1800;
        Projectile.penetrate = -1;
        Projectile.tileCollide = false;
    }
    public override void AI()
    {
        base.AI();
        Timer++;
        if(Timer == 1)
        {
            SoundStyle inSound = new SoundStyle($"Stellamod/Assets/Sounds/StarCharge");
         //   inSound.PitchVariance = 0.3f;
            SoundEngine.PlaySound(inSound, Projectile.position);
        }
        if (Timer >= 60 && Timer < 600)
        {
            int divisor = (int)MathHelper.Lerp(30, 10, EasingFunction.InOutSine(Timer / 400));
            if (Timer % divisor == 0)
            {
                _flashAlpha = 1f;
                if (this.OwnedByLocalClient())
                {
                    Player player = PlayerHelper.FindClosestPlayer(Projectile.Center, 4000);
                    if (player != null)
                    {
                        Vector2 velocity = player.Center - Projectile.Center;
                        velocity = velocity.SafeNormalize(Vector2.Zero);

                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center + velocity * 192, velocity * 15,
                            ModContent.ProjectileType<MoonSnipe>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                    }

                }
            }
        }
        else if (Timer > 600)
        {
            CameraTargetSystem.AddTarget(Projectile.Center);
            ShakeModSystem.Shake = 2;
            Projectile.tileCollide = true;
            if (Projectile.velocity.Y < 5)
            {
                Projectile.velocity.Y += 0.2f;
            }
            _flashAlpha = MathHelper.Lerp(0f, 1f, EasingFunction.InOutSine((Timer - 600f) / 60f));
        }
        _scale = MathHelper.Lerp(0f, 1f, EasingFunction.InOutSine(Timer / 60f));
        _flashAlpha = MathHelper.Lerp(_flashAlpha, 0f, 0.1f);
    }
    private void DrawPixelatedMoon(SpriteBatch sb, Vector2 screenPos)
    {
        Vector2 scale = Vector2.One * _scale;
        SpritebatchDrawer moonSprite = SpritebatchDrawer.FromProjectile(Projectile);
        _scrollingMoonTextureAsset ??= ModContent.Request<Texture2D>(Texture + "_ScrollingMoon");
        moonSprite = SpritebatchDrawer.FromProjectile(Projectile);
        SpritebatchDrawer glowDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.SimpleGlowCircle, Projectile.Center);
        glowDrawer.color = Color.Blue * 0.8f * ExtraMath.Osc(0.5f, 1f, speed: 6);
        glowDrawer.color.A = 0;
        glowDrawer.scale *= 1.8f;
        glowDrawer.scale *= scale;
        Main.spriteBatch.Draw(glowDrawer);



        glowDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.StarFlare1, Projectile.Center);
        glowDrawer.color = Color.SkyBlue * 0.2f;
        glowDrawer.color.A = 0;
        glowDrawer.scale.X *= 1.2f;
        glowDrawer.scale.Y *= 0.6f;
        glowDrawer.scale *= scale;
        Main.spriteBatch.Draw(glowDrawer);


        ScrollingMoonShader scrollingMoonShader = ScrollingMoonShader.Instance;
        scrollingMoonShader.ScrollingTexture = _scrollingMoonTextureAsset.Value;
        scrollingMoonShader.MaskSize = TextureAssets.Projectile[Type].Value.Size();

        float time = Main.GlobalTimeWrappedHourly * 0.6f * 1;
        time += Projectile.whoAmI * 0.5f;
        scrollingMoonShader.ScrollOffset = new Vector2(time, 0f);
        scrollingMoonShader.BendStrength = 1.8f;
        scrollingMoonShader.Tiling = new Vector2(0.13f, 0.45f);


        //Draw the moon itself
        sb.Restart(effect: scrollingMoonShader.Effect);
        moonSprite.rotation = MathHelper.ToRadians(-12);
        moonSprite.color = Color.Lerp(Color.White, Color.LightSkyBlue, ExtraMath.Osc(0f, 0.3f, speed: 8));
        moonSprite.scale *= scale;
        Main.spriteBatch.Draw(moonSprite);
        sb.RestartDefaults();


        glowDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.SolarRing, Projectile.Center);
        glowDrawer.color = Color.SkyBlue * 0.6f;
        glowDrawer.color.A = 0;
        glowDrawer.scale *= 0.5f;
        glowDrawer.scale *= scale * 3f;
        Main.spriteBatch.Draw(glowDrawer);


        moonSprite.color = Color.Lerp(Color.Transparent, Color.White, _flashAlpha);
        Main.spriteBatch.Draw(moonSprite);
    }
    public override bool PreDraw(ref Color lightColor)
    {
        Vector2 scale = Vector2.One * _scale;
        _outlineTextureAsset ??= ModContent.Request<Texture2D>(Texture + "_Outline");
        _scrollingMoonTextureAsset ??= ModContent.Request<Texture2D>(Texture + "_ScrollingMoon");
        _shadowMoonTextureAsset ??= ModContent.Request<Texture2D>(Texture + "_Shadow");

        SpritebatchDrawer shadowDrawer = SpritebatchDrawer.FromTextureAsset(_shadowMoonTextureAsset, Projectile.Center);
        shadowDrawer.color *= 0.45f;
        Main.spriteBatch.Draw(shadowDrawer);

        SpritebatchDrawer outlineDrawer = SpritebatchDrawer.FromTextureAsset(_outlineTextureAsset, Projectile.Center);
        outlineDrawer.color = Color.Red;
        outlineDrawer.scale *= scale;
        Main.spriteBatch.Draw(outlineDrawer);
        PixelationManager.QueueSpritebatchDrawAction(DrawPixelatedMoon);
        return false;
    }
    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
        if (this.OwnedByLocalClient())
        {
            float numBlades = 12;
            for (float f = 0; f < numBlades; f++)
            {
                float ratio = f / numBlades;
                Vector2 vel = (ratio * MathHelper.TwoPi).ToRotationVector2();
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center + vel * 128, vel * 15, ModContent.ProjectileType<MoonBlade>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
            }

            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero,
                ModContent.ProjectileType<VerliaBouncingMoonShockwave>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero,
                ModContent.ProjectileType<VerliaBouncingMoonBoom>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
        }
        var fx = FXUtil.GlowCircleBoom(Projectile.Center, Color.White, Color.SkyBlue, Color.DarkBlue);
        fx.Scale *= 8f;
        float numDust = 32;
        for (float f = 0; f < numDust; f++)
        {
            DustParticleSpawnParams spawnParams = DustParticleSpawnParams.Default;
            spawnParams.outerColor = Color.Blue;
            spawnParams.scaleRange *= 2;
            var dp = DustParticle.Spawn(Projectile.Center, Main.rand.NextVector2Circular(16, 16), spawnParams);
            dp.fast = true;
            dp.noTileCollide = true;
            dp.dampening = 0.05f;
        }
    }
}
