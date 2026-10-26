using Microsoft.Xna.Framework;
using Stellamod.Common;
using Stellamod.Common.Particles;
using Stellamod.Content.CommonMaterials;
using Stellamod.Core.Bases;
using Stellamod.Core.Effects.Trails;
using Stellamod.Core.SwingSystem;
using Stellamod.Helpers;
using Stellamod.Items;
using Stellamod.Projectiles.Magic;
using Stellamod.Trailing;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using static Stellamod.Core.LocalizationReferences.Mods.Stellamod.Projectiles;

namespace Stellamod.Content.Areas.Fable.WeaponsFB;

public class LightSpand : BaseSwingItemV2
{
    public override string Texture => AssetReferences.Assets.Textures.Fable.Items.LightSpand.KEY;
    public override void SetDefaults2()
    {
        base.SetDefaults2();
        Item.shoot = ModContent.ProjectileType<LightSpandSlash>();
        staminaProjectileShoot = ModContent.ProjectileType<LightSpandStaminaSlash>();
        meleeWeaponType = MeleeWeaponType.Sword;
    }

    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew<AlcadizScrap, BlankSword>();
    }
}

public class LightSpandConstellation : ModProjectile
{
    Projectile[] ConstellationProjectiles
    {
        get
        {
            var typeToLookFor = ModContent.ProjectileType<LightSpandStar>();
            var points = new List<Projectile>();
            foreach (var proj in Main.ActiveProjectiles)
            {
                if (proj.owner != Projectile.owner)
                    continue;
                if (proj.type != typeToLookFor)
                    continue;
                points.Add(proj);
            }
            return points.ToArray();
        }
    }
    Vector2[] ConstellationPoints
    {
        get
        {
            field = GetPointsToConnectTo();
            return field;
        }
    }

    Vector2[] GetPointsToConnectTo()
    {
        var points = new List<Vector2>();
        foreach(var proj in ConstellationProjectiles)
        {
            points.Add(proj.Center);
        }
        return points.ToArray();
    }
    ref float Timer => ref Projectile.ai[0];
    float Easing => EasingFunction.InOutSine(Timer / 30f) * EasingFunction.InOutSine((float)Projectile.timeLeft / 30f);
    public override string Texture => AssetReferences.Assets.Textures.Fable.Items.LightSpandStarLine.KEY;
    public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
    {
        return ProjectileHelper.OldPosColliding(ConstellationPoints, projHitbox, targetHitbox);
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 16;
        Projectile.height = 16;
        Projectile.tileCollide = false;
        Projectile.ignoreWater = true;
        Projectile.timeLeft = 120;
        Projectile.LocalPiercingImmunityTime = 30;
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
        if(Timer == 1)
        {
            var summonSound = AssetReferences.Assets.Sounds.STARR.StarSummon.Asset with { PitchVariance = 0.5f };
            SoundEngine.PlaySound(summonSound, Projectile.position);
        }
        foreach(var constellationProj in ConstellationProjectiles)
        {
            constellationProj.ai[1] = 1;
        }
    }
    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        base.OnHitNPC(target, hit, damageDone);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        var pass = AssetReferences.Effects.Fable.FableConstellation.CreatePixelPass();
        pass.Parameters.time = Main.GlobalTimeWrappedHourly;
        pass.Apply();
        var spriteBatch = Main.spriteBatch;
        using(spriteBatch.Ctx(spriteBatch.Parameters with { effect = pass.Shader }))
        {
            for (var i = 1; i < ConstellationPoints.Length; i++)
            {
                ref var last = ref ConstellationPoints[i - 1];
                ref var current = ref ConstellationPoints[i];
                var rotation = (current - last).ToRotation();
                var drawer = Projectile.Drawer;
                drawer.color = Color.White * Easing;
                drawer.color.A = 0;
                drawer.worldPosition = last;

                var scale = Vector2.Distance(last, current);
                drawer.scale.X = scale;
                drawer.scale.Y = 0.6f;
                drawer.scale *= Easing;
                Main.spriteBatch.Draw(drawer);
            }
        }


        return false;
    //    return base.PreDraw(ref lightColor);
    }
}
public class LightSpandStar : ModProjectile
{
    ref float Timer => ref Projectile.ai[0];
    ref float Glow => ref Projectile.ai[1];
    ref float GlowTimer => ref Projectile.ai[2];
    float GlowTime => 30;
    float GlowAlpha => EasingFunction.InOutSine(GlowTimer / GlowTime);
    public override string Texture => AssetReferences.Assets.Textures.Fable.Items.LightSpandStar.KEY;
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        Projectile.SetTrailCacheLength(16);
        
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.friendly = false;
        Projectile.tileCollide = false;
        Projectile.WidthAndHeight = 16;
        Projectile.ignoreWater = true;
        Projectile.timeLeft = 300;
        Projectile.light = 0.5f;
    }

    public override void AI()
    {
        base.AI();
        Timer++;
        if(Timer % 16 == 0)
        {
            var pos = Projectile.Center + Main.rand.NextVector2Circular(32, 32);
            var vel = Main.rand.NextVector2Circular(1, 1);
            var d = Dust.NewDustPerfect(pos, DustID.GoldCoin, vel);
            d.noGravity = true;
        }
        if(Glow > 0)
        {
            GlowTimer++;
            GlowTimer = MathHelper.Clamp(GlowTimer, 0, GlowTime);
        }
        Projectile.velocity *= 0.96f;
        Projectile.rotation += Projectile.velocity.X * 0.05f;
     
    }

    public override bool PreDraw(ref Color lightColor)
    {
        var drawer = Projectile.Drawer;
        drawer.rotation += Main.GlobalTimeWrappedHourly;
        Main.spriteBatch.Draw(drawer);

        drawer.scale *= 1.1f;
        drawer.color = Color.Gold;
        drawer.color *= 0.5f;
        Main.spriteBatch.Draw(drawer);


        var drawer3 = Projectile.Drawer;
        drawer3.color = Color.Lerp(Color.Transparent, Color.White, ExtraMath.Osc(0.5f, 1f, speed: 16) * GlowAlpha);
        drawer3.rotation += Main.GlobalTimeWrappedHourly;
        Main.spriteBatch.Draw(drawer3);
        return false;
    }
    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
        foreach(PositionVelocity posVel in new CircleOrientation(Projectile.Center, 16, 8))
        {
            Particles.SwirlingFlameDust.Spawn(BitDustFactory.SlowingOverTime with
            {
                position = posVel.position,
                velocity = posVel.velocity,
                innerColor = Color.Yellow.ToVector4(),
                outerColor = Color.DarkOrange.ToVector4(),
                scale = new Vector2(Main.rand.NextFloat(0.4f, 0.8f))
            });
        }

        var starDeath = AssetReferences.Assets.Sounds.Starrer.Asset with { PitchVariance = 0.6F };
        SoundEngine.PlaySound(starDeath, Projectile.position);
    }
}
public class LightSpandSlash : BaseSwingProjectileV2
{
    bool _projSpawned;
    public override void DefineCombo()
    {
        base.DefineCombo();
        SoundStyle swingSound1 = AssetRegistry.Sounds.Melee.NormalSwordSlash1;
        swingSound1.PitchVariance = 0.25f;
        swingSound1.Volume = 0.25f;

        SoundStyle swingSound2 = AssetRegistry.Sounds.Melee.NormalSwordSlash2;
        swingSound2.PitchVariance = 0.25f;

        SoundStyle swingSound3 = AssetRegistry.Sounds.Melee.SwordSpin1;
        swingSound3.PitchVariance = 0.5f;
        swingSound3.Volume = 0.5f;


        int style = 0;
        SoundStyle s1 = swingSound1;
        SoundStyle s2 = swingSound2;

        Add(new OvalSwing
        {
            Duration = 10,
            XSwingRadius = 96,
            YSwingRadius = 48,
            SwingDegrees = 320,
            Easing = EasingFunction.InOutExpo,
            Sound = s1
        });

        Add(new OvalSwing
        {
            Duration = 10,
            XSwingRadius = 96,
            YSwingRadius = 48,
            SwingDegrees = 320,
            Easing = EasingFunction.InOutExpo,
            Sound = s2
        });


        Add(new OvalSwing
        {
            Duration = 24,
            XSwingRadius = 64,
            YSwingRadius = 64,
            SwingDegrees = 370,
            Easing = EasingFunction.InOutExpo,
            Sound = swingSound3,
        });


        Add(new OvalSwing
        {
            Duration = 24,
            XSwingRadius = 64,
            YSwingRadius = 64,
            SwingDegrees = 370,
            Easing = EasingFunction.InOutExpo,
            Sound = swingSound3,
        });

        Add(new SwordSwing
        {
            baseSwingTime = 28,
            swordMovementFunction = new Swings.HoldUpSwing
            {
                thrustDistance = 64,
            },
            sound = null,
        });

        SlashTrailBuilder slashTrailBuilder = new SlashTrailBuilder();
        SlashTrailer slashTrailer = slashTrailBuilder.Instantiate();
        slashTrailer.invert = ComboIndex % 2 != 0;
        Trailer = slashTrailer;

        useBloom = true;
        bloom.innerBloomColor = Color.Gold;
        bloom.outerBloomColor = Color.DarkOrange;
        bloom.bloomWidthFunction = GetBloomWidth;
        bloom.bloomColorFunction = GetBloomColor;
        useAfterImage = true;
    }

    private float GetBloomWidth(float ratio)
    {
        return MathHelper.SmoothStep(4, 32, ratio) * 1.15f * MathHelper.Lerp(1f, 0f, EasingFunction.InExpo(Interpolant));
    }

    private Color GetBloomColor(float ratio)
    {
        Color blue = Color.Lerp(Color.Lerp(Color.White, Color.DarkOrange, 0.5f), Color.DarkGoldenrod, ExtraMath.Osc(0f, 1f, speed: 4));
        return Color.Lerp(blue * 0.9f, Color.Black, ratio);
    }

    public override void AI()
    {
        base.AI();
        //Every a swing, a tiny star expands outward from you and expands slightly
        //and sits there
        //Once you finally do the finishing move, you hold the sword up and connect a line between all of them which hits multiple times for a bit and destroys the stars
        if(IsFinishingSwing())
        {
            if (Interpolant > 0.3f && !_projSpawned)
            {
                var firer = ProjFirer.From<LightSpandConstellation>(Projectile);
                firer.velocity = Main.rand.NextVector2Circular(4, 4);
                firer.New();
                _projSpawned = true;
            }
        }
        else
        {
            if (this.OwnedByLocalClient())
            {
                if (Interpolant > 0.3f && !_projSpawned)
                {
                    var firer = ProjFirer.From<LightSpandStar>(Projectile);
                    firer.velocity = Main.rand.NextVector2Circular(4, 4);
                    firer.New();
                    _projSpawned = true;
                }
            }

        }
    }

    public override void RenderSwingTrail(ref Color lightColor, Vector2[] points)
    {
        base.RenderSwingTrail(ref lightColor, points);
    }

    public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
    {
        base.ModifyHitNPC(target, ref modifiers);
        SoundStyle spearHit = SoundRegistry.SpearHit1;
        spearHit.PitchVariance = 0.5f;
        SoundEngine.PlaySound(spearHit, Projectile.position);
        target.AddBuff(BuffID.OnFire, 120);
        if (ComboIndex == 5)
        {
            modifiers.FinalDamage *= 2;
        }
    }
}

public class LightSpandStaminaSlash : BaseSwingProjectileV2
{
    bool _firedProj;
    private float _projTimer;
    public override void DefineCombo()
    {
        base.DefineCombo();
        SoundStyle swingSound1 = SoundRegistry.HeavySwordSlash1;
        swingSound1.PitchVariance = 0.5f;

        Add(new OvalSwing
        {
            Duration = 68,
            XSwingRadius = 160 / 1.5f,
            YSwingRadius = 80 / 1.5f,
            SwingDegrees = 270,
            Easing = (lerpValue) => Easing.InOutExpo(lerpValue, 10),
            Sound = swingSound1,
        });

        Add(new SwordSwing
        {
            baseSwingTime = 28,
            swordMovementFunction = new Swings.HoldUpSwing
            {
                thrustDistance = 64,
            },
            sound = null,
        });

        SlashTrailBuilder slashTrailBuilder = new SlashTrailBuilder();
        SlashTrailer slashTrailer = slashTrailBuilder.Instantiate();
        slashTrailer.invert = ComboIndex % 2 != 0;
        Trailer = slashTrailer;

        useBloom = true;
        bloom.innerBloomColor = Color.Gold;
        bloom.outerBloomColor = Color.DarkOrange;
        bloom.bloomWidthFunction = GetBloomWidth;
        bloom.bloomColorFunction = GetBloomColor;
        useAfterImage = true;
    }
    private float GetBloomWidth(float ratio)
    {
        return MathHelper.SmoothStep(4, 32, ratio) * 1.15f * MathHelper.Lerp(1f, 0f, EasingFunction.InExpo(Interpolant));
    }

    private Color GetBloomColor(float ratio)
    {
        Color blue = Color.Lerp(Color.Lerp(Color.White, Color.DarkOrange, 0.5f), Color.DarkGoldenrod, ExtraMath.Osc(0f, 1f, speed: 4));
        return Color.Lerp(blue * 0.9f, Color.Black, ratio);
    }

    public override void AI()
    {
        base.AI();
        if(ComboIndex == 0)
        {
            _projTimer++;
            if (_projTimer % 25 == 0 && Main.myPlayer == Projectile.owner)
            {
                var firer = ProjFirer.From<LightSpandStar>(Projectile);
                firer.position = Owner.Center;
                firer.velocity = Main.rand.NextVector2Circular(7, 7);
                firer.New();
            }
        } else if (IsFinishingSwing())
        {
            if(this.OwnedByLocalClient() && !_firedProj && Interpolant > 0.3f)
            {
                var firer = ProjFirer.From<LightSpandConstellation>(Projectile);
                firer.New();
                _firedProj = true;
            }
        }
    }


    public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
    {
        base.ModifyHitNPC(target, ref modifiers);
        SoundStyle spearHit = SoundRegistry.CrystalHit1;
        spearHit.PitchVariance = 0.5f;
        SoundEngine.PlaySound(spearHit, Projectile.position);

        SoundStyle spearHit2 = SoundRegistry.NSwordHit1;
        spearHit2.PitchVariance = 0.2f;
        SoundEngine.PlaySound(spearHit2, Projectile.position);

        modifiers.FinalDamage *= 3;
        modifiers.Knockback *= 4;
    }

    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
        SwingPlayerV2 comboPlayer = Owner.GetModPlayer<SwingPlayerV2>();
        int combo = ComboIndex + 1;
        int dir = comboPlayer.ComboDirection;

        if (ComboIndex < ComboCount && this.OwnedByLocalClient())
        {
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position, Main.MouseWorld - Owner.Center, Projectile.type, Projectile.damage, Projectile.knockBack,
                        Projectile.owner, ai2: combo, ai1: dir);
        }
    }
}