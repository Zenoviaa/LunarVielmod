using Stellamod.Common.Particles;
using Stellamod.Common.XixianFlaskSystem;
using Stellamod.Content.CommonMaterials;
using Stellamod.Content.Rendering;
using Stellamod.Core;
using Stellamod.Core.Bases;
using Stellamod.Core.SwingSystem;
using Stellamod.Items;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.SpringHills.WeaponsSH;

public class FlaskuserBlade : BaseSwingItemV2
{
    public override void SetDefaults2()
    {
        base.SetDefaults2();
        Item.damage = 7;
        Item.shoot = ModContent.ProjectileType<FlaskuserBladeSlash>();
        Item.autoReuse = true;
        staminaProjectileShoot = ModContent.ProjectileType<FlaskuserBladeStaminaSlash>();
        meleeWeaponType = MeleeWeaponType.Sword;
        staminaCost = 8;
    }
    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew<Ivythorn, BlankSword>();
    }
}



public class FlaskuserBladeSlash : BaseSwingProjectileV2
{
    bool _thrusted;
    public override void SetDefaults2()
    {
        base.SetDefaults2();
        Projectile.width = 66;
        trailOffsetOverride = 1.5f;
    }

    public override void DefineCombo()
    {
        base.DefineCombo();
        //two wide slashs
        //Then two smaller slashes that thrust you forward a bit
        //on the last hit of the combo hold the sword up and drink a 2hp potion
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

        SoundStyle s1 = swingSound1;
        SoundStyle s2 = swingSound2;

        Add(new SwordSwing
        {
            baseSwingTime = 21,
            swordMovementFunction = new Swings.CurlSwing
            {
                frequency = 6,
                swingRadians = 2,
                xRange = 96
            },
            sound = s1,
        });
        Add(new SwordSwing
        {
            baseSwingTime = 21,
            swordMovementFunction = new Swings.CurlSwing
            {
                frequency = 6,
                swingRadians = 2,
                xRange = 96
            },
            sound = s2,
        });

        Add(new SwordSwing
        {
            baseSwingTime = 14,
            swordMovementFunction = new Swings.CurlSwing
            {
                frequency = 6,
                swingRadians = 0.68f,
                xRange = 96
            },
            sound = s1,
        });

        Add(new SwordSwing
        {
            baseSwingTime = 14,
            swordMovementFunction = new Swings.CurlSwing
            {
                frequency = 6,
                swingRadians = 0.68f,
                xRange = 96
            },
            sound = s2,
        });

        Add(new SwordSwing
        {
            baseSwingTime = 28,
            swordMovementFunction = new Swings.HoldUpSwing
            {
                thrustDistance = 64,
            },
            sound = s1,
        });
    }


    public override void AI()
    {
        base.AI();
        switch (ComboIndex)
        {
            case 2:
            case 3:
                if (!_thrusted && Interpolant >= 0.22f)
                {
                    Owner.ImpulseVelocity = Projectile.velocity.SafeNormalize(Vector2.Zero) * 5;
                    _thrusted = true;
                }
                break;
            case 4:
                {
                    if (!_thrusted && Interpolant >= 0.22f)
                    {
                        if (this.OwnedByLocalClient())
                        {
                            var firer = ProjFirer.From<LittleFlask>(Projectile);
                            firer.position += new Vector2(0, -16);
                            firer.velocity = new Vector2(0, -8);
                            firer.New();
                        }
                        _thrusted = true;
                    }
                    Projectile.velocity = -Vector2.UnitY;
                }
                break;
        }
    }

    public override void RenderSwingTrail(ref Color lightColor, Vector2[] points)
    {
        base.RenderSwingTrail(ref lightColor, points);
        CommonTrails.DrawGlowTrail(points,
            (float f) => Color.Lerp(Color.White, Color.DarkGray, f),
            (float f) => MathHelper.SmoothStep(16, 8, f),
            TriColorPalette.Silver);
    }
    public override void PostDrawSword(Vector2 position, Rectangle srcRect, Color drawColor, float rotation, Vector2 origin, Vector2 drawScale, SpriteEffects spriteEffect, float layerDepth)
    {
        base.PostDrawSword(position, srcRect, drawColor, rotation, origin, drawScale, spriteEffect, layerDepth);
        if (ComboIndex == 4)
        {
            var drawer = SpritebatchDrawer.FromTextureAsset(AssetReferences.Assets.GlowMasks.MagicCastSparkle.Asset, Projectile.Center);
            drawer.color = Color.Lerp(Color.Transparent, Color.White, EasingFunction.QuickOutSlowIn(Interpolant));
            drawer.color *= ExtraMath.Osc(0.7f, 1f, speed: 6);
            drawer.color.A = 0;
            drawer.scale = Vector2.Lerp(new Vector2(1.4f), Vector2.One, EasingFunction.OutCirc(Interpolant));
            drawer.scale *= 0.45f;
            drawer.worldPosition.Y -= 42;
            Main.spriteBatch.Draw(drawer);
        }
    }

    public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
    {
        base.ModifyHitNPC(target, ref modifiers);
        if (IsFinishingSwing())
        {
            DamageHelper.PercentIncreasedamage(ref modifiers, 0.25f);
        }
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        base.OnHitNPC(target, hit, damageDone);
    }
}

public class LittleFlask : ModProjectile
{
    Player Owner => Main.player[Projectile.owner];
    ref float Timer => ref Projectile.ai[0];
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 4;
        Projectile.height = 4;
        Projectile.timeLeft = 120;
        Projectile.light = 0.5f;
    }
    public override void AI()
    {
        base.AI();
        Timer++;
        Projectile.SetTrailCacheLength(12);
        Projectile.rotation += Projectile.velocity.Length() + 0.01f;
        Projectile.velocity.Y += 0.3f;
    }

    public override bool PreDraw(ref Color lightColor)
    {
        DrawUtilities.DrawAdditiveFadingTrail(Projectile, Color.Red, Color.Transparent, 0.25f);
        Main.spriteBatch.Draw(Projectile.Drawer);
        return false;
    }

    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
        var factory = ParticleUtils.ParticleFactory.FromSmallBurst(Projectile.Center, Projectile.Center - new Vector2(0, 16), TriColorPalette.Health, new Vector2(5, 15f));
        ParticleUtils.CreateSwirlingDustBurst(factory);
        Owner.Heal(2);
        SoundEngine.PlaySound(SoundID.Shatter with { PitchVariance = 0.6f, Volume = 0.24f }, Projectile.position);
    }
}
public class FlaskuserBladeStaminaSlash : BaseSwingProjectileV2
{
    bool _thrusted;
    public override void SetDefaults2()
    {
        base.SetDefaults2();
        Projectile.width = 66;
    }

    public override void DefineCombo()
    {
        base.DefineCombo();
        //two wide slashs
        //Then two smaller slashes that thrust you forward a bit
        //on the last hit of the combo hold the sword up and drink a 2hp potion
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

        SoundStyle s1 = swingSound1;
        SoundStyle s2 = swingSound2;

        Add(new SwordSwing
        {
            baseSwingTime = 28,
            swordMovementFunction = new Swings.HoldUpSwing
            {
                thrustDistance = 64,
            },
            sound = s1,
        });
    }

    public override void AI()
    {
        base.AI();
        if (!_thrusted && Interpolant >= 0.22f)
        {
            if (this.OwnedByLocalClient())
            {
                var firer = ProjFirer.From<LittleFlask>(Projectile);
                firer.position += new Vector2(0, -8);
                firer.New();
            }
            Owner.GetModPlayer<FlaskPlayer>().ProcFlaskEffects();
            _thrusted = true;
        }
    }

    public override void RenderSwingTrail(ref Color lightColor, Vector2[] points)
    {
        base.RenderSwingTrail(ref lightColor, points);
        CommonTrails.DrawGlowTrail(points,
            (float f) => Color.Lerp(Color.White, Color.Transparent, f),
            (float f) => MathHelper.SmoothStep(32, 16, f),
            TriColorPalette.Silver);
    }

    public override void PostDrawSword(Vector2 position, Rectangle srcRect, Color drawColor, float rotation, Vector2 origin, Vector2 drawScale, SpriteEffects spriteEffect, float layerDepth)
    {
        base.PostDrawSword(position, srcRect, drawColor, rotation, origin, drawScale, spriteEffect, layerDepth);
        var drawer = SpritebatchDrawer.FromTextureAsset(AssetReferences.Assets.GlowMasks.MagicCastSparkle.Asset, Projectile.Center);
        drawer.color = Color.Lerp(Color.Transparent, Color.White, EasingFunction.QuickOutSlowIn(Interpolant));
        drawer.color *= ExtraMath.Osc(0.7f, 1f, speed: 6);
        drawer.scale = Vector2.Lerp(new Vector2(1.4f), Vector2.One, EasingFunction.OutCirc(Interpolant));
        Main.spriteBatch.Draw(drawer);
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        base.OnHitNPC(target, hit, damageDone);
    }
}