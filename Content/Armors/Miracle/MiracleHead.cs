using ReLogic.Content;
using Stellamod.Assets;
using Stellamod.Common.ArmorRework;
using Stellamod.Core.Particles;
using Stellamod.Helpers;
using Stellamod.Visual.Particles;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI.Chat;

namespace Stellamod.Content.Armors.Miracle;

public class ManaSpider : ModProjectile
{
    private Asset<Texture2D> _outlineTextureAsset;
    private Player Owner => Main.player[Projectile.owner];
    private ref float Timer => ref Projectile.ai[0];
    public override void Unload()
    {
        base.Unload();
        _outlineTextureAsset = null;
    }
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        Main.projFrames[Type] = 4;
        ProjectileID.Sets.TrailCacheLength[Type] = 8;
        ProjectileID.Sets.TrailingMode[Type] = 3;
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 64;
        Projectile.height = 64;
        Projectile.hostile = false;
        Projectile.timeLeft = 360;
        Projectile.tileCollide = false;
        Projectile.ignoreWater = true;
    }

    public override void AI()
    {
        base.AI();
        if (Timer >= 30)
            Projectile.hostile = true;

        Timer++;
        if(Timer == 1)
        {
            SoundStyle deeyCast = AssetRegistry.Sounds.Magic.DeeyaCast1 with { PitchVariance = 0.4f };
            if (Main.rand.NextBool(2))
            {
                deeyCast = AssetRegistry.Sounds.Magic.DeeyaCast2 with { PitchVariance = 0.4f };
            }
            SoundEngine.PlaySound(deeyCast, Projectile.position);
        }

        if(Timer % 16 == 0)
        {
            FaintSmokeParticle fs = FaintSmokeParticle.Spawn(Projectile.Center + Main.rand.NextVector2Circular(64, 64), Vector2.Zero);
            fs.behindLayer = true;
            fs.color = Color.Lerp(Color.Purple, Color.Black, 0.5f);
            fs.fadeToColor = Color.Black;
        }
        Vector2 velocity = (Owner.Center - Projectile.Center);
        velocity = velocity.SafeNormalize(Vector2.Zero);
        float distance = Vector2.Distance(Projectile.Center, Owner.Center);


        float blackMana = Owner.GetModPlayer<MiraclePlayer>().blackMana;
     
        float speed = 1f;
        speed *= MathHelper.Lerp(1f, 8.5f, EasingFunction.Clamp(blackMana / 500f));
        velocity *= speed;
        velocity.Y += MathF.Sin(Timer * 0.1f) * 0.5f;
        Projectile.velocity = velocity;
        Projectile.rotation = Projectile.velocity.X * 0.025f;
        DrawHelper.AnimateTopToBottom(Projectile, 5);
    }

    public override void OnHitPlayer(Player target, Player.HurtInfo info)
    {
        base.OnHitPlayer(target, info);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        _outlineTextureAsset ??= ModContent.Request<Texture2D>(Texture + "_Outline");

        float alpha = EasingFunction.InOutSine(Timer / 30f) * EasingFunction.InOutSine((float)Projectile.timeLeft / 30f);
        SpritebatchDrawer sbDrawer = SpritebatchDrawer.FromProjectile(Projectile);
        sbDrawer.drawOrigin.Y += 50;
        sbDrawer.color *= alpha; 

        for(int i = 0; i < Projectile.oldPos.Length; i++)
        {
            Vector2 pos = Projectile.oldPos[i] + Projectile.Size * 0.5f;
            sbDrawer.color = Color.Lerp(Color.Purple, Color.Transparent, EasingFunction.InOutSine((float)i / (float)Projectile.oldPos.Length));
            sbDrawer.color *= 0.2f * alpha;
            sbDrawer.worldPosition = pos;
            Main.spriteBatch.Draw(sbDrawer);
        }
        sbDrawer = SpritebatchDrawer.FromProjectile(Projectile);
        sbDrawer.drawOrigin.Y += 50;
        sbDrawer.color *= alpha;
        Main.spriteBatch.Draw(sbDrawer);


        SpritebatchDrawer outlineDrawer = SpritebatchDrawer.FromTextureAsset(_outlineTextureAsset, Projectile.Center);
        outlineDrawer.sourceRect = sbDrawer.sourceRect;
        outlineDrawer.drawOrigin = sbDrawer.drawOrigin;
        outlineDrawer.color = Color.Red * alpha;
        Main.spriteBatch.Draw(outlineDrawer);


        outlineDrawer.color = Color.Lerp(Color.Transparent, Color.Purple, ExtraMath.Osc(0f, 0.3f, speed: 6, Projectile.whoAmI)) * alpha;
        Main.spriteBatch.Draw(outlineDrawer);
        return false;
        // return base.PreDraw(ref lightColor);
    }

    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
    }
}
public class MiraclePlayer : ModPlayer
{
    private int _globalTimer;
    private int _blackManaTimer;
    private Asset<Texture2D> _blackStarTextureAsset;
    public bool hasMiracleSet;
    public int blackMana;
    

    public static float TicksPerMonster => 120;
    public static float RegenerationRate => 3;

    public override void ResetEffects()
    {
        hasMiracleSet = false;
    }

    public override void PostUpdateMiscEffects()
    {
        base.PostUpdateMiscEffects();
        if(blackMana > 0)
        {
            _blackManaTimer++;
            if(_blackManaTimer >= TicksPerMonster && blackMana > 20)
            {
                if(Player.whoAmI == Main.myPlayer)
                {
                    
                    Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center + Main.rand.NextVector2CircularEdge(444, 444), Vector2.Zero,
                        ModContent.ProjectileType<ManaSpider>(), 50, 1, Player.whoAmI);
                }
                _blackManaTimer = 0;
            }
            _globalTimer++;
            if(_globalTimer % 4 == 0)
            {
                blackMana--;
            }
      
            if (Main.rand.NextBool(2))
            {
                var p = LegacyParticle.NewParticle<EmberParticle>(Player.position + new Vector2(Main.rand.Next(0, Player.width),
                    Main.rand.Next(0, Player.height)), -Vector2.UnitY.RotatedByRandom(1.5f), Color.Purple, Main.rand.NextFloat(0.9f, 1.5f));
                p.innerColor = Color.LightPink;
                p.outerColor = Color.Purple;
                p.fadeToColor = Color.Black;
            }
            Player.statMana = 0;

        }
     //   Main.NewText(blackMana);
    }
    public override void UpdateDead()
    {
        base.UpdateDead();
        if(blackMana > 0)
        {
            blackMana -= 1000;
            if (blackMana <= 0)
                blackMana = 0;
        }
    }
    public override void ModifyManaCost(Item item, ref float reduce, ref float mult)
    {
        base.ModifyManaCost(item, ref reduce, ref mult);
        if (!hasMiracleSet)
            return;
        if (!Player.CheckMana(item.mana, false, false))
        {
            mult = 0;
            if(blackMana <= 0)
            {
                SoundStyle goBlack = new SoundStyle("Stellamod/Assets/Sounds/OverGrowth_TP2") with { PitchVariance = 0.5f };
                SoundEngine.PlaySound(goBlack, Player.position);
            }
    
        }
    }

    public override void OnConsumeMana(Item item, int manaConsumed)
    {
        base.OnConsumeMana(item, manaConsumed);
        if (!hasMiracleSet)
            return;
        if (!Player.CheckMana(item.mana, false, false))
        {
            blackMana += item.mana;
        }


    }

    public override void DrawEffects(PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
    {
        base.DrawEffects(drawInfo, ref r, ref g, ref b, ref a, ref fullBright);
        _blackStarTextureAsset ??= ModContent.Request<Texture2D>(ModContent.GetInstance<MiracleHead>().Texture + "_BlackStar");
        if (drawInfo.shadow != 0f)
            return;
        if (!hasMiracleSet)
            return;
        if (blackMana <= 0)
            return;

        float numStars = (int)MathF.Ceiling(blackMana / 20f);
        float maxStars = 32;
        for(float f = 0; f < maxStars; f++)
        {
            if (f >= numStars)
                break;

            float ratio = f / maxStars;
            float radians = ratio * MathHelper.TwoPi;
            radians += Main.GlobalTimeWrappedHourly;
            Vector2 offset = radians.ToRotationVector2();
            offset *= 64;
            SpritebatchDrawer starDrawer = SpritebatchDrawer.FromTextureAsset(_blackStarTextureAsset, drawInfo.drawPlayer.Center);
            starDrawer.rotation = ExtraMath.Osc(-0.05f, 0.05f, speed: 1, offset: f);
            starDrawer.worldPosition += offset;

            float substractor = f * 20f;
            float alpha = (blackMana - substractor) / 20f;
            starDrawer.color *= alpha;
            Main.spriteBatch.Draw(starDrawer);
        }
        string waveString = $"-{blackMana}";
        float x = FontAssets.DeathText.Value.MeasureString(waveString).X;
        float y = FontAssets.DeathText.Value.MeasureString(waveString).Y;
        Vector2 drawPosition = drawInfo.drawPlayer.Center - Vector2.UnitY * 64;
        ChatManager.DrawColorCodedString(Main.spriteBatch, FontAssets.DeathText.Value, waveString,
            drawPosition - new Vector2(4, 24) - Main.screenPosition, Color.White, 0, new Vector2(x * 0.5f, y * 0.5f), Vector2.One * 0.5f);
    }
}

[AutoloadEquip(EquipType.Head)]
public class MiracleHead : ModItem
{
    public override void SetStaticDefaults()
    {
        ItemID.Sets.ItemNoGravity[Item.type] = true;
        ArmorIDs.Head.Sets.DrawHatHair[Item.headSlot] = true;
        ArmorSetSystem.RegisterArmorSet<MiracleHead, MiracleBody>(ArmorGroup.Act_II);
    }

    public override void SetDefaults()
    {
        Item.width = 36;
        Item.height = 24;
        Item.value = 10000;
        Item.rare = ItemRarityID.LightPurple;
    }

    public override void UpdateEquip(Player player)
    {
        var stats = player.GetStats();
        stats.defenseBonus += 7;
        stats.artifactManaReduction += 0.5f;
     //   stats.accessorySlots += 2;
    }

    public override void UpdateArmorSet(Player player)
    {
        player.GetModPlayer<MiraclePlayer>().hasMiracleSet = true;
    }

    public override bool IsArmorSet(Item head, Item body, Item legs)
    {
        return body.type == ModContent.ItemType<MiracleBody>() && legs.IsAir;
    }

    public override void ArmorSetShadows(Player player)
    {
        player.armorEffectDrawShadow = true;
    }

    public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
    {
        DrawHelper.DrawGlow2InWorld(Item, spriteBatch, ref rotation, ref scale, whoAmI);
        return true;
    }
}

[AutoloadEquip(EquipType.Body)]
public class MiracleBody : ModItem
{
    public override void SetStaticDefaults()
    {
        // DisplayName.SetDefault("Astrasilk Jacket");
        ItemID.Sets.ItemNoGravity[Item.type] = true;
    }

    public override void SetDefaults()
    {
        Item.width = 40;
        Item.height = 28;
        Item.value = Item.sellPrice(0, 0, 20, 0);
        Item.rare = ItemRarityID.LightPurple;
    }

    public override void UpdateEquip(Player player)
    {
        var stats = player.GetStats();
        stats.defenseBonus += 10;
        stats.magicDamage += 0.18f;
        stats.accessorySlots += 3;
    }

    public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
    {
        DrawHelper.DrawGlow2InWorld(Item, spriteBatch, ref rotation, ref scale, whoAmI);
        return true;
    }
}
