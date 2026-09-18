using Stellamod.Content.Dusts;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Illuria.BossesIL.EStyr
{
    public class BlackStarsPlayer : ModPlayer
    {
        private int _timer;
        private int _maxHealthLoss;

        public override void PostUpdateBuffs()
        {
            base.PostUpdateBuffs();
            if (Player.HasBuff<BlackStars>())
            {
                _timer++;
                if (_timer >= BlackStars.DecayRate)
                {
                    _maxHealthLoss--;
                    _timer = 0;
                }
            }
            if ((!NPC.AnyNPCs(ModContent.NPCType<E>()) || Player.dead))
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

    public class BlackStars : ModBuff
    {
        public static int DecayRate => 4;
        public static int CalculateMaxHealthLoss(int hitPointsToLose)
        {
            return BlackStars.DecayRate * hitPointsToLose;
        }

        public static void AddBuff(Player target, int hitPointsToLose)
        {
            int buffType = ModContent.BuffType<BlackStars>();
            int time = CalculateMaxHealthLoss(hitPointsToLose);
            target.AddBuff(buffType, time);
        }
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
                }
                else
                {
                    Vector2 pos = player.Center + Main.rand.NextVector2Circular(16, 16);
                    Vector2 velocity = -Vector2.UnitY;
                    float scale = Main.rand.NextFloat(0.5f, 0.75f);
                    Dust d = Dust.NewDustPerfect(pos, ModContent.DustType<Sparkle>(), velocity, Scale: scale, newColor: Color.White);
                    d.noGravity = true;
                }
            }
        }
    }
}
