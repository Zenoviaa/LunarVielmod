using ReLogic.Content;
using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Content.Areas.SpringHills.WeaponsSH;
using Stellamod.Content.CommonMaterials;
using Stellamod.Content.Dusts;
using Stellamod.Core.Bases;
using Stellamod.Core.Palettes;
using Stellamod.Core.Particles;
using Stellamod.Core.Utilities;
using Stellamod.Helpers;
using Stellamod.Items;
using Stellamod.Items.Ores;
using Stellamod.Trails;
using Stellamod.Visual.Particles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Collosseum.WeaponsCL;

public class ElectricTrident : ModItem
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(5, 7));
        ItemID.Sets.AnimatesAsSoul[Item.type] = true;
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.DefaultToArtifact();
        Item.damage = 80;
        Item.width = 16;
        Item.height = 16;
        Item.mana = 50;
        Item.useAnimation = Item.useTime = 70;
        Item.useStyle = ItemUseStyleID.Shoot;
        Item.UseSound = SoundID.Item92 with { PitchVariance = 0.4f, Volume = 0.3f };
        Item.knockBack = 2;
        Item.shoot = ModContent.ProjectileType<ElectricTridentThrow>();
        Item.shootSpeed = 12;
        Item.noMelee = true;
        Item.noUseGraphic = true;
    }
    public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
    {
        base.ModifyShootStats(player, ref position, ref velocity, ref type, ref damage, ref knockback);
    }
    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        return base.Shoot(player, source, position, velocity, type, damage, knockback);
    }
    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew<GintzlMetal, BlankStaff>();
    }
}

public class ElectricTridentThrow : ModProjectile
{
    private Asset<Texture2D> _outlineTextureAsset;
    private Asset<Texture2D> _whiteTextureAsset;
    private Vector2 _throwVelocity;
    private Vector2 _startPosition;
    private Player Owner => Main.player[Projectile.owner];
    private ref float Timer => ref Projectile.ai[0];
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        ProjectileID.Sets.TrailCacheLength[Type] = 128;
        ProjectileID.Sets.TrailingMode[Type] = 2;

    }
    public override void Unload()
    {
        base.Unload();
        _outlineTextureAsset = null;
        _whiteTextureAsset = null;
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 16;
        Projectile.height = 16;
        Projectile.friendly = true;
        Projectile.tileCollide = false;
        Projectile.timeLeft = 600;
    }
    public override void AI()
    {
        base.AI();

        Timer++;
        if(Timer == 1)
        {
            _throwVelocity = Projectile.velocity;
            Projectile.velocity *= 0;
        }

        float time = 30f;
        if(Timer < time)
        {
            float targetRotation = _throwVelocity.ToRotation() + MathHelper.PiOver4;
            Projectile.rotation = targetRotation;

            float ratio = Timer / time;
            Vector2 normalDirection = _throwVelocity.SafeNormalize(Vector2.Zero);
            Vector2 offset = Vector2.Lerp(normalDirection * 32, -normalDirection * 32, EasingFunction.QuadraticBump(ratio));
            offset.Y -= MathHelper.Lerp(0, 16, EasingFunction.QuadraticBump(ratio));
            Projectile.Center = Owner.MountedCenter + offset;
            _startPosition = Projectile.Center;
            AI_OrientHand();
            for(int i = 0; i < Projectile.oldPos.Length; i++)
            {
                Projectile.oldPos[i] = Vector2.Zero;
            }
        }
        else
        {
            Projectile.tileCollide = true;
            Projectile.extraUpdates = 32;
            Projectile.velocity = _throwVelocity;
            if (Timer % 2 == 0)
            {
                var dp = DustParticle.Spawn(Projectile.Center, _throwVelocity.RotatedByRandom(MathHelper.ToRadians(22)));
                dp.Scale *= 0.6f;
                dp.innerColor = Color.Goldenrod;
                dp.outerColor = Color.DarkBlue;
                dp.gravity = 0;
                dp.dampening = 0.1f;
            }
            if (Main.rand.NextBool(3))
            {
                Vector2 stretchPos = Projectile.Center + Main.rand.NextVector2Circular(32, 32);
                var fx = FXUtil.GlowStretch(stretchPos, Projectile.velocity);
                fx.OuterGlowColor = Color.Gold;
            }
            if (Timer % 4 == 0 && Main.netMode != NetmodeID.Server)
            {
                AfterImageRenderer afRenderer = ModContent.GetInstance<AfterImageRenderer>();
                AfterImageRenderer.New(Texture, Projectile.Frame(), Projectile.Center, Projectile.velocity * 0.3f, Projectile.rotation, Vector2.One, TextureAssets.Projectile[Type].Value.Size() * 0.5f, Color.White * 0.5f, SpriteEffects.None);
            }

        }
    }
    private void AI_OrientHand()
    {

        float rotation = (Projectile.Center - Owner.Center).ToRotation();
        Owner.ChangeDir(Projectile.direction);
        Projectile.spriteDirection = Owner.direction;
        if (Main.myPlayer == Projectile.owner)
        {
            Owner.direction = Main.MouseWorld.X > Owner.MountedCenter.X ? 1 : -1;
        }

        //  Owner.GetModPlayer<SwingPlayerV2>().isSwinging = true;
        Owner.itemRotation = rotation * Owner.direction;
    //    Owner.itemTime = 2;
  //      Owner.itemAnimation = 2;
        // Set composite arm allows you to set the rotation of the arm and stretch of the front and back arms independently
        Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - MathHelper.ToRadians(135));// set arm position (90 degree offset since arm starts lowered)

    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        base.OnHitNPC(target, hit, damageDone);
    }


    public override bool PreDraw(ref Color lightColor)
    {
        if (Timer < 3)
            return false;
        _outlineTextureAsset ??= ModContent.Request<Texture2D>(Texture + "_Outline");
        _whiteTextureAsset ??= ModContent.Request < Texture2D>(Texture + "_White");
        if(Timer > 30)
        {
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                SpritebatchDrawer afDrawer = SpritebatchDrawer.FromProjectile(Projectile);
                afDrawer.worldPosition = Projectile.oldPos[i] + Projectile.Size * 0.5f;
                afDrawer.rotation = Projectile.oldRot[i];
                afDrawer.color = Color.Lerp(Color.Yellow, Color.Transparent, i / (float)Projectile.oldPos.Length) * 0.3f;
                Main.spriteBatch.Draw(afDrawer);
            }
        }


        SpritebatchDrawer sbDrawer = SpritebatchDrawer.FromProjectile(Projectile);
        Main.spriteBatch.Draw(sbDrawer);

        float ratio = EasingFunction.OutExpo(Timer / 30f);
        sbDrawer.texture = _outlineTextureAsset.Value;
        sbDrawer.color = Color.Lerp(Color.Transparent, Color.Lerp(Color.White, Color.Goldenrod, ExtraMath.Osc(0f, 1f, speed: 24)), ratio);
        Main.spriteBatch.Draw(sbDrawer);

        sbDrawer.texture = _whiteTextureAsset.Value;
        sbDrawer.color = Color.Lerp(Color.White, Color.Transparent, ratio);
        Main.spriteBatch.Draw(sbDrawer);

        return false;
    }
    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        return base.OnTileCollide(oldVelocity);
    }
    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
        if (this.OwnedByLocalClient())
        {
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<ElectricTridentBoom>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
        }
        if (this.OwnedByLocalClient())
        {
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), _startPosition, (Projectile.Center - _startPosition),
                ModContent.ProjectileType<ElectricTridentLightning>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
        }
    }
}

public class ElectricTridentBoom : ModProjectile
{
    private ref float Timer => ref Projectile.ai[0];
    public override string Texture => TextureRegistry.EmptyTexture;
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 100;
        Projectile.height = 100;
        Projectile.penetrate = -1;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = -1;
        Projectile.friendly = true;
        Projectile.timeLeft = 60;
        Projectile.tileCollide = false;
    }

    public override void AI()
    {
        base.AI();
        Timer++;
        if(Timer == 1)
        {


            SoundStyle lightningSoundStyle = new SoundStyle("Stellamod/Assets/Sounds/StormDragon_LightingZap");
            lightningSoundStyle.PitchVariance = 0.4f;
            SoundEngine.PlaySound(lightningSoundStyle, Projectile.position);

            SoundStyle hitSound = AssetRegistry.Sounds.Melee.Vinger2;
            hitSound.PitchVariance = 0.2f;
            SoundEngine.PlaySound(hitSound, Projectile.position);

            FXUtil.ShakeCamera(Projectile.Center, 1024, 32);
            FXUtil.GlowCircleBoom(Projectile.Center,
                innerColor: Color.White,
                glowColor: Color.Yellow,
                outerGlowColor: Color.DarkBlue, duration: 25, baseSize: 0.28f);

            var fx = FXUtil.GlowStretch(Projectile.Center, Main.rand.NextVector2Circular(1, 1));
            fx.VectorScale.X *= 4;
            fx.VectorScale.Y *= 6;
            fx.GlowColor = Color.Yellow;
            fx.OuterGlowColor = Color.Red;
            SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode, Projectile.position);

            for (float f = 0; f < 16; f++)
            {
                var dp = Particle<DustParticle>.Spawn(Projectile.Center, Vector2.UnitY.RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(10, 15), Scale: Main.rand.NextFloat(0.5f, 1f));
                dp.innerColor = Color.Yellow;
                dp.outerColor = Color.DarkBlue;
                dp.noTileCollide = true;
                dp.gravity = 0;
                dp.dampening = 0.1f;
            }
            for (float f = 0; f < 4; f++)
            {
                var dp = Particle<DustParticle>.Spawn(Projectile.Center, Vector2.UnitY.RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(15, 22), Scale: Main.rand.NextFloat(1.5f, 2f));
                dp.innerColor = Color.Yellow;
                dp.outerColor = Color.DarkBlue;
                dp.noTileCollide = true;
                dp.gravity = 0;
                dp.dampening = 0.1f;
            }


            for (float f = 0; f < 4; f++)
            {
                Vector2 pos = Projectile.Center;
                pos += Main.rand.NextVector2Circular(32, 32);
                var fs = FaintSmokeParticle.SpawnInAlphaLayer(pos, -Vector2.UnitY, Scale: Main.rand.NextFloat(0.25f, 0.5f));
                fs.noShrink = true;
                fs.Scale *= Main.rand.NextFloat(0.35f, 0.7f);
                fs.color = Color.Lerp(Color.Lerp(Color.Goldenrod, Color.Red, Main.rand.NextFloat(0f, 1f)), Color.Black, 0.7f);
                fs.fadeToColor = Color.Lerp(Color.DarkGoldenrod, Color.Black, 0.8f);
            }
            for (float f = 0; f < 4; f++)
            {
                var smoke = Particle<SmokeParticle>.SpawnInAlphaLayer(Projectile.Center, -Vector2.UnitY.RotatedByRandom(MathHelper.PiOver4) * Main.rand.NextFloat(1, 1f), Color.White, Scale: Main.rand.NextFloat(0.5f, 1f));
                smoke.initialColor = Color.DarkGray;
            }


            for (float i = 0; i < 8; i++)
            {
                float progress = i / 4f;
                float rot = progress * MathHelper.ToRadians(360);
                rot += Main.rand.NextFloat(-0.5f, 0.5f);
                Vector2 offset = rot.ToRotationVector2() * 24;
                var particle = FXUtil.GlowCircleDetailedBoom1(Projectile.Center,
                    innerColor: Color.White,
                    glowColor: Color.Yellow,
                    outerGlowColor: Color.Red,
                    baseSize: Main.rand.NextFloat(0.1f, 0.2f),
                    duration: Main.rand.NextFloat(15, 25));
                particle.Rotation = rot + MathHelper.ToRadians(45);
            }
        }
        if (ModContent.GetInstance<LunarVeilClientConfig>().DramaticEffects)
        {
            SpecialEffectsPlayer effectsPlayer = Main.LocalPlayer.GetModPlayer<SpecialEffectsPlayer>();
            effectsPlayer.darknessCurve = MathHelper.Lerp(0.25f, 0f, EasingFunction.InOutExpo(Timer / 60));
        }

    }
    public override bool PreDraw(ref Color lightColor)
    {
        return base.PreDraw(ref lightColor);
    }
}
public class ElectricTridentLightning : ModProjectile
{
    private Vector2 _lightningHitPos;
    private bool _calculatedStrikePoints;
    public float BeamLength;
    public Vector2[] BeamPoints;
    public float[] BeamRot;
    private float _lightningPower;
    private float _lightningTime;
    public override string Texture => TextureRegistry.EmptyTexture;
    private ref float Timer => ref Projectile.ai[0];
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 6;
        Projectile.height = 6;
        Projectile.penetrate = -1;
        Projectile.friendly = false;
        Projectile.hostile = false;
        Projectile.timeLeft = 60;
        Projectile.tileCollide = false;
        Projectile.extraUpdates = 1;
    }

    public override bool ShouldUpdatePosition()
    {
        return false;
    }

    public override void AI()
    {
        base.AI();

        BeamLength = Projectile.velocity.Length();
        if (!_calculatedStrikePoints)
        {
            List<Vector2> beamPoints = new List<Vector2>();
            Vector2 direction = Projectile.velocity.SafeNormalize(Vector2.Zero);
            float numPoints = 80;
            float randOffset = Main.rand.NextFloat(-1f, 1f);
            Vector2 start = Projectile.Center;
            Vector2 end = Projectile.Center + direction * BeamLength;
            for (float i = 0; i <= numPoints; i++)
            {


                float interp = i / numPoints;
                Vector2 point = Vector2.Lerp(start, end, interp);
                //   point.X += EasingFunction.QuadraticBump(interp) * 64 * randOffset;
                //if(i % 4 == 0)
                //point.X += Main.rand.Next(-16, 16);
                beamPoints.Add(point);
            }

            BeamPoints = beamPoints.ToArray();
            BeamRot = new float[BeamPoints.Length];

            _calculatedStrikePoints = true;
        }
        Timer++;
        if (Timer == 1)
        {

            _lightningPower = 0.9f;
            _lightningTime = 0;


            Vector2 direction = Projectile.velocity.SafeNormalize(Vector2.Zero);
            for (int i = 0; i < 16; i++)
            {
                Vector2 dustSpawnPoint = Projectile.Center + direction * BeamLength;
                Vector2 dustVelocity = Main.rand.NextVector2Circular(8, 8);
                Dust d = Dust.NewDustPerfect(dustSpawnPoint, DustID.GoldCoin, dustVelocity, Scale: 0.5f);
                d.noGravity = true;
            }


            _lightningHitPos = Projectile.position + Projectile.velocity.SafeNormalize(Vector2.Zero) * BeamLength; // new Vector2(0, BeamLength);
            Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(_lightningHitPos, 1024, 32);

            var part = FXUtil.GlowCircleBoom(_lightningHitPos,
                innerColor: Color.White,
                glowColor: Color.Yellow,
                outerGlowColor: Color.Blue, duration: 12, baseSize: 0.14f);
            part.Scale *= 2;
            for (float f = 0; f < 32; f++)
            {
                Dust.NewDustPerfect(_lightningHitPos, DustID.Torch,
                    (Vector2.One * Main.rand.NextFloat(0.2f, 5f)).RotatedByRandom(19.0), 0, Color.White, Main.rand.NextFloat(1f, 3f)).noGravity = true;
            }


            for (float i = 0; i < 15; i++)
            {
                float rot = rot = -Projectile.velocity.ToRotation();
                rot += Main.rand.NextFloat(-0.5f, 0.5f);

                Vector2 offset = rot.ToRotationVector2() * Main.rand.NextFloat(32, 64);
                Vector2 velocity = rot.ToRotationVector2() * Main.rand.NextFloat(2, 15);
                var particle = FXUtil.GlowCircleDetailedBoom1(_lightningHitPos + offset,
                    innerColor: Color.White,
                    glowColor: Color.Yellow,
                    outerGlowColor: Color.Blue,
                    baseSize: Main.rand.NextFloat(0.03f, 0.1f),
                    duration: Main.rand.NextFloat(5, 25));
                particle.Velocity = velocity;
                particle.Scale *= 0.35f;
                particle.Rotation = rot;
            }


            Vector2 position = _lightningHitPos;
            Vector2 lvelocity = -Projectile.velocity * 8;
            for (float f = 0; f < 8; f++)
            {
                Vector2 pVelocity = (lvelocity.SafeNormalize(Vector2.Zero)).RotatedByRandom(MathHelper.PiOver4 / 3f);
                pVelocity *= Main.rand.NextFloat(0.5f, 2f);
                var frag = LegacyParticle.NewParticle<GlowFragmentParticle>(position, pVelocity);
                FXUtil.GlowFragmentParticle(position, pVelocity,
                    innerColor: Color.White,
                    outerColor: Color.Yellow,
                    fadeToColor: Color.Purple,
                    distortOut: true);

                if (Main.rand.NextBool(4))
                {
                    Dust.NewDustPerfect(position, ModContent.DustType<TSmokeDust>(),
                                     lvelocity.RotatedByRandom(MathHelper.PiOver4 / 2f) * 2);
                }
                if (Main.rand.NextBool(4))
                {
                    Dust.NewDustPerfect(position, ModContent.DustType<GlowDust>(),
                                     lvelocity.RotatedByRandom(MathHelper.PiOver4 / 2f) * 3 * Main.rand.NextFloat(0.4f, 1f), newColor: Color.White, Scale: 0.2f);
                }
            }
            for (float f = 0; f < 8; f++)
            {
                Vector2 pVelocity = (lvelocity.SafeNormalize(Vector2.Zero)).RotatedByRandom(MathHelper.PiOver4 / 3f);
                pVelocity *= Main.rand.NextFloat(0.5f, 1f);
                var spark = LegacyParticle.NewParticle<SparkParticle>(position + Main.rand.NextVector2Circular(64, 64), pVelocity);
            }

            var sear = LegacyParticle.NewParticle<SearParticle>(_lightningHitPos, Vector2.Zero);

            for (int i = 0; i < BeamPoints.Length; i++)
            {
                if (Main.rand.NextBool(16))
                {
                    Vector2 pos = BeamPoints[i];
                    pos += Main.rand.NextVector2Circular(32, 32);
                    var zap = LegacyParticle.NewParticle<ZapParticle>(pos, Vector2.UnitY.RotatedByRandom(MathHelper.PiOver4) * Main.rand.NextFloat(2, 4));

                }
            }
        }

        if (Timer == 15)
        {
            _lightningPower = 5;
        }

        if (Timer == 15)
        {
            _lightningPower = 30;
        }

        if (Timer == 30)
        {
        }

        if (Timer > 35)
        {
            _lightningPower = MathHelper.Lerp(_lightningPower, 10, 0.1f);



        }

        if (Timer == 42)
        {
            _lightningPower = 1.5f;
        }
        if (Timer == 42)
        {
            var part = FXUtil.GlowCircleBoom(_lightningHitPos,
                              innerColor: Color.White,
                              glowColor: Color.Yellow,
                              outerGlowColor: Color.Blue, duration: 6, baseSize: 0.12f);
        }
        if (Timer == 52)
        {
            _lightningPower = 2.3f;
        }
        if (Timer == 52)
        {
            var part = FXUtil.GlowCircleBoom(_lightningHitPos,
                              innerColor: Color.White,
                              glowColor: Color.Yellow,
                              outerGlowColor: Color.Blue, duration: 6, baseSize: 0.07f);
        }


        if (Timer == 58)
        {
            SoundStyle zap = SoundID.DD2_LightningBugZap;
            zap.PitchVariance = 0.3f;
            SoundEngine.PlaySound(zap, Projectile.position);

            for (float f = 0; f < 2; f++)
            {
                Vector2 pVelocity = -Vector2.UnitY.RotatedByRandom(MathHelper.PiOver4);
                pVelocity *= Main.rand.NextFloat(0.5f, 1f);
                var spark = LegacyParticle.NewParticle<ZapParticle>(_lightningHitPos + Main.rand.NextVector2Circular(64, 64), pVelocity);
                spark.Scale *= 0.5f;
                spark.Rotation = Main.rand.NextFloat(0f, 3.14f);
            }
        }
        _lightningTime -= 0.1f;

    }

    public override bool? CanDamage()
    {
        return Timer > 30;
    }

    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        return false;
    }

    public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
    {
        float _ = 0f;
        float width = Projectile.width * 0.8f;
        Vector2 start = Projectile.Center;

        Vector2 direction = Projectile.velocity.SafeNormalize(Vector2.Zero);
        Vector2 end = start + direction * BeamLength;
        return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, end, width, ref _);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        if (BeamPoints == null)
            return false;

        SpriteBatch spriteBatch = Main.spriteBatch;
        LightningShader lightningShader = LightningShader.Instance;
        lightningShader.Time = _lightningTime;
        lightningShader.Power = _lightningPower;
        TrailDrawer.Draw(spriteBatch, BeamPoints, BeamRot, LightningColorFunction, LightningWidthFunction, lightningShader);
        if (Timer >= 30)
            TrailDrawer.Draw(spriteBatch, BeamPoints, BeamRot, LightningColorFunction, LightningWidthFunction, lightningShader);

        return false;
    }

    private float LightningWidthFunction(float completionRatio)
    {
        return MathHelper.Lerp(180, 0, completionRatio);
    }

    private Color LightningColorFunction(float completionRatio)
    {
        Color lerpColor = Color.Lerp(Color.White, Color.Blue, (Timer - 30f) / 30f);
        return Color.Lerp(Color.Transparent, lerpColor, EasingFunction.QuadraticBump(completionRatio)); ;
    }
}