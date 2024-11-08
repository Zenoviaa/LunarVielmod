using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Helpers;
using Stellamod.Trails;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Bosses.DaedusTheDevoted.Projectiles
{
    internal class ElectricSingularity : ModNPC
    {
        private float _scale;
        private Vector2[] _lightningZaps;
        private ref float Timer => ref NPC.ai[0];
        private ref float AttackTimer => ref NPC.ai[1];
        public CommonLightning Lightning { get; set; } = new CommonLightning();
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Main.npcFrameCount[Type] = 4;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            _lightningZaps = new Vector2[4];
            NPC.width = 49;
            NPC.height = 49;
            NPC.lifeMax = 1000;
            NPC.damage = 10;
            NPC.noTileCollide = true;
            NPC.dontTakeDamage = true;
            NPC.noGravity = true;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawOrigin = texture.Size() / 2f;
            float drawRotation = NPC.rotation;
            float drawScale = _scale;

            Vector2 drawPos = NPC.Center - Main.screenPosition;
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            for (int i = 0; i < 16; i++)
            {
                Vector2 flameDrawPos = drawPos + Main.rand.NextVector2Circular(8, 8);
                float rot = Main.rand.NextFloat(0f, 3.14f);

                spriteBatch.Draw(texture, flameDrawPos, NPC.frame, drawColor * 0.5f, drawRotation + rot, NPC.frame.Size() / 2f,
                    drawScale * VectorHelper.Osc(0.95f, 2f, speed: 6, offset: i), SpriteEffects.None, 0);
            }

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            for (int i = 0; i < 32; i++)
            {
                Vector2 flameDrawPos = drawPos + Main.rand.NextVector2Circular(2, 2);
                float rot = Main.rand.NextFloat(0f, 3.14f);

                spriteBatch.Draw(texture, flameDrawPos, NPC.frame, Color.Black, drawRotation + rot, NPC.frame.Size() / 2f,
                    drawScale * VectorHelper.Osc(0.85f, 0.95f, speed: 2, offset: i), SpriteEffects.None, 0);
            }


            Lightning.WidthMultiplier = 0.35f;
            for (int i = 0; i < Lightning.Trails.Length; i++)
            {
                var trail = Lightning.Trails[i];
                trail.PrimaryColor = Color.Black;
                trail.NoiseColor = Color.Black;
            }
            Lightning.DrawAlpha(spriteBatch, _lightningZaps, null);
            return false;
        }

        private int _frame;
        public override void FindFrame(int frameHeight)
        {
            base.FindFrame(frameHeight);

            //Animation Speed
            NPC.frameCounter += 0.5f;
            if (NPC.frameCounter >= 1f)
            {
                _frame++;
                NPC.frameCounter = 0f;

            }
            if(_frame >= 4)
            {
                _frame = 0;
            }
            NPC.frame.Y = frameHeight * _frame;
        }
        public override void AI()
        {
            base.AI();
            AttackTimer++;
            if (AttackTimer % 4 == 0)
            {
                Vector2 dustSpawnPoint = NPC.Center + Main.rand.NextVector2CircularEdge(64, 64);
                Vector2 dustVelocity = (NPC.Center - dustSpawnPoint).SafeNormalize(Vector2.Zero);
                dustVelocity *= 4;
                float progress = AttackTimer / 80f;

                Dust d = Dust.NewDustPerfect(dustSpawnPoint, DustID.GoldCoin, Velocity: dustVelocity, Scale: progress * 1f);
                d.noGravity = true;
            }

            Player playerToTarget = PlayerHelper.FindClosestPlayer(NPC.position, 1024);
            if (AttackTimer >= 60)
            {
      
                if(playerToTarget != null)
                {
                    Vector2 velToPlayer = (playerToTarget.Center - NPC.Center).SafeNormalize(Vector2.Zero);
                    velToPlayer *= 9;
                    if(StellaMultiplayer.IsHost)
                    {
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, velToPlayer,
                            ModContent.ProjectileType<ElectricSingularityBolt>(), 21, 1, Owner: Main.myPlayer);
                    }
                }

                AttackTimer = 0;
            }

            //Some interesting movement code for the singularity
            if(playerToTarget != null)
            {
                float diffX = playerToTarget.Center.X - NPC.Center.X;
                NPC.velocity.X = diffX * 0.03f;
            }
       
            NPC.velocity.Y = MathF.Sin(Timer * 0.05f) * 2;

            Timer++;
            if (Timer == 1)
            {
                SoundStyle soundStyle = new SoundStyle("Stellamod/Assets/Sounds/StormDragon_Enrage");
                soundStyle.PitchVariance = 0.15f;
                SoundEngine.PlaySound(soundStyle, NPC.position);

                //Spawn Dust Circle
                for(int i = 0; i < 32; i++)
                {
                    float progress = (float)i / 32f;
                    float rot = progress * MathHelper.TwoPi;
                    Vector2 vel = rot.ToRotationVector2() * 8;
                    Dust.NewDustPerfect(NPC.Center, DustID.GoldCoin, vel);
                }
            }

            if (Timer % 3 == 0)
            {
                for (int i = 0; i < _lightningZaps.Length; i++)
                {
                    float progress = (float)i / (float)_lightningZaps.Length;
                    float rot = progress * MathHelper.TwoPi * 1 + (Timer * 0.05f);
                    Vector2 offset = rot.ToRotationVector2() * MathF.Sin(Timer * 8 * i) * MathF.Sin(Timer * i) * VectorHelper.Osc(64, 80, speed: 3);
                    _lightningZaps[i] = NPC.Center + offset;
                }

                Lightning.RandomPositions(_lightningZaps);
            }

            if (Timer % 12 == 0)
            {
                Vector2 vel = Vector2.Zero;
                Dust d = Dust.NewDustPerfect(NPC.Center, DustID.GoldCoin, vel, Scale: 1);
                d.noGravity = true;
            }

            if (Timer % 6 == 0)
            {
                Vector2 vel = Vector2.Zero;
                Dust d = Dust.NewDustPerfect(NPC.Center + Main.rand.NextVector2Circular(8, 8), DustID.GoldCoin, vel, Scale: 1);
                d.noGravity = true;
            }

            if (Timer <= 15)
            {
                _scale = MathHelper.Lerp(0f, Main.rand.NextFloat(1f, 1.4f), Easing.InCubic(Timer / 15f));
            }

            if(Timer > 400)
            {
                _scale *= 0.98f;
            }

            if(Timer >= 440)
            {
                NPC.Kill();
            }
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            base.OnHitPlayer(target, info);

            SoundStyle zapSound = SoundID.DD2_LightningBugZap;
            zapSound.PitchVariance = 0.5f;
            SoundEngine.PlaySound(zapSound, target.Center);
        }

        public override void OnKill()
        {
            base.OnKill();
            for (int i = 0; i < 16; i++)
            {
                float progress = (float)i / 16f;
                float rot = progress * MathHelper.TwoPi;
                Vector2 vel = rot.ToRotationVector2() * 2;
                Dust d = Dust.NewDustPerfect(NPC.Center + Main.rand.NextVector2Circular(8, 8), DustID.GoldCoin, vel, Scale: 1);
                d.noGravity = true;
            }
        }
    }
}
