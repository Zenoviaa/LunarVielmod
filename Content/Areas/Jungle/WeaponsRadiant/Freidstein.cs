using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Content.CommonMaterials;
using Stellamod.Content.Dusts;
using Stellamod.Core.Bases;
using Stellamod.Core.Pixelation;
using Stellamod.Core.SwingSystem;
using Stellamod.Core.Utilities;
using Stellamod.Helpers;
using Stellamod.Items;
using Stellamod.Items.Accessories.Players;
using Stellamod.Items.Weapons.Mage.Stein;
using Stellamod.Projectiles.IgniterExplosions.Stein;
using Stellamod.Projectiles.Steins;
using Stellamod.Visual.Particles;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Jungle.WeaponsRadiant
{
    public class Friedstein : BaseSwingItemV2
    {
        public override void SetDefaults2()
        {
            base.SetDefaults2();
            Item.damage = 96;
            Item.useTime = 6;
            Item.useAnimation = 6;
            Item.shoot = ModContent.ProjectileType<FriedsteinBarrage>();
            staminaProjectileShoot = ModContent.ProjectileType<FriedFist>();
            meleeWeaponType = MeleeWeaponType.Stein;
            staminaDamageMultiplier = 2;
            staminaCost = 3;
        }


        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew(
                mold: ModContent.ItemType<BlankStein>(),
                material: ModContent.ItemType<RadiantNectar>());
        }

    }

    public class FriedsteinBarrage : ModProjectile
    {
        private Vector2 _start;
        private Vector2 _end;
        private ref float Timer => ref Projectile.ai[0];
        private Player Owner => Main.player[Projectile.owner];
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            ProjectileID.Sets.TrailCacheLength[Type] = 8;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 64;
            Projectile.height = 64;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 12;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            base.SendExtraAI(writer);
            writer.WriteVector2(_start);
            writer.WriteVector2(_end);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            base.ReceiveExtraAI(reader);
            _start = reader.ReadVector2();
            _end = reader.ReadVector2();
        }
        public override bool ShouldUpdatePosition()
        {
            return false;
        }

        private Vector2 CalculateSwingPoint(float time)
        {
            float ratio = time / 12f;
            float ease = EasingFunction.QuadraticBump(ratio);
            Vector2 pos = Vector2.Lerp(_start, _end, ease);
            return pos;
        }
        public override void AI()
        {
            base.AI();
            //   ProjectileID.Sets.TrailCacheLength[Type] = 8;
            Timer++;
            if (Timer == 1)
            {
                if (this.OwnedByLocalClient())
                {
                    _start = Owner.Center + Main.rand.NextVector2Circular(45, 45);
                    _end = _start + Projectile.velocity.SafeNormalize(Vector2.Zero) * 128;
                    Projectile.netUpdate = true;
                }
                for(int i = 0; i < 2; i++)
                {
                    var sp = SparkleParticle.Spawn(Owner.Center, Projectile.velocity.SafeNormalize(Vector2.Zero) * 32, Scale: 0.66f);
                    sp.Velocity = sp.Velocity.RotatedByRandom(MathHelper.ToRadians(8));
                    sp.Velocity *= Main.rand.NextFloat(0.5f, 1f);
                    sp.outerColor = Color.Goldenrod;
                    sp.fast = true;
                    sp.noTileCollide = true;
                    sp.gravity = 0;
                    sp.dampening = 0.2f;
                }
            }
            if (Timer == 2)
            {

                SoundStyle sounds = new SoundStyle("Stellamod/Assets/Sounds/AssassinsKnifeProg");
                sounds.PitchVariance = 0.3f;
                SoundEngine.PlaySound(sounds, Projectile.position);
                ThrustParticle ts = ThrustParticle.Spawn(Projectile.Center, Projectile.velocity);
                ts.bloomColor = Color.Gold;
                ts.Scale *= 0.5f;
            }

            if (Timer % 8 == 0)
            {
                var ts = ThickSmokeParticle.Spawn(Projectile.Center, Vector2.Zero);
                ts.expand = true;
                ts.color *= 0.5f;
                ts.Scale *= 0.2f;
            }
            Projectile.Center = CalculateSwingPoint(Timer);
            Projectile.rotation = Projectile.velocity.ToRotation();
        }

        public override bool PreDraw(ref Color lightColor)
        {
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                float ratio = (float)(i + 1) / (float)Projectile.oldPos.Length;
                SpritebatchDrawer fadeDrawer = SpritebatchDrawer.FromProjectile(Projectile);
                fadeDrawer.worldPosition = Projectile.oldPos[i] + Projectile.Size * 0.5f;
                fadeDrawer.color = Color.Lerp(Color.White, Color.Transparent, ratio) * 0.3f;
                Main.spriteBatch.Draw(fadeDrawer);
            }
            return false;

        }
    }
    public class FriedFist : ModProjectile
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
            SwingTime = 60;
            Projectile.timeLeft = SwingTime;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.height = 100;
            Projectile.width = 100;
            Projectile.friendly = true;
            Projectile.scale = 1f;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        private ref float Timer => ref Projectile.ai[0];
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if (!player.active || player.dead || player.CCed || player.noItems)
                return;

            Vector2 teleportPosition = Main.MouseWorld;
            Timer++;
            if (Timer == 1)
            {
                _originalPosition = Projectile.Center;

            }
            if (Timer == 5 && Main.myPlayer == Projectile.owner)
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

            if(Timer == 8)
            {
                for (int i = 0; i < 64; i++)
                {
                    if (!Main.rand.NextBool(3))
                        continue;
                    float ratio = Main.rand.NextFloat(0f, 1f);
                    Vector2 point = Vector2.Lerp(_originalPosition, Projectile.Center, ratio);
                    point += Main.rand.NextVector2Circular(32, 32);
                    var vfx = FXUtil.GlowStretch(point, Projectile.velocity.SafeNormalize(Vector2.Zero));
                    vfx.VectorScale *= 1f;
                    vfx.OuterGlowColor = Color.Goldenrod;

                }
            }
            Projectile.velocity *= 0.97f;
            Vector2 oldMouseWorld = Main.MouseWorld;
            if (Timer > 8)
            {
                if (Timer < 10 && Main.myPlayer == Projectile.owner)
                {
                    player.velocity = Projectile.DirectionTo(oldMouseWorld) * 13f;
                }
            }

            if(Timer < 25)
            {

                player.itemTime = 2;
                player.itemAnimation = 2;
            }
        }

        public override bool? CanDamage()
        {
            if (Timer > 8)
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
                PixelPrimitiveCircleFactory.CreateHeavenlyBoom(target.Center);
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

                player.velocity = Projectile.DirectionTo(oldMouseWorld) * -17f;
                bounced = true;
                switch (Main.rand.Next(3))
                {
                    case 0:
                        SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/GoldenSlice1") { Pitch = Main.rand.NextFloat(-0.5f, 0.5f) });
                        SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Steinhit1"), Projectile.Center);
                        break;
                    case 1:
                        SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/GoldenSlice2") { Pitch = Main.rand.NextFloat(-0.5f, 0.5f) });
                        SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Steinhit2"), Projectile.Center);
                        break;
                    case 2:
                        SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/GoldenSlice3") { Pitch = Main.rand.NextFloat(-0.5f, 0.5f) });
                        SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Steinhit3"), Projectile.Center);
                        break;

                }

                //Wow, Amazing, So Hot, SEXY, Great
                switch (Main.rand.Next(7))
                {
                    case 0:
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), player.position.X, player.position.Y, 0, 0, ModContent.ProjectileType<GREAT>(), 
                            (int)(Projectile.damage * 1), 0f, Projectile.owner, 0f, 0f);
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), player.position.X, player.position.Y, 0, 0, ModContent.ProjectileType<AMAZING>(),
                            (int)(Projectile.damage * 1.5), 0f, Projectile.owner, 0f, 0f);
                        break;
                    case 1:
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), player.position.X, player.position.Y, 0, 0, ModContent.ProjectileType<GREAT>(), 
                            (int)(Projectile.damage * 1), 0f, Projectile.owner, 0f, 0f);
                        break;
                    case 2:
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), player.position.X, player.position.Y, 0, 0, ModContent.ProjectileType<AMAZING>(), 
                            (int)(Projectile.damage * 1), 0f, Projectile.owner, 0f, 0f);
                        break;
                    case 3:
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), player.position.X, player.position.Y, 0, 0, ModContent.ProjectileType<SOHOT>(), 
                            (int)(Projectile.damage * 1), 0f, Projectile.owner, 0f, 0f);
                        break;
                    case 4:
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), player.position.X, player.position.Y, 0, 0, ModContent.ProjectileType<WOW>(), 
                            (int)(Projectile.damage * 1), 0f, Projectile.owner, 0f, 0f);
                        break;
                    case 5:
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), player.position.X, player.position.Y, 0, 0, ModContent.ProjectileType<WOW>(), 
                            (int)(Projectile.damage * 1), 0f, Projectile.owner, 0f, 0f);
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), player.position.X, player.position.Y, 0, 0, ModContent.ProjectileType<SOHOT>(),
                            (int)(Projectile.damage * 1), 0f, Projectile.owner, 0f, 0f);
                        break;

                    case 6:
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), player.position.X, player.position.Y, 0, 0, ModContent.ProjectileType<WOW>(), 
                            (int)(Projectile.damage * 1), 0f, Projectile.owner, 0f, 0f);
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), player.position.X, player.position.Y, 0, 0, ModContent.ProjectileType<GREAT>(), 
                            (int)(Projectile.damage * 1), 0f, Projectile.owner, 0f, 0f);
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), player.position.X, player.position.Y, 0, 0, ModContent.ProjectileType<AMAZING>(), 
                            (int)(Projectile.damage * 1), 0f, Projectile.owner, 0f, 0f);
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), player.position.X, player.position.Y, 0, 0, ModContent.ProjectileType<SOHOT>(), 
                            (int)(Projectile.damage * 2), 0f, Projectile.owner, 0f, 0f);
                        break;


                }

                float rot = player.velocity.ToRotation();
                float spread = 0.6f;
                Vector2 offset = new Vector2(1.5f, -0.1f * player.direction).RotatedBy(rot);
                for (int k = 0; k < 7; k++)
                {
                    Vector2 direction = offset.RotatedByRandom(spread);
                    Dust.NewDustPerfect(Projectile.position + offset * 43, ModContent.DustType<Dusts.GlowDust>(), new Vector2(0, 0), 125, new Color(150, 180, 40), 1);
                    Dust.NewDustPerfect(player.Center + offset * 43, ModContent.DustType<Dusts.TSmokeDust>(), Vector2.UnitY * -2 + offset.RotatedByRandom(spread), 150, Color.SpringGreen * 0.5f, Main.rand.NextFloat(0.5f, 1));
                }


                switch (Main.rand.Next(3))
                {
                    case 0:
                        target.SimpleStrikeNPC(Projectile.damage , 1, crit: false, Projectile.knockBack);
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), player.position.X, player.position.Y, 0, 0, ModContent.ProjectileType<Freidhit1>(), (int)(Projectile.damage * 1.5), 0f, Projectile.owner, 0f, 0f);


                        for (int i = 0; i < 13; i++)
                        {
                            Dust.NewDustPerfect(target.Center, ModContent.DustType<GlowDust>(), (Vector2.One * Main.rand.Next(1, 9)).RotatedByRandom(MathHelper.TwoPi), 0, Color.SpringGreen, 1f).noGravity = true;
                        }
                        for (int i = 0; i < 10; i++)
                        {
                            Dust.NewDustPerfect(target.Center, ModContent.DustType<TSmokeDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(MathHelper.TwoPi), 0, Color.LightGoldenrodYellow, 1f).noGravity = true;
                        }



                        break;
                    case 1:

                        target.SimpleStrikeNPC(Projectile.damage, 1, crit: false, Projectile.knockBack);
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), player.position.X, player.position.Y, 0, 0, ModContent.ProjectileType<Freidhit2>(), (int)(Projectile.damage * 1.5), 0f, Projectile.owner, 0f, 0f);
                        for (int i = 0; i < 23; i++)
                        {
                            Dust.NewDustPerfect(target.Center, ModContent.DustType<GlowDust>(), (Vector2.One * Main.rand.Next(1, 9)).RotatedByRandom(MathHelper.TwoPi), 0, Color.SpringGreen, 1f).noGravity = true;
                        }
                        for (int i = 0; i < 10; i++)
                        {
                            Dust.NewDustPerfect(target.Center, ModContent.DustType<TSmokeDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(MathHelper.TwoPi), 0, Color.LightGoldenrodYellow, 1f).noGravity = true;
                        }
                        break;
                    case 2:
                        target.SimpleStrikeNPC(Projectile.damage, 1, crit: false, Projectile.knockBack);
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), player.position.X, player.position.Y, 0, 0, ModContent.ProjectileType<Freidhit3>(), (int)(Projectile.damage * 1.5), 0f, Projectile.owner, 0f, 0f);
                        for (int i = 0; i < 32; i++)
                        {
                            Dust.NewDustPerfect(target.Center, ModContent.DustType<GlowDust>(), (Vector2.One * Main.rand.Next(1, 9)).RotatedByRandom(MathHelper.TwoPi), 0, Color.SpringGreen, 1f).noGravity = true;
                        }
                        for (int i = 0; i < 10; i++)
                        {
                            Dust.NewDustPerfect(target.Center, ModContent.DustType<TSmokeDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(MathHelper.TwoPi), 0, Color.LightGoldenrodYellow, 1f).noGravity = true;
                        }
                        break;

                }

                target.SimpleStrikeNPC(Projectile.damage * 5, 1, crit: false, 1);
                Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(base.Projectile.Center, 512f, 32f);
            }
        }
        public float WidthFunction(float completionRatio)
        {
            return 124 * MathHelper.SmoothStep(1f, 0f, Timer / (float)SwingTime);
        }
        public float WidthFunction2(float completionRatio)
        {
            return WidthFunction(completionRatio) * 1.5f;
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
            for (int i = 0; i < array.Length; i++)
            {
                float ratio = (float)i / (float)array.Length;
                ref Vector2 point = ref array[i];
                point = Vector2.Lerp(_originalPosition, Projectile.Center, ratio);
            }
            blackFireShader.InnerColor = Color.White;
            blackFireShader.OuterColor = Color.Goldenrod;
            blackFireShader.BackColor = Color.DarkGoldenrod;
            blackFireShader.InnerEmitColor = Color.Gold;
            blackFireShader.OuterEmiteColor = Color.DarkGoldenrod;
            blackFireShader.PrimaryTexture2 = TrailRegistry.LightningTrail;
            TrailDrawer.Draw(Main.spriteBatch, array, ColorFunction, WidthFunction, blackFireShader);
        }

        private void DrawPixelatedBloom(GraphicsDevice gDevice)
        {

            Vector2[] array = new Vector2[64];
            for (int i = 0; i < array.Length; i++)
            {
                float ratio = (float)i / (float)array.Length;
                ref Vector2 point = ref array[i];
                point = Vector2.Lerp(_originalPosition, Projectile.Center, ratio);
            }
            BloomTrailShader bloomTrail = BloomTrailShader.Instance;
            bloomTrail.InnerColor = Color.Goldenrod;
            bloomTrail.OuterColor = Color.DarkGoldenrod;
            TrailDrawer.Draw(Main.spriteBatch, array, ColorFunction, WidthFunction2, bloomTrail);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            PixelationManager.QueuePrimitivesDrawAction(DrawPixelatedTrails);
            PixelationManager.QueuePrimitivesDrawAction(DrawPixelatedBloom);
            return false;

        }

        public override void PostDraw(Color lightColor)
        {

        }
    }
}