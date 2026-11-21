using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Common;

public class StylishPlayer : ModPlayer
{
    public Vector2? moveToPosition;
    public Vector2? thrustVelocity;
    public float spin;
    public float spinSpeed;
    public bool shakingNCrying;
    public override void ResetEffects()
    {
        base.ResetEffects();
        shakingNCrying = false;
    }
    public override void PreUpdateMovement()
    {
        base.PreUpdateMovement();
        if (moveToPosition.HasValue)
        {
            Vector2 vel = moveToPosition.Value - Player.Center;
            Player.velocity = vel;
            moveToPosition = null;
        }
        if (thrustVelocity.HasValue)
        {
            Player.velocity = thrustVelocity.Value;
            thrustVelocity = null;
        }
        if(spin > 0)
        {
            Player.fullRotationOrigin = Player.Size * 0.5f;
            Player.fullRotation += spinSpeed;
            spin--;
            if(spin <= 0)
            {
                Player.fullRotation = 0;
            }
        }

     
    }
    
}

public static class StylishPlayerExtensions
{
    extension(Player stylishPlayer)
    {
        /// <summary>
        /// Plays a rumbling sound over the entire screen
        /// </summary>
        public bool ShakingNCrying
        {
            get => stylishPlayer.GetModPlayer<StylishPlayer>().shakingNCrying;
            set => stylishPlayer.GetModPlayer<StylishPlayer>().shakingNCrying = value;
        }
        public Vector2? moveToPosition
        {
            get => stylishPlayer.GetModPlayer<StylishPlayer>().moveToPosition;
            set => stylishPlayer.GetModPlayer<StylishPlayer>().moveToPosition = value;
        }
        public Vector2? thrustVelocity
        {
            get => stylishPlayer.GetModPlayer<StylishPlayer>().thrustVelocity;
            set => stylishPlayer.GetModPlayer<StylishPlayer>().thrustVelocity = value;
        }
        public void Spin(float spinSpeed, float spinTime)
        {
            var sp = stylishPlayer.GetModPlayer<StylishPlayer>();
            sp.spinSpeed = spinSpeed;
            sp.spin = spinTime;
        }
    }
}
