using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace DBCHelper
{

    public class DBCParser
    {
        #region Public Properties

        public Dictionary<string, List<CANSignal>> NetworkNodeDictionary
        {
            get;
            set;
        }

        public Dictionary<uint, CANMessage> MessageDictionary
        {
            get;
            set;
        }

        public List<CANAttribute> AttributeList
        {
            get;
            set;
        }

        

        #endregion

        #region Private Field

        private const string NODE_IDENTIFIER = "BU_:";
        private const string MESSAGE_IDENTIFIER = "BO_";
        private const string COMMENT_IDENTIFIER = "CM_";
        private const string SIGNAL_IDENTIFIER = "SG_";

        private const string ATTRIBUTE_DEF_IDENTIFIER = "BA_DEF_";
        private const string ATTRIBUTE_DEF_DEFAULT_IDENTIFIER = "BA_DEF_DEF_";

        private const string NONE = "NONE";

        private string mPath;

        #endregion

        public DBCParser()
        {
            NetworkNodeDictionary = new Dictionary<string, List<CANSignal>>();
            MessageDictionary = new Dictionary<uint, CANMessage>();
            

            mPath = $"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\\dbcsamp.dbc";
        }

        public void LoadFile()
        {
            using FileStream fileStream = new FileStream(mPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using StreamReader streamReader = new StreamReader(fileStream);

            string rawLine;
            while (!streamReader.EndOfStream)
            {
                rawLine = streamReader.ReadLine();

                if (rawLine.StartsWith(NODE_IDENTIFIER))
                {
                    string[] receivedNetworkNodes = rawLine.Split(' ');
                    for (int i = 1; i < receivedNetworkNodes.Length; i++)
                    {
                        NetworkNodeDictionary.Add(receivedNetworkNodes[i], new List<CANSignal>());
                    }

                    continue;
                }

                if (rawLine.StartsWith(MESSAGE_IDENTIFIER))
                {
                    string[] receivedMessageData = rawLine.Split(' ');

                    CANMessage message = new CANMessage();
                    message.ID = uint.Parse(receivedMessageData[1]);
                    message.MessageName = receivedMessageData[2];
                    message.DLC = byte.Parse(receivedMessageData[3]);
                    message.Transmitter = receivedMessageData[4];

                    rawLine = streamReader.ReadLine();

                    while (rawLine.Contains(SIGNAL_IDENTIFIER))
                    {
                        string[] receivedSignalData = rawLine.Split(' ');

                        CANSignal signal = new CANSignal();
                        signal.ID = message.ID;
                        signal.MessageName = message.MessageName;
                        signal.Transmitter = message.Transmitter;
                        signal.SignalName = receivedSignalData[2];

                        string[] bitInfoData = receivedSignalData[4].Split(new char[] { '|', '@' });

                        signal.StartBit = uint.Parse(bitInfoData[0]);
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

                        string[] unitData = receivedSignalData[7].Split(new char[] { '"' });
                        signal.Unit = unitData[1];

                        if (receivedSignalData.Length > 9)
                        {
                            signal.ReceiverName = receivedSignalData[9];
                        }
                        else
                        {
                            signal.ReceiverName = NONE;
                        }

                        message.SignalLIst.Add(signal);

                        if(NetworkNodeDictionary.ContainsKey(signal.Transmitter))
                        {
                            NetworkNodeDictionary[signal.Transmitter].Add(signal);
                        }

                        if(NetworkNodeDictionary.ContainsKey(signal.ReceiverName))
                        {
                            NetworkNodeDictionary[signal.ReceiverName].Add(signal);
                        }

                        if(signal.ReceiverName.Contains(','))
                        {
                            string[] nodeData = signal.ReceiverName.Split(',');

                            foreach (var receiverName in nodeData)
                            {
                                if(NetworkNodeDictionary.ContainsKey(receiverName))
                                {
                                    NetworkNodeDictionary[receiverName].Add(signal);
                                }
                            }
                        }

                        rawLine = streamReader.ReadLine();
                    }

                    MessageDictionary.Add(message.ID, message);
                    

                    continue;
                }

                if (rawLine.StartsWith(COMMENT_IDENTIFIER))
                {
                    string[] commentData = rawLine.Split(' ');
                    uint tempMessageKey;

                    switch (commentData[1])
                    {
                        case "SG_":
                            tempMessageKey = uint.Parse(commentData[2]);

                            if (MessageDictionary.ContainsKey(tempMessageKey))
                            {
                                List<CANSignal> tempSignalList = MessageDictionary[tempMessageKey].SignalLIst;

                                foreach (var item in tempSignalList)
                                {
                                    if (item.SignalName == commentData[3])
                                    {
                                        string[] signalCommentData = rawLine.Split('"');
                                        item.Comment = signalCommentData[1];

                                        //CANMessage changedMessage = MessageDictionary[tempMessageKey];
                                        //changedMessage.SignalLIst = signalCommentData[1];

                                        //MessageDictionary[tempMessageKey].SignalLIst = tempSignalList;
                                    }
                                }
                            }
                            break;

                        case "BO_":
                            tempMessageKey = uint.Parse(commentData[2]);

                            if (MessageDictionary.ContainsKey(tempMessageKey))
                            {
                                string[] messageCommentData = rawLine.Split('"');
                                MessageDictionary[tempMessageKey].Comment = messageCommentData[1];
                            }
                            break;

                        default:
                            break;
                    }
                }

                if(rawLine.StartsWith(ATTRIBUTE_DEF_IDENTIFIER) && !rawLine.StartsWith(ATTRIBUTE_DEF_DEFAULT_IDENTIFIER))
                {
                    string[] attributeData = rawLine.Split(' ');
                    CANAttribute tempAttribute = new CANAttribute();

                    switch (attributeData[1])
                    {
                        case NODE_IDENTIFIER:
                            tempAttribute.AttributeType = EAttributeType.Node;
                            string[] attributeNameData = attributeData[3].Split('"');

                            tempAttribute.AttributeName = attributeNameData[1];
                            switch(attributeData[4])
                            {
                                case "HEX":
                                    tempAttribute.AttributeValueType = EAttributeValueType.Hex;
                                    break;
                                case "ENUM":
                                    tempAttribute.AttributeValueType = EAttributeValueType.Enumeration;
                                    break;
                                case "INT":
                                    tempAttribute.AttributeValueType = EAttributeValueType.Integer;
                                    break;
                                case "STRING":
                                    tempAttribute.AttributeValueType = EAttributeValueType.String;
                                    break;
                                case "FLOAT":
                                    tempAttribute.AttributeValueType = EAttributeValueType.Float;
                                    break;


                                default:
                                    break;
                            }
                            break;

                        case MESSAGE_IDENTIFIER:
                            tempAttribute.AttributeType = EAttributeType.Message;
                            break;

                        case SIGNAL_IDENTIFIER:
                            tempAttribute.AttributeType = EAttributeType.Signal;
                            break;

                        default:
                            tempAttribute.AttributeType = EAttributeType.Network;
                            break;
                    }

                }
            }



        }


    }
}
