using Stellamod.Assets;
using Stellamod.Common.Particles;
using Stellamod.Common.Shaders;
using Stellamod.Common.ShockCircleSystem;
using Stellamod.Content.Areas.Tundra.MoonspiralTower.VerliaBoss;
using Stellamod.Content.Areas.Tundra.MoonspiralTower.VerliaBoss.Projectiles;
using Stellamod.Content.CommonMaterials;
using Stellamod.Core;
using Stellamod.Core.Bases;
using Stellamod.Core.NPCHelpers;
using Stellamod.Core.Pixelation;
using Stellamod.Core.SwingSystem;
using Stellamod.Effects.RoyalMagic;
using Stellamod.Items;
using Stellamod.Visual.Particles;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Tundra.MoonspiralTower.WeaponsMT;

public class MagicalAxe : BaseSwingItemV2
{
    public override void SetDefaults2()
    {
        base.SetDefaults2();
        Item.damage = 26;
        Item.shoot = ModContent.ProjectileType<MagicalAxeSwing>();
        staminaProjectileShoot = ModContent.ProjectileType<MagicalAxeThrow>();
        meleeWeaponType = MeleeWeaponType.Hammer;
        staminaCost = 1;
        staminaDamageMultiplier = 3;
    }

    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew(mold: ModContent.ItemType<BlankSword>(), material: ModContent.ItemType<PearlescentScrap>());
    }
}

public class MagicalAxeSlamSwing : ModProjectile
{
    bool _slammed;
    Vector2 _swingOffset;
    Player Owner => Main.player[Projectile.owner];
    ref float Timer => ref Projectile.ai[0];
    float SwingTime => 32;
    float SwingHoldOffset => 72;
    public override string Texture => ModContent.GetInstance<MagicalAxe>().Texture;
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        Projectile.SetTrailCacheLength(32);
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 48;
        Projectile.height = 48;
        Projectile.friendly = true;
        Projectile.tileCollide = false;
        Projectile.penetrate = -1;
        Projectile.light = 0.6f;
        Projectile.ignoreWater = true;
        Projectile.timeLeft = (int)SwingTime;
    }

    public override bool ShouldUpdatePosition()
    {
        return false;
    }
    private float GetTrailWidth(float ratio)
    {
        return MathHelper.SmoothStep(0, 32, ratio);
    }
    private Color GetTrailColor(float ratio)
    {
        return DrawUtilities.InterpolateColorArray(1f - ratio, Color.White, Color.Aqua) * 1.4f * MathHelper.SmoothStep(0f, 1f, ratio);
    }

    public void RenderSwingTrail(ref Color lightColor, Vector2[] points)
    {
        AlcadSlashShader shader = ShaderContent.GetInstance<AlcadSlashShader>();
        shader.ScrollingLaser = TrailRegistry.Beamlight.Value;
        shader.Noise = AssetManager.Noise.Whirly.Value;
        shader.Slash = AssetManager.GlowMask.SwordSlashForward.Asset.Value;
        shader.BloomColor = Color.Blue;
        shader.Time = Main.GlobalTimeWrappedHourly * 24;
        shader.TransformMatrix = TrailDrawer.WorldViewPoint2;
        shader.Distortion = 0.15f;
        TrailDrawer.Draw(points, GetTrailColor, GetTrailWidth, shader);
    }

    public override void AI()
    {
        base.AI();

        Timer++;
        var ratio = Timer / SwingTime;
        var ease = EasingFunction.InExpo(ratio);
        var dir = MathF.Sign(Projectile.velocity.X);
        var startOffset = -Vector2.UnitY;
        var o = -.52f;
        o -= MathHelper.Lerp(0, 0.5f, EasingFunction.OutExpo(ratio));
        startOffset = startOffset.RotatedBy(o * dir);
        var endOffset = Vector2.UnitX * dir;
        var offset = startOffset.RotatedBy(dir * (MathHelper.PiOver2 + 0.3f + -o) * ease);
        offset *= SwingHoldOffset;
        _swingOffset = offset;
        Projectile.Center = Owner.Center + _swingOffset;
        Projectile.rotation = (Projectile.Center - Owner.Center).ToRotation() + MathHelper.PiOver4;
        if(!_slammed && this.OwnedByLocalClient() && ease >= 0.8f)
        {
            var firer = ProjFirer.From<MagicalAxeSlam>(Projectile);
            firer.position.Y += 16;
            firer.New();
            _slammed = true;
            Projectile.Kill();
        }
        Owner.ChangeDir((int)(Projectile.velocity.X < 0 ? -1 : 1));

        if (Projectile.velocity.X < 0)
        {
            Projectile.spriteDirection = -1;
            Projectile.rotation += MathHelper.PiOver2;
        }

        else
            Projectile.spriteDirection = 1;
        if (Main.rand.NextBool(4))
        {
            var pos = Projectile.Center + Main.rand.NextVector2Circular(32, 32);
            var vel = Main.rand.NextVector2Circular(4, 4);
            Particles.SwirlingFlameDust.Spawn(BitDustFactory.SlowingOverTime with
            {
                innerColor = Color.SkyBlue.ToVector4(),
                outerColor = Color.DarkBlue.ToVector4(),
                position = pos,
                velocity = vel,
                timeLeft = 120,
                scale = new Vector2(Main.rand.NextFloat(0.55f, 1f))
            });
        }
        Owner.itemTime = 2;
        Owner.itemAnimation = 2;
        Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, (Projectile.Center - Owner.Center).ToRotation() - MathHelper.PiOver2);
    }

    void RenderPixelatedSwingTrail(GraphicsDevice gDevice)
    {
        var points = new List<Vector2>();

        for(var i = 64; i > 0; i--)
        {
            var t = Timer;
            t -= i * 0.5f;
            var ratio = t / SwingTime;
            var ease = EasingFunction.InExpo(ratio);
            var dir = MathF.Sign(Projectile.velocity.X);
            var startOffset = -Vector2.UnitY;
            var o = -.52f;
            o -= MathHelper.Lerp(0, 0.5f, EasingFunction.OutExpo(ratio));
            startOffset = startOffset.RotatedBy(o * dir);
            var endOffset = Vector2.UnitX * dir;
            var offset = startOffset.RotatedBy(dir * (MathHelper.PiOver2 + 0.3f + -o) * ease);
            offset *= SwingHoldOffset;
            points.Add(Owner.Center + offset);
        }
        var c = Color.White;
        RenderSwingTrail(ref c, points.ToArray());
    }
    public override bool PreDraw(ref Color lightColor)
    {
        PixelationManager.QueuePrimitivesDrawAction(RenderPixelatedSwingTrail, DrawLayer.OverNPCs);
        var drawer = Projectile.Drawer;
        if(Timer >= 18)
            DrawUtilities.DrawAdditiveFadingTrail(Projectile, Color.SkyBlue, Color.Transparent, 0.3f);
        Main.spriteBatch.Draw(drawer);

        var drawer2 = drawer;
        drawer2.color = Color.Lerp(Color.SkyBlue, Color.Transparent, EasingFunction.OutCirc(Timer / SwingTime));
        drawer2.color.A = 0;
        Main.spriteBatch.Draw(drawer2);

        drawer2.color = Color.Lerp(Color.Transparent, Color.SkyBlue, EasingFunction.InCirc(Timer / SwingTime));
        drawer2.color.A = 0;
        Main.spriteBatch.Draw(drawer2);

        return false;
    }

    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
        var fx = FXUtil.GlowCircleBoom(Projectile.Center, Color.White, Color.SkyBlue, Color.DarkBlue, duration: 25, baseSize: 0.19f);
        fx.Scale *= 1.2f;
    }
}

public class MagicalAxeThrow : ModProjectile
{
    struct CaughtNPC
    {
        public Vector2 offset;
        public int npcID;
    }
    List<CaughtNPC> _caughtNPCs = new();
    float PullRadius => 40;
    public override string Texture => ModContent.GetInstance<MagicalAxe>().Texture;
    float ThrowRange => 384;
    float Time => 60;
    ref float Timer => ref Projectile.ai[0];
    Player Owner => Main.player[Projectile.owner];
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        Projectile.SetTrailCacheLength(16);
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = Projectile.height = 64;
        Projectile.friendly = true;
        Projectile.timeLeft = (int)Time;
        Projectile.tileCollide = false;
        Projectile.penetrate = -1;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 25;
        Projectile.ignoreWater = true;
        Projectile.light = 0.6F;
    }

    public override bool ShouldUpdatePosition()
    {
        return false;
    }

    bool HasNpc(NPC npc)
    {
        foreach(var caughtNpc in _caughtNPCs)
        {
            if (caughtNpc.npcID == npc.whoAmI)
                return true;
        }
        return false;
    }

    void AddNpc(NPC npc)
    {
        _caughtNPCs.Add(new CaughtNPC { npcID = npc.whoAmI, offset = (npc.Center - Projectile.Center) });
    }

    public override void AI()
    {
        base.AI();
        var radiusSquared = PullRadius * PullRadius;
        foreach(var npc in Main.ActiveNPCs)
        {
            if (npc.boss)
                continue;
            if (NPCSets.Heavy[npc.type])
                continue;

            var distSquard = Vector2.DistanceSquared(npc.Center, Projectile.Center);
            if(distSquard <= radiusSquared)
            {
                if (HasNpc(npc))
                {
                    continue;
                }
                AddNpc(npc);
            }
        }
        foreach(var caught in _caughtNPCs)
        {
            var npc = Main.npc[caught.npcID];
            npc.Center = Projectile.Center + caught.offset;
        }

        Timer++;
        if(Timer == 1)
        {
            ShockCircles.CreateQuickWhiteFlash(Projectile.Center);
            var iceyWind = AssetReferences.Assets.Sounds.IceyWind.Asset with { PitchVariance = 0.8f, Volume = 0.5f };
            SoundEngine.PlaySound(iceyWind, Projectile.position);
        }

        var startPos = Owner.Center;
        var direction = Projectile.velocity.SafeNormalize(Vector2.Zero);
        var endPos = startPos + direction * ThrowRange;


        var ratio = Timer / Time;
        var easeOut = EasingFunction.OutExpo(ratio);
        var easeIn = EasingFunction.InExpo(ratio);

        var lerp1 = Vector2.Lerp(startPos, endPos, easeOut);
        var lerp2 = Vector2.Lerp(endPos, startPos, easeIn * 0.9f);
        var lerp3 = Vector2.Lerp(lerp1, lerp2, ratio);
        Projectile.Center = lerp3;
        Projectile.rotation += MathF.Sign(Projectile.velocity.X) * 0.3f;

        if (Timer % 7 == 0)
        {
            var sp = SparkleParticle.Spawn(Projectile.Center + Main.rand.NextVector2Circular(32, 32), Vector2.Zero);
            sp.flickering = true;
            sp.outerColor = Color.Blue;
            sp.fast = true;
            sp.behindLayer = true;
            sp.gravity = 0;
        }

        if (Main.rand.NextBool(8))
        {
            var pos = Projectile.Center + Main.rand.NextVector2Circular(64, 64);
            var vel = Main.rand.NextVector2Circular(3, 3);
            var d = Dust.NewDustPerfect(pos, DustID.GemSapphire, vel, Scale: Main.rand.NextFloat(0.5f, 1f));
            d.noGravity = true;
        }

        if (Timer % 4 == 0)
        {
            var pos = Projectile.Center + Main.rand.NextVector2Circular(64, 64);
            var vel = Main.rand.NextVector2Circular(2, 2);
            Particles.SwirlingFlameDust.Spawn(BitDustFactory.SlowingOverTime with
            {
                position = pos,
                velocity = vel,
                timeLeft = 120,
                innerColor = Color.SkyBlue.ToVector4(),
                outerColor = Color.DarkBlue.ToVector4(),
                scale = new Vector2(Main.rand.NextFloat(0.5f, 1f))
            });
        }
        Owner.itemTime = 2;
        Owner.itemAnimation = 2;
        Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, (Projectile.Center - Owner.Center).ToRotation() - MathHelper.PiOver2);
    }
    void DrawPixelated(SpriteBatch sb, Vector2 sp)
    {
        var swirlDrawer = SpritebatchDrawer.FromTextureAsset(AssetReferences.Assets.GlowMasks.SpiralVortex.Asset, Projectile.Center);
        swirlDrawer.color = Color.SkyBlue;
        swirlDrawer.color.A = 0;
        swirlDrawer.scale *= 0.4f;
        swirlDrawer.rotation = Main.GlobalTimeWrappedHourly * 2;
        Main.spriteBatch.Draw(swirlDrawer);

        swirlDrawer.color *= 0.5f;
        swirlDrawer.scale *= 2f;
        swirlDrawer.rotation *= -1;
        Main.spriteBatch.Draw(swirlDrawer);
        var glowDrawer = SpritebatchDrawer.FromTextureAsset(AssetReferences.Assets.GlowMasks.SimpleGlowCircle.Asset, Projectile.Center);
        glowDrawer.color = Color.SkyBlue * 0.4f;
        glowDrawer.color.A = 0;
        glowDrawer.scale *= 0.66f;
        Main.spriteBatch.Draw(glowDrawer);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        PixelationManager.QueueSpritebatchDrawAction(DrawPixelated);
        var drawer = Projectile.Drawer;


        DrawUtilities.DrawAdditiveFadingTrail(Projectile, Color.SkyBlue, Color.Transparent, 0.1f);
        Main.spriteBatch.Draw(drawer);
        return false;
    }

    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
        if (this.OwnedByLocalClient())
        {
            var firer = ProjFirer.From<MagicalAxeSlamSwing>(Projectile);
            firer.velocity = (Main.MouseWorld - Owner.Center);
            firer.New();
        }
    }
}

public class MagicalAxeWave : ModProjectile
{
    ref float Timer => ref Projectile.ai[0];
    public override string Texture => TextureRegistry.EmptyTexture;
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 512;
        Projectile.height = 64;
        Projectile.tileCollide = false;
        Projectile.penetrate = -1;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = -1;
        Projectile.light = 0.6f;
        Projectile.timeLeft = 120;
    }
    public override void AI()
    {
        base.AI();
        Timer++;
    }
    public override bool PreDraw(ref Color lightColor)
    {
        return false;
    }

    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
    }
}
public class MagicalAxeSlam : ModProjectile
{
    float Scale => 0.3f;
    float Time => 54;
    ref float Timer => ref Projectile.ai[0];
    public override string Texture => TextureRegistry.EmptyTexture;
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 512;
        Projectile.height = 64;
        Projectile.tileCollide = false;
        Projectile.penetrate = -1;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = -1;
        Projectile.light = 0.6f;
        Projectile.timeLeft = (int)Time;
        Projectile.friendly = true;
    }
    public override void AI()
    {
        base.AI();
        Timer++;
        if (Timer == 1)
        {
            Particles.CrackDust.Spawn(CrackImpactDust.Data.Default with { position = Projectile.Center, timeLeft = 200, color = Color.DarkOrange });
            Particles.CrackDust.Spawn(CrackImpactDust.Data.Default with { position = Projectile.Center, scale = 2f, timeLeft = 120, color = Color.DarkOrange });
            Particles.CrackDust.Spawn(CrackImpactDust.Data.Default with { position = Projectile.Center, scale = 3f, timeLeft = 45, color = Color.DarkOrange });
            Particles.CrackDust.Spawn(CrackImpactDust.Data.Default with { position = Projectile.Center, scale = 5f, timeLeft = 25, color = Color.DarkOrange });
            ShockCircles.CreateQuickWhiteFlash(Projectile.Center);
            FXUtil.ShakeCamera(Projectile.Center, 1024, 8);
            for(var i = 0; i < 64; i++)
            {
                var pos = Projectile.Center + Main.rand.NextVector2Circular(127, 40);
                var vel = -Vector2.UnitY * Main.rand.NextFloat(5, 25);
                var d = Dust.NewDustPerfect(pos, DustID.GemSapphire, vel, Scale: Main.rand.NextFloat(0.6f, 1.2f));
            }
            var dsBomb = AssetReferences.Assets.Sounds.DeathShotBomb.Asset with { PitchVariance = 0.3f, Pitch = 0.6f };
            SoundEngine.PlaySound(dsBomb, Projectile.position);

            var fx = FXUtil.GlowCircleBoom(Projectile.Center, Color.White, Color.SkyBlue, Color.DarkBlue, duration: 25);
            fx.Scale *= 1.5f;

            var fx2 = FXUtil.GlowCircleBoom(Projectile.Center, Color.White, Color.SkyBlue, Color.DarkBlue, duration: 45);
            fx2.VectorScale.X *= 6;
            fx2.VectorScale.Y *= 0.8f;
            fx2.VectorScale *= 1.5f;
            fx2.noRot = true;
            for (var i =0; i < 100; i++)
            {
                var pos = Projectile.Center + Main.rand.NextVector2Circular(40, 40);
                var vel = Main.rand.NextVector2Circular(48, 48);
                Particles.SwirlingFlameDust.Spawn(BitDustFactory.SlowingOverTime with
                {
                    position = pos,
                    velocity = vel,
                    innerColor = Color.SkyBlue.ToVector4(),
                    outerColor = Color.DarkBlue.ToVector4(),
                    timeLeft = 120,
                    scale = new Vector2(Main.rand.NextFloat(0.5f, 1f))
                });
            }

            for (var i = 0; i < 14; i++)
            {
                var pos = Projectile.Center + new Vector2(Main.rand.NextFloat(-252, 252), 0);
                FXUtil.MakeSoilParticle(pos, -Vector2.UnitY * Main.rand.NextFloat(5f, 10f));
            }
        }
    }
    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        base.OnHitNPC(target, hit, damageDone);
        if (target.boss)
            return;
        if (NPCSets.Heavy[target.type])
            return;

        target.velocity.Y = -12;
    }

    public override bool PreDraw(ref Color lightColor)
    {
  //      DrawShockwave();
        return false;
    }

    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
    }
}
public class MagicalAxeSwing : BaseSwingProjectileV2
{
    bool _hit;
    bool _slammed;
    public override void DefineCombo()
    {
        base.DefineCombo();
        //For this combo, the magical axe does two overhead swings, but the second part of the combo doesn't happen unless you hit something
        //So basically, it hits twice, then it uppercuts sending the enemy up into the air a little bit
        //Then it comes back down and slams sending a shockwave across the ground and pushing things back

        //For the stamina it throws the weapon forward and it grabs everything like hammers in brawl
        //When it returns to you you slam down and do a big crash
        SoundStyle hammerSlash1 = SoundRegistry.HeavySwordSlash1;
        hammerSlash1.PitchVariance = 0.2f;

        SoundStyle hammerSlash2 = SoundRegistry.HeavySwordSlash2;
        hammerSlash2.PitchVariance = 0.2f;

        Add(new OvalSwing
        {
            Duration = 75,
            SwingDegrees = 310,
            XSwingRadius = 64,
            YSwingRadius = 64,
            Easing = (float lerpValue) => Easing.InOutBack(lerpValue),
            Sound = hammerSlash1,
            HitCount = 2
        });

        //This should go upward i think
        Add(new OvalSwing
        {
            Duration = 32,
            SwingDegrees = 165,
            XSwingRadius = 128,
            YSwingRadius = 48,
            Easing = (float lerpValue) => EasingFunction.GreatswordAnticipation(lerpValue),
            Sound = hammerSlash1,
            HitCount = 1
        });

        Add(new OvalSwing
        {
            Duration = 44,
            SwingDegrees = 330,
            XSwingRadius = 64,
            YSwingRadius = 64,
            Easing = (float lerpValue) => EasingFunction.GreatswordAnticipation(lerpValue),
            Sound = hammerSlash2,
            HitCount = 2
        });

        Add(new OvalSwing
        {
            Duration = 30,
            SwingDegrees = 420,
            XSwingRadius = 64,
            YSwingRadius = 64,
            Easing = (float lerpValue) => lerpValue,
            Sound = hammerSlash1,
            HitCount = 1
        });

        hitStopTime = EXTRA_UPDATE_COUNT * 4;
        trailOffsetOverride = 1.25f;
        outlineColor = Color.SkyBlue;
        glowAfterImageColor = Color.SkyBlue;

        useBloom = true;
        bloom.innerBloomColor = Color.SkyBlue;
        bloom.outerBloomColor = Color.Blue;
        bloom.bloomWidthFunction = GetTrailWidth;
        useAfterImage = true;
    }

    bool IsValidForKnockup(NPC npc)
    {
        return !npc.boss && !NPCSets.Heavy[npc.type];
    }

    private float GetTrailWidth(float ratio)
    {
        return MathHelper.SmoothStep(0, 32, ratio);
    }
    private Color GetTrailColor(float ratio)
    {
        return DrawUtilities.InterpolateColorArray(1f - ratio, Color.White, Color.Aqua) * 1.4f * MathHelper.SmoothStep(0f, 1f, ratio);
    }

    public override void RenderSwingTrail(ref Color lightColor, Vector2[] points)
    {
        base.RenderSwingTrail(ref lightColor, points);

        AlcadSlashShader shader = ShaderContent.GetInstance<AlcadSlashShader>();
        shader.ScrollingLaser = TrailRegistry.Beamlight.Value;
        shader.Noise = AssetManager.Noise.Whirly.Value;
        shader.Slash = AssetManager.GlowMask.SwordSlashForward.Asset.Value;
        shader.BloomColor = Color.Blue;
        shader.Time = Main.GlobalTimeWrappedHourly * 24;
        shader.TransformMatrix = TrailDrawer.WorldViewPoint2;
        shader.Distortion = 0.15f;
        TrailDrawer.Draw(points, GetTrailColor, GetTrailWidth, shader);
    }

    public override void AI()
    {
        base.AI();
        growScale = MathHelper.Lerp(0f, 0.1f, EasingFunction.QuadraticBump(Interpolant));
        outlineColor = Color.Lerp(Color.White, Color.SkyBlue, ExtraMath.Osc(0f, 1f, speed: 6));
    }

    public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
    {
        base.ModifyHitNPC(target, ref modifiers);
        if (ComboIndex == 2 || ComboIndex == 3)
            modifiers.FinalDamage *= 1.5f;
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        base.OnHitNPC(target, hit, damageDone);
        if(ComboIndex == 0)
        {
            if(IsValidForKnockup(target))
                target.velocity.Y = -4;
        }
        if (ComboIndex == 2)
        {
            //Send upward
            if (IsValidForKnockup(target))
                target.velocity.Y = -12;
            for (var i = 0; i < 20; i++)
            {
                var pos = target.Center + Main.rand.NextVector2Circular(32, 32);
                var vel = -Vector2.UnitY * Main.rand.NextFloat(5f, 10f);
                vel = vel.RotatedByRandom(1.54f);
                Particles.SwirlingFlameDust.Spawn(BitDustFactory.SlowingOverTime with
                {
                    position = pos,
                    velocity = vel,
                    timeLeft = 120,
                    innerColor = Color.SkyBlue.ToVector4(),
                    outerColor = Color.DarkBlue.ToVector4(),
                    scale = new Vector2(Main.rand.NextFloat(0.5f, 1.3f))
                });
            }

            for (var i = 0; i < 3; i++)
            {
                var pos = target.Center + Main.rand.NextVector2Circular(8, 32);
                var vel = -Vector2.UnitY * Main.rand.NextFloat(5f, 15f);
                Dust.NewDustPerfect(pos, DustID.GemSapphire, vel, Scale: Main.rand.NextFloat(0.5f, 1f));
            }

            var fx = FXUtil.GlowCircleBoom(target.Center, Color.White, Color.SkyBlue, Color.DarkBlue, duration: 0.2f, baseSize: 0.2f);
            fx.VectorScale *= new Vector2(0.8f, 1.5f);
        }

        if (!_hit)
        {
            FXUtil.GlowCircleBoom(target.Center, Color.White, Color.SkyBlue, Color.DarkBlue);
        }
        SoundStyle smashSound = Main.rand.NextBool(2) ? SoundRegistry.HammerHit1 : SoundRegistry.HammerHit2;
        smashSound.PitchVariance = 0.2f;
        SoundEngine.PlaySound(smashSound, Projectile.position);
        _hit = true;
    }
    public override void OnKill(int timeLeft)
    {
        var comboPlayer = Owner.GetModPlayer<SwingPlayerV2>();
        if (!_hit)
        {

            comboPlayer.ResetCombo();
        }
        if (_hit && ComboIndex == 2)
        {
            int combo = ComboIndex + 1;
            int dir = comboPlayer.ComboDirection;


            if (ComboIndex < ComboCount && this.OwnedByLocalClient())
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position, Main.MouseWorld - Owner.Center, Projectile.type, Projectile.damage, Projectile.knockBack,
                            Projectile.owner, ai2: combo, ai1: dir);
            }
        }

        if(ComboIndex == 3)
        {
            //Throw
            if (this.OwnedByLocalClient())
            {
                var firer = ProjFirer.From<MagicalAxeThrow>(Projectile);
                firer.velocity = (Main.MouseWorld - Owner.Center);
                firer.damage *= 2;
                firer.New();
            }
        }
    }
}