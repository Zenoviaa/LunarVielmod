using Stellamod.Common.Particles;
using Stellamod.Common.RarityRendering;
using Stellamod.Core;
using Stellamod.Core.Bases;
using Stellamod.Core.Effects;
using Stellamod.Core.Effects.Trails;
using Stellamod.Core.SwingSystem;
using Stellamod.Trailing;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.SpringHills.WeaponsSH;

public class Jiisword : BaseSwingItemV2
{
    // The Display Name and Tooltip of this item can be edited in the 'Localization/en-US_Mods.Stellamod.hjson' file.
    public override void SetDefaults2()
    {
        base.SetDefaults2();
        Item.damage = 14;
        Item.DamageType = DamageClass.Melee;
        Item.width = 40;
        Item.height = 40;
        Item.noUseGraphic = true;
        Item.noMelee = true;
        Item.useTime = 64;
        Item.useAnimation = 64;
        Item.useStyle = ItemUseStyleID.Swing;
        Item.knockBack = 6;
        Item.value = Item.buyPrice(silver: 1);
        Item.rare = ModContent.RarityType<BossRewardRarity>();
        Item.shootSpeed = 10;
        Item.shoot = ModContent.ProjectileType<JiiswordSlash>();
        Item.autoReuse = true;
        staminaProjectileShoot = ModContent.ProjectileType<JiiswordStaminaSlash>();
        staminaCost = 1;
    }
}

public class JiiswordSlash : BaseSwingProjectileV2
{
    private bool _throwBomb;
    private bool _hasThrownBomb;
    private bool _hit;
    private float GetBloomWidth(float ratio)
    {
        return MathHelper.SmoothStep(4, 42, ratio) * 1.15f * MathHelper.Lerp(1f, 0f, EasingFunction.InExpo(Interpolant));
    }
    private Color GetBloomColor(float ratio)
    {
        Color blue = Color.Lerp(Color.Lerp(Color.Yellow, Color.Blue, 0.5f), Color.Blue, ExtraMath.Osc(0f, 1f, speed: 4));
        return Color.Lerp(blue * 0.9f, Color.Yellow, ratio);
    }

    public override void DefineCombo()
    {
        base.DefineCombo();
        SlashTrailBuilder slashTrailBuilder = new SlashTrailBuilder();
        SlashTrailer slashTrailer = slashTrailBuilder.Instantiate();
        slashTrailer.invert = ComboIndex % 2 != 0;
        Trailer = slashTrailer;

        useBloom = true;
        bloom.innerBloomColor = Color.White;
        bloom.outerBloomColor = Color.SkyBlue;
        bloom.bloomWidthFunction = GetBloomWidth;
        bloom.bloomColorFunction = GetBloomColor;

        additive = true;
        useAfterImage = true;
        SwingV2Helper.AddJiiSwordSwingStyle(this);
    }


    public override void AI()
    {
        base.AI();
        if(Timer == 1 && Main.myPlayer == Projectile.owner)
        {
            if(IsFinishingSwing())
                _throwBomb = true;
        }

        if (Main.rand.NextBool(64))
        {
            Particles.SwirlingFlameDust.Spawn(BitDustFactory.Default with
            {
                position = Projectile.Center + Main.rand.NextVector2Circular(32, 32),
                velocity = Main.rand.NextVector2Circular(14, 14),
                outerColor = Color.Blue.ToVector4(),
                innerColor = Color.SkyBlue.ToVector4(),
                timeLeft = 60,
                scale = new Vector2(Main.rand.NextFloat(0.5f, 1.2f))
            });
        }
        if(_throwBomb && !_hasThrownBomb && Main.myPlayer == Projectile.owner)
        {
            if(EasedInterpolant >= 0.45f)
            {
                ProjFirer firer = ProjFirer.From<JiiswordBomb>(Projectile);
                firer.velocity = Projectile.velocity.SafeNormalize(Vector2.Zero) * 7;
                firer.velocity.Y -= 3;

                firer.New();
                _hasThrownBomb = true;
            }
        }
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        base.OnHitNPC(target, hit, damageDone);
        if (!_hit)
        {
            var fx = FXUtil.GlowCircleBoom(target.Center, Color.White, Color.SkyBlue, Color.Blue);
            fx.Scale *= 0.6f;
            for (int i = 0; i < 8; i++)
            {
                Particles.SwirlingFlameDust.Spawn(BitDustFactory.Default with
                {
                    position = target.Center + Main.rand.NextVector2Circular(32, 32),
                    velocity = Main.rand.NextVector2Circular(14, 14),
                    outerColor = Color.Blue.ToVector4(),
                    innerColor = Color.SkyBlue.ToVector4(),
                    timeLeft = 60,
                    scale = new Vector2(Main.rand.NextFloat(0.5f, 1.2f))
                });
            }
            var sound = SoundID.DD2_CrystalCartImpact with { PitchVariance = 0.4f, Volume = 0.4f };
            SoundEngine.PlaySound(sound, Projectile.position);
            _hit = true;
        }

    }
    public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
    {
        base.ModifyHitNPC(target, ref modifiers);
        SoundStyle spearHit = AssetRegistry.Sounds.Melee.SpearHit1;
        spearHit.PitchVariance = 0.5f;
        SoundEngine.PlaySound(spearHit, Projectile.position);
        if (ComboIndex == 5)
        {
            modifiers.FinalDamage *= 2;
        }
    }
}


public class JiiswordStaminaSlash : BaseSwingProjectileV2
{
    public override void DefineCombo()
    {
        base.DefineCombo();
        SoundStyle chargeSound = AssetRegistry.Sounds.Melee.ScythePull;
        chargeSound.PitchVariance = 0.1f;
        Add(new ThrustSwing
        {
            Duration = 64,
            Easing = EasingFunction.InOutExpo,
            OverrideVelocity = -Vector2.UnitY,
            ThrowDistance = 64,
            Sound = chargeSound,
        });

    }

    public override void AI()
    {
        base.AI();
        if(Main.myPlayer == Projectile.owner)
        {
            if (Timer % 48 ==0)
            {
                ProjFirer firer = ProjFirer.From<JiiswordBomb>(Projectile);
                firer.velocity = -Vector2.UnitY * Main.rand.NextFloat(4f, 10f);
                firer.velocity = firer.velocity.RotatedByRandom(0.5f);
                firer.New();
            }
        }
    }

    public override void RenderSwingTrail(ref Color lightColor, Vector2[] points)
    {
        base.RenderSwingTrail(ref lightColor, points);
    }
}

public class JiiswordBomb : ScarletProjectile
{

    private ref float Timer => ref Projectile.ai[0];
    public override void SetDefaults()
    {
        base.SetDefaults();
        TrailCacheLength = 4;
        Projectile.width = 18;
        Projectile.height = 18;
        Projectile.friendly = true;
    }

    public override void AI()
    {
        base.AI();
        Timer++;
        if (Timer == 1)
        {
            SoundStyle fuse = AssetRegistry.Sounds.Jiitas.JiitasBombFuse;
            fuse.PitchVariance = 0.2f;
            fuse.Volume = 0.6f;
            SoundEngine.PlaySound(fuse, Projectile.position);
        }
        if (Timer % 8 == 0)
        {
            Vector2 muzzlePosition = Projectile.Center;
            muzzlePosition += Main.rand.NextVector2Circular(8, 8);
            for (float i = 0; i < 8; i++)
            {
                float progress = i / 4f;
                float rot = progress * MathHelper.ToRadians(360);
                rot += Main.rand.NextFloat(-0.5f, 0.5f);
                Vector2 offset = rot.ToRotationVector2() * 24;
                var particle = FXUtil.GlowCircleLongBoom(muzzlePosition,
                    innerColor: Color.White,
                    glowColor: Color.Yellow,
                    outerGlowColor: Color.Red,
                    baseSize: Main.rand.NextFloat(0.025f, 0.035f),
                    duration: Main.rand.NextFloat(15, 25));
                particle.Rotation = rot + MathHelper.ToRadians(45);
            }
        }

        Projectile.velocity.Y += 0.15f;
        Projectile.rotation += Projectile.velocity.Length() * 0.05f;
    }

    public override bool PreDraw(ref Color lightColor)
    {
        SpriteBatch spriteBatch = Main.spriteBatch;
        Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
        Vector2 drawOrigin = texture.Size() * 0.5f;


        //draw after image trail
        for (int i = 0; i < OldCenterPos.Length; i++)
        {
            Vector2 centerPos = OldCenterPos[i];
            Vector2 drawPos = centerPos - Main.screenPosition;
            float interpolant = i / (float)OldCenterPos.Length;
            Color drawColor = Color.Lerp(Color.Red, Color.Yellow, interpolant);
            drawColor *= MathHelper.SmoothStep(1.0f, 0f, interpolant);
            drawColor = drawColor.MultiplyRGB(lightColor);
            drawColor.A = 0;
            spriteBatch.Draw(texture, drawPos, null, drawColor, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, layerDepth: 0);
        }

        spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, layerDepth: 0);
        return false; ;
    }

    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
        if (Main.myPlayer == Projectile.owner)
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<JiitaswordBoom>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
    }
}

public class JiitaswordBoom : ModProjectile
{
    public override string Texture => TextureRegistry.EmptyTexture;
    private ref float Timer => ref Projectile.ai[0];
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 32;
        Projectile.height = 32;
        Projectile.friendly = true;
        Projectile.penetrate = -1;
        Projectile.timeLeft = 4;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = -1;
    }

    public override void AI()
    {
        base.AI();
        Timer++;
        if (Timer == 1)
        {
            //Spawn effects
            SoundStyle shotSound = AssetRegistry.Sounds.Jiitas.JiitasGunShot;
            shotSound.PitchVariance = 0.2f;
            SoundEngine.PlaySound(shotSound, Projectile.position);

            //IMPACT EFFECT
            FXUtil.ShakeCamera(Projectile.position, 1024, 2);
            FXUtil.GlowCircleBoom(Projectile.Center,
                innerColor: Color.White,
                glowColor: Color.Yellow,
                outerGlowColor: Color.Red, duration: 25, baseSize: 0.09f);

            SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode, Projectile.position);
            for (float f = 0; f < 4; f++)
            {
                Dust.NewDustPerfect(Projectile.Center, DustID.Torch,
                    (Vector2.One * Main.rand.NextFloat(0.2f, 5f)).RotatedByRandom(19.0), 0, Color.White, Main.rand.NextFloat(1f, 3f)).noGravity = true;
            }

            for (float i = 0; i < 8; i++)
            {
                float progress = i / 4f;
                float rot = progress * MathHelper.ToRadians(360);
                rot += Main.rand.NextFloat(-0.5f, 0.5f);
                Vector2 offset = rot.ToRotationVector2() * 24;
                var particle = FXUtil.GlowCircleLongBoom(Projectile.Center,
                    innerColor: Color.White,
                    glowColor: Color.Yellow,
                    outerGlowColor: Color.Red,
                    baseSize: Main.rand.NextFloat(0.025f, 0.25f),
                    duration: Main.rand.NextFloat(15, 25));
                particle.Rotation = rot + MathHelper.ToRadians(45);
            }
        }
    }
}