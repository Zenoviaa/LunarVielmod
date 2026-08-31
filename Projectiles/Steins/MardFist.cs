using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Content.Dusts;
using Stellamod.Items.Weapons.Mage.Stein;
using Stellamod.Projectiles.IgniterExplosions.Stein;
using Stellamod.Trails;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Stellamod.Content.Dusts;
namespace Stellamod.Projectiles.Steins
{
    public class MardFist : ModProjectile
    {
        public static bool swung = false;
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
        public override void AI()
        {
            Vector3 RGB = new Vector3(1.45f, 2.55f, 0.94f);
            float multiplier = 1;
            float max = 2.25f;
            float min = 1.0f;
            RGB *= multiplier;
            if (RGB.X > max)
            {
                multiplier = 0.5f;
            }
            if (RGB.X < min)
            {
                multiplier = 1.5f;
            }
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10000;
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

                    player.velocity = Projectile.DirectionTo(oldMouseWorld) * 10f;
                }


            }


            if (timer > 25)
            {
                if (!bounced)
                {
                    player.itemTime = 185;
                    player.itemAnimation = 185;
                }
                if (bounced)
                {
                    player.itemTime = 50;
                    player.itemAnimation = 50;
                }
                Projectile.Kill();
            }



            //Projectile.netUpdate = true;
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
                player.velocity = Projectile.DirectionTo(oldMouseWorld) * -15f;
                bounced = true;

                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/SteinShading") { Pitch = Main.rand.NextFloat(-0.5f, 0.5f) });
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
                switch (Main.rand.Next(3))
                {
                    case 0:
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), player.position.X, player.position.Y, 0, 0, ModContent.ProjectileType<AMAZING>(), (int)(Projectile.damage * 1.5), 0f, Projectile.owner, 0f, 0f);
                        break;
                    case 1:
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), player.position.X, player.position.Y, 0, 0, ModContent.ProjectileType<GREAT>(), (int)(Projectile.damage * 1), 0f, Projectile.owner, 0f, 0f);
                        break;
                    case 2:
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), player.position.X, player.position.Y, 0, 0, ModContent.ProjectileType<WOW>(), (int)(Projectile.damage * 2), 0f, Projectile.owner, 0f, 0f);
                        break;
                }
                float rot = player.velocity.ToRotation();
                float spread = 0.6f;

                Vector2 offset = new Vector2(1.5f, -0.1f * player.direction).RotatedBy(rot);
                for (int k = 0; k < 7; k++)
                {
                    Vector2 direction = offset.RotatedByRandom(spread);
                    Dust.NewDustPerfect(Projectile.position + offset * 43, ModContent.DustType<GlowDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.Purple, 1f).noGravity = true;
                    Dust.NewDustPerfect(player.Center + offset * 43, ModContent.DustType<TSmokeDust>(), Vector2.UnitY * -2 + offset.RotatedByRandom(spread), 0, Color.Purple, 1f).noGravity = true;

                }




                switch (Main.rand.Next(4))
                {
                    case 0:
                        target.SimpleStrikeNPC(Projectile.damage * 5, 1, crit: false, Projectile.knockBack);
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), player.position.X, player.position.Y, 0, 0, ModContent.ProjectileType<Shit1>(), (int)(Projectile.damage * 1.5), 0f, Projectile.owner, 0f, 0f);


                        for (int i = 0; i < 6; i++)
                        {
                            Dust.NewDustPerfect(target.Center, ModContent.DustType<GlowDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.Purple, 1f).noGravity = true;
                        }
                        for (int i = 0; i < 14; i++)
                        {
                            Dust.NewDustPerfect(target.Center, ModContent.DustType<TSmokeDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.Purple, 1f).noGravity = true;
                        }


                        break;
                    case 1:

                        target.SimpleStrikeNPC(Projectile.damage * 10, 1, crit: false, Projectile.knockBack);
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), player.position.X, player.position.Y, 0, 0, ModContent.ProjectileType<Shit2>(), (int)(Projectile.damage * 1.5), 0f, Projectile.owner, 0f, 0f);
                        for (int i = 0; i < 26; i++)
                        {
                            Dust.NewDustPerfect(target.Center, ModContent.DustType<GlowDust>(), (Vector2.One * Main.rand.Next(1, 9)).RotatedByRandom(MathHelper.TwoPi), 0, Color.Purple, 1f).noGravity = true;
                        }
                        for (int i = 0; i < 20; i++)
                        {
                            Dust.NewDustPerfect(target.Center, ModContent.DustType<TSmokeDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(MathHelper.TwoPi), 0, Color.Purple, 1f).noGravity = true;
                        }
                        break;
                    case 2:
                        target.SimpleStrikeNPC(Projectile.damage * 15, 1, crit: false, Projectile.knockBack);
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), player.position.X, player.position.Y, 0, 0, ModContent.ProjectileType<Shit3>(), (int)(Projectile.damage * 1.5), 0f, Projectile.owner, 0f, 0f);
                        for (int i = 0; i < 15; i++)
                        {
                            Dust.NewDustPerfect(target.Center, ModContent.DustType<GlowDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.Purple, 1f).noGravity = true;
                        }
                        for (int i = 0; i < 14; i++)
                        {
                            Dust.NewDustPerfect(target.Center, ModContent.DustType<TSmokeDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.Purple, 1f).noGravity = true;
                        }
                        break;

                    case 3:
                        target.SimpleStrikeNPC(Projectile.damage * 20, 1, crit: false, Projectile.knockBack);
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), player.position.X, player.position.Y, 0, 0, ModContent.ProjectileType<Shit4>(), (int)(Projectile.damage * 1.5), 0f, Projectile.owner, 0f, 0f);
                        for (int i = 0; i < 35; i++)
                        {
                            Dust.NewDustPerfect(target.Center, ModContent.DustType<GlowDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.Purple, 1f).noGravity = true;
                        }
                        for (int i = 0; i < 14; i++)
                        {
                            Dust.NewDustPerfect(target.Center, ModContent.DustType<TSmokeDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.Purple, 1f).noGravity = true;
                        }

                        for (int i = 0; i < 4; i++)
                        {
                           // Dust.NewDustPerfect(target.Center, ModContent.DustType<LumiDust>(), (Vector2.One * Main.rand.Next(1, 3)).RotatedByRandom(19.0), 170, Color.Purple, 1f).noGravity = true;
                        }
                        for (int i = 0; i < 4; i++)
                        {
                            Dust.NewDustPerfect(target.Center, ModContent.DustType<TSmokeDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.Black, 0.5f).noGravity = true;
                        }
                        break;
                }

                target.SimpleStrikeNPC(Projectile.damage * 5, 1, crit: false, 1);
                Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(base.Projectile.Center, 512f, 32f);


                if (target.lifeMax <= 1000)
                {
                    if (target.life < target.lifeMax / 2)
                    {
                        target.SimpleStrikeNPC(99999, 1, crit: false, 1);
                    }
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {


            Texture2D texture = (Texture2D)ModContent.Request<Texture2D>(Texture);

            int frameHeight = texture.Height / Main.projFrames[Projectile.type];
            int startY = frameHeight * Projectile.frame;

            Rectangle sourceRectangle = new Rectangle(0, startY, texture.Width, frameHeight);
            Vector2 origin = sourceRectangle.Size() / 2f;
            Color drawColor = Projectile.GetAlpha(lightColor);


            Main.EntitySpriteDraw(texture,
               Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY),
               sourceRectangle, drawColor, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0); // drawing the sword itself

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