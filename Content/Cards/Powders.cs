using Stellamod.Assets;
using Stellamod.Common.IgnitersNPowders;
using Stellamod.Common.Shaders;
using Stellamod.Common.WeaponUpgrade.UI;
using Stellamod.Content.CommonMaterials;
using Stellamod.Core.Palettes;
using Stellamod.Items;
using Stellamod.Projectiles;
using Stellamod.Projectiles.IgniterExplosions;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Cards;

public class MushyPowder : BasePowder
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        //Percent increase, 1 is +100% damage
        DamageModifier = 1f;
        ExplosionType = ModContent.ProjectileType<MushyBoom>();

        SoundStyle explosionSoundStyle = new SoundStyle("Stellamod/Assets/Sounds/Green");
        explosionSoundStyle.PitchVariance = 0.15f;
        ExplosionSound = explosionSoundStyle;
        ExplosionScreenshakeAmt = 4f;
    }
    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew(
            mold: ModContent.ItemType<BlankBag>(), 
            material: ModContent.ItemType<Mushroom>());
    }
}

public class GrassDirtPowder : BasePowder
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        DamageModifier = 1f;
        ExplosionType = ModContent.ProjectileType<GrassExSps>();

        SoundStyle explosionSoundStyle = SoundID.DD2_ExplosiveTrapExplode;
        explosionSoundStyle.PitchVariance = 0.15f;
        ExplosionSound = explosionSoundStyle;
        ExplosionScreenshakeAmt = 1.5f;
    }
    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew(
            mold: ModContent.ItemType<BlankBag>(),
            material: ModContent.ItemType<Ivythorn>());
    }
}

public class FlamePowder : BasePowder
{
    public override void SetDefaults()
    {
        base.SetDefaults();

        DamageModifier = 1f;
        ExplosionType = ModContent.ProjectileType<KaBoom>();

        SoundStyle explosionSoundStyle = new SoundStyle($"Stellamod/Assets/Sounds/Kaboom");
        explosionSoundStyle.PitchVariance = 0.15f;
        ExplosionSound = explosionSoundStyle;
        ExplosionScreenshakeAmt = 2;
    }

    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew(mold: ModContent.ItemType<BlankBag>(),
            material: ModContent.ItemType<AlcadizScrap>());
    }
}
public class AlcadizPowder : BasePowder
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        //Percent increase, 1 is +100% damage
        DamageModifier = 1f;
        ExplosionType = ModContent.ProjectileType<FableExSps>();

        SoundStyle explosionSoundStyle = new SoundStyle($"Stellamod/Assets/Sounds/HeatExplosion");
        explosionSoundStyle.PitchVariance = 0.15f;
        ExplosionSound = explosionSoundStyle;
        ExplosionScreenshakeAmt = 2;
    }

    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew(mold: ModContent.ItemType<BlankBag>(), material: ModContent.ItemType<AlcadizScrap>());
    }
}

public class FrostedPowder : BasePowder
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        //Percent increase, 1 is +100% damage
        DamageModifier = 1f;
        ExplosionType = ModContent.ProjectileType<FrostbiteProj>();

        SoundStyle explosionSoundStyle = new SoundStyle($"Stellamod/Assets/Sounds/Frosty");
        explosionSoundStyle.PitchVariance = 0.15f;
        ExplosionSound = explosionSoundStyle;
        ExplosionScreenshakeAmt = 2;
    }

    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew(mold: ModContent.ItemType<BlankBag>(),
            material: ModContent.ItemType<WinterbornShard>());
    }
}

public class BloodPowder : BasePowder
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        //Percent increase, 1 is +100% damage
        DamageModifier = 1.35f;
        ExplosionType = ModContent.ProjectileType<KaBoomKaev>();


        SoundStyle explosionSoundStyle = new SoundStyle($"Stellamod/Assets/Sounds/Suckler");
        explosionSoundStyle.PitchVariance = 0.15f;
        ExplosionSound = explosionSoundStyle;
        ExplosionScreenshakeAmt = 2;
    }
    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew(mold: ModContent.ItemType<BlankBag>(), material: ModContent.ItemType<TerrorFragments>());
    }
}

public class AivanPowder : BasePowder
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        //Percent increase, 1 is +100% damage
        DamageModifier = 1.35f;
        ExplosionType = ModContent.ProjectileType<AivanKaboom>();

        SoundStyle explosionSoundStyle = SoundID.DD2_ExplosiveTrapExplode;
        explosionSoundStyle.PitchVariance = 0.15f;
        ExplosionSound = explosionSoundStyle;
        ExplosionScreenshakeAmt = 2;
    }
    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew(mold: ModContent.ItemType<BlankBag>(),
            material: ModContent.ItemType<GintzlMetal>());
    }
}


public class AgreviPowder : BasePowder
{
    public override void SetDefaults()
    {
        base.SetDefaults();

        //Percent increase, 1 is +100% damage
        DamageModifier = 1.65f;
        ExplosionType = ModContent.ProjectileType<AgreviBoom>();

        SoundStyle explosionSoundStyle = AssetManager.GetSound("Fire/FireExplosion1");
        explosionSoundStyle.PitchVariance = 0.3f;
        ExplosionSound = explosionSoundStyle;
        ExplosionScreenshakeAmt = 8;
    }
    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew(mold: ModContent.ItemType<BlankBag>(), material: ModContent.ItemType<Cinderscrap>());
    }
}

public class AgreviBoom : BaseIgniterExplosion
{
    public override int FrameCount => 15;
    public override void SetDefaults()
    {
        base.SetDefaults();
        DrawScale = 0.5f;
        Projectile.width = 132;
        Projectile.height = 132;

    }

    public override void Start()
    {
        base.Start();
        float numDust = 8;
        for (float n = 0; n < numDust; n++)
        {
            DustParticleSpawnParams spawnParams = new DustParticleSpawnParams
            {
                innerColor = Color.Yellow,
                outerColor = Color.Red
            };
            Vector2 velocity = Main.rand.NextVector2Circular(16, 16) * 1.2f;
            var dp = DustParticle.Spawn(Projectile.Center, velocity, spawnParams);
            dp.superFast = true;
        }
        FXUtil.GlowCircleDetailedBoom1(Projectile.Center, Color.Yellow, Color.OrangeRed, Color.Red);
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        base.OnHitNPC(target, hit, damageDone);
        if (Main.rand.NextBool(3))
        {
            target.AddBuff(BuffID.OnFire, 120);
        }
    }
}

public class IllusionistPowder : BasePowder
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        //Percent increase, 1 is +100% damage
        DamageModifier = 1.65f;
        ExplosionType = ModContent.ProjectileType<EldritchBoom>();

        SoundStyle explosionSoundStyle = new SoundStyle($"Stellamod/Assets/Sounds/StormDragon_LightingZap");
        explosionSoundStyle.PitchVariance = 0.15f;
        ExplosionSound = explosionSoundStyle;
        ExplosionScreenshakeAmt = 1.5f;
    }
    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew(mold: ModContent.ItemType<BlankBag>(), material: ModContent.ItemType<HypnotizedSoul>());
    }
}

public class AbyssalPowder : BasePowder
{
    public override void SetDefaults()
    {
        base.SetDefaults();

        DamageModifier = 1.65f;
        ExplosionType = ModContent.ProjectileType<VoidKaboom>();

        SoundStyle explosionSoundStyle = new SoundStyle($"Stellamod/Assets/Sounds/ExplosionBurstBomb");
        explosionSoundStyle.PitchVariance = 0.15f;
        ExplosionSound = explosionSoundStyle;
        ExplosionScreenshakeAmt = 4;
    }
    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew(mold: ModContent.ItemType<BlankBag>(), material: ModContent.ItemType<ConvulgingMater>());
    }
}

public class ArcanalPowder : BasePowder
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        //Percent increase, 1 is +100% damage
        DamageModifier = 1.65f;
        ExplosionType = ModContent.ProjectileType<SepsisExSps>();

        SoundStyle explosionSoundStyle = new SoundStyle($"Stellamod/Assets/Sounds/ArcaneExplode");
        explosionSoundStyle.PitchVariance = 0.15f;
        ExplosionSound = explosionSoundStyle;
        ExplosionScreenshakeAmt = 3;
    }
    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew(mold: ModContent.ItemType<BlankBag>(), material: ModContent.ItemType<PearlescentScrap>());
    }
}
public class GovheilPowder : BasePowder
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        //Percent increase, 1 is +100% damage
        DamageModifier = 1.65f;
        ExplosionType = ModContent.ProjectileType<GovheilKaboom>();

        SoundStyle explosionSoundStyle = new SoundStyle($"Stellamod/Assets/Sounds/Binding_Abyss_Rune");
        explosionSoundStyle.PitchVariance = 0.15f;
        ExplosionSound = explosionSoundStyle;
        ExplosionScreenshakeAmt = 1.5f;
    }
    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew(mold: ModContent.ItemType<BlankBag>(), material: ModContent.ItemType<MarshScrap>());
    }

}
public class CrystalPowder : BasePowder
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        //Percent increase, 1 is +100% damage
        DamageModifier = 1.65f;
        ExplosionType = ModContent.ProjectileType<CrystalBloom>();


        SoundStyle explosionSoundStyle = new SoundStyle("Stellamod/Assets/Sounds/GhostExcalibur1");
        explosionSoundStyle.PitchVariance = 0.15f;
        ExplosionSound = explosionSoundStyle;
        ExplosionScreenshakeAmt = 2;
    }

    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew(mold: ModContent.ItemType<BlankBag>(), material: ModContent.ItemType<KaleidoscopicInk>());
    }
}
public class ArtoriaIllurePowder : BasePowder
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        //Percent increase, 1 is +100% damage
        DamageModifier = 2f;
        ExplosionType = ModContent.ProjectileType<IlluredBoom>();

        SoundStyle explosionSoundStyle = new SoundStyle($"Stellamod/Assets/Sounds/Green");
        explosionSoundStyle.PitchVariance = 0.15f;
        ExplosionSound = explosionSoundStyle;
        ExplosionScreenshakeAmt = 3;
    }
    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew(mold: ModContent.ItemType<BlankBag>(), material: ModContent.ItemType<IllurineScale>());
    }
}

public class FrozenPowder : BasePowder
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        //Percent increase, 1 is +100% damage
        DamageModifier = 2f;
        ExplosionType = ModContent.ProjectileType<FrozenBoom>();

        SoundStyle explosionSoundStyle = new SoundStyle($"Stellamod/Assets/Sounds/Green");
        explosionSoundStyle.PitchVariance = 0.15f;
        ExplosionSound = explosionSoundStyle;
        ExplosionScreenshakeAmt = 3;
    }
    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew(mold: ModContent.ItemType<BlankBag>(), material: ModContent.ItemType<IllurineScale>());
    }
}
public class FrozenBoom : BaseIgniterExplosion
{
    public override int FrameCount => 6;
    public override void SetDefaults()
    {
        base.SetDefaults();
        FrameSpeed = 0.5f;
    }

    public override void Start()
    {
        base.Start();
        FXUtil.GlowCircleBoom(Projectile.Center, Color.White, Color.SkyBlue, Color.DarkBlue, duration: 24);
        if (Main.myPlayer == Projectile.owner)
        {
            var circle = EffectsHelper.SimpleExplosionCircle(Projectile, Color.Cyan);
        }
    }

    public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
    {
        base.ModifyHitNPC(target, ref modifiers);
        if (Main.rand.NextBool(3))
        {
            target.AddBuff(BuffID.Frostburn2, 120);
            // target.AddBuff(BuffID.Poisoned, 120);
        }
    }
}
public class TrickPowder : BasePowder
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        //Percent increase, 1 is +100% damage
        DamageModifier = 2f;
        ExplosionType = ModContent.ProjectileType<KaBoomTrick>();

        SoundStyle explosionSoundStyle = new SoundStyle($"Stellamod/Assets/Sounds/trickbomb");
        explosionSoundStyle.PitchVariance = 0.15f;
        ExplosionSound = explosionSoundStyle;
        ExplosionScreenshakeAmt = 6f;
    }
    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew(mold: ModContent.ItemType<BlankBag>(), material: ModContent.ItemType<MiracleThread>());
    }
}

public class EreshkinPowder : BasePowder
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        //Percent increase, 1 is +100% damage
        DamageModifier = 2f;
        ExplosionType = ModContent.ProjectileType<IshBoom>();

        SoundStyle explosionSoundStyle = new SoundStyle("Stellamod/Assets/Sounds/ExplosionGaseous");
        explosionSoundStyle.PitchVariance = 0.15f;
        ExplosionSound = explosionSoundStyle;
        ExplosionScreenshakeAmt = 2;
    }
    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew(mold: ModContent.ItemType<BlankBag>(), material: ModContent.ItemType<EreshkinCandle>());
    }
}
public class PoisonedPowder : BasePowder
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        //Percent increase, 1 is +100% damage
        DamageModifier = 2f;
        ExplosionType = ModContent.ProjectileType<JungleBoom>();

        SoundStyle explosionSoundStyle = new SoundStyle("Stellamod/Assets/Sounds/StaalkerDescend");
        explosionSoundStyle.PitchVariance = 0.15f;
        ExplosionSound = explosionSoundStyle;
        ExplosionScreenshakeAmt = 2f;
    }
    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew(mold: ModContent.ItemType<BlankBag>(), material: ModContent.ItemType<RadiantNectar>());
    }
}

public class RadiantPowder : BasePowder
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        //Percent increase, 1 is +100% damage
        DamageModifier = 2f;
        ExplosionType = ModContent.ProjectileType<RadiantBoom>();


        ExplosionScreenshakeAmt = 2f;
    }
    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew(mold: ModContent.ItemType<BlankBag>(), material: ModContent.ItemType<RadiantNectar>());
    }
}

public class RadiantBoom : BaseIgniterExplosion
{
    public override int FrameCount => 7;
    public override void SetDefaults()
    {
        FrameSpeed = 0.25f;
        base.SetDefaults();

        DrawScale = 1.5f;
    }

    public override void Start()
    {
        base.Start();
        SoundStyle glowSound;
        switch (Main.rand.Next(3))
        {
            default:
            case 0:
                glowSound = new SoundStyle("Stellamod/Assets/Sounds/GoldenSlice1");
                break;
            case 1:
                glowSound = new SoundStyle("Stellamod/Assets/Sounds/GoldenSlice2");
                break;
            case 2:
                glowSound = new SoundStyle("Stellamod/Assets/Sounds/GoldenSlice3");
                break;
        }
        glowSound.PitchVariance = 0.6f;
        SoundEngine.PlaySound(glowSound, Projectile.position);

        var fx = FXUtil.GlowCircleBoom(Projectile.Center, Color.White, Color.Gold, Color.DarkGoldenrod, 15, baseSize: 0.24f);
        fx.Scale *= 2f;
        for(float f =0;f < 4; f++)
        {
            var dp = DustParticle.Spawn(Projectile.Center, Main.rand.NextVector2Circular(24, 24));
            dp.outerColor = Color.Gold;
            dp.dampening = 0.1f;
            dp.Scale *= 0.5f;
            dp.noTileCollide = true;
            dp.dampening = 0.05f;
            dp.gravity = 0;
        }
    }

    public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
    {
        base.ModifyHitNPC(target, ref modifiers);
        if (Main.rand.NextBool(3))
        {
           // target.AddBuff(BuffID.Poisoned, 120);
        }
    }
}

public class SpiritPowder : BasePowder
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        //Percent increase, 1 is +100% damage
        DamageModifier = 2.5f;
        ExplosionType = ModContent.ProjectileType<KaBoomSpirit>();

        SoundStyle explosionSoundStyle = new SoundStyle($"Stellamod/Assets/Sounds/Briskfly");
        explosionSoundStyle.PitchVariance = 0.15f;
        ExplosionSound = explosionSoundStyle;
        ExplosionScreenshakeAmt = 2f;
    }
    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew(mold: ModContent.ItemType<BlankBag>(), material: ModContent.ItemType<AlcaricMush>());
    }
}
public class Verstidust : BasePowder
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        //Percent increase, 1 is +100% damage
        DamageModifier = 2.5f;
        ExplosionType = ModContent.ProjectileType<VerstiExSps>();

        SoundStyle explosionSoundStyle = new SoundStyle($"Stellamod/Assets/Sounds/windpetal");
        explosionSoundStyle.PitchVariance = 0.15f;
        ExplosionSound = explosionSoundStyle;
        ExplosionScreenshakeAmt = 2f;
    }
    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew(mold: ModContent.ItemType<BlankBag>(), material: ModContent.ItemType<FallenEyes>());
    }
}
public class RunicPowder : BasePowder
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        //Percent increase, 1 is +100% damage
        DamageModifier = 2.5f;
        ExplosionType = ModContent.ProjectileType<RunicBoom>();

        SoundStyle explosionSoundStyle = new SoundStyle($"Stellamod/Assets/Sounds/windpetal");
        explosionSoundStyle.PitchVariance = 0.15f;
        ExplosionSound = explosionSoundStyle;
        ExplosionScreenshakeAmt = 2f;
    }
    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew(mold: ModContent.ItemType<BlankBag>(), material: ModContent.ItemType<GhastlySpirit>());
    }
}
public class RunicBoom : BaseIgniterExplosion
{
    private float _timer;
    private AnimationFramer _sunAnimationFrame;
    public override int FrameCount => 24;
    public override void SetDefaults()
    {
        base.SetDefaults();
        FrameSpeed = 0.5f;
    }

    public override void Start()
    {
        base.Start();
        var fx = FXUtil.GlowCircleBoom(Projectile.Center, Color.White, Color.SkyBlue, Color.DarkBlue, 15, baseSize: 0.24f);
        fx.Scale *= 2f;
        for (float f = 0; f < 4; f++)
        {
            var dp = DustParticle.Spawn(Projectile.Center, Main.rand.NextVector2Circular(24, 24));
            dp.outerColor = Color.Blue;
            dp.dampening = 0.1f;
            dp.Scale *= 0.5f;
            dp.noTileCollide = true;
            dp.dampening = 0.05f;
            dp.gravity = 0;
        }
    }

    public override void AI()
    {
        base.AI();
        _timer++;
        _sunAnimationFrame.frameSpeed = 1;
        _sunAnimationFrame.maxFrame = 6 * 4;
        _sunAnimationFrame.UpdateTick();
    }

    public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
    {
        base.ModifyHitNPC(target, ref modifiers);
        if (Main.rand.NextBool(3))
        {
            // target.AddBuff(BuffID.Poisoned, 120);
        }
    }
    protected override void DrawPixelExplosion(SpriteBatch spriteBatch, Vector2 screenPos)
    {
        PalettizerShader shader = PalettizerShader.Use(PaletteAssets.MOONSPIRALTOWER);
        SpritebatchParams @params = SpritebatchParams.InWorldAndZoomed() with { effect = shader, sortMode = SpriteSortMode.Immediate };
        float a = EasingFunction.InOutSine(_timer / 40f);
        using (SpritebatchStarter.Begin(spriteBatch, @params))
        {
            SpritebatchDrawer drawer = SpritebatchDrawer.FromProjectile(Projectile);
            drawer.color = Color.White  * MathHelper.Lerp(1f, 0f, a);
            drawer.color.A = 0;// (byte)(MathHelper.Lerp(255, 0, 0.5f));

            Rectangle sunFrame = drawer.texture.GetFrame(_sunAnimationFrame.frame, 6, 4);
            drawer.sourceRect = sunFrame;
            drawer.CenterOrigin();
            spriteBatch.Draw(drawer);
        }
    }
}
public class WinglessPowder : BasePowder
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        //Percent increase, 1 is +100% damage
        DamageModifier = 2.5f;
        ExplosionType = ModContent.ProjectileType<WinglessBoom>();

        ExplosionScreenshakeAmt = 2f;
    }
    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew(mold: ModContent.ItemType<BlankBag>(), material: ModContent.ItemType<MothlightWing>());
    }
}

public class WinglessBoom : BaseIgniterExplosion
{
    public override int FrameCount => 18;
    public override void SetDefaults()
    {
        FrameSpeed = 0.5f;
        base.SetDefaults();
        DrawScale = 2.5f;
    
    }

    public override void Start()
    {
        base.Start();
        PixelPrimitiveCircleFactory.CreateInGoldBoom(Projectile.Center);
        var fx = FXUtil.GlowCircleBoom(Projectile.Center, Color.White, Color.LightGoldenrodYellow, Color.Gold, 15, baseSize: 0.24f);
 
        for (float f = 0; f < 4; f++)
        {
            var dp = DustParticle.Spawn(Projectile.Center, Main.rand.NextVector2Circular(24, 24));
            dp.outerColor = Color.Gold;
            dp.dampening = 0.1f;
            dp.Scale *= 0.5f;
            dp.noTileCollide = true;
            dp.dampening = 0.05f;
            dp.gravity = 0;
        }

        SoundStyle glowSound;
        switch (Main.rand.Next(3))
        {
            default:
            case 0:
                glowSound = new SoundStyle("Stellamod/Assets/Sounds/GW1");
                break;
            case 1:
                glowSound = new SoundStyle("Stellamod/Assets/Sounds/GW2");
                break;
            case 2:
                glowSound = new SoundStyle("Stellamod/Assets/Sounds/GW3");
                break;
        }
        glowSound.Volume = 0.6f;
        glowSound.PitchVariance = 0.6f;
        SoundEngine.PlaySound(glowSound, Projectile.position);
    }

    public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
    {
        base.ModifyHitNPC(target, ref modifiers);
        if (Main.rand.NextBool(3))
        {
            // target.AddBuff(BuffID.Poisoned, 120);
        }
    }
    protected override void DrawPixelExplosion(SpriteBatch spriteBatch, Vector2 screenPos)
    {
        PalettizerShader shader = PalettizerShader.Use(PaletteAssets.PERFECT);
        SpritebatchParams @params = SpritebatchParams.InWorldAndZoomed() with { effect = shader, sortMode = SpriteSortMode.Immediate };
        using (SpritebatchStarter.Begin(spriteBatch, @params))
        {
            SpritebatchDrawer drawer = SpritebatchDrawer.FromProjectile(Projectile);
            drawer.color = Color.White;
            drawer.color.A = 0;
            drawer.scale *= 1.5f;
            spriteBatch.Draw(drawer);
        }
    }
}

public class AlcaricPowder : BasePowder
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        //Percent increase, 1 is +100% damage
        DamageModifier = 2.5f;
        ExplosionType = ModContent.ProjectileType<AlcaBoom>();

        ExplosionScreenshakeAmt = 2f;
    }
    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew(mold: ModContent.ItemType<BlankBag>(), material: ModContent.ItemType<AlcaricMush>());
    }
}

public class AlcaBoom : BaseIgniterExplosion
{
    public override int FrameCount => 10;
    public override void SetDefaults()
    {
        FrameSpeed = 0.5f;
        base.SetDefaults();
 
    }

    public override void Start()
    {
        base.Start();
    
        var fx = FXUtil.GlowCircleBoom(Projectile.Center, Color.Purple, Color.Black, Color.White, 15, baseSize: 0.24f);



        SoundStyle glowSound;
        switch (Main.rand.Next(2))
        {
            default:
            case 0:
                glowSound = new SoundStyle("Stellamod/Assets/Sounds/Magic/AutomationCast1");
                break;
            case 1:
                glowSound = new SoundStyle("Stellamod/Assets/Sounds/Magic/AutomationCast2");
                break;
        }
        glowSound.Volume = 0.6f;
        glowSound.PitchVariance = 0.6f;
        SoundEngine.PlaySound(glowSound, Projectile.position);
    }

    public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
    {
        base.ModifyHitNPC(target, ref modifiers);
        if (Main.rand.NextBool(3))
        {
            // target.AddBuff(BuffID.Poisoned, 120);
        }
    }
    protected override void DrawPixelExplosion(SpriteBatch spriteBatch, Vector2 screenPos)
    {
        PalettizerShader shader = PalettizerShader.Use(PaletteAssets.ROYALCAPITAL);
        SpritebatchParams @params = SpritebatchParams.InWorldAndZoomed() with { effect = shader, sortMode = SpriteSortMode.Immediate };
        using (SpritebatchStarter.Begin(spriteBatch, @params))
        {
            SpritebatchDrawer drawer = SpritebatchDrawer.FromProjectile(Projectile);
            drawer.color = Color.White;
            drawer.color.A = 0;
            drawer.scale *= 0.6f;
            spriteBatch.Draw(drawer);
        }
    }
}