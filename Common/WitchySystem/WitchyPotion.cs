using Stellamod.Common.Shaders;
using Stellamod.Core.Rendering.RTs;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.ResourceSets;
using Terraria.GameInput;
using Terraria.ModLoader;
using Terraria.UI.Chat;

namespace Stellamod.Common.WitchySystem;
public class WitchyPotion : ModResourceDisplaySet
{
    PlayerStatsSnapshot _snapShot;
    PotionLiquidRenderer PotionLiquidRenderer => field ??= new();
    Vector2[] PointBuffer => field ??= new Vector2[64];
    Vector2 TopLeftLiquid => TopLeftBar + new Vector2(0, 0);
    Vector2 TopLeftBar
    {
        get
        {
            var offset = new Vector2();
            offset.X += Main.screenWidth - TowardsLeft - BarOffsetTowardsLeft;
            offset.X -= Steps * 32;
            offset.X -= 16;
            offset.Y += TowardsBottom + BarOffsetTowardsBottom;
            offset.Y -= 100;
            return offset;
        }
    }
    Rectangle LiquidRect
    {
        get
        {
            var rect = new Rectangle((int)TopLeftBar.X, (int)TopLeftBar.Y, LiquidRectWidth, LiquidRectHeight);
            return rect;
        }
    }
    Rectangle PotionGlowMaskRect
    {
        get
        {
            return new(68, 4, 172, 162);
        }
    }
    Rectangle PotionBarGlowMaskRect
    {
        get
        {
            return new(32, 4, 32, 48);
        }
    }
    Rectangle PotionEdgeBarGlowMaskRect
    {
        get
        {
            return new(6, 4, 22, 48);
        }
    }
    bool DitheredVersion => false;
    int LiquidRectWidth => 528;
    int LiquidRectHeight => 192;
    int TowardsLeft => 128;
    int TowardsBottom => 111;
    int BarOffsetTowardsLeft => 111;
    int BarOffsetTowardsBottom => 18;
    int BarOffset2TowardsBottom => 32;
    int Steps => (_snapShot.LifeMax - 200) / 20;
    float HealthInBar
    {
        get
        {
            return 200;
        }
    }
    public override void PreDrawResources(PlayerStatsSnapshot snapshot)
    {
        base.PreDrawResources(snapshot);
        _snapShot = snapshot;
    }

    public override void DrawLife(SpriteBatch spriteBatch)
    {
        base.DrawLife(spriteBatch);

        if (Main.gameMenu)
            return;


        void DrawInner()
        {
            void DrawMask()
            {
                var gradientDrawer = SpritebatchDrawer.FromTextureGetLightingColor(AssetReferences.Assets.Textures.UI.Witchy.PotionGuardBackingGradient.Asset, Main.screenPosition);
                gradientDrawer.color = Color.White;
                gradientDrawer.worldPosition.X += Main.screenWidth - TowardsLeft;
                gradientDrawer.worldPosition.Y += TowardsBottom;
                spriteBatch.Draw(gradientDrawer);

                var barDrawer = SpritebatchDrawer.FromTextureGetLightingColor(AssetReferences.Assets.Textures.UI.Witchy.PotionBar.Asset, Main.screenPosition);
                barDrawer.VerticalFrame(2, 4);
                barDrawer.color = Color.White;
                barDrawer.worldPosition.X += Main.screenWidth - TowardsLeft - BarOffsetTowardsLeft;
                barDrawer.worldPosition.Y += TowardsBottom + BarOffsetTowardsBottom + BarOffset2TowardsBottom;
                spriteBatch.Draw(barDrawer);


                var steps = (_snapShot.LifeMax - 200) / 20;
                var x = barDrawer.worldPosition.X;
                barDrawer.VerticalFrame(3, 4);
                barDrawer.color = Color.White;
                for (var i = 0; i < steps - 1; i++)
                {
                    barDrawer.worldPosition.X -= 32;
                    spriteBatch.Draw(barDrawer);
                }


                var barEdgeDrawer = SpritebatchDrawer.FromTextureGetLightingColor(AssetReferences.Assets.Textures.UI.Witchy.PotionBarEdge.Asset, Main.screenPosition);
                barEdgeDrawer.VerticalFrame(2, 3);
                barEdgeDrawer.color = Color.White;
                barEdgeDrawer.worldPosition.X = barDrawer.worldPosition.X - 32 - 2;
                barEdgeDrawer.worldPosition.Y += TowardsBottom + BarOffsetTowardsBottom;
                spriteBatch.Draw(barEdgeDrawer);
            }

            void DrawGlowMask()
            {
                var tex = DitheredVersion ? AssetReferences.Assets.Textures.UI.Witchy.PotionGuardGlowMaskDithered.Asset : AssetReferences.Assets.Textures.UI.Witchy.PotionGuardGlowMask.Asset;
                var barEdgeDrawer = SpritebatchDrawer.FromTextureGetLightingColor(tex, Main.screenPosition);
                barEdgeDrawer.sourceRect = PotionGlowMaskRect;
                barEdgeDrawer.drawOrigin = Vector2.Zero;
                barEdgeDrawer.color = Color.White * ExtraMath.Osc(0.5f, 1f, speed: 2F);
                barEdgeDrawer.worldPosition = new Vector2(Main.screenWidth - 227, 40) + Main.screenPosition;
                spriteBatch.Draw(barEdgeDrawer);

                barEdgeDrawer.sourceRect = PotionBarGlowMaskRect;

                //            barEdgeDrawer.worldPosition = new Vector2(Main.screenWidth - 227, 40) + Main.screenPosition;
                var steps = (_snapShot.LifeMax - (int)HealthInBar) / 20;
                var x = barEdgeDrawer.worldPosition.X;
                barEdgeDrawer.worldPosition.X = x - 32;
                spriteBatch.Draw(barEdgeDrawer);
                for (var i = 0; i < steps - 1; i++)
                {
                    barEdgeDrawer.worldPosition.X -= 32;
                    spriteBatch.Draw(barEdgeDrawer);
                }
                barEdgeDrawer.worldPosition.X -= 22;
                barEdgeDrawer.sourceRect = PotionEdgeBarGlowMaskRect;
                spriteBatch.Draw(barEdgeDrawer);
            }


            var barDrawer = SpritebatchDrawer.FromTextureGetLightingColor(AssetReferences.Assets.Textures.UI.Witchy.PotionBar.Asset, Main.screenPosition);
            barDrawer.worldPosition.X += Main.screenWidth - TowardsLeft - BarOffsetTowardsLeft;
            barDrawer.worldPosition.Y += TowardsBottom + BarOffsetTowardsBottom + BarOffset2TowardsBottom;
            barDrawer.VerticalFrame(1, 4);
            barDrawer.color = Color.White;


            var steps = (_snapShot.LifeMax - (int)HealthInBar) / 20;
            var x = barDrawer.worldPosition.X;
            barDrawer.worldPosition.X = x;
            spriteBatch.Draw(barDrawer);
            for (var i = 0; i < steps - 1; i++)
            {
                barDrawer.worldPosition.X -= 32;
                spriteBatch.Draw(barDrawer);
            }


            var barEdgeDrawer = SpritebatchDrawer.FromTextureGetLightingColor(AssetReferences.Assets.Textures.UI.Witchy.PotionBarEdge.Asset, Main.screenPosition);
            barEdgeDrawer.color = Color.White;
            barEdgeDrawer.worldPosition.X = barDrawer.worldPosition.X - 32 - 2;
            barEdgeDrawer.worldPosition.Y += TowardsBottom + BarOffsetTowardsBottom;
            barEdgeDrawer.VerticalFrame(1, 3);
            spriteBatch.Draw(barEdgeDrawer);



            DrawMask();


            var drawer = SpritebatchDrawer.FromTextureGetLightingColor(AssetReferences.Assets.Textures.UI.Witchy.PotionGuard.Asset, Main.screenPosition);
            drawer.color = Color.White;
            drawer.worldPosition.X += Main.screenWidth - TowardsLeft;
            drawer.worldPosition.Y += TowardsBottom;
            spriteBatch.Draw(drawer);

            /*
            var rect = AssetReferences.Assets.GlowMasks.WhiteSquare.Asset;
            spriteBatch.Draw(rect.Value, LiquidRect, Color.White);
            */


            var index = 0;
            var remaining = _snapShot.Life - HealthInBar;
            var remainingMax = _snapShot.LifeMax - HealthInBar;
            var lifeRatio = remaining / (float)remainingMax;
            lifeRatio = 1f - lifeRatio;

            var lifeRatio2 = (float)MathHelper.Clamp(_snapShot.Life / HealthInBar, 0f, 1f);
            lifeRatio2 = 1f - lifeRatio2;
            //Nowe we need the center point of the ball and to spam points around it
            var ballCenter = new Vector2(Main.screenWidth - 138, 124);


            void Push(Vector2 pos)
            {
                if (PointBuffer.Length <= index)
                    return;
                PointBuffer[index++] = pos;
            }
            var rightBar = new Vector2(TopLeftBar.X + 562, TopLeftBar.Y + 36);
            var midPoint = TopLeftBar.X + rightBar.X;
            midPoint *= 0.5F;
            for (var j = 0; j < 3; j++)
            {
                for (var i = TopLeftBar.X; i <= rightBar.X; i += 32)
                {
                    if ((j == 1 || j == 2) && i < midPoint)
                        continue;

                    var pos = new Vector2(i + 24, TopLeftBar.Y + 64);
                    if (j == 0)
                    {
                        pos.X = MathHelper.Lerp(pos.X, rightBar.X, lifeRatio * 0.3f);
                    }
                    pos = Vector2.Lerp(pos, pos + new Vector2(0, 76), lifeRatio);
                    pos.Y += j * 80;
                    Push(pos);
                }
            }


            /*

            for (var i = 0; i < 4; i++)
            {
                var ratio2 = (float)i / 4F;
                for (var j = 0; j < 4; j++)
                {
                    var ratio = (float)j / 4F;
                    var ballOffset = new Vector2();
                    ballOffset.X = MathHelper.Lerp(0f, 128, ratio2);

                    ballOffset.Y += ratio * 128;
                    ballOffset.X -= 48;
                    var healthPos = ballCenter + ballOffset;
                    healthPos = Vector2.Lerp(healthPos, healthPos + new Vector2(0, LowPullOfset), lifeRatio2);
                    Push(healthPos);

                }
            }
            Push(ballCenter);*/
            PotionLiquidRenderer.waveAmplitude = 0.02f;
            PotionLiquidRenderer.waveFrequency = 0.025f;
            PotionLiquidRenderer.SimulateWithInitialPotions(PointBuffer, TopLeftBar, LiquidRect.Size());
            PotionLiquidRenderer.SimulateBackWithInitialPotions(PointBuffer, TopLeftBar, LiquidRect.Size());

            using var liquidTarget = RT.Context(RenderTargets.ScreenTarget);

            using var potionTarget = RT.Context(RenderTargets.ScreenTarget);
            using var glowMaskOcclusionTarget = RT.Context(RenderTargets.ScreenTarget);

            spriteBatch.EndOut(out var oldParameters);
            using (RT.Clear(new RenderTargetBinding[2] { potionTarget.Target, glowMaskOcclusionTarget.Target }, Color.Transparent))
            {
                PotionLiquidRenderer.Render(spriteBatch, TopLeftLiquid);
            }
            using (RT.Clear(liquidTarget, Color.Transparent))
            {
                var white = SpriteWhiteShader.Instance;


                using (spriteBatch.Ctx(oldParameters with { sortMode = SpriteSortMode.Deferred, effect = white }))
                {
                    DrawMask();
                }
                using (spriteBatch.Ctx(oldParameters with { blendState = CustomBlendStates.Multiply, matrix = Matrix.Identity }))
                {
                    spriteBatch.Draw(potionTarget, Vector2.Zero, Color.White);
                }
            }



            spriteBatch.Begin(oldParameters with { matrix = Matrix.Identity });
            spriteBatch.Draw(liquidTarget, Vector2.Zero, Color.White);
            spriteBatch.End();

            using (RT.Clear(liquidTarget, Color.Transparent))
            {
                //Draw glow masks

                var hueShift = AssetReferences.Effects.Generic.LiquidHealth.HueShift.CreatePixelPass();
                var hs = Main.LocalPlayer.GetModPlayer<PotionHealthPlayer>().hueShift;
                if(hs != 0)
                {

                    hueShift.Parameters.hueShift = hs;
                    hueShift.Apply();
                    using (spriteBatch.Ctx(oldParameters with
                    {
                        effect = hueShift.Shader,
                        sortMode = SpriteSortMode.Deferred,
                        blendState = BlendState.AlphaBlend
                    }))
                    {
                        DrawGlowMask();
                    }
                }
                else
                {
                    using (spriteBatch.Ctx(oldParameters with
                    {
                        sortMode = SpriteSortMode.Deferred,
                        blendState = BlendState.AlphaBlend
                    }))
                    {
                        DrawGlowMask();
                    }
                }

            }


            var liquidCombine = AssetReferences.Effects.Generic.LiquidHealth.LiquidCombine.CreatePixelPass();
            liquidCombine.Parameters.mixSampler = new()
            {
                Texture = glowMaskOcclusionTarget,
                Sampler = SamplerState.LinearClamp
            };
            liquidCombine.Apply();
            spriteBatch.Begin(oldParameters with { effect = liquidCombine.Shader, matrix = Matrix.Identity });
            spriteBatch.Draw(liquidTarget, Vector2.Zero, Color.White);
            spriteBatch.End();


            if (PotionHealthPlayer.hurtTimer > 0)
            {
                var white2 = SpriteWhiteShader.Instance;
                spriteBatch.Begin(oldParameters with { effect = white2, matrix = Matrix.Identity });
                spriteBatch.Draw(liquidTarget, Vector2.Zero, Color.Red * EasingFunction.OutQuad(PotionHealthPlayer.hurtTimer / 30f));
                spriteBatch.End();
            }

            spriteBatch.Begin(oldParameters);

            /*
            foreach (var pos in PointBuffer)
            {
                var d = SpritebatchDrawer.FromTextureAsset(AssetReferences.Assets.GlowMasks.SimpleGlowCircle.Asset, pos + Main.screenPosition);
                d.scale *= 0.05F;
                d.color = Color.White;
                d.color.A = 0;
                spriteBatch.Draw(d);
            }*/
            var font = FontAssets.DeathText.Value;
            var healthText = Main.LocalPlayer.statLife.ToString();
            var size = font.MeasureString(healthText);
            var origin = size * 0.5F;
            var healthScale = 1.25f;
            ChatManager.DrawColorCodedStringWithShadow(spriteBatch, font,
            healthText, ballCenter + new Vector2(0, 0), Color.White, 0, origin, Vector2.One * healthScale);

            var maxHealthText = "/" + Main.LocalPlayer.statLifeMax2.ToString();
            size = font.MeasureString(maxHealthText);
            origin = size * 0.5F;
            ChatManager.DrawColorCodedStringWithShadow(spriteBatch, font,
                maxHealthText, ballCenter + new Vector2(0, 32), Color.DarkGray, 0, origin, Vector2.One * 0.5F * healthScale);
        }
        float uIScale = Main.UIScale;
        float downScale = uIScale - 0.34F;
        spriteBatch.EndOut(out var oldParameters);
        Main.UIScale = downScale;
        PlayerInput.SetZoom_Scaled(1f / downScale);
        spriteBatch.Begin(oldParameters with { matrix = Main.UIScaleMatrix });

        DrawInner();

        spriteBatch.End();

        Main.UIScale = uIScale;
        PlayerInput.SetZoom_Scaled(1f / uIScale);

        spriteBatch.Begin(oldParameters with { matrix = Main.UIScaleMatrix });

    }
    public override void DrawMana(SpriteBatch spriteBatch)
    {
        base.DrawMana(spriteBatch);
    }

}
