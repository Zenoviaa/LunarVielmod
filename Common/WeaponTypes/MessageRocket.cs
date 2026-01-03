using Stellamod.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace Stellamod.Common.WeaponTypes
{
    public class MessageFireworkRocket : ModProjectile
    {
        private enum AIState
        {
            Lit,
            Launching
        }
        private ref float Timer => ref Projectile.ai[0];
        public override string Texture => TextureRegistry.EmptyTexture;
        private AIState State
        {
            get => (AIState)Projectile.ai[1];
            set => Projectile.ai[1] = (float)value;
        }
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 120;
        }
        public override void AI()
        {
            base.AI();
            switch (State)
            {
                case AIState.Lit:
                    AI_Lit();
                    break;
                case AIState.Launching:
                    AI_Launching();
                    break;
            }
        }

        private void AI_Lit()
        {

        }

        private void AI_Launching()
        {

        }

        private void SwitchState(AIState state)
        {
            if (this.OwnedByLocalClient())
            {
                Timer = 0;
                State = state;
                Projectile.netUpdate = true;
            }
        }
    }
}
