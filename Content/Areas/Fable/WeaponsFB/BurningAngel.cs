using Stellamod.Common;
using Stellamod.Common.Particles;
using Stellamod.Common.Shaders;
using Stellamod.Content.Areas.Cinderspark.WeaponsCS;
using Stellamod.Content.CommonMaterials;
using Stellamod.Core.Bases;
using Stellamod.Core.Effects.Trails;
using Stellamod.Core.SwingSystem;
using Stellamod.Helpers;
using Stellamod.Items;
using Stellamod.Projectiles;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Fable.WeaponsFB;

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
    float OutEase => EasingFunction.InOutSine(Timer / Time);
    ref float Timer => ref Projectile.ai[0];
    public override string Texture => AssetReferences.Assets.Textures.Fable.Items.BurningAngelBubbleBurst.KEY;
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 64;
        Projectile.height = 64;
        Projectile.LocalHitOnce = true;
        Projectile.timeLeft = (int)Time;
        Projectile.ignoreWater = true;
        Projectile.tileCollide = false;
        Projectile.light = 0.6f;
    }

    public override void AI()
    {
        base.AI();
        Timer++;
        if(Timer == 1)
        {
            var pop = AssetReferences.Assets.Sounds.Fire.FireballImpact1.Asset with { PitchVariance = 0.8f };
            SoundEngine.PlaySound(pop, Projectile.position);
            PixelPrimitiveCircleFactory.CreateGenericBoom(Projectile.Center, Color.White, Color.Yellow, 24, 64);
            foreach(PositionVelocity posVel in new CircleOrientation(Projectile.Center, 32, 8))
            {
                Particles.SwirlingFlameDust.Spawn(BitDustFactory.SlowingOverTime with
                {
                    position = posVel.position,
                    velocity = posVel.velocity,
                    innerColor = Color.Yellow.ToVector4(),
                    outerColor = Color.Red.ToVector4(),
                    scale = new Vector2(Main.rand.NextFloat(0.3f, 0.6f)),
                    timeLeft = 40
                });
            }
        }

        Projectile.rotation = Main.GlobalTimeWrappedHourly * 4;
    }

    public override bool PreDraw(ref Color lightColor)
    {
        var drawer = Projectile.Drawer;
        drawer.color *= OutEase * 0.4f;
        drawer.scale *= MathHelper.Lerp(1f, 1.2f, OutEase);
        Main.spriteBatch.Draw(drawer);
        return false;
    }
}
public class BurningAngelBubble : ModProjectile
{
    public override string Texture => AssetReferences.Assets.Textures.Fable.Items.BurningAngelBubble.KEY;
    ref float Timer => ref Projectile.ai[0];
    float InScale => EasingFunction.InOutSine(Timer / 30f);

    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        Projectile.SetTrailCacheLength(4);
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 24;
        Projectile.height = 24;
        Projectile.LocalPiercingImmunityTime = 30;
        Projectile.timeLeft = 120;
        Projectile.ignoreWater = true;
        Projectile.light=0.6F;
    }
    public override void AI()
    {
        base.AI();
        Timer++;
        if(Timer % 12 == 0)
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
        Projectile.velocity.X = MathHelper.Lerp(Projectile.velocity.X, MathF.Sin(Timer * 0.3f) * 0.3f, 0.05f);
        Projectile.velocity.Y = MathHelper.Lerp(Projectile.velocity.Y, -0.5f, MathHelper.Lerp(0.02f, 0.05f, MathF.Sin(Projectile.identity * 4) * 0.5f + 0.5f));
        Projectile.rotation += MathF.Sign(Projectile.velocity.X) * 0.04f;
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

    public override bool PreDraw(ref Color lightColor)
    {
        var drawer = Projectile.Drawer;
        drawer.scale *= InScale;
        drawer.scale.X *= MathHelper.Lerp(1f, 1.1f, ExtraMath.Osc(0f, 1f, speed: 3, Projectile.identity));//
        drawer.scale.Y *= MathHelper.Lerp(0.9f, 1f, ExtraMath.Osc(0f, 1f, speed: 3, Projectile.identity));                                                                                                         
        Main.spriteBatch.Draw(drawer);

        drawer.color = Color.Lerp(Color.Transparent, Color.White, ExtraMath.Osc(0f, 0.5f, speed: 4, offset: Projectile.identity));
        drawer.color.A = 0;
        Main.spriteBatch.Draw(drawer);
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
        hammerSlash1.PitchVariance = 0.2f;

        SoundStyle hammerSlash2 = SoundRegistry.HeavySwordSlash2;
        hammerSlash2.PitchVariance = 0.2f;

        Add(new OvalSwing
        {
            Duration = 66,
            SwingDegrees = 310 * 6,
            XSwingRadius = 64,
            YSwingRadius = 64,
            Easing = (float lerpValue) => EasingFunction.InOutSine(lerpValue / 0.3f) * lerpValue,
            Sound = hammerSlash1,
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
        if(Timer == 1)
        {
            var fireShood = AssetReferences.Assets.Sounds.Fire.FireballShoot1.Asset with { PitchVariance = 0.5f };
            SoundEngine.PlaySound(fireShood, Projectile.position);
        }
        if(Timer % 25 == 0)
        {
            if (this.OwnedByLocalClient())
            {
                var firer = ProjFirer.From<BurningAngelBubble>(Projectile);
                firer.position = Owner.Center;
                firer.velocity = -Vector2.UnitY.RotatedByRandom(0.7f);
                firer.velocity *= 8;
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
            XSwingRadius = 64,
            YSwingRadius = 64,
            Easing = (float lerpValue) => Easing.InOutBack(lerpValue),
            Sound = hammerSlash1,
            HitCount = 2
        });

        Add(new OvalSwing
        {
            Duration = 54,
            SwingDegrees = 330,
            XSwingRadius = 64,
            YSwingRadius = 64,
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
            var firer = ProjFirer.From<BurningAngelBubble>(Projectile);
            firer.position = target.Center;
            firer.velocity = -Vector2.UnitY.RotatedByRandom(0.7f);
            firer.velocity *= 8;
            firer.New();
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












