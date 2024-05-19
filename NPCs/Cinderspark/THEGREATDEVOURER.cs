using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Assets.Biomes;
using Stellamod.DropRules;
using Stellamod.Helpers;
using Stellamod.Items.Harvesting;
using Stellamod.Items.Materials;
using Stellamod.Items.Weapons.Thrown.Jugglers;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Cinderspark
{
    internal class THEGREATDEVOURER : ModNPC
    {
        private ref float ai_Counter => ref NPC.ai[0];
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 30; // The amount of frames the NPC has
            NPCID.Sets.TrailCacheLength[NPC.type] = 10;
            NPCID.Sets.TrailingMode[NPC.type] = 0;
        }

        public override void SetDefaults()
        {
            NPC.width = 118;
            NPC.height = 84;
            NPC.aiStyle = -1;
            NPC.damage = 60;
            NPC.defense = 8;
            NPC.lifeMax = 600;
            NPC.knockBackResist = 0f;
            NPC.npcSlots = 2;
            NPC.lavaImmune = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.HitSound = new SoundStyle("Stellamod/Assets/Sounds/Gintze_Hit") with { PitchVariance = 0.1f };
            NPC.DeathSound = new SoundStyle("Stellamod/Assets/Sounds/Gintze_Death") with { PitchVariance = 0.1f };
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter += 0.8f;
            NPC.frameCounter %= Main.npcFrameCount[NPC.type];
            int frame = (int)NPC.frameCounter;
            NPC.frame.Y = frame * frameHeight;
        }

        public override void AI()
        {
            NPC.TargetClosest();
            if (NPC.HasValidTarget)
            {
                ai_Counter++;
                Player target = Main.player[NPC.target];
                float targetRotation = NPC.DirectionTo(target.Center).ToRotation();
                NPC.rotation = MathHelper.Lerp(NPC.rotation, targetRotation, 0.33f);

                int chaseTicks = 300;
                int attackTicks = 600;
                Vector2 direction = NPC.Center.DirectionTo(target.Center);
                if(ai_Counter == chaseTicks)
                {
                    SoundEngine.PlaySound(SoundID.Item20, NPC.position);
                }

                if (ai_Counter == attackTicks)
                {
                    //Thrust
                    float speed = 20;
                    Vector2 targetVelocity = direction * speed;
                    NPC.velocity += targetVelocity;

                    SoundEngine.PlaySound(SoundID.DD2_BetsyFireballShot, NPC.position);
                    for (int i = 0; i < 16; i++)
                    {
                        Vector2 dustSpeed = Main.rand.NextVector2CircularEdge(8f, 8f);
                        var d = Dust.NewDustPerfect(NPC.Center, DustID.InfernoFork, dustSpeed, Scale: 3f);
                        d.noGravity = true;
                    }

                    ai_Counter = 0;
                }
                else
                {
                    //Slowly move towards player
                    float speed = ai_Counter > chaseTicks ? 2 : 4;
                    float accel = 0.2f;
                    float distanceToTarget = Vector2.Distance(NPC.Center, target.Center);
                    if(distanceToTarget < 300)
                    {
                        speed = 0.5f;
                    } else if (distanceToTarget > 700)
                    {
                        speed = 5;
                    }

                    Vector2 targetVelocity = direction * speed;
                    Vector2 diffVelocity = NPC.velocity.DirectionTo(targetVelocity);

                    float dist = Vector2.Distance(NPC.velocity, targetVelocity);
                
                    if (dist > 0.1f)
                    {
                        NPC.velocity += diffVelocity * accel;
                    }
                }

                if (ai_Counter > chaseTicks)
                {
                    //Charging
                    Dust dust = Dust.NewDustDirect(NPC.Center, NPC.width, NPC.height, DustID.InfernoFork);
                    dust.velocity *= -1f;
                    dust.scale *= .8f;
                    dust.noGravity = true;

                    int dist = 128;
                    Vector2 randVector = new Vector2(Main.rand.Next(-dist, dist), Main.rand.Next(-dist, dist));
                    randVector.Normalize();

                    Vector2 randVector2 = randVector * (Main.rand.Next(50, 100) * 0.04f);
                    dust.velocity = randVector2;
                    randVector2.Normalize();

                    Vector2 vector2_3 = randVector2 * 34f;
                    dust.position = NPC.Center - vector2_3;
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

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            SpriteEffects Effects = ((base.NPC.spriteDirection != -1) ? SpriteEffects.FlipHorizontally : SpriteEffects.None);
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            for (int k = 0; k < NPC.oldPos.Length; k++)
            {
                Vector2 drawPos = NPC.oldPos[k] - Main.screenPosition + NPC.Size / 2 + new Vector2(0f, NPC.gfxOffY);
                Color color = NPC.GetAlpha(Color.Lerp(Color.OrangeRed, Color.Transparent, 1f / NPC.oldPos.Length * k) * (1f - 1f / NPC.oldPos.Length * k));
                spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, drawPos, new Microsoft.Xna.Framework.Rectangle?(NPC.frame), color, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, Effects, 0f);
            }

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            Vector3 huntrianColorXyz = DrawHelper.HuntrianColorOscillate(
                Color.OrangeRed.ToVector3(),
                Color.Red.ToVector3(),
                new Vector3(3, 3, 3), 0);

            DrawHelper.DrawDimLight(NPC, huntrianColorXyz.X, huntrianColorXyz.Y, huntrianColorXyz.Z, Color.OrangeRed, Color.White, 0);
            Lighting.AddLight(screenPos, Color.OrangeRed.ToVector3() * 1.0f * Main.essScale);
            return base.PreDraw(spriteBatch, screenPos, drawColor);
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(BuffID.OnFire, 300);
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.Player.InModBiome<CindersparkBiome>() && !spawnInfo.Player.ZoneUnderworldHeight)
            {
                return 0.1f;
            }

            //Else, the example bone merchant will not spawn if the above conditions are not met.
            return 0f;
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Cinderscrap>(), chanceDenominator: 4, minimumDropped: 2, maximumDropped: 5));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<MoltenScrap>(), chanceDenominator: 2, minimumDropped: 1, maximumDropped: 3));
           
            LeadingConditionRule hardmodeDropRule = new LeadingConditionRule(new HardmodeDropRule());
            hardmodeDropRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<CinderBomber>(), 
                chanceDenominator: 3));
            npcLoot.Add(hardmodeDropRule);
        }
    }
}
