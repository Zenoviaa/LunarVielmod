using Microsoft.Xna.Framework;
using Stellamod.Helpers;
using Stellamod.NPCs.Bosses.DaedusRework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Bosses.SunStalker
{
    internal class SunStalkerPreSpawn : ModNPC
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Sun Stalker Lighting");
        }


        public override void SetDefaults()
        {
            NPC.alpha = 255;
            NPC.width = 0;
            NPC.height = 0;
            NPC.damage = 8;
            NPC.defense = 8;
            NPC.lifeMax = 156;
            NPC.value = 30f;
            NPC.buffImmune[BuffID.Poisoned] = true;
            NPC.buffImmune[BuffID.Venom] = true;
            NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.dontTakeDamage = true;
            NPC.dontCountMe = true;
            NPC.friendly = true;
        }
        public float Spawner = 0;
        public override void AI()
        {
            Spawner++;
            NPC.ai[0]++;
            var entitySource = NPC.GetSource_FromThis();
            if (NPC.ai[0] == 100 && Main.dayTime)
            {
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/SunStalker_Charge"), NPC.position);
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    NPC.NewNPC(entitySource, (int)(NPC.position.X), (int)(NPC.position.Y),
                        ModContent.NPCType<SunStalker>(), NPC.whoAmI, NPC.whoAmI);
                }
                else
                {
                    if (Main.netMode == NetmodeID.SinglePlayer)
                        return;

                    StellaMultiplayer.SpawnBossFromClient((byte)Main.LocalPlayer.whoAmI, 
                        ModContent.NPCType<SunStalker>(), (int)(NPC.position.X), (int)(NPC.position.Y));
                }
            }
            if (NPC.ai[0] == 200 && Main.dayTime)
            {
                NPC.active = false;
            }
            if (Main.rand.NextBool(6) && NPC.ai[0] >= 20 && Main.dayTime)
            {
                if (StellaMultiplayer.IsHost)
                {
                    NPC.NewNPC(entitySource, (int)NPC.Center.X, (int)NPC.Center.Y, 
                        ModContent.NPCType<SunStalkerRayLightBig>());
                }
            }
            if (Main.rand.NextBool(6) && NPC.ai[0] >= 20 && !Main.dayTime)
            {
                NPC.active = false;
            }
            if (NPC.ai[0] == 1)
            {
                Player player = Main.player[NPC.target];
                Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(NPC.Center, 1024f, 54f);

                if (Main.dayTime)
                {
                    player.velocity.X = NPC.direction * 6f;
                    player.velocity.Y = -9f;
                    SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/SunStalker_PreSpawn2"), NPC.position);
                    SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/SunStalker_PreSpawn"), NPC.position);
                }
                else
                {
                    CombatText.NewText(new Rectangle((int)NPC.position.X, (int)NPC.position.Y, NPC.width, NPC.height), new Color(220, 126, 58, 44),
                            LangText.Misc("SunStalkerPreSpawn"));
                    player.velocity.X = NPC.direction * 13f;
                    player.velocity.Y = -9f;
                    SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/SunStalker_PreSpawn_Fail"), NPC.position);
                }

                for (int i = 0; i < 50; i++)
                {
                    int num = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.GoldCoin, 0f, -2f, 0, default(Color), 1.5f);
                    Main.dust[num].noGravity = true;
                    Main.dust[num].position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
                    Main.dust[num].position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
                    {
                        Main.dust[num].velocity = NPC.DirectionTo(Main.dust[num].position) * 6f;
                    }
                }
                for (int i = 0; i < 14; i++)
                {
                    Dust.NewDustPerfect(base.NPC.Center, DustID.GoldCoin, (Vector2.One * Main.rand.Next(1, 12)).RotatedByRandom(19.0), 0, default(Color), 4f).noGravity = true;
                }
                for (int i = 0; i < 40; i++)
                {
                    Dust.NewDustPerfect(base.NPC.Center, DustID.GoldCoin, (Vector2.One * Main.rand.Next(1, 12)).RotatedByRandom(10.0), 0, default(Color), 1f).noGravity = false;
                }
                for (int i = 0; i < 40; i++)
                {
                    Dust.NewDustPerfect(base.NPC.Center, DustID.CopperCoin, (Vector2.One * Main.rand.Next(1, 12)).RotatedByRandom(25.0), 0, default(Color), 6f).noGravity = true;
                }
                for (int i = 0; i < 20; i++)
                {
                    Dust.NewDustPerfect(base.NPC.Center, DustID.CopperCoin, (Vector2.One * Main.rand.Next(1, 12)).RotatedByRandom(25.0), 0, default(Color), 2f).noGravity = false;
                }
            }
        }
    }
}

