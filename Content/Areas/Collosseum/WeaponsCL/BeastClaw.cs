using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Common.Shaders.MagicTrails;
using Stellamod.Content.CommonMaterials;
using Stellamod.Core.Bases;
using Stellamod.Core.Pixelation;
using Stellamod.Core.SwingSystem;
using Stellamod.Helpers;
using Stellamod.Items;
using Stellamod.Items.Ores;
using Stellamod.Trails;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Collosseum.WeaponsCL;


public class BeastClaw : BaseSwingItemV2
{
    public override void SetDefaults2()
    {
        base.SetDefaults2();
        Item.damage = 12;
        Item.shoot = ModContent.ProjectileType<BeastClawSlash>();
        staminaProjectileShoot = ModContent.ProjectileType<BeastClawStaminaSlash>();
        meleeWeaponType = MeleeWeaponType.Knives;
        staminaDamageMultiplier = 2;
    }

    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew<
            GintzlMetal,
            BlankSword>();
    }
}

public class BeastClawSlash : BaseSwingProjectileV2
{
    private bool _hasSpawnedSecondKnife;
    public override void DefineCombo()
    {
        base.DefineCombo();
        SwingV2Helper.AddKnivesSwingStyle(this);
        useAfterImage = true;

       // swordBeamLength = 90;
        outlineColor = Color.DarkGray;
        glowAfterImageColor = Color.Goldenrod * 0.1f;
        hitStopTime = 0;
        bigSwingTrailOffset = -32;
        //      hitStopTime = EXTRA_UPDATE_COUNT * 2;
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
            shader.LaserColor = Color.Lerp(Color.White, Color.DarkGray, ExtraMath.Osc(0f, 1f, speed: 8, offset: 2));
            shader.InnerColor = Color.Lerp(Color.White, Color.DarkGray, ExtraMath.Osc(0f, 1f, speed: 8));
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
    }

    public override void AI()
    {
        base.AI();
        if (!_hasSpawnedSecondKnife && ComboIndex != ComboCount - 1 && Interpolant >= 0.75f)
        {
            TrueCloneProjectile();
            _hasSpawnedSecondKnife = true;
        }
        float strength = 0.15f;
        foreach (var npc in Main.ActiveNPCs)
        {
            GlobalNPCSucker npcSucker = npc.GetGlobalNPC<GlobalNPCSucker>();
            float dist = Vector2.Distance(Projectile.Center, npc.Center);

            Vector2 normal = (npc.Center - Owner.Center).SafeNormalize(Vector2.Zero);
            Vector2 projNormal = Projectile.velocity.SafeNormalize(Vector2.Zero);
            float dp = Vector2.Dot(normal, projNormal);
            if (dist <= 384 && dp > 0)
            {
                Vector2 suckPosition = Projectile.Center + Projectile.velocity.SafeNormalize(Vector2.Zero) * 128;
                Vector2 diff = suckPosition - npc.Center;
                Vector2 velocity = Vector2.Lerp(Vector2.Zero, diff, strength) * npc.knockBackResist;
                Vector2 diffVelocity = velocity - npcSucker.SuckVelocity;
                npcSucker.SuckVelocity += diffVelocity;
            }
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
    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D texture = AssetManager.GlowMask.SpiralVortex2.Value;
        SpritebatchDrawer vortexDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.SpiralVortex, Projectile.Center);
        vortexDrawer.color = Color.SandyBrown;
        vortexDrawer.color *= 0.35f;
        vortexDrawer.color *= EasingFunction.QuadraticBump(Interpolant);
        vortexDrawer.color.A = 0;
        vortexDrawer.rotation = Main.GlobalTimeWrappedHourly * 6f;
        vortexDrawer.scale = Vector2.One * 0.6f;
        SpriteBatch spriteBatch = Main.spriteBatch;
        spriteBatch.Draw(vortexDrawer);

        return base.PreDraw(ref lightColor);
    }
}

public class BeastClawStaminaSlash : BaseSwingProjectileV2
{
    private bool _hasSpawnedSecondKnife;
    public override void DefineCombo()
    {
        base.DefineCombo();
        ComboBuilder comboBuilder = new ComboBuilder();
        comboBuilder.AddChakramSpin2(duration: 90, xSwingRadius: 64, ySwingRadius: 64, hitCount: 32, swingDegrees: 1440*2);
        comboBuilder.AddToProjectile(this);
        useAfterImage = true;

       // swordBeamLength = 90;
        outlineColor = Color.DarkGray;
        glowAfterImageColor = Color.Goldenrod * 0.1f;
        hitStopTime = 0;
        bigSwingTrailOffset = -32;
        //      hitStopTime = EXTRA_UPDATE_COUNT * 2;
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
            shader.LaserColor = Color.Lerp(Color.White, Color.DarkGray, ExtraMath.Osc(0f, 1f, speed: 8, offset: 2));
            shader.InnerColor = Color.Lerp(Color.White, Color.DarkGray, ExtraMath.Osc(0f, 1f, speed: 8));
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
    }

    public override void AI()
    {
        base.AI();
        if (!_hasSpawnedSecondKnife && Interpolant > 0.2f)
        {
            TrueCloneProjectile();

            /*
            if (this.OwnedByLocalClient())
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Owner.Center, Projectile.velocity,
                    ModContent.ProjectileType<BeastClawWindGust>(), Projectile.damage, Projectile.knockBack * 8, Projectile.owner);
            }*/
            _hasSpawnedSecondKnife = true;
        }

        if (Timer % 32 == 0)
        {
            Vector2 pos = Owner.Bottom;
            float dir = Main.rand.NextBool(2) ? 1 : -1;
            Vector2 velocity = Vector2.UnitX * dir * 8;
            ThickSmokeParticle ts = ThickSmokeParticle.Spawn(pos, velocity, Scale: 2);
            ts.expand = true;
            ts.color = Color.SandyBrown * 0.6f;
        }

        if(Timer % 16 == 0)
        {
            Vector2 pos = Owner.Bottom;
            float dir = Main.rand.NextBool(2) ? 1 : -1;
            Vector2 velocity = Vector2.UnitX * dir * 6;
            velocity.Y -= 5 * Main.rand.NextFloat(0.4f, 0.8f);
            pos += Main.rand.NextVector2Circular(24, 24);
            Dust d = Dust.NewDustPerfect(pos, DustID.Sand, velocity);
            d.noGravity = false;

        }



        if (Timer % 64 == 0)
        {
            Vector2 pos = Owner.Bottom;
            float dir = Main.rand.NextBool(2) ? 1 : -1;
            Vector2 velocity = Vector2.UnitX * dir * 6;
            velocity.Y -= 5 * Main.rand.NextFloat(0.4f, 0.8f);
            pos += Main.rand.NextVector2Circular(128, 128);
            pos += Main.rand.NextVector2CircularEdge(32, 32);
            SwirlParticle sp = SwirlParticle.Spawn(pos, Vector2.Zero);
            sp.color *= 0.3f;
        }

        float strength = 0.15f;
        foreach (var npc in Main.ActiveNPCs)
        {
            GlobalNPCSucker npcSucker = npc.GetGlobalNPC<GlobalNPCSucker>();
            float dist = Vector2.Distance(Projectile.Center, npc.Center);
            if (dist <= 384)
            {
                Vector2 suckPosition = Projectile.Center;
                suckPosition.Y -= 64;
                suckPosition.Y += ExtraMath.Osc(0f, 32, 0, npc.whoAmI);

                Vector2 diff = suckPosition - npc.Center;
                Vector2 velocity = Vector2.Lerp(Vector2.Zero, diff, strength) * npc.knockBackResist;
                Vector2 diffVelocity = velocity - npcSucker.SuckVelocity;
                npcSucker.SuckVelocity += diffVelocity;
            }
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
    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D texture = AssetManager.GlowMask.SpiralVortex2.Value;
        SpritebatchDrawer vortexDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.SpiralVortex, Projectile.Center);
        vortexDrawer.color = Color.Lerp(Color.Black, Color.White, EasingFunction.QuadraticBump(Timer / 60f)) * 0.2f;
        vortexDrawer.color.A = 0;
        vortexDrawer.rotation = Projectile.rotation;
        vortexDrawer.scale = Vector2.One * EasingFunction.QuadraticBump(Timer / 60f) * 0.3f;
        SpriteBatch spriteBatch = Main.spriteBatch;
        spriteBatch.Draw(vortexDrawer);

        vortexDrawer.color = Color.Lerp(Color.Black, Color.SandyBrown, EasingFunction.QuadraticBump(Timer / 60f)) * 0.2f;
        vortexDrawer.color.A = 0;
        vortexDrawer.rotation = Projectile.rotation + Main.GlobalTimeWrappedHourly * 2f;
        vortexDrawer.scale *= 2f;
        spriteBatch.Draw(vortexDrawer);
        return base.PreDraw(ref lightColor);
    }
}

public class BeastClawWindGust : ModProjectile
{
    private ref float Timer => ref Projectile.ai[0];
    public override string Texture => TextureRegistry.EmptyTexture;
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        ProjectileID.Sets.TrailCacheLength[Type] = 32;
        ProjectileID.Sets.TrailingMode[Type] = 2;
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 64;
        Projectile.height = 64;
        Projectile.friendly = true;
        Projectile.tileCollide = false;
        Projectile.penetrate = -1;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 30;
        Projectile.timeLeft = 180;
        Projectile.extraUpdates = 1;
    }

    public override void AI()
    {
        base.AI();
        Timer++;
    }
    public override bool PreDraw(ref Color lightColor)
    {
        PixelationManager.QueuePrimitivesDrawAction(DrawPixelatedWindTrail, DrawLayer.OverNPCs);
        return false;
    }

    private void DrawPixelatedWindTrail(GraphicsDevice graphicsDevice)
    {
        var shader = MagicRadianceShader.Instance;
        shader.PrimaryTexture = TrailRegistry.GlowTrail;
        shader.NoiseTexture = TrailRegistry.CloudsSmall;
        shader.OutlineTexture = TrailRegistry.DottedTrailOutline;
        shader.PrimaryColor = Color.White;
        shader.NoiseColor = Color.LightGray;
        shader.OutlineColor = Color.LightGray;
        shader.BlendState = BlendState.Additive;
        shader.SamplerState = SamplerState.PointWrap;
        shader.Speed = 5.2f;
        shader.Distortion = 0.15f;
        shader.Power = 0.25f;



        TrailDrawer.Draw(Main.spriteBatch, Projectile.oldPos, null, StripColors, StripWidth, shader, Projectile.Size * 0.5f);
    }

    private Color StripColors(float progressOnStrip)
    {
        Color stripColor = Color.Lerp(Color.Black, Color.LightGray, EasingFunction.QuadraticBump(progressOnStrip));
        stripColor *= EasingFunction.QuadraticBump(progressOnStrip);
        return stripColor;
    }


    private float StripWidth(float progressOnStrip)
    {
        float maxWidth = 36;
        float width = MathHelper.Lerp(maxWidth * 0.8f, maxWidth, EasingFunction.QuadraticBump(progressOnStrip));
        float outScale = Timer / 60f;
        float outEasedScale = MathHelper.Lerp(1f, 0f, EasingFunction.InOutSine(outScale));
        return width * outEasedScale;
    }
}