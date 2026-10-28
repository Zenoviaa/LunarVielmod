using Stellamod.Common;
using Stellamod.Common.MeleeSwingSystem;
using Stellamod.Common.MeteorHammerSystem;
using Stellamod.Common.Particles;
using Stellamod.Common.Shaders;
using Stellamod.Content.CommonMaterials;
using Stellamod.Core.Pixelation;
using Stellamod.Items;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.SpringHills.WeaponsSH;

public class ThornedMeteorHammer : ModItem
{
    public override string Texture => AssetReferences.Assets.Textures.SpringHills.Items.ThornedMeteorHammer.KEY;
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        Item.IsMeteorHammer = true;
        Item.MeteorHammerStaminaCost = 2;
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.DefaultToMeteorHammer<ThornedMeteorHammerBall>();
        Item.damage = 15;
    }

    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew<Ivythorn, BlankSafunai>();
    }
}


public class ThornedMeteorShard : ModProjectile
{
    ref float Timer => ref Projectile.ai[0];
    public override string Texture => AssetReferences.Assets.Textures.SpringHills.Items.ThornedMeteorShard.KEY;
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        Projectile.SetTrailCacheLength(6);
        Main.projFrames[Type] = 2;
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.LocalHitOnce = true;
        Projectile.WidthAndHeight = 12;
        Projectile.friendly = true;
        Projectile.light = 0.2f;
        Projectile.ignoreWater = true;
    }
    public override void AI()
    {
        base.AI();
        Timer++;
        if(Timer == 1)
        {
            var stinger = SoundID.Item17 with { PitchVariance = 0.5f };
            SoundEngine.PlaySound(stinger, Projectile.position);
        }

        if(Projectile.velocity.Y < 12)
            Projectile.velocity.Y += 0.5f;
        Projectile.rotation = Projectile.velocity.ToRotation();
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        base.OnHitNPC(target, hit, damageDone);
        Projectile.Kill();
        target.AddBuff(BuffID.Poisoned, 120);

    }
    public override bool PreDraw(ref Color lightColor)
    {
        DrawUtilities.DrawSpriteAfterImage(Main.spriteBatch, Projectile, Color.Green, Color.Transparent, 0.2f);
        var drawer = Projectile.Drawer;
        Main.spriteBatch.Draw(drawer);

        drawer.VerticalFrame(1, Main.projFrames[Type]);
        drawer.color = Color.Lerp(Color.Transparent, Color.LightGreen, ExtraMath.Osc(0f, 0.5f, speed: 6));
        Main.spriteBatch.Draw(drawer);
        return false;
    }

    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
        var fac = ParticleUtils.ParticleFactory.FromSmallBurst(Projectile.Center, Projectile.Center + Projectile.velocity, TriColorPalette.Foresty, new Vector2(4, 15));
        fac.particleCount /= 2;
        ParticleUtils.CreateSwirlingDustCircle(fac);

    }
}
public class ThornedMeteorHammerBall : ModProjectile
{

    private Chain Chain
    {
        get
        {
            field ??= new Chain(Projectile.Center, 16, 16);
            return field;
        }
    }

    float Scale => EasingFunction.InOutSine((float)Projectile.timeLeft / 60F);
    float _outlineAlpha;
    bool _playedBounceSound;
    Vector2 _initialVelocity;
    Vector2 _startPoint;
    float _swingDir;
    float _xSwingRadius;
    float YSwingRadius => 80;
    float SwingRadians => 5.75f;
    float MaxXSwingRadius => 256;
    float SwingRatio => Projectile.HammerState == MeteorHammerState.SwingOut ? Projectile.HammerTimer / SwingTime  : Projectile.HammerTimer /StaminaSwingTime;
    float SwingTime => 60F * (1f / Projectile.PlayerOwner.GetTotalAttackSpeed(DamageClass.Ranged)) * MathHelper.Lerp(0.75f, 1f, _xSwingRadius / MaxXSwingRadius);
    float StaminaSwingTime => 120F * (1f / Projectile.PlayerOwner.GetTotalAttackSpeed(DamageClass.Ranged));
    float TipDamageMultiplier => 1.5f;
    public override string Texture => AssetReferences.Assets.Textures.SpringHills.Items.ThornedMeteorHammerBall.KEY;
    public override void SendExtraAI(BinaryWriter writer)
    {
        base.SendExtraAI(writer);
        writer.Write(_xSwingRadius);
        writer.Write(_swingDir);
        writer.WriteVector2(_startPoint);
        writer.Write(_outlineAlpha);
    }
    public override void ReceiveExtraAI(BinaryReader reader)
    {
        base.ReceiveExtraAI(reader);
        _xSwingRadius = reader.ReadSingle();
        _swingDir = reader.ReadSingle();
        _startPoint = reader.ReadVector2();
        _outlineAlpha = reader.ReadSingle();
    }
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        Projectile.SetTrailCacheLength(16);
        Main.projFrames[Type] = 2;
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.DefaultToMeteorHammerBall();
        Projectile.WidthAndHeight = 32;
        Projectile.tileCollide = false;
    }

    public override bool? CanHitNPC(NPC target)
    {
        var e = (Projectile.HammerState == MeteorHammerState.SwingOut || Projectile.HammerState == MeteorHammerState.StaminaSwingOut);
        return e;
    }

    public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
    {
        //Check the last 5 points as well
        //wait why didn't we just do this with swords?
        //bruh?
        //This is the easiest way to fix the fast collision issue :sob:
        for(var i = 0; i < 6; i++)
        {
            var ratio = SwingRatio - 0.015f * i;
            var point = GetSwingPoint(ratio);
            var oldRect = projHitbox;
            oldRect.Location = (point - projHitbox.Size() * 0.5f).ToPoint();
            if (oldRect.Intersects(targetHitbox))
                return true;
        }
        float collisionPoint = 0F;
        if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), 
            Projectile.PlayerOwner.Center, Projectile.Center, 6, ref collisionPoint))
            return true;
        return base.Colliding(projHitbox, targetHitbox);
    }

    public override void AI()
    {
        base.AI();
        IncreaseTimerNetSyncStart();
        MoonUtils.MeteorHammer_KillIfNotHoldingBall(Projectile);
        Projectile.tileCollide = false;
        if (Projectile.timeLeft < 60)
        {
            Projectile.Center = Vector2.Lerp(Projectile.Center, Projectile.PlayerOwner.Center, 0.2f);
        }
        else
        {

        }

        switch (Projectile.HammerState)
        {
            case MeteorHammerState.Hold:
                AI_HoldOut();
                break;
            case MeteorHammerState.SwingOut:
                AI_Swing();
                break;
            case MeteorHammerState.StaminaSwingOut:
                AI_StaminaSwing();
                break;
        }
    }

    void IncreaseTimerNetSyncStart()
    {
        Projectile.HammerTimer++;
        if (Projectile.HammerTimer == 1)
        {
            _playedBounceSound = false;
            Projectile.netUpdate = true;
        }
    }

    void AI_HoldOut()
    {
        Vector2 effector = MoonUtils.MeteorHammer_SimulateChainn( Chain, Projectile.PlayerOwner);
        Vector2 targetVelocity = effector - Projectile.Center;
        Projectile.velocity = Vector2.Lerp(_initialVelocity, targetVelocity, EasingFunction.InOutSine(Projectile.HammerTimer / 60F));
        Projectile.rotation += Projectile.velocity.X * 0.02f;
        //MoonUtils.MeteorHammer_ChainDragging(Projectile, Chain);
        //  MoonUtils.MeteorHammer_Drag(Projectile, Projectile.PlayerOwner, dragDistance: 128);
    }

    public override bool ShouldUpdatePosition()
    {
        if (Projectile.HammerState != MeteorHammerState.Hold)
            return false;
        return base.ShouldUpdatePosition();
    }

    Vector2 GetHelicopterSwingPoint(float ratio)
    {
        //For this stamina swing, the ball is going to spin around you and randomly throw thorns everywhere lol
        //It spins in a helicopter fashion above your head btw
        var offset = MoonUtils.OvalProgressPoint(ratio, 24.56F, 128, 32);
        offset.Y -= 80;
        var pointToMoveTo = Projectile.PlayerOwner.Center + offset;
        var easedOvalPoint = Vector2.Lerp(_startPoint, pointToMoveTo, EasingFunction.InOutSine(ratio / 0.15f));
        return easedOvalPoint;
    }
    Vector2 GetSwingPoint(float ratio)
    {
        var ovalOffset = MoonUtils.OvalProgressPoint(EasingFunction.InOutBack(ratio), SwingRadians, _xSwingRadius, YSwingRadius);
        if(_swingDir <= 0)
        {
            ovalOffset = MoonUtils.OvalProgressPoint(1F - EasingFunction.InOutBack(ratio), SwingRadians, _xSwingRadius, YSwingRadius);
        }
        ovalOffset = ovalOffset.RotatedBy(Projectile.velocity.ToRotation());
        var ovalPoint = Projectile.PlayerOwner.Center + ovalOffset;
        var easedOvalPoint = Vector2.Lerp(_startPoint, ovalPoint, EasingFunction.InOutSine(ratio / 0.15f));
        return ovalPoint;
    }

    void AI_Swing()
    {
        if (Projectile.HammerTimer == 1)
        {
            var saf = AssetReferences.Assets.Sounds.Melee.Safunais.Asset with { PitchVariance = 0.5f, Volume = 0.4f };
            SoundEngine.PlaySound(saf, Projectile.position);

            _startPoint = Projectile.Center;
            if (this.OwnedByLocalClient())
            {
                if (_swingDir <= 0)
                    _swingDir = 1;
                else
                    _swingDir = -1;
            }
            MoonUtils.MeteorHammer_PrepareSwing(Projectile, MaxXSwingRadius, ref _xSwingRadius);
        }

        for(var i = 0; i < Chain.points.Length; i++)
        {
            ref var p = ref Chain.points[i];
            p = Vector2.Lerp(Projectile.PlayerOwner.Center, Projectile.Center, (float)i / (float)Chain.points.Length);
        }
        MoonUtils.OrientHandForLikeMeteorHammer(Projectile, Projectile.PlayerOwner);
        _outlineAlpha = EasingFunction.QuadraticBump(SwingRatio);
        Projectile.Center = GetSwingPoint(SwingRatio);
        if(Projectile.HammerTimer >= SwingTime)
        {
            _initialVelocity = Projectile.velocity;
            Projectile.HammerTimer = 0;
            Projectile.HammerState = MeteorHammerState.Hold;
        }
    }

    void AI_StaminaSwing()
    {
        if (Projectile.HammerTimer == 1)
        {
            _startPoint = Projectile.Center;
            if (this.OwnedByLocalClient())
            {
                if (_swingDir <= 0)
                    _swingDir = 1;
                else
                    _swingDir = -1;
            }
            Projectile.netUpdate = true;
        }
        _outlineAlpha = EasingFunction.QuadraticBump(SwingRatio);

        if(Projectile.HammerTimer % 5 == 0)
        {
            if (this.OwnedByLocalClient())
            {
                var firer = ProjFirer.From<ThornedMeteorShard>(Projectile);
                firer.velocity.Y = Main.rand.Next(-10, -5);
                firer.velocity.X = Main.rand.NextFloat(-10, 10);
                firer.damage *= 2;
                firer.New();
            }
        }
        for (var i = 0; i < Chain.points.Length; i++)
        {
            ref var p = ref Chain.points[i];
            p = Vector2.Lerp(Projectile.PlayerOwner.Center, Projectile.Center, (float)i / (float)Chain.points.Length);
        }
        if (Main.rand.NextBool(4))
        {
            var pos = Projectile.Center + Main.rand.NextVector2Circular(32, 32);
            Particles.IvynLeaf.Spawn(new()
            {
                position = pos,
                velocity = (pos - Projectile.Center).Resize(8) * Main.rand.NextFloat(0.5f, 1f),
                timeLeft = Main.rand.Next(45, 180),
                scale = Main.rand.NextFloat(0.5f, 1.2f),
                rotation = Main.rand.NextFloat() * 6.28f,
            });
        }

        MoonUtils.OrientHandForLikeMeteorHammer(Projectile, Projectile.PlayerOwner);
  
        Projectile.Center = GetHelicopterSwingPoint(SwingRatio);
        if (Projectile.HammerTimer >= StaminaSwingTime)
        {
            _initialVelocity = Projectile.velocity;
            Projectile.HammerTimer = 0;
            Projectile.HammerState = MeteorHammerState.Hold;
        }
    }

    public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
    {
        base.ModifyHitNPC(target, ref modifiers);
        var tipper = MoonUtils.MeteorHammer_TipperDamageMechanic(Projectile, target, ref modifiers, TipDamageMultiplier);

        var hitSound = AssetReferences.Assets.Sounds.NaturalHit.Asset with { PitchVariance = 0.5f };
        SoundEngine.PlaySound(hitSound, target.Center);

        if (tipper)
        {
            var tippere = AssetReferences.Assets.Sounds.AssassinsKnifeHit.Asset with { PitchVariance = 0.5f };
            SoundEngine.PlaySound(tippere, target.Center);

            var fx = FXUtil.GlowCircleBoom(target.Center, Color.White, Color.LightGreen, Color.DarkGreen, 22);
            fx.Scale *= 1.5f;

            var fac = ParticleUtils.ParticleFactory.FromSmallBurst(target.Center, TriColorPalette.Foresty, new Vector2(4, 15));
            ParticleUtils.CreateSwirlingDustCircle(fac);

            for(var i = 0; i < 8; i++)
            {
                var pos = target.Center + Main.rand.NextVector2Circular(32, 32);
                Particles.IvynLeaf.Spawn(new()
                {
                    position = pos,
                    velocity = (pos - target.Center).Resize(8) * Main.rand.NextFloat(0.5f, 1f),
                    timeLeft = Main.rand.Next(45, 180),
                    scale = Main.rand.NextFloat(0.5f, 1.2f),
                    rotation = Main.rand.NextFloat() * 6.28f,
                });
            }
        }
        else
        {
            for (var i = 0; i < 3; i++)
            {
                var pos = target.Center + Main.rand.NextVector2Circular(32, 32);
                Particles.IvynLeaf.Spawn(new()
                {
                    position = pos,
                    velocity = (pos - target.Center).Resize(8) * Main.rand.NextFloat(0.5f, 1f),
                    timeLeft = Main.rand.Next(45, 180),
                    scale = Main.rand.NextFloat(0.5f, 1.2f),
                    rotation = Main.rand.NextFloat() * 6.28f,
                });
            }
        }
    }

    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        if(Projectile.HammerState == MeteorHammerState.Hold)
        {
            //Bounce code
            if (Projectile.velocity.X != oldVelocity.X)
                Projectile.velocity.X = -oldVelocity.X;
            if (Projectile.velocity.Y != oldVelocity.Y)
                Projectile.velocity.Y = -oldVelocity.Y * 0.2f;

            if (!_playedBounceSound)
            {
                SoundStyle bounceStyle = SoundID.DD2_WitherBeastCrystalImpact;
                bounceStyle.PitchVariance = 0.15f;
                SoundEngine.PlaySound(bounceStyle, Projectile.position);
                _playedBounceSound = true;
            }
        }

        return false;
    }
    
    float GetTrailWidth(float ratio)
    {
        return MathHelper.SmoothStep(24, 0, ratio) * MathHelper.Lerp(1f, 0f, EasingFunction.InSine(SwingRatio));
    }

    Color GetTrailColor(float ratio)
    {
        return Color.Lerp(Color.White, Color.Lerp(Color.DarkGreen, Color.Transparent, ratio), ratio) * MathHelper.Lerp(1f, 0f, SwingRatio);
    }

    void DrawPixelatedSlashTrail(GraphicsDevice gDevice)
    {
        if (Projectile.HammerState == MeteorHammerState.Hold)
            return;


        Vector2[] swingPoints = CoolSwingHandler.TrailCacheBuffer;
        Func<float, Vector2> pointFunction = Projectile.HammerState == MeteorHammerState.SwingOut ? GetSwingPoint : GetHelicopterSwingPoint;
        var subtract = -0.002f;
        DrawUtilities.FillSwingTrailPoints(ref swingPoints, pointFunction, SwingRatio, subtract);
        DrawUtilities.DrawCoolSlashEffectTrail(swingPoints, Color.Lerp(Color.Gold, Color.White, 0.5f), Color.DarkGreen, GetTrailWidth, GetTrailColor);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        if(Projectile.HammerState != MeteorHammerState.Hold)
            PixelationManager.QueuePrimitivesDrawAction(DrawPixelatedSlashTrail);
        var chainDrawer = SpritebatchDrawer.FromTextureAsset(AssetReferences.Assets.Textures.SpringHills.Items.ThornedMeteorHammerChain.Asset, Projectile.Center);
        chainDrawer.scale *= Scale;
        var points = Chain.points;//DrawUtilities.CalculateMeteorHammerChainPositions(Projectile.PlayerOwner.Center, Projectile.Center, chainDrawer.Height);
        DrawUtilities.DrawChain(Main.spriteBatch, ref points, chainDrawer);

        var drawer = Projectile.Drawer;
        drawer.scale *= Scale;
        Main.spriteBatch.Draw(drawer);

        drawer.VerticalFrame(1, Main.projFrames[Type]);
        drawer.color = Color.Lerp(Color.Transparent, Color.LightGreen, _outlineAlpha);
        Main.spriteBatch.Draw(drawer);
        return false;
        //return base.PreDraw(ref lightColor);
    }

    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
    }
}
