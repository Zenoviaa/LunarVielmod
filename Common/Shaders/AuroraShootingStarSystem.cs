using ReLogic.Content;
using Stellamod.Assets;
using Stellamod.Helpers;
using System;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.ModLoader;

namespace Stellamod.Common.Shaders;

[Autoload(Side = ModSide.Client)]
public class AuroraShootingStarSystem : ModSystem
{
    public struct AuroraStar
    {
        public Vector2 position;
        public Vector2 velocity;
        public float time;
        public bool active;
    }

    private int _lastIndex;
    private float _activeTimer;
    private float _particleSpawnTimer;
    private float[] _alphaValues;
    private AuroraStar[] _starParticles;

    public override void Load()
    {
        base.Load();
        _alphaValues = new float[120];
        CalculateAlphaValues();
        _starParticles = new AuroraStar[100];
        On_OverlayManager.Draw += DrawAuroraStars;
    }

    private void CalculateAlphaValues()
    {
        for (int i = 0; i < _alphaValues.Length; i++)
        {
            float tick = i + 1;
            float inAlpha = MathHelper.Lerp(0f, 1f, EasingFunction.InOutSine(tick / 30f));
            float outAlpha = MathHelper.Lerp(1f, 0f, EasingFunction.InOutSine((tick-90) / 30f));
            _alphaValues[i] = inAlpha * outAlpha;
        }
    }

    public override void Unload()
    {
        base.Unload();
        On_OverlayManager.Draw -= DrawAuroraStars;
    }

    public void SpawnStarParticle(AuroraStar star)
    {
        for (int i = 0; i < _starParticles.Length; i++)
        {
            _lastIndex++;
            _lastIndex %= _starParticles.Length;
            ref AuroraStar starParticle = ref _starParticles[_lastIndex];
            if (!starParticle.active)
            {
                starParticle = star;
                starParticle.active = true;
                break;
            }
        }
    }

    private void UpdateParticles()
    {
     
        float starTime = 120;
        Vector2 cameraMovement = Main.screenPosition - Main.screenLastPosition;
        for (int i = 0; i < _starParticles.Length; i++)
        {
            ref AuroraStar starParticle = ref _starParticles[i];
            if (!starParticle.active)
            {
                continue;
            }

            starParticle.position += starParticle.velocity;
            Vector2 parallax = cameraMovement * 0.9f;
            parallax *= starParticle.velocity.Length() * 0.2f;
            starParticle.position -= parallax;
            starParticle.time++;
            if (starParticle.time >= starTime)
            {
                starParticle.active = false;
            }
        }
    }
    private void DrawAuroraStars(On_OverlayManager.orig_Draw orig, OverlayManager self, SpriteBatch spriteBatch, RenderLayers layer, bool beginSpriteBatch)
    {
        if (layer == RenderLayers.Background)
        {
            if (!Main.gameMenu && _activeTimer > 0)
            {
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Deferred,
                      BlendState.AlphaBlend,
                      SamplerState.PointWrap,
                      DepthStencilState.None,
                      RasterizerState.CullCounterClockwise,
                      null);

                //Render all particles
                RenderStarParticles(spriteBatch);

                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);
            }
        }

        orig(self, spriteBatch, layer, beginSpriteBatch);
    }

    private void RenderStarParticles(SpriteBatch spriteBatch)
    {
        Asset<Texture2D> glint = AssetManager.GlowMask.ShootingStarGlint;
        //Draw glint
        Vector2 glintDrawOrigin = glint.Size() * 0.5f;

        Asset<Texture2D> trail = AssetManager.GlowMask.ShootingStarTrail;
        Vector2 trailDrawOrigin = new Vector2(trail.Width(), trail.Height() * 0.5f);
        for (int i = 0; i < _starParticles.Length; i++)
        {
            ref AuroraStar starParticle = ref _starParticles[i];
            if (!starParticle.active)
            {
                continue;
            }

            int alphaIndex = (int)starParticle.time;
            if (alphaIndex >= 120)
                alphaIndex = 120 - 1;
            float alpha = _alphaValues[alphaIndex];
            Color trailDrawColor = Color.White;
            trailDrawColor *= alpha;
            trailDrawColor.A = 0;
            spriteBatch.Draw(trail.Value, starParticle.position, null, trailDrawColor, starParticle.velocity.ToRotation(), trailDrawOrigin, Vector2.One, SpriteEffects.None, 0);

            Color glintDrawColor = Color.White;
            glintDrawColor *= alpha;
            glintDrawColor *= ExtraMath.Osc(0.5f, 1f, speed: 16, offset: i);
            glintDrawColor.A = 0;
            Vector2 glintDrawScale = Vector2.One * 1f;
            spriteBatch.Draw(glint.Value, starParticle.position, null, glintDrawColor, 0, glintDrawOrigin, glintDrawScale, SpriteEffects.None, 0);
        }
    }

    public override void PostUpdateEverything()
    {
        base.PostUpdateEverything();
        UpdateParticles();
        if (Main.LocalPlayer.ZoneSnow && !Main.dayTime && Main.LocalPlayer.ZoneOverworldHeight)
        {
            _activeTimer++;
        }
        else
        {
            _activeTimer--;
        }

        _activeTimer = Math.Clamp(_activeTimer, 0f, 60f);
        if (_activeTimer <= 0)
            return;

        _particleSpawnTimer++;
        if (_particleSpawnTimer % 5 == 0)
        {
            Rectangle spawnRectangle = new Rectangle(0, 0, Main.screenWidth, (int)(Main.screenHeight * 0.5f));
            float randX = Main.rand.NextFloat();
            float randY = Main.rand.NextFloat();
            float x = MathHelper.Lerp(spawnRectangle.Left, spawnRectangle.Right, randX);
            float y = MathHelper.Lerp(spawnRectangle.Top, spawnRectangle.Bottom, randY);
            Vector2 spawnPosition = new Vector2(x, y);
            Vector2 spawnVelocity = new Vector2(-1, 1);
            spawnVelocity = spawnVelocity.SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(2f, 5f);
            AuroraStar auroraStar = new AuroraStar
            {
                position = spawnPosition,
                velocity = spawnVelocity,
            };
            SpawnStarParticle(auroraStar);
        }
    }
}
