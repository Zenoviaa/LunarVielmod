
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.DropRules;
using Stellamod.Helpers;
using Stellamod.Items.Accessories;
using Stellamod.Items.Materials;
using Stellamod.Items.Weapons.Ranged.GunSwapping;
using Stellamod.NPCs.Bosses.INest;
using Stellamod.Utilis;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Stellamod.NPCs.Acidic
{

    public class AcidProbe : ModNPC
    {
        public bool Rocks;
        public float Timer;
        public float RotSpeed = 0.3f;
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(Timer);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Timer = reader.ReadSingle();
        }

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Acid Probe");
            Main.npcFrameCount[Type] = 1;
            NPCID.Sets.TrailCacheLength[NPC.type] = 10;
            NPCID.Sets.TrailingMode[NPC.type] = 0;
        }

        public override void SetDefaults()
        {
            NPC.width = 44;
            NPC.height = 54;
            NPC.damage = 30;
            NPC.defense = 14;
            NPC.lifeMax = 130;
            NPC.HitSound = SoundID.NPCHit42;
            NPC.DeathSound = new SoundStyle("Stellamod/Assets/Sounds/AcidProbeDeath") with { PitchVariance = 0.1f };
            NPC.value = 30f;
            NPC.buffImmune[BuffID.Poisoned] = true;
            NPC.buffImmune[BuffID.Venom] = true;
            NPC.noGravity = true;
            NPC.alpha = 0;
            NPC.knockBackResist = .75f;
            NPC.aiStyle = -1;
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter += 0.2f;
            NPC.frameCounter %= Main.npcFrameCount[NPC.type];
            int frame = (int)NPC.frameCounter;
            NPC.frame.Y = frame * frameHeight;
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            for (int k = 0; k < 3; k++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height,
                    ModContent.DustType<Dusts.GlowDust>(), newColor: new Color(24, 142, 61));
                int d = Dust.NewDust(NPC.position, NPC.width, NPC.height,
                    ModContent.DustType<Dusts.GunFlash>(), newColor: new Color(24, 142, 61));
                Main.dust[d].rotation = (Main.dust[d].position - NPC.position).ToRotation() - MathHelper.PiOver4;
            }

            if (NPC.life <= 0)
            {
                for (int i = 0; i < 20; i++)
                {
                    int num = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.CursedTorch, 0f, -2f, 0, default(Color), .8f);
                    Main.dust[num].noGravity = true;
                    Main.dust[num].position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
                    Main.dust[num].position.Y += Main.rand.Next(-50, 51) * .05f - 1.5f;
                    if (Main.dust[num].position != NPC.Center)
                        Main.dust[num].velocity = NPC.DirectionTo(Main.dust[num].position) * 6f;
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
        {
            Lighting.AddLight(NPC.Center, Color.GreenYellow.ToVector3() * 1.25f * Main.essScale);
            SpriteEffects Effects = ((base.NPC.spriteDirection != -1) ? SpriteEffects.FlipHorizontally : SpriteEffects.None);
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;

            Vector2 frameOrigin = NPC.frame.Size() / 2;
            Vector2 sizeOrigin = NPC.Size / 2;
            Vector2 DrawPos = NPC.position - screenPos + sizeOrigin; 

            float time = Main.GlobalTimeWrappedHourly;
            float timer = Main.GlobalTimeWrappedHourly / 2f + time * 0.04f;

            time %= 4f;
            time /= 2f;

            if (time >= 1f)
            {
                time = 2f - time;
            }

            time = time * 0.5f + 0.5f;
            for (float i = 0f; i < 1f; i += 0.25f)
            {
                float radians = (i + timer) * MathHelper.TwoPi;
                spriteBatch.Draw(texture, DrawPos + new Vector2(0f, 2).RotatedBy(radians) * time, NPC.frame, new Color(53, 107, 112, 0), NPC.rotation, frameOrigin, NPC.scale, Effects, 0);
            }

            for (float i = 0f; i < 1f; i += 0.34f)
            {
                float radians = (i + timer) * MathHelper.TwoPi;
                spriteBatch.Draw(texture, DrawPos + new Vector2(0f, 4).RotatedBy(radians) * time, NPC.frame, new Color(152, 208, 113, 0), NPC.rotation, frameOrigin, NPC.scale, Effects, 0);
            }

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
          
            for (int k = 0; k < NPC.oldPos.Length; k++)
            {
                Vector2 drawPos = NPC.oldPos[k] - Main.screenPosition + sizeOrigin + new Vector2(0f, NPC.gfxOffY);
                Color color = NPC.GetAlpha(Color.Lerp(new Color(255, 253, 90), new Color(72, 131, 56), 1f / NPC.oldPos.Length * k) * (1f - 1f / NPC.oldPos.Length * k));
                spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, drawPos, new Microsoft.Xna.Framework.Rectangle?(NPC.frame), color, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, Effects, 0f);
            }

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            return true;
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            Player player = spawnInfo.Player;
            if (!(player.ZoneTowerSolar || player.ZoneTowerVortex || player.ZoneTowerNebula || player.ZoneTowerStardust && !Main.pumpkinMoon && !Main.snowMoon))
            {
                return spawnInfo.Player.ZoneAcid() ? 0.8f : 0f;
            }

            return 0f;
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            LeadingConditionRule hardmodeDropRule = new LeadingConditionRule(new HardmodeDropRule());
            hardmodeDropRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<GraftedSoul>(), 2, minimumDropped: 1, maximumDropped: 5));
            hardmodeDropRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<SrTetanus>(), 20, minimumDropped: 1, maximumDropped: 1));

            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<VirulentPlating>(), minimumDropped: 1, maximumDropped: 4));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<AcidStaketers>(), chanceDenominator: 30));
        }

        public override void AI()
        {
            NPC.spriteDirection = NPC.direction;
            Player player = Main.player[NPC.target];
            int Distance = (int)(NPC.Center - player.Center).Length();

            if (Distance > 300f)
            {
                Timer = 299;
            }
            else
            {
                Timer++;
            }

            if (Timer == 300)
            {
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/AcidProbe1"), NPC.position);
                RotSpeed = 0.3f;
                NPC.ai[3] = 35;
            }
            if (Timer >= 350 && Timer <= 450)
            {
                if (Timer % 11 == 0)
                {
                    float rot = NPC.velocity.ToRotation();
                    float spread = 0.4f;
                         
        
                    Vector2 position = NPC.position;
                    Vector2 direction = Vector2.Normalize(Main.player[NPC.target].Center - NPC.Center) * 8.5f;
                    Vector2 offset = new Vector2(1.5f, 0).RotatedBy(direction.ToRotation());
                    for (int k = 0; k < 7; k++)
                    {
                        Vector2 direction2 = offset.RotatedByRandom(spread);


                        Dust.NewDustPerfect(NPC.Center + offset * 43, ModContent.DustType<Dusts.GlowDust>(), direction2 * Main.rand.NextFloat(8), 125, ColorFunctions.AcidFlame, Main.rand.NextFloat(0.2f, 0.5f));
                    }

                    Dust.NewDustPerfect(NPC.Center, ModContent.DustType<Dusts.GlowDust>(), new Vector2(0, 0), 125, new Color(150, 80, 40), 1);
                    Dust.NewDustPerfect(NPC.Center, ModContent.DustType<Dusts.TSmokeDust>(), Vector2.UnitY * -2 + offset.RotatedByRandom(spread), 150, ColorFunctions.AcidFlame * 0.5f, Main.rand.NextFloat(0.5f, 1));
                    SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/AcidProbe3"), NPC.position);
                    Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(base.NPC.Center, 512f, 4f);
        
                    SoundEngine.PlaySound(SoundID.Item8, NPC.position);
                    SoundEngine.PlaySound(SoundID.Zombie53, NPC.position);
                    float offsetX = Main.rand.Next(-350, 350) * 0.01f;
                    float offsetY = Main.rand.Next(-350, 350) * 0.01f;

                    if (StellaMultiplayer.IsHost)
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y, direction.X + offsetX, direction.Y + offsetY, 
                            ModContent.ProjectileType<ToxicMissile>(), 12, 1, Main.myPlayer, 0, 0);
                }
            }
            if (Timer == 450)
            {
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/AcidProbe2"), NPC.position);
                RotSpeed = 0.1f;
                Timer = 2;
                NPC.ai[3] = 15;
                NPC.netUpdate = true;
            }

            float velMax = 1f;
            float acceleration = 0.011f;
            NPC.TargetClosest(true);
            Vector2 center = NPC.Center;
            float deltaX = Main.player[NPC.target].position.X + (Main.player[NPC.target].width / 2) - center.X;
            float deltaY = Main.player[NPC.target].position.Y + (Main.player[NPC.target].height / 2) - center.Y;
            float distance = (float)Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
            if (NPC.ai[1] > 200.0)
            {
                if (NPC.ai[1] > 300.0)
                {
                    NPC.ai[1] = 0f;
                }
            }
            else if (distance < 120.0)
            {
                NPC.ai[0] += 0.9f;
                if (NPC.ai[0] > 0f)
                {
                    NPC.velocity.Y = NPC.velocity.Y + 0.039f;
                }
                else
                {
                    NPC.velocity.Y = NPC.velocity.Y - 0.019f;
                }
                if (NPC.ai[0] < -100f || NPC.ai[0] > 100f)
                {
                    NPC.velocity.X = NPC.velocity.X + 0.029f;
                }
                else
                {
                    NPC.velocity.X = NPC.velocity.X - 0.029f;
                }
                if (NPC.ai[0] > 25f)
                {
                    NPC.ai[0] = -200f;
                }
            }

            if (Main.rand.NextBool(30) && Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (Main.rand.NextBool(2))
                {
                    NPC.velocity.Y = NPC.velocity.Y + 0.439f;
                }
                else
                {
                    NPC.velocity.Y = NPC.velocity.Y - 0.419f;
                }
                NPC.netUpdate = true;
            }
            if (distance > 350.0)
            {
                velMax = 5f;
                acceleration = 0.2f;
            }
            else if (distance > 300.0)
            {
                velMax = 3f;
                acceleration = 0.25f;
            }
            else if (distance > 250.0)
            {
                velMax = 2.5f;
                acceleration = 0.13f;
            }

            float stepRatio = velMax / distance;
            float velLimitX = deltaX * stepRatio;
            float velLimitY = deltaY * stepRatio;
            if (Main.player[NPC.target].dead)
            {
                velLimitX = (float)((NPC.direction * velMax) / 2.0);
                velLimitY = (float)((-velMax) / 2.0);
            }
            
            if (NPC.velocity.X < velLimitX)
                NPC.velocity.X = NPC.velocity.X + acceleration;
            else if (NPC.velocity.X > velLimitX)
                NPC.velocity.X = NPC.velocity.X - acceleration;

            if (NPC.velocity.Y < velLimitY)
                NPC.velocity.Y = NPC.velocity.Y + acceleration;
            else if (NPC.velocity.Y > velLimitY)
                NPC.velocity.Y = NPC.velocity.Y - acceleration;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            if (Main.rand.NextBool(3))
            {
                target.AddBuff(BuffID.Poisoned, 180);
            }
        }
    }
}