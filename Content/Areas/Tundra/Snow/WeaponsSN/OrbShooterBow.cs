using Stellamod.Assets;
using Stellamod.Common;
using Stellamod.Common.Particles;
using Stellamod.Content.CommonMaterials;
using Stellamod.Core.Bases;
using Stellamod.Items;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Tundra.Snow.WeaponsSN;

public class OrbShooterBowOrbBoom : ModProjectile
{
    public override string Texture => TextureRegistry.EmptyTexture;
    ref float Timer => ref Projectile.ai[0];
    ref float Strength => ref Projectile.ai[1];
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.WidthAndHeight = 256;
        Projectile.friendly = true;
        Projectile.LocalHitOnce = true;
        Projectile.light = 0.5f;
        Projectile.tileCollide = false;
        Projectile.ignoreWater = true;
        Projectile.timeLeft = 30;
    }

    public override void AI()
    {
        base.AI();
        Timer++;
        if(Timer == 1)
        {
            ShakeScreenPosition.Shake = 2;
            var sound = AssetReferences.Assets.Sounds.Binding_Abyss_Rune_SoulStar.Asset with { PitchVariance = 0.6f };
            SoundEngine.PlaySound(sound, Projectile.position);

            var fac = ParticleUtils.ParticleFactory.FromSmallBurst(Projectile.Center, TriColorPalette.Icey, new Vector2(5, 15f));
            fac.particleCount *= 2;
            ParticleUtils.CreateSwirlingDustCircle(fac);

            var fx = FXUtil.GlowCircleBoom(Projectile.Center, Color.SkyBlue, Color.DarkBlue, Color.Black, duration: 25, baseSize: 0.18f);
            fx.Scale *= 2f * Strength / 3f;
            foreach (PositionVelocity posVel in new CircleOrientation(Projectile.Center, 64, 48))
            {
                Particles.GlowDust.Spawn(new()
                {
                    position = posVel.position,
                    velocity = posVel.velocity * Main.rand.NextFloat(0.5f, 1f) * 0.3f,
                    color = Color.Lerp(Color.SkyBlue, Color.DarkBlue, Main.rand.NextFloat()),
                    timeLeft = Main.rand.Next(45, 120),
                    scale = Main.rand.NextFloat(0.15f, 0.25f) * 2
                });
            }

            var color = Color.Lerp(Color.DarkBlue, Color.Black, 0.75f);
            foreach (PositionVelocity posVel in new CircleOrientation(Projectile.Center, 64, 32))
            {
                Particles.BackCloud.Spawn(new()
                {
                    position = posVel.position,
                    velocity = posVel.velocity * Main.rand.NextFloat(0.5f, 1f) * 0.3f,
                    color = color,
                    timeLeft = Main.rand.Next(60, 120),
                    scale = Main.rand.NextFloat(0.5f, 1f)
                });
            }
        }
    }

    public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
    {
        base.ModifyHitNPC(target, ref modifiers);
        modifiers.Knockback *= 2;
    }
}


public class OrbShooterBowOrb : ModProjectile
{
    bool _bounceDeath;
    float BounceTime => 24;
    float InScale => MathHelper.Lerp(0f, 1f, EasingFunction.InOutSine(Timer / 30f));
    float GrowthScale => MathHelper.Lerp(0.5f, 1f, BounceCount / 3f);
    float BounceEase => EasingFunction.OutCirc(BounceTimer / BounceTime);
    float BounceScale => MathHelper.Lerp(1f, 1.2f, BounceEase);
    Vector2 Scale => Vector2.One * InScale * GrowthScale * BounceScale * Vector2.Lerp(Vector2.One, new Vector2(1.1f, 0.9f), Projectile.velocity.Length() / 20f);
    ref float BounceCount => ref Projectile.ai[1];
    ref float Timer => ref Projectile.ai[0];
    ref float BounceTimer => ref Projectile.ai[2];
    public override string Texture => AssetReferences.Assets.Textures.Ice.Items.OrbShooterOrb.KEY;
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        Projectile.SetTrailCacheLength(16);
        Main.projFrames[Type] = 4;
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.WidthAndHeight = 32;
        Projectile.friendly = false;
        Projectile.LocalPiercingImmunityTime = 24;
        Projectile.timeLeft = 600;
        Projectile.light = 0.9f;
        Projectile.ignoreWater = true;
    }

    void BounceDieAfter()
    {
        PlayBounceSound();

        Projectile.velocity *= -1.5f;
        BounceTimer = BounceTime;

        PixelPrimitiveCircleFactory.CreateGenericBoom(Projectile.Center, Color.White, Color.SkyBlue, 20, 64);
        var fac = ParticleUtils.ParticleFactory.FromSmallBurst(Projectile.Center, TriColorPalette.Icey, new Vector2(5, 15));
        ParticleUtils.CreateSwirlingDustCircle(fac);
    }

    void PlayBounceSound()
    {
        var tentacle = AssetReferences.Assets.Sounds.TentacleBubbleOut.Asset with { PitchVariance = 0.6f };
        SoundEngine.PlaySound(tentacle, Projectile.position);
    }

    void BounceGrow()
    {
        PlayBounceSound();


        BounceTimer = BounceTime;
        BounceCount++;
        PixelPrimitiveCircleFactory.CreateGenericBoom(Projectile.Center, Color.White, Color.SkyBlue, 20, 64);
        var fac = ParticleUtils.ParticleFactory.FromSmallBurst(Projectile.Center, TriColorPalette.Icey, new Vector2(5, 15));
        ParticleUtils.CreateSwirlingDustCircle(fac);
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        base.OnHitNPC(target, hit, damageDone);
        Projectile.Kill();
    }

    public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
    {
        base.ModifyHitNPC(target, ref modifiers);
        modifiers.FinalDamage *= MathHelper.Lerp(1f, 2f, BounceCount / 3f);
    }

    public override void AI()
    {
        base.AI();
        Timer++;

        if (Timer < 15)
            Projectile.velocity *= 0.92f;

        if(Projectile.timeLeft < 60)
        {
            if (BounceTimer <= 0)
            {
                Projectile.tileCollide = false;
                var npc = NPCHelper.FindClosestNPC(Projectile.Center, 1024);
                if(npc != null)
                {
                    var ve2l = (npc.Center - Projectile.Center).SafeNormalize(Vector2.Zero);
                    Projectile.velocity = Vector2.Lerp(Projectile.velocity, ve2l * 25, 0.07f);
                    if (Vector2.Distance(Projectile.Center, npc.Center) <= 64)
                        Projectile.Kill();
                }
        
            }

        }
        else
        {
            if (BounceTimer <= 0)
            {
                var owner = Projectile.PlayerOwner;
                var ve2l = (owner.Center - Projectile.Center).SafeNormalize(Vector2.Zero);
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, ve2l * 6, 0.03f);
            }

        }

        Projectile.rotation = Projectile.velocity.ToRotation();
        DrawHelper.AnimateTopToBottom(Projectile, 5);
        if (Main.rand.NextBool(3))
        {
            var pos = Projectile.Center + Main.rand.NextVector2Circular(32, 32);
            var vel = Main.rand.NextVector2Circular(2, 2);
            var d = Dust.NewDustPerfect(pos, DustID.Ice, vel, Scale: 1.1f);
            d.noGravity = true;
        }

        if (Main.rand.NextBool(6))
        {
            var pos = Projectile.Center + Main.rand.NextVector2Circular(32, 32);
            var vel = Main.rand.NextVector2Circular(2, 2);
            Particles.LightningSpikeySpark.Spawn(new()
            {
                position = pos,
                velocity = vel,
                timeLeft = 50,
                color = Color.SkyBlue,
                scale = Main.rand.NextFloat(0.66f, 1f) * 0.2f
            });
        }

        if (BounceTimer > 0)
        {
            Projectile.extraUpdates = 1;
            BounceTimer--;
            if (_bounceDeath)
            {
                Projectile.velocity = Projectile.velocity.RotatedBy(0.05f);
            }
            if (_bounceDeath && BounceTimer <= 0)
            {
                Projectile.Kill();
            }
            return;
        }

        var t = ModContent.ProjectileType<OrbArrow>();
        Projectile.extraUpdates = 0;
        foreach (var proj in Main.ActiveProjectiles)
        {
            if (proj.owner != Projectile.owner)
                continue;
            if (proj.identity == Projectile.identity)
                continue;
            if (!proj.friendly)
                continue;
            if (proj.type != t)
                continue;

            if (Timer < 15)
                continue;


            var otherRect = proj.getRect();
            var myRect = Projectile.getRect();
            if (proj.Colliding(myRect, otherRect))
            {
                Projectile.velocity = proj.velocity.SafeNormalize(Vector2.Zero) * Projectile.velocity.Length() * 1.5f;
                BounceGrow();
            }
        }
    }

    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        if (Projectile.velocity.X != oldVelocity.X)
            Projectile.velocity.X = -oldVelocity.X;
        if (Projectile.velocity.Y != oldVelocity.Y)
            Projectile.velocity.Y = -oldVelocity.Y;
        return false;
        //return base.OnTileCollide(oldVelocity);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        var drawer = Projectile.Drawer;
        drawer.scale *= Scale;
        Main.spriteBatch.Draw(drawer);

        drawer.color = Color.Lerp(Color.Transparent, Color.White, BounceEase);
        drawer.color.A = 0;
        Main.spriteBatch.Draw(drawer);

        DrawUtilities.DrawBasicGlow(Main.spriteBatch, Projectile.Center, 0.2f, Color.SkyBlue * 0.15f);
        return false;
    }
    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);


        var firer = ProjFirer.From<OrbShooterBowOrbBoom>(Projectile);
        firer.damage = (int)((float)Projectile.damage * BounceCount);
        firer.ai1 = BounceCount;
        firer.New();
        ShakeScreenPosition.Shake = 3;
        var fx = FXUtil.GlowCircleBoom(Projectile.Center, Color.White, Color.SkyBlue, Color.DarkBlue, 25);
        fx.Scale *= 1.5f;


        PixelPrimitiveCircleFactory.CreateGenericBoom(Projectile.Center, Color.White, Color.SkyBlue, 15, 64);
        var fac = ParticleUtils.ParticleFactory.FromSmallBurst(Projectile.Center, TriColorPalette.Icey, new Vector2(5, 25));
        fac.particleCount *= 2;
        ParticleUtils.CreateSwirlingDustCircle(fac);
    }
}

public class OrbArrow : ModProjectile
{
    public override string Texture => AssetReferences.Assets.Textures.Ice.Items.OrbArrow.KEY;
    public override void SetStaticDefaults()
    {
        Projectile.SetTrailCacheLength(12);
    }

    public override void SetDefaults()
    {
        Projectile.width = 8;
        Projectile.height = 8;
        Projectile.friendly = true;
    //    Projectile.aiStyle = ProjAIStyleID.Arrow;
    }

    public override void AI()
    {
        base.AI();
        if (Main.rand.NextBool(4))
        {
            var pos = Projectile.Center + Main.rand.NextVector2Circular(24, 24);
            var vel = Main.rand.NextVector2Circular(2, 2);
            var d = Dust.NewDustPerfect(pos, DustID.GemDiamond, vel, Scale: Main.rand.NextFloat(0.5f, 1f));
            d.noGravity = true;
        }

        if (Main.rand.NextBool(12))
        {
            FXUtil.GlowDonut(Projectile.Center, -Projectile.velocity.SafeNormalize(Vector2.Zero), Color.LightSkyBlue, Color.DarkBlue, Color.Black, 0.4f);
        }
        Projectile.extraUpdates = 3;
        Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

    }

    public override bool PreDraw(ref Color lightColor)
    {
        DrawUtilities.DrawSpriteAfterImage(Main.spriteBatch, Projectile, Color.SkyBlue, Color.Transparent, 0.15f);
        var drawer = Projectile.Drawer;
        Main.spriteBatch.Draw(drawer);
        DrawUtilities.DrawBasicGlow(Main.spriteBatch, Projectile.Center, 0.2f, Color.SkyBlue * 0.1f);
        return false;
    }

    public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
    {
        base.ModifyHitNPC(target, ref modifiers);
        modifiers.Knockback *= 2;
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        base.OnHitNPC(target, hit, damageDone);
    }

    public override void OnKill(int timeLeft)
    {

    }
}

public class OrbShooterBow : BaseCrossbowItem
{
    public override string Texture => AssetReferences.Assets.Textures.Ice.Items.OrbShooterBow.KEY;
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.damage = 13;
        Item.rare = ItemRarityID.Green;
        Item.useAnimation = Item.useTime = 30;
        staminaCost = 1;
    }

    public override void ShootBow(Player player, EntitySource_ItemUse_WithAmmo source, ShootParams shootParams)
    {
        Vector2 fireVelocity = shootParams.velocity * shootParams.speed;
        fireVelocity *= 3;
        fireVelocity *= shootParams.chargeStrength;

        shootParams.projToShoot = ModContent.ProjectileType<OrbArrow>();
        float bowDamage = shootParams.damage * shootParams.chargeStrength;
        Projectile crossShot1 = Projectile.NewProjectileDirect(source, shootParams.position, fireVelocity,
            shootParams.projToShoot, (int)bowDamage, shootParams.knockBack, player.whoAmI, ai0: shootParams.projToShoot);
        crossShot1.GetGlobalProjectile<CrossbowGlobalProjectile>().isCrossbowShot = true;
    }
    public override void StaminaShootBow(Player player, EntitySource_ItemUse_WithAmmo source, ShootParams shootParams)
    {
        base.StaminaShootBow(player, source, shootParams);
        float bowDamage = shootParams.damage * shootParams.chargeStrength * 2;
        Vector2 bulletVelocity = shootParams.velocity * shootParams.chargeStrength * 32;
        Projectile.NewProjectile(source, shootParams.position, bulletVelocity,
            ModContent.ProjectileType<OrbShooterBowOrb>(), (int)bowDamage, shootParams.knockBack, player.whoAmI);
    }

    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew(
            mold: ModContent.ItemType<BlankBow>(),
            material: ModContent.ItemType<WinterbornShard>());
    }
}
