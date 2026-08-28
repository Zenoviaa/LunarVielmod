using Microsoft.Xna.Framework;
using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Content.CommonMaterials;
using Stellamod.Core.Bases;
using Stellamod.Core.Pixelation;
using Stellamod.Core.SwingSystem;
using Stellamod.Dusts;
using Stellamod.Items;
using Stellamod.Projectiles.Swords;
using Stellamod.Trailing;
using Stellamod.Visual.Particles;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.WondrousDarkspace.WeaponsWD;

public class Starvast : BaseSwingItemV2
{
    public override void SetDefaults2()
    {
        base.SetDefaults2();
        Item.damage = 10;
        Item.shoot = ModContent.ProjectileType<StarvastSlash>();
        staminaProjectileShoot = ModContent.ProjectileType<StarvastStaminaSlash>();
        meleeWeaponType = MeleeWeaponType.Sword;
    }

    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew<HypnotizedSoul, BlankSword>();
    }
}

public class StarvastSlash : BaseSwingProjectileV2
{
    private float _oldRot;
    private float _traveledRotation;
    private bool _hit;
    public override void DefineCombo()
    {
        base.DefineCombo();
        SwingV2Helper.AddSwordSwingStyle(this);
        var swingTrailer = TrailPresets.Starvast;
        swingTrailer.invert = ComboIndex % 2 != 0;
        Trailer = swingTrailer;

        useBloom = true;
        bloom.innerBloomColor = Color.White;
        bloom.outerBloomColor = Color.LightGoldenrodYellow;
        bloom.bloomWidthFunction = GetBloomWidth;
        bloom.bloomColorFunction = GetBloomColor;

        additive = true;
        useAfterImage = true;
        glowAfterImageColor = Color.LightBlue;
    }
    private float GetBloomWidth(float ratio)
    {
        return MathHelper.SmoothStep(4, 32, ratio) * 1.15f * MathHelper.Lerp(1f, 0f, EasingFunction.InExpo(Interpolant));
    }
    private Color GetBloomColor(float ratio)
    {
        Color blue = Color.Lerp(Color.Lerp(Color.LightGoldenrodYellow, Color.DarkGoldenrod, 0.5f), Color.DarkGoldenrod, ExtraMath.Osc(0f, 1f, speed: 4));
        return Color.Lerp(blue * 0.9f, Color.Violet, ratio);
    }
    public override void AI()
    {
        base.AI();
        bloomScale = MathHelper.Lerp(0.08f, 0f, EasingFunction.InExpo(Interpolant));

        if (Timer % 16 == 0)
        {
            int index = (int)(Interpolant * swingTrailCache.Length) % swingTrailCache.Length;
            Vector2 spawnPos = swingTrailCache[index];
            SparkleParticle dp = SparkleParticle.Spawn(spawnPos, Vector2.Zero, Scale: 0.3f);
            dp.color = Color.Lerp(Color.Lerp(Color.Black, Color.Red, 0.1f), Color.Black, Main.rand.NextFloat(0f, 1f));
            dp.innerColor = Color.White;
            dp.outerColor = Color.Blue;
            dp.gravity = 0;
            dp.noTileCollide = true;
            dp.fast = true;
        }

        _traveledRotation += MathF.Abs(Projectile.rotation - _oldRot);
        _oldRot = Projectile.rotation;
        if (_traveledRotation > 0.1f)
        {
            _traveledRotation = 0f;
            int index = (int)(Interpolant * swingTrailCache.Length) % swingTrailCache.Length;
            Vector2 spawnPos = swingTrailCache[index];

            index = (int)(Interpolant * swingTrailCache.Length) % swingTrailCache.Length;
            int nextIndex = index + 4;
            nextIndex %= swingTrailCache.Length;

            spawnPos = swingTrailCache[index];
            Vector2 spawnPos2 = swingTrailCache[nextIndex];
            Vector2 spawnVelocity = spawnPos2 - spawnPos;
            spawnVelocity = spawnVelocity.SafeNormalize(Vector2.Zero);
            spawnVelocity *= 24;


            if (Main.rand.NextBool(12))
            {


                DustParticle dp = DustParticle.Spawn(spawnPos, spawnVelocity);
                dp.color = Color.Lerp(Color.Lerp(Color.Black, Color.Red, 0.1f), Color.Black, Main.rand.NextFloat(0f, 1f));
                dp.innerColor = Color.LightGoldenrodYellow;
                dp.outerColor = Color.DarkGoldenrod;
                dp.gravity = 0;
                dp.noTileCollide = true;
                dp.fast = true;
                dp.superFast = true;
            }
        }
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        base.OnHitNPC(target, hit, damageDone);
        if (!_hit)
        {
            FXUtil.ShakeCamera(target.Center, 1024, 4);
            _hit = true;
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), target.Center, Projectile.velocity + new Vector2(0, -16),
                ModContent.ProjectileType<StarvastStarProj>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
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

public class StarvastStaminaSlash : BaseSwingProjectileV2
{
    public bool Hit;
    public bool AuroraProj2;
    public override void DefineCombo()
    {
        base.DefineCombo();
        SoundStyle swingSound1 = SoundRegistry.HeavySwordSlash1;
        swingSound1.PitchVariance = 0.5f;

        useBloom = true;
        bloom.innerBloomColor = Color.White;
        bloom.outerBloomColor = Color.LightGoldenrodYellow;
        bloom.bloomWidthFunction = GetBloomWidth;
        bloom.bloomColorFunction = GetBloomColor;


        additive = true;
        Trailer = TrailPresets.Starvast2;
        Add(new OvalSwing
        {
            Duration = 44,
            XSwingRadius = 160 / 1.5f,
            YSwingRadius = 80 / 1.5f,
            SwingDegrees = 270,
            Easing = (lerpValue) => Easing.InOutExpo(lerpValue, 10),
            Sound = swingSound1,

        });

        Add(new OvalSwing
        {
            Duration = 44,
            XSwingRadius = 160 / 1.5f,
            YSwingRadius = 80 / 1.5f,
            SwingDegrees = 270,
            Easing = (lerpValue) => Easing.InOutExpo(lerpValue, 10),
            Sound = swingSound1,
        });
    }
    private float GetBloomWidth(float ratio)
    {
        return MathHelper.SmoothStep(4, 32, ratio) * 1.15f * MathHelper.Lerp(1f, 0f, EasingFunction.InExpo(Interpolant));
    }
    private Color GetBloomColor(float ratio)
    {
        Color blue = Color.Lerp(Color.Lerp(Color.LightGoldenrodYellow, Color.DarkGoldenrod, 0.5f), Color.Blue, ExtraMath.Osc(0f, 1f, speed: 4));
        return Color.Lerp(blue * 0.9f, Color.Violet, ratio);
    }

    public override void AI()
    {
        base.AI();

        Vector2 swingDirection = Projectile.velocity.SafeNormalize(Vector2.Zero);
        if (Interpolant > 0.5f && !AuroraProj2)
        {
            SoundStyle soundStyle = new SoundStyle("Stellamod/Assets/Sounds/StarFlower3") with { PitchVariance = 0.4f };
            soundStyle.Volume = 0.15f;
            SoundEngine.PlaySound(soundStyle, Projectile.position);


            var fx = FXUtil.GlowCircleBoom(Projectile.Center, Color.White, Color.LightGoldenrodYellow, Color.DarkGoldenrod);
            fx.Scale *= 1.5f;
            float numDust = 4;
            for (float n = 0; n < numDust; n++)
            {
                Vector2 vel = -Projectile.velocity;
                vel = vel.RotatedByRandom(MathHelper.ToRadians(60));
                vel = vel.SafeNormalize(Vector2.Zero);
                vel *= Main.rand.NextFloat(6, 12);
                DustParticleSpawnParams spawnParams = DustParticleSpawnParams.Default;
                spawnParams.outerColor = Color.DarkGoldenrod;
                var dp = DustParticle.Spawn(Projectile.Center, vel, spawnParams);
                dp.fast = true;
                dp.noTileCollide = true;
                dp.dampening = 0.05f;
                dp.gravity = 0;
                dp.Scale *= 0.85f;
            }

            if (Main.myPlayer == Projectile.owner)
            {
                Vector2 shootVelocity = Projectile.velocity;

                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Owner.Center, Projectile.velocity + new Vector2(0, -16),
                    ModContent.ProjectileType<StarvastStarProj>(), Projectile.damage * 2, Projectile.knockBack, Projectile.owner);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Owner.Center, Projectile.velocity + new Vector2(0, 16),
                    ModContent.ProjectileType<StarvastStarProj>(), Projectile.damage * 2, Projectile.knockBack, Projectile.owner);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Owner.Center, Projectile.velocity + new Vector2(16, 0),
                    ModContent.ProjectileType<StarvastStarProj>(), Projectile.damage * 2, Projectile.knockBack, Projectile.owner);

            }
            AuroraProj2 = true;
        }
    }

    public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
    {
        base.ModifyHitNPC(target, ref modifiers);

        SoundStyle spearHit = SoundRegistry.CrystalHit1;
        spearHit.PitchVariance = 0.5f;
        spearHit.Volume = 0.2f;
        SoundEngine.PlaySound(spearHit, Projectile.position);

        SoundStyle spearHit2 = SoundRegistry.NSwordHit1;
        spearHit2.PitchVariance = 0.2f;
        spearHit2.Volume = 0.2f;
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

        if (ComboIndex < 1 && this.OwnedByLocalClient())
        {
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position, Projectile.velocity, Projectile.type, Projectile.damage, Projectile.knockBack,
                        Owner.whoAmI, ai2: combo, ai1: dir);
        }
    }



}


public class StarvastStarProj : ModProjectile,
    IDrawToRenderTarget
{
    private ref float Timer => ref Projectile.ai[0];
    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5;
        ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
    }

    public override void SetDefaults()
    {
        Projectile.width = 22;
        Projectile.height = 22;
        Projectile.friendly = true;
        Projectile.hostile = false;
        Projectile.aiStyle = -1;
        Projectile.penetrate = 2;
        Projectile.timeLeft = 200;
        Projectile.tileCollide = false;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 30;
    }

    private void AI_Movement(Vector2 targetCenter, float moveSpeed, float accel = 1f)
    {
        //This code should give quite interesting movement
        //Accelerate to being on top of the player
        float distX = targetCenter.X - Projectile.Center.X;
        if (Projectile.Center.X < targetCenter.X && Projectile.velocity.X < moveSpeed)
        {
            Projectile.velocity.X += accel;
        }
        else if (Projectile.Center.X > targetCenter.X && Projectile.velocity.X > -moveSpeed)
        {
            Projectile.velocity.X -= accel;
        }

        //Accelerate to being above the player.
        float distY = targetCenter.Y - Projectile.Center.Y;
        if (Projectile.Center.Y < targetCenter.Y && Projectile.velocity.Y < moveSpeed)
        {
            Projectile.velocity.Y += accel;
        }
        else if (Projectile.Center.Y > targetCenter.Y && Projectile.velocity.Y > -moveSpeed)
        {
            Projectile.velocity.Y -= accel;
        }
    }

    public override void AI()
    {
        Timer++;
        if(Timer == 1)
        {
        }

        Player owner = Main.player[Projectile.owner];
        NPC npc = NPCHelper.FindClosestNPC(Projectile.position, 700);
        if (npc != null)
        {
            AI_Movement(npc.Center, 4, 0.3f);
        }
        else
        {
            Timer += 0.02f;
            Vector2 orbitCenter = MovementUtilities.OrbitAround(owner.Center, Vector2.UnitY, 64, Timer);
            Vector2 targetVel = (orbitCenter - Projectile.Center);
            Projectile.velocity = Vector2.Lerp(Projectile.velocity, targetVel, 0.02f);
        }

        if (Main.rand.NextBool(10))
        {
            var sp = SparkleParticle.Spawn(Projectile.Center, Vector2.Zero);
            sp.Scale *= 0.35f;
            sp.outerColor = Color.Gold;
            sp.gravity = 0;
        }

        // So it will lean slightly towards the direction it's moving
        float rotation = MathHelper.ToRadians(Timer * 3);
        Projectile.rotation = rotation;

        // Some visuals here
        Lighting.AddLight(Projectile.Center, Color.White.ToVector3() * 0.78f);
    }

    public override void OnKill(int timeLeft)
    {
        FXUtil.GlowCircleBoom(Projectile.Center,
            innerColor: Color.White,
            glowColor: Color.Yellow,
            outerGlowColor: Color.DarkGoldenrod, duration: 25, baseSize: 0.03f);
    }



    //Visual Stuffs
    public override bool PreDraw(ref Color lightColor)
    {
        SpritebatchDrawer sbDrawer = SpritebatchDrawer.FromProjectile(Projectile);
        Main.spriteBatch.Draw(sbDrawer);

        SpritebatchDrawer glowDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.SimpleGlowCircle, Projectile.Center);
        glowDrawer.color = Color.Goldenrod * 0.3f;
        glowDrawer.color.A = 0;
        glowDrawer.scale *= 0.15f;
        Main.spriteBatch.Draw(glowDrawer);
        return false;
    }

    private void DrawTrail(GraphicsDevice gDevice)
    {
        var shader2 = RichLaserShader.Instance;
        shader2.LaserColor = Color.White;
        shader2.LaserTexture = TrailRegistry.WaterTrail;
        shader2.InnerColor = Color.LightGoldenrodYellow * 0.5f;
        shader2.OuterColor = Color.DarkGoldenrod;
        TrailDrawer.Draw(Main.spriteBatch, Projectile.oldPos, ColorFunction, WidthFunction, shader2, Projectile.Size * 0.5f);

        var bloom = BloomTrailShader.Instance;
        bloom.InnerColor = Color.LightGoldenrodYellow * 0.5f;
        bloom.OuterColor = Color.DarkGoldenrod;
        TrailDrawer.Draw(Main.spriteBatch, Projectile.oldPos, ColorFunction, WidthFunction2, bloom, Projectile.Size * 0.5f);
    }

    private Color ColorFunction(float completionRatio)
    {
        Color inColor = Color.White;
        Color trailColor = Color.Lerp(Color.LightGoldenrodYellow, Color.DarkGoldenrod, completionRatio);
        Color easeColor = Color.Lerp(inColor, trailColor, EasingFunction.InExpo(Timer / 60f));
        return easeColor;
    }

    private float WidthFunction(float completionRatio)
    {
        return MathHelper.SmoothStep(8, 2, completionRatio);
    }

    private float WidthFunction2(float completionRatio)
    {
        return WidthFunction(completionRatio) * 2f;
    }

    public void DrawToRenderTargets()
    {
        PixelationManager.QueuePrimitivesDrawAction(DrawTrail);
    }
}
