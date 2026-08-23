using Stellamod.Assets;
using Stellamod.Common.GunSystem;
using Stellamod.Common.Shaders;
using Stellamod.Content.CommonMaterials;
using Stellamod.Core.Particles;
using Stellamod.Core.Pixelation;
using Stellamod.Helpers;
using Stellamod.Items;
using Stellamod.Visual.Particles;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Tundra.Snow.WeaponsSN;

public class Pearlinator : BaseGun
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.width = 62;
        Item.height = 32;
        Item.useTime = 4;
        Item.useAnimation = 4;
        Item.useStyle = ItemUseStyleID.Shoot;
        Item.autoReuse = true;
        Item.UseSound = SoundID.Item34;

        Item.DamageType = DamageClass.Ranged;
        Item.damage = 4;
        Item.knockBack = 4;
        Item.noMelee = true;

        Item.shoot = ModContent.ProjectileType<PearlinatorFlame>();
        Item.useTime = 8;
        Item.useAnimation = 8;
        Item.shootSpeed = 1;
    }

    public override void SetMagazine(ref GunReloadParams fireParams)
    {
        base.SetMagazine(ref fireParams);
        fireParams.maxAmmo = 32;
        fireParams.reloadWindow = 120;
    }
    public override Vector2? HoldoutOffset()
    {
        return new Vector2(16, 0);
    }

    public override bool ShootProjectile(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        type = ModContent.ProjectileType<PearlinatorFlame>();
        Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
        return false;
    }

    public override void ShootEffects(Vector2 position, Vector2 velocity)
    {
        muzzleOrigin = new Vector2(64, 10);
        BasicMuzzleFlash(position, velocity, Color.SkyBlue, Color.Purple);
    }

    public override void ModifyMuzzleFlashColors(ref Color hottestColor, ref Color coldestColor)
    {
        base.ModifyMuzzleFlashColors(ref hottestColor, ref coldestColor);
        hottestColor = Color.White;
        coldestColor = Color.SkyBlue;
    }

    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew<WinterbornShard, BlankGun>();
    }
}

public class PearlinatorFlame : ModProjectile,
    IDrawToRenderTarget
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
        Projectile.idStaticNPCHitCooldown = 14;
        Projectile.usesIDStaticNPCImmunity = true;
        Projectile.tileCollide = false;
        Projectile.timeLeft = (int)LifeTime;
    }

    public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
    {
        return ProjectileHelper.OldPosColliding(IncineratorPos, projHitbox, targetHitbox, 48);
    }
    public override bool ShouldUpdatePosition()
    {
        return true;
    }

    public override void AI()
    {
        float numPoints = NumPoints;
        Vector2 start = Projectile.Center;
        Vector2 end = start + Projectile.velocity * 64;

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


        Lighting.AddLight(Projectile.Center + Projectile.velocity * 32, TorchID.Ice);
        Projectile.rotation += 0.05f;
    }

    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        return false;
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        base.OnHitNPC(target, hit, damageDone);
        target.AddBuff(BuffID.Frostburn, 120);
    }

    private float WidthFunction(float completionRatio)
    {
        float width = 192;
        float w = MathHelper.SmoothStep(16, width, completionRatio);
        float o = MathHelper.Lerp(1f, 0f, EasingFunction.InCirc(completionRatio));
        float progress = Timer / LifeTime;
        float o2 = MathHelper.Lerp(1f, 2f, progress);
        float i = MathHelper.Lerp(0f, 1f, EasingFunction.OutExpo(progress));
        return w * o * o2 * i;
    }

    private Color ColorFunction(float completionRatio)
    {
        Color tipColor = Color.Lerp(Color.White, Color.Pink, completionRatio);
        Color finalColor = Color.Lerp(Color.Cyan, tipColor, EasingFunction.QuadraticBump(MathF.Pow(completionRatio, 0.5f)));
        Color finalColor2 = Color.Lerp(Color.White, finalColor, EasingFunction.QuadraticBump(completionRatio));
        finalColor2 *= EasingFunction.QuadraticBump(completionRatio);
        float progress = Timer / LifeTime;
        float o2 = MathHelper.Lerp(1f, 0f, progress);
        finalColor2 *= o2;
        finalColor2 *= MathHelper.Lerp(1f, 0f, EasingFunction.InExpo(completionRatio));
        return finalColor2;
    }

    private void DrawMainShader(Vector2[] oldPos)
    {
        BlackFireOldShader blackFireShader = BlackFireOldShader.Instance;
        blackFireShader.InnerColor = Color.LightCyan;
        blackFireShader.OuterColor = Color.Blue;
        blackFireShader.PrimaryTexture2 = TrailRegistry.StarTrail;
        blackFireShader.BackColor = Color.Violet;
        TrailDrawer.Draw(Main.spriteBatch, oldPos, null, ColorFunction, WidthFunction, blackFireShader, Vector2.Zero);
    }

    private void DrawPixelatedFlames(GraphicsDevice graphicsDevice)
    {
        DrawMainShader(IncineratorPos);
    }

    public override bool PreDraw(ref Color lightColor)
    {

        return false;
    }

    public void DrawToRenderTargets()
    {
        PixelationManager.QueuePrimitivesDrawAction(DrawPixelatedFlames, DrawLayer.OverNPCsWithOutline);
    }
}