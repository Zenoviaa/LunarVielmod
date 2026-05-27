using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Assets;
using Stellamod.Common.Players;
using Stellamod.Common.Shaders;
using Stellamod.Common.Shaders.MagicTrails;
using Stellamod.Content.CommonMaterials;
using Stellamod.Content.Gores;
using Stellamod.Core.Bases;
using Stellamod.Core.Effects.Trails;
using Stellamod.Core.Particles;
using Stellamod.Core.SwingSystem;
using Stellamod.Helpers;
using Stellamod.Items;
using Stellamod.Trailing;
using Stellamod.Trails;
using Stellamod.Visual.Particles;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;


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
        Item.damage = 54;
        Item.shoot = ModContent.ProjectileType<SwingalingSlash>();
        staminaProjectileShoot = ModContent.ProjectileType<SwingalingCharge>();
        meleeWeaponType = MeleeWeaponType.Hammer;
        staminaCost = 2;
    }

    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew(mold: ModContent.ItemType<BlankSword>(), material: ModContent.ItemType<MarshScrap>());
    }
}

public class RuneSmasherSwing : BaseSwingProjectileV2
{
    private float _hitCount;
    private bool _hit;
    private bool _playSound;
    public override void DefineCombo()
    {
        base.DefineCombo();
        SlashTrailer slashTrailer = TrailPresets.CreateIvynSlashTrail();
        slashTrailer.TrailWidthFunction = GetTrailWidth;
        Trailer = slashTrailer;
        SwingV2Helper.AddHammerSwingStyle(this);
        useAfterImage = true;
        hitStopTime = 4 * EXTRA_UPDATE_COUNT;
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

public class RuneSmasherCharge : ModProjectile
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        Main.projFrames[Type] = 3;
        ProjectileID.Sets.TrailCacheLength[Type] = 16;
        ProjectileID.Sets.TrailingMode[Type] = 2;
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
    }
    public override void AI()
    {
        base.AI();
    }
    public override bool PreDraw(ref Color lightColor)
    {
        return base.PreDraw(ref lightColor);
    }
    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        base.OnHitNPC(target, hit, damageDone);
    }
    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
    }
}

public class RuneSmasherLightning : ModProjectile
{
    public override string Texture => TextureRegistry.EmptyTexture;
    public override void SetStaticDefaults()
    {

    }
    public override void SetDefaults()
    {
        base.SetDefaults();
    }
    public override void AI()
    {
        base.AI();
    }
    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        base.OnHitNPC(target, hit, damageDone);
    }
    public override bool PreDraw(ref Color lightColor)
    {
        return base.PreDraw(ref lightColor);
    }
    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
    }
}