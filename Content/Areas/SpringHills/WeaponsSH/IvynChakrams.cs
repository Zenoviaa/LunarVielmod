using Stellamod.Common.Particles;
using Stellamod.Common.Shaders;
using Stellamod.Content.CommonMaterials;
using Stellamod.Core.Bases;
using Stellamod.Core.Effects.Trails;
using Stellamod.Core.SwingSystem;
using Stellamod.Items;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.SpringHills.WeaponsSH;

public class IvynChakrams : BaseSwingItemV2
{
    public override void SetDefaults2()
    {
        base.SetDefaults2();
        Item.damage = 2;
        Item.ArmorPenetration = 5;
        Item.DamageType = DamageClass.Summon;
        Item.shoot = ModContent.ProjectileType<IvynChakramsSlash>();
   
    }

    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew<Ivythorn, BlankSafunai>();
    }
}

public class IvynChakramsSlash : BaseSwingProjectileV2
{
    bool _hit;
    bool _spawnedClone;
    public override void DefineCombo()
    {
        base.DefineCombo();
        trailOffsetOverride = 1;
        ComboBuilder comboBuilder = new ComboBuilder();
        for (int i = 0; i < 2; i++)
        {
            comboBuilder.AddChakramSpin2(duration: 24, xSwingRadius: 64, ySwingRadius: 64, hitCount: 3, swingDegrees: 375);
        }
        comboBuilder.AddChakramUppercut(duration: 30, xSwingRadius: 96, hitCount: 3, swingDegrees: 199);
        comboBuilder.AddChakramThrow(throwDistance: 96);
        comboBuilder.AddToProjectile(this);



        BlackFireShader blackFireShader = new BlackFireShader();
        blackFireShader.SetDefaults();
        blackFireShader.InnerColor = Color.White;
        blackFireShader.OuterColor = Color.RosyBrown;
        blackFireShader.BackColor = Color.DarkGreen;

        SlashTrailer devilsPeak = new SlashTrailer
        {
            Shader = blackFireShader,
            TrailWidthFunction = (interpolant) =>
            {
                return EasingFunction.QuadraticBump(interpolant) * 35;
            },
            TrailColorFunction = (interpolant) =>
            {
                Color lerp1 = Color.Lerp(Color.DarkGreen, Color.RosyBrown, interpolant);
                return Color.Lerp(lerp1, Color.Transparent, EasingFunction.InExpo(interpolant));
            }
        };

        Trailer = devilsPeak;
        useAfterImage = true;
    }

    public override void AI()
    {
        base.AI();
        if (Timer % 32 == 0)
        {

        }
        if (Main.rand.NextBool(144))
        {
            MakeLeaf(Projectile.Center + Main.rand.NextVector2Circular(8, 8));
        }
        if (!_spawnedClone && Interpolant > 0.1f)
        {
            if (IsFinishingSwing())
            {
                Owner.velocity += Projectile.velocity * 0.1f;
            }
            MirrorProjectile();
            _spawnedClone = true;
        }
        glowColor = Color.Lerp(Color.Transparent, Color.RosyBrown * 0.5f, EasingFunction.QuadraticBump(Interpolant));
        growScale = MathHelper.Lerp(0f, 0.3f, EasingFunction.QuadraticBump(Interpolant));
    }

    void MakeLeaf(Vector2 pos)
    {

        var vel = Main.rand.NextVector2Circular(6, 6);
        Particles.IvynLeaf.Spawn(new()
        {
            position = pos,
            velocity = vel,
            timeLeft = Main.rand.Next(90, 120),
        });
    }
    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        base.OnHitNPC(target, hit, damageDone);
        Main.player[Projectile.owner].MinionAttackTargetNPC = target.whoAmI;
        if (!_hit)
        {
            for (var i = 0; i < 1; i++)
            {
                MakeLeaf(target.Center + Main.rand.NextVector2Circular(8, 8));
            }

            var fac = Common.Particles.ParticleUtils.ParticleFactory.FromSmallBurst(target.Center, Projectile.Center, TriColorPalette.Foresty, new Vector2(5f, 15f));
            fac.particleCount /= 3;
            Common.Particles.ParticleUtils.CreateSwirlingDustCircle(fac);
            _hit = true;
        }

        target.AddBuff(BuffID.Poisoned, 15);
    }

    public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
    {
        base.ModifyHitNPC(target, ref modifiers);
        SoundStyle spearHit = SoundRegistry.SpearHit1;
        spearHit.PitchVariance = 0.5f;
        SoundEngine.PlaySound(spearHit, Projectile.position);
        if (IsFinishingSwing())
        {
            DamageHelper.PercentIncreasedamage(ref modifiers, 0.5f);
        }
    }
}
