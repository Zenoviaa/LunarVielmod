using Stellamod.Assets;
using Stellamod.Common.Particles;
using Stellamod.Common.XixianFlaskSystem;
using Stellamod.Content.Areas.SpringHills.WeaponsSH;
using Stellamod.Content.CommonMaterials;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace Stellamod.Items.Insources;

public class BarbedWireInsourcePlayer : ModPlayer
{
    public int stacks;
    public override void PostUpdateBuffs()
    {
        base.PostUpdateBuffs();
        if (!Player.HasBuff<BarbedWireInsourceBuff>())
            stacks = 0;
        
    }

    public override void PostHurt(Player.HurtInfo info)
    {
        base.PostHurt(info);
        if (stacks <= 0)
            return;
        if (info.DamageSource.SourceNPCIndex != -1)
        {
            NPC npc = Main.npc[info.DamageSource.SourceNPCIndex];
            float damage = info.Damage * stacks * 3;
            if (npc.boss)
                damage *= 0.5f;
            damage = MathHelper.Clamp(damage, 0, 200);
            npc.SimpleStrikeNPC((int)damage, -info.HitDirection);


            if (Player.whoAmI == Main.myPlayer)
            {
                for(var i = 0; i < 4; i++)
                {
                    var firer = ProjFirer.From<ThornedMeteorShard>(Player);
                    firer.velocity.Y = Main.rand.Next(-10, -5);
                    firer.velocity.X = Main.rand.NextFloat(-10, 10);
                    firer.damage = (int)(damage / 4);
                    firer.New();
                }
            }

            SoundStyle sound = AssetManager.GetSound("Thorny");
            sound.PitchVariance = 0.3f;
            SoundEngine.PlaySound(sound, Player.position);

            Vector2 velocity = (npc.Center - Player.Center);
            velocity = velocity.SafeNormalize(Vector2.Zero);

            var fac = ParticleUtils.ParticleFactory.FromSmallBurst(Player.Center, TriColorPalette.Foresty, new Vector2(3, 15));
            ParticleUtils.CreateSwirlingDustCircle(fac);

            FXUtil.ShakeCamera(Player.Center, 1024, 4);
            for (int i = 0; i < 8; i++)
            {
                Vector2 thornVel = velocity;
                thornVel = thornVel.RotatedByRandom(0.1f);
                thornVel *= Main.rand.NextFloat(2f, 10f) * 10;

                Vector2 pos = Player.position;
                pos.X += Main.rand.Next(0, Player.width);
                pos.Y += Main.rand.Next(0, Player.height);
                var fx = FXUtil.GlowStretch(pos, thornVel);
                fx.VectorScale.X *= 1.5f;
                fx.VectorScale.Y *= 0.75f;
                fx.OuterGlowColor = Color.Red;
            }
        }
    }
}
public class BarbedWireInsourceBuff : ModBuff
{
    public override string Texture => AssetReferences.Assets.Textures.Buffs.BarbedWireInsourceBuff.KEY;
    public override void Update(Player player, ref int buffIndex)
    {
        base.Update(player, ref buffIndex);
    }
}

public class BarbedWireInsource : InsourceItem
{
    public override string Texture => AssetReferences.Assets.Textures.SpringHills.Items.BarbedWireInsource.KEY;
    public override int GetAddedTime()
    {
        return 60 * 15;
    }

    public override void UseInsource(FlaskPlayer flaskPlayer)
    {
        base.UseInsource(flaskPlayer);
        Player player = flaskPlayer.Player;
        var berserkingPlayer = player.GetModPlayer<BarbedWireInsourcePlayer>();
        berserkingPlayer.stacks++;
        player.AddBuff(ModContent.BuffType<BarbedWireInsourceBuff>(), 60 * 10);
    }

    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew<Ivythorn, BlankBrooch>();
    }
}