using Microsoft.Xna.Framework;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.UI.Systems;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Bosses.Verlia
{
    internal class VerliaDP : ModNPC
    {
        private int _bossType = -1;
        private float _centerSparkleSize = 0.4f;
        private ref float ai_Timer => ref NPC.ai[0];
        public override void SetDefaults()
        {
            NPC.lifeMax = 1;
            NPC.damage = 1;
            NPC.friendly = true;
            NPC.dontTakeDamage = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.width = 1;
            NPC.height = 1;
        }

        int gren = 0;
        public override void AI()
        {
            float ai1 = NPC.whoAmI;
            gren++;

            if (gren == 2)
            {

            }
            _centerSparkleSize += 0.02f;


            ai_Timer++;
            if (ai_Timer % 4 == 0)
            {

                for (int i = 0; i < 1; i++)
                {
                    Dust.NewDustPerfect(NPC.Center, ModContent.DustType<GlowDust>(), (Vector2.Zero), 0, Color.LightBlue, _centerSparkleSize);
                }

            }

            int duration = 300;
            if (ai_Timer < duration)
            {
                ShakeModSystem.Shake = ai_Timer / 18;
                if (ai_Timer % 4 == 0)
                {

                }

                if (Main.rand.NextBool(4))
                {
                    Vector2 edge = Main.rand.NextVector2CircularEdge(32, 32);
                    Vector2 spawnPosition = NPC.Center + edge;
                    Vector2 velocity = NPC.DirectionFrom(spawnPosition);
                    var d = Dust.NewDustPerfect(NPC.Center, DustID.GemDiamond, velocity, Scale: 1.5f);
                    d.noGravity = true;
                }
            }
            else if (ai_Timer > duration)
            {

                for (int i = 0; i < 64; i++)
                {
                    Vector2 speed = Main.rand.NextVector2CircularEdge(4f, 4f);
                    var d = Dust.NewDustPerfect(NPC.Center, DustID.GemDiamond, speed * 8, Scale: 1.5f);
                    d.noGravity = true;
                }

                ShakeModSystem.Shake = 0;
                Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(NPC.Center, 1024f, 32f);

                //oh wait i need net code
                // NPC.NewNPC(, (int)ai_Boss_Spawn);

                NPC.Kill();
                ai_Timer = 0;
            }
        }
    }
}
