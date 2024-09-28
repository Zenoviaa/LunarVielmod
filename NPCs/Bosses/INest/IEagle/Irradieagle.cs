
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.NPCs.Catacombs;
using Stellamod.NPCs.Event.GreenSun.Dulacrowe;
using Stellamod.Utilis;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;


//By Al0n37
namespace Stellamod.NPCs.Bosses.INest.IEagle
{
    [AutoloadBossHead]
    public class Irradieagle : ModNPC
    {
        bool Dir;
        float DashSpeed = 9;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Irradieagle");
            NPCID.Sets.TrailCacheLength[NPC.type] = 14;
            Main.npcFrameCount[NPC.type] = 8;
            NPCID.Sets.TrailingMode[NPC.type] = 2;
        }

        public override void SetDefaults()
        {

            NPC.width = 200;
            NPC.height = 200;
            NPC.damage = 83;
            NPC.defense = 33;
            NPC.lifeMax = 17500;
            NPC.HitSound = SoundID.NPCHit51;
            NPC.DeathSound = SoundID.NPCHit53;
            NPC.value = 60f;
            NPC.knockBackResist = 0.0f;
            NPC.scale = 0.8f;
            NPC.boss = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.aiStyle = 0;
            NPC.BossBar = ModContent.GetInstance<NestBossBar>();
        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * balance);
        }


        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter += 0.15f;
            NPC.frameCounter %= Main.npcFrameCount[NPC.type];
            int frame = (int)NPC.frameCounter;
            NPC.frame.Y = frame * frameHeight;
        }

        public float DrugRidus = 0;
        public int DrugAlpha = 0;
        private void Disappear()
        {
            Player obj = Main.player[base.NPC.target];
            base.NPC.velocity.Y += 0.5f;
            if (Vector2.Distance(obj.position, base.NPC.position) > 1000f)
            {
                NPC.active = false;
            }
        }

        public int previousAttack;
        public Vector2 PlayerPastPost;
        public override void AI()
        {
            Lighting.AddLight((int)((NPC.position.X + (NPC.width / 2)) / 16f), (int)((NPC.position.Y + (NPC.height / 2)) / 16f), 0.46f, 0.32f, .1f);
            NPC.rotation = NPC.velocity.X * 0.01f;
            bool expertMode = Main.expertMode;
            NPC.spriteDirection = NPC.direction;
            Player player = Main.player[NPC.target];
            NPC.TargetClosest(true);
            if (!NPC.HasPlayerTarget)
            {
                NPC.TargetClosest(false);
                Player player1 = Main.player[NPC.target];

                if (!NPC.HasPlayerTarget || NPC.Distance(player1.Center) > 3000f)
                {
                    return;
                }
            }

            int distance = (int)(NPC.Center - player.Center).Length();
            if (distance > 3000f || player.dead)
            {
                NPC.ai[2] = 2;
                Disappear();
            }

            NPC.rotation = NPC.velocity.X * 0.01f;
            Lighting.AddLight((int)(NPC.Center.X / 16), (int)(NPC.Center.Y / 16), 0.46f, 0.32f, .1f);
            if (NPC.ai[2] == 0)
            {
                NPC.Center = player.Center - Vector2.UnitY * (1800f - 300f);
                NPC.ai[0]++;
                if (NPC.ai[0] >= 1)
                {
                    NPC.ai[0] = 0;
                    NPC.ai[2] = 1;
                }
            }

            if (DrugRidus >= 0)
            {
                DrugRidus -= 1.5f;
            }
            bool p2 = NPC.life < NPC.lifeMax / 2;
            Vector2 targetPos;
            if (NPC.ai[2] == 1)
                switch (NPC.ai[1])
                {
                    case 0:
                        // default attack, just moves above player, waits  seconds then does a random attack
                        targetPos = player.Center;
                        Movement(targetPos, 0f, -500f, 0.05f);
                        NPC.ai[0]++;
                        if (NPC.ai[0] > 0)
                        {
                            NPC.ai[0] = 0;
                            if (StellaMultiplayer.IsHost)
                            {
                                while (NPC.ai[1] == previousAttack)
                                    NPC.ai[1] = Main.rand.Next(1, 1 + 1);
                                NPC.netUpdate = true;
                            }
         
                        }
                        break;
                    case 1:
                        NPC.ai[0]++;
                        if (NPC.ai[0] == 100)
                        {
                            PlayerPastPost = player.position;
                        }
                        if (NPC.ai[0] >= 100)
                        {
                            if (NPC.position.X >= player.position.X)
                            {
                                Movement(PlayerPastPost, 200f, 0f, 0.05f);
                            }
                            else
                            {
                                Movement(PlayerPastPost, -200f, 0f, 0.05f);
                            }

                        }
                        if (NPC.ai[0] == 200)
                        {

                            NPC.ai[0] = 0;
                            previousAttack = 1;
                            if (StellaMultiplayer.IsHost)
                            {
                                NPC.ai[1] = Main.rand.Next(2, 6);
                                NPC.netUpdate = true;
                            }
                        }
                        break;
                    case 2:
                        NPC.ai[0]++;
                        if(NPC.life < NPC.lifeMax * 0.5f)
                        {
                            if (NPC.ai[0] == 40 || NPC.ai[0] == 80)
                            {
                                DrugRidus = 50;
                                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Irradieagle_Wave"));
                                var entitySource = NPC.GetSource_FromThis();
                                targetPos = player.Center;
                                DrugRidus = 60;
                                Vector2 direction = Vector2.Normalize(NPC.Center - Main.player[NPC.target].Center) * 8.5f;
                                Movement(targetPos, (direction.X * 170), (direction.Y * 170), 0.05f);
                                SoundEngine.PlaySound(SoundID.DD2_BetsyWindAttack, NPC.position);
                                SoundEngine.PlaySound(SoundID.DD2_DarkMageAttack, NPC.position);
                                int bloodproj;
                                bloodproj = Main.rand.Next(new int[] { Mod.Find<ModProjectile>("AcidBlast").Type, Mod.Find<ModProjectile>("AcidBlast").Type, Mod.Find<ModProjectile>("AcidBlast").Type });
         
                                if (StellaMultiplayer.IsHost)
                                {
                                     Projectile.NewProjectile(entitySource, NPC.Center.X + (7 * NPC.direction), NPC.Center.Y - 10, -(NPC.position.X - player.position.X) / distance * 8, -(NPC.position.Y - player.position.Y + Main.rand.Next(-50, 50)) / distance * 8, bloodproj, 25, 0);
                                }

                            }
                        }
                        else
                        {
                            if (NPC.ai[0] == 50)
                            {
                                DrugRidus = 70;
                                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Irradieagle_Wave"));
                                var entitySource = NPC.GetSource_FromThis();
                                targetPos = player.Center;
                                DrugRidus = 60;
                                Vector2 direction = Vector2.Normalize(NPC.Center - Main.player[NPC.target].Center) * 8.5f;
                                Movement(targetPos, (direction.X * 170), (direction.Y * 170), 0.05f);
                                SoundEngine.PlaySound(SoundID.DD2_BetsyWindAttack, NPC.position);
                                SoundEngine.PlaySound(SoundID.DD2_DarkMageAttack, NPC.position);
                                int bloodproj;
    
                                bloodproj = Main.rand.Next(new int[] { Mod.Find<ModProjectile>("AcidBlast").Type, Mod.Find<ModProjectile>("AcidBlast").Type, Mod.Find<ModProjectile>("AcidBlast").Type });
                                if (StellaMultiplayer.IsHost)
                                {
                                    Projectile.NewProjectile(entitySource, NPC.Center.X + (7 * NPC.direction), NPC.Center.Y - 10, -(NPC.position.X - player.position.X) / distance * 8, -(NPC.position.Y - player.position.Y + Main.rand.Next(-50, 50)) / distance * 8, bloodproj, 25, 0);
                                }
                            }
                        }

                        if (NPC.ai[0] == 100)
                        {
                            NPC.ai[0] = 0;
                            previousAttack = 1;
                            NPC.ai[1] = 10;
                        }
                        break;
                    case 3:
                        NPC.ai[0]++;
                        if (NPC.ai[0] >= 20 && NPC.ai[0] <= 100)
                        {
   
                            if (NPC.ai[0] % 8 == 0)
                            {
                                DrugRidus = 20;
                                //SoundEngine.PlaySound(new SoundStyle("Stellamod/Sounds/Custom/Npc/AcidProbe3"), NPC.position);
                                targetPos = player.Center;
                                Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(base.NPC.Center, 512f, 32f);
                                Vector2 direction = Vector2.Normalize(Main.player[NPC.target].Center - NPC.Center) * 8.5f;
                                SoundEngine.PlaySound(SoundID.Item8, NPC.position);
                                SoundEngine.PlaySound(SoundID.Zombie53, NPC.position);
                                Movement(targetPos, (direction.X * 30) * -1, (direction.Y * 30) * -1, 0.05f);

                                if (StellaMultiplayer.IsHost)
                                {
                                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y, direction.X * 2, direction.Y * 2, ModContent.ProjectileType<ToxicMissile>(), 30, 1, Main.myPlayer, 0, 0);
                                }
                            }
                        }
                        if (NPC.ai[0] == 100)
                        {
               
                            NPC.ai[0] = 0;
                            previousAttack = 2;
                            NPC.ai[1] = 10;
                        }
                        break;
                    case 4:
                        NPC.ai[0]++;
                        if (NPC.ai[0] >= 20 && NPC.ai[0] <= 100)
                        {
                            targetPos = player.Center;
                            if (NPC.ai[0] % 30 == 0)
                            {
                                DrugRidus = 30;
                                var entitySource = NPC.GetSource_FromThis();
                                if (StellaMultiplayer.IsHost)
                                {
                                    NPC.NewNPC(entitySource, (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<IrradieagleSurvent>());
                                }
                            }
                        }
                        if (NPC.ai[0] == 20)
                        {
                            SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Irradieagle_Spawn"), NPC.position);
                        }
                        if (NPC.ai[0] == 100)
                        {
 
                            NPC.ai[0] = 0;
                            previousAttack = 2;
                            NPC.ai[1] = 10;
                        }
                        break;
                    case 5:
                        NPC.ai[0]++;
                        if (NPC.ai[0] >= 20 && NPC.ai[0] <= 160)
                        {
                            if (NPC.ai[0] % 30 == 0)
                            {
                                DrugRidus = 50;
                                int Sound = Main.rand.Next(1, 3);
                                if (Sound == 1)
                                {
                                    SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Irradieagle_Flare1"), NPC.position);
                                }
                                else
                                {
                                    SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Irradieagle_Flare2"), NPC.position);
                                }
                                targetPos = player.Center;
                                Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(base.NPC.Center, 512f, 32f);
                                Vector2 direction = Vector2.Normalize(Main.player[NPC.target].Center - NPC.Center) * 8.5f;
                                SoundEngine.PlaySound(SoundID.Item8, NPC.position);
                                SoundEngine.PlaySound(SoundID.Zombie53, NPC.position);
                                Movement(targetPos, (direction.X * 30) * -1, (direction.Y * 30) * -1, 0.05f);


                                if (StellaMultiplayer.IsHost)
                                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y, direction.X * 2, direction.Y * 2, ModContent.ProjectileType<TulacroweFireball>(), 40, 1, Main.myPlayer, 0, 0);
                            }
                        }

                        if (NPC.ai[0] == 160)
                        {         
                            NPC.ai[0] = 0;
                            previousAttack = 2;
                            NPC.ai[1] = 10;
                        }
                        break;
                    case 10:
                        NPC.ai[0]++;

                        if(NPC.ai[0] == 2)
                        {
                            if (StellaMultiplayer.IsHost)
                            {
                                int Chance2 = Main.rand.Next(1, 3 + 1);
                                if (Chance2 == 1)
                                {
                                    DashSpeed = 9f;
                                    NPC.ai[0] = 0;
                                    previousAttack = 1;
                                    NPC.ai[1] = 1;
                                }
                                NPC.netUpdate = true;
                            }

                 
                            NPC.spriteDirection = -NPC.direction;
                            if (NPC.position.X >= player.position.X)
                            {
                                Dir = true;
                            }
                            else
                            {
                                Dir = false;
                            }
                        }
 
                        if (NPC.ai[0] == 10)
                        {
                            SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Irradieagle_Dash"));
                        }

                        if (NPC.ai[0] >= 10)
                        {
                            if (Dir)
                            {
                                NPC.velocity.X = -DashSpeed;
                                DashSpeed *= 1.02f;
                            }
                            else
                            {
                                NPC.velocity.X = DashSpeed;
                                DashSpeed *= 1.02f;

                            }
                        }
                        if (NPC.ai[0] >= 100)
                        {
                            NPC.Center = player.Center - Vector2.UnitY * (1000f - 300f);
                            DashSpeed = 9f;                  
                            NPC.ai[0] = 0;
                            previousAttack = 1;
                            NPC.ai[1] = 1;
                        }
                        break;
                }
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            int d = 74;
            int d1 = DustID.CursedTorch;
            for (int k = 0; k < 30; k++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, d, 2.5f * hit.HitDirection, -2.5f, 0, Color.White, 0.7f);
                Dust.NewDust(NPC.position, NPC.width, NPC.height, d1, 2.5f * hit.HitDirection, -2.5f, 0, default(Color), .74f);
            }
            if (NPC.life <= 0)
            {
                Player player = Main.player[NPC.target];
                player.GetModPlayer<MyPlayer>().IrradiatedKilled = 1;
                for (int i = 0; i < 20; i++)
                {
                    int num = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.CursedTorch, 0f, -2f, 0, default(Color), .8f);
                    Main.dust[num].noGravity = true;
                    Main.dust[num].position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
                    Main.dust[num].position.Y += Main.rand.Next(-50, 51) * .05f - 1.5f;
                    if (Main.dust[num].position != NPC.Center)
                        Main.dust[num].velocity = NPC.DirectionTo(Main.dust[num].position) * 6f;
                }
            }
        }
        public void Movement(Vector2 Player2, float PosX, float PosY, float Speed)
        {
            Player player = Main.player[NPC.target];
            Vector2 target = Player2 + new Vector2(PosX, PosY);
            base.NPC.velocity = Vector2.Lerp(base.NPC.velocity, VectorHelper.MovemontVelocity(base.NPC.Center, Vector2.Lerp(base.NPC.Center, target, 0.5f), base.NPC.Center.Distance(target) * Speed), 0.1f);
        }
        public virtual string GlowTexturePath => Texture + "_Glow";
        private Asset<Texture2D> _glowTexture;
        public Texture2D GlowTexture => (_glowTexture ??= (ModContent.RequestIfExists<Texture2D>(GlowTexturePath, out var asset) ? asset : null))?.Value;
        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (GlowTexture is not null)
            {
                SpriteEffects spriteEffects = SpriteEffects.None;
                if (NPC.spriteDirection == 1)
                {
                    spriteEffects = SpriteEffects.FlipHorizontally;
                }
                Vector2 halfSize = new Vector2(GlowTexture.Width / 2, GlowTexture.Height / Main.npcFrameCount[NPC.type] / 2);
                spriteBatch.Draw(
                    GlowTexture,
                    new Vector2(NPC.position.X - screenPos.X + (NPC.width / 2) - GlowTexture.Width * NPC.scale / 2f + halfSize.X * NPC.scale, NPC.position.Y - screenPos.Y + NPC.height - GlowTexture.Height * NPC.scale / Main.npcFrameCount[NPC.type] + 4f + halfSize.Y * NPC.scale + Main.NPCAddHeight(NPC) + NPC.gfxOffY),
                    NPC.frame,
                    Color.White,
                    NPC.rotation,
                    halfSize,
                    NPC.scale,
                    spriteEffects,
                0);
            }
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
        {


            SpriteEffects Effects = NPC.spriteDirection != -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;


            Vector2 center = NPC.Center + new Vector2(0f, NPC.height * -0.1f);
            // This creates a randomly rotated vector of length 1, which gets it's components multiplied by the parameters
            Vector2 direction = Main.rand.NextVector2CircularEdge(NPC.width * 0.6f, NPC.height * 0.6f);
            float distance = 0.3f + Main.rand.NextFloat() * 0.5f;
            Vector2 velocity = new Vector2(0f, -Main.rand.NextFloat() * 0.3f - 1.5f);
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;



            Vector2 frameOrigin = NPC.frame.Size();
            Vector2 offset = new Vector2(NPC.width - frameOrigin.X + 10, NPC.height - NPC.frame.Height + 0);
            Vector2 DrawPos = NPC.position - screenPos + frameOrigin + offset;

            float time = Main.GlobalTimeWrappedHourly;
            float timer = Main.GlobalTimeWrappedHourly / 2f + time * 0.04f;

            time %= 4f;
            time /= 2f;

            if (time >= 1f)
            {
                time = 2f - time;
            }

            time = time * 0.5f + 0.5f;
            for (float i = 0f; i < 1f; i += 0.25f)
            {
                float radians = (i + timer) * MathHelper.TwoPi;

                spriteBatch.Draw(texture, DrawPos + new Vector2(0f, DrugRidus).RotatedBy(radians) * time, NPC.frame, new Color(53, 107, 112, DrugAlpha), NPC.rotation, frameOrigin, NPC.scale, Effects, 0);
            }

            for (float i = 0f; i < 1f; i += 0.34f)
            {
                float radians = (i + timer) * MathHelper.TwoPi;

                spriteBatch.Draw(texture, DrawPos + new Vector2(0f, DrugRidus).RotatedBy(radians) * time, NPC.frame, new Color(152, 208, 113, DrugAlpha), NPC.rotation, frameOrigin, NPC.scale, Effects, 0);
            }

            Lighting.AddLight(NPC.Center, Color.GreenYellow.ToVector3() * 2.25f * Main.essScale);
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            var drawOrigin = new Vector2(TextureAssets.Npc[NPC.type].Width() * 0.5f, NPC.height * 0.5f);
            for (int k = 0; k < NPC.oldPos.Length; k++)
            {
                Vector2 drawPos = NPC.oldPos[k] - Main.screenPosition + NPC.Size / 2 + new Vector2(0f, NPC.gfxOffY);
                Color color = NPC.GetAlpha(Color.Lerp(new Color(152, 208, 113), new Color(53, 107, 112), 1f / NPC.oldPos.Length * k) * (1f - 1f / NPC.oldPos.Length * k));
                spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, drawPos, new Microsoft.Xna.Framework.Rectangle?(NPC.frame), color, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, Effects, 0f);
            }

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            return true;
        }
        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            scale = 1.9f;   //this make the NPC Health Bar biger
            return null;
        }

    }
}