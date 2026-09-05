using ReLogic.Content;
using Stellamod.Assets;
using Stellamod.Common.ArmorRework;
using Stellamod.Common.Shaders;
using Stellamod.Core.Pixelation;
using Stellamod.Core.SwingSystem;
using Stellamod.Helpers;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Armors.Radianthal;

public class RadianthalAura : ModProjectile
{
    public override string Texture => TextureRegistry.EmptyTexture;
    private ref float Timer => ref Projectile.ai[0];
    private Player Owner => Main.player[Projectile.owner];
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 128;
        Projectile.height = 128;
        Projectile.friendly = true;
        Projectile.tileCollide = false;
        Projectile.ignoreWater = true;
        Projectile.penetrate = -1;
        Projectile.timeLeft = 60;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = -1;
    }

    public override void AI()
    {
        base.AI();
        Timer++;
        if (Timer == 1)
        {
            var glowStretch = FXUtil.GlowStretch(Projectile.Center, new Vector2(-1, 1));
            glowStretch.VectorScale.X *= 4;
            glowStretch.OuterGlowColor = Color.Goldenrod;
            SoundStyle spawnSound = new SoundStyle("Stellamod/Assets/Sounds/Parendine2");
            spawnSound.PitchVariance = 0.3f;
            spawnSound.Volume = 0.5f;
            spawnSound.Pitch = -0.3f;
            SoundEngine.PlaySound(spawnSound, Projectile.position);
        }

        if (Timer % 12 == 0)
        {
            SparkleParticle sp = SparkleParticle.Spawn(Projectile.Center + Main.rand.NextVector2Circular(64, 64), Vector2.Zero, Color.White, Scale: 0.5f);
            sp.fast = true;
            sp.gravity = 0;
        }

        if (Main.rand.NextBool(32))
        {
            Vector2 initialVelocity = -Vector2.UnitY * 4;
            DustParticleSpawnParams spawnParams = new DustParticleSpawnParams
            {
                innerColor = Color.LightGoldenrodYellow,
                outerColor = Color.DarkGoldenrod
            };

            Vector2 pos = Owner.position + new Vector2(Main.rand.Next(0, Owner.width), Main.rand.Next(0, Owner.height));
            DustParticle dp = DustParticle.Spawn(pos, initialVelocity, spawnParams);
            dp.gravity = 0f;
            dp.dampening = 0.05f;
        }
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        base.OnHitNPC(target, hit, damageDone);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        PixelationManager.QueueSpritebatchDrawAction(DrawPixelSprites);
        return false;
    }

    private void DrawPixelSprites(SpriteBatch spriteBatch, Vector2 screenPos)
    {
        Asset<Texture2D> noise = AssetManager.GlowMask.MagicCircleVampiricVine;
        Vector2 drawOrigin = noise.Size() / 2f;
        Texture2D texture = noise.Value;

        Vector2 drawCenter = Projectile.Center - Main.screenPosition;
        drawCenter.Y += Owner.gfxOffY;

        float ease = EasingFunction.InOutSine((float)Projectile.timeLeft / 60f) * EasingFunction.InOutSine(Timer / 10f);
        Color drawColor = Color.White;
        drawColor.A = 0;
        Color drawColor2 = Color.Orange;
        drawColor2.A = 0;
        //     drawColor *= 0.5f;

        Vector2 scale = Vector2.One;
        scale *= ease;
        scale *= 4;
        var shader = CelestialAuraShader.Instance;
        shader.InnerColor = Color.DarkGoldenrod;
        shader.OuterColor = Color.Black;
        shader.Time = -Timer * 0.05f + 1;
        shader.Tiling = Vector2.One * 0.1f;
        spriteBatch.Restart(effect: shader.Effect);
        for (float f = 0; f < 3; f++)
        {
            Color glowColor = Color.Lerp(drawColor, drawColor2, (f + 1) / 3f);
            glowColor.A = 0;
            float rotOffset = (f / 4f) * MathHelper.TwoPi;
            spriteBatch.Draw(texture, drawCenter, null, glowColor, rotOffset + 0.5f, drawOrigin,
                new Vector2(0.8f, 1f) * 0.25f * 0.75f * scale, SpriteEffects.None, 0);
            spriteBatch.Draw(texture, drawCenter, null, glowColor, rotOffset, drawOrigin,
                new Vector2(0.8f, 1f) * 0.25f * scale, SpriteEffects.None, 0);
        }

        spriteBatch.RestartDefaults();
    }
    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
    }
}


public class RadianthalGlobalProjectile : GlobalProjectile
{
    public bool hasHitSomething;
    public override bool InstancePerEntity => true;
    public override void SetDefaults(Projectile entity)
    {
        base.SetDefaults(entity);
        hasHitSomething = false;
    }

    public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
    {
        base.OnHitNPC(projectile, target, hit, damageDone);
        RadianthalPlayer player = Main.player[projectile.owner].GetModPlayer<RadianthalPlayer>();
 
        if (!hasHitSomething)
        {

            if(projectile.ModProjectile is BaseSwingProjectileV2)
            {
                if (player.hasSetBonus)
                {
                    player.stacks++;
                }
            }
        

            if (player.stacks >= 3)
            {
                Projectile.NewProjectile(projectile.GetSource_FromThis(), target.Center, Vector2.Zero, ModContent.ProjectileType<RadianthalAura>(),
                    (int)(projectile.damage * 0.5f),
                    projectile.knockBack,
                    projectile.owner);
            }

            hasHitSomething = true;
        }

    }
    public override void OnKill(Projectile projectile, int timeLeft)
    {
        base.OnKill(projectile, timeLeft);
        if (!hasHitSomething && projectile.friendly && projectile.owner == Main.myPlayer && projectile.DamageType == DamageClass.Melee)
        {
            RadianthalPlayer player = Main.player[projectile.owner].GetModPlayer<RadianthalPlayer>();
            player.stacks = 0;
           // Main.NewText("nuh uh");
        }
    }
}

public class RadianthalPlayer : ModPlayer
{
    public bool hasSetBonus;
    public float stacks;

    public override void ResetEffects()
    {
        hasSetBonus = false;
    }

    public override void PostUpdateEquips()
    {
        base.PostUpdateEquips();
        if (!hasSetBonus)
        {
            stacks = 0;
            return;
        }
        float maxAttackSpeedPenalty = -0.5f;
        float maxDamageBonus = 2;
        float lerp = stacks / 10f;
        lerp = MathHelper.Clamp(lerp, 0f, 1f);
        float attackSpeedPenalty = MathHelper.Lerp(0, maxAttackSpeedPenalty, lerp);
        float damageBonus = MathHelper.Lerp(0, maxDamageBonus, lerp);
        Player.GetAttackSpeed(DamageClass.Melee) += attackSpeedPenalty;
        Player.GetDamage(DamageClass.Melee) += damageBonus;
    }
}

[AutoloadEquip(EquipType.Head)]
public class RadianthalHead : ModItem
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        ArmorSetSystem.RegisterArmorSet<RadianthalHead, RadianthalBody, RadianthalLegs>(ArmorGroup.Act_II);
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
        stats.meleeAttackSpeed -= 0.05f;
        stats.meleeAggressiveness += 200;
        stats.defenseBonus += 13;
        stats.accessorySlots++;
    }

    public override bool IsArmorSet(Item head, Item body, Item legs)
    {
        return body.type == ModContent.ItemType<RadianthalBody>() && legs.type == ModContent.ItemType<RadianthalLegs>();
    }


    public override void UpdateArmorSet(Player player)
    {
        player.GetModPlayer<RadianthalPlayer>().hasSetBonus = true;
    }
}

[AutoloadEquip(EquipType.Body)]
public class RadianthalBody : ModItem
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
        stats.defenseBonus += 22;
        stats.stamina += 1;
        stats.meleeDamage += 0.5f;
        stats.accessorySlots++;
    }
}

[AutoloadEquip(EquipType.Legs)]
public class RadianthalLegs : ModItem
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
        stats.meleeArmorPenetration += 10;
        stats.defenseBonus += 12;
        stats.accessorySlots++;
    }
}
