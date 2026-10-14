using Stellamod.Assets;
using Stellamod.Common;
using Stellamod.Common.Particles;
using Stellamod.Common.Shaders;
using Stellamod.Content.CommonMaterials;
using Stellamod.Core.Bases;
using Stellamod.Core.SwingSystem;
using Stellamod.Effects.RoyalMagic;
using Stellamod.Items;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Tundra.MoonspiralTower.WeaponsMT;

public class FrostSlashers : BaseSwingItemV2
{
    public override string Texture => AssetReferences.Assets.Textures.Item.FrostSlashers.KEY;
    public override void SetDefaults2()
    {
        base.SetDefaults2();
        Item.damage = 17;
        Item.shoot = ModContent.ProjectileType<FrostSlashersSwing>();
        staminaProjectileShoot = ModContent.ProjectileType<FrostSlashersStaminaThrow>();
        meleeWeaponType = MeleeWeaponType.Knives;
        staminaCost = 1;
        staminaDamageMultiplier = 2;
    }

    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew(mold: ModContent.ItemType<BlankSword>(), material: ModContent.ItemType<PearlescentScrap>());
    }
}

public class FrostSlashersSwing : BaseSwingProjectileV2
{
    bool _slowVelocity;
    bool _hasSpawnedSecondKnife;
    bool _hasThrusted;
    public override void DefineCombo()
    {
        base.DefineCombo();
        SoundStyle swingSound3 = AssetRegistry.Sounds.Melee.SwordSpin1;
        swingSound3.PitchVariance = 0.5f;
        swingSound3.Volume = 0.5f;
        Add(new SwordSwing
        {
            baseSwingTime = 60,
            hitCount = 8,
            swordMovementFunction = new Swings.SpinningSwing
            {
                radius = 48,
                radians = 32
            },
            sound = swingSound3,
        });

        for (var i = 0; i < 4; i++)
        {
            Add(new SwordSwing
            {
                baseSwingTime = 12,
                swordMovementFunction = new Swings.CurlSwing
                {
                    frequency = 6,
                    swingRadians = 1.5f,
                    xRange = 96
                },
                sound = SwingV2Helper.LightSlashSound(),
            });

            Add(new SwordSwing
            {
                baseSwingTime = 10,
                swordMovementFunction = new Swings.CurlSwing
                {
                    frequency = 6,
                    swingRadians = 1f,
                    xRange = 80
                },
                sound = SwingV2Helper.LightSlashSound(),
            });
        }
    }

    public override void AI()
    {
        base.AI();
        if (ComboIndex == 0 && !_hasThrusted && !isChildProjectile)
        {
            _hasThrusted = true;
            Owner.thrustVelocity = Projectile.velocity.Resize(12);
        }
        if(ComboIndex == 0 && _slowVelocity && Owner.velocity.Length() > 1)
        {
            Owner.thrustVelocity = Owner.velocity * 0.9f;
        }

        if (ComboIndex == 0)
        {
            Owner.Spin(MathHelper.TwoPi * MathF.Sign(Projectile.velocity.X) / swingTime * 12, 2);
            Owner.SetImmuneTimeForAllTypes(10);
            if (Main.rand.NextBool(64))
            {
                var pos = Owner.Center;
                pos += Main.rand.NextVector2Circular(32, 32);
                var vel = Main.rand.NextVector2Circular(0.5f, 0.5f);
                var d = Dust.NewDustPerfect(pos, DustID.IceTorch, vel, Scale: 0.2f);
                d.noGravity = true;
            }
        }

        if (!_hasSpawnedSecondKnife && ComboIndex != ComboCount - 1 && Interpolant >= 0.5f)
        {
            MirrorProjectile();
            _hasSpawnedSecondKnife = true;
        }

        growScale = MathHelper.Lerp(0f, 0.3f, EasingFunction.QuadraticBump(Interpolant));
        glowColor = Color.Lerp(Color.Black, Color.SkyBlue, EasingFunction.QuadraticBump(Interpolant));
    }

    private float GetTrailWidth(float ratio)
    {
        return MathHelper.SmoothStep(32, 0, ratio);
    }

    private Color GetTrailColor(float ratio)
    {
        return Color.Lerp(Color.White, Color.SkyBlue, ratio) * 1.4f * MathHelper.SmoothStep(0f, 1f, ratio);
    }

    public override void RenderSwingTrail(ref Color lightColor, Vector2[] points)
    {
        base.RenderSwingTrail(ref lightColor, points);
        var shader = ShaderContent.GetInstance<AlcadSlashShader>();
        shader.ScrollingLaser = TrailRegistry.Beamlight.Value;
        shader.Noise = AssetManager.Noise.Whirly.Value;
        shader.Slash = AssetManager.GlowMask.SwordSlashForward.Asset.Value;
        shader.BloomColor = Color.SkyBlue;
        shader.Time = Main.GlobalTimeWrappedHourly * 24;
        shader.TransformMatrix = TrailDrawer.WorldViewPoint2;
        shader.Distortion = 0.25F;
        TrailDrawer.Draw(points, GetTrailColor, GetTrailWidth, shader);
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        base.OnHitNPC(target, hit, damageDone);
        if (ComboIndex == 0)
        {
            _slowVelocity = true;
        }
        var fac = ParticleUtils.ParticleFactory.FromSmallBurst(Projectile.Center, target.Center, TriColorPalette.Icey, new Vector2(5f, 15f));
        fac.particleCount /= 3;
        ParticleUtils.CreateSwirlingDustBurst(fac);
        target.AddBuff(BuffID.Frostburn, 15);

        var spearHit = SoundRegistry.SpearHit1;
        spearHit.PitchVariance = 0.5f;
        spearHit.Volume = 0.2f;
        SoundEngine.PlaySound(spearHit, Projectile.position);

        Particles.IceShard.Spawn(new()
        {
            position = target.position + Main.rand.NextVector2Circular(32, 32),
            velocity = Main.rand.NextVector2Circular(4, 4),
            scale = Main.rand.NextFloat(0.7f, 1f),
            rotation = Main.rand.NextFloat(7f),
            timeLeft = Main.rand.Next(25, 40)
        });
    }

    public override void PostDrawSword(Vector2 position, Rectangle srcRect, Color drawColor, float rotation, Vector2 origin, Vector2 drawScale, SpriteEffects spriteEffect, float layerDepth)
    {
        base.PostDrawSword(position, srcRect, drawColor, rotation, origin, drawScale, spriteEffect, layerDepth);
        if (ComboIndex != 0)
            return;

        var drawer = SpritebatchDrawer.FromTextureAsset(AssetReferences.Assets.NoiseTextures.Spiin.Asset, Owner.Center);
        drawer.color = Color.White * 0.3f;
        drawer.color *= EasingFunction.QuadraticBump(Interpolant);
        drawer.color.A = 0;
        drawer.rotation = Main.GlobalTimeWrappedHourly * MathF.Sign(Projectile.velocity.X) * 7f;
        drawer.scale *= 0.3f;
        Main.spriteBatch.Draw(drawer);
    }
}

public class MarkedforIceI : ModBuff
{
    public override string Texture => AssetReferences.Assets.Textures.Buffs.MarkedforIce.KEY;
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
    }
    public override void Update(NPC npc, ref int buffIndex)
    {
        base.Update(npc, ref buffIndex);
        if (Main.rand.NextBool(4))
        {
            var pos = npc.Center;
            pos += Main.rand.NextVector2Circular(32, 32);
            var vel = Main.rand.NextVector2Circular(0.5f, 0.5f);
            var d = Dust.NewDustPerfect(pos, DustID.IceTorch, vel, Scale: 0.2f);
            d.noGravity = true;
        }
    }
}

public class MarkedforIceII : MarkedforIceI
{

}

public class FrostSlashersBoom : ModProjectile
{
    float Time => 60f;
    public override string Texture => AssetReferences.Assets.Textures.Projectile.SnowflakeBoom.KEY;
    ref float Timer => ref Projectile.ai[0];
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.WidthAndHeight = 100;
        Projectile.LocalHitOnce = true;
        Projectile.tileCollide = false;
        Projectile.light = 0.7f;
        Projectile.friendly = true;
    }

    public override void AI()
    {
        base.AI();
        Timer++;
        if (Timer == 1)
        {
            var parendineHitSound = AssetRegistry.Sounds.Melee.Parendine;
            parendineHitSound.Volume = 0.3f;
            parendineHitSound.PitchVariance = 0.2f;
            SoundEngine.PlaySound(parendineHitSound, Projectile.Center);

            FXUtil.ShakeCamera(Projectile.Center, 1024, 16);

            var fx = FXUtil.GlowCircleBoom(Projectile.Center,
                innerColor: Color.White,
                glowColor: Color.LightBlue,
                outerGlowColor: Color.Blue, duration: 25, baseSize: 0.16f);
            fx.Scale *= 2f;
            var fac = ParticleUtils.ParticleFactory.FromSmallBurst(Projectile.Center, Projectile.Center + new Vector2(0, 15), TriColorPalette.Icey, new Vector2(5f, 45f));
            fac.particleCount *= 4;
            ParticleUtils.CreateSwirlingDustCircle(fac);

            for (var i = 0; i < 32; i++)
            {
                var pos = Projectile.Center + Main.rand.NextVector2Circular(32, 32);
                var vel = (pos - Projectile.Center);
                vel = vel.Resize(12);
                vel *= Main.rand.NextFloat(0.75f, 1.2f);
                Particles.IceShard.Spawn(new()
                {
                    position = pos,
                    velocity = vel,
                    scale = Main.rand.NextFloat(0.5f, 1f),
                    rotation = Main.rand.NextFloat(7f),
                    timeLeft = Main.rand.Next(60, 100)
                });
            }
        }

        if (Main.rand.NextBool(64))
        {
            var pos = Projectile.Center;
            pos += Main.rand.NextVector2Circular(32, 32);
            var vel = Main.rand.NextVector2Circular(0.5f, 0.5f);
            var d = Dust.NewDustPerfect(pos, DustID.IceTorch, vel, Scale: 0.2f);
            d.noGravity = true;
        }

        if (Timer >= Time)
            Projectile.Kill();
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        base.OnHitNPC(target, hit, damageDone);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        var drawer = Projectile.Drawer;
        drawer.color = Color.Lerp(Color.SkyBlue, Color.White, ExtraMath.Osc(0f, 1f, speed: 16f));
        drawer.scale = Vector2.Lerp(new Vector2(0.1f), Vector2.One, EasingFunction.OutCirc(Timer / Time));
        drawer.color *= MathHelper.Lerp(1f, 0f, EasingFunction.InSine(Timer / Time));
        Main.spriteBatch.Draw(drawer);
        return false;
    }
    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
    }
}

public class FrostSlashersThrow : ModProjectile
{
    ref float Timer => ref Projectile.ai[0];
    bool IsDead
    {
        get => Projectile.ai[2] == 1;
        set => Projectile.ai[2] = value ? 1 : 0;
    }
    public override string Texture => AssetReferences.Assets.Textures.Item.FrostSlashers.KEY;
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        Projectile.SetTrailCacheLength(32);
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 8;
        Projectile.height = 8;
        Projectile.timeLeft = 120;
        Projectile.friendly = true;
        Projectile.extraUpdates = 1;
        Projectile.light = 0.4f;
        Projectile.timeLeft = 160;
    }

    public override void AI()
    {
        base.AI();
        Timer++;
        if (Timer % 4 == 0)
        {
            var pos = Projectile.Center;
            pos += Main.rand.NextVector2Circular(32, 32);
            var vel = Main.rand.NextVector2Circular(0.5f, 0.5f);
            var d = Dust.NewDustPerfect(pos, DustID.IceTorch, vel, Scale: 0.2f);
            d.noGravity = true;
        }
        if (IsDead)
            return;
        Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;
    }


    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        base.OnHitNPC(target, hit, damageDone);
        if (!target.HasBuff<MarkedforIceI>())
            target.AddBuff(ModContent.BuffType<MarkedforIceI>(), 30);
        else if (!target.HasBuff<MarkedforIceII>())
            target.AddBuff(ModContent.BuffType<MarkedforIceII>(), 30);
        else
        {
            var explosion = ProjFirer.From<FrostSlashersBoom>(Projectile);
            explosion.position = target.Center;
            explosion.damage *= 3;
            explosion.New();
        }

        target.AddBuff(BuffID.Frostburn, 15);
    }

    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        IsDead = true;
        Projectile.velocity = Vector2.Zero;
        return false;
    }

    public override bool PreDraw(ref Color lightColor)
    {
        var drawer = Projectile.Drawer;
        drawer.color *= EasingFunction.InOutSine(Projectile.timeLeft / 45f);
        DrawUtilities.DrawAdditiveFadingTrail(Projectile, Color.SkyBlue, Color.Transparent, 0.3f);
        Main.spriteBatch.Draw(drawer);
        return false;
    }

    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
        var fac = ParticleUtils.ParticleFactory.FromSmallBurst(Projectile.Center, Projectile.Center + Projectile.velocity, TriColorPalette.Icey, new Vector2(5f, 15f));
        fac.particleCount /= 2;
        ParticleUtils.CreateSwirlingDustBurst(fac);
    }
}

public class FrostSlashersStaminaThrow : ModProjectile
{
    Player Owner => Main.player[Projectile.owner];
    ref float Timer => ref Projectile.ai[0];
    ref float Count => ref Projectile.ai[1];
    public override string Texture => TextureRegistry.EmptyTexture;
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.tileCollide = false;
        Projectile.penetrate = -1;
        Projectile.ignoreWater = true;
    }

    public override bool ShouldUpdatePosition()
    {
        return false;
    }

    public override void AI()
    {
        Projectile.Center = Owner.Center;
        if (this.OwnedByLocalClient())
        {
            Projectile.velocity = (Main.MouseWorld - Projectile.Center);
            Projectile.netUpdate = true;
        }

        Timer++;
        if (Timer % 12 == 0)
        {
            if (this.OwnedByLocalClient())
            {
                var firer = ProjFirer.From<FrostSlashersThrow>(Projectile);
                firer.velocity = Projectile.velocity.SafeNormalize(Vector2.Zero) * 15;
                firer.New();
            }

            Count++;
            if (Count >= 3)
            {
                Projectile.Kill();
            }
        }
    }
}