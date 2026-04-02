using Microsoft.Xna.Framework;
using Stellamod.Common.GunSystem;
using Stellamod.Common.Shaders;
using Stellamod.Content.CommonMaterials;
using Stellamod.Core.Pixelation;
using Stellamod.Helpers;
using Stellamod.Items;
using Stellamod.Items.Materials;
using Stellamod.Projectiles.Gun;
using Stellamod.Trails;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.WondrousDarkspace.WeaponsWD;

public class Shredder : BaseGun
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.damage = 18;
        Item.crit = 4;
        Item.knockBack = 3f;
        Item.width = 62;
        Item.height = 54;
        Item.useTime = 21;
        Item.useAnimation = 21;
        Item.useStyle = ItemUseStyleID.Shoot;
        Item.UseSound = SoundID.Item1;
        Item.value = Item.buyPrice(0, 30, 0, 0);
        Item.rare = ItemRarityID.LightPurple;
        Item.DamageType = DamageClass.Ranged;
        Item.shoot = ModContent.ProjectileType<ShreddingLine>();
        Item.shootSpeed = 25;
        Item.autoReuse = true;
        Item.noMelee = true;
    }

    public override void SetMagazine(ref GunReloadParams fireParams)
    {
        base.SetMagazine(ref fireParams);
        fireParams.maxAmmo = 12;
        fireParams.reloadWindow = 120;
    }

    public override bool GunShot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        SoundStyle shootSound = new SoundStyle("Stellamod/Assets/Sounds/StormDragon_CloudBolt");
        shootSound.PitchVariance = 0.3f;
        SoundEngine.PlaySound(shootSound, player.position);
        //Funny Screenshake
        FXUtil.ShakeCamera(player.position, 4, 8);
        int numProjectiles = Main.rand.Next(2, 5);

        for (int p = 0; p < numProjectiles; p++)
        {
            float direction = Main.rand.NextBool(2) ? -1 : 1;
            float speedMultiplier = Main.rand.NextFloat(0.5f, 1f);
            // Rotate the velocity randomly by 30 degrees at max.
            Vector2 newVelocity = velocity.RotatedByRandom(MathHelper.ToRadians(15));
            newVelocity *= 1f - Main.rand.NextFloat(0.3f);
            Projectile.NewProjectileDirect(source, position, newVelocity * speedMultiplier, ModContent.ProjectileType<ShreddingLine>(), damage, knockback, player.whoAmI, direction);
        }
        return false;
    }

    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew(
            mold: ModContent.ItemType<BlankGun>(), 
            material: ModContent.ItemType<HypnotizedSoul>());
    }
}

public class ShreddingLine : ModProjectile
{
    public override string Texture => TextureRegistry.EmptyTexture;
    private float _rotationSpeed;
    private ref float Timer => ref Projectile.ai[1];
    public override void OnSpawn(IEntitySource source)
    {
        for (int i = 0; i < Projectile.oldPos.Length; i++)
        {
            Projectile.oldPos[i] = Projectile.position;
        }
    }

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 18;
        ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
    }

    public override void SetDefaults()
    {
        Projectile.friendly = true;
        Projectile.width = 16;
        Projectile.height = 16;
        Projectile.timeLeft = 180;
        Projectile.penetrate = -1;
        Projectile.usesIDStaticNPCImmunity = true;
        Projectile.idStaticNPCHitCooldown = 12;
    }

    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        Projectile.velocity = -Projectile.velocity;
        return false;
    }

    public override void AI()
    {
        Timer++;
        if(Timer % 8 == 0)
        {
            var sp = SparkleParticle.Spawn(Projectile.Center, Vector2.Zero, Scale: 0.4f);
            sp.innerColor = Color.Black;
            sp.outerColor = Color.Pink;
            sp.noTileCollide = true;
            sp.fast = true;
        }

        float rotationDirection = Projectile.ai[0];
        Projectile.velocity = Projectile.velocity.RotatedBy(MathHelper.ToRadians(_rotationSpeed * rotationDirection));
        _rotationSpeed += 0.15f;

        //Dunno if this is needed but whatever
        Projectile.rotation = Projectile.velocity.ToRotation();
    }

    public float WidthFunction(float completionRatio)
    {

        float baseWidth = Projectile.scale * 4;
        float w = MathHelper.SmoothStep(baseWidth, 3.5f, completionRatio);
        float outScale = (float)Projectile.timeLeft / 30f;
        w *= outScale;
        w *= EasingFunction.QuadraticBump(completionRatio);
        return w;
    }

    public Color ColorFunction(float completionRatio)
    {
        Color startColor = Color.Lerp(Color.Black, Color.Black, ExtraMath.Osc(0f, 1f, speed: 12));
        Color endColor = Color.Lerp(Color.Cyan, Color.Purple, ExtraMath.Osc(0f, 1f, speed: 12, offset: 4));
        return Color.Lerp(startColor, endColor, completionRatio);
    }

    public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
    {
        //This damages everything in the trail
        Vector2[] positions = Projectile.oldPos;
        float collisionPoint = 0;
        for (int i = 1; i < positions.Length; i++)
        {
            Vector2 position = positions[i];
            Vector2 previousPosition = positions[i - 1];
            if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), position, previousPosition, 6, ref collisionPoint))
                return true;
        }
        return base.Colliding(projHitbox, targetHitbox);
    }

    private void RenderPixelatedLines(GraphicsDevice gDevice)
    {
        var shader = BasicLaserAlphaShader.Instance;
        shader.LaserTexture = TrailRegistry.LightningTrail2;
        TrailDrawer.Draw(Main.spriteBatch, Projectile.oldPos, ColorFunction, WidthFunction, shader, Projectile.Size * 0.5f);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        PixelationManager.QueuePrimitivesDrawAction(RenderPixelatedLines, DrawLayer.OverNPCsWithOutline);
        return false;
    }

    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
    }
}
