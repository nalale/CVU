using System;
using System.Collections.Generic;
using System.Text;

namespace DC_Debug.Devices
{ 
    public interface IPowerConverter
    {
        double GetInVoltage { get; }
        double GetInCurrent { get; }
        double GetOutVoltage { get; }
        double GetOutCurrent { get; }
        int GetTemperature { get; }
        int GetTemperature2 { get; }

        int ParseIncomeMessage(int MessageID, byte[] Data);
    }
}
