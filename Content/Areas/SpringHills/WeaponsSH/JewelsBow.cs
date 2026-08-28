using Stellamod.Common.Particles;
using Stellamod.Common.RarityRendering;
using Stellamod.Core.Bases;
using Stellamod.Core.Particles;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;


namespace Stellamod.Content.Areas.SpringHills.WeaponsSH;


public class JewelsBow : BaseCrossbowItem
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.damage = 8;
        Item.rare = ModContent.RarityType<BossRewardRarity>();
    }

    public override void StaminaShootBow(Player player, EntitySource_ItemUse_WithAmmo source, ShootParams shootParams)
    {
        base.StaminaShootBow(player, source, shootParams);
        float bowDamage = shootParams.damage * shootParams.chargeStrength;
        for (float f = 0; f < 6; f++)
        {
            Vector2 position = shootParams.position;
            Vector2 velocity = shootParams.velocity * shootParams.chargeStrength * 24;
            velocity = velocity.RotatedByRandom(MathHelper.ToRadians(15));
            velocity *= Main.rand.NextFloat(0.4f, 1f);
            velocity.Y -= 5;
            Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<JewelShot>(), (int)bowDamage, 0, player.whoAmI);
        }
    }
}

public class JewelShot : ModProjectile
{
    private Player Owner => Main.player[Projectile.owner];
    private ref float Timer => ref Projectile.ai[0];
    private ref float BounceCount => ref Projectile.ai[1];
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        ProjectileID.Sets.TrailCacheLength[Type] = 8;
        ProjectileID.Sets.TrailingMode[Type] = 2;
        Main.projFrames[Type] = 2;
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 12;
        Projectile.height = 12;
        Projectile.penetrate = -1;
        Projectile.friendly = true;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 20;
        Projectile.tileCollide = true;
        Projectile.ignoreWater = true;
    }

    public override void AI()
    {
        base.AI();
        Timer++;
        if (Timer % 12 == 0)
        {
            int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.GemSapphire);
            Main.dust[d].noGravity = true;
        }

        Projectile.velocity.X *= 0.99f;
        Projectile.velocity.Y += 0.5f;
        Projectile.rotation = Projectile.velocity.ToRotation();
    }

    private void BounceFX()
    {
        var p2 = LegacyParticle.NewParticle<GlowDonutParticle>(Projectile.Center, -Projectile.velocity.SafeNormalize(Vector2.Zero), newColor: Color.White);
        p2.fadeToColor = Color.DarkBlue;
        p2.Scale *= 0.185f;
        var sound = SoundID.DD2_CrystalCartImpact with { PitchVariance = 0.4f, Volume = 0.4f };
        SoundEngine.PlaySound(sound, Projectile.position);
    }
    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        if (Projectile.velocity.X != oldVelocity.X)
        {
            Projectile.velocity.X *= -1;
            BounceCount++;
            BounceFX();

        }

        if (Projectile.velocity.Y != oldVelocity.Y)
        {
            Projectile.velocity.Y *= -1;
            BounceCount++;
            BounceFX();

        }

        if (BounceCount >= 3)
            return true;
        return false;
    }


    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
        var fx = FXUtil.GlowCircleBoom(Projectile.Center, Color.White, Color.SkyBlue, Color.Blue);
        fx.Scale *= 0.6f;
        for (int i = 0; i < 8; i++)
        {
            Particles.SwirlingFlameDust.Spawn(BitDustFactory.Default with
            {
                position = Projectile.Center + Main.rand.NextVector2Circular(32, 32),
                velocity = Main.rand.NextVector2Circular(14, 14),
                outerColor = Color.Blue.ToVector4(),
                innerColor = Color.SkyBlue.ToVector4(),
                timeLeft = 60,
                scale = new Vector2(Main.rand.NextFloat(0.5f, 1.2f))
            });
        }
    }
    public override bool PreDraw(ref Color lightColor)
    {
        SpritebatchDrawer drawer = SpritebatchDrawer.FromProjectile(Projectile);
        foreach (OldPosition pos in Projectile.IterateOldPosBackwards())
        {
            SpritebatchDrawer afterDrawer = drawer;
            afterDrawer.worldPosition = pos.position + Projectile.Size * 0.5f;
            afterDrawer.color = Color.Lerp(Color.Aqua, Color.Transparent, pos.progress) * 0.3f;
            afterDrawer.color.A = 0;
            afterDrawer.rotation = Projectile.oldRot[pos.index];
            Main.spriteBatch.Draw(afterDrawer);
        }

        Main.spriteBatch.Draw(drawer);

        SpritebatchDrawer outlineDrawer = drawer;
        outlineDrawer.VerticalFrame(1, Main.projFrames[Type]);
        outlineDrawer.color = Color.Lerp(Color.Transparent, Color.Aquamarine, ExtraMath.Osc(0f, 1f, speed: 12, Projectile.identity));
        Main.spriteBatch.Draw(outlineDrawer);
        return false;
    }
}