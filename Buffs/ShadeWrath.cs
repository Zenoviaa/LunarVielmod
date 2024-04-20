using Microsoft.Xna.Framework;
using ParticleLibrary;
using Stellamod.Helpers;
using Stellamod.Particles;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Buffs
{
    internal class ShadeWrath : ModBuff
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Charm Buff!");
            // Description.SetDefault("A true warrior such as yourself knows no bounds");
            Main.debuff[Type] = false;
            Main.pvpBuff[Type] = true;
            Main.buffNoTimeDisplay[Type] = false;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetDamage(DamageClass.Generic) += 0.5f;
            player.moveSpeed += 0.5f;

            if (Main.rand.NextBool(2))
            {
                int count = Main.rand.Next(6);
                for (int i = 0; i < count; i++)
                {
                    Vector2 position = player.RandomPositionWithinEntity();
                    Vector2 speed = new Vector2(0, Main.rand.NextFloat(-0.2f, -1f));
                    Color color = default(Color).MultiplyAlpha(0.1f);
                    Particle p = ParticleManager.NewParticle(position, speed, ParticleManager.NewInstance<Ink2>(), color, Main.rand.NextFloat(0.2f, 0.8f));
                    p.layer = Particle.Layer.BeforePlayersBehindNPCs;
                }
            }
        }
    }
}
