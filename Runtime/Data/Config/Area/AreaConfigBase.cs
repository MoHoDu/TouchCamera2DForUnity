using System.Collections.Generic;
using CameraBehaviour.DataLayer.Config.Area.Interface;
using CameraBehaviour.DataLayer.Input;

namespace CameraBehaviour.DataLayer.Config.Area
{
    public abstract class AreaConfigBase : ConfigBase, IAreaConfig
    {
        public override void Validate(List<string> warnings)
        {
            base.Validate(warnings);
        }

        public abstract void Calibrate(InputContext context);
    }
}