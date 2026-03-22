using ReLogic.Content;
using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Content.CommonMaterials;
using Stellamod.Core.Bases;
using Stellamod.Core.Pixelation;
using Stellamod.Core.SwingSystem;
using Stellamod.Helpers;
using Stellamod.Items;
using Stellamod.Items.Materials;
using Stellamod.UI.Systems;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Falling.WeaponsF;

public class LightKnives : BaseSwingItemV2
{
    public override void SetDefaults2()
    {
        base.SetDefaults2();
        Item.damage = 256;
        Item.shoot = ModContent.ProjectileType<LightKnivesSlash>();
        staminaProjectileShoot = ModContent.ProjectileType<LightKnivesDash>();
        meleeWeaponType = MeleeWeaponType.Knives;
    }

    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew<
            AlcaricMush,
            BlankSword>();
    }
}


public class LightSpasm : ModProjectile
{
    private ref float Timer => ref Projectile.ai[0];
    private ref float Style => ref Projectile.ai[1];
    public override string Texture => TextureRegistry.EmptyTexture;
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        ProjectileID.Sets.TrailCacheLength[Type] = 16;
        ProjectileID.Sets.TrailingMode[Type] = 2;
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 16;
        Projectile.height = 16;
        Projectile.friendly = true;
        Projectile.timeLeft = 60;
        Projectile.extraUpdates = 3;
        Projectile.tileCollide = false;
        Projectile.ignoreWater = true;
        Projectile.penetrate = 3;
        Projectile.usesIDStaticNPCImmunity = true;
        Projectile.idStaticNPCHitCooldown = 15;
    }

    public override void AI()
    {
        base.AI();
 

        Timer++;
        if(Timer == 1)
        {
            SoundStyle zap = SoundID.DD2_LightningAuraZap;
            zap.PitchVariance = 0.2f;
            SoundEngine.PlaySound(zap, Projectile.Center);
        }
        if (Timer % 16 == 0)
        {
            DustParticleSpawnParams spawnParams = DustParticleSpawnParams.Default;
            spawnParams.outerColor = Color.Gold;
            spawnParams.scaleRange *= 0.5f;
            DustParticle dp = DustParticle.Spawn(Projectile.Center, -Projectile.velocity, spawnParams);
            dp.gravity = 0;
            dp.fast = true;
        }
        if (this.OwnedByLocalClient())
        {
            if (Timer % 15 == 0)
            {
                Projectile.velocity = Projectile.velocity.RotatedByRandom(MathHelper.ToRadians(45));
                Projectile.netUpdate = true;
            }
        }

        Projectile.velocity *= 1.01f;
    }


    private Color GetTrailColor(float completionRatio)
    {
        return Color.Lerp(Color.White, Color.Black, completionRatio);
    }

    private float GetTrailWidth(float completionRatio)
    {
        float outScale = EasingFunction.InOutSine((float)Projectile.timeLeft / 30f);
        if (Style == 1)
        {
            outScale *= 0.5f;
        }
        return MathHelper.SmoothStep(8, 0, completionRatio) * outScale;
    }

    private void RenderLightningZaps(GraphicsDevice gDevice)
    {
        var shader = RichLaserShader.Instance;
        shader.LaserColor = Color.White;
        shader.InnerColor = Color.Lerp(Color.LightGoldenrodYellow, Color.Goldenrod, ExtraMath.Osc(0f, 1f, speed: 8));
        shader.OuterColor = Color.DarkGoldenrod;
        shader.LaserTexture = AssetManager.LaserTextures.TexturedLaser;
        shader.BloomTexture = AssetManager.LaserTextures.Lightning2;
        TrailDrawer.Draw(Main.spriteBatch, Projectile.oldPos, GetTrailColor, GetTrailWidth, shader, Projectile.Size * 0.5f);
    }

    private void RenderBloom(SpriteBatch spriteBatch, Vector2 screenPos)
    {
        Texture2D bloomTexture = AssetManager.GlowMask.SimpleGlowCircle.Value;
        Vector2 glowScale = Vector2.One * 0.25f;
        float rotation = Main.GlobalTimeWrappedHourly * 4;
        float outScale = EasingFunction.InOutSine((float)Projectile.timeLeft / 30f) * 0.25f;
        SpritebatchDrawer bloomDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.SimpleGlowCircle, Projectile.Center);
        bloomDrawer.color = Color.Goldenrod;
        bloomDrawer.color.A = 0;
        bloomDrawer.color *= 0.1f;
        bloomDrawer.color *= outScale;
        spriteBatch.Draw(bloomDrawer);
        for (int i = 0; i < Projectile.oldPos.Length; i += 2)
        {
            Vector2 pos = Projectile.oldPos[i];

            Color glowColor = Color.Lerp(Color.White, Color.Goldenrod, 0.6f);
            glowColor.A = 0;
            glowColor *= 0.2f;
            glowColor *= ExtraMath.Osc(0.6f, 1f, speed: 6, offset: i);
            glowColor *= outScale;

            bloomDrawer.worldPosition = pos;
            bloomDrawer.color = glowColor;
            bloomDrawer.scale = glowScale;
            spriteBatch.Draw(bloomDrawer);
        }

        SpritebatchDrawer spiralDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.SpiralVortex, Projectile.Center);

        Color spiralGlowColor = Color.Lerp(Color.White, Color.Goldenrod, 0.6f);
        spiralGlowColor.A = 0;
        spiralGlowColor *= 0.2f;
        spiralGlowColor *= ExtraMath.Osc(0.6f, 1f, speed: 6);
        spiralGlowColor *= outScale;
        spiralDrawer.color = spiralGlowColor;
        spriteBatch.Draw(spiralDrawer);
    }


    public override bool PreDraw(ref Color lightColor)
    {
        PixelationManager.QueueSpritebatchDrawAction(RenderBloom);
        PixelationManager.QueuePrimitivesDrawAction(RenderLightningZaps);
        return false;
    }

    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
    }
}

public class LightKnivesSlash : BaseSwingProjectileV2
{
    private bool _hasSpawnedSecondKnife;
    public override void DefineCombo()
    {
        base.DefineCombo();
        SwingV2Helper.AddKnivesSwingStyle(this);
        useAfterImage = true;

        swordBeamLength = 90;
        outlineColor = Color.DarkGoldenrod;
        glowAfterImageColor = Color.Goldenrod * 0.1f;
        hitStopTime = 0;
  //      hitStopTime = EXTRA_UPDATE_COUNT * 2;
    }

    public override Asset<Texture2D> RequestHologramTexture()
    {
        return TextureRegistry.GlowSword_LightKnives;
    }
    private Color GetTrailColor(float completionRatio)
    {
        return Color.Lerp(Color.White, Color.Black, completionRatio);
    }

    private float GetTrailWidth(float completionRatio)
    {
        return MathHelper.SmoothStep(12, 0, completionRatio) * EasingFunction.QuadraticBump(Interpolant) * EasingFunction.QuadraticBump(completionRatio);
    }

    public override void RenderSwingTrail(ref Color lightColor, Vector2[] points)
    {
        base.RenderSwingTrail(ref lightColor, points);
        void DrawPixelatedSwingTrail(GraphicsDevice gDevice)
        {
            var shader = RichLaserShader.Instance;
            shader.LaserColor = Color.Lerp(Color.LightGoldenrodYellow, Color.Goldenrod, ExtraMath.Osc(0f, 1f, speed: 8, offset: 2));
            shader.InnerColor = Color.Lerp(Color.LightGoldenrodYellow, Color.Goldenrod, ExtraMath.Osc(0f, 1f, speed: 8));
            shader.OuterColor = Color.Red;
            shader.LaserTexture = AssetManager.LaserTextures.TexturedLaser;
            shader.BloomTexture = AssetManager.LaserTextures.Lightning2;
            TrailDrawer.Draw(Main.spriteBatch, points, GetTrailColor, GetTrailWidth, shader);
        }
        PixelationManager.QueuePrimitivesDrawAction(DrawPixelatedSwingTrail);
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        base.OnHitNPC(target, hit, damageDone);
        Vector2 shootVelocity = Main.rand.NextVector2CircularEdge(8, 8);

        int damage = (int)(Projectile.damage * 0.3f);
        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, shootVelocity,
            ModContent.ProjectileType<LightSpasm>(), damage, Projectile.knockBack, Projectile.owner, ai1: 1);
    }

    public override void AI()
    {
        base.AI();
        if (!_hasSpawnedSecondKnife && ComboIndex != ComboCount - 1 && Interpolant >= 0.9f)
        {
            CloneProjectile();
            _hasSpawnedSecondKnife = true;
        }

        growScale = MathHelper.Lerp(0f, 0.3f, EasingFunction.QuadraticBump(Interpolant));
        glowColor = Color.Lerp(Color.Black, Color.Goldenrod, EasingFunction.QuadraticBump(Interpolant));
    }


    public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
    {
        base.ModifyHitNPC(target, ref modifiers);
        SoundStyle spearHit = SoundRegistry.SpearHit1;
        spearHit.PitchVariance = 0.5f;
        SoundEngine.PlaySound(spearHit, Projectile.position);
        if (ComboIndex == 5)
        {
            StatModifier statModifier = new StatModifier(0.5f, 1f);
            modifiers.FinalDamage.CombineWith(statModifier);
        }
    }
}

public class LightKnivesDash : ModProjectile
{
    public override string Texture => TextureRegistry.EmptyTexture;
    private ref float Timer => ref Projectile.ai[0];
    private int TargetNPC
    {
        get => (int)Projectile.ai[1];
        set => Projectile.ai[1] = value;
    }
    private Player Owner => Main.player[Projectile.owner];
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 64;
        Projectile.height = 64;
        Projectile.friendly = true;
        Projectile.timeLeft = 180;
        Projectile.extraUpdates = 1;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 30;
        Projectile.tileCollide = false;
        Projectile.penetrate = -1;
    }

    public override void AI()
    {
        base.AI();
        Timer++;
        if(Timer == 1)
        {
            TargetNPC = -1;
            Projectile.velocity *= 0.1f;
            SoundStyle shocKSound = AssetRegistry.Sounds.LeviathanEel.Electrify;
            SoundEngine.PlaySound(shocKSound, Projectile.Center);
        }
  
        if(Timer > 5)
        {
            if(TargetNPC > -1)
            {
                NPC npc = Main.npc[TargetNPC];
                if (!npc.active)
                    Projectile.Kill();
                else
                {
                    Projectile.velocity = (npc.Center - Projectile.Center);
                    ShakeModSystem.Shake = 1;
                    if(Timer % 8 == 0)
                    {
                        SoundStyle zap = SoundID.DD2_LightningAuraZap;
                        zap.PitchVariance = 0.2f;
                        SoundEngine.PlaySound(zap, Projectile.Center);
                        if (this.OwnedByLocalClient())
                        {
                            Vector2 shootVelocity = Main.rand.NextVector2CircularEdge(8, 8);

                            int damage = (int)(Projectile.damage * 0.3f);
                            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, shootVelocity,
                                ModContent.ProjectileType<LightSpasm>(), damage, Projectile.knockBack, Projectile.owner);
                        }
                        FXUtil.GlowCircleBoom(npc.Center, Color.Yellow, Color.Goldenrod, Color.DarkGoldenrod, duration: 5, Main.rand.NextFloat(0.06f, 0.2f));
                    }

                }
            }
        
        }
        if (Timer % 4 == 0)
        {
            ThrustParticle thrustParticle = ThrustParticle.Spawn(Projectile.Center, Projectile.velocity * 2, Color.White, Scale: 1f);
            thrustParticle.bloomColor = Color.Goldenrod;
        }
        if (Timer % 8 == 0)
        {
            DustParticleSpawnParams spawnParams = DustParticleSpawnParams.Default;
            spawnParams.outerColor = Color.Gold;
            spawnParams.scaleRange *= 0.5f;
            Vector2 velocity = -Projectile.velocity ;
            DustParticle dp = DustParticle.Spawn(Projectile.Center, velocity, spawnParams);
            dp.gravity = 0;
        }
        if(Projectile.velocity.Length() < 35)
            Projectile.velocity *= 1.2f;
        Projectile.rotation = Projectile.velocity.ToRotation();
    }

    private void RenderPixelatedDash(SpriteBatch spriteBatch, Vector2 screenPos)
    {

    }

    public override bool PreDraw(ref Color lightColor)
    {
        PixelationManager.QueueSpritebatchDrawAction(RenderPixelatedDash);
        return false;
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        base.OnHitNPC(target, hit, damageDone);
        if(TargetNPC == -1)
        {
            TargetNPC = target.whoAmI;
            Projectile.netUpdate = true;
        }
    }

    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
    }
}
