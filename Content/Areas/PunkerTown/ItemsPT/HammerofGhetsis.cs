using ReLogic.Content;
using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Content.CommonMaterials;
using Stellamod.Core.Bases;
using Stellamod.Core.Effects.Trails;
using Stellamod.Core.Particles;
using Stellamod.Core.Pixelation;
using Stellamod.Core.SwingSystem;
using Stellamod.Helpers;
using Stellamod.Items;
using Stellamod.Trailing;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;


namespace Stellamod.Content.Areas.PunkerTown.ItemsPT;

/// <summary>
/// Hammer of Ghetsis 
/// - Stamina - 1, Throws the flaming hammer and it homes and bounces off enemies multiple times and tries to hit again for a maximum stack of 10.
/// </summary>
public class HammerofGhetsis : BaseSwingItemV2
{
    public override void SetDefaults2()
    {
        base.SetDefaults2();
        Item.damage = 90;
        Item.shoot = ModContent.ProjectileType<HammerofGhetsisSlash>();
        staminaProjectileShoot = ModContent.ProjectileType<HammerofGhetsisThrow>();
        meleeWeaponType = MeleeWeaponType.Hammer;
        staminaCost = 2;
    }
    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew(
            mold: ModContent.ItemType<BlankSword>(),
            material: ModContent.ItemType<MarshScrap>());
    }
}


public class HammerofGhetsisSlash : BaseSwingProjectileV2
{
    private float _hitCount;
    private bool _hit;
    private bool _playSound;
    public override void DefineCombo()
    {
        base.DefineCombo();
        SlashTrailBuilder slashTrailBuilder = new SlashTrailBuilder();
        slashTrailBuilder.baseColor = Color.DarkGray;
        slashTrailBuilder.windColor = Color.LightGray;
        slashTrailBuilder.lightColor = Color.White;
        slashTrailBuilder.colorFunction = GetTrailColor;
        slashTrailBuilder.widthFunction = GetTrailWidth;
        SlashTrailer slashTrailer = slashTrailBuilder.Instantiate();
        slashTrailer.invert = ComboIndex % 2 != 0;
        Trailer = slashTrailer;


        //Bloom
        useBloom = true;
        bloom.innerBloomColor = Color.White;
        bloom.outerBloomColor = Color.DarkGray;
        bloom.bloomWidthFunction = GetBloomWidth;
        bloom.bloomColorFunction = GetBloomColor;

        Trailer = slashTrailer;
        SoundStyle hammerSlash1 = SoundRegistry.HeavySwordSlash1;
        hammerSlash1.PitchVariance = 0.2f;

        SoundStyle hammerSlash2 = SoundRegistry.HeavySwordSlash2;
        hammerSlash2.PitchVariance = 0.2f;

        Add(new OvalSwing
        {
            Duration = 100,
            SwingDegrees = 310,
            XSwingRadius = 64,
            YSwingRadius = 64,
            Easing = (float lerpValue) => EasingFunction.GreatswordAnticipation(lerpValue),
            Sound = hammerSlash1,
            HitCount = 2
        });

        Add(new OvalSwing
        {
            Duration = 100,
            SwingDegrees = 310,
            XSwingRadius = 64,
            YSwingRadius = 64,
            Easing = (float lerpValue) => EasingFunction.GreatswordAnticipation(lerpValue),
            Sound = hammerSlash2,
            HitCount = 2
        });


        Add(new OvalSwing
        {
            Duration = 100,
            SwingDegrees = 330,
            XSwingRadius = 64,
            YSwingRadius = 64,
            Easing = (float lerpValue) => EasingFunction.GreatswordAnticipation(lerpValue),
            Sound = hammerSlash2,
            HitCount = 2
        });

        glowAfterImageColor = Color.White * 0.3f;
        swordBeamLength = 180;
        useAfterImage = true;
        hitStopTime = 4 * EXTRA_UPDATE_COUNT;
    }
    private Color GetTrailColor(float completionRatio)
    {
        return Color.Lerp(Color.White, Color.DarkGray, EasingFunction.InCirc(completionRatio)) * MathHelper.Lerp(0f, 1f, EasingFunction.InCirc(completionRatio));
    }
    private float GetBloomWidth(float ratio)
    {
        return MathHelper.SmoothStep(8, 64, ratio) * 1.5f * MathHelper.SmoothStep(1f, 0f, EasingFunction.InExpo(Interpolant));
    }
    private Color GetBloomColor(float ratio)
    {
        return Color.Lerp(Color.DarkGray * 0.9f, Color.Transparent, EasingFunction.InExpo(ratio)) * 0.3f;
    }

    public override Asset<Texture2D> RequestHologramTexture()
    {
        return TextureRegistry.GlowSword_Chillrend;
    }

    public override void PostDrawSword(Vector2 position, Rectangle srcRect, Color drawColor, float rotation, Vector2 origin, Vector2 drawScale, SpriteEffects spriteEffect, float layerDepth)
    {
        base.PostDrawSword(position, srcRect, drawColor, rotation, origin, drawScale, spriteEffect, layerDepth);
    }
    private float GetTrailWidth(float interpolant)
    {
        return EasingFunction.QuadraticBump(interpolant) * 8;
    }

    public override void AI()
    {
        base.AI();
        if (!_playSound && Interpolant >= 0.5f)
        {
            SoundStyle leafSound = Main.rand.NextBool(2) ? AssetRegistry.Sounds.Nature.LeafRustle1 : AssetRegistry.Sounds.Nature.LeafRustle2;
            leafSound.PitchVariance = 0.2f;
            SoundEngine.PlaySound(leafSound, Projectile.position);
            _playSound = true;
        }
        outlineColor = Color.DarkGray * 0.24f;
        growScale = MathHelper.Lerp(0f, 0.3f, EasingFunction.QuadraticBump(Interpolant));
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        float pitch = MathHelper.Clamp(_hitCount * 0.05f, 0f, 1f);
        SoundStyle smashSound = Main.rand.NextBool(2) ? SoundRegistry.HammerHit1 : SoundRegistry.HammerHit2;
        smashSound.PitchVariance = 0.2f;
        SoundEngine.PlaySound(smashSound, Projectile.position);

        base.OnHitNPC(target, hit, damageDone);
        if (!_hit)
        {
            Bounce(8);
            FXUtil.ShakeCamera(target.Center, 1024, 16);
            FXUtil.PunchCamera(target.Center, Projectile.velocity, 0.5f, 2, 30);
            _hit = true;
        }
    }

    public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
    {
        base.ModifyHitNPC(target, ref modifiers);
        if (!_hit)
        {
            modifiers.Knockback *= 0.5f;
        }
        else
        {
            modifiers.Knockback *= 2;
        }

        if (ComboIndex == ComboCount - 1)
        {
            modifiers.FinalDamage += 0.5f;
        }
    }
}


public class HammerofGhetsisThrow : ModProjectile,
    IDrawToRenderTarget
{
    private ref float Timer => ref Projectile.ai[0];
    private ref float HitCount => ref Projectile.ai[1];
    private ref float HitstopTimer => ref Projectile.ai[2];
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        Main.projFrames[Type] = 2;
        ProjectileID.Sets.TrailCacheLength[Type] = 16;
        ProjectileID.Sets.TrailingMode[Type] = 2;
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 64;
        Projectile.height = 64;
        Projectile.friendly = true;
        Projectile.penetrate = -1;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = -1;
        Projectile.timeLeft = 300;
    }

    public override bool ShouldUpdatePosition()
    {
        if (HitstopTimer > 0)
            return false;
        return base.ShouldUpdatePosition();
    }

    public override bool? CanHitNPC(NPC target)
    {
        if (HitstopTimer > 0)
            return false;
        return base.CanHitNPC(target);
    }

    public override void AI()
    {
        base.AI();
        if (HitstopTimer > 0)
            HitstopTimer--;
        Timer++;
        if (Main.rand.NextBool(4))
        {
            Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Dirt);
        }

        if (Main.rand.NextBool(16))
        {
            FlameParticle dp = Particle<FlameParticle>.Spawn(Projectile.Center, Main.rand.NextVector2Circular(8, 8), Scale: Main.rand.NextFloat(0.2f, 0.35f));
            dp.innerColor = Color.Goldenrod;
            dp.outerColor = Color.Red;
            dp.parent = Projectile;
            dp.gravity = 0f;
            dp.dampening = 0.05f;
            dp.fast = true;
        }

        if (Main.rand.NextBool(8))
        {
            if (Main.rand.NextBool(3))
            {
                SmokeParticle sp = Particle<SmokeParticle>.Spawn(Projectile.Center + Main.rand.NextVector2Circular(32, 32), -Projectile.velocity.SafeNormalize(Vector2.Zero), Color.OrangeRed, Main.rand.NextFloat(0.9f, 1.5f));
                sp.initialColor = Color.Lerp(Color.OrangeRed, Color.RosyBrown, Main.rand.NextFloat(0f, 1f)) * 0.4f;
                sp.expand = true;
            }
            if (Main.rand.NextBool(3))
            {
                LegacyParticle.NewParticle<EmberParticle>(Projectile.Center + Main.rand.NextVector2Circular(32, 32), -Vector2.UnitY.RotatedByRandom(1.5f), Color.OrangeRed, Main.rand.NextFloat(0.9f, 1.5f));
            }
        }

        //This weapon should home to the nearest enemy and bounce off them up to 10 times with a lot of impact and whatnot
        NPC nearest = NPCHelper.FindClosestNPC(Projectile.Center, 1024);
        if(nearest != null)
        {
            Vector2 targetVelocity = (nearest.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * 13;
            Projectile.velocity = Projectile.velocity.MoveTowards(targetVelocity, 3);
        }
        Projectile.rotation += Projectile.velocity.Length() * 0.05f;
    }

    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        if (Projectile.velocity.Y != oldVelocity.Y)
            Projectile.velocity.Y = -oldVelocity.Y;
        if (Projectile.velocity.X != oldVelocity.X)
            Projectile.velocity.X = -oldVelocity.X;
        return false;
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        base.OnHitNPC(target, hit, damageDone);
        HitstopTimer = 8;
        Projectile.netUpdate = true;
        SoundStyle smashSound = Main.rand.NextBool(2) ? SoundRegistry.HammerHit1 : SoundRegistry.HammerHit2;
        smashSound.PitchVariance = 0.2f;
        SoundEngine.PlaySound(smashSound, Projectile.position);

        FXUtil.GlowCircleBoom(target.Center, Color.IndianRed, Color.DarkRed, Color.DarkViolet, duration: 30, baseSize: 0.2f);
        for (float n = 0; n < 5; n++)
        {
            DustParticle dp = DustParticle.Spawn(target.Center, Projectile.velocity.RotatedByRandom(MathHelper.ToRadians(60)) * Main.rand.NextFloat(0.5f, 5f));
            dp.innerColor = Color.RosyBrown;
            dp.outerColor = Color.Black;
            dp.color = Color.Lerp(Color.Lerp(Color.Black, Color.Red, 0.1f), Color.Black, Main.rand.NextFloat(0f, 1f));
            dp.gravity = 0;
            dp.noTileCollide = true;
            dp.dampening = 0.1f;
            dp.Scale *= 0.85f;
        }

        Projectile.velocity = -Projectile.velocity.RotatedByRandom(MathHelper.ToRadians(50));
        HitCount++;
        if (HitCount >= 10)
        {
            Projectile.Kill();
        }
    }

    public override bool PreDraw(ref Color lightColor)
    {
        SpritebatchDrawer sbDrawer = SpritebatchDrawer.FromProjectile(Projectile);
        for(int i = 0; i < Projectile.oldPos.Length; i++)
        {
            SpritebatchDrawer sbDrawer2 = SpritebatchDrawer.FromProjectile(Projectile);
            Vector2 pos = Projectile.oldPos[i] + Projectile.Size * 0.5f;
            sbDrawer2.worldPosition = pos;
            sbDrawer2.rotation = Projectile.oldRot[i];
            sbDrawer2.color = Color.Lerp(Color.White, Color.Transparent, (float)i / (float)Projectile.oldPos.Length) * 0.2f;
            Main.spriteBatch.Draw(sbDrawer2);
        }
        sbDrawer.worldPosition = Projectile.Center;
        Main.spriteBatch.Draw(sbDrawer);

        sbDrawer.color = Color.Lerp(Color.Gold, Color.Transparent, ExtraMath.Osc(0f, 1f, speed: 6));
        sbDrawer.VerticalFrame(1, 2);
        Main.spriteBatch.Draw(sbDrawer);

        Texture2D glowMask = AssetManager.GlowMask.SimpleGlowCircle.Value;
        Vector2 glowDrawOrigin = glowMask.Size() / 2f;
        Color glowColor = Color.Lerp(Color.OrangeRed, Color.Red, ExtraMath.Osc(0f, 1f, speed: 8));
        glowColor.A = 0;
        Main.spriteBatch.Draw(glowMask, Projectile.Center - Main.screenPosition, null, glowColor, 0, glowDrawOrigin,
            Projectile.scale * ExtraMath.Osc(0.9f, 1.2f, speed: 8) * 0.3f, SpriteEffects.None, 0);

        return false;
        //    return base.PreDraw(ref lightColor);
    }
    public float WidthFunction(float completionRatio)
    {
        float osc = VectorHelper.Osc(0.75f, 1f);
        float w = MathHelper.SmoothStep(0f, 1f, (float)Projectile.timeLeft / 30f);
        return (Projectile.width * Projectile.scale) * osc * 2 * w;
    }
    public Color ColorFunction(float completionRatio)
    {
        return Color.Lerp(Color.Red, Color.White, ExtraMath.Osc(0f, 1f, speed: 32));
    }
    public void DrawPixelated(GraphicsDevice graphicsDevice)
    {
        //Put in the points
        //This is just a straight beam that collides with tiles
        var shader = RichLaserShader.Instance;
        shader.LaserColor = Color.Gold;
        shader.InnerColor = Color.OrangeRed;
        shader.OuterColor = Color.DarkRed;
        TrailDrawer.Draw(Main.spriteBatch, Projectile.oldPos, ColorFunction, WidthFunction, shader, Projectile.Size * 0.5f);
    }
    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
        
    }

    public void DrawToRenderTargets()
    {
        PixelationManager.QueuePrimitivesDrawAction(DrawPixelated);
    }
}