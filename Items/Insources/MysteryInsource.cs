using Stellamod.Common.XixianFlaskSystem;
using Stellamod.Items.Materials;
using Stellamod.Items.Materials.Molds;
using Terraria;
using Terraria.ID;

namespace Stellamod.Items.Insources
{
    public class MysteryInsource : InsourceItem
    {
        public override int GetAddedTime()
        {
            return 60 * 15;
        }
        public override void UseInsource(FlaskPlayer flaskPlayer)
        {
            base.UseInsource(flaskPlayer);
            Player player = flaskPlayer.Player;
            int[] buffPool = new int[]
            {
                BuffID.AmmoReservation,
                BuffID.Archery,
                BuffID.Endurance,
                BuffID.Regeneration,
                BuffID.Swiftness,
                BuffID.Ironskin,
                BuffID.ManaRegeneration,
                BuffID.MagicPower,
                BuffID.Lifeforce,
                BuffID.Lucky,
                BuffID.WellFed3
            };

            player.AddBuff(buffPool[Main.rand.Next(0, buffPool.Length)], 60 * 15);
        }
        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew<HypnotizedSoul, BlankBrooch>();
        }
    }
}
