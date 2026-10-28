using Stellamod.Common.Particles;
using Stellamod.Common.Shaders;
using Stellamod.Content.CommonMaterials;
using Stellamod.Content.Dusts;
using Stellamod.Core.Bases;
using Stellamod.Core.Effects.Trails;
using Stellamod.Core.SwingSystem;
using Stellamod.Items;
using Stellamod.Projectiles.Swords.Altride;
using Stellamod.Trailing;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;


namespace Stellamod.Content.Areas.Tundra.Snow.WeaponsSN;

public class MooningSlicer : BaseSwingItemV2
{
    public override string Texture => AssetReferences.Assets.Textures.Ice.Items.MooningSlicer.KEY;
    public override void SetDefaults2()
    {
        base.SetDefaults2();
        Item.shoot = ModContent.ProjectileType<MooningSlicerSlash>();
        staminaProjectileShoot = ModContent.ProjectileType<MooningSlicerStaminaSlash>();
        meleeWeaponType = MeleeWeaponType.Knives;
        staminaCost = 3;
    }

    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew(
            mold: ModContent.ItemType<BlankSword>(),
            material: ModContent.ItemType<WinterbornShard>());
    }
}

public class MooningSlicerSlash : BaseSwingProjectileV2
{
    Vector2 _hitPoint;
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

        SoundStyle s1 = swingSound1;
        SoundStyle s2 = swingSound2;
        Add(new OvalSwing
        {
            Duration = 24,
            XSwingRadius = 44,
            YSwingRadius = 24,
            SwingDegrees = 270,
            Easing = EasingFunction.OutExpo,
            Sound = s1,
        });

        Add(new OvalSwing
        {
            Duration = 24,
            XSwingRadius = 44,
            YSwingRadius = 24,
            SwingDegrees = 270,
            Easing = EasingFunction.OutExpo,
            Sound = s2
        });


        SlashTrailBuilder slashTrailBuilder = new SlashTrailBuilder();
        SlashTrailer slashTrailer = slashTrailBuilder.Instantiate();
        slashTrailer.invert = ComboIndex % 2 != 0;
        Trailer = slashTrailer;

        trailOffsetOverride = 1.75f;
        useBloom = true;
        bloom.innerBloomColor = Color.White;
        bloom.outerBloomColor = Color.SkyBlue;
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
        Color blue = Color.Lerp(Color.Lerp(Color.White, Color.SkyBlue, 0.5f), Color.SkyBlue, ExtraMath.Osc(0f, 1f, speed: 4));
        return blue;
    }

    public override void AI()
    {
        base.AI();
        if(_hit && Interpolant >= 0.5f)
        {
            if(Timer % 45 == 0)
            {
                if (this.OwnedByLocalClient())
                {
                    var firer = ProjFirer.From<FallingMoonKnife>(Projectile);
                    firer.damage /= 2;
                    firer.position = _hitPoint + new Vector2(0, -252);
                    firer.position.X += Main.rand.NextFloat(-200, 200);
                    firer.velocity = Vector2.UnitY * 4;
                    firer.velocity = (_hitPoint - firer.position).Resize(8);
                    firer.New();
                }
            }
        }
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        base.OnHitNPC(target, hit, damageDone);
        if (!_hit)
        {
            _hit = true;
            _hitPoint = target.Center;

            var sound = AssetReferences.Assets.Sounds.SunStalker_Sun_Shot.Asset with { PitchVariance = 0.6f, Pitch = 0.3f, Volume = 0.2f };
            SoundEngine.PlaySound(sound, target.Center);
        }

    }
    
    public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
    {
        base.ModifyHitNPC(target, ref modifiers);
        SoundStyle spearHit = SoundRegistry.SpearHit1;
        spearHit.PitchVariance = 0.5f;
        SoundEngine.PlaySound(spearHit, Projectile.position);
        target.AddBuff(BuffID.Frostburn, 120);
        if (ComboIndex == 5)
        {
            modifiers.FinalDamage *= 2;
        }
    }
}

public class MooningSlicerStaminaSlash : BaseSwingProjectileV2
{
    private bool _fire;
 
    public override void DefineCombo()
    {
        base.DefineCombo();
        SoundStyle swingSound1 = SoundRegistry.HeavySwordSlash1;
        swingSound1.PitchVariance = 0.5f;
        Add(new OvalSwing
        {
            Duration = 44,
            XSwingRadius = 160 / 1.5f,
            YSwingRadius = 80 / 1.5f,
            SwingDegrees = 270,
            Easing = (lerpValue) => Easing.InOutExpo(lerpValue, 10),
            Sound = swingSound1
        });

        SlashTrailBuilder slashTrailBuilder = new SlashTrailBuilder();
        SlashTrailer slashTrailer = slashTrailBuilder.Instantiate();
        slashTrailer.invert = ComboIndex % 2 != 0;
        Trailer = slashTrailer;

    
        useBloom = true;
        bloom.innerBloomColor = Color.White;
        bloom.outerBloomColor = Color.SkyBlue;
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
        Color blue = Color.Lerp(Color.Lerp(Color.White, Color.SkyBlue, 0.5f), Color.SkyBlue, ExtraMath.Osc(0f, 1f, speed: 4));
        return blue;
    }

    public override void AI()
    {
        base.AI();

        Vector2 swingDirection = Projectile.velocity.SafeNormalize(Vector2.Zero);
        if (Interpolant > 0.5f && !_fire)
        {
            for (float i = 0; i < 2f; i++)
            {
                float progress = i / 4f;
                float rot = progress * MathHelper.ToRadians(360);
                rot += Main.rand.NextFloat(-0.5f, 0.5f);
                Vector2 offset = rot.ToRotationVector2() * 24;
                var particle = FXUtil.GlowCircleDetailedBoom1(Projectile.Center,
                    innerColor: Color.LightCyan,
                    glowColor: Color.LightCyan,
                    outerGlowColor: Color.Cyan,
                    baseSize: Main.rand.NextFloat(0.1f, 0.2f),
                    duration: Main.rand.NextFloat(15, 25));
                particle.Rotation = rot + MathHelper.ToRadians(45);
            }


            for (float f = 0; f < 7; f++)
            {
                Vector2 vel = Projectile.velocity;
                vel *= Main.rand.NextFloat(0.5f, 1.5f);
                vel = vel.RotatedByRandom(MathHelper.ToRadians(65));
                Dust.NewDustPerfect(Owner.Center, ModContent.DustType<GlowDust>(), vel, newColor: Color.LightCyan);
            }
            if (Main.myPlayer == Projectile.owner)
            {
                Vector2 shootVelocity = Projectile.velocity;

                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Owner.Center + new Vector2(0, -4), Projectile.velocity,
                    ModContent.ProjectileType<MooningProj>(), Projectile.damage * 2, Projectile.knockBack, Projectile.owner);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Owner.Center + new Vector2(0, 4), Projectile.velocity,
                    ModContent.ProjectileType<MooningProj>(), Projectile.damage * 2, Projectile.knockBack, Projectile.owner);
            }
            _fire = true;
        }
    }

    public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
    {
        base.ModifyHitNPC(target, ref modifiers);
        SoundStyle spearHit = SoundRegistry.SpearHit1;
        spearHit.PitchVariance = 0.5f;
        SoundEngine.PlaySound(spearHit, Projectile.position);
        if (ComboIndex == 5)
        {
            modifiers.FinalDamage *= 2;
        }
    }
}

public class FallingMoonKnife : ModProjectile
{
    ref float Timer => ref Projectile.ai[0];
    float InScale => EasingFunction.InOutSine(Timer / 30f);
    public override string Texture => AssetReferences.Assets.Textures.Ice.Items.MooningSlicerKnife.KEY;
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        Projectile.SetTrailCacheLength(32);
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = Projectile.height = 16;
        Projectile.timeLeft = 150;
        Projectile.light = 0.4f;
        Projectile.friendly = true;
        Projectile.tileCollide = false;
        Projectile.extraUpdates = 1;
    }

    public override void AI()
    {
        base.AI();

        Timer++;
        if(Projectile.velocity.Length() < 14f)
            Projectile.velocity *= 1.05f;
        if (Timer >= 30)
            Projectile.tileCollide = true;
        Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;
    }
    public float GetTrailWidth(float ratio)
    {
        return MathHelper.SmoothStep(17, 0, ratio);
    }
    public Color GetTrailColor(float ratio)
    {
        return DrawUtilities.InterpolateColorArray(ratio, Color.White, Color.White, Color.Gray, Color.SkyBlue, Color.White) * 1.4f * MathHelper.SmoothStep(1f, 0f, ratio);
    }
    public override bool PreDraw(ref Color lightColor)
    {
        var pass = AssetReferences.Effects.Abyss.SingularTrail.CreatePrimitivesPass();
        pass.Parameters.transformMatrix = TrailDrawer.WorldViewPoint2;
        pass.Parameters.insideColor = Color.DarkBlue.ToVector3();
        pass.Parameters.bloomColor = Color.White.ToVector3();
        //  pass.Parameters.time = Main.GlobalTimeWrappedHourly * 

        var hlsl = new HlslSampler();
        hlsl.Sampler = SamplerState.LinearWrap;
        hlsl.Texture = AssetReferences.Assets.LaserTextures.Beamlight.Asset.Value;
        pass.Parameters.laserSampler = hlsl;
        pass.Apply();

        TrailDrawer.Draw(Projectile.oldPos, GetTrailColor, GetTrailWidth, pass.Shader, Projectile.Size * 0.5F);

        DrawUtilities.DrawSpriteAfterImage(Main.spriteBatch, Projectile, Color.SkyBlue, Color.Transparent, 0.44f * InScale);
        var drawer = Projectile.Drawer;
        drawer.color *= InScale;
        Main.spriteBatch.Draw(drawer);

        drawer.color = Color.White * ExtraMath.Osc(0.5f, 1f, speed: 12, Projectile.identity * 4) * InScale;
        drawer.color.A = 0;
        Main.spriteBatch.Draw(drawer);
        DrawUtilities.DrawBasicGlow(Main.spriteBatch, Projectile.Center, 0.2f, Color.SkyBlue * 0.45f * ExtraMath.Osc(0.5f, 1f, speed: 16));
        return false;
    }

    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
        var factory = ParticleUtils.ParticleFactory.FromSmallBurst(Projectile.Center, TriColorPalette.Icey, new Vector2(4, 10));
        ParticleUtils.CreateSwirlingDustBurst(factory);
    }
}

public class MooningProj : ModProjectile
{
    public override string Texture => AssetReferences.Assets.Textures.Ice.Items.MooningProj.KEY;
    private ref float Timer => ref Projectile.ai[0];
    Vector2 StartVelocity;
    public override void SetStaticDefaults()
    {
        // DisplayName.SetDefault("Gladiator Spear");
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 15;
        ProjectileID.Sets.TrailingMode[Projectile.type] = 1;
    }
    public override void SetDefaults()
    {
        Projectile.DamageType = DamageClass.Melee;
        Projectile.penetrate = 2;
        Projectile.width = 30;
        Projectile.height = 30;
        Projectile.timeLeft = 700;
        Projectile.alpha = 255;
        Projectile.friendly = false;
        Projectile.hostile = false;
        Projectile.ignoreWater = true;
        Projectile.tileCollide = false;
    }
    public override void AI()
    {
        Timer++;
        if (Timer == 1)
        {
            StartVelocity = Projectile.velocity;
        }

        if (Timer == 2)
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + 1.57f + 3.14f;
            Projectile.velocity = -Projectile.velocity;
        }

        if (Timer == 5 || Timer == 10 || Timer == 15 || Timer == 20)
        {
            var EntitySource = Projectile.GetSource_FromThis();
            if (Main.rand.NextBool(8) && Main.myPlayer == Projectile.owner)
            {
                Projectile.NewProjectile(EntitySource, Projectile.Center.X + Main.rand.Next(-50, 50), Projectile.Center.Y + Main.rand.Next(-50, 50), StartVelocity.X, StartVelocity.Y, ModContent.ProjectileType<Altride4>(), Projectile.damage * 2, 1, Projectile.owner, 0, 0);
            }

            if (Main.rand.NextBool(3) && Main.myPlayer == Projectile.owner)
            {
                Projectile.NewProjectile(EntitySource, Projectile.Center.X + Main.rand.Next(-50, 50), Projectile.Center.Y + Main.rand.Next(-50, 50), StartVelocity.X, StartVelocity.Y, ModContent.ProjectileType<Altride4>(), Projectile.damage * 2, 1, Projectile.owner, 0, 0);
            }
            else
            {
                if (Main.myPlayer == Projectile.owner)
                {
                    Projectile.NewProjectile(EntitySource, Projectile.Center.X + Main.rand.Next(-50, 50), Projectile.Center.Y + Main.rand.Next(-50, 50), StartVelocity.X, StartVelocity.Y, ModContent.ProjectileType<Altride4>(), Projectile.damage, 1, Projectile.owner, 0, 0);
                }

            }
        }
        if (Timer >= 0 && Timer <= 20)
        {
            Projectile.velocity *= .86f;

        }
        if (Timer == 20)
        {
            Projectile.velocity = -Projectile.velocity;
        }
        if (Timer >= 21 && Timer <= 60)
        {
            Projectile.velocity /= .90f;

        }
        if (Timer == 60)
        {
            Projectile.tileCollide = true;
        }
    }

    public override void OnKill(int timeLeft)
    {

    }

    public override Color? GetAlpha(Color lightColor)
    {
        return Color.White;
    }

    public override bool PreDraw(ref Color lightColor)
    {
        return false;
    }

    public override void PostDraw(Color lightColor)
    {

    }
}