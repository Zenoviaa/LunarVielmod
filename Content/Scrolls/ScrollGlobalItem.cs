using Stellamod.Assets;
using Stellamod.Common.ArmorRework;
using Stellamod.Common.GunSystem;
using Stellamod.Core.Particles;
using Stellamod.Core.Pixelation;
using Stellamod.Core.SwingSystem;
using Stellamod.Core.Tooltips;
using Stellamod.Helpers;
using Stellamod.Items.Accessories.Players;
using Stellamod.Visual.Particles;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Stellamod.Content.Scrolls;


public enum ScrollAbility : byte
{
    _None,
    _ACT_1,
    Enrager,
    Anger,
    Flame,
    Poison,
    Endurance,
    SimpleHome,
    _ACT_2,
    MyScarab,
    _ACT_3,
    _Length,
}

public class Anger : ModBuff
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
    }
    public override void Update(Player player, ref int buffIndex)
    {
        base.Update(player, ref buffIndex);
        player.GetDamage(DamageClass.Generic) += 0.05f * player.GetModPlayer<AngerPlayer>().stacks;
        if (Main.rand.NextBool(14))
        {
            var dp = DustParticle.Spawn(player.Center + Main.rand.NextVector2Circular(24, 24), -(Vector2.UnitY * Main.rand.NextFloat(1f, 5f)).RotatedByRandom(MathHelper.ToRadians(45)));
    //        dp.gravity = 0.05f;
            dp.Scale *= 0.5f;
            dp.dampening = 0.05f;
            dp.innerColor = Color.Red;
        }
    }
}

public class Endurance : ModBuff
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
    }
    public override void Update(Player player, ref int buffIndex)
    {
        base.Update(player, ref buffIndex);
    }
}

public class Enrager : ModBuff
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
    }

    public override void Update(Player player, ref int buffIndex)
    {
        base.Update(player, ref buffIndex);
        player.GetAttackSpeed(DamageClass.Generic) += 0.1f;
    }
}
public class Flame : ModBuff
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
    }
    public override void Update(NPC npc, ref int buffIndex)
    {
        base.Update(npc, ref buffIndex);
    }
}
public class Poison : ModBuff
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
    }
    public override void Update(NPC npc, ref int buffIndex)
    {
        base.Update(npc, ref buffIndex);
    }
}
public class SimpleSpikeball : ModProjectile
{
    private ref float Timer => ref Projectile.ai[0];
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
    }
    public override void AI()
    {
        base.AI();
    }
    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        return base.OnTileCollide(oldVelocity);
    }
    public override bool PreDraw(ref Color lightColor)
    {
        return false;
    }
    public void DrawToRenderTargets()
    {
        //  throw new NotImplementedException();
    }
}
public class SimpleWhiteHomingBolt : ModProjectile,
    IDrawToRenderTarget
{
    public override string Texture => TextureRegistry.EmptyTexture;
    private ref float Timer => ref Projectile.ai[0];
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
    }
    public override void AI()
    {
        base.AI();
    }
    public override bool PreDraw(ref Color lightColor) => false;
    public void DrawToRenderTargets()
    {
      //  throw new NotImplementedException();
    }
}
public class ScrollExpandingTooltip : AbstractExpandingTooltip
{
    public override void ModifyExpandableTooltips(Item item, List<TooltipLine> lines)
    {
        if (item.TryGetGlobalItem<ScrollGlobalItem>(out var scroll) is false)
            return;
        if (scroll.scroll == ScrollAbility._None)
            return;
        if (!ScrollAbilitySystem.scrollsToContentTemplates.ContainsKey(scroll.scroll))
            return;
        //Bruh
      
        TooltipLine line;
        line = new TooltipLine(Mod, "ScrollEnchantment", LangText.Common("ScrollEnchantment"));
        line.OverrideColor = Color.GreenYellow;
        lines.Add(line);

        line = new TooltipLine(Mod, "ScrollStaminaSlash", LangText.Item(ScrollAbilitySystem.scrollsToContentTemplates[scroll.scroll].ModItem, "Tooltip"));
        lines.Add(line);

        line = new TooltipLine(Mod, "ScrollStaminaCost", LangText.Common("StaminaCost",
            ScrollAbilitySystem.GetStaminaCost(scroll.scroll)));
        line.OverrideColor = Color.Goldenrod;
        lines.Add(line);
    }
}
public class ScrollGlobalItem : GlobalItem
{
    public override bool InstancePerEntity => true;
    public ScrollAbility scroll;
    public int StaminaCost => ScrollAbilitySystem.GetStaminaCost(scroll);
    public override bool PreDrawInInventory(Item item, SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
    {
        return base.PreDrawInInventory(item, spriteBatch, position, frame, drawColor, itemColor, origin, scale);
    }
    public override void PostDrawInInventory(Item item, SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
    {
        base.PostDrawInInventory(item, spriteBatch, position, frame, drawColor, itemColor, origin, scale);
        if(ScrollAbilitySystem.IsEnchanting() && ScrollAbilitySystem.IsApplicable(item))
        {
            SpritebatchDrawer glowDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.SimpleGlowCircle, Main.screenPosition + position);
            glowDrawer.color = Color.Green * ExtraMath.Osc(0.5f, 1f, speed: 10) * ScrollAbilitySystem.Alpha; 
            glowDrawer.color.A = 0;
            glowDrawer.scale *= 0.2f;
            spriteBatch.Draw(glowDrawer);
        }
    }
    public override bool AppliesToEntity(Item entity, bool lateInstantiation)
    {
        return base.AppliesToEntity(entity, lateInstantiation);
    }

    public override void RightClick(Item item, Player player)
    {
        base.RightClick(item, player);
        if (!ScrollAbilitySystem.IsEnchanting())
            return;
        if (!ScrollAbilitySystem.IsApplicable(item))
            return;
        ScrollAbilitySystem.ConsumeEnchantment(item);
    }

    public override bool CanRightClick(Item item)
    {
        if (ScrollAbilitySystem.IsEnchanting() && ScrollAbilitySystem.IsApplicable(item))
            return true;
        return base.CanRightClick(item);
    }

    public override bool ConsumeItem(Item item, Player player)
    {
        if (scroll != ScrollAbility._None)
            return false;
        return base.ConsumeItem(item, player);
    }
    public override bool AltFunctionUse(Item item, Player player)
    {
        if (scroll > 0)
        {
            return true;
        }
        return base.AltFunctionUse(item, player);
    }
    public override bool CanShoot(Item item, Player player)
    {
        if (scroll != ScrollAbility._None)
        {
            DashPlayer dashPlayer = player.GetModPlayer<DashPlayer>();
            if (player.altFunctionUse == 2)
            {
                if (dashPlayer.CanConsume(StaminaCost))
                {
                    dashPlayer.Consume(StaminaCost);
                }
                else
                {
                    return false;
                }

            }
        }
        return base.CanShoot(item, player);
    }
    public override bool Shoot(Item item, Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        if (player.altFunctionUse == 2 && scroll != ScrollAbility._None)
        {
            ScrollAbilitySystem.UseAbility(item, player, source, position, velocity, type, damage, knockback, scroll);
            return false;
        }

        return base.Shoot(item, player, source, position, velocity, type, damage, knockback);
    }
    public override void SaveData(Item item, TagCompound tag)
    {
        base.SaveData(item, tag);
        tag["scroll"] = (byte)scroll;
    }

    public override void LoadData(Item item, TagCompound tag)
    {
        base.LoadData(item, tag);
        scroll = (ScrollAbility)tag.Get<byte>("scroll");
    }
}
