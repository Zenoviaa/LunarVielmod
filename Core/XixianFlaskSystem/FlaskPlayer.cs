using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Stellamod.Core.XixianFlaskSystem
{
    internal class FlaskPlayer : ModPlayer
    {
        private Item _insource;
        public bool hasTime;
        public Item Insource
        {
            get
            {
                if (_insource == null)
                {
                    _insource = new Item();
                    _insource.SetDefaults(0);
                }
                return _insource;
            }
            set
            {
                _insource = value;
            }
        }

        public override void PostUpdate()
        {
            base.PostUpdate();
            if (!Insource.IsAir)
            {
                int ownedCount = Player.ownedProjectileCounts[ModContent.ProjectileType<InsourceDefaultProjectile>()];
                if (ownedCount == 0)
                {
                    Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, Vector2.Zero,
                        ModContent.ProjectileType<InsourceDefaultProjectile>(), 0, 0,
                        Owner: Player.whoAmI, ai1: Insource.type);
                }
            }
        }
        public override void SaveData(TagCompound tag)
        {
            base.SaveData(tag);
            tag["insource"] = Insource;
        }

        public override void LoadData(TagCompound tag)
        {
            base.LoadData(tag);
            if (tag.ContainsKey("insource"))
                Insource = tag.Get<Item>("insource");
        }
    }
}
