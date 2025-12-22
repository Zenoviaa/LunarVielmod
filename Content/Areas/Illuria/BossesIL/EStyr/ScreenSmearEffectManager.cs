using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Core.Shaders;
using Stellamod.Core.Utilities;
using Stellamod.Helpers;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Illuria.BossesIL.EStyr
{

    [Autoload(Side = ModSide.Client)]
    public class ScreenSmearEffectManager : ModSystem
    {
        //Represents a single screen smear
        //What we're going to do is basically have it animate between the start and end positions
        //Then linger for a bit while shrinking
        private class SmearParticle
        {
            public Vector2 startPosition;
            public Vector2 endPosition;
            public float time;
            public float travelTime;
            public float lingerTime;
            public float strength;
            public bool screenBound;
            public Vector2 scale;
        }
        private ManagedRenderTarget _smearMaskRT;
        private List<SmearParticle> _particles;
        public override void OnModLoad()
        {
            base.OnModLoad();
            _particles = new();
            _smearMaskRT = ManagedRenderTarget.New(GetScreenSize);
            On_Main.CheckMonoliths += RenderSmearRT;
            On_Main.DoDraw += DrawToScreen;
       
        }

        public override void OnModUnload()
        {
            base.OnModUnload();
            On_Main.CheckMonoliths -= RenderSmearRT;
            On_Main.DoDraw -= DrawToScreen;
        }

        private void DrawToScreen(On_Main.orig_DoDraw orig, Main self, GameTime gameTime)
        {
            orig(self, gameTime);
            if (Main.gameMenu)
                return;
        }

        public override void PostUpdateDusts()
        {
            base.PostUpdateDusts();
            UpdateSmears();
        }

        private void UpdateSmears()
        {
            if(_particles.Count > 0)
            {
                for (int i = 0; i < _particles.Count; i++)
                {
                    var smear = _particles[i];
                    smear.time++;
                }
                _particles.RemoveAll(x => x.time >= x.travelTime + x.lingerTime);
            }

        }
        private void RenderSmearRT(On_Main.orig_CheckMonoliths orig)
        {
            GraphicsDevice graphicsDevice = Main.graphics.GraphicsDevice;
            SpriteBatch spriteBatch = Main.spriteBatch;
            graphicsDevice.SetRenderTarget(_smearMaskRT);
            graphicsDevice.Clear(Color.Black);
            if(_particles.Count > 0)
            {
                string path = this.GetType().DirectoryHere() + "/Smear";
                Texture2D smearTexture = ModContent.Request<Texture2D>(path).Value;
                Vector2 drawOrigin = new Vector2(0, smearTexture.Height / 2);

                DarkSmearWriteShader writeShader = DarkSmearWriteShader.Instance;
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, 
                    Main.Rasterizer, writeShader.Effect);

                //Loop through all of our smears and draw them to the mask
                for (int i = 0; i < _particles.Count; i++)
                {
                    var smear = _particles[i];
                    Vector2 velocity = smear.endPosition - smear.startPosition;
                    Vector2 normalVelocity = velocity.SafeNormalize(Vector2.Zero);
                    float angle = MathF.Atan2(-velocity.Y, -velocity.X);

                    //Normalize the angle between 0-1
                    float normalAngle = angle / MathHelper.Pi * 0.5f + 0.5f;

                    Vector2 drawPosition = smear.startPosition - Main.screenPosition;

                    //Yeah this has some weird side effects ok but we're not using it for anything else
                    if (smear.screenBound)
                    {
                        drawPosition += Main.screenPosition;
                        float lerp = smear.time / smear.travelTime;
                        float strength = MathHelper.SmoothStep(1f, 0f, lerp);

                        Color smearColor = new Color(strength * smear.strength, normalAngle, velocity.Y);
                        float rotation = velocity.ToRotation();

                        Vector2 scale = Vector2.One;

                        float distance = Vector2.Distance(smear.startPosition, smear.endPosition);
                        scale.X = distance / (float)smearTexture.Width;
                        scale.Y = 2;
                        scale *= smear.scale;
                        spriteBatch.Draw(smearTexture, drawPosition, null, smearColor, velocity.ToRotation(), drawOrigin, scale, SpriteEffects.None, 0);
                    } 
                    else
                    {


                        float lerp = smear.time / smear.travelTime;
                        float strength = MathHelper.SmoothStep(0f, 1f, lerp);
                        Color smearColor = new Color(strength * smear.strength, normalAngle, velocity.Y);
                        float rotation = velocity.ToRotation();

                        Vector2 scale = Vector2.One;

                        float distance = Vector2.Distance(smear.startPosition, smear.endPosition);
                        scale.X = distance / (float)smearTexture.Width;
                        scale.Y = MathHelper.SmoothStep(2, 0f, lerp); ;
                        scale *= smear.scale;
                        spriteBatch.Draw(smearTexture, drawPosition, null, smearColor, velocity.ToRotation(), drawOrigin, scale, SpriteEffects.None, 0);
                    }

                }
                spriteBatch.End();
                //Apply to the screne shader
                DarkSmear s = ScreenShader.GetInstance<DarkSmear>();
                s.maskTexture = _smearMaskRT;
                s.strength = 1000;
                s.alpha = 1;
            }
          
            graphicsDevice.SetRenderTarget(null);


            orig();
        }


        public static void NewParticle(Vector2 position, Vector2 direction, float length, float time, float strength = 1f)
        {
            if (Main.netMode == NetmodeID.Server)
                return;

            ScreenSmearEffectManager smearDrawManager = ModContent.GetInstance<ScreenSmearEffectManager>();
            SmearParticle particle = new SmearParticle
            {
                startPosition = position,
                endPosition = position + direction * length,
                time = 0,
                travelTime = time,
                lingerTime = time / 2f,
                strength = strength,
                scale = Vector2.One,
            };
            smearDrawManager._particles.Add(particle);
        }
        public static void DiagonalCut()
        {
            if (Main.netMode == NetmodeID.Server)
                return;

            ScreenSmearEffectManager smearDrawManager = ModContent.GetInstance<ScreenSmearEffectManager>();

            Vector2 position = new Vector2(900, -450);
            SmearParticle particle = new SmearParticle
            {
                startPosition = position,
                endPosition = position + new Vector2(1280),
                time = 0,
                travelTime = 400,
                lingerTime = 300 / 2f,
                strength = 0.2f,
                screenBound = true,
                scale = new Vector2(2, 16)
            };
            smearDrawManager._particles.Add(particle);
            /*
            position = new Vector2(900, -450);
            particle = new SmearParticle
            {
                startPosition = position,
                endPosition = position + new Vector2(-1280, 1280),
                time = 0,
                travelTime = 400,
                lingerTime = 300 / 2f,
                strength = 0.2f,
                screenBound = true,
                scale = new Vector2(2, 16)
            };
            smearDrawManager._particles.Add(particle);*/
        }
        private Point GetScreenSize()
        {
            return new Point(Main.screenTarget.Width, Main.screenTarget.Height);
        }
    }
}
