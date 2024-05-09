using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Buffs;
using Stellamod.Helpers;
using Stellamod.Projectiles.IgniterExplosions;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Magic
{
    internal class RibbonStaffStart : ModProjectile
    {
        //AI
        private ref float Timer => ref Projectile.ai[0];
        private int BuffType => ModContent.BuffType<RibbonWrapped>();
        private Player Owner => Main.player[Projectile.owner];

        //Animation Stuff
        public Vector2[] CirclePos = new Vector2[48];
        public int FrameCounter;
        public int FrameTick;


        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 128;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = int.MaxValue;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 30;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            Timer++;
            if (!Owner.channel)
            {
                Projectile.Kill();
            }
            else if(Main.myPlayer == Projectile.owner)
            {
                Projectile.Center = Main.MouseWorld;
                Projectile.netUpdate = true;
            }
            Visuals();
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (!target.HasBuff(BuffType))
            {
                SoundStyle soundStyle = new SoundStyle("Stellamod/Assets/Sounds/RibbonStaffWrap1");
                soundStyle.PitchVariance = 0.15f;
                SoundEngine.PlaySound(soundStyle, Projectile.position);

                //Spawn the wrapping projectile here
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), target.Center, Vector2.Zero,
                    ModContent.ProjectileType<RibbonStaffTieProj>(), Projectile.damage, Projectile.knockBack, Projectile.owner, ai1: target.whoAmI);
            }
            target.AddBuff(BuffType, 120);
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (!npc.active)
                    continue;
                if (npc.HasBuff(BuffType))
                {
                    int buffIndex = npc.FindBuffIndex(BuffType);
                    npc.DelBuff(buffIndex);

                    if (Timer > 60)
                    {
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), npc.Center, Vector2.Zero,
                            ModContent.ProjectileType<RibbonBoom>(), Projectile.damage * 7, Projectile.knockBack, Projectile.owner);
                    }
                }
            }
        }

        // Here you can decide if your minion breaks things like grass or pots
        public override bool? CanCutTiles()
        {
            return false;
        }

        // This is mandatory if your minion deals contact damage (further related stuff in AI() in the Movement region)
        private void Visuals()
        {
            DrawHelper.DrawCircle(Projectile.Center, Projectile.width, CirclePos);
            Lighting.AddLight(Projectile.Center, Color.White.ToVector3() * 0.78f);
        }


        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D chainTexture = ModContent.Request<Texture2D>(Texture).Value;
            int frameCount = 8;
            int frameTime = 2;
            Rectangle animationFrame = chainTexture.AnimationFrame(
                ref FrameCounter, ref FrameTick, frameTime, frameCount, true);
            DrawHelper.DrawFlowerChains(chainTexture, CirclePos, animationFrame, 1f);
            return false;
        }
    }
}
