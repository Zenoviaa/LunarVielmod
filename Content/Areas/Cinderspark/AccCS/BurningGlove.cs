using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Content.Areas.Abyss.AccAB;
using Stellamod.Content.Areas.Cinderspark.WeaponsCS;
using Stellamod.Content.CommonMaterials;
using Stellamod.Core.Pixelation;
using Stellamod.Core.SwingSystem;
using Stellamod.Items;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Cinderspark.AccCS;

public class BurningGlove : AbstractMeleeAddon
{
    private Dictionary<int, bool> _hasShotSwingProj = new Dictionary<int, bool>();
    public override void OnSpawn(BaseSwingProjectileV2 projectile)
    {
        base.OnSpawn(projectile);
        int id = projectile.Projectile.identity;
        if (_hasShotSwingProj.ContainsKey(id))
            _hasShotSwingProj[id] = false;
        else
            _hasShotSwingProj.Add(id, false);
    }
    
    public override void AI(BaseSwingProjectileV2 projectile)
    {
        base.AI(projectile);
        if (!projectile.OwnedByLocalClient())
            return;
        var proj = projectile.Projectile;
        int id = projectile.Projectile.identity;
        if (projectile.Timer % 28 == 0)
        {

            Projectile.NewProjectile(proj.GetSource_FromAI(), proj.Center, proj.rotation.ToRotationVector2() * 12,
                ModContent.ProjectileType<BurningGloveFlamethrower>(), (int)(proj.damage * 0.1f), proj.knockBack, proj.owner);
        }
        if (!projectile.IsThrust())
            return;

        if (!_hasShotSwingProj.ContainsKey(id))
            return;

        if (!_hasShotSwingProj[id] && projectile.Interpolant >= 0.1f)
        {
            for(int i = 0; i < 2; i++)
            {
                Projectile.NewProjectile(projectile.Projectile.GetSource_FromAI(), projectile.Owner.Center,
                              projectile.Projectile.velocity.SafeNormalize(Vector2.Zero).RotatedByRandom(0.1) * 15, ModContent.ProjectileType<MoltenFireball>(),
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
    }

    public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
    {
        return ProjectileHelper.OldPosColliding(IncineratorPos, projHitbox, targetHitbox, 64);
    }
    public override bool ShouldUpdatePosition()
    {
        return true;
    }

    public override void AI()
    {
        float numPoints = NumPoints;
        Vector2 start = Projectile.Center;
        Vector2 end = start + Projectile.velocity * 100;

        float progress = Timer / LifeTime;
        float easeOut = EasingFunction.InOutSine(progress);
        start = Vector2.Lerp(start, end, easeOut * 0.5f);
        for (int i = 0; i < numPoints; i++)
        {
            float f = i;
            float ratio = f / numPoints;
            Vector2 point = Vector2.Lerp(start, end, ratio);
            IncineratorPos[i] = point;
        }

        Timer++;
        if (Timer == 1 && Main.rand.NextBool(8))
        {
            SoundEngine.PlaySound(SoundID.DD2_BetsyFireballShot, Projectile.position);
            SoundEngine.PlaySound(SoundID.DD2_EtherianPortalSpawnEnemy, Projectile.position);
        }

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
        float width = 300;
        float w = MathHelper.SmoothStep(16, width, completionRatio);
        float o = MathHelper.Lerp(1f, 0f, EasingFunction.InCirc(completionRatio));
        float progress = Timer / LifeTime;
        float o2 = MathHelper.Lerp(1f, 2f, progress);
        float i = MathHelper.Lerp(0f, 1f, EasingFunction.OutExpo(progress));
        return w * o * o2 * i;
    }

    private Color ColorFunction(float completionRatio)
    {
        Color tipColor = Color.Lerp(Color.Goldenrod, Color.DarkRed, completionRatio);
        Color finalColor = Color.Lerp(Color.Red, tipColor, EasingFunction.QuadraticBump(MathF.Pow(completionRatio, 0.5f)));
        Color finalColor2 = Color.Lerp(Color.White, finalColor, EasingFunction.QuadraticBump(completionRatio));
        finalColor2 *= EasingFunction.QuadraticBump(completionRatio);
        float progress = Timer / LifeTime;
        float o2 = MathHelper.Lerp(1f, 0f, progress);
        finalColor2 *= o2;
        finalColor2 *= MathHelper.Lerp(1f, 0f, EasingFunction.InExpo(completionRatio));
        return finalColor2;
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

    private void DrawMainShader(Vector2[] oldPos)
    {
        BlackFireOldShader blackFireShader = BlackFireOldShader.Instance;
        TrailDrawer.Draw(Main.spriteBatch, oldPos, ColorFunction, WidthFunction, blackFireShader, Vector2.Zero);

        var shader = RichLaserShader.Instance;
        shader.LaserColor = Color.Yellow * 0.2f;
        shader.InnerColor = Color.Lerp(Color.Yellow, Color.Red, 0.75f) * 0.2f;
        shader.OuterColor = Color.Yellow * 0.2f;
        shader.LaserTexture = TrailRegistry.Beamlight;
        shader.BloomTexture = TrailRegistry.SmallWhispyTrail;
        TrailDrawer.Draw(Main.spriteBatch, oldPos, ColorFunction2, WidthFunction2, shader);

    }

    private void DrawPixelatedFlames(GraphicsDevice graphicsDevice)
    {
        DrawMainShader(IncineratorPos);
    }
    public override bool PreDraw(ref Color lightColor)
    {
        PixelationManager.QueuePrimitivesDrawAction(DrawPixelatedFlames, DrawLayer.OverNPCsWithOutline);
        return false;
    }
}