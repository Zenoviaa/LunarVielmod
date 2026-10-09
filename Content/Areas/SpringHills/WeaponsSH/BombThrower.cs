using ReLogic.Content;
using Stellamod.Common;
using Stellamod.Common.Particles;
using Stellamod.Common.ShockCircleSystem;
using Stellamod.Content.CommonMaterials;
using Stellamod.Core;
using Stellamod.Core.Bases;
using Stellamod.Items;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.SpringHills.WeaponsSH;

public class BombThrower : ModItem
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.DefaultToCustomChanneledWeapon<BombThrowerHeld>();
        Item.DamageType = DamageClass.Ranged;
        Item.damage = 25;
        Item.knockBack = 1;
    }

    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew<Ivythorn, BlankGun>();
    }
}

public class BombThrowerHeld : ModProjectile
{
    float _actualScale;
    public override string Texture => TextureRegistry.EmptyTexture;
    Asset<Texture2D> LittleBomb => TextureAssets.Projectile[ModContent.ProjectileType<ThrowingBomb>()];
    Asset<Texture2D> BigBomb => TextureAssets.Projectile[ModContent.ProjectileType<ThrowingBigBomb>()];
    Player Owner => Main.player[Projectile.owner];
    ref float Timer => ref Projectile.ai[0];
    ref float AttackCycle => ref Projectile.ai[1];
    ref float KillMe => ref Projectile.ai[2];
    float LittleChargeTime => 60;
    float BigChargeTIme => 100;
    float ChargeScale => AttackCycle == 0 ? EasingFunction.InOutSine(Timer / LittleChargeTime) : 1f;
    float BigChargeScale => AttackCycle == 1 ? EasingFunction.InOutSine(Timer / BigChargeTIme) : 0f;
    float AggregateScale =>MathHelper.Lerp(0.5f, 1f, ChargeScale)+ BigChargeScale * 0.5f;
    Vector2 HoldPosition => Owner.MountedCenter + new Vector2(0, -48);
    int ThrowingDamage
    {
        get
        {
            return (int)(AggregateScale * Projectile.damage * 2F);
        }
    }
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        Main.projFrames[Type] = 2;
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = Projectile.height = 60;
        Projectile.tileCollide = false;
        Projectile.ignoreWater = true;
        Projectile.light = 0.5f;
        Projectile.penetrate = -1;
    }
    void PlayBombGrowSound()
    {
        var sound = AssetReferences.Assets.Sounds.BombGrowStart.Asset with { PitchVariance = 0.4f };
        SoundEngine.PlaySound(sound, Projectile.position);
    }

    void PlayBombBigGrowSound()
    {
        var factory = ParticleUtils.ParticleFactory.FromSmallBurst(Projectile.Center, Projectile.Center + new Vector2(0, -2), TriColorPalette.Silver, new Vector2(5, 15f));
        ParticleUtils.CreateSwirlingDustCircle(factory);
        var sound = AssetReferences.Assets.Sounds.BombGrowMini.Asset with { PitchVariance = 0.4f };
        SoundEngine.PlaySound(sound, Projectile.position);
    }
    void PlayLittleBombBigGrowCompleteSound()
    {
        var sound = AssetReferences.Assets.Sounds.BombGrowComplete.Asset with { PitchVariance = 0.4f, Pitch = -0.5f };
        SoundEngine.PlaySound(sound, Projectile.position);
        ShockCircles.CreateSmallQuickWhiteFlash(Projectile.Center);
        var factory = ParticleUtils.ParticleFactory.FromSmallBurst(Projectile.Center, Projectile.Center + new Vector2(0, -2), TriColorPalette.Silver, new Vector2(5, 15f));
        ParticleUtils.CreateSwirlingDustCircle(factory);
    }

    void PlayBombBigGrowCompleteSound()
    {
        var sound = AssetReferences.Assets.Sounds.BombGrowComplete.Asset with { PitchVariance = 0.4f, Pitch = -0.5f };
        SoundEngine.PlaySound(sound, Projectile.position);
        ShockCircles.CreateQuickWhiteFlash(Projectile.Center);
        var factory = ParticleUtils.ParticleFactory.FromSmallBurst(Projectile.Center, Projectile.Center + new Vector2(0, -2), TriColorPalette.Silver, new Vector2(5, 15f));
        ParticleUtils.CreateSwirlingDustCircle(factory);
    }

    void MakeSuckingParticle()
    {
        if (!Main.rand.NextBool(3))
            return;
        var pos = Projectile.Center + Main.rand.NextVector2CircularEdge(128, 128);
        var vel = (Projectile.Center - pos) * 0.09f;
        Particles.BitDust.Spawn(BitDustFactory.SlowingOverTime with { position = pos, velocity = vel, innerColor = Color.White.ToVector4(), outerColor = Color.SkyBlue.ToVector4(), timeLeft = 15, scale = new Vector2(Main.rand.NextFloat(0.5f, 1.2f)) * 0.4f });
    }

    void MakeSuckingParticleLittle()
    {
        if (!Main.rand.NextBool(3))
            return;
        var pos = Projectile.Center + Main.rand.NextVector2CircularEdge(128, 128);
        var vel = (Projectile.Center - pos) * 0.09f;
        Particles.BitDust.Spawn(BitDustFactory.SlowingOverTime with { position = pos, velocity = vel, innerColor = Color.White.ToVector4(), outerColor = Color.SkyBlue.ToVector4(), timeLeft = 15, scale = new Vector2(Main.rand.NextFloat(0.5f, 1.2f) * 0.15f) });
    }

    public override bool ShouldUpdatePosition()
    {
        return false;
    }

    public override void AI()
    {
        base.AI();

        Timer++;
        switch (AttackCycle)
        {
            case 0:
                {
                    if (Timer == 1)
                    {
                        PlayBombGrowSound();
                    }

                    if(Timer < LittleChargeTime)
                    {
                        MakeSuckingParticle();
                    }
                    else
                    {
                        MakeSuckingParticleLittle();
                    }
       
                    if (Timer == LittleChargeTime - 1)
                    {
                        PlayLittleBombBigGrowCompleteSound();
                    }

                    if (Timer >= LittleChargeTime * 2f)
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 1:
                {
                    if (Timer == 1)
                    {
                        PlayBombBigGrowSound();
                    }

                    if (Timer < BigChargeTIme)
                    {
                        MakeSuckingParticle();
                    }
                    if (Timer == BigChargeTIme - 1)
                    {
                        PlayBombBigGrowCompleteSound();
                    }

                    if (Timer >= BigChargeTIme)
                    {
                        Timer = BigChargeTIme;
                    }
                }
                break;
        }

        if (this.OwnedByLocalClient() && !Owner.channel)
        {
            if (AttackCycle == 0)
            {
                if(Timer >= LittleChargeTime * 0.5f)
                {
                    var firer = ProjFirer.From<ThrowingBomb>(Projectile);
                    firer.damage = ThrowingDamage;
                    firer.position = HoldPosition;
                    firer.velocity = (Main.MouseWorld - Projectile.Center).Resize(14);
                    firer.velocity.Y -= 3;
                    firer.ai0 = 60;
                    firer.ai2 = AggregateScale;
                    firer.New();
                }

            }
            else
            {
                var firer = ProjFirer.From<ThrowingBigBomb>(Projectile);
                firer.damage = ThrowingDamage;
                firer.position = HoldPosition;
                firer.velocity = (Main.MouseWorld - Projectile.Center).Resize(14);
                firer.velocity.Y -= 3;
                firer.ai0 = 120;
                firer.ai2 = AggregateScale;
                firer.New();
            }
            KillMe = 1;
            Projectile.netUpdate = true;
        }

        if (this.OwnedByLocalClient())
        {
            Projectile.velocity.X = Main.MouseWorld.X < Projectile.Center.X ? -1 : 1;

            Projectile.netUpdate = true;
        }
        Owner.ChangeDir((int)Projectile.velocity.X);
        _actualScale = MathHelper.Lerp(_actualScale, AggregateScale, 0.1f);
        Projectile.Center = HoldPosition;
        if (KillMe > 0)
            Projectile.Kill();
        var osc = ExtraMath.Osc(-0.05f, 0.5f, offset: Projectile.identity);
        Owner.heldProj = Projectile.whoAmI;
        Owner.itemTime = 2;
        Owner.itemAnimation = 2;
        Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, (-Vector2.UnitY).ToRotation() - MathHelper.PiOver2 + osc);
        Owner.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Full, (-Vector2.UnitY).ToRotation() - MathHelper.PiOver2 + osc);

    }
    public override bool PreDraw(ref Color lightColor)
    {
        var holdPos = Owner.MountedCenter + new Vector2(0, -18);
        var asset = AttackCycle == 0 ? LittleBomb : BigBomb;
        var drawer = SpritebatchDrawer.FromTextureAsset(asset, holdPos);
        drawer.VerticalFrame(0, Main.projFrames[Type]);
        drawer.scale *= _actualScale;
        drawer.scale *= ExtraMath.Osc(0.95f, 1f, speed: 3, Projectile.identity);
        Main.spriteBatch.Draw(drawer);
        return false;
    }
}

public class ThrowingBombBigBoom : ThrowingBombBoom
{
    public override string Texture => ModContent.GetInstance<ThrowingBombBoom>().Texture;
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.WidthAndHeight = 285;
    }
    public override void AI()
    {
        base.AI();
        if (Timer == 1)
        {
            FXUtil.ShakeCamera(Projectile.Center, 1024, 24);
            ShockCircles.CreateQuickWhiteFlash(Projectile.Center);
            var factory = ParticleUtils.ParticleFactory.FromSmallBurst(Projectile.Center, Projectile.Center + new Vector2(0, -2), TriColorPalette.Fiery, new Vector2(15, 45f));
            factory.particleCount *= 2;
            ParticleUtils.CreateSwirlingDustCircle(factory);
            ParticleUtils.CreateSmokeCircleBig(Projectile.Center, Color.Lerp(Color.RosyBrown, Color.Black, 0.6f), 32, 100);
            var fx = FXUtil.GlowCircleBoom(Projectile.Center, Color.Yellow, Color.OrangeRed, Color.Red, duration: 25, baseSize: 0.16f);
            fx.Scale *= 3f;
        }
    }
}

public class ThrowingBombBoom : ModProjectile
{
    protected ref float Timer => ref Projectile.ai[0];

    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.LocalHitOnce = true;
        Projectile.WidthAndHeight = 128;
        Projectile.tileCollide = false;
        Projectile.timeLeft = 15;
        Projectile.light = 1.5f;
        Projectile.friendly = true;
    }

    public override bool ShouldUpdatePosition()
    {
        return false;
    }

    public override void AI()
    {
        base.AI();
        Timer++;
        if (Timer == 1)
        {
            var explosionSound = AssetReferences.Assets.Sounds.Explosions.ExplosionGeneric1.Asset with { PitchVariance = 0.6f };
            SoundEngine.PlaySound(explosionSound, Projectile.position);

            var fx = FXUtil.GlowCircleBoom(Projectile.Center, Color.Yellow, Color.OrangeRed, Color.Red, duration: 25, baseSize: 0.16f);
            fx.Scale *= 1.5f;

            var factory = ParticleUtils.ParticleFactory.FromSmallBurst(Projectile.Center, Projectile.Center + new Vector2(0, -2), TriColorPalette.Fiery, new Vector2(5, 15f));
            factory.particleCount *= 2;
            ParticleUtils.CreateSwirlingDustCircle(factory);
            ParticleUtils.CreateSmokeCircle(Projectile.Center, Color.Lerp(Color.RosyBrown, Color.Black, 0.6f), 24, 48);
            FXUtil.ShakeCamera(Projectile.Center, 1024, 8);
        }
    }

    public override bool PreDraw(ref Color lightColor)
    {
        var boomSprite = Projectile.Drawer;
        boomSprite.color = Color.Lerp(Color.White, Color.Yellow, ExtraMath.Osc(0f, 1f, speed: 7, offset: Projectile.identity));
        Main.spriteBatch.Draw(boomSprite);
        return false;
    }

    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
    }
}
public class ThrowingBomb : ModProjectile
{
    bool _resized;
    float Gravity => 0.3f;
    ref float DetonationTime => ref Projectile.ai[0];
    ref float FlashTime => ref Projectile.ai[1];
    ref float Scale => ref Projectile.ai[2];
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        Projectile.SetTrailCacheLength(16);
        Main.projFrames[Type] = 2;
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.WidthAndHeight = 32;
        Projectile.light = 0.7f;

    }

    public override void AI()
    {
        base.AI();
        if (Main.rand.NextBool(4))
        {
            var pos = Projectile.Center + Main.rand.NextVector2Circular(32, 32);
            var d = Dust.NewDustPerfect(pos, DustID.Torch, Vector2.Zero, Scale: Main.rand.NextFloat(0.5f, 1f));
            d.noGravity = true;
        }
        if (!_resized)
        {
            var size = 32F * Scale;
            Projectile.Resize((int)size, (int)size);
            _resized = true;
        }


        DetonationTime--;
        if (DetonationTime <= 0)
            Projectile.Kill();
        if (DetonationTime % 25 == 0)
        {
            FlashTime = 10;
        }
        if (FlashTime > 0)
        {
            FlashTime--;
        }
        Projectile.velocity.X *= 0.96f;
        Projectile.velocity.Y += Gravity;
        Projectile.rotation += Projectile.velocity.Length() * 0.01f * MathF.Sign(Projectile.velocity.X);
    }

    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        return false;
    }

    public override bool PreDraw(ref Color lightColor)
    {
        DrawUtilities.DrawAdditiveFadingTrail(Projectile, Color.Red, Color.Transparent, 0.25f);
        var drawer = Projectile.Drawer;
        drawer.scale *= Scale;
        Main.spriteBatch.Draw(drawer);
        var glowDrawer = drawer;
        glowDrawer.VerticalFrame(1, Main.projFrames[Type]);
        glowDrawer.color = Color.Lerp(Color.Transparent, Color.Red, EasingFunction.OutExpo(FlashTime / 10f));
        glowDrawer.color.A = 0;
        Main.spriteBatch.Draw(glowDrawer);
        return false;
    }

    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
        if (this.OwnedByLocalClient())
        {
            var boomFirer = ProjFirer.From<ThrowingBombBoom>(Projectile);
            boomFirer.New();
        }
    }
}

public class ThrowingBigBomb : ThrowingBomb
{

    public override void OnKill(int timeLeft)
    {
        // base.OnKill(timeLeft);
        if (this.OwnedByLocalClient())
        {
            var boomFirer = ProjFirer.From<ThrowingBombBigBoom>(Projectile);
            boomFirer.New();
        }
    }
}
