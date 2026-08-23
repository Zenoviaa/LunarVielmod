using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Content.Areas.Cinderspark.WeaponsCS;
using Stellamod.Content.Areas.PunkerTown.ItemsPT;
using Stellamod.Content.CommonMaterials;
using Stellamod.Core.Pixelation;
using Stellamod.Core.SwingSystem;
using Stellamod.Items;
using Stellamod.Visual.Particles;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Cinderspark.AccCS;

public class BurningGlove : AbstractMeleeAddon
{
    private Dictionary<int, (float, float)> _fireTimer = new Dictionary<int, (float, float)>();
    private Dictionary<int, bool> _hasShotSwingProj = new Dictionary<int, bool>();
    public override void OnSpawn(BaseSwingProjectileV2 projectile)
    {
        base.OnSpawn(projectile);
        int id = projectile.Projectile.identity;
        if (_hasShotSwingProj.ContainsKey(id))
            _hasShotSwingProj[id] = false;
        else
            _hasShotSwingProj.Add(id, false);


        if (_fireTimer.ContainsKey(id))
            _fireTimer[id] = (0, 0);
        else
            _fireTimer.Add(id, (0, 0));
    }


    public override void AI(BaseSwingProjectileV2 projectile)
    {
        base.AI(projectile);
        if (!projectile.OwnedByLocalClient())
            return;
        var proj = projectile.Projectile;
        int id = projectile.Projectile.identity;

        if (_fireTimer.ContainsKey(id))
        {
            (float oldRot, float traveled) = _fireTimer[id];
            traveled += MathF.Abs(proj.rotation - oldRot);
            oldRot = proj.rotation;

            if (traveled >= 0.4f)
            {
                Projectile.NewProjectile(proj.GetSource_FromAI(), proj.Center, proj.rotation.ToRotationVector2() * 2,
                ModContent.ProjectileType<IncineratorProj>(), (int)(proj.damage * 0.25f), proj.knockBack, proj.owner);
                traveled = 0;
                _fireTimer[id] = (oldRot, traveled);
            }
        }

        if (!projectile.IsThrust())
            return;

        if (!_hasShotSwingProj.ContainsKey(id))
            return;

        if (!_hasShotSwingProj[id] && projectile.Interpolant >= 0.1f)
        {
            for (int i = 0; i < 2; i++)
            {
                Projectile.NewProjectile(projectile.Projectile.GetSource_FromAI(), projectile.Owner.Center,
                              projectile.Projectile.velocity.SafeNormalize(Vector2.Zero).RotatedByRandom(0.1) * 15, ModContent.ProjectileType<MoltenManaBlast>(),
                              (int)(projectile.Projectile.damage * 0.45f), projectile.Projectile.knockBack,
                              projectile.Projectile.owner);
            }

            _hasShotSwingProj[id] = true;
        }
    }

    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew<Cinderscrap, BlankAccessory>();
    }
}
public class BurningGloveFlamethrower : ModProjectile
{
    public override string Texture => TextureRegistry.EmptyTexture;

    private ref float Timer => ref Projectile.ai[0];
    private Vector2[] IncineratorPos;
    private float LifeTime => 32;
    private int NumPoints => 64;
    public override void SetDefaults()
    {
        base.SetDefaults();
        IncineratorPos = new Vector2[NumPoints];
        Projectile.width = 150;
        Projectile.height = 150;
        Projectile.friendly = true;
        Projectile.hostile = false;
        Projectile.penetrate = -1;
        Projectile.idStaticNPCHitCooldown = 7;
        Projectile.usesIDStaticNPCImmunity = true;
        Projectile.tileCollide = false;
        Projectile.timeLeft = (int)LifeTime;
        Projectile.light = 0.7f;
    }

    public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
    {
        bool? e = base.Colliding(projHitbox, targetHitbox);
        if (e.HasValue && e.Value)
            return true;
        return ProjectileHelper.OldPosColliding(IncineratorPos, projHitbox, targetHitbox, 16);
    }
    public override bool ShouldUpdatePosition()
    {
        return true;
    }

    public override void AI()
    {
        if (Main.rand.NextBool(16))
        {
            Color color = new Color(41, 43, 66);
            var sp2 = FaintSmokeParticle.SpawnInAlphaLayer(Projectile.Center + Main.rand.NextVector2Circular(32, 32), Vector2.Zero * 0.02f);
            sp2.color = Color.Lerp(color, Color.White, 0.25f) * 0.5f;
            sp2.Scale *= 0.35f;
            sp2.fadeToColor = Color.Black * 0.5f;
        }
        ProjectileID.Sets.TrailCacheLength[Type] = 8;
        ProjectileID.Sets.TrailingMode[Type] = 2;
        IncineratorPos = Projectile.oldPos;
        Timer++;
        if (Timer == 1 && Main.rand.NextBool(8))
        {
            SoundEngine.PlaySound(SoundID.DD2_BetsyFireballShot, Projectile.position);
            SoundEngine.PlaySound(SoundID.DD2_EtherianPortalSpawnEnemy, Projectile.position);
        }

        Projectile.velocity = Projectile.velocity.RotatedBy(0.12f);
        //   Lighting.AddLight(Projectile.Center + Projectile.velocity * 64, TorchID.Torch); 
        Projectile.rotation += 0.05f;
    }

    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        return false;
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        base.OnHitNPC(target, hit, damageDone);
        target.AddBuff(BuffID.OnFire3, 120);
    }

    private float WidthFunction(float completionRatio)
    {
        return MathHelper.Lerp(56, 0, completionRatio) * EasingFunction.QuadraticBump(Timer / LifeTime);
    }

    private Color ColorFunction(float completionRatio)
    {
        return Color.White;
    }
    public float SmokeWidthFunction(float completionRatio)
    {
        return WidthFunction(completionRatio) * 0.85f;
    }

    public Color SmokeColorFunction(float completionRatio)
    {
        return ColorFunction(completionRatio) * 0.5f;
    }
    private Color ColorFunction2(float completionRatio)
    {
        Color finalColor2 = Color.White;
        finalColor2 *= EasingFunction.QuadraticBump(completionRatio);
        float progress = Timer / LifeTime;
        float o2 = MathHelper.Lerp(1f, 0f, progress);
        finalColor2 *= o2;
        finalColor2 *= MathHelper.Lerp(1f, 0f, EasingFunction.InExpo(completionRatio));
        finalColor2 *= EasingFunction.QuadraticBump(completionRatio);
        return finalColor2;
    }
    private float WidthFunction2(float completionRatio)
    {
        float width = 96;
        float w = MathHelper.SmoothStep(16, width, completionRatio);
        float o = MathHelper.Lerp(1f, 0f, EasingFunction.InCirc(completionRatio));
        float progress = Timer / LifeTime;
        float o2 = MathHelper.Lerp(1f, 2f, progress);
        float i = MathHelper.Lerp(0f, 1f, EasingFunction.OutExpo(progress));
        return w * o * o2 * i;
    }
    private float GetBloomWidth(float ratio)
    {
        return MathHelper.SmoothStep(44, 8, ratio) * 1.5f * EasingFunction.QuadraticBump(Timer / LifeTime); ;
    }
    private Color GetBloomColor(float ratio)
    {
        return Color.Lerp(Color.Transparent, Color.Red, EasingFunction.InOutSine(ratio));
    }
    private void DrawMainShader(Vector2[] oldPos)
    {
        BlackFireShader blackFireShader = new BlackFireShader();
        blackFireShader.SetDefaults();
        TrailDrawer.Draw(Main.spriteBatch, oldPos, ColorFunction, WidthFunction, blackFireShader, Projectile.Size * 0.5f);

        BloomTrailShader bloomTrailShader = BloomTrailShader.Instance;
        bloomTrailShader.InnerColor = Color.White;
        bloomTrailShader.OuterColor = Color.Red;
        TrailDrawer.Draw(Main.spriteBatch, oldPos, GetBloomColor, GetBloomWidth, bloomTrailShader, Projectile.Size * 0.5f);
    }

    private void DrawPixelatedFlames(GraphicsDevice graphicsDevice)
    {
        DrawMainShader(IncineratorPos);
    }
    private void DrawPixelatedCore(SpriteBatch sb, Vector2 screenPos)
    {
        float alpha = EasingFunction.QuadraticBump(Timer / LifeTime);
        SpriteBatch spriteBatch = Main.spriteBatch;
        Vector2 drawPos = Projectile.Center - Main.screenPosition;
        Texture2D glowMask = AssetManager.GlowMask.SimpleGlowCircle.Value;
        Vector2 glowDrawOrigin = glowMask.Size() / 2f;
        Color glowColor = Color.Lerp(Color.OrangeRed, Color.Red, ExtraMath.Osc(0f, 1f, speed: 8));
        glowColor.A = 0;
        spriteBatch.Draw(glowMask, drawPos, null, glowColor, 0, glowDrawOrigin, Projectile.scale * ExtraMath.Osc(0.9f, 1.2f, speed: 8) * 0.15f * alpha, SpriteEffects.None, 0);

        Color innerGlowColor = Color.Goldenrod;
        innerGlowColor.A = 0;
        spriteBatch.Draw(glowMask, drawPos, null, innerGlowColor, 0, glowDrawOrigin, Projectile.scale * ExtraMath.Osc(0.9f, 1.2f, speed: 8) * 0.1f * alpha, SpriteEffects.None, 0);

        // spriteBatch.RestartDefaults();


        glowMask = AssetManager.GlowMask.SpiralVortex.Value;
        glowDrawOrigin = glowMask.Size() / 2f;
        glowColor = Color.Red * 0.3f;
        glowColor.A = 0;
        spriteBatch.Draw(glowMask, drawPos, null, glowColor, Main.GlobalTimeWrappedHourly * 8, glowDrawOrigin, Projectile.scale * ExtraMath.Osc(0.99f, 1.01f, speed: 8) * 0.3f * alpha, SpriteEffects.None, 0);
    }
    public override bool PreDraw(ref Color lightColor)
    {
        PixelationManager.QueuePrimitivesDrawAction(DrawPixelatedFlames, DrawLayer.OverNPCsWithOutline);
        PixelationManager.QueueSpritebatchDrawAction(DrawPixelatedCore, DrawLayer.OverNPCsWithOutline);


        return false;
    }
}