using Microsoft.Xna.Framework;
using Stellamod.Common;
using Stellamod.Common.Particles;
using Stellamod.Content.CommonMaterials;
using Stellamod.Core.Bases;
using Stellamod.Core.Effects.Trails;
using Stellamod.Core.Pixelation;
using Stellamod.Core.SwingSystem;
using Stellamod.Helpers;
using Stellamod.Items;
using Stellamod.Projectiles.Magic;
using Stellamod.Trailing;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;
using static Stellamod.Core.LocalizationReferences.Mods.Stellamod.Projectiles;

namespace Stellamod.Content.Areas.Fable.WeaponsFB;

public class LightSpand : BaseSwingItemV2
{
    public override string Texture => AssetReferences.Assets.Textures.Fable.Items.LightSpand.KEY;
    public override void SetDefaults2()
    {
        base.SetDefaults2();
        Item.damage = 9;
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
    UnifiedRandom Random
    {
        get
        {
            field ??= new();
            return field;
        }
    }
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
            Random.SetSeed((int)Seed);
            Random.Shuffle(field);
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
        points.Add(Main.player[Projectile.owner].Center);
        return points.ToArray();
    }
    ref float Timer => ref Projectile.ai[0];
    ref float TimeToUse => ref Projectile.ai[1];
    ref float Seed => ref Projectile.ai[2];
    float Easing => EasingFunction.InOutSine(Timer / (TimeToUse * 0.25f)) * EasingFunction.InOutSine((float)Projectile.timeLeft / (TimeToUse * 0.25f));
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
        Projectile.LocalPiercingImmunityTime = 15;
        Projectile.friendly = true;
    }
    public override bool ShouldUpdatePosition()
    {
        return false;
    }
    public override void AI()
    {
        base.AI();
        if(this.OwnedByLocalClient() && Seed == 0)
        {
            Seed = Main.rand.Next(0, 2140);
        }
        if(Projectile.timeLeft > (int)TimeToUse)
        {
            Projectile.timeLeft = (int)TimeToUse;
        }
        Timer++;
        if(Timer == 1)
        {
            var summonSound = AssetReferences.Assets.Sounds.STARR.StarSummon.Asset with { PitchVariance = 0.5f, Volume = 0.125f };
            SoundEngine.PlaySound(summonSound, Projectile.position);
        }
        foreach(var constellationProj in ConstellationProjectiles)
        {
            constellationProj.ai[1] = 5;
        }
        if(Timer >= TimeToUse)
        {
            Projectile.Kill();
        }
    }
    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        base.OnHitNPC(target, hit, damageDone);
    }

    void DrawPixelated(SpriteBatch sb, Vector2 sp)
    {
        var pass = AssetReferences.Effects.Fable.FableConstellation.CreatePixelPass();
        pass.Parameters.time = Main.GlobalTimeWrappedHourly;
        pass.Apply();
        var spriteBatch = Main.spriteBatch;
        using (spriteBatch.Ctx(spriteBatch.Parameters with { effect = pass.Shader }))
        {
            for (var i = 1; i < ConstellationPoints.Length; i++)
            {
                ref var last = ref ConstellationPoints[i - 1];
                ref var current = ref ConstellationPoints[i];
                var rotation = (current - last).ToRotation();
                var drawer = Projectile.Drawer;
                drawer.color = Color.Gold * Easing;
                drawer.color.A = 0;
                drawer.worldPosition = last;
                drawer.rotation = rotation;
                drawer.LeftCenterOrigin();
                var scale = Vector2.Distance(last, current) / (float)drawer.Width;
                drawer.scale.X = scale;
                drawer.scale.Y = 0.72f;
                drawer.scale.Y *= Easing;


                spriteBatch.Draw(drawer);

                drawer.color = Color.White * Easing;
                drawer.color.A = 0;
                drawer.scale.Y *= 0.5f;
                spriteBatch.Draw(drawer);
            }
        }

    }
    public override bool PreDraw(ref Color lightColor)
    {

        PixelationManager.QueueSpritebatchDrawAction(DrawPixelated);
        return false;
    //    return base.PreDraw(ref lightColor);
    }
}
public class LightSpandStar : ModProjectile
{
    float _glowing;
    ref float Timer => ref Projectile.ai[0];
    ref float Glow => ref Projectile.ai[1];
    ref float GlowTimer => ref Projectile.ai[2];
    float GlowTime => 30;
    float GlowAlpha => EasingFunction.InOutSine(_glowing / GlowTime);
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
            _glowing++;
            Glow--;
          if(Glow <= 0)
            {
                Projectile.Kill();
            }
        }

        Projectile.velocity *= 0.92f;
        Projectile.velocity = Projectile.velocity.RotatedBy(0.02f);
        Projectile.rotation += Projectile.velocity.X * 0.05f;
     
    }

    void DrawPixelated(SpriteBatch sb, Vector2 sp)
    {
        var drawer = Projectile.Drawer;
        drawer.rotation += Main.GlobalTimeWrappedHourly;
        drawer.scale *= 0.4f;
        sb.Draw(drawer);

        drawer.scale *= 1.1f;
        drawer.color = Color.Lerp(Color.White, Color.Gold, 0.3f);
        drawer.color *= 0.5f;
        sb.Draw(drawer);


        var drawer3 = Projectile.Drawer;
        drawer3.color = Color.Lerp(Color.Transparent, Color.White, ExtraMath.Osc(0.5f, 1f, speed: 16) * GlowAlpha);
        drawer3.rotation += Main.GlobalTimeWrappedHourly;
        sb.Draw(drawer3);

        var drawer4 = SpritebatchDrawer.FromTextureAsset(AssetReferences.Assets.GlowMasks.SimpleGlowCircle.Asset, Projectile.Center);
        drawer4.color = Color.Gold * 0.16f;
        drawer4.color.A = 0;
        drawer4.scale *= 0.15f;
        sb.Draw(drawer4);
    }
    public override bool PreDraw(ref Color lightColor)
    {
        PixelationManager.QueueSpritebatchDrawAction(DrawPixelated);
        return false;
    }
    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
        if(GlowTimer < 1)
        {
            foreach (PositionVelocity posVel in new CircleOrientation(Projectile.Center, 8, 8))
            {
                Particles.SwirlingFlameDust.Spawn(BitDustFactory.SlowingOverTime with
                {
                    position = posVel.position,
                    velocity = posVel.velocity * 0.4F,
                    innerColor = Color.White.ToVector4(),
                    outerColor = Color.Yellow.ToVector4(),
                    scale = new Vector2(Main.rand.NextFloat(0.4f, 0.8f))
                });
            }
            return;
        }
        foreach(PositionVelocity posVel in new CircleOrientation(Projectile.Center, 16, 8))
        {
            Particles.SwirlingFlameDust.Spawn(BitDustFactory.SlowingOverTime with
            {
                position = posVel.position,
                velocity = posVel.velocity * 0.66F,
                innerColor = Color.White.ToVector4(),
                outerColor = Color.Yellow.ToVector4(),
                scale = new Vector2(Main.rand.NextFloat(0.4f, 0.8f))
            });
        }

        var starDeath = AssetReferences.Assets.Sounds.Starrer.Asset with { PitchVariance = 0.6F, Volume = 0.3f };
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

        SoundStyle swingSound3 = AssetReferences.Assets.Sounds.Melee.SlashQuickSpin1.Asset;
        swingSound3.PitchVariance = 0.5f;
        swingSound3.Volume = 0.5f;


        int style = 0;
        SoundStyle s1 = swingSound1;
        SoundStyle s2 = swingSound2;

        Add(new OvalSwing
        {
            Duration = 15,
            XSwingRadius = 96,
            YSwingRadius = 48,
            SwingDegrees = 320,
            Easing = EasingFunction.InOutExpo,
            Sound = s1
        });

        Add(new OvalSwing
        {
            Duration = 15,
            XSwingRadius = 92,
            YSwingRadius = 42,
            SwingDegrees = 320,
            Easing = EasingFunction.InOutExpo,
            Sound = s2
        });
        Add(new OvalSwing
        {
            Duration = 15,
            XSwingRadius = 108,
            YSwingRadius = 40,
            SwingDegrees = 320,
            Easing = EasingFunction.InOutExpo,
            Sound = s1
        });

        Add(new OvalSwing
        {
            Duration = 15,
            XSwingRadius = 94,
            YSwingRadius = 36,
            SwingDegrees = 320,
            Easing = EasingFunction.InOutExpo,
            Sound = s2
        });


        Add(new OvalSwing
        {
            Duration = 35,
            XSwingRadius = 64,
            YSwingRadius = 64,
            SwingDegrees = 480,
            Easing = EasingFunction.InOutExpo,
            Sound = swingSound3
        });



        Add(new SwordSwing
        {
            baseSwingTime = 60,
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
        outlineColor = Color.Yellow;
        useBloom = true;
        bloom.innerBloomColor = Color.Gold;
        bloom.outerBloomColor = Color.DarkOrange;
        bloom.bloomWidthFunction = GetBloomWidth;
        bloom.bloomColorFunction = GetBloomColor;
        additive = true;
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
        outlineColor = Color.Lerp(Color.LightGoldenrodYellow, Color.Transparent, ExtraMath.Osc(0f, 0.5f, speed: 32));
   
        //Every a swing, a tiny star expands outward from you and expands slightly
        //and sits there
        //Once you finally do the finishing move, you hold the sword up and connect a line between all of them which hits multiple times for a bit and destroys the stars
        if (this.OwnedByLocalClient() && ComboIndex == 4)
        {
            if( Interpolant > 0.4f && !_projSpawned)
            {
                foreach(PositionVelocity posVel in new CircleOrientation(Owner.Center, 64, 8))
                {
                    var firer = ProjFirer.From<LightSpandStar>(Projectile);
        
                    firer.velocity = posVel.velocity.SafeNormalize(Vector2.Zero) * 32;
                    firer.position = Owner.Center;
                    firer.New();
                }
         
                _projSpawned = true;
            }

        }

        if(IsFinishingSwing())
        {
            Projectile.velocity = -Vector2.UnitY;
            if (this.OwnedByLocalClient() && !_projSpawned && Interpolant > 0.3f)
            {
                var firer = ProjFirer.From<LightSpandConstellation>(Projectile);
                firer.ai1 = 60;
                firer.New();
                _projSpawned = true;
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
            Duration = 35,
            XSwingRadius = 64,
            YSwingRadius = 64,
            SwingDegrees = 235,
            Easing = EasingFunction.InOutExpo,
            Sound = swingSound1
        });


        Add(new SwordSwing
        {
            baseSwingTime = 120,
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
        outlineColor = Color.Yellow;
        useBloom = true;
        bloom.innerBloomColor = Color.Gold;
        bloom.outerBloomColor = Color.DarkOrange;
        bloom.bloomWidthFunction = GetBloomWidth;
        bloom.bloomColorFunction = GetBloomColor;

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
        if(ComboIndex == 0 && !_firedProj && this.OwnedByLocalClient() && Interpolant > 0.3f)
        {
            for(var i = 0; i < 14; i++)
            {
                var firer = ProjFirer.From<LightSpandStar>(Projectile);
                firer.position = Owner.Center;
                firer.velocity = Projectile.velocity.RotatedByRandom(1.5f).Resize(Main.rand.NextFloat(6f, 45F)).RotatedBy(-0.05f * MathF.Sign(Projectile.velocity.X));
                firer.New();
            }
            _firedProj = true;
        }
        if (IsFinishingSwing())
        {
            Projectile.velocity = -Vector2.UnitY;
            if (this.OwnedByLocalClient() && !_firedProj && Interpolant > 0.3f)
            {
                var firer = ProjFirer.From<LightSpandConstellation>(Projectile);
                firer.ai1 = 120;
                firer.New();
                _firedProj = true;
            }
        }

    }


    public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
    {
        base.ModifyHitNPC(target, ref modifiers);

        modifiers.FinalDamage *= 3;
        modifiers.Knockback *= 4;
    }

    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
        SwingPlayerV2 comboPlayer = Owner.GetModPlayer<SwingPlayerV2>();
        int combo = ComboIndex + 1;
        int dir = comboPlayer.ComboDirection;

        if (combo < ComboCount && this.OwnedByLocalClient())
        {
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position, Main.MouseWorld - Owner.Center, Projectile.type, Projectile.damage, Projectile.knockBack,
                        Projectile.owner, ai2: combo, ai1: dir);
        }
    }
}