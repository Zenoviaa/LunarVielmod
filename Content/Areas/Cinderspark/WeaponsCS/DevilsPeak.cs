using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Content.Cards;
using Stellamod.Content.CommonMaterials;
using Stellamod.Core.Bases;
using Stellamod.Core.Effects.Trails;
using Stellamod.Core.Particles;
using Stellamod.Core.SwingSystem;
using Stellamod.Core.Utilities;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Items;
using Stellamod.Visual.Particles;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;


namespace Stellamod.Content.Areas.Cinderspark.WeaponsCS;

public class DevilsPeak : BaseSwingItemV2
{
    public override void SetDefaults2()
    {
        base.SetDefaults2();
        Item.damage = 21;
        Item.rare = ItemRarityID.Green;
        Item.shoot = ModContent.ProjectileType<DevilsPeakSlash>();
        staminaProjectileShoot = ModContent.ProjectileType<DevilsPeakStaminaSlash>();
        meleeWeaponType = MeleeWeaponType.Scythe;
        staminaCost = 2;
    }

    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew<Cinderscrap, BlankSword>();
    }
}

/// <summary>
/// Contains default rendering functionality for fire-y trails
/// </summary>
public class FireTrailRenderer
{
    public FireTrailRenderer()
    {
        Shader = new BlackFireShader();
        Shader.SetDefaults();
        Shader.InnerEmitColor = Color.Yellow * 0.2f;
        Shader.OuterEmiteColor = Color.Red;

        SlashTrailer = new SlashTrailer();
        SlashTrailer.Shader = Shader;
        SlashTrailer.TrailWidthFunction = GetTrailWidth;
        SlashTrailer.TrailColorFunction = GetTrailColor;
    }

    public readonly BlackFireShader Shader;
    public readonly SlashTrailer SlashTrailer;
    public float GetTrailWidth(float interpolant)
    {
        return MathHelper.SmoothStep(8, 64, interpolant) * MathF.Sin(interpolant * 8);
    }

    public Color GetTrailColor(float interpolant)
    {
        return Color.Lerp(Color.White, Color.Transparent, EasingFunction.InExpo(interpolant));
    }
    /*
    public float GetBloomWidth(float ratio)
    {
        return MathHelper.SmoothStep(8, 64, ratio) * 1.5f * MathHelper.SmoothStep(1f, 0f, EasingFunction.InExpo(Interpolant));
    }
    public Color GetBloomColor(float ratio)
    {
        return Color.Lerp(Color.Red * 0.9f, Color.Transparent, EasingFunction.InExpo(ratio));
    }
    */
}

public class DevilsPeakSlash : BaseSwingProjectileV2
{
    private float _oldRot;
    private float _traveledRotation;
    private bool _hitEnemy;
    private bool _playedSound;
    private bool _flareCircle;
    private FireTrailRenderer _fireTrailRenderer;
    public override void DefineCombo()
    {
        base.DefineCombo();
        SwingV2Helper.AddScytheSwingStyle(this);
        outlineColor = Color.Yellow;

        //Bloom
        useBloom = true;
        bloom.innerBloomColor = Color.OrangeRed;
        bloom.outerBloomColor = Color.DarkRed;
        bloom.bloomWidthFunction = GetBloomWidth;
        bloom.bloomColorFunction = GetBloomColor;

        _fireTrailRenderer = new FireTrailRenderer();
        additive = true;
        Trailer = _fireTrailRenderer.SlashTrailer;
        useAfterImage = true;
        glowAfterImageColor = Color.Red;
    }

    private float GetTrailWidthFunction(float ratio)
    {
        return MathHelper.SmoothStep(8, 64, ratio) * 1.5f * MathHelper.SmoothStep(1f, 0f, EasingFunction.InExpo(Interpolant)) * MathF.Sin(ratio * 8);
    }
    private Color GetTrailColorFunction(float ratio)
    {
        return Color.Lerp(Color.Red * 0.9f, Color.Transparent, EasingFunction.InExpo(ratio));
    }


    private float GetBloomWidth(float ratio)
    {
        return MathHelper.SmoothStep(8, 64, ratio) * 1.5f * MathHelper.SmoothStep(1f, 0f, EasingFunction.InExpo(Interpolant));
    }
    private Color GetBloomColor(float ratio)
    {
        return Color.Lerp(Color.Red * 0.9f, Color.Transparent, EasingFunction.InExpo(ratio));
    }


    public override void AI()
    {
        base.AI();
        bloomScale = MathHelper.Lerp(0.12f, 0f, EasingFunction.InExpo(Interpolant));
        glowColor = Color.Lerp(Color.Transparent, Color.Red * 0.5f, EasingFunction.QuadraticBump(Interpolant));
        growScale = MathHelper.Lerp(0f, 0.3f, EasingFunction.QuadraticBump(Interpolant));
        if (ComboIndex == ComboCount - 1 && Interpolant >= 0.3f && !_flareCircle)
        {
            _flareCircle = true;
            for (float f = 0; f < 16; f++)
            {
                float lerp = f / 16f;
                Vector2 offset = Vector2.UnitY.RotatedBy(lerp * MathHelper.TwoPi) * 196;
                Vector2 pos = Owner.Center + offset;
                Vector2 velocity = (Owner.Center - pos).SafeNormalize(Vector2.Zero) * 16;
                var part = LegacyParticle.NewParticle<FlareParticle>(Owner.Center + offset, velocity);
                part.Scale *= 0.5f;
            }
            SoundStyle fireSound = AssetRegistry.Sounds.MagicWand.FireCharge;
            fireSound.PitchVariance = 0.2f;
            SoundEngine.PlaySound(fireSound, Projectile.position);
        }
        if (Timer % 16 == 0 && Interpolant >= 0.3f)
        {
            if (!_playedSound)
            {
                SoundStyle fireSound = AssetRegistry.Sounds.Magic.RadianceCast1;
                fireSound.PitchVariance = 0.2f;
                SoundEngine.PlaySound(fireSound, Projectile.position);
                _playedSound = true;
            }
            /*
            var d = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<TSmokeDust>(), newColor: Color.DarkRed);
            d.velocity = -Vector2.UnitY * 3;
            d.scale *= 0.5f;*/
        }
        _traveledRotation += MathF.Abs(Projectile.rotation - _oldRot);
        _oldRot = Projectile.rotation;

        if (_traveledRotation > 0.1f)
        {
            _traveledRotation = 0f;
            int index = (int)(Interpolant * swingTrailCache.Length) % swingTrailCache.Length;
            Vector2 spawnPos = swingTrailCache[index];
            FaintSmokeParticle sp = FaintSmokeParticle.SpawnInAlphaLayer(spawnPos, Vector2.Zero);
            sp.color = Color.Lerp(Color.Lerp(Color.Orange, Color.Red, Main.rand.NextFloat(0f, 1f)), Color.Black, 0.7f);
            sp.fadeToColor = Color.DarkGray;
          //  sp.gravity = 0;
          //  sp.noTileCollide = true;
            sp.Scale *= 0.25f;
        //    sp.offsetRot = Main.rand.NextFloat(0f, MathHelper.TwoPi);
            sp.behindLayer = true;
        }
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        base.OnHitNPC(target, hit, damageDone);
        if (!_hitEnemy)
        {
            SoundStyle fireImpact = new SoundStyle("Stellamod/Assets/Sounds/Fire/FireballImpact1");
            fireImpact.PitchVariance = 0.4f;
            SoundEngine.PlaySound(fireImpact, target.Center);
            for (float f = 0; f < 16; f++)
            {
                Vector2 fireVelocity = -Vector2.UnitY;
                fireVelocity *= Main.rand.NextFloat(2, 7);
                Vector2 firePos = target.Center + Main.rand.NextVector2Circular(48, 48);
                Dust.NewDustPerfect(firePos, DustID.Torch, fireVelocity, Scale: Main.rand.NextFloat(1f, 1.75f));
            }
            for (float f = 0; f < 3; f++)
            {
                Vector2 fireVelocity = -Vector2.UnitY;
                fireVelocity *= Main.rand.NextFloat(1, 3);
                Vector2 firePos = target.Center + Main.rand.NextVector2Circular(48, 48);
                ThickSmokeParticle.Spawn(firePos, fireVelocity);
            }
            Vector2 vel = Main.rand.NextVector2Circular(1, 1);
            var fx = FXUtil.GlowStretch(target.Center, vel);
            fx.VectorScale.X *= 2;
            fx.VectorScale.Y *= 5;
            fx.InnerColor = Color.Yellow;
            fx.OuterGlowColor = Color.Red;

            var fx2 = FXUtil.GlowStretch(target.Center, vel);
            fx2.VectorScale.X *= 1.5f;
            fx2.VectorScale.Y *= 4.5f;
            fx2.InnerColor = Color.LightGoldenrodYellow;
            fx2.OuterGlowColor = Color.DarkRed;
            FXUtil.ShakeCamera(target.Center, 1024, 4);

            PixelPrimitiveCircleFactory.CreateFlamingCircle(target.Center);
            _hitEnemy = true;
        }
        Vector2 position = target.Center;
        Vector2 lvelocity = Projectile.velocity.SafeNormalize(Vector2.Zero) * 4;
        for (float f = 0; f < 4; f++)
        {
            Vector2 pVelocity = lvelocity.RotatedByRandom(MathHelper.PiOver4 / 3f);
            pVelocity *= Main.rand.NextFloat(0.5f, 2f);
            var frag = LegacyParticle.NewParticle<GlowFragmentParticle>(position, pVelocity);
            FXUtil.GlowFragmentParticle(position, pVelocity,
                innerColor: Color.Yellow,
                outerColor: Color.Orange,
                fadeToColor: Color.Red,
                distortOut: true);

            if (Main.rand.NextBool(4))
            {
                Dust.NewDustPerfect(position, ModContent.DustType<TSmokeDust>(),
                                 lvelocity.RotatedByRandom(MathHelper.PiOver4 / 2f) * 2);
            }
            if (Main.rand.NextBool(4))
            {
                Dust.NewDustPerfect(position, ModContent.DustType<GlowDust>(),
                                 lvelocity.RotatedByRandom(MathHelper.PiOver4 / 2f) * 3 * Main.rand.NextFloat(0.4f, 1f), newColor: Color.White, Scale: 0.2f);
            }
        }
    }

    public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
    {
        base.ModifyHitNPC(target, ref modifiers);
        if (ComboIndex == ComboCount - 1)
        {
            modifiers.FinalDamage *= 2;
        }
        SoundStyle spearHit = SoundRegistry.SpearHit1;
        spearHit.PitchVariance = 0.5f;
        SoundEngine.PlaySound(spearHit, Projectile.position);

        SoundStyle scytheHit;

        int rand = Main.rand.Next(0, 3);
        switch (rand)
        {
            default:
            case 0:
                scytheHit = AssetRegistry.Sounds.Melee.ScytheHit1;
                break;
            case 1:
                scytheHit = AssetRegistry.Sounds.Melee.ScytheHit2;
                break;
        }
        target.AddBuff(BuffID.OnFire, 120);
        scytheHit.PitchVariance = 0.5f;
        SoundEngine.PlaySound(scytheHit, Projectile.position);
    }
}

public class DevilsPeakStaminaSlash : BaseSwingProjectileV2
{
    private bool _hit;
    public override void DefineCombo()
    {
        base.DefineCombo();
        SoundStyle chargeSound = AssetRegistry.Sounds.Melee.ScythePull;
        chargeSound.PitchVariance = 0.1f;

        BlackFireShader blackFireShader = new BlackFireShader();
        blackFireShader.SetDefaults();
        blackFireShader.InnerEmitColor = Color.Yellow * 0.2f;
        blackFireShader.OuterEmiteColor = Color.Red;
        SlashTrailer devilsPeak = new SlashTrailer
        {
            Shader = blackFireShader,
            TrailWidthFunction = (interpolant) =>
            {
                return MathHelper.SmoothStep(8, 64, interpolant) * MathF.Sin(interpolant * 8);
            },
            TrailColorFunction = (interpolant) =>
            {
                return Color.Lerp(Color.White, Color.Transparent, EasingFunction.InExpo(interpolant));
            }

        };

        outlineColor = Color.Yellow;

        //Bloom
        useBloom = true;
        bloom.innerBloomColor = Color.OrangeRed;
        bloom.outerBloomColor = Color.DarkRed;
        bloom.bloomWidthFunction = GetBloomWidth;
        bloom.bloomColorFunction = GetBloomColor;

        additive = true;
        Trailer = devilsPeak;

        Add(new OvalSwing
        {
            Duration = 32,
            XSwingRadius = 160 / 1.5f,
            YSwingRadius = 80 / 1.5f,
            SwingDegrees = 270,
            Easing = (lerpValue) => Easing.InOutBack(lerpValue),
            Sound = chargeSound,

        });
        useAfterImage = true;

    }
    private float GetBloomWidth(float ratio)
    {
        return MathHelper.SmoothStep(8, 64, ratio) * 1.5f * MathHelper.SmoothStep(1f, 0f, EasingFunction.InExpo(Interpolant));
    }
    private Color GetBloomColor(float ratio)
    {
        return Color.Lerp(Color.Red * 0.9f, Color.Transparent, EasingFunction.InExpo(ratio));
    }
    public override void AI()
    {
        base.AI();
        glowColor = Color.Lerp(Color.Transparent, Color.Red, EasingFunction.QuadraticBump(Interpolant));
        growScale = MathHelper.Lerp(0f, 0.3f, EasingFunction.QuadraticBump(Interpolant));
        if (Timer % 8 == 0)
        {
            Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<TSmokeDust>(), newColor: Color.Black);
        }
    }
    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        base.OnHitNPC(target, hit, damageDone);
        if (!_hit)
        {
            Player player = Owner;
            Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(player.Center, 1024f, 32f);
            float recoilStrength = 8;
            Vector2 direction = target.DirectionTo(player.Center);
            Vector2 targetVelocity = direction * recoilStrength;
            player.velocity = VectorHelper.VelocityUpTo(player.velocity, targetVelocity);

            Projectile.NewProjectile(Projectile.GetSource_FromThis(), target.Center, Vector2.Zero, ModContent.ProjectileType<DevilsPeakBoom>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
            FXUtil.PunchCamera(Owner.Center, Projectile.velocity, 1, 2, 16);
            _hit = true;
        }
        target.AddBuff(BuffID.OnFire, 120);

    }
}

public class DevilsPeakBoom : ModProjectile
{
    private float _scale = 1f;
    public override string Texture => TextureRegistry.EmptyTexture;
    private ref float Timer => ref Projectile.ai[0];
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 164;
        Projectile.height = 164;
        Projectile.tileCollide = false;
        Projectile.friendly = true;
        Projectile.penetrate = -1;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = -1;
        Projectile.timeLeft = 30;
    }

    public override void AI()
    {
        base.AI();
        Timer++;
        if (Timer == 1)
        {
            FXUtil.ShakeCamera(Projectile.Center, 1024, 32);
            FXUtil.PunchCamera(Projectile.Center, Vector2.UnitY * 2, 8, 8, 32);
            if (this.OwnedByLocalClient())
            {
                float damage = Projectile.damage;
                damage *= 3f;
                var p = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero,
                    ModContent.ProjectileType<AgreviBoom>(), (int)damage, 3);

            }

            int count = 32;
            float degreesPer = 360 / (float)count;
            for (int k = 0; k < count; k++)
            {
                float degrees = k * degreesPer;
                Vector2 d = Vector2.One.RotatedBy(MathHelper.ToRadians(degrees));
                Vector2 vel = d * 8;
                Dust.NewDust(Projectile.Center, 0, 0, DustID.Lava, vel.X * 0.5f, vel.Y * 0.5f);
            }
            for (float f = 0; f < 16; f++)
            {
                Vector2 spawnPos = Projectile.Center + Main.rand.NextVector2Circular(90, 90);
                Vector2 velocity = Main.rand.NextVector2Circular(16, 16);
                Dust.NewDustPerfect(spawnPos, ModContent.DustType<TSmokeDust>(), velocity, newColor: Color.DarkRed);
            }

            int sound = Main.rand.Next(0, 2);
            switch (sound)
            {
                case 0:
                    SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/M38F30Bomb1"), Projectile.position);
                    break;
                case 1:
                    SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/M38F30Bomb2"), Projectile.position);
                    break;
            }
            for (float f = 0; f < 10; f++)
            {
                Vector2 velocity = Main.rand.NextVector2Circular(64, 64);
                FXUtil.GlowStretch(Projectile.Center, velocity);
            }
        }
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D texture = TrailRegistry.BeamTrail.Value;
        Vector2 drawPos = Projectile.Center - Main.screenPosition;
        var shader = RadialBlastShader.Instance;

        float prog = Timer / 30f;
        float interp = EasingFunction.OutExpo(prog);
        shader.Offset = Vector2.Lerp(Vector2.One * 0.25f, -Vector2.One * 0.25f, interp);
        shader.Tiling = Vector2.Lerp(Vector2.One * 4, Vector2.One * 32, interp);
        shader.InnerColor = Color.Lerp(Color.Yellow, Color.Black, interp);
        shader.OuterColor = Color.Lerp(Color.Red, Color.Black, EasingFunction.OutSine(prog));
        SpriteBatch spriteBatch = Main.spriteBatch;
        spriteBatch.Restart(blendState: BlendState.Additive, effect: shader.Effect);
        spriteBatch.Draw(texture, drawPos, null, Color.White, Projectile.rotation, texture.Size() / 2f, _scale * 0.4f, SpriteEffects.None, 0);
        spriteBatch.Draw(texture, drawPos, null, Color.White, Projectile.rotation, texture.Size() / 2f, _scale * 0.8f, SpriteEffects.None, 0);
        spriteBatch.Draw(texture, drawPos, null, Color.White, Projectile.rotation, texture.Size() / 2f, _scale, SpriteEffects.None, 0);
        spriteBatch.RestartDefaults();
        return false;
    }
    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
    }
}