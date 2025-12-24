using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Core.Particles;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Items.Materials;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.WondrousDarkspace.NPCsWD
{
    public class HangingCageExplosionProjectile : ModProjectile
    {
        private ref float Timer => ref Projectile.ai[0];
        public override string Texture => TextureRegistry.EmptyTexture;
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 80;
            Projectile.height = 80;
            Projectile.hostile = true;
            Projectile.timeLeft = 4;
            Projectile.friendly = false;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
        }

        public override void AI()
        {
            base.AI();
            Timer++;
            if (Timer == 1)
            {
                //EXPLODE
                float boomSize = Main.rand.NextFloat(0.15f, 0.2f);
                FXUtil.GlowCircleBoom(Projectile.Center,
                    innerColor: Color.White,
                    glowColor: Color.LightBlue,
                    outerGlowColor: Color.Blue, duration: 25, baseSize: boomSize);

                FXUtil.ShakeCamera(Projectile.position, 1024, 32);

                SoundStyle explosionSound = new SoundStyle("Stellamod/Assets/Sounds/ExplosionCrystalShard");
                explosionSound.PitchVariance = 0.2f;
                SoundEngine.PlaySound(explosionSound, Projectile.position);

                for (float i = 0; i < 4; i++)
                {
                    float progress = i / 4f;
                    float rot = progress * MathHelper.ToRadians(360);
                    rot += Main.rand.NextFloat(-0.5f, 0.5f);
                    Vector2 offset = rot.ToRotationVector2() * 24;
                    var particle = FXUtil.GlowCircleLongBoom(Projectile.Center,
                        innerColor: Color.White,
                        glowColor: Color.LightBlue,
                        outerGlowColor: Color.DarkBlue,
                        baseSize: Main.rand.NextFloat(0.1f, 0.2f),
                        duration: Main.rand.NextFloat(15, 25));
                    particle.Rotation = rot + MathHelper.ToRadians(45);
                }


                for (float f = 0f; f < 16; f++)
                {
                    float rot = Main.rand.NextFloat(0f, 1f) * MathHelper.TwoPi;
                    Vector2 velocity = rot.ToRotationVector2() * Main.rand.NextFloat(3f, 8f);
                    Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowDust>(), velocity, newColor: Color.LightCyan, Scale: Main.rand.NextFloat(0.5f, 1f));
                }
            }
        }
    }

    public class HangingCage : ModNPC
    {
        private int _mothFrame;
        private ref float Timer => ref NPC.ai[0];
        private enum AIState
        {
            Idle,
            Exploding
        }
        private AIState State
        {
            get => (AIState)NPC.ai[1];
            set => NPC.ai[1] = (float)value;
        }

        private ref float HangingDistance => ref NPC.ai[2];
        private bool IsSmall
        {
            get => NPC.ai[3] == 1;
            set => NPC.ai[3] = value ? 1 : -1;
        }

        private Vector2 _scale;
        private Color OutlineColor;
        private void SwitchState(AIState state)
        {
            if (MultiplayerHelper.IsHost)
            {
                Timer = 0;
                State = state;
                NPC.netUpdate = true;
            }
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            _scale = Vector2.One;
            NPC.width = 32;
            NPC.height = 32;
            NPC.lifeMax = 100;
            NPC.defense = 4;
            NPC.noTileCollide = true;
            NPC.noGravity = true;
            NPC.knockBackResist = 0f;
            NPC.HitSound = SoundID.DD2_WitherBeastCrystalImpact;
            NPC.damage = 12;
        }

        public override void AI()
        {
            base.AI();
            NPC.rotation = MathHelper.Lerp(-0.05f, 0.05f, ExtraMath.Osc(0f, 1f, offset: NPC.whoAmI));
            Lighting.AddLight(NPC.Center, Color.White.ToVector3() * 0.6f);
            switch (State)
            {
                case AIState.Idle:
                    AI_Idle();
                    break;
                case AIState.Exploding:
                    AI_Exploding();
                    break;
            }
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            return false;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Vector2 drawPos = NPC.position - screenPos + NPC.Size / 2 + new Vector2(0f, NPC.gfxOffY);
            Texture2D chainTexture = ModContent.Request<Texture2D>(Texture + "_Chain").Value;
            float length = HangingDistance;
            int index = 0;
            while (length > 0f)
            {
                Vector2 chainDrawOrigin = chainTexture.Size() / 2;
                Vector2 offset = -Vector2.UnitY * chainTexture.Height * index;
                Vector2 vel = (NPC.rotation + MathHelper.PiOver2).ToRotationVector2();
                spriteBatch.Draw(chainTexture, drawPos + offset - vel * NPC.frame.Height / 2, null, Color.White.MultiplyRGB(drawColor),
                    NPC.rotation, chainDrawOrigin, _scale, SpriteEffects.None, 0);

                length -= chainTexture.Height;
                index++;
            }

            Vector2 drawOrigin = NPC.frame.Size() / 2;

            SpriteEffects spriteEffects = NPC.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            string texturePath = IsSmall ? Texture + "_Small" : Texture;
            Texture2D texture = ModContent.Request<Texture2D>(texturePath).Value;


            this.DrawOutline(OutlineColor, yOffset: 0, _scale, texturePath, null);
            Texture2D mothTexture = ModContent.Request<Texture2D>(Texture + "_Moth").Value;
            Rectangle mothFrame = mothTexture.GetFrame(_mothFrame, 4);
            Vector2 hoverOffset = Vector2.Lerp(Vector2.Zero, Vector2.UnitY * 8, ExtraMath.Osc(0f, 1f, speed: 4, offset: NPC.whoAmI));
            spriteBatch.Draw(mothTexture, drawPos + new Vector2(0, 8) + hoverOffset, mothFrame, Color.White.MultiplyRGB(drawColor), NPC.rotation, mothFrame.Size() / 2f, _scale, spriteEffects, 0);
            spriteBatch.Draw(texture, drawPos, null, Color.White.MultiplyRGB(drawColor), NPC.rotation, texture.Size() / 2f, _scale, spriteEffects, 0);


            Texture2D texture2D4 = ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/DimLight").Value;
            Main.spriteBatch.Draw(texture2D4, NPC.Center - Main.screenPosition, null, new Color(255, 128, 125, 0), NPC.rotation, new Vector2(32, 32), 0.17f * (7 + 0.6f), SpriteEffects.None, 0f);

            Lighting.AddLight(NPC.Center, Color.Yellow.ToVector3() * 1.0f * Main.essScale);
            return false;
        }
        private void AI_Idle()
        {
            Timer++;
            if (Main.rand.NextBool(25))
            {
                float xRand = Main.rand.NextFloat(-32, 32);
                float yRand = Main.rand.NextFloat(-32, 32);
                LegacyParticle.NewParticle<StarParticle>(NPC.Center + new Vector2(xRand, yRand), Vector2.Zero);
            }

            if (Timer % 10 == 0)
            {
                _mothFrame++;
                if (_mothFrame >= 4f)
                {
                    _mothFrame = 0;
                }
            }

            if (Timer == 1)
            {
                if (MultiplayerHelper.IsHost)
                {
                    float targetBeamLength = ProjectileHelper.PerformBeamHitscan(NPC.Center, -Vector2.UnitY, 1200);
                    NPC.Center = NPC.Center - Vector2.UnitY * targetBeamLength;

                    HangingDistance = Main.rand.NextFloat(300, 650);
                    NPC.Center += Vector2.UnitY * HangingDistance;
          
                    if (Main.rand.NextBool(3))
                    {
                        IsSmall = true;
                        NPC.netUpdate = true;
                    }
                }

       
            }

            NPC.TargetClosest();
            if (NPC.HasValidTarget)
            {
                Player target = Main.player[NPC.target];
                float distanceToTarget = Vector2.Distance(NPC.Center, target.Center);
                if (distanceToTarget <= 96)
                {
                    SwitchState(AIState.Exploding);
                }
            }
        }

        private void AI_Exploding()
        {
            Timer++;
            if (Timer == 1)
            {

                SoundStyle windUpSound = new SoundStyle("Stellamod/Assets/Sounds/StormDragon_WaveCharge");
                windUpSound.PitchVariance = 0.2f;
                SoundEngine.PlaySound(windUpSound, NPC.position);

            }
            OutlineColor = Color.Lerp(OutlineColor, Color.Yellow, 0.1f);
            float interpolant = Timer / 60f;
            if (Timer >= 60f)
            {
                if (MultiplayerHelper.IsHost)
                {
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero,
                        ModContent.ProjectileType<HangingCageExplosionProjectile>(), 37, 2, Main.myPlayer);
                }
                NPC.Kill();
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            base.ModifyNPCLoot(npcLoot);
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<HypnotizedSoul>(), minimumDropped: 2, maximumDropped: 4));
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (!spawnInfo.Player.GetModPlayer<MyPlayer>().ZoneWonder)
                return 0;
            return ScarletSpawnChance.Wondrous_Spawn_Rate;
        }
    }
}
