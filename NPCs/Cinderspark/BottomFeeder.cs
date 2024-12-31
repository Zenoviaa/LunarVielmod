
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Assets.Biomes;
using Stellamod.Buffs;
using Stellamod.Helpers;
using Stellamod.Items.Consumables;
using Stellamod.Items.Harvesting;
using Stellamod.Items.Materials;
using Stellamod.Items.Weapons.Mage;
using Stellamod.Items.Weapons.Melee;
using Stellamod.Items.Weapons.Ranged;
using Stellamod.Items.Weapons.Summon;
using Stellamod.NPCs.Bosses.DreadMire;
using Stellamod.NPCs.Bosses.DreadMire.Heart;
using Stellamod.NPCs.Bosses.singularityFragment;
using Stellamod.NPCs.Bosses.singularityFragment.Phase1;
using Stellamod.NPCs.Bosses.Verlia;
using Stellamod.Projectiles;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Stellamod.NPCs.Cinderspark
{
    internal class BottomFeeder : ModNPC
    {
        private ref float ai_Counter => ref NPC.ai[0];
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 6; // The amount of frames the NPC has
        }

        public override void SetDefaults()
        {
            NPC.width = 88;
            NPC.height = 70;
            NPC.aiStyle = -1;
            NPC.damage = 45;
            NPC.defense = 42;
            NPC.lifeMax = 200;
            NPC.knockBackResist = 0f;
            NPC.npcSlots = 1;
            NPC.lavaImmune = true;
            NPC.HitSound = new SoundStyle("Stellamod/Assets/Sounds/Gintze_Hit") with { PitchVariance = 0.1f };
            NPC.DeathSound = new SoundStyle("Stellamod/Assets/Sounds/Gintze_Death") with { PitchVariance = 0.1f };
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter += 0.25f;
            NPC.frameCounter %= Main.npcFrameCount[NPC.type];
            int frame = (int)NPC.frameCounter;
            NPC.frame.Y = frame * frameHeight;
        }
        Vector2 Drawoffset => new Vector2(0, NPC.gfxOffY) + Vector2.UnitX * NPC.spriteDirection * 0;
        public virtual string GlowTexturePath => Texture + "_Glow";
        private Asset<Texture2D> _glowTexture;
        public Texture2D GlowTexture => (_glowTexture ??= (RequestIfExists<Texture2D>(GlowTexturePath, out var asset) ? asset : null))?.Value;
        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            float num108 = 4;
            float num107 = (float)Math.Cos((double)(Main.GlobalTimeWrappedHourly % 1.4f / 1.4f * 6.28318548f)) / 2f + 0.5f;
            float num106 = 0f;
            Color color1 = Color.OrangeRed * num107 * .8f;
            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            spriteBatch.Draw(
                GlowTexture,
                NPC.Center - Main.screenPosition + Drawoffset,
                NPC.frame,
                color1,
                NPC.rotation,
                NPC.frame.Size() / 2,
                NPC.scale,
                effects,
                0
            );
            SpriteEffects spriteEffects3 = NPC.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Vector2 vector33 = new Vector2(NPC.Center.X, NPC.Center.Y) - Main.screenPosition + Drawoffset - NPC.velocity;
            Color color29 = new Color(127 - NPC.alpha, 127 - NPC.alpha, 127 - NPC.alpha, 0).MultiplyRGBA(Color.LightBlue);
            for (int num103 = 0; num103 < 1; num103++)
            {
                Color color28 = color29;
                color28 = NPC.GetAlpha(color28);
                color28 *= 1f - num107;
                Vector2 vector29 = NPC.Center + (num103 / (float)num108 * 6.28318548f + NPC.rotation + num106).ToRotationVector2() * (4f * num107 + 2f) - Main.screenPosition + Drawoffset - NPC.velocity * num103;
                Main.spriteBatch.Draw(GlowTexture, vector29, NPC.frame, color28, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, spriteEffects3, 0f);
            }
        }
        public override void AI()
        {
            NPC.TargetClosest();
            if (NPC.HasValidTarget)
            {
                Player target = Main.player[NPC.target];
                float xDist = target.position.X - NPC.position.X;
                xDist = Math.Abs(xDist);

                bool canAttack = xDist < 256 && target.position.Y < NPC.position.Y; 
                if(ai_Counter < 120 && canAttack)
                {
                    ai_Counter++;
                }
         
                if(ai_Counter == 120)
                {
                    Dust.QuickDustLine(NPC.Center, NPC.Center + new Vector2(0, -700), 48, Color.OrangeRed);
                    ai_Counter++;
                }

                if(ai_Counter > 120 && ai_Counter < 240)
                {
                    ai_Counter++;
                }

                if(ai_Counter == 240)
                {
                    //Shoot projectile
                    if (StellaMultiplayer.IsHost)
                    {
                        for(int i = 0; i < Main.rand.Next(4, 8); i++)
                        {
                            Vector2 velocity = new Vector2(Main.rand.NextFloat(-4f, 4f), -30);
                            velocity *= Main.rand.NextFloat(0.5f, 1f);
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, velocity,
                                ModContent.ProjectileType<CinderFireball>(), (int)(NPC.damage * 0.33f), 1, Main.myPlayer);
                           
                            //Dust Particles
                            for (int k = 0; k < 4; k++)
                            {
                                Vector2 newVelocity = velocity.RotatedByRandom(MathHelper.ToRadians(7));
                                newVelocity *= 1f - Main.rand.NextFloat(0.3f);
                                Dust.NewDust(NPC.Center, 0, 0, DustID.Smoke, newVelocity.X * 0.5f, newVelocity.Y * 0.5f);
                            }
                        }
                    }

                    SoundEngine.PlaySound(SoundID.Item73, NPC.position);
                    ai_Counter = 0;
                }
            }
        }

        public override void OnKill()
        {
            for (int i = 0; i < 16; i++)
            {
                float speedX = Main.rand.NextFloat(-1f, 1f);
                float speedY = Main.rand.NextFloat(-1f, 1f);
                float scale = Main.rand.NextFloat(0.66f, 1f);
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.InfernoFork,
                    speedX, speedY, Scale: scale);
            }
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(BuffID.OnFire, 180);
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.Player.InModBiome<CindersparkBiome>() && !spawnInfo.Player.ZoneUnderworldHeight)
            {
                return 0.6f;
            }

            //Else, the example bone merchant will not spawn if the above conditions are not met.
            return 0f;
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Cinderscrap>(), chanceDenominator: 4, minimumDropped: 2,  maximumDropped: 5));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<MoltenScrap>(), chanceDenominator: 2, minimumDropped: 1, maximumDropped: 3));
        }
    }
}
