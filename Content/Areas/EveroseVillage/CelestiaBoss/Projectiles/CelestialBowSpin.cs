using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Core.Pixelation;
using Stellamod.Helpers;
using Stellamod.Trails;
using Stellamod.Visual.Particles;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.EveroseVillage.CelestiaBoss.Projectiles;

public class CelestialBowSpin : ModProjectile
{
    private Vector2 _mirageOffset;
    private ref float Timer => ref Projectile.ai[0];
    private NPC Parent
    {
        get
        {
            int parentIndex = (int)Projectile.ai[1];
            //Genuinely so confused how this even happens
            if (parentIndex < 0 || parentIndex >= Main.npc.Length)
                return Main.npc[0];
            NPC parent = Main.npc[parentIndex];
            return parent;
        }
    }
    private ref float Style => ref Projectile.ai[2];
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        ProjectileID.Sets.TrailCacheLength[Type] = 24;
        ProjectileID.Sets.TrailingMode[Type] = 2;
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 80;
        Projectile.height = 80;
        Projectile.hostile = false;
        Projectile.timeLeft = 120;
        Projectile.penetrate = -1;
        Projectile.tileCollide = false;
    }
    public override bool ShouldUpdatePosition()
    {
        return false;
    }

    public override void AI()
    {
        base.AI();
        Timer++;
        if(Timer == 1 && Style == 0)
        {
            SoundStyle throwSound = AssetRegistry.Sounds.Celestia.CelestiaBowThrow with { PitchVariance = 0.3f };
            SoundEngine.PlaySound(throwSound, Projectile.position);
        }
        if (Style == 1 && Projectile.timeLeft > 90)
            Projectile.timeLeft = 90;
        if (Timer % 4 == 0)
        {
            _mirageOffset = Main.rand.NextVector2Circular(4, 4);
        }

        if (Timer % 4 == 0)
        {
            Vector2 pos = Projectile.Center + Main.rand.NextVector2Circular(48, 48);
            var d = Dust.NewDustPerfect(pos, DustID.GemEmerald, Scale: 1f);
            d.noGravity = true;
        }

        if (Timer % 6 == 0)
        {

            SparkleParticle sp = SparkleParticle.Spawn(Projectile.Center + Main.rand.NextVector2Circular(32, 32), Projectile.velocity.RotatedBy(MathHelper.ToRadians(30) * 0.1f));
            sp.Scale *= 0.5f;
            sp.flickering = true;
            sp.outerColor = Color.Turquoise;
            sp.noTileCollide = true;
            sp.gravity = 0;
            sp.dampening = 0.05f;

        }

        if (Style == 0)
        {
       
            Vector2 maxOffset = Projectile.velocity * 400;

            float ratio = Timer / 100f;
            float ratio2 = Timer / 80f;
            float ease1 = MathHelper.Lerp(0f, 1f, EasingFunction.OutExpo(ratio2));

            float rotIncrease = MathHelper.Lerp(0.15f, 0.25f, ease1);
            Projectile.rotation += rotIncrease;

            float easeBack = MathHelper.Lerp(1f, 0f, EasingFunction.InExpo(ratio));
            Vector2 offset = Vector2.Lerp(Vector2.Zero, maxOffset, ease1 * easeBack);
            Projectile.Center = Parent.Center + offset;
            Projectile.hostile = true;

        } else if (Style == 1)
        {
            Projectile.rotation = new Vector2(MathF.Sign(-Projectile.velocity.X), 1).ToRotation();
            Vector2 pos = Parent.Center + Projectile.rotation.ToRotationVector2() * 40;
            pos.Y -= 8;
            Projectile.Center = pos;
        }


        //        Projectile.Center = Parent.Center;
    }

    private void DrawCelestialTrail(GraphicsDevice gDevice)
    {
        BlackFireShader laserShader =BlackFireShader.Instance;
        laserShader.Tiling = new Vector2(1f, 1f);
        laserShader.PrimaryTexture = TrailRegistry.WhispyTrail;
        laserShader.BloomTexture = TrailRegistry.WhispyTrail;
        laserShader.InnerColor = Color.Turquoise;
        laserShader.OuterColor = Color.Lerp(Color.Turquoise, Color.Black, 0.85f);
        TrailDrawer.Draw(Main.spriteBatch, Projectile.oldPos, GetTrailColor, GetTrailWidth, laserShader, Projectile.Size * 0.5f);


        BloomTrailShader bloomTrail = BloomTrailShader.Instance;
        bloomTrail.InnerColor = Color.Turquoise;
        bloomTrail.OuterColor = Color.Black;
        TrailDrawer.Draw(Main.spriteBatch, Projectile.oldPos, GetTrailColor, GetTrailWidth2, bloomTrail, Projectile.Size * 0.5f);
    }

    private float GetTrailWidth(float completionRatio)
    {
        float outAlpha = EasingFunction.Clamp((float)Projectile.timeLeft / 60f);
        return MathHelper.SmoothStep(32, 0, completionRatio) * outAlpha;
    }
    private float GetTrailWidth2(float completionRatio)
    {
        return GetTrailWidth(completionRatio) * 2f;
    }

    private Color GetTrailColor(float completionRatio)
    {
        return Color.Lerp(Color.Transparent, Color.White, EasingFunction.QuadraticBump(completionRatio));
    }
    private void DrawPixelatedBows(SpriteBatch sb, Vector2 screenPos)
    {
        float alpha = EasingFunction.InSine(Timer / 30f);
        if (Style == 0)
            alpha = 1f;
        alpha *= (float)(EasingFunction.Clamp(Projectile.timeLeft / 30f));
        Vector2 pullScale = Vector2.One;
        pullScale *= MathHelper.Lerp(1.45f, 1f, EasingFunction.InSine(Timer / 30f));
        pullScale *= 0.65f;

        float come = EasingFunction.QuadraticBump(Timer / 60f);

        SpritebatchDrawer backGlowDrawer = SpritebatchDrawer.FromTextureAsset(ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/BasicGlow"), Projectile.Center); ;
        backGlowDrawer.scale *= pullScale * 2;
        backGlowDrawer.color = Color.Black * 0.5f * alpha;
        Main.spriteBatch.Draw(backGlowDrawer);

        SpritebatchDrawer glowDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.SimpleGlowCircle, Projectile.Center); ;
        glowDrawer.scale *= pullScale * 0.5f;
        glowDrawer.color = Color.Lerp(Color.Teal, Color.LightGreen, ExtraMath.Osc(0f, 1f, 0, Projectile.whoAmI)) * 0.2f * alpha;
        glowDrawer.color.A = 0;
        Main.spriteBatch.Draw(glowDrawer);

        SpritebatchDrawer spiralVortexDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.SpiralVortex, Projectile.Center); ;
        spiralVortexDrawer.scale *= pullScale * 0.5f;
        spiralVortexDrawer.color = Color.Lerp(Color.Teal, Color.LightGreen, ExtraMath.Osc(0f, 1f, 0, Projectile.whoAmI)) * 0.1f * alpha;
        spiralVortexDrawer.color.A = 0;
        spiralVortexDrawer.rotation = Main.GlobalTimeWrappedHourly;
        Main.spriteBatch.Draw(spiralVortexDrawer);

        SpritebatchDrawer bowDrawer = SpritebatchDrawer.FromProjectile(Projectile);
        bowDrawer.scale *= pullScale;
        bowDrawer.color = Color.Lerp(Color.Teal, Color.LightGreen, ExtraMath.Osc(0f, 1f, 0, Projectile.whoAmI)) * 0.5f * alpha;
        bowDrawer.color.A = 0;
        Main.spriteBatch.Draw(bowDrawer);


        bowDrawer.worldPosition -= Projectile.rotation.ToRotationVector2() * 8;
        bowDrawer.worldPosition += _mirageOffset;
        bowDrawer.color =
            Color.Lerp(Color.DarkTurquoise, Color.DarkGreen, ExtraMath.Osc(0f, 1f, 0, Projectile.whoAmI)) * 0.2f * alpha;
        bowDrawer.color.A = 0;
        bowDrawer.scale *= 1.3f;
        Main.spriteBatch.Draw(bowDrawer);

        float lineOut = Timer / 60f;
        lineOut = EasingFunction.InOutSine(lineOut);
        float lineOutAlpha = MathHelper.Lerp(1f, 0f, lineOut);
        SpritebatchDrawer bloomlineDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.SimpleGlowCircle, Parent.Center);
        bloomlineDrawer.color = Color.LightGreen * alpha * lineOutAlpha;
        bloomlineDrawer.color.A = 0;

        float dist = Vector2.Distance(Projectile.Center, Main.player[Parent.target].Center);
        float bloomLineSize = dist / (float)bloomlineDrawer.texture.Width;
        bloomlineDrawer.scale.X *= bloomLineSize;
        bloomlineDrawer.scale.Y *= 0.025f;
        bloomlineDrawer.LeftCenterOrigin();
        bloomlineDrawer.drawOrigin.X += 64;
        bloomlineDrawer.rotation = Projectile.velocity.ToRotation();
        Main.spriteBatch.Draw(bloomlineDrawer);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        PixelationManager.QueuePrimitivesDrawAction(DrawCelestialTrail, DrawLayer.BehindNPCsWithOutline);
        PixelationManager.QueueSpritebatchDrawAction(DrawPixelatedBows);
        return false;
    }
}