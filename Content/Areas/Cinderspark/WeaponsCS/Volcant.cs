using Stellamod.Common.Particles;
using Stellamod.Common.ShockCircleSystem;
using Stellamod.Core;
using Stellamod.Core.Bases;
using Stellamod.Core.Particles;
using Stellamod.Core.SwingSystem;
using Stellamod.Visual.Particles;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Cinderspark.WeaponsCS;

public class Volcant : BaseSwingItemV2
{
    public override void SetDefaults2()
    {
        base.SetDefaults2();
        Item.damage = 7;
        Item.ArmorPenetration = 15;
        Item.shoot = ModContent.ProjectileType<VolcantSlash>();
        staminaProjectileShoot = ModContent.ProjectileType<VolcantStaminaSlash>();
        meleeWeaponType = MeleeWeaponType.Greatsword;
        staminaDamageMultiplier = 3;
    }
}

public class VolcantStaminaSlash : BaseSwingProjectileV2
{
    float _hitDir;
    float _flashTimer;
    float _frameCounter;
    int _frame;
    bool _hit;
    bool _balls;
    bool _grounded;
    public override void SetDefaults2()
    {
        base.SetDefaults2();
        Projectile.hide = true;
    }
    public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
    {
        base.DrawBehind(index, behindNPCsAndTiles, behindNPCs, behindProjectiles, overPlayers, overWiresUI);
        behindNPCsAndTiles.Add(index);
    }
    public override void DefineCombo()
    {
        base.DefineCombo();
        Add(new SwordSwing
        {
            sound = AssetReferences.Assets.Sounds.Melee.ChainSawSwing.Asset with { PitchVariance = 0.6f, Volume = 1f },
            baseSwingTime = 100,
            hitCount = 8,
            swordMovementFunction = new Swings.GroundSwipeSwing
            {
                arcRadians = 3.5f,
                thrustDistance = 96
            },
        });

        hitStopTime = EXTRA_UPDATE_COUNT * 7;
        useAfterImage = true;

        glowAfterImageColor = Color.Red * 0.1f;
        glowAfterImageColor = Color.OrangeRed;
    }

    public override void RenderSwingTrail(ref Color lightColor, Vector2[] points)
    {
        base.RenderSwingTrail(ref lightColor, points);
    }

    public override void AI()
    {
        base.AI();
        if (_flashTimer > 0)
        {
            _flashTimer -= 0.5f;
        }
        _frameCounter += 0.3f;
        while (_frameCounter >= 1f)
        {
            _frameCounter -= 0.3f;
            _frame++;
            _frame %= 4;
        }

        if(Timer == 1)
        {
            _hitDir = MathF.Sign(Projectile.velocity.X);
            SwingDirection = _hitDir;

            SetSwingDirection();
            Projectile.velocity = Vector2.UnitY.RotatedBy(_hitDir * 0.3f);
        }
        var playerPoint = Owner.Center;
        var tileplayerPoint = TileUtilities.FallToSolidTile(playerPoint);
        var dist = Vector2.Distance(playerPoint, tileplayerPoint);
        var t = Projectile.Bottom.ToTileCoordinates();

        Tile tiile = Main.tile[t];
        if(WorldGen.SolidTile(tiile) || WorldGen.SolidTile(Main.tile[Projectile.Top.ToTileCoordinates()]))
        {
            _grounded = true;
        }
        if(_grounded && Interpolant >= 0.6f && !_balls )
        {
            _balls = true;
            if (this.OwnedByLocalClient())
            {
                var eruptor = ProjFirer.From<VolcantEruption>(Projectile);
                eruptor.position = Projectile.Center;
                eruptor.position.Y = Owner.Center.Y;
                eruptor.position.Y -= 32;

                eruptor.position = TileUtilities.FallToSolidTile(eruptor.position);
                eruptor.position.Y -= 128;
                eruptor.velocity = -Vector2.UnitY.RotatedBy(_hitDir * 0.6f);
                eruptor.New();
            }
            
        }


        glowColor = Color.Lerp(Color.Transparent, Color.Red, EasingFunction.QuadraticBump(Interpolant) * 0.2f);
        growScale = MathHelper.Lerp(0f, 0.15f, EasingFunction.QuadraticBump(Interpolant));
        if (Main.rand.NextBool(8))
        {
            Vector2 spawnPos = Projectile.Center + Main.rand.NextVector2Circular(32, 32);
            FaintSmokeParticle sp = FaintSmokeParticle.SpawnInAlphaLayer(spawnPos, Vector2.Zero);
            sp.color = Color.Lerp(Color.Lerp(Color.Black, Color.DarkViolet, 0.15f), Color.Black, Main.rand.NextFloat(0f, 1f)) * 0.25f;
            sp.Scale *= 0.48f;
            sp.behindLayer = true;
        }
    }

    public override void PostDrawSword(Vector2 position, Rectangle srcRect, Color drawColor, float rotation, Vector2 origin, Vector2 drawScale, SpriteEffects spriteEffect, float layerDepth)
    {
        base.PostDrawSword(position, srcRect, drawColor, rotation, origin, drawScale, spriteEffect, layerDepth);
        var drawer = SpritebatchDrawer.FromTextureAsset(AssetReferences.Content.Areas.Cinderspark.WeaponsCS.Volcant_Sawblade.Asset, Projectile.Center);
        var alpha = MathHelper.Lerp(0f, 1f, _flashTimer / 10f);
        drawer.VerticalFrame(_frame, 4);
        drawer.LeftCenterOrigin();
        drawer.rotation = Projectile.rotation - MathHelper.PiOver4;
        drawer.color = Color.Lerp(Color.OrangeRed, Color.Red, ExtraMath.Osc(0f, 1f, speed: 6));
        drawer.color = Color.Lerp(drawer.color, Color.Yellow, alpha);
        drawer.color.A = 0;
        Main.spriteBatch.Draw(drawer);

        drawer.scale *= 1.2f;
        drawer.color *= 0.5f;
        Main.spriteBatch.Draw(drawer);
    }

    void RevHit()
    {
        if (!_hit)
        {
            FXUtil.ShakeCamera(Projectile.Center, 1024, 8);
            FXUtil.GlowCircleBoom(Projectile.Center, Color.OrangeRed, Color.Red, Color.DarkRed, duration: 24, baseSize: 0.16f);

            _hit = true;
        }
        _flashTimer = 10;
        for (var i = 0; i < 2; i++)
        {
            var pos = Projectile.Center;
            pos += Main.rand.NextVector2Circular(32, 32);
            var vel = (pos - Projectile.Center);
            vel = vel.SafeNormalize(Vector2.Zero);
            vel *= Main.rand.NextFloat(5f, 45f);
            vel = vel.RotatedByRandom(1.3f);
            var fx = FXUtil.GlowStretch(pos, vel);
            fx.InnerColor = Color.Red;
            fx.OuterGlowColor = Color.DarkRed;
            fx.VectorScale.Y *= 0.4f;
        }

        FXUtil.ShakeCamera(Projectile.Center, 1024, 3);
        for (var i = 0; i < 4; i++)
        {
            var pos = Projectile.Center + Main.rand.NextVector2Circular(32, 32);
            var vel = Main.rand.NextVector2Circular(15, 15);
            Particles.SwirlingFlameDust.Spawn(BitDustFactory.SlowingOverTime with
            {
                position = pos,
                velocity = vel,
                innerColor = Color.OrangeRed.ToVector4(),
                outerColor = Color.DarkRed.ToVector4(),
                scale = new Vector2(Main.rand.NextFloat(0.5f, 1.2f)),
                timeLeft = Main.rand.NextFloat(30, 90)
            });
        }
    }
    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        base.OnHitNPC(target, hit, damageDone);
     

        target.AddBuff(BuffID.OnFire, 120);
    }

    public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
    {
        base.ModifyHitNPC(target, ref modifiers);
        if (IsFinishingSwing())
        {
            DamageHelper.PercentIncreasedamage(ref modifiers, 0.5f);
        }
    }
}


public class VolcantMeteor : ModProjectile
{
    float _rotDir;
    ref float Timer => ref Projectile.ai[0];
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        Projectile.SetTrailCacheLength(16);
        Main.projFrames[Type] = 3;
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = Projectile.height = 36;
        Projectile.friendly = true;
        Projectile.timeLeft = 120;
        Projectile.tileCollide = false;
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
            SoundStyle hitSound = AssetRegistry.Sounds.Melee.Vinger2;
            hitSound.PitchVariance = 0.2f;
            SoundEngine.PlaySound(hitSound, Projectile.Bottom);
            Projectile.frame = Main.rand.Next(3);
            _rotDir = Main.rand.NextFloat(-1f, 1f);
        }
        if (Timer >= 100)
            Projectile.Kill();
        Projectile.velocity.Y += 0.35f;
        Projectile.rotation += 0.25f * _rotDir;
        var pos = Projectile.Center + Main.rand.NextVector2Circular(24, 24);
        var d = Dust.NewDustPerfect(pos, DustID.Torch, Main.rand.NextVector2Circular(2, 2), Scale: Main.rand.NextFloat(0.4f, 0.8f));
        d.noGravity = true;
    }

    public override bool PreDraw(ref Color lightColor)
    {
        var drawer = Projectile.Drawer;
        DrawUtilities.DrawAdditiveFadingTrail(Projectile, Color.Red, Color.Transparent, 0.3f);
        Main.spriteBatch.Draw(drawer);
        return false;
    }


    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
        for(var i = 0; i < 24; i++)
        {
            var pos = Projectile.Center + Main.rand.NextVector2Circular(24, 24);
            var vel = Main.rand.NextVector2Circular(16, 16);
            Particles.SwirlingFlameDust.Spawn(BitDustFactory.SlowingOverTime with
            {
                position = pos,
                velocity = vel,
                innerColor = Color.OrangeRed.ToVector4(),
                outerColor = Color.DarkRed.ToVector4(),
                timeLeft = Main.rand.Next(45, 100),
                scale = new Vector2(Main.rand.NextFloat(0.5f, 1.2f))
            });
        }
    }
}
public class VolcantEruption : ModProjectile
{
    public override string Texture => TextureRegistry.EmptyTexture;
    ref float Timer => ref Projectile.ai[0];
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 32;
        Projectile.height = 256;
        Projectile.friendly = true;
        Projectile.penetrate = -1;
        Projectile.tileCollide = false;
        Projectile.light = 0.6f;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = -1;
        Projectile.hide = true;
    }
    public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
    {
        base.DrawBehind(index, behindNPCsAndTiles, behindNPCs, behindProjectiles, overPlayers, overWiresUI);
        behindNPCsAndTiles.Add(index);
    }
    public void CrackVFX(Vector2 position)
    {
        Particles.CrackDust.Spawn(CrackImpactDust.Data.Default with { position = position, timeLeft = 200, color = Color.DarkOrange });
        Particles.CrackDust.Spawn(CrackImpactDust.Data.Default with { position = position, scale = 2f, timeLeft = 120, color = Color.DarkOrange });
    }
    public override bool ShouldUpdatePosition()
    {
        return false;
    }

    public override void AI()
    {
        base.AI();
        Timer++;
        if (Timer == 1)
        {
            var baseDir = Projectile.velocity.SafeNormalize(Vector2.Zero);
            var flameSound = AssetReferences.Assets.Sounds.Fire.FireballShoot.Asset with { PitchVariance = 1f, MaxInstances = 10 };
            SoundEngine.PlaySound(flameSound, Projectile.Bottom);
            for(var i = 0; i < 2; i++)
            {
                FXUtil.MakeSoilParticle(Projectile.Bottom + Main.rand.NextVector2Circular(32, 32), baseDir * Main.rand.NextFloat(5, 15f));
            }
            if (this.OwnedByLocalClient())
            {
                for(var i =0; i < 8; i++)
                {
                    var meteorFirer = ProjFirer.From<VolcantMeteor>(Projectile);
                    meteorFirer.velocity = baseDir * 15 * Main.rand.NextFloat(0.6f, 1.4f);
                    meteorFirer.velocity = meteorFirer.velocity.RotatedByRandom(0.6f);
                    meteorFirer.position = Projectile.Bottom;
                    meteorFirer.position += Main.rand.NextVector2Circular(48, 48);
                    meteorFirer.New();
                }
  
            }
     

            for (float f = 0; f < 4; f++)
            {
                var smoke = Particle<SmokeParticle>.SpawnInAlphaLayer(Projectile.Bottom, baseDir.RotatedByRandom(MathHelper.PiOver4) * Main.rand.NextFloat(1, 1f), Color.White, Scale: Main.rand.NextFloat(0.5f, 1f));
                smoke.initialColor = Color.DarkGray;
            }

            FXUtil.ShakeCamera(Projectile.Center, 1024, 8);
            CrackVFX(Projectile.Bottom);
            var fx = FXUtil.GlowCircleBoom(Projectile.Bottom, Color.Gold, Color.Red, Color.DarkRed, duration: 24, baseSize: 0.2f);
            fx.noRot = true;
            fx.VectorScale.X *= 0.8f;
            fx.VectorScale.Y *= 1.3f;
            for (var i = 0; i < 32; i++)
            {
                var pos = Projectile.Bottom;
                pos.X += Main.rand.NextFloat(-32f, 32f);
                pos.Y += Main.rand.NextFloat(-16f, 16f);
                var vel = baseDir * Main.rand.NextFloat(10f, 30);
                vel = vel.RotatedByRandom(0.5f);
                Particles.SwirlingFlameDust.Spawn(BitDustFactory.SlowingOverTime with
                {
                    position = pos,
                    velocity = vel,
                    innerColor = Color.OrangeRed.ToVector4(),
                    outerColor = Color.DarkRed.ToVector4(),
                    scale = new Vector2(Main.rand.NextFloat(0.4f, 1f)),
                    timeLeft = Main.rand.Next(30, 150)
                });
            }
            for (var i = 0; i < 32; i++)
            {
                var pos = Projectile.Bottom;
                pos.X += Main.rand.NextFloat(-32f, 32f);
                pos.Y += Main.rand.NextFloat(-16f, 16f);
                var vel = baseDir * Main.rand.NextFloat(10f, 20f);
                vel = vel.RotatedByRandom(1.5f);
                Dust.NewDustPerfect(pos, DustID.Torch, vel, Scale: Main.rand.NextFloat(0.5f, 1f));
            }
        }
    }
    public override bool PreDraw(ref Color lightColor)
    {
        return false;
        // return base.PreDraw(ref lightColor);
    }
    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
    }
}

public class VolcantThrow : ModProjectile
{
    Vector2 _startPos;

    float _flashTimer;
    float _frameCounter;
    int _frame;
    float _wiggle;
    bool _hit;
    enum AIState : byte
    {
        Out,
        Rev,
        Back
    }

    ref float Timer => ref Projectile.ai[0];
    AIState State
    {
        get => (AIState)Projectile.ai[1];
        set => Projectile.ai[1] = (float)value;
    }

    Player Owner => Main.player[Projectile.owner];
    public override string Texture => ModContent.GetInstance<Volcant>().Texture;
    public override void SendExtraAI(BinaryWriter writer)
    {
        base.SendExtraAI(writer);
        writer.Write(_hit);
    }
    public override void ReceiveExtraAI(BinaryReader reader)
    {
        base.ReceiveExtraAI(reader);
        _hit = reader.ReadBoolean();
    }
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        Projectile.SetTrailCacheLength(32);
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 100;
        Projectile.height = 100;
        Projectile.friendly = true;
        Projectile.timeLeft = 360;
        Projectile.penetrate = -1;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 7;
        Projectile.tileCollide = false;
    }

    public override void AI()
    {
        base.AI();
        Timer++;
        if (Timer % 8 == 0)
        {
            CreateFlameDust(Projectile.Center);
        }

        if (_flashTimer > 0)
        {
            _flashTimer -= 2f;
        }
        _frameCounter += 0.3f;
        while (_frameCounter >= 1f)
        {
            _frameCounter -= 0.3f;
            _frame++;
            _frame %= 4;
        }


        if (Main.rand.NextBool(8))
        {
            Vector2 spawnPos = Projectile.Center + Main.rand.NextVector2Circular(32, 32);
            FaintSmokeParticle sp = FaintSmokeParticle.SpawnInAlphaLayer(spawnPos, Vector2.Zero);
            sp.color = Color.Lerp(Color.Lerp(Color.Black, Color.DarkViolet, 0.15f), Color.Black, Main.rand.NextFloat(0f, 1f)) * 0.25f;
            sp.Scale *= 0.48f;
            sp.behindLayer = true;
        }

        switch (State)
        {
            case AIState.Out:
                AI_Out();
                break;
            case AIState.Rev:
                AI_Rev();
                break;
            case AIState.Back:
                AI_Back();
                break;
        }
        Owner.itemTime = 2;
        Owner.itemAnimation = 2;
    }
    void CreateFlameDust(Vector2 pos)
    {
        pos += Main.rand.NextVector2Circular(32, 32);
        var vel = Main.rand.NextVector2Circular(12, 12);
        Particles.SwirlingFlameDust.Spawn(BitDustFactory.SlowingOverTime with
        {
            position = pos,
            velocity = vel,
            innerColor = Color.OrangeRed.ToVector4(),
            outerColor = Color.DarkRed.ToVector4(),
            scale = new Vector2(Main.rand.NextFloat(0.5f, 1.2f)),
            timeLeft = Main.rand.NextFloat(30, 90)
        });
    }
    void SwitchState(AIState state)
    {
        Timer = 0;
        State = state;
        Projectile.netUpdate = true;
    }

    void AI_Out()
    {
        Projectile.extraUpdates = 2;
        Projectile.velocity *= 1.02f;
        Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;
        if (_hit)
        {
            SwitchState(AIState.Rev);
        }

        if (Timer >= 60)
        {
            SwitchState(AIState.Back);
        }
    }

    void AI_Rev()
    {
        if(Timer == 1)
        {
            var rev = AssetReferences.Assets.Sounds.Melee.ChainSawSwing.Asset with { PitchVariance = 0.6f, Volume = 1f };
            SoundEngine.PlaySound(rev, Projectile.position);
        }

        Projectile.extraUpdates = 0;
        if(Projectile.velocity.Length() > 0.3f)
            Projectile.velocity *= 0.7f;
        if (Main.rand.NextBool(8))
            _wiggle = Main.rand.NextFloat(-0.1f, 0.1f);
        if (Timer >= 60)
        {
            SwitchState(AIState.Back);
        }
    }
    void AI_Back()
    {
        if(Timer == 1)
        {
            _startPos = Projectile.Center;
            var fx = FXUtil.GlowCircleBoom(Projectile.Center, Color.Yellow, Color.Orange, Color.Red);
            fx.Scale *= 1.8f;
            ShockCircles.CreateQuickWhiteFlash(Projectile.Center);
            for(var i = 0; i < 16; i++)
            {
                var pos = Projectile.Center;
                var vel = Main.rand.NextVector2Circular(15, 15);
                var d = Dust.NewDustPerfect(pos, DustID.Torch, vel, Scale: Main.rand.NextFloat(0.5f, 1.2f));
                d.noGravity = true;
            }
        }

        Projectile.extraUpdates = 0;

        var dist = Vector2.Distance(Owner.Center, Projectile.Center);

        var time = 54;
        var ratio = Timer / time;
        
        var outV = Vector2.Lerp(_startPos, Owner.Center, ratio);
        var arcHeight = 212;
        var yUp = MathHelper.Lerp(0, -arcHeight, EasingFunction.OutExpo(ratio));
        var yDown = MathHelper.Lerp(-arcHeight, 0, EasingFunction.InExpo(ratio));
        var y = MathHelper.Lerp(yUp, yDown, ratio);
        var combinedPos = outV + new Vector2(0, y);
        Projectile.Center = combinedPos;
        Projectile.velocity *= 0;
        Projectile.rotation += 0.125f;
        
        if (Timer >= time)
        {
            PixelPrimitiveCircleFactory.CreateGenericBoom(Projectile.Center, Color.Yellow, Color.OrangeRed, 18, 64);
            Projectile.active = false;
        }
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        base.OnHitNPC(target, hit, damageDone);
        _flashTimer = 10;
        for (var i = 0; i < 2; i++)
        {
            var pos = target.Center;
            pos += Main.rand.NextVector2Circular(32, 32);
            var vel = (pos - Projectile.Center);
            vel = vel.SafeNormalize(Vector2.Zero);
            vel *= Main.rand.NextFloat(5f, 45f);
            vel = vel.RotatedByRandom(1.3f);
            var fx = FXUtil.GlowStretch(pos, vel);
            fx.InnerColor = Color.Red;
            fx.OuterGlowColor = Color.DarkRed;
            fx.VectorScale.Y *= 0.4f;
        }
        Projectile.velocity = Projectile.velocity.RotatedByRandom(0.03f);
  
        for (var i = 0; i < 4; i++)
        {
            var pos = target.Center + Main.rand.NextVector2Circular(32, 32);
            var vel = (target.Center - Projectile.Center).SafeNormalize(Vector2.Zero).RotatedByRandom(4.5f) * Main.rand.NextFloat(5f, 25);
            Particles.SwirlingFlameDust.Spawn(BitDustFactory.SlowingOverTime with
            {
                position = pos,
                velocity = vel,
                innerColor = Color.OrangeRed.ToVector4(),
                outerColor = Color.DarkRed.ToVector4(),
                scale = new Vector2(Main.rand.NextFloat(0.5f, 1.2f)),
                timeLeft = Main.rand.NextFloat(30, 90)
            });
        }

        if (!_hit)
        {
            ShockCircles.CreateQuickWhiteFlash(target.Center);
            FXUtil.ShakeCamera(target.Center, 1024, 4);
            Projectile.netUpdate = true;
            _hit = true;
        }
        else
        {
            FXUtil.ShakeCamera(target.Center, 1024, 3);
        }
    }
    public override bool PreDraw(ref Color lightColor)
    {
        var drawer = Projectile.Drawer;
        drawer.rotation += _wiggle;
        DrawUtilities.DrawAdditiveFadingTrail(Projectile, Color.Red, Color.Transparent, 0.3f);
        Main.spriteBatch.Draw(drawer);

        drawer = SpritebatchDrawer.FromTextureAsset(AssetReferences.Content.Areas.Cinderspark.WeaponsCS.Volcant_Sawblade.Asset, Projectile.Center);
        var alpha = MathHelper.Lerp(0f, 1f, _flashTimer / 10f);
        drawer.VerticalFrame(_frame, 4);
        drawer.LeftCenterOrigin();
        drawer.rotation = Projectile.rotation - MathHelper.PiOver4;
        drawer.color = Color.Lerp(Color.OrangeRed, Color.Red, ExtraMath.Osc(0f, 1f, speed: 6));
        drawer.color = Color.Lerp(drawer.color, Color.Yellow, alpha);
        drawer.color.A = 0;
        Main.spriteBatch.Draw(drawer);

        drawer.scale *= 1.2f;
        drawer.color *= 0.5f;
        Main.spriteBatch.Draw(drawer);
        return false;
    }
    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
    }
}

public class VolcantSlash : BaseSwingProjectileV2
{
    float _flashTimer;
    float _frameCounter;
    int _frame;
    bool _hit;
    public override void DefineCombo()
    {
        base.DefineCombo();
        Add(new SwordSwing
        {
            baseSwingTime = 62,
            swordMovementFunction = new Swings.CurlSwing
            {
                frequency = 6,
                swingRadians = 0.7f,
                xRange = 144
            },
            hitCount = 8,
            sound = AssetReferences.Assets.Sounds.Melee.ChainSawSwing.Asset with { PitchVariance = 0.6f, Volume = 1f },
        });

        Add(new SwordSwing
        {
            sound = AssetReferences.Assets.Sounds.Melee.ChainSawSwing.Asset with { PitchVariance = 0.6f, Volume = 1f },
            baseSwingTime = 62,
            hitCount = 8,
            swordMovementFunction = new Swings.ChainsawSwing
            {
                arcRadians = 0.15f,
                backupDistance = 128,
                thrustDistance = 164
            },
        });
        Add(new SwordSwing
        {
            baseSwingTime = 62,
            swordMovementFunction = new Swings.CurlSwing
            {
                frequency = 6,
                swingRadians = 0.7f,
                xRange = 144
            },
            hitCount = 8,
            sound = AssetReferences.Assets.Sounds.Melee.ChainSawSwing.Asset with { PitchVariance = 0.6f, Volume = 1f },
        });

        Add(new SwordSwing
        {
            sound = AssetReferences.Assets.Sounds.Melee.ChainSawSwing.Asset with { PitchVariance = 0.6f, Volume = 1f },
            baseSwingTime = 62,
            hitCount = 8,
            swordMovementFunction = new Swings.ChainsawSwing
            {
                arcRadians = 0.15f,
                backupDistance = 128,
                thrustDistance = 96
            },
        });

        Add(new SwordSwing
        {
            sound = AssetReferences.Assets.Sounds.Melee.ChainSawSwing.Asset with { PitchVariance = 0.6f, Volume = 1f },
            baseSwingTime = 62,
            hitCount = 8,
            swordMovementFunction = new Swings.ChainsawSwing
            {
                arcRadians = 0.15f,
                backupDistance = 128,
                thrustDistance = 96
            },
        });

        hitStopTime = EXTRA_UPDATE_COUNT * 7;
        useAfterImage = true;

        glowAfterImageColor = Color.Red * 0.1f;
        glowAfterImageColor = Color.OrangeRed;
    }

    public override void RenderSwingTrail(ref Color lightColor, Vector2[] points)
    {
        base.RenderSwingTrail(ref lightColor, points);
    }

    public override void AI()
    {
        base.AI();
        if (_flashTimer > 0)
        {
            _flashTimer -= 0.5f;
        }
        _frameCounter += 0.3f;
        while (_frameCounter >= 1f)
        {
            _frameCounter -= 0.3f;
            _frame++;
            _frame %= 4;
        }

        if (ComboIndex == 4 && Timer >= 100)
        {
            if (this.OwnedByLocalClient())
            {
                var firer = ProjFirer.From<VolcantThrow>(Projectile);
                firer.velocity = Projectile.velocity.Resize(7);
                firer.New();
            }
            Projectile.active = false;
        }


        glowColor = Color.Lerp(Color.Transparent, Color.Red, EasingFunction.QuadraticBump(Interpolant) * 0.2f);
        growScale = MathHelper.Lerp(0f, 0.15f, EasingFunction.QuadraticBump(Interpolant));
        if (Main.rand.NextBool(8))
        {
            Vector2 spawnPos = Projectile.Center + Main.rand.NextVector2Circular(32, 32);
            FaintSmokeParticle sp = FaintSmokeParticle.SpawnInAlphaLayer(spawnPos, Vector2.Zero);
            sp.color = Color.Lerp(Color.Lerp(Color.Black, Color.DarkViolet, 0.15f), Color.Black, Main.rand.NextFloat(0f, 1f)) * 0.25f;
            sp.Scale *= 0.48f;
            sp.behindLayer = true;
        }
    }

    public override void PostDrawSword(Vector2 position, Rectangle srcRect, Color drawColor, float rotation, Vector2 origin, Vector2 drawScale, SpriteEffects spriteEffect, float layerDepth)
    {
        base.PostDrawSword(position, srcRect, drawColor, rotation, origin, drawScale, spriteEffect, layerDepth);
        var drawer = SpritebatchDrawer.FromTextureAsset(AssetReferences.Content.Areas.Cinderspark.WeaponsCS.Volcant_Sawblade.Asset, Projectile.Center);
        var alpha = MathHelper.Lerp(0f, 1f, _flashTimer / 10f);
        drawer.VerticalFrame(_frame, 4);
        drawer.LeftCenterOrigin();
        drawer.rotation = Projectile.rotation - MathHelper.PiOver4;
        drawer.color = Color.Lerp(Color.OrangeRed, Color.Red, ExtraMath.Osc(0f, 1f, speed: 6));
        drawer.color = Color.Lerp(drawer.color, Color.Yellow, alpha);
        drawer.color.A = 0;
        Main.spriteBatch.Draw(drawer);

        drawer.scale *= 1.2f;
        drawer.color *= 0.5f;
        Main.spriteBatch.Draw(drawer);
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        base.OnHitNPC(target, hit, damageDone);
        if (!_hit)
        {
            FXUtil.ShakeCamera(target.Center, 1024, 8);
            FXUtil.GlowCircleBoom(target.Center, Color.OrangeRed, Color.Red, Color.DarkRed, duration: 24, baseSize: 0.16f);

            _hit = true;
        }
        _flashTimer = 10;
        for (var i = 0; i < 2; i++)
        {
            var pos = target.Center;
            pos += Main.rand.NextVector2Circular(32, 32);
            var vel = (pos - Projectile.Center);
            vel = vel.SafeNormalize(Vector2.Zero);
            vel *= Main.rand.NextFloat(5f, 45f);
            vel = vel.RotatedByRandom(1.3f);
            var fx = FXUtil.GlowStretch(pos, vel);
            fx.InnerColor = Color.Red;
            fx.OuterGlowColor = Color.DarkRed;
            fx.VectorScale.Y *= 0.4f;
        }
        Projectile.velocity = Projectile.velocity.RotatedByRandom(0.03f);
        HitstopTimer += 12;
        FXUtil.ShakeCamera(target.Center, 1024, 3);
        for (var i = 0; i < 4; i++)
        {
            var pos = target.Center + Main.rand.NextVector2Circular(32, 32);
            var vel = (target.Center - Projectile.Center).SafeNormalize(Vector2.Zero).RotatedByRandom(4.5f) * Main.rand.NextFloat(5f, 25);
            Particles.SwirlingFlameDust.Spawn(BitDustFactory.SlowingOverTime with
            {
                position = pos,
                velocity = vel,
                innerColor = Color.OrangeRed.ToVector4(),
                outerColor = Color.DarkRed.ToVector4(),
                scale = new Vector2(Main.rand.NextFloat(0.5f, 1.2f)),
                timeLeft = Main.rand.NextFloat(30, 90)
            });
        }

        target.AddBuff(BuffID.OnFire, 120);
    }

    public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
    {
        base.ModifyHitNPC(target, ref modifiers);
        if (IsFinishingSwing())
        {
            DamageHelper.PercentIncreasedamage(ref modifiers, 0.5f);
        }
    }
}