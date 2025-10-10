using Microsoft.Xna.Framework;
using Stellamod.Core.Particles;
using Stellamod.Core.XixianFlaskSystem;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Items.Insources
{
    public class ManaRegeneratingInsourceBuff : ModBuff
    {
        public override void Update(Player player, ref int buffIndex)
        {
            base.Update(player, ref buffIndex);
            if (player.statMana < player.statManaMax2)
            {
                player.statMana += 1;
            }

            if (Main.rand.NextBool(10))
            {
                Vector2 position = player.Center + Main.rand.NextVector2CircularEdge(64, 64);
                Vector2 velocity = player.Center - position;
                velocity *= 0.05f;
                Particle.NewParticle<StarParticle>(position, velocity, Color.LightBlue, 0.3f);
            }
        }
    }
    public class ManaRegeneratingInsource : InsourceItem
    {
        public override int GetAddedTime()
        {
            return 60 * 10;
        }
        public override void UseInsource(FlaskPlayer flaskPlayer)
        {
            base.UseInsource(flaskPlayer);
            flaskPlayer.Player.AddBuff(ModContent.BuffType<ManaRegeneratingInsourceBuff>(), 60 * 5);
        }
    }
}
