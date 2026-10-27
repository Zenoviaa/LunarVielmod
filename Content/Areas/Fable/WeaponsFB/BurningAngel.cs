using Stellamod.Common;
using Stellamod.Common.Particles;
using Stellamod.Common.ShockCircleSystem;
using Stellamod.Content.Areas.Cinderspark.WeaponsCS;
using Stellamod.Content.CommonMaterials;
using Stellamod.Core.Bases;
using Stellamod.Core.Pixelation;
using Stellamod.Core.SwingSystem;
using Stellamod.Items;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Fable.WeaponsFB;

[Autoload(Side = ModSide.Client)]
public class BurningBubbleRenderer : ModSystem
{
    public static readonly List<Action<SpriteBatch, Vector2>> DrawActions = new();
    public override void Load()
    {
        base.Load();
        On_Main.CheckMonoliths += RenderBubbles;
    }

    private void RenderBubbles(On_Main.orig_CheckMonoliths orig)
    {
        orig();
        if (DrawActions.Count <= 0)
            return;

        //       throw new NotImplementedException();
        PixelationManager.QueueSpritebatchDrawAction(DrawFlames);
    }

    void DrawFlames(SpriteBatch sb, Vector2 sp)
    {
        var pass = AssetReferences.Effects.Fable.BubbleFlame.CreatePixelPass();
        pass.Parameters.time = Main.GlobalTimeWrappedHourly;
        var noise = AssetReferences.Assets.NoiseTextures.PerlinNoise.Asset.Value;
        pass.Parameters.noiseSampler = new()
        {
            Sampler = SamplerState.LinearWrap,
            Texture = noise
        };
        pass.Parameters.spriteSize = AssetReferences.Assets.NoiseTextures.CandleFlame.Asset.Size();
        pass.Parameters.noiseTexelSize = noise.GetTexelSize();
        pass.Parameters.flameInnerColor = Color.Yellow.ToVector3();
        pass.Parameters.flameOuterColor = Color.Red.ToVector3();
        pass.Apply();
        using (sb.Ctx(sb.Parameters with { effect = pass.Shader }))
        {
            for (var i = 0; i < DrawActions.Count; i++)
            {
                DrawActions[i](sb, sp);
            }
            DrawActions.Clear();
        }
    }
}
public class BurningAngel : BaseSwingItemV2
{
    public override string Texture => AssetReferences.Assets.Textures.Fable.Items.BurningAngel.KEY;
    public override void SetDefaults2()
    {
        base.SetDefaults2();
        Item.damage = 12;
        Item.shoot = ModContent.ProjectileType<BurningAngelSlash>();
        staminaProjectileShoot = ModContent.ProjectileType<BurningAngelStaminaSlash>();
        meleeWeaponType = MeleeWeaponType.Hammer;
        staminaCost = 2;
    }

    public override Vector2? HoldoutOffset()
    {
        return new Vector2(2f, -2f);
    }

    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew(mold: ModContent.ItemType<BlankJuggler>(),
            material: ModContent.ItemType<AlcadizScrap>());
    }
}

public class BurningAngelBubbleBurst : ModProjectile
{
    float Time => 30;
    float OutEase => EasingFunction.OutCirc(Timer / Time);
    ref float Timer => ref Projectile.ai[0];
    public override string Texture => AssetReferences.Assets.Textures.Fable.Items.BurningAngelBubbleBurst.KEY;
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 128;
        Projectile.height = 128;
        Projectile.LocalHitOnce = true;
        Projectile.timeLeft = (int)Time;
        Projectile.ignoreWater = true;
        Projectile.tileCollide = false;
        Projectile.friendly = true;
        Projectile.light = 0.6f;
    }

    public override void AI()
    {
        base.AI();
        Timer++;
        if (Timer == 1)
        {
            var pop = SoundID.Item54 with { PitchVariance = 0.6f };
            SoundEngine.PlaySound(pop, Projectile.position);
            ShockCircles.CreateSmallerQuickWhiteFlash(Projectile.Center);
            foreach (PositionVelocity posVel in new CircleOrientation(Projectile.Center, 32, 12))
            {
                Particles.SwirlingFlameDust.Spawn(BitDustFactory.SlowingOverTime with
                {
                    position = posVel.position,
                    velocity = posVel.velocity * Main.rand.NextFloat(0.3f, 0.6f) * 0.4f,
                    innerColor = Color.White.ToVector4(),
                    outerColor = Color.Gold.ToVector4(),
                    scale = new Vector2(Main.rand.NextFloat(0.3f, 0.6f)),
                    timeLeft = 40
                });
            }
        }
    }

    public override bool PreDraw(ref Color lightColor)
    {
        var drawer = Projectile.Drawer;
        drawer.color *= MathHelper.Lerp(1f, 0f, OutEase);
        drawer.scale *= MathHelper.Lerp(1f, 1.2f, OutEase) * 0.5f;
        Main.spriteBatch.Draw(drawer);
        return false;
    }
}
public class BurningAngelBubble : ModProjectile
{
    public override string Texture => AssetReferences.Assets.Textures.Fable.Items.BurningAngelBubble.KEY;
    ref float Timer => ref Projectile.ai[0];
    ref float Scale => ref Projectile.ai[1];
    ref float ExplosionTime => ref Projectile.ai[2];
    float InScale => EasingFunction.InOutSine(Timer / 30f);

    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        Projectile.SetTrailCacheLength(4);
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 12;
        Projectile.height = 12;
        Projectile.LocalPiercingImmunityTime = 30;
        Projectile.timeLeft = 60;
        Projectile.ignoreWater = true;
        Projectile.light = 0.6F;
    }
    public override void AI()
    {
        base.AI();
        Timer++;
        if (Timer == 1 && this.OwnedByLocalClient())
        {
            Scale = Main.rand.NextFloat(0.6f, 1f);
            ExplosionTime = Main.rand.NextFloat(30, 60);
            Projectile.netUpdate = true;
        }
        if (Main.rand.NextBool(16))
        {
            var pos = Projectile.Center + Main.rand.NextVector2Circular(24, 24);
            var vel = Main.rand.NextVector2Circular(2, 2);
            //   Particles.RagingFlameDust.Spawn(RagingFlameDustData.Default with { position = pos, timeleft = 65, style = 2 });
        }
        if (Timer % 12 == 0)
        {
            var pos = Projectile.Center + Main.rand.NextVector2Circular(48, 48);
            var vel = Main.rand.NextVector2Circular(2, 2);
            Particles.SwirlingFlameDust.Spawn(BitDustFactory.SlowingOverTime with
            {
                position = pos,
                velocity = vel,
                innerColor = Color.Yellow.ToVector4(),
                outerColor = Color.Red.ToVector4(),
                scale = new Vector2(Main.rand.NextFloat(0.3f, 0.6f)),
                timeLeft = 40
            });
        }
        if (Timer >= ExplosionTime)
            Projectile.Kill();
        Projectile.velocity.X = MathHelper.Lerp(Projectile.velocity.X, MathF.Sin(Timer * 0.3f) * 0.3f, 0.07f);
        Projectile.velocity.Y = MathHelper.Lerp(Projectile.velocity.Y, -0.25f, MathHelper.Lerp(0.02f, 0.05f, MathF.Sin(Projectile.identity * 4) * 0.5f + 0.5f));
        Projectile.rotation = 0;
    }

    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
        if (this.OwnedByLocalClient())
        {
            var firer = ProjFirer.From<BurningAngelBubbleBurst>(Projectile);
            firer.New();
        }
    }
    void DrawPixelated(SpriteBatch sb, Vector2 sp)
    {
        // var sb = Main.spriteBatch;
        var drawer = Projectile.Drawer;
        drawer.color = drawer.color.MultiplyRGB(Color.LightGoldenrodYellow);
        drawer.scale = new Vector2(Scale);
        drawer.scale *= MathHelper.Lerp(0.2f, 1f, InScale);
        drawer.scale.X *= MathHelper.Lerp(0.8f, 1f, ExtraMath.Osc(0f, 1f, speed: 10, Projectile.identity));//
        drawer.scale.Y *= MathHelper.Lerp(0.6f, 1f, ExtraMath.Osc(0f, 1f, speed: 5, Projectile.identity));
        sb.Draw(drawer);

        drawer.color = Color.Lerp(Color.Transparent, Color.White, ExtraMath.Osc(0f, 0.5f, speed: 12, offset: Projectile.identity));
        drawer.color.A = 0;
        sb.Draw(drawer);
    }

    void DrawFlame(SpriteBatch sb, Vector2 sp)
    {
        var drawer = SpritebatchDrawer.FromTextureAsset(AssetReferences.Assets.NoiseTextures.CandleFlame.Asset, Projectile.Center);
        drawer.scale = new Vector2(Scale);
        drawer.scale *= MathHelper.Lerp(0.2f, 1f, InScale);
        drawer.scale.X *= MathHelper.Lerp(0.9f, 1f, ExtraMath.Osc(0f, 1f, speed: 10, Projectile.identity));//
        drawer.scale.Y *= MathHelper.Lerp(0.8f, 1f, ExtraMath.Osc(0f, 1f, speed: 5, Projectile.identity));
        drawer.scale *= 0.2f;
        drawer.BottomCenterOrigin();

        drawer.color = Color.Lerp(Color.Transparent, Color.White, ExtraMath.Osc(0.35f, 0.75f, speed: 12, offset: Projectile.identity));
        sb.Draw(drawer);
    }
    public override bool PreDraw(ref Color lightColor)
    {
        PixelationManager.QueueSpritebatchDrawAction(DrawPixelated);
        BurningBubbleRenderer.DrawActions.Add(DrawFlame);
        return false;
    }

    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        if (Projectile.velocity.X != oldVelocity.X)
            Projectile.velocity.X = -oldVelocity.X;
        if (Projectile.velocity.Y != oldVelocity.Y)
            Projectile.velocity.Y = -oldVelocity.Y;
        return false;
    }
}

public class BurningAngelStaminaSlash : BaseSwingProjectileV2
{
    float _t;
    private FireTrailRenderer _fireTrailRenderer;
    private float _hitCount;
    private bool _hit;
    public override void DefineCombo()
    {
        base.DefineCombo();
        outlineColor = Color.Yellow;

        //Bloom


        useAfterImage = true;
        glowAfterImageColor = Color.Red;


        SoundStyle hammerSlash1 = SoundRegistry.HeavySwordSlash1;
        hammerSlash1.PitchVariance = 0.2f;

        SoundStyle hammerSlash2 = SoundRegistry.HeavySwordSlash2;
        hammerSlash2.PitchVariance = 0.2f;

        Add(new OvalSwing
        {
            Duration = 66,
            SwingDegrees = 310 * 3,
            XSwingRadius = 64,
            YSwingRadius = 34,
            Easing = (float lerpValue) => EasingFunction.InOutSine(lerpValue / 0.5f) * lerpValue,
            Sound = hammerSlash1,
            HitCount = 2
        });


        useAfterImage = true;
        hitStopTime = 4 * EXTRA_UPDATE_COUNT;

        useBloom = true;
        bloom.innerBloomColor = Color.OrangeRed;
        bloom.outerBloomColor = Color.DarkRed;
        bloom.bloomWidthFunction = GetBloomWidth;
        bloom.bloomColorFunction = GetBloomColor;

        _fireTrailRenderer = new FireTrailRenderer();
        _fireTrailRenderer.SlashTrailer.TrailWidthFunction = GetTrailWidth;
        additive = true;
        Trailer = _fireTrailRenderer.SlashTrailer;
        useAfterImage = true;
        glowAfterImageColor = Color.Red;

    }

    public float GetTrailWidth(float interpolant)
    {
        return MathHelper.SmoothStep(4, 24, interpolant) * MathF.Sin(interpolant * 8);
    }

    private float GetBloomWidth(float ratio)
    {
        return MathHelper.SmoothStep(8, 32, ratio) * 1.5f * MathHelper.SmoothStep(1f, 0f, EasingFunction.InExpo(Interpolant));
    }

    private Color GetBloomColor(float ratio)
    {
        return Color.Lerp(Color.Red * 0.9f, Color.Transparent, EasingFunction.InExpo(ratio));
    }



    public override void AI()
    {
        base.AI();
        _fireTrailRenderer.SetDefaults();
        if (Timer == 1)
        {
            var fireShood = AssetReferences.Assets.Sounds.Fire.FireballShoot1.Asset with { PitchVariance = 0.5f };
            SoundEngine.PlaySound(fireShood, Projectile.position);
        }
        _t++;
        if (_t % 35 == 0)
        {
            if (this.OwnedByLocalClient())
            {
                var firer = ProjFirer.From<BurningAngelBubble>(Projectile);
                firer.position = Owner.Center;
                firer.velocity = Projectile.velocity.SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(0, 0.8f);
                firer.velocity *= 32;
                firer.velocity = firer.velocity.RotatedByRandom(0.8f);
                firer.New();
            }
        }
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        _hitCount++;
        target.AddBuff(BuffID.OnFire, 60);
        float pitch = MathHelper.Clamp(_hitCount * 0.05f, 0f, 1f);
        SoundStyle smashSound = Main.rand.NextBool(2) ? SoundRegistry.HammerHit1 : SoundRegistry.HammerHit2;
        smashSound.PitchVariance = 0.2f;
        SoundEngine.PlaySound(smashSound, Projectile.position);

        base.OnHitNPC(target, hit, damageDone);
    }

    public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
    {
        base.ModifyHitNPC(target, ref modifiers);
        if (!_hit)
        {
            modifiers.Knockback *= 0.5f;
        }
        else
        {
            modifiers.Knockback *= 2;
        }
        if (IsFinishingSwing())
        {
            modifiers.FinalDamage += 0.5f;
        }
    }

}
public class BurningAngelSlash : BaseSwingProjectileV2
{
    private FireTrailRenderer _fireTrailRenderer;
    private float _hitCount;
    private bool _hit;
    public override void DefineCombo()
    {
        base.DefineCombo();
        outlineColor = Color.Yellow;

        //Bloom
        useBloom = true;
        bloom.innerBloomColor = Color.OrangeRed;
        bloom.outerBloomColor = Color.DarkRed;
        bloom.bloomWidthFunction = GetBloomWidth;
        bloom.bloomColorFunction = GetBloomColor;

        _fireTrailRenderer = new FireTrailRenderer();
        _fireTrailRenderer.SlashTrailer.TrailWidthFunction = GetTrailWidth;
        additive = true;
        Trailer = _fireTrailRenderer.SlashTrailer;
        useAfterImage = true;
        glowAfterImageColor = Color.Red;


        SoundStyle hammerSlash1 = SoundRegistry.HeavySwordSlash1;
        hammerSlash1.PitchVariance = 0.4f;

        SoundStyle hammerSlash2 = SoundRegistry.HeavySwordSlash2;
        hammerSlash2.PitchVariance = 0.4f;

        Add(new OvalSwing
        {
            Duration = 66,
            SwingDegrees = 310,
            XSwingRadius = 54,
            YSwingRadius = 32,
            Easing = (float lerpValue) => Easing.InOutBack(lerpValue),
            Sound = hammerSlash1,
            HitCount = 2
        });

        Add(new OvalSwing
        {
            Duration = 54,
            SwingDegrees = 330,
            XSwingRadius = 54,
            YSwingRadius = 32,
            Easing = (float lerpValue) => EasingFunction.GreatswordAnticipation(lerpValue),
            Sound = hammerSlash2,
            HitCount = 2
        });

        useAfterImage = true;
        hitStopTime = 4 * EXTRA_UPDATE_COUNT;
    }

    public float GetTrailWidth(float interpolant)
    {
        return MathHelper.SmoothStep(4, 24, interpolant) * MathF.Sin(interpolant * 8);
    }

    private float GetBloomWidth(float ratio)
    {
        return MathHelper.SmoothStep(8, 32, ratio) * 1.5f * MathHelper.SmoothStep(1f, 0f, EasingFunction.InExpo(Interpolant));
    }

    private Color GetBloomColor(float ratio)
    {
        return Color.Lerp(Color.Red * 0.9f, Color.Transparent, EasingFunction.InExpo(ratio));
    }


    public override void AI()
    {
        base.AI();
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        _hitCount++;
        target.AddBuff(BuffID.OnFire, 60);
        float pitch = MathHelper.Clamp(_hitCount * 0.05f, 0f, 1f);
        SoundStyle smashSound = Main.rand.NextBool(2) ? SoundRegistry.HammerHit1 : SoundRegistry.HammerHit2;
        smashSound.PitchVariance = 0.2f;
        SoundEngine.PlaySound(smashSound, Projectile.position);

        base.OnHitNPC(target, hit, damageDone);
        if (!_hit)
        {
            for (var i = 0; i < 6; i++)
            {
                var firer = ProjFirer.From<BurningAngelBubble>(Projectile);
                firer.position = target.Center;
                firer.velocity = Projectile.velocity.SafeNormalize(Vector2.Zero).RotatedByRandom(0.9f) * Main.rand.NextFloat(0.14f, 1f);
                firer.velocity *= 25;
                firer.New();
            }

            Bounce(8);
            FXUtil.ShakeCamera(target.Center, 1024, 4);
            FXUtil.PunchCamera(target.Center, Projectile.velocity, 0.15f, 2, 12);
            _hit = true;
        }
    }

    public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
    {
        base.ModifyHitNPC(target, ref modifiers);
        if (!_hit)
        {
            modifiers.Knockback *= 0.5f;
        }
        else
        {
            modifiers.Knockback *= 2;
        }
        if (IsFinishingSwing())
        {
            modifiers.FinalDamage += 0.5f;
        }
    }
}












