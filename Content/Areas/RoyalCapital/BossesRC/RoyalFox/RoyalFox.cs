
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json.Linq;
using ReLogic.Content;
using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Content.Areas.Ishtar.BossesIS.SanguineSingularity;
using Stellamod.Core;
using Stellamod.Core.Particles;
using Stellamod.Core.Pixelation;
using Stellamod.Core.Utilities;
using Stellamod.Effects.RoyalMagic;
using Stellamod.Helpers;
using Stellamod.Trails;
using Stellamod.Visual.Particles;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.RoyalCapital.BossesRC.RoyalFox
{

   
    public class DashLine : ModProjectile,
        IDrawToRenderTarget
    {
        private float DeathTime => 25;
        private ref float Timer => ref Projectile.ai[0];
        private ref float IsUsed => ref Projectile.ai[1];
        private ref float DeathTimer => ref Projectile.ai[2];
        public override string Texture => TextureRegistry.EmptyTexture;
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.timeLeft = 300;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
        }

        public override void AI()
        {
            base.AI();
            Timer++;
            if(IsUsed == 1)
            {
                IsUsed = 2;
                Projectile.netUpdate = true;
            }
            if (IsUsed == 2)
            {
                DeathTimer++;
                if(DeathTimer >= 25f)
                {
                    Projectile.Kill();
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }

        private void DrawLine(SpriteBatch sb, Vector2 screenPos)
        {
            float alpha = EasingFunction.OutSine(Timer / 60) * MathHelper.Lerp(1f, 0f, DeathTimer / DeathTime);
            SpritebatchDrawer lineDrawer = SpritebatchDrawer.FromTextureAsset(ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/RayLight4"), Projectile.Center - Projectile.velocity * 1200);
            lineDrawer.color = Color.Lerp(Color.Black, Color.White, alpha);
            lineDrawer.color.A = 0;
            lineDrawer.rotation = Projectile.velocity.ToRotation();
            lineDrawer.LeftCenterOrigin();

            Vector2 scale = Vector2.Lerp(new Vector2(0f, 1f), new Vector2(2f, 1f), alpha);
            scale.Y = 0.5f;
            scale.X *= 12;
            lineDrawer.scale = scale;
            sb.Draw(lineDrawer);
        }
        private Color StarryTrailColorFunction(float completionRatio)
        {

            return Color.Lerp(Color.White, Color.Transparent, completionRatio) *
                MathHelper.Lerp(0f, 1f, EasingFunction.Clamp((float)Projectile.timeLeft / 30f)) * EasingFunction.QuadraticBump(DeathTimer / DeathTime);
        }

        private float StarryTrailWidthFunction(float completionRatio)
        {
            return MathHelper.SmoothStep(80, 0, completionRatio);
        }

        private void RenderStarryDashTrail(GraphicsDevice gDevice)
        {
            List<Vector2> points = new List<Vector2>();
            float numPoints = 24;
            Vector2 endPoint = Vector2.Lerp(Projectile.Center - Projectile.velocity * 1000, Projectile.Center + Projectile.velocity * 1200, DeathTimer / DeathTime);
            Vector2 startPoint = endPoint - Projectile.velocity * 3500;
            for(float f = 0; f < numPoints; f++)
            {
                Vector2 p = Vector2.Lerp(endPoint, startPoint, f / numPoints);
                points.Add(p);
            }
            Vector2[] trailPoints = points.ToArray();
            FixedRichLaserShader laserShader = ShaderContent.GetInstance<FixedRichLaserShader>();
            laserShader.LaserTexture = TrailRegistry.Beamlight;
            laserShader.InnerColor = Color.White;
            laserShader.OuterColor = Color.Lerp(Color.White, Color.SkyBlue, ExtraMath.Osc(0f, 1f, speed: 16));
            TrailDrawer.Draw(Main.spriteBatch, trailPoints, StarryTrailColorFunction, StarryTrailWidthFunction, laserShader);
        }

        public void DrawToRenderTargets()
        {
            PixelationManager.QueueSpritebatchDrawAction(DrawLine);
            if (DeathTimer <= 0)
                return;

            PixelationManager.QueuePrimitivesDrawAction(RenderStarryDashTrail);
            //  throw new NotImplementedException();
        }
    }

    public class RoyalMagicMiniStar : ModProjectile,
        IDrawToRenderTarget
    {
        private Vector2 _targetPosition;
        private ref float Timer => ref Projectile.ai[0];
        private ref float Mode => ref Projectile.ai[1];
        private ref float FlashTimer => ref Projectile.ai[2];
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            ProjectileID.Sets.TrailCacheLength[Type] = 24;
            ProjectileID.Sets.TrailingMode[Type] = 2;
            Main.projFrames[Type] = 2;
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.timeLeft = 360;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
        }

        public override bool ShouldUpdatePosition()
        {
            return base.ShouldUpdatePosition();
        }

        public override void AI()
        {
            base.AI();
            Timer++;
            if(FlashTimer > 0)
            {
                FlashTimer--;
            }


            if(Mode == 1)
            {
                Timer = 0;
                Mode = 2;
                FlashTimer = 30;
                Projectile.netUpdate = true;
            }

            if(Mode == 2)
            {
                if (Main.rand.NextBool(4))
                {
                    var dp = DustParticle.Spawn(Projectile.Center, Main.rand.NextVector2Circular(4, 4));
                    dp.outerColor = Color.DarkViolet;
                    dp.gravity = 0;
                    dp.noTileCollide = true;
                    dp.dampening = 0.05f;
                }
                Projectile.hostile = true;
                if(Timer < 30)
                {
                    Player closest = PlayerHelper.FindClosestPlayer(Projectile.Center, 2048);
                    if(closest != null)
                    {
                        _targetPosition = closest.Center;
                    }
                  
                }

                Vector2 vel = (_targetPosition - Projectile.Center).SafeNormalize(Vector2.Zero) * 35;
                Projectile.velocity = Projectile.velocity.MoveTowards(vel, MathHelper.Lerp(0f, 1f, Timer / 30f));
                if(Timer >= 70)
                {
                    Projectile.Kill();
                }
            }
            else
            {
                Projectile.velocity *= 0.96f;
            }
            Projectile.scale = ExtraMath.Osc(0.5f, 0.75f, speed: 3, Projectile.whoAmI);
        }

        private Color StarryTrailColorFunction(float completionRatio)
        {
            return Color.Lerp(Color.White, Color.Transparent, completionRatio);
        }

        private float StarryTrailWidthFunction(float completionRatio)
        {
            return MathHelper.SmoothStep(32, 0, completionRatio);
        }

        private void RenderStarryDashTrail(GraphicsDevice gDevice)
        {
            FixedRichLaserShader laserShader = ShaderContent.GetInstance<FixedRichLaserShader>();
            laserShader.LaserTexture = TrailRegistry.Beamlight;
            laserShader.InnerColor = Color.White;
            laserShader.OuterColor = Color.Lerp(Color.White, Color.SkyBlue, ExtraMath.Osc(0f, 1f, speed: 16));
            TrailDrawer.Draw(Main.spriteBatch, Projectile.oldPos, StarryTrailColorFunction, StarryTrailWidthFunction, laserShader, Projectile.Size * 0.5f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            SpritebatchDrawer drawer = SpritebatchDrawer.FromProjectile(Projectile);

            for(int i = 0; i < Projectile.oldPos.Length; i++)
            {
                drawer.worldPosition = Projectile.oldPos[i] + Projectile.Size * 0.5f;
                drawer.color = Color.Lerp(Color.White, Color.Black, (float)i / (float)Projectile.oldPos.Length) * 0.05f;
                drawer.color.A = 0;
                Main.spriteBatch.Draw(drawer);
            }
            drawer.color = Color.White * ExtraMath.Osc(0.75f, 1f, speed: 6);
            drawer.color.A = 0;

            drawer.worldPosition = Projectile.Center;
            Main.spriteBatch.Draw(drawer);

            drawer.rotation = MathHelper.Lerp(0, MathHelper.TwoPi * 1, EasingFunction.InOutSine(FlashTimer / 30f));
            drawer.scale = Vector2.Lerp(Vector2.Zero, Vector2.One * 2, EasingFunction.InOutSine(FlashTimer / 30f));
            drawer.color = Color.Lerp(Color.Black, Color.White, EasingFunction.InOutSine(FlashTimer / 30f)) * 0.7f;
            drawer.color.A = 0;
            Main.spriteBatch.Draw(drawer);

            Color targetColor = Projectile.hostile ? Color.Red : Color.Yellow;
            drawer.color = targetColor * ExtraMath.Osc(0.5f, 1f, speed: 12);
            drawer.color.A = 0;
            drawer.scale = Vector2.One;
            drawer.rotation = 0;
            drawer.VerticalFrame(1, 2);
            Main.spriteBatch.Draw(drawer);


            float alpha = MathHelper.Lerp(0f, 1f, FlashTimer / 30f);
            var ta = ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/RayLight4");
            SpritebatchDrawer lineDrawer = SpritebatchDrawer.FromTextureAsset(ta, Projectile.Center) ;
            lineDrawer.color = Color.Lerp(Color.Black, Color.White, alpha);
            lineDrawer.color.A = 0;
            lineDrawer.rotation = (_targetPosition - Projectile.Center).ToRotation();
            lineDrawer.LeftCenterOrigin();

            Vector2 scale = Vector2.Lerp(new Vector2(0f, 1f), new Vector2(2f, 1f), alpha);
            scale.Y = 0.5f;

            float xScale = (Vector2.Distance(Projectile.Center, _targetPosition))/ ta.Width() ;
            scale.X *= xScale;
            lineDrawer.scale = scale;
            Main.spriteBatch.Draw(lineDrawer);
            return false;
        }
        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
            PixelPrimitiveCircleFactory.CreateGenericBoom(Projectile.Center, Color.White, Color.Violet, 30, Main.rand.NextFloat(100, 200));
            FXUtil.GlowCircleBoom(Projectile.Center, Color.White, Color.DarkGray, Color.DarkViolet, duration: 45, baseSize: Main.rand.NextFloat(0.06f, 0.24f));
            for(float n = 0; n < 8; n++)
            {
                var dp = DustParticle.Spawn(Projectile.Center, Main.rand.NextVector2Circular(16, 16));
                dp.outerColor = Color.DarkGray;
                dp.dampening = 0.05f;
                dp.gravity = 0;
                dp.noTileCollide = true;
            }
            if (Main.netMode == NetmodeID.Server)
                return;


            for(float f = 0; f < 4; f++)
            {
                RoyalMagicRenderer royalMagicRenderer = ModContent.GetInstance<RoyalMagicRenderer>();
                Vector2 vel = Main.rand.NextVector2Circular(4, 4);
                royalMagicRenderer.SpawnParticle(Projectile.Center + Main.rand.NextVector2Circular(64, 64), vel, 90);

            }
        }

        public void DrawToRenderTargets()
        {
            if (Mode != 2)
                return;

            PixelationManager.QueuePrimitivesDrawAction(RenderStarryDashTrail);
           // throw new NotImplementedException();
        }
    }

    public class RoyalMagicDashTrail : ModProjectile,
        IDrawToRenderTarget
    {
        private ref float Timer => ref Projectile.ai[0];
        private NPC Parent => Main.npc[(int)Projectile.ai[1]];
        private ref float ShouldDie => ref Projectile.ai[2];
        public override string Texture => TextureRegistry.EmptyTexture;
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            ProjectileID.Sets.TrailCacheLength[Type] = 32;
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 60;
            Projectile.ignoreWater = true;
        }
        public override void AI()
        {
            base.AI();
            Timer++;
            ProjectileID.Sets.TrailingMode[Type] = 2;
            float movement = Vector2.Distance(Parent.position, Parent.oldPosition);
            if(movement > 64)
            {
                ShouldDie = 1;
            }
            if (ShouldDie >= 1)
                return;

            Vector2 vel = (Parent.Center - Projectile.Center);
            Projectile.velocity = vel;
        }

        private Color StarryTrailColorFunction(float completionRatio)
        {

            return Color.Lerp(Color.White, Color.Transparent, completionRatio) * 
                MathHelper.Lerp(0f, 1f, EasingFunction.Clamp((float)Projectile.timeLeft / 30f));
        }

        private float StarryTrailWidthFunction(float completionRatio)
        {
            return MathHelper.SmoothStep(96, 0, completionRatio);
        }

        private void RenderStarryDashTrail(GraphicsDevice gDevice)
        {
            FixedRichLaserShader laserShader = ShaderContent.GetInstance<FixedRichLaserShader>();
            laserShader.LaserTexture = TrailRegistry.Beamlight;
            laserShader.InnerColor = Color.White;
            laserShader.OuterColor = Color.Lerp(Color.White, Color.SkyBlue, ExtraMath.Osc(0f, 1f, speed: 16));
            TrailDrawer.Draw(Main.spriteBatch, Projectile.oldPos, StarryTrailColorFunction, StarryTrailWidthFunction, laserShader, Projectile.Size * 0.5f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            return false;
            //return base.PreDraw(ref lightColor);
        }

        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
        }

        public void DrawToRenderTargets()
        {
            // throw new NotImplementedException();
        //    PixelationManager.QueuePrimitivesDrawAction(RenderStarryDashTrail);
        }
    }

    public class FenixDomainShader : CrystalShader<FenixDomainShader>
    {
        public float Time
        {
            set
            {
                Effect.Parameters["time"].SetValue(value);
            }
        }
        public Texture2D GradientMap
        {
            set
            {
                Main.graphics.GraphicsDevice.Textures[1] = value;
                Main.graphics.GraphicsDevice.SamplerStates[1] = SamplerState.LinearClamp;
            }
        }

        public Color[] Gradient
        {
            set
            {
                Vector3[] colors = new Vector3[value.Length];
                for(int i = 0; i < colors.Length; i++)
                {
                    colors[i] = value[i].ToVector3();
                }
                Effect.Parameters["gradient"].SetValue(colors);
            }
        }
    }
    [Autoload(Side = ModSide.Client)]
    public class FenixDomain : ModSystem
    {

        public bool drawFenix;
        public override void OnModLoad()
        {
            On_Main.DrawNPCs += DrawBlack;
        }
        public override void Unload()
        {
            base.Unload();
        }

        public override void OnModUnload()
        {
            base.OnModUnload();
            On_Main.DrawNPCs -= DrawBlack;
        }

        private void DrawBlack(On_Main.orig_DrawNPCs orig, Main self, bool behindTiles)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            if (drawFenix)
            {
                GraphicsDevice graphicsDevice = Main.graphics.GraphicsDevice;
                graphicsDevice.Clear(Color.Transparent);
                FenixDomainShader fenixDomainShader = ShaderContent.GetInstance<FenixDomainShader>();
                fenixDomainShader.GradientMap = TextureRegistry.CloudNoise3.Value;
                fenixDomainShader.Time = Main.GlobalTimeWrappedHourly;
                spriteBatch.Restart(effect: fenixDomainShader.Effect);

                Rectangle targetRect = new Rectangle(0, 0, Main.screenWidth, Main.screenHeight);
                spriteBatch.Draw(TextureAssets.BlackTile.Value, targetRect, Color.White);

                spriteBatch.RestartDefaults();
  
            
                DomainExpansionManager singularityFallSystem = ModContent.GetInstance<DomainExpansionManager>();
                if (singularityFallSystem.hoveringPlatform)
                {
                    Texture2D bloomLine = ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/BloomLine").Value;
                    Vector2 drawOrigin = new Vector2(bloomLine.Size().X / 2, 0);
                    float rotation = MathHelper.PiOver2;
                    Color drawColor = Color.White;
                    drawColor.A = 0;
                    drawColor *= 0.5f;
                    drawColor *= ExtraMath.Osc(0.5f, 1f);
                    Vector2 drawPosition = new Vector2(Main.LocalPlayer.Center.X, singularityFallSystem.hoverPlatformY);
                    drawPosition -= Main.screenPosition;
                    drawPosition.Y += 48;
                    Vector2 drawScale = new Vector2(1, 2);
                    spriteBatch.Draw(bloomLine, drawPosition, null, drawColor, rotation, drawOrigin, drawScale, SpriteEffects.None, 0);
                    spriteBatch.Draw(bloomLine, drawPosition, null, drawColor, -rotation, drawOrigin, drawScale, SpriteEffects.None, 0);
                }

          
                drawFenix = false;
            }

            orig(self, behindTiles);
        }
    }
    public partial class RoyalFox : ScarletBoss,
        IDrawToRenderTarget
    {
        private Vector2 _teleportPosition;
        private Vector2 _startDashPoint;
        private Vector2 _dashLineVelocity;
        private float _dashTrailAlpha;
        private bool _renderDashTrail;
        private bool _renderMotionBlur;
        private float _invisibleAlpha;
        private bool _goInvisible;
        private bool _dontRender;

        private float _direction;
        private Outliner _outliner;
        private bool _contactDamage;
        private RoyalFoxRig _rigBackingField;
        private RoyalFoxRig Rig
        {
            get
            {
                _rigBackingField ??= CreateRig();
                return _rigBackingField;
            }
        }

        private ref float Timer => ref NPC.ai[0];
        private enum AIState
        {
            Spawn,
            Despawn,
            Idle,

            Zoom_SparkleStarRain,
            Zoom_DashDance,
            Zoom_CometStarDash,
            Zoom_BigFatLaser,

            Precision_OutOfBreathTransition,
            Precision_SwordSlashChase,
            Precision_SpinningCharge,
            Precision_CometTeleportShots
        }

        private AIState State
        {
            get => (AIState)NPC.ai[1];
            set => NPC.ai[1] = (float)value;
        }

        private ref float AttackCycle => ref NPC.ai[2];
        private ref float AttackCounter => ref NPC.ai[3];

        private float _miniAttackCount;

        private Chain _chain;
        private Chain Chain
        {
            get
            {
                if (_chain == null)
                {

                    _chain = new Chain(NPC.Center, 4, 128);
                }
                return _chain;
            }
        }
        private Chain[] _chains;
        private Chain[] Chains
        {
            get
            {
                if (_chains == null)
                {
                    _chains = new Chain[3];
                    for(int i = 0; i < 3; i++)
                    {

                        _chains[i] = new Chain(NPC.Center, 4, 128);
                    }
                }
                return _chains;
            }
        }


        //Dash Dance Attack
        private int DashDanceDamage => 80;
        private float NumDashDanceLines => 7;
        private int NumDashDanceBursts => 3;
        private float DashDanceTime => 15;
        private float DelayBetweenDashDanceBursts => 25;
        public Texture2D GetSubTexture(string fileName)
        {
            string path = Texture + $"_{fileName}";
            return ModContent.Request<Texture2D>(path, AssetRequestMode.ImmediateLoad).Value;
        }

        public RoyalFoxRig CreateRig()
        {
            Texture2D[] backLegTextures = new Texture2D[3];
            backLegTextures[0] = GetSubTexture("BackThigh");
            backLegTextures[1] = GetSubTexture("BackLeg");
            backLegTextures[2] = GetSubTexture("Foot");

            Texture2D[] frontLegTextures = new Texture2D[3];
            frontLegTextures[0] = GetSubTexture("FrontThigh");
            frontLegTextures[1] = GetSubTexture("FrontLeg");
            frontLegTextures[2] = GetSubTexture("Foot");

            Texture2D head = GetSubTexture("Head");

            Texture2D[] bodyTextures = new Texture2D[4];
            bodyTextures[0] = GetSubTexture("Body3");
            bodyTextures[1] = GetSubTexture("Body2");
            bodyTextures[2] = GetSubTexture("Body1");
            bodyTextures[3] = GetSubTexture("Neck");

            var rig = new RoyalFoxRig(backLegTextures, frontLegTextures, bodyTextures, head);
            return rig;
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            return base.CanHitPlayer(target, ref cooldownSlot) && _contactDamage;
        }

        public override BossLevel GetBossLevel()
        {
            return BossLevel.Superboss;
        }
        private void EnablePlatformArena()
        {
            DomainExpansionManager fallSystem = ModContent.GetInstance<DomainExpansionManager>();
            fallSystem.noWings = true;
            fallSystem.inSpace = true;
            fallSystem.hoveringPlatform = true;
            fallSystem.hoverPlatformY = 16000;
            fallSystem.noProjTileCollide = true;
            if (Main.netMode == NetmodeID.Server)
                return;
            ModContent.GetInstance<FenixDomain>().drawFenix = true;
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            base.SendExtraAI(writer);
            writer.WriteVector2(_startDashPoint);
            writer.WriteVector2(_dashLineVelocity);
            writer.WriteVector2(_teleportPosition);
            writer.Write(_miniAttackCount);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            base.ReceiveExtraAI(reader);
            _startDashPoint = reader.ReadVector2();
            _dashLineVelocity = reader.ReadVector2();
            _teleportPosition = reader.ReadVector2();
            _miniAttackCount = reader.ReadSingle();
        }
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Main.npcFrameCount[NPC.type] = 1;
            NPCID.Sets.TrailCacheLength[NPC.type] = 32;
            NPCID.Sets.TrailingMode[Type] = 3;
            NPCID.Sets.MPAllowedEnemies[NPC.type] = true;
            NPCID.Sets.BossBestiaryPriority.Add(Type);
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            NPC.width = 90;
            NPC.height = 90;
            NPC.damage = 100;
            NPC.defense = 20;
            NPC.lifeMax = 24000;
            NPC.scale = 1f;
            NPC.aiStyle = -1;

            NPC.value = Item.buyPrice(gold: 5);
            NPC.knockBackResist = 0f;
            NPC.boss = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.npcSlots = 30f;

            Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/AlcaricFox");
            NPC.HitSound = new SoundStyle("Stellamod/Assets/Sounds/Gintze_Hit") with { PitchVariance = 0.1f, Pitch = -0.5f, Volume = 0.2f };
            NPC.DeathSound = new SoundStyle("Stellamod/Assets/Sounds/Gintze_Death") with { PitchVariance = 0.1f, Pitch = -0.5f, Volume = 0.2f };
        }


        private void Teleport(Vector2 position)
        {
            if (!MultiplayerHelper.IsHost)
                return;
            _teleportPosition = position;
            NPC.netUpdate = true;
        }

        public override void AI()
        {
            base.AI();
            _contactDamage = false;
            _renderMotionBlur = false;
            _renderDashTrail = false;
            _goInvisible = false;
            _dontRender = false;
            _outliner.SetDefaults();
            EnablePlatformArena();
            switch (State)
            {
                default:
                case AIState.Idle:
                    AI_Idle();
                    break;
                case AIState.Zoom_DashDance:
                    AI_ZoomDashDance();
                    break;
            }

            if(_teleportPosition != Vector2.Zero)
            {
                for(int i = 0; i < Chain.points.Length; i++)
                {
                    Chain.points[i] = NPC.Center;
                }
                NPC.Center = _teleportPosition;
                _teleportPosition = Vector2.Zero;
        
            }

            float targetInvisibleAlpha = _goInvisible ? 0f : 1f;
            _invisibleAlpha = MathHelper.Lerp(_invisibleAlpha, targetInvisibleAlpha, 0.1f);

            float targetDashTrailAlpha = _renderDashTrail ? 1f : 0f;
            _dashTrailAlpha = MathHelper.Lerp(_dashTrailAlpha, targetDashTrailAlpha, 0.1f);
            _outliner.Update();
           // AI_DebugRig();
            UpdateRig();
            SimulateHair();
        }

        private void SwitchState(AIState state)
        {
            _miniAttackCount = 0;
            Timer = 0;
            AttackCycle = 0;
            AttackCounter = 0;
            State = state;
            NPC.netUpdate = true;
            Main.NewText(state);
        }

        private void DebugTeleportLeftOfPlayer()
        {

            Vector2 pos = (MyTarget.Center + new Vector2(-512, 0));
            NPC.velocity = Vector2.Zero;
            NPC.Center = pos;
        }

        private void AI_Idle()
        {
            Timer++;
            if(Timer == 1)
            {
                NPC.TargetClosest();
            }
            DebugTeleportLeftOfPlayer();
            NPC.velocity *= 0.8f;
            if(Timer >= 100)
            {
                ChooseAttack();
            }
            AnimateStanding();
        }
        private void ChooseAttack()
        {
            if (MultiplayerHelper.IsHost)
            {
                SwitchState(AIState.Zoom_DashDance);
            }
        }

        private float Zoom_Prepare_Time => 80;

        private void PoofParticles()
        {
            if (Main.netMode == NetmodeID.Server)
                return;
            RoyalMagicRenderer royalMagicRenderer = ModContent.GetInstance<RoyalMagicRenderer>();


            for(float f = 0; f < 13; f++)
            {

                Vector2 vel = -Vector2.UnitY * Main.rand.NextFloat(3f, 7f);
                royalMagicRenderer.SpawnParticle(NPC.Center + Main.rand.NextVector2Circular(64, 128), vel, 180);

                if (Main.rand.NextBool(2))
                {
                    var sp = RoyalMagicStarParticle.Spawn(NPC.Center + Main.rand.NextVector2Circular(64, 64), -vel, Scale: Main.rand.NextFloat(0.25f, 0.6f));
                    sp.color = Color.Lerp(new Color(117, 100, 210), Color.White, Main.rand.NextFloat(0f, 1f));
                }
                if (Main.rand.NextBool(2))
                {
                    var sp = RoyalMagicSwordParticle.Spawn(NPC.Center + Main.rand.NextVector2Circular(64, 64), vel, Scale: Main.rand.NextFloat(0.25f, 0.6f));
                    sp.color = Color.Lerp(new Color(117, 100, 210), Color.White, Main.rand.NextFloat(0f, 1f));
                    sp.behindLayer = Main.rand.NextBool(2);
                }
                if (Main.rand.NextBool(2))
                {
                    var sp = FaintSmokeParticle.SpawnInAlphaLayer(NPC.Center + Main.rand.NextVector2Circular(64, 64), -vel, Scale: Main.rand.NextFloat(0.25f, 0.6f));
                    sp.Scale *= 0.5f;
                    sp.color = Color.Lerp(Color.Black, Color.White, Main.rand.NextFloat(0f, 0.33f));
                    sp.behindLayer = true;
                }
            }
        }
        private void WalkParticles()
        {
            if (Main.netMode == NetmodeID.Server)
                return;
            RoyalMagicRenderer royalMagicRenderer = ModContent.GetInstance<RoyalMagicRenderer>();


            if (Main.rand.NextBool(4))
            {
                var d = Dust.NewDustPerfect(NPC.Center + Main.rand.NextVector2Circular(16, 16), DustID.GemDiamond, Scale: 1f);
                d.noGravity = true;
            }

            if (!Main.rand.NextBool(2))
                return;


            Vector2 vel = -NPC.velocity.SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(3f, 7f);
            royalMagicRenderer.SpawnParticle(NPC.Center + Main.rand.NextVector2Circular(64, 64), vel, 180);

            vel = -NPC.velocity.SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(3f, 7f);
            royalMagicRenderer.SpawnParticle(NPC.Center + Main.rand.NextVector2Circular(64, 64), vel, 180);

            if (!Main.rand.NextBool(4))
                return;

            if (Main.rand.NextBool(2))
            {
                var sp = RoyalMagicStarParticle.Spawn(NPC.Center + Main.rand.NextVector2Circular(64, 64), vel, Scale: Main.rand.NextFloat(0.25f, 0.6f));
                sp.color = Color.Lerp(new Color(117, 100, 210), Color.White, Main.rand.NextFloat(0f, 1f));
            }
            if (Main.rand.NextBool(2))
            {
                var sp = RoyalMagicSwordParticle.Spawn(NPC.Center + Main.rand.NextVector2Circular(64, 64), -vel, Scale: Main.rand.NextFloat(0.25f, 0.6f));
                sp.color = Color.Lerp(new Color(117, 100, 210), Color.White, Main.rand.NextFloat(0f, 1f));
                sp.behindLayer = Main.rand.NextBool(2);
            }
            if (Main.rand.NextBool(2))
            {
                var sp = FaintSmokeParticle.SpawnInAlphaLayer(NPC.Center + Main.rand.NextVector2Circular(64, 64), -vel, Scale: Main.rand.NextFloat(0.25f, 0.6f));
                sp.Scale *= 0.5f;
                sp.color = Color.Lerp(Color.Black, Color.White, Main.rand.NextFloat(0f, 0.33f));
                sp.behindLayer = true;
            }
        }
        private void WalkParticles2()
        {
            if (Main.netMode == NetmodeID.Server)
                return;
            RoyalMagicRenderer royalMagicRenderer = ModContent.GetInstance<RoyalMagicRenderer>();


            if (Main.rand.NextBool(4))
            {
                var d = Dust.NewDustPerfect(NPC.Center + Main.rand.NextVector2Circular(16, 16), DustID.GemDiamond, Scale: 1f);
                d.noGravity = true;
            }

            if (!Main.rand.NextBool(2))
                return;


            if (!Main.rand.NextBool(4))
                return;

            Vector2 vel = -NPC.velocity.SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(2f, 5f);
            if (Main.rand.NextBool(2))
            {
                var sp = RoyalMagicStarParticle.Spawn(NPC.Center + Main.rand.NextVector2Circular(64, 64), vel, Scale: Main.rand.NextFloat(0.25f, 0.6f));
                sp.color = Color.Lerp(new Color(117, 100, 210), Color.White, Main.rand.NextFloat(0f, 1f));
            }
            if (Main.rand.NextBool(2))
            {
                var sp = RoyalMagicSwordParticle.Spawn(NPC.Center + Main.rand.NextVector2Circular(64, 64), -vel, Scale: Main.rand.NextFloat(0.25f, 0.6f));
                sp.color = Color.Lerp(new Color(117, 100, 210), Color.White, Main.rand.NextFloat(0f, 1f));
                sp.behindLayer = Main.rand.NextBool(2);
            }
            if (Main.rand.NextBool(2))
            {
                var sp = FaintSmokeParticle.SpawnInAlphaLayer(NPC.Center + Main.rand.NextVector2Circular(64, 64), -vel, Scale: Main.rand.NextFloat(0.25f, 0.6f));
                sp.Scale *= 0.5f;
                sp.color = Color.Lerp(Color.Black, Color.White, Main.rand.NextFloat(0f, 0.33f));
                sp.behindLayer = true;
            }
        }
        private void AnimateStanding()
        {
            float start = MathHelper.ToRadians(-2);
            float end = MathHelper.ToRadians(2);

            float runningSpeed = 4;
            float frontFrontLeg = ExtraMath.Osc(0f, 1f, speed: runningSpeed);
            //     float easeing = EasingFunction.InOutSine(legPair1);
            Rig.frontFrontLeg[0].eulerAngles.Z = MathHelper.Lerp(start, end, frontFrontLeg);
            Rig.frontFrontLeg[1].eulerAngles.Z = MathHelper.Lerp(start * 2, end * 2, frontFrontLeg);

            float frontBackLeg = ExtraMath.Osc(0f, 1f, speed: runningSpeed, offset: 1);
            Rig.frontBehindLeg[0].eulerAngles.Z = MathHelper.Lerp(start, end, frontBackLeg);
            Rig.frontBehindLeg[1].eulerAngles.Z = MathHelper.Lerp(start * 2, end * 2, frontBackLeg);


            //Back Legs
            float backFrontLeg = ExtraMath.Osc(0f, 1f, speed: runningSpeed, offset: 3);
            Rig.backFrontLeg[0].eulerAngles.Z = MathHelper.Lerp(start, end, backFrontLeg);
            Rig.backFrontLeg[1].eulerAngles.Z = MathHelper.Lerp(start * 2, end * 2, backFrontLeg);


            float backBackLeg = ExtraMath.Osc(0f, 1f, speed: runningSpeed, offset: 3 + 1);
            Rig.backBehindLeg[0].eulerAngles.Z = MathHelper.Lerp(start, end, backBackLeg);
            Rig.backBehindLeg[1].eulerAngles.Z = MathHelper.Lerp(start * 2, end * 2, backBackLeg);


            float headRotOffset = MathHelper.Lerp(start, end, ExtraMath.Osc(0f, 1f, speed: runningSpeed));
            Rig.bodyParts[3].eulerAngles.Z = MathHelper.ToRadians(19) + headRotOffset;
        }
        private void AnimateRunning()
        {
            float start = MathHelper.ToRadians(-25);
            float end = MathHelper.ToRadians(25);

            float runningSpeed = 9;
            float frontFrontLeg = ExtraMath.Osc(0f, 1f, speed: runningSpeed);
       //     float easeing = EasingFunction.InOutSine(legPair1);
            Rig.frontFrontLeg[0].eulerAngles.Z = MathHelper.Lerp(start, end, frontFrontLeg);
            Rig.frontFrontLeg[1].eulerAngles.Z = MathHelper.Lerp(start * 2, end * 2, frontFrontLeg);

            float frontBackLeg = ExtraMath.Osc(0f, 1f, speed: runningSpeed, offset: 1);
            Rig.frontBehindLeg[0].eulerAngles.Z = MathHelper.Lerp(start, end, frontBackLeg);
            Rig.frontBehindLeg[1].eulerAngles.Z = MathHelper.Lerp(start * 2, end * 2, frontBackLeg);


            //Back Legs
            float backFrontLeg = ExtraMath.Osc(0f, 1f, speed: runningSpeed, offset: 3);
            Rig.backFrontLeg[0].eulerAngles.Z = MathHelper.Lerp(start, end, backFrontLeg);
            Rig.backFrontLeg[1].eulerAngles.Z = MathHelper.Lerp(start * 2, end * 2, backFrontLeg);


            float backBackLeg = ExtraMath.Osc(0f, 1f, speed: runningSpeed, offset: 3 + 1);
            Rig.backBehindLeg[0].eulerAngles.Z = MathHelper.Lerp(start, end, backBackLeg);
            Rig.backBehindLeg[1].eulerAngles.Z = MathHelper.Lerp(start * 2, end * 2, backBackLeg);


            Rig.bodyParts[3].eulerAngles.Z = MathHelper.ToRadians(15);
            //   Rig.frontFrontLeg[2].eulerAngles.Z = MathHelper.Lerp(start * 2, end * 2, legPair1);
        }

        #region Zoom Mode 


        private void CreateDashLines()
        {
            if (MultiplayerHelper.IsHost)
            {
                for (int i = 0; i < NumDashDanceLines; i++)
                {
                    Vector2 posToPutLine = Vector2.Zero;
                    posToPutLine.X = MathHelper.Lerp(-350, 350, (float)i / NumDashDanceLines);
                    posToPutLine.Y = Main.rand.NextFloat(-300, 300);
                    posToPutLine += MyTarget.Center;

                    Vector2 velocity = (posToPutLine - MyTarget.Center).RotatedByRandom(MathHelper.ToDegrees(45)).SafeNormalize(Vector2.Zero);
                    if (i == 0)
                    {
                        velocity = (posToPutLine - MyTarget.Center).SafeNormalize(Vector2.Zero);
                    }
                    Projectile.NewProjectile(SourceFromThis, posToPutLine, velocity, ModContent.ProjectileType<DashLine>(), DashDanceDamage, 1,
                        Main.myPlayer, ai0: i * -2);
                }


            }

        }

        private void CommandStars()
        {
            foreach(var proj in Main.ActiveProjectiles)
            {
                if (proj.type != ModContent.ProjectileType<RoyalMagicMiniStar>())
                    continue;
                proj.ai[1] = 1;
            }
        }
        private void AI_ZoomDashDance()
        {
            (Vector2, Vector2) NextDashLine()
            {

                (Vector2 position, Vector2 velocity) dashLine = new(Vector2.Zero, Vector2.Zero);
                foreach(var proj in Main.ActiveProjectiles)
                {
                    if (proj.type != ModContent.ProjectileType<DashLine>())
                        continue;
                    if (proj.ai[1] > 0)
                        continue;
                    proj.ai[1] = 1;
                    dashLine.position = proj.Center;
                    dashLine.velocity = proj.velocity;
                    break;
                }
                return dashLine;
            }

            bool HasNextDashLine()
            {
                foreach (var proj in Main.ActiveProjectiles)
                {
                    if (proj.type != ModContent.ProjectileType<DashLine>())
                        continue;
                    if (proj.ai[1] > 0)
                        continue;
                    return true;
                }
                return false;
            }
            //Fenix flies up and does the like cogwork dancers thing where a bunch of lines appear and she dashes through them really fast, this is a two shot btw
            //For this attack, we'll create a new blurring shader for the motion blur
            //And also have cool effects for the trailing
            
            //PART ONE:
            //Let's break it down
            //First, fenix, with a bit of anticipation, slowly flies up and teleports/fades out, cool starry/smoke visuals on this
            //For the starry/smoke part, we'll create a new smoke effect
           
            //PART TWO:
            //We generate a bunch of positions that Fenix will dash through, this can just be a projectile, they fade in around her target
            //She then dashes through each of the lines one by one with a really fast and cool blurring shader

            //PART THREE
            //She probably does the attack 3 times before ending the attack and going back to her cycle
            //The first dash has quite a bit of anticipation btw.
            Timer++;
            switch (AttackCycle)
            {
                case 0:
                    if(Timer == 1)
                    {
                        NPC.TargetClosest();
                        _direction = FacingDirectionToTarget;
                    }
                    {
                        //I want fenix to move forward and then go up while rotating it'll look so cool, then she poofs
                        float progress = Timer / Zoom_Prepare_Time;
                        NPC.velocity.X = MathHelper.Lerp(_direction * 2, _direction * 8, EasingFunction.QuadraticBump(Timer / Zoom_Prepare_Time));

                        float yVelcoity = MathHelper.Lerp(0f, -14, EasingFunction.InOutExpo(progress));
                        NPC.velocity.Y = yVelcoity;
                        Rig.rootSegment.eulerAngles.W = MathHelper.Lerp(0f, MathHelper.ToRadians(-90), EasingFunction.InOutSine(progress));
                        Rig.rootSegment.eulerAngles.X = MathHelper.Lerp(0f, MathHelper.ToRadians(90 + 360), EasingFunction.InOutSine(progress));

                    }


                    _outliner.warning = true;
                    _renderDashTrail = true;
                    AnimateRunning();
                    WalkParticles();
                    if(Timer == Zoom_Prepare_Time - 5)
                    {
                        CreateDashLines();
                    }
                    if (Timer >= Zoom_Prepare_Time)
                    {
                        PoofParticles();
                        Timer = 0;
                        AttackCycle++;
                    }
                    break;

                    //how htis is gonna work is dash line sare gonna appear
                   //and as logn as a dash line projectile exists she'll dash through them all

                case 1:
                    if(Timer == 1)
                    {
                        NPC.TargetClosest();
          
                    }
                    _goInvisible = true;
                    if(Timer >= DelayBetweenDashDanceBursts)
                    {
                        Timer = 0;
                        AttackCycle++;
                    }

                    NPC.velocity *= 0.98f;
                    break;
                case 2:

                    if(Timer == 1)
                    {
                   
                      //  PoofParticles();
                        (Vector2 position, Vector2 velocity) = NextDashLine();
                        if(position != default(Vector2))
                        {
                      
                            _dashLineVelocity = velocity;
                            position -= velocity * 384;
                            _startDashPoint = position;
                            Teleport(position);
                            NPC.netUpdate = true;
                        }
                    }
                    if(Timer == 3)
                    {
                    
                        if (MultiplayerHelper.IsHost)
                        {
                            Projectile.NewProjectile(SourceFromThis, NPC.Center, Vector2.Zero, ModContent.ProjectileType<RoyalMagicDashTrail>(), 0, 1, Main.myPlayer, ai1: NPC.whoAmI);
                        }
                    }

                    if(Timer < 3)
                    {
                        _goInvisible = true;
                    }

                    if(Timer == 3)
                    {
                        FXUtil.CreateRipple(_startDashPoint);
                        FXUtil.ShakeCamera(_startDashPoint, 1024, 2);
                        ShakeScreenPosition.Shake = 2;

                        for(int i = 1; i < 5; i++)
                        {
                            var tp = ThrustParticle.Spawn(_startDashPoint, _dashLineVelocity * 14 * i, Scale: 2);
                            tp.bloomColor = Color.White;
                        }
                        for (int i = 1; i < 16; i++)
                        {
                            var tp = DustParticle.Spawn(_startDashPoint + Main.rand.NextVector2Circular(40, 40), _dashLineVelocity * 14 * i);
                            tp.Scale *= 2;
                            tp.outerColor = Color.Violet;
                            tp.gravity = 0;
                            tp.dampening = 0.1f;
                            tp.noTileCollide = true;
                            
                        }
                    }

                    if(Timer < 3)
                    {
                        _dontRender = true;
                    }
                    if(Timer == 3)
                    {
                     
                    }
                    float ratio = Timer / DashDanceTime;
                    float easing = EasingFunction.InOutExpo(ratio);
                    if(_miniAttackCount > 0)
                    {
                        easing = 1f;
                    }

                    if(Timer % 10 == 0)
                    {
                        if (MultiplayerHelper.IsHost)
                        {
                            Projectile.NewProjectile(SourceFromThis, NPC.Center, _dashLineVelocity * 35, 
                                ModContent.ProjectileType<RoyalMagicMiniStar>(), DashDanceDamage, 1, Main.myPlayer);
                        }
                    }

                    if(Timer > 3)
                    {
                        Vector2 pointToMoveTo = Vector2.Lerp(_startDashPoint, _startDashPoint + _dashLineVelocity * 700, ratio);
                        Vector2 vel = pointToMoveTo - NPC.Center;
                        NPC.velocity = Vector2.Zero;
                        NPC.Center = pointToMoveTo;

                    }
                    var sp = RoyalMagicStarParticle.Spawn(NPC.Center + Main.rand.NextVector2Circular(64, 64), _dashLineVelocity, Scale: Main.rand.NextFloat(0.15f, 0.25f));
                    sp.color = Color.Lerp(new Color(117, 100, 210), Color.White, Main.rand.NextFloat(0f, 1f));

                    if (Timer % 4 == 0)
                    {
                        var donute = LegacyParticle.NewParticle<GlowDonutParticle>(NPC.Center, -_dashLineVelocity * 3);
                    }

                    Rig.rootSegment.eulerAngles.W = _dashLineVelocity.ToRotation();
                    Rig.rootSegment.eulerAngles.X = MathHelper.Lerp(0f, MathHelper.ToRadians(360), EasingFunction.InOutSine(_miniAttackCount / NumDashDanceLines));

                    _contactDamage = true;
                    _outliner.attacking = true;
                    _renderMotionBlur = true;
                    AnimateRunning();
                    WalkParticles2();

                    var fx = FXUtil.GlowStretch(NPC.Center + Main.rand.NextVector2Circular(32, 32), _dashLineVelocity);
                    fx.VectorScale *= 0.5f;
                    if (Timer >= DashDanceTime)
                    {
                        if (!HasNextDashLine())
                        {
                            AttackCycle++;
                        }
                        Timer = 0;
                     
                    }
                    break;
                case 3:
                    if(Timer == 1)
                    {
                        PixelPrimitiveCircleFactory.CreateGenericInBoom(MyTarget.Center, Color.Transparent, Color.White * 0.35f, 45, 512);
                        CommandStars();
                        if (_direction == 0)
                            _direction = 1;
                        else
                            _direction *= -1;
                 //       PoofParticles();
                        Teleport(MyTarget.Center);
                    }

                    bool hasAnotherAttack = false;

                    if(Timer < 10)
                    {
                        _goInvisible = true;
                    }
                  

                    {
                        //I want fenix to move forward and then go up while rotating it'll look so cool, then she poofs
                        float progress = Timer / Zoom_Prepare_Time;
                        NPC.velocity.X = MathHelper.Lerp(_direction * 25, _direction * 2, EasingFunction.OutExpo(Timer / Zoom_Prepare_Time));

                        float yVelcoity = MathHelper.Lerp(0f, -14, EasingFunction.InOutExpo(progress));
                        if (_direction == -1)
                            yVelcoity *= -1;
                        NPC.velocity.Y = yVelcoity;
                     //   Rig.rootSegment.eulerAngles.X = _direction == -1 ? MathHelper.ToRadians(-180) : 0;
                        Rig.rootSegment.eulerAngles.W = MathHelper.Lerp(0f, MathHelper.ToRadians(-90), EasingFunction.InOutSine(progress));
                        if (_direction == -1)
                            Rig.rootSegment.eulerAngles.W += MathHelper.ToRadians(-180);
                        Rig.rootSegment.eulerAngles.X = MathHelper.Lerp(0f, MathHelper.ToRadians(90 + 360 * _direction), EasingFunction.InOutSine(progress));

                    }
                    WalkParticles();

                    if(Timer == Zoom_Prepare_Time - 25 && (AttackCounter+1) < NumDashDanceBursts)
                    {
                        CreateDashLines();
                    }

                    if(AttackCounter + 1 < NumDashDanceBursts)
                    {
                        _outliner.warning = true;
                    }

                    AnimateRunning();

                    if (Timer >= Zoom_Prepare_Time)
                    {
                        AttackCounter++;
                        Timer = 0;
                        if(AttackCounter >= NumDashDanceBursts)
                        {
                            AttackCycle++;
                        }
                        else
                        {
                            AttackCycle -= 2;
                        }
               
                    }
                    break;
                case 4:
                    _goInvisible = true;
                    if(Timer >= DelayBetweenDashDanceBursts)
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                    break;
                default:
                    SwitchState(AIState.Idle);
                    break;
            }
        }
        #endregion


        private Color DashTrailColorFunction(float completionRatio)
        {
            return Color.Lerp(Color.Transparent, Color.White, EasingFunction.QuadraticBump(completionRatio)) * _dashTrailAlpha;
        }

        private float DashTrailWidthFunction(float completionRatio)
        {
            return MathHelper.SmoothStep(80, 80, completionRatio);
        }
        private void RenderPixelatedDashTrail(GraphicsDevice gDevice)
        {
            BasicLaserShader laserShader = BasicLaserShader.Instance;
            laserShader.LaserTexture = AssetManager.LaserTextures.SplittingTrail;
            laserShader.InnerColor = Color.White;
            laserShader.OuterColor = Color.Lerp(Color.White, Color.SkyBlue, ExtraMath.Osc(0f, 1f, speed: 16));
            TrailDrawer.Draw(Main.spriteBatch, NPC.oldPos, DashTrailColorFunction, DashTrailWidthFunction, laserShader, NPC.Size * 0.5f);
        }


        #region Hair
        private void SimulateHair()
        {
            Vector2 up = (Rig.rootSegment.eulerAngles.W-MathHelper.PiOver2).ToRotationVector2();
            for(int c = 0; c < Chains.Length; c++)
            {
                var chain = Chains[c];

                chain.points[0] = NPC.Center + ExtraMath.Osc(-32, 32, speed: 2, c) * up;
                chain.points[0].Y -= 4 + ExtraMath.Osc(0f, 16, speed: 2);
                chain.pinned[0] = true;

                for(int k = 0; k < chain.points.Length; k++)
                {
                 //   chain.points[k] += ExtraMath.Osc(0f, 16, speed: 2, offset: k + c * 4) * -NPC.velocity.SafeNormalize(Vector2.Zero);
                }

                for (int i = 0; i < 32; i++)
                {
                    chain.ResolveBackToRoot();
                }
            }

        }
        private float GetHairWidth(float ratio)
        {
            return MathHelper.SmoothStep(80, 0, ratio) * _invisibleAlpha * EasingFunction.QuadraticBump(ratio);
        }
        private Color GetHairColor(float ratio)
        {
            return Color.White * _invisibleAlpha * EasingFunction.OutExpo(ratio + 0.5f);
        }

        private void DrawHair(GraphicsDevice gDevice)
        {
            HairShader shader = ShaderContent.GetInstance<HairShader>();
            shader.LaserTexture = TrailRegistry.GlowTrailNoBlack;
            shader.Time = Main.GlobalTimeWrappedHourly * 0.2f;
            shader.WaveFrequency = 8;
            shader.XOffset = 12;
            for(int i = 0; i < Chains.Length; i++)
            {
                TrailDrawer.Draw(Main.spriteBatch, Chains[i].points, GetHairColor, GetHairWidth, shader);
            }
        }

        #endregion
        private void UpdateRig()
        {
            //Calling update twice sine it has to calculate the new x axis position
            //Yeah this is technically inefficient but it's too inexpensive to matter, quick and dirty solution :p
            Rig.rootSegment.worldPosition = NPC.Center;
            Rig.Update();
            Rig.Update();
        }

        public override void OnKill()
        {
            base.OnKill();
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (_dontRender)
                return false;
            if (_renderMotionBlur)
            {
                DashBlurShader dashBlurShader = ShaderContent.GetInstance<DashBlurShader>();
                spriteBatch.Restart(effect: dashBlurShader.Effect);
            }

            for(int i =0; i < NPC.oldPos.Length; i++)
            {
                Vector2 offset = ((NPC.oldPos[i] + NPC.Size * 0.5f) - NPC.Center);
                float alpha = MathHelper.Lerp(0.04f, 0f, (float)i / (float)NPC.oldPos.Length) * _invisibleAlpha;
                for (int j = 0; j < Rig.segmentsByZLayer.Length; j++)
                {
                    Rig.segmentsByZLayer[j].alpha = alpha;
                }
                Rig.Draw(spriteBatch, screenPos - offset, drawColor);
            }
            for(int i = 0; i < Rig.segmentsByZLayer.Length; i++)
            {
                Rig.segmentsByZLayer[i].alpha = _invisibleAlpha;
            }

            Rig.Draw(spriteBatch, screenPos, drawColor);

            if (_renderMotionBlur)
            {
                spriteBatch.RestartDefaults();
            }
            return false;
        }

        private void DrawOutlines(SpriteBatch sb)
        {
            Rig.Draw(sb, Main.screenPosition, _outliner.outlineColor);
        }

        public void DrawToRenderTargets()
        {
            OutlineRenderer.Queue(DrawOutlines);
            if (_dashTrailAlpha > 0)
            {
                PixelationManager.QueuePrimitivesDrawAction(RenderPixelatedDashTrail);
            }
            PixelationManager.QueuePrimitivesDrawAction(DrawHair);
        }
    }
}
