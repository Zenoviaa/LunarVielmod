using ReLogic.Content;
using Stellamod.Assets;
using Stellamod.Common.ArmorRework;
using Stellamod.Common.Shaders;
using Stellamod.Core.Particles;
using Stellamod.Core.Pixelation;
using Stellamod.Helpers;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Armors.Velioza;

/*
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
        Projectile.localNPCHitCooldown = 20;
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
*/


public class VeliozaLifeDrain : ModBuff
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        Main.debuff[Type] = true;
    }

    public override void Update(NPC npc, ref int buffIndex)
    {
        base.Update(npc, ref buffIndex);
        npc.lifeRegen -= 60;
        if (Main.rand.NextBool(3))
        {
            SmokeParticle sp = Particle<SmokeParticle>.Spawn(npc.position + new Vector2(Main.rand.Next(0, npc.width), Main.rand.Next(0, npc.height)), -Vector2.UnitY, Color.DarkRed, Main.rand.NextFloat(0.9f, 1.5f));
            sp.initialColor = Color.Lerp(Color.Red, Color.DarkRed, Main.rand.NextFloat(0f, 1f)) * 0.4f;
            sp.expand = true;
        }
    }
}

public class VeliozaPlayer : ModPlayer
{
    public float timer;
    public float alphaTimer;
    public bool hasSetBonus;
    public float stacks;
    public bool somethingInCircle;
    public float suckDryDistance;
    public override void ResetEffects()
    {
        suckDryDistance = 100;
        somethingInCircle = false;
        hasSetBonus = false;
    }

    public override void PostUpdateEquips()
    {
        base.PostUpdateEquips();
        if (!hasSetBonus)
        {
            alphaTimer-=0.05f;
            if (alphaTimer <= 0)
                alphaTimer = 0;
            stacks = 0;
            return;
        }
        alphaTimer+=0.05f;
        if (alphaTimer >= 1f)
            alphaTimer = 1f;

        stacks = MathHelper.Clamp(stacks, 0, 20);

        foreach(var npc in Main.ActiveNPCs)
        {
            float dist = Vector2.Distance(Player.Center, npc.Center);
            if(dist <= suckDryDistance)
            {
                npc.AddBuff(ModContent.BuffType<VeliozaLifeDrain>(), 10);
                somethingInCircle = true;
            }
        }

        if (somethingInCircle)
        {
            timer++;
            if(timer % 20 == 0)
            {
                Player.Heal(1);
            }
        }
        else if (stacks > 0)
        {
            timer++;
            if(timer % 10 == 0)
            {
                stacks--;
            }
        }
        float scale = MathHelper.Clamp(stacks / 20f, 0f, 1f);
        Player.GetAttackSpeed(DamageClass.Generic) += scale;
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        base.OnHitNPC(target, hit, damageDone);
        if (!hasSetBonus)
            return;

        float dist = Vector2.Distance(Player.Center, target.Center);
        stacks++;
    }
    private void DrawPixelSprites(SpriteBatch spriteBatch, Vector2 screenPos)
    {
        Asset<Texture2D> noise = AssetManager.GlowMask.MagicCircle;
        Vector2 drawOrigin = noise.Size() / 2f;
        Texture2D texture = noise.Value;

        Vector2 drawCenter = Player.Center - Main.screenPosition;
        drawCenter.Y += Player.gfxOffY;

        float ease = EasingFunction.InOutSine(alphaTimer);
        Color drawColor = Color.White;
        drawColor.A = 0;
        Color drawColor2 = Color.Red;
        drawColor2.A = 0;
        //     drawColor *= 0.5f;

        Vector2 scale = Vector2.One;
        scale *= ease;
        scale *= 4;
        var shader = CelestialAuraShader.Instance;
        shader.InnerColor = Color.Red;
        shader.OuterColor = Color.Black;

        float time = MathHelper.Lerp(0.8f, 1f, ExtraMath.Osc(0f, 1f, speed: 3)) - 1;
        shader.Time = time;
        shader.Tiling = Vector2.One * 0.1f;
        spriteBatch.Restart(effect: shader.Effect);
        for (float f = 0; f < 4; f++)
        {
            Color glowColor = Color.Lerp(drawColor, drawColor2, (f + 1) / 3f);
         //   glowColor *= 0.4f;
            glowColor.A = 0;
            float rotOffset = (f / 4f) * MathHelper.TwoPi;
            rotOffset += Main.GlobalTimeWrappedHourly * 0.4f;
            spriteBatch.Draw(texture, drawCenter, null, glowColor, rotOffset + 0.5f, drawOrigin,
                new Vector2(0.8f, 1f) * 0.25f * 0.75f * scale, SpriteEffects.None, 0);
            spriteBatch.Draw(texture, drawCenter, null, glowColor, rotOffset, drawOrigin,
                new Vector2(0.8f, 1f) * 0.25f * scale, SpriteEffects.None, 0);
        }

        spriteBatch.RestartDefaults();
    }
    public override void DrawEffects(PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
    {
        base.DrawEffects(drawInfo, ref r, ref g, ref b, ref a, ref fullBright);
        if (!hasSetBonus)
            return;
        PixelationManager.QueueSpritebatchDrawAction(DrawPixelSprites);
    }
}

[AutoloadEquip(EquipType.Head)]
public class VeliozaHead : ModItem
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        ArmorSetSystem.RegisterArmorSet<VeliozaHead, VeliozaBody, VeliozaLegs>();
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
        stats.healthBonus += 200;
        stats.defenseBonus += 10;
        stats.accessorySlots+=2;
    }

    public override bool IsArmorSet(Item head, Item body, Item legs)
    {
        return body.type == ModContent.ItemType<VeliozaBody>() && legs.type == ModContent.ItemType<VeliozaLegs>();
    }


    public override void UpdateArmorSet(Player player)
    {
        player.GetModPlayer<VeliozaPlayer>().hasSetBonus = true;
    }
}

[AutoloadEquip(EquipType.Body)]
public class VeliozaBody : ModItem
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
        stats.defenseBonus += 18;
        stats.stamina += 1;
        stats.accessorySlots++;
    }
}

[AutoloadEquip(EquipType.Legs)]
public class VeliozaLegs : ModItem
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
        stats.movementSpeedBonus += 0.5f;
        stats.defenseBonus += 14;
        stats.accessorySlots++;
    }
}
