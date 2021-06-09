using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace DC_Debug.DataBase
{
    class DataBaseMessages : IDataBase
    {
        class DataBaseNoteDevice
        {
            public string Name;
            public int Code;
            public List<int> MessageIDs;
            public Dictionary<string, double> Parameters;
        }

        class DataBaseNoteMessage
        {
            public string Name;            
            public int Id;
            public int DeviceCode;
            public Dictionary<string, double> Parameters;
        }

        class DataBaseNoteParameter
        {
            public string Name;
            public double Value;
            public int MessageId;
            public int DeviceCode;
        }

        List<DataBaseNoteParameter> database = new List<DataBaseNoteParameter>();

        public DataBaseMessages()
        {

        }


        double IDataBase.GetParameter(int Device, string Parameter)
        {
            var note = FindParameterNote(Device, Parameter);

            if (note != null)
                return note.Value;
            else
                return 0;
        }

        public int UpdateParameter(int Device, string Parameter, double Value)
        {
            var note = FindParameterNote( Device, Parameter);

            if(note == null)
                database.Add(new DataBaseNoteParameter() { DeviceCode = Device, Name = Parameter, Value = Value });
            else
                note.Value = Value;

            return 1;                
        }

        public int UpdateMessage(int Device, int MsgID, List<Tuple<string, double>> Parameters)
        {
            // Go through messages parameter list
            foreach (var parameter in Parameters)
            {
                UpdateParameter(Device, parameter.Item1, parameter.Item2);
            }

            return 1;
        }

        DataBaseNoteParameter FindParameterNote(int Device, string Parameter)
        {
            // Find all notes with requested DeviceCode
            var dev_notes = database.FindAll(note => note.DeviceCode == Device);

            // If none return null            
            if (dev_notes.Count == 0)
                return null;

            // otherwise find requested Parameter
            var par_note = dev_notes.Find(note => note.Name == Parameter);

            return par_note;
        }
    }    
}
