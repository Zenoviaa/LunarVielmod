using Stellamod.Assets;
using Stellamod.Common.ArmorRework;
using Stellamod.Common.Shaders;
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

namespace Stellamod.Content.Armors.Scrappy;

public class ScrappyPlayer : ModPlayer
{
    public bool hasSetBonus;
    public override void ResetEffects()
    {
        hasSetBonus = false;
    }
    public override void PostUpdateEquips()
    {
        base.PostUpdateEquips();
        if (!hasSetBonus)
            return;
        if (Main.myPlayer != Player.whoAmI)
            return;

        if (Player.ownedProjectileCounts[ModContent.ProjectileType<ScrappyTurret>()] < 3)
        {
            Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, Vector2.Zero,
                ModContent.ProjectileType<ScrappyTurret>(), 40, 4, Player.whoAmI);
        }
    }
}
public class ScrappyBullet : ScarletProjectile
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

            FXUtil.GlowCircleBoom(Projectile.Center, Color.Red, Color.DarkRed, Color.Black);
            for (float f = 0; f < 4; f++)
            {
                Vector2 fireVelocity = Projectile.velocity.SafeNormalize(Vector2.Zero);
                fireVelocity = fireVelocity.RotatedByRandom(MathHelper.ToRadians(60));
                fireVelocity *= Main.rand.NextFloat(3f, 8f);

                DustParticleSpawnParams spawnParams = DustParticleSpawnParams.Default;
                spawnParams.outerColor = Color.Red;
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
        return Color.Lerp(Color.White, Color.Red, osc);
    }

    private float GetTrailWidth(float completionRatio)
    {
        return MathHelper.SmoothStep(5, 3, completionRatio);
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
        shader.InnerColor = Color.Red;
        shader.OuterColor = Color.DarkRed;
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
        headDrawer.color = Color.Red;
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
                ModContent.DustType<GlowDust>(), velocity, newColor: Color.Red, Scale: Main.rand.NextFloat(0.5f, 2f));
        }
        for (float f = 0; f < 4; f++)
        {
            Vector2 fireVelocity = -Projectile.oldVelocity.SafeNormalize(Vector2.Zero);
            fireVelocity = fireVelocity.RotatedByRandom(MathHelper.ToRadians(60));
            fireVelocity *= Main.rand.NextFloat(3f, 8f);

            DustParticleSpawnParams spawnParams = DustParticleSpawnParams.Default;
            spawnParams.outerColor = Color.Red;
            spawnParams.scaleRange *= 0.5f;
            DustParticle.Spawn(Projectile.Center, fireVelocity, spawnParams);
        }
    }
}

public class ScrappyTurret : ModProjectile
{
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
        if (Owner.GetModPlayer<ScrappyPlayer>().hasSetBonus)
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
            if (Timer >= 65 + RandOffset)
            {

                Vector2 firePoint = Projectile.Center + fireDirection * 24;
                var p = FXUtil.GlowCircleBoom(firePoint, Color.Yellow, Color.Red, Color.Black);
                p.Scale *= 0.5f;

                var sp = SmokeParticle.SpawnInAlphaLayer(firePoint, fireDirection * 8, Color.DarkGray);
                sp.initialColor = Color.Lerp(Color.Red, Color.Black, 0.6f);
                sp.fast = true;

                MuzzleFlashParticle flashParticle = MuzzleFlashParticle.Spawn(firePoint, fireDirection, Color.Red);
                flashParticle.innerColor = Color.Yellow;
                flashParticle.bloomColor = Color.Red;
                flashParticle.Scale *= 0.25f;

                for (float f = 0; f < 4; f++)
                {
                    DustParticleSpawnParams spawnParams = new DustParticleSpawnParams
                    {
                        gravity = 0f,
                        innerColor = Color.Yellow,
                        outerColor = Color.Red,
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
                        ModContent.ProjectileType<ScrappyBullet>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
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

        int index = SummonHelper.GetProjectileIndex(Projectile);
        Vector2 targetPosition = Owner.Center + new Vector2(0, -16 + RandOffset).RotatedBy(OffsetTimer * 0.2f);
        targetPosition += new Vector2(0, 128).RotatedBy(index * 0.3f);
        targetPosition.Y -= 200;
        Vector2 velocityToPlayer = (targetPosition - Projectile.Center);
        velocityToPlayer = velocityToPlayer.SafeNormalize(Vector2.Zero);
        float dist = Vector2.Distance(Projectile.Center, targetPosition);
        if (dist <= 0)
            dist = 1;

        float interp = dist / 384;
        interp = EasingFunction.InOutSine(interp);
        float speed = MathHelper.Lerp(6, 20, interp);

        if (dist < speed)
            speed = dist;
        velocityToPlayer *= speed;
        Projectile.velocity = Vector2.Lerp(Projectile.velocity, velocityToPlayer, 0.04f);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        return base.PreDraw(ref lightColor);
    }

}

[AutoloadEquip(EquipType.Head)]
public class ScrappyHead : ModItem
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        ArmorSetSystem.RegisterArmorSet<ScrappyHead, ScrappyBody, ScrappyLegs>(ArmorGroup.Act_II);
    }

    public override void SetDefaults()
    {
        Item.width = 26; // Width of the item
        Item.height = 22; // Height of the item
        Item.value = Item.sellPrice(gold: 5); // How many coins the item is worth
        Item.rare = ItemRarityID.Lime; // The rarity of the item
    }

    public override void UpdateEquip(Player player)
    {
        var stats = player.GetStats();
        stats.accessorySlots += 1;
        stats.insourceTimeFlatBonus += 4;
        stats.defenseBonus += 2;
    }

    public override bool IsArmorSet(Item head, Item body, Item legs)
    {
        return body.type == ModContent.ItemType<ScrappyBody>()
            && legs.type == ModContent.ItemType<ScrappyLegs>();
    }

    public override void UpdateArmorSet(Player player)
    {
        player.GetModPlayer<ScrappyPlayer>().hasSetBonus = true;

    }
}


[AutoloadEquip(EquipType.Body)]
public class ScrappyBody : ModItem
{
    public override void SetDefaults()
    {
        Item.width = 34; // Width of the item
        Item.height = 20; // Height of the item
        Item.value = Item.sellPrice(gold: 6); // How many coins the item is worth
        Item.rare = ItemRarityID.Lime; // The rarity of the item

    }

    public override void UpdateEquip(Player player)
    {
        var stats = player.GetStats();
        stats.mainSummonDamage += 0.1f;
        stats.defenseBonus += 6;
        stats.summonDamage += 0.4f;
    }


}
[AutoloadEquip(EquipType.Legs)]
public class ScrappyLegs : ModItem
{
    public override void SetDefaults()
    {
        Item.width = 22; // Width of the item
        Item.height = 12; // Height of the item
        Item.value = Item.sellPrice(gold: 5);
        Item.rare = ItemRarityID.Lime;
    }

    public override void UpdateEquip(Player player)
    {
        var stats = player.GetStats();
        stats.minionSummonHealth += 1;
        stats.defenseBonus += 2;
        stats.accessorySlots += 1;
    }
}
