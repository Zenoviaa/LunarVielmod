using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Buffs.Minions;
using Stellamod.Common.ArmorRework;
using Stellamod.Content.Areas.Illuria.ArmorsIL;
using Stellamod.Core.Effects;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Trailing;
using System;
using System.IO;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.WondrousDarkspace.ArmorWD
{
    public class LittlemothBuff : MinionBuff<Littlemoth>
    {

    }

    public class Littlemoth : ModProjectile
    {
        private ITrailer _trailer;
        private enum AIState
        {
            GoHome,
            Seek
        }
        private ref float Timer => ref Projectile.ai[0];

        private AIState State
        {
            get => (AIState)Projectile.ai[1];
            set => Projectile.ai[1] = (float)value;
        }
        private ref float Cooldown => ref Projectile.ai[2];

        private Vector2 _targetCenter;
        private Vector2 _startCenter;
        private Player Owner => Main.player[Projectile.owner];

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Main.projFrames[Type] = 5;

            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;

            Main.projPet[Projectile.type] = true; // Denotes that this projectile is a pet or minion
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
            ProjectileID.Sets.TrailCacheLength[Type] = 16;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.tileCollide = false; // Makes the minion go through tiles freely

            // These below are needed for a minion weapon
            Projectile.friendly = true; // Only controls if it deals damage to enemies on contact (more on that later)
            Projectile.minion = true; // Declares this as a minion (has many effects)
            Projectile.DamageType = DamageClass.Summon; // Declares the damage type (needed for it to deal damage)
            Projectile.minionSlots = 0f; // Amount of slots this minion occupies from the total minion slots available to the player (more on that later)
            Projectile.penetrate = -1; // Needed so the minion doesn't despawn on collision with enemies or tiles
            Projectile.light = 0.5f;
        }

        public override bool? CanCutTiles()
        {
            return true;
        }

        public override bool MinionContactDamage()
        {
            return true;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            base.SendExtraAI(writer);
            writer.WriteVector2(_targetCenter);
            writer.WriteVector2(_startCenter);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            base.ReceiveExtraAI(reader);
            _targetCenter = reader.ReadVector2();
            _startCenter = reader.ReadVector2();
        }

        public override void AI()
        {
            base.AI();
            DaediaPlayer daediaPlayer = Owner.GetModPlayer<DaediaPlayer>();
            if (!daediaPlayer.hasDaediaSetBonus)
                return;
            if (Main.rand.NextBool(32))
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<GlyphDust>(), newColor: Color.LightPink, Scale: Main.rand.NextFloat(0.2f, 0.4f));
            }

            Projectile.timeLeft = 2;
            Projectile.rotation = Projectile.velocity.X * 0.05f;
            Projectile.direction = Projectile.velocity.X < 0 ? -1 : 1;
            Projectile.spriteDirection = -Projectile.direction;
            switch (State)
            {
                case AIState.GoHome:
                    AI_GoHome();
                    break;
                case AIState.Seek:
                    AI_Seek();
                    break;
            }
            DrawHelper.AnimateTopToBottom(Projectile, 5);
        }

        private void SwitchState(AIState state)
        {
            Timer = 0;
            State = state;
            Projectile.netUpdate = true;
        }

        private Vector2 GetHomePosition()
        {
            float index = SummonHelper.GetProjectileIndex(Projectile);

            float swingRange = MathHelper.TwoPi;
            float swingXRadius = 128;
            float swingYRadius = 96;
            float swingProgress = Main.GlobalTimeWrappedHourly * 0.125f;
            swingProgress += index * MathHelper.TwoPi;
            float xOffset = swingXRadius * MathF.Sin(swingProgress * swingRange + swingRange);
            float yOffset = swingYRadius * MathF.Cos(swingProgress * swingRange + swingRange);
            Vector2 offset = new Vector2(xOffset, yOffset);
            Vector2 targetCenter = Owner.Center + offset + new Vector2(0, -64);
            return targetCenter;
        }
        private void AI_GoHome()
        {

            Cooldown--;
            float index = SummonHelper.GetProjectileIndex(Projectile);
            Vector2 targetCenter = GetHomePosition();
            Projectile.velocity = (targetCenter - Projectile.Center) * 0.1f;

            SummonHelper.SearchForTargets(Owner, Projectile, out bool foundTarget, out float distanceFromTarget, out Vector2 tc);

            float homeTime = 15 * index;
            if (foundTarget && Cooldown <= 0 && distanceFromTarget <= 512)
            {
                Timer++;
                if(Timer >= homeTime)
                {
                    _startCenter = Projectile.Center;
                    _targetCenter = tc;
                    SwitchState(AIState.Seek);
                }
      
            }
        }

        private void AI_Seek()
        {
            Timer++;

            float attackTicks = 52;
            float interpolant = Timer / attackTicks;
            float easing = EasingFunction.QuadraticBump(interpolant);

            Vector2 homeCenter = GetHomePosition();
            Vector2 homeVelocity = (homeCenter - Projectile.Center);
            Vector2 targetVelocity = (_targetCenter - Projectile.Center);
            Vector2 velocity = Vector2.Lerp(homeVelocity, targetVelocity, easing);
            Projectile.velocity = Vector2.Lerp(Projectile.velocity, velocity, 0.2f);
            if (Timer >= attackTicks)
            {
                Cooldown = 30;
                SwitchState(AIState.GoHome);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            _trailer ??= TrailPresets.StarringBalls;
            _trailer.DrawTrail(ref lightColor, Projectile.oldPos);
            _trailer.TrailWidthFunction = (float interpolant) =>
            {
                return MathHelper.Lerp(6, 3, EasingFunction.InOutSine(interpolant));
            };
            this.DrawCentered(ref lightColor);
            return false;
        }
        public override void PostDraw(Color lightColor)
        {
            base.PostDraw(lightColor);
            Texture2D texture2D4 = ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/DimLight").Value;
            Color glowColor = Color.LightPink;
            glowColor.A = 0;
            for (int i = 0; i < 1; i++)
            {
                Main.spriteBatch.Draw(texture2D4, Projectile.Center - Main.screenPosition, null, glowColor, Projectile.rotation, new Vector2(32, 32), 0.17f * (7 + 0.6f) * VectorHelper.Osc(0.75f, 1f, speed: 3), SpriteEffects.None, 0f);
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            float boomSize = Main.rand.NextFloat(0.05f, 0.1f);
            FXUtil.GlowCircleBoom(Projectile.Center,
                innerColor: Color.White,
                glowColor: Color.Pink,
                outerGlowColor: Color.Blue, duration: 15, baseSize: boomSize);
        }
    }

    public class DaediaPlayer : ModPlayer
    {
        public bool hasDaediaSetBonus;
        public override void ResetEffects()
        {
            base.ResetEffects();
            hasDaediaSetBonus = false;
        }
        public override void PostUpdateEquips()
        {
            base.PostUpdateEquips();
            int projectileType = ModContent.ProjectileType<Littlemoth>();
            if (hasDaediaSetBonus && Player.ownedProjectileCounts[projectileType] == 0 && Main.myPlayer == Player.whoAmI)
            {
                for (int i = 0; i < 3; i++)
                {
                    int damage = 10;
                    int knockback = 2;
                    Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, Vector2.Zero,
                        ModContent.ProjectileType<Littlemoth>(), damage, knockback, Main.myPlayer);
                }
            }
            if (hasDaediaSetBonus)
            {
                Player.AddBuff(ModContent.BuffType<LittlemothBuff>(), 2);
            }
        }
    }

    // The AutoloadEquip attribute automatically attaches an equip texture to this item.
    // Providing the EquipType.Head value here will result in TML expecting a X_Head.png file to be placed next to the item's main texture.
    [AutoloadEquip(EquipType.Head)]
    public class DaediaMask : ModItem
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            ArmorSetSystem.RegisterArmorSet<DaediaMask, DaediaBreastplate, DaediaThighs>();
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 18; // Width of the item
            Item.height = 18; // Height of the item
            Item.value = Item.sellPrice(gold: 1); // How many coins the item is worth
            Item.rare = ItemRarityID.LightRed; // The rarity of the item
        }

        public override void UpdateEquip(Player player)
        {
            ArmorStatsPlayer armorStatsPlayer = player.GetModPlayer<ArmorStatsPlayer>();
            armorStatsPlayer.summonCastTime += 0.2f;
            armorStatsPlayer.accessorySlots += 1;
            armorStatsPlayer.defenseBonus += 3;
        }

        // IsArmorSet determines what armor pieces are needed for the setbonus to take effect
        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<DaediaBreastplate>() && legs.type == ModContent.ItemType<DaediaThighs>();
        }

        // UpdateArmorSet allows you to give set bonuses to the armor.
        public override void UpdateArmorSet(Player player)
        {
            player.GetModPlayer<DaediaPlayer>().hasDaediaSetBonus = true;
        }

        // Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
    }

    // The AutoloadEquip attribute automatically attaches an equip texture to this item.
    // Providing the EquipType.Body value here will result in TML expecting X_Arms.png, X_Body.png and X_FemaleBody.png sprite-sheet files to be placed next to the item's main texture.
    [AutoloadEquip(EquipType.Body)]
    public class DaediaBreastplate : ModItem
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
            Item.rare = ItemRarityID.LightRed; // The rarity of the item
        }

        public override void UpdateEquip(Player player)
        {
            ArmorStatsPlayer armorStatsPlayer = player.GetModPlayer<ArmorStatsPlayer>();
            armorStatsPlayer.accessorySlots += 2;
            armorStatsPlayer.defenseBonus += 3;

        }

        // Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.

    }

    // The AutoloadEquip attribute automatically attaches an equip texture to this item.
    // Providing the EquipType.Legs value here will result in TML expecting a X_Legs.png file to be placed next to the item's main texture.
    [AutoloadEquip(EquipType.Legs)]
    public class DaediaThighs : ModItem
    {
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 18; // Width of the item
            Item.height = 18; // Height of the item
            Item.value = Item.sellPrice(gold: 1); // How many coins the item is worth
            Item.rare = ItemRarityID.LightRed; // The rarity of the item
        }
        public override void UpdateEquip(Player player)
        {
            ArmorStatsPlayer armorStatsPlayer = player.GetModPlayer<ArmorStatsPlayer>();
            armorStatsPlayer.minionAggressiveness += 100;
            armorStatsPlayer.defenseBonus += 2;
        }


        // Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.

    }
}