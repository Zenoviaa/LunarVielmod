using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Buffs.Minions;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Projectiles.Ammo;
using Stellamod.Trails;
using System;
using System.Threading;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Summons.Minions
{
    public class ReflectionSeekerMinionProj : ModProjectile
    {
        private static RenderTarget2D playerRT;
        public static Player[] playerVisualClone = new Player[256];

        public override void Load()
        {
            if(!Main.dedServ)
            {
                using var eventSlim = new ManualResetEventSlim();
                Main.QueueMainThreadAction(() =>
                {
                    if (playerRT != null && !playerRT.IsDisposed)
                        playerRT.Dispose();
                    playerRT = new RenderTarget2D(Main.graphics.GraphicsDevice, Main.screenWidth, Main.screenHeight);
                    eventSlim.Set();
                });

                eventSlim.Wait();
            }
        }

        public override void Unload()
        {
            /*
            if (Main.netMode != NetmodeID.Server)
            {
                Main.graphics.GraphicsDevice.DeviceReset -= OnDeviceReset;

                if (playerRT is not null)
                {
                    using var eventSlim = new ManualResetEventSlim();

                    Main.QueueMainThreadAction(() =>
                    {
                        playerRT.Dispose();
                        eventSlim.Set();
                    });

                    eventSlim.Wait();

                    playerRT = null;
                }
            }*/
        }

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 15;
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
            ProjectileID.Sets.CultistIsResistantTo[Type] = true;
            ProjectileID.Sets.MinionSacrificable[Type] = true;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 50;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;

        }

        public override void SetDefaults()
        {
            Projectile.width = 26;
            Projectile.height = 42;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.netImportant = true;
            Projectile.minion = true;
            Projectile.minionSlots = 1f;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 12;
            Projectile.decidesManualFallThrough = true;
        }

        private bool AI_067_CustomEliminationCheck_Pirates(Entity otherEntity, int currentTarget)
        {
            return true;
        }

        int Grenns = 0;
        public override void AI()
        {
            Grenns++;
            Player player = Main.player[Projectile.owner];
            if (!SummonHelper.CheckMinionActive<ReflectionSeekerMinionBuff>(player, Projectile))
                return;


            if (!player.active)
            {
                Projectile.active = false;
                return;
            }
            int num = 850;
            float num12 = 1000f; //distance before flight With wings
            float num21 = 600f;
            float num13 = 2000f; //distance before flight Without wings
            float num22 = 1200f;
            if (player.dead)
            {
                player.GetModPlayer<MyPlayer>().ReflectionS = false;
            }
            if (player.GetModPlayer<MyPlayer>().ReflectionS)
            {

            }
            num = 800;
            Vector2 vector = player.Center;
            vector.X -= (15 + player.width / 2) * player.direction;
            vector.X -= (player.direction == 1 ? 0 : -36) + Projectile.minionPos * (player.direction == 1 ? 20 : -20);
            bool flag14 = true;
            Projectile.shouldFallThrough = player.position.Y + player.height - 12f > Projectile.position.Y + Projectile.height;
            Projectile.friendly = false;
            int num51 = 0;
            int num52 = 15;
            int attackTarget = -1;
            bool flag15 = true;
            bool flag2 = Projectile.ai[0] == 5f;
            bool flag3 = Projectile.ai[0] == 0f;
            if (flag3 && flag14)
            {
                Projectile.Minion_FindTargetInRange(num, ref attackTarget, skipIfCannotHitWithOwnBody: true, AI_067_CustomEliminationCheck_Pirates);
            }
            float playerDistance;
            float myDistance;
            bool closerIsMe;
            if (Projectile.ai[0] == 1f)
            {
                Projectile.ai[2] = player.wingTimeMax > 0 ? 0f : 1f;
                Projectile.tileCollide = false;
                float num9 = 0.2f;
                float num10 = 10f;
                int num11 = 200;
                if (num10 < Math.Abs(player.velocity.X) + Math.Abs(player.velocity.Y))
                {
                    num10 = Math.Abs(player.velocity.X) + Math.Abs(player.velocity.Y);
                }
                Vector2 vector10 = player.Center - Projectile.Center;
                float num55 = vector10.Length();
                if (num55 > 2000f)
                {
                    Projectile.position = player.Center - new Vector2(Projectile.width, Projectile.height) / 2f;
                }
                if (num55 < num11 && player.velocity.Y == 0f && Projectile.position.Y + Projectile.height <= player.position.Y + player.height && !Collision.SolidCollision(Projectile.position, Projectile.width, Projectile.height))
                {
                    Projectile.ai[0] = 0f;
                    Projectile.netUpdate = true;
                    if (Projectile.velocity.Y < -6f)
                    {
                        Projectile.velocity.Y = -6f;
                    }
                }
                if (!(num55 < 60f))
                {
                    vector10.Normalize();
                    vector10 *= num10;
                    if (Projectile.velocity.X < vector10.X)
                    {
                        Projectile.velocity.X += num9;
                        if (Projectile.velocity.X < 0f)
                        {
                            Projectile.velocity.X += num9 * 1.5f;
                        }
                    }
                    if (Projectile.velocity.X > vector10.X)
                    {
                        Projectile.velocity.X -= num9;
                        if (Projectile.velocity.X > 0f)
                        {
                            Projectile.velocity.X -= num9 * 1.5f;
                        }
                    }
                    if (Projectile.velocity.Y < vector10.Y)
                    {
                        Projectile.velocity.Y += num9;
                        if (Projectile.velocity.Y < 0f)
                        {
                            Projectile.velocity.Y += num9 * 1.5f;
                        }
                    }
                    if (Projectile.velocity.Y > vector10.Y)
                    {
                        Projectile.velocity.Y -= num9;
                        if (Projectile.velocity.Y > 0f)
                        {
                            Projectile.velocity.Y -= num9 * 1.5f;
                        }
                    }
                }
                if (Projectile.velocity.X != 0f)
                {
                    Projectile.spriteDirection = Math.Sign(Projectile.velocity.X);
                }
                Projectile.frameCounter++;
                if (Projectile.frameCounter > 3)
                {
                    Projectile.frame++;
                    Projectile.frameCounter = 0;
                }
                if (Projectile.frame < 10 | Projectile.frame > 13)
                {
                    Projectile.frame = 10;
                }
                if (player.wingTimeMax > 0)
                {
                    Projectile.rotation = Projectile.velocity.X * 0.1f;
                }
            }
            else
            {
                Projectile.ai[2] = 0f;
            }
            if (Projectile.ai[0] == 2f && Projectile.ai[1] < 0f)
            {
                Projectile.friendly = false;
                Projectile.ai[1] += 1f;
                if (num52 >= 0)
                {
                    Projectile.ai[1] = 0f;
                    Projectile.ai[0] = 0f;
                    Projectile.netUpdate = true;
                    return;
                }
            }
            else if (Projectile.ai[0] == 2f)
            {
                Projectile.spriteDirection = Projectile.direction;
                Projectile.rotation = 0f;
                Projectile.friendly = true;
                Projectile.frame = 4 + (int)(num52 - Projectile.ai[1]) / (num52 / 3);
                if (Projectile.velocity.Y != 0f)
                {
                    Projectile.frame += 3;//When attack flying
                }
                Projectile.velocity.Y += 0.4f;
                if (Projectile.velocity.Y > 10f)
                {
                    Projectile.velocity.Y = 10f;
                }
                Projectile.ai[1] -= 1f;
                if (Projectile.ai[1] <= 0f)
                {
                    if (num51 <= 0)
                    {
                        Projectile.ai[1] = 0f;
                        Projectile.ai[0] = 0f;
                        Projectile.netUpdate = true;
                        return;
                    }
                    Projectile.ai[1] = -num51;
                }
            }
            if (attackTarget >= 0)
            {
                float maxDistance2 = num;
                float num17 = 20f;
                NPC nPC2 = Main.npc[attackTarget];
                Vector2 center = nPC2.Center;
                vector = center;
                if (Projectile.IsInRangeOfMeOrMyOwner(nPC2, maxDistance2, out myDistance, out playerDistance, out closerIsMe))
                {
                    Projectile.shouldFallThrough = nPC2.Center.Y > Projectile.Bottom.Y;
                    bool flag4 = Projectile.velocity.Y == 0f;
                    if (Projectile.wet && Projectile.velocity.Y > 0f && !Projectile.shouldFallThrough)
                    {
                        flag4 = true;
                    }
                    if (center.Y < Projectile.Center.Y - 30f && flag4)
                    {
                        float num58 = (center.Y - Projectile.Center.Y) * -1f;
                        float num18 = 0.4f;
                        float num19 = (float)Math.Sqrt(num58 * 2f * num18);
                        if (num19 > 26f)
                        {
                            num19 = 26f;
                        }
                        Projectile.velocity.Y = 0f - num19;
                    }
                    if (flag15 && Vector2.Distance(Projectile.Center, vector) < num17)
                    {
                        if (Projectile.velocity.Length() > 10f)
                        {
                            Projectile.velocity /= Projectile.velocity.Length() / 10f;
                        }
                        Projectile.ai[0] = 2f;
                        Projectile.ai[1] = num52;
                        Projectile.netUpdate = true;
                        Projectile.direction = center.X - Projectile.Center.X > 0f ? 1 : -1;
                    }
                }
            }

            Player owner = Main.player[Projectile.owner];


            SummonHelper.SearchForTargets(owner, Projectile, out bool foundTarget, out float distanceFromTarget, out Vector2 targetCenter);
            Vector2 targetOffset = new Vector2(0, -64);
            if (foundTarget)
            {

                Grenns++;
                if (Grenns > 50)
                {
                    Vector2 bulletVelocity = Projectile.Center.DirectionTo(targetCenter) * 52;
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, bulletVelocity,
                        ModContent.ProjectileType<EldritchBolt>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                    SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Laserlock") { Pitch = Main.rand.NextFloat(-5f, 5f) }, Projectile.Center);




                    for (int i = 0; i < 26; i++)
                    {
                        Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowDust>(), (Vector2.One * Main.rand.Next(1, 3)).RotatedByRandom(MathHelper.TwoPi), 0, Color.Pink, 1f).noGravity = true;
                    }


                    Grenns = 0;


                }
            }






            if (Projectile.ai[0] == 0f && attackTarget < 0)
            {
                if (player.rocketDelay2 > 0 && player.wingTimeMax > 0) //Wingtime because obv the minion shouldnt fly with withs when the player doesnt have wings
                {
                    Projectile.ai[0] = 1f;
                    Projectile.netUpdate = true;
                }
                Vector2 vector11 = player.Center - Projectile.Center;
                if (vector11.Length() > 2000f)
                {
                    Projectile.position = player.Center - new Vector2(Projectile.width, Projectile.height) / 2f;
                }
                else if ((vector11.Length() > num12 || Math.Abs(vector11.Y) > num21) && player.wingTimeMax > 0) //Wingtime because obv the minion shouldnt fly with withs when the player doesnt have wings
                {
                    Projectile.ai[0] = 1f;//The initial trigger for when the summons are too far away
                    Projectile.netUpdate = true;
                    if (Projectile.velocity.Y > 0f && vector11.Y < 0f)
                    {
                        Projectile.velocity.Y = 0f;
                    }
                    if (Projectile.velocity.Y < 0f && vector11.Y > 0f)
                    {
                        Projectile.velocity.Y = 0f;
                    }
                }
                else if (vector11.Length() > num13 || Math.Abs(vector11.Y) > num22) //Wingtime because obv the minion shouldnt fly with withs when the player doesnt have wings
                {
                    Projectile.ai[0] = 1f;//The initial trigger for when the summons are too far away
                    Projectile.netUpdate = true;
                    if (Projectile.velocity.Y > 0f && vector11.Y < 0f)
                    {
                        Projectile.velocity.Y = 0f;
                    }
                    if (Projectile.velocity.Y < 0f && vector11.Y > 0f)
                    {
                        Projectile.velocity.Y = 0f;
                    }
                }
            }
            if (Projectile.ai[0] == 0f)
            {
                if (attackTarget < 0)
                {
                    if (Projectile.Distance(player.Center) > 60f && Projectile.Distance(vector) > 60f && Math.Sign(vector.X - player.Center.X) != Math.Sign(Projectile.Center.X - player.Center.X))
                    {
                        vector = player.Center; //+ new Vector2((player.direction == 1 ? 0 : -200), 0);
                    }
                    Rectangle r = Utils.CenteredRectangle(vector, Projectile.Size);
                    for (int i = 0; i < 20; i++)
                    {
                        if (Collision.SolidCollision(r.TopLeft(), r.Width, r.Height))
                        {
                            break;
                        }
                        r.Y += 16;
                        vector.Y += 16f;
                    }
                    Vector2 vector12 = Collision.TileCollision(player.Center - Projectile.Size / 2f, vector - player.Center, Projectile.width, Projectile.height);
                    vector = player.Center - Projectile.Size / 2f + vector12;
                    if (Projectile.Distance(vector) < 32f)
                    {
                        float num24 = player.Center.Distance(vector);
                        if (player.Center.Distance(Projectile.Center) < num24)
                        {
                            vector = Projectile.Center; //The projectile is in rest, not moving to the player, not attacking an enemy
                        }
                    }
                    Vector2 vector13 = player.Center - vector;
                    if (vector13.Length() > num12 || Math.Abs(vector13.Y) > num21)
                    {
                        Rectangle r2 = Utils.CenteredRectangle(player.Center, Projectile.Size);
                        Vector2 vector2 = vector - player.Center;
                        Vector2 vector3 = r2.TopLeft();
                        for (float num25 = 0f; num25 < 1f; num25 += 0.05f)
                        {
                            Vector2 vector4 = r2.TopLeft() + vector2 * num25;
                            if (Collision.SolidCollision(r2.TopLeft() + vector2 * num25, r.Width, r.Height))
                            {
                                break;
                            }
                            vector3 = vector4;
                        }
                        vector = vector3 + Projectile.Size / 2f;
                    }
                }
                Projectile.tileCollide = true;
                float num26 = 0.5f;
                float num27 = 4f;
                float num28 = 4f;
                float num29 = 0.1f;
                if (attackTarget != -1)
                {
                    num26 = 1f;
                    num27 = 8f;
                    num28 = 8f;
                }
                if (num28 < Math.Abs(player.velocity.X) + Math.Abs(player.velocity.Y))
                {
                    num28 = Math.Abs(player.velocity.X) + Math.Abs(player.velocity.Y);
                    num26 = 0.7f;
                }
                int num31 = 0;
                bool flag5 = false;
                float num33 = vector.X - Projectile.Center.X;
                Vector2 vector5 = vector - Projectile.Center;
                if (Math.Abs(num33) > 5f)
                {
                    if (num33 < 0f)
                    {
                        num31 = -1;
                        if (Projectile.velocity.X > 0f - num27)
                        {
                            Projectile.velocity.X -= num26;
                        }
                        else
                        {
                            Projectile.velocity.X -= num29;
                        }
                    }
                    else
                    {
                        num31 = 1;
                        if (Projectile.velocity.X < num27)
                        {
                            Projectile.velocity.X += num26;
                        }
                        else
                        {
                            Projectile.velocity.X += num29;
                        }
                    }
                    bool flag6 = false;
                    if (flag6)
                    {
                        flag5 = true;
                    }
                }
                else
                {
                    Projectile.velocity.X *= 0.9f;
                    if (Math.Abs(Projectile.velocity.X) < num26 * 2f)
                    {
                        Projectile.velocity.X = 0f;
                    }
                }
                bool flag7 = Math.Abs(vector5.X) >= 64f || vector5.Y <= -48f && Math.Abs(vector5.X) >= 8f;
                if (num31 != 0 && flag7)
                {
                    int num34 = (int)(Projectile.position.X + Projectile.width / 2) / 16;
                    int num35 = (int)Projectile.position.Y / 16;
                    num34 += num31;
                    num34 += (int)Projectile.velocity.X;
                    for (int j = num35; j < num35 + Projectile.height / 16 + 1; j++)
                    {
                        if (WorldGen.SolidTile(num34, j))
                        {
                            flag5 = true;
                        }
                    }
                }
                Collision.StepUp(ref Projectile.position, ref Projectile.velocity, Projectile.width, Projectile.height, ref Projectile.stepSpeed, ref Projectile.gfxOffY);
                float num36 = Utils.GetLerpValue(0f, 100f, vector5.Y, clamped: true) * Utils.GetLerpValue(-2f, -6f, Projectile.velocity.Y, clamped: true);
                if (Projectile.velocity.Y == 0f)
                {
                    if (flag5)
                    {
                        for (int k = 0; k < 3; k++)
                        {
                            int num37 = (int)(Projectile.position.X + Projectile.width / 2) / 16;
                            if (k == 0)
                            {
                                num37 = (int)Projectile.position.X / 16;
                            }
                            if (k == 2)
                            {
                                num37 = (int)(Projectile.position.X + Projectile.width) / 16;
                            }
                            int num38 = (int)(Projectile.position.Y + Projectile.height) / 16;
                            if (!WorldGen.SolidTile(num37, num38) && !Main.tile[num37, num38].IsHalfBlock && Main.tile[num37, num38].Slope <= 0 && (!TileID.Sets.Platforms[Main.tile[num37, num38].TileType] || !Main.tile[num37, num38].HasTile || Main.tile[num37, num38].IsActuated))
                            {
                                continue;
                            }
                            try
                            {
                                num37 = (int)(Projectile.position.X + Projectile.width / 2) / 16;
                                num38 = (int)(Projectile.position.Y + Projectile.height / 2) / 16;
                                num37 += num31;
                                num37 += (int)Projectile.velocity.X;
                                if (!WorldGen.SolidTile(num37, num38 - 1) && !WorldGen.SolidTile(num37, num38 - 2))
                                {
                                    Projectile.velocity.Y = -5.1f;
                                }
                                else if (!WorldGen.SolidTile(num37, num38 - 2))
                                {
                                    Projectile.velocity.Y = -7.1f;
                                }
                                else if (WorldGen.SolidTile(num37, num38 - 5))
                                {
                                    Projectile.velocity.Y = -11.1f;
                                }
                                else if (WorldGen.SolidTile(num37, num38 - 4))
                                {
                                    Projectile.velocity.Y = -10.1f;
                                }
                                else
                                {
                                    Projectile.velocity.Y = -9.1f;
                                }
                            }
                            catch
                            {
                                Projectile.velocity.Y = -9.1f;
                            }
                        }
                        if (vector.Y - Projectile.Center.Y < -48f)
                        {
                            float num39 = vector.Y - Projectile.Center.Y;
                            num39 *= -1f;
                            if (num39 < 60f)
                            {
                                Projectile.velocity.Y = -6f;
                            }
                            else if (num39 < 80f)
                            {
                                Projectile.velocity.Y = -7f;
                            }
                            else if (num39 < 100f)
                            {
                                Projectile.velocity.Y = -8f;
                            }
                            else if (num39 < 120f)
                            {
                                Projectile.velocity.Y = -9f;
                            }
                            else if (num39 < 140f)
                            {
                                Projectile.velocity.Y = -10f;
                            }
                            else if (num39 < 160f)
                            {
                                Projectile.velocity.Y = -11f;
                            }
                            else if (num39 < 190f)
                            {
                                Projectile.velocity.Y = -12f;
                            }
                            else if (num39 < 210f)
                            {
                                Projectile.velocity.Y = -13f;
                            }
                            else if (num39 < 270f)
                            {
                                Projectile.velocity.Y = -14f;
                            }
                            else if (num39 < 310f)
                            {
                                Projectile.velocity.Y = -15f;
                            }
                            else
                            {
                                Projectile.velocity.Y = -16f;
                            }
                        }
                        if (Projectile.wet && num36 == 0f)
                        {
                            Projectile.velocity.Y *= 2f;
                        }
                    }
                }
                if (Projectile.velocity.X > num28)
                {
                    Projectile.velocity.X = num28;
                }
                if (Projectile.velocity.X < 0f - num28)
                {
                    Projectile.velocity.X = 0f - num28;
                }
                if (Projectile.velocity.X < 0f)
                {
                    Projectile.direction = -1;
                }
                if (Projectile.velocity.X > 0f)
                {
                    Projectile.direction = 1;
                }
                if (Projectile.velocity.X == 0f)
                {
                    Projectile.direction = player.Center.X > Projectile.Center.X ? 1 : -1;
                }
                if (Projectile.velocity.X > num26 && num31 == 1)
                {
                    Projectile.direction = 1;
                }
                if (Projectile.velocity.X < 0f - num26 && num31 == -1)
                {
                    Projectile.direction = -1;
                }
                Projectile.spriteDirection = Projectile.direction;

                //Frame shit?
                Projectile.rotation = 0f;
                if (Projectile.velocity.Y == 0f)
                {
                    if (Projectile.velocity.X == 0f)
                    {
                        Projectile.frame = 0;
                        Projectile.frameCounter = 0;
                    }
                    else if (Math.Abs(Projectile.velocity.X) >= 0.5f)
                    {
                        Projectile.frameCounter += (int)Math.Abs(Projectile.velocity.X);
                        Projectile.frameCounter++;
                        if (Projectile.frameCounter > 10)
                        {
                            Projectile.frame++;
                            Projectile.frameCounter = 0;
                        }
                        if (Projectile.frame >= 4)
                        {
                            Projectile.frame = 0;
                        }
                    }
                    else
                    {
                        Projectile.frame = 0;
                        Projectile.frameCounter = 0;
                    }
                }
                else if (Projectile.velocity.Y != 0f)
                {
                    Projectile.frameCounter = 0;
                    Projectile.frame = 14; //Falling
                }
                //end of Frame shit?
                Projectile.velocity.Y += 0.4f + num36 * 1f;
                if (Projectile.velocity.Y > 10f)
                {
                    Projectile.velocity.Y = 10f;
                }
            }
            if (Projectile.ai[2] == 1f)
            {
                Projectile.rotation += 0.3f * Projectile.direction;

                for (int x = 2; x < 30; x++)
                {
                    for (int y = 2; y < 30; y++)
                    {
                        if (Main.rand.NextBool(400))
                        {
                            int dust = Dust.NewDust(Projectile.position + new Vector2(x, y), 0, 0, ModContent.DustType<TSmokeDust>());
                            Main.dust[dust].shader = GameShaders.Armor.GetSecondaryShader(player.cMinion, player);
                        }
                    }
                }
            }
        }



        public TrailRenderer SwordSlash;
        public TrailRenderer SwordSlash2;
        public TrailRenderer SwordSlash3;

        public override bool PreDraw(ref Color lightColor)
        {


            var TrailTex = ModContent.Request<Texture2D>("Stellamod/Effects/Primitives/Trails/StarTrail").Value;
            var TrailTex2 = ModContent.Request<Texture2D>("Stellamod/Effects/Primitives/Trails/StringTrail").Value;
            var TrailTex3 = ModContent.Request<Texture2D>("Stellamod/Effects/Primitives/Trails/CrystalTrail").Value;
            Color color = Color.Multiply(new(1.50f, 1.75f, 3.5f, 0), 200);
            if (SwordSlash == null)
            {
                SwordSlash = new TrailRenderer(TrailTex, TrailRenderer.DefaultPass,
                    (p) => Vector2.Lerp(new Vector2(150), new Vector2(100), p),
                    (p) => new Color(230, 255, 255, 125) * (1f - p));
                SwordSlash.drawOffset = Projectile.Size / 2f;
            }
            if (SwordSlash2 == null)
            {
                SwordSlash2 = new TrailRenderer(TrailTex2, TrailRenderer.DefaultPass,
                    (p) => Vector2.Lerp(new Vector2(75), new Vector2(55), p),
                    (p) => new Color(247, 178, 239, 125) * (1f - p));
                SwordSlash2.drawOffset = Projectile.Size / 2f;
            }
            if (SwordSlash3 == null)
            {
                SwordSlash3 = new TrailRenderer(TrailTex3, TrailRenderer.DefaultPass,
                    (p) => Vector2.Lerp(new Vector2(30), new Vector2(30), p),
                    (p) => new Color(69, 93, 149, 125) * (1f - p));
                SwordSlash3.drawOffset = Projectile.Size / 2f;
            }


            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Texture, null, null, null, null, null, Main.GameViewMatrix.ZoomMatrix);


            SwordSlash.Draw(Projectile.oldPos, Projectile.oldRot);
            SwordSlash2.Draw(Projectile.oldPos, Projectile.oldRot);
            SwordSlash3.Draw(Projectile.oldPos, Projectile.oldRot);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin();


            Main.spriteBatch.End();

            int projectileAmount = 0;
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                if (Main.projectile[i].type == Type && Main.projectile[i].active)
                {
                    projectileAmount++;
                }
            }

            var graphicsDevice = Main.instance.GraphicsDevice;
            var sb = Main.spriteBatch;

            graphicsDevice.SetRenderTarget(Main.screenTargetSwap);
            graphicsDevice.Clear(Color.Transparent);
            sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
            sb.Draw(Main.screenTarget, Vector2.Zero, Color.White);
            sb.End();

            graphicsDevice.SetRenderTarget(playerRT);
            graphicsDevice.Clear(Color.Transparent);
            sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);
            try
            {
                int owner = Projectile.owner;
                Player other = Main.player[owner];
                if (playerVisualClone[owner] == null)
                {
                    playerVisualClone[owner] = new Player();
                }
                Player player = playerVisualClone[owner];
                player.CopyVisuals(other);
                player.ResetEffects();
                player.ResetVisibleAccessories();
                player.DisplayDollUpdate();
                player.UpdateSocialShadow();
                player.Center = Projectile.Center;
                player.direction = Projectile.direction;
                player.velocity = Projectile.velocity / projectileAmount;
                player.wingFrame = Projectile.frame - 10;
                player.PlayerFrame();
                player.socialIgnoreLight = true;

                Main.pixelShader.CurrentTechnique.Passes[0].Apply();
                if (Projectile.ai[2] == 0f)
                {
                    using (BaseRenderTargetOverrider.OverrideSpecific(playerRT, Main.screenTarget))
                        Main.PlayerRenderer.DrawPlayer(Main.Camera, player, Projectile.position, 0f, player.fullRotationOrigin);
                }
                else
                {
                    Texture2D texture = Main.Assets.Request<Texture2D>("Images/UI/PanelBackground").Value;
                    Rectangle rect = new(0, 0, texture.Width, texture.Width);
                    Vector2 drawOrigin = new Vector2(texture.Width * 0.5f, texture.Height * 0.5f);
                    var effects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
                    sb.Draw(texture, Projectile.Center - Main.screenPosition, rect, Projectile.GetAlpha(lightColor), Projectile.rotation, drawOrigin, Projectile.scale, effects, 0f);
                }
            }
            catch (Exception e)
            {
                TimeLogger.DrawException(e);
                Projectile.active = false;
            }
            sb.End();
            graphicsDevice.SetRenderTarget(Main.screenTarget);
            graphicsDevice.Clear(Color.Transparent);

            sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
            sb.Draw(Main.screenTargetSwap, Vector2.Zero, Color.White);
            sb.End();

            sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);

            var drawData = new DrawData(playerRT, Vector2.Zero, Color.White);
            var playerOwner = Main.player[Projectile.owner];
            GameShaders.Misc["Stellamod:SilhouetteShader"].Apply(drawData);
            if (playerOwner.cMinion != 0)
            {
                GameShaders.Armor.ApplySecondary(playerOwner.cMinion, Projectile, drawData);
            }
            drawData.Draw(Main.spriteBatch);

            var visualPlayer = playerVisualClone[Projectile.owner];
            var eyesNotVisible = playerOwner.head > 0 && !ArmorIDs.Head.Sets.DrawHead[playerOwner.head] && Projectile.ai[2] != 0f;
            if (!eyesNotVisible)
            {
                var headVect = new Vector2(visualPlayer.legFrame.Width * 0.5f, visualPlayer.legFrame.Height * 0.4f);
                var position = new Vector2(
                    (int)(visualPlayer.position.X - Main.screenPosition.X - visualPlayer.bodyFrame.Width / 2 + visualPlayer.width / 2) - (visualPlayer.direction == 1 ? 1 : 5),
                    (int)(visualPlayer.position.Y - Main.screenPosition.Y + visualPlayer.height - visualPlayer.bodyFrame.Height + 4f)
                ) + visualPlayer.headPosition + headVect;

                sb.Draw(TextureAssets.Players[visualPlayer.skinVariant, 1].Value,
                    position,
                    visualPlayer.bodyFrame,
                    Color.White,
                    visualPlayer.headRotation,
                    headVect,
                    Projectile.scale,
                    Projectile.direction < 0 ? SpriteEffects.FlipHorizontally : 0,
                    0f);
            }

            sb.End();

            sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);


            DrawHelper.DrawAdditiveAfterImage(Projectile, new Color(85, 112, 188) * 0.4f, Color.Transparent, ref lightColor);
            return false;
        }




        public override void ModifyDamageHitbox(ref Rectangle hitbox)
        {
            if (Projectile.ai[2] == 1f)
            {
                hitbox = new(0, 0, 0, 0);
            }
            else
            {
                base.ModifyDamageHitbox(ref hitbox);
            }
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            fallThrough = false;
            return true;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return false;
        }
    }
}