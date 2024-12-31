using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Helpers;
using Stellamod.NPCs.Bosses.IrradiaNHavoc.Havoc.Projectiles;
using Stellamod.NPCs.Bosses.STARBOMBER.Projectiles;
using Stellamod.Projectiles.Summons;
using Stellamod.Projectiles.Visual;
using Stellamod.Utilis;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Bosses.GothiviaTheSun.GOS.Projectiles
{
    public class CarmodyGreen : ModNPC
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Morrowed Swampster");
            Main.npcFrameCount[NPC.type] = 16;
        }

        public enum ActionState
        {

            Speed,
            Wait
        }
        // Current state
        public int frameTick;
        // Current state's timer
        public float timer;
        public int PrevAtack;
        float DaedusDrug = 4;
        // AI counter
        public int counter;

        public ActionState State = ActionState.Wait;
        public override void SetDefaults()
        {
            NPC.width = 750;
            NPC.height = 450;
            NPC.damage = 910;
            NPC.defense = 30;
            NPC.lifeMax = 500000;
            NPC.value = 0f;
            NPC.timeLeft = 320;
            NPC.knockBackResist = .0f;
            NPC.noTileCollide = true;
            NPC.noGravity = true;
            NPC.dontTakeDamage = true;
        }

        int invisibilityTimer;
        public override void HitEffect(NPC.HitInfo hit)
        {
            for (int k = 0; k < 11; k++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Torch, 1, -1f, 1, default, .61f);
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 255, 255, 0) * (1f - NPC.alpha / 80f);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            // Since the NPC sprite naturally faces left, we want to flip it when its X velocity is positive

            SpriteEffects effects = SpriteEffects.None;

            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;

            spriteBatch.Draw(texture, new Vector2(NPC.Center.X, NPC.Center.Y) - Main.screenPosition + new Vector2(0, NPC.gfxOffY), NPC.frame, new Color(255, 255, 255, 0) * (1f - NPC.alpha / 1f), NPC.rotation, NPC.frame.Size() / 2, NPC.scale * 2, effects, 0);


            // Draw the periodic glow effect behind the item when dropped in the world (hence PreDrawInWorld)


            // Using a rectangle to crop a texture can be imagined like this:
            // Every rectangle has an X, a Y, a Width, and a Height
            // Our X and Y values are the position on our texture where we start to sample from, using the top left corner as our origin
            // Our Width and Height values specify how big of an area we want to sample starting from X and Y
            return false;
        }

        float trueFrame = 0;
        public override void FindFrame(int frameHeight)
        {
            NPC.frame.Width = 750;
            NPC.frame.X = ((int)trueFrame % 5) * NPC.frame.Width;
            NPC.frame.Y = (((int)trueFrame - ((int)trueFrame % 5)) / 5) * NPC.frame.Height;
        }

        public void UpdateFrame(float speed, int minFrame, int maxFrame)
        {
            trueFrame += speed;
            if (trueFrame < minFrame)
            {
                trueFrame = minFrame;
            }
            if (trueFrame > maxFrame)
            {
                trueFrame = minFrame;
            }
        }
        public float Shooting = 0f;

        int bee = 1100;
        int bee2 = 320;
        public int rippleCount = 20;
        public int rippleSize = 5;
        public int rippleSpeed = 15;
        public float distortStrength = 300f;
        int gr = 38;
        public bool HHH = false;
        public override void AI()
        {
            var entitySource = NPC.GetSource_FromAI();
            timer++;
            NPC.TargetClosest();
            NPC.spriteDirection = NPC.direction;
            float ai1 = NPC.whoAmI;
            Vector2 LightPos;

            Shooting++;
            if (Shooting == 1)
            {
                LightPos.X = NPC.Center.X;
                LightPos.Y = NPC.Center.Y;
                var EntitySource = NPC.GetSource_FromThis();
                if (StellaMultiplayer.IsHost)
                {
                    Utilities.NewProjectileBetter(LightPos.X, LightPos.Y - 2000, 0, 10,
                        ModContent.ProjectileType<GREENLS>(), 1500, 0f, owner: Main.myPlayer);

                }

                if (StellaMultiplayer.IsHost)
                {
                    int type = ModContent.ProjectileType<GreenSunsSuckingProj>();
                    int damage = 10;
                    int knockback = 1;
                    Vector2 pos = NPC.Center;
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), pos, NPC.rotation.ToRotationVector2(),
                     type, damage, knockback, Main.myPlayer, 0, ai1: NPC.whoAmI);

                }






                ScreenShaderSystem shaderSystem = ModContent.GetInstance<ScreenShaderSystem>();
                shaderSystem.TintScreen(Color.DarkTurquoise, 0.2f, timer: 320);
                shaderSystem.DistortScreen(TextureRegistry.NormalNoise1, new Vector2(0.001f, 0.001f), blend: 0.05f, timer: 320);
                shaderSystem.VignetteScreen(-1f, timer: 320);

                //NPC.NewNPC(entitySource, (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<ALCADSWIRL>());
            }
            NPC.noTileCollide = true;
        
           
            UpdateFrame(1f, 1, 80);
            bee2--;
            gr++;

            Vector2 direction = Vector2.Normalize(Main.player[NPC.target].Center - NPC.Center) * 8.5f;


            
            if (gr == 80 && !HHH)
            {

                if (StellaMultiplayer.IsHost)
                {
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y, direction.X * 3, direction.Y * 3, ModContent.ProjectileType<GothDarkBlastProj>(), 600, 1, Main.myPlayer, 0, 0);
                }

                gr = 0;
            }
            

            if (bee2 <= 90)
            {
                HHH = true;
            }

           

            if (bee2 == 0)
            {
                NPC.Kill();

                ScreenShaderSystem shaderSystem = ModContent.GetInstance<ScreenShaderSystem>();
                shaderSystem.UnTintScreen();
                shaderSystem.UnDistortScreen();
                shaderSystem.UnVignetteScreen();

                if (StellaMultiplayer.IsHost)
                {
                    int type = ModContent.ProjectileType<GreenSunsBoomProj>();
                    int damage = 10;
                    int knockback = 1;
                    Vector2 pos = NPC.Center;
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), pos, NPC.rotation.ToRotationVector2(),
                     type, damage, knockback, Main.myPlayer, 0, ai1: NPC.whoAmI);
                }


                for (int i = 0; i < 150; i++)
                {
                    Vector2 speed = Main.rand.NextVector2CircularEdge(4f, 4f);
                    var d = Dust.NewDustPerfect(NPC.Center, DustID.CoralTorch, speed * 17, Scale: 5f);
                    d.noGravity = true;

                    Vector2 speeda = Main.rand.NextVector2CircularEdge(4f, 4f);
                    var da = Dust.NewDustPerfect(NPC.Center, DustID.CoralTorch, speeda * 11, Scale: 5f);
                    da.noGravity = false;

                    Vector2 speedab = Main.rand.NextVector2CircularEdge(5f, 5f);
                    var dab = Dust.NewDustPerfect(NPC.Center, DustID.CoralTorch, speeda * 30, Scale: 5f);
                    dab.noGravity = false;
                }

                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/STARGROP"));
                if (Main.netMode != NetmodeID.Server && Terraria.Graphics.Effects.Filters.Scene["Shockwave"].IsActive())
                {
                    Terraria.Graphics.Effects.Filters.Scene["Shockwave"].Deactivate();
                }
            }


            switch (State)
            {

                case ActionState.Wait:
                    counter++;
                    Wait();

                    if (Main.netMode != NetmodeID.Server && !Terraria.Graphics.Effects.Filters.Scene["Shockwave"].IsActive())
                    {
                        Terraria.Graphics.Effects.Filters.Scene.Activate("Shockwave", NPC.Center).GetShader().UseColor(rippleCount, rippleSize, rippleSpeed).UseTargetPosition(NPC.Center);

                    }

                    if (Main.netMode != NetmodeID.Server && Terraria.Graphics.Effects.Filters.Scene["Shockwave"].IsActive())
                    {
                        float progress = (180f - bee) / 60f; // Will range from -3 to 3, 0 being the point where the bomb explodes.
                        Terraria.Graphics.Effects.Filters.Scene["Shockwave"].GetShader().UseProgress(progress).UseOpacity(distortStrength * (1 - progress / 3f));
                    }


                    break;

                case ActionState.Speed:
                    counter++;
                    Speed();

                    if (Main.netMode != NetmodeID.Server && !Terraria.Graphics.Effects.Filters.Scene["Shockwave"].IsActive())
                    {
                        Terraria.Graphics.Effects.Filters.Scene.Activate("Shockwave", NPC.Center).GetShader().UseColor(rippleCount, rippleSize, rippleSpeed).UseTargetPosition(NPC.Center);

                    }

                    if (Main.netMode != NetmodeID.Server && Terraria.Graphics.Effects.Filters.Scene["Shockwave"].IsActive())
                    {
                        float progress = (180f - bee) / 60f; // Will range from -3 to 3, 0 being the point where the bomb explodes.
                        Terraria.Graphics.Effects.Filters.Scene["Shockwave"].GetShader().UseProgress(progress).UseOpacity(distortStrength * (1 - progress / 3f));
                    }
                    break;


                default:
                    counter++;
                    break;
            }
        }


        public void Wait()
        {
            timer++;
            if (timer > 50)
            {




            }
            else if (timer == 60)
            {
                State = ActionState.Speed;
                timer = 0;


                if (Main.netMode != NetmodeID.Server && Terraria.Graphics.Effects.Filters.Scene["Shockwave"].IsActive())
                {
                    Terraria.Graphics.Effects.Filters.Scene["Shockwave"].Deactivate();
                }
            }
        }


        public void Speed()
        {
            timer++;
            if (timer == 60)
            {
                State = ActionState.Wait;
                timer = 0;


                if (Main.netMode != NetmodeID.Server && Terraria.Graphics.Effects.Filters.Scene["Shockwave"].IsActive())
                {
                    Terraria.Graphics.Effects.Filters.Scene["Shockwave"].Deactivate();
                }
            }

        }
    }
}