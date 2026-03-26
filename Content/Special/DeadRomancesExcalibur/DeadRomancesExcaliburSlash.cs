using Mono.Cecil;
using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Core.Effects.Trails;
using Stellamod.Core.SwingSystem;
using Stellamod.Helpers;
using Stellamod.Trails;
using Stellamod.Visual.Particles;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;
using static System.Net.Mime.MediaTypeNames;

namespace Stellamod.Content.Special.DeadRomancesExcalibur;

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
        if(romancePlayer.attackSpeedStacks >= 29)
        {
            romancePlayer.useGreatBlade = true;
            target.AddBuff(ModContent.BuffType<HeavenlyMark>(), 60 * 60);
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
        DeadRomancePlayer romancePlayer = Owner.GetModPlayer<DeadRomancePlayer>();
        if (!romancePlayer.useGreatBlade)
            return;
        if (ComboIndex > 1)
            return;
        if (SwingDirection != 1)
            return;

        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity, ModContent.ProjectileType<DeadRomanceGreatBlade>(),
              Projectile.damage * 5, Projectile.knockBack, Projectile.owner, ai1: -1);
        romancePlayer.ConsumeGreatBlade();
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
