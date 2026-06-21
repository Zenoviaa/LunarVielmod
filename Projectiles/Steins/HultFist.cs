using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Core.Pixelation;
using Stellamod.Core.SwingSystem;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Items.Accessories.Players;
using Stellamod.Items.Weapons.Mage.Stein;
using Stellamod.Projectiles.IgniterExplosions.Stein;
using Stellamod.Trails;
using Stellamod.Visual.Particles;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Steins
{
    public class HultFist : ModProjectile
    {
        private Vector2 _originalPosition;
        public int SwingTime = 60;
        public float holdOffset = 0f;
        public bool bounced = false;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Slasher");
            Main.projFrames[Projectile.type] = 1;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 20; // The length of old position to be recorded
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2; // The recording mode
        }
        public override void SetDefaults()
        {
            Projectile.damage = 10;
            Projectile.timeLeft = SwingTime;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.height = 100;
            Projectile.width = 100;
            Projectile.friendly = true;
            Projectile.scale = 1f;
        }
        int timer = 0;
        public float Timer
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }
        public virtual float Lerp(float val)
        {
            return val == 1f ? 1f : (val == 1f ? 1f : (float)Math.Pow(2, val * 6.5f - 5f) / 2f);
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            base.SendExtraAI(writer);
            writer.WriteVector2(_originalPosition);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            base.ReceiveExtraAI(reader);
            _originalPosition = reader.ReadVector2();
        }

        public override void AI()
        {
            Timer++;
            if(Timer == 1)
            {
                _originalPosition = Projectile.Center;
            }

            AttachToPlayer();
        }
        bool Beans = false;

        public void AttachToPlayer()
        {
            Player player = Main.player[Projectile.owner];
            if (!player.active || player.dead || player.CCed || player.noItems)
                return;
            Vector2 teleportPosition = Main.MouseWorld;
            timer++;
            if (timer == 5 && Main.myPlayer == Projectile.owner)
            {
                if (Collision.CanHitLine(player.Center, 1, 1, teleportPosition, 1, 1))
                {
                    player.Teleport(teleportPosition, 6);
                    NetMessage.SendData(MessageID.TeleportEntity, -1, -1, null, 0, player.whoAmI, teleportPosition.X, teleportPosition.Y, 1);
                    float speed = 5;
                    Projectile.velocity = Projectile.DirectionTo(Main.MouseWorld) * speed;
                    Projectile.netUpdate = true;

                    player.immune = true;
                    player.immuneTime = 3;
                    Projectile.Center = player.Center;
                }
            }

            Projectile.velocity *= 0.97f;



            Vector2 oldMouseWorld = Main.MouseWorld;
            if (timer > 8)
            {
                Beans = true;
                if (timer < 10 && Main.myPlayer == Projectile.owner)
                {
                    player.velocity = Projectile.DirectionTo(oldMouseWorld) * 5f;
                }
            }


            
            if (timer == 25)
            {
                player.itemTime = 40;
                player.itemAnimation = 40;
            }
        }

        public override bool? CanDamage()
        {

            if (Beans)
            {
                return false;
            }

            return base.CanDamage();
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Player player = Main.player[Projectile.owner];
            Vector2 oldMouseWorld = Main.MouseWorld;
            player.GetModPlayer<SteinPlayer>().HasHitDance = true;






            if (!bounced)
            {

                player.GetModPlayer<DashPlayer>().DashCount += 3;
                player.velocity = Projectile.DirectionTo(oldMouseWorld) * -10f;
                bounced = true;



                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/SteinHulting") { Pitch = Main.rand.NextFloat(-0.5f, 0.5f) });
                switch (Main.rand.Next(3))
                {
                    case 0:
                        SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Steinhit1"), Projectile.Center);
                        break;
                    case 1:
                        SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Steinhit2"), Projectile.Center);
                        break;
                    case 2:
                        SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Steinhit3"), Projectile.Center);
                        break;

                }

                //Wow, Amazing, So Hot, SEXY, Great
                for(int i = 0; i < player.GetModPlayer<MeleeEffectsPlayer>().steinWordBonus + 1; i++)
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), player.Center, Vector2.Zero, ModContent.ProjectileType<GREAT>(),
                        (int)(Projectile.damage * 1.5), 0f, Projectile.owner, 0f, 0f);
                }

                float rot = player.velocity.ToRotation();
                float spread = 0.6f;
                Vector2 offset = new Vector2(1.5f, -0.1f * player.direction).RotatedBy(rot);
                for (int k = 0; k < 7; k++)
                {
                    Vector2 direction = offset.RotatedByRandom(spread);
                    Dust.NewDustPerfect(Projectile.position + offset * 43, ModContent.DustType<Dusts.GlowDust>(), new Vector2(0, 0), 125, new Color(255, 255, 255), 1);
                    Dust.NewDustPerfect(player.Center + offset * 43, ModContent.DustType<Dusts.TSmokeDust>(), Vector2.UnitY * -2 + offset.RotatedByRandom(spread), 150, Color.LightPink * 0.5f, Main.rand.NextFloat(0.5f, 1));
                }

                target.SimpleStrikeNPC(Projectile.damage * 2, 1, crit: false, Projectile.knockBack);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), player.position.X, player.position.Y, 0, 0, 
                    ModContent.ProjectileType<Hulthit1>(), Projectile.damage, 0f, Projectile.owner, 0f, 0f);
                for (int i = 0; i < 26; i++)
                {
                    DustParticleSpawnParams spawnParams = DustParticleSpawnParams.Default;
                    spawnParams.outerColor = Color.DarkGray;
                    spawnParams.scaleRange *= 0.5f;
                    DustParticle.Spawn(target.Center, (Vector2.One * Main.rand.Next(1, 9)).RotatedByRandom(MathHelper.TwoPi), spawnParams);
                }

                for (int i = 0; i < 12; i++)
                {
                    var sp = SparkleParticle.Spawn(target.Center + Main.rand.NextVector2CircularEdge(128, 128), Vector2.Zero);
                    Color color = new Color(Main.rand.Next(0, 255), Main.rand.Next(0, 255), Main.rand.Next(0, 255));
                    sp.innerColor = color;
                    sp.outerColor = Color.Lerp(color, Color.Black, 0.5f);
                    sp.flickering = true;
                    sp.Scale *= 0.75f;
                    sp.Velocity = (sp.Center - target.Center).SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(0.5f, 1.5f);
                    sp.gravity = 0;
                    sp.noTileCollide = true;
                }

                for (int i = 0; i < 20; i++)
                {
                    Dust.NewDustPerfect(target.Center, ModContent.DustType<TSmokeDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(MathHelper.TwoPi), 0, Color.DeepPink, 1f).noGravity = true;
                }

                target.SimpleStrikeNPC(Projectile.damage * 2, 1, crit: false, 1);
                FXUtil.ShakeCamera(Projectile.Center, 512, 16);
            }
        }

   //     public PrimDrawer TrailDrawer { get; private set; } = null;
        public float WidthFunction(float completionRatio)
        {
            return 124 * MathHelper.SmoothStep(1f, 0f, Timer / (float)SwingTime);
        }
        public Color ColorFunction(float completionRatio)
        {
            float inRatio = completionRatio / 0.3f;
            inRatio = EasingFunction.InOutSine(inRatio);
            float outRatio = (1f - completionRatio) / 0.3f;
            outRatio = EasingFunction.InOutSine(outRatio);
            return Color.White * inRatio * outRatio;
        }

        private void DrawPixelatedTrails(GraphicsDevice gDevice)
        {
            BlackFireShader blackFireShader = BlackFireShader.Instance;
            Vector2[] array = new Vector2[64];
            for(int i = 0; i < array.Length; i++)
            {
                float ratio = (float)i / (float)array.Length;
                ref Vector2 point = ref array[i];
                point = Vector2.Lerp(_originalPosition, Projectile.Center, ratio);
            }
            blackFireShader.InnerColor = Color.White;
            blackFireShader.OuterColor = Color.LightGray;
            blackFireShader.BackColor = Color.DarkGray;
            blackFireShader.PrimaryTexture2 = TrailRegistry.LightningTrail;
            TrailDrawer.Draw(Main.spriteBatch, array, ColorFunction, WidthFunction, blackFireShader);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            PixelationManager.QueuePrimitivesDrawAction(DrawPixelatedTrails);
            return false;

        }

        public override void PostDraw(Color lightColor)
        {
            Player player = Main.player[Projectile.owner];
            Texture2D texture = (Texture2D)ModContent.Request<Texture2D>(Texture);

            int frameHeight = texture.Height / Main.projFrames[Projectile.type];
            int startY = frameHeight * Projectile.frame;

            float mult = Lerp(Utils.GetLerpValue(0f, SwingTime, Projectile.timeLeft));
            float alpha = (float)Math.Sin(mult * Math.PI);
            Vector2 pos = player.Center + Projectile.velocity * (mult);

            Rectangle sourceRectangle = new Rectangle(0, startY, texture.Width, frameHeight);
            Vector2 origin = sourceRectangle.Size() / 2f;
            Color drawColor = Projectile.GetAlpha(lightColor);

            Main.EntitySpriteDraw(texture,
                Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY),
                sourceRectangle, drawColor, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);

            float rotation = Projectile.rotation;


            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            Main.instance.LoadProjectile(Projectile.type);


            // Redraw the projectile with the color not influenced by light
            Vector2 Dorigin = sourceRectangle.Size() / 2f;
            Vector2 drawOrigin = new Vector2(texture.Width * 0.5f, Projectile.height * 0.5f);
            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = (Projectile.oldPos[k] - Main.screenPosition) + Dorigin + new Vector2(0f, Projectile.gfxOffY);
                Color color = Projectile.GetAlpha(Color.Lerp(new Color(93, 203, 243), new Color(59, 72, 168), 1f / Projectile.oldPos.Length * k) * (1f - 1f / Projectile.oldPos.Length * k / 0.2f));
                Main.EntitySpriteDraw(texture, drawPos, null, color, rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            }
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            return;
        }
    }
}