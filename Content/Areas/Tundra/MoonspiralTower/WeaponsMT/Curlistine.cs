using Stellamod.Assets;
using Stellamod.Common.Particles;
using Stellamod.Common.Shaders;
using Stellamod.Common.ShockCircleSystem;
using Stellamod.Content.CommonMaterials;
using Stellamod.Core;
using Stellamod.Core.Bases;
using Stellamod.Core.Effects.Trails;
using Stellamod.Core.SwingSystem;
using Stellamod.Effects.RoyalMagic;
using Stellamod.Items;
using Stellamod.Items.Accessories.Players;
using Stellamod.Trailing;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Tundra.MoonspiralTower.WeaponsMT;

file static class CurlistineCommon
{
    public static float GetTrailWidth(float ratio)
    {
        return MathHelper.SmoothStep(24, 12, ratio);
    }
    public static Color GetTrailColor(float ratio)
    {
        return DrawUtilities.InterpolateColorArray(ratio, Color.Purple, Color.SkyBlue, Color.White) * 1.4f * MathHelper.SmoothStep(1f, 0f, ratio);
    }
    public static void DrawChain(Vector2[] points)
    {
        for (var i = 1; i < points.Length; i++)
        {
            var drawer = SpritebatchDrawer.FromTextureAsset(AssetReferences.Content.Areas.Tundra.MoonspiralTower.WeaponsMT.Curlistine_Chain.Asset, points[i]);
            drawer.rotation = (points[i] - points[i-1]).ToRotation();
            drawer.color *= ExtraMath.Osc(0.15f, 0.45f, speed: 6, offset: i * 10);
            drawer.color.A = 0;
            Main.spriteBatch.Draw(drawer);
        }
    }
    public static void DrawChain(Player Owner, Projectile Projectile)
    {
        var ownerPos = Owner.Center;
        var projPos = Projectile.Center;
        var len = 8F;
        var dist = Vector2.Distance(ownerPos, projPos);
        var pullPos = Vector2.Lerp(ownerPos, projPos, 0.5f);
        var dir = (projPos - ownerPos).SafeNormalize(Vector2.Zero);
        dir = dir.RotatedBy(MathHelper.PiOver2);
        pullPos += dir * 24;

        dist /= len;
        var stepper = 1f / dist;
        for (var i = 0F; i < 1f; i += stepper)
        {
      
            var inbetween = Vector2.Lerp(ownerPos, projPos, i);
            inbetween = Vector2.Lerp(inbetween, pullPos, EasingFunction.QuadraticBump(i) * 0.7f);
            var drawer = SpritebatchDrawer.FromTextureAsset(AssetReferences.Content.Areas.Tundra.MoonspiralTower.WeaponsMT.Curlistine_Chain.Asset, inbetween);
            drawer.rotation = (Projectile.Center - inbetween).ToRotation();
            drawer.color *= ExtraMath.Osc(0.15f, 0.45f, speed: 6, offset: i * 10);
            drawer.color.A = 0;
            Main.spriteBatch.Draw(drawer);
        }
    }
    public static void DrawTrail(Vector2[] trailCache)
    {

        AlcadSlashShader shader = ShaderContent.GetInstance<AlcadSlashShader>();
        shader.ScrollingLaser = TrailRegistry.Beamlight.Value;
        shader.Noise = AssetManager.Noise.Whirly.Value;
        shader.Slash = AssetManager.GlowMask.SwordSlashForward.Asset.Value;
        shader.BloomColor = Color.SkyBlue;
        shader.Time = Main.GlobalTimeWrappedHourly * 24;
        shader.TransformMatrix = TrailDrawer.WorldViewPoint2;
        shader.Distortion = 0.6f;
        TrailDrawer.Draw(trailCache, GetTrailColor, GetTrailWidth, shader);
    }
}
public class Curlistine : BaseSwingItemV2
{
    public override void SetDefaults2()
    {
        base.SetDefaults2();
        Item.damage = 47;
        Item.ArmorPenetration = 5;
        Item.shoot = ModContent.ProjectileType<CurlistineSwing>();
        staminaProjectileShoot = ModContent.ProjectileType<CurlistineStaminaSwing>();
        meleeWeaponType = MeleeWeaponType.Sword;
        staminaCost = 1;
    }

    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew(mold: ModContent.ItemType<BlankSword>(), material: ModContent.ItemType<PearlescentScrap>());
    }
}
public class CurlistineSwing : BaseSwingProjectileV2
{
    Vector2[] _chainPoints;
    Vector2[] _flippedPoints;
    bool _hit;
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


        SoundStyle swingSoundAlt1 = AssetRegistry.Sounds.Melee.SwordSwing2;
        swingSoundAlt1.PitchVariance = 0.25f;

        SoundStyle swingSoundAlt2 = AssetRegistry.Sounds.Melee.SwordSwing3;
        swingSoundAlt2.PitchVariance = 0.25f;

        int style = 0;
        SoundStyle s1 = style == 0 ? swingSound1 : swingSoundAlt1;
        SoundStyle s2 = style == 0 ? swingSound2 : swingSoundAlt2;
        Add(new SwordSwing
        {
            baseSwingTime = 28,
            swordMovementFunction = new Swings.CurlSwing
            {
                frequency = 6,
                swingRadians = 1.5f,
                xRange = 144
            },
            sound = s1,
        });
        Add(new SwordSwing
        {
            baseSwingTime = 28,
            swordMovementFunction = new Swings.CurlSwing
            {
                frequency = 6,
                swingRadians = 1.5f,
                xRange = 128
            },
            sound = s2,
        });
        Add(new SwordSwing
        {
            baseSwingTime = 22,
            swordMovementFunction = new Swings.CurlSwing
            {
                frequency = 6,
                swingRadians = 1f,
                xRange = 144
            },
            sound = s1,
        });
        Add(new SwordSwing
        {
            baseSwingTime = 22,
            swordMovementFunction = new Swings.CurlSwing
            {
                frequency = 6,
                swingRadians = 0.7f,
                xRange = 128
            },
            sound = s2,
        }); 
        Add(new SwordSwing
        {
            baseSwingTime = 36,
            swordMovementFunction = new Swings.CurlSwing
            {
                frequency = 6,
                swingRadians = 6.5f,
                xRange = 164
            },
            sound = s1,
        });
        Add(new SwordSwing
        {
            baseSwingTime = 100,
            swordMovementFunction = new Swings.SpinningSwing
            {
                radians = 24,
                radius = 129
            },
            hitCount = 4,
            sound = swingSound3,
        });

        useAfterImage = true;

        useBloom = true;
        bloom.innerBloomColor = Color.SkyBlue;
        bloom.outerBloomColor = Color.Blue;
        bloom.bloomWidthFunction = CurlistineCommon.GetTrailWidth;
        bloom.bloomColorFunction = CurlistineCommon.GetTrailColor;
        glowAfterImageColor = Color.Blue;
    }

    public override void AI()
    {
        base.AI();
        _chainPoints ??= new Vector2[24];
        if(Timer == 1)
        {
            for(var i =0;i  < _chainPoints.Length; i++)
            {
                _chainPoints[i] = Projectile.velocity.SafeNormalize(Vector2.Zero);
            }
        }
        var targetAngle = (Projectile.Center - Owner.Center).ToRotation();
        var distPer = Vector2.Distance(Owner.Center, Projectile.Center) / (float)_chainPoints.Length;
        for(var i = 0; i < _chainPoints.Length; i++)
        {
            ref var chp = ref _chainPoints[i];
            var ratio = (float)i / (float)_chainPoints.Length;
            var myAngle = (chp - Owner.Center).ToRotation();
            var newAngle = Utils.AngleLerp(myAngle, targetAngle, ratio * 0.1f);
            var newDirection = newAngle.ToRotationVector2();
            chp = Owner.Center + i * newDirection * distPer;
        }
        growScale = MathHelper.Lerp(0f, 0.1f, EasingFunction.QuadraticBump(Interpolant));
        outlineColor = Color.Lerp(Color.White, Color.Transparent, ExtraMath.Osc(0f, 1f, speed: 12));
        if (Timer % 16 == 0)
        {
            var pos = Projectile.Center;
            pos += Main.rand.NextVector2Circular(32, 32);
            var vel = Main.rand.NextVector2Circular(12, 12);
            Particles.SwirlingFlameDust.Spawn(BitDustFactory.SlowingOverTime with
            {
                position = pos,
                velocity = vel,
                innerColor = Color.SkyBlue.ToVector4(),
                outerColor = Color.DarkBlue.ToVector4(),
                scale = new Vector2(Main.rand.NextFloat(0.6f, 1f)),
                timeLeft = 120
            });
        }
    }
    public override bool PreDraw(ref Color lightColor)
    {
        _chainPoints ??= new Vector2[24];
        _flippedPoints ??= new Vector2[_chainPoints.Length];
        for(var i =0; i  < _flippedPoints.Length; i++)
        {
            ref var fp = ref _flippedPoints[i];
            var axis = Vector2.Lerp(Owner.Center, Projectile.Center, (float)i / (float)_flippedPoints.Length);
            var chainPoint = _chainPoints[i];
            var dirTo = axis - chainPoint;
            fp = axis + dirTo;
        }
        CurlistineCommon.DrawChain(_flippedPoints);
        return base.PreDraw(ref lightColor);
    }


    public override void RenderSwingTrail(ref Color lightColor, Vector2[] points)
    {
        base.RenderSwingTrail(ref lightColor, points);
        CurlistineCommon.DrawTrail(points);
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        base.OnHitNPC(target, hit, damageDone);
        if (!_hit)
        {
            for (var i = 0; i < 7; i++)
            {
                var pos = target.Center + Main.rand.NextVector2Circular(32, 32);
                var vel = -Vector2.UnitY * Main.rand.NextFloat(2f, 6f);
                vel = vel.RotatedByRandom(4.44f);
                Particles.SwirlingFlameDust.Spawn(BitDustFactory.SlowingOverTime with
                {
                    position = pos,
                    velocity = vel,
                    timeLeft = 120,
                    innerColor = Color.SkyBlue.ToVector4(),
                    outerColor = Color.DarkBlue.ToVector4(),
                    scale = new Vector2(Main.rand.NextFloat(0.5f, 1.3f))
                });
            }
            FXUtil.GlowCircleBoom(target.Center, Color.White, Color.SkyBlue, Color.DarkBlue);
            _hit = true;
        }
    }
}
public class CurlistineStaminaSwing : BaseSwingProjectileV2
{
    Vector2[] _chainPoints;
    Vector2[] _flippedPoints;
    bool _hit;
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

        Add(new SwordSwing
        {
            baseSwingTime = 100,
            swordMovementFunction = new Swings.SpinningSwing
            {
                radians = 32,
                radius = 162
            },
            hitCount = 8,
            sound = swingSound3,
        });

        useAfterImage = true;
        useBloom = true;
        bloom.innerBloomColor = Color.SkyBlue;
        bloom.outerBloomColor = Color.Blue;
        bloom.bloomWidthFunction = CurlistineCommon.GetTrailWidth;
        bloom.bloomColorFunction = CurlistineCommon.GetTrailColor;
        glowAfterImageColor = Color.Blue;
    }

    public override void AI()
    {
        base.AI();
        Owner.GetModPlayer<DashPlayer>().noRecharge = true;
        growScale = MathHelper.Lerp(0f, 0.1f, EasingFunction.QuadraticBump(Interpolant));
        outlineColor = Color.Lerp(Color.White, Color.Transparent, ExtraMath.Osc(0f, 1f, speed: 12));
        if(Timer == 1)
        {
            ShockCircles.CreateQuickWhiteFlash(Owner.Center);
        }
        _chainPoints ??= new Vector2[24];
        if (Timer == 1)
        {
            for (var i = 0; i < _chainPoints.Length; i++)
            {
                _chainPoints[i] = Projectile.velocity.SafeNormalize(Vector2.Zero);
            }
        }
        var targetAngle = (Projectile.Center - Owner.Center).ToRotation();
        var distPer = Vector2.Distance(Owner.Center, Projectile.Center) / (float)_chainPoints.Length;
        for (var i = 0; i < _chainPoints.Length; i++)
        {
            ref var chp = ref _chainPoints[i];
            var ratio = (float)i / (float)_chainPoints.Length;
            var myAngle = (chp - Owner.Center).ToRotation();
            var newAngle = Utils.AngleLerp(myAngle, targetAngle, ratio * 0.3f);
            var newDirection = newAngle.ToRotationVector2();
            chp = Owner.Center + i * newDirection * distPer;
        }
        if (Timer % 16 == 0)
        {
            var pos = Projectile.Center;
            pos += Main.rand.NextVector2Circular(32, 32);
            var vel = Main.rand.NextVector2Circular(12, 12);
            Particles.SwirlingFlameDust.Spawn(BitDustFactory.SlowingOverTime with
            {
                position = pos,
                velocity = vel,
                innerColor = Color.SkyBlue.ToVector4(),
                outerColor = Color.DarkBlue.ToVector4(),
                scale = new Vector2(Main.rand.NextFloat(0.6f, 1f)),
                timeLeft = 120
            });
        }
    }
    public override bool PreDraw(ref Color lightColor)
    {
        _chainPoints ??= new Vector2[24];
        _flippedPoints ??= new Vector2[_chainPoints.Length];
        for (var i = 0; i < _flippedPoints.Length; i++)
        {
            ref var fp = ref _flippedPoints[i];
            var axis = Vector2.Lerp(Owner.Center, Projectile.Center, (float)i / (float)_flippedPoints.Length);
            var chainPoint = _chainPoints[i];
            var dirTo = axis - chainPoint;
            fp = axis + dirTo;
        }
        CurlistineCommon.DrawChain(_flippedPoints);
        return base.PreDraw(ref lightColor);
    }

    public override void RenderSwingTrail(ref Color lightColor, Vector2[] points)
    {
        base.RenderSwingTrail(ref lightColor, points);
        CurlistineCommon.DrawTrail(points);
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

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        base.OnHitNPC(target, hit, damageDone);
        SoundStyle spearHit2 = SoundRegistry.NSwordHit1;
        spearHit2.PitchVariance = 0.2f;
        spearHit2.Volume = 0.6f;
        SoundEngine.PlaySound(spearHit2, Projectile.position);
        if (!_hit)
        {

            for (var i = 0; i < 7; i++)
            {
                var pos = target.Center + Main.rand.NextVector2Circular(32, 32);
                var vel = -Vector2.UnitY * Main.rand.NextFloat(2f, 6f);
                vel = vel.RotatedByRandom(4.44f);
                Particles.SwirlingFlameDust.Spawn(BitDustFactory.SlowingOverTime with
                {
                    position = pos,
                    velocity = vel,
                    timeLeft = 120,
                    innerColor = Color.SkyBlue.ToVector4(),
                    outerColor = Color.DarkBlue.ToVector4(),
                    scale = new Vector2(Main.rand.NextFloat(0.5f, 1.3f))
                });
            }
            FXUtil.GlowCircleBoom(target.Center, Color.White, Color.SkyBlue, Color.DarkBlue);
            _hit = true;
        }
    }
}
