using ReLogic.Content;
using Stellamod.Common.ArmorRework;
using Stellamod.Common.XixianFlaskSystem;
using Stellamod.Content.Areas.SpringHills.AccSH;
using Stellamod.Content.Special.DeadRomancesExcalibur;
using Stellamod.Core.Particles;
using Stellamod.Core.Utilities;
using Stellamod.Helpers;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Armors.Elegant
{
    public class Elegant : ModBuff
    {
        public override void Update(Player player, ref int buffIndex)
        {
            base.Update(player, ref buffIndex);
            if (Main.rand.NextBool(3))
            {
                SmokeParticle sp = Particle<SmokeParticle>.Spawn(player.position + new Vector2(Main.rand.Next(0, player.width), Main.rand.Next(0, player.height)), -Vector2.UnitY, Color.OrangeRed, Main.rand.NextFloat(0.9f, 1.5f));
                sp.initialColor = Color.Lerp(Color.OrangeRed, Color.RosyBrown, Main.rand.NextFloat(0f, 1f)) * 0.4f;
                sp.expand = true;
            }
            if (Main.rand.NextBool(3))
            {
                LegacyParticle.NewParticle<EmberParticle>(player.position + new Vector2(Main.rand.Next(0, player.width), Main.rand.Next(0, player.height)), -Vector2.UnitY.RotatedByRandom(1.5f), Color.OrangeRed, Main.rand.NextFloat(0.9f, 1.5f));
            }
        }
    }
    public class ElegantPlayer : ModPlayer
    {
        private Asset<Texture2D> _featherTextureAsset;
        public bool hasSetBonus;
        public float alphaTimer;
        public override void ResetEffects()
        {
            hasSetBonus = false;
        }
        public override void Load()
        {
            base.Load();
            FlaskPlayer.OnProc += ApplyImmunity;
        }

        public override void Unload()
        {
            base.Unload();
            FlaskPlayer.OnProc -= ApplyImmunity;
            _featherTextureAsset = null;
        }

        public override void PostUpdateEquips()
        {
            base.PostUpdateEquips();
            if (Player.HasBuff<Elegant>())
            {
                alphaTimer += 0.05f;
            }
            else
            {
                alphaTimer-=0.05f;
            }
            alphaTimer = MathHelper.Clamp(alphaTimer, 0, 1f);
        }
        private void ApplyImmunity(Player player)
        {
            ElegantPlayer elegant = player.GetModPlayer<ElegantPlayer>();
            if (!elegant.hasSetBonus)
                return;
            player.SetImmuneTimeForAllTypes(180);
            player.AddBuff(ModContent.BuffType<Elegant>(), 180);
            SoundStyle drinkSound = new SoundStyle("Stellamod/Assets/Sounds/HolyCast1");
            SoundEngine.PlaySound(drinkSound, player.position);
        }

        public override void DrawEffects(PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
        {
            base.DrawEffects(drawInfo, ref r, ref g, ref b, ref a, ref fullBright);
            if (drawInfo.shadow != 0f)
                return;

            int maxNumBlades = 6;
            SpriteBatch sb = Main.spriteBatch;
            if (alphaTimer <= 0)
            {
                return;
            }
            _featherTextureAsset ??= ModContent.Request<Texture2D>(this.GetTypeDirectoryWithSlash() + "ElegantFeather");
            for (int i = 0; i < maxNumBlades; i++)
            {
                float ratio = (float)i / (float)maxNumBlades;
                float radians = ratio * MathHelper.TwoPi;
                radians += Main.GlobalTimeWrappedHourly * 0.5f;
                Vector2 drawCenter = radians.ToRotationVector2() * 48 + drawInfo.drawPlayer.Center;

                Texture2D texture = _featherTextureAsset.Value;
                SpritebatchDrawer swordDrawer = SpritebatchDrawer.FromTextureAsset(texture, drawCenter);
              //  float rads = MathHelper.ToRadians(3);
              
                swordDrawer.rotation = (drawCenter - drawInfo.drawPlayer.Center).ToRotation() + MathHelper.PiOver2;
                swordDrawer.color = Color.White * alphaTimer;
                sb.Draw(swordDrawer);
            }
        }
    }

    [AutoloadEquip(EquipType.Head)]
    public class ElagentHead : ModItem
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
            ArmorSetSystem.RegisterArmorSet<ElagentHead, ElagentBody, ElagentLegs>(ArmorGroup.Act_I);
        }

        public override void SetDefaults()
        {
            Item.width = 40;
            Item.height = 30;
            Item.value = 10000;
            Item.rare = ItemRarityID.Orange;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<ElagentBody>()
                && legs.type == ModContent.ItemType<ElagentLegs>();
        }

        public override void UpdateEquip(Player player)
        {
            var stats = player.GetStats();
            stats.insourceTimeFlatBonus += 4;
            stats.defenseBonus += 10;
            stats.accessorySlots++;
        }

        public override void UpdateArmorSet(Player player)
        {
            player.GetModPlayer<ElegantPlayer>().hasSetBonus = true;
        }
    }


    [AutoloadEquip(EquipType.Body)]
    public class ElagentBody : ModItem
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
            Item.rare = ItemRarityID.Orange; // The rarity of the item

        }

        public override void UpdateEquip(Player player)
        {
            var stats = player.GetStats();
            stats.defenseBonus += 11;
            stats.accessorySlots += 3;
        }
    }


    [AutoloadEquip(EquipType.Legs)]
    public class ElagentLegs : ModItem
    {
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 22;
            Item.value = 10000;
            Item.rare = ItemRarityID.Orange;
        }

        public override void UpdateEquip(Player player)
        {
            var stats = player.GetStats();
            stats.defenseBonus += 4;
            stats.insourceSlots += 3;
            stats.accessorySlots += 1;
        }
    }
}
