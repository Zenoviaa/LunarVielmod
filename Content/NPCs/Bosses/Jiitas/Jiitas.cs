using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Core;
using Stellamod.Core.Effects;
using Stellamod.Core.Helpers;
using Stellamod.Core.Helpers.Math;
using Stellamod.Core.TitleSystem;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.NPCs.Bosses.Jiitas
{
    internal partial class Jiitas : ScarletBoss
    {
        /*
         * 
         * Jiitas
            Idle - Hovers around in place a little, like wiggles around like a floating puppet, will jump a few times before doing an attack

            Knife Spin - Spins with knives poking out and just tries to hit you, all you have to do is jump over it or dash through. If you get hit she jumps away, laughing and stops the attack. This also poisons you

            Bombs Away - Goes down and throws out bombs all around, will sometimes get pulled by the strings to mix up the movement, just back away to dodge it

            Spin Jump - Like Deton's and Nexa's jumping attacks, except she throws out knives at you

            Machine Gun Surprise - Pulls up her cloak and let's loose a barrage of bullets, these slowly hitscan (the bullets have no travel time and just raycast) towards you, just weave around it

            Fakeout - Laughs and makes several illusionary clones, if you attack a clone she throws a spread of knives or bombs


            Phase 2
            Her mask breaks, she gets faster, attack pattern changes a bit

            Bomb Drop - Uses strings to drag bombs above you and try to drop them on you
            Empower - She'll sometimes laugh and buff herself, which makes the next attack she does stronger
            -Empowered Knife Spin - Spins faster
            -Empowered Bombs Away - Bombs have a bigger explosion
            -Empowered Spin Jump - Throws more knives
            -Empowered Machine Gun - Does two barrages instead of just one, is a bit faster
            -Empowere Fakeout - Creates more clones
         */

        private SilhouetteShader _silhouetteShader;
        public enum AnimationState
        {
            Idle,
            Sitdown,
            Knifeout,
            Knifespin,
            Knifein,
            Situp,
            BombsAway,
            Bombing,
            Laugh,
            Undress,
            Redress,
            Jumpspin1,
            Jumpspin2,
            Jumpspin3,
            Dragup,
            Death
        }
        public enum ActionState
        {
            Idle,
            Spawn,
            KnifeSpin,
            BombsAway,
            SpinJump,
            MachineGunSurprise,
            Fakeout,
            BombDrop,
            Empower,
            Phase2Transition,
            Death
        }
        private int _frame;
        private FastNoiseLite _noise;
        private float _dancingPuppetSpawnTimer;
        private ref float Timer => ref NPC.ai[0];
        private ActionState State
        {
            get => (ActionState)NPC.ai[1];
            set => NPC.ai[1] = (float)value;
        }
        private ref float ActionStep => ref NPC.ai[2];
        private ref float AttackCounter => ref NPC.ai[3];
        public AnimationState Animation { get; set; }

        private Player Target => Main.player[NPC.target];
        private Vector2 DirectionToTarget => (Target.Center - NPC.Center).SafeNormalize(Vector2.Zero);
        private Color OutlineColor { get; set; }
        private bool OutlineFlicker { get; set; }

        public float PrimaryDrawAlpha { get; set; }
        private bool CloneDraw { get; set; }
        private bool InPhase2 => NPC.life <= NPC.lifeMax / 2;
        private bool HasPhaseTransitioned { get; set; }
        private bool Empowered { get; set; }

        private bool HasSaidTitle;
        private Vector2[] OldCenterPos;
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            NPCID.Sets.TrailingMode[Type] = 3;
            NPCID.Sets.TrailCacheLength[Type] = 8;
            NPCID.Sets.MPAllowedEnemies[Type] = true;
            Main.npcFrameCount[Type] = 56;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            OldCenterPos = new Vector2[8];
            NPC.width = 32;
            NPC.height = 64;
            NPC.damage = 32;
            NPC.defense = 0;
            NPC.lifeMax = 1100;
            NPC.HitSound = SoundID.NPCHit16;
            NPC.value = Item.buyPrice(silver: 50);
            NPC.knockBackResist = 0f;
            NPC.noGravity = false;
            NPC.boss = true;
            NPC.npcSlots = 10f;


            OutlineColor = Color.Transparent;

            //Setup the music and boss bar
            Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/JiitasMask");
            NPC.aiStyle = 0;
            //  NPC.BossBar = ModContent.GetInstance<JiitasBossBar>();
        }


        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            //Draw strings
            Asset<Texture2D> stringTextureAsset = ModContent.Request<Texture2D>(Texture + "_String");
            Vector2[] stringDrawPoints = new Vector2[]
            {
                NPC.Center + new Vector2(32, -10),
                NPC.Center + new Vector2(-32, -10),
                NPC.Center + new Vector2(-16, 8),
                NPC.Center + new Vector2(16, 8),
                NPC.Center + new Vector2(0, -8)
            };

            //Draw Strings
            //I LOVE STRINGS
            for (int i = 0; i < stringDrawPoints.Length; i++)
            {
                Vector2 drawPoint = stringDrawPoints[i];
                drawPoint -= screenPos;
                drawPoint.Y -= ExtraMath.Osc(0f, 1f, speed: 1, offset: (float)i * 4);
                Vector2 drawOrigin = new Vector2(0, stringTextureAsset.Height());
                float drawRotation = MathHelper.Lerp(-0.05f, 0.05f, ExtraMath.Osc(0f, 1f, speed: 1, offset: (float)i * 4));
                float drawAlpha = ExtraMath.Osc(0f, 1f, speed: 1, offset: (float)i * 4);
                spriteBatch.Draw(stringTextureAsset.Value, drawPoint, null, drawColor * drawAlpha * PrimaryDrawAlpha, drawRotation, drawOrigin, 1, SpriteEffects.None, 0);
            }

            //Draw Glow
            spriteBatch.Restart(blendState: BlendState.Additive);
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Rectangle frame = NPC.frame;
            Vector2 mainDrawOrigin = frame.Size() / 2f;
            SpriteEffects effects = NPC.spriteDirection != -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            float glowDrawOffset = VectorHelper.Osc(3f, 4f, 5);

            for (float f = 0f; f < 1.0f; f += 0.2f)
            {
                Vector2 offsetDrawPos = NPC.Top - screenPos + (f * MathHelper.TwoPi).ToRotationVector2() * glowDrawOffset;
                spriteBatch.Draw(texture, offsetDrawPos, frame, drawColor * 0.35f * PrimaryDrawAlpha, NPC.rotation, mainDrawOrigin, NPC.scale, effects, layerDepth: 0);
            }
            spriteBatch.RestartDefaults();
            DrawOutline(drawColor);
            DrawClones(drawColor);
            if (Empowered)
            {
                DrawEmpoweredAfterImage(drawColor);
            }
            DrawCharacter(NPC.Center, drawColor * PrimaryDrawAlpha);
            return false;
        }

        private void DrawOutline(Color drawColor)
        {
            _silhouetteShader ??= new SilhouetteShader();
            if (OutlineColor == Color.Transparent)
                return;

            SpriteBatch spriteBatch = Main.spriteBatch;
            spriteBatch.Restart(effect: _silhouetteShader.Effect);

            float flickerOsc = OutlineFlicker ? ExtraMath.Osc(0f, 1f, speed: 16, 0) : 1;
            Color outlineColor = OutlineColor * flickerOsc;
            DrawCharacter(NPC.Center + new Vector2(2, 0), outlineColor);
            DrawCharacter(NPC.Center + new Vector2(-2, 0), outlineColor);
            DrawCharacter(NPC.Center + new Vector2(0, 2), outlineColor);
            DrawCharacter(NPC.Center + new Vector2(0, -2), outlineColor);
            spriteBatch.RestartDefaults();
        }

        private void DrawCharacter(Vector2 position, Color drawColor)
        {
            position.Y -= NPC.height / 4;
            position += Vector2.Lerp(Vector2.Zero, -Vector2.UnitY * 16, ExtraMath.Osc(0f, 1f));
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Rectangle frame = NPC.frame;
            Vector2 mainDrawOrigin = frame.Size() / 2f;
            SpriteEffects effects = NPC.spriteDirection != -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Vector2 drawPosition = position - Main.screenPosition;
            SpriteBatch spriteBatch = Main.spriteBatch;
            spriteBatch.Draw(texture, drawPosition, frame, drawColor, NPC.rotation, mainDrawOrigin, NPC.scale, effects, 0);
            if (HasPhaseTransitioned)
            {
                Texture2D brokenMaskTexture = ModContent.Request<Texture2D>(Texture + "_MaskBroken").Value;
                spriteBatch.Draw(brokenMaskTexture, drawPosition, frame, drawColor, NPC.rotation, mainDrawOrigin, NPC.scale, effects, 0);
            }
        }
        private void DrawEmpoweredAfterImage(Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Rectangle frame = NPC.frame;
            Vector2 drawOrigin = frame.Size() / 2f;
            SpriteBatch spriteBatch = Main.spriteBatch;
            spriteBatch.Restart(effect: _silhouetteShader.Effect, blendState: BlendState.Additive);
            SpriteEffects effects = NPC.spriteDirection != -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            for (int i = 0; i < OldCenterPos.Length; i++)
            {
                Vector2 centerPos = OldCenterPos[i];
                Vector2 drawPos = centerPos - Main.screenPosition;
                drawPos.Y -= NPC.height / 4;
                drawPos += Vector2.Lerp(Vector2.Zero, -Vector2.UnitY * 16, ExtraMath.Osc(0f, 1f));
                float interpolant = (float)i / (float)OldCenterPos.Length;
                Color drawColor = Color.Lerp(Color.Blue, Color.LightBlue, interpolant);
                drawColor *= MathHelper.SmoothStep(1.0f, 0f, interpolant);
                drawColor = drawColor.MultiplyRGB(lightColor);
                spriteBatch.Draw(texture, drawPos, frame, drawColor * PrimaryDrawAlpha, NPC.rotation, drawOrigin, NPC.scale, effects, layerDepth: 0);
            }

            spriteBatch.RestartDefaults();

        }

        private void Warn()
        {
            OutlineColor = Color.Yellow;
            OutlineFlicker = true;
        }
        private void WarnContactDamage()
        {
            OutlineColor = Color.Red;
            OutlineFlicker = false;
        }
        private void NoWarn()
        {
            OutlineColor = Color.Transparent;
            OutlineFlicker = false;
        }
        private void SwitchState(ActionState state)
        {
            if (MultiplayerHelper.IsHost)
            {
                Timer = 0;
                ActionStep = 0;
                AttackCounter = 0;
                State = state;
                NPC.netUpdate = true;
            }
        }

        private void PlayAnimation(AnimationState animation)
        {
            if (Animation == animation)
                return;
            Animation = animation;
            NPC.frameCounter = 0;
            switch (animation)
            {
                default:
                case AnimationState.Idle:
                    _frame = 0;
                    break;
                case AnimationState.Sitdown:
                    _frame = 5;
                    break;
                case AnimationState.Knifeout:
                    _frame = 11;
                    break;
                case AnimationState.Knifespin:
                    _frame = 14;
                    break;
                case AnimationState.Knifein:
                    _frame = 18;
                    break;
                case AnimationState.Situp:
                    _frame = 21;
                    break;
                case AnimationState.BombsAway:
                    _frame = 26;
                    break;
                case AnimationState.Bombing:
                    _frame = 30;
                    break;
                case AnimationState.Laugh:
                    _frame = 34;
                    break;
                case AnimationState.Undress:
                    _frame = 37;
                    break;
                case AnimationState.Redress:
                    _frame = 42;
                    break;
                case AnimationState.Jumpspin1:
                    _frame = 46;
                    break;
                case AnimationState.Jumpspin2:
                    _frame = 47;
                    break;
                case AnimationState.Jumpspin3:
                    _frame = 48;
                    break;
                case AnimationState.Dragup:
                    _frame = 49;
                    break;
                case AnimationState.Death:
                    _frame = 53;
                    break;
            }
        }
        public override void FindFrame(int frameHeight)
        {
            base.FindFrame(frameHeight);

            //Animation Speed
            NPC.frameCounter += 0.15f;
            if (NPC.frameCounter >= 1f)
            {
                _frame++;
                NPC.frameCounter = 0f;
            }

            switch (Animation)
            {
                default:
                case AnimationState.Idle:
                    if (_frame >= 5f)
                    {
                        _frame = 0;
                    }
                    break;
                case AnimationState.Sitdown:
                    //Don't loop this one
                    //Starting frame
                    if (_frame >= 12)
                    {
                        _frame = 11;
                    }
                    break;
                case AnimationState.Knifeout:
                    if (_frame >= 14)
                    {
                        _frame = 13;
                    }
                    break;
                case AnimationState.Knifespin:
                    if (_frame >= 18)
                    {
                        _frame = 14;
                    }
                    break;
                case AnimationState.Knifein:
                    if (_frame >= 21)
                    {
                        _frame = 20;
                    }
                    break;
                case AnimationState.Situp:
                    if (_frame >= 26)
                    {
                        _frame = 25;
                    }
                    break;
                case AnimationState.BombsAway:
                    if (_frame >= 30)
                    {
                        _frame = 29;
                    }
                    break;
                case AnimationState.Bombing:
                    if (_frame >= 34)
                    {
                        _frame = 30;
                    }
                    break;
                case AnimationState.Laugh:
                    if (_frame >= 37)
                    {
                        _frame = 34;
                    }
                    break;
                case AnimationState.Undress:
                    if (_frame >= 42)
                    {
                        _frame = 41;
                    }
                    break;
                case AnimationState.Redress:
                    if (_frame >= 46)
                    {
                        _frame = 45;
                    }
                    break;
                case AnimationState.Jumpspin1:
                    _frame = 46;
                    break;
                case AnimationState.Jumpspin2:
                    _frame = 47;
                    break;
                case AnimationState.Jumpspin3:
                    _frame = 48;
                    break;
                case AnimationState.Dragup:
                    if (_frame >= 53)
                    {
                        _frame = 52;
                    }
                    break;
                case AnimationState.Death:
                    if (_frame >= 56)
                    {
                        _frame = 53;
                    }
                    break;
            }

            NPC.frame.Y = frameHeight * _frame;
        }

        public bool ShouldDealContactDamage;
        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            //NO CONTACT DAMAGE
            return ShouldDealContactDamage;
        }
        public override void AI()
        {
            base.AI();
            int trailCacheLength = 8;
            if (OldCenterPos == null && trailCacheLength > 0)
                OldCenterPos = new Vector2[trailCacheLength];
            if (OldCenterPos != null)
            {
                for (int i = trailCacheLength - 1; i > 0; i--)
                {
                    OldCenterPos[i] = OldCenterPos[i - 1];
                }
                OldCenterPos[0] = NPC.Center;

            }
            if (CloneDraw)
            {
                CloneAlpha = MathHelper.SmoothStep(CloneAlpha, 1.0f, 0.1f);
            }
            else
            {
                CloneAlpha = MathHelper.SmoothStep(CloneAlpha, 0f, 0.1f);
            }

            if(HasPhaseTransitioned && MultiplayerHelper.IsHost)
            {
                _dancingPuppetSpawnTimer++;
                if(_dancingPuppetSpawnTimer % 60 == 0)
                {
                    int x = (int)NPC.Center.X + Main.rand.Next(-256, 256);
                    int y = (int)NPC.Center.Y - 256;
                    NPC.NewNPC(NPC.GetSource_FromThis(), x, y, ModContent.NPCType<DancingPuppet>());
                }
            }
            if (!HasSaidTitle)
            {
                TitleCardUISystem uiSystem = ModContent.GetInstance<TitleCardUISystem>();
                uiSystem.OpenUI("Jiitas the Jhastas", 240);
                HasSaidTitle = true;
            }
 
            NPC.TargetClosest();
            switch (State)
            {
                case ActionState.Idle:
                    AI_Idle();
                    break;
                case ActionState.Spawn:
                    AI_Spawn();
                    break;
                case ActionState.KnifeSpin:
                    AI_KnifeSpin();
                    break;
                case ActionState.BombsAway:
                    AI_BombsAway();
                    break;
                case ActionState.SpinJump:
                    AI_SpinJump();
                    break;
                case ActionState.MachineGunSurprise:
                    AI_MachineGunSurprise();
                    break;
                case ActionState.Fakeout:
                    AI_Fakeout();
                    break;
                case ActionState.BombDrop:
                    AI_BombDrop();
                    break;
                case ActionState.Empower:
                    AI_Empower();
                    break;
                case ActionState.Phase2Transition:
                    AI_Phase2Transition();
                    break;
                case ActionState.Death:
                    AI_Death();
                    break;
            }
        }

        public override void OnHitByProjectile(Projectile projectile, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitByProjectile(projectile, hit, damageDone);
            HasBeenHit = true;
        }
        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0)
            {
                NPC.life = 1;
                if (State != ActionState.Death)
                    SwitchState(ActionState.Death);
            }
        }
    }
}

