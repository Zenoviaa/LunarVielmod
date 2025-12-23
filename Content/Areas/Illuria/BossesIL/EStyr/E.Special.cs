using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Core.Shaders;
using Stellamod.Core.Shaders.MagicTrails;
using Stellamod.Core.Utilities;
using Stellamod.Helpers;
using Stellamod.Trails;
using Stellamod.UI.Systems;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Illuria.BossesIL.EStyr
{
    public interface IDrawBlackRiverMask
    {
        void DrawRiverMask();
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
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 1;
            Projectile.height = 1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 600;
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
        }

        public override bool PreDraw(ref Color lightColor)
        {
            BlackRiverRenderer.AddMask(this);
          //  DrawRiverMask();
            return false;
        }

        public void DrawRiverMask()
        {
            Projectile.Center = Main.Camera.Center + new Vector2(Main.screenWidth / 2f, 0);
            BasicLaserAlphaShader shader = BasicLaserAlphaShader.Instance;
            shader.OuterColor = Color.White;
            shader.InnerColor = Color.Black;
            shader.LaserTexture = ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/RiverMask");
            shader.Time = Main.GlobalTimeWrappedHourly * -16;
            float width = 1000;
            Vector2 start = Projectile.Center;
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
        private List<IDrawBlackRiverMask> _draws;
        public override void OnModLoad()
        {
            base.OnModLoad();
            _draws = new List<IDrawBlackRiverMask>();
            _riverRT = ManagedRenderTarget.New(GetScreenSize);
            _riverMaskRT = ManagedRenderTarget.New(GetScreenSize);
            _pixelRT = ManagedRenderTarget.New(GetScreenSize, 2);
            On_Main.CheckMonoliths += RenderRiverRT;
            On_Main.DrawPlayers_AfterProjectiles += DrawRiverToScreen;
        }



        public override void OnModUnload()
        {
            base.OnModUnload();
            On_Main.CheckMonoliths -= RenderRiverRT;
            On_Main.DrawPlayers_AfterProjectiles -= DrawRiverToScreen;
        }

        private void RenderRiverMaskRT()
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            if (_draws.Count > 0)
            {
                spriteBatch.GraphicsDevice.SetRenderTarget(_riverMaskRT);
                spriteBatch.GraphicsDevice.Clear(Color.Transparent);
                spriteBatch.Begin();
                for (int i = 0; i < _draws.Count; i++)
                {
                    IDrawBlackRiverMask mask = _draws[i];
                    mask.DrawRiverMask();
                }
                spriteBatch.End();
            }
            _draws.Clear();
        }

        private void RenderRiverTextureRT()
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            spriteBatch.GraphicsDevice.SetRenderTarget(_riverRT);
            spriteBatch.GraphicsDevice.Clear(Color.Black);

            MixerShader mixerShader = MixerShader.Instance;
            Asset<Texture2D> mixTexture = ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/Refraction");
            Asset<Texture2D> noiseTexture = ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/PerlinNoise");
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
            spriteBatch.Draw(_riverMaskRT,new Vector2(0, -2), null, Color.White, 0, Vector2.Zero, 0.5f, SpriteEffects.None, 0);
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
        private void DrawRiverToScreen(On_Main.orig_DrawPlayers_AfterProjectiles orig, Main self)
        {
            orig(self);
            DrawRiverToScreen();
        }

        private void DrawRiverToScreen()
        {
       
            SpriteBatch spriteBatch = Main.spriteBatch;
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer,null);
            spriteBatch.Draw(_pixelRT, Vector2.Zero, null, Color.White, 0, Vector2.Zero, 2, SpriteEffects.None, 0);
            spriteBatch.End();
        }

        public static void AddMask(IDrawBlackRiverMask mask)
        {
            BlackRiverRenderer renderer = ModContent.GetInstance<BlackRiverRenderer>();
            renderer._draws.Add(mask);
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

            float warnTime = 120f;
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
            if(Timer == 1)
            {
                TargetVector = NPC.velocity;
                if (MultiplayerHelper.IsHost)
                {
                    Projectile.NewProjectile(SourceFromThis, NPC.Center, -Vector2.UnitX, 
                        ModContent.ProjectileType<BlackRiver>(), 1, 1, Main.myPlayer);
                }
            }

            float dripDropTime = 240;
            float completionRatio = Timer / dripDropTime;
            float ease = EasingFunction.InOutSine(completionRatio);
            Vector2 positionToMoveTo = NPCAIHelper.CalculatePositionToMoveTo(MyTarget.Center, NPC.Center, new Vector2(64, -64));
            Vector2 targetVelocity = positionToMoveTo - NPC.Center;
            NPC.velocity = Vector2.Lerp(TargetVector, targetVelocity, ease);
            if(Timer >= dripDropTime)
            {
                SwitchState(AIState.Idle);
            }
        }
    }
}
