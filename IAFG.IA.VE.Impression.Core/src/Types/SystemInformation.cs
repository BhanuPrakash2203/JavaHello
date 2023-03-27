using System;

namespace IAFG.IA.VE.Impression.Core.Types
{
    public class SystemInformation : ISystemInformation
    {
        public DateTime CurrentDate
        {
            get
            {
                return DateTime.Now;
            }
        }
    }
}