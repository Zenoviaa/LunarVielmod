using Microsoft.Xna.Framework;
using Stellamod.Assets;
using Stellamod.Common.ClassReworkSystem;
using Stellamod.Common.StaminaSystem;
using Stellamod.Content.CommonMaterials;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Accessories;

public class BurningScarf : AbstractDashItem
{
    public override string Texture => AssetReferences.Assets.Textures.Fable.Items.BurningScarf.KEY;
    private float Timer;
    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew<AlcadizScrap, BlankAccessory>();
    }

    public override void BeginDash(Player player)
    {
        base.BeginDash(player);
        Timer = 0;
        SoundEngine.PlaySound(SoundID.Item73, player.position);
        for (int i = 0; i < 16; i++)
        {
            float progress = (float)i / 16f;
            float rot = progress * MathHelper.TwoPi;
            Vector2 vel = rot.ToRotationVector2() * 3;
            Dust.NewDustPerfect(player.Center, DustID.Torch, vel, Scale: 2);
        }
    }

    public override void UpdateDash(Player player)
    {
        base.UpdateDash(player);
        Timer++;
        if (Timer % 6 == 0)
        {
            for (int i = 0; i < 12; i++)
            {
                float progress = (float)i / 12f;
                float rot = progress * MathHelper.TwoPi;
                Vector2 vel = rot.ToRotationVector2() * 1.3f;
                Dust d = Dust.NewDustPerfect(player.Center, DustID.Torch, vel, Scale: 2);
                d.noGravity = true;
            }

            //Make Flame
            int damage = player.GetModPlayer<ClassReworkPlayer>().ScalingWeaponDamage() / 2;
            float knockback = Item.knockBack;
            Vector2 velocity = -player.velocity.SafeNormalize(Vector2.Zero) * 8;
            velocity -= new Vector2(0, 0.013f);
            Projectile.NewProjectile(player.GetSource_FromThis(), player.Center, velocity,
                ModContent.ProjectileType<BurningScarfFlame>(), damage, knockback, player.whoAmI);
        }
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        base.UpdateAccessory(player, hideVisual);

        //Just gonna make your dash go a lil' farther
        StaminaNDashPlayer dashPlayer = player.GetModPlayer<StaminaNDashPlayer>();
        dashPlayer.DashVelocity += 3;
    }
}

public class BurningScarfFlame : ModProjectile
{
    public override string Texture => AssetReferences.Assets.Textures.Fable.Items.BurningScarfFlame.KEY;
    private ref float Timer => ref Projectile.ai[0];

    private float _scale;
    private Vector2 InitialVelocity;
    private Vector2 TargetVelocity;
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        ProjectileID.Sets.TrailCacheLength[Type] = 16;
        ProjectileID.Sets.TrailingMode[Type] = 2;
        Main.projFrames[Type] = 4;
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 12;
        Projectile.height = 12;
        Projectile.friendly = true;
        Projectile.light = 0.278f;
        Projectile.timeLeft = 180;
    }


    public override void AI()
    {
        base.AI();
        Timer++;
        if (Timer == 1)
        {
            InitialVelocity = Projectile.velocity;
            SoundEngine.PlaySound(SoundID.DD2_BetsyFireballShot, Projectile.position);
        }
        if (Timer % 12 == 0)
        {
            Vector2 vel = Vector2.Zero;
            Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.Torch, vel, Scale: 1);
            d.noGravity = true;
        }
        if (Timer % 6 == 0)
        {
            Vector2 vel = Vector2.Zero;
            Dust d = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(8, 8), DustID.Torch, vel, Scale: 1);
            d.noGravity = true;
        }
        if (Timer <= 5)
        {
            _scale = MathHelper.Lerp(0f, Main.rand.NextFloat(0.15f, 0.66f), Easing.InCubic(Timer / 5f));
            Projectile.velocity *= 0.5f;
        }

        if (Timer == 5)
        {
            //Ping Sound
            SoundStyle soundStyle = new SoundStyle("Stellamod/Assets/Sounds/Jack_FirePing");
            soundStyle.PitchVariance = 0.1f;
            SoundEngine.PlaySound(soundStyle, Projectile.position);
        }

        if (Timer == 10)
        {
            TargetVelocity = InitialVelocity;
        }

        if (Timer > 10)
        {
            Projectile.velocity = Vector2.Lerp(Projectile.velocity, TargetVelocity, 0.02f);
        }

        Projectile.rotation = Projectile.velocity.X * 0.05f;
        DrawHelper.AnimateTopToBottom(Projectile, 4);
    }

    public float WidthFunction(float completionRatio)
    {
        float baseWidth = Projectile.scale * Projectile.width * 1.2f;
        return MathHelper.SmoothStep(baseWidth, 3.5f, completionRatio);
    }

    public Color ColorFunction(float completionRatio)
    {
        return Color.Lerp(Color.LightGoldenrodYellow * 0.1361f, Color.Transparent, completionRatio);
    }

    public override bool PreDraw(ref Color lightColor)
    {

        SpriteBatch spriteBatch = Main.spriteBatch;
        Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
        Vector2 drawOrigin = texture.Size() / 2f;
        Color drawColor = Color.White.MultiplyRGB(lightColor);
        float drawRotation = Projectile.rotation;
        float drawScale = _scale;

        Vector2 drawPos = Projectile.Center - Main.screenPosition;
        spriteBatch.Draw(texture, drawPos, Projectile.Frame(), drawColor, drawRotation, Projectile.Frame().Size() / 2f, drawScale, SpriteEffects.None, 0);


        for (int i = 0; i < 4; i++)
        {
            float rot = (float)i / 4f;
            Vector2 vel = rot.ToRotationVector2() * VectorHelper.Osc(0f, 4f, speed: 16);
            Vector2 flameDrawPos = drawPos + vel + Main.rand.NextVector2Circular(2, 2);
            flameDrawPos -= Vector2.UnitY * 4;
            spriteBatch.Draw(texture, flameDrawPos, Projectile.Frame(), drawColor with { A = 0 }, drawRotation, Projectile.Frame().Size() / 2f, drawScale, SpriteEffects.None, 0);
        }

        for (int i = 0; i < 4; i++)
        {
            Vector2 flameDrawPos = drawPos + Main.rand.NextVector2Circular(2, 2);
            spriteBatch.Draw(texture, flameDrawPos, Projectile.Frame(), drawColor with { A = 0 }, drawRotation, Projectile.Frame().Size() / 2f, drawScale, SpriteEffects.None, 0);
        }

     
        return false;
    }

    public override void OnKill(int timeLeft)
    {
        for (int i = 0; i < 24; i++)
        {
            int num = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.FlameBurst, 0f, -2f, 0, default(Color), 1.5f);
            Dust dust = Main.dust[num];
            dust.noGravity = true;
            dust.position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
            dust.position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
            dust.velocity = Projectile.DirectionTo(dust.position) * 6f;
        }

        SoundEngine.PlaySound(SoundID.DD2_BetsyFireballImpact, Projectile.position);
    }

    public override void PostDraw(Color lightColor)
    {
        base.PostDraw(lightColor);
       
        var drawer = SpritebatchDrawer.FromTextureAsset(AssetReferences.Assets.GlowMasks.SimpleGlowCircle.Asset, Projectile.Center);
        drawer.color = new Color(85, 45, 15);
        drawer.color.A = 0;
        drawer.scale *= VectorHelper.Osc(0.75f, 1f, speed: 32, offset: Projectile.whoAmI);
        drawer.scale *= 0.25f;
        Main.spriteBatch.Draw(drawer);

        //  Lighting.AddLight(Projectile.Center, Color.Yellow.ToVector3() * 0.3f * Main.essScale);
    }
}