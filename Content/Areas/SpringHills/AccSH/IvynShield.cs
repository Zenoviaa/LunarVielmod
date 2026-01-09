using Microsoft.Xna.Framework;
using Stellamod.Assets;
using Stellamod.Common.WeaponTypes;
using Stellamod.Content.CommonMaterials;
using Stellamod.Content.Items.Materials;
using Stellamod.Core.Bases;
using Stellamod.Helpers;
using Stellamod.Items;
using Stellamod.Items.Accessories.Players;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.SpringHills.AccSH
{
    public class IvynShield : ModItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.DefaultToShield(ModContent.ProjectileType<IvynShieldHeld>());
        }
        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew<Ivythorn, BlankCard>();
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            base.UpdateAccessory(player, hideVisual);
            DashPlayer dashPlayer = player.GetModPlayer<DashPlayer>();
            dashPlayer.DashCount += 2;
            dashPlayer.DashRegenerationPenalty += 0.3f;
        }
    }

    public class IvynShieldHeld : AbstractShieldProjectile
    {
        public override void OnBlockMovement(NPC npc)
        {
            base.OnBlockMovement(npc);
            if (!npc.HasBuff<IvynVines>())
            {
                SoundStyle soundStyle = AssetRegistry.Sounds.Magic.VineWrap;
                soundStyle.PitchVariance = 0.3f;
                SoundEngine.PlaySound(soundStyle, npc.position);

                npc.AddBuff(ModContent.BuffType<IvynVines>(), 60);
                Owner.Heal(2);
                if(Main.myPlayer == Projectile.owner)
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), npc.Center, -Vector2.UnitY, ModContent.ProjectileType<IvynVinesWrap>(), 0, 1, Projectile.owner, ai1: npc.whoAmI);
                }
            }
        }
    }

    public class IvynVinesWrap : ModProjectile
    {
        public override string Texture => ModContent.GetInstance<IvynthornChokerVine>().Texture;
        private NPC Parent => Main.npc[(int)Projectile.ai[0]];
        private ref float Timer => ref Projectile.ai[1];
        private ref float State => ref Projectile.ai[2];
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Main.projFrames[Type] = 5;
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.hostile = false;
            Projectile.friendly = true;
            Projectile.timeLeft = 45;
            Projectile.penetrate = -1;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 15;
            Projectile.tileCollide = false;
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }
        public override void AI()
        {
            base.AI();
            Projectile.Center =Parent.Center;
            Projectile.Center += Projectile.velocity.SafeNormalize(Vector2.Zero) * 36;
            Projectile.Center -= new Vector2(24, 0);
            Timer++;
            if (Timer == 1)
            {
                FXUtil.GlowCircleBoom(Projectile.Center, Color.White, Color.Brown, Color.RosyBrown, 8, 0.05f);
                float num = 8;
                for (float i = 0; i < num; i++)
                {
                    float l = (i) / num;
                    float rot = l * MathHelper.TwoPi;
                    Vector2 vel = rot.ToRotationVector2() * 2;
                    Dust.NewDustPerfect(Projectile.Center, DustID.t_LivingWood, vel);
                }
            }

            Projectile.rotation = Projectile.velocity.ToRotation();

            float inFrameSpeed = 3;
            float outFrameSpeed = 7;
            switch (State)
            {
                case 0:
                    Projectile.frameCounter++;
                    if (Projectile.frameCounter >= inFrameSpeed)
                    {
                        Projectile.frameCounter = 0;
                        Projectile.frame++;

                        if (Projectile.frame >= Main.projFrames[Projectile.type])
                        {
                            Projectile.frame = Main.projFrames[Projectile.type] - 1;
                            State = 1;
                        }
                    }

                    break;
                case 1:
                    Projectile.frameCounter++;
                    if (Projectile.frameCounter >= outFrameSpeed)
                    {
                        Projectile.frameCounter = 0;
                        Projectile.frame--;
                        if (Projectile.frame <= 0)
                        {
                            Projectile.frame = 0;
                        }
                    }
                    break;
            }
        }
        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
            float num = 8;
            for (float i = 0; i < num; i++)
            {
                float l = (i) / num;
                float rot = l * MathHelper.TwoPi;
                Vector2 vel = rot.ToRotationVector2() * 2;
                Dust.NewDustPerfect(Projectile.Center, DustID.t_LivingWood, vel);
            }
        }
    }
    public class IvynVines : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoTimeDisplay[Type] = false;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.lifeRegen -= 10;
            npc.velocity *= 0.8f;
        }
    }
}
