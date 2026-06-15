using ReLogic.Content;
using Stellamod.Assets;
using Stellamod.Common.ArmorRework;
using Stellamod.Common.Shaders;
using Stellamod.Core.Bases;
using Stellamod.Core.Particles;
using Stellamod.Core.Pixelation;
using Stellamod.Helpers;
using Stellamod.Visual.Particles;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Armors.Gothin;

public class GothinBlast : ModProjectile
{
    public override string Texture => TextureRegistry.EmptyTexture;
    //Ai
    private ref float Timer => ref Projectile.ai[0];
    private float LifeTime => 45f;
    private float BlowtorchDistance => 1024;

    //Draw Code
    private Vector2[] LinePos;
    public override void SetDefaults()
    {
        Projectile.width = 256;
        Projectile.height = 16;
        Projectile.friendly = true;
        Projectile.penetrate = -1;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = (int)LifeTime / 3;
        Projectile.tileCollide = false;
        Projectile.timeLeft = (int)LifeTime;
        Projectile.hide = true;
        LinePos = new Vector2[5];
    }

    public override void AI()
    {
        Timer++;
        if (Timer == 1)
        {

            for (float f = 0; f < 7; f++)
            {
                Vector2 vel = Projectile.velocity.SafeNormalize(Vector2.Zero);
                vel = vel.RotatedByRandom(MathHelper.ToRadians(60));
                vel *= Main.rand.NextFloat(10f, 20f);

                DustParticleSpawnParams spawnParams = DustParticleSpawnParams.Default;
                spawnParams.innerColor = Color.Yellow;
                spawnParams.outerColor = Color.Red;
                spawnParams.scaleRange *= 1.5f;
                //spawnParams.scaleRange *= 0.5f;
                DustParticle.Spawn(Projectile.Center, vel, spawnParams);
            }

            //Effects

            SoundStyle impact = AssetManager.GetSound("Fire/FireballShoot1");
            impact.PitchVariance = 0.3f;
            SoundEngine.PlaySound(impact, Projectile.position);
            SoundEngine.PlaySound(SoundID.DD2_BetsyFireballShot, Projectile.position);
            SoundEngine.PlaySound(SoundID.DD2_EtherianPortalSpawnEnemy, Projectile.position);
            Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(Projectile.Center, 1024, 16f);
        }


        if (Projectile.scale < 1f || Timer <= 1)
        {
            Projectile.scale = MathF.Sin(Timer / 600f * MathHelper.Pi) * 3f;
            if (Projectile.scale > 1f)
                Projectile.scale = 1f;
        }


        float progress = Timer / LifeTime;
        float easedProgress = Easing.OutExpo(progress);
        List<Vector2> points = new();
        float numPoints = 32;
        for (int i = 0; i <= numPoints; i++)
        {
            points.Add(Vector2.Lerp(Projectile.Center, Projectile.Center + Projectile.velocity.SafeNormalize(Vector2.Zero) * easedProgress
                * BlowtorchDistance, i / numPoints));
        }
        LinePos = points.ToArray();
    }

    public override bool ShouldUpdatePosition()
    {
        //Returning false makes velocity not move the projectile
        return false;
    }


    public float WidthFunction(float completionRatio)
    {
        float inScale = EasingFunction.OutExpo(Timer / 15f);
        float outScale = EasingFunction.InOutSine(Projectile.timeLeft / 15f);
        float w = Projectile.width * 0.5f;
        float width = MathHelper.Lerp(w, w * 0.15f, MathHelper.SmoothStep(0f, 1f, completionRatio));
        return width * outScale * inScale;
    }

    public Color ColorFunction(float completionRatio)
    {
        return Color.Lerp(Color.White, Color.Red, completionRatio);
    }

    public float GetBloomWidth(float completionRatio)
    {
        return WidthFunction(completionRatio) * 1.5f;
    }

    public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
    {
        //This damages everything in the trail
        Vector2[] positions = LinePos;
        float collisionPoint = 0;
        for (int i = 1; i < positions.Length; i++)
        {
            Vector2 position = positions[i];
            Vector2 previousPosition = positions[i - 1];
            if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), position, previousPosition, 6, ref collisionPoint))
                return true;
        }

        //Return false to not use default collision
        return false;
    }

    private void DrawPixelatedBeam(GraphicsDevice gDevice)
    {
        BlackFireShader blackFireShader = BlackFireShader.Instance;
        blackFireShader.SetDefaults();
        blackFireShader.InnerEmitColor = Color.Yellow * 0.5f;
        blackFireShader.OuterEmiteColor = Color.Red;
        TrailDrawer.Draw(Main.spriteBatch, LinePos, ColorFunction, WidthFunction, blackFireShader);

        BloomTrailShader bloomTrailShader = BloomTrailShader.Instance;
        bloomTrailShader.InnerColor = Color.Red;
        bloomTrailShader.OuterColor = Color.DarkRed;
        TrailDrawer.Draw(Main.spriteBatch, LinePos, ColorFunction, GetBloomWidth, bloomTrailShader);
    }
    public void DrawPixelatedMuzzleFlash(SpriteBatch spriteBatch, Vector2 screenPos)
    {
        Asset<Texture2D> muzzleFlashTexture = ModContent.Request<Texture2D>("Stellamod/Assets/LaserTextures/MuzzleFlash");
        Vector2 drawOrigin = muzzleFlashTexture.Size() / 2f;
        Vector2 drawCenter = Projectile.Center - screenPos;
        Color drawColor = Color.Red;
        drawColor.A = 0;

        float width = Projectile.timeLeft / 30f;
        float outWidth = EasingFunction.InOutSine(width);
        float scale = outWidth;
        Vector2 flashScale = Vector2.One;
        flashScale.X *= 1.5f;
        flashScale.Y *= 3f;
        flashScale *= scale;
        spriteBatch.Draw(muzzleFlashTexture.Value, drawCenter, null, drawColor, Projectile.velocity.ToRotation(), drawOrigin, flashScale, SpriteEffects.None, 0);

        drawColor = Color.Yellow;
        drawColor.A = 0;
        spriteBatch.Draw(muzzleFlashTexture.Value, drawCenter, null, drawColor, Projectile.velocity.ToRotation(), drawOrigin, flashScale * 0.6f, SpriteEffects.None, 0);

    }
    public override bool PreDraw(ref Color lightColor)
    {
        PixelationManager.QueueSpritebatchDrawAction(DrawPixelatedMuzzleFlash);
        PixelationManager.QueuePrimitivesDrawAction(DrawPixelatedBeam);
        return false;
    }

    public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
    {
        base.DrawBehind(index, behindNPCsAndTiles, behindNPCs, behindProjectiles, overPlayers, overWiresUI);
        behindNPCs.Add(index);
    }
}
public class GothinGlobalProjectile : GlobalProjectile
{
    public bool gothinEnchanted;
    public override bool InstancePerEntity => true;
    public override void SetDefaults(Projectile entity)
    {
        base.SetDefaults(entity);
        gothinEnchanted = false;
    }
    public override void PostAI(Projectile projectile)
    {
        base.PostAI(projectile);
        if (!gothinEnchanted)
        {
            Player player = Main.player[projectile.owner];
            CrossbowPlayer crossbowPalyer = player.GetModPlayer<CrossbowPlayer>();
            GothinPlayer gothinPlayer = player.GetModPlayer<GothinPlayer>();
            if (gothinPlayer.hasSetBonus && crossbowPalyer.gothinEnchant > 0)
            {
                if (Main.myPlayer == projectile.owner)
                {
                    Projectile.NewProjectile(projectile.GetSource_FromThis(), projectile.Center, projectile.velocity,
                        ModContent.ProjectileType<GothinBlast>(), projectile.damage, projectile.knockBack, projectile.owner);
                }

                for (float f = 0; f < 7; f++)
                {
                    Vector2 vel = projectile.velocity;
                    vel = vel.RotatedByRandom(MathHelper.ToRadians(60));
                    vel *= Main.rand.NextFloat(1f, 2f);

                    DustParticleSpawnParams spawnParams = DustParticleSpawnParams.Default;
                    //spawnParams.scaleRange *= 0.5f;
                    DustParticle.Spawn(projectile.Center, vel, spawnParams);
                }

                crossbowPalyer.gothinEnchant--;
                var p = LegacyParticle.NewParticle<GlowDonutParticle>(projectile.Center, -projectile.velocity * 2, newColor: Color.Red);
                p.Scale *= 0.33f;
                gothinEnchanted = true;
            }
        }


        if (gothinEnchanted)
        {
            if (Main.myPlayer == projectile.owner)
            {
                if (Main.rand.NextBool(8))
                {
                    SparkleParticle sp = SparkleParticle.Spawn(projectile.Center, Vector2.Zero, Scale: 0.5f);
                    sp.gravity = 0;
                    sp.outerColor = Color.Yellow;
                }
            }
        }
    }
}
public class GothinPlayer : ModPlayer
{
    public bool hasSetBonus;
    public override void ResetEffects()
    {
        base.ResetEffects();
        hasSetBonus = false;
    }

    public override void PostUpdateEquips()
    {
        base.PostUpdateEquips();
        if (!hasSetBonus)
            return;
        CrossbowPlayer crossbowPlayer = Player.GetModPlayer<CrossbowPlayer>();
        crossbowPlayer.countShots = true;
        if ((crossbowPlayer.shotCount + 1) % 3 == 0)
        {
            crossbowPlayer.magicCircleTextureAsset = AssetManager.GlowMask.GothinMagicCircle;
            crossbowPlayer.magicCircleColor = Color.Lerp(Color.Yellow, Color.Red, ExtraMath.Osc(0f, 1f, speed: 3));
            crossbowPlayer.magicCircleScale *= 1.5f;
        }
    }
}

[AutoloadEquip(EquipType.Head)]
public class GothinMask : ModItem
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        ArmorSetSystem.RegisterArmorSet<GothinMask, GothinRobe, GothinPants>(ArmorGroup.Act_III);
    }

    public override void SetDefaults()
    {
        Item.width = 40;
        Item.height = 30;
        Item.value = 10000;
        Item.rare = ItemRarityID.Green;
    }

    public override void UpdateEquip(Player player)
    {
        var stats = player.GetStats();
        stats.defenseBonus += 20;
        stats.accessorySlots+=2;
    }

    public override bool IsArmorSet(Item head, Item body, Item legs)
    {
        return body.type == ModContent.ItemType<GothinRobe>() && legs.type == ModContent.ItemType<GothinPants>();
    }

    public override void UpdateArmorSet(Player player)
    {
        player.GetModPlayer<GothinPlayer>().hasSetBonus = true;
    }
}

[AutoloadEquip(EquipType.Body)]
public class GothinRobe : ModItem
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
    }

    public override void SetDefaults()
    {
        Item.width = 18; // Width of the item
        Item.height = 18; // Height of the item
        Item.value = Item.sellPrice(gold: 1); // How many coins the item is worth
        Item.rare = ItemRarityID.Green; // The rarity of the item
    }

    public override void UpdateEquip(Player player)
    {
        var stats = player.GetStats();
        stats.rangedDamage += 0.56f;
        stats.accessorySlots++;
        stats.stamina += 2;
        stats.defenseBonus += 23;
    }
}
[AutoloadEquip(EquipType.Legs)]
public class GothinPants : ModItem
{
    public override void SetDefaults()
    {
        Item.width = 28;
        Item.height = 22;
        Item.value = 10000;
        Item.rare = ItemRarityID.Green;
    }

    public override void UpdateEquip(Player player)
    {
        var stats = player.GetStats();
        stats.rangedBowChargeTime += 0.5f;
        stats.defenseBonus += 16;
        stats.accessorySlots++;
    }
}