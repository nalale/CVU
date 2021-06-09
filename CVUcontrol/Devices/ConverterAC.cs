using System;
using System.Collections.Generic;
using System.Text;

namespace DC_Debug.Devices
{
    class ConverterAC : IPowerConverter
    {
        double IPowerConverter.GetInVoltage => throw new NotImplementedException();

        double IPowerConverter.GetInCurrent => throw new NotImplementedException();

        double IPowerConverter.GetOutVoltage => throw new NotImplementedException();

        double IPowerConverter.GetOutCurrent => throw new NotImplementedException();

        int IPowerConverter.GetTemperature => throw new NotImplementedException();

        int IPowerConverter.GetTemperature2 => throw new NotImplementedException();

        int IPowerConverter.ParseIncomeMessage(int MessageID, byte[] Data)
        {
            switch(MessageID)
            {
                case 0x180:
                    break;

                case 0x280:
                    break;

                case 0x380:
                    break;

                case 0x480:
                    break;
                default:
                    return 1;
            }

            return 0;
        }
    }
}
