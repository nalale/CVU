using System;
using System.Collections.Generic;
using System.Text;

namespace DC_Debug.DataBase
{

    interface IDataBase
    {
        double GetParameter(int Device, string Parameter);
        int UpdateParameter(int Device, string Parameter, double Value);
        int UpdateMessage(int Device, int MsgID, List<Tuple<string, double>> Parameters);
    }
}
