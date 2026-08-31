using ReLogic.Content;
using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Content.CommonMaterials;
using Stellamod.Content.Dusts;
using Stellamod.Core.Bases;
using Stellamod.Core.Effects.Trails;
using Stellamod.Core.Particles;
using Stellamod.Core.Pixelation;
using Stellamod.Core.SwingSystem;
using Stellamod.Core.Utilities;
using Stellamod.Helpers;
using Stellamod.Items;
using Stellamod.Trailing;
using Stellamod.Visual.Particles;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.PunkerTown.ItemsPT;

public class TheIrradiaspear : BaseSwingItemV2
{
    public override void SetStaticDefaults()
    {
        CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
    }

    public override void SetDefaults2()
    {
        base.SetDefaults2();
        Item.damage = 68;
        Item.width = 50;
        Item.height = 50;
        Item.useStyle = ItemUseStyleID.Swing;
        Item.noMelee = true;
        Item.noUseGraphic = true;
        Item.knockBack = 8;
        Item.autoReuse = false;
        Item.channel = true;
        Item.DamageType = DamageClass.Melee;
        Item.shoot = ModContent.ProjectileType<IrradiaspearSlash>();
        staminaProjectileShoot = ModContent.ProjectileType<TheIrradiaspearP>();
        meleeWeaponType = MeleeWeaponType.Spear;
        Item.shootSpeed = 15;
        Item.useAnimation = 20;
        Item.useTime = 20;
        staminaDamageMultiplier = 1.5f;
    }

    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew<MarshScrap, BlankSword>();
    }
}

public class IrradiaspearSlash : BaseSwingProjectileV2
{
    
    private bool _init;
    private bool _hit;
    private bool _didHitStop;
    private float _traveledRotation;
    private float _oldRot;
    public override void DefineCombo()
    {
        base.DefineCombo();
        SwingV2Helper.AddSpearSwingStyle2(this);
        swordBeamLength = 180;

        hitStopTime = EXTRA_UPDATE_COUNT * 8;
        glowAfterImageColor = Color.Green * 0.13f;
        outlineColor = Color.Green;
      //  useBloom = true;
        bloom.innerBloomColor = Color.White;
        bloom.outerBloomColor = Color.Violet;
        bloom.bloomWidthFunction = GetBloomWidth;
        bloom.bloomColorFunction = GetBloomColor;
        useAfterImage = true;
    }
    private Color GetTrailColor(float completionRatio)
    {
        return Color.Lerp(Color.Green, Color.LightGreen, EasingFunction.InCirc(completionRatio)) * MathHelper.Lerp(0f, 1f, EasingFunction.InCirc(completionRatio));
    }
    private float GetTrailWidth(float completionRatio)
    {
        if (Interpolant < 0.3f)
            return 0;
        return MathHelper.Lerp(0, 24, EasingFunction.InOutSine(completionRatio));
    }

    private float GetBloomWidth(float ratio)
    {
        return MathHelper.SmoothStep(0, 32, ratio) * 1.15f * MathHelper.Lerp(1f, 0f, EasingFunction.InExpo(Interpolant));
    }
    private Color GetBloomColor(float ratio)
    {
        return Color.Lerp(Color.LightGreen, Color.DarkBlue, ratio) * MathHelper.SmoothStep(0f, 1f, ratio) * 0.5f;
    }
    public override Asset<Texture2D> RequestHologramTexture()
    {
        return TextureRegistry.GlowSword_Irradiaspear;
    }

    public float WidthFunction(float completionRatio)
    {
        return MathHelper.SmoothStep(8, 3.5f, completionRatio);
    }

    public Color ColorFunction(float completionRatio)
    {
        return Color.Lerp(Color.Transparent, ColorFunctions.AcidFlame, EasingFunction.QuadraticBump(completionRatio)) * 0.5f;
    }

    public override void AI()
    {
        base.AI();

        if (Interpolant > 0.1f && IsFinishingSwing() && !_init)
        {
            _init = true;
  
            SlashTrailBuilder slashTrailBuilder = new SlashTrailBuilder();
            slashTrailBuilder.baseColor = Color.DarkGreen;
            slashTrailBuilder.windColor = Color.DarkGray;
            slashTrailBuilder.lightColor = Color.WhiteSmoke;
            slashTrailBuilder.colorFunction = GetTrailColor;
            slashTrailBuilder.widthFunction = GetTrailWidth;
            SlashTrailer slashTrailer = slashTrailBuilder.Instantiate();
            FixedRichLaserShader rls = new FixedRichLaserShader();
            rls.SetDefaults();
            rls.LaserColor = Color.LightGreen;
            rls.InnerColor = Color.Green;
            rls.OuterColor = Color.DarkGray;
            rls.LaserTexture = TrailRegistry.BeamTrail;
            rls.BloomTexture = AssetManager.LaserTextures.Bloom;
            slashTrailer.Shader = rls;

            slashTrailer.invert = ComboIndex % 2 != 0;
            Trailer = slashTrailer;

        }
        _traveledRotation += MathF.Abs(Projectile.rotation - _oldRot);
        _oldRot = Projectile.rotation;
        if (IsFinishingSwing())
        {
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
                sp.color = Color.Lerp(Color.Lerp(Color.Black, Color.DarkGreen, 0.15f), Color.Black, Main.rand.NextFloat(0f, 1f)) * 0.125f * 0.5f;
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
                    sp2.color = Color.Lerp(color, Color.White, 0.25f) * 0.125f * 0.5f;
                    sp2.Scale *= 0.5f;
                }
            }
        }
        outlineColor = Color.Lerp(Color.Green, Color.White, Interpolant);

    }

    public override void DrawSwingTrail(ref Color lightColor, Vector2[] swingTrailCache)
    {
        base.DrawSwingTrail(ref lightColor, swingTrailCache);


    }
    public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
    {
        base.ModifyHitNPC(target, ref modifiers);
        if (!_hit)
        {
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), target.Center, Vector2.Zero,
                ModContent.ProjectileType<IrradiaspearBoom>(),
                Projectile.damage, Projectile.knockBack, Projectile.owner, ai1: 1);
            _hit = true;
        }
   
        SoundStyle spearHit = SoundRegistry.SpearHit1;
        spearHit.PitchVariance = 0.5f;
        SoundEngine.PlaySound(spearHit, Projectile.position);
        if (IsFinishingSwing())
        {
            DamageHelper.PercentIncreasedamage(ref modifiers, 0.5f);
        }
    }
}

public class IrradiaspearBoom : ModProjectile,
    IDrawToRenderTarget
{
    public override string Texture => TextureRegistry.EmptyTexture;
    private ref float Timer => ref Projectile.ai[0];
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 232;
        Projectile.height = 232;
        Projectile.friendly = true;
        Projectile.timeLeft = 60;
        Projectile.tileCollide = false;
        Projectile.light = 0.78f;
        Projectile.ignoreWater = true;
        Projectile.penetrate = -1;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = -1;
    }

    public override void AI()
    {
        base.AI();
        Timer++;
        if (Timer == 1)
        {
            SoundStyle soundStyle = new SoundStyle("Stellamod/Assets/Sounds/IrradiatedNest_Missile_Land") with { PitchVariance = 0.6f };
            SoundEngine.PlaySound(soundStyle, Projectile.position);
            PixelPrimitiveCircleFactory.CreateGenericBoom(Projectile.Center, Color.White, Color.LightGreen, 45, 64);


            for (int i = 0; i < 16; i++)
            {
                Vector2 velocity = Main.rand.NextVector2Circular(12, 12);
                DustParticleSpawnParams spawnParams = DustParticleSpawnParams.Default;
                spawnParams.outerColor = Color.Green;
                spawnParams.scaleRange *= 0.85f;
                spawnParams.innerColor = Color.White;
                var dp = DustParticle.Spawn(Projectile.Center, velocity, spawnParams);
                dp.gravity = 0;
                dp.noTileCollide = true;
                dp.dampening = 0.05f;
            }

            var fx = FXUtil.GlowCircleBoom(Projectile.Center,
                innerColor: Color.White,
                glowColor: Color.LightGreen,
                outerGlowColor: Color.DarkBlue, duration: 25, baseSize: 0.24f);
            fx.Scale *= 1f;
            FXUtil.ShakeCamera(Projectile.Center, 1024, 8);
            FXUtil.PunchCamera(Projectile.Center, Projectile.velocity.SafeNormalize(Vector2.Zero), 4, 4, 4);
        }
    }
    public override bool PreDraw(ref Color lightColor)
    {
        if (Timer < 2)
            return false;
        float outRatio = Timer / 60f;
        RadialShearShader shearShader = RadialShearShader.Instance;
        shearShader.Time = outRatio * 1.4f;

        Asset<Texture2D> magicCircle = AssetManager.GlowMask.SpiralVortex;
        SpritebatchDrawer waveDrawer = SpritebatchDrawer.FromTextureAsset(magicCircle, Projectile.Center);
        waveDrawer.rotation += Main.GlobalTimeWrappedHourly * 4;
        waveDrawer.scale = Vector2.Lerp(Vector2.One * 0.8f, Vector2.One * 1.6f, EasingFunction.OutExpo(outRatio)) * 0.3f;
        waveDrawer.color = Color.Green;
        waveDrawer.color *= MathHelper.SmoothStep(1f, 0f, outRatio);
        waveDrawer.color.A = 0;

        Main.spriteBatch.Restart(effect: shearShader.Effect);
        Main.spriteBatch.Draw(waveDrawer);

        SpritebatchDrawer backGlowDrawwer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.SimpleGlowCircle, Projectile.Center);
        backGlowDrawwer.color = Color.DarkGreen * 0.5f;
        backGlowDrawwer.color.A = 0;
        backGlowDrawwer.scale = Vector2.One * 1f;
        Main.spriteBatch.Draw(backGlowDrawwer);

        waveDrawer.color = Color.Lerp(Color.Black, Color.White, EasingFunction.InOutSine(outRatio));
        waveDrawer.color.A = 0;
        Main.spriteBatch.Draw(waveDrawer);
        Main.spriteBatch.RestartDefaults();
        return false;
    }

    public void DrawToRenderTargets()
    {
        //  throw new NotImplementedException();
    }
}
public class TheIrradiaspearP : ModProjectile,
    IDrawToRenderTarget
{
    private bool _hit;
    private enum ActionState
    {
        Charge,
        Out,
        HitStun
    }

    //Values
    private Vector2 HoldOffset => new Vector2(56, 12);
    private float RotationOffset => MathHelper.ToRadians(120);
    private float ChargeTime => 25 / Owner.GetAttackSpeed(DamageClass.Melee);
    private float SwingTime => 25f / Owner.GetAttackSpeed(DamageClass.Melee);
    private float ThrustDistance => 96;
    private float MaxChargeDistanceMult => 3.5f;
    private bool MaxCharge;
    private Player Owner => Main.player[Projectile.owner];
    //AI
    private float FlashTimer;
    private ref float Timer => ref Projectile.ai[0];
    private ActionState State
    {
        get => (ActionState)Projectile.ai[1];
        set => Projectile.ai[1] = (float)value;
    }

    private ref float HitStunTimer => ref Projectile.ai[2];
    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Type] = 12;
        ProjectileID.Sets.TrailingMode[Type] = 2;
    }

    public override void SetDefaults()
    {
        Projectile.DamageType = DamageClass.Melee;
        Projectile.width = 100;
        Projectile.height = 100;
        Projectile.friendly = true;
        Projectile.tileCollide = false;
        Projectile.penetrate = -1;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = -1;
    }

    public override bool? CanDamage()
    {
        //Don't do damage whil charging
        return State != ActionState.Charge;
    }

    public override void AI()
    {
        FlashTimer -= 0.02f;
        if (FlashTimer <= 0)
            FlashTimer = 0;
        switch (State)
        {
            case ActionState.Charge:
                AI_Charge();
                break;
            case ActionState.Out:
                AI_Out();
                break;
            case ActionState.HitStun:
                AI_HitStun();
                break;
        }
    }

    private void AI_Charge()
    {
        Timer++;
        Vector2 mouseWorld = Main.MouseWorld;
        Vector2 directionToMouseWorld = Owner.Center.DirectionTo(mouseWorld);
        Vector2 playerCenter = Owner.RotatedRelativePoint(Owner.MountedCenter, true);
        if (Main.myPlayer == Projectile.owner)
        {
            Owner.ChangeDir(Projectile.direction);
            Projectile.velocity = directionToMouseWorld * ThrustDistance;
            Projectile.netUpdate = true;
        }

        float progress = Timer / ChargeTime;
        float easedProgress = Easing.OutCubic(progress);
        float rotation = Projectile.velocity.ToRotation();


        float holdRotation = rotation;
        Vector2 holdOffset = HoldOffset;
        if (Owner.direction == -1)
        {
            holdOffset.Y *= -1;
        }

        Vector2 swingStart = playerCenter + holdOffset.RotatedBy(holdRotation);
        Vector2 swingEnd = playerCenter + Projectile.velocity + holdOffset.RotatedBy(rotation);
        Vector2 swingCenter = Vector2.Lerp(swingEnd, swingStart, easedProgress);

        Projectile.Center = swingCenter;
        Projectile.rotation = rotation + RotationOffset * Owner.direction;
        if (Owner.direction == -1)
        {
            Projectile.rotation -= MathHelper.Pi;
        }

        Owner.heldProj = Projectile.whoAmI;
        Owner.itemTime = 2;
        Owner.itemAnimation = 2;
        Owner.itemRotation = (float)Math.Atan2(Projectile.velocity.Y * Projectile.direction, Projectile.velocity.X * Projectile.direction);

        if ((int)Timer == (int)ChargeTime)
        {
            SoundStyle soundStyle = new SoundStyle("Stellamod/Assets/Sounds/IrradiatedNest_Teleport");
            SoundEngine.PlaySound(soundStyle, Projectile.position);

            FlashTimer = 1;
            //Idk some visual or sound here
            for (int i = 0; i < 8; i++)
            {
                Vector2 velocity = Main.rand.NextVector2CircularEdge(2, 2);
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowDust>(), velocity,
                    newColor: ColorFunctions.AcidFlame);
            }
        }

        if (Timer >= ChargeTime && Timer % 8 == 0)
        {
            Vector2 velocity = Main.rand.NextVector2CircularEdge(2, 2);
            Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<TSmokeDust>(), velocity,
                newColor: Color.DarkGray);
        }

        if (this.OwnedByLocalClient() && !Main.mouseRight)
        {
            if (Timer >= ChargeTime)
            {
                MaxCharge = true;
                Timer = 0;
                State = ActionState.Out;
            }
            else if (Timer > ChargeTime / 2)
            {
                Timer = 0;
                State = ActionState.Out;

            }
            else
            {
                Projectile.Kill();
            }
            Projectile.netUpdate = true;
        }
    }

    private void AI_Out()
    {
        FlashTimer -= 0.1f;
        Timer++;
        if (Timer == 1 && MaxCharge)
        {
            //Throw Sound
            SoundStyle soundStyle = new SoundStyle("Stellamod/Assets/Sounds/IrradiatedNest_Egg_Shot");
            SoundEngine.PlaySound(soundStyle, Projectile.position);

            //Rocket Boost
            Vector2 velocity = -Projectile.velocity.SafeNormalize(Vector2.Zero);
            for (int i = 0; i < 16; i++)
            {
                Vector2 dustVelocity = velocity.RotatedByRandom(MathHelper.PiOver4 / 4);
                dustVelocity = dustVelocity * Main.rand.NextFloat(2, 15f);
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowDust>(), dustVelocity,
                    newColor: ColorFunctions.AcidFlame);
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<TSmokeDust>(), dustVelocity,
                    newColor: Color.DarkGray);
            }

        }
        float progress = Timer / SwingTime;
        float easedProgress = Easing.SpikeOutCirc(progress);

        //
        Vector2 playerCenter = Owner.RotatedRelativePoint(Owner.MountedCenter, true);

        //Lerp between two points ig

        float distanceMult = MaxCharge ? MaxChargeDistanceMult : 1f;
        float rotation = Projectile.velocity.ToRotation();

        float holdRotation = rotation;
        Vector2 holdOffset = HoldOffset;
        if (Owner.direction == -1)
        {
            holdOffset.Y *= -1;
        }

        Vector2 swingStart = playerCenter + holdOffset.RotatedBy(holdRotation);
        Vector2 swingEnd = playerCenter + (Projectile.velocity * distanceMult) + holdOffset.RotatedBy(rotation);
        Vector2 swingCenter = Vector2.Lerp(swingStart, swingEnd, easedProgress);

        Projectile.Center = swingCenter;
        Projectile.rotation = rotation + RotationOffset * Owner.direction;
        if (Owner.direction == -1)
        {
            Projectile.rotation -= MathHelper.Pi;
        }

        Owner.heldProj = Projectile.whoAmI;
        Owner.itemTime = 2;
        Owner.itemAnimation = 2;
        Owner.itemRotation = (float)Math.Atan2(Projectile.velocity.Y * Projectile.direction, Projectile.velocity.X * Projectile.direction);

        if (Timer >= SwingTime)
        {
            Projectile.Kill();
        }
    }

    private void AI_HitStun()
    {
        HitStunTimer--;

        float progress = Timer / SwingTime;
        float easedProgress = Easing.SpikeOutCirc(progress);

        //
        Vector2 playerCenter = Owner.RotatedRelativePoint(Owner.MountedCenter, true);

        //Lerp between two points ig

        float distanceMult = MaxCharge ? MaxChargeDistanceMult : 1f;
        float rotation = Projectile.velocity.ToRotation();

        float holdRotation = rotation;
        Vector2 holdOffset = HoldOffset;
        if (Owner.direction == -1)
        {
            holdOffset.Y *= -1;
        }

        Vector2 swingStart = playerCenter + holdOffset.RotatedBy(holdRotation);
        Vector2 swingEnd = playerCenter + (Projectile.velocity * distanceMult) + holdOffset.RotatedBy(rotation);
        Vector2 swingCenter = Vector2.Lerp(swingStart, swingEnd, easedProgress);

        Projectile.Center = swingCenter;
        Projectile.rotation = rotation + RotationOffset * Owner.direction;
        if (Owner.direction == -1)
        {
            Projectile.rotation -= MathHelper.Pi;
        }

        Owner.heldProj = Projectile.whoAmI;
        Owner.itemTime = 2;
        Owner.itemAnimation = 2;
        Owner.itemRotation = (float)Math.Atan2(Projectile.velocity.Y * Projectile.direction, Projectile.velocity.X * Projectile.direction);


        if (HitStunTimer <= 0)
        {
            State = ActionState.Out;
        }
    }

    public override bool ShouldUpdatePosition()
    {
        return false;
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        if (MaxCharge)
        {
            //Big impact sound
            SoundStyle soundStyle = new SoundStyle("Stellamod/Assets/Sounds/Irradieagle_Wave");
            SoundEngine.PlaySound(soundStyle, Projectile.position);

            HitStunTimer = 15;
            State = ActionState.HitStun;
            Projectile.netUpdate = true;
            Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(target.position, 1024, 24);
            FXUtil.GlowCircleBoom(target.Center, Color.Yellow, Color.Green, Color.DarkGreen, 45, baseSize: 0.24f);
            PixelPrimitiveCircleFactory.CreateGenericBoom(target.Center, Color.Yellow, Color.Green, 45, 144);
            PixelPrimitiveCircleFactory.CreateGenericBoom(target.Center, Color.LightGreen, Color.Green, 25, 232);
            for (float f = 0; f < 16; f++)
            {
                var dp = DustParticle.Spawn(target.Center, Main.rand.NextVector2Circular(16, 16));
                dp.outerColor = Color.DarkGreen;
                dp.innerColor = Color.LightGreen;
                dp.dampening = 0.05f;
                dp.noTileCollide = true;
                dp.gravity = 0;
                dp.Scale *= 0.8f;
            }
            for (float f = 0; f < 5f; f++)
            {
                Vector2 spawnPosition = target.Center + Main.rand.NextVector2Circular(64, 64);
                Vector2 spawnVelocity = Vector2.Zero;
                spawnVelocity.Y = Main.rand.NextFloat(-10, -1f);

                float spawnScale = Main.rand.NextFloat(0.75f, 1f);
                var steamParticle = Particle<ThickSmokeParticle>.Spawn(spawnPosition, spawnVelocity, color: Color.DarkGray, Scale: spawnScale);


                var sp2 = SirestiasSmokeParticle.SpawnInAlphaLayer(target.Center + Main.rand.NextVector2Circular(64, 64), -Vector2.UnitY * Main.rand.NextFloat(0.2f, 0.8f));
                sp2.color = Color.Lerp(Color.White, Color.Green, Main.rand.NextFloat(1f));
                sp2.gravity = 0;
                sp2.noTileCollide = true;
                sp2.Scale *= 1f;
                //      sp2.stretchScale2 = new Vector2(1f, 0.5f);
                sp2.offsetRot = Main.rand.NextFloat(3.14f);
                sp2.noRot = true;
            }

            if (_hit)
                return;
            _hit = true;
            float num = 5;
            for (int i = 0; i < num; i++)
            {
                float progress = i / num;
                float rot = MathHelper.TwoPi * progress;
                Vector2 velocity = -Projectile.velocity.SafeNormalize(Vector2.Zero);
                velocity = velocity.RotatedBy(rot);
                velocity = velocity * 8;
                float explosionDelay = progress * 30;

                Projectile.NewProjectile(Projectile.GetSource_FromThis(), target.Center, velocity,
                    ModContent.ProjectileType<TheIrradiaspearSparkProj>(),
                    Projectile.damage, Projectile.knockBack, Projectile.owner,
                    ai1: explosionDelay);
            }

            Projectile.NewProjectile(Projectile.GetSource_FromThis(), target.Center, Vector2.Zero,
                ModContent.ProjectileType<IrradiaspearBoom>(),
                Projectile.damage, Projectile.knockBack, Projectile.owner, ai1: 1);
        }
    }

    public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
    {
        if (MaxCharge)
        {
            modifiers.FlatBonusDamage += 50;
        }
    }

    public float WidthFunction(float completionRatio)
    {
        float baseWidth = Projectile.scale * Projectile.width * 0.3f;
        return MathHelper.SmoothStep(baseWidth, 3.5f, completionRatio);
    }

    public Color ColorFunction(float completionRatio)
    {
        if (MaxCharge)
        {
            return Color.Lerp(ColorFunctions.AcidFlame, Color.Transparent, completionRatio);
        }
        else
        {
            return Color.Transparent;
        }
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
        Texture2D whiteTexture = ModContent.Request<Texture2D>($"{Texture}_White").Value;

        //Draw White
        float whiteProgress = 0f;
        switch (State)
        {
            default:
            case ActionState.Charge:
                whiteProgress = Timer / ChargeTime;
                break;
            case ActionState.Out:
                whiteProgress = Timer / SwingTime;
                whiteProgress = 1f - whiteProgress;
                break;
        }

        whiteProgress = MathHelper.Clamp(whiteProgress, 0f, 1f);
        SpriteBatch spriteBatch = Main.spriteBatch;
        Vector2 drawPos = Projectile.Center - Main.screenPosition;
        if (Timer >= ChargeTime)
        {
            drawPos += Main.rand.NextVector2Circular(2, 2);
        }

        Vector2 drawSize = texture.Size();
        Vector2 drawOrigin = drawSize / 2;
        Rectangle? drawRectangle = null;
        Color drawColor = Color.White;
        float drawRotation = Projectile.rotation;
        float drawScale = Projectile.scale;
        SpriteEffects spriteEffects = Owner.direction == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

        //Glow stuff
        float time = Main.GlobalTimeWrappedHourly;
        float timer = Main.GlobalTimeWrappedHourly / 2f + time * 0.04f;
        float rotationOffset = MathF.Sin(Timer * 0.05f) * 4;//VectorHelper.Osc(1f, 2f, 5);
        time %= 4f;
        time /= 2f;

        if (time >= 1f)
        {
            time = 2f - time;
        }

        time = time * 0.5f + 0.5f;
        for (float i = 0f; i < 1f; i += 0.1f)
        {
            float radians = (i + timer) * MathHelper.TwoPi;
            Vector2 rotatedPos = drawPos + new Vector2(0f, 8f * rotationOffset * (1f - whiteProgress)).RotatedBy(radians) * time;
            spriteBatch.Draw(texture, rotatedPos, drawRectangle, drawColor * whiteProgress * 0.2f, drawRotation, drawOrigin, drawScale, spriteEffects, 0);
        }

        //Draw Main sprite
        Main.EntitySpriteDraw(texture, drawPos, drawRectangle, drawColor, drawRotation, drawOrigin, drawScale, spriteEffects, 0);

        //Draw White Flash
        spriteBatch.End();
        spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);


        var shader = ShaderRegistry.MiscSilPixelShader;
        float progress = 1f + MathF.Sin(Timer * 0.1f);

        //The color to lerp to
        shader.UseColor(Color.White);

        //Should be between 0-1
        //1 being fully opaque
        //0 being the original color
        shader.UseSaturation(progress * FlashTimer);

        // Call Apply to apply the shader to the SpriteBatch. Only 1 shader can be active at a time.
        shader.Apply(null);

        spriteBatch.Draw(texture, drawPos, drawRectangle, drawColor, drawRotation, drawOrigin, drawScale, spriteEffects, 0);
        spriteBatch.End();
        spriteBatch.Begin();


        //Draw white overlay
        spriteBatch.Draw(whiteTexture, drawPos, drawRectangle, drawColor * whiteProgress, drawRotation, drawOrigin, drawScale, spriteEffects, 0);
        return false;
    }

    public override void PostDraw(Color lightColor)
    {
        Lighting.AddLight(Projectile.Center, Color.DarkSeaGreen.ToVector3() * 1.75f * Main.essScale);
        if (Main.rand.NextBool(5))
        {
            int dustnumber = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.CursedTorch, 0f, 0f, 150, Color.LightGoldenrodYellow, 1f);
            Main.dust[dustnumber].velocity *= 0.3f;
        }
    }

    public void DrawToRenderTargets()
    {
        //  throw new NotImplementedException();
    }
}

public class TheIrradiaspearSparkProj : ModProjectile,
    IDrawToRenderTarget
{
    private Vector2 _originalPoint;
    public override string Texture => TextureRegistry.EmptyTexture;
    private float LifeTime => 45;
    private ref float Timer => ref Projectile.ai[0];
    private ref float RandOffset => ref Projectile.ai[1];
    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Type] = 12;
        ProjectileID.Sets.TrailingMode[Type] = 2;
    }

    public override void SetDefaults()
    {
        Projectile.width = 32;
        Projectile.height = 32;
        Projectile.penetrate = -1;
        Projectile.tileCollide = false;
        Projectile.friendly = false;
        Projectile.hostile = false;
        Projectile.light = 0.2f;
    }

    public override void AI()
    {
        Timer++;
        if (Timer == 1)
        {
            _originalPoint = Projectile.Center;
            Projectile.velocity *= 3f;
        }

        if (Main.rand.NextBool(8))
        {
            var dp = DustParticle.Spawn(Projectile.Center, Main.rand.NextVector2Circular(8, 8));
            dp.innerColor = Color.LightGreen;
            dp.outerColor = Color.DarkGreen;
            dp.gravity = 0;
            dp.noTileCollide = true;
            dp.dampening = 0.05f;
            dp.Scale *= 0.6f;
        }

        Projectile.velocity *= 0.92f;
        Projectile.velocity = Projectile.velocity.RotatedBy(0.05f);
        Projectile.rotation = Projectile.velocity.ToRotation() + Projectile.velocity.Length() * 0.05f;

        float lifeTime = LifeTime + RandOffset;
        if (Timer >= lifeTime / 2f)
        {
            Projectile.velocity += (_originalPoint - Projectile.Center) * 0.006f;
        }

        if (Timer >= lifeTime - 20)
        {
            Projectile.scale *= 1.01f;
        }
        if (Timer >= lifeTime)
        {
            Projectile.Kill();
        }
    }


    public override bool PreDraw(ref Color lightColor)
    {
        return false;
    }

    public override void OnKill(int timeLeft)
    {
        if (this.OwnedByLocalClient())
        {
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero,
                ModContent.ProjectileType<IrradiaspearBoom>(), Projectile.damage, Projectile.knockBack, Projectile.owner, ai1: 0);
        }
    }


    private Color ColorFunction(float completionRatio)
    {
        Color inColor = Color.White;
        Color trailColor = Color.Lerp(Color.LightGreen, Color.DarkGreen, completionRatio);
        Color easeColor = Color.Lerp(inColor, trailColor, EasingFunction.InExpo(Timer / 60f));
        return easeColor;
    }

    private float WidthFunction(float completionRatio)
    {
        return MathHelper.SmoothStep(10, 2, completionRatio);
    }

    private float WidthFunction2(float completionRatio)
    {
        return WidthFunction(completionRatio) * 2f;
    }

    private void DrawTrails(GraphicsDevice gDevice)
    {
        var shader2 = RichLaserShader.Instance;
        shader2.LaserColor = Color.White;
        shader2.LaserTexture = TrailRegistry.StarTrail;
        shader2.InnerColor = Color.LightGreen * 0.5f;
        shader2.OuterColor = Color.DarkGreen;
        TrailDrawer.Draw(Main.spriteBatch, Projectile.oldPos, ColorFunction, WidthFunction, shader2, Projectile.Size * 0.5f);

        var bloom = BloomTrailShader.Instance;
        bloom.InnerColor = Color.LightGreen * 0.5f;
        bloom.OuterColor = Color.DarkGreen;
        TrailDrawer.Draw(Main.spriteBatch, Projectile.oldPos, ColorFunction, WidthFunction2, bloom, Projectile.Size * 0.5f);
    }

    private void DrawSprite(SpriteBatch sb, Vector2 screenpos)
    {
        SpritebatchDrawer sbDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.SimpleGlowCircle, Projectile.Center);
        sbDrawer.color = Color.Green;
        sbDrawer.color.A = 0;
        sbDrawer.scale *= 0.1f * ExtraMath.Osc(0.8f, 1f, speed: 6, Projectile.whoAmI) * Projectile.scale;
        sb.Draw(sbDrawer);

        sbDrawer.color = Color.LightGoldenrodYellow;
        sbDrawer.color.A = 0;
        sbDrawer.scale *= 0.92f;
        sb.Draw(sbDrawer);
    }

    public void DrawToRenderTargets()
    {
        PixelationManager.QueuePrimitivesDrawAction(DrawTrails);
        PixelationManager.QueueSpritebatchDrawAction(DrawSprite);
    }
}