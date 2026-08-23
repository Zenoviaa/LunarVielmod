using Stellamod.Content.CommonMaterials;
using Stellamod.Helpers;
using Stellamod.Items;
using Stellamod.Visual.Particles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Tundra.Snow.AccsSN;


public class FlashbackPlayer : ModPlayer
{
    private Vector2[] _oldPos;
    private float _flashbackTimer;
    public bool hasFlashback;
    public float flashbackTime;
    public override void Load()
    {
        base.Load();
    
    }
    public override void ResetEffects()

    {
        base.ResetEffects();
        hasFlashback = false;
        flashbackTime = 1;
    }
    public override void PreUpdateMovement()
    {
        base.PreUpdateMovement();
        if(_flashbackTimer > 0)
        {
            for(int i = 1; i < _oldPos.Length; i++)
            {
                Vector2 center = _oldPos[i] + Player.Size * 0.5f;
                if (Main.rand.NextBool(4))
                {
                    Vector2 stretchPos = center + Main.rand.NextVector2Circular(32, 32);
                    Vector2 velocity = (_oldPos[i] - _oldPos[i - 1]);
                    var fx = FXUtil.GlowStretch(stretchPos, velocity);
                    fx.OuterGlowColor = Color.Blue;
                    fx.VectorScale *= 0.4f;
                }

                if (Main.rand.NextBool(4))
                {
                    FaintSmokeParticle sp = FaintSmokeParticle.Spawn(center, Vector2.Zero);
                    sp.color = Color.Lerp(Color.Lerp(Color.LightBlue, Color.DarkBlue, Main.rand.NextFloat(0f, 1f)), Color.Black, 0.7f);
                    sp.fadeToColor = Color.DarkGray;
                    //  sp.gravity = 0;
                    //  sp.noTileCollide = true;
                    sp.Scale *= 0.25f;
                    //    sp.offsetRot = Main.rand.NextFloat(0f, MathHelper.TwoPi);
                    sp.behindLayer = true;
                }
            }
            Player.position = _oldPos[_oldPos.Length - 1];
            Player.velocity = Vector2.Zero;

            FXUtil.GlowCircleBoom(Player.Center, Color.White, Color.Blue, Color.DarkBlue, duration: 45, baseSize: 0.2f);
            _flashbackTimer--;
        }
    }

    public override void OnHurt(Player.HurtInfo info)
    {
        base.OnHurt(info);
        if (!hasFlashback)
            return;
        SoundStyle warp = new SoundStyle("Stellamod/Assets/Sounds/ArcaneExplode");
        warp.PitchVariance = 0.2f;
        SoundEngine.PlaySound(warp);
        _flashbackTimer = flashbackTime;
    }

    public override void PostUpdateMiscEffects()
    {
        base.PostUpdateMiscEffects();
        _oldPos ??= new Vector2[60];
        for (int i = _oldPos.Length - 1; i > 0; i--)
        {
            _oldPos[i] = _oldPos[i - 1];
        }
        _oldPos[0] = Player.position;
    }
}
public class Flashback : ModItem
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.DefaultToAccessory();
    }

    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew<WinterbornShard, BlankAccessory>();
    }
    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        base.UpdateAccessory(player, hideVisual);
        player.GetModPlayer<FlashbackPlayer>().hasFlashback = true;
    }
}
