using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShiftOS.Objects;
using ShiftOS.Engine;


namespace ShiftOS.WinForms
{
    public class AcquireCodepointsJobTask : JobTask
    {
        public AcquireCodepointsJobTask(int amount)
        {
            CodepointsRequired = SaveSystem.CurrentSave.Codepoints + amount;
        }

        public long CodepointsRequired { get; private set; }

        public override bool IsComplete
        {
            get
            {
                return (SaveSystem.CurrentSave.Codepoints >= CodepointsRequired);
            }
        }
    }

    public class AcquireUpgradeJobTask : JobTask
    {
        public AcquireUpgradeJobTask(string upgId)
        {
            UpgradeID = upgId;
        }

        public string UpgradeID { get; private set; }

        public override bool IsComplete
        {
            get
            {
                return Shiftorium.UpgradeInstalled(UpgradeID);
            }
        }
    }
}
