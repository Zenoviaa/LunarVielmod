using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary;
using Stellamod.Common.MaskingShaderSystem;
using Stellamod.Helpers;
using Stellamod.Items.Accessories.Runes;
using Stellamod.NPCs.Bosses.DaedusRework;
using Stellamod.Particles;
using Stellamod.Projectiles.Slashers.ScarecrowSaber;
using Stellamod.Trails;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;


namespace Stellamod.Projectiles.Magic
{
    internal class AlcadBombSuckDraw
    {
        public AlcadBombSuckDraw()
        {
            oldPos = new Vector2[16];
            scale = 1f;
            randScale = Main.rand.NextFloat(0.275f, 1f);
        }
        public Vector2[] oldPos;
        public Vector2 position;
        public float rotation;
        public float timer;
        public float scale;
        public float randScale;
        public Color ColorFunc(float p)
        {
            return Color.White;
        }

        public float WidthFunc(float p)
        {
            return MathHelper.Lerp(384, 0, Easing.InCubic(p)) * scale * randScale;
         }

        public void DrawPrims(PrimDrawer primDrawer)
        {
            primDrawer.WidthFunc = WidthFunc;
            primDrawer.ColorFunc = ColorFunc;
            GameShaders.Misc["VampKnives:SuperSimpleTrail"].SetShaderTexture(TrailRegistry.SimpleTrail);
            Vector2 trailOffset = -Main.screenPosition;
            primDrawer.DrawPrims(oldPos, trailOffset, 155);
        }
    }

    internal class AlcadBombProj : ModProjectile,
          IPreDrawMaskShader,
          IDrawMaskShader
    {
        private float _drawScale;
        private float _scaleOutMult;
        private List<AlcadBombSuckDraw> _suckDraws = new List<AlcadBombSuckDraw>();

        private ref float Timer => ref Projectile.ai[0];
        private ref float Die => ref Projectile.ai[1];
        public bool IsCharged => Timer >= 60;

        private Player Owner => Main.player[Projectile.owner];
        public override void SetDefaults()
        {
            Projectile.width = 128;
            Projectile.height = 128;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 5;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.timeLeft = int.MaxValue;
        }

        private bool ShouldConsumeMana()
        {
            return Timer % 7 == 0;
        }
        private void UpdateDamageForManaSickness(Player player)
        {
            Projectile.damage = (int)player.GetDamage(DamageClass.Magic).ApplyTo(player.HeldItem.damage);
        }
        private void AI_Attack()
        {
            //here we handle calculating when to attack!
            if (Main.myPlayer != Projectile.owner)
                return;

            Player player = Owner;
            UpdateDamageForManaSickness(player);
            bool stillInUse = player.channel &&  player.ownedProjectileCounts[ModContent.ProjectileType<AlcadBombHeldProj>()] > 0;
            if (!stillInUse)
            {
                Die = 1;
                Projectile.netUpdate = true;
            }
        }

        public override void AI()
        {
            base.AI();
            Timer++;
            Projectile.velocity = Vector2.Zero;
            if(Timer == 1 && Main.myPlayer == Projectile.owner)
            {
                float maxBeamLength = Vector2.Distance(Owner.Center, Main.MouseWorld);
                Vector2 direction = (Main.MouseWorld - Owner.Center).SafeNormalize(Vector2.Zero);
                float length = ProjectileHelper.PerformBeamHitscan(Owner.Center, direction, maxBeamLength);
                Projectile.Center = Owner.Center + direction * length;
                Projectile.netUpdate = true;
            }

            if(Timer == 10)
            {
                for(float f = 0; f < 32; f++)
                {
                    float progress = f / 32;
                    float rot = progress * MathHelper.TwoPi;
                    Vector2 vel = rot.ToRotationVector2() * 4;
                    Dust.NewDustPerfect(Projectile.Center, DustID.CorruptTorch, vel);
                }
                SoundStyle explodeStyle = new SoundStyle("Stellamod/Assets/Sounds/STARGROP");
                SoundEngine.PlaySound(explodeStyle, Projectile.position);
                Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(Projectile.position, 1024, 10);
            }
           
            if(Timer < 60)
            {
                float progress = Timer / 60f;
                float easedProgress = Easing.OutExpo(progress);
                float scale = MathHelper.Lerp(0f, Main.rand.NextFloat(0.95f, 1f), easedProgress);
                _drawScale = scale;
                _scaleOutMult = 1f;
            }

            if(Die == 0)
            {
                _drawScale += 0.005f;
                if (_drawScale >= 2f)
                {
                    _drawScale = 2f;
                }
            }
            else if (Die == 1)
            {
                _drawScale *= 0.926f;
                if(_drawScale <= 0.01f)
                {
                    Projectile.Kill();
                }
            }



            AI_Suck();
            AI_Attack();
            AI_KeepAlive();
            ManageSuckDraws();
        }

        private void AI_Suck()
        {
            if (Die == 1)
                return;
            foreach (NPC npc in Main.ActiveNPCs)
            {

                if (!npc.CanBeChasedBy())
                    continue;

                float distance = Vector2.Distance(npc.Center, Projectile.Center);
                float maxPullDistance = 800 * MathHelper.Clamp(Timer / 60f, 0f, 1f);
                if(distance <= maxPullDistance)
                {
                    Vector2 blowVelocity = Projectile.Center - npc.Center;
                    float p = distance / maxPullDistance;
                    p = 1f - p;
                    blowVelocity *= 0.0052f * p * MathHelper.Clamp(Timer / 60f, 0f, 1f);
                    npc.GetGlobalNPC<RuneOfWindBlowNPC>().BlowVelocity = blowVelocity;
                }
            }
        }
        private void AI_KeepAlive()
        {
            Player player = Main.player[Projectile.owner];
            if (player.noItems || player.CCed || player.dead || !player.active)
            {
                if (Main.myPlayer == Projectile.owner)
                {
                    Die = 1;
                    Projectile.netUpdate = true;
                }
            }

            if (Main.myPlayer == Projectile.owner)
            {
                if (!player.channel)
                {
                    Die = 1;
                    Projectile.netUpdate = true;
                }
            }
        }

        private void ManageSuckDraws()
        {
            float radius = 82;
            if(Timer % 1 == 0)
            {
                Vector2 pos = Projectile.Center + Main.rand.NextVector2CircularEdge(radius, radius);
                AlcadBombSuckDraw suckDraw = new AlcadBombSuckDraw
                {
                    position = pos,
                    rotation = (Projectile.Center - pos).SafeNormalize(Vector2.Zero).ToRotation()
                };
                _suckDraws.Add(suckDraw);
            }


            //Mange them all
            for(int i = 0; i < _suckDraws.Count; i++)
            {
                AlcadBombSuckDraw suckDraw = _suckDraws[i];
                suckDraw.timer++;
                float progress = suckDraw.timer / 60f;
                float easedProgress = Easing.InOutCubic(progress);
                suckDraw.position = Vector2.Lerp(suckDraw.position, Projectile.Center, easedProgress);
                suckDraw.scale = _drawScale;

                //Update old pos
                for (int j = suckDraw.oldPos.Length - 1; j > 0; j--)
                {
                    suckDraw.oldPos[j] = suckDraw.oldPos[j - 1];
                }
                if (suckDraw.oldPos.Length > 0)
                    suckDraw.oldPos[0] = suckDraw.position;
            }
        }

        public MiscShaderData GetMaskDrawShader()
        {
            //Use the defaults
            var shaderData = GameShaders.Misc["LunarVeil:SimpleDistortion"];
            shaderData.Shader.Parameters["time"].SetValue(Main.GlobalTimeWrappedHourly * 15);
            shaderData.Shader.Parameters["distortion"].SetValue(0.2f);
            shaderData.Shader.Parameters["distortingNoiseTexture"].SetValue(TextureRegistry.CloudNoise2.Value);
            return shaderData;
        }

        public void PreDrawMask(SpriteBatch spriteBatch)
        {
            var shaderData = GameShaders.Misc["LunarVeil:SimpleDistortion"];
            shaderData.Shader.Parameters["time"].SetValue(Main.GlobalTimeWrappedHourly * 15);
            shaderData.Shader.Parameters["distortion"].SetValue(0.2f);
            shaderData.Shader.Parameters["distortingNoiseTexture"].SetValue(TextureRegistry.CloudNoise2.Value);
            shaderData.Apply();

            Texture2D texture = ModContent.Request<Texture2D>(Texture + "_Outline").Value;
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            Color drawColor = Color.White;
            Vector2 drawOrigin = texture.Size() / 2f;
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer,
                   shaderData.Shader, Main.GameViewMatrix.TransformationMatrix);

            for (int i = 0; i < _suckDraws.Count; i++)
            {
                AlcadBombSuckDraw suckDraw = _suckDraws[i];
                DrawSuckParticle2(spriteBatch, suckDraw);
            }

            //spriteBatch.Draw(texture, drawPos, null, drawColor, Projectile.rotation, drawOrigin, _drawScale * _scaleOutMult, SpriteEffects.None, 0f);
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
        }

        public PrimDrawer PrimDrawer { get; set; }
        private void DrawSuckParticle(SpriteBatch spriteBatch, AlcadBombSuckDraw draw)
        {
            //PrimDrawer ??= new PrimDrawer(null, null, GameShaders.Misc["VampKnives:SuperSimpleTrail"]);
            //draw.DrawPrims(PrimDrawer);

            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawPos = draw.position - Main.screenPosition;
            Color drawColor = Color.White;
            Vector2 drawOrigin = texture.Size() / 2f;
            Vector2 drawScale = new Vector2(4f, 0.25f) * draw.randScale;
            drawScale *= Easing.SpikeOutCirc(draw.timer / 60f);
            float drawRot = draw.rotation;
            spriteBatch.Draw(texture, drawPos, null, drawColor, drawRot, drawOrigin, drawScale * _drawScale * _scaleOutMult, SpriteEffects.None, 0f);
        }

        private void DrawSuckParticle2(SpriteBatch spriteBatch, AlcadBombSuckDraw draw)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture + "_Outline").Value;
            Vector2 drawPos = draw.position - Main.screenPosition;
            Color drawColor = Color.White;
            Vector2 drawOrigin = texture.Size() / 2f;
            Vector2 drawScale = new Vector2(4f, 0.25f) * draw.randScale;
            drawScale *= Easing.SpikeOutCirc(draw.timer / 60f);
            float drawRot = draw.rotation;
            spriteBatch.Draw(texture, drawPos, null, drawColor, drawRot, drawOrigin, drawScale * _drawScale * _scaleOutMult, SpriteEffects.None, 0f);
        }

        public void DrawMask(SpriteBatch spriteBatch)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            Color drawColor = Color.White;
            Vector2 drawOrigin = texture.Size() / 2f;

            for(int i = 0; i < _suckDraws.Count; i++)
            {
                AlcadBombSuckDraw suckDraw = _suckDraws[i];
                DrawSuckParticle(spriteBatch, suckDraw);
            }

            //Draw Main Texture
          //  spriteBatch.Draw(texture, drawPos, null, drawColor, Projectile.rotation, drawOrigin, _drawScale * _scaleOutMult, SpriteEffects.None, 0f);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }
    }
}
