using Stellamod.Assets;
using Stellamod.Common.ArmorRework;
using Stellamod.Common.Shaders;
using Stellamod.Core.Particles;
using Stellamod.Core.Pixelation;
using Stellamod.Helpers;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ModLoader;

namespace Stellamod.Content.Armors.Huntrian
{
    public class HuntrianVenomExtraDamage : GlobalNPC
    {
        public override void ModifyIncomingHit(NPC npc, ref NPC.HitModifiers modifiers)
        {
            base.ModifyIncomingHit(npc, ref modifiers);
            if (npc.HasBuff<HuntrianVenom>())
            {
                modifiers.FlatBonusDamage += 5;
            }
        }
    }
    public class HuntrianVenom : ModBuff
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoTimeDisplay[Type] = false;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            base.Update(npc, ref buffIndex);
            npc.lifeRegen -= 25;
            if (Main.rand.NextBool(6))
            {
                SmokeParticle sp = Particle<SmokeParticle>.Spawn(npc.position + new Vector2(Main.rand.Next(0, npc.width), Main.rand.Next(0, npc.height)), -Vector2.UnitY, Color.DarkGreen, Main.rand.NextFloat(0.9f, 1.5f));
                sp.initialColor = Color.Lerp(Color.DarkGreen, Color.DarkSeaGreen, Main.rand.NextFloat(0f, 1f)) * 0.4f;
                sp.expand = true;
            }
            if (Main.rand.NextBool(6))
            {
                LegacyParticle.NewParticle<EmberParticle>(npc.position + new Vector2(Main.rand.Next(0, npc.width), Main.rand.Next(0, npc.height)), -Vector2.UnitY.RotatedByRandom(1.5f), Color.DarkGreen, Main.rand.NextFloat(0.9f, 1.5f));
            }
        }
    }
    public class HuntrianAura : ModProjectile
    {
        public override string Texture => TextureRegistry.EmptyTexture;
        private Player Owner => Main.player[Projectile.owner];
        private ref float Timer => ref Projectile.ai[0];
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 196;
            Projectile.height = 196;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 30;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 60;
        }

        public override void AI()
        {
            base.AI();
            Timer++;
            HuntrianPlayer huntrianPlayer = Owner.GetModPlayer<HuntrianPlayer>();
            if (huntrianPlayer.hasHuntrianSetBonus)
                Projectile.timeLeft = 60;
            if (Main.rand.NextBool(32))
            {
                Vector2 pos = Projectile.position;
                pos.X += Main.rand.Next(0, Projectile.width);
                pos.Y += Main.rand.Next(0, Projectile.height);
                SmokeParticle sp = SmokeParticle.Spawn(pos, -Vector2.UnitY, Scale: 0.5f);
                sp.initialColor = Color.DarkGreen * 0.85f;
            }
            Projectile.Center = Owner.Center;
            Projectile.rotation += 0.005f;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            target.AddBuff(ModContent.BuffType<HuntrianVenom>(), 1800);
        }

        private void DrawPixelSprites(SpriteBatch spriteBatch, Vector2 screenPos)
        {
            Texture2D texture = AssetManager.GlowMask.SpiralVortex.Value;
            BubbleShader bubbleShader = BubbleShader.Instance;
            bubbleShader.InnerColor = Color.LightGreen * 0.5f;
            bubbleShader.OuterColor = Color.DarkSeaGreen * 0.5f;
            bubbleShader.Distortion = 0.05f;
            bubbleShader.Time = Main.GlobalTimeWrappedHourly * -1;
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            spriteBatch.Restart(blendState: BlendState.Additive, effect: bubbleShader.Effect);
            spriteBatch.Draw(texture, drawPos, null, Color.White * EasingFunction.InOutSine(Projectile.timeLeft / 60f) * EasingFunction.InOutSine(Timer / 60f), Projectile.rotation, texture.Size() / 2f, Projectile.scale * 0.66f, SpriteEffects.None, 0);

            spriteBatch.RestartDefaults();


            Texture2D texture2 = AssetManager.GlowMask.SimpleGlowCircle.Value;
            Color sickColor = Color.SeaGreen * EasingFunction.InOutSine(Projectile.timeLeft / 60f) * EasingFunction.InOutSine(Timer / 60f);
            sickColor.A = 0;
            sickColor *= 0.5f;
            spriteBatch.Draw(texture2, drawPos, null, sickColor, Projectile.rotation, texture2.Size() / 2f, Projectile.scale * 0.4f, SpriteEffects.None, 0);

        }
        public override bool PreDraw(ref Color lightColor)
        {
            PixelationManager.QueueSpritebatchDrawAction(DrawPixelSprites);
            return false;
        }
    }

    public class HuntrianPlayer : ModPlayer
    {
        public bool hasHuntrianSetBonus;
        public override void ResetEffects()
        {
            base.ResetEffects();
            hasHuntrianSetBonus = false;
        }
        public override void PostUpdateMiscEffects()
        {
            base.PostUpdateMiscEffects();
            if (Main.myPlayer != Player.whoAmI)
                return;
            if (!hasHuntrianSetBonus)
                return;
            int type = ModContent.ProjectileType<HuntrianAura>();
            if (Player.ownedProjectileCounts[type] > 0)
                return;
            int damage = 2;
            float knockback = 1;
            Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, Vector2.Zero, type, damage, knockback, Player.whoAmI);
        }
    }

    [AutoloadEquip(EquipType.Head)]
    public class HuntrianHelmet : ModItem
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
            ArmorSetSystem.RegisterArmorSet<HuntrianHelmet, HuntrianChestplate, HuntrianBoots>();
        }

        public override void UpdateEquip(Player player)
        {
            ArmorStatsPlayer stats = player.GetModPlayer<ArmorStatsPlayer>();
            stats.defenseBonus += 1;
            stats.accessorySlots++;
            stats.stamina += 1;
            stats.insourceTimeFlatBonus = 3;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<HuntrianChestplate>() && legs.type == ModContent.ItemType<HuntrianBoots>();
        }

        // UpdateArmorSet allows you to give set bonuses to the armor.
        public override void UpdateArmorSet(Player player)
        {
            player.GetModPlayer<HuntrianPlayer>().hasHuntrianSetBonus = true;

        }
    } // The AutoloadEquip attribute automatically attaches an equip texture to this item.
    // Providing the EquipType.Body value here will result in TML expecting X_Arms.png, X_Body.png and X_FemaleBody.png sprite-sheet files to be placed next to the item's main texture.
    [AutoloadEquip(EquipType.Body)]
    public class HuntrianChestplate : ModItem
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void UpdateEquip(Player player)
        {
            ArmorStatsPlayer stats = player.GetModPlayer<ArmorStatsPlayer>();
            stats.defenseBonus += 3;
            stats.generalEndurance += 0.09f;
            stats.accessorySlots += 3;
            stats.stamina += 1;
        }

    }

    [AutoloadEquip(EquipType.Legs)]
    public class HuntrianBoots : ModItem
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void UpdateEquip(Player player)
        {
            ArmorStatsPlayer stats = player.GetModPlayer<ArmorStatsPlayer>();
            stats.defenseBonus += 2;
            stats.accessorySlots += 1;
            stats.insourceSlots += 3;
        }
    }
}