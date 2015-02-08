using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RMUD;

namespace Space
{
    public class CargoBayHatch : Hatch
    {
        public override void Initialize()
        {
            Move(new ControlPanel(), this);
        }
    }
}
