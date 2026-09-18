using ReLogic.Content;
using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Content.Dusts;
using Stellamod.Core.Particles;
using Stellamod.Core.ProjectileHelpers;
using Stellamod.Core.Utilities;
using Stellamod.Helpers;
using Stellamod.Visual.Particles;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Cinderspark.BossesCS.Skullrunner.Projectiles;

public class BurningSkullTrailRenderer : PixelPrimitiveRenderer<BurningSkullTrailRenderer>
{
    public override BaseShader PrepareShader()
    {
        BloomTrailShader blackFireShader = BloomTrailShader.Instance;
        blackFireShader.InnerColor = Color.White;
        blackFireShader.OuterColor = Color.Red;
        return blackFireShader;
    }

    public override Color GetTrailColor(float completionRatio)
    {
        return Color.Lerp(Color.White, Color.Red, completionRatio);
    }

    public override float GetTrailWidth(float completionRatio)
    {
        return MathHelper.SmoothStep(7, 1, completionRatio) * MathF.Sin(Main.GlobalTimeWrappedHourly * 4 + completionRatio * 8) * 0.5f + 0.5f;
    }
}

public class BurningBlackSkull : ModProjectile
{
    private Asset<Texture2D> _outlineTextureAsset;
    private Asset<Texture2D> _whiteTextureAsset;
    private ref float Timer => ref Projectile.ai[0];
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        Main.projFrames[Type] = 3;
        ProjectileID.Sets.TrailCacheLength[Type] = 16;
        ProjectileID.Sets.TrailingMode[Type] = 2;
        ProjectileSets.ResistedByFlamecrestShield[Type] = true;
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 6;
        Projectile.height = 6;
        Projectile.penetrate = 2;
        Projectile.hostile = true;
        Projectile.timeLeft = 400;
        Projectile.tileCollide = false;
    }

    private void FlameParticles()
    {
        if (Main.rand.NextBool(3))
        {
            FlameParticle dp = Particle<FlameParticle>.Spawn(Projectile.Center, Main.rand.NextVector2Circular(4, 4),
              Scale: Main.rand.NextFloat(0.2f, 0.35f) * 0.125f);
            dp.innerColor = Color.Goldenrod;
            dp.outerColor = Color.Red;
            dp.parent = Projectile;
            dp.gravity = 0f;
            dp.dampening = 0.05f;
            dp.fast = true;
        }
        if (Main.rand.NextBool(8))
        {
            switch (Main.rand.Next(2))
            {
                case 0:
                    DustParticle sp = Particle<DustParticle>.Spawn(Projectile.Center + Main.rand.NextVector2Circular(12, 12), -Projectile.velocity.SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(0.3f, 16), Scale: Main.rand.NextFloat(0.5f, 1.5f));
                    sp.gravity = 0f;
                    sp.fast = true;
                    sp.dampening = 0.1f;
                    break;
                case 1:
                    FlameParticle sp2 = Particle<FlameParticle>.Spawn(Projectile.Center + Main.rand.NextVector2Circular(32, 32), -Projectile.velocity.SafeNormalize(Vector2.Zero)
                        * Main.rand.NextFloat(1f, 16), Scale: Main.rand.NextFloat(0.1f, 0.2f) * 0.15f);
                    sp2.gravity = 0f;
                    sp2.fast = true;
                    sp2.dampening = 0.1f;
                    break;
            }

        }

        if (Main.rand.NextBool(12))
        {
            FlameSparksParticle sp = Particle<FlameSparksParticle>.Spawn(Projectile.Center + Main.rand.NextVector2Circular(32, 32), -Projectile.velocity.SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(0.6f, 8f),
                color: Color.OrangeRed, Scale: Main.rand.NextFloat(0.35f, 0.1375f));
            sp.gravity = 0f;
            sp.fast = true;
            sp.dampening = 0.1f;
        }
    }
    public override void AI()
    {
        base.AI();
        FlameParticles();
        Timer++;
        if (Timer == 1)
        {
            Vector2 velocity = Projectile.velocity;
            Vector2 position = Projectile.Center;
            for (float f = 0; f < 16; f++)
            {
                Vector2 pVelocity = velocity.RotatedByRandom(MathHelper.PiOver4 / 3f);
                pVelocity *= Main.rand.NextFloat(0.5f, 2f);
                var frag = LegacyParticle.NewParticle<GlowFragmentParticle>(position, pVelocity);
                FXUtil.GlowFragmentParticle(position, pVelocity,
                    innerColor: Color.Red,
                    outerColor: Color.Orange,
                    fadeToColor: Color.Purple,
                    distortOut: true);

                if (Main.rand.NextBool(4))
                {
                    Dust.NewDustPerfect(position, ModContent.DustType<TSmokeDust>(),
                                     velocity.RotatedByRandom(MathHelper.PiOver4 / 2f) * 2);
                }
                if (Main.rand.NextBool(4))
                {
                    Dust.NewDustPerfect(position, ModContent.DustType<GlowDust>(),
                                     velocity.RotatedByRandom(MathHelper.PiOver4 / 2f) * 3 * Main.rand.NextFloat(0.4f, 1f), newColor: Color.White, Scale: 0.2f);
                }
                if (Main.rand.NextBool(4))
                {

                    var part = FXUtil.GlowFragmentParticle(position, pVelocity,
                     innerColor: Color.DarkRed,
                     outerColor: Color.DarkBlue,
                     fadeToColor: Color.Black,
                     distortOut: false);
                    part.Scale *= 1.3f;
                }
            }
        }

        if (Timer > 200)
        {
            Projectile.tileCollide = true;
        }

        if (Timer % 8 == 0)
        {
            if (Main.rand.NextBool(2))
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlyphDust>(), Projectile.velocity * 0.1f, 0,
                    Color.Red, Main.rand.NextFloat(0.5f, 1f)).noGravity = true;
            if (Main.rand.NextBool(2))
            {
                var dp = DustParticle.Spawn(Projectile.Center, Vector2.Zero);
                dp.noTileCollide = true;
                dp.gravity = 0;
                dp.dampening = 0.1f;
                dp.Scale *= 0.4f;
                dp.innerColor = Color.Yellow;
                dp.outerColor = Color.Red;
            }
        }

        Player target = PlayerHelper.FindClosestPlayer(Projectile.position, 1024);
        if (target != null)
        {
            Vector2 homingVelocity = ProjectileHelper.SimpleHomingVelocity(Projectile, target.Center, 4);

            //very slight lerp to this thing
            Projectile.velocity = Vector2.Lerp(Projectile.velocity, homingVelocity, 0.1f);
        }

        Projectile.rotation = Projectile.velocity.ToRotation();
        DrawHelper.AnimateTopToBottom(Projectile, 6);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        _outlineTextureAsset ??= ModContent.Request<Texture2D>(Texture + "_Outline");
        _whiteTextureAsset ??= ModContent.Request<Texture2D>(Texture + "_White");
        SpritebatchDrawer skullDrawer = SpritebatchDrawer.FromProjectile(Projectile);
        skullDrawer.scale *= MathHelper.Lerp(0.9f, 1f, ExtraMath.Osc(0f, 1f, speed: 2, Projectile.whoAmI));
        for (int i = 0; i < Projectile.oldPos.Length; i++)
        {
            SpritebatchDrawer afDrawer = skullDrawer;
            afDrawer.worldPosition = Projectile.oldPos[i] + Projectile.Size * 0.5f;
            afDrawer.rotation = Projectile.oldRot[i];

            float ratio = (float)i / (float)Projectile.oldPos.Length;
            afDrawer.scale = Vector2.Lerp(Vector2.One, Vector2.One * 0.5f, ratio);
            afDrawer.color = Color.Lerp(Color.White, Color.Transparent, ratio) * 0.3f;
         //   afDrawer.color.A = 0;
            Main.spriteBatch.Draw(afDrawer);
        }

        Main.spriteBatch.Draw(skullDrawer);

        skullDrawer.texture = _outlineTextureAsset.Value;
        skullDrawer.color = Color.Lerp(Color.Red, Color.Red * 0.5f, ExtraMath.Osc(0f, 1f, speed: 16, offset: Projectile.whoAmI));
        Main.spriteBatch.Draw(skullDrawer);

        skullDrawer.texture = _whiteTextureAsset.Value;
        skullDrawer.color = Color.Lerp(Color.Red * 0.1f, Color.Red * 0.3f, ExtraMath.Osc(0f, 1f, speed: 16, offset: Projectile.whoAmI));
        Main.spriteBatch.Draw(skullDrawer);

        SpritebatchDrawer glowDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.SimpleGlowCircle, Projectile.Center);
        glowDrawer.color = Color.Lerp(Color.Yellow * 0.2f, Color.Yellow * 0.5f, ExtraMath.Osc(0f, 1f, speed: 16, offset: Projectile.whoAmI)) * 0.3f;
        glowDrawer.color.A = 0;
        glowDrawer.scale *= .35f;
        Main.spriteBatch.Draw(glowDrawer);

        Vector2[] pos = new Vector2[Projectile.oldPos.Length];
        for(int i = 0; i < pos.Length; i++)
        {
            pos[i] = Projectile.oldPos[i] + Projectile.Size * 0.5f;
        }
        BurningSkullTrailRenderer.Queue(pos);
        return false;
    }

    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
        for (int i = 0; i < 32; i++)
        {
            Dust.NewDustPerfect(Projectile.Center, DustID.Torch, (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.Yellow, 1f).noGravity = true;
        }
        for (int i = 0; i < 3; i++)
        {
            Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.Yellow, 1f).noGravity = true;
        }
        for (int i = 0; i < 3; i++)
        {
            Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<TSmokeDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.Orange, 1f).noGravity = true;
        }

        SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/MorrowExp"), Projectile.position);
        FXUtil.GlowCircleBoom(Projectile.Center,
            innerColor: Color.White,
            glowColor: Color.Yellow,
            outerGlowColor: Color.Red,
            duration: Main.rand.Next(10, 25),
            baseSize: Main.rand.NextFloat(0.05f, 0.16f));
    }
}
