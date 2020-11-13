using DBCHelper.Extension;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace DBCHelper
{

    public class DBCParser
    {
        #region Public Properties

        public Dictionary<string, NetworkNodeCAN> NetworkNodeDictionary
        {
            get;
            private set;
        }

        public Dictionary<uint, MessageCAN> MessageDictionary
        {
            get;
            private set;
        }

        public Dictionary<string, AttributeCAN> AttributeDictionary
        {
            get;
            private set;
        }

        public Dictionary<string, ValueTableCAN> ValueTableDictionary
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
        private const string SIGNAL_VALUE_TABLE_IDENTIFIER = "VAL_";
        private const string VALUE_TABLE_IDENTIFIER = "VAL_TABLE_";

        private const string ATTRIBUTE_VALUE_IDENTIFIER = "BA_";
        private const string ATTRIBUTE_DEF_IDENTIFIER = "BA_DEF_";
        private const string ATTRIBUTE_DEF_DEFAULT_IDENTIFIER = "BA_DEF_DEF_";

        private const string ENVIRONMENT_VARIABLE_IDENTIFIER = "EV_";

        // TODO::
        private const string ATTRIBUTE_RELATIVE_IDENTIFIER = "BA_DEF_REL_";
        private const string ATTRIBUTE_DEF_REL_IDENTIFIER = "BA_DEF_DEF_REL_";

        private const string NONE = "NONE";

        private const StringComparison COMPARISON = StringComparison.OrdinalIgnoreCase;
        private readonly IFormatProvider NUMBER_FORMAT = CultureInfo.GetCultureInfo("en-US");

        private string mPath;

        #endregion

        public DBCParser()
        {
            NetworkNodeDictionary = new Dictionary<string, NetworkNodeCAN>();
            MessageDictionary = new Dictionary<uint, MessageCAN>();
            AttributeDictionary = new Dictionary<string, AttributeCAN>();
            ValueTableDictionary = new Dictionary<string, ValueTableCAN>();

            mPath = $"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\\sampmo.dbc";
        }

        public void LoadFile()
        {
            using FileStream fileStream = new FileStream(mPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using StreamReader streamReader = new StreamReader(fileStream);

            string rawLine;
            while (!streamReader.EndOfStream)
            {
                rawLine = streamReader.ReadLine();

                if (rawLine.StartsWith(NODE_IDENTIFIER, COMPARISON))
                {
                    string[] receivedNetworkNodes = rawLine.Split(' ');
                    for (int i = 1; i < receivedNetworkNodes.Length; i++)
                    {
                        NetworkNodeDictionary.Add(receivedNetworkNodes[i], new NetworkNodeCAN());
                    }

                    continue;
                }

                if (rawLine.StartsWith(MESSAGE_IDENTIFIER, COMPARISON))
                {
                    string[] receivedMessageData = rawLine.Split(' ');

                    MessageCAN message = new MessageCAN();
                    message.ID = uint.Parse(receivedMessageData[1], NUMBER_FORMAT);
                    message.MessageName = receivedMessageData[2];
                    message.DLC = byte.Parse(receivedMessageData[3], NUMBER_FORMAT);
                    message.Transmitter = receivedMessageData[4];

                    rawLine = streamReader.ReadLine();

                    while (rawLine.Contains(SIGNAL_IDENTIFIER, COMPARISON))
                    {
                        string[] receivedSignalData = rawLine.Split(' ');

                        SignalCAN signal = new SignalCAN();
                        signal.ID = message.ID;
                        signal.MessageName = message.MessageName;
                        signal.Transmitter = message.Transmitter;
                        signal.SignalName = receivedSignalData[2];

                        string[] bitInfoData = receivedSignalData[4].Split(new char[] { '|', '@' });

                        signal.StartBit = uint.Parse(bitInfoData[0], NUMBER_FORMAT);
                        signal.Length = uint.Parse(bitInfoData[1], NUMBER_FORMAT);

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
                        signal.Factor = double.Parse(offsetAndFactorData[1], NUMBER_FORMAT);
                        signal.Offset = double.Parse(offsetAndFactorData[2], NUMBER_FORMAT);

                        string[] minAndMaxData = receivedSignalData[6].Split(new char[] { '[', '|', ']' });
                        signal.Minimum = double.Parse(minAndMaxData[1], NUMBER_FORMAT);
                        signal.Maximum = double.Parse(minAndMaxData[2], NUMBER_FORMAT);

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

                        message.SignalList.Add(signal);

                        if (NetworkNodeDictionary.ContainsKey(signal.Transmitter))
                        {
                            NetworkNodeDictionary[signal.Transmitter].SignalList.Add(signal);
                        }

                        if (NetworkNodeDictionary.ContainsKey(signal.ReceiverName))
                        {
                            NetworkNodeDictionary[signal.ReceiverName].SignalList.Add(signal);
                        }

                        if (signal.ReceiverName.Contains(',', COMPARISON))
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

                if (rawLine.StartsWith(COMMENT_IDENTIFIER, COMPARISON))
                {
                    string[] commentData = rawLine.Split(' ');
                    uint tempMessageKey;

                    switch (commentData[1])
                    {
                        case "SG_":
                            tempMessageKey = uint.Parse(commentData[2], NUMBER_FORMAT);

                            if (MessageDictionary.ContainsKey(tempMessageKey))
                            {
                                List<SignalCAN> tempSignalList = (List<SignalCAN>)MessageDictionary[tempMessageKey].SignalList;

                                foreach (var item in tempSignalList)
                                {
                                    if (item.SignalName == commentData[3])
                                    {
                                        string[] signalCommentData = rawLine.Split('"');
                                        item.Comment = signalCommentData[1];

                                        //MessageCAN changedMessage = MessageDictionary[tempMessageKey];
                                        //changedMessage.SignalLIst = signalCommentData[1];

                                        //MessageDictionary[tempMessageKey].SignalLIst = tempSignalList;
                                    }
                                }
                            }
                            break;

                        case "BO_":
                            tempMessageKey = uint.Parse(commentData[2], NUMBER_FORMAT);

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

                if (rawLine.StartsWith(ATTRIBUTE_DEF_DEFAULT_IDENTIFIER, COMPARISON))
                {
                    string[] attributeDefaultData = rawLine.Split(' ');
                    string attributeName = attributeDefaultData[2].Split('"')[1];

                    if (AttributeDictionary.ContainsKey(attributeName))
                    {
                        string defaultValue = attributeDefaultData[3];

                        if (defaultValue.Contains('"', COMPARISON))
                        {
                            defaultValue = defaultValue.Split('"')[1];
                        }

                        if (defaultValue.Contains(';', COMPARISON))
                        {
                            defaultValue = defaultValue.Remove(defaultValue.Length - 1);
                        }

                        AttributeDictionary[attributeName].Default = defaultValue;
                    }

                    continue;
                }

                if (rawLine.StartsWith(ATTRIBUTE_DEF_IDENTIFIER, COMPARISON) && !rawLine.StartsWith(ATTRIBUTE_DEF_DEFAULT_IDENTIFIER, COMPARISON))
                {
                    string[] attributeData = rawLine.Split(' ');
                    AttributeCAN tempAttribute = new AttributeCAN();

                    if (rawLine.Contains(NODE_IDENTIFIER, COMPARISON) || rawLine.Contains(MESSAGE_IDENTIFIER, COMPARISON) || rawLine.Contains(SIGNAL_IDENTIFIER, COMPARISON))
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
                                tempAttribute.AttributeValueType = EAttributeValueType.HexType;
                                tempAttribute.Minimum = attributeData[5];
                                tempAttribute.Maximum = attributeData[6];
                                break;
                            case "ENUM":
                                tempAttribute.AttributeValueType = EAttributeValueType.EnumType;
                                string enumRawValue = rawLine.Split("ENUM", StringSplitOptions.None)[1];
                                string[] enumValues = enumRawValue.Split(',');

                                for (int i = 0; i < enumValues.Length; i++)
                                {
                                    tempAttribute.EnumValueList.Add(enumValues[i].Split('"')[1]);
                                }
                                break;
                            case "INT":
                                tempAttribute.AttributeValueType = EAttributeValueType.IntegerType;
                                tempAttribute.Minimum = attributeData[5];
                                tempAttribute.Maximum = attributeData[6];
                                break;
                            case "STRING":
                                tempAttribute.AttributeValueType = EAttributeValueType.StringType;
                                break;
                            case "FLOAT":
                                tempAttribute.AttributeValueType = EAttributeValueType.FloatType;
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
                                tempAttribute.AttributeValueType = EAttributeValueType.HexType;
                                tempAttribute.Minimum = attributeData[4];
                                tempAttribute.Maximum = attributeData[5];
                                break;
                            case "ENUM":
                                tempAttribute.AttributeValueType = EAttributeValueType.EnumType;
                                string enumRawValue = rawLine.Split("ENUM", StringSplitOptions.None)[1];
                                string[] enumValues = enumRawValue.Split(',');

                                for (int i = 0; i < enumValues.Length; i++)
                                {
                                    tempAttribute.EnumValueList.Add(enumValues[i].Split('"')[1]);
                                }
                                break;
                            case "INT":
                                tempAttribute.AttributeValueType = EAttributeValueType.IntegerType;
                                tempAttribute.Minimum = attributeData[4];
                                tempAttribute.Maximum = attributeData[5];
                                break;
                            case "STRING":
                                tempAttribute.AttributeValueType = EAttributeValueType.StringType;
                                break;
                            case "FLOAT":
                                tempAttribute.AttributeValueType = EAttributeValueType.FloatType;
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

                if (rawLine.StartsWith(ATTRIBUTE_VALUE_IDENTIFIER, COMPARISON))
                {
                    string[] attributeValueData = rawLine.Split(' ');

                    string attributeName = attributeValueData[1].Split('"')[1];

                    string dataValue;

                    uint messageID;

                    // Network
                    if (attributeValueData.Length < 4)
                    {
                        dataValue = attributeValueData[2];

                        if (dataValue.Contains(';', COMPARISON))
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

                        AttributeCAN tempAttribute = new AttributeCAN();

                        if (AttributeDictionary.ContainsKey(attributeName))
                        {
                            tempAttribute.CopyAttribute(AttributeDictionary[attributeName]);
                        }

                        switch (attributeValueData[2])
                        {
                            case NODE_IDENTIFIER:

                                dataValue = attributeValueData[4];

                                if (dataValue.Contains('"', COMPARISON))
                                {
                                    dataValue = dataValue.Split('"')[1];
                                }

                                if (dataValue.Contains(';', COMPARISON))
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

                                if (dataValue.Contains('"', COMPARISON))
                                {
                                    dataValue = dataValue.Split('"')[1];
                                }

                                if (dataValue.Contains(';', COMPARISON))
                                {
                                    dataValue = dataValue.Remove(dataValue.Length - 1);
                                }

                                tempAttribute.Data = dataValue;

                                messageID = uint.Parse(typeName, NUMBER_FORMAT);

                                if (MessageDictionary.ContainsKey(messageID))
                                {
                                    MessageDictionary[messageID].AttributeList.Add(tempAttribute);
                                }

                                break;


                            case SIGNAL_IDENTIFIER:

                                dataValue = attributeValueData[5];

                                if (dataValue.Contains('"', COMPARISON))
                                {
                                    dataValue = dataValue.Split('"')[1];
                                }

                                if (dataValue.Contains(';', COMPARISON))
                                {
                                    dataValue = dataValue.Remove(dataValue.Length - 1);
                                }

                                tempAttribute.Data = dataValue;

                                messageID = uint.Parse(typeName, NUMBER_FORMAT);

                                if (MessageDictionary.ContainsKey(messageID))
                                {
                                    string tempSignalName = attributeValueData[4];

                                    foreach (var signal in MessageDictionary[messageID].SignalList)
                                    {
                                        if (signal.SignalName == tempSignalName)
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

                if (rawLine.StartsWith(VALUE_TABLE_IDENTIFIER, COMPARISON))
                {
                    string[] valueTableData = rawLine.Split(' ');
                    string valueTableName = valueTableData[1];

                    ValueTableCAN tempValueTable = new ValueTableCAN();
                    tempValueTable.ValueTableName = valueTableName;

                    string valueTableStr = rawLine.Split(valueTableData[1], StringSplitOptions.RemoveEmptyEntries)[1];
                    string[] decimalAndDescriptionArr = valueTableStr.Split(new char[] { '"', ';' }, StringSplitOptions.RemoveEmptyEntries);

                    int rawDecimal;
                    string description;

                    for (int i = 0; i < decimalAndDescriptionArr.Length - 1; i += 2)
                    {
                        rawDecimal = int.Parse(decimalAndDescriptionArr[i], NUMBER_FORMAT);
                        description = decimalAndDescriptionArr[i + 1];

                        tempValueTable.ValueMap.Add(rawDecimal, description);
                    }

                    ValueTableDictionary.Add(valueTableName, tempValueTable);

                    continue;
                }

                if (rawLine.StartsWith(SIGNAL_VALUE_TABLE_IDENTIFIER, COMPARISON) && !rawLine.StartsWith(VALUE_TABLE_IDENTIFIER, COMPARISON))
                {
                    string[] valueTableData = rawLine.Split(' ');

                    uint messageID = uint.Parse(valueTableData[1], NUMBER_FORMAT);
                    string tempSignalName = valueTableData[2];

                    if (MessageDictionary.ContainsKey(messageID))
                    {
                        foreach (var signal in MessageDictionary[messageID].SignalList)
                        {
                            if (signal.SignalName == tempSignalName)
                            {
                                ValueTableCAN signalValueTable = new ValueTableCAN();
                                signalValueTable.ValueTableName = $"VtSig_{tempSignalName}";

                                string valueTableStr = rawLine.Split(valueTableData[2], StringSplitOptions.RemoveEmptyEntries)[1];
                                string[] decimalAndDescriptionArr = valueTableStr.Split(new char[] { '"', ';' }, StringSplitOptions.RemoveEmptyEntries);

                                int rawDecimal;
                                string description;

                                for (int i = 0; i < decimalAndDescriptionArr.Length - 1; i += 2)
                                {
                                    rawDecimal = int.Parse(decimalAndDescriptionArr[i], NUMBER_FORMAT);
                                    description = decimalAndDescriptionArr[i + 1];

                                    signalValueTable.ValueMap.Add(rawDecimal, description);
                                }

                                signal.ValueTable = signalValueTable;
                                ValueTableDictionary.Add(signalValueTable.ValueTableName, signalValueTable);

                                break;
                            }
                        }
                    }

                    continue;
                }

                if (rawLine.StartsWith(ENVIRONMENT_VARIABLE_IDENTIFIER, COMPARISON))
                {
                    // EV_ (Env Variable Name) : (Env Variable Type) [ (Minimum) | (Maximum) ] (Unit) (initial Value) (Ev ID) (Access Type) (Access node)
                    string[] splitData = new string[] { ":" };
                    string[] values = rawLine.Split(splitData, StringSplitOptions.RemoveEmptyEntries);
                    EnvironmentVariableCAN envInfo = new EnvironmentVariableCAN();
                    envInfo.Name = values[0].Replace(ENVIRONMENT_VARIABLE_IDENTIFIER, "", COMPARISON).Trim();
                    splitData = new string[] { "|", "@", "(", ",", ")", "[", "]", " " };
                    string[] values2 = values[1].Split(splitData, StringSplitOptions.RemoveEmptyEntries);

                    switch(values2[0])
                    {
                        case "0": envInfo.VariableType = EVariableType.IntegerType; break;
                        case "1": envInfo.VariableType = EVariableType.FloatType; break;
                        case "2": envInfo.VariableType = EVariableType.StringType; break;
                        default: envInfo.VariableType = EVariableType.StringType; break;
                    }
                    // 0 [0|1] \"\" 0 1 DUMMY_NODE_VECTOR0 Vector__XXX;
                    envInfo.Minimum = values2[1];
                    envInfo.Maximum = values2[2];
                    envInfo.Unit = values2[3].Replace("\"", "", COMPARISON);
                    envInfo.InitialValue = values2[4];
                    envInfo.ID = uint.Parse(values2[5], NUMBER_FORMAT);

                    switch (values2[6])
                    {
                        case "0": envInfo.AccessType = EAccessType.Unrestricted; break;
                        case "1": envInfo.AccessType = EAccessType.Read; break;
                        case "2": envInfo.AccessType = EAccessType.Write; break;
                        case "3": envInfo.AccessType = EAccessType.ReadWrite; break;
                        default: envInfo.AccessType = EAccessType.Unrestricted; break;
                    }

                    //envInfo.AccessType = values2[6];
                    envInfo.AccessNode = values2[7].Replace(";", "", COMPARISON);
                }
            }
        }



    }


}


