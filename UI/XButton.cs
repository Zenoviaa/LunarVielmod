using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.UI;

namespace Stellamod.UI
{
    public class XButton : UIElement 
    {
        private Action _onClick;
        public XButton(Action onClick)
        {
            _onClick = onClick;
            OnLeftClick += Click;
        }

        private void Click(UIMouseEvent evt, UIElement listeningElement)
        {
            _onClick();
        }
    }
}
