using Stellamod.Common.Shaders;
using Stellamod.Content.CommonMaterials;
using Stellamod.Core;
using Stellamod.Core.Bases;
using Stellamod.Core.Effects.Trails;
using Stellamod.Core.Pixelation;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Items;
using Stellamod.Items.Materials;
using Stellamod.NPCs.Bosses.Verlia.Projectiles;
using Stellamod.Trails;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.WondrousDarkspace.WeaponsWD;

public class Hypnotizer : BaseCrossbowItem
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.damage = 16;
    }

    public override void ShootBow(Player player, EntitySource_ItemUse_WithAmmo source, ShootParams shootParams)
    {

        base.ShootBow(player, source, shootParams);
        int Sound = Main.rand.Next(1, 3);
        /*
        SoundStyle shootSound;
        if (Sound == 1)
        {
            shootSound = new SoundStyle("Stellamod/Assets/Sounds/ArchariliteEnergyShot");
        }
        else
        {
            shootSound = new SoundStyle("Stellamod/Assets/Sounds/ArchariliteEnergyShot2");
        }

        shootSound.Volume = 0.5f;
        shootSound.PitchVariance = 0.25f;
            SoundEngine.PlaySound(shootSound, position);

         */
        Vector2 position = shootParams.position;
        Vector2 velocity = shootParams.fireVelocity * 3;
        int damage = shootParams.damage;
        float knockback = shootParams.knockBack;

        float numberProjectiles = 2;
        float rotation = MathHelper.ToRadians(15);
        position += Vector2.Normalize(new Vector2(velocity.X, velocity.Y)) * 25f;
        for (int i = 0; i < numberProjectiles; i++)
        {
            Vector2 perturbedSpeed = new Vector2(velocity.X, velocity.Y).RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (numberProjectiles - 1))) * .4f; // This defines the projectile roatation and speed. .4f == projectile speed
            var crossShot = Projectile.NewProjectileDirect(source, position, perturbedSpeed, shootParams.projToShoot, damage, knockback, player.whoAmI);
            crossShot.GetGlobalProjectile<CrossbowGlobalProjectile>().isCrossbowShot = true;
        }
    }

    public override void StaminaShootBow(Player player, EntitySource_ItemUse_WithAmmo source, ShootParams shootParams)
    {
        base.StaminaShootBow(player, source, shootParams);
        var crossShot = Projectile.NewProjectileDirect(source, shootParams.position, shootParams.fireVelocity, ModContent.ProjectileType<HypnotizerArrow>(), shootParams.damage, shootParams.knockBack, player.whoAmI);
       // crossShot.GetGlobalProjectile<CrossbowGlobalProjectile>().CrossbowShot = true;
    }
    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew<HypnotizedSoul, BlankBow>();
    }
}


public class HypnotizerArrow : ScarletProjectile
{
    private ref float Timer => ref Projectile.ai[0];
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        ProjectileID.Sets.TrailCacheLength[Type] = 16;
        ProjectileID.Sets.TrailingMode[Type] = 2;
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 16;
        Projectile.height = 16;
        Projectile.friendly = true;
        Projectile.timeLeft = 200;
        Projectile.light = 1.5f;
        Projectile.extraUpdates = 1;
    }

    public override void AI()
    {
        base.AI();
        Projectile.velocity *= 1.04f;
        Projectile.rotation = Projectile.velocity.ToRotation();
        Timer++;
        if (Timer % 5 == 0)
        {
            SparkleParticle sp = SparkleParticle.Spawn(Projectile.Center + Main.rand.NextVector2Circular(8, 8), Main.rand.NextVector2Circular(15, 15), Scale: Main.rand.NextFloat(0.25f, 0.5f));
            sp.innerColor = Color.White;
            sp.outerColor = Color.Purple;
            sp.fast = true;
            sp.noTileCollide = true;
            sp.gravity = 0;
            sp.dampening = 0.15f;
            //Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(8, 8), ModContent.DustType<GlowDust>(), Scale: Main.rand.NextFloat(0.25f, 0.5f));
        }
    }

    private void DrawPixelatedTrails(GraphicsDevice gDevice)
    {
        BlackFireShader blackFireShader = BlackFireShader.Instance;
        blackFireShader.SetDefaults();
        blackFireShader.InnerColor = Color.LightBlue;
        blackFireShader.OuterColor = Color.DarkBlue;
        blackFireShader.InnerEmitColor = Color.LightBlue * 0.2f;
        blackFireShader.OuterEmiteColor = Color.Purple;
        blackFireShader.BloomTexture = TrailRegistry.StarTrail;
        TrailDrawer.Draw(Main.spriteBatch, Projectile.oldPos, GetBloomColor, GetBloomWidth, blackFireShader, Projectile.Size * 0.5f);

        BloomTrailShader bloomTrailShader = BloomTrailShader.Instance;
        bloomTrailShader.InnerColor = Color.White;
        bloomTrailShader.OuterColor = Color.BlueViolet;
        TrailDrawer.Draw(Main.spriteBatch, Projectile.oldPos, GetBloomColor, GetBloomWidth, bloomTrailShader, Projectile.Size * 0.5f);
    }

    private float GetBloomWidth(float ratio)
    {
        return MathHelper.SmoothStep(36, 8, ratio) * 1.5f;
    }

    private Color GetBloomColor(float ratio)
    {
        return Color.Lerp(Color.Purple * 0.9f, Color.Lerp(Color.DeepSkyBlue, Color.Violet, ExtraMath.Osc(0f, 1f, speed: 24)), ratio);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        PixelationManager.QueuePrimitivesDrawAction(DrawPixelatedTrails);
        this.DrawCentered(ref lightColor);
        return false;
    }

    public override void PostDraw(Color lightColor)
    {
        base.PostDraw(lightColor);
        Texture2D texture2D4 = ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/DimLight").Value;
        Main.spriteBatch.Draw(texture2D4, Projectile.Center - Main.screenPosition, null, new Color(255, 128, 125, 0), Projectile.rotation, new Vector2(32, 32), 0.17f * (7 + 0.6f), SpriteEffects.None, 0f);
        Lighting.AddLight(Projectile.Center, Color.White.ToVector3() * 0.5f * Main.essScale);
    }

    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
        if (this.OwnedByLocalClient())
        {
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<HypnotizerStaminaShotExplosionProjectile>(),
                Projectile.damage, Projectile.knockBack, Projectile.owner);
        }

    }
}


public class HypnotizerStaminaShotExplosionProjectile : ModProjectile
{
    private ref float Timer => ref Projectile.ai[0];
    public override string Texture => TextureRegistry.EmptyTexture;
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 80;
        Projectile.height = 80;
        //Projectile.hostile = true;
        Projectile.timeLeft = 4;
        Projectile.friendly = true;
        Projectile.tileCollide = false;
        Projectile.penetrate = -1;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = -1;
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        base.OnHitNPC(target, hit, damageDone);
        target.AddBuff(BuffID.Confused, 120);
        target.AddBuff(BuffID.ShadowFlame, 60);
    }

    public override void AI()
    {
        base.AI();
        Timer++;
        if (Timer == 1)
        {
            //EXPLODE
            float boomSize = Main.rand.NextFloat(0.15f, 0.2f);
            FXUtil.GlowCircleBoom(Projectile.Center,
                innerColor: Color.White,
                glowColor: Color.Pink,
                outerGlowColor: Color.Blue, duration: 25, baseSize: boomSize);

            FXUtil.ShakeCamera(Projectile.position, 1024, 32);

            SoundStyle explosionSound = new SoundStyle("Stellamod/Assets/Sounds/ExplosionCrystalShard");
            explosionSound.PitchVariance = 0.4f;
            SoundEngine.PlaySound(explosionSound, Projectile.position);

            for (float i = 0; i < 4; i++)
            {
                float progress = i / 4f;
                float rot = progress * MathHelper.ToRadians(360);
                rot += Main.rand.NextFloat(-0.5f, 0.5f);
                Vector2 offset = rot.ToRotationVector2() * 24;
                var particle = FXUtil.GlowCircleLongBoom(Projectile.Center,
                    innerColor: Color.White,
                    glowColor: Color.Pink,
                    outerGlowColor: Color.DarkBlue,
                    baseSize: Main.rand.NextFloat(0.1f, 0.2f),
                    duration: Main.rand.NextFloat(15, 25));
                particle.Rotation = rot + MathHelper.ToRadians(45);
            }
            for (float f = 0f; f < 4; f++)
            {
                float rot = Main.rand.NextFloat(0f, 1f) * MathHelper.TwoPi;
                Vector2 velocity = rot.ToRotationVector2() * Main.rand.NextFloat(1, 2);
                var sp = SmokeParticle.SpawnInAlphaLayer(Projectile.Center, velocity);
                sp.initialColor = Color.DarkGray;
                // Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowDust>(), velocity, newColor: Color.Pink, Scale: Main.rand.NextFloat(0.5f, 1f));
            }
            for (float f = 0f; f < 8; f++)
            {
                float rot = Main.rand.NextFloat(0f, 1f) * MathHelper.TwoPi;
                Vector2 velocity = rot.ToRotationVector2() * Main.rand.NextFloat(8, 16);
                var fx = FXUtil.GlowStretch(Projectile.Center + velocity * 8, velocity);
                fx.OuterGlowColor = Color.Pink;
                fx.VectorScale *= 0.5f;
                // Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowDust>(), velocity, newColor: Color.Pink, Scale: Main.rand.NextFloat(0.5f, 1f));
            }
            for (float f = 0f; f < 4; f++)
            {
                float rot = Main.rand.NextFloat(0f, 1f) * MathHelper.TwoPi;
                Vector2 velocity = rot.ToRotationVector2() * Main.rand.NextFloat(8, 16);
                DustParticle.Spawn(Projectile.Center , velocity);
               // Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowDust>(), velocity, newColor: Color.Pink, Scale: Main.rand.NextFloat(0.5f, 1f));
            }

            for (float f = 0f; f < 16; f++)
            {
                float rot = Main.rand.NextFloat(0f, 1f) * MathHelper.TwoPi;
                Vector2 velocity = rot.ToRotationVector2() * Main.rand.NextFloat(3f, 8f);
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowDust>(), velocity, newColor: Color.Pink, Scale: Main.rand.NextFloat(0.5f, 1f));
            }
        }
    }
}
