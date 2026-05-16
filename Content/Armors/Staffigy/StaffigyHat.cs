using Stellamod.Common.ArmorRework;
using Stellamod.Content.Items.MoonlightMagic;
using Stellamod.Helpers;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Armors.Staffigy
{

    public class StaffigyCrescent : ModProjectile
    {
        private ref float Timer => ref Projectile.ai[0];
        private Player Owner => Main.player[Projectile.owner];
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.light = 0.2f;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.hide = true;
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            base.DrawBehind(index, behindNPCsAndTiles, behindNPCs, behindProjectiles, overPlayers, overWiresUI);
            behindNPCs.Add(index);
        }
        public override void AI()
        {
            base.AI();
            StaffigyPlayer staffigyPlayer = Owner.GetModPlayer<StaffigyPlayer>();
            if (staffigyPlayer.hasStaffigySetBonus)
            {
                Projectile.timeLeft = 2;
            }

            Timer++;
            float osc = MathF.Sin(Timer) + 1f;
            Projectile.Center = Owner.Center + Vector2.Lerp(Vector2.Zero, -Vector2.UnitY * 32, ExtraMath.OscTimer(Timer * 0.03f, 0f, 1f));
            Projectile.Center -= Vector2.UnitY * 24;
            Projectile.Center += Vector2.UnitX * 12 * -Owner.direction;
            Projectile.spriteDirection = Owner.direction;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            this.DrawCentered(ref lightColor);
            return false;
        }
        public override void PostDraw(Color lightColor)
        {
            base.PostDraw(lightColor);
            Texture2D texture2D4 = ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/DimLight").Value;
            Color glowColor = Color.White;
            glowColor.A = 0;
            for (int i = 0; i < 1; i++)
            {
                Main.spriteBatch.Draw(texture2D4, Projectile.Center - Main.screenPosition, null, glowColor, Projectile.rotation, new Vector2(32, 32), 0.17f * (7 + 0.6f) * VectorHelper.Osc(0.75f, 1f, speed: 3), SpriteEffects.None, 0f);
            }
        }
    }

    public class StaffigyPlayer : ModPlayer
    {
        public bool hasStaffigySetBonus;
        public override void ResetEffects()
        {
            base.ResetEffects();
            hasStaffigySetBonus = false;
        }
        public override void PostUpdateEquips()
        {
            base.PostUpdateEquips();
            int projType = ModContent.ProjectileType<StaffigyCrescent>();
            if (hasStaffigySetBonus && Player.ownedProjectileCounts[projType] == 0 && Main.myPlayer == Player.whoAmI)
            {
                Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, Vector2.Zero, projType, 1, 1, Main.myPlayer);
            }
        }
    }

    [AutoloadEquip(EquipType.Head)]
    public class StaffigyHat : ModItem
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
            ArmorIDs.Head.Sets.DrawHatHair[Item.headSlot] = true;
            ArmorSetSystem.RegisterArmorSet<StaffigyHat, StaffigyRobe, StaffigyPants>(ArmorGroup.Act_I);
        }

        public override void UpdateEquip(Player player)
        {
            var stats = player.GetModPlayer<ArmorStatsPlayer>();
            stats.wandTimerEnchantmentSlots += 2;
            stats.defenseBonus += 4;
            stats.accessorySlots += 1;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<StaffigyRobe>()
                && legs.type == ModContent.ItemType<StaffigyPants>();
        }

        public override void ArmorSetShadows(Player player)
        {
            player.armorEffectDrawShadow = true;
        }

        public override void UpdateArmorSet(Player player)
        {
            player.GetModPlayer<AdvancedMagicPlayer>().chargeTimeBonus += 0.25f;
            player.GetModPlayer<StaffigyPlayer>().hasStaffigySetBonus = true;
        }
    }

    [AutoloadEquip(EquipType.Body)]
    public class StaffigyRobe : ModItem
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void UpdateEquip(Player player)
        {
            var stats = player.GetModPlayer<ArmorStatsPlayer>();
            stats.magicDamage += 0.07f;
            stats.defenseBonus += 5;
            stats.accessorySlots += 1;
        }
    }

    [AutoloadEquip(EquipType.Legs)]
    public class StaffigyPants : ModItem
    {
        public override void UpdateEquip(Player player)
        {
            var stats = player.GetModPlayer<ArmorStatsPlayer>();
            stats.defenseBonus += 2;
            stats.totalMana += 40;
        }
    }
}
