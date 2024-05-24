using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary;
using Stellamod.Buffs;
using Stellamod.Helpers;
using Stellamod.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Accessories
{
    internal class IridinePlayer : ModPlayer
    {
        public bool hasIridineNecklace;
        public override void ResetEffects()
        {
            hasIridineNecklace = false;
        }

        public override bool PreKill(double damage, int hitDirection, bool pvp, ref bool playSound, ref bool genDust, ref PlayerDeathReason damageSource)
        {
            Player player = Player;
            if (player.whoAmI == Main.myPlayer 
                && player.statLife <= 0 && !player.HasBuff<IridineNecklaceCDBuff>() && hasIridineNecklace)
            {
                Revive(player);
                return false;
            }
            return base.PreKill(damage, hitDirection, pvp, ref playSound, ref genDust, ref damageSource);
        }

        private void Revive(Player player)
        {
            int healAmount = player.statLifeMax2;
            player.statLife = healAmount;
            player.HealEffect(healAmount);
            player.SetImmuneTimeForAllTypes(120);

            int reviveCooldown = 60 * 60 * 5;
            player.AddBuff(ModContent.BuffType<IridineNecklaceCDBuff>(), reviveCooldown);
            for(int i = 0; i < 48; i++)
            {
                Vector2 velocity = Main.rand.NextVector2CircularEdge(4, 4);
                ParticleManager.NewParticle<StarParticle>(player.Center, velocity, Color.White, 1f);
            }

            SoundEngine.PlaySound(SoundRegistry.IridineRevive, player.position);
        }
    }

    internal class IridineNecklace : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.ItemNoGravity[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.value = 2500;
            Item.rare = ModContent.RarityType<NiiviSpecialRarity>();
            Item.accessory = true;
        }

        public override void PostUpdate()
        {
            Lighting.AddLight(Item.Center, Color.WhiteSmoke.ToVector3() * 0.55f * Main.essScale); // Makes this item glow when thrown out of inventory.
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            base.UpdateAccessory(player, hideVisual);
            player.GetModPlayer<IridinePlayer>().hasIridineNecklace = true;

        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            DrawHelper.DrawGlowInInventory(Item, spriteBatch, position, Color.Purple);
            return true;
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            DrawHelper.DrawGlow2InWorld(Item, spriteBatch, ref rotation, ref scale, whoAmI);
            return true;
        }

        public override void Update(ref float gravity, ref float maxFallSpeed)
        {
            //The below code makes this item hover up and down in the world
            //Don't forget to make the item have no gravity, otherwise there will be weird side effects
            float hoverSpeed = 5;
            float hoverRange = 0.2f;
            float y = VectorHelper.Osc(-hoverRange, hoverRange, hoverSpeed);
            Vector2 position = new Vector2(Item.position.X, Item.position.Y + y);
            Item.position = position;
        }
    }
}
