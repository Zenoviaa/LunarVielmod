using Stellamod.Assets;
using Stellamod.Common.ArmorRework;
using Stellamod.Content.Areas.Illuria.BossesIL.EStyr;
using Stellamod.Core.Bases;
using Stellamod.Core.Pixelation;
using Stellamod.Helpers;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Armors.Harpy;

public class HarpyTornado : ModProjectile
{

    private LittleStarParticleManager _tornadoStreakParticlesBackingField;
    private LittleStarParticleManager TornadoStreakParticles
    {
        get
        {
            _tornadoStreakParticlesBackingField ??= new LittleStarParticleManager(100, 8, GetTrailWidth, GetTrailColor);
            return _tornadoStreakParticlesBackingField;
        }
    }

    private Player Owner => Main.player[Projectile.owner];
    private ref float Timer => ref Projectile.ai[0];
    private ref float DeathTimer => ref Projectile.ai[2];
    public override string Texture => TextureRegistry.EmptyTexture;
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 90;
        Projectile.height = 128;
        Projectile.penetrate = -1;
        Projectile.tileCollide = false;
        Projectile.ignoreWater = true;
        Projectile.timeLeft = 30;
        Projectile.friendly = false;
        Projectile.usesIDStaticNPCImmunity = true;
        Projectile.idStaticNPCHitCooldown = 8;
    }

    public override void AI()
    {
        base.AI();
        Player player = Main.player[Projectile.owner];
        Vector2 rrp = player.RotatedRelativePoint(player.MountedCenter, true);
        Projectile.Center = player.Center;
        if (DeathTimer > 0)
            DeathTimer++;
        // This ensures that the Prism never times out while in use.
        HarpyPlayer harpyPlayer = Owner.GetModPlayer<HarpyPlayer>();
        if (harpyPlayer.tornadoTimer > 0)
            Projectile.timeLeft = 30;
        else
            DeathTimer++;

        Timer++;
       // Main.NewText("E");
        float inTornado = Timer / 30f;
        float outTornado = DeathTimer / 30f;
        if (DeathTimer >= 30f)
            Projectile.Kill();

        float strength = 0.15f;
        foreach (var npc in Main.ActiveNPCs)
        {
            GlobalNPCSucker npcSucker = npc.GetGlobalNPC<GlobalNPCSucker>();
            float dist = Vector2.Distance(Projectile.Center, npc.Center);
            if (!npc.friendly && !npc.boss && dist <= 384)
            {
                float timer = Timer;
                timer += npc.whoAmI * 3;
                float xRadius = MathF.Sin(timer * 0.15f) * 256;
                float yRadius = MathF.Cos(timer * 0.15f) * 24f;
                Vector2 suckPosition = Projectile.Center + new Vector2(xRadius, yRadius);
                suckPosition.Y -= 64;
                suckPosition.Y += ExtraMath.Osc(0f, 32, 0, npc.whoAmI);

                Vector2 diff = suckPosition - npc.Center;
                Vector2 velocity = Vector2.Lerp(Vector2.Zero, diff, strength) * npc.knockBackResist;
                Vector2 diffVelocity = velocity - npcSucker.SuckVelocity;
                npcSucker.SuckVelocity -= diffVelocity.SafeNormalize(Vector2.Zero) * 2;
            }
        }
        if (Timer % 12 == 0)
        {

            SoundStyle jiitasSit = AssetRegistry.Sounds.Jiitas.JiitasLightSpin;
            jiitasSit.PitchVariance = 0.2f;
            jiitasSit.Pitch = 0f;
            jiitasSit.Volume = 0.25f;
            SoundEngine.PlaySound(jiitasSit, Projectile.position);
        }
        inTornado = EasingFunction.InOutSine(inTornado);
        outTornado = MathHelper.Lerp(1f, 0f, EasingFunction.InOutSine(outTornado));
        float alpha = inTornado * outTornado;

        TornadoStreakParticles.xOvalRadius = 64;
        TornadoStreakParticles.yOvalRadius = MathHelper.Lerp(75, 600, EasingFunction.InOutSine(Timer / 30f));
        TornadoStreakParticles.minX = ExtraMath.Osc(25, 45, speed: 3) + MathHelper.Lerp(0f, 25f, EasingFunction.InOutSine(Timer / 150f));
        TornadoStreakParticles.spinTime = 25;
        TornadoStreakParticles.rotationAxis = new Vector3(0, 1, 0.2f);
        TornadoStreakParticles.alpha = 1f * alpha;
        TornadoStreakParticles.topOnly = false;
        TornadoStreakParticles.wideOnly = true;
        TornadoStreakParticles.scale = 0.3f;
        TornadoStreakParticles.Update(Projectile.Center);
    }
    private float GetTrailWidth(float completionRatio)
    {
        return MathHelper.Lerp(0.2f, 0.5f, EasingFunction.QuadraticBump(completionRatio));
    }

    private Color GetTrailColor(float completionRatio)
    {
        Color trailColor = Color.Lerp(Color.Lerp(Color.White, Color.Black, 0.5f), Color.Gray, EasingFunction.QuadraticBump(completionRatio));
        float alpha = EasingFunction.QuadraticBump(completionRatio);
        trailColor *= alpha;
        return trailColor;
    }

    public override bool PreDraw(ref Color lightColor)
    {
        PixelationManager.QueuePrimitivesDrawAction(DrawPixelated, DrawLayer.OverNPCsAdditive);
        return false;
    }

    public void DrawPixelated(GraphicsDevice graphicsDevice)
    {
        TornadoStreakParticles.Draw();
    }
}

public class HarpyPlayer : ModPlayer
{
    public bool hasSetBonus;
    public float tornadoTimer;
    public bool IsFlying()
    {
        return Player.controlJump && !Player.mount.Active && Player.wingTime > 0;
    }
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
        float wtm = Player.wingTimeMax;
        float inc = wtm * 0.45f;
        Player.wingTimeMax = (int)(wtm + inc);
        if (Main.myPlayer != Player.whoAmI)
            return;
      
        if (IsFlying())
        {
            tornadoTimer++;
            if (tornadoTimer > 10)
                tornadoTimer = 10;
        }
        else
        {
            tornadoTimer--;
            if (tornadoTimer <= 0)
                tornadoTimer = 0;
        }
        if (IsFlying() && Player.ownedProjectileCounts[ModContent.ProjectileType<HarpyTornado>()] == 0)
        {
            Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, Vector2.Zero, 
                ModContent.ProjectileType<HarpyTornado>(), 1, 1, Player.whoAmI);
        }
    }
}

[AutoloadEquip(EquipType.Head)]
public class HarpyHead : ModItem
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        ArmorIDs.Head.Sets.DrawHatHair[Item.headSlot] = true;
        ArmorSetSystem.RegisterArmorSet<HarpyHead, HarpyBody, HarpyLegs>();
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
        stats.insourceTimeFlatBonus += 2;
        stats.defenseBonus += 13;
        stats.accessorySlots += 1;
    }

    public override bool IsArmorSet(Item head, Item body, Item legs)
    {
        return body.type == ModContent.ItemType<HarpyBody>() && legs.type == ModContent.ItemType<HarpyLegs>();
    }


    public override void UpdateArmorSet(Player player)
    {
        player.GetModPlayer<HarpyPlayer>().hasSetBonus = true;
    }
}

[AutoloadEquip(EquipType.Body)]
public class HarpyBody : ModItem
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
        stats.defenseBonus += 15;
        stats.stamina += 1;
        stats.accessorySlots++;
    }
}

[AutoloadEquip(EquipType.Legs)]
public class HarpyLegs : ModItem
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
        stats.insourceSlots += 7;
        stats.defenseBonus += 12;
        stats.accessorySlots++;
        stats.movementSpeedBonus += 0.6f;
    }
}
