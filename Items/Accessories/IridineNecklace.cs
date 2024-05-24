using Microsoft.Xna.Framework;
using ParticleLibrary;
using Stellamod.Buffs;
using Stellamod.Helpers;
using Stellamod.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
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
                && player.statLife <= 0 && !player.HasBuff<IridineNecklaceCDBuff>())
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
    }
}
