using Stellamod.Assets;
using Stellamod.Common.GunSystem;
using Stellamod.Common.Particles;
using Stellamod.Content.Dusts;
using Stellamod.Core;
using Stellamod.Core.Bases;
using Stellamod.Core.Particles;
using Stellamod.Core.Pixelation;
using Stellamod.Visual.Particles;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Tundra.Abyss.WeaponsAB;

public class VenaticiBoom : ModProjectile
{
    private ref float Timer => ref Projectile.ai[0];
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        Main.projFrames[Type] = 30;
    }
    
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 64;
        Projectile.height = 64;
        Projectile.penetrate = -1;
        Projectile.friendly = true;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = -1;
        Projectile.tileCollide = false;
        Projectile.timeLeft = 120;
    }

  
    public override void AI()
    {
        base.AI();
        Timer++;
        if(Timer == 1)
        {
            var starExplosion = AssetReferences.Assets.Sounds.Starexplosion.Asset with { PitchVariance = 1f, Volume = 0.6f };
            SoundEngine.PlaySound(starExplosion, Projectile.position);

            for(float f = 0; f < 16; f++)
            {
                Vector2 pos = Projectile.Center;
                Vector2 vel = Main.rand.NextVector2Circular(16, 16);
                Particles.SwirlingFlameDust.Spawn(BitDustFactory.SlowingOverTime with
                {
                    position = pos,
                    velocity = vel,
                    innerColor = Color.White.ToVector4(),
                    outerColor = Color.Violet.ToVector4(),
                    scale = new Vector2(Main.rand.NextFloat(0.4f, 0.8f))
                });
            }
        }

        Projectile.frame = (int)(Timer / 2);
        if (Projectile.frame >= Main.projFrames[Type])
            Projectile.Kill();
    }
    public override bool PreDraw(ref Color lightColor)
    {
        SpritebatchDrawer starDrawer = SpritebatchDrawer.FromProjectile(Projectile);
        starDrawer.color = Color.White;
        starDrawer.color.A = 0;
        starDrawer.scale *= 2f;
        Main.spriteBatch.Draw(starDrawer);
        return false;
    }

}
public class VenaticiStarShot : ModProjectile
{
    private ref float Timer => ref Projectile.ai[0];
    private ref float Charge => ref Projectile.ai[1];
    private Player Owner => Main.player[Projectile.owner];
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        ProjectileID.Sets.TrailCacheLength[Type] = 32;
        ProjectileID.Sets.TrailingMode[Type] = 2;
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 64;
        Projectile.height = 64;
        Projectile.friendly = true;
        Projectile.tileCollide = true;
        Projectile.timeLeft = 180;
    }

    public override void AI()
    {
        base.AI();
        Timer++;
        if(Timer % 8 == 0)
        {
            Particles.SwirlingFlameDust.Spawn(BitDustFactory.SlowingOverTime with
            {
                position = Projectile.Center,
                velocity =-Projectile.velocity.RotatedByRandom(0.5f) * Main.rand.NextFloat(0.2f, 0.8f),
                innerColor = Color.White.ToVector4(),
                outerColor = Color.Pink.ToVector4(),
                scale = new Vector2(Main.rand.NextFloat(0.3f, 0.8f))
            });
        }

        if (Projectile.velocity.Length() < 1f)
            Projectile.Kill();
        Projectile.scale = Charge;
        Projectile.velocity *= 0.96f;
        Projectile.rotation += Projectile.velocity.X * 0.03f;
    }


    public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
    {
        base.ModifyHitNPC(target, ref modifiers);
        modifiers.FinalDamage *= Charge * 2;
    }

    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
        if (this.OwnedByLocalClient())
        {
            ProjFirer firer = ProjFirer.From<VenaticiBoom>(Projectile);
            firer.New();
        }
    }


    private void DrawPixelatedStar(SpriteBatch sb, Vector2 sp)
    {
        SpritebatchDrawer drawer = SpritebatchDrawer.FromProjectile(Projectile);
        drawer.color = DrawUtilities.InterpolateColorArray(ExtraMath.Osc(0f, 1f, speed: 1, offset: Projectile.whoAmI), Color.SkyBlue, Color.Magenta, Color.White, Color.DarkBlue);
        drawer.color.A = 0;

        foreach (OldPosition oldPos in Projectile.IterateOldPosBackwards())
        {
            SpritebatchDrawer trailDrawer = drawer;
            trailDrawer.worldPosition = oldPos.position + Projectile.Size * 0.5f;
            trailDrawer.color *= MathHelper.SmoothStep(1f, 0f, oldPos.progress) * 0.1f;
            trailDrawer.scale *= MathHelper.SmoothStep(1f, 0.5f, oldPos.progress);
            trailDrawer.rotation = Projectile.oldRot[oldPos.index];
            sb.Draw(trailDrawer);
        }
        drawer.scale *= 0.9f;
        sb.Draw(drawer);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        PixelationManager.QueueSpritebatchDrawAction(DrawPixelatedStar);
        return false;
    }
}
public class VenaticiStar : ModProjectile
{
    private float _charge;
    private float Charge_Time => 95;
    private ref float Timer => ref Projectile.ai[0];
    private ref float Shot => ref Projectile.ai[1];
    private Player Owner => Main.player[Projectile.owner];
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
        Projectile.penetrate = 3;
        Projectile.tileCollide = false;
    }

    public override bool ShouldUpdatePosition()
    {
        return true;
    }

    public override void AI()
    {
        base.AI();

        if (Timer == 1)
        {
            SoundStyle impact = AssetManager.GetSound("Fire/FireballCharge1");
            impact.PitchVariance = 0.3f;
            impact.Pitch = 0.8f;
            SoundEngine.PlaySound(impact, Projectile.position);
        }

        if (this.OwnedByLocalClient() && Shot == 0)
        {
            Timer++;
            Vector2 targetPosition = Owner.Center + (Main.MouseWorld - Owner.Center).SafeNormalize(Vector2.Zero) * 64;
            targetPosition.Y -= 16;
            Vector2 targetVelocity = targetPosition - Projectile.Center;
            Projectile.velocity = targetVelocity;
            Projectile.netUpdate = true;
        }

        if (this.OwnedByLocalClient() && !Owner.channel && Shot == 0)
        {
            Vector2 velocity = (Main.MouseWorld - Projectile.Center).SafeNormalize(Vector2.Zero) * _charge * 15;
            Shot = 1;
            var shootSound = AssetReferences.Assets.Sounds.StarFlower2.Asset with { PitchVariance = 0.3f, Volume = 0.5f };
            SoundEngine.PlaySound(shootSound, Projectile.position);
            Projectile.velocity = velocity;
            Projectile.netUpdate = true;
        }

        if(Shot == 1)
        {
            Projectile.penetrate = 1;
            if (Timer % 8 == 0)
            {
                Particles.SwirlingFlameDust.Spawn(BitDustFactory.SlowingOverTime with
                {
                    position = Projectile.Center,
                    velocity = -Projectile.velocity.RotatedByRandom(0.5f) * Main.rand.NextFloat(0.2f, 0.8f),
                    innerColor = Color.White.ToVector4(),
                    outerColor = Color.Pink.ToVector4(),
                    scale = new Vector2(Main.rand.NextFloat(0.3f, 0.8f))
                });
            }

            Projectile.velocity *= 0.96f;
            if (Projectile.velocity.Length() < 1f)
                Projectile.Kill();
        }


        if(Timer % 6 == 0)
        {
            Vector2 spawnPos = Main.rand.NextVector2CircularEdge(164, 164) + Projectile.Center;
            Particles.SwirlingFlameDust.Spawn(BitDustFactory.SlowingOverTime with
            {
                position = spawnPos,
                velocity = (Projectile.Center - spawnPos).SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(0.6f, 25),
                innerColor = Color.White.ToVector4(),
                outerColor = Color.Pink.ToVector4(),
                scale = new Vector2(Main.rand.NextFloat(0.3f, 0.8f))
            });
        }
        _charge = MathHelper.Clamp(EasingFunction.OutSine(Timer / Charge_Time), 0f, 1f);
        Projectile.scale = MathHelper.SmoothStep(0f, 1f, _charge);
        Projectile.rotation += MathHelper.Lerp(0.02f, 0.1f, EasingFunction.InOutSine(Timer / Charge_Time));
    }
    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
        var sound = AssetReferences.Assets.Sounds.StarKeeper.Asset with { PitchVariance = 0.4f };
        SoundEngine.PlaySound(sound, Projectile.position);
        PixelPrimitiveCircleFactory.CreateGenericBoom(Projectile.Center, Color.White, Color.Pink, 32, 64);
        for(float f = 0; f < 12; f++)
        {
            Particles.SwirlingFlameDust.Spawn(BitDustFactory.SlowingOverTime with
            {
                position = Projectile.Center,
                velocity = Main.rand.NextVector2Circular(12, 12),
                innerColor = Color.White.ToVector4(),
                outerColor = Color.Pink.ToVector4(),
                scale = new Vector2(Main.rand.NextFloat(0.3f, 0.8f))
            });
        }
        if (this.OwnedByLocalClient())
        {
            float numStars =  5;
       

            for (float f = 0; f < numStars; f++)
            {
                float alpha = f / numStars;
                float substract = f * 0.2f;
                float c = MathHelper.Clamp((_charge - substract) / 0.2f, 0f, 1f);
                if(c > 0)
                {
                    ProjFirer starFirer = ProjFirer.From<VenaticiStarShot>(Projectile);
                    float rot = alpha * MathHelper.TwoPi + Projectile.rotation;
                    starFirer.velocity = rot.ToRotationVector2() * 15;
                    starFirer.ai1 = c;
                    starFirer.New();
                }

            }
        }
    
    }

    private void DrawPixelatedStar(SpriteBatch spriteBatch, Vector2 sp)
    {
        float num = 5;
        for (float f = 0; f < num; f++)
        {
            float alpha = f / num;
            float substract = f * 0.2f;
            float c = MathHelper.Clamp((_charge - substract) / 0.2f, 0f, 1f);

            SpritebatchDrawer starShard = SpritebatchDrawer.FromProjectile(Projectile);
            starShard.color = DrawUtilities.InterpolateColorArray(ExtraMath.Osc(0f, 1f, speed: 3, offset: Projectile.whoAmI + f), Color.SkyBlue, Color.Magenta, Color.White, Color.DarkBlue);
            starShard.color *= c;
            starShard.color.A = 0;
            starShard.rotation = alpha * MathHelper.TwoPi + Projectile.rotation;
            starShard.LeftCenterOrigin();
            starShard.scale *= c * 0.7f * ExtraMath.Osc(0.7f, 1f, speed: 4, Projectile.whoAmI + f);
            spriteBatch.Draw(starShard);
        }

        SpritebatchDrawer glowCircle = SpritebatchDrawer.FromTextureAsset(AssetReferences.Assets.GlowMasks.SimpleGlowCircle.Asset, Projectile.Center);
        glowCircle.color = Color.White * _charge;
        glowCircle.color.A = 0;
        glowCircle.scale *= MathHelper.Lerp(0.5f, 1f, _charge) * 0.2f;
        spriteBatch.Draw(glowCircle);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        PixelationManager.QueueSpritebatchDrawAction(DrawPixelatedStar, DrawLayer.OverPlayers);
        return false;
    }
}

public class Venatici : BaseGun
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.DefaultToGun();
        Item.DefaultToChargeGun();
        Item.shootSpeed = 30;
        Item.damage = 15;
        Item.shoot = ModContent.ProjectileType<VenaticiStar>();
        muzzleOrigin = new Vector2(78, 17);
    }

    public override Vector2? HoldoutOffset()
    {
        return new Vector2(20, 4);
    }

    public override void ShootEffects(Vector2 position, Vector2 velocity)
    {
        muzzleOrigin = new Vector2(78, 17);
        BasicMuzzleFlash(position, velocity, Color.SkyBlue, Color.DarkBlue);


        for (float f = 0; f < 9; f++)
        {
            Vector2 posToShootFrom = position + velocity * 2;
            posToShootFrom += Main.rand.NextVector2Circular(32, 32);
            Vector2 vel = position - posToShootFrom;
            vel = vel.SafeNormalize(Vector2.Zero);
            vel = vel.RotatedByRandom(1.5f);
            vel *= Main.rand.NextFloat(0.6f, 15f);

            Particles.SwirlingFlameDust.Spawn(BitDustFactory.SlowingOverTime with
            {
                position = posToShootFrom,
                velocity = vel,
                innerColor = Color.SkyBlue.ToVector4(),
                outerColor = Color.DarkBlue.ToVector4(),
                scale = new Vector2(Main.rand.NextFloat(0.6f, 1.6f))
            });
        }
    }

    public override bool CanUseItem(Player player)
    {
        return base.CanUseItem(player);
    }

    public override void SetMagazine(ref GunReloadParams fireParams)
    {
        base.SetMagazine(ref fireParams);
        fireParams.maxAmmo = 3;
        fireParams.reloadWindow = 150;
    }
    public override bool ShootProjectile(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        type = ModContent.ProjectileType<VenaticiStar>();
        Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);

        return false;
    }
}
