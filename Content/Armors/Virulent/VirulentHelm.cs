using Stellamod.Assets;
using Stellamod.Buffs;
using Stellamod.Common.ArmorRework;
using Stellamod.Common.Shaders;
using Stellamod.Content.Armors.Scrappy;
using Stellamod.Core;
using Stellamod.Core.Pixelation;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Visual.Particles;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Armors.Virulent;

public class VirulentBullet : ScarletProjectile
{
    private ref float Timer => ref Projectile.ai[0];
    public override string Texture => TextureRegistry.EmptyTexture;
    public override void SetDefaults()
    {
        base.SetDefaults();
        TrailCacheLength = 8;
        Projectile.width = 8;
        Projectile.height = 8;
        Projectile.friendly = true;
        Projectile.timeLeft = 180;
        Projectile.extraUpdates = 2;
    }
    public override void AI()
    {
        base.AI();
        Timer++;
        if (Timer == 1)
        {
            SoundStyle shootSound = new SoundStyle("Stellamod/Assets/Sounds/GunShootNew7");
            shootSound.PitchVariance = 0.3f;
            shootSound.Volume = 0.005f;
            SoundEngine.PlaySound(shootSound, Projectile.position);

            FXUtil.GlowCircleBoom(Projectile.Center, Color.Green, Color.DarkGreen, Color.Black);
            for (float f = 0; f < 1; f++)
            {
                Vector2 fireVelocity = Projectile.velocity.SafeNormalize(Vector2.Zero);
                fireVelocity = fireVelocity.RotatedByRandom(MathHelper.ToRadians(60));
                fireVelocity *= Main.rand.NextFloat(3f, 8f);

                DustParticleSpawnParams spawnParams = DustParticleSpawnParams.Default;
                spawnParams.outerColor = Color.Green;
                spawnParams.scaleRange *= 0.5f;
                DustParticle.Spawn(Projectile.Center, fireVelocity, spawnParams);
            }
        }
        if (Projectile.ai[1] == 1 && Timer > 50)
            Projectile.velocity.Y += 0.15f;
        Projectile.velocity *= 1.01f;
        Projectile.rotation = Projectile.velocity.ToRotation();
    }


    private Color GetTrailColor(float completionRatio)
    {
        float osc = MathF.Sin(Main.GlobalTimeWrappedHourly * 4 + completionRatio * 8) * 0.5f + 0.5f;
        return Color.Lerp(Color.White, Color.Green, osc);
    }

    private float GetTrailWidth(float completionRatio)
    {
        return MathHelper.SmoothStep(3, 2, completionRatio);
    }
    public override bool PreDraw(ref Color lightColor)
    {
        PixelationManager.QueueSpritebatchDrawAction(DrawHead);
        PixelationManager.QueuePrimitivesDrawAction(DrawPixelated);
        return false;
    }

    public void DrawPixelated(GraphicsDevice graphicsDevice)
    {
        var shader = RichLaserShader.Instance;
        shader.LaserColor = Color.White;
        shader.InnerColor = Color.Green;
        shader.OuterColor = Color.DarkGreen;
        shader.LaserTexture = AssetManager.LaserTextures.TexturedLaser;
        shader.BloomTexture = AssetManager.LaserTextures.TexturedLaser2;
        //This just applis the shader changes
        TrailDrawer.Draw(Main.spriteBatch, OldCenterPos, GetTrailColor, GetTrailWidth, shader);
    }

    private void DrawHead(SpriteBatch sb, Vector2 screenPos)
    {
        SpritebatchDrawer headDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.StarFlare1, Projectile.Center);
        headDrawer.scale *= 0.3f;
        headDrawer.scale.Y *= 0.15f;
        headDrawer.scale.X *= 1;
        headDrawer.rotation = Projectile.rotation;
        headDrawer.color = Color.Green;
        headDrawer.color.A = 0;
        sb.Draw(headDrawer);

        headDrawer.color = Color.White;
        headDrawer.color.A = 0;
        headDrawer.scale *= 0.75f;
        sb.Draw(headDrawer);

    }
    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
        float numDust = 1f;
        for (float d = 0; d < numDust; d++)
        {
            Vector2 velocity = Main.rand.NextVector2Circular(8, 8);
            Dust.NewDustPerfect(Projectile.Center,
                ModContent.DustType<GlowDust>(), velocity, newColor: Color.Green, Scale: Main.rand.NextFloat(0.5f, 2f));
        }
        for (float f = 0; f < 2; f++)
        {
            Vector2 fireVelocity = -Projectile.oldVelocity.SafeNormalize(Vector2.Zero);
            fireVelocity = fireVelocity.RotatedByRandom(MathHelper.ToRadians(60));
            fireVelocity *= Main.rand.NextFloat(3f, 8f);

            DustParticleSpawnParams spawnParams = DustParticleSpawnParams.Default;
            spawnParams.outerColor = Color.Green;
            spawnParams.scaleRange *= 0.5f;
            DustParticle.Spawn(Projectile.Center, fireVelocity, spawnParams);
        }
    }
}

public class VirulentTurret : ModProjectile
{
    private Vector2 _fireOffset;
    private Player Owner => Main.player[Projectile.owner];
    private ref float Timer => ref Projectile.ai[0];
    private ref float RandOffset => ref Projectile.ai[1];
    private ref float OffsetTimer => ref Projectile.ai[2];
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
        Projectile.timeLeft = 3;
        Projectile.penetrate = -1;
        Projectile.tileCollide = false;
        Projectile.light = 1.5f;
    }

    public override void AI()
    {
        base.AI();
        if (Owner.GetModPlayer<AcidPlayer>().hasSetBonus)
            Projectile.timeLeft = 3;

        float targetRotation = Projectile.velocity.X * 0.02f;
        SummonHelper.SearchForTargets(Owner, Projectile, out bool foundTarget, out float distanceFromTarget, out Vector2 targetCenter);
        if (foundTarget)
        {
            Vector2 fireDirection = targetCenter - Projectile.Center;
            fireDirection = fireDirection.SafeNormalize(Vector2.Zero);
            targetRotation = fireDirection.ToRotation();
            if (Projectile.spriteDirection == -1)
                targetRotation += MathHelper.Pi;
            Timer++;
            if (Timer >= 7)
            {
                _fireOffset = -fireDirection * 32;
                Vector2 firePoint = Projectile.Center + fireDirection * 24;
                var p = FXUtil.GlowCircleBoom(firePoint, Color.Yellow, Color.Red, Color.Black);
                p.Scale *= 0.5f;

                MuzzleFlashParticle flashParticle = MuzzleFlashParticle.Spawn(firePoint, fireDirection, Color.Red);
                flashParticle.innerColor = Color.Yellow;
                flashParticle.bloomColor = Color.Green;
                flashParticle.Scale *= 0.25f;

                for (float f = 0; f < 4; f++)
                {
                    DustParticleSpawnParams spawnParams = new DustParticleSpawnParams
                    {
                        gravity = 0f,
                        innerColor = Color.Yellow,
                        outerColor = Color.Green,
                        scaleRange = new Vector2(0.3f, 1f)
                    };
                    var dp = DustParticle.Spawn(firePoint, (fireDirection * 8).RotatedByRandom(0.3f) * Main.rand.NextFloat(0.5f, 1f), spawnParams);
                    dp.dampening = 0.1f;
                }

                if (this.OwnedByLocalClient())
                {
                    RandOffset = Main.rand.NextFloat(-10, 10f);
                    Projectile.netUpdate = true;
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), firePoint, fireDirection * 8,
                        ModContent.ProjectileType<VirulentBullet>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                }


                Timer = 0;
            }
        }
        else
        {
            Timer--;
            if (Timer <= 0)
                Timer = 0;
        }

        _fireOffset *= 0.8f;
        OffsetTimer++;
        if (OffsetTimer == 1)
        {
            if (this.OwnedByLocalClient())
            {
                OffsetTimer += Main.rand.NextFloat(0, 180);
                Projectile.netUpdate = true;
            }
        }

        Projectile.spriteDirection = Projectile.velocity.X > 0 ? 1 : -1;
        Projectile.rotation = Utils.AngleLerp(Projectile.rotation, targetRotation, 0.3f);

        Vector2 targetPosition = Owner.Center + new Vector2(0, -16 + RandOffset);
        targetPosition += new Vector2(0, 128);
        targetPosition.Y -= 200;
        Vector2 velocityToPlayer = (targetPosition - Projectile.Center);
        velocityToPlayer = velocityToPlayer.SafeNormalize(Vector2.Zero);
        float dist = Vector2.Distance(Projectile.Center, targetPosition);
        if (dist <= 0)
            dist = 1;

        float interp = dist / 384;
        interp = EasingFunction.InOutSine(interp);
        float speed = MathHelper.Lerp(6, 50, interp);

        if (dist < speed)
            speed = dist;
        velocityToPlayer *= speed;
        Projectile.velocity = velocityToPlayer;
    }

    public override bool PreDraw(ref Color lightColor)
    {
        SpritebatchDrawer drawer = SpritebatchDrawer.FromProjectile(Projectile);
        drawer.worldPosition += _fireOffset;
        Main.spriteBatch.Draw(drawer);
        return false;
        //return base.PreDraw(ref lightColor);
    }
}

public class AcidPlayer : ModPlayer
{
    /*
     * Immunity to acid water contamination, 
standing still gives you an acid aura that stays where you were when you leave The aura will deal damage to enemies for a certain amount of time
    */

    private int _acidTimer;
    public bool hasSetBonus;

    public override void ResetEffects()
    {
        hasSetBonus = false;
    }

    public override void PostUpdateEquips()
    {
        if (!hasSetBonus)
            return;

        //Immunity to contamination
        Player.ClearBuff(ModContent.BuffType<AcidFlame>());
        Player.ClearBuff(ModContent.BuffType<Irradiation>());

        //Standing still for the acid aura
        if (Player.velocity == Vector2.Zero
            && Player.ownedProjectileCounts[ModContent.ProjectileType<AcidAuraProj>()] == 0)
        {
            _acidTimer++;
        }
        else
        {
            _acidTimer = 0;
        }

        if (_acidTimer >= 30 && Player.whoAmI == Main.myPlayer)
        {
            int damage = 18;
            Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, Vector2.Zero,
                ModContent.ProjectileType<AcidAuraProj>(), damage, 1, Player.whoAmI);
            _acidTimer = 0;
        }

        if (Player.whoAmI == Main.myPlayer && Player.ownedProjectileCounts[ModContent.ProjectileType<VirulentTurret>()] < 1)
        {
            Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, Vector2.Zero,
                ModContent.ProjectileType<VirulentTurret>(), 19, 4, Player.whoAmI);
        }
    }
}

[AutoloadEquip(EquipType.Head)]
public class VirulentHelm : ModItem
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        ArmorSetSystem.RegisterArmorSet<VirulentHelm, VirulentArmor, VirulentLegs>();
    }

    public override void SetDefaults()
    {
        Item.width = 40;
        Item.height = 30;
        Item.value = 10000;
        Item.rare = ItemRarityID.Blue;
    }

    public override void UpdateEquip(Player player)
    {
        var stats = player.GetStats();
        stats.rangedGunAmmoAmountPct += 1;
        stats.defenseBonus += 5;
        stats.accessorySlots++;
    }

    public override bool IsArmorSet(Item head, Item body, Item legs)
    {
        return body.type == ModContent.ItemType<VirulentArmor>() && legs.type == ModContent.ItemType<VirulentLegs>();
    }

    public override void UpdateArmorSet(Player player)
    {
        player.GetModPlayer<AcidPlayer>().hasSetBonus = true;
    }
}

[AutoloadEquip(EquipType.Body)]
public class VirulentArmor : ModItem
{
    public override void SetDefaults()
    {
        Item.width = 28;
        Item.height = 22;
        Item.value = 80000;
        Item.rare = ItemRarityID.Blue;
    }

    public override void UpdateEquip(Player player)
    {
        var stats = player.GetStats();
        stats.rangedDamage += 0.25f;
        stats.defenseBonus += 9;
        stats.accessorySlots += 2;
    }
}

[AutoloadEquip(EquipType.Legs)]
public class VirulentLegs : ModItem
{
    public override void SetDefaults()
    {
        Item.width = 28;
        Item.height = 22;
        Item.value = 10000;
        Item.rare = ItemRarityID.Blue;
    }

    public override void UpdateEquip(Player player)
    {
        var stats = player.GetStats();
        stats.defenseBonus += 6;
        stats.accessorySlots += 1;
    }
}
