using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary;
using ReLogic.Content;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Items.Accessories.Brooches;
using Stellamod.Items.Special.MinerLogs;
using Stellamod.Items.Weapons.Mage;
using Stellamod.Items.Weapons.Ranged;
using Stellamod.Items.Weapons.Summon;
using Stellamod.Particles;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;

namespace Stellamod.NPCs.Underground
{
    internal class JellyGlow : ModNPC
    {
        private enum ActionState
        {
            Friendly,
            Angry
        }

        const float Movement_Osc_Time = 120;

        private Vector2 _wanderDirection;
        private float _wanderTimer;

        ref float Timer => ref NPC.ai[0];
        ref float TimerDirection => ref NPC.ai[1];
        ActionState State
        {
            get
            {
                return (ActionState)NPC.ai[2];
            }
            set
            {
                NPC.ai[2] = (float)value;
            }
        }
        bool Pregnate;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 12;
            NPCID.Sets.TrailCacheLength[NPC.type] = 90;
        }

        public override void SetDefaults()
        {
            NPC.lifeMax = 1000;
            NPC.width = 79;
            NPC.height = 140;
            NPC.damage = 150;
            NPC.defense = 4;
            NPC.knockBackResist = 0;
            NPC.noGravity = true;
            NPC.aiStyle = -1;
            NPC.scale = 2;
            NPC.noTileCollide = true;
            NPC.HitSound = SoundID.NPCHit25; // The sound the NPC will make when being hit.
            NPC.DeathSound = new SoundStyle("Stellamod/Assets/Sounds/Morrowsc1");
        }

     

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            target.AddBuff(BuffID.Electrified, 180);
        }

        public override void AI()
        {

            UpdateFrame(0.8f, 1, 60);

            if (StellaMultiplayer.IsHost && !Pregnate)
            {
                int range = 256;
                for(int i = 0; i < Main.rand.Next(5, 13); i++)
                {
                    int x = (int)NPC.Center.X + Main.rand.Next(-range, range);
                    int y = (int)NPC.Center.Y + Main.rand.Next(-range, range);
                    NPC.NewNPC(NPC.GetSource_FromThis(), x, y, 
                        ModContent.NPCType<BabyJellyGlow>(), 
                        ai0: NPC.whoAmI, 
                        ai1: Main.rand.NextFloat(0.00f, 1.00f));
                }

                Pregnate = true;
            }

            OscillateTimer();
            switch (State)
            {
                case ActionState.Friendly:
                    Friendly();
                    break;
                case ActionState.Angry:
                    Angry();
                    break;
            }

            Visuals();
        }

        private void Visuals()
        {
            Vector2 drawPos = NPC.Center + new Vector2(0, -24);
            if (Main.rand.NextBool(2))
            {
                Particle p = ParticleManager.NewParticle(drawPos, new Vector2(0, 7).RotatedByRandom(MathHelper.PiOver4  / 2), ParticleManager.NewInstance<ShadeParticle>(),
                          Color.White,  2f);
                p.layer = Particle.Layer.BeforeNPCs;

            }
            if (Main.rand.NextBool(16))
            {
                Dust.NewDustPerfect(drawPos, ModContent.DustType<GlowDust>(), new Vector2(0, 7).RotatedByRandom(MathHelper.PiOver2), 0, Color.Lerp(Color.Purple, Color.Black, 0.5f), 2f); ;
            }

            // Some visuals here
            Lighting.AddLight(NPC.Center + new Vector2(0, -64), Color.White.ToVector3() * 0.28f);
        }

        private void OscillateTimer()
        {
            if (TimerDirection <= -1)
            {
                Timer--;
                if (Timer < 0)
                {
                    TimerDirection = 1;
                }
            }
            else
            {
                Timer++;
                if (Timer > Movement_Osc_Time)
                {
                    TimerDirection = -1;
                }
            }
        }

        private void Friendly()
        {
            if(_wanderDirection == Vector2.Zero)
            {
                _wanderDirection = Vector2.UnitX;
            }

            _wanderTimer++;
            if (_wanderTimer >= 60)
            {
                _wanderDirection = _wanderDirection.RotatedBy(MathHelper.PiOver4);
                _wanderTimer = 0;
            }

            Vector2 targetVelocity = _wanderDirection * 1.25f;


            //Move to the player
            float progress = Timer / Movement_Osc_Time;
            float easedProgress = Easing.InOutCirc(progress);
            Vector2 smoothVelocity = targetVelocity * easedProgress;
           // NPC.velocity = smoothVelocity;
            NPC.velocity.Y = MathHelper.Lerp(-0.5f, 0.5f, easedProgress);
        }

        private void Angry()
        {
            NPC.TargetClosest();
            Player target = Main.player[NPC.target];

            //Get the direction to the player
            Vector2 targetDirection = NPC.Center.DirectionTo(target.Center);
            Vector2 targetVelocity = targetDirection * 2;


            //Move to the player
            float progress = Timer / Movement_Osc_Time;
            float easedProgress = Easing.InOutCirc(progress);
            Vector2 smoothVelocity = targetVelocity * easedProgress;
            NPC.velocity = smoothVelocity;
            NPC.velocity.Y += MathHelper.Lerp(-0.5f, 0.5f, easedProgress);
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            //You can't be in the surface and underground at the same time so this should work
            //0.05f should make it 20 less common than normal spawns.
            return (SpawnCondition.Cavern.Chance * SpawnRates.Flower_Spawn_Chance);
        }

        public override void ModifyIncomingHit(ref NPC.HitModifiers modifiers)
        {
            base.ModifyIncomingHit(ref modifiers);
            State = ActionState.Angry;
        }
        public void UpdateFrame(float speed, int minFrame, int maxFrame)
        {
            trueFrame += speed;
            if (trueFrame < minFrame)
            {
                trueFrame = minFrame;
            }
            if (trueFrame > maxFrame)
            {
                trueFrame = minFrame;
            }
        }
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.OneFromOptions(1,
                ModContent.ItemType<JellyBow>(),
                ModContent.ItemType<JellyStaff>(),
                   ModContent.ItemType<VeiledScriptureMiner9>()));
        }
      

        float trueFrame = 0;
        public override void FindFrame(int frameHeight)
        {
            NPC.frame.Width = 79;
            NPC.frame.X = ((int)trueFrame % 5) * NPC.frame.Width;
            NPC.frame.Y = (((int)trueFrame - ((int)trueFrame % 5)) / 5) * NPC.frame.Height;
        }


        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            SpriteEffects effects = NPC.direction == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            Texture2D texture2 = ModContent.Request<Texture2D>(Texture).Value;

            spriteBatch.Draw(texture2, new Vector2(NPC.Center.X, NPC.Center.Y) - Main.screenPosition + new Vector2(0, NPC.gfxOffY), NPC.frame, new Color(255, 255, 255, 0) * (1f - NPC.alpha / 80f), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);



            Texture2D texture = ModContent.Request<Texture2D>("Stellamod/Effects/Masks/DimLight").Value;
            Vector3 huntrianColorXyz = DrawHelper.HuntrianColorOscillate(
                    Color.Purple.ToVector3(),
                    Color.Violet.ToVector3(),
                    new Vector3(2, 2, 2), 0);

            Vector2 drawPos = NPC.Center - Main.screenPosition + new Vector2(0, -64);
            for (int i = 0; i < 4; i++)
            {
                Main.spriteBatch.Draw(texture, drawPos, null, new Color((int)(huntrianColorXyz.X * 1), (int)(huntrianColorXyz.Y * 1), (int)(huntrianColorXyz.Z * 1), 0), 
                    NPC.rotation, new Vector2(32, 32), 0.66f * (7 + 0.6f), SpriteEffects.None, 0f);
            }

            Lighting.AddLight(drawPos, Color.Purple.ToVector3() * 1.0f * Main.essScale);
            return false;
        }

        Vector2 Drawoffset => new Vector2(0, NPC.gfxOffY) + Vector2.UnitX * NPC.spriteDirection * 0;
        public virtual string GlowTexturePath => Texture + "_Glow";
        private Asset<Texture2D> _glowTexture;
        public Texture2D GlowTexture => (_glowTexture ??= (ModContent.RequestIfExists<Texture2D>(GlowTexturePath, out var asset) ? asset : null))?.Value;
        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            float num108 = 4;
            float num107 = (float)Math.Cos((double)(Main.GlobalTimeWrappedHourly % 1.4f / 1.4f * 6.28318548f)) / 2f + 0.5f;
            float num106 = 0f;
            Color color1 = Color.White * num107 * .8f;
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
    }
}
