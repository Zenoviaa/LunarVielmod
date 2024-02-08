using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Projectiles.IgniterExplosions;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;
using ReLogic.Content;
using Stellamod.Trails;
using Terraria.Graphics.Shaders;
using ParticleLibrary;
using Stellamod.Particles;

namespace Stellamod.Projectiles.Swords.Fenix
{

    internal class AngelenthalThrow : ModProjectile
    {
        bool Moved;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Shadow Hand");
           // Main.projFrames[Projectile.type] = 30;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 35;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.penetrate = 5;
            Projectile.width = 79;
            Projectile.height = 79;
            Projectile.timeLeft = 660;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
        }
        public PrimDrawer TrailDrawer { get; private set; } = null;
        public float WidthFunction(float completionRatio)
        {
            float baseWidth = Projectile.scale * Projectile.width * 1.3f;
            return MathHelper.SmoothStep(baseWidth, 3.5f, completionRatio);
        }
        public Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(Color.White, Color.Transparent, completionRatio) * 0.7f;
        }
        public override bool PreDraw(ref Color lightColor)
        {

            TrailDrawer ??= new PrimDrawer(WidthFunction, ColorFunction, GameShaders.Misc["VampKnives:BasicTrail"]);
            GameShaders.Misc["VampKnives:BasicTrail"].SetShaderTexture(TrailRegistry.SmallWhispyTrail);
            TrailDrawer.DrawPrims(Projectile.oldPos, Projectile.Size * 0.5f - Main.screenPosition, 155);
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;

            if (Projectile.spriteDirection != 1)
            {
                DrawOffset.X = Projectile.Center.X - 18;
                DrawOffset.Y = Projectile.Center.Y;
            }
            else
            {
                DrawOffset.X = Projectile.Center.X - 25;
                DrawOffset.Y = Projectile.Center.Y;
            }

            Texture2D texture2D4 = Request<Texture2D>("Stellamod/Effects/Masks/Spiin").Value;
            Main.spriteBatch.Draw(texture2D4, DrawOffset - Main.screenPosition, null, new Color((int)(85f * alphaCounter), (int)(65f * alphaCounter), (int)(85f * alphaCounter), 0), Projectile.rotation, new Vector2(200, 200), 0.07f * (5 + 0.6f), SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(texture2D4, DrawOffset - Main.screenPosition, null, new Color((int)(85f * alphaCounter), (int)(65f * alphaCounter), (int)(85f * alphaCounter), 0), Projectile.rotation, new Vector2(200, 200), 0.07f * (5 + 0.6f), SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(texture2D4, DrawOffset - Main.screenPosition, null, new Color((int)(85f * alphaCounter), (int)(65f * alphaCounter), (int)(85f * alphaCounter), 0), Projectile.rotation, new Vector2(200, 200), 0.07f * (5 + 0.6f), SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(texture2D4, DrawOffset - Main.screenPosition, null, new Color((int)(85f * alphaCounter), (int)(65f * alphaCounter), (int)(85f * alphaCounter), 0), Projectile.rotation, new Vector2(200, 200), 0.07f * (5 + 0.6f), SpriteEffects.None, 0f);
            SpriteEffects Effects = Projectile.spriteDirection != 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            Vector2 scale = new(Projectile.scale, 1f);
            Color drawColor = Color.Purple;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;

            for (int i = 0; i < 8; i++)
            {
                Vector2 drawOffset = (MathHelper.TwoPi * i / 8f).ToRotationVector2() * 4f;
                Main.EntitySpriteDraw(texture, DrawOffset - Main.screenPosition + drawOffset, null, Color.Yellow with { A = 160 } * Projectile.Opacity, Projectile.rotation, texture.Size() * 0.5f, scale, Effects, 0);
            }
            for (int i = 0; i < 7; i++)
            {
                float scaleFactor = 1f - i / 6f;
                Vector2 drawOffset = Projectile.velocity * i * -0.34f;
                Main.EntitySpriteDraw(texture, DrawOffset - Main.screenPosition + drawOffset, null, drawColor with { A = 160 } * Projectile.Opacity, Projectile.rotation, texture.Size() * 0.5f, scale * scaleFactor, Effects, 0);
            }
            Main.EntitySpriteDraw(texture, DrawOffset - Main.screenPosition, null, drawColor with { A = 250 }, Projectile.rotation, texture.Size() * 0.5f, scale, Effects, 0);

            if (Main.rand.NextBool(5))
            {
                int dustnumber = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, 0f, 0f, 150, Color.OrangeRed, 1f);
                Main.dust[dustnumber].velocity *= 0.3f;
                Main.dust[dustnumber].noGravity = true;
            }

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            Main.instance.LoadProjectile(Projectile.type);

            // Redraw the projectile with the color not influenced by light
            Vector2 drawOrigin = new Vector2(texture.Width * 0.5f, Projectile.height * 0.5f);
            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Color color = Projectile.GetAlpha(Color.Lerp(new Color(254, 231, 97), new Color(247, 118, 34), 1f / Projectile.oldPos.Length * k) * (1f - 1f / Projectile.oldPos.Length * k));
                Main.EntitySpriteDraw(texture, drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale, Effects, 0);
            }
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            int frameHeight = texture.Height / Main.projFrames[Projectile.type];
            int startY = frameHeight * Projectile.frame;

            Rectangle sourceRectangle = new Rectangle(0, startY, texture.Width, frameHeight);
            Vector2 origin = sourceRectangle.Size() / 2f;
            Color drawColor2 = Projectile.GetAlpha(lightColor);

            Main.EntitySpriteDraw(texture,
                Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY),
                sourceRectangle, drawColor2, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);




            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            Main.instance.LoadProjectile(Projectile.type);


            // Redraw the projectile with the color not influenced by light
            Vector2 drawOrigin2 = new Vector2(texture.Width * 0.5f, Projectile.height * 0.5f);
            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = (Projectile.oldPos[k] - Main.screenPosition) + drawOrigin2 + new Vector2(0f, Projectile.gfxOffY);
                Color color = Projectile.GetAlpha(Color.Lerp(new Color(93, 203, 243), new Color(59, 72, 168), 1f / Projectile.oldPos.Length * k) * (1f - 1f / Projectile.oldPos.Length * k));
                Main.EntitySpriteDraw(texture, drawPos, null, color, Projectile.rotation, drawOrigin2, Projectile.scale, SpriteEffects.None, 0);
            }
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);


            return false;
        }

       

        public override void AI()
        {
            float distance2 = 128;
            float particleSpeed = 8;
            Vector2 position = Projectile.Center + Main.rand.NextVector2CircularEdge(distance2, distance2);
            Vector2 speed = (Projectile.Center - position).SafeNormalize(Vector2.Zero) * particleSpeed;
            Particle p = ParticleManager.NewParticle(position, speed, ParticleManager.NewInstance<ShadeParticle>(), default(Color), 0.5f);



            Projectile.velocity *= .96f;
            Projectile.ai[1]++;
            if (!Moved && Projectile.ai[1] >= 0)
            {
                SoundEngine.PlaySound(SoundID.DD2_BetsyFireballShot, Projectile.position);

                Projectile.spriteDirection = Projectile.direction;
                Projectile.rotation = Projectile.velocity.ToRotation() + 1.57f + 3.14f;
                for (int j = 0; j < 10; j++)
                {
                    Vector2 vector2 = Vector2.UnitX * -Projectile.width / 2f;
                    vector2 += -Vector2.UnitY.RotatedBy(j * 3.141591734f / 6f, default) * new Vector2(8f, 16f);
                    vector2 = vector2.RotatedBy(Projectile.rotation - 1.57079637f, default);
                    int num8 = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.GoldCoin, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f);
                    Main.dust[num8].scale = 1.3f;
                    Main.dust[num8].noGravity = true;
                    Main.dust[num8].position = Projectile.Center + vector2;
                    Main.dust[num8].velocity = Projectile.velocity * 0.1f;
                    Main.dust[num8].noLight = true;
                    Main.dust[num8].velocity = Vector2.Normalize(Projectile.Center - Projectile.velocity * 3f - Main.dust[num8].position) * 1.25f;
                }
                Moved = true;
            }

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];

                if (npc.active && !npc.friendly && !npc.boss)
                {
                    float distance = Vector2.Distance(Projectile.Center, npc.Center);
                    if (distance <= 200)
                    {
                        Vector2 direction = npc.Center - Projectile.Center;
                        direction.Normalize();
                        npc.velocity -= direction * 0.5f;
                    }
                }
            }

            Vector2 ParOffset;
            if (Projectile.ai[1] >= 60)
            {
                ParOffset.X = Projectile.Center.X - 18;
                ParOffset.Y = Projectile.Center.Y;
                if (Main.rand.NextBool(1))
                {
                    int dustnumber = Dust.NewDust(ParOffset, Projectile.width, Projectile.height, DustID.SilverCoin, 0f, 0f, 150, Color.White, 3f);
                    Main.dust[dustnumber].velocity *= 0.3f;
                    Main.dust[dustnumber].noGravity = true;
                }
                Projectile.velocity.Y += 3.2f;
            }
            if (Projectile.ai[1] >= 20)
            {
                Projectile.rotation /= 1.20f;
                alphaCounter -= 0.09f;
                Projectile.tileCollide = true;
            }
            else
            {
                if (alphaCounter <= 2)
                {
                    alphaCounter += 0.15f;
                }
                Projectile.rotation += -0.6f;
            }

            Projectile.spriteDirection = Projectile.direction;
        }

        Vector2 BombOffset;
        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Infernis1"), Projectile.position);

            SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Binding_Abyss_Rune"), Projectile.position);
            Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(base.Projectile.Center, 512f, 120f);
            var EntitySource = Projectile.GetSource_FromThis();
            int fireball = Projectile.NewProjectile(EntitySource, Projectile.Center.X, Projectile.Center.Y, 0, 0, ModContent.ProjectileType<KaBoomFenix>(), Projectile.damage * 2, 1, Projectile.owner);
            Projectile ichor = Main.projectile[fireball];
            ichor.hostile = false;
            ichor.friendly = true;

            if (Projectile.ai[1] >= 60)
            {
                BombOffset.X = Projectile.Center.X - 50;
                BombOffset.Y = Projectile.Center.Y;
                Projectile.NewProjectile(EntitySource, BombOffset.X, BombOffset.Y, 0, 0, 
                    ModContent.ProjectileType<KaBoomSigil>(), Projectile.damage, 1, Projectile.owner);

                BombOffset.X = Projectile.Center.X + 70;
                BombOffset.Y = Projectile.Center.Y;
                Projectile.NewProjectile(EntitySource, BombOffset.X, BombOffset.Y, 0, 0, 
                    ModContent.ProjectileType<KaBoomSigil>(), Projectile.damage, 1, Projectile.owner);

                BombOffset.X = Projectile.Center.X - 150;
                BombOffset.Y = Projectile.Center.Y;
                Projectile.NewProjectile(EntitySource, BombOffset.X, BombOffset.Y, 0, 0, 
                    ModContent.ProjectileType<KaBoomSigil>(), Projectile.damage, 1, Projectile.owner);

                BombOffset.X = Projectile.Center.X + 170;
                BombOffset.Y = Projectile.Center.Y;
                Projectile.NewProjectile(EntitySource, BombOffset.X, BombOffset.Y, 0, 0, 
                    ModContent.ProjectileType<KaBoomSigil>(), Projectile.damage, 1, Projectile.owner);

                BombOffset.X = Projectile.Center.X - 250;
                BombOffset.Y = Projectile.Center.Y;
                Projectile.NewProjectile(EntitySource, BombOffset.X, BombOffset.Y, 0, 0, 
                    ModContent.ProjectileType<KaBoomSigil>(), Projectile.damage, 1, Projectile.owner);

                BombOffset.X = Projectile.Center.X + 270;
                BombOffset.Y = Projectile.Center.Y;
                Projectile.NewProjectile(EntitySource, BombOffset.X, BombOffset.Y, 0, 0, 
                    ModContent.ProjectileType<KaBoomSigil>(), Projectile.damage, 1, Projectile.owner);

                Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.position, -Projectile.velocity, 
                    ProjectileID.LostSoulFriendly, Projectile.damage * 1, 0f, Projectile.owner);

                Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.position, -Projectile.velocity * 0.5f,
                    ProjectileID.LostSoulFriendly, Projectile.damage * 1, 0f, Projectile.owner);

                Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.position, -Projectile.velocity * 1.5f, 
                    ProjectileID.LostSoulFriendly, Projectile.damage * 1, 0f, Projectile.owner);
            }



            SoundEngine.PlaySound(SoundID.DD2_EtherianPortalOpen, Projectile.position);
            Projectile.ownerHitCheck = true;

            int radius = 250;

            // Damage enemies within the splash radius
            for (int i = 0; i < Main.npc.Length; i++)
            {
                NPC target = Main.npc[i];
                if (target.active && !target.friendly && Vector2.Distance(Projectile.Center, target.Center) < radius)
                {
                    int damage = Projectile.damage * 2;
                    target.SimpleStrikeNPC(damage: 12, 0);
                }
            }

            for (int i = 0; i < 150; i++)
            {
                Vector2 speed = Main.rand.NextVector2CircularEdge(4f, 4f);
                var d = Dust.NewDustPerfect(Projectile.Center, DustID.SilverFlame, speed * 11, Scale: 3f);
                ;
                d.noGravity = true;
            }



        }

        float alphaCounter = 0;
        Vector2 DrawOffset;

       


        public override void PostDraw(Color lightColor)
        {
            Lighting.AddLight(Projectile.Center, Color.White.ToVector3() * 1.75f * Main.essScale);
        }
    }
}


