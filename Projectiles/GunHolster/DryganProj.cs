using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Dusts;
using Stellamod.Projectiles.IgniterExplosions;
using Stellamod.Trails;
using Stellamod.UI.Systems;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.GunHolster
{
    internal class DryganProj : ModProjectile
    {
        NPC target;
        int afterImgCancelDrawCount = 0;

        Vector2 endPoint;
        Vector2 controlPoint1;
        Vector2 controlPoint2;
        Vector2 initialPos;
        Vector2 wantedEndPoint;
        bool initialization = false;
        float AoERadiusSquared = 36000;//it's squared for less expensive calculations
        public bool[] hitByThisStardustExplosion = new bool[200] { false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, };
        float t = 0;

        public static Vector2 CubicBezier(Vector2 start, Vector2 controlPoint1, Vector2 controlPoint2, Vector2 end, float t)
        {
            float tSquared = t * t;
            float tCubed = t * t * t;
            return
                -(start * (-tCubed + (3 * tSquared) - (3 * t) - 1) +
                controlPoint1 * ((3 * tCubed) - (6 * tSquared) + (3 * t)) +
                controlPoint2 * ((-3 * tCubed) + (3 * tSquared)) +
                end * tCubed);
        }

        public override void AI()
        {

            int dustnumber = Dust.NewDust(Projectile.Center, Projectile.width, Projectile.height, DustID.GoldCoin, 0f, 0f, 150, Color.White, 1f);
            Main.dust[dustnumber].velocity *= 0f;
            Main.dust[dustnumber].noGravity = true;
            Main.dust[dustnumber].noLight = false;
            Main.dust[dustnumber].scale = 0.5f;



            if (!initialization)
            {
                initialPos = Projectile.Center;
                endPoint = Projectile.Center;
            }
            float distanceSQ = float.MaxValue;
            if (target == null || !target.active)
                for (int i = 0; i < Main.npc.Length; i++)
                {
                    if ((target == null || Main.npc[i].DistanceSQ(Projectile.Center) < distanceSQ) && Main.npc[i].active && !Main.npc[i].friendly && !Main.npc[i].dontTakeDamage && Main.npc[i].type != NPCID.CultistBossClone)
                    {
                        target = Main.npc[i];
                        distanceSQ = Projectile.Center.DistanceSQ(target.Center);
                    }
                }
            if (target != null && target.DistanceSQ(Projectile.Center) < 10000000 && target.active && !hitByThisStardustExplosion[target.whoAmI])
            {
                wantedEndPoint = initialPos - (target.Center - initialPos);
                if (Projectile.ai[0] < 10)
                {
                    endPoint = wantedEndPoint;
                }
            }
            if (!initialization)
            {
                controlPoint1 = Projectile.Center + Main.rand.NextVector2CircularEdge(1000, 1000);
                controlPoint2 = endPoint + Main.rand.NextVector2CircularEdge(1000, 1000);
                //controlPoint2 = Vector2.Lerp(endPoint, initialPos, 0.33f) + Main.player[Projectile.owner].velocity * 70;
                //if (target != null)
                //    controlPoint1 = Vector2.Lerp(endPoint, initialPos, 0.66f) + target.velocity * 70;
                //else
                //    Projectile.Kill();
                initialization = true;
            }
            Projectile.velocity = Vector2.Zero;
            Projectile.rotation = (Projectile.Center - CubicBezier(initialPos, controlPoint1, controlPoint2, endPoint, t + 0.025f)).ToRotation() - MathHelper.PiOver2;
            endPoint = endPoint.MoveTowards(wantedEndPoint, 16);
            if (t > 1)
            {
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC npc = Main.npc[i];
                    if (npc.Center.DistanceSQ(Projectile.Center) < AoERadiusSquared && !npc.dontTakeDamage && !hitByThisStardustExplosion[npc.whoAmI])
                    {
                        hitByThisStardustExplosion[npc.whoAmI] = true;
                        NPC.HitInfo hitInfo = new();
                        hitInfo.Damage = Projectile.damage;
                        //(int)Main.player[Projectile.owner].GetDamage(DamageClass.Summon).ApplyTo(Projectile.damage)
                        hitInfo.DamageType = DamageClass.Melee;
                        npc.StrikeNPC(hitInfo);
                    }
                }
                afterImgCancelDrawCount++;
            }
            else if (target != null)
            {
                Projectile.Center = CubicBezier(initialPos, controlPoint1, controlPoint2, endPoint, t);
            }
            if (target == null || Projectile.ai[0] > 200)
                Projectile.Kill();

            t += 0.01f;

            Projectile.ai[0]++;

        }
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 100;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
            //DisplayName.SetDefault("Stardust bolt");
            //DisplayName.AddTranslation(8, "Tiro de Pó Estelar");
        }
        public override void SetDefaults()
        {
            Projectile.penetrate = 3;
            Projectile.usesIDStaticNPCImmunity = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 120;
            Projectile.Size = new Vector2(30, 30);
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 200;
            Projectile.extraUpdates = 10;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }

        public PrimDrawer TrailDrawer { get; private set; } = null;
        public float WidthFunction(float completionRatio)
        {
            float baseWidth = Projectile.scale * Projectile.width * 1.0f;
            return MathHelper.SmoothStep(baseWidth, 0.35f, completionRatio);
        }

        public Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(Color.LightYellow, Color.Orange, completionRatio) * 0.2f;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, new Vector2(texture.Width / 2, texture.Height / 2), 1f, Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
            TrailDrawer ??= new PrimDrawer(WidthFunction, ColorFunction, GameShaders.Misc["VampKnives:BasicTrail"]);
            GameShaders.Misc["VampKnives:BasicTrail"].SetShaderTexture(TrailRegistry.FadedStreak);
            TrailDrawer.DrawPrims(Projectile.oldPos, Projectile.Size * 0.5f - Main.screenPosition, 155);
            return false;
        }




        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            ShakeModSystem.Shake = 4;
            for (int i = 0; i < 5; i++)
            {
                Dust.NewDustPerfect(base.Projectile.Center, ModContent.DustType<GlowDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.LightGoldenrodYellow, 1f).noGravity = true;
            }
            for (int i = 0; i < 2; i++)
            {
                Dust.NewDustPerfect(base.Projectile.Center, ModContent.DustType<TSmokeDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.DarkGray, Main.rand.NextFloat(0.3f, 0.4f)).noGravity = true;
            }
            SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode, Projectile.position);
            Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(Projectile.Center, 1024f, 16f);




            SoundStyle soundStyle = new SoundStyle("Stellamod/Assets/Sounds/ExplosionBurstBomb");
            soundStyle.PitchVariance = 0.5f;
            SoundEngine.PlaySound(soundStyle, Projectile.position);

            Vector2 velocity2 = Projectile.velocity;
            velocity2 = -velocity2;
            velocity2 = velocity2.RotatedByRandom(MathHelper.PiOver4 + MathHelper.PiOver2);
            velocity2 *= Main.rand.NextFloat(0.5f, 1f);
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity2 * 0,
                ModContent.ProjectileType<PiranhaBoomMini>(), Projectile.damage / 10, Projectile.knockBack, Projectile.owner);


        }

    }



}

