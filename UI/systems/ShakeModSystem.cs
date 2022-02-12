using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.UI.systems
{
    public class ShakeModSystem : ModSystem
    {

        public static float Shake;
            
        public override void ModifyScreenPosition()
        {



            Main.screenPosition += Utils.RandomVector2(Main.rand, Main.rand.NextFloat(-Shake, Shake), Main.rand.NextFloat(-Shake, Shake));


            if (Shake > 0){
                Shake--;
            }
        }
    }
}


    

