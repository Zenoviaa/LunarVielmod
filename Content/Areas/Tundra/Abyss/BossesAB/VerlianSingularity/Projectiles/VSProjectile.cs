using Stellamod.Core;
using Terraria;

namespace Stellamod.Content.Areas.Tundra.Abyss.BossesAB.VerlianSingularity.Projectiles
{
    public abstract class VSProjectile : ScarletProjectile
    {
        protected int Parent
        {
            get => (int)Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        protected ref float Timer => ref Projectile.ai[1];
      
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
        }

        public override void AI()
        {
            base.AI();
        }

        public NPC GetParentNPC()
        {
            return Main.npc[Parent];
        }
    }
}
