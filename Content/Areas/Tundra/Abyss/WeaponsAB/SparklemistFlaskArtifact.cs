using Stellamod.Common.Particles;
using Stellamod.Content.CommonMaterials;
using Stellamod.Core;
using Stellamod.Core.Bases;
using Stellamod.Core.Pixelation;
using Stellamod.Items;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Tundra.Abyss.WeaponsAB;

public class SparklemistFlaskArtifact : ModItem
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.DefaultToArtifact();
        Item.damage = 24;
        Item.autoReuse = true;
        Item.mana = 25;
        Item.useAnimation = Item.useTime = 36;
        Item.useStyle = ItemUseStyleID.HoldUp;
        Item.UseSound = SoundID.Item45;
        Item.shoot = ModContent.ProjectileType<Sparklemist>();
        Item.shootSpeed = 25;
        Item.noMelee = true;
    }
    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew<ConvulgingMater, BlankStaff>();
    }
}

public class Sparklemist : ModProjectile
{
    private Color _sparkleMistColor;
    public override string Texture => TextureRegistry.EmptyTexture;
    private ref float Timer => ref Projectile.ai[0];
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 128;
        Projectile.height = 128;
        Projectile.timeLeft = 120;
        Projectile.friendly = true;
        Projectile.penetrate = -1;
        Projectile.usesIDStaticNPCImmunity = true;
        Projectile.idStaticNPCHitCooldown = 15;
        Projectile.ignoreWater = true;
        Projectile.light = 0.6f;
        Projectile.tileCollide = false;
    }

    public override void AI()
    {
        base.AI();
        Timer++;
        if(Timer == 1)
        {
            _sparkleMistColor = DrawUtilities.InterpolateColorArray(Main.rand.NextFloat(0f, 1f), Color.SkyBlue, Color.DarkBlue, Color.Violet, Color.White);
        }
        if (Main.rand.NextBool(4))
        {
            var sparkle = SparkleParticle.Spawn(Projectile.Center + Main.rand.NextVector2Circular(96, 96), Vector2.Zero, Scale: Main.rand.NextFloat(0.3f, 0.6f));
            sparkle.noTileCollide = true;
            sparkle.gravity = 0f;
            sparkle.dampening = 0.05f;
            sparkle.innerColor = Color.Lerp(Color.White, Color.Cyan, Main.rand.NextFloat(0f, 1f));
            sparkle.outerColor = Color.DarkBlue;
            sparkle.Scale *= 0.6f;
        }
        if (Main.rand.NextBool(3))
        {
            Color randomColor = DrawUtilities.InterpolateColorArray(Main.rand.NextFloat(0f, 1f), Color.SkyBlue, Color.DarkBlue, Color.Violet, Color.White);
            Particles.Sparklemist.Spawn(Common.Particles.Sparklemist.Data.Default with
            {
                position = Projectile.Center + Main.rand.NextVector2Circular(64, 64),
                velocity = Main.rand.NextVector2Circular(3, 3),
                color = randomColor,
                timeLeft=60
            }); 
        }
        Projectile.velocity *= 0.94f;
    }

    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        return false;
    }

    private void DrawPixelatedSmog(SpriteBatch sb, Vector2 screenPos)
    {
        SpritebatchDrawer drawer = SpritebatchDrawer.FromTextureAsset(AssetReferences.Assets.GlowMasks.SimpleGlowCircle.Asset, Projectile.Center);
        float ratio = Timer / 120f;
        float ease = EasingFunction.QuadraticBump(ratio);
        drawer.color = Color.Lerp(Color.Transparent, _sparkleMistColor, ease) * 0.16f;
        drawer.color.A = 0;
        drawer.scale *= 0.5f;
        sb.Draw(drawer);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        PixelationManager.QueueSpritebatchDrawAction(DrawPixelatedSmog, DrawLayer.OverNPCs);
        return false;
        //  return base.PreDraw(ref lightColor);
    }
}
