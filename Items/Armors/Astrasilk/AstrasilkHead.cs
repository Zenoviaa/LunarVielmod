using Microsoft.Xna.Framework;
using Stellamod.Helpers;
using Stellamod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Armors.Astrasilk
{
    public class AstrasilkPlayer : ModPlayer
    {
        /*
         * Astrasilk 
            Player obtains five spinning stars around them,
            Hitting an enemy, will get rid of one of the stars if all five stars are removed, a giant falling star will rain and hit the enemy
            If you hit one enemy and hit a different enemy, all stars will come back
         */

        private int _starCount;
        private int _lastHitEnemy;

        private int StarProjType => ModContent.ProjectileType<AstrasilkStarProj>();
        private int GigaStarProjType => ModContent.ProjectileType<AstrasilkGigaStarProj>();
        private int OwnedStars => Player.ownedProjectileCounts[StarProjType];

        public bool hasSetBonus;
        public override void ResetEffects()
        {
            hasSetBonus = false;
        }


        public override void PostUpdateEquips()
        {
            if (!hasSetBonus)
                return;

            if(OwnedStars < _starCount)
            {
                Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, Vector2.Zero,
                    StarProjType, 0, 1, Player.whoAmI);
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (!hasSetBonus)
                return;

            if (target.whoAmI != _lastHitEnemy)
            {
                ResetStarCount();
            }
            else
            {
                _starCount--;
                KillOneStar();
                if (_starCount == 0)
                {
                    //Big star
                    float speed = 20;
                    Vector2 spawnPosition = target.Center + new Vector2(0, -512);
                    Vector2 velocity = spawnPosition.DirectionTo(target.Center).RotatedByRandom(MathHelper.PiOver4 / 64);
                    velocity *= speed;

                    int damage = 20;
                    int knockback = 5;
                    Projectile.NewProjectile(Player.GetSource_FromThis(), spawnPosition, velocity,
                        GigaStarProjType, damage, knockback, Player.whoAmI);
                }
            }
            _lastHitEnemy = target.whoAmI;
        }

        private void ResetStarCount()
        {
            _starCount = 5;
            foreach (Projectile proj in Main.ActiveProjectiles)
            {
                if (proj.owner != Player.whoAmI)
                    continue;
                if (proj.type != StarProjType)
                    continue;
                proj.Kill();
            }
        }

        private void KillOneStar()
        {
            foreach (Projectile proj in Main.ActiveProjectiles)
            {
                if (proj.owner != Player.whoAmI)
                    continue;
                if (proj.type != StarProjType)
                    continue;
                proj.Kill();
                break;
            }
        }
    }


    [AutoloadEquip(EquipType.Head)]
    public class AstrasilkHead : ModItem
    {

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Astolfo Wig");
            /* Tooltip.SetDefault("Yummy kummy :3"
				+ "\n+2% increased damage" +
				"\n+35 Health" +
				"\nIncreased Pickaxe Speed!"); */
            ArmorIDs.Head.Sets.DrawHatHair[Item.headSlot] = true;
        

            // If your head equipment should draw hair while drawn, use one of the following:
            // ArmorIDs.Head.Sets.DrawHead[Item.headSlot] = false; // Don't draw the head at all. Used by Space Creature Mask
            // ArmorIDs.Head.Sets.DrawHatHair[Item.headSlot] = true; // Draw hair as if a hat was covering the top. Used by Wizards Hat
            // ArmorIDs.Head.Sets.DrawFullHair[Item.headSlot] = true; // Draw all hair as normal. Used by Mime Mask, Sunglasses
            // ArmorIDs.Head.Sets.DrawBackHair[Item.headSlot] = true;
            // ArmorIDs.Head.Sets.DrawsBackHairWithoutHeadgear[Item.headSlot] = true; 
        }


        public override void SetDefaults()
        {
            Item.width = 40;
            Item.height = 30;
            Item.value = 10000;
            Item.rare = ItemRarityID.Blue;
            Item.defense = 3;
        }

        public override void UpdateEquip(Player player)
        {
            player.manaRegen += 4;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<AstrasilkBody>() 
                && legs.type == ModContent.ItemType<AstrasilkLegs>();
        }

        public override void ArmorSetShadows(Player player)
        {
            player.armorEffectDrawShadow = true;
        }

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = LangText.SetBonus(this);//"Stars orbit around you\nHit an enemy 5 times to rain down a gigantic star upon them!");
            player.GetModPlayer<AstrasilkPlayer>().hasSetBonus = true;
        }

        public override void AddRecipes() 
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ModContent.ItemType<StarSilk>(), 8);
            recipe.AddIngredient(ModContent.ItemType<AuroreanStarI>(), 2);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
        }
    }
}
