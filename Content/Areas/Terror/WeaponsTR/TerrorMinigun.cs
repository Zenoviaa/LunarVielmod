using Stellamod.Assets;
using Stellamod.Common.GunSystem;
using Stellamod.Common.Players;
using Stellamod.Common.Shaders;
using Stellamod.Content.CommonMaterials;
using Stellamod.Core.Pixelation;
using Stellamod.Dusts;
using Stellamod.Effects.Generic;
using Stellamod.Items;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Terror.WeaponsTR;

public class TerrorMinigun : BaseGun
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.damage = 12;
        Item.ArmorPenetration = 15;
        Item.DamageType = DamageClass.Ranged;
        Item.width = 40;
        Item.height = 40;
        Item.useTime = 3;
        Item.useAnimation = 3;
        Item.useStyle = ItemUseStyleID.Shoot;
        Item.knockBack = 6;
        Item.value = 100000;
        Item.rare = ItemRarityID.LightPurple;
        Item.autoReuse = true;
        Item.shoot = ProjectileID.Bullet;
        Item.shootSpeed = 35f;
        Item.useAmmo = AmmoID.Bullet;
        Item.noMelee = true;
        muzzleOrigin = new Vector2(135, 24);
    }

    public override Vector2? HoldoutOffset()
    {
        return new Vector2(16, 0);
    }
    public override bool CanUseItem(Player player)
    {


        return base.CanUseItem(player);
    }

    public override void SetMagazine(ref GunReloadParams fireParams)
    {
        base.SetMagazine(ref fireParams);
        fireParams.maxAmmo = 64;
        fireParams.reloadWindow = 150;
    }

    public override void ModifyWeaponDamage(Player player, ref StatModifier damage)
    {
        base.ModifyWeaponDamage(player, ref damage);

    }

    public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
    {
        base.ModifyShootStats(player, ref position, ref velocity, ref type, ref damage, ref knockback);
        type = ModContent.ProjectileType<TerrorMinigunShot>();
    }

    public override bool ShootProjectile(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        return base.ShootProjectile(player, source, position, velocity, type, damage, knockback);
    }
    public override void ShootEffects(Vector2 position, Vector2 velocity)
    {
        //        base.ShootEffects(position, velocity);
        int Sound2 = Main.rand.Next(1, 3);

        SoundStyle s;
        if (Sound2 == 1)
        {
            s = new SoundStyle("Stellamod/Assets/Sounds/XX4160");
        }
        else
        {
            s = new SoundStyle("Stellamod/Assets/Sounds/XX41602");
        }
        s = s with { PitchVariance = 0.6f, Volume = 0.7f };
        SoundEngine.PlaySound(s, position);
        BasicMuzzleFlash(position, velocity, Color.White, Color.DarkRed);
    }

    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew<MarshScrap, BlankGun>();
    }

}

public class TerrorMinigunShot : ModProjectile,
    IDrawToRenderTarget
{
    private Player Owner => Main.player[Projectile.owner];
    public override string Texture => TextureRegistry.EmptyTexture;
    private ref float Timer => ref Projectile.ai[0];
    private ref float RandScale => ref Projectile.ai[1];
    private ref float Recoil => ref Projectile.ai[2];
    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 16;
        ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
    }

    public override void SetDefaults()
    {
        Projectile.width = 16;
        Projectile.height = 16;
        Projectile.friendly = true;
        Projectile.hostile = false;
        Projectile.tileCollide = true;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = -1;
        Projectile.extraUpdates = 3;
        Projectile.penetrate = 2;
    }

    public override void AI()
    {

        Timer++;
        if(Timer == 1)
        {
            if(Recoil == 0)
            {
                Owner.AddRecoil(-Projectile.velocity.SafeNormalize(Vector2.Zero) * 0.35f);
                FXUtil.ShakeCamera(Projectile.Center, 1024, 2);
            }

            if (this.OwnedByLocalClient())
            {
                RandScale = Main.rand.NextFloat(0.5f, 1f);

                Projectile.velocity = Projectile.velocity.RotatedByRandom(0.05f);
                Projectile.netUpdate = true;
            }
        }
        if (Timer < 10 && Timer % 2 == 0)
        {
            var dp = DustParticle.Spawn(Projectile.Center, -Projectile.velocity.SafeNormalize(Vector2.Zero));
            dp.gravity = 0;
            dp.dampening = 0.05f;
            dp.noTileCollide = true;
            dp.Scale *= 0.4f;
            dp.outerColor = Color.DarkRed;
            dp.innerColor = Color.Lerp(Color.White, Color.Red, 0.5f);
        }
        Projectile.rotation = Projectile.velocity.ToRotation();
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        target.AddBuff(BuffID.Slow, 300);
    }

    public override void OnKill(int timeLeft)
    {
        FXUtil.GlowCircleBoom(Projectile.Center, Color.White, Color.Red, Color.DarkRed, duration: 12, baseSize: 0.07f);
        for(float f =0; f < 3; f++)
        {
            var dp = DustParticle.Spawn(Projectile.Center, Projectile.velocity.SafeNormalize(Vector2.Zero).RotatedByRandom(0.5f));
            dp.dampening = 0.12f;
            dp.gravity = 0;
            dp.noTileCollide = true;
            dp.Scale *= 0.4f;
        }

        for (int i = 0; i < 2; i++)
        {
            Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<Dusts.SmokeDust>(), Projectile.velocity.RotatedByRandom(MathHelper.ToRadians(30)) * Main.rand.NextFloat(0.2f, 1f), 0, Color.Red, 1f).noGravity = true;
        }
    }



    public override bool PreDraw(ref Color lightColor)
    {
        return false;
    }

    private void DrawTrail(GraphicsDevice gDevice)
    {
        float GetTrailWidth(float completionRatio)
        {
            return MathHelper.SmoothStep(16 * RandScale, 0, completionRatio);
        }
        Color GetTrailColor(float completionRatio)
        {
            Color additive = Color.Lerp(Color.White, Color.Red, ExtraMath.Osc(0f, 1f, speed: 32) * 0.5f);
            return additive;
        }
        BasicGlowTrailShader glowTrailShader = ShaderContent.GetInstance<BasicGlowTrailShader>();
        glowTrailShader.InsideColor = Color.White;
        glowTrailShader.BloomColor = Color.DarkRed;
        glowTrailShader.GlowColor = Color.Red;
        TrailDrawer.Draw(Projectile.oldPos, GetTrailColor, GetTrailWidth, glowTrailShader, Projectile.Size * 0.5f);
    }

    private void DrawRed(SpriteBatch sb, Vector2 sp)
    {
        SpritebatchDrawer glowDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.SimpleGlowCircle, Projectile.Center);
        glowDrawer.color = Color.Red * 0.6f;
        glowDrawer.color.A = 0;
        glowDrawer.scale.X *= 1.6f;
        glowDrawer.scale.Y *= 0.6f;
        glowDrawer.scale *= 0.1f * RandScale;
        glowDrawer.rotation = Projectile.rotation;
        sb.Draw(glowDrawer);

        glowDrawer.color = Color.White;
        glowDrawer.color.A = 0;
        sb.Draw(glowDrawer);
    }
    public void DrawToRenderTargets()
    {
      //  PixelationManager.QueuePrimitivesDrawAction(DrawTrail, DrawLayer.OverNPCs);
        PixelationManager.QueuePrimitivesDrawAction(DrawTrail, DrawLayer.OverNPCs);
        PixelationManager.QueueSpritebatchDrawAction(DrawRed, DrawLayer.OverPlayers);
    }
}
