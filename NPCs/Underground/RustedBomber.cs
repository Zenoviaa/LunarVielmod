using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Audio;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Stellamod.Projectiles;
using Stellamod.Items.Materials.Tech;
using Terraria.GameContent.ItemDropRules;

namespace Stellamod.NPCs.Underground
{
    internal class RustedBomber : ModNPC
    {
        private bool _attack;
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 13;
        }

        public override void SetDefaults()
        {
            NPC.width = 50;
            NPC.height = 58;
            NPC.damage = 51;
            NPC.defense = 12;
            NPC.lifeMax = 70;
            NPC.HitSound = new SoundStyle("Stellamod/Assets/Sounds/Gintze_Hit") with { PitchVariance = 0.1f };
            NPC.DeathSound = SoundID.NPCDeath14;
            NPC.value = 63f;
            NPC.knockBackResist = 0f;
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter += 0.15f;
            NPC.frameCounter %= Main.npcFrameCount[NPC.type];
            int frame = (int)NPC.frameCounter;
            NPC.frame.Y = frame * frameHeight;
        }

        public override void AI()
        {
            //NPC.velocity.X *= 0.98f;
            //Syncing the attack to the animation
            int frame = (int)NPC.frameCounter;
            if(frame == 0)
            {
                _attack = true;
            }

            if(frame == 7 && _attack)
            {
                _attack = false;
                Vector2 fireCenter = NPC.Center + new Vector2(0, -NPC.height / 2);
                if (StellaMultiplayer.IsHost)
                {
          
                    for(int i = 0; i < Main.rand.Next(2, 4); i++)
                    {
                        Vector2 velocity = new Vector2(0, -10);
                        velocity = velocity.RotatedByRandom(MathHelper.ToRadians(45));
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), fireCenter, velocity,
                            ModContent.ProjectileType<RustedBomb>(), 10, 4, Main.myPlayer);
                    }
             
                }

                for(int i = 0; i < 16; i++)
                {
                    Vector2 velocity = new Vector2(0, -10);
                    velocity = velocity.RotatedByRandom(MathHelper.ToRadians(45));
                    Dust.NewDustPerfect(fireCenter, DustID.Smoke, velocity);
                }

                SoundEngine.PlaySound(SoundID.Item14, NPC.position);
            }

            Visuals();
        }

        private void Visuals()
        {
            if (Main.rand.NextBool(80))
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Electric);
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<BrokenTech>(), 6, 1, 3));
            npcLoot.Add(ItemDropRule.Common(ItemID.IronOre, 1, 1, 5));
        }
    }
}
