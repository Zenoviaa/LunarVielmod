using ReLogic.Content;
using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Content.CommonMaterials;
using Stellamod.Core.Pixelation;
using Stellamod.Core.SwingSystem;
using Stellamod.Items;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Tundra.Abyss.AccAB;

public class GhastlyGlove : AbstractMeleeAddon
{
    private Dictionary<BaseSwingProjectileV2, bool> _hasShotSwingProj = new Dictionary<BaseSwingProjectileV2, bool>();
    public override void OnSpawn(BaseSwingProjectileV2 projectile)
    {
        base.OnSpawn(projectile);
        if (_hasShotSwingProj.ContainsKey(projectile))
            _hasShotSwingProj[projectile] = false;
        else
            _hasShotSwingProj.Add(projectile, false);
    }
    public override void AI(BaseSwingProjectileV2 projectile)
    {
        base.AI(projectile);
        if (!projectile.OwnedByLocalClient())
            return;
        if (!projectile.IsThrust())
            return;


        if (!_hasShotSwingProj.ContainsKey(projectile))
            return;

        if (!_hasShotSwingProj[projectile] && projectile.Interpolant >= 0.1f)
        {
            Projectile.NewProjectile(projectile.Projectile.GetSource_FromAI(), projectile.Owner.Center,
                projectile.Projectile.velocity.SafeNormalize(Vector2.Zero) * 45, ModContent.ProjectileType<GhastlyThrust>(),
                (int)(projectile.Projectile.damage * 0.45f), projectile.Projectile.knockBack, projectile.Projectile.owner, ai1: projectile.Owner.HeldItem.type);
            _hasShotSwingProj[projectile] = true;
        }
    }


    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew<ConvulgingMater, BlankAccessory>();
    }
}

public class GhastlyThrust : ModProjectile,
    IDrawToRenderTarget
{
    private float Alpha => MathHelper.Lerp(0f, 1f, EasingFunction.InOutSine(Projectile.timeLeft / 60f));
    private Player Owner => Main.player[Projectile.owner];
    private Asset<Texture2D> _swordTextureAsset;
    private ref float Timer => ref Projectile.ai[0];
    private int ProjectileType => (int)Projectile.ai[1];
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
        Projectile.width = 64;
        Projectile.height = 64;
        Projectile.tileCollide = false;
        Projectile.friendly = true;
        Projectile.penetrate = -1;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = -1;
        Projectile.timeLeft = 60;
    }

    public override void AI()
    {
        base.AI();
        Timer++;
        if (_swordTextureAsset == null || Timer == 1)
        {
            _swordTextureAsset = TextureAssets.Item[ProjectileType];
        }
        Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(45);
        Projectile.velocity *= 0.92f;
    }

    public float WidthFunction(float completionRatio)
    {
        float baseWidth = Projectile.scale * Projectile.width;
        return MathHelper.SmoothStep(baseWidth, 3.5f, completionRatio);
    }

    private Color GetColorFunction(float completionRatio)
    {

        Color inColor = Color.White;
        Color trailColor = Color.Lerp(Color.SpringGreen, Color.DarkBlue, completionRatio);


        Color rainbow = Color.Red;
        float degrees = completionRatio * 360f;
        degrees += Main.GlobalTimeWrappedHourly * 400;
        degrees %= 360;
        rainbow.ScrollHue(degrees);
        //DrawUtilities.IncreaseHueBy(ref rainbow, degrees, out float hue);
        trailColor = Color.Lerp(trailColor, rainbow, 0.5f);
        Color easeColor = Color.Lerp(inColor, trailColor, EasingFunction.InExpo(Timer / 60f));
        return easeColor * 1f * Alpha * ExtraMath.Osc(0.4f, 0.6f, 32f) * EasingFunction.QuadraticBump(completionRatio);
    }

    private float GetWidthFunction(float completionRatio)
    {
        return MathHelper.SmoothStep(54, 2, completionRatio);
    }

    private float GetWidthFunction2(float completionRatio)
    {
        return WidthFunction(completionRatio) * 2f;
    }

    private void DrawTrail(GraphicsDevice gDevice)
    {
        var shader2 = BasicLaserShader.Instance;

        shader2.LaserTexture = TrailRegistry.StarTrail;
        shader2.InnerColor = Color.Turquoise * 0.5f;
        shader2.OuterColor = Color.DarkTurquoise;
        TrailDrawer.Draw(Main.spriteBatch, Projectile.oldPos, GetColorFunction, GetWidthFunction, shader2, Projectile.Size * 0.5f);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        if (_swordTextureAsset == null)
            return false;
     
        SpritebatchDrawer phaseDrawer = SpritebatchDrawer.FromTextureAsset(_swordTextureAsset, Projectile.Center);
        DrawUtilities.DrawSpriteAfterImage(Main.spriteBatch, phaseDrawer, Projectile.oldPos, Projectile.oldRot, Color.White, Color.Transparent, 0.1f * Alpha, offset: Projectile.Size * 0.5f);
        phaseDrawer.rotation = Projectile.rotation;
        phaseDrawer.color = Color.White * ExtraMath.Osc(0.4f, 0.6f, speed: 64) * Alpha;
        Main.spriteBatch.Draw(phaseDrawer);

        phaseDrawer.color = Color.White * ExtraMath.Osc(0.8f, 0.9f, speed: 64) * Alpha;
        phaseDrawer.color.A = 0;
        Main.spriteBatch.Draw(phaseDrawer);
        return false;
    }

    public void DrawToRenderTargets()
    {
        PixelationManager.QueuePrimitivesDrawAction(DrawTrail);

    }
}