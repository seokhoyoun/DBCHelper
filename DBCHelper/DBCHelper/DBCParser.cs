using System;
using System.Collections.Generic;
using System.Diagnostics;
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
                    string[] receivedMessageData = rawLine.Split(' ');

                    CANMessage message = new CANMessage();
                    message.ID = uint.Parse(receivedMessageData[1]);
                    message.MessageName = receivedMessageData[2];
                    message.DLC = byte.Parse(receivedMessageData[3]);
                    message.Transmitter = receivedMessageData[4];

                    rawLine = streamReader.ReadLine();

                    while(rawLine.StartsWith(" SG_"))
                    {
                        string[] receivedSignalData = rawLine.Split(' ');

                        CANSignal signal = new CANSignal();
                        signal.ID = message.ID;
                        signal.MessageName = message.MessageName;
                        signal.SignalName = receivedSignalData[2];

                        string[] bitInfoData  = receivedSignalData[4].Split(new char[] { '|', '@' });

                        signal.StartBit = uint.Parse( bitInfoData[0]);
                        signal.Length = uint.Parse(bitInfoData[1]);

                        switch (bitInfoData[2])
                        {
                            case "0-":
                                signal.ByteOrder = EByteOrder.Motorola;
                                signal.ValueType = EValueType.SignedType;
                                break;

                            case "0+":
                                signal.ByteOrder = EByteOrder.Motorola;
                                signal.ValueType = EValueType.UnsignedType;
                                break;

                            case "1-":
                                signal.ByteOrder = EByteOrder.Intel;
                                signal.ValueType = EValueType.SignedType;
                                break;

                            case "1+":
                                signal.ByteOrder = EByteOrder.Intel;
                                signal.ValueType = EValueType.UnsignedType;
                                break;

                            default:
                                Debug.Assert(false);
                                break;
                        }

                        string[] offsetAndFactorData = receivedSignalData[5].Split(new char[] { '(', ',', ')' });
                        signal.Factor = double.Parse(offsetAndFactorData[1]);
                        signal.Offset = double.Parse(offsetAndFactorData[2]);

                        string[] minAndMaxData = receivedSignalData[6].Split(new char[] { '[', '|', ']' });
                        signal.Minimum = double.Parse(minAndMaxData[1]);
                        signal.Maximum = double.Parse(minAndMaxData[2]);

                        string[] unitData = receivedSignalData[7].Split(new char[] { '"'});
                        signal.Unit = unitData[1];

                        if(receivedSignalData.Length > 9)
                        {
                            signal.ReceiverName = receivedSignalData[9];
                        }

                        message.SignalLIst.Add(signal);

                        rawLine = streamReader.ReadLine();
                    }

                    MessageDictionary.Add(message.ID, message);

                    
                }
            }

            //string[] splitedByID = rawText.Split("BO_", StringSplitOptions.RemoveEmptyEntries);

            

        }


    }
}
