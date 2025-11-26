using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Assets;
using Stellamod.Core;
using Stellamod.Core.Camera;
using Stellamod.Core.Particles;
using Stellamod.Core.Pixelation;
using Stellamod.Core.Shaders;
using Stellamod.Core.Shaders.MagicTrails;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.UI.Systems;
using Stellamod.Visual.Particles;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

/*

- Deer with a singularity for a head, in its spawn animation at first it looks like a normal deer before the head explodes and parts start orbiting it, ooo I know exactly how to code this

- The legs and everything are rigged, we’ll use forward kinematics to animate the boss, so we’ll have to make a run animation and idle animation

- Opens the fight with several exploding blood magic projectiles that loosely track the player

- Winds up a charge and then runs directly at the player really fast, and explodes into bloody bits before merging itself back together elsewhere

- Runs up into the sky and rains down acidic blood

- Walks slowly around the player as bloody boils explode from its body and then home back towards you

- Cracks form in its body and it violently erupts into multiple bloody geysers

- Winds up a charge and then keeps running at you while swerving around and trying to juke you out
 
- In phase 2 every attack gets more deadlier, triggers at under 50% health
 */
namespace Stellamod.Content.Areas.Ishtar.BossesIS.SanguineSingularity
{
    public interface IDrawSanguineBlood
    {
        void DrawToSanguineMask(SpriteBatch spriteBatch);
    }

    [Autoload(Side = ModSide.Client)]
    public class SanguineBloodRenderManager : ModSystem
    {

        private Point _oldScreenSize;
        private RenderTarget2D _pixelRenderRT;
        private RenderTarget2D _pixelScreenRenderRT;
        private List<IDrawSanguineBlood> _draws = new List<IDrawSanguineBlood>(100);

        public int DownSamples => 2;
        public override void OnModLoad()
        {
            base.OnModLoad();
            On_Main.CheckMonoliths += RenderToPixelationRT;
            On_Main.DoDraw_DrawNPCsOverTiles += DrawPixelRTToScreen;
            Main.OnResolutionChanged += ResizeTargets;
        }

        public override void Load()
        {
            base.Load();
            ResizeRenderTargets();
        }
        public override void OnModUnload()
        {
            base.OnModUnload();
            On_Main.CheckMonoliths -= RenderToPixelationRT;
            On_Main.DoDraw_DrawNPCsOverTiles -= DrawPixelRTToScreen;
            Main.OnResolutionChanged -= ResizeTargets;
        }

        private void RenderToPixelationRT(On_Main.orig_CheckMonoliths orig)
        {
            orig();
            if (Main.gameMenu)
                return;

            _draws.Clear();
            foreach (var proj in Main.ActiveProjectiles)
            {
                if (proj.ModProjectile is IDrawSanguineBlood pixelated)
                {
                    _draws.Add(pixelated);
                }
            }

            if (_draws.Count > 0)
            {
                SpriteBatch spriteBatch = Main.spriteBatch;
                GraphicsDevice graphicsDevice = Main.graphics.GraphicsDevice;
                graphicsDevice.SetRenderTarget(_pixelScreenRenderRT);
                graphicsDevice.Clear(Color.Transparent);

                //Alright, so what we're going to do is actually use two render targets to get around the issue of misplaced pixels
                //This costs a bit of extra performance but it'll look good
                //So, first draw at fully quality to the screen render target

                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
                for (int i = 0; i < _draws.Count; i++)
                {
                    var draw = _draws[i];
                    draw.DrawToSanguineMask(spriteBatch);
                }
                spriteBatch.End();


                //Now we take that output and downscale it to the pixel RT
                graphicsDevice.SetRenderTarget(_pixelRenderRT);
                graphicsDevice.Clear(Color.Transparent);

                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
                float denom = DownSamples;
                float scale = 1f / denom;
                spriteBatch.Draw(_pixelScreenRenderRT, Vector2.Zero, null, Color.White, 0, Vector2.Zero, scale, SpriteEffects.None, 0);
                spriteBatch.End();
            }
        }


        private void DrawPixelRTToScreen(On_Main.orig_DoDraw_DrawNPCsOverTiles orig, Main self)
        {
            orig(self);
            if (Main.gameMenu)
                return;

            if (_draws.Count <= 0)
                return;

            var bloodyShader = BloodyShader.Instance;
            bloodyShader.InnerColor = Color.Red;
            bloodyShader.OuterColor = Color.Lerp(Color.Red, Color.Black, 0.6f);
            bloodyShader.Distortion = 0.2f;
            bloodyShader.Tiling = Vector2.One * 2f;

            SpriteBatch spriteBatch = Main.spriteBatch;
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, 
                DepthStencilState.Default, RasterizerState.CullNone, bloodyShader.Effect);

            float scale = DownSamples;

            spriteBatch.Draw(_pixelRenderRT, Vector2.Zero, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            spriteBatch.End();
        }

        private void ResizeRenderTargets()
        {
            Point screenSize = Main.ScreenSize;
            if (_oldScreenSize != screenSize)
            {
                Main.QueueMainThreadAction(() =>
                {
                    _pixelRenderRT.Release();
                    _pixelRenderRT = new RenderTarget2D(Main.graphics.GraphicsDevice, screenSize.X / DownSamples, screenSize.Y / DownSamples);

                    _pixelScreenRenderRT.Release();
                    _pixelScreenRenderRT = new RenderTarget2D(Main.graphics.GraphicsDevice, screenSize.X, screenSize.Y);

                });
                _oldScreenSize = screenSize;
            }
        }
        private void ResizeTargets(Vector2 vector)
        {
            ResizeRenderTargets();
        }
    }
    public class SanguineGoreManager
    {
        private float _timer;
        private float _orbitRadius;
        public SanguineGoreManager(Texture2D goreTexture, int frameCount)
        {
            gores = new SanguineGore[frameCount];
            int height = goreTexture.Height / frameCount;
            for (int i = 0; i < gores.Length; i++)
            {
                int y = i * height;
                Rectangle frame = new Rectangle(0, y, goreTexture.Width, height);
                gores[i] = new SanguineGore(goreTexture, frame);


                //Calculate the initial position of the orbit
                float f = i;
                float num = gores.Length;
                float completionRatio = f / num;
                float z = MathHelper.Lerp(1f, -1f, EasingFunction.QuadraticBump(completionRatio));
                float x = MathHelper.Lerp(-1f, 1f, completionRatio);
                Vector3 initialPosition = new Vector3(x, 0, z);
                gores[i].initialPosition = initialPosition;
            }
            orbitingRadius = 40f;
        }

        public SanguineGore[] gores;
        public bool draw;
        public float orbitingRadius;
 
        public bool HasTinyOrbit()
        {
            return _orbitRadius <= 0.1f;
        }

        public void Update(Vector2 orbitCenter)
        {
            _orbitRadius = MathHelper.Lerp(_orbitRadius, orbitingRadius, 0.01f);

            //Rotate the damn thing
            _timer++;

            float angle = _timer * 0.05f;
            Quaternion quaternion = Quaternion.CreateFromAxisAngle(new Vector3(-0.2f, -0.2f, 1f), angle);
            Matrix rotationMatrix = Matrix.CreateFromQuaternion(quaternion);
            for(int i = 0; i < gores.Length; i++)
            {
                SanguineGore gore = gores[i];

                //So we need to calculate some type of orbiting movement
                Vector3 newPosition = Vector3.Transform(gore.initialPosition * _orbitRadius, rotationMatrix);
                Vector2 offset = new Vector2(newPosition.X, newPosition.Y);
                Vector2 gorePosition = orbitCenter + offset;
                gore.position = gorePosition;
                gore.rotation = offset.ToRotation();

                float osc = ExtraMath.Osc(0f, 1f, speed: 2f, offset: i);
                Vector2 oscScale = Vector2.Lerp(new Vector2(1.2f, 0.9f), Vector2.One, osc);
                gore.oscScale = oscScale;
                if (Main.rand.NextBool(100))
                {
                    var d = Dust.NewDustPerfect(gore.position, DustID.Blood, Scale: Main.rand.NextFloat(0.5f, 1f));
                    d.noGravity = true;
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
        {
            if (!draw)
                return;

            for(int i = 0; i < gores.Length; i++)
            {
                SanguineGore gore = gores[i];
                gore.Draw(spriteBatch, screenPos, lightColor);
            }
        }
    }

    public class SanguineGore
    {
        public SanguineGore(Texture2D texture, Rectangle frame)
        {
            this.position = Vector2.Zero;
            this.texture = texture;
            this.frame = frame;
            this.color = Color.White;
            this.scale = Vector2.One;
            this.oscScale = Vector2.One;
        }
        public Vector2 position;
        public Vector3 initialPosition;
        public Vector2 scale;
        public Vector2 oscScale;
        public Rectangle frame;
        public Texture2D texture;
        public Color color;
        public float rotation;
        public float sortingOrder;
        public void Draw(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
        {
            Vector2 drawCenter = position - screenPos;
            Vector2 drawOrigin = frame.Size() / 2f;
            Color drawColor = color.MultiplyRGB(lightColor);
            spriteBatch.Draw(texture, drawCenter, frame, drawColor, rotation, drawOrigin, scale * oscScale, SpriteEffects.None, 0 );
        }
    }

    public class BloodGeyser : ModProjectile,
        IDrawPixelated
    {
        private Vector2[] BlastPos;
        private ref float Timer => ref Projectile.ai[0];
        public override string Texture => TextureRegistry.EmptyTexture;
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 100;
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }

        public override void AI()
        {
            base.AI();
            Timer++;
            List<Vector2> blastPoints = new List<Vector2>();
            float numPoints = 80;
            for (float f = 0; f < numPoints; f++)
            {
                float completionRatio = f / numPoints;
                Vector2 point = Vector2.Lerp(Projectile.Center, Projectile.Center + Projectile.velocity, completionRatio);
                blastPoints.Add(point);
            }
            BlastPos = blastPoints.ToArray();
        }


        private Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(Color.Black, Color.White, completionRatio);
        }

        private float WidthFunction(float completionRatio)
        {
            float width = 5 * 1.5f;
            return MathHelper.Lerp(width, 0, EasingFunction.InOutExpo(completionRatio));
        }

        public void DrawPixelated()
        {
            if (BlastPos == null)
                return;

            FlamingTrailShader flamingTrailShader = FlamingTrailShader.Instance;
            flamingTrailShader.OuterColor = Color.DarkBlue;
            flamingTrailShader.InnerColor = Color.Red;
            flamingTrailShader.Power = 0.3f;
            flamingTrailShader.Distortion = 6;
            flamingTrailShader.Tiling = Vector2.One * 0.5f;
            //This just applis the shader changes
            TrailDrawer.Draw(Main.spriteBatch, BlastPos, ColorFunction, WidthFunction, flamingTrailShader);
        }
    }
    public class BloodRain : ScarletProjectile,
        IDrawPixelated
    {
        private ref float Timer => ref Projectile.ai[0];
        public override string Texture => TextureRegistry.EmptyTexture;
        public override void SetDefaults()
        {
            base.SetDefaults();
            TrailCacheLength = 32;
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.penetrate = -1;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 180;
        }

        public override void AI()
        {
            base.AI();
            Timer++;
            if (Projectile.velocity.Y < 5)
            {
                Projectile.velocity.Y += 0.4f;
            }

            Projectile.velocity.X = -2;
            Projectile.rotation = Projectile.velocity.ToRotation();
        }

        private Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(Color.Black, Color.White, completionRatio);
        }

        private float WidthFunction(float completionRatio)
        {
            float width = 5 * 1.5f;
            return MathHelper.Lerp(width, 0, EasingFunction.InOutExpo(completionRatio));
        }

        public void DrawPixelated()
        {
            FlamingTrailShader flamingTrailShader = FlamingTrailShader.Instance;
            flamingTrailShader.OuterColor = Color.DarkBlue;
            flamingTrailShader.InnerColor = Color.Red;
            flamingTrailShader.Power = 0.3f;
            flamingTrailShader.Distortion = 6;
            flamingTrailShader.Tiling = Vector2.One * 0.5f;
            //This just applis the shader changes
            TrailDrawer.Draw(Main.spriteBatch, OldCenterPos, ColorFunction, WidthFunction, flamingTrailShader);
        }
    }

    public class BloodyChargeBoom : ModProjectile
    {
        public override string Texture => TextureRegistry.EmptyTexture;
        private ref float Timer => ref Projectile.ai[0];
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 64;
            Projectile.height = 64;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 10;
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }

        public override void AI()
        {
            base.AI();
            Timer++;
            if (Timer == 1)
            {
                var p = FXUtil.GlowCircleBoom(Projectile.Center, Color.Red, Color.DarkBlue, Color.Black);
                p.Scale *= 3f;

                var p2 = FXUtil.GlowCircleBoom(Projectile.Center, Color.Red, Color.DarkBlue, Color.Black);
                p2.Scale *= 2f;

                ShakeModSystem.Shake = 8;
                FXUtil.ShakeCamera(Projectile.Center, 1024, 8);
                for (float f = 0; f < 16f; f++)
                {
                    Vector2 velocity = Main.rand.NextVector2Circular(24, 24);
                    Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowDust>(), velocity,
                        newColor: Color.Red,
                        Scale: Main.rand.NextFloat(0.5f, 3f));
                }

                for (float f = 0; f < Main.rand.NextFloat(10f, 16f); f++)
                {
                    Color color = Color.White;
                    Vector2 velocity = Main.rand.NextVector2Circular(24, 24);
                    Particle.NewBlackParticle<BloodSparkleParticle>(Projectile.Center, velocity, color, Scale: Main.rand.NextFloat(0.5f, 3f));
                }
            }
        }
    }

    public class BloodyBurst : ScarletProjectile,
        IDrawPixelated
    {
        public override string Texture => TextureRegistry.EmptyTexture;

        private ref float Timer => ref Projectile.ai[0];
        public override void SetDefaults()
        {
            base.SetDefaults();
            TrailCacheLength = 32;
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.hostile = true;
            Projectile.friendly = false;
        }

        public override void AI()
        {
            base.AI();
            Timer++;
            Player closest = PlayerHelper.FindClosestPlayer(Projectile.position, 10000);
            if (closest != null)
            {
                Projectile.velocity = ProjectileHelper.SimpleHomingVelocity(Projectile, closest.Center, 1);
            }

            if (Timer % 7 == 0)
            {
                Particle.NewBlackParticle<BloodSparkleParticle>(Projectile.Center, Vector2.Zero, Color.Red, Scale: Main.rand.NextFloat(0.5f, 1f));
            }
            Projectile.rotation = Projectile.velocity.ToRotation();
        }

        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
            FXUtil.GlowCircleBoom(Projectile.Center, Color.Red, Color.DarkBlue, Color.Black);
        }

        private Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(Color.Black, Color.White, completionRatio);
        }

        private float WidthFunction(float completionRatio)
        {
            float width = 18 * 1.5f;
            return MathHelper.Lerp(width, 0, EasingFunction.InOutExpo(completionRatio));
        }
        public void DrawPixelated()
        {
            FlamingTrailShader flamingTrailShader = FlamingTrailShader.Instance;
            flamingTrailShader.OuterColor = Color.DarkBlue;
            flamingTrailShader.InnerColor = Color.Red;
            flamingTrailShader.Power = 0.3f;
            flamingTrailShader.Distortion = 6;
            flamingTrailShader.Tiling = Vector2.One * 0.5f;
            //This just applis the shader changes
            TrailDrawer.Draw(Main.spriteBatch, OldCenterPos, ColorFunction, WidthFunction, flamingTrailShader);
        }
    }

    public class SanguineSingularity : ScarletBoss,
        IDrawOutlines
    {
        public struct SanguineDraw
        {
            public Color outlineColor;
            public Vector2 scale;
            public Vector2 shake;
            public Vector2 singularityScale;
            public float alpha;
            public float flashAlpha;
            public float afterImageAlpha;
            public bool headless;


            public void SetDefaults()
            {
                outlineColor = Color.Transparent;
                scale = Vector2.One;
                shake = Vector2.Zero;
                singularityScale = Vector2.Zero;
                alpha = 1f;
                flashAlpha = 0f;
                afterImageAlpha = 0f;
                headless = false;
            }
        }
        private enum AnimationState
        {
            Idle,
            Walk,
            Run
        }

        private enum AIState
        {
            Spawn,
            Idle,
            Despawn,
            Death,

            BloodyBurst_Start,
            BloodyBurst,
            BloodyBurst_End,

            BloodyCharge_Start,
            BloodyCharge_Rush,
            BloodyCharge_End,

            BloodRain_Start,
            BloodRain_Rain,
            BloodRain_End,

            BloodBoil_Start,
            BloodBoil_Walk,
            BloodBoil_End,

            BloodCrack_Start,
            BloodCrack_Geyser,
            BloodCrack_End,

            GhastlyBloodDash_Start,
            GhastlyBloodDash_Run,
            GhastlyBloodDash_End,

            Phase2Transition
        }

        private bool _contactDamage;
        private bool _headless;

        private Vector2 _scale;
        private Vector2 _runDirection;

        private SanguineDraw _draw;
        private SanguineGoreManager _goreManagerBacking;
        private SanguineGoreManager GoreManager
        {
            get
            {
                if(_goreManagerBacking == null)
                {
                    _goreManagerBacking = new SanguineGoreManager(ModContent.Request<Texture2D>(Texture + "_HeadGore", AssetRequestMode.ImmediateLoad).Value, 5);
                }
                return _goreManagerBacking;
            }
        }
        private Vector2 _teleportPosition;

        private Color TargetOutlineColor;
        private ref float Timer => ref NPC.ai[0];

        private AIState State
        {
            get => (AIState)NPC.ai[1];
            set => NPC.ai[1] = (float)value;
        }

        private ref float AttackNumber => ref NPC.ai[2];
        private int BloodyBurstDamage => 26;
        private int BloodyChargeBoomDamage => 30;
        private int BloodRainDamage => 18;
        private int BloodGeyserDamage => 26;

        private AnimationState _animation;
        private PatternManager<AIState> _patternManagerBacking;
        private PatternManager<AIState> PatternManager
        {
            get
            {
                if (_patternManagerBacking == null)
                {
                    _patternManagerBacking = new PatternManager<AIState>(
                        new Tuple<AIState, float>(AIState.BloodyBurst_Start, 1f),
                        new Tuple<AIState, float>(AIState.BloodyCharge_Start, 1f),
                        new Tuple<AIState, float>(AIState.BloodRain_Start, 1f),
                        new Tuple<AIState, float>(AIState.BloodBoil_Start, 1f),
                        new Tuple<AIState, float>(AIState.BloodCrack_Start, 1f),
                        new Tuple<AIState, float>(AIState.GhastlyBloodDash_Start, 1f));
                }
                return _patternManagerBacking;
            }
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            base.SendExtraAI(writer);
            writer.WriteVector2(_teleportPosition);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            base.ReceiveExtraAI(reader);
            _teleportPosition = reader.ReadVector2();
        }
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Main.npcFrameCount[NPC.type] = 28;
            NPCID.Sets.TrailCacheLength[NPC.type] = 16;
            NPCID.Sets.TrailingMode[Type] = 3;
            NPCID.Sets.MPAllowedEnemies[NPC.type] = true;
            NPCID.Sets.BossBestiaryPriority.Add(Type);
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            NPC.width = 128;
            NPC.height = 200;
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

            Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/SanguineSingularity");
            NPC.HitSound = new SoundStyle("Stellamod/Assets/Sounds/Gintze_Hit") with { PitchVariance = 0.1f, Pitch = -0.5f, Volume = 0.2f };
            NPC.DeathSound = new SoundStyle("Stellamod/Assets/Sounds/Gintze_Death") with { PitchVariance = 0.1f, Pitch = -0.5f, Volume = 0.2f };
        }

        private void SwitchState(AIState state)
        {
            if (MultiplayerHelper.IsHost)
            {
                State = state;
                Timer = 0;
                NPC.netUpdate = true;
            }
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            return base.CanHitPlayer(target, ref cooldownSlot) && _contactDamage;
        }

        public override void AI()
        {
            base.AI();
            _draw.outlineColor = Color.Lerp(_draw.outlineColor, TargetOutlineColor, 0.1f);
            if (!NPC.HasValidTarget)
            {
                NPC.TargetClosest();
                if (!NPC.HasValidTarget && State != AIState.Despawn)
                {
                    SwitchState(AIState.Despawn);
                }
            }

            if (_teleportPosition != Vector2.Zero)
            {
                NPC.position.X = _teleportPosition.X;
                NPC.position.Y = _teleportPosition.Y;
                NPC.velocity = Vector2.Zero;
                _teleportPosition = Vector2.Zero;
            }
            _draw.flashAlpha *= 0.99f;
            if (_draw.headless)
            {
                GoreManager.orbitingRadius = 60f;
                GoreManager.draw = true;
            }
            else
            {
                GoreManager.orbitingRadius = 0f;
                if (GoreManager.HasTinyOrbit())
                {
                    GoreManager.draw = false;
                }
            }
                GoreManager.Update(NPC.Center);
            NPC.spriteDirection = NPC.direction;
            switch (State)
            {
                case AIState.Spawn:
                    AI_Spawn();
                    break;
                case AIState.Idle:
                    AI_Idle();
                    break;
                case AIState.Despawn:
                    AI_Despawn();
                    break;
                case AIState.Death:
                    AI_Death();
                    break;


                case AIState.BloodyBurst_Start:
                    AI_BloodyBurst_Start();
                    break;
                case AIState.BloodyBurst:
                    AI_BloodyBurst();
                    break;
                case AIState.BloodyBurst_End:
                    AI_BloodyBurst_End();
                    break;


                case AIState.BloodyCharge_Start:
                    AI_BloodyCharge_Start();
                    break;
                case AIState.BloodyCharge_Rush:
                    AI_BloodyCharge_Rush();
                    break;
                case AIState.BloodyCharge_End:
                    AI_BloodyCharge_End();
                    break;


                case AIState.BloodRain_Start:
                    AI_BloodRain_Start();
                    break;
                case AIState.BloodRain_Rain:
                    AI_BloodRain_Rain();
                    break;
                case AIState.BloodRain_End:
                    AI_BloodRain_End();
                    break;


                case AIState.BloodBoil_Start:
                    AI_BloodBoil_Start();
                    break;
                case AIState.BloodBoil_Walk:
                    AI_BloodBoil_Walk();
                    break;
                case AIState.BloodBoil_End:
                    AI_BloodBoil_End();
                    break;


                case AIState.BloodCrack_Start:
                    AI_BloodCrack_Start();
                    break;
                case AIState.BloodCrack_Geyser:
                    AI_BloodCrack_Geyser();
                    break;
                case AIState.BloodCrack_End:
                    AI_BloodCrack_End();
                    break;


                case AIState.GhastlyBloodDash_Start:
                    AI_GhastlyBloodDash_Start();
                    break;
                case AIState.GhastlyBloodDash_Run:
                    AI_GhastlyBloodDash_Run();
                    break;
                case AIState.GhastlyBloodDash_End:
                    AI_GhastlyBloodDash_End();
                    break;

                case AIState.Phase2Transition:
                    AI_Phase2Transition();
                    break;
            }
        }


        private void AI_Spawn()
        {
            Timer++;
            if (Timer == 1)
            {
                NPC.TargetClosest();
     
            }

            _scale = Vector2.One;
            _draw.alpha = MathHelper.Lerp(0f, 1f, EasingFunction.InOutSine(Timer / 60f));
            _draw.afterImageAlpha = MathHelper.Lerp(0f, 1f, EasingFunction.InOutSine(Timer / 60f));

            NPC.rotation = 0;
            NPC.noGravity = true;
            NPC.noTileCollide = true;

            if (Timer == 1)
            {
                SoundStyle spawnSound = AssetRegistry.Sounds.SanguineSingularity.SanguinePreBurst;
                spawnSound.PitchVariance = 0.1f;
                SoundEngine.PlaySound(spawnSound, NPC.position);
            }


            NPC.velocity.Y = MathHelper.Lerp(-1f, 1f, MathF.Sin(Timer * 0.02f) * 0.5f + 0.5f);
            RetargetCameraModifier.ReTargetPosition = NPC.Center;
            if (Timer == 200)
            {
                CreateRedFlash();
                FXUtil.ShakeCamera(NPC.Center, 1024, 8);
                SoundStyle spawnSound = AssetRegistry.Sounds.SanguineSingularity.SanguineSpawn;
                SoundEngine.PlaySound(spawnSound, NPC.position);
            }

            if (Timer >= 200 && Timer < 300)
            {
                NPC.velocity *= 0.9f;
                _animation = AnimationState.Idle;
                ShakeModSystem.Shake = 8;
                if (Timer % 9 == 0)
                {
                    Particle.NewParticle<ShockParticle>(NPC.Center, Vector2.Zero, Color.White);
                }
                if (Timer % 4 == 0)
                {
                    Vector2 velocity = Main.rand.NextVector2Circular(32, 32);
                    Dust.NewDustPerfect(NPC.Center, ModContent.DustType<GlowDust>(), velocity, newColor: Color.Red, Scale: Main.rand.NextFloat(0.5f, 1.5f));
                }
            }
            else if (Timer < 200)
            {
                _animation = AnimationState.Walk;
                Vector2 targetVelocity = (MyTarget.Center - NPC.Center).SafeNormalize(Vector2.Zero);
                NPC.velocity = Vector2.Lerp(NPC.velocity, targetVelocity, 0.1f);
                NPC.direction = TargetDirection;
            }

            if (Timer >= 400)
            {
                SwitchState(AIState.Idle);
            }
        }


        private void AI_Despawn()
        {
            Timer++;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.velocity.X *= 0.5f;
            NPC.velocity.Y -= 0.5f;
            TargetOutlineColor = Color.Transparent;
            if (Timer >= 100)
            {
                NPC.active = false;
            }
        }

        private void ChooseAttack()
        {
            SwitchState(PatternManager.NextPattern());
            SwitchState(AIState.BloodyBurst_Start);
        }

        private void AI_Idle()
        {
            Timer++;
            if (Timer == 1)
            {
                NPC.TargetClosest();
            }


            _contactDamage = false;
            _scale = Vector2.One;
            _draw.alpha = MathHelper.Lerp(_draw.alpha, 1f, 0.1f);
            _draw.afterImageAlpha *= 0.5f;

            AttackNumber = 0;
            NPC.direction = TargetDirection;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            if (Timer > 60)
            {
                _animation = AnimationState.Walk;
                Vector2 directionToTarget = (MyTarget.Center - NPC.Center);
                directionToTarget = directionToTarget.SafeNormalize(Vector2.Zero);
                NPC.velocity = Vector2.Lerp(NPC.velocity, directionToTarget, 0.1f);
                NPC.rotation *= 0.5f;
            }
            else
            {
                _animation = AnimationState.Idle;
            }

            TargetOutlineColor = Color.Transparent;
            if (Timer >= 200)
            {
                ChooseAttack();
            }
        }


        public override void HitEffect(NPC.HitInfo hit)
        {
            base.HitEffect(hit);
            if (NPC.life <= 0 && State != AIState.Death)
            {
                NPC.life = 1;
                SwitchState(AIState.Death);
            }


            if (NPC.life <= 0)
            {
                NPC.life = 1;
            }
        }

        private void AI_Death()
        {
            Timer++;
            if (Timer == 1)
            {
                NPC.TargetClosest();
            }

            _animation = AnimationState.Walk;
            _draw.alpha = MathHelper.Lerp(_draw.alpha, 1f, 0.1f);
            _draw.afterImageAlpha *= 0.9f;

            //I think for this I want to slow down the movement and make it violently shake before it explodes into bloody geysers
            //Yes this can kill you (lol)
            //ok, so first slow down the velocity I think?
            NPC.velocity.X *= 0.99f;
            NPC.velocity.Y *= 0.99f;


            //Slowly hover upwards I think that'd be cool
            if (NPC.velocity.Y < -1f)
                NPC.velocity.Y -= 0.1f;
            NPC.rotation = NPC.velocity.X * 0.025f;
            RetargetCameraModifier.ReTargetPosition = NPC.Center;
            TargetOutlineColor = Color.Yellow;


            //Shake the goober around quite a bit
            //Maybe emit some dusts?
            if (Timer % 5 == 0)
            {
                Vector2 velocity = Main.rand.NextVector2Circular(16, 16);
                Color color = Color.Red;
                Dust.NewDustPerfect(NPC.Center, ModContent.DustType<GlowDust>(), newColor: color,
                    Velocity: velocity,
                    Scale: Main.rand.NextFloat(0.5f, 1f)); ;
            }

            float deathTime = 200;
            float completionRatio = Timer / deathTime;
            float shakeAmount = MathHelper.Lerp(0f, 16, completionRatio);
            Vector2 shake = Main.rand.NextVector2Circular(shakeAmount, shakeAmount);
            _draw.shake = shake;
            if (Timer == deathTime)
            {
                if (MultiplayerHelper.IsHost)
                {
                    for (float n = 0; n < 8; n++)
                    {
                        Vector2 vel = Main.rand.NextVector2Circular(16, 16);
                        Projectile.NewProjectile(SourceFromThis, NPC.Center, vel, ModContent.ProjectileType<BloodGeyser>(), BloodGeyserDamage, 1, Main.myPlayer);
                    }
                }

                var p = FXUtil.GlowCircleBoom(NPC.Center, Color.Red, Color.DarkBlue, Color.Black);
                var p2 = FXUtil.GlowCircleBoom(NPC.Center, Color.Red, Color.DarkBlue, Color.Black);
                p.Scale *= 3;
                p2.Scale *= 2;

                for (float f = 0; f < 16; f++)
                {
                    Vector2 vel = Main.rand.NextVector2Circular(16, 16);
                    Dust.NewDustPerfect(NPC.Center, ModContent.DustType<GlowDust>(), vel,
                        newColor: Color.Red,
                        Scale: Main.rand.NextFloat(0.5f, 3f));
                }

                SoundStyle deathSound = AssetRegistry.Sounds.SanguineSingularity.SanguineDeath;
                SoundEngine.PlaySound(deathSound, NPC.position);
                FXUtil.ShakeCamera(NPC.Center, 1024, 8);
                ShakeModSystem.Shake = 9;
                NPC.Kill();
            }
        }

        public override void OnKill()
        {
            base.OnKill();
            DownedBossTracker.ClearFlag(DownedBossFlag.SanguineSingularity);
        }

        #region Bloody Burst
        private void CreateRedFlash()
        {
            _draw.flashAlpha = 1f;
        }

        private void AI_BloodyBurst_Start()
        {
            Timer++;
            if (Timer == 1)
            {
                NPC.TargetClosest();

                SoundStyle telegraphSound = AssetRegistry.Sounds.SanguineSingularity.SanguinePreBurst;
                telegraphSound.PitchVariance = 0.2f;
                SoundEngine.PlaySound(telegraphSound, NPC.position);
            }

            //- Opens the fight with several exploding blood magic projectiles
            //- That loosely track the player


            if(Timer == 1)
            {
                Vector2 velocityToPlayer = (MyTarget.Center - NPC.Center);
                velocityToPlayer = velocityToPlayer.SafeNormalize(Vector2.Zero);
                _runDirection = velocityToPlayer;
                NPC.direction = TargetDirection;
            }

            float time = MathHelper.Lerp(60f, 30f, AttackNumber / 6f);
            float lerp = Timer / time;
            float ease = EasingFunction.Anticipation(lerp);

   
            NPC.velocity = Vector2.Lerp(-_runDirection, _runDirection * 22, ease);
            NPC.rotation = NPC.velocity.X * 0.025f;
            NPC.noTileCollide = true;
            NPC.noGravity = true;

            _contactDamage = false;
            _animation = AnimationState.Walk;
            _draw.alpha = MathHelper.Lerp(_draw.alpha, 1f, 0.1f);
            _scale = Vector2.Lerp(new Vector2(1.2f, 1.2f), Vector2.One, ease);
            if (Timer < 30f)
            {
                _draw.afterImageAlpha = MathHelper.Lerp(_draw.afterImageAlpha, 0f, 0.1f);
                _animation = AnimationState.Walk;
            }
            else
            {
                _draw.afterImageAlpha = MathHelper.Lerp(_draw.afterImageAlpha, 1f, 0.1f);
                _animation = AnimationState.Run;
            }

            if (Timer == (int)(time))
            {
                CreateRedFlash();
                SoundStyle sanguineBurstReady = AssetRegistry.Sounds.SanguineSingularity.SanguineBurstReady;
                sanguineBurstReady.PitchVariance = 0.3f;
                SoundEngine.PlaySound(sanguineBurstReady, NPC.position);
                FXUtil.ShakeCamera(NPC.Center, 1024, 8);
                ShakeModSystem.Shake = 2;
            }

            TargetOutlineColor = Color.Yellow;
            if (Timer >= time + 10f)
            {
                SwitchState(AIState.BloodyBurst);
            }
        }


        private void AI_BloodyBurst()
        {
            Timer++;
            float ease = EasingFunction.InOutSine(Timer / 60f);
            _draw.afterImageAlpha = MathHelper.Lerp(0f, 1f, ease);
            _draw.alpha = MathHelper.Lerp(1f, 0f, ease);
            _animation = AnimationState.Run;
            _contactDamage = true;
            TargetOutlineColor = Color.Red;
            if (NPC.velocity.Y > -3)
                NPC.velocity.Y -= 0.5f;

            if (Timer == 15f)
            {
                CreateRedFlash();
            }

            if(Timer >= 15f && Timer < 30f)
            {
                NPC.velocity *= 1.01f;
            }

            NPC.rotation *= 0.99f;
            NPC.noTileCollide = true;
            NPC.noGravity = true;
            if (Timer == 30f)
            {
                if (MultiplayerHelper.IsHost)
                {
                    Vector2 upVelocity = -Vector2.UnitY * Main.rand.NextFloat(3f, 6f);
                    upVelocity = upVelocity.RotatedByRandom(MathHelper.ToRadians(35));
                    int bloodyBurstProjectile = ModContent.ProjectileType<BloodyBurst>();
                    Projectile.NewProjectile(SourceFromThis, NPC.Center, upVelocity, bloodyBurstProjectile, BloodyBurstDamage, 1, Main.myPlayer);
                }

                for (float f = 0f; f < 16; f++)
                {
                    Vector2 velocity = Main.rand.NextVector2Circular(32, 32);
                    Dust.NewDustPerfect(NPC.Center, ModContent.DustType<GlowDust>(), velocity, newColor: Color.Red, Scale: Main.rand.NextFloat(1f, 3f));
                }

                SoundStyle burstSound = AssetRegistry.Sounds.SanguineSingularity.SanguineBurst;
                burstSound.PitchVariance = 0.3f;
                SoundEngine.PlaySound(burstSound, NPC.position);


                ShakeModSystem.Shake = 4;
                FXUtil.ShakeCamera(NPC.Center, 1024, 8);
                for (float f = 0f; f < 4f; f++)
                {
                    Vector2 velocity = Main.rand.NextVector2Circular(3, 3);
                    var boom = FXUtil.GlowCircleBoom(NPC.Center, Color.Red, Color.DarkBlue, Color.Black);
                    boom.Scale *= Main.rand.NextFloat(1.5f, 2f);
                    boom.Velocity = velocity;

                    var stretch = FXUtil.GlowStretch(NPC.Center, velocity * 4);
                    stretch.color = Color.Red;
                    stretch.OuterGlowColor = Color.Red;
                }
            }


            if (Timer >= 30f)
            {
                SwitchState(AIState.BloodyBurst_End);
            }
        }

        private void AI_BloodyBurst_End()
        {
            Timer++;
            if (Timer == 1)
            {
                NPC.TargetClosest();
            }

            _draw.alpha *= 0.9f;
            _draw.afterImageAlpha *= 0.9f;
            _animation = AnimationState.Run;
            TargetOutlineColor = Color.Transparent;
            NPC.velocity *= 0.9f;
            NPC.rotation = NPC.velocity.X * 0.025f;
            NPC.noTileCollide = true;
            NPC.noGravity = true;
            if (MultiplayerHelper.IsHost)
            {
                float direction = AttackNumber % 2 == 0 ? -1 : 1;
                _teleportPosition = MyTarget.Center - new Vector2(500 * direction, 0);
                NPC.netUpdate = true;
            }

            if (Timer == 30)
            {
                AttackNumber++;
                if (AttackNumber >= 6)
                {
                    SwitchState(AIState.Idle);
                }
                else
                {
                    SwitchState(AIState.BloodyBurst_Start);
                }
            }
        }

        #endregion


        #region Bloody Charge
        private void AI_BloodyCharge_Start()
        {
            // -Winds up a charge and then runs directly at the player really fast,
            // and explodes into bloody bits before merging itself back together elsewhere

            Timer++;
            if (Timer == 1)
            {
                CreateRedFlash();
                NPC.TargetClosest();
                SoundStyle preDash = AssetRegistry.Sounds.SanguineSingularity.SanguinePreBurst;
                SoundEngine.PlaySound(preDash, NPC.position);
            }

            _draw.alpha = MathHelper.Lerp(_draw.alpha, 1f, 0.1f);
            _draw.afterImageAlpha *= 0.5f;
            _animation = AnimationState.Run;


            Vector2 directionToPlayer = (MyTarget.Center - NPC.Center);
            float chargeUpTime = 80f;
            float ease = EasingFunction.Anticipation2(Timer / chargeUpTime);
            Vector2 velocity = Vector2.Lerp(-directionToPlayer, directionToPlayer * 5f, ease);

            TargetOutlineColor = Color.Yellow;
            NPC.velocity = velocity;
            NPC.rotation = NPC.velocity.X * 0.025f;
            if (Timer >= chargeUpTime)
            {
                SwitchState(AIState.BloodyCharge_Rush);
            }
        }

        private void AI_BloodyCharge_Rush()
        {
            Timer++;
            if (Timer == 1)
            {
                CreateRedFlash();
                NPC.TargetClosest();
                SoundStyle chargeSound = AssetRegistry.Sounds.SanguineSingularity.SanguineCharge;
                SoundEngine.PlaySound(chargeSound, NPC.position);
            }

            _draw.alpha = MathHelper.Lerp(_draw.alpha, 1f, 0.1f);
            _draw.afterImageAlpha = MathHelper.Lerp(0f, 1f, EasingFunction.InOutSine(Timer / 30f));
            _animation = AnimationState.Run;

            TargetOutlineColor = Color.Red;
            NPC.velocity *= 0.5f;
            NPC.rotation = NPC.velocity.X * 0.025f;
            if (Timer == 30)
            {
                if (MultiplayerHelper.IsHost)
                {
                    Projectile.NewProjectile(SourceFromThis, NPC.Center, Vector2.Zero,
                        ModContent.ProjectileType<BloodyChargeBoom>(), BloodyChargeBoomDamage, 1, Main.myPlayer);
                }
            }

            if (Timer >= 60f)
            {
                SwitchState(AIState.BloodyCharge_End);
            }
        }

        private void AI_BloodyCharge_End()
        {
            _draw.alpha = MathHelper.Lerp(_draw.alpha, 1f, 0.1f);
            _draw.afterImageAlpha *= 0.9f;
            _animation = AnimationState.Walk;

            Timer++;
            if (Timer % 5 == 0)
            {
                Color color = Color.White;
                Vector2 velocity = Main.rand.NextVector2Circular(24, 24);
                Particle.NewBlackParticle<BloodSparkleParticle>(NPC.Center, velocity, color, Scale: Main.rand.NextFloat(0.5f, 3f));
            }

            TargetOutlineColor = Color.Transparent;
            NPC.velocity.X *= 0.5f;
            NPC.rotation = NPC.velocity.X * 0.025f;
            if (Timer >= 60f)
            {
                SwitchState(AIState.Idle);
            }
        }

        #endregion


        #region Blood Rain
        private void AI_BloodRain_Start()
        {
            Timer++;
            if (Timer == 1)
            {
                NPC.TargetClosest();
            }

            float chargeUpTime = 60f;
            float ease = EasingFunction.Anticipation(Timer / chargeUpTime);
            float siningEase = EasingFunction.InOutSine(Timer / chargeUpTime);

            _animation = AnimationState.Walk;
            _draw.alpha = MathHelper.Lerp(1f, 0f, siningEase);
            _draw.afterImageAlpha = MathHelper.Lerp(0f, 1f, siningEase);

            NPC.noGravity = true;
            NPC.noTileCollide = true;

            float yVelocity = MathHelper.Lerp(2f, -4f, ease);
            NPC.velocity.Y = yVelocity;
            NPC.velocity.X *= 0.9f;
            NPC.rotation = NPC.velocity.X * 0.025f;
            if (Timer >= chargeUpTime)
            {
                SwitchState(AIState.BloodRain_Rain);
            }
        }

        private void AI_BloodRain_Rain()
        {
            Timer++;
            if (Timer == 1)
            {
                NPC.TargetClosest();
                SoundStyle laughSound = AssetRegistry.Sounds.SanguineSingularity.SanguineLaugh;
                laughSound.PitchVariance = 0.15f;
                SoundEngine.PlaySound(laughSound, NPC.position);
            }

            _draw.alpha = 0f;
            _draw.afterImageAlpha *= 0.9f;
            _animation = AnimationState.Walk;

            NPC.velocity = Vector2.Zero;
            NPC.rotation = 0;
            if (Timer % 20 == 0)
            {
                if (MultiplayerHelper.IsHost)
                {
                    Vector2 spawnPoint = MyTarget.Center + new Vector2(0, -1000);
                    spawnPoint.X = Main.rand.NextFloat(-1000f, 1000f);
                    Projectile.NewProjectile(SourceFromThis, spawnPoint, Vector2.Zero,
                        ModContent.ProjectileType<BloodRain>(), BloodRainDamage, 1, Main.myPlayer);
                }
                AttackNumber++;
            }

            if (AttackNumber >= 30)
            {
                SwitchState(AIState.BloodRain_End);
            }
        }

        private void AI_BloodRain_End()
        {
            Timer++;
            if (Timer == 1)
            {
                NPC.TargetClosest();
                if (MultiplayerHelper.IsHost)
                {
                    _teleportPosition = MyTarget.Center + new Vector2(-500, 0);
                    NPC.netUpdate = true;
                }
            }
            _animation = AnimationState.Walk;

            float time = 60f;
            _draw.alpha = MathHelper.Lerp(0f, 1f, EasingFunction.InOutSine(Timer / time));
            NPC.velocity.X = MathHelper.Lerp(3f, 0f, EasingFunction.InOutSine(Timer / time));
            if (Timer >= time)
            {
                SwitchState(AIState.Idle);
            }
        }

        #endregion


        #region Blood Boil
        private void AI_BloodBoil_Start()
        {
            //Walks slowly around the player as bloody boils explode from its body and then home back towards you
            Timer++;
            if (Timer == 1)
            {
                NPC.TargetClosest();
            }

            _animation = AnimationState.Walk;
            TargetOutlineColor = Color.Yellow;
            NPC.velocity.X *= 0.9f;
            NPC.velocity.Y *= 0.95f;
            NPC.rotation = NPC.velocity.X * 0.025f;

            if (Timer >= 30)
            {
                SwitchState(AIState.BloodBoil_Walk);
            }
        }

        private void AI_BloodBoil_Walk()
        {
            Timer++;
            if (Timer == 1)
            {
                NPC.TargetClosest();
            }
            NPC.noTileCollide = true;
            NPC.noGravity = true;

            _animation = AnimationState.Walk;
            TargetOutlineColor = Color.Red;
        }

        private void AI_BloodBoil_End()
        {
            Timer++;
            if (Timer == 1)
            {
                NPC.TargetClosest();
            }

            _animation = AnimationState.Walk;
            if (Timer >= 60)
            {
                SwitchState(AIState.Idle);
            }
        }
        #endregion


        #region Blood Crack
        private void AI_BloodCrack_Start()
        {
            Timer++;
            if (Timer == 1)
            {
                NPC.TargetClosest();
            }

            if (Timer >= 60)
            {
                SwitchState(AIState.BloodCrack_Geyser);
            }
        }

        private void CreateGeyser()
        {
            if (MultiplayerHelper.IsHost)
            {
                Vector2 velocity = Main.rand.NextVector2CircularEdge(800, 800);
                Projectile.NewProjectile(SourceFromThis, NPC.Center, velocity,
                    ModContent.ProjectileType<BloodGeyser>(), BloodGeyserDamage, 1, Main.myPlayer);
            }
        }

        private void AI_BloodCrack_Geyser()
        {
            Timer++;
            if (Timer == 1)
            {
                NPC.TargetClosest();
            }

            if (Timer % 60 == 0)
            {
                NPC.velocity = Vector2.Zero;
                AttackNumber++;
                CreateGeyser();
            }

            if (AttackNumber >= 6)
            {
                SwitchState(AIState.BloodCrack_End);
            }
        }

        private void AI_BloodCrack_End()
        {
            Timer++;
            if (Timer == 1)
            {
                NPC.TargetClosest();
            }
            SwitchState(AIState.Idle);
        }
        #endregion

        #region Ghastly Blood Dash

        private void AI_GhastlyBloodDash_Start()
        {

        }

        private void AI_GhastlyBloodDash_Run()
        {

        }

        private void AI_GhastlyBloodDash_End()
        {

        }

        #endregion

        private void AI_Phase2Transition()
        {

        }


        #region Drawcode
        private void DrawAfterImage(SpriteBatch spriteBatch, Vector2 screenPos)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
      
            Rectangle frame = NPC.frame;
            for (int i = 0; i < NPC.oldPos.Length; i++)
            {
                Vector2 oldPos = NPC.oldPos[i];
                Vector2 oldDrawPos = oldPos - screenPos;
                Vector2 drawOrigin = new Vector2(128, 64);
                float f = i;
                float interpolant = f / (float)NPC.oldPos.Length;
                Color fadeColor = Color.Lerp(Color.Red, Color.Transparent, interpolant);
                fadeColor *= _draw.afterImageAlpha;
                oldDrawPos += NPC.Size / 2f;
                fadeColor *= 0.1f;

                SpriteEffects spriteEffects = NPC.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
                if (spriteEffects == SpriteEffects.FlipHorizontally)
                    drawOrigin.X = (frame.Width - drawOrigin.X);

                spriteBatch.Draw(texture, oldDrawPos, NPC.frame, fadeColor, NPC.oldRot[i], drawOrigin, NPC.scale, spriteEffects, 0f);
            }
        }


        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            DrawAfterImage(spriteBatch, screenPos);
            DrawSingularity(spriteBatch, screenPos, drawColor);
            Draw(spriteBatch, screenPos, drawColor);
            DrawRedFlash(spriteBatch, screenPos, drawColor);
            GoreManager.Draw(spriteBatch, screenPos, drawColor);
            return false;
        }

        private void DrawRedFlash(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawCenter = NPC.Center - screenPos;
            Rectangle frame = NPC.frame;
            Vector2 drawOrigin = new Vector2(128, 64);
            SpriteEffects spriteEffects = NPC.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            if (spriteEffects == SpriteEffects.FlipHorizontally)
                drawOrigin.X = (frame.Width - drawOrigin.X);

            Color glowColor = Color.Red;
            glowColor.A = 0;
            glowColor *= _draw.flashAlpha;
            spriteBatch.Draw(texture, drawCenter, frame, glowColor, NPC.rotation, drawOrigin, _scale, spriteEffects, 0);

        }
        private void DrawSingularity(SpriteBatch spriteBatch, Vector2 screenPos, Color color)
        {
            Texture2D texture = ModContent.Request<Texture2D>(TextureRegistry.EmptyBigTexture).Value;
            Vector2 drawPosition = NPC.Center - screenPos;
            Vector2 drawOrigin = texture.Size() / 2f;
            Vector2 drawScale = NPC.scale * Vector2.One;
            drawScale *= _draw.singularityScale;

            var shader = SingularityShader.Instance;
            spriteBatch.Restart(effect: shader.Effect);

            Color redSingularity = Color.Red;
            redSingularity *= _draw.alpha;
            spriteBatch.Draw(texture, drawPosition, null, redSingularity, NPC.rotation, drawOrigin, drawScale * 0.25f, SpriteEffects.None, 0);
            spriteBatch.RestartDefaults();


            Texture2D diskTexture = ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/SF").Value;
            Vector2 diskDrawOrigin = diskTexture.Size() / 2f;
            Color diskDrawColor = Color.Lerp(Color.White, Color.Lerp(Color.White, Color.Red, 0.15f), ExtraMath.Osc(0f, 1f, speed: 2));
            diskDrawColor = diskDrawColor.MultiplyRGB(Color.Red);
            diskDrawColor.A = 0;
            diskDrawColor *= _draw.alpha;

            float scaleOsc = ExtraMath.Osc(0.5f, 0.65f, speed: 1);
            spriteBatch.Draw(diskTexture, drawPosition, null, diskDrawColor, NPC.rotation, diskDrawOrigin, drawScale * 0.5f * scaleOsc, SpriteEffects.None, 0);
            spriteBatch.Draw(diskTexture, drawPosition, null, diskDrawColor, NPC.rotation, diskDrawOrigin, drawScale * 0.4f * scaleOsc, SpriteEffects.None, 0);
        }

        private int _frame;
        public override void FindFrame(int frameHeight)
        {
            base.FindFrame(frameHeight);
            NPC.frameCounter += 0.2f;
            if (NPC.frameCounter >= 1f)
            {
                _frame++;
                NPC.frameCounter = 0f;
            }

            switch (_animation)
            {
                case AnimationState.Idle:
                    if (_headless)
                    {
                        _frame = 1;
                    }
                    else
                    {
                        _frame = 0;
                    }

                    break;
                case AnimationState.Walk:
                    if (_headless)
                    {
                        if (_frame < 10)
                        {
                            _frame = 10;
                        }
                        else if (_frame >= 18)
                        {
                            _frame = 10;
                        }
                    }
                    else
                    {
                        if (_frame < 2)
                        {
                            _frame = 2;
                        }
                        else if (_frame >= 10)
                        {
                            _frame = 2;
                        }
                    }

                    break;
                case AnimationState.Run:
                    if (_headless)
                    {
                        if (_frame < 23)
                        {
                            _frame = 23;
                        }
                        else if (_frame >= 28)
                        {
                            _frame = 23;
                        }
                    }
                    else
                    {
                        if (_frame < 18)
                        {
                            _frame = 18;
                        }
                        else if (_frame >= 23)
                        {
                            _frame = 18;
                        }
                    }
                    break;
            }
            NPC.frame.Y = frameHeight * _frame;
        }


        private void DrawSprite(SpriteBatch spriteBatch, Vector2 screenPos, Color color)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawCenter = NPC.Center - screenPos;
            Rectangle frame = NPC.frame;
            Vector2 drawOrigin = new Vector2(128, 64);
            SpriteEffects spriteEffects = NPC.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            if (spriteEffects == SpriteEffects.FlipHorizontally)
                drawOrigin.X = (frame.Width - drawOrigin.X);
            spriteBatch.Draw(texture, drawCenter, frame, color * _draw.alpha, NPC.rotation, drawOrigin, _scale, spriteEffects, 0);
        }

        private void Draw(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
        {
            DrawSprite(spriteBatch, screenPos, Color.White.MultiplyRGB(lightColor));
        }

        private void DrawOutline(SpriteBatch spriteBatch, Vector2 screenPos)
        {
            DrawSprite(spriteBatch, screenPos, _draw.outlineColor);
        }

        public void DrawOutlines(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
        {
            float outlineOffset = 2;
            Vector2 v = Vector2.UnitX * outlineOffset;
            Vector2 h = Vector2.UnitY * outlineOffset;
            DrawOutline(spriteBatch, screenPos + v);
            DrawOutline(spriteBatch, screenPos - v);
            DrawOutline(spriteBatch, screenPos + h);
            DrawOutline(spriteBatch, screenPos - h);
        }

        #endregion
    }
}
