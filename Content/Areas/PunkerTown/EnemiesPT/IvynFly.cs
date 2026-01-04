using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Common;
using Stellamod.Common.Shaders;
using Stellamod.Content.Items.Materials;
using Stellamod.Core.Astar;
using Stellamod.Core.Particles;
using Stellamod.Dusts;
using Stellamod.Visual.Particles;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.PunkerTown.EnemiesPT
{
    public class IvynFly : ModNPC,
        IDrawOutlines
    {
        private enum AIState
        {
            Idle,
            Chase,
            BuzzAround,
            Dash
        }

        private enum AnimationState
        {
            Stop,
            Flap,
            Dash
        }

        private Color _outlineColor;
        private bool _warn;
        private bool _contactDamage;
        private bool _hitWall;
        private ref float Timer => ref NPC.ai[0];

        private AIState State
        {
            get => (AIState)NPC.ai[1];
            set => NPC.ai[1] = (float)value;
        }
        private Player Target => Main.player[NPC.target];
        private Stack<Vector2> Path;
        private Vector2 CurrentNode;
        private Vector2 NextNode;
        private AnimationState Animation;
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Main.npcFrameCount[Type] = 7;
            NPCID.Sets.TrailCacheLength[Type] = 8;
            NPCID.Sets.TrailingMode[Type] = 3;
            this.AddToMarsh();
            // this.ModifySpawnWeight(0.5f);
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            NPC.width = 20;
            NPC.height = 20;
            NPC.lifeMax = 50;
            NPC.damage = 18;
            NPC.defense = 4;
            NPC.noGravity = true;
            NPC.noTileCollide = false;
            NPC.HitSound = SoundID.NPCHit15;
            NPC.DeathSound = SoundID.NPCDeath11;
        }

        private int _frame;
        public override void FindFrame(int frameHeight)
        {
            base.FindFrame(frameHeight);
            NPC.frameCounter += 0.15f;
            if (NPC.frameCounter >= 1f)
            {
                NPC.frameCounter = 0f;
                _frame++;
            }

            switch (Animation)
            {
                case AnimationState.Stop:
                    _frame = 0;
                    break;
                case AnimationState.Flap:
                    if (_frame < 1)
                        _frame = 1;
                    if (_frame >= 6)
                        _frame = 1;
                    break;
                case AnimationState.Dash:
                    _frame = 6;
                    break;
            }

            NPC.frame.Y = _frame * frameHeight;
        }


        public override void AI()
        {
            base.AI();
            if (!NPC.HasValidTarget)
                NPC.TargetClosest();

            _warn = false;
            _contactDamage = false;
            switch (State)
            {
                case AIState.Idle:
                    AI_Idle();
                    break;
                case AIState.Chase:
                    AI_Chase();
                    break;
                case AIState.BuzzAround:
                    AI_BuzzAround();
                    break;
                case AIState.Dash:
                    AI_Dash();
                    break;
            }

            NPC.velocity += GetTileOutwardVelocity();
            Color targetOutlineColor;
            if (_contactDamage)
                targetOutlineColor = Color.Red;
            else if (_warn)
                targetOutlineColor = Color.Yellow;
            else
                targetOutlineColor = Color.Transparent;
            _outlineColor = Color.Lerp(_outlineColor, targetOutlineColor, 0.2f);
        }


        private void AI_Idle()
        {
            Timer++;
            if (Timer == 1)
            {
                NPC.TargetClosest();
            }

            float targetY = MathF.Sin(Timer * 0.1f) * 3;
            NPC.velocity.Y = MathHelper.Lerp(NPC.velocity.Y, targetY, 0.1f);
            if (Vector2.Distance(NPC.Center, Target.Center) < 1500)
            {
                SwitchState(AIState.Chase);
            }
            Animation = AnimationState.Flap;
        }

        private void AI_Chase()
        {
            Timer++;
            if (Timer == 1)
            {
                NPC.TargetClosest();
            }

            //Recalculate path once per second


            bool canHitLine = Collision.CanHitLine(NPC.position, 1, 1, Target.position, 1, 1);
            if (!canHitLine)
            {
                if (Timer % 30 == 0 && NPC.HasValidTarget)
                {
                    SearchJob searchJob = new SearchJob(NPC.Center, Target.Center, tileSearchRange: 50, airPadding: 1);
                    Path = Astar.Search(searchJob);
                    if (Path != null && Path.Count > 0)
                    {
                        CurrentNode = Path.Pop();
                        if (Path.Count > 0)
                            NextNode = Path.Peek();
                    }
                    else
                    {
                        CurrentNode = Vector2.Zero;
                    }
                }
            }
            if (canHitLine)
            {
                Vector2 targetVelocity = (Target.Center - NPC.Center).SafeNormalize(Vector2.Zero);
                targetVelocity *= 5f;
                NPC.velocity = Vector2.Lerp(NPC.velocity, targetVelocity, 0.1f);
                NPC.rotation = NPC.velocity.X * 0.05f;
                NPC.direction = (Target.Center.X > NPC.Center.X) ? 1 : -1;
            }
            else if (CurrentNode != Vector2.Zero)
            {
                Vector2 targetVelocity = (CurrentNode - NPC.Center).SafeNormalize(Vector2.Zero);
                targetVelocity *= 5f;
                NPC.velocity = Vector2.Lerp(NPC.velocity, targetVelocity, 0.1f);
                NPC.rotation = NPC.velocity.X * 0.05f;
                NPC.direction = (CurrentNode.X > NPC.Center.X) ? 1 : -1;

                //node has been crossed
                //not sure howe ewll this is gonna work but we'll see

                float distanceToCurrentNode = Vector2.Distance(NPC.Center, CurrentNode);
                float distanceToNextNode = Vector2.Distance(NPC.Center, CurrentNode);
                if (distanceToNextNode <= distanceToCurrentNode)
                {
                    CurrentNode = Vector2.Zero;
                }
            }
            else if (Path != null && Path.Count > 0)
            {
                CurrentNode = Path.Pop();
                if (Path.Count > 0)
                    NextNode = Path.Peek();
            }
            else if (CurrentNode == Vector2.Zero)
            {
                Vector2 targetVelocity = (Target.Center - NPC.Center).SafeNormalize(Vector2.Zero);
                targetVelocity *= 5f;
                NPC.velocity = Vector2.Lerp(NPC.velocity, targetVelocity, 0.1f);
                NPC.rotation = NPC.velocity.X * 0.05f;
                NPC.direction = (Target.Center.X > NPC.Center.X) ? 1 : -1;
            }



            float distanceToTarget = Vector2.Distance(NPC.Center, Target.Center);
            if (distanceToTarget <= 100 && Collision.CanHitLine(NPC.position, 1, 1, Target.position, 1, 1))
            {
                SwitchState(AIState.BuzzAround);
            }
            Animation = AnimationState.Flap;
        }

        private void AI_BuzzAround()
        {
            Timer++;
            if (Timer == 1)
            {

            }
            _warn = true;
            //buzz around in place prearing to attack
            NPC.velocity = NPC.velocity.RotatedBy(0.1f);
            if (NPC.velocity.Length() > 2f)
            {
                NPC.velocity *= 0.9f;
            }
            else
            {
                NPC.velocity *= 1.1f;
            }

            if (!Collision.CanHitLine(NPC.position, 1, 1, Target.position, 1, 1))
            {
                SwitchState(AIState.Chase);
            }
            NPC.direction = (Target.Center.X > NPC.Center.X) ? 1 : -1;
            NPC.rotation = NPC.velocity.X * 0.05f;
            if (Timer >= 90)
            {
                SwitchState(AIState.Dash);
            }
            Animation = AnimationState.Flap;
        }

        private void ImpactParticles()
        {
            if (_hitWall)
                return;

            float numDust = 3;
            for (float n = 0; n < numDust; n++)
            {
                Vector2 velocity = NPC.velocity.SafeNormalize(Vector2.Zero) * 15;
                velocity = velocity.RotatedByRandom(MathHelper.PiOver4);
                velocity *= Main.rand.NextFloat(0.3f, 2f);
                Dust.NewDustPerfect(NPC.Center + NPC.velocity, ModContent.DustType<GlowDust>(), velocity, newColor: Color.White, Scale: 0.4f);
                SparkleParticle sp = Particle<SparkleParticle>.Spawn(NPC.Center + NPC.velocity, velocity, Scale: Main.rand.NextFloat(0.2f, 0.6f));
                sp.gravity = 0;
                sp.dampening = 0.1f;
                sp.outerColor = Color.DarkGreen;
            }
        }

        private Vector2 GetTileOutwardVelocity()
        {
            Vector2 outwardVelocity = Vector2.Zero;
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    Point tile = NPC.Center.ToTileCoordinates() + new Point(i, j);
                    if (!WorldGen.InWorld(tile.X, tile.Y))
                        continue;
                    if (WorldGen.SolidOrSlopedTile(tile.X, tile.Y))
                    {

                        outwardVelocity -= new Vector2(i, j);
                    }
                }
            }
            outwardVelocity *= 0.5f;
            return outwardVelocity;
        }

        private void AI_Dash()
        {
            Timer++;
            Animation = AnimationState.Dash;
            if (Timer < 20)
            {
                Animation = AnimationState.Stop;
                NPC.velocity *= 0.9f;
            }
            if (Timer < 21)
            {
                _warn = true;
                _hitWall = false;
            }
            if (Timer == 21)
            {
                SoundStyle buzz = new SoundStyle("Stellamod/Assets/Sounds/Jack_Land");
                buzz.PitchVariance = 0.3f;
                SoundEngine.PlaySound(buzz, NPC.position);
                NPC.velocity = (Target.Center - NPC.Center).SafeNormalize(Vector2.Zero) * 15;



            }

            if (Timer > 21)
            {
                NPC.direction = NPC.velocity.X > 0 ? 1 : -1;
                NPC.rotation = NPC.velocity.ToRotation();
            }

            if (NPC.collideX)
            {
                NPC.velocity.X *= -0.75f;
                ImpactParticles();
                _hitWall = true;

            }

            if (NPC.collideY)
            {
                NPC.velocity.Y *= -0.75f;
                ImpactParticles();
                _hitWall = true;

            }

            if (_hitWall)
            {
                NPC.velocity.Y += 0.05f;
            }

            if (Timer > 32)
            {
                NPC.velocity *= 0.98f;
            }

            if (Timer >= 21 && Timer < 100)
            {
                _contactDamage = true;
            }
            if (Timer >= 120)
            {
                SwitchState(AIState.Idle);
            }

        }


        private void SwitchState(AIState state)
        {
            if (MultiplayerHelper.IsHost)
            {
                Timer = 0;
                State = state;
                NPC.netUpdate = true;
            }
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            base.HitEffect(hit);
            if (Main.netMode == NetmodeID.Server)
                return;
            if (NPC.life <= 0)
            {
                //Create gores
                for (int k = 0; k < 2; k++)
                {
                    Vector2 pos = NPC.position;
                    pos.X += Main.rand.Next(0, NPC.width);
                    pos.Y += Main.rand.Next(0, NPC.height);
                    DustParticle dp = Particle<DustParticle>.Spawn(pos, Vector2.UnitX * hit.HitDirection * Main.rand.NextFloat(1f, 4f), Scale: 0.5f);
                    dp.outerColor = Color.DarkGray;
                    dp.gravity = 0.01f;
                    dp.fast = true;
                }


                int headGore = Mod.Find<ModGore>($"{Name}_Gore_Top").Type;
                int legGore = Mod.Find<ModGore>($"{Name}_Gore_Bottom").Type;

                // Spawn the gores. The positions of the arms and legs are lowered for a more natural look.
                Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, headGore, 1f);
                Gore.NewGore(NPC.GetSource_Death(), NPC.position + new Vector2(0, 34), NPC.velocity, legGore);
            }
        }
        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            return base.CanHitPlayer(target, ref cooldownSlot) && _contactDamage;
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            base.ModifyNPCLoot(npcLoot);
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Ivythorn>(), minimumDropped: 1, maximumDropped: 3));
        }

        private void DrawSprite(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = TextureAssets.Npc[Type].Value;
            Vector2 drawOrigin = NPC.frame.Size() / 2f;
            SpriteEffects spriteEffects = NPC.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Vector2 drawCenter = NPC.Center - screenPos;
            spriteBatch.Draw(texture, drawCenter, NPC.frame, drawColor, NPC.rotation, drawOrigin, NPC.scale, spriteEffects, 0);
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = TextureAssets.Npc[Type].Value;
            Vector2 drawOrigin = NPC.frame.Size() / 2f;
            SpriteEffects spriteEffects = NPC.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            for (int i = 0; i < NPC.oldPos.Length; i++)
            {
                Vector2 oldDrawCenter = NPC.oldPos[i] + NPC.Size / 2f - screenPos;
                float ratio = (float)i / (float)NPC.oldPos.Length;
                Color afterImageColor = Color.Lerp(Color.White, Color.Transparent, ratio);
                afterImageColor *= 0.2f;
                spriteBatch.Draw(texture, oldDrawCenter, NPC.frame, afterImageColor, NPC.oldRot[i], drawOrigin, NPC.scale, spriteEffects, 0);
            }

            DrawSprite(spriteBatch, screenPos, drawColor);
            return false;
        }
        public void DrawOutlines(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
        {
            Vector2 v = Vector2.UnitY * 2;
            Vector2 h = Vector2.UnitX * 2;
            DrawSprite(spriteBatch, screenPos + v, _outlineColor);
            DrawSprite(spriteBatch, screenPos - v, _outlineColor);
            DrawSprite(spriteBatch, screenPos + h, _outlineColor);
            DrawSprite(spriteBatch, screenPos - h, _outlineColor);
        }
    }
}
