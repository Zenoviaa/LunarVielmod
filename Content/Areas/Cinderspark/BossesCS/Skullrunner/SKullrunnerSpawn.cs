using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using Stellamod.Assets;
using Stellamod.Content.Areas.Cinderspark.BossesCS.Skullrunner.Projectiles;
using Stellamod.Content.Areas.WondrousDarkspace.NPCsWD;
using Stellamod.Core.Particles;
using Stellamod.Core.Shaders;
using Stellamod.Core.Shaders.MagicTrails;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Visual.Particles;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Cinderspark.BossesCS.Skullrunner
{
    public class SkullrunnerSpawn : ModNPC
    {
        private int _frame;
        private enum AnimationState
        {
            Laugh,
            Deadass,
            NoDamage,
            Sideframe,
            Dunking,
            Confusednograb,
            Abttograb,
            Outtabreath,
        }
        private enum AIState
        {
            SpawningLaughingGrin,
        }

        private bool _freezeFrame;
        private AnimationState _animation;

        private ref float Timer => ref NPC.ai[0];
        private AIState State
        {
            get => (AIState)NPC.ai[1];
            set => NPC.ai[1] = (float)value;
        }
        private ref float BeatTimer => ref NPC.ai[2];
        private ref float Cycle => ref NPC.ai[3];
        private int BurningBlackSkullDamage => 20;
        private Player Target => Main.player[NPC.target];
        private Vector2 DirectionToTarget => (Target.Center - NPC.Center).SafeNormalize(Vector2.Zero);

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            NPCID.Sets.TrailingMode[Type] = 3;
            NPCID.Sets.TrailCacheLength[Type] = 8;
            NPCID.Sets.MPAllowedEnemies[Type] = true;
            Main.npcFrameCount[Type] = 17;
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            NPC.width = 32;
            NPC.height = 64;
            NPC.damage = 32;
            NPC.defense = 0;
            NPC.lifeMax = 1100;
            NPC.HitSound = SoundID.NPCHit16;
            NPC.value = Item.buyPrice(silver: 50);
            NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.npcSlots = 10f;

            //Setup the music and boss bar
            NPC.aiStyle = -1;
        }

        public override void FindFrame(int frameHeight)
        {
            base.FindFrame(frameHeight);
            NPC.frameCounter += 0.25f;
            if (NPC.frameCounter >= 1f)
            {
                _frame++;
                NPC.frameCounter = 0f;
            }

            switch (_animation)
            {
                case AnimationState.Laugh:
                    if (_frame >= 5)
                    {
                        _frame = 0;
                    }
                    break;
                case AnimationState.Deadass:
                    _frame = 5;
                    break;
                case AnimationState.NoDamage:
                    _frame = 6;
                    break;
                case AnimationState.Sideframe:
                    _frame = 7;
                    break;
                case AnimationState.Dunking:
                    if (_frame < 8)
                    {
                        _frame = 8;
                    }
                    if (_frame >= 12)
                    {
                        _frame = 8;
                    }
                    break;
                case AnimationState.Confusednograb:
                    _frame = 12;
                    break;
                case AnimationState.Abttograb:
                    _frame = 13;
                    break;
                case AnimationState.Outtabreath:
                    if (_frame < 14)
                    {
                        _frame = 14;
                    }
                    if (_frame >= 17)
                    {
                        _frame = 14;
                    }
                    break;
            }

            NPC.frame.Y = frameHeight * _frame;
        }

        public override void AI()
        {
            base.AI();
            BeatTimer++;
            switch (State)
            {
                case AIState.SpawningLaughingGrin:
                    AI_SpawningLaughingGrin();
                    break;
            }

            Lighting.AddLight(NPC.position, Color.OrangeRed.ToVector3() * 0.78f);
        }

        private bool BeatHit()
        {
            return BeatTimer % 27 == 0;
        }

 
        private void FaceDirection()
        {
            NPC.direction = (Target.Center.X < NPC.Center.X) ? 1 : -1;
            NPC.spriteDirection = -NPC.direction;
        }
     

        private void AI_SpawningLaughingGrin()
        {
            //Starts out by flying out of the lava with a laughing grin
            _animation = AnimationState.Laugh;
            Timer++;
            if (Timer % 8 == 0)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Lava);
            }

            //Probably a lot of lava particles and stuff
            if (Timer == 1)
            {
                SoundStyle lavaSpawnSound = SoundID.Lavafall;
                lavaSpawnSound.PitchVariance = 0.1f;
                SoundEngine.PlaySound(lavaSpawnSound, NPC.position);

                SoundEngine.PlaySound(SoundID.Item73, NPC.position);
                for (float f = 0; f < 16; f++)
                {
                    Vector2 particleVelocity = -Vector2.UnitY * Main.rand.NextFloat(0.5f, 1f);
                    particleVelocity *= 8;
                    particleVelocity = particleVelocity.RotatedByRandom(MathHelper.PiOver4);
                    Dust.NewDustPerfect(NPC.Center, DustID.Lava, particleVelocity, Scale: Main.rand.NextFloat(0.5f, 1f));
                }


                //Extra Fire effects
                for (float f = 0; f < 16; f++)
                {
                    Vector2 position = NPC.Center;
                    Vector2 velocity = -Vector2.UnitY;
                    Vector2 pVelocity = velocity.RotatedByRandom(MathHelper.PiOver4 / 3f);
                    pVelocity *= Main.rand.NextFloat(0.5f, 2f);
                    var frag = Particle.NewParticle<GlowFragmentParticle>(position, pVelocity);
                    FXUtil.GlowFragmentParticle(position, pVelocity,
                        innerColor: Color.Red,
                        outerColor: Color.Orange,
                        fadeToColor: Color.Purple,
                        distortOut: true);

                    if (Main.rand.NextBool(4))
                    {
                        Dust.NewDustPerfect(position, ModContent.DustType<TSmokeDust>(),
                                         velocity.RotatedByRandom(MathHelper.PiOver4 / 2f) * 2);
                    }
                    if (Main.rand.NextBool(4))
                    {
                        Dust.NewDustPerfect(position, ModContent.DustType<GlowDust>(),
                                         velocity.RotatedByRandom(MathHelper.PiOver4 / 2f) * 3 * Main.rand.NextFloat(0.4f, 1f), newColor: Color.White, Scale: 0.2f);
                    }
                    if (Main.rand.NextBool(4))
                    {

                        var part = FXUtil.GlowFragmentParticle(position, pVelocity,
                         innerColor: Color.DarkRed,
                         outerColor: Color.DarkBlue,
                         fadeToColor: Color.Black,
                         distortOut: false);
                        part.Scale *= 1.3f;
                    }
                }
            }

            if (Timer < 60)
            {
                _freezeFrame = true;
                //Rising into the air
                float risingInterpolant = Timer / 60f;
                NPC.velocity.Y = MathHelper.Lerp(-4, 0, risingInterpolant);
            }
            else if (Timer < 180)
            {
                _freezeFrame = false;
                //Sin left and right bobbing up and down while laughing
                NPC.velocity.Y = MathF.Sin(Timer * 0.25f);

                float targetRotation = MathF.Sin(Timer * 0.5f) * 0.5f;
                NPC.rotation = MathHelper.Lerp(NPC.rotation, targetRotation, 0.1f);
            }
            else if (Timer < 300)
            {
                //Slow down, give a bith of breathing room
                NPC.velocity.Y *= 0.9f;
                NPC.rotation *= 0.9f;
            }

            if (Timer > 300)
            {
                //Start fight
                if (StellaMultiplayer.IsHost)
                {
                    NPC.NewNPC(NPC.GetSource_FromThis(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<Skullrunner>());
                    NPC.active = false;
                }
            }
        }
        private void SwitchState(AIState state)
        {
            if (StellaMultiplayer.IsHost)
            {
                Timer = 0;
                State = state;
                Cycle = 0;
            }
        }
        private void DrawGlowingAura(SpriteBatch spriteBatch, Vector2 screenPos)
        {
            float drawScale = 1;
            Texture2D auraTexture = ModContent.Request<Texture2D>(Texture + "_Aura").Value;
            Texture2D auraTexture2 = ModContent.Request<Texture2D>(Texture + "_Aura2").Value;



            Vector2 auraDrawPos = NPC.Center - screenPos;
            auraDrawPos -= Vector2.UnitY * 16 * ExtraMath.Osc(0f, 1f);
            Vector2 auraDrawOrigin = auraTexture.Size() / 2f;
            Vector2 auraDrawOrigin2 = auraTexture2.Size() / 2f;
            Vector2 auraDrawScale = Vector2.One * 0.75f;
            spriteBatch.Restart(blendState: BlendState.Additive);

            float auraDrawRotation = Main.GlobalTimeWrappedHourly * 0.4f;
            spriteBatch.Draw(auraTexture2, auraDrawPos, null, Color.White * 0.85f, auraDrawRotation, auraDrawOrigin2, auraDrawScale * drawScale, SpriteEffects.None, 0);
            spriteBatch.Draw(auraTexture, auraDrawPos, null, Color.White * 0.85f, -auraDrawRotation * 0.5f, auraDrawOrigin, auraDrawScale * drawScale, SpriteEffects.None, 0);

            spriteBatch.RestartDefaults();
        }


        private void DrawAura(SpriteBatch spriteBatch)
        {
            Texture2D texture2D4 = ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/DimLight").Value;
            Color glowColor = Color.OrangeRed;
            glowColor.A = 0;

            for (int i = 0; i < 3; i++)
            {
                spriteBatch.Draw(texture2D4, NPC.Center - Main.screenPosition, null, glowColor, NPC.rotation, new Vector2(32, 32), 0.35f * (5 + 0.6f), SpriteEffects.None, 0f);
            }

        }
        private void DrawTrail(SpriteBatch spriteBatch)
        {

            FlamingTrailShader flamingTrailShader = FlamingTrailShader.Instance;
            flamingTrailShader.BlendState = BlendState.Additive;
            TrailDrawer.Draw(spriteBatch, NPC.oldPos, NPC.oldRot, ColorFunction, WidthFunction, flamingTrailShader, NPC.Size / 2f);

        }
        public float WidthFunction(float completionRatio)
        {
            float baseWidth = 48;
            return MathHelper.SmoothStep(baseWidth, 0.5f, completionRatio);
        }

        public Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(Color.OrangeRed, Color.Red, completionRatio) * 0.7f;
        }

  
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            float drawScale = 1.5f;
            DrawAura(spriteBatch);
            DrawTrail(spriteBatch);
            DrawGlowingAura(spriteBatch, screenPos);
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawPos = NPC.position - screenPos + NPC.Size / 2 + new Vector2(0f, NPC.gfxOffY);


            SpriteEffects spriteEffects = NPC.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            Vector2 drawOrigin = NPC.frame.Size() / 2;
            spriteBatch.Draw(texture, drawPos, NPC.frame, Color.White.MultiplyRGB(drawColor), NPC.rotation, drawOrigin, drawScale, spriteEffects, 0);
            return false;
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            base.ModifyNPCLoot(npcLoot);
        }
    }
}
