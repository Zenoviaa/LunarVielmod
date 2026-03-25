using ReLogic.Content;
using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Core.Bases;
using Stellamod.Core.Effects.Trails;
using Stellamod.Core.Pixelation;
using Stellamod.Core.SwingSystem;
using Stellamod.Helpers;
using Stellamod.Trails;
using Stellamod.Visual.Particles;
using System;
using System.Reflection;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace Stellamod.Content.Special;

public class DeadRomancesExcalibur : BaseSwingItemV2
{
    public override void SetDefaults2()
    {
        base.SetDefaults2();
        Item.damage = 10;
        Item.shoot = ModContent.ProjectileType<DeadRomancesExcaliburSlash>();
        staminaCost = 1;
        staminaDamageMultiplier = 2;
        comboResetTime = 60;
        meleeWeaponType = MeleeWeaponType.Greatsword;
    }
    public override void ShootSwing(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        DeadRomancePlayer romancePlayer = player.GetModPlayer<DeadRomancePlayer>();
        if (romancePlayer.useGreatBlade)
        {
            Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<DeadRomanceGreatBlade>(), damage, knockback, player.whoAmI);
            romancePlayer.ConsumeGreatBlade();
        }
        else
        {
            base.ShootSwing(player, source, position, velocity, type, damage, knockback);
        }

    }
}
public class DeadRomancePlayer : ModPlayer
{
    public float attackSpeedStacks;
    public float hitResetTimer;
    public bool useGreatBlade;
    public float swingRatio => attackSpeedStacks / 20f;
    public override void ResetEffects()
    {
        base.ResetEffects();
    }
    public override void PostUpdateEquips()
    {
        base.PostUpdateEquips();
        if (hitResetTimer > 0)
            hitResetTimer--;
        if (hitResetTimer <= 0)
        {
            attackSpeedStacks = 0;
        }
         
        if (attackSpeedStacks >= 20)
            attackSpeedStacks = 20;
        Player.GetAttackSpeed(DamageClass.Melee) += MathHelper.Lerp(0f, 2f, attackSpeedStacks / 20f);
    }
    public void ConsumeGreatBlade()
    {
        attackSpeedStacks = 0;
        useGreatBlade = false;
    }
}
public class DeadRomancesExcaliburSlash : BaseSwingProjectileV2
{
    private float _oldRot;
    private float _traveledRotation;
    private SlashTrailer _wideTrailer;
    private SlashTrailer _auraTrailer;
    private float _flashTimer;


    public float flashRatio => _flashTimer / 120f;
    public SlashTrailer BuildBladeSlashesTrailer()
    {
        float GetTrailWidth(float interpolant)
        {
            float w = ComboIndex > 1 ? 0.35f : 1f;
            return EasingFunction.QuadraticBump(interpolant) * 48 * w;
        }
        Color GetTrailColor(float interpolant)
        {
            float ratio = _flashTimer / 120f;
            ratio = 1f - ratio;
            Color lerp1 = Color.Lerp(Color.White, Color.DarkGray, interpolant);
            Color lerp2 = Color.Lerp(Color.Transparent, lerp1, interpolant);
            return Color.Lerp(lerp2, Color.Black, 0.75f * ratio);
        }
        SlashEffect slashEffect = new SlashEffect();
        slashEffect.BaseColor = Color.White;
        slashEffect.HighlightColor = Color.White;
        slashEffect.RimHighlightColor = Color.DarkRed;
        slashEffect.WindColor = Color.SkyBlue;
        slashEffect.BlendState = BlendState.Additive;
        slashEffect.WindTexture = TrailRegistry.CausticTrail.Value;

        SlashTrailer bladeSlashes = new SlashTrailer();
        bladeSlashes.Shader = slashEffect;
        bladeSlashes.TrailWidthFunction = GetTrailWidth;
        bladeSlashes.TrailColorFunction = GetTrailColor;
        bladeSlashes.invert = ComboIndex % 2 != 0;
        return bladeSlashes;
    }

    /// <summary>
    /// The large faint trail on this sword
    /// </summary>
    /// <returns></returns>
    public SlashTrailer BuildBladeSlashesWideTrailer()
    {
        float GetTrailWidth(float interpolant)
        {
            float w = ComboIndex > 1 ? 0.35f : 1f;
            return EasingFunction.QuadraticBump(interpolant) * 64 * w;
        }
        Color GetTrailColor(float interpolant)
        {
            float ratio = _flashTimer / 120f;
            Color lerp1 = Color.Lerp(Color.White, Color.DarkRed, interpolant);
            return Color.Lerp(Color.Transparent, lerp1, interpolant) * 0.3f * ratio;
        }
        SlashEffect slashEffect = new SlashEffect();
        slashEffect.BaseColor = Color.White;
        slashEffect.HighlightColor = Color.White;
        slashEffect.RimHighlightColor = Color.DarkRed;
        slashEffect.WindColor = Color.SkyBlue;
        slashEffect.BlendState = BlendState.Additive;
        slashEffect.WindTexture = TrailRegistry.CausticTrail.Value;

        SlashTrailer bladeSlashes = new SlashTrailer();
        bladeSlashes.Shader = slashEffect;
        bladeSlashes.TrailWidthFunction = GetTrailWidth;
        bladeSlashes.TrailColorFunction = GetTrailColor;
        bladeSlashes.invert = ComboIndex % 2 != 0;
        return bladeSlashes;
    }

    public SlashTrailer BuildAuraTrailer()
    {
        float GetTrailWidth(float interpolant)
        {
            float w = ComboIndex > 1 ? 0.35f : 1f;
            return EasingFunction.QuadraticBump(interpolant) * 128 * w;
        }
        Color GetTrailColor(float interpolant)
        {
            float ratio = _flashTimer / 120f;
            Color lerp1 = Color.Lerp(Color.White, Color.Goldenrod, interpolant);
            return Color.Lerp(Color.Transparent, lerp1, interpolant) * ratio;
        }
        BlackFireShader blackFireShader = new BlackFireShader();
        blackFireShader.SetDefaults();
        blackFireShader.InnerColor = Color.Black;
        blackFireShader.OuterEmiteColor = Color.Black;
        blackFireShader.OuterColor = Color.Goldenrod;

        SlashTrailer slashTrailer = new SlashTrailer();
        slashTrailer.Shader = blackFireShader;
        slashTrailer.TrailWidthFunction = GetTrailWidth;
        slashTrailer.TrailColorFunction = GetTrailColor;
        slashTrailer.invert = ComboIndex % 2 != 0;
        return slashTrailer;

    }
    public override void DefineCombo()
    {
        base.DefineCombo();
        SoundStyle swingSound1 = AssetRegistry.Sounds.Melee.ExcaliburStartSlash1;
        swingSound1.PitchVariance = 0.3f;

        SoundStyle swingSound2 = AssetRegistry.Sounds.Melee.ExcaliburStartSlash2;
        swingSound2.PitchVariance = 0.35f;

        SoundStyle swingSound3 = AssetRegistry.Sounds.Melee.ExcaliburStartSlash3;
        swingSound3.PitchVariance = 0.3f;

        SoundStyle swingSound4 = AssetRegistry.Sounds.Melee.ExcaliburStartSlash4;
        swingSound4.PitchVariance = 0.3f;

        SoundStyle oddSwing = Main.rand.NextBool(2) ? swingSound1 : swingSound3;
        SoundStyle evenSwing = Main.rand.NextBool(2) ? swingSound2 : swingSound4;

        oddSwing.Pitch = MathHelper.Lerp(0f, 0.75f, Owner.GetModPlayer<DeadRomancePlayer>().swingRatio);
        evenSwing.Pitch = MathHelper.Lerp(0f, 0.75f, Owner.GetModPlayer<DeadRomancePlayer>().swingRatio);

        Add(new OvalSwing
        {
            Duration = 32,
            XSwingRadius = 140,
            YSwingRadius = 115,
            SwingDegrees = 270,
            Easing = EasingFunction.InOutExpo,
            Sound = oddSwing,
        }); 
        Add(new OvalSwing
        {
            Duration = 32,
            XSwingRadius = 140,
            YSwingRadius = 115,
            SwingDegrees = 270,
            Easing = EasingFunction.InOutExpo,
            Sound = evenSwing,
        });
        Add(new OvalSwing
        {
            Duration = 32,
            XSwingRadius = 140,
            YSwingRadius = 35,
            SwingDegrees = 270,
            Easing = EasingFunction.InOutExpo,
            Sound = oddSwing,
        });

        Add(new OvalSwing
        {
            Duration = 32,
            XSwingRadius = 140,
            YSwingRadius = 35,
            SwingDegrees = 270,
            Easing = EasingFunction.InOutExpo,
            Sound = evenSwing,
        });
        useAfterImage = true;
        var bladeSlashes = BuildBladeSlashesTrailer();
        _wideTrailer = BuildBladeSlashesWideTrailer();
        _auraTrailer = BuildAuraTrailer();
        Trailer = bladeSlashes;
    }

    public override void AI()
    {
        base.AI();
        _traveledRotation += MathF.Abs(Projectile.rotation - _oldRot);
        _oldRot = Projectile.rotation;
        if (_flashTimer > 0)
        {
            _flashTimer--;
        }
        if (Timer % 16 == 0)
        {
            int index = (int)(Interpolant * swingTrailCache.Length) % swingTrailCache.Length;
            Vector2 spawnPos = swingTrailCache[index];
            SirestiasSparkleParticle sp = SirestiasSparkleParticle.Spawn(spawnPos, Vector2.Zero);
            sp.gravity = 0;
            sp.noTileCollide = true;
            sp.Scale *= 0.25f;
            sp.fast = true;
            sp.outerColor = Color.Yellow;
        }
        if (Timer % 8 == 0)
        {
            for (int i = 0; i < 4; i++)
            {
                int index = (int)(Interpolant * swingTrailCache.Length) % swingTrailCache.Length;
                Vector2 spawnPos = swingTrailCache[index];
                spawnPos += Main.rand.NextVector2Circular(32, 32);
                SirestiasSparkleParticle sp = SirestiasSparkleParticle.Spawn(spawnPos, Vector2.Zero);
                sp.gravity = 0;
                sp.noTileCollide = true;
                sp.Scale *= 0.1f;
                sp.fast = true;
                sp.outerColor = Color.Yellow;
            }

        }

        if (_traveledRotation > 0.1f)
        {
            _traveledRotation = 0f;
            int index = (int)(Interpolant * swingTrailCache.Length) % swingTrailCache.Length;
            Vector2 spawnPos = swingTrailCache[index];
            SirestiasSmokeParticle sp = SirestiasSmokeParticle.SpawnInAlphaLayer(spawnPos, Vector2.Zero);
            sp.color = Color.Lerp(Color.Lerp(Color.Black, Color.Blue, 0.15f), Color.Black, Main.rand.NextFloat(0f, 1f));
            sp.gravity = 0;
            sp.noTileCollide = true;
            sp.Scale *= 0.8f;
            sp.offsetRot = Main.rand.NextFloat(0f, MathHelper.TwoPi);

            index = (int)(Interpolant * swingTrailCache.Length) % swingTrailCache.Length;
            int nextIndex = index + 4;
            nextIndex %= swingTrailCache.Length;

            spawnPos = swingTrailCache[index];
            Vector2 spawnPos2 = swingTrailCache[nextIndex];
            Vector2 spawnVelocity = spawnPos2 - spawnPos;
            spawnVelocity = spawnVelocity.SafeNormalize(Vector2.Zero);
            spawnVelocity *= 24;

            if (Main.rand.NextBool(2))
            {
                Color color = new Color(41, 43, 66);
                var sp2 = SirestiasSmokeParticle2.SpawnInAlphaLayer(spawnPos + Main.rand.NextVector2Circular(32, 32), spawnVelocity * 0.02f);
                sp2.color = Color.Lerp(color, Color.White, 0.25f);
                sp2.gravity = 0;
                sp2.noTileCollide = true;
                sp2.Scale *= 1;
                sp2.stretchScale2 = new Vector2(1f, 0.5f);
                sp2.offsetRot = 0;
                sp2.noRot = true;
            }
            

            int denom = (int)MathHelper.Lerp(12, 4, flashRatio);
            if (Main.rand.NextBool(denom))
            {
            
              
                DustParticle dp = DustParticle.Spawn(spawnPos, spawnVelocity);
                dp.color = Color.Lerp(Color.Lerp(Color.Black, Color.Red, 0.1f), Color.Black, Main.rand.NextFloat(0f, 1f));
                dp.gravity = 0;
                dp.noTileCollide = true;
                dp.fast = true;
                dp.superFast = true;
            }

        }
    }
    public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
    {
        base.ModifyHitNPC(target, ref modifiers);
    }
    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        base.OnHitNPC(target, hit, damageDone);
        DeadRomancePlayer romancePlayer = Owner.GetModPlayer<DeadRomancePlayer>();
        romancePlayer.attackSpeedStacks++;
        if(romancePlayer.attackSpeedStacks >= 21)
        {
            romancePlayer.useGreatBlade = true;
        }
        romancePlayer.hitResetTimer = 80;

        SoundStyle hitSound = Main.rand.NextBool(2) ? AssetRegistry.Sounds.Melee.ExcaliburStartHit1 : AssetRegistry.Sounds.Melee.ExcaliburStartHit2;
        hitSound.PitchVariance = 0.4f;
        SoundEngine.PlaySound(hitSound, target.position);
        CrackParticle cp = CrackParticle.Spawn(target.Center, Vector2.Zero);
        cp.fast = true;
        _flashTimer = 120;
    }
    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
    }
    public override void DrawSwordBeam(ref Color lightColor)
    {
        base.DrawSwordBeam(ref lightColor);
    }

    public override void DrawSwordSprite(ref Color lightColor)
    {
        base.DrawSwordSprite(ref lightColor);
    }

    public override void PostDrawSword(Vector2 position, Rectangle srcRect, Color drawColor, float rotation, Vector2 origin, Vector2 drawScale, SpriteEffects spriteEffect, float layerDepth)
    {
        base.PostDrawSword(position, srcRect, drawColor, rotation, origin, drawScale, spriteEffect, layerDepth);
        if (_flashTimer <= 0)
        {
            return;
        }

        Texture2D texture = ModContent.Request<Texture2D>(Owner.HeldItem.ModItem.Texture + "_Ascended").Value;
        float ratio = _flashTimer / 120f;
        ratio = 1f - ratio;
        float ease = EasingFunction.InOutSine(ratio);
        drawColor = Color.Lerp(drawColor, Color.Transparent, ease);
        SpriteBatch spriteBatch = Main.spriteBatch;
        spriteBatch.Draw(texture, position,
            srcRect, drawColor, rotation, origin, drawScale, spriteEffect, 0);
    }

    public override bool PreDraw(ref Color lightColor)
    {

        if (_flashTimer > 0)
        {
            float ratio = _flashTimer / 120f;
            ratio = 1f - ratio;
            float ease = EasingFunction.OutExpo(ratio);
            SpriteBatch spriteBatch = Main.spriteBatch;
            SpritebatchDrawer glowDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.SimpleGlowCircle, Projectile.Center);
            glowDrawer.color = Color.Goldenrod;
            glowDrawer.color = Color.Lerp(glowDrawer.color, Color.Black, ease);
            glowDrawer.color.A = 0;
            glowDrawer.scale *= 0.35f * new Vector2(2f, 1f);
            glowDrawer.rotation = Projectile.rotation - MathHelper.PiOver4;
            spriteBatch.Draw(glowDrawer);
        }

        return base.PreDraw(ref lightColor);
    }
    public override void RenderSwingTrail(ref Color lightColor, Vector2[] points)
    {
        base.RenderSwingTrail(ref lightColor, points);
        _wideTrailer.DrawTrail(ref lightColor, points);
        _auraTrailer.DrawTrail(ref lightColor, points);
    }
}


public class DeadRomanceGreatBlade : ModProjectile
{
    public override string Texture => TextureRegistry.EmptyTexture;

    private ref float Timer => ref Projectile.ai[0];
    private ref float SwingDirection => ref Projectile.ai[1];
    private Player Owner => Main.player[Projectile.owner];
    private Vector2 _rotationalVelocity;

    public float ratio;
    public float bladeRatio;
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 16;
        Projectile.height = 16;
        Projectile.timeLeft = 180;
        Projectile.tileCollide = false;
        Projectile.penetrate = -1;
    }

    public override void AI()
    {
        base.AI();
        Timer++;
        if(Timer % 8 == 0)
        {
            Vector2 startPos = Projectile.Center;
            Vector2 endPos = startPos + _rotationalVelocity * 300;
            Vector2 spawnPos = Vector2.Lerp(startPos, endPos, Main.rand.NextFloat(0f, 1f));
        
            var sp = SirestiasSparkleParticle.Spawn(spawnPos + Main.rand.NextVector2Circular(80, 80), Vector2.Zero);
            sp.fast = true;
            sp.noTileCollide = true;
            sp.gravity = 0;
        }
        if(Timer % 8 == 0)
        {
            Vector2 spawnPos = Projectile.Center;
            SirestiasSmokeParticle sp = SirestiasSmokeParticle.SpawnInAlphaLayer(spawnPos, Vector2.Zero);
            sp.color = Color.Lerp(Color.Lerp(Color.Black, Color.Blue, 0.15f), Color.Black, Main.rand.NextFloat(0f, 1f));
            sp.gravity = 0;
            sp.noTileCollide = true;
            sp.Scale *= 0.8f;
            sp.offsetRot = Main.rand.NextFloat(0f, MathHelper.TwoPi);


            Vector2 spawnPos2 = Projectile.Center + _rotationalVelocity * 300f;
            Vector2 spawnVelocity = spawnPos2 - spawnPos;
            spawnVelocity = spawnVelocity.SafeNormalize(Vector2.Zero);
            spawnVelocity *= 24;

            if (Main.rand.NextBool(2))
            {
                Color color = new Color(41, 43, 66);
                var sp2 = SirestiasSmokeParticle2.SpawnInAlphaLayer(spawnPos + Main.rand.NextVector2Circular(32, 32), spawnVelocity * 0.02f);
                sp2.color = Color.Lerp(color, Color.White, 0.25f);
                sp2.gravity = 0;
                sp2.noTileCollide = true;
                sp2.Scale *= 1;
                sp2.stretchScale2 = new Vector2(1f, 0.5f);
                sp2.offsetRot = 0;
                sp2.noRot = true;
            }

        }


        Vector2 initialVelocity = -Vector2.UnitY;
        Vector2 targetVelocity = new Vector2(-1, -1);
        float time = 120;
        ratio = Timer / time;
        bladeRatio = Timer / (time * 0.5f);
        float ease = EasingFunction.InOutExpo(ratio);
        float radians = MathHelper.Lerp(0f, -MathHelper.PiOver4, ease);
        _rotationalVelocity = initialVelocity.RotatedBy(radians);
        Projectile.Center = Owner.Center + _rotationalVelocity.SafeNormalize(Vector2.Zero) * 64;
        Projectile.rotation = _rotationalVelocity.ToRotation() + MathHelper.PiOver4;

        float rotation = Projectile.rotation;
        Owner.ChangeDir(Projectile.direction);
        Projectile.spriteDirection = Owner.direction;
        if (Main.myPlayer == Projectile.owner)
        {
            Owner.direction = Main.MouseWorld.X > Owner.MountedCenter.X ? 1 : -1;
        }

        Owner.itemRotation = rotation * Owner.direction;
        Owner.itemTime = 2;
        Owner.itemAnimation = 2;
        // Set composite arm allows you to set the rotation of the arm and stretch of the front and back arms independently
        Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - MathHelper.ToRadians(135));
    }

    private Color GetTrailColor(float completionRatio)
    {
        return Color.Lerp(Color.White, Color.Goldenrod, completionRatio);
    }

    private float GetTrailWidth(float completionRatio)
    {
        float baseWidth = 46;
        float point = 0.5f;
        float ratio2 = completionRatio - point;
        if (ratio2 < 0)
            ratio2 = 0;

        float otherHalf = 1f - point;
        float width2 = MathHelper.SmoothStep(0f, 60f, EasingFunction.QuadraticBump(ratio2 / otherHalf));
        width2 *= MathHelper.SmoothStep(1f, 0f, ratio2 / otherHalf);
        float totalWidth = baseWidth + width2;
        totalWidth *= MathHelper.Lerp(1f, 0f, EasingFunction.InCirc(completionRatio));
        return totalWidth;
    }
    private Color GetTrailColor2(float completionRatio)
    {
        return Color.Lerp(Color.White, Color.Goldenrod, completionRatio);
    }

    private float GetTrailWidth2(float completionRatio)
    {
        float maxW = 66 * ratio;
        float w = MathHelper.SmoothStep(maxW, 0, completionRatio);
        return w;
    }
    private void DrawGlowSwordPixelPrims(GraphicsDevice gDevice)
    {
     
        Vector2 startPos = Projectile.Center;
        Vector2 endPos = startPos + _rotationalVelocity * 300 * MathHelper.SmoothStep(0.5f, 1f, ratio);
        float numPoints = 128;
        Vector2[] trailPoints = new Vector2[128];
        for(int i = 0; i < trailPoints.Length; i++)
        {
            ref Vector2 trailPoint = ref trailPoints[i];
            float completionRatio = (float)i / numPoints;
            trailPoint = Vector2.Lerp(startPos, endPos, completionRatio);

        }
        BlackFireShader laserShader = BlackFireShader.Instance;
    //    laserShader.LaserColor = Color.White;
        laserShader.InnerColor = Color.White;
        laserShader.OuterEmiteColor = Color.Yellow;
        laserShader.OuterColor = Color.Lerp(Color.Red, Color.Yellow, 0.2f);
        laserShader.PrimaryTexture2 = TrailRegistry.BeamTrail;
        laserShader.Distortion = 0.05f;
      //  laserShader.LaserTexture = TrailRegistry.StarTrail;
        //laserShader.BloomTexture = TrailRegistry.StarTrail;
        laserShader.Time = Main.GlobalTimeWrappedHourly * 24;
        TrailDrawer.Draw(Main.spriteBatch, trailPoints, GetTrailColor, GetTrailWidth, laserShader);


       
    }

    private void DrawPixelatedGlowSword(SpriteBatch spriteBatch, Vector2 screenPos)
    {
        float rotation = Projectile.rotation;
        SpriteEffects spriteEffects = SpriteEffects.None;
        if (SwingDirection == 1)
        {
            spriteEffects = SpriteEffects.FlipVertically;
            rotation -= MathHelper.PiOver2;
        }

        GlowingSwordMaskShader shader = GlowingSwordMaskShader.Instance;
        shader.TrailTexture = TrailRegistry.BeamTrail;
        shader.Distortion = 0.04f;
        shader.DistortionTexture = TrailRegistry.DirnTrail;
        shader.Time = Main.GlobalTimeWrappedHourly * 16;
        shader.Bloom = 0.3f;
        spriteBatch.Restart(effect: shader.Effect);
        SpritebatchDrawer glowSwordSprite = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.RomanceGlowSword, Projectile.Center);
        glowSwordSprite.rotation = rotation - MathHelper.PiOver4;
        glowSwordSprite.blackIsTransparency = true;
        glowSwordSprite.color = Color.White;
        glowSwordSprite.scale = new Vector2(2f, 1f);
        glowSwordSprite.worldPosition += _rotationalVelocity * 200;
        spriteBatch.Draw(glowSwordSprite);

//        glowSwordSprite.worldPosition += Vector2.UnitY.RotatedBy(Main.GlobalTimeWrappedHourly * 4) * 12;
        glowSwordSprite.color = Color.Goldenrod;
        glowSwordSprite.scale *= 1.2f;
        glowSwordSprite.color *= 0.5f;
        spriteBatch.Draw(glowSwordSprite);
        spriteBatch.RestartDefaults();

    }
    private void DrawGlowSwordSprite(ref Color lightColor)
    {
        SpriteBatch spriteBath = Main.spriteBatch;
        SpritebatchDrawer glowBallDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.StarFlare1, Projectile.Center);
        glowBallDrawer.scale = new Vector2(0.75f, 0.2f);
        glowBallDrawer.rotation = Projectile.rotation + MathHelper.PiOver4;
        glowBallDrawer.color = Color.Goldenrod;
        glowBallDrawer.color *= 0.5f;
        glowBallDrawer.color.A = 0;
        glowBallDrawer.worldPosition += _rotationalVelocity * 64;
        spriteBath.Draw(glowBallDrawer);



        glowBallDrawer.scale = new Vector2(0.66f, 0.2f);
        //glowBallDrawer.rotation = Projectile.rotation;
        glowBallDrawer.color = Color.White;
        glowBallDrawer.color *= 0.5f;
        glowBallDrawer.color.A = 0;
        spriteBath.Draw(glowBallDrawer);


        /*
        glowBallDrawer.LeftCenterOrigin();
        glowBallDrawer.scale = new Vector2(2f, 0.2f);
        glowBallDrawer.worldPosition -= _rotationalVelocity * 249;
        glowBallDrawer.rotation -= MathHelper.PiOver2;
        glowBallDrawer.color = Color.White;
        glowBallDrawer.color.A = 0;
        spriteBath.Draw(glowBallDrawer);*/
    }
    
    private void DrawSwordSprite(ref Color lightColor)
    {
        float rotation = Projectile.rotation;
        SpriteEffects spriteEffects = SpriteEffects.None;
        if (SwingDirection == 1)
        {
            spriteEffects = SpriteEffects.FlipVertically;
            rotation -= MathHelper.PiOver2;
        }

        SpriteBatch spriteBatch = Main.spriteBatch;

        Texture2D texture = ModContent.Request<Texture2D>(Owner.HeldItem.ModItem.Texture + "_Ascended").Value;
        int frameHeight = texture.Height / Main.projFrames[Projectile.type];
        int startY = frameHeight * Projectile.frame;

        Rectangle sourceRectangle = new Rectangle(0, startY, texture.Width, frameHeight);
        Vector2 origin = sourceRectangle.Size() / 2f;
        Color drawColor = Projectile.GetAlpha(lightColor);

   
        float drawScale = 1;



        float swordRotation = rotation;


        Vector2 drawPosition = Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY);

        spriteBatch.Draw(texture, drawPosition,
            sourceRectangle, drawColor, rotation, origin, drawScale, spriteEffects, 0);
        SpritebatchDrawer bloomSprite = 
            SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.StarFlare1, Projectile.Center);
        bloomSprite.rotation = Projectile.rotation;
        bloomSprite.worldPosition += _rotationalVelocity * 180;
        bloomSprite.blackIsTransparency = true;
        bloomSprite.color = Color.Goldenrod;
        bloomSprite.scale = new Vector2(2f, 0.5f);
        bloomSprite.rotation -= MathHelper.PiOver4;
        spriteBatch.Draw(bloomSprite);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        //   PixelationManager.QueuePrimitivesDrawAction(DrawGlowSwordPixelPrims);
        PixelationManager.QueueSpritebatchDrawAction(DrawPixelatedGlowSword);
        DrawGlowSwordSprite(ref lightColor);
        DrawSwordSprite(ref lightColor);
        return false;
    }

    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
    }
}