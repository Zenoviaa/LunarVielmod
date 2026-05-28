using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Assets;
using Stellamod.Common.Players;
using Stellamod.Common.Shaders;
using Stellamod.Common.Shaders.MagicTrails;
using Stellamod.Content.Areas.WaterSide.KingJellyfishBoss;
using Stellamod.Content.Armors.Radianthal;
using Stellamod.Content.CommonMaterials;
using Stellamod.Content.Gores;
using Stellamod.Core.Bases;
using Stellamod.Core.Effects;
using Stellamod.Core.Effects.Trails;
using Stellamod.Core.Particles;
using Stellamod.Core.Pixelation;
using Stellamod.Core.SwingSystem;
using Stellamod.Core.Utilities;
using Stellamod.Helpers;
using Stellamod.Items;
using Stellamod.Trailing;
using Stellamod.Trails;
using Stellamod.Visual.Particles;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.GameContent.Animations.Actions.Sprites;


namespace Stellamod.Content.Areas.PunkerTown.ItemsPT;


public class RuneSmasherPlayer : ModPlayer
{
    public float levelOfCharge;
}

public class RuneSmasher : BaseSwingItemV2
{
    public override void SetDefaults2()
    {
        base.SetDefaults2();
        Item.damage = 192;
        Item.shoot = ModContent.ProjectileType<RuneSmasherSwing>();
        staminaProjectileShoot = ModContent.ProjectileType<RuneSmasherCharge>();
        meleeWeaponType = MeleeWeaponType.Hammer;
        staminaCost = 3;
        staminaDamageMultiplier = 2;
    }

    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew(mold: ModContent.ItemType<BlankSword>(), material: ModContent.ItemType<MarshScrap>());
    }
}

public class RuneSmasherSwing : BaseSwingProjectileV2
{
    private float _oldRot;
    private float _traveledRotation;
    private float _hitCount;
    private bool _hit;
    private bool _playSound;
    private float LevelOfCharge => Owner.GetModPlayer<RuneSmasherPlayer>().levelOfCharge;
    private bool MaxCharge => LevelOfCharge >= 30;

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
        bloom.outerBloomColor = Color.DarkKhaki;
        bloom.bloomWidthFunction = GetBloomWidth;
        bloom.bloomColorFunction = GetBloomColor;

        SwingV2Helper.AddHammerSwingStyle2(this);

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
        return Color.Lerp(Color.Brown * 0.9f, Color.Transparent, EasingFunction.InExpo(ratio)) * 0.3f;
    }

    public override Asset<Texture2D> RequestHologramTexture()
    {
        return TextureRegistry.GlowSword_Sword;
    }

    public override void PostDrawSword(Vector2 position, Rectangle srcRect, Color drawColor, float rotation, Vector2 origin, Vector2 drawScale, SpriteEffects spriteEffect, float layerDepth)
    {
        Texture2D texture = GetTexture();
        base.PostDrawSword(position, srcRect, drawColor, rotation, origin, drawScale, spriteEffect, layerDepth);
        SpriteBatch spriteBatch = Main.spriteBatch;
        drawColor = Color.Lerp(Color.Transparent, Color.Goldenrod, LevelOfCharge / 20f);
        drawColor.A = 0;
        spriteBatch.Draw(texture, position,
             srcRect, drawColor, rotation, origin, drawScale, spriteEffect, 0);

        SpritebatchDrawer sbDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.SimpleGlowCircle, Projectile.Center);
        sbDrawer.color = drawColor * 0.3f;
        sbDrawer.scale *= 0.5f;
        spriteBatch.Draw(sbDrawer);

        if (MaxCharge)
        {
            sbDrawer.color = drawColor * 0.3f * ExtraMath.Osc(0f, 1f, speed: 32);
            sbDrawer.scale *= ExtraMath.Osc(1.5f, 2f, speed: 32);
            spriteBatch.Draw(sbDrawer);
        }
    }
    private float GetTrailWidth(float interpolant)
    {
        return EasingFunction.QuadraticBump(interpolant) * 14;
    }

    public override void AI()
    {
        base.AI(); glowAfterImageColor = Color.White * 0.2f * (LevelOfCharge / 30f);
        if (!_playSound && Interpolant >= 0.5f)
        {
            SoundStyle leafSound = Main.rand.NextBool(2) ? AssetRegistry.Sounds.Nature.LeafRustle1 : AssetRegistry.Sounds.Nature.LeafRustle2;
            leafSound.PitchVariance = 0.2f;
            SoundEngine.PlaySound(leafSound, Projectile.position);
            _playSound = true;
        }
        if (_hit)
        {
            Projectile.damage = Owner.HeldItem.OriginalDamage;
        }
        outlineColor = Color.Lerp(Color.DarkGoldenrod, Color.White, ExtraMath.Osc(0f, 1f, speed: 6));

        int denom = (int)MathHelper.Lerp(64, 8, LevelOfCharge / 30f);
        if (denom <= 8)
            denom = 8;
        if (Main.rand.NextBool(denom))
        {
            var dp = DustParticle.Spawn(Projectile.Center, Main.rand.NextVector2Circular(24, 24));
            dp.innerColor = Color.Lerp(Color.White, Color.Gold, Main.rand.NextFloat(0f, 1f));
            dp.outerColor = Color.DarkGoldenrod;
            dp.Scale *= 0.5f;
            dp.dampening = 0.1f;
            dp.gravity = 0;
        }
        _traveledRotation += MathF.Abs(Projectile.rotation - _oldRot);
        _oldRot = Projectile.rotation;

        if (_traveledRotation > 0.05f)
        {
            _traveledRotation = 0f;
            int index = (int)(Interpolant * swingTrailCache.Length) % swingTrailCache.Length;
            Vector2 spawnPos = swingTrailCache[index];
            if (SwingDirection == 2)
            {
                Vector2 diff = (spawnPos - Owner.Center);
                diff = diff.SafeNormalize(Vector2.Zero);
                spawnPos += diff * 64;
            }
            FaintSmokeParticle sp = FaintSmokeParticle.SpawnInAlphaLayer(spawnPos, Vector2.Zero);
            sp.color = Color.Lerp(Color.Lerp(Color.Black, Color.Brown, 0.15f), Color.Black, Main.rand.NextFloat(0f, 1f)) * 0.125f;
            sp.Scale *= 0.48f;
            if (SwingDirection == 2)
                sp.Scale *= 2;
            sp.behindLayer = true;

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
                var sp2 = FaintSmokeParticle.SpawnInAlphaLayer(spawnPos + Main.rand.NextVector2Circular(32, 32), spawnVelocity * 0.02f);
                sp2.color = Color.Lerp(color, Color.White, 0.25f) * 0.125f;
                sp2.Scale *= 0.5f;
            }
        }
        growScale = MathHelper.Lerp(0f, 0.3f, EasingFunction.QuadraticBump(Interpolant));
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        float pitch = MathHelper.Clamp(_hitCount * 0.05f, 0f, 1f);
        SoundStyle smashSound = Main.rand.NextBool(2) ? SoundRegistry.HammerHit1 : SoundRegistry.HammerHit2;
        smashSound.PitchVariance = 0.2f;
        SoundEngine.PlaySound(smashSound, Projectile.position);
        SoundStyle zapSound;
        int rand = Main.rand.Next(4);
        switch (rand)
        {
            default:
            case 0:
                zapSound = AssetRegistry.Sounds.LeviathanEel.LeviZap1 with { PitchVariance = 0.3f };
                break;
            case 1:
                zapSound = AssetRegistry.Sounds.LeviathanEel.LeviZap2 with { PitchVariance = 0.3f };
                break;
            case 2:
                zapSound = AssetRegistry.Sounds.LeviathanEel.LeviZap3 with { PitchVariance = 0.3f };
                break;
            case 3:
                zapSound = AssetRegistry.Sounds.LeviathanEel.LeviZap4 with { PitchVariance = 0.3f };
                break;
        }
        zapSound.MaxInstances = 3;
        zapSound.Volume = 0.3f;
        SoundEngine.PlaySound(zapSound, target.position);

        int[] gores = AutoGoreLoader.FindGores("IvynWood");
        foreach (int g in gores)
        {
            Gore.NewGore(Projectile.GetSource_FromThis(),
                Projectile.Center,
                Projectile.velocity.RotatedByRandom(MathHelper.ToRadians(20)), g, 1f);
        }

        for (float f = 0; f < 3; f++)
        {
            var dp = DustParticle.Spawn(target.Center, Main.rand.NextVector2Circular(24, 24));
            dp.innerColor = Color.Lerp(Color.White, Color.Gold, Main.rand.NextFloat(0f, 1f));
            dp.outerColor = Color.DarkGoldenrod;
            dp.Scale *= 0.5f;
            dp.dampening = 0.1f;
            dp.gravity = 0;
        }
        for (float f = 0; f < 3; f++)
        {
            var sp = SparkleParticle.Spawn(target.Center, Main.rand.NextVector2Circular(24, 24));
            sp.innerColor = Color.Lerp(Color.White, Color.Gold, Main.rand.NextFloat(0f, 1f));
            sp.outerColor = Color.DarkGoldenrod;
            sp.Scale *= 0.5f;
            sp.dampening = 0.1f;
            sp.gravity = 0;
            sp.flickering = true;
        }

        Vector2 pos2 = target.Center;
        pos2 += Main.rand.NextVector2Circular(128, 64);
        pos2.Y -= 444;
        Vector2 targetPos2 = target.Center;
        targetPos2 += Main.rand.NextVector2Circular(32, 8);
        Projectile.NewProjectile(Projectile.GetSource_FromThis(), pos2, (targetPos2 - pos2).SafeNormalize(Vector2.Zero) * 10,
                ModContent.ProjectileType<RuneSmasherSword>(), Projectile.damage, Projectile.knockBack, Projectile.owner, ai2: Main.rand.NextFloat(0, 45));

        Projectile.NewProjectile(Projectile.GetSource_FromThis(), target.Center, Vector2.Zero, ModContent.ProjectileType<RadianthalAura>(),
                (int)(Projectile.damage * 0.5f),
                Projectile.knockBack,
                Projectile.owner);
        if (_hitCount < 4)
        {
            Bounce(8);
        }
        if(LevelOfCharge < 30)
        {
            Owner.GetModPlayer<RuneSmasherPlayer>().levelOfCharge++;
            if(LevelOfCharge >= 30)
            {
                SoundStyle chargeSound = new SoundStyle("Stellamod/Assets/Sounds/SingularityFragment_Shot") with { PitchVariance = 0.3f };
                SoundEngine.PlaySound(chargeSound, Projectile.position);
                FXUtil.GlowCircleBoom(Projectile.Center, Color.White, Color.Gold, Color.DarkGoldenrod, duration: 30, baseSize: 0.2f);
            }
        }
 
        _hitCount++;
        base.OnHitNPC(target, hit, damageDone);
        if (!_hit)
        {



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

public class RuneSmasherCharge : BaseSwingProjectileV2
{
    private float _hitCount;
    private bool _hit;
    private bool _playSound;
    private float LevelOfCharge => Owner.GetModPlayer<RuneSmasherPlayer>().levelOfCharge;
    private bool MaxCharge => LevelOfCharge >= 30;
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
        bloom.outerBloomColor = Color.DarkKhaki;
        bloom.bloomWidthFunction = GetBloomWidth;
        bloom.bloomColorFunction = GetBloomColor;


        SoundStyle hammerSlash1 = SoundRegistry.HeavySwordSlash1;
        hammerSlash1.PitchVariance = 0.2f;

        SoundStyle hammerSlash2 = SoundRegistry.HeavySwordSlash2;
        hammerSlash2.PitchVariance = 0.2f;
        Add(new OvalSwing
        {
            Duration = 120,
            SwingDegrees = 330,
            XSwingRadius = 64,
            YSwingRadius = 64,
            Easing = (float lerpValue) => EasingFunction.GreatswordAnticipation(lerpValue),
            Sound = hammerSlash1,
            HitCount = 2
        });

        swordBeamLength = 180;
        useAfterImage = true;
        hitStopTime = 8 * EXTRA_UPDATE_COUNT;
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
        return Color.Lerp(Color.Brown * 0.9f, Color.Transparent, EasingFunction.InExpo(ratio)) * 0.3f;
    }

    public override Asset<Texture2D> RequestHologramTexture()
    {
        return TextureRegistry.GlowSword_Sword;
    }

    public override void PostDrawSword(Vector2 position, Rectangle srcRect, Color drawColor, float rotation, Vector2 origin, Vector2 drawScale, SpriteEffects spriteEffect, float layerDepth)
    {
        Texture2D texture = GetTexture();
        base.PostDrawSword(position, srcRect, drawColor, rotation, origin, drawScale, spriteEffect, layerDepth);
        SpriteBatch spriteBatch = Main.spriteBatch;
        drawColor = Color.Lerp(Color.Transparent, Color.Goldenrod, LevelOfCharge / 20f);
        drawColor.A = 0;
        spriteBatch.Draw(texture, position,
             srcRect, drawColor, rotation, origin, drawScale, spriteEffect, 0);

        SpritebatchDrawer sbDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.SimpleGlowCircle, Projectile.Center);
        sbDrawer.color = drawColor * 0.3f;
        sbDrawer.scale *= 0.5f;
        spriteBatch.Draw(sbDrawer);

        if (MaxCharge)
        {
            sbDrawer.color = drawColor * 0.3f * ExtraMath.Osc(0f, 1f, speed: 32);
            sbDrawer.scale *= ExtraMath.Osc(1.5f, 2f, speed: 32);
            spriteBatch.Draw(sbDrawer);
        }
    }
    private float GetTrailWidth(float interpolant)
    {
        return EasingFunction.QuadraticBump(interpolant) * 14;
    }

    public override void AI()
    {
        base.AI();
        if (_hit)
        {
            ShakeScreenPosition.Shake = 4;
        }
        if (!_playSound && Interpolant >= 0.5f)
        {
            SoundStyle leafSound = Main.rand.NextBool(2) ? AssetRegistry.Sounds.Nature.LeafRustle1 : AssetRegistry.Sounds.Nature.LeafRustle2;
            leafSound.PitchVariance = 0.2f;
            SoundEngine.PlaySound(leafSound, Projectile.position);
            _playSound = true;
        }
        outlineColor = Color.Lerp(Color.DarkGoldenrod, Color.White, ExtraMath.Osc(0f, 1f, speed: 6));
        glowAfterImageColor = Color.White * 0.2f * (LevelOfCharge / 30f);
        int denom = (int)MathHelper.Lerp(64, 8, LevelOfCharge / 30f);
        if (denom <= 8)
            denom = 8;
        if (Main.rand.NextBool(denom))
        {
            var dp = DustParticle.Spawn(Projectile.Center, Main.rand.NextVector2Circular(24, 24));
            dp.innerColor = Color.Lerp(Color.White, Color.Gold, Main.rand.NextFloat(0f, 1f));
            dp.outerColor = Color.DarkGoldenrod;
            dp.Scale *= 0.5f;
            dp.dampening = 0.1f;
            dp.gravity = 0;
        }

        growScale = MathHelper.Lerp(0f, 0.3f, EasingFunction.QuadraticBump(Interpolant));
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        float pitch = MathHelper.Clamp(_hitCount * 0.05f, 0f, 1f);
        SoundStyle smashSound = Main.rand.NextBool(2) ? SoundRegistry.HammerHit1 : SoundRegistry.HammerHit2;
        smashSound.PitchVariance = 0.2f;
        SoundEngine.PlaySound(smashSound, Projectile.position);

        int numBolts = (int)MathHelper.Lerp(2, 10, LevelOfCharge / 30f);
        for(int i = 0; i < numBolts; i++)
        {
            Vector2 pos = target.Center - Vector2.UnitY * 384;
            pos += Main.rand.NextVector2Circular(256, 64);
            Vector2 targetPos = target.Center;
            targetPos += Main.rand.NextVector2Circular(129, 64);
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), pos, targetPos - pos, 
                ModContent.ProjectileType<RuneSmasherLightning>(), Projectile.damage, Projectile.knockBack, Projectile.owner, ai2: Main.rand.NextFloat(0, 45));
        }


        Owner.velocity.Y -= 6;
        Owner.GetModPlayer<RuneSmasherPlayer>().levelOfCharge=0;
        _hitCount++;
        base.OnHitNPC(target, hit, damageDone);
        if (!_hit)
        {
            Bounce(8);

            FXUtil.ShakeCamera(target.Center, 1024, 16);
            FXUtil.PunchCamera(target.Center, Projectile.velocity, 0.5f, 2, 30);
            FXUtil.GlowCircleBoom(target.Center, Color.White, Color.Gold, Color.DarkGoldenrod, duration: 30, baseSize: 0.2f);
            FXUtil.GlowCircleBoom(target.Center, Color.White, Color.Gold, Color.DarkGoldenrod, duration: 45, baseSize: 0.24f);
            PixelPrimitiveCircleFactory.CreateInGoldBoom(target.Center);
            for (float f = 0; f < 24; f++)
            {
                var dp = DustParticle.Spawn(target.Center, -Vector2.UnitY.RotatedByRandom(MathHelper.ToRadians(60)) * Main.rand.NextFloat(5, 55));
                dp.innerColor = Color.Lerp(Color.White, Color.Gold, Main.rand.NextFloat(0f, 1f));
                dp.outerColor = Color.DarkGoldenrod;
                dp.dampening = 0.1f;
                dp.gravity = 0;
                dp.noTileCollide = true;
            }
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

public class RuneSmasherSword : ModProjectile,
    IDrawToRenderTarget
{
    private float _scale;
    private float _randScale;
    private ref float Timer => ref Projectile.ai[0];
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
        Projectile.friendly = true;
        Projectile.penetrate = -1;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = -1;
    }

    public override void AI()
    {
        base.AI();
        Timer++;
        if(Timer == 1)
        {
            _randScale = Main.rand.NextFloat(0.7f, 1f);
        }
        Projectile.velocity *= 1.1f;
        Projectile.rotation = Projectile.velocity.ToRotation();
        _scale = MathHelper.Lerp(0f, 1f, EasingFunction.OutExpo(Timer / 60f));
    }

    public override bool PreDraw(ref Color lightColor)
    {

        return false;
    }
    private void DrawPixelatedSwords(SpriteBatch sb, Vector2 screenPos)
    {
        SpritebatchDrawer sbDrawer = SpritebatchDrawer.FromProjectile(Projectile);
        sbDrawer.color = Color.Lerp(Color.Gold, Color.DarkGoldenrod, ExtraMath.Osc(0f, 1f, 0, Projectile.whoAmI));
        sbDrawer.color.A = 0;
        sbDrawer.scale *= _scale * _randScale * new Vector2(1f, 0.4f);
        Main.spriteBatch.Draw(sbDrawer);
        for (int i = 0; i < Projectile.oldPos.Length; i++)
        {
            Vector2 pos = Projectile.oldPos[i] + Projectile.Size * 0.5f;
            sbDrawer.worldPosition = pos;
            float ratio = i / (float)Projectile.oldPos.Length;
            sbDrawer.color = Color.Lerp(Color.Gold, Color.DarkGoldenrod, ratio);
            sbDrawer.color *= MathHelper.SmoothStep(1f, 0f, EasingFunction.OutExpo(ratio));
            sbDrawer.color.A = 0;
            Main.spriteBatch.Draw(sbDrawer);
        }
        sbDrawer = SpritebatchDrawer.FromProjectile(Projectile);
        sbDrawer.color = Color.White * ExtraMath.Osc(0.35f, 0.6f, speed: 6, Projectile.whoAmI);
        sbDrawer.color.A = 0;
        sbDrawer.scale *= _scale * _randScale * new Vector2(1f, 0.4f);
        sbDrawer.scale *= 0.9f;
        Main.spriteBatch.Draw(sbDrawer);
    }
    
    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        base.OnHitNPC(target, hit, damageDone);

    }

    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
        float numDust = 8;
        for (float n = 0; n < numDust; n++)
        {
            Vector2 vel = -Projectile.oldVelocity;
            vel = vel.RotatedByRandom(MathHelper.ToRadians(60));
            vel = vel.SafeNormalize(Vector2.Zero);
            vel *= Main.rand.NextFloat(6, 12f);
            DustParticleSpawnParams spawnParams = DustParticleSpawnParams.Default;
            spawnParams.outerColor = Color.Gold;
            var dp = DustParticle.Spawn(Projectile.Center, vel, spawnParams);
            dp.fast = true;
            dp.noTileCollide = true;
            dp.dampening = 0.05f;
        }
        FXUtil.ShakeCamera(Projectile.Center, 1024, 8);
    }

    public void DrawToRenderTargets()
    {
        PixelationManager.QueueSpritebatchDrawAction(DrawPixelatedSwords);
    }
}
public class RuneSmasherLightning : ModProjectile,
    IDrawToRenderTarget
{
    private float _widthMultiplier;
    private float _zapTime;
    private float _flashTimer;
    private Vector2 _controlPoint1;
    private Vector2 _controlPoint2;

    private Vector2 _controlPoint3;
    private Vector2 _controlPoint4;
    private ref float Timer => ref Projectile.ai[0];
    private ref float Style => ref Projectile.ai[1];
    private ref float RandOffset => ref Projectile.ai[2];
    private Vector2 EndPoint => Projectile.Center + Projectile.velocity;
    private Vector2 EndPoint2 => Projectile.Center - Projectile.velocity;
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.friendly = true;
        Projectile.width = 16;
        Projectile.height = 16;
        Projectile.timeLeft = 30;
        Projectile.penetrate = -1;
        Projectile.ignoreWater = true;
        Projectile.tileCollide = false;
    }

    public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
    {
        if (RandOffset > 0)
            return false;
        float collisionPoint = 0;
        if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), EndPoint + (EndPoint - Projectile.Center).SafeNormalize(Vector2.Zero) * 32, Projectile.Center, 12, ref collisionPoint))
            return true;

        return false;
    }

    public override bool ShouldUpdatePosition()
    {
        return false;
    }

    public override void AI()
    {
        base.AI();
        if(RandOffset > 0)
        {
            RandOffset--;
            Projectile.timeLeft = 30;
            return;
        }
        Timer++;
        if (Timer == 1)
        {
            if (Style == 0)
            {
                SoundStyle lightningSoundStyle = new SoundStyle("Stellamod/Assets/Sounds/StormDragon_LightingZap");
                lightningSoundStyle.PitchVariance = 0.4f;
                SoundEngine.PlaySound(lightningSoundStyle, Projectile.position);

                SoundStyle hitSound = AssetRegistry.Sounds.Melee.Vinger2;
                hitSound.PitchVariance = 0.2f;
                SoundEngine.PlaySound(hitSound, Projectile.position);
            }


            FXUtil.ShakeCamera(Projectile.Center, 1024, 12);

            for (float f = 0; f < 10; f++)
            {
                Vector2 vel = Main.rand.NextVector2Circular(9, 9);
                DustParticleSpawnParams spawnParams = DustParticleSpawnParams.Default;
                spawnParams.innerColor = Color.Lerp(Color.White, Color.Goldenrod, Main.rand.NextFloat(0f, 1f));
                spawnParams.outerColor = Color.DarkBlue;
                var d = DustParticle.Spawn(EndPoint, vel, spawnParams);
                d.dampening = 0.05f;
                d.gravity = 0;
                d.noTileCollide = true;
                d.Scale *= 0.5f;
            }
            _controlPoint1 = Projectile.Center + Main.rand.NextVector2CircularEdge(192, 192);
            _controlPoint2 = Projectile.Center + Main.rand.NextVector2CircularEdge(192, 192);
            _controlPoint3 = Projectile.Center + Main.rand.NextVector2Circular(192, 192);
            _controlPoint4 = Projectile.Center + Main.rand.NextVector2Circular(192, 192);
            _widthMultiplier = Main.rand.NextFloat(0.5f, 1f);
            var fx = FXUtil.GlowCircleBoom(EndPoint, Color.White, Color.Gold, Color.DarkGoldenrod);
            fx.Scale *= 2;
        }

        if (Timer % 5 == 0 && Timer < 30)
        {
            FXUtil.GlowCircleBoom(EndPoint, Color.Gold, Color.DarkGoldenrod, Color.Black);
        }

        if (Timer % 3 == 0)
        {


        }
        if (Timer % 10 == 0)
        {
            _zapTime = Main.rand.NextFloat(0, 100);

        }

        if (Timer % 40 == 0)
        {
            _flashTimer = 28;
        }
        _flashTimer--;
    }


    private Color GetTrailColor(float ratio)
    {
        Color bloomColor = Color.Lerp(Color.White, Color.Goldenrod, 0.8f);
        Color bloomColor2 = Color.Lerp(Color.Transparent, bloomColor, EasingFunction.QuadraticBump(ratio));
        return Color.Lerp(Color.DarkGoldenrod, Color.White, EasingFunction.QuadraticBump(ratio));
    }

    private float GetTrailWidth(float ratio)
    {
        float ease = EasingFunction.InOutSine(_flashTimer / 30f);
        float w = 20 * _widthMultiplier;
        float outEasing = EasingFunction.InOutSine((float)Projectile.timeLeft / 30f);
        return MathHelper.SmoothStep(w * 0.85f, w, EasingFunction.QuadraticBump(ratio)) * outEasing;
    }

    private Color GetTrailColor2(float ratio)
    {
        Color bloomColor = Color.Lerp(Color.White, Color.Gold, 0.8f);
        Color bloomColor2 = Color.Lerp(Color.Transparent, bloomColor, EasingFunction.QuadraticBump(ratio));
        return Color.Lerp(Color.DarkGoldenrod, Color.White, EasingFunction.QuadraticBump(ratio));
    }

    private float GetTrailWidth2(float ratio)
    {
        return GetTrailWidth(ratio) * 1.6f;
    }

    private void DrawPixelatedLightning(GraphicsDevice gDevice)
    {
        int numPoints = 64;
        List<Vector2> trailPoints = new List<Vector2>(numPoints);
        Vector2 up = Projectile.velocity.SafeNormalize(Vector2.Zero).RotatedBy(MathHelper.PiOver2);
        float outEase = EasingFunction.InOutSine((float)Projectile.timeLeft / 30f);
        Vector2 cp1 = Vector2.Lerp(_controlPoint1, _controlPoint3, outEase);
        Vector2 cp2 = Vector2.Lerp(_controlPoint2, _controlPoint4, outEase);
        for (float f = 0; f < numPoints; f++)
        {
            float ratio = (float)f / (float)numPoints;
            Vector2 startPoint = Projectile.Center;
            Vector2 trailPoint = ExtraMath.CubicBezier(startPoint,
                cp1, cp2, EndPoint2, ratio);
            trailPoint += up * MathF.Sin(ratio * 16 + _zapTime) * 32;
            trailPoints.Add(trailPoint);
        }

        for (int i = 0; i < 4; i++)
            trailPoints.Add(trailPoints[trailPoints.Count - 1]);
        Vector2[] lightningPoints = trailPoints.ToArray();
        ZapLightningShader lightingShader = ZapLightningShader.Instance;
        lightingShader.Amplitude = 0.8f;

        float time = Main.GlobalTimeWrappedHourly * 16;
        float levels = 4;
        time = MathF.Floor(time * levels) / levels;
        lightingShader.Time = time;
        Asset<Texture2D> laserTexture = Timer > 15 ? AssetManager.LaserTextures.TexturedLaser : AssetManager.LaserTextures.TexturedLaser2;
        lightingShader.LaserTexture = laserTexture;
        lightingShader.Noise = ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/BlurryPerlinNoise").Value;
        lightingShader.Gradient = TextureAssets.Projectile[Type].Value;
        lightingShader.TransformMatrix = TrailDrawer.WorldViewPoint2;
        lightingShader.Levels = 64;
        lightingShader.Tiling = new Vector2(2f);

        TrailDrawer.Draw(Main.spriteBatch, lightningPoints, GetTrailColor, GetTrailWidth, lightingShader, Projectile.Size * 0.5f);
        BloomTrailShader bloom = BloomTrailShader.Instance;
        bloom.InnerColor = Color.Gold;
        bloom.OuterColor = Color.DarkGoldenrod;
        TrailDrawer.Draw(Main.spriteBatch, lightningPoints, GetTrailColor2, GetTrailWidth2, bloom, Projectile.Size * 0.5f);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        return false;
    }

    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
    }

    public void DrawToRenderTargets()
    {
        if (RandOffset > 0)
            return;
        PixelationManager.QueuePrimitivesDrawAction(DrawPixelatedLightning);
    }
}