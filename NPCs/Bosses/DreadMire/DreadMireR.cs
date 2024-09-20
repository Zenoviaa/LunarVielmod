using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Helpers;
using Stellamod.Items.Accessories.Brooches;
using Stellamod.Items.Consumables;
using Stellamod.Items.Materials;
using Stellamod.Items.Weapons.Mage;
using Stellamod.Items.Weapons.Melee;
using Stellamod.Items.Weapons.Ranged;
using Stellamod.NPCs.Bosses.DreadMire.Heart;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Bosses.DreadMire
{
    [AutoloadBossHead]
    internal class DreadMireR : ModNPC
    {
        private float _teleportX;
        private float _teleportY;
        private bool _resetTimers;
        private enum ActionState
        {
            Spawn,
            DeSpawn,
            Idle,
            Teleport,
            Charge,
            Dash,
            Heart_Phase,
            Summon_Skulls,
            Shoot_Bombs,
            Laser_Rain,
            Final_Laser,
      
        }


        private enum AnimationState
        {
            Idle,
            HandUp,
            Flame,
            Flame2,
            HandDown,
            TwoHandsUp,
            TwoHandsUpIdle,
            ForceHandDown,
            PowerUp
        }

        //Animation Stuff
        private AnimationState Animation;
        private Rectangle DrawRectangle;
        private int frameCounter;
        private int frameTick;
        private float hoverY;

        //AI
        private ActionState State
        {
            get => (ActionState)NPC.ai[0];
            set => NPC.ai[0] = (float)value;
        }
        private ActionState PreviousState
        {
            get => (ActionState)NPC.ai[1];
            set => NPC.ai[1] = (float)value;
        }
        private ref float Timer => ref NPC.ai[2];
        private bool _invincible;
        private bool _phase2Special;
        private bool _phase3Special;
        private bool InPhase2 => NPC.life < NPC.lifeMax * 0.6f;
        private bool InPhase3 => NPC.life < NPC.lifeMax * 0.3f;

   
        private float LaserTimer;
        private Player Target => Main.player[NPC.target];
        private Vector2 OldTargetPos;

        //Damage Values
        private int DreadFireBombDamage => 31;
        private int BoltDashDamage => 50;
        private int DreadSineDamage => 20;
        private int FinalLaserDamage => 500;
        private int FinalLaserRedSkullDamage => 38;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 48;
            NPCID.Sets.MPAllowedEnemies[NPC.type] = true;
            NPCID.Sets.TrailCacheLength[NPC.type] = 12;
            NPCID.Sets.TrailingMode[NPC.type] = 4;


            NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                CustomTexturePath = "Stellamod/NPCs/Bosses/DreadMire/DreadMireBestiary",
                PortraitScale = 0.8f, // Portrait refers to the full picture when clicking on the icon in the bestiary
                PortraitPositionYOverride = 0f,
            };

            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            // We can use AddRange instead of calling Add multiple times in order to add multiple items at once
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				// Sets the preferred biomes of this town NPC listed in the bestiary.
				// With Town NPCs, you usually set this to what biome it likes the most in regards to NPC happiness.
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,

				// Sets your NPC's flavor text in the bestiary.
				new FlavorTextBestiaryInfoElement(LangText.Bestiary(this, "Traded her singularity in turn for more dreadful power, ridden of anxiety and all things dreadful")),

				// You can add multiple elements if you really wanted to
				// You can also use localization keys (see Localization/en-US.lang)
				new FlavorTextBestiaryInfoElement(LangText.Bestiary(this, "Cozmire- Dreadmire Veil??", "2"))
            });
        }

        public override void SetDefaults()
        {
            NPC.noGravity = true;
            NPC.lifeMax = 2300;
            NPC.defense = 9;
            NPC.damage = 1;
            NPC.value = Item.buyPrice(gold: 4);
            NPC.knockBackResist = 0f;
            NPC.width = 30;
            NPC.height = 40;
            NPC.scale = 1f;
            NPC.lavaImmune = false;
            NPC.alpha = 0;
            NPC.boss = true;
            NPC.noTileCollide = true;
            NPC.npcSlots = 10f;
            NPC.HitSound = SoundID.NPCHit9;
            NPC.DeathSound = SoundID.NPCDeath23;
            NPC.aiStyle = -1;
            NPC.BossBar = ModContent.GetInstance<DreadBossBar>();
            Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/DreadmireV2");
        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * balance);
        }

        public void ResetTimers()
        {
            if (StellaMultiplayer.IsHost)
            {
                _resetTimers = true;
                NPC.netUpdate = true;
            }
        }

        private void SetInvincible(bool invincibleState)
        {
            if(invincibleState && !_invincible && StellaMultiplayer.IsHost)
            {
                _invincible = true;
                NPC.netUpdate = true;
            }

            if(!invincibleState && _invincible && StellaMultiplayer.IsHost)
            {
                _invincible = false;
                NPC.netUpdate = true;
            }
        }

        private void ResetState(ActionState state)
        {
            if(State != ActionState.Teleport && State != ActionState.Idle && State != ActionState.DeSpawn)
            {
                PreviousState = State;
            }

            State = state;
            NPC.netUpdate = true;
            ResetTimers();
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            //No contact damage return false here 
            return false;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(_resetTimers);
            writer.Write(_invincible);
            writer.Write(_teleportX);
            writer.Write(_teleportY);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            _resetTimers = reader.ReadBoolean();
            _invincible = reader.ReadBoolean();
            _teleportX = reader.ReadSingle();
            _teleportY = reader.ReadSingle();
        }

        private void Animate(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor, int startFrame, int endFrame, int frameTime)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            SpriteEffects effects = SpriteEffects.None;
            if (NPC.direction == 1)
            {
                effects = SpriteEffects.FlipHorizontally;
            }

            int width = 114;
            int height = 124;
            int frameCount = (endFrame - startFrame) + 1;
            hoverY = VectorHelper.Osc(0, -4, 3);

            Rectangle rect = new Rectangle(0, startFrame * height, width, frameCount * height);
            Vector2 frameSize = new Vector2(width, height);
            Vector2 drawOrigin = frameSize / 2;
            Vector2 hoverOffset = new Vector2(0, hoverY);
            Vector2 extraOffset = new Vector2(0, -4);

            drawColor *= NPC.Opacity;
            DrawRectangle = texture.AnimationFrame(ref frameCounter, ref frameTick, frameTime, frameCount, rect);
            spriteBatch.Draw(texture, NPC.position - screenPos + (NPC.Size / 2) + hoverOffset + extraOffset, DrawRectangle, drawColor,
                NPC.rotation, drawOrigin, NPC.scale, effects, 0f);
        }

        private void PreDrawAfterImage(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
        {
            SpriteEffects Effects = NPC.direction == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;

            Vector2 frameOrigin = DrawRectangle.Size();
            Vector2 offset = new Vector2(NPC.width - frameOrigin.X - 15, NPC.height - DrawRectangle.Height + 8);
            Vector2 DrawPos = NPC.position - screenPos + frameOrigin + offset;

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
                spriteBatch.Draw(texture, DrawPos + new Vector2(0f, 1).RotatedBy(radians) * time,DrawRectangle, new Color(99, 39, 51, 0), 0, frameOrigin, NPC.scale, Effects, 0);
            }

            for (float i = 0f; i < 1f; i += 0.34f)
            {
                float radians = (i + timer) * MathHelper.TwoPi;
                spriteBatch.Draw(texture, DrawPos + new Vector2(0f, 1).RotatedBy(radians) * time, DrawRectangle, new Color(255, 8, 55, 0), 0, frameOrigin, NPC.scale, Effects, 0);
            }

            Lighting.AddLight(NPC.Center, Color.DarkRed.ToVector3() * 2.25f * Main.essScale);

            spriteBatch.Draw((Texture2D)TextureAssets.Npc[NPC.type], NPC.Center - Main.screenPosition, DrawRectangle, NPC.GetAlpha(lightColor), NPC.rotation, DrawRectangle.Size() / 2f, NPC.scale, Effects, 0f);
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            for (int k = 0; k < NPC.oldPos.Length; k++)
            {
                Vector2 drawPos = NPC.oldPos[k] - Main.screenPosition + NPC.Size / 2 + new Vector2(0f, NPC.gfxOffY);
                Color color = NPC.GetAlpha(Color.Lerp(new Color(255, 8, 55), new Color(99, 39, 51), 1f / NPC.oldPos.Length * k) * (1f - 1f / NPC.oldPos.Length * k));
                spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, drawPos, new Microsoft.Xna.Framework.Rectangle?(DrawRectangle), color, NPC.rotation, DrawRectangle.Size() / 2, NPC.scale, Effects, 0f);
            }

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            PreDrawAfterImage(spriteBatch, screenPos, drawColor);
            int frameTime = 8;
            int startFrame;
            int endFrame;
            switch (Animation)
            {
                default:
                case AnimationState.Idle:
                    startFrame = 0;
                    endFrame = 6;
                    break;
                case AnimationState.PowerUp:
                    frameTime = 4;
                    startFrame = 44;
                    endFrame = 47;
                    break;
                case AnimationState.HandUp:
                    frameTime = 4;
                    startFrame = 7;
                    endFrame = 12;
                    break;
                case AnimationState.Flame:
                    frameTime = 4;
                    startFrame = 13;
                    endFrame = 16;
                    break;
                case AnimationState.TwoHandsUpIdle:
                    frameTime = 4;
                    startFrame = 33;
                    endFrame = 36;
                    break;
                case AnimationState.ForceHandDown:
                    frameTime = 4;
                    startFrame = 37;
                    endFrame = 43;
                    break;
            }
            Animate(spriteBatch, screenPos, drawColor, startFrame, endFrame, frameTime);
            return false;
        }

        public override void AI()
        {
            //Reset Timers, we do it this way so it's netcoded nicely.
            if (_resetTimers)
            {
                Timer = 0;
                frameCounter = 0;
                frameTick = 0;
                _resetTimers = false;
            }

            if (_teleportX != 0 || _teleportY != 0)
            {
                NPC.position.X = _teleportX;
                NPC.position.Y = _teleportY;
                NPC.velocity.X = 0f;
                NPC.velocity.Y = 0f;
                _teleportX = 0f;
                _teleportY = 0f;
            }

            //Select music
            if (NPC.AnyNPCs(ModContent.NPCType<DreadMiresHeart>()))
            {
                Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/DreadHeart");
            }
            else
            {
                Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/DreadmireV2");
            }

            NPC.dontCountMe = _invincible;
            NPC.dontTakeDamage = _invincible;
            NPC.TargetClosest();
            if (!NPC.HasValidTarget && State != ActionState.DeSpawn)
            {
                ResetState(ActionState.DeSpawn);
                return;
            }

            if (InPhase3 && State != ActionState.Final_Laser)
            {
                AI_RandomLasers();
            }

            if (InPhase2 && !_phase2Special && State != ActionState.Heart_Phase)
            {
                ResetState(ActionState.Heart_Phase);
                _phase2Special = true;
                return;
            }
           
            if (InPhase3 && !_phase3Special && State != ActionState.Heart_Phase)
            {
                ResetState(ActionState.Heart_Phase);
                _phase3Special = true;
                return;
            }

            switch (State)
            {
                case ActionState.Spawn:
                    AI_Spawn();
                    break;
                case ActionState.DeSpawn:
                    AI_Despawn();
                    break;
                case ActionState.Idle:
                    AI_Idle();
                    break;
                case ActionState.Teleport:
                    AI_Teleport();
                    break;
                case ActionState.Charge:
                    AI_Charge();
                    break;
                case ActionState.Dash:
                    AI_Dash();
                    break;
                case ActionState.Heart_Phase:
                    AI_HeartPhase();
                    break;
                case ActionState.Summon_Skulls:
                    AI_SummonSkulls();
                    break;
                case ActionState.Shoot_Bombs:
                    AI_ShootBombs();
                    break;
                case ActionState.Laser_Rain:
                    AI_LaserRain();
                    break;
                case ActionState.Final_Laser:
                    AI_FinalLaser();
                    break;
            }
        }

        private void AI_Spawn()
        {
            Timer++;
            if (Timer == 1)
            {
                //Set alpha to 255 to be invisible
                NPC.alpha = 255;
            }

            //Fade in
            NPC.alpha -= 5;
            if (NPC.alpha <= 0)
            {
                NPC.alpha = 0;
            }

            //Rise up and then slow down
            if (Timer < 15)
            {
                NPC.velocity.Y -= 0.33f;
            }
            else
            {
                NPC.velocity *= 0.98f;
            }

            if (Timer == 89)
            {
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Dreadmire_Spawn2"), NPC.position);
                var entitySource = NPC.GetSource_FromThis();
                if (StellaMultiplayer.IsHost)
                {
                    NPC.NewNPC(entitySource, (int)NPC.Center.X, (int)NPC.Center.Y, 
                        ModContent.NPCType<DreadMirePentagramV2>());
                }
                Animation = AnimationState.PowerUp;
            }

            if (Timer > 90 && Timer < 200)
            {
                Main.bloodMoon = true;
                Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(NPC.Center, 512f, 32f);
                Dust dust = Dust.NewDustDirect(NPC.Center, NPC.width, NPC.height, DustID.Firework_Red);
                dust.velocity *= -1f;
                dust.scale *= .8f;
                dust.noGravity = true;
                Vector2 vector2_1 = new Vector2(Main.rand.Next(-80, 81), Main.rand.Next(-80, 81));
                vector2_1.Normalize();
                Vector2 vector2_2 = vector2_1 * (Main.rand.Next(50, 100) * 0.04f);
                dust.velocity = vector2_2;
                vector2_2.Normalize();
                Vector2 vector2_3 = vector2_2 * 34f;
                dust.position = NPC.Center - vector2_3;
            }

            if(Timer >= 200)
            {
                ResetState(ActionState.Teleport);
            }
        }

        private void AI_Despawn()
        {
            NPC.velocity.Y -= 0.2f;
            NPC.alpha += 5;
            NPC.EncourageDespawn(120);
        }

        private void AI_Idle()
        {         
            Timer++;
            if(Timer < 10)
            {
                NPC.velocity.Y -= 0.33f;
            }
            else
            {
                NPC.velocity.Y *= 0.92f;
            }

            if(Timer < 26)
            {
                NPC.alpha -= 10;
            }
    
            Animation = AnimationState.Idle;
            if(Timer >= 30)
            {
                List<ActionState> possibleAttacks = new List<ActionState>();
                switch (PreviousState)
                {
                    default:
                        possibleAttacks.Add(ActionState.Charge);
                        break;
                    case ActionState.Charge:
                        possibleAttacks.Add(ActionState.Dash);
                        possibleAttacks.Add(ActionState.Shoot_Bombs);
                        break;
                    case ActionState.Dash:
                        possibleAttacks.Add(ActionState.Summon_Skulls);
                        possibleAttacks.Add(ActionState.Shoot_Bombs);
                        break;
                    case ActionState.Shoot_Bombs:
                        possibleAttacks.Add(ActionState.Summon_Skulls);
                        if (InPhase2)
                        {
                            possibleAttacks.Add(ActionState.Laser_Rain);
                        }
                        break;
                    case ActionState.Summon_Skulls:
                        possibleAttacks.Add(ActionState.Charge);
                        if (InPhase3)
                        {
                            possibleAttacks.Add(ActionState.Final_Laser);
                        }
                        break;
                    case ActionState.Laser_Rain:
                        possibleAttacks.Add(ActionState.Charge);
                        possibleAttacks.Add(ActionState.Dash);
                        if (InPhase3)
                        {
                            possibleAttacks.Add(ActionState.Final_Laser);
                        }
                        break;
                }

                ResetState(possibleAttacks[Main.rand.Next(0, possibleAttacks.Count)]);
            }
        }

        private void AI_RandomLasers()
        {
            LaserTimer++;
            if(LaserTimer % 240 == 0 && StellaMultiplayer.IsHost)
            {
                Vector2 targetCenter = Target.Center;
                targetCenter.X += Main.rand.NextFloat(-600, 600);
                NPC.NewNPC(NPC.GetSource_FromThis(), (int)targetCenter.X, (int)targetCenter.Y,
                    ModContent.NPCType<DreadMireZapwarn>());
            }
        }

        private void AI_Charge()
        {
            Timer++;
            Animation = AnimationState.Flame;
            if (Timer == 1)
            {
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Dreadmire_Fire1"), NPC.position);
                float radius = 64;
                float rot = MathHelper.TwoPi / 3f;
                for (int I = 0; I < 3f; I++)
                {
                    if (StellaMultiplayer.IsHost)
                    {
                        Vector2 position = NPC.Center + radius * (I * rot).ToRotationVector2();
                        NPC.NewNPC(NPC.GetSource_FromAI(), (int)(position.X), (int)(position.Y),
                            ModContent.NPCType<DreadFireCircle>(), NPC.whoAmI, NPC.whoAmI, I * rot, radius);
                    }
                }
            }

            if(Timer > 10 && Timer < 180)
            {
                Vector2 directionToTarget = NPC.Center.DirectionTo(Target.Center);
                float minSpeed = 3;
                float maxSpeed = 6;
                float speed = MathHelper.Lerp(minSpeed, maxSpeed, Timer / 120);
                Vector2 velocityToTarget = directionToTarget * speed;
                NPC.velocity = Vector2.Lerp(NPC.velocity, velocityToTarget, 0.5f);
                NPC.rotation = NPC.velocity.X * 0.025f;
            }

            if(Timer > 180)
            {
                float speed = 2f;
                Vector2 directionToTarget = NPC.Center.DirectionTo(Target.Center);
                Vector2 velocityToTarget = directionToTarget * speed;
                NPC.velocity = Vector2.Lerp(NPC.velocity, velocityToTarget, 0.02f);
                NPC.rotation *= 0.94f;
            }

            if(Timer == 240)
            {
                ResetState(ActionState.Teleport);
            }
        }

        private void AI_Teleport()
        {
            Timer++;
            if(Timer == 1)
            {
                if (StellaMultiplayer.IsHost)
                {
                    NPC.NewNPC(NPC.GetSource_FromThis(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<DreadMirePentagramSmall>());
                }
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Dreadmire_TP_Out"), NPC.position);
            }
        
            if(Timer < 26)
            {
                NPC.alpha += 10;
            }

            if (NPC.alpha >= 255)
                NPC.alpha = 255;

            int waitTime = 52;
            if (InPhase2)
            {
                waitTime = 26;
            }

            if(Timer == waitTime)
            {
                if (StellaMultiplayer.IsHost)
                {
                    Vector2 randEdge = Main.rand.NextVector2CircularEdge(252, 252);
                    Vector2 targetPos = Target.Center + randEdge;
                    _teleportX = targetPos.X;
                    _teleportY = targetPos.Y;
                    NPC.netUpdate = true;
                }
                ResetState(ActionState.Idle);
            }

            NPC.rotation *= 0.9f;
            NPC.velocity *= 0.9f;
        }

        private void AI_Dash()
        {
            Animation = AnimationState.ForceHandDown;
            Timer++;
            if (Timer == 10)
            {
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Dreadmire__PreDash"), NPC.position);
                Vector2 direction = NPC.Center.DirectionTo(Target.Center) * 8.5f;
                NPC.velocity = -direction;
            }

            if (Timer == 25)
            {
                Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(base.NPC.Center, 2048f, 124f);
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Dreadmire__Dash"), NPC.position);
                Vector2 direction = Vector2.Normalize(Main.player[NPC.target].Center - NPC.Center) * 8.5f;
                NPC.alpha = 255;

                SoundEngine.PlaySound(SoundID.Item8, NPC.position);
                SoundEngine.PlaySound(SoundID.Zombie53, NPC.position);
                if (StellaMultiplayer.IsHost)
                {
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y, direction.X * 2, direction.Y * 2,
                        ModContent.ProjectileType<DreadMireDash>(), BoltDashDamage, 1, Owner: Main.myPlayer);
                }
            }

            if(Timer >= 55)
            {
                ResetState(ActionState.Teleport);
            }
        }

        private void AI_HeartPhase()
        {
            int dreadMiresHeartType = ModContent.NPCType<DreadMiresHeart>();
            Animation = AnimationState.TwoHandsUp;

            NPC.alpha += 5;
            NPC.velocity *= 0.9f;
            Timer++;
            if(Timer == 1)
            {
                SetInvincible(true);
                var entitySource = NPC.GetSource_FromThis();
                if (StellaMultiplayer.IsHost)
                {
                    NPC.NewNPC(entitySource, (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<DreadMirePentagramSmall>());
                }
            }

            if(Timer == 20)
            {
                if (StellaMultiplayer.IsHost)
                {
                    NPC.NewNPC(NPC.GetSource_FromThis(), (int)NPC.Center.X, (int)NPC.Center.Y, dreadMiresHeartType);
                }
            }

            if(Timer >= 60)
            {
                if (StellaMultiplayer.IsHost)
                {
                    Vector2 targetPos = Target.Center + new Vector2(0, 200);
                    _teleportX = targetPos.X;
                    _teleportY = targetPos.Y;
                    NPC.netUpdate = true;
                }
            }

            if(Timer >= 60 && !NPC.AnyNPCs(dreadMiresHeartType))
            {
                SetInvincible(false);
                ResetState(ActionState.Teleport);
            }
        }

        private void AI_SummonSkulls()
        {
            NPC.velocity *= 0.9f;
            Timer++;
            Animation = AnimationState.TwoHandsUpIdle;
            if (Timer == 1)
            {
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Dreadmire_Pentagram"), NPC.position);
                if (StellaMultiplayer.IsHost)
                {
                    var entitySource = NPC.GetSource_FromThis();
                    NPC.NewNPC(entitySource, (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<DreadMirePentagramV2>());
                }
            }

            if(Timer % 5 == 0)
            {
                int Sound = Main.rand.Next(1, 3);
                if (Sound == 1)
                {
                    SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Dreadmire_Pentagram_Skull1"), NPC.position);
                }
                else
                {
                    SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Dreadmire_Pentagram_Skull1"), NPC.position);
                }

                if (StellaMultiplayer.IsHost)
                {
                    Vector2 randOffset = Main.rand.NextVector2Circular(32, 32);
                    Vector2 randPos = NPC.Center + randOffset;
                    Vector2 velocity = randPos.DirectionTo(Target.Center) * 10;
                    velocity = velocity.RotatedByRandom(MathHelper.PiOver4 / 4f);
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), randPos, velocity,
                        ModContent.ProjectileType<DreadSineSkull>(), DreadSineDamage, 1, Main.myPlayer);
                }
            }


            if(Timer >= 100)
            {
                ResetState(ActionState.Teleport);
            }
        }

        private void AI_ShootBombs()
        {
            Timer++;
            Animation = AnimationState.Flame;
            NPC.rotation = NPC.velocity.X * 0.05f;
            if (Timer == 10 || Timer == 30 || Timer == 50)
            {
                int Sound = Main.rand.Next(1, 3);
                if (Sound == 1)
                {
                    SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Dreadmire_BoneSpawn1"), NPC.position);
                }
                else
                {
                    SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Dreadmire_BoneSpawn2"), NPC.position);
                }

                Vector2 recoilVelocity = Target.Center.DirectionTo(NPC.Center) * 8.5f;
                Vector2 direction = NPC.Center.DirectionTo(Target.Center) * 8.5f;
                NPC.velocity = recoilVelocity;
                if (StellaMultiplayer.IsHost)
                {
                    int amountOfProjectiles = Main.rand.Next(1, 3);
                    for (int i = 0; i < amountOfProjectiles; ++i)
                    {
                        float offsetX = Main.rand.Next(-200, 200) * 0.01f;
                        float offsetY = Main.rand.Next(-200, 200) * 0.01f;
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y, direction.X + offsetX, direction.Y + offsetY,
                            ModContent.ProjectileType<DreadFire>(), DreadFireBombDamage, 1, Owner: Main.myPlayer);
                    }
                }
            }

            NPC.velocity *= 0.92f;
            if (Timer >= 60)
            {
                ResetState(ActionState.Teleport);
            }
        }

        private void AI_LaserRain()
        {
            Timer++;
            Animation = AnimationState.TwoHandsUpIdle;
            if (Timer == 10)
            {
                if (StellaMultiplayer.IsHost)
                {
                    var entitySource = NPC.GetSource_FromThis();
                    Projectile.NewProjectile(entitySource, NPC.Center, new Vector2(0, 0),
                        ModContent.ProjectileType<DreadMireMagic>(), 19, 0, Owner: Main.myPlayer);
                }

                OldTargetPos.Y = Target.Center.Y;
                OldTargetPos.X = Target.Center.X;
            }

            if (Timer == 20)
            {
                if (StellaMultiplayer.IsHost)
                {
                    var entitySource = NPC.GetSource_FromThis();
                    NPC.NewNPC(entitySource, (int)OldTargetPos.X - 10, (int)OldTargetPos.Y, 
                        ModContent.NPCType<DreadMireZapwarn>());
                }
            }

            if (Timer == 30)
            {
                if (StellaMultiplayer.IsHost)
                {
                    var entitySource = NPC.GetSource_FromThis();
                    NPC.NewNPC(entitySource, (int)OldTargetPos.X - 200, (int)OldTargetPos.Y, 
                        ModContent.NPCType<DreadMireZapwarn>());
                    NPC.NewNPC(entitySource, (int)OldTargetPos.X + 200, (int)OldTargetPos.Y, 
                        ModContent.NPCType<DreadMireZapwarn>());
                }
            }

            if (Timer == 40)
            {
                if (StellaMultiplayer.IsHost)
                {
                    var entitySource = NPC.GetSource_FromThis();
                    NPC.NewNPC(entitySource, (int)OldTargetPos.X - 400, (int)OldTargetPos.Y, 
                        ModContent.NPCType<DreadMireZapwarn>());
                    NPC.NewNPC(entitySource, (int)OldTargetPos.X + 400, (int)OldTargetPos.Y, 
                        ModContent.NPCType<DreadMireZapwarn>());
                }
            }

            if (Timer == 50)
            {
                if (StellaMultiplayer.IsHost)
                {
                    var entitySource = NPC.GetSource_FromThis();
                    NPC.NewNPC(entitySource, (int)OldTargetPos.X - 600, (int)OldTargetPos.Y, 
                        ModContent.NPCType<DreadMireZapwarn>());
                    NPC.NewNPC(entitySource, (int)OldTargetPos.X + 600, (int)OldTargetPos.Y, 
                        ModContent.NPCType<DreadMireZapwarn>());
                }
            }

            if (Timer == 60)
            {
                if (StellaMultiplayer.IsHost)
                {
                    var entitySource = NPC.GetSource_FromThis();
                    NPC.NewNPC(entitySource, (int)OldTargetPos.X - 800, (int)OldTargetPos.Y, 
                        ModContent.NPCType<DreadMireZapwarn>());
                    NPC.NewNPC(entitySource, (int)OldTargetPos.X + 800, (int)OldTargetPos.Y, 
                        ModContent.NPCType<DreadMireZapwarn>());
                }
            }

            if(Timer >= 90)
            {
                ResetState(ActionState.Teleport);
            }
        }

        private void AI_FinalLaser()
        {
            Timer++;
            NPC.velocity *= 0.94f;
            Animation = AnimationState.PowerUp;

            if (Timer == 1)
            {
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Dreadmire_Spawn2"), NPC.position);
                var entitySource = NPC.GetSource_FromThis();
                if (StellaMultiplayer.IsHost)
                {
                    NPC.NewNPC(entitySource, (int)NPC.Center.X, (int)NPC.Center.Y, 
                        ModContent.NPCType<DreadMirePentagramV2>());
                }
                Animation = AnimationState.PowerUp;
            }

            if (Timer > 1 && Timer < 30)
            {
                Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(NPC.Center, 512f, 32f);
                Dust dust = Dust.NewDustDirect(NPC.Center, NPC.width, NPC.height, DustID.Firework_Red);
                dust.velocity *= -1f;
                dust.scale *= .8f;
                dust.noGravity = true;
                Vector2 vector2_1 = new Vector2(Main.rand.Next(-80, 81), Main.rand.Next(-80, 81));
                vector2_1.Normalize();
                Vector2 vector2_2 = vector2_1 * (Main.rand.Next(50, 100) * 0.04f);
                dust.velocity = vector2_2;
                vector2_2.Normalize();
                Vector2 vector2_3 = vector2_2 * 98;
                dust.position = NPC.Center - vector2_3;
            }

            if (Timer == 30)
            {
                if (StellaMultiplayer.IsHost)
                {
                    var entitySource = NPC.GetSource_FromThis();
                    for(int i = 0; i < 5; i++)
                    {
                        NPC.NewNPC(entitySource, (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<DreadMireZapwarn>());
                    }                    
                }
            }

            if (Timer == 130)
            {
                Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(base.NPC.Center, 2048f, 124f);
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Dreadmire__FinalBeam"));
                if (StellaMultiplayer.IsHost)
                {
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center - new Vector2(0, 900), new Vector2(0, 10),
                        ModContent.ProjectileType<FinalBeam>(), FinalLaserDamage, 0, Owner: Main.myPlayer, ai1: NPC.whoAmI);         
                }
            }
            if (Timer >= 200 && Timer <= 600)
            {
                if (Timer % 20 == 0)
                {
                    int Sound = Main.rand.Next(1, 3);
                    if (Sound == 1)
                    {
                        SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/ShadeHand"));
                    }
                    else
                    {
                        SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/ShadeHand2"));
                    }

                    var entitySource = NPC.GetSource_FromThis();
                    Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(NPC.Center, 2048f, 64f);
                    if (StellaMultiplayer.IsHost)
                    {
                        int OffSet = Main.rand.Next(-240, 240 + 1);
                        Vector2 NukePos;
                        NukePos.X = NPC.Center.X;
                        NukePos.Y = Target.Center.Y + OffSet;
                        Projectile.NewProjectile(entitySource, NukePos, new Vector2(15, 0),
                            ModContent.ProjectileType<RedSkull>(), FinalLaserRedSkullDamage, 0, Owner: Main.myPlayer);
                        Projectile.NewProjectile(entitySource, NukePos.X, NukePos.Y, 0, 0,
                            ModContent.ProjectileType<DreadSpawnEffect>(), 40, 1, Owner: Main.myPlayer);

                        OffSet = Main.rand.Next(-240, 240 + 1);
                        NukePos.X = NPC.Center.X;
                        NukePos.Y = Target.Center.Y + OffSet;
                        Projectile.NewProjectile(entitySource, NukePos, new Vector2(-15, 0),
                          ModContent.ProjectileType<RedSkull>(), FinalLaserRedSkullDamage, 0, Owner: Main.myPlayer);
                        Projectile.NewProjectile(entitySource, NukePos.X, NukePos.Y, 0, 0,
                            ModContent.ProjectileType<DreadSpawnEffect>(), 40, 1, Owner: Main.myPlayer);
                    }
                }
            }

            if(Timer >= 600)
            {
                ResetState(ActionState.Teleport);
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {

            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<ManifestedAnxiety>(), 1, 1, 1));
            npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<DreadmireBag>()));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Gambit>(), 1, 1, 1));
            npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ModContent.ItemType<Items.Placeable.DreadBossRel>()));

            LeadingConditionRule notExpertRule = new LeadingConditionRule(new Conditions.NotExpert());
            notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<DreadFoil>(), minimumDropped: 40, maximumDropped: 65));
            notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<Aneuriliac>(), chanceDenominator: 2));
            notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<TheRedSkull>(), chanceDenominator: 2));
            notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<Pericarditis>(), chanceDenominator: 2));
            notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<Myocardia>(), chanceDenominator: 2));
            notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<DreadBroochA>()));
            npcLoot.Add(notExpertRule);
        }

        public override void OnKill()
        {
            NPC.SetEventFlagCleared(ref DownedBossSystem.downedDreadBoss, -1);        
        }
    }
}
