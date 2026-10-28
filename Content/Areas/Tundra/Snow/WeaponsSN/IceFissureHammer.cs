using Stellamod.Common;
using Stellamod.Common.MeleeSwingSystem;
using Stellamod.Common.Particles;
using Stellamod.Common.Shaders;
using Stellamod.Content.CommonMaterials;
using Stellamod.Core.Bases;
using Stellamod.Core.Pixelation;
using Stellamod.Items;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Tundra.Snow.WeaponsSN;


public class IceFissureHammer : ModItem
{
    public override string Texture => AssetReferences.Assets.Textures.Ice.Items.IceFissureHammer.KEY;
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.DefaultToCoolSwingItem<
            IceFissureHammerSlash,
            IceFissureHammerStaminaSlash>(MeleeWeaponType.Hammer);
        Item.damage = 6;
        Item.StaminaDamageMultiplier=1.5f;
    }
    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        CoolSwingHelper.DoBasicOrStaminaSwing(Item, player, source, position, velocity, type, damage, knockback);
        return false;
        //return base.Shoot(player, source, position, velocity, type, damage, knockback);
    }
    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew(
            mold: ModContent.ItemType<BlankSword>(),
            material: ModContent.ItemType<WinterbornShard>());
    }
}

public class IceFissureSpikeSpawner : ModProjectile
{
    ref float Timer => ref Projectile.ai[0];
    ref float AttackIndex => ref Projectile.ai[1];
    ref float StaminaPowered => ref Projectile.ai[2];
    bool IsNormalSwing => StaminaPowered == 1;
    public float strength;
    public override string Texture => TextureRegistry.EmptyTexture;
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 16;
        Projectile.height = 16;
        Projectile.timeLeft = 25;
        Projectile.extraUpdates = 1;
    }
    public override void AI()
    {
        base.AI();
        Timer++;

        if(StaminaPowered == 1)
        {
            Projectile.velocity *= 0.96f;
        }
        Projectile.velocity.Y = 0;
        var d2 = 7F;
        d2 /= 2F;
        d2 *= MathHelper.Lerp(1f, 2f, 1f - strength);
        if (Timer >= d2)
        {
            if (AttackIndex == 0)
            {
                var fx = FXUtil.GlowCircleBoom(Projectile.Center, Color.White, Color.SkyBlue, Color.DarkBlue, 20);
                fx.Scale *= 1.5f;

                var fac = ParticleUtils.ParticleFactory.FromSmallBurst(Projectile.Center, TriColorPalette.Icey, new Vector2(3, 7));
                fac.particleCount *= 4;
                ParticleUtils.CreateSwirlingDustCircle(fac);
            }

            Timer = 0;
            var vel = Projectile.velocity.Resize(AttackIndex * 64);
            var spawnPos = Projectile.Center + new Vector2(0, -64);
            spawnPos = TileUtilities.FallToSolidTileOrPlatform(spawnPos);

            var firer = ProjFirer.From<IceFissureSpikeBig>(Projectile);
            if (IsNormalSwing)
            {
                firer = ProjFirer.From<IceFissureSpikeBig>(Projectile);
            }
            else
            {
                if(Projectile.timeLeft <= 3)
                {
                    firer = ProjFirer.From<IceFissureSpikeBigBig>(Projectile);
                } 
                else if (Projectile.timeLeft < 12)
                {
                    firer = ProjFirer.From<IceFissureSpike>(Projectile);
                }
                else
                {
                    firer = ProjFirer.From<IceFissureSpikeBig>(Projectile);
                }
            }
             
            firer.position = spawnPos;
            firer.velocity = Projectile.velocity;
            var d = firer.NewDirect();
            if(d is IceFissureSpikeBig big)
            {
                big.strength = strength + (1f - ((float)Projectile.timeLeft / 25f));
            }
            if (d is IceFissureSpikeBigBig big2)
            {
                big2.strength = strength * 0.75f;
            }
            AttackIndex++;
        }

    }
    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        return false;
    }
    public override bool ShouldUpdatePosition()
    {
        return true;
        //  return base.ShouldUpdatePosition();
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
public class IceFissureSpike : ModProjectile
{
    Vector2 Scale
    {
        get
        {
            var lerpOne = Vector2.Lerp(Vector2.Zero, new Vector2(1f), EasingFunction.OutExpo(Timer / InTime));
            var lerpTow = Vector2.Lerp(Vector2.One, new Vector2(0.75f, 0.75f), EasingFunction.InOutSine(Timer / Time));
            var lerpThree = Vector2.Lerp(lerpOne, lerpTow, Timer / Time);
            return lerpThree;
        }
    }
    float Time => 36;
    float InTime => 12;
    ref float Timer => ref Projectile.ai[0];
    ref float RandScale => ref Projectile.ai[1];
    ref float RandDirection => ref Projectile.ai[2];
    public override string Texture => AssetReferences.Assets.Textures.Ice.Items.IceFissureSpike.KEY;
    public override void OnSpawn(IEntitySource source)
    {
        base.OnSpawn(source);
        RandScale = Main.rand.NextFloat(0.4f, 1f);
        RandDirection = Main.rand.NextBool(2) ? -1 : 1;
    }

    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.LocalHitOnce = true;
        Projectile.friendly = true;
        Projectile.tileCollide = false;
        Projectile.width = 64;
        Projectile.height = 256;
        Projectile.light = 0.5f;
        Projectile.ignoreWater = true;
        Projectile.timeLeft = (int)Time;
    }

    public override void AI()
    {
        base.AI();
        Projectile.direction = (int)RandDirection;
        Projectile.spriteDirection = (int)RandDirection;
        Timer++;
        if (Timer == 1)
        {
            if (this.OwnedByLocalClient())
            {
                for (float f = 0; f < 2f; f++)
                {
                    Vector2 upVelocity = -Vector2.UnitY;
                    upVelocity *= Main.rand.NextFloat(7, 15);
                    upVelocity = upVelocity.RotatedByRandom(MathHelper.ToRadians(75));
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, upVelocity,
                        ModContent.ProjectileType<IceCrashCubeShard>(), (int)(Projectile.damage * 0.3f), Projectile.knockBack, Projectile.owner);
                }
            }

            for (var i = 0; i < 32; i++)
            {
                var pos = Projectile.Center;
                pos.X += Main.rand.NextFloat(-32, 32f);
                var vel = -Vector2.UnitY * Main.rand.NextFloat(5, 15f);
                Dust.NewDustPerfect(pos, DustID.IceTorch, vel, Scale: 1.5f);
            }
            var fac = ParticleUtils.ParticleFactory.FromSmallBurst(Projectile.Center, Projectile.Center - new Vector2(0, 5), TriColorPalette.Icey, new Vector2(5, 15));
            ParticleUtils.CreateSwirlingDustBurst(fac);

            FXUtil.ShakeCamera(Projectile.Center, 1024, 8);

            var rsound = AssetReferences.Assets.Sounds.Melee.HammerSmash.Asset with { PitchVariance = 0.5f };
            SoundEngine.PlaySound(rsound, Projectile.position);
        }
    }

    public override bool ShouldUpdatePosition()
    {
        return false;
        //    return base.ShouldUpdatePosition();
    }


    public override bool PreDraw(ref Color lightColor)
    {
        var sb = Main.spriteBatch;
        var drawer = Projectile.Drawer;
        drawer.BottomCenterOrigin();
        drawer.scale = Scale * RandScale;
        drawer.scale.X *= 0.75f;
        sb.Draw(drawer);
        return false;
    }
    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
        var fac = ParticleUtils.ParticleFactory.FromSmallBurst(Projectile.Center, TriColorPalette.Icey, new Vector2(12, 30));
        fac.scaleRange *= 0.35f;
        fac.particleCount *= 2;
        ParticleUtils.CreateSwirlingDustBurst(fac);

        var sound = AssetReferences.Assets.Sounds.Melee.HammerHit.Asset with { PitchVariance = 0.5f, Volume = 0.25f };
        SoundEngine.PlaySound(sound, Projectile.position);
    }
}

public class IceFissureSpikeBig : ModProjectile
{
    Vector2 Scale
    {
        get
        {
            var lerpOne = Vector2.Lerp(Vector2.Zero, new Vector2(1f), EasingFunction.OutExpo(Timer / InTime));
            var lerpTow = Vector2.Lerp(Vector2.One, new Vector2(0.75f, 0.75f), EasingFunction.InOutSine(Timer / Time));
            var lerpThree = Vector2.Lerp(lerpOne, lerpTow, Timer / Time);
            return lerpThree;
        }
    }
    float Time => 54;
    float InTime => 12;
    ref float Timer => ref Projectile.ai[0];
    public float strength;
    public override string Texture => AssetReferences.Assets.Textures.Ice.Items.IceFissureSpikeSmall.KEY;
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.LocalHitOnce = true;
        Projectile.friendly = true;
        Projectile.tileCollide = false;
        Projectile.width = 100;
        Projectile.height = 64;
        Projectile.light = 0.5f;
        Projectile.ignoreWater = true;
        Projectile.timeLeft = (int)Time;
    
    }

    public override void AI()
    {
        base.AI();
        Timer++;
        if (Timer == 1)
        {
            if (this.OwnedByLocalClient() && Main.rand.NextBool(3))
            {
                for (float f = 0; f < 1f; f++)
                {
                    Vector2 upVelocity = -Vector2.UnitY;
                    upVelocity *= Main.rand.NextFloat(7, 15);
                    upVelocity = upVelocity.RotatedByRandom(MathHelper.ToRadians(75));
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, upVelocity,
                        ModContent.ProjectileType<IceCrashCubeShard>(), (int)(Projectile.damage * 0.3f), Projectile.knockBack, Projectile.owner);
                }
            }
            for (var i = 0; i < 8; i++)
            {
                var pos = Projectile.Center;
                pos.X += Main.rand.NextFloat(-32, 32f);
                var vel = -Vector2.UnitY * Main.rand.NextFloat(5, 15f);
                Dust.NewDustPerfect(pos, DustID.IceTorch, vel, Scale: 1.5f);
            }
            var fac = ParticleUtils.ParticleFactory.FromSmallBurst(Projectile.Center, Projectile.Center - new Vector2(0, 5), TriColorPalette.Icey, new Vector2(5, 15));
            fac.particleCount *= 1;
            ParticleUtils.CreateSwirlingDustBurst(fac);

            FXUtil.ShakeCamera(Projectile.Center, 1024, 8);

            var rsound = AssetReferences.Assets.Sounds.Melee.HammerSmash.Asset with { PitchVariance = 0.5f };
            SoundEngine.PlaySound(rsound, Projectile.position);
        }
    }

    public override bool ShouldUpdatePosition()
    {
        return false;
        //    return base.ShouldUpdatePosition();
    }


    public override bool PreDraw(ref Color lightColor)
    {
        var sb = Main.spriteBatch;
        var drawer = Projectile.Drawer;
        drawer.texture = AssetReferences.Assets.Textures.Ice.Items.IceFissureSpikeSmall.Asset.Value;
        drawer.BottomCenterOrigin();
        drawer.scale = Scale * strength;
        drawer.scale.X *= 0.75f;
        drawer.spriteEffects = Projectile.velocity.X < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
        sb.Draw(drawer);
        return false;
    }
    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
        var fac = ParticleUtils.ParticleFactory.FromSmallBurst(Projectile.Center, TriColorPalette.Icey, new Vector2(12, 30));
        fac.scaleRange *= 0.35f;
        fac.particleCount *= 1;
        ParticleUtils.CreateSwirlingDustBurst(fac);

        var sound = AssetReferences.Assets.Sounds.Illuria.IceImpact.Asset with { PitchVariance = 0.5f, Volume = 0.15f };
        SoundEngine.PlaySound(sound, Projectile.position);
    }
}
public class IceFissureSpikeBigBig : ModProjectile
{
    Vector2 Scale
    {
        get
        {
            var lerpOne = Vector2.Lerp(Vector2.Zero, new Vector2(1f), EasingFunction.OutExpo(Timer / InTime));
            var lerpTow = Vector2.Lerp(Vector2.One, new Vector2(0.75f, 0.75f), EasingFunction.InOutSine(Timer / Time));
            var lerpThree = Vector2.Lerp(lerpOne, lerpTow, Timer / Time);
            return lerpThree;
        }
    }
    float Time => 54;
    float InTime => 12;
    ref float Timer => ref Projectile.ai[0];
    public float strength;
    public override string Texture => AssetReferences.Assets.Textures.Ice.Items.IceFissureSpikeBig.KEY;
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.LocalHitOnce = true;
        Projectile.friendly = true;
        Projectile.tileCollide = false;
        Projectile.width = 100;
        Projectile.height = 256;
        Projectile.light = 0.5f;
        Projectile.ignoreWater = true;
        Projectile.timeLeft = (int)Time;

    }

    public override void AI()
    {
        base.AI();
        Timer++;
        if (Timer == 1)
        {
            if (this.OwnedByLocalClient() && Main.rand.NextBool(3))
            {
                for (float f = 0; f < 1f; f++)
                {
                    Vector2 upVelocity = -Vector2.UnitY;
                    upVelocity *= Main.rand.NextFloat(7, 15);
                    upVelocity = upVelocity.RotatedByRandom(MathHelper.ToRadians(75));
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, upVelocity,
                        ModContent.ProjectileType<IceCrashCubeShard>(), (int)(Projectile.damage * 0.3f), Projectile.knockBack, Projectile.owner);
                }
            }
            for (var i = 0; i < 8; i++)
            {
                var pos = Projectile.Center;
                pos.X += Main.rand.NextFloat(-32, 32f);
                var vel = -Vector2.UnitY * Main.rand.NextFloat(5, 15f);
                Dust.NewDustPerfect(pos, DustID.IceTorch, vel, Scale: 1.5f);
            }
            var fac = ParticleUtils.ParticleFactory.FromSmallBurst(Projectile.Center, Projectile.Center - new Vector2(0, 5), TriColorPalette.Icey, new Vector2(5, 15));
            fac.particleCount *= 1;
            ParticleUtils.CreateSwirlingDustBurst(fac);

            FXUtil.ShakeCamera(Projectile.Center, 1024, 8);

            var rsound = AssetReferences.Assets.Sounds.Melee.HammerSmash.Asset with { PitchVariance = 0.5f };
            SoundEngine.PlaySound(rsound, Projectile.position);
        }
    }

    public override bool ShouldUpdatePosition()
    {
        return false;
        //    return base.ShouldUpdatePosition();
    }


    public override bool PreDraw(ref Color lightColor)
    {

        var sb = Main.spriteBatch;
        var drawer = Projectile.Drawer;
        drawer.texture = AssetReferences.Assets.Textures.Ice.Items.IceFissureSpikeBig.Asset.Value;
        drawer.BottomCenterOrigin();
        drawer.scale = Scale * strength;
        drawer.scale.X *= 0.75f;
        drawer.spriteEffects = Projectile.velocity.X < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
        sb.Draw(drawer);
        return false;
    }
    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
        var fac = ParticleUtils.ParticleFactory.FromSmallBurst(Projectile.Center, TriColorPalette.Icey, new Vector2(12, 30));
        fac.scaleRange *= 0.35f;
        fac.particleCount *= 1;
        ParticleUtils.CreateSwirlingDustBurst(fac);

        var sound = AssetReferences.Assets.Sounds.Illuria.IceImpact.Asset with { PitchVariance = 0.5f, Volume = 0.15f };
        SoundEngine.PlaySound(sound, Projectile.position);
    }
}

public class IceFissureHammerStaminaSlash : IceFissureHammerSlash
{
    public override bool StaminaPowered => true;
}

public class IceFissureHammerSlash : ModProjectile
{
    bool _isGrounded;
    public virtual bool StaminaPowered => false;
    float _deathTimer;
    float _chargeTimer;
    bool _spawnwedProj;
    bool _chargedSound;
    const int WIND_UP_SWING = 0;
    const int COME_DOWN_SWING = 1;
    CoolSwingHandler SwingHandler
    {
        get
        {
            if (field == null)
            {
                field = new();
                //Initialize swings here
                field.Add(new()
                {
                    movement = new Swings.ChargeBackSwing
                    {
                        arcBackRadians = StaminaPowered ? 1.5f : 0.9f,
                        holdDistance = 64
                    },
                    fixRot = true,
                    swingTime = StaminaPowered ? 110 : 97
                });
                field.Add(new()
                {
                    movement = new Swings.OvalMotionSwing
                    {
                        arcRadians = 5.2F,
                        xRadius = 64,
                        yRadius = 64,
                        easingFunction = EasingFunction.InOutExpo,
                    },
                    swingTime = 28,

                });
            }
            return field;
        }
    }
    public float strength;
    public override string Texture => AssetReferences.Assets.Textures.Ice.Items.IceFissureHammer.KEY;
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        Projectile.SetTrailCacheLength(16);
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.DefaultToCoolSwing(SwingHandler);
        Projectile.width = 36;
    }

    public override void AI()
    {
        base.AI();

        if (Projectile.PlayerOwner.direction == -1)
        {
            Projectile.CoolSwingDirection = -1;
        }
        if (_spawnwedProj)
        {
            Projectile.CoolSwingTimer--;
            ShakeScreenPosition.Shake = 3;
            _deathTimer++;
            if (_deathTimer >= 12)
            {
                Projectile.Kill();
            }
        }

        var dir = Projectile.velocity.X > 0 ? 1 : -1;
        Projectile.direction = dir;
        Projectile.velocity.Y = 0;

        Projectile.PlayerOwner.ChangeDir(dir);

        SwingHandler.ApplyTo(Projectile, allowKill: false);

        if (StaminaPowered)
        {
            if (Projectile.CoolSwingComboIndex == WIND_UP_SWING && SwingHandler.GetInterpolant(Projectile) >= 1f)
            {
                var firer = SwingHandler.KillThisSwingGetNext(Projectile);
                if (this.OwnedByLocalClient())
                {

                    firer.damage = (int)(SwingHandler.GetInterpolant(Projectile) * Projectile.damage * 2);
                    firer.New();
                }
            }
        }
        else
        {
            if (Projectile.CoolSwingComboIndex == WIND_UP_SWING && !Projectile.PlayerOwner.channel)
            {
                var interp = SwingHandler.GetInterpolant(Projectile);
                interp = MathHelper.Clamp(interp, 0f, 1f);
                var firer = SwingHandler.KillThisSwingGetNext(Projectile);
                if (this.OwnedByLocalClient() && interp >= 0.5f)
                {

                    firer.damage = (int)(interp * Projectile.damage * 2);
                    var d = firer.NewDirect();
                    if (d is IceFissureHammerSlash slash)
                    {
                        slash.strength = interp;
                    }
                }
            }
        }


        if (Projectile.CoolSwingComboIndex == WIND_UP_SWING && SwingHandler.GetInterpolant(Projectile) < 1f)
        {
            if (Main.rand.NextBool(2))
            {
                var pos = Projectile.Center + Main.rand.NextVector2Circular(256, 256);
                var vel = (Projectile.Center - pos).Resize(7);
                Particles.BitDust.Spawn(BitDustFactory.SlowingOverTime with
                {
                    position = pos,
                    velocity = vel,
                    timeLeft = 60,
                    innerColor = Color.White.ToVector4(),
                    outerColor = Color.Cyan.ToVector4(),
                    scale = new Vector2(Main.rand.NextFloat(0.3f, 0.7f))
                });
            }

        }

        if (!_chargedSound && Projectile.CoolSwingComboIndex == WIND_UP_SWING && SwingHandler.GetInterpolant(Projectile) >= 1f)
        {
            _chargeTimer = 45;
            FXUtil.GlowCircleBoom(Projectile.Center, Color.White, Color.SkyBlue, Color.DarkBlue, 20);

            var fac = ParticleUtils.ParticleFactory.FromSmallBurst(Projectile.Center, TriColorPalette.Icey, new Vector2(5, 15));
            ParticleUtils.CreateSwirlingDustBurst(fac);

            var sound = AssetReferences.Assets.Sounds.CyroBolt.Asset with { PitchVariance = 0.5f, Volume = 0.5f };
            SoundEngine.PlaySound(sound, Projectile.position);
            _chargedSound = true;
        }

        if (_chargeTimer > 0)
        {
            _chargeTimer--;
        }


        var playerPoint = Projectile.PlayerOwner.Top;
        var tileplayerPoint = TileUtilities.FallToSolidTileOrPlatform(playerPoint);
        var dist = Vector2.Distance(playerPoint, tileplayerPoint);
        var t = Projectile.Bottom.ToTileCoordinates();

        Tile tiile = Main.tile[t];
        if (WorldGen.SolidTile(tiile) ||
            WorldGen.SolidTile(Main.tile[Projectile.Top.ToTileCoordinates()]) ||
            Main.tileSolidTop[tiile.type] || dist <= 64)
        {
            _isGrounded = true;
        }

        if (Projectile.CoolSwingComboIndex == COME_DOWN_SWING && !_spawnwedProj)
        {
            var swinger = SwingHandler.GetSwinger(Projectile);
            swinger.direction = -Projectile.PlayerOwner.direction;
            var interp = SwingHandler.GetInterpolant(Projectile);
            if (interp >= 0.52f)
            {
                _chargeTimer = 45;


                if (this.OwnedByLocalClient() && _isGrounded)
                {
                    //Create fissure
                    var firer = ProjFirer.From<IceFissureSpikeSpawner>(Projectile);
                    firer.ai0 = 35;
                    firer.ai2 = StaminaPowered ? 0 : 1;
                    firer.position = TileUtilities.FallToSolidTileOrPlatform(firer.position + new Vector2(Projectile.PlayerOwner.direction * 24, -64));
                    firer.position.Y -= 32;
                    firer.velocity = Projectile.velocity.Resize(12);
                    var p = firer.NewDirect();
                  if(p is IceFissureSpikeSpawner spawner)
                    {
                        spawner.strength = strength;
                        if (StaminaPowered)
                            spawner.strength = 1f;
                    }   
                }

                _spawnwedProj = true;

            }
        }
    }
    public float GetTrailWidth(float ratio)
    {
        return MathHelper.SmoothStep(24, 12, ratio) * MathHelper.SmoothStep(1f, 0f, _deathTimer / 12f);
    }
    public Color GetTrailColor(float ratio)
    {
        return DrawUtilities.InterpolateColorArray(ratio, Color.White, Color.White, Color.Gray, Color.SkyBlue, Color.White) * 1.4f * MathHelper.SmoothStep(1f, 0f, ratio);
    }
    void DrawPixelatedTrail(GraphicsDevice gDevice)
    {
        var trail = CoolSwingHandler.TrailCacheBuffer;
        SwingHandler.CalculateTrailingPoints(Projectile, ref trail);
        var pass = AssetReferences.Effects.Abyss.SingularTrail.CreatePrimitivesPass();
        pass.Parameters.transformMatrix = TrailDrawer.WorldViewPoint2;
        pass.Parameters.insideColor = Color.DarkBlue.ToVector3();
        pass.Parameters.bloomColor = Color.White.ToVector3();
        //  pass.Parameters.time = Main.GlobalTimeWrappedHourly * 

        var hlsl = new HlslSampler();
        hlsl.Sampler = SamplerState.LinearWrap;
        hlsl.Texture = AssetReferences.Assets.LaserTextures.Beamlight.Asset.Value;
        pass.Parameters.laserSampler = hlsl;
        pass.Apply();

        TrailDrawer.Draw(trail, GetTrailColor, GetTrailWidth, pass.Shader, Projectile.PlayerOwner.Center);
    }
    public override bool PreDraw(ref Color lightColor)
    {
        if (Projectile.CoolSwingComboIndex == COME_DOWN_SWING)
        {
            PixelationManager.QueuePrimitivesDrawAction(DrawPixelatedTrail);
        }

        for (var i = 0; i < Projectile.oldPos.Length; i++)
        {
            ref var pos = ref Projectile.oldPos[i];
            var oldCenter = pos + Projectile.Size * 0.5f;
            var drawer = Projectile.CoolSwingDrawer;
            drawer.rotation = Projectile.oldRot[i];
            drawer.worldPosition = oldCenter;
            drawer.color = Color.Lerp(Color.White, Color.Transparent, i / (float)Projectile.oldPos.Length) * 0.05f;
            drawer.color.A = 0;
            Main.spriteBatch.Draw(drawer);
        }


        Main.spriteBatch.Draw(Projectile.CoolSwingDrawer);
        if (Projectile.CoolSwingComboIndex == WIND_UP_SWING)
        {

            var flashColor = Color.Lerp(Color.Transparent, Color.White, ExtraMath.Osc(0.5f, 1f, speed: 16));
            var drawer = Projectile.CoolSwingDrawer;
            drawer.texture = AssetReferences.Assets.Textures.Ice.Items.IceFissureHammerWhite.Asset.Value;
            drawer.color = Color.Lerp(Color.Transparent, flashColor, SwingHandler.GetInterpolant(Projectile));
            Main.spriteBatch.Draw(drawer);


            drawer.color *= _chargeTimer / 45f;
            drawer.color.A = 0;
            Main.spriteBatch.Draw(drawer);
        }
        return false;
    }
}












