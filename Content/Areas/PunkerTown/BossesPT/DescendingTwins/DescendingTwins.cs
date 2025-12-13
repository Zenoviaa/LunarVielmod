using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json.Linq;
using Stellamod.Assets;
using Stellamod.Core;
using Stellamod.Core.Particles;
using Stellamod.Core.Shaders;
using Stellamod.Helpers;
using Stellamod.Visual.Particles;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.PunkerTown.BossesPT.DescendingTwins
{

    public class DescendingTwins : ScarletBoss
    {
        private enum TwinAttackState
        {
            SummonTwins,
            Idle,
            DashDance_Part1,
            DashDance_Part2,
        }

        private ref float Timer => ref NPC.ai[0];

        private int _retinaIndex;
        private int _spazzIndex;
        private NPC Retina => Main.npc[_retinaIndex];
        private NPC Spazz => Main.npc[_spazzIndex];

        private bool IsAwaitingCommand(NPC npc)
        {
            DescendingTwin.TwinAIState state = (DescendingTwin.TwinAIState)npc.ai[1];
            if (state == DescendingTwin.TwinAIState.Idle)
                return true;
            return false;
        }

        private void Command(NPC npc, DescendingTwin.TwinAIState state)
        {
            npc.ai[2] = (float)state;
        }

        private bool RetinaAwaitingCommand => IsAwaitingCommand(Retina);
        private bool SpazzAwaitingCommand => IsAwaitingCommand(Spazz);
        private void CommandRetina(DescendingTwin.TwinAIState state) => Command(Retina, state);
        private void CommandSpazz(DescendingTwin.TwinAIState state) => Command(Spazz, state);

        private TwinAttackState State
        {
            get => (TwinAttackState)NPC.ai[1];
            set => NPC.ai[1] = (float)value;
        }

        private ref float AttackNumber => ref NPC.ai[2];


        public override void SendExtraAI(BinaryWriter writer)
        {
            base.SendExtraAI(writer);
            writer.Write(_retinaIndex);
            writer.Write(_spazzIndex);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            base.ReceiveExtraAI(reader);
            _retinaIndex = reader.ReadInt32();
            _spazzIndex = reader.ReadInt32();
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            NPC.width = 64;
            NPC.height = 64;
            NPC.damage = 100;
            NPC.defense = 19;
            NPC.lifeMax = 6000;

            NPC.value = Item.buyPrice(gold: 5);
            NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.npcSlots = 30f;

            NPC.dontTakeDamage = true;
            NPC.dontCountMe = true;
            NPC.dontTakeDamageFromHostiles = true;
            NPC.HitSound = new SoundStyle("Stellamod/Assets/Sounds/Gintze_Hit") with { PitchVariance = 0.1f, Pitch = -0.5f, Volume = 0.2f };
            NPC.DeathSound = new SoundStyle("Stellamod/Assets/Sounds/Gintze_Death") with { PitchVariance = 0.1f, Pitch = -0.5f, Volume = 0.2f };
        }

        public override void AI()
        {
            base.AI();
            switch (State)
            {
                case TwinAttackState.SummonTwins:
                    AI_SummonTwins();
                    break;
                case TwinAttackState.Idle:
                    AI_Idle();
                    break;
                case TwinAttackState.DashDance_Part1:
                    AI_DashDancePart1();
                    break;
                case TwinAttackState.DashDance_Part2:
                    AI_DashDancePart2();
                    break;
            }
        }


        private void SwitchState(TwinAttackState state)
        {
            if (MultiplayerHelper.IsHost)
            {
                Timer = 0;
                State = state;
                NPC.netUpdate = true;
            }
        }

        private void AI_SummonTwins()
        {
            Timer++;
            if (Timer == 3)
            {
                if (MultiplayerHelper.IsHost)
                {
                    var source = NPC.GetSource_FromThis();
                    int x = (int)NPC.Center.X;
                    int y = (int)NPC.Center.Y;
                    _retinaIndex = NPC.NewNPC(source, x, y, ModContent.NPCType<DescendingTwin>(), ai0: 0,
                        ai1: (int)DescendingTwin.TwinAIState.SpawnRetina,
                        ai2: NPC.whoAmI);

                    _spazzIndex = NPC.NewNPC(source, x, y, ModContent.NPCType<DescendingTwin>(), ai0: 0,
                        ai1: (int)DescendingTwin.TwinAIState.SpawnSpazz,
                        ai2: NPC.whoAmI);
             
                    SwitchState(TwinAttackState.Idle);
                }
            }
        }

        private void ChooseAttack()
        {
            SwitchState(TwinAttackState.DashDance_Part1);
        }

        private void AI_Idle()
        {

            //Alright, So nowe have the commander setup, let's get this dash dance attack working
            AttackNumber = 0f;
            if(SpazzAwaitingCommand && RetinaAwaitingCommand)
            {
                Timer++;
                if (Timer == 1)
                {
                    NPC.TargetClosest();
                }


                if(Timer >= 60)
                {
                    ChooseAttack();
                }
            }
        }

        private void AI_DashDancePart1()
        {
            Timer++;
            if (Timer == 1)
            {
                NPC.TargetClosest();
            }

            //So how do we want this to work?
            //It should be pretty simple actually,
            //We're going to have each twin dash 5 times
            //Alternating between each other for a total of 10 dashes
            //Then we'll wait for them to both stop and throw it into the second dash dance
            if (AttackNumber < 10)
            {
                if(Timer >= 60)
                {                //Alternate between the twins and make them dash at you
                                 //The timing between these is based on the twin itself, not the commander
                                 //If you want to make it faster or slower, just edit that
                    if (AttackNumber % 2 == 0)
                    {
                        if (SpazzAwaitingCommand)
                        {
                            CommandSpazz(DescendingTwin.TwinAIState.SimpleDashStart);
                            AttackNumber++;
                        }
                    }
                    else
                    {
                        if (RetinaAwaitingCommand)
                        {
                            CommandRetina(DescendingTwin.TwinAIState.SimpleDashStart);
                            AttackNumber++;
                        }
                    }
                    Timer = 0;
                }

            }
            else
            {
                SwitchState(TwinAttackState.DashDance_Part2);
            }
        }

        private void AI_DashDancePart2()
        {
            Timer++;
            if (Timer == 1)
            {
                NPC.TargetClosest();
            }

            //Wait for both of them to finish and then put them into the dash dance state
            if (SpazzAwaitingCommand && RetinaAwaitingCommand)
            {
                CommandSpazz(DescendingTwin.TwinAIState.DashDanceStart);
                CommandRetina(DescendingTwin.TwinAIState.DashDanceStart);
                SwitchState(TwinAttackState.Idle);
            }
        }
    }


    //The thing with this boss is that it's a dual synced boss
    //I think the easiest way to do that is to have a single twin npc, and a controller npc
    //That basically sends commands to them telling them what to do
    //In that case, let's create a base class
    //I'm also going to use partial classing here to see how I feel about organizing with it

    public class DescendingTwin : ModNPC,
        IDrawOutlines
    {
        public enum TwinAIState
        {
            SpawnSpazz,
            SpawnRetina,

            Idle,


            SimpleDashStart,
            SimpleDash,
            SimpleDashEnd,

            DashDanceStart,
            DashDancePrepare,
            DashDance,
            DashDanceTwirl,
            DashDanceEnd
        }


        private enum TwinVariant
        {
            Spazz,
            Retina
        }

        private bool _contactDamage;
        private int _parentIndex;
        private ref float Timer => ref NPC.ai[0];
        private TwinAIState State
        {
            get => (TwinAIState)NPC.ai[1];
            set => NPC.ai[1] = (float)value;
        }

        private TwinAIState NextCommandState
        {
            get => (TwinAIState)NPC.ai[2];
            set => NPC.ai[2] = (float)value;
        }

        private ref float AttackNumber => ref NPC.ai[3];
        private TwinVariant Variant;
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Main.npcFrameCount[NPC.type] = 1;
            NPCID.Sets.MPAllowedEnemies[NPC.type] = true;
            NPCID.Sets.BossBestiaryPriority.Add(Type);
            NPCID.Sets.TrailCacheLength[NPC.type] = 16;
            NPCID.Sets.TrailingMode[Type] = 3;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            NPC.width = 64;
            NPC.height = 64;
            NPC.damage = 100;
            NPC.defense = 20;
            NPC.lifeMax = 18000;
            NPC.scale = 1f;
            NPC.aiStyle = -1;

            NPC.value = Item.buyPrice(gold: 5);
            NPC.knockBackResist = 0f;
            NPC.boss = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.npcSlots = 30f;

            Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/Boss6");
            NPC.HitSound = new SoundStyle("Stellamod/Assets/Sounds/Gintze_Hit") with { PitchVariance = 0.1f, Pitch = -0.5f, Volume = 0.2f };
            NPC.DeathSound = new SoundStyle("Stellamod/Assets/Sounds/Gintze_Death") with { PitchVariance = 0.1f, Pitch = -0.5f, Volume = 0.2f };
        }

        public override bool CheckActive()
        {
            return false;
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            return base.CanHitPlayer(target, ref cooldownSlot) && _contactDamage;
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            base.SendExtraAI(writer);
            writer.WriteVector2(_simpleDashNormal);
            writer.Write((float)Variant);
            writer.Write(_parentIndex);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            base.ReceiveExtraAI(reader);
            _simpleDashNormal = reader.ReadVector2();
            Variant = (TwinVariant)reader.ReadSingle();
            _parentIndex = reader.ReadInt32();
        }

        private void SwitchState(TwinAIState state)
        {
            if (MultiplayerHelper.IsHost)
            {
                Timer = 0;
                State = state;
                NPC.netUpdate = true;
            }
        }

        public override void AI()
        {
            base.AI();

            //If we don't have a valid target automatically retarget.
            if (!NPC.HasValidTarget)
            {
                NPC.TargetClosest();
            }

            _contactDamage = false;
            switch (State)
            {
                case TwinAIState.SpawnSpazz:
                    AI_SpawnSpazz();
                    break;
                case TwinAIState.SpawnRetina:
                    AI_SpawnRetina();
                    break;

                case TwinAIState.Idle:
                    AI_Idle();
                    break;


                case TwinAIState.SimpleDashStart:
                    AI_SimpleDashStart();
                    break;
                case TwinAIState.SimpleDash:
                    AI_SimpleDash();
                    break;
                case TwinAIState.SimpleDashEnd:
                    AI_SimpleDashEnd();
                    break;

                case TwinAIState.DashDanceStart:
                    AI_DashDanceStart();
                    break;
                case TwinAIState.DashDancePrepare:
                    AI_DashDancePrepare();
                    break;
                case TwinAIState.DashDanceTwirl:
                    AI_DashDanceTwirl();
                    break;
                case TwinAIState.DashDance:
                    AI_DashDance();
                    break;
                case TwinAIState.DashDanceEnd:
                    AI_DashDanceEnd();
                    break;
            }
            Lighting.AddLight(NPC.Center, Variant == TwinVariant.Spazz ? TorchID.Cursed : TorchID.Red);
            UpdateDraw();
        }

        private Player Target => Main.player[NPC.target];
        private Vector2 TargetNormal => NPC.DirectionTo(Target.Center);
        private void AI_SpawnRetina()
        {
            Variant = TwinVariant.Retina;
            _parentIndex = (int)NPC.ai[2];
            NPC.ai[2] = (float)TwinAIState.Idle;
            SwitchState(TwinAIState.Idle);
        }

        private void AI_SpawnSpazz()
        {
            Variant = TwinVariant.Spazz;
            _parentIndex = (int)NPC.ai[2];
            NPC.ai[2] = (float)TwinAIState.Idle;
            SwitchState(TwinAIState.Idle);
        }

        private void AI_Idle()
        {
            //Ok, so in the idle state, the goober is basically waiting on a command from the commander
            //So it should just slowly wander around and target the player
            Timer++;
            if (Timer == 1)
            {
                NPC.TargetClosest();
            }

     
            //Reset draw variables
            _scale = Vector2.One;
            _afterImageAlpha = 0f;

            //So we should slowly move towards the player if they're far, if not we'll just hover in place.
            //Step 1. Look towards the player, we can do this by calculating a target normal, calculating an angle and then lerping to it
            Vector2 targetNormal = TargetNormal;
            float targetAngle = targetNormal.ToRotation();
            NPC.rotation = Utils.AngleLerp(NPC.rotation, targetAngle, 0.1f);

            //Step 2. Check the distance between this current twin and the player
            //If the distance is too far we'll move closer to them, if not we just slow down/sit there
            float distanceToTarget = Vector2.Distance(NPC.Center, Target.Center);
            float maxDistance = 400;
            if (distanceToTarget > maxDistance)
            {
                //We should scale the movement velocity based on the distance, so the farther they are the faster we'll move
                Vector2 movementVelocity = targetNormal * distanceToTarget / 32f;
                NPC.velocity = Vector2.Lerp(NPC.velocity, movementVelocity, 0.05f);
            }
            else
            {
                //Otherwise, we'll just slow down
                //We want to keep a little bit of movement velocity so it's not just completely static
                NPC.velocity *= 0.8f;

                //Stpe 3. Add a little bit of hovering velocity for a cool effect
                float yHover = MathF.Sin(Timer * 0.1f) * 0.5f;
                NPC.velocity.Y += yHover;
            }

            //Remember, we're just waiting on a command from up above, so we don't actually need to do anything else here
            //However, we will create a few steam particles just for funsies
            if (Timer % 10 == 0)
            {
                Particle.NewParticle<BlackSmokeParticle>(
                    NPC.Center + Main.rand.NextVector2Circular(64, 64),
                    -Vector2.UnitY * Main.rand.NextFloat(0.2f, 0.5f), newColor: Color.White);
            }

            TargetOutlineColor = Color.Transparent;
            AttackNumber = 0f;

            //Receive the next command state.
            //This should be automatically netcoded btw
            if (NextCommandState != TwinAIState.Idle)
            {
                SwitchState(NextCommandState);
                NextCommandState = TwinAIState.Idle;
            }
        }

        #region Simple Dash
        //Both dash at you multiple times, crossing each other in the middle, making like a swirl dance
        //Alright, this attack is kinda like that one silksong attack from the cogwork dancers
        //We're going to need to make some really cool movement and visuals for this
        //We'll split this into two attacks
        private Vector2 _simpleDashNormal;
        private void AI_SimpleDashStart()
        {
            //The first attack is a basic dash where the eye looks at you
            //A telegraph line appears, and after a bit of anticipation, they go backward and then forward and do a quick dash
            //Alright so
            //Step 1. target a player, look at them
            Timer++;
            if (Timer == 1)
            {
                NPC.TargetClosest();
                _simpleDashNormal = TargetNormal;
            }


            float targetAngle = _simpleDashNormal.ToRotation();
            NPC.rotation = Utils.AngleLerp(NPC.rotation, targetAngle, 0.1f);

            //2. Calculate anticipation
            float windUpTime = 20f;
            float completionRatio = Timer / windUpTime;
            float ease = EasingFunction.Anticipation2(completionRatio);
            Vector2 movementNormal = Vector2.Lerp(-_simpleDashNormal * 0.5f, _simpleDashNormal, ease);
            Vector2 anticipationVelocity = movementNormal * 10f;
            NPC.velocity = anticipationVelocity;

            //3. Draw the telegraph line
            _telegraphLineAlpha = MathHelper.Lerp(0f, 1f, completionRatio / 0.5f);
            _telegraphLineRot = _simpleDashNormal.ToRotation();

            TargetOutlineColor = Color.Yellow;

            if (Timer >= windUpTime)
            {
                SwitchState(TwinAIState.SimpleDash);
            }
        }

        private int GetDustType()
        {
            switch (Variant)
            {
                default:
                case TwinVariant.Spazz:
                    return DustID.CursedTorch;
                case TwinVariant.Retina:
                    return DustID.RedTorch;
            }
        }
        private Color GetTwinColor()
        {
            switch (Variant)
            {
                default:
                case TwinVariant.Spazz:
                    return Color.Green;
                case TwinVariant.Retina:
                    return Color.Red;
            }
        }
        private void SpawnFlameDust()
        {
            Dust.NewDust(NPC.position, NPC.width, NPC.height, GetDustType(), Scale: Main.rand.NextFloat(1f, 2f));
            var p = Particle.NewParticle<GlowFragmentParticle>(NPC.Center, Vector2.Zero, Color.White);
            Color twinColor = GetTwinColor();
            p.innerColor = twinColor;
            p.outerColor = Color.Lerp(twinColor, Color.Black, 0.5f);
            p.fadeToColor = Color.Lerp(twinColor, Color.DarkBlue, 0.5f);
        }

        private void SpawnFlameDonut()
        {
            //movement donut particles
            var donut = Particle.NewParticle<GlowDonutParticle>(NPC.Center, -NPC.velocity.SafeNormalize(Vector2.Zero) * 2, newColor: Color.White);
            Color twinColor = GetTwinColor();
            donut.innerColor = twinColor;
            donut.outerColor = Color.Lerp(twinColor, Color.Black, 0.5f);
            donut.fadeToColor = Color.Lerp(twinColor, Color.DarkBlue, 0.5f);
        }
        private void AI_SimpleDash()
        {
            Timer++;
            if (Timer == 1)
            {
                AttackNumber++;

                //Play a cool little dash sound
                //Wait, I have an idea for how this can sound like
                SoundStyle dashSound = AttackNumber % 2 == 0 ?
                    AssetRegistry.Sounds.SteamPunking.DescendingDash1
                    : AssetRegistry.Sounds.SteamPunking.DescendingDash2;
                dashSound.PitchVariance = 0.3f;
                SoundEngine.PlaySound(dashSound, NPC.position);
            }

            if (Timer % 5 == 0)
            {
                SpawnFlameDonut();
            }

            if(Timer % 3 == 0)
            {
                SpawnFlameDust();
            }
            //Fade out the dash line and just move in the direction that we were moving
            //We can just multiply the velocity
            float dashTime = 20f;
            float completionRatio = Timer / dashTime;

            float dashSpeed = 35f;
            if (NPC.velocity.Length() < dashSpeed)
            {
                NPC.velocity *= 1.5f;
            }

            NPC.rotation = NPC.velocity.ToRotation();

            //Fade out the dash line
            _telegraphLineAlpha = MathHelper.Lerp(1f, 0f, completionRatio);
            _afterImageAlpha = MathHelper.Lerp(0f, 1f, EasingFunction.QuadraticBump(completionRatio));

            //Stretch the sprite a little bit to give a bit of a motion blurring effect
            _scale = Vector2.Lerp(new Vector2(1.5f, 1f), Vector2.One, completionRatio);

            //Set contact damage to be true
            //Make sure we telegraph this properly with red outlines.
            _contactDamage = true;
            TargetOutlineColor = Color.Red;
            if (Timer >= dashTime)
            {
                SwitchState(TwinAIState.SimpleDashEnd);
            }
        }

        private void AI_SimpleDashEnd()
        {
            Timer++;

            //Simply just slow down
            TargetOutlineColor = Color.Transparent;
            float endDashTime = 15f;
            NPC.velocity = NPC.velocity.RotatedBy(-0.05f);
            NPC.velocity *= 0.95f;
            NPC.rotation = NPC.velocity.ToRotation();
            if (Timer >= endDashTime)
            {
                SwitchState(TwinAIState.Idle);
            }
        }
        #endregion


        #region Dash Dance
        private void AI_DashDanceStart()
        {
            Timer++;
            if (Timer == 1)
            {
                NPC.TargetClosest();
                SoundStyle circlePrepareSound = AssetRegistry.Sounds.SteamPunking.DescendingCircle;
                circlePrepareSound.PitchVariance = 0.3f;
                SoundEngine.PlaySound(circlePrepareSound, NPC.position);
                _simpleDashNormal = NPC.velocity;
            }

            //So how do we want this attack to look?
            //I think the twins should orbit around a circle for a bit, on opposite points
            //Then after a while, they look towards you and dash to the point, when they touch each other
            //They'll burst into the dash
            //Alright so

            //First we need to create a circle around our target
            float windUpTime = 80f;
            float circleRadius = 300f;
            Vector2 initialDirection = -Vector2.UnitY;
            Vector2 dashVector = initialDirection * circleRadius;

            //Get an offset based on the variant that this goober is
            float radiansOffset = Variant == TwinVariant.Spazz ? MathHelper.Pi : 0;
            radiansOffset -= MathHelper.PiOver2;

            //get a ratio of how far we are into this prepation state
            float completionRatio = Timer / windUpTime;
            float rads = (MathHelper.TwoPi * 2);
            float radiansToRotateBy = MathHelper.Lerp(0f, rads, completionRatio);
            Vector2 rotatedVector = dashVector.RotatedBy(radiansToRotateBy + radiansOffset);
            Vector2 positionToMoveTo = Target.Center + rotatedVector;
            Vector2 targetVelocity = positionToMoveTo - NPC.Center;

            float inLerp = EasingFunction.InOutSine(completionRatio / 0.5f);
            NPC.velocity = Vector2.Lerp(_simpleDashNormal, targetVelocity, completionRatio);

            //We also need to rotate towards the target, we are facing them after all!
            Vector2 targetNormal = TargetNormal;
            float targetAngle = TargetNormal.ToRotation();
            NPC.rotation = Utils.AngleLerp(NPC.rotation, targetAngle, completionRatio);

            _telegraphLineAlpha = MathHelper.Lerp(0f, 1f, completionRatio);
            _telegraphLineRot = targetAngle;
            TargetOutlineColor = Color.Yellow;
            if (Timer >= windUpTime)
            {
                SwitchState(TwinAIState.DashDancePrepare);
            }
        }

        private void AI_DashDancePrepare()
        {
            Timer++;
            if (Timer == 1)
            {
                _simpleDashNormal = NPC.rotation.ToRotationVector2();
                SoundStyle windupPrepareSound = AssetRegistry.Sounds.SteamPunking.DescendingWindup;
                windupPrepareSound.PitchVariance = 0.3f;
                SoundEngine.PlaySound(windupPrepareSound, NPC.position);
            }
            _telegraphLineAlpha *= 0.5f;
            //Make sure there's a bit of preparation time
            float prepareTime = 30f;
            float completionRatio = Timer / prepareTime;
            float anticipationEase = EasingFunction.Anticipation2(completionRatio);
            Vector2 anticipationVelocity = Vector2.Lerp(-_simpleDashNormal * 5f, _simpleDashNormal * 5f, anticipationEase);
            NPC.velocity = anticipationVelocity;

            //So we build up some anticipation before the dash happens
            //And also fade out the dash line
            TargetOutlineColor = Color.Yellow;
            if (Timer >= prepareTime)
            {
                SwitchState(TwinAIState.DashDance);
            }
        }

        private void AI_DashDance()
        {
            Timer++;
            float dashTime = 15f;

            //Speed up the dash speed
            float dashSpeed = 30f;
            if (NPC.velocity.Length() < dashSpeed)
            {
                NPC.velocity *= 1.5f;
            }


            if (Timer % 5 == 0)
            {
                SpawnFlameDonut();
            }

            if (Timer % 3 == 0)
            {
                SpawnFlameDust();
            }

            //Create a cool little effect for have motion blurring
            float completionRatio = Timer / dashTime;
            float ease = EasingFunction.InOutSine(completionRatio);
            _scale = Vector2.Lerp(new Vector2(1.5f, 1f), Vector2.One, ease);

            //Add an after image
            _afterImageAlpha = MathHelper.Lerp(0f, 1f, completionRatio / 0.5f);

            //Enable the contact damage
            _contactDamage = true;
            TargetOutlineColor = Color.Red;

            if (Timer >= dashTime)
            {
                SwitchState(TwinAIState.DashDanceTwirl);
            }
        }

        private void AI_DashDanceTwirl()
        {
            Timer++;
            if (Timer == 1)
            {
                SoundStyle twirlSound = AssetRegistry.Sounds.SteamPunking.DescendingTwirl;
                twirlSound.PitchVariance = 0.3f;
                SoundEngine.PlaySound(twirlSound, NPC.position);
            }
            if (Timer % 3 == 0)
            {
                SpawnFlameDust();
            }

            //In this state, the twins rotate their velocity and sin a bit upwards
            //Alright so
            float twirlTime = 30f;
            float radiansToRotateVelocityBy = (MathHelper.TwoPi + MathHelper.Pi) / twirlTime;

            //We need to calculate the direction to rotate by, whether clockwise or counter clockwise
            //This is based on the way the twin
            float direction = Variant == TwinVariant.Spazz ? -1f : 1f;
            radiansToRotateVelocityBy *= direction;

            NPC.velocity = NPC.velocity.RotatedBy(-radiansToRotateVelocityBy);
            NPC.rotation = NPC.velocity.ToRotation();

            //By this point we already smoothed into this, so we can just set the draw variables
            _scale = Vector2.One;
            _afterImageAlpha = 1f;
            if (Timer >= twirlTime)
            {
                SwitchState(TwinAIState.DashDanceEnd);
            }

            //Enable contact damage
            _contactDamage = true;
            TargetOutlineColor = Color.Red;
        }

        private void AI_DashDanceEnd()
        {
            Timer++;
            float endTime = 45f;
            NPC.velocity *= 0.9f;
            NPC.rotation = Utils.AngleLerp(NPC.rotation, TargetNormal.ToRotation(), 0.1f);

            //Fade out the after image
            float completionRatio = Timer / endTime;
            float ease = EasingFunction.InOutSine(completionRatio);
            _afterImageAlpha = MathHelper.Lerp(1f, 0f, ease);
            if (Timer >= endTime)
            {
                SwitchState(TwinAIState.Idle);
            }
        }
        #endregion


        //telegraph line
        #region Draw Code
        private float _telegraphLineAlpha;
        private float _telegraphLineRot;


        private float _afterImageAlpha;
        private Vector2 _scale;

        private Color _outlineColor;
        private Color TargetOutlineColor;
        private void UpdateDraw()
        {
            _outlineColor = Color.Lerp(_outlineColor, TargetOutlineColor, 0.1f);
        }

        private void DrawTelegraphLine(SpriteBatch spriteBatch, Vector2 screenPos)
        {
            Texture2D bloomLineTexture = ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/BloomLine").Value;
            Vector2 drawOrigin = new Vector2(bloomLineTexture.Width / 2f, 0f);
            Vector2 drawScale = Vector2.One;
            drawScale.Y *= 2f;
            drawScale.X *= 0.5f;

            Color telegraphLineColor = Variant == TwinVariant.Spazz ? Color.Green : Color.Red;
            telegraphLineColor.A = 0;
            telegraphLineColor *= _telegraphLineAlpha;
            spriteBatch.Draw(bloomLineTexture, NPC.Center - screenPos, null, telegraphLineColor, _telegraphLineRot - MathHelper.PiOver2, drawOrigin, drawScale, SpriteEffects.None, 0);
        }


        private Texture2D GetTwinTexture()
        {
            if(Variant == TwinVariant.Spazz)
            {
                Texture2D twinTexture = ModContent.Request<Texture2D>(Texture + "_Spazz").Value;
                return twinTexture;
            }
            else
            {
                Texture2D twinTexture = ModContent.Request<Texture2D>(Texture).Value;
                return twinTexture;
            }
        }

        private Color GetFlamingTrailColor(float completionRatio)
        {
            return Color.Lerp(Color.White, Color.Transparent, completionRatio) * _afterImageAlpha;
        }

        private float GetFlamingTrailWidth(float completionRatio)
        {
            return MathHelper.SmoothStep(180, 180, completionRatio);
        }


        private void DrawFlamingTrail(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            var shader = BlackFireShader.Instance;
            shader.Time = Main.GlobalTimeWrappedHourly * 16;
            shader.InnerColor = Variant == TwinVariant.Spazz ? Color.Green : Color.Red;
            shader.OuterColor = Variant == TwinVariant.Spazz ? Color.DarkGreen : Color.DarkRed;
            TrailDrawer.Draw(spriteBatch, NPC.oldPos, GetFlamingTrailColor, GetFlamingTrailWidth, shader, offset: NPC.Size / 2f);
        }
        private void DrawAfterImages(SpriteBatch spriteBatch, Vector2 screenPos)
        {
            Texture2D twinTexture = GetTwinTexture();
            Rectangle frame = NPC.frame;
            Vector2 drawOrigin = frame.Size() / 2f;
            float trailLength = NPC.oldPos.Length;
            for (int i = 0; i < NPC.oldPos.Length; i++)
            {
                Vector2 drawCenter = NPC.oldPos[i] + NPC.Size / 2f - screenPos;
                float f = i;
                float completionRatio = f / trailLength;

                //After image
                Color drawColor = Color.Lerp(Color.White, Color.Transparent, completionRatio);
                drawColor *= _afterImageAlpha;

                drawColor *= 0.5f;
                SpriteEffects spriteEffects = SpriteEffects.None;
                if (NPC.spriteDirection == -1)
                {
                    spriteEffects = SpriteEffects.FlipVertically;
                }
                spriteBatch.Draw(twinTexture, drawCenter, frame, drawColor, NPC.oldRot[i], drawOrigin, _scale, spriteEffects, 0f);
            }
        }


        private void DrawSprite(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D twinTexture = GetTwinTexture();
            Rectangle frame = NPC.frame;
            Vector2 drawOrigin = frame.Size() / 2f;
            Vector2 drawCenter = NPC.Center - screenPos;
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (NPC.spriteDirection == -1)
            {
                spriteEffects = SpriteEffects.FlipVertically;
            }
            spriteBatch.Draw(twinTexture, drawCenter, frame, drawColor, NPC.rotation, drawOrigin, _scale, spriteEffects, 0f);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {

            DrawAfterImages(spriteBatch, screenPos);
            DrawFlamingTrail(spriteBatch, screenPos, drawColor);
            DrawFlamingTrail(spriteBatch, screenPos, drawColor);
            DrawTelegraphLine(spriteBatch, screenPos);
            DrawSprite(spriteBatch, screenPos, drawColor);
            drawColor *= ExtraMath.Osc(0f, 0.5f, speed: 3f);
            drawColor.A = 0;
            DrawSprite(spriteBatch, screenPos, drawColor);
            return false;
        }

        public void DrawOutlines(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
        {
            float outlineOffset = 2;
            DrawSprite(spriteBatch, screenPos + Vector2.UnitX * outlineOffset, _outlineColor);
            DrawSprite(spriteBatch, screenPos - Vector2.UnitX * outlineOffset, _outlineColor);
            DrawSprite(spriteBatch, screenPos + Vector2.UnitY * outlineOffset, _outlineColor);
            DrawSprite(spriteBatch, screenPos - Vector2.UnitY * outlineOffset, _outlineColor);
        }
        #endregion
    }
}
