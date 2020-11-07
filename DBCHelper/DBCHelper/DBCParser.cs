using DBCHelper.Extension;
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

        public Dictionary<string, CANNetworkNode> NetworkNodeDictionary
        {
            get;
            private set;
        }

        public Dictionary<uint, CANMessage> MessageDictionary
        {
            get;
            private set;
        }

        public Dictionary<string, CANAttribute> AttributeDictionary
        {
            get;
            private set;
        }





        #endregion

        #region Private Field

        private const string NODE_IDENTIFIER = "BU_";
        private const string MESSAGE_IDENTIFIER = "BO_";
        private const string COMMENT_IDENTIFIER = "CM_";
        private const string SIGNAL_IDENTIFIER = "SG_";
        private const string VALUE_TABLE_IDENTIFIER = "VAL_";

        private const string ATTRIBUTE_VALUE_IDENTIFIER = "BA_";
        private const string ATTRIBUTE_DEF_IDENTIFIER = "BA_DEF_";
        private const string ATTRIBUTE_DEF_DEFAULT_IDENTIFIER = "BA_DEF_DEF_";

        private const string NONE = "NONE";

        private string mPath;

        #endregion

        public DBCParser()
        {
            NetworkNodeDictionary = new Dictionary<string, CANNetworkNode>();
            MessageDictionary = new Dictionary<uint, CANMessage>();
            AttributeDictionary = new Dictionary<string, CANAttribute>();

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
                        NetworkNodeDictionary.Add(receivedNetworkNodes[i], new CANNetworkNode());
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

                        if (NetworkNodeDictionary.ContainsKey(signal.Transmitter))
                        {
                            NetworkNodeDictionary[signal.Transmitter].SignalList.Add(signal);
                        }

                        if (NetworkNodeDictionary.ContainsKey(signal.ReceiverName))
                        {
                            NetworkNodeDictionary[signal.ReceiverName].SignalList.Add(signal);
                        }

                        if (signal.ReceiverName.Contains(','))
                        {
                            string[] nodeData = signal.ReceiverName.Split(',');

                            foreach (var receiverName in nodeData)
                            {
                                if (NetworkNodeDictionary.ContainsKey(receiverName))
                                {
                                    NetworkNodeDictionary[receiverName].SignalList.Add(signal);
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

                if (rawLine.StartsWith(ATTRIBUTE_DEF_IDENTIFIER) && !rawLine.StartsWith(ATTRIBUTE_DEF_DEFAULT_IDENTIFIER))
                {
                    string[] attributeData = rawLine.Split(' ');
                    CANAttribute tempAttribute = new CANAttribute();

                    if (rawLine.Contains(NODE_IDENTIFIER) || rawLine.Contains(MESSAGE_IDENTIFIER) || rawLine.Contains(SIGNAL_IDENTIFIER))
                    {
                        switch (attributeData[1])
                        {
                            case NODE_IDENTIFIER:
                                tempAttribute.AttributeType = EAttributeType.Node;
                                break;

                            case MESSAGE_IDENTIFIER:
                                tempAttribute.AttributeType = EAttributeType.Message;
                                break;

                            case SIGNAL_IDENTIFIER:
                                tempAttribute.AttributeType = EAttributeType.Signal;
                                break;

                            default:
                                Debug.Assert(false);
                                break;
                        }

                        tempAttribute.AttributeName = attributeData[3].Split('"')[1];

                        switch (attributeData[4])
                        {
                            case "HEX":
                                tempAttribute.AttributeValueType = EAttributeValueType.Hex;
                                tempAttribute.Minimum = attributeData[5];
                                tempAttribute.Maximum = attributeData[6];
                                break;
                            case "ENUM":
                                tempAttribute.AttributeValueType = EAttributeValueType.Enumeration;
                                string enumRawValue = rawLine.Split("ENUM", StringSplitOptions.None)[1];
                                string[] enumValues = enumRawValue.Split(',');

                                for (int i = 0; i < enumValues.Length; i++)
                                {
                                    tempAttribute.EnumValueList.Add(enumValues[i].Split('"')[1]);
                                }
                                break;
                            case "INT":
                                tempAttribute.AttributeValueType = EAttributeValueType.Integer;
                                tempAttribute.Minimum = attributeData[5];
                                tempAttribute.Maximum = attributeData[6];
                                break;
                            case "STRING":
                                tempAttribute.AttributeValueType = EAttributeValueType.String;
                                break;
                            case "FLOAT":
                                tempAttribute.AttributeValueType = EAttributeValueType.Float;
                                tempAttribute.Minimum = attributeData[5];
                                tempAttribute.Maximum = attributeData[6];
                                break;

                            default:
                                Debug.Assert(false);
                                break;
                        }
                    }
                    else
                    {
                        tempAttribute.AttributeType = EAttributeType.Network;
                        tempAttribute.AttributeName = attributeData[2].Split('"')[1];

                        switch (attributeData[3])
                        {
                            case "HEX":
                                tempAttribute.AttributeValueType = EAttributeValueType.Hex;
                                tempAttribute.Minimum = attributeData[4];
                                tempAttribute.Maximum = attributeData[5];
                                break;
                            case "ENUM":
                                tempAttribute.AttributeValueType = EAttributeValueType.Enumeration;
                                string enumRawValue = rawLine.Split("ENUM", StringSplitOptions.None)[1];
                                string[] enumValues = enumRawValue.Split(',');

                                for (int i = 0; i < enumValues.Length; i++)
                                {
                                    tempAttribute.EnumValueList.Add(enumValues[i].Split('"')[1]);
                                }
                                break;
                            case "INT":
                                tempAttribute.AttributeValueType = EAttributeValueType.Integer;
                                tempAttribute.Minimum = attributeData[4];
                                tempAttribute.Maximum = attributeData[5];
                                break;
                            case "STRING":
                                tempAttribute.AttributeValueType = EAttributeValueType.String;
                                break;
                            case "FLOAT":
                                tempAttribute.AttributeValueType = EAttributeValueType.Float;
                                tempAttribute.Minimum = attributeData[4];
                                tempAttribute.Maximum = attributeData[5];
                                break;

                            default:
                                Debug.Assert(false);
                                break;
                        }
                    }

                    AttributeDictionary.Add(tempAttribute.AttributeName, tempAttribute);

                    continue;
                }

                if (rawLine.StartsWith(ATTRIBUTE_DEF_DEFAULT_IDENTIFIER))
                {
                    string[] attributeDefaultData = rawLine.Split(' ');
                    string attributeName = attributeDefaultData[2].Split('"')[1];

                    if (AttributeDictionary.ContainsKey(attributeName))
                    {
                        string defaultValue = attributeDefaultData[3];

                        if (defaultValue.Contains('"'))
                        {
                            defaultValue = defaultValue.Split('"')[1];
                        }

                        if (defaultValue.Contains(';'))
                        {
                            defaultValue = defaultValue.Remove(defaultValue.Length - 1);
                        }

                        AttributeDictionary[attributeName].Default = defaultValue;
                    }

                    continue;
                }

                if (rawLine.StartsWith(ATTRIBUTE_VALUE_IDENTIFIER))
                {
                    string[] attributeValueData = rawLine.Split(' ');

                    string attributeName = attributeValueData[1].Split('"')[1];

                    string dataValue;

                    uint messageID;

                    // Network
                    if (attributeValueData.Length < 4)
                    {
                        dataValue = attributeValueData[2];

                        if (dataValue.Contains(';'))
                        {
                            dataValue = dataValue.Remove(dataValue.Length - 1);
                        }

                        if (AttributeDictionary.ContainsKey(attributeName))
                        {
                            AttributeDictionary[attributeName].Data = dataValue;
                        }
                    }
                    else
                    {
                        string typeName = attributeValueData[3];

                        CANAttribute tempAttribute = new CANAttribute();

                        if (AttributeDictionary.ContainsKey(attributeName))
                        {
                            tempAttribute.CopyAttribute(AttributeDictionary[attributeName]);
                        }

                        switch (attributeValueData[2])
                        {
                            case NODE_IDENTIFIER:

                                dataValue = attributeValueData[4];

                                if (dataValue.Contains('"'))
                                {
                                    dataValue = dataValue.Split('"')[1];
                                }

                                if (dataValue.Contains(';'))
                                {
                                    dataValue = dataValue.Remove(dataValue.Length - 1);
                                }

                                tempAttribute.Data = dataValue;

                                if (NetworkNodeDictionary.ContainsKey(typeName))
                                {
                                    NetworkNodeDictionary[typeName].AttributeList.Add(tempAttribute);
                                }

                                break;

                            case MESSAGE_IDENTIFIER:

                                dataValue = attributeValueData[4];

                                if (dataValue.Contains('"'))
                                {
                                    dataValue = dataValue.Split('"')[1];
                                }

                                if (dataValue.Contains(';'))
                                {
                                    dataValue = dataValue.Remove(dataValue.Length - 1);
                                }

                                tempAttribute.Data = dataValue;

                                messageID = uint.Parse(typeName);

                                if(MessageDictionary.ContainsKey(messageID))
                                {
                                    MessageDictionary[messageID].AttributeList.Add(tempAttribute);
                                }

                                break;


                            case SIGNAL_IDENTIFIER:

                                dataValue = attributeValueData[5];

                                if (dataValue.Contains('"'))
                                {
                                    dataValue = dataValue.Split('"')[1];
                                }

                                if (dataValue.Contains(';'))
                                {
                                    dataValue = dataValue.Remove(dataValue.Length - 1);
                                }

                                tempAttribute.Data = dataValue;

                                messageID = uint.Parse(typeName);

                                if (MessageDictionary.ContainsKey(messageID))
                                {
                                    string tempSignalName = attributeValueData[4];

                                    foreach (var signal in MessageDictionary[messageID].SignalLIst)
                                    {
                                        if(signal.SignalName == tempSignalName)
                                        {
                                            signal.AttributeList.Add(tempAttribute);

                                            break;
                                        }
                                    } 
                                }

                                break;
                        }

                    }

                    continue;
                }

                if(rawLine.StartsWith(VALUE_TABLE_IDENTIFIER))
                {
                    string[] valueTableData = rawLine.Split(' ');

                    uint messageID = uint.Parse(valueTableData[1]);
                    string tempSignalName = valueTableData[2];

                    if(MessageDictionary.ContainsKey(messageID))
                    {
                        foreach (var signal in MessageDictionary[messageID].SignalLIst)
                        {
                            if(signal.SignalName == tempSignalName)
                            {
                                string physicAndDescPart = rawLine.Split(valueTableData[2], StringSplitOptions.RemoveEmptyEntries)[1];
                                string[] tempPhysicAndDescArr = physicAndDescPart.Split(new char[] { '"',';' }, StringSplitOptions.RemoveEmptyEntries);

                                int physicalDecimal;
                                string description;

                                for (int i = 0; i < tempPhysicAndDescArr.Length -1; i += 2)
                                {
                                    physicalDecimal = int.Parse(tempPhysicAndDescArr[i]);
                                    description = tempPhysicAndDescArr[i + 1];

                                    //signal.ValueTableList.Add(new Tuple<int, string>(physicalDecimal, description));
                                }

                                break;
                            }
                        }  
                    }

                }
            }
        }



    }


}


