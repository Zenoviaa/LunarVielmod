using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Content.CommonMaterials;
using Stellamod.Core.Bases;
using Stellamod.Core.Pixelation;
using Stellamod.Core.SwingSystem;
using Stellamod.Items;
using Stellamod.Visual.Particles;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Tundra.MoonspiralTower.AccMT;

public class MoonSlasher : AbstractMeleeAddon
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

    private bool IsValidWeaponType(BaseSwingProjectileV2 projectile)
    {
        var swingItem = projectile.Owner.HeldItem.ModItem as BaseSwingItemV2;
        if (swingItem == null)
            return false;
        switch (swingItem.meleeWeaponType)
        {
            default:
                return false;
            case MeleeWeaponType.Sword:
            case MeleeWeaponType.Knives:
            case MeleeWeaponType.Hammer:
                return true;
        }
    }
    public override void AI(BaseSwingProjectileV2 projectile)
    {
        base.AI(projectile);
        if (!projectile.OwnedByLocalClient())
            return;
        if (!IsValidWeaponType(projectile))
            return;


        if (!_hasShotSwingProj.ContainsKey(projectile))
            return;

        if (!_hasShotSwingProj[projectile] && projectile.EasedInterpolant >= 0.3f)
        {
            Projectile.NewProjectile(projectile.Projectile.GetSource_FromAI(), projectile.Owner.Center,
                projectile.Projectile.velocity.SafeNormalize(Vector2.Zero) * 45, ModContent.ProjectileType<FlyingMoonSlash>(),
                (int)(projectile.Projectile.damage * 0.45f), projectile.Projectile.knockBack, projectile.Projectile.owner, ai1: projectile.Type, ai2: projectile.Size);
            _hasShotSwingProj[projectile] = true;
        }
    }


    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew<PearlescentScrap, BlankAccessory>();
    }
}

public class FlyingMoonSlash : ModProjectile,
    IDrawToRenderTarget
{
    private ref float Timer => ref Projectile.ai[0];
    private int ParentProjectileType => (int)Projectile.ai[1];
    private ref float Size => ref Projectile.ai[2];
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
        Projectile.timeLeft = 30;
        Projectile.penetrate = -1;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = -1;
        Projectile.tileCollide = false;
    }
    private Color GetPrimaryColor()
    {
        if (ParentProjectileType == 0)
            return Color.Blue;
        var proj = ModContent.GetModProjectile(ParentProjectileType);
        if (proj is BaseSwingProjectileV2 v2)
        {
            return v2.glowAfterImageColor;
        }
        return Color.Blue;
    }
    public override void AI()
    {
        base.AI();
        Timer++;
        Projectile.rotation = Projectile.velocity.ToRotation();
        Projectile.velocity *= 0.9f;


        if (Timer % 16 == 0)
        {
            Vector2 pos = Projectile.Center + Main.rand.NextVector2Circular(64, 64);
            var dp = DustParticle.Spawn(pos, Vector2.Zero, DustParticleSpawnParams.Default);
            dp.Scale *= 0.5f;
            dp.noTileCollide = true;
            dp.gravity = 0;
            dp.dampening = 0.05f;
            dp.outerColor = GetPrimaryColor();
        }

        if (Timer % 12 == 0)
        {
            Vector2 pos = Projectile.Center + Main.rand.NextVector2Circular(64, 64);
            Vector2 vel = -Projectile.velocity * 4f;
            var fx = FXUtil.GlowStretch(pos, vel);
            fx.VectorScale *= 0.5f;
            fx.OuterGlowColor = GetPrimaryColor();
        }

    }

    public override bool PreDraw(ref Color lightColor)
    {
        float scale = Size / 64;
        float outAlpha = EasingFunction.InOutSine(Projectile.timeLeft / 30f);
        SpritebatchDrawer afterDrawer = SpritebatchDrawer.FromProjectile(Projectile);
        afterDrawer.scale = Vector2.One * scale;
        for (int i = 0; i < Projectile.oldPos.Length; i++)
        {
            Vector2 pos = Projectile.oldPos[i] + Projectile.Size * 0.5f;
            float ratio = i / (float)Projectile.oldPos.Length;
            afterDrawer.color = Color.Lerp(Color.LightBlue, Color.DarkBlue, ratio) * 0.15f * outAlpha;
            afterDrawer.color.A = 0;
            afterDrawer.worldPosition = pos;
            Main.spriteBatch.Draw(afterDrawer);
        }

        GlowingSwordMaskShader shader = GlowingSwordMaskShader.Instance;
        shader.TrailTexture = TrailRegistry.BulbTrail;
        shader.Distortion = 0.02f;
        shader.DistortionTexture = TrailRegistry.WhispyTrail;
        shader.Time = Main.GlobalTimeWrappedHourly * 16;
        shader.Bloom = MathHelper.Lerp(4f, 0f, EasingFunction.OutExpo(Timer / 30f));
        shader.Tiling = Vector2.One * 0.75f;
        shader.InnerColor = Color.Lerp(Color.LightBlue, Color.Lerp(Color.LightBlue, Color.Blue, 0.4f), ExtraMath.Osc(0f, 1f, 12)) * 0.5f;
        shader.OuterColor = Color.DarkBlue * 0.5f;
        Main.spriteBatch.Restart(effect: shader.Effect);

        SpritebatchDrawer sbDrawer = SpritebatchDrawer.FromProjectile(Projectile);
        sbDrawer.scale *= 1.5f * scale;
        sbDrawer.color = Color.LightBlue * 0.5f * outAlpha;
        sbDrawer.color.A = 0;
        //sbDrawer.worldPosition += _mirageOffset;
        Main.spriteBatch.Draw(sbDrawer);

        sbDrawer = SpritebatchDrawer.FromProjectile(Projectile);
        sbDrawer.color = Color.White * outAlpha * 0.5f;
        sbDrawer.color.A = 0;
        sbDrawer.scale *= scale;
        Main.spriteBatch.Draw(sbDrawer);

        Main.spriteBatch.RestartDefaults();
        return false;
        //        return base.PreDraw(ref lightColor);
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        base.OnHitNPC(target, hit, damageDone);
    }
    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
    }
    public void DrawToRenderTargets()
    {

    }
}