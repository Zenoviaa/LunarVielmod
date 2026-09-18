using Stellamod.Common;
using Stellamod.Common.Particles;
using Stellamod.Common.Shaders;
using Stellamod.Content.CommonMaterials;
using Stellamod.Core;
using Stellamod.Core.Bases;
using Stellamod.Core.SwingSystem;
using Stellamod.Items;
using Stellamod.Visual.Particles;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Tundra.Abyss.WeaponsAB;

public class SingularDive : BaseSwingItemV2
{
    public override void SetDefaults2()
    {
        base.SetDefaults2();
        Item.damage = 27;
        Item.shoot = ModContent.ProjectileType<SingularDiveSlash>();
        staminaProjectileShoot = ModContent.ProjectileType<SingularDiveDive>();
        meleeWeaponType = MeleeWeaponType.Greatsword;
        staminaCost = 2;
        staminaDamageMultiplier = 5;
    }

    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew<ConvulgingMater, BlankSword>();
    }
}

public class SingularDiveBoom : ModProjectile
{
    private ref float Timer => ref Projectile.ai[0];
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        Main.projFrames[Type] = 30;
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 128;
        Projectile.height = 128;
        Projectile.tileCollide = false;
        Projectile.penetrate = -1;
        Projectile.friendly = true;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = -1;
        Projectile.timeLeft = 120;
    }
    public override void AI()
    {
        base.AI();
        Timer++;
        if(Timer == 1)
        {
            var sound = AssetReferences.Assets.Sounds.SingularityFragment_Shot1.Asset with { PitchVariance = 0.7f };
            SoundEngine.PlaySound(sound, Projectile.position);
            for (float f = 0; f < 16; f++)
            {
                Vector2 pos = Projectile.Bottom;
                pos.X += Main.rand.NextFloat(-32, 32);
                pos.Y += Main.rand.NextFloat(-64, 0);
                
                Vector2 vel = -Vector2.UnitY;
                vel *= Main.rand.NextFloat(4f, 35);
                vel = vel.RotatedByRandom(0.6f);
                Particles.SwirlingFlameDust.Spawn(BitDustFactory.SlowingOverTime with
                {
                    position = pos,
                    velocity = vel,
                    innerColor = Color.White.ToVector4(),
                    outerColor = Color.Blue.ToVector4(),
                    scale = new Vector2(Main.rand.NextFloat(0.4f, 1.4f))
                });
            }

            for(float f = 0; f < 16; f++)
            {
                Vector2 pos = Projectile.Bottom;
                pos.X += Main.rand.NextFloat(-32, 32);
                pos.Y += Main.rand.NextFloat(-64, 0);

                Vector2 vel = -Vector2.UnitY;
                vel *= Main.rand.NextFloat(4f, 35);
                vel = vel.RotatedByRandom(0.6f);
            
                var dp = DustParticle.Spawn(pos, vel);
                dp.dampening = 0.05f;
                dp.outerColor = Color.Violet;
                dp.innerColor = Color.LightBlue;
                dp.Scale = Main.rand.NextFloat(0.5f, 1f);
            }

            FXUtil.ShakeCamera(Projectile.Center, 1024, 8);
            var fx = FXUtil.GlowCircleDetailedBoom1(Projectile.Bottom, Color.White, Color.SkyBlue, Color.DarkViolet, 45, baseSize: 0.2f);
            fx.Scale *= 2;
            var fx2 = FXUtil.GlowCircleBoom(Projectile.Bottom, Color.White, Color.SkyBlue, Color.DarkViolet, 45, baseSize: 0.2f);
            fx2.VectorScale *= new Vector2(1.5f, 0.8f);
            fx2.noRot = true;
        }

        float frameRate = 2;
        Projectile.frame = (int)(Timer / frameRate);
        if(Timer >= frameRate * Main.projFrames[Type])
        {
            Projectile.Kill();
        }
    }

    public override bool ShouldUpdatePosition()
    {
        return false;
    }

    public override bool PreDraw(ref Color lightColor)
    {
        SpritebatchDrawer drawer = SpritebatchDrawer.FromProjectile(Projectile);
        drawer.BottomCenterOrigin();
        drawer.worldPosition.Y += 64;
        drawer.color = Color.White;
        drawer.color.A = 0;
        drawer.scale *= 2;
        Main.spriteBatch.Draw(drawer);
        SpritebatchDrawer glowDrawer = SpritebatchDrawer.FromTextureAsset(AssetReferences.Assets.GlowMasks.SimpleGlowCircle.Asset, Projectile.Bottom);
        glowDrawer.color = Color.SkyBlue;
        glowDrawer.color.A = 0;
        glowDrawer.scale *= new Vector2(1f, 0.2f);
        glowDrawer.scale.Y *= MathHelper.Lerp(1f, 0f, Timer / 66);
        glowDrawer.color *= MathHelper.Lerp(1f, 0f, Timer / 66);
        Main.spriteBatch.Draw(glowDrawer);
        Main.spriteBatch.Draw(glowDrawer);
        return false;
    }

    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
    }
}

/// <summary>
/// This projectile should take the player into the sky and make them come crashing down
/// We wanna do that out expo in expo crashing movement
/// That should be fairly simple to do in this case
/// </summary>
public class SingularDiveDive : ModProjectile
{
    private Vector2 _startPosition;
    private Vector2 _endPosition;
    private float Time => 60;
    private ref float Timer => ref Projectile.ai[0];
    public override void SendExtraAI(BinaryWriter writer)
    {
        base.SendExtraAI(writer);
        writer.WriteVector2(_startPosition);
        writer.WriteVector2(_endPosition);
    }
    public override void ReceiveExtraAI(BinaryReader reader)
    {
        base.ReceiveExtraAI(reader);
        _startPosition = reader.ReadVector2();
        _endPosition = reader.ReadVector2();
    }
    
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        ProjectileID.Sets.TrailCacheLength[Type] = 16;
        ProjectileID.Sets.TrailingMode[Type] = 2;
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 64;
        Projectile.height = 64;
        Projectile.friendly = true;
        Projectile.penetrate = -1;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = -1;
        Projectile.tileCollide = false;
        Projectile.ignoreWater = true;
        Projectile.light = 0.7f;
    }

    public override void AI()
    {
        base.AI();
        Timer++;
        if(Timer == 1)
        {
            var sound = AssetReferences.Assets.Sounds.SingularityFragment_TPIn.Asset with { PitchVariance = 0.7f };
            SoundEngine.PlaySound(sound, Projectile.position);
            _startPosition = Projectile.Center;
            if (this.OwnedByLocalClient())
            {
                _endPosition = Main.MouseWorld;
                _endPosition = TileUtilities.FallToSolidTile(_endPosition);
                _endPosition.Y -= 64;
                Projectile.netUpdate = true;
            }
        }
        if(Timer % 2 == 0)
        {
            Vector2 pos = Projectile.Center;
            pos += Main.rand.NextVector2Circular(36, 36);

            Vector2 vel = Main.rand.NextVector2Circular(9, 9);
            Particles.SwirlingFlameDust.Spawn(BitDustFactory.SlowingOverTime with
            {
                position = pos,
                velocity = vel,
                innerColor = Color.White.ToVector4(),
                outerColor = Color.Blue.ToVector4(),
                scale = new Vector2(Main.rand.NextFloat(0.4f, 1.4f))
            });
        }

        float ratio = Timer / Time;
        Vector2 lerp1 = Vector2.Lerp(_startPosition, _endPosition, ratio);
        float yOut = MathHelper.Lerp(0, -384, EasingFunction.OutExpo(ratio));
        float yIn = MathHelper.Lerp(-384, 0, EasingFunction.InExpo(ratio));
        float yOffset = MathHelper.Lerp(yOut, yIn, ratio);
        Vector2 posToMoveTo = lerp1;
        posToMoveTo.Y += yOffset;

        float rotationToHave = (posToMoveTo - Projectile.Center).ToRotation() + MathHelper.PiOver4;
        float stylishRotation = MathHelper.Lerp(12f, 0f, EasingFunction.OutSine(ratio));
        Projectile.rotation = rotationToHave + stylishRotation;
        Projectile.Center = posToMoveTo;
        Projectile.PlayerOwner.moveToPosition = posToMoveTo;
        Projectile.PlayerOwner.SetImmuneTimeForAllTypes(60);
        if (Timer >= Time)
        {
            Projectile.Kill();
        }
    }

    public override bool ShouldUpdatePosition()
    {
        return false;
    }

    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        return base.OnTileCollide(oldVelocity);
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        base.OnHitNPC(target, hit, damageDone);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        SpritebatchDrawer drawer = SpritebatchDrawer.FromProjectile(Projectile);
        Main.spriteBatch.Draw(drawer);
        return false;
    }

    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
        if (this.OwnedByLocalClient())
        {
            ProjFirer firer = ProjFirer.From<SingularDiveBoom>(Projectile);
            firer.velocity = -Vector2.UnitY;
            firer.New();
        }
    }
}
public class SingularDiveSlash : BaseSwingProjectileV2
{
    private float _traveledRotation;
    private float _oldRot;
    private bool _hit;
    public override void DefineCombo()
    {
        base.DefineCombo();
        SwingV2Helper.AddGreatswordSwingStyle3(this);
        hitStopTime = EXTRA_UPDATE_COUNT * 8;
        trailOffsetOverride = 2f;
    }

    public override void DrawSwingTrail(ref Color lightColor, Vector2[] swingTrailCache)
    {
        base.DrawSwingTrail(ref lightColor, swingTrailCache);   
        var pass = AssetReferences.Effects.Abyss.SingularTrail.CreatePrimitivesPass();
        pass.Parameters.transformMatrix = TrailDrawer.WorldViewPoint2;
        pass.Parameters.insideColor = Color.DarkBlue.ToVector3();
        pass.Parameters.bloomColor = Color.SkyBlue.ToVector3();
      //  pass.Parameters.time = Main.GlobalTimeWrappedHourly * 

        var hlsl = new HlslSampler();
        hlsl.Sampler = SamplerState.LinearWrap;
        hlsl.Texture = AssetReferences.Assets.LaserTextures.Beamlight.Asset.Value;
        pass.Parameters.laserSampler = hlsl;
        pass.Apply();
     
        TrailDrawer.Draw(swingTrailCache, GetTrailColor, GetTrailWidth, pass.Shader);
    }

    private float GetTrailWidth(float ratio)
    {
        return MathHelper.SmoothStep(0, 48, ratio);
    }
    private Color GetTrailColor(float ratio)
    {
        return DrawUtilities.InterpolateColorArray(1f - ratio, Color.Black, Color.SkyBlue, Color.Lerp(Color.DarkBlue, Color.Black, 0.5f), Color.Black) * 2;
    }

    public override void AI()
    {
        base.AI();
        _traveledRotation += MathF.Abs(Projectile.rotation - _oldRot);
        _oldRot = Projectile.rotation;
    
        outlineColor = Color.Lerp(Color.DarkBlue, Color.SkyBlue, ExtraMath.Osc(0f, 1f, speed: 10));
        glowColor = Color.Lerp(Color.Transparent, Color.White, EasingFunction.QuadraticBump(Interpolant));
        growScale = MathHelper.Lerp(0f, 0.15f, EasingFunction.QuadraticBump(Interpolant));

        if (_traveledRotation > 0.1f)
        {
            _traveledRotation = 0f;
            Vector2 spawnPos = swingTrailCache.InterpolateArrayClamped(Interpolant);
            if (Main.rand.NextBool(6))
            {
                Particles.SwirlingFlameDust.Spawn(BitDustFactory.SlowingOverTime with
                {
                    position = spawnPos,
                    innerColor = Color.White.ToVector4(),
                    outerColor = Color.Blue.ToVector4(),
                    velocity = swingTrailCache.DirectionToNextPoint(Interpolant) * 4
                });
            }
            FaintSmokeParticle sp = FaintSmokeParticle.SpawnInAlphaLayer(spawnPos, Vector2.Zero);
            sp.color = Color.Lerp(Color.Lerp(Color.Black, Color.DarkViolet, 0.15f), Color.Black, Main.rand.NextFloat(0f, 1f)) * 0.25f;
            sp.Scale *= 0.48f;
            sp.behindLayer = true;

            Vector2 spawnVelocity = swingTrailCache.DirectionToNextPoint(Interpolant);
            if (Main.rand.NextBool(2))
            {
                Color color = new Color(41, 43, 66);
                var sp2 = FaintSmokeParticle.SpawnInAlphaLayer(spawnPos + Main.rand.NextVector2Circular(32, 32), spawnVelocity * 0.02f);
                sp2.color = Color.Lerp(color, Color.White, 0.25f) * 0.25f;
                sp2.Scale *= 0.5f;
            }
        }
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        base.OnHitNPC(target, hit, damageDone);
        SoundStyle soundStyle = SoundID.DD2_WitherBeastCrystalImpact with { PitchVariance = 0.5f };
        SoundEngine.PlaySound(soundStyle, target.position);
        if (!_hit)
        {
            FXUtil.ShakeCamera(target.Center, 384, 4);
            FXUtil.GlowCircleBoom(target.Center, Color.White, Color.SkyBlue, Color.DarkBlue, duration: 30, baseSize: 0.2f);
            for (float f = 0; f < 3; f++)
            {
                Particles.SwirlingFlameDust.Spawn(BitDustFactory.SlowingOverTime with
                {
                    innerColor = Color.SkyBlue.ToVector4(),
                    outerColor = Color.DarkBlue.ToVector4(),
                    scale = new Vector2(Main.rand.NextFloat(0.4f, 0.8f))
                });
            }
            _hit = true;
        }
    }

    public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
    {
        base.ModifyHitNPC(target, ref modifiers);
        SoundStyle spearHit = SoundRegistry.SpearHit1;
        spearHit.PitchVariance = 0.5f;
        SoundEngine.PlaySound(spearHit, Projectile.position);
        if (IsFinishingSwing())
        {
            modifiers.FinalDamage *= 0.5f;
        }
    }
}

