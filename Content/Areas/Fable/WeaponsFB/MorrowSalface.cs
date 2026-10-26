using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Assets;
using Stellamod.Common;
using Stellamod.Common.Particles;
using Stellamod.Common.Shaders;
using Stellamod.Common.ShockCircleSystem;
using Stellamod.Content.CommonMaterials;
using Stellamod.Content.Dusts;
using Stellamod.Core.Bases;
using Stellamod.Helpers;
using Stellamod.Items;
using Stellamod.Trails;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Fable.WeaponsFB;

public class MorrowSalfaceCircle : ModPlayer
{
    public override void PostUpdateEquips()
    {
        base.PostUpdateEquips();
        if (Player.HeldItem.type == ModContent.ItemType<MorrowSalface>())
        {
            CrossbowPlayer crossbowPlayer = Player.GetModPlayer<CrossbowPlayer>();
            crossbowPlayer.magicCircleColor = Color.LightGreen;
            crossbowPlayer.magicCircleTextureAsset = AssetReferences.Assets.GlowMasks.PotionMagicCircle.Asset;
        }
    }
}

public class MorrowSalfaceClouds : ModProjectile
{
    private ref float Timer => ref Projectile.ai[0];
    public override string Texture => TextureRegistry.EmptyTexture;
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 64;
        Projectile.height = 64;
        Projectile.hostile = true;
        Projectile.penetrate = -1;
        Projectile.tileCollide = false;
        Projectile.timeLeft = 60;
    }

    public override void AI()
    {
        base.AI();
        Timer++;
        if (Timer % 6 == 0)
        {
            var pos = Projectile.Center + Main.rand.NextVector2Circular(32, 32);
            Particles.RoughSmoke.Spawn(new()
            {
                position = pos,
                timeLeft = 52,
                color = DrawUtilities.InterpolateColorArray(Main.rand.NextFloat(0f, 1f), Color.DarkRed, Color.Red, Color.DarkOrange, Color.Pink),
                scale = Main.rand.NextFloat(0.8f, 1f),
                rotation = Main.rand.NextFloat(6f)
            });
        }

        if (Main.rand.NextBool(8))
        {
            var pos = Projectile.Center + Main.rand.NextVector2Circular(32, 32);
            var d = Dust.NewDustPerfect(pos, DustID.Torch, Vector2.Zero, Scale: 1f);
            d.noGravity = true;
        }
        Projectile.velocity.X *= 0.98f;
        Projectile.velocity.Y -= 0.05f;
    }

    public override bool PreDraw(ref Color lightColor)
    {
        return false;
        //  return base.PreDraw(ref lightColor);
    }
    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
        if (this.OwnedByLocalClient())
        {
            foreach(PositionVelocity posVel in new CircleOrientation(Projectile.Center, 64, 8))
            {
                var firer = ProjFirer.From<MorrowSalfaceClouds>(Projectile);
                firer.position = posVel.position;
                firer.velocity = posVel.velocity;
                firer.New();
            }
        }
    }
}

public class MorrowSalfaceBigPotion : ModProjectile
{
    ref float Timer => ref Projectile.ai[0];
    public override string Texture => AssetReferences.Assets.Textures.Fable.Items.MorrowSalfaceBigPotion.KEY;
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        Projectile.SetTrailCacheLength(8);
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.WidthAndHeight = 24;
        Projectile.friendly = true;
        Projectile.tileCollide = true;
        Projectile.timeLeft = 240;
        Projectile.light = 0.4f;
    }

    public override void AI()
    {
        base.AI();
        Timer++;
        if(Timer % 4 == 0)
        {
            var pos = Projectile.Center + Main.rand.NextVector2Circular(64, 64);
            var vel = Main.rand.NextVector2Circular(2, 2);
            Particles.SwirlingFlameDust.Spawn(BitDustFactory.SlowingOverTime with
            {
                position = pos,
                velocity = vel,
                innerColor = Color.DarkRed.ToVector4(),
                outerColor = Color.Lerp(Color.DarkRed, Color.Black, 0.5f).ToVector4(),
                timeLeft = 60
            });
        }
        Projectile.velocity.X *= 0.98f;
        Projectile.velocity.Y += 0.25f;
        Projectile.rotation += MathF.Sign(Projectile.velocity.X) * 0.04f;
    }
    public override bool PreDraw(ref Color lightColor)
    {

        DrawUtilities.DrawSpriteAfterImage(Main.spriteBatch, Projectile, Color.Red, Color.Transparent, 0.3f);
        var drawer = Projectile.Drawer;
        Main.spriteBatch.Draw(drawer);
        return false;
    }
}

public class MorrowSalfacePotion : ModProjectile
{
    ref float Timer => ref Projectile.ai[0];
    ref float Frame => ref Projectile.ai[1];
    Color GetColor()
    {
        switch (Frame)
        {
            default:
            case 0:
                return Color.Blue;
            case 1:
                return Color.Green;
            case 2:
                return Color.Red;

        }
        
    }
    public override string Texture => AssetReferences.Assets.Textures.Fable.Items.MorrowSalfacePotion.KEY;
    public override void OnSpawn(IEntitySource source)
    {
        base.OnSpawn(source);
        Frame = Main.rand.Next(3);
    }
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        Main.projFrames[Type] = 3;
        Projectile.SetTrailCacheLength(16);
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.WidthAndHeight = 16;
        Projectile.friendly = true;
        Projectile.tileCollide = true;
        Projectile.friendly = true;
        Projectile.light = 0.4f;
    }

    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
        foreach(PositionVelocity posVel in new CircleOrientation(Projectile.Center, 32, 8))
        {
            var pos = posVel.position;
            var vel = posVel.velocity * Main.rand.NextFloat(0.5f, 1f);
            Particles.SwirlingFlameDust.Spawn(BitDustFactory.SlowingOverTime with
            {
                position = pos,
                velocity = vel,
                innerColor = GetColor().ToVector4(),
                outerColor = Color.Lerp(GetColor(), Color.Black, 0.5f).ToVector4(),
                timeLeft = 60
            });
        }
    }

    public override void AI()
    {
        base.AI();
        Timer++;
        if(Timer % 6 == 0)
        {
            var pos = Projectile.Center + Main.rand.NextVector2Circular(32, 32);
            var velocity = Main.rand.NextVector2Circular(2, 2);
            var d = Dust.NewDustPerfect(pos, DustID.GemDiamond, velocity, Scale: 0.5f);
            d.noGravity = true;
        }

        Projectile.frame = (int)Frame;
        Projectile.velocity.Y += 0.25f;
        Projectile.rotation += MathF.Sign(Projectile.velocity.X) * 0.04f;
        Projectile.velocity.X *= 0.98F;
    }
    public override bool PreDraw(ref Color lightColor)
    {
        DrawUtilities.DrawSpriteAfterImage(Main.spriteBatch, Projectile, GetColor(), Color.Transparent, 0.3f);
        var drawer = Projectile.Drawer;
        Main.spriteBatch.Draw(drawer);
        return false;
    }
}

public class MorrowSalface : BaseCrossbowItem
{
    public override string Texture => AssetReferences.Assets.Textures.Fable.Items.MorrowSalface.KEY;
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.damage = 13;
        Item.channel = true;
        Item.autoReuse = false;
        staminaProjectileShoot = ModContent.ProjectileType<MorrowSalfacePotionHeld>();
    }

    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew(
            mold: ModContent.ItemType<BlankBow>(),
            material: ModContent.ItemType<AlcadizScrap>());
    }

    public override void ShootBow(Player player, EntitySource_ItemUse_WithAmmo source, ShootParams shootParams)
    {
        Vector2 fireVelocity = shootParams.velocity * shootParams.speed;
        fireVelocity *= 3;
        fireVelocity *= shootParams.chargeStrength;
        if (shootParams.projToShoot == ProjectileID.WoodenArrowFriendly)
            shootParams.projToShoot = ModContent.ProjectileType<MorrowSalfacePotion>();

        for(var i = 0; i < 3; i++)
        {
            float bowDamage = shootParams.damage * shootParams.chargeStrength;
            Projectile crossShot = Projectile.NewProjectileDirect(source, shootParams.position, fireVelocity.RotatedByRandom(0.6f) * Main.rand.NextFloat(0.5f, 1f),
                shootParams.projToShoot, (int)bowDamage, shootParams.knockBack, player.whoAmI, ai0: shootParams.projToShoot);

            if (shootParams.projToShoot != ModContent.ProjectileType<MorrowSalfacePotion>())
                crossShot.GetGlobalProjectile<CrossbowGlobalProjectile>().isCrossbowShot = true;
        }

    }


}

public class MorrowSalfacePotionHeld : ModProjectile
{
    float _actualScale;
    public override string Texture => TextureRegistry.EmptyTexture;
    Asset<Texture2D> LittleBomb => AssetReferences.Assets.Textures.Fable.Items.MorrowSalfaceBigPotion.Asset;
    Player Owner => Main.player[Projectile.owner];
    ref float Timer => ref Projectile.ai[0];
    ref float AttackCycle => ref Projectile.ai[1];
    ref float KillMe => ref Projectile.ai[2];
    float LittleChargeTime => 60;
    float BigChargeTIme => 100;
    float ChargeScale => AttackCycle == 0 ? EasingFunction.InOutSine(Timer / LittleChargeTime) : 1f;
    float AggregateScale => MathHelper.Lerp(0.5f, 1f, ChargeScale);
    Vector2 HoldPosition => Owner.MountedCenter + new Vector2(0, -48);
    int ThrowingDamage
    {
        get
        {
            return (int)(AggregateScale * Projectile.damage * 2F);
        }
    }
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        Main.projFrames[Type] = 2;
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = Projectile.height = 60;
        Projectile.tileCollide = false;
        Projectile.ignoreWater = true;
        Projectile.light = 0.5f;
        Projectile.penetrate = -1;
    }
    void PlayBombGrowSound()
    {
        var sound = AssetReferences.Assets.Sounds.BombGrowStart.Asset with { PitchVariance = 0.4f };
        SoundEngine.PlaySound(sound, Projectile.position);
    }

    void PlayLittleBombBigGrowCompleteSound()
    {
        var sound = AssetReferences.Assets.Sounds.BombGrowComplete.Asset with { PitchVariance = 0.4f, Pitch = -0.5f };
        SoundEngine.PlaySound(sound, Projectile.position);
        ShockCircles.CreateSmallQuickWhiteFlash(Projectile.Center);
        var factory = ParticleUtils.ParticleFactory.FromSmallBurst(Projectile.Center, Projectile.Center + new Vector2(0, -2), TriColorPalette.Silver, new Vector2(5, 15f));
        ParticleUtils.CreateSwirlingDustCircle(factory);
    }



    void MakeSuckingParticle()
    {
        if (!Main.rand.NextBool(3))
            return;
        var pos = Projectile.Center + Main.rand.NextVector2CircularEdge(128, 128);
        var vel = (Projectile.Center - pos) * 0.09f;
        Particles.BitDust.Spawn(BitDustFactory.SlowingOverTime with { position = pos, velocity = vel, innerColor = Color.White.ToVector4(), outerColor = Color.SkyBlue.ToVector4(), timeLeft = 15, scale = new Vector2(Main.rand.NextFloat(0.5f, 1.2f)) * 0.4f });
    }

    void MakeSuckingParticleLittle()
    {
        if (!Main.rand.NextBool(3))
            return;
        var pos = Projectile.Center + Main.rand.NextVector2CircularEdge(128, 128);
        var vel = (Projectile.Center - pos) * 0.09f;
        Particles.BitDust.Spawn(BitDustFactory.SlowingOverTime with { position = pos, velocity = vel, innerColor = Color.White.ToVector4(), outerColor = Color.SkyBlue.ToVector4(), timeLeft = 15, scale = new Vector2(Main.rand.NextFloat(0.5f, 1.2f) * 0.15f) });
    }

    public override bool ShouldUpdatePosition()
    {
        return false;
    }

    public override void AI()
    {
        base.AI();

        Timer++;
        switch (AttackCycle)
        {
            case 0:
                {
                    if (Timer == 1)
                    {
                        PlayBombGrowSound();
                    }

                    if (Timer < LittleChargeTime)
                    {
                        MakeSuckingParticle();
                    }
                    else
                    {
                        MakeSuckingParticleLittle();
                    }

                    if (Timer == LittleChargeTime - 1)
                    {
                        PlayLittleBombBigGrowCompleteSound();
                    }


                }
                break;
        }

        if (this.OwnedByLocalClient() && !Owner.controlRight)
        {
            if (Timer >= LittleChargeTime * 0.5f)
            {
                var firer = ProjFirer.From<MorrowSalfaceBigPotion>(Projectile);
                firer.damage = ThrowingDamage;
                firer.position = HoldPosition;
                firer.velocity = (Main.MouseWorld - Projectile.Center).Resize(14);
                firer.velocity.Y -= 3;
                firer.ai0 = 60;
                firer.ai2 = AggregateScale;
                firer.New();
            }

            KillMe = 1;
            Projectile.netUpdate = true;
        }

        if (this.OwnedByLocalClient())
        {
            Projectile.velocity.X = Main.MouseWorld.X < Projectile.Center.X ? -1 : 1;

            Projectile.netUpdate = true;
        }
        Owner.ChangeDir((int)Projectile.velocity.X);
        _actualScale = MathHelper.Lerp(_actualScale, AggregateScale, 0.1f);
        Projectile.Center = HoldPosition;
        if (KillMe > 0)
            Projectile.Kill();
        var osc = ExtraMath.Osc(-0.05f, 0.5f, offset: Projectile.identity);
        Owner.heldProj = Projectile.whoAmI;
        Owner.itemTime = 2;
        Owner.itemAnimation = 2;
        Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, (-Vector2.UnitY).ToRotation() - MathHelper.PiOver2 + osc);
        Owner.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Full, (-Vector2.UnitY).ToRotation() - MathHelper.PiOver2 + osc);

    }
    public override bool PreDraw(ref Color lightColor)
    {
        var holdPos = Owner.MountedCenter + new Vector2(0, -18);
        var asset = LittleBomb;
        var drawer = SpritebatchDrawer.FromTextureAsset(asset, holdPos);
        drawer.VerticalFrame(0, Main.projFrames[Type]);
        drawer.scale *= _actualScale;
        drawer.scale *= ExtraMath.Osc(0.95f, 1f, speed: 3, Projectile.identity);
        Main.spriteBatch.Draw(drawer);
        return false;
    }
}

