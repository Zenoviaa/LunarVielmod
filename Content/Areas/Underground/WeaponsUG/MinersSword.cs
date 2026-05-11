using ReLogic.Content;
using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Content.CommonMaterials;
using Stellamod.Core.Bases;
using Stellamod.Core.Effects.Trails;
using Stellamod.Core.SwingSystem;
using Stellamod.Core.Utilities;
using Stellamod.Helpers;
using Stellamod.Items;
using Stellamod.Trails;
using Stellamod.Visual.Particles;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Underground.WeaponsUG;


public class MinersSword : BaseSwingItemV2
{
    public override void SetDefaults2()
    {
        base.SetDefaults2();
        Item.damage = 16;
        Item.DamageType = DamageClass.Melee;
        Item.shootSpeed = 10;
        Item.shoot = ModContent.ProjectileType<MinersSwordSlash>();
        Item.autoReuse = true;
        staminaCost = 3;
        staminaProjectileShoot = ModContent.ProjectileType<MinersSwordStaminaSlash>();
        meleeWeaponType = MeleeWeaponType.Sword;
    }

    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew(
            mold: ModContent.ItemType<BlankSword>(),
            material: ModContent.ItemType<MinersGold>());
    }
}
public class MinersSwordSlash : BaseSwingProjectileV2
{
    public bool Hit;
    private float _traveledRotation;
    private float _oldRot;
    public override void Init_Rendering()
    {
        base.Init_Rendering();
        SlashTrailer swingTrailer = new SlashTrailer
        {
            Shader = new SlashEffect()
            {
                BaseColor = Color.White,
                HighlightColor = Color.White,
                RimHighlightColor = Color.Brown,
                WindColor = Color.Brown,
                BlendState = Microsoft.Xna.Framework.Graphics.BlendState.Additive,
                WindTexture = TrailRegistry.CrystalTrail.Value
            },
            TrailWidthFunction = (float interpolant) =>
            {
                return EasingFunction.QuadraticBump(interpolant) * 16;
            },
            TrailColorFunction = (float interpolant) =>
            {
                Color lerp1 = Color.Lerp(Color.White, Color.Black, interpolant);
                return Color.Lerp(Color.Transparent, lerp1, interpolant);
            }
        };

        swingTrailer.invert = ComboIndex % 2 != 0;
        Trailer = swingTrailer;

        useBloom = true;
        bloom.innerBloomColor = Color.White * 0.7f;
        bloom.outerBloomColor = Color.Black;
        bloom.bloomWidthFunction = GetBloomWidth;
        bloom.bloomColorFunction = GetBloomColor;
    }

    public override void AI()
    {
        base.AI();
        bloomScale = MathHelper.Lerp(0.12f, 0f, EasingFunction.InExpo(Interpolant));
        glowColor = Color.Lerp(Color.Transparent, Color.White * 0.5f, EasingFunction.QuadraticBump(Interpolant));
        growScale = MathHelper.Lerp(0f, 0.3f, EasingFunction.QuadraticBump(Interpolant));

        _traveledRotation += MathF.Abs(Projectile.rotation - _oldRot);
        _oldRot = Projectile.rotation;
        if (_traveledRotation > 0.3f)
        {
            _traveledRotation = 0f;
            int index = (int)(Interpolant * swingTrailCache.Length) % swingTrailCache.Length;
            Vector2 spawnPos = swingTrailCache[index];
            FaintSmokeParticle sp = FaintSmokeParticle.SpawnInAlphaLayer(spawnPos, Vector2.Zero);
            sp.color = Color.Lerp(Color.Lerp(Color.White, Color.Brown, Main.rand.NextFloat(0f, 1f)), Color.Black, 0.7f);
            sp.fadeToColor = Color.DarkGray;
            sp.Scale *= 0.15f;
            sp.behindLayer = true;
        }
    }

    private float GetBloomWidth(float ratio)
    {
        return MathHelper.SmoothStep(0, 32, ratio) * 1.5f * MathHelper.SmoothStep(1f, 0f, EasingFunction.InExpo(Interpolant));
    }
    private Color GetBloomColor(float ratio)
    {
        return Color.Lerp(Color.White * 0.9f, Color.Black, EasingFunction.InExpo(ratio));
    }
    private float DefaultWidthFunction(float completionRatio)
    {
        return MathHelper.Lerp(0, 16, completionRatio) * EasingFunction.QuadraticBump(completionRatio);
    }

    private Color DefaultColorFunction(float p)
    {
        Color trailColor = Color.Lerp(Color.White, Color.Brown, p) * EasingFunction.QuadraticBump(p);
        return trailColor;
    }

    public override void DefineCombo()
    {
        base.DefineCombo();
        ComboBuilder comboBuilder = new ComboBuilder();
        comboBuilder.AddSwordSlash1(duration: 17)
            .AddSwordSlash2(duration: 17)
            .AddSpinningSwordSlash(duration: 30, xSwingRadius: 64, ySwingRadius: 64, swingDegrees: 360, hitCount: 2)
            .AddSpinningSwordSlash(duration: 30, xSwingRadius: 64, ySwingRadius: 64, swingDegrees: 360, hitCount: 2)
            .AddSwordSlash3(duration: 38, swingDegress: 720, hitCount: 3);
        comboBuilder.AddToProjectile(this);
        useAfterImage = true;
    }

    public override void RenderSwingTrail(ref Color lightColor, Vector2[] points)
    {
        base.RenderSwingTrail(ref lightColor, points);
        var shader = BlackFireShader.Instance;
        shader.InnerColor = Color.White * 0.15f;
        shader.OuterColor = Color.Brown * 0.15f;
        shader.BackColor = Color.Black * 0.15f;
        TrailDrawer.Draw(Main.spriteBatch, points, Projectile.oldRot, DefaultColorFunction, DefaultWidthFunction, shader);
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        base.OnHitNPC(target, hit, damageDone);
        if (!Hit && ComboIndex == 5)
        {
            FXUtil.ShakeCamera(target.Center, 1024, 4);
            FXUtil.GlowCircleBoom(target.Center,
                innerColor: Color.White,
                glowColor: Color.Black,
                outerGlowColor: Color.Black, duration: 12, baseSize: 0.24f);

            Hit = true;
        }

        for (float i = 0; i < 2; i++)
        {
            float progress = i / 4f;
            float rot = progress * MathHelper.ToRadians(360);
            rot += Main.rand.NextFloat(-0.5f, 0.5f);
            Vector2 offset = rot.ToRotationVector2() * 24;
            var particle = FXUtil.GlowCircleLongBoom(target.Center,
                innerColor: Color.White,
                glowColor: Color.LightGray,
                outerGlowColor: Color.Black,
                baseSize: Main.rand.NextFloat(0.05f, 0.1f),
                duration: Main.rand.NextFloat(5, 10));
            particle.Rotation = rot + MathHelper.ToRadians(45);
        }
    }

    public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
    {
        base.ModifyHitNPC(target, ref modifiers);
        SoundStyle spearHit = AssetRegistry.Sounds.Melee.SpearHit1;
        spearHit.PitchVariance = 0.5f;
        SoundEngine.PlaySound(spearHit, Projectile.position);
        if (ComboIndex == 5)
        {
            modifiers.FinalDamage *= 2;
        }
    }
}


public class ThrowRock : ModProjectile
{
    private ref float Timer => ref Projectile.ai[0];
    private ref float HitstopTimer => ref Projectile.ai[1];
    private ref float BounceTimer => ref Projectile.ai[2];
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        ProjectileID.Sets.TrailCacheLength[Type] = 16;
        ProjectileID.Sets.TrailingMode[Type] = 2;
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 32;
        Projectile.height = 32;
        Projectile.penetrate = -1;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.timeLeft = 240;
        Projectile.tileCollide = true;
        Projectile.friendly = true;
    }
    public override bool ShouldUpdatePosition()
    {
        return base.ShouldUpdatePosition() && HitstopTimer <= 0;
    }
    private void BounceEffect()
    {
        BounceTimer = 30;
        SoundStyle smashSound = Main.rand.NextBool(2) ? SoundRegistry.HammerHit1 : SoundRegistry.HammerHit2;
        smashSound.PitchVariance = 0.2f;
        smashSound.Volume = 0.2f;
        SoundEngine.PlaySound(smashSound, Projectile.position);
    }
    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        if(Projectile.velocity.X != oldVelocity.X)
        {
            BounceEffect();
            Projectile.velocity.X = -oldVelocity.X;
        }

        if(Projectile.velocity.Y != oldVelocity.Y)
        {
            BounceEffect();
    
            Projectile.velocity.Y = -oldVelocity.Y;
        }
        return false;
    }
    public override void AI()
    {
        base.AI();
        HitstopTimer--;
        if (HitstopTimer >= 1)
            return;

        Timer++;
        if (Timer == 1)
        {
            SoundStyle sounds = new SoundStyle("Stellamod/Assets/Sounds/AssassinsKnifeProg");
            sounds.PitchVariance = 0.3f;
            SoundEngine.PlaySound(sounds, Projectile.position);
        }
        if (Timer % 8 == 0)
        {
            Dust.NewDustPerfect(Projectile.Center, DustID.Stone);
        }

        if (Timer % 4 == 0)
        {
            Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Stone, Scale: 0.3f);
        }

        if (Timer % 16 == 0)
        {
            SparkleParticle sp = SparkleParticle.Spawn(Projectile.Center, Vector2.Zero, Scale: 0.5f);
            sp.gravity = 0;
            sp.fast = true;
            sp.easeInFade = true;
        }
        Projectile.velocity.Y += 0.3f;

        Projectile.rotation += 0.05f;
        Projectile.rotation += Projectile.velocity.Length() * 0.05f;
    }
    private Asset<Texture2D> _outlineTextureAsset;
    public override bool PreDraw(ref Color lightColor)
    {
        Vector2 scale = Vector2.Lerp(Vector2.One, new Vector2(1.2f, 0.8f), EasingFunction.InOutSine(BounceTimer / 30f));
        SpritebatchDrawer hammerDrawer = SpritebatchDrawer.FromProjectile(Projectile);
        Color c = hammerDrawer.color;
        for (int i = 0; i < Projectile.oldPos.Length; i++)
        {
            hammerDrawer.worldPosition = Projectile.oldPos[i];
            hammerDrawer.worldPosition += Projectile.Size * 0.5f;
            hammerDrawer.color = Color.Lerp(Color.White, Color.Transparent, (float)i / (float)Projectile.oldPos.Length) * 0.2f;
            hammerDrawer.rotation = Projectile.oldRot[i];
            Main.spriteBatch.Draw(hammerDrawer);
        }

        hammerDrawer.color = c;
        hammerDrawer.rotation = Projectile.rotation;
        hammerDrawer.worldPosition = Projectile.Center;
        hammerDrawer.scale *= scale;
        Main.spriteBatch.Draw(hammerDrawer);

        _outlineTextureAsset ??= ModContent.Request<Texture2D>(Texture + "_Outline");
        SpritebatchDrawer outlineDrawer = SpritebatchDrawer.FromTextureAsset(_outlineTextureAsset, Projectile.Center);
        outlineDrawer.color = lightColor;
        outlineDrawer.rotation = Projectile.rotation;
        outlineDrawer.scale *= scale;
        Main.spriteBatch.Draw(outlineDrawer);
        return false;
    }
    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        base.OnHitNPC(target, hit, damageDone);
        HitstopTimer = 10;
        Vector2 forwardVelocity = (target.Center - Projectile.Center);
      
        ShakeScreenPosition.Shake = 2;

        SoundStyle smashSound = Main.rand.NextBool(2) ? SoundRegistry.HammerHit1 : SoundRegistry.HammerHit2;
        smashSound.PitchVariance = 0.2f;
        SoundEngine.PlaySound(smashSound, Projectile.position);

        for (float f = 0; f < 7; f++)
        {
            Dust.NewDustPerfect(Projectile.Center, DustID.Stone, Main.rand.NextVector2Circular(16, 16), Scale: 1f);
        }

        for (float f = 0; f < 32; f++)
        {
            Vector2 vel = -Vector2.UnitY.RotatedBy(f / 32f * MathHelper.TwoPi) * 2;
            Dust.NewDustPerfect(Projectile.Center, DustID.Stone, vel, Scale: 1.5f);
        }
    }
    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
        for (float f = 0; f < 7; f++)
        {
            Dust.NewDustPerfect(Projectile.Center, DustID.Stone, Main.rand.NextVector2Circular(8, 8), Scale: 1.5f);
        }

    }
}

public class MinersSwordStaminaSlash : ModProjectile
{
    public override string Texture => TextureRegistry.EmptyTexture;
    private ref float Timer => ref Projectile.ai[0];
    private Player Owner => Main.player[Projectile.owner];
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 16;
        Projectile.height = 16;
        Projectile.penetrate = -1;
        Projectile.tileCollide = false;
        Projectile.timeLeft = 69;
    }

    public override void AI()
    {
        base.AI();
        Timer++;
        if(Timer == 1)
        {
            if (this.OwnedByLocalClient())
            {
                for(int i = 0; i < 4; i++)
                {
                    Vector2 throwVelocity = Projectile.velocity.RotatedByRandom(MathHelper.ToRadians(15)) + -Vector2.UnitY * 5;
                    throwVelocity *= Main.rand.NextFloat(0.5f, 1f);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Owner.Center,
                            throwVelocity,
                        ModContent.ProjectileType<ThrowRock>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                }
            }
        }
    }
}
