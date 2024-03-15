
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Buffs;
using Stellamod.Helpers;
using Stellamod.Items.Consumables;
using Stellamod.Items.Materials;
using Stellamod.Items.Weapons.Mage;
using Stellamod.Items.Weapons.Melee;
using Stellamod.Items.Weapons.Ranged;
using Stellamod.Items.Weapons.Summon;
using Stellamod.NPCs.Bosses.DreadMire;
using Stellamod.NPCs.Bosses.DreadMire.Heart;
using Stellamod.NPCs.Bosses.singularityFragment;
using Stellamod.NPCs.Bosses.singularityFragment.Phase1;
using Stellamod.NPCs.Bosses.Verlia;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Stellamod.NPCs.Bosses.Veiizal
{

    public class Veiizal : ModNPC
    {
        private const int TELEPORT_DISTANCE = 400;
        public bool PH2 = false;
        public bool Spawned = false;
        public bool TP = false;
        public bool Lazer = false;
        public int Timer = 0;
        public int PrevAttac = 0;
        public int State = 0;
        public int MaxAttac = 0;
        public static int LazerType = 0;
        public static int SingularityOrbs = 0;
        public static Vector2 SingularityPos;
        public static Vector2 SingularityStart;
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 31;
            // DisplayName.SetDefault("Binding Abyss");
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            // This will always be true, so it's a nothing statement
            /*
            if (Main.rand.Next(1) == 0)
                target.AddBuff(Mod.Find<ModBuff>("AbyssalFlame").Type, 200);
            */

            //Use ModContent.BuffType<> instead of Mod.Find, it's faster and cleaner
            target.AddBuff(BuffType<AbyssalFlame>(), 200);
        }

        public override void SetDefaults()
        {
            NPC.scale = 1;
            NPC.width = 80;
            NPC.height = 190;
            NPC.damage = 40;
            NPC.defense = 11;
            NPC.lifeMax = 13000;
            NPC.scale = 0.9f;
            NPC.DeathSound = new SoundStyle("Stellamod/Assets/Sounds/VoidDead1") with { PitchVariance = 0.1f };
            NPC.value = 60f;
            NPC.knockBackResist = 0f;
            NPC.boss = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.npcSlots = 10f;
            //Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/VampireDance");
            NPC.HitSound = new SoundStyle("Stellamod/Assets/Sounds/VoidHit") with { PitchVariance = 0.1f };
            NPC.BossBar = GetInstance<VerliaBossBar>();
            NPC.aiStyle = 0;


            if (!Main.dedServ)
            {
                Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/VampireDance");
            }
        }
        int FrameSpeed = 10;
        int frame = 0;
        public override void FindFrame(int frameHeight)
        {
            //bool expertMode = Main.expertMode;
            //Player player = Main.player[NPC.target];
            NPC.frameCounter++;


            if (NPC.frameCounter >= FrameSpeed)
            {
                frame++;
                NPC.frameCounter = 0;
            }

            if (State == 0)
            {
                if (frame >= 4 && frame <= 6)
                {
                    frame = 0;
                }
                if (frame <= 28 && frame >= 4)
                {
                    frame = 29;
                }
                if (frame >= 31)
                {
                    frame = 0;
                }
            }
            if (State == 1)
            {
                if (frame >= 10)
                {
                    frame = 9;
                }
                if (frame == 8)
                {
                    frame = 9;
                }
                if (frame <= 4)
                {
                    frame = 6;
                }
            }
            if (State == 2)
            {

                if (frame >= 14)
                {
                    State = 0;
                    frame = 0;
                }
                if (frame <= 10)
                {
                    frame = 11;
                }
            }
            if (State == 3)
            {
                if (frame >= 19)
                {
                    frame = 18;
                }
                if (frame == 17)
                {
                    frame = 18;
                }
                if (frame <= 14)
                {
                    frame = 15;
                }
            }
            if (State == 4)
            {

                if (frame >= 22)
                {
                    frame = 20;
                }
                if (frame <= 19)
                {
                    frame = 20;
                }
            }
            if (State == 5)
            {

                if (frame >= 28)
                {
                    frame = 25;
                }
                if (frame <= 22)
                {
                    frame = 23;
                }
            }
            NPC.frame.Y = frameHeight * frame;
        }



        public void CasuallyApproachChild(float AddPosx, float AddPosy)
        {
            Player player = Main.player[NPC.target];
            NPC.velocity.Y *= 0.94f;
            NPC.velocity = Vector2.Lerp(base.NPC.velocity, VectorHelper.MovemontVelocity(base.NPC.Center, Vector2.Lerp(base.NPC.Center, new Vector2(player.Center.X + AddPosx, player.Center.Y + AddPosy), 0.025f), base.NPC.Center.Distance(player.Center) * 0.3f), 0.005f);
            NPC.netUpdate = true;
        }



        int bee = 220;
        public Vector2 LastBacklash;
        public Vector2 LastDirection;
        public int Attack;
        public int SparkCount;
        public int SparkCountMax;
        public float Spawner = 0;

        public override void AI()
        {
            Spawner++;

            PH2 = NPC.life < NPC.lifeMax * 0.4f;
            if (PH2)
            {
                MaxAttac = 7;
            }
            else
            {
                MaxAttac = 5;
            }

            var entitySource = NPC.GetSource_FromThis();
            Player player = Main.player[NPC.target];
            bool expertMode = Main.expertMode;
            if (!NPC.HasPlayerTarget)
            {
                NPC.TargetClosest(false);
                Player player1 = Main.player[NPC.target];
                NPC.ai[2] = 0;
                if (!NPC.HasPlayerTarget || NPC.Distance(player1.Center) > 3000f)
                {
                    NPC.velocity.Y -= 1f;
                    if (NPC.timeLeft > 30)
                        NPC.timeLeft = 30;
                    return;
                }
            }

            Player playerT = Main.player[NPC.target];
            int distance = (int)(NPC.Center - playerT.Center).Length();
            if (distance > 3000f || playerT.dead)
            {
                NPC.ai[2] = 5;
                Disappear();
            }


            if (NPC.ai[2] == 0)
            {
                SingularityStart = NPC.position;
                NPC.ai[2] = 1;
            }


            if (NPC.ai[2] == 1)
                switch (NPC.ai[1])
                {
                    case 0:
                        // default attack, just moves above player, waits  seconds then does a random attack
                        NPC.ai[0]++;
                        if (NPC.ai[0] > 2)
                        {
                            NPC.ai[0] = 0;
                            Attack = Main.rand.Next(1, MaxAttac);
                            NPC.ai[1] = Attack;
                        }

                        break;
                    case 1:
                        // default attack, just moves above player, waits  seconds then does a random attack
                        NPC.ai[0]++;
                        if (NPC.ai[0] > 2)
                        {
                            if (NPC.Center.X >= player.Center.X)
                            {
                                CasuallyApproachChild(300, 0);
                            }
                            else
                            {
                                CasuallyApproachChild(-300, 0);
                            }


                        }
                        if (NPC.ai[0] == 50)
                        {
                            int Sound = Main.rand.Next(0, 3);
                            if (Sound == 0)
                            {
                                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Veiizal__Uopen1"), NPC.position);
                            }
                            if (Sound == 1)
                            {
                                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Veiizal__Uopen2"), NPC.position);
                            }
                            if (Sound == 2)
                            {
                                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Veiizal__Uopen3"), NPC.position);
                            }
                            State = 1;
                        }
                        if (NPC.ai[0] == 120)
                        {
                            Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(base.NPC.Center, 1212f, 82f);
                            SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Veiizal__UShot1"), NPC.position);
                            State = 2;
                            if (NPC.Center.X >= player.Center.X)
                            {
                                NPC.velocity.X += 20;
                            }
                            else
                            {
                                NPC.velocity.X -= 20;
                            }
                            int damage = Main.expertMode ? 50 : 68;
                            if (NPC.Center.X <= player.Center.X)
                            {
                                for (int j = -1; j <= 1; j++)
                                {
                                    if (Main.netMode != NetmodeID.MultiplayerClient)
                                        Projectile.NewProjectile(entitySource, new Vector2(NPC.Center.X + 40, NPC.Center.Y + 10), new Vector2(0, 0), ModContent.ProjectileType<DreadSpawnEffect>(), damage, 0f);
                                    if (Main.netMode != NetmodeID.MultiplayerClient)
                                        Projectile.NewProjectile(entitySource, new Vector2(NPC.Center.X + 40, NPC.Center.Y + 10), new Vector2(30, 0), ModContent.ProjectileType<VeiizalBeam>(), damage, 0f);
                                }
                            }
                            else
                            {
                                for (int j = -1; j <= 1; j++)
                                {
                                    if (Main.netMode != NetmodeID.MultiplayerClient)
                                        Projectile.NewProjectile(entitySource, new Vector2(NPC.Center.X - 40, NPC.Center.Y + 10), new Vector2(0, 0), ModContent.ProjectileType<DreadSpawnEffect>(), damage, 0f);
                                    if (Main.netMode != NetmodeID.MultiplayerClient)
                                        Projectile.NewProjectile(entitySource, new Vector2(NPC.Center.X - 40, NPC.Center.Y + 10), new Vector2(-30, 0), ModContent.ProjectileType<VeiizalBeam>(), damage, 0f);
                                }
                            }

                        }
                        if (NPC.ai[0] >= 130)
                        {
                            NPC.ai[1] = 0;
                            NPC.ai[0] = 0;
                        }

                        break;

                    case 2:
                        // default attack, just moves above player, waits  seconds then does a random attack
                        NPC.ai[0]++;
                        if (NPC.ai[0] > 2 && NPC.ai[0] < 120)
                        {
                            if (NPC.Center.X >= player.Center.X)
                            {
                                CasuallyApproachChild(300, 0);
                            }
                            else
                            {
                                CasuallyApproachChild(-300, 0);
                            }


                        }
                        if (NPC.ai[0] > 120)
                        {
                            if (NPC.Center.X >= player.Center.X)
                            {
                                CasuallyApproachChild(600, 0);
                            }
                            else
                            {
                                CasuallyApproachChild(-600, 0);
                            }


                        }
                        if (NPC.ai[0] == 50)
                        {
                            int Sound = Main.rand.Next(0, 3);
                            if (Sound == 0)
                            {
                                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Veiizal__Uopen1"), NPC.position);
                            }
                            if (Sound == 1)
                            {
                                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Veiizal__Uopen2"), NPC.position);
                            }
                            if (Sound == 2)
                            {
                                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Veiizal__Uopen3"), NPC.position);
                            }
                            State = 3;
                        }
                        if (NPC.ai[0] == 120)
                        {
                            Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(base.NPC.Center, 1212f, 122f);
                            if (NPC.Center.X >= player.Center.X)
                            {
                                NPC.velocity.X -= 100;
                            }
                            else
                            {
                                NPC.velocity.X += 100;
                            }

                        }
                        if (NPC.ai[0] >= 180)
                        {
                            State = 0;
                            NPC.ai[1] = 0;
                            NPC.ai[0] = 0;
                        }

                        break;
                    case 3:
                        // default attack, just moves above player, waits  seconds then does a random attack
                        NPC.ai[0]++;
                        if (NPC.ai[0] > 2 && NPC.ai[0] < 80)
                        {
                            if (NPC.Center.X >= player.Center.X)
                            {
                                CasuallyApproachChild(300, 0);
                            }
                            else
                            {
                                CasuallyApproachChild(-300, 0);
                            }


                        }
                        if (NPC.ai[0] > 80)
                        {
                            if (NPC.Center.X >= player.Center.X)
                            {
                                CasuallyApproachChild(100, 0);
                            }
                            else
                            {
                                CasuallyApproachChild(-100, 0);
                            }
                            if (NPC.ai[0] % 7 == 0)
                            {
                                Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(base.NPC.Center, 1212f, 18f);
                                int Sound = Main.rand.Next(1, 3);
                                if (Sound == 1)
                                {
                                    SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Veiizal__MG1"), NPC.position);
                                }
                                if (Sound == 2)
                                {
                                    SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Veiizal__MG2"), NPC.position);
                                }
                                if (NPC.Center.X >= player.Center.X)
                                {
                                    NPC.velocity.X += 5;
                                }
                                else
                                {
                                    NPC.velocity.X -= 5;
                                }
                                Vector2 direction = Main.player[NPC.target].Center - NPC.Center;
                                direction.Normalize();
                                int damage = Main.expertMode ? 40 : 58;


                                if (NPC.Center.X <= player.Center.X)
                                {
                                    for (int j = -1; j <= 1; j++)
                                    {
                                        if (Main.netMode != NetmodeID.MultiplayerClient)
                                            Projectile.NewProjectile(entitySource, new Vector2(NPC.Center.X + 40, NPC.Center.Y + 10), new Vector2(0, 0), ModContent.ProjectileType<DreadSpawnEffect>(), damage, 0f);
                                        if (Main.netMode != NetmodeID.MultiplayerClient)
                                            Projectile.NewProjectile(entitySource, new Vector2(NPC.Center.X + 40, NPC.Center.Y + 10), new Vector2(30, 0), ModContent.ProjectileType<VeiizalBullet>(), damage, 0f);
                                    }
                                }
                                else
                                {
                                    for (int j = -1; j <= 1; j++)
                                    {
                                        if (Main.netMode != NetmodeID.MultiplayerClient)
                                            Projectile.NewProjectile(entitySource, new Vector2(NPC.Center.X - 40, NPC.Center.Y + 10), new Vector2(0, 0), ModContent.ProjectileType<DreadSpawnEffect>(), damage, 0f);
                                        if (Main.netMode != NetmodeID.MultiplayerClient)
                                            Projectile.NewProjectile(entitySource, new Vector2(NPC.Center.X - 40, NPC.Center.Y + 10), new Vector2(-30, 0), ModContent.ProjectileType<VeiizalBullet>(), damage, 0f);
                                    }
                                }

                            }

                        }
                        if (NPC.ai[0] == 50)
                        {
                            int Sound = Main.rand.Next(0, 3);
                            if (Sound == 0)
                            {
                                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Veiizal__Uopen1"), NPC.position);
                            }
                            if (Sound == 1)
                            {
                                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Veiizal__Uopen2"), NPC.position);
                            }
                            if (Sound == 2)
                            {
                                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Veiizal__Uopen3"), NPC.position);
                            }
                            SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Veiizal__MG3"), NPC.position);
                            State = 3;
                        }
                        if (NPC.ai[0] == 80)
                        {
                            State = 4;

                            FrameSpeed = 3;
                        }
                        if (NPC.ai[0] >= 220)
                        {
                            FrameSpeed = 10;
                            State = 0;
                            NPC.ai[1] = 0;
                            NPC.ai[0] = 0;
                        }

                        break;
                    case 4:
                        // default attack, just moves above player, waits  seconds then does a random attack
                        NPC.velocity.X *= 1.03f;
                        NPC.velocity.Y *= 0.99f;
                        NPC.ai[0]++;                   
                        if (NPC.ai[0] == 20)
                        {
                            int Sound = Main.rand.Next(1, 3);
                            if (Sound == 1)
                            {
                                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Veiizal__USpawn1"), NPC.position);
                            }
                            if (Sound == 2)
                            {
                                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Veiizal__USpawn2"), NPC.position);
                            }
                            NPC.NewNPC(entitySource, (int)player.Center.X + Main.rand.Next(-50, 50), (int)NPC.Center.Y, ModContent.NPCType<Zapwarn>());
                            NPC.NewNPC(entitySource, (int)player.Center.X + Main.rand.Next(-600, 600), (int)NPC.Center.Y, ModContent.NPCType<Zapwarn>());
                            NPC.NewNPC(entitySource, (int)player.Center.X + Main.rand.Next(-600, 600), (int)NPC.Center.Y, ModContent.NPCType<Zapwarn>());
                            NPC.NewNPC(entitySource, (int)player.Center.X + Main.rand.Next(-600, 600), (int)NPC.Center.Y, ModContent.NPCType<Zapwarn>());
                            NPC.NewNPC(entitySource, (int)player.Center.X + Main.rand.Next(-600, 600), (int)NPC.Center.Y, ModContent.NPCType<Zapwarn>());
                            NPC.NewNPC(entitySource, (int)player.Center.X + Main.rand.Next(-600, 600), (int)NPC.Center.Y, ModContent.NPCType<Zapwarn>());
                            State = 5;
                        }
                        if (NPC.ai[0] == 130)
                        {
                            State = 0;
                            NPC.ai[1] = 0;
                            NPC.ai[0] = 0;
                        }
    

                        break;
                }
        }
        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0)
            {
                for (int j = 0; j < 50; j++)
                {

                }
            }
            for (int k = 0; k < 7; k++)
            {

            }
        }
        public override void OnKill()
        {
            NPC.SetEventFlagCleared(ref DownedBossSystem.downedSOMBoss, -1);

        }

        public void Movement(Vector2 Player2, float PosX, float PosY, float Speed)
        {
            Player player = Main.player[NPC.target];
            Vector2 target = Player2 + new Vector2(PosX, PosY);
            NPC.velocity = Vector2.Lerp(NPC.velocity, VectorHelper.MovemontVelocity(NPC.Center, Vector2.Lerp(NPC.Center, target, 0.5f), NPC.Center.Distance(target) * Speed), 0.1f);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
        {
            Vector2 center = NPC.Center + new Vector2(0f, NPC.height * -0.1f);
            Lighting.AddLight(NPC.Center, Color.LightBlue.ToVector3() * 1.25f * Main.essScale);
            // This creates a randomly rotated vector of length 1, which gets it's components multiplied by the parameters
            Vector2 direction = Main.rand.NextVector2CircularEdge(NPC.width * 0.6f, NPC.height * 0.6f);
            float distance = 0.3f + Main.rand.NextFloat() * 0.5f;
            Vector2 velocity = new Vector2(0f, -Main.rand.NextFloat() * 0.3f - 1.5f);
            Texture2D texture = Request<Texture2D>(Texture).Value;



            Vector2 frameOrigin = NPC.frame.Size();
            Vector2 offset = new Vector2(NPC.width - frameOrigin.X + 100, NPC.height - NPC.frame.Height + 0);
            Vector2 drawPos = NPC.position - screenPos + frameOrigin + offset;

            float time = Main.GlobalTimeWrappedHourly;
            float timer = Main.GlobalTimeWrappedHourly / 2f + time * 0.04f;

            time %= 4f;
            time /= 2f;

            if (time >= 1f)
            {
                time = 2f - time;
            }

            time = time * 0.5f + 0.5f;
            SpriteEffects Effects = NPC.spriteDirection != -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            for (float i = 0f; i < 1f; i += 0.25f)
            {
                float radians = (i + timer) * MathHelper.TwoPi;

                spriteBatch.Draw(texture, drawPos + new Vector2(0f, 8f).RotatedBy(radians) * time, NPC.frame, new Color(99, 39, 51, 50), NPC.rotation, frameOrigin, NPC.scale, Effects, 0);
            }

            for (float i = 0f; i < 1f; i += 0.34f)
            {
                float radians = (i + timer) * MathHelper.TwoPi;

                spriteBatch.Draw(texture, drawPos + new Vector2(0f, 16f).RotatedBy(radians) * time, NPC.frame, new Color(255, 8, 55, 77), NPC.rotation, frameOrigin, NPC.scale, Effects, 0);
            }

            return true;
        }
        Vector2 Drawoffset => new Vector2(0, NPC.gfxOffY) + Vector2.UnitX * NPC.spriteDirection * 0;
        public virtual string GlowTexturePath => Texture + "_Glow";
        private Asset<Texture2D> _glowTexture;
        public Texture2D GlowTexture => (_glowTexture ??= (RequestIfExists<Texture2D>(GlowTexturePath, out var asset) ? asset : null))?.Value;
        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            float num108 = 4;
            float num107 = (float)Math.Cos((double)(Main.GlobalTimeWrappedHourly % 1.4f / 1.4f * 6.28318548f)) / 2f + 0.5f;
            float num106 = 0f;
            Color color1 = Color.DarkRed * num107 * .8f;
            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            spriteBatch.Draw(
                GlowTexture,
                NPC.Center - Main.screenPosition + Drawoffset,
                NPC.frame,
                color1,
                NPC.rotation,
                NPC.frame.Size() / 2,
                NPC.scale,
                effects,
                0
            );
            SpriteEffects spriteEffects3 = NPC.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Vector2 vector33 = new Vector2(NPC.Center.X, NPC.Center.Y) - Main.screenPosition + Drawoffset - NPC.velocity;
            Color color29 = new Color(127 - NPC.alpha, 127 - NPC.alpha, 127 - NPC.alpha, 0).MultiplyRGBA(Color.LightBlue);
            for (int num103 = 0; num103 < 1; num103++)
            {
                Color color28 = color29;
                color28 = NPC.GetAlpha(color28);
                color28 *= 1f - num107;
                Vector2 vector29 = NPC.Center + (num103 / (float)num108 * 6.28318548f + NPC.rotation + num106).ToRotationVector2() * (4f * num107 + 2f) - Main.screenPosition + Drawoffset - NPC.velocity * num103;
                Main.spriteBatch.Draw(GlowTexture, vector29, NPC.frame, color28, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, spriteEffects3, 0f);
            }
        }
        private void Disappear()
        {

            Player obj = Main.player[NPC.target];
            NPC.velocity.Y += 0.1f;
            NPC.scale -= 0.01f;
            if (NPC.scale <= 0)
            {
                NPC.active = false;
            }
            NPC.netUpdate = true;
        }
    }
}