using Microsoft.Xna.Framework;
using Stellamod.Assets.Biomes;
using Stellamod.Items.Materials;
using Stellamod.Projectiles;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.NPCs.RoyalCapital
{
    internal class CarianKnight : ModNPC
    {
        private float ai_Counter;
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 15;
        }

        public override void SetDefaults()
        {
            NPC.width = 56;
            NPC.height = 62;
            NPC.damage = 40;
            NPC.defense = 10;
            NPC.lifeMax = 975;
            NPC.value = 90f;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.aiStyle = 10;
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.Player.InModBiome<AlcadziaBiome>())
            {
                return 0.6f;
            }

            //Else, the example bone merchant will not spawn if the above conditions are not met.
            return 0f;
        }

        public override void AI()
        {
            base.AI();
            ai_Counter++;
 
            Player player = Main.player[NPC.target];
            NPC.rotation = NPC.velocity.X * 0.03f;
            if(ai_Counter == 400)
            {
                if (StellaMultiplayer.IsHost)
                {
                    Vector2 direction = NPC.DirectionTo(player.Center);
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, direction * 9,
                        ModContent.ProjectileType<CarianKnightProj>(), 40, 1, Main.myPlayer);
                }
  
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/GhostExcalibur1"));
                for (int i = 0; i < 16; i++)
                {
                    Vector2 speed = Main.rand.NextVector2CircularEdge(4f, 4f);
                    var d = Dust.NewDustPerfect(NPC.Center, DustID.GemAmethyst, speed, Scale: 1.5f);
                    d.noGravity = true;
                }
                ai_Counter = 0;
            } 
            else if (ai_Counter > 300)
            {
                NPC.velocity *= 0.2f;
                float distance = 128;
                float particleSpeed = 8;

                Vector2 position = NPC.Center + Main.rand.NextVector2CircularEdge(distance, distance);
                Vector2 speed = (NPC.Center - position).SafeNormalize(Vector2.Zero) * particleSpeed;
                var d = Dust.NewDustPerfect(position, DustID.GemAmethyst, speed, Scale: 2f);
                d.noGravity = true;
            } 
            else if(ai_Counter == 300)
            {
                SoundEngine.PlaySound(SoundID.Zombie82, NPC.position);
                SoundEngine.PlaySound(SoundID.Zombie99, NPC.position);
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<AlcaricMush>(), 2, 1, 2));
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter += 0.3f;
            NPC.frameCounter %= Main.npcFrameCount[NPC.type];
            int frame = (int)NPC.frameCounter;
            NPC.frame.Y = frame * frameHeight;
        }
    }
}
