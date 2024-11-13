using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Helpers;
using System.Collections.Generic;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Common.Bases
{
    public enum BroochType
    {
        Simple,
        Advanced,
        Radiant
    }

    public class BroochFollowerProjectile : ModProjectile
    {
        public override string Texture => TextureRegistry.EmptyTexture;
        private int BroochItemType
        {
            get => (int)Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        public override void SetDefaults()
        {
            Projectile.width = 28;
            Projectile.height = 28;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.scale = 0.6f;
            Projectile.timeLeft = 10;
        }

        // Here you can decide if your minion breaks things like grass or pots
        public override bool? CanCutTiles()
        {
            return false;
        }

        // This is mandatory if your minion deals contact damage (further related stuff in AI() in the Movement region)
        public override bool MinionContactDamage()
        {
            return false;
        }

        // The AI of this minion is split into multiple methods to avoid bloat. This method just passes values between calls actual parts of the AI.
        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            BroochSpawnerPlayer spawnerPlayer = owner.GetModPlayer<BroochSpawnerPlayer>();
            if (spawnerPlayer.BroochActive(BroochItemType))
            {
                Projectile.timeLeft = 2;
            }
            if (BroochItemType == 0)
                Projectile.Kill();


            SummonHelper.CalculateIdleValues(owner, Projectile, out Vector2 vectorToIdlePosition, out float distanceToIdlePosition);
            SummonHelper.Idle(Projectile, distanceToIdlePosition, vectorToIdlePosition);
            Visuals();
        }

        public void Visuals()
        {
            // So it will lean slightly towards the direction it's moving
            Projectile.rotation = Projectile.velocity.X * 0.05f;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(ModContent.GetModItem(BroochItemType).Texture).Value;
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            Vector2 drawOrigin = texture.Size() / 2f;
            SpriteBatch spriteBatch = Main.spriteBatch;
            spriteBatch.Draw(texture, drawPos, null, Color.White.MultiplyRGB(lightColor), Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0f);
            return false;
        }
    }

    public class BroochSpawnerPlayer : ModPlayer
    {
        public List<int> broochesToSpawn = new List<int>();
        public bool hasAdvancedBrooches;
        public bool hasRadiantBrooches;
        public override void ResetEffects()
        {
            base.ResetEffects();
            hasAdvancedBrooches = false;
            hasRadiantBrooches = false;
            broochesToSpawn.Clear();
        }

        public override void PostUpdateEquips()
        {
            base.PostUpdateEquips();
            int broochFollowerProjectileType = ModContent.ProjectileType<BroochFollowerProjectile>();
            int index = 0;

            //Gotta do it this way cause of radiant brooches
            while (index < broochesToSpawn.Count)
            {
                int broochItemType = broochesToSpawn[index];
                BaseBrooch brooch = ModContent.GetModItem(broochItemType) as BaseBrooch;
                switch (brooch.BroochType)
                {
                    case BroochType.Simple:
                        brooch.UpdateBrooch(Player);
                        break;
                    case BroochType.Advanced:
                        if (hasAdvancedBrooches)
                        {
                            brooch.UpdateBrooch(Player);
                        }

                        break;
                    case BroochType.Radiant:
                        if (hasRadiantBrooches)
                        {
                            brooch.UpdateBrooch(Player);
                        }
                        break;
                }

                index++;
            }

            for (int i = 0; i < broochesToSpawn.Count; i++)
            {
                int broochItemType = broochesToSpawn[i];
                BaseBrooch brooch = ModContent.GetModItem(broochItemType) as BaseBrooch;
                switch (brooch.BroochType)
                {
                    case BroochType.Simple:
                        if (!OwnsBroochProjectile(broochItemType))
                        {
                            SpawnBroochProjectile(broochItemType);
                        }
                        if (brooch.Item.buffType != 0)
                            Player.AddBuff(brooch.Item.buffType, 2);
                        break;
                    case BroochType.Advanced:
                        if (hasAdvancedBrooches)
                        {
                            if (!OwnsBroochProjectile(broochItemType))
                            {
                                SpawnBroochProjectile(broochItemType);
                            }
                            if (brooch.Item.buffType != 0)
                                Player.AddBuff(brooch.Item.buffType, 2);
                        }

                        break;
                    case BroochType.Radiant:
                        if (hasRadiantBrooches)
                        {
                            if (!OwnsBroochProjectile(broochItemType))
                            {
                                SpawnBroochProjectile(broochItemType);
                            }
                            if (brooch.Item.buffType != 0)
                                Player.AddBuff(brooch.Item.buffType, 2);
                        }
                        break;
                }
            }
        }

        public bool OwnsBroochProjectile(int itemType)
        {
            int broochFollowerProjectileType = ModContent.ProjectileType<BroochFollowerProjectile>();
            foreach (var projectile in Main.ActiveProjectiles)
            {
                if (projectile.type != broochFollowerProjectileType)
                    continue;
                if (projectile.owner != Player.whoAmI)
                    continue;
                if (projectile.ai[0] == itemType)
                    return true;
            }
            return false;
        }

        public void SpawnBroochProjectile(int itemType)
        {
            Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, Vector2.Zero,
                ModContent.ProjectileType<BroochFollowerProjectile>(), 0, 0, Player.whoAmI, itemType);
        }

        public bool BroochActive(int itemType)
        {
            int broochItemType = broochesToSpawn.Find(x => x == itemType);
            if (broochItemType == -1)
                return false;
            ModItem modItem = ModContent.GetModItem(broochItemType);
            if (modItem == null)
                return false;

            BaseBrooch brooch = modItem as BaseBrooch;
            if (brooch == null)
                return false;

            switch (brooch.BroochType)
            {
                default:
                case BroochType.Simple:
                    return true;
                case BroochType.Advanced:
                    return hasAdvancedBrooches || hasRadiantBrooches;
                case BroochType.Radiant:
                    return hasRadiantBrooches;
            }
        }
    }

    public abstract class BaseBrooch : ModItem
    {
        public BroochType BroochType;
        public bool HideVisual;

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.accessory = true;
            Item.rare = ItemRarityID.Green;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            // Here we add a tooltipline that will later be removed, showcasing how to remove tooltips from an item
            var line = new TooltipLine(Mod, "", "");
            switch (BroochType)
            {
                case BroochType.Simple:
                    line = new TooltipLine(Mod, "Brooch of Ame", LangText.Common("SimpleBrooch"))
                    {
                        OverrideColor = new Color(198, 124, 225)

                    };
                    tooltips.Add(line);
                    break;
                case BroochType.Advanced:
                    line = new TooltipLine(Mod, "", "");
                    line = new TooltipLine(Mod, "Brooch of the TaGo", LangText.Common("AdvancedBrooch"))
                    {
                        OverrideColor = new Color(254, 128, 10)

                    };
                    tooltips.Add(line);

                    line = new TooltipLine(Mod, "Brooch of the TaGo", LangText.Common("AdvancedBackpack"))
                    {
                        OverrideColor = new Color(198, 124, 225)

                    };
                    tooltips.Add(line);
                    break;
                case BroochType.Radiant:
                    line = new TooltipLine(Mod, "", "");
                    line = new TooltipLine(Mod, "Brooch of the HV", LangText.Common("RadiantBrooch"))
                    {
                        OverrideColor = new Color(220, 252, 255)
                    };

                    tooltips.Add(line);
                    line = new TooltipLine(Mod, "Brooch of the Radiant", LangText.Common("RadiantBackpack"))
                    {
                        OverrideColor = new Color(177, 255, 117)

                    };
                    tooltips.Add(line);
                    break;
            }
        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            Player player = Main.player[Main.myPlayer];
            BroochSpawnerPlayer broochPlayer = player.GetModPlayer<BroochSpawnerPlayer>();

            //Check that this item is equipped
            switch (BroochType)
            {
                case BroochType.Advanced:
                    if (player.HasItemEquipped(Item))
                    {
                        //Check that you have advanced brooches since these don't work without
                        if (broochPlayer.hasAdvancedBrooches)
                        {
                            //Give backglow to show that the effect is active
                            DrawHelper.DrawAdvancedBroochGlow(Item, spriteBatch, position, new Color(198, 124, 225));
                        }
                        else
                        {
                            float sizeLimit = 28;
                            //Draw the item icon but gray and transparent to show that the effect is not active
                            Main.DrawItemIcon(spriteBatch, Item, position, Color.Gray * 0.8f, sizeLimit);
                            return false;
                        }
                    }

                    break;
                case BroochType.Radiant:
                    if (player.HasItemEquipped(Item))
                    {
                        //Check that you have advanced brooches since these don't work without
                        if (broochPlayer.hasRadiantBrooches)
                        {
                            //Give backglow to show that the effect is active
                            DrawHelper.DrawAdvancedBroochGlow(Item, spriteBatch, position, new Color(247, 209, 92));
                        }
                        else
                        {
                            float sizeLimit = 49;
                            //Draw the item icon but gray and transparent to show that the effect is not active
                            Main.DrawItemIcon(spriteBatch, Item, position, Color.Gray * 0.8f, sizeLimit);
                            return false;
                        }
                    }
                    break;
            }

            return true;
        }


        public sealed override void UpdateAccessory(Player player, bool hideVisual)
        {
            base.UpdateAccessory(player, hideVisual);
            BroochSpawnerPlayer broochSpawnerPlayer = player.GetModPlayer<BroochSpawnerPlayer>();
            broochSpawnerPlayer.broochesToSpawn.Add(Type);
            HideVisual = hideVisual;
        }

        public virtual void UpdateBrooch(Player player)
        {

        }
    }
}
