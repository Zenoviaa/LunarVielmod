using ReLogic.Content;
using Stellamod.Assets;
using Stellamod.Common.ArmorRework;
using Stellamod.Core.Particles;
using Stellamod.Core.Utilities;
using Stellamod.Helpers;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Armors.Candlelight;

public class CandlelightPlayer : ModPlayer
{
    private Asset<Texture2D> _goldenTextureAsset;
    public bool hasSetBonus;
    public float takenHits;
    public float candleTimer;
    public override void Unload()
    {
        base.Unload();
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
        if (takenHits >= 5)
            return;

        candleTimer++;

        Vector2 drawCenter = Player.Center;
        drawCenter.Y -= 80;
        drawCenter.Y += ExtraMath.Osc(0f, 16, speed: 2);
        drawCenter += Main.rand.NextVector2Circular(16, 16);

        if (candleTimer % 24 == 0)
        {
            DustParticleSpawnParams spawnParams = DustParticleSpawnParams.Default;
            spawnParams.outerColor = Color.DarkGray;
            spawnParams.scaleRange *= 0.4f;


            DustParticle.Spawn(drawCenter, Vector2.Zero, spawnParams);


        }
        float strength = takenHits / 5f;
        strength = MathHelper.Clamp(strength, 0f, 1f);
        drawCenter.Y -= 24 * MathHelper.Lerp(1f, 0f, strength); ;

    
        if (Main.rand.NextBool(3))
        {
            SmokeParticle sp = Particle<SmokeParticle>.Spawn(drawCenter, -Vector2.UnitY, Color.OrangeRed, Main.rand.NextFloat(0.9f, 1.5f));
            sp.initialColor = Color.Lerp(Color.OrangeRed, Color.RosyBrown, Main.rand.NextFloat(0f, 1f)) * 0.4f;
            sp.expand = true;
            sp.parent = Player;
            sp.Scale *= MathHelper.Lerp(1f, 0f, strength);
        }
        if (Main.rand.NextBool(3))
        {
            var sp = LegacyParticle.NewParticle<EmberParticle>(drawCenter, -Vector2.UnitY.RotatedByRandom(1.5f), Color.OrangeRed, Main.rand.NextFloat(0.9f, 1.5f));
            sp.Scale *= MathHelper.Lerp(1f, 0f, strength);
        }
    }

    public override void UpdateDead()
    {
        base.UpdateDead();
        takenHits = 0;
    }
    
    public override void ModifyHurt(ref Player.HurtModifiers modifiers)
    {
        base.ModifyHurt(ref modifiers);
        if (!hasSetBonus)
            return;
        if (takenHits >= 5)
        {
            modifiers.FinalDamage *= 1.25f;
            return;
        }
        modifiers.FinalDamage *= 0;
    }

    public override void OnHurt(Player.HurtInfo info)
    {
        base.OnHurt(info);
        if (!hasSetBonus)
            return;
        if(takenHits < 5)
        {
            SoundStyle sound = AssetManager.GetSound("Fire/Waxing");
            sound.PitchVariance = 0.3f;
            sound.Volume = 0.75f;
            SoundEngine.PlaySound(sound, Player.position);

            float numPoints = 4;
            for(float n = 0; n < numPoints; n++)
            {
                DustParticleSpawnParams spawnParams = DustParticleSpawnParams.Default;
                spawnParams.outerColor = Color.DarkGray;
                spawnParams.scaleRange *= 0.4f;


                Vector2 vel = (n / numPoints * MathHelper.TwoPi).ToRotationVector2();
                vel *= 3;
                DustParticle.Spawn(Player.Center, vel, spawnParams);
            }
            takenHits++;
            if(takenHits >= 5)
            {
                //oh no

            }
        }

    }

    public override void DrawEffects(PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
    {
        base.DrawEffects(drawInfo, ref r, ref g, ref b, ref a, ref fullBright);
        if (!hasSetBonus)
        {
            return;
        }
        if (drawInfo.shadow != 0f)
            return;
        if (drawInfo.drawPlayer.dead)
            return;
        _goldenTextureAsset ??= ModContent.Request<Texture2D>(this.GetTypeDirectoryWithSlash() + "GoldenCandle");
        Vector2 drawCenter = drawInfo.Center;
        drawCenter.Y -= 80;
        drawCenter.Y += ExtraMath.Osc(0f, 16, speed: 2);
        SpritebatchDrawer drawer = SpritebatchDrawer.FromTextureAsset(_goldenTextureAsset, drawCenter);
        float scale = MathHelper.Lerp(1f, 0f, MathHelper.Clamp(takenHits / 5f, 0f, 1f));

        drawer.scale = Vector2.One * scale;
        Main.spriteBatch.Draw(drawer);
    }
}

[AutoloadEquip(EquipType.Head)]
public class CandlelightHood : ModItem
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        ArmorSetSystem.RegisterArmorSet<CandlelightHood, CandlelightBody, CandlelightLegs>();
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
        stats.defenseBonus += 30;
        stats.accessorySlots++;
    }

    public override bool IsArmorSet(Item head, Item body, Item legs)
    {
        return body.type == ModContent.ItemType<CandlelightBody>() && legs.type == ModContent.ItemType<CandlelightLegs>();
    }


    public override void UpdateArmorSet(Player player)
    {
        player.GetModPlayer<CandlelightPlayer>().hasSetBonus = true;
    }
}

[AutoloadEquip(EquipType.Body)]
public class CandlelightBody : ModItem
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
        stats.defenseBonus += 37;
        stats.enemyEndurance += 0.75f;
        stats.accessorySlots++;
    }
}

[AutoloadEquip(EquipType.Legs)]
public class CandlelightLegs : ModItem
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
        stats.inventorySlots += 40;
        stats.defenseBonus += 33;
        stats.accessorySlots++;
    }
}
