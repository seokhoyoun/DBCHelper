using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DBCHelper
{

    public class DBCParser
    {
        #region Public Properties

        public Dictionary<string, Dictionary<uint,CANMessage>> NetworkNodeDictionary
        {
            get;
            set;
        }

        public Dictionary<uint,CANMessage> MessageDictionary
        {
            get;
            set;
        }

        #endregion

        #region Private Field

        private string mPath;

        #endregion

        public DBCParser()
        {
            NetworkNodeDictionary = new Dictionary<string, Dictionary<uint,CANMessage>>();
            MessageDictionary = new Dictionary<uint, CANMessage>();
                
            mPath = $"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\\dbcsamp.dbc";
        }

        public void LoadFile()
        {
            using FileStream fileStream = new FileStream(mPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using StreamReader streamReader = new StreamReader(fileStream);

            string rawLine; 
            while(!streamReader.EndOfStream)
            {
                rawLine = streamReader.ReadLine();
                if (rawLine.StartsWith("BU_:"))
                {
                    string[] receivedNetworkNodes = rawLine.Split(' ');
                    for (int i = 1; i < receivedNetworkNodes.Length; i++)
                    {
                        NetworkNodeDictionary.Add(receivedNetworkNodes[i], new Dictionary<uint, CANMessage>());
                    }

                    continue;
                }

                if(rawLine.StartsWith("BO_"))
                {
                    string[] receivedMessageInfo = rawLine.Split(' ');

                    CANMessage message = new CANMessage();
                    message.ID = uint.Parse(receivedMessageInfo[1]);
                    message.MessageName = receivedMessageInfo[2].Remove(receivedMessageInfo.Length - 1, 1);
                    message.DLC = byte.Parse(receivedMessageInfo[3]);
                }
            }

            //string[] splitedByID = rawText.Split("BO_", StringSplitOptions.RemoveEmptyEntries);

            

        }


    }
}
