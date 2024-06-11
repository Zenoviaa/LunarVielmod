using Microsoft.Xna.Framework;
using ParticleLibrary;
using Stellamod.Dusts;
using Stellamod.NPCs.Bosses.DaedusRework;
using Stellamod.NPCs.Bosses.GothiviaTheSun.GOS;
using Stellamod.NPCs.Bosses.GothiviaTheSun.REK;
using Stellamod.Particles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Buffs
{
    internal class GothivianPlayer : ModPlayer
    {
        private int _timer;
        private int _maxHealthLoss;
        public override void PostUpdateBuffs()
        {
            base.PostUpdateBuffs();
            if (Player.HasBuff<GothivianFlames>())
            {
                _timer++;
                if(_timer >= 4)
                {
                    _maxHealthLoss--;
                    _timer = 0;
                }
            }
            if ((!NPC.AnyNPCs(ModContent.NPCType<RekSnake>()) && !NPC.AnyNPCs(ModContent.NPCType<DaedusR>()) && !NPC.AnyNPCs(ModContent.NPCType<GothiviaIyx>())) || Player.dead)
            {
                _maxHealthLoss = 0;
            }
        }

        public override void PostUpdateEquips()
        {
            base.PostUpdateEquips();
            Player.statLifeMax2 += _maxHealthLoss;
            
        }
    }

    internal class GothivianFlames : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoTimeDisplay[Type] = false;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            base.Update(player, ref buffIndex);
            if (Main.rand.NextBool(8))
            {
                if (Main.rand.NextBool(2))
                {
                    Vector2 pos = player.Center + Main.rand.NextVector2Circular(16, 16);
                    Vector2 velocity = -Vector2.UnitY;
                    float scale = Main.rand.NextFloat(0.5f, 0.75f);
                    ParticleManager.NewParticle<morrowstar>(pos, velocity, Color.White, scale);
                }
                else
                {
                    Vector2 pos = player.Center + Main.rand.NextVector2Circular(16, 16);
                    Vector2 velocity = -Vector2.UnitY;
                    float scale = Main.rand.NextFloat(0.5f, 0.75f);
                    Dust d = Dust.NewDustPerfect(pos, ModContent.DustType<GlowDust>(), velocity, Scale: scale, newColor: Color.White);
                    d.noGravity = true;
                }
            }
        }
    }
}
