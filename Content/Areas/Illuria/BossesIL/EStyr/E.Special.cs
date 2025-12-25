using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Assets;
using Stellamod.Core;
using Stellamod.Core.BlackSystem;
using Stellamod.Core.Camera;
using Stellamod.Core.Shaders;
using Stellamod.Core.Utilities;
using Stellamod.Helpers;
using Stellamod.Trails;
using Stellamod.UI.Systems;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Illuria.BossesIL.EStyr
{
    public interface IDrawBlackRiverMask
    {
        void DrawRiverMask();
    }

    public class BlackSlash : ModProjectile,
        IDrawBlackStar
    {
        private Asset<Texture2D> _textureAsset;
        private Vector2 _drawScale = Vector2.One;
        private Vector2[] LinePos = new Vector2[2];
        public override void Load()
        {
            base.Load();
            _textureAsset = ModContent.Request<Texture2D>(Texture);
        }
        private ref float Timer => ref Projectile.ai[0];
        private bool IsThick => Projectile.ai[1] == 1;
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 30;
        }

        public override bool CanHitPlayer(Player target)
        {
            return base.CanHitPlayer(target) && Timer < 5;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float lineWidth = 12;
            if (IsThick)
                lineWidth *= 3;
            return ProjectileHelper.OldPosColliding(LinePos, projHitbox, targetHitbox, lineWidth);
        }

        public override void AI()
        {
            base.AI();
            Timer++;
            if (Timer == 1)
            {
                _drawScale.Y = 0;
                SoundStyle hurriSlash = AssetRegistry.Sounds.E.Hurrislash;
                hurriSlash.PitchVariance = 0.3f;
                SoundEngine.PlaySound(hurriSlash, Projectile.position);
            }


            float outScale = EasingFunction.InOutSine((float)Projectile.timeLeft / 30f);
            _drawScale.Y = MathHelper.Lerp(0f, 1f, outScale);

            LinePos[0] = Projectile.Center - Projectile.velocity.SafeNormalize(Vector2.Zero) * 500;
            LinePos[1] = Projectile.Center + Projectile.velocity.SafeNormalize(Vector2.Zero) * 500;

        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            base.OnHitPlayer(target, info);
            BlackStars.AddBuff(target, 50);
        }

        public void DrawBlackStar(SpriteBatch spriteBatch)
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            _textureAsset ??= ModContent.Request<Texture2D>(Texture);
            Vector2 drawCenter = Projectile.Center - Main.screenPosition;
            Vector2 drawOrigin = _textureAsset.Size() / 2f;
            Color drawColor = Color.White;
            Vector2 drawScale = _drawScale;
            drawScale.X *= 12;
            drawScale.Y *= 1.5f;
            if (IsThick)
                drawScale.Y *= 2.25f;

            //Create a cool flickering effect
            //   drawColor *= ExtraMath.Osc(0f, 1f, speed: 32);
            float alpha = MathHelper.Lerp(1f, 0f, Timer / 30f);
            drawColor *= alpha;
            spriteBatch.Draw(_textureAsset.Value, drawCenter, null, drawColor, Projectile.rotation, drawOrigin, drawScale, SpriteEffects.None, 0);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            BlackStarRenderer.QueueBlackStarDraw(this);
            return false;
        }
    }

    public class BlackSlashLine : ModProjectile
    {
        private Asset<Texture2D> _textureAsset;
        public override void Load()
        {
            base.Load();
            _textureAsset = ModContent.Request<Texture2D>(Texture);
        }

        private ref float Timer => ref Projectile.ai[0];
        private ref float ExtraLifetime => ref Projectile.ai[1];
        private ref float IsThick => ref Projectile.ai[2];
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 60;
        }


        public override bool ShouldUpdatePosition()
        {
            return false;
        }
        public override void AI()
        {
            base.AI();
            Timer++;
            if (ExtraLifetime > 0)
            {
                ExtraLifetime--;
                Projectile.timeLeft++;
                Timer--;
            }

            float easeInTime = 30f;
            float completionRatio = Timer / easeInTime;
            float ease = EasingFunction.OutCirc(completionRatio);


            float targetRotation = Projectile.velocity.ToRotation();
            float startRotation = targetRotation - MathHelper.ToRadians(5);
            float interpolatedRotation = Utils.AngleLerp(startRotation, targetRotation, ease);
            Projectile.rotation = interpolatedRotation;
        }

        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity,
                ModContent.ProjectileType<BlackSlash>(), Projectile.damage, Projectile.knockBack, Projectile.owner, ai1: IsThick);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            _textureAsset ??= ModContent.Request<Texture2D>(Texture);
            Vector2 drawCenter = Projectile.Center - Main.screenPosition;
            SpriteBatch spriteBatch = Main.spriteBatch;
            Vector2 drawOrigin = _textureAsset.Size() / 2f;
            Color drawColor = Color.White;
            Vector2 drawScale = Vector2.One;
            drawScale.X *= 3;

            //Create a cool flickering effect
            drawColor *= ExtraMath.Osc(0.25f, 0.5f, speed: 16);

            float inAlpha = EasingFunction.InOutSine(Timer / 10f);
            drawColor *= inAlpha;
            spriteBatch.Draw(_textureAsset.Value, drawCenter, null, drawColor, Projectile.rotation, drawOrigin, drawScale, SpriteEffects.None, 0);
            return false;
        }


    }
    public class BlackBox : ModProjectile,
        IDrawBlackRiverMask
    {
        private ref float Timer => ref Projectile.ai[0];
        private bool KillMe => Projectile.ai[1] == 1;
        private ref float DeathTimer => ref Projectile.ai[2];
        private float InScale;
        private float OutScale;
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = false;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            base.AI();
            if (!NPC.AnyNPCs(ModContent.NPCType<E>()))
                Projectile.active = false;
            float size = 384;
            float leftBound = Projectile.Center.X - size;
            float rightBound = Projectile.Center.X + size;
            float topBound = Projectile.Center.Y - size;
            float bottomBound = Projectile.Center.Y + size;
            foreach (var player in Main.ActivePlayers)
            {
                if (player.Left.X < leftBound)
                    player.Left = new Vector2(leftBound, player.Left.Y);
                if (player.Right.X > rightBound)
                    player.Right = new Vector2(rightBound, player.Right.Y);
                if (player.Top.Y < topBound)
                    player.Top = new Vector2(player.Top.X, topBound);
                if (player.Bottom.Y > bottomBound)
                    player.Bottom = new Vector2(player.Bottom.X, bottomBound);
            }

            Timer++;
            InScale = MathHelper.Lerp(0f, 1f, EasingFunction.OutCirc(Timer / 140f));
            if (KillMe)
            {
                DeathTimer++;
                float killTime = 30f;
                float killRatio = DeathTimer / killTime;
                OutScale = MathHelper.Lerp(1f, 0f, EasingFunction.InOutSine(killRatio));
                if (DeathTimer >= killTime)
                    Projectile.Kill();
            }
            else
            {
                OutScale = 1f;
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            BlackRiverRenderer.QueuePostDraw(this);
            return false;
        }

        public void DrawRiverMask()
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, CustomBlendStates.Multiply, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer, null);
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawOrigin = texture.Size() / 2f;
            Vector2 drawCenter = Projectile.Center - Main.screenPosition;
            Vector2 scale = Vector2.One * InScale * OutScale;
            float osc = ExtraMath.Osc(0.95f, 1f, 0.5f);
                scale *= osc;
            float rotation = Projectile.rotation;

            spriteBatch.Draw(texture, drawCenter, null, new Color(0, 0, 0, 0), rotation, drawOrigin, scale, SpriteEffects.None, 0);
            spriteBatch.End();
            spriteBatch.Begin();
        }

    }
    public class RiverWhip : ModProjectile,
        IDrawBlackRiverMask
    {
        private Vector2 _scale = Vector2.One;
        private ref float Timer => ref Projectile.ai[0];
        private ref float RandScale => ref Projectile.ai[1];
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Main.projFrames[Type] = 9;
            ProjectileID.Sets.DrawScreenCheckFluff[Type] = 1500;
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 1;
            Projectile.height = 1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 400;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
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
                SoundStyle riverSound = AssetRegistry.Sounds.E.DarkTentacleStab;
                riverSound.PitchVariance = 0.3f;
                SoundEngine.PlaySound(riverSound);
            }
            if (RandScale == 0 && this.OwnedByLocalClient())
            {
                RandScale = Main.rand.NextFloat(0.05f, 2);
                Projectile.netUpdate = true;
            }

            float tentacleTime = 30;
            float ratio = Timer / tentacleTime;
            _scale.X = MathHelper.Lerp(4f, 4.5f, EasingFunction.QuadraticBump(ratio));
            _scale.Y = MathHelper.SmoothStep(0f, 1f, ratio) * RandScale;

            float outScale = (float)Projectile.timeLeft / 30f;
            outScale = EasingFunction.InOutSine(outScale);
            _scale.Y *= outScale;

            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.frameCounter++;

            float frameSpeed = 5;
            if (Projectile.frameCounter >= frameSpeed)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;

                if (Projectile.frame >= Main.projFrames[Projectile.type])
                {
                    Projectile.frame = Main.projFrames[Projectile.type] - 1;
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            BlackRiverRenderer.QueueDraw(this);
            return false;
        }
        public void DrawRiverMask()
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            float xDiff = Projectile.Center.X - Main.Camera.Center.X;
            drawPosition.X += xDiff * RandScale * 0.4f;
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawOrigin = new Vector2(0, 25);
            float rotation = Projectile.rotation;
            Rectangle frame = Projectile.Frame();
            spriteBatch.Draw(texture, drawPosition, frame, Color.White, rotation, drawOrigin, _scale, SpriteEffects.None, 0);
        }
    }
    public class BlackRiver : ModProjectile,
        IDrawBlackRiverMask
    {
        private TexturedQuad _quadBackingField;
        private TexturedQuad TexturedQuad
        {
            get
            {
                if (_quadBackingField == null)
                    _quadBackingField = new TexturedQuad();
                return _quadBackingField;
            }
        }
        public override string Texture => TextureRegistry.EmptyTexture;
        private ref float Timer => ref Projectile.ai[0];
        private bool KillMe => Projectile.ai[1] == 1;
        private ref float DeathTimer => ref Projectile.ai[2];
        private float CompletionRatio;
        private float DeathRatio;
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            ProjectileID.Sets.DrawScreenCheckFluff[Type] = 1500;
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 1;
            Projectile.height = 1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 3500;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }

        public override void AI()
        {
            base.AI();
            if (!NPC.AnyNPCs(ModContent.NPCType<E>()))
                Projectile.ai[1] = 1;
            Timer++;
            float time = 400f;
            CompletionRatio = Timer / time;

            Projectile.Center = Main.Camera.Center;
            if (KillMe)
            {
                float deathTime = 360f;
                DeathTimer++;
                DeathRatio = DeathTimer / deathTime;
                if(DeathTimer >= deathTime)
                {
                    Projectile.Kill();
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            BlackRiverRenderer.QueueDraw(this);
            //  DrawRiverMask();
            return false;
        }

        public void DrawRiverMask()
        {
            Vector2 pos = Main.Camera.Center + new Vector2(Main.screenWidth / 2f, 0);
            BasicLaserAlphaShader shader = BasicLaserAlphaShader.Instance;
            shader.OuterColor = Color.White;
            shader.InnerColor = Color.Black;
            shader.LaserTexture = ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/RiverMask");
            shader.Time = Main.GlobalTimeWrappedHourly * -16;

            float width = Main.screenHeight * 3;
            Vector2 start = pos;
            start.Y += 1000;

            float ease = EasingFunction.InOutSine(CompletionRatio);
            start.Y -= MathHelper.Lerp(0, 2000, ease);
            start.Y += MathHelper.Lerp(0, 3000, DeathRatio);
            TexturedQuad.CalculateVertices(start, Projectile.velocity,
                Main.screenWidth, width);
            Main.spriteBatch.GraphicsDevice.BlendState = BlendState.AlphaBlend;
            TexturedQuad.DrawWithShader(shader);
        }
    }
    [Autoload(Side = ModSide.Client)]
    public class BlackRiverRenderer : ModSystem
    {
        private ManagedRenderTarget _riverRT;
        private ManagedRenderTarget _riverMaskRT;
        private ManagedRenderTarget _pixelRT;
        private Queue<IDrawBlackRiverMask> _draws;
        private Queue<IDrawBlackRiverMask> _postDraws;
        private Asset<Texture2D> _noiseTextureAsset;
        private Asset<Texture2D> _waterTextureAsset;

        public static bool renderBehindNPCs;
        public static bool invert;

        public override void OnModLoad()
        {
            base.OnModLoad();
            _draws = new Queue<IDrawBlackRiverMask>(100);
            _postDraws = new Queue<IDrawBlackRiverMask>(100);
            _riverRT = ManagedRenderTarget.New(GetScreenSize);
            _riverMaskRT = ManagedRenderTarget.New(GetScreenSize);
            _pixelRT = ManagedRenderTarget.New(GetScreenSize, 2);

            On_Main.CheckMonoliths += RenderRiverRT;
            On_Main.DrawPlayers_BehindNPCs += DrawRiverToScreenBehindNPCs;
            On_Main.DrawPlayers_AfterProjectiles += DrawRiverToScreen;
        }

        public override void Load()
        {
            base.Load();
            _waterTextureAsset = ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/Refraction");
            _noiseTextureAsset = ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/PerlinNoise");
        }

        public override void OnModUnload()
        {
            base.OnModUnload();
            On_Main.CheckMonoliths -= RenderRiverRT;
            On_Main.DrawPlayers_BehindNPCs -= DrawRiverToScreenBehindNPCs;
            On_Main.DrawPlayers_AfterProjectiles -= DrawRiverToScreen;
        }

        private void RenderRiverMaskRT()
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            spriteBatch.GraphicsDevice.SetRenderTarget(_riverMaskRT);
            spriteBatch.GraphicsDevice.Clear(Color.Transparent);
            if (invert)
                spriteBatch.GraphicsDevice.Clear(Color.White);
            if(_draws.Count > 0)
            {
                spriteBatch.Begin();
                while(_draws.Count > 0)
                {
                    IDrawBlackRiverMask mask = _draws.Dequeue();
                    mask.DrawRiverMask();
                }
                spriteBatch.End();
            }
            if (_postDraws.Count > 0)
            {
                spriteBatch.Begin();
                while (_postDraws.Count > 0)
                {
                    IDrawBlackRiverMask mask = _postDraws.Dequeue();
                    mask.DrawRiverMask();
                }
                spriteBatch.End();
            }

        }

        public static void QueueDraw(IDrawBlackRiverMask mask)
        {
            if (Main.netMode == NetmodeID.Server)
                return;
            BlackRiverRenderer renderer = ModContent.GetInstance<BlackRiverRenderer>();
            renderer._draws.Enqueue(mask);
        }
        public static void QueuePostDraw(IDrawBlackRiverMask mask)
        {
            if (Main.netMode == NetmodeID.Server)
                return;
            BlackRiverRenderer renderer = ModContent.GetInstance<BlackRiverRenderer>();
            renderer._postDraws.Enqueue(mask);
        }
        private void RenderRiverTextureRT()
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            spriteBatch.GraphicsDevice.SetRenderTarget(_riverRT);
            spriteBatch.GraphicsDevice.Clear(Color.Black);

            MixerShader mixerShader = MixerShader.Instance;
            Asset<Texture2D> mixTexture = _waterTextureAsset;
            Asset<Texture2D> noiseTexture = _noiseTextureAsset;
            mixerShader.MixTexture = mixTexture;
            mixerShader.NoiseTexture = noiseTexture;
            mixerShader.Time = Main.GlobalTimeWrappedHourly * 3;
            mixerShader.Strength = 2;
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer, mixerShader.Effect);
            spriteBatch.Draw(_riverRT, Vector2.Zero, Color.White * 0.6f);
            spriteBatch.End();
        }

        private void RenderToPixelRT()
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            MaskCombineShader maskCombineShader = MaskCombineShader.Instance;
            spriteBatch.GraphicsDevice.SetRenderTarget(_pixelRT);
            spriteBatch.GraphicsDevice.Clear(Color.Transparent);

            //Draw at half size to downscale
            maskCombineShader.MixTexture = _riverRT;

            SpriteWhiteShader whiteShader = SpriteWhiteShader.Instance;
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp,
                DepthStencilState.None, Main.Rasterizer, whiteShader.Effect);

            spriteBatch.Draw(_riverMaskRT, new Vector2(0, 2), null, Color.White, 0, Vector2.Zero, 0.5f, SpriteEffects.None, 0);
            spriteBatch.Draw(_riverMaskRT, new Vector2(-2, 0), null, Color.White, 0, Vector2.Zero, 0.5f, SpriteEffects.None, 0);
            spriteBatch.Draw(_riverMaskRT, new Vector2(2, 0), null, Color.White, 0, Vector2.Zero, 0.5f, SpriteEffects.None, 0);
            spriteBatch.Draw(_riverMaskRT, new Vector2(0, -2), null, Color.White, 0, Vector2.Zero, 0.5f, SpriteEffects.None, 0);
            spriteBatch.End();

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp,
                DepthStencilState.None, Main.Rasterizer, maskCombineShader.Effect);
            spriteBatch.Draw(_riverMaskRT, Vector2.Zero, null, Color.White, 0, Vector2.Zero, 0.5f, SpriteEffects.None, 0);
            spriteBatch.End();
        }

        private void RenderRiverRT(On_Main.orig_CheckMonoliths orig)
        {
            RenderRiverMaskRT();
            RenderRiverTextureRT();
            RenderToPixelRT();
            orig();
        }
        private void DrawRiverToScreenBehindNPCs(On_Main.orig_DrawPlayers_BehindNPCs orig, Main self)
        {
            if (renderBehindNPCs)
            {
                DrawRiverToScreen();
            }
            orig(self);
        }

        private void DrawRiverToScreen(On_Main.orig_DrawPlayers_AfterProjectiles orig, Main self)
        {
            orig(self);
            if (!renderBehindNPCs)
            {
                DrawRiverToScreen();
            }

        }

        private void DrawRiverToScreen()
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer, null);
            spriteBatch.Draw(_pixelRT, Vector2.Zero, null, Color.White, 0, Vector2.Zero, 2, SpriteEffects.None, 0);
            spriteBatch.End();

        }

        private Point GetScreenSize()
        {
            return new Point(Main.screenTarget.Width, Main.screenTarget.Height);
        }
    }
    public partial class E
    {
        /*
         * Turns his back to you and raises his hand up, and then stabs it with his sword,
         * a bunch of black star liquid comes flowing out and down to the ground and filling up the floor, 
         * and then tendrils pop out and pull you into it and the screen goes black-
         * When it comes back you’re in a small box and he appears to the side, 
         * and starts doing screen sweeping slashes that gradually keep getting faster and 
         * doing different patterns until it basically turns into one of those circling attacks,
         * as a tunnel forms around that you also have to go through, then he does one final slash and lets you free
         */

        private Vector2 _boxCenter;
        private int BlackSlashLineDamage => 50;
        private void AI_SpecialWarn()
        {
            //Alright, for this attack we're going to have to get a little crazyyyyyy
            //Lmao

            //First thing I want to happen is for him to move above you, and then like hovering around with afterimages coming out of him
            //Make the screen starts shaking a little bit
            Timer++;
            if (Timer == 1)
            {
                TargetVector = NPC.velocity;
                ShakeModSystem.Shake = 2;
            }

            float warnTime = 30f;
            float completionRatio = Timer / warnTime;
            float ease = EasingFunction.InOutSine(completionRatio);
            Vector2 positionToMoveTo = MyTarget.Center - new Vector2(0, 128);
            Vector2 targetVelocity = positionToMoveTo - NPC.Center;
            Vector2 easeVelocity = Vector2.Lerp(NPC.velocity, targetVelocity, ease);
            NPC.velocity = easeVelocity;
            NPC.direction = TargetDirection;


            _extraAfterImageAlpha = MathHelper.Lerp(0f, 0.7f, ease);
            Animator.PlayAnimation(Anim_BattleIdle);
            if (Timer >= warnTime)
            {
                SwitchState(AIState.Special_Warn2);
            }
        }

        private void AI_SpecialWarn2()
        {
            Timer++;
            if (Timer % 5 == 0)
            {
                CreateNewAfterImage();
            }

            float warnTime = 90f;
            Vector2 hoverVelocity = new Vector2();
            hoverVelocity.Y = MathF.Sin(Timer * 0.05f);
            NPC.velocity = Vector2.Lerp(NPC.velocity, hoverVelocity, 0.1f);


            //Wiggle wiggle wiggle lol
            Vector2 targetScale = Vector2.One;
            targetScale.X = MathF.Sin(Timer * 0.5f) * 0.5f + 0.5f;
            targetScale.Y = MathF.Cos(Timer * 0.5f) * 0.5f + 0.5f;
            _drawScale = targetScale;
            _extraAfterImageAlpha = 0.7f;

            Animator.PlayAnimation(Anim_Holding);
            if (Timer >= warnTime)
            {
                SwitchState(AIState.Special_HandStab);
            }
        }

        private void AI_SpecialHandStab()
        {
            Timer++;
            float stabTime = 120f;
            float completionRatio = Timer / stabTime;
            float ease = EasingFunction.InOutExpo7(completionRatio);
            Vector2 scale = Vector2.Lerp(Vector2.One * 1.2f, Vector2.One, ease);
            _drawScale = scale;
            NPC.velocity *= 0.9f;
            if (Timer >= stabTime)
            {
                SwitchState(AIState.Special_DripDrop);
            }
        }

        private void AI_SpecialDripDrop()
        {
            Timer++;
            if (Timer == 1)
            {
                TargetVector = NPC.velocity;
                if (MultiplayerHelper.IsHost)
                {
                    Projectile.NewProjectile(SourceFromThis, NPC.Center, -Vector2.UnitX,
                        ModContent.ProjectileType<BlackRiver>(), 1, 1, Main.myPlayer);
                }
                SoundStyle riverSound = AssetRegistry.Sounds.E.DescendingDark;
                SoundEngine.PlaySound(riverSound);
            }
            if (MultiplayerHelper.IsHost && Timer % 6 == 0)
            {
                Vector2 tentacleSpawnPoint = NPC.Center;
                tentacleSpawnPoint.X += Main.rand.NextFloat(-750f, 750f);
                tentacleSpawnPoint.Y += 777;

                Vector2 upVelocity = -Vector2.UnitY;
                upVelocity = upVelocity.RotatedByRandom(0.15f);
                Projectile.NewProjectile(SourceFromThis, tentacleSpawnPoint, upVelocity,
                    ModContent.ProjectileType<RiverWhip>(), 1, 1, Main.myPlayer);
            }

            if (Timer % 6 == 0)
            {
                Dust.NewDustPerfect(NPC.Center, DustID.GemDiamond);
            }
            float dripDropTime = 240;
            float completionRatio = Timer / dripDropTime;
            float ease = EasingFunction.InOutSine(completionRatio);
            Vector2 positionToMoveTo = NPCAIHelper.CalculatePositionToMoveTo(MyTarget.Center, NPC.Center, new Vector2(64, -64));
            Vector2 targetVelocity = positionToMoveTo - NPC.Center;
            NPC.velocity = Vector2.Lerp(TargetVector, targetVelocity, ease);
            ShakeModSystem.Shake = MathHelper.Lerp(0f, 8, completionRatio);
            if (Timer >= dripDropTime)
            {
                SwitchState(AIState.Special_FadeToBlack);
            }
        }

        private void SetRiverBoxParams()
        {
            //  BlackRiverRenderer.invert = _inRiver;
            BlackRiverRenderer.invert = false;
            BlackRiverRenderer.renderBehindNPCs = _inRiver;
        }


        private void AI_SpecialFadeToBlack()
        {
            Timer++;
            float fadeTime = 100;
            float completionRatio = Timer / fadeTime;

            //Fade the screen to black
            ShakeModSystem.Shake = 8;
            Vector2 boxPosition = GetBoxPosition();
            RetargetCameraModifier.ReTargetPosition = Vector2.Lerp(Main.LocalPlayer.Center, boxPosition, completionRatio);
            //  FullTint.SetColor(Color.Black, completionRatio);
            NPC.velocity *= 0.9f;
            if (Timer >= fadeTime)
            {
                SwitchState(AIState.Special_MakeBox);
            }
        }
        private Vector2 GetBoxPosition()
        {
            DomainExpansionManager fallSystem = ModContent.GetInstance<DomainExpansionManager>();
            Vector2 boxCenter = NPC.Center;
            boxCenter.Y = fallSystem.hoverPlatformY;
            boxCenter.Y -= 340;
            return boxCenter;
        }
        private void AI_SpecialMakeBox()
        {
            Timer++;
            if (Timer == 1)
            {
                if (MultiplayerHelper.IsHost)
                {
                    Vector2 boxCenter = GetBoxPosition();
                    _boxCenter = boxCenter;
                    Projectile.NewProjectile(SourceFromThis, boxCenter, Vector2.Zero,
                        ModContent.ProjectileType<BlackBox>(), 1, 1, Main.myPlayer);
                    NPC.netUpdate = true;
                }
            }

            Vector2 boxPosition = GetBoxPosition();
            RetargetCameraModifier.ReTargetPosition = boxPosition;

            NPC.velocity *= 0.9f;
            if (Timer >= 30)
            {
                SwitchState(AIState.Special_FadeOutFromBlack);
            }
        }
        private void AI_SpecialFadeOutFromBlack()
        {
            Timer++;
            if (Timer == 1)
            {

            }

            _inRiver = true;
            float fadeTime = 100;
            float completionRatio = Timer / fadeTime;
            float alpha = MathHelper.Lerp(1f, 0f, completionRatio);
            //   FullTint.SetColor(Color.Black, alpha);
            if (Timer >= fadeTime)
            {
                SwitchState(AIState.Special_SlashQuickStart);
            }
        }

        private void AI_SpecialSlashQuickStart()
        {
            Timer++;
            if (Timer == 1)
            {
                NPC.TargetClosest();
                TargetVector = NPC.velocity;
            }
            _inRiver = true;
            _extraAfterImageAlpha = 0.7f;
            float lerp = _attackNumber / 10f;
            float ease = EasingFunction.InOutSine(lerp);
            float startTime = MathHelper.Lerp(12, 2, ease);
            ForwardSlashStartupMovement(startTime);
            TargetOutlineColor = Color.Yellow;
            if (Timer >= startTime)
            {
                SwitchState(AIState.Special_Slash);
            }
        }

        private void CrossSlash()
        {
            if (MultiplayerHelper.IsHost)
            {
                Projectile.NewProjectile(SourceFromThis, _boxCenter, Vector2.UnitY, ModContent.ProjectileType<BlackSlashLine>(), BlackSlashLineDamage, 1,
                    Main.myPlayer);
                Projectile.NewProjectile(SourceFromThis, _boxCenter, Vector2.UnitX, ModContent.ProjectileType<BlackSlashLine>(), BlackSlashLineDamage, 1,
                    Main.myPlayer);
            }
        }

        private void XSlash()
        {
            if (MultiplayerHelper.IsHost)
            {
                Projectile.NewProjectile(SourceFromThis, _boxCenter, new Vector2(1, 1), ModContent.ProjectileType<BlackSlashLine>(), BlackSlashLineDamage, 1,
                    Main.myPlayer);
                Projectile.NewProjectile(SourceFromThis, _boxCenter, new Vector2(1, -1), ModContent.ProjectileType<BlackSlashLine>(), BlackSlashLineDamage, 1,
                    Main.myPlayer);
            }
        }

        private void RainSlash1()
        {
            if (MultiplayerHelper.IsHost)
            {
                for (int i = 0; i < 16; i++)
                {
                    Projectile.NewProjectile(SourceFromThis, _boxCenter - new Vector2(i * 24, 0), new Vector2(0, 1), ModContent.ProjectileType<BlackSlashLine>(), BlackSlashLineDamage, 1,
                        Main.myPlayer, ai1: i * 4);
                }
            }
        }

        private void RainSlash2()
        {
            if (MultiplayerHelper.IsHost)
            {
                for (int i = 0; i < 16; i++)
                {
                    Projectile.NewProjectile(SourceFromThis, _boxCenter + new Vector2(i * 24, 0), new Vector2(0, 1), ModContent.ProjectileType<BlackSlashLine>(), BlackSlashLineDamage, 1,
                        Main.myPlayer, ai1: i * 4);
                }
            }
        }
        private void VerticalSlash()
        {
            if (MultiplayerHelper.IsHost)
            {
                for (int i = 0; i < 3; i++)
                {
                    Vector2 spawnPoint = _boxCenter;
                    spawnPoint.X += Main.rand.NextFloat(-390, 390f);
                    Projectile.NewProjectile(SourceFromThis, spawnPoint, Vector2.UnitY, ModContent.ProjectileType<BlackSlashLine>(), BlackSlashLineDamage, 1,
                        Main.myPlayer, ai1: i * 4);
                }

            }
        }
        private void HorizontalSlash()
        {
            if (MultiplayerHelper.IsHost)
            {
                for (int i = 0; i < 3; i++)
                {
                    Vector2 spawnPoint = _boxCenter;
                    spawnPoint.Y += Main.rand.NextFloat(-390, 390f);
                    Projectile.NewProjectile(SourceFromThis, spawnPoint, Vector2.UnitX, ModContent.ProjectileType<BlackSlashLine>(), BlackSlashLineDamage, 1,
                        Main.myPlayer, ai1: i * 4);
                }

            }
        }
        private float _blackSlashrotateCounter;
        private void RotateSlash()
        {
            if (MultiplayerHelper.IsHost)
            {
                Vector2 spawnPoint = _boxCenter;

                Vector2 fireVelocity = Vector2.UnitY;
                fireVelocity = fireVelocity.RotatedBy(_blackSlashrotateCounter);
                Projectile.NewProjectile(SourceFromThis, spawnPoint, fireVelocity, ModContent.ProjectileType<BlackSlashLine>(), BlackSlashLineDamage, 1,
                 Main.myPlayer, ai2: 1);
                _blackSlashrotateCounter += MathHelper.TwoPi / 24f;
                _blackSlashrotateCounter += Main.rand.NextFloat(0.2f);
            }
        }
        private void AI_SpecialSlash()
        {
            Timer++;
            if (Timer == 1)
            {
                TargetVector = NPC.Center;
                _forwardVector = (NPC.Center - MyTarget.Center).SafeNormalize(Vector2.Zero);

                SoundStyle newSlashSound = new SoundStyle("Stellamod/Assets/Sounds/SwordSlice");
                newSlashSound.PitchVariance = 0.2f;
                newSlashSound.Volume = 0.5f;
                SoundEngine.PlaySound(newSlashSound, NPC.position);

                //Basically gonna be a really long attack pattern
                switch (_attackNumber)
                {
                    default:
                        RotateSlash();
                        break;
                    case 0:
                        CrossSlash();
                        break;
                    case 1:
                        XSlash();
                        break;
                    case 2:
                        CrossSlash();
                        break;
                    case 3:
                        XSlash();
                        break;
                    case 4:
                        RainSlash1();
                        break;
                    case 5:
                        CrossSlash();
                        break;
                    case 6:
                        XSlash();
                        break;
                    case 7:
                        CrossSlash();
                        break;
                    case 8:
                        XSlash();
                        break;
                    case 9:
                        RainSlash2();
                        break;
                    case 10:
                        CrossSlash();
                        break;
                    case 11:
                        XSlash();
                        break;
                    case 12:
                        CrossSlash();
                        break;
                    case 13:
                        XSlash();
                        break;
                    case 14:
                        RainSlash1();
                        RainSlash2();
                        break;
                    case 15:
                        GothFlare();
                        break;
                    case 16:
                        VerticalSlash();
                        break;
                    case 17:
                        VerticalSlash();
                        break;
                    case 18:
                        VerticalSlash();
                        break;
                    case 19:
                        VerticalSlash();
                        break;

                    case 20:
                        HorizontalSlash();
                        break;
                    case 21:
                        HorizontalSlash();
                        break;
                    case 22:
                        HorizontalSlash();
                        break;
                    case 23:
                        HorizontalSlash();
                        break;

                    case 24:
                        GothFlare();
                        break;
                }
                if (_attackNumber == 25)
                {
                    ShakeModSystem.Shake = 64;
                    FXUtil.ShakeCamera(NPC.position, 1024, 4);
                    ScreenSmearEffectManager.DiagonalCut();
                    SoundStyle hurriboom = AssetRegistry.Sounds.E.Hurriboom;
                    hurriboom.PitchVariance = 0.3f;
                    SoundEngine.PlaySound(hurriboom, NPC.position);
                }
                NPC.direction = _forwardVector.X > 0 ? -1 : 1;
            }
            _inRiver = true;
            _extraAfterImageAlpha = 0.7f;



            float forwardSlashTime = 5;
            if (_attackNumber == 15 || _attackNumber == 24)
            {
                Animator.PlayAnimation(Anim_Holding);
                forwardSlashTime = 120;
            }
            else
            {
                if (_attackNumber % 2 == 0)
                {
                    Animator.PlayAnimation(Anim_ForwardSlash);
                }
                else
                {
                    Animator.PlayAnimation(Anim_BackSlash);
                }

            }


            float completionRatio = Timer / forwardSlashTime;
            float ease = EasingFunction.OutSine(completionRatio);

            float maxRadians = MathHelper.PiOver2;
            float radiansOffset = completionRatio * maxRadians;
            Vector2 forwardVector = _forwardVector.RotatedBy(radiansOffset);
            Vector2 recoilStartVector = TargetVector;
            Vector2 recoilEndVector = recoilStartVector + forwardVector * 100;

            Vector2 recoilPosition = Vector2.Lerp(recoilStartVector, recoilEndVector, ease);
            Vector2 targetVelocity = recoilPosition - NPC.Center;
            NPC.velocity = targetVelocity;

            TargetOutlineColor = Color.Red;
            if (Timer >= forwardSlashTime)
            {


                SwitchState(AIState.Special_SlashReposition);
            }
        }

        private void AI_SpecialSlashReposition()
        {
            Timer++;
            if (Timer == 1)
            {
                _forwardVector = (NPC.Center - MyTarget.Center);
                TargetVector = NPC.velocity;
            }

            float speedUpRatio = _attackNumber / 4f;
            speedUpRatio = MathHelper.Clamp(speedUpRatio, 0f, 1f);
            float rotateTime = MathHelper.Lerp(45, 25, speedUpRatio);
            if (_attackNumber >= 24)
                rotateTime *= MathHelper.Lerp(0.5f, 0.05f, (_attackNumber - 24f) / 30f);
            float completionRatio = Timer / rotateTime;
            float maxRadians = MathHelper.PiOver4;
            float radiansOffset = completionRatio * maxRadians;
            Vector2 forwardVector = _forwardVector.RotatedBy(radiansOffset);
            Vector2 targetPosition = MyTarget.Center + forwardVector;
            Vector2 targetVelocity = targetPosition - NPC.Center;

            _inRiver = true;
            _extraAfterImageAlpha = 0.7f;
            float ease = EasingFunction.InOutSine(completionRatio);
            NPC.velocity = Vector2.Lerp(TargetVector, targetVelocity, ease);
            if (Timer >= rotateTime)
            {
                _attackNumber++;
                if (_attackNumber < 124)
                {
                    SwitchState(AIState.Special_SlashQuickStart);
                }
                else
                {
                    SwitchState(AIState.Special_SlashEndInBlack);
                }
            }
        }

        private void AI_SpecialSlashEndP1()
        {
            Timer++;
            NPC.velocity *= 0.9f;
            _inRiver = true;

            float fadeOuTime = 120;
            float completionRatio = Timer / fadeOuTime;
            ShakeModSystem.Shake = 8;
            if (Timer >= fadeOuTime)
            {
                SwitchState(AIState.Special_SlashEndOutBlack);
            }
        }

        private void KillBlackBox()
        {
            int type = ModContent.ProjectileType<BlackBox>();
            int type2 = ModContent.ProjectileType<BlackRiver>();
            foreach (var proj in Main.ActiveProjectiles)
            {
                if (proj.type == type || proj.type == type2)
                    proj.ai[1] = 1;
            }

        }
        private void AI_SpecialSlashEndP2()
        {
            Timer++;
            NPC.velocity *= 0.9f;
            float fadeOuTime = 120;
            float completionRatio = Timer / fadeOuTime;

            KillBlackBox();
            Animator.PlayAnimation(Anim_BattleIdle);
            if (Timer >= fadeOuTime)
            {
                SwitchState(AIState.Idle);
            }
        }
    }
}
