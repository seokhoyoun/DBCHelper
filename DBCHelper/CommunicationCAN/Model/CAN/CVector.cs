using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using vxlapi_NET;

namespace DBCHelper
{
    public class CVector
    {
        #region .dll

        // -----------------------------------------------------------------------------------------------
        // DLL Import for RX events
        // -----------------------------------------------------------------------------------------------
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern int WaitForSingleObject(int handle, int timeOut);
        // -----------------------------------------------------------------------------------------------
        #endregion

        #region Public Properties
        public Dictionary<uint, string> CanMsgMap
        {
            get;
            private set;
        }



      
        private int _time = 0;
        public int Time
        {
            get { return _time; }
            set { _time = value; }
        }

        #endregion

        #region Field

   
        private XLDriver CANDemo = new XLDriver();
        private String appName = "EOL";

        // Driver configuration
        private XLClass.xl_driver_config driverConfig = new XLClass.xl_driver_config();

        // Variables required by XLDriver
        private XLDefine.XL_HardwareType hwType = XLDefine.XL_HardwareType.XL_HWTYPE_NONE;
        private uint hwIndex = 0;
        private uint hwChannel = 0;
        private int fdPortHandle = -1;
        private int portHandle = -1;
        private int eventHandle = -1;
        private UInt64 accessMask = 0;
        private UInt64 accessMaskFD = 0;
        private UInt64 permissionMask = 0;



        public static UInt64 CH1_MASK = 0; // INT CAN FD
        public static UInt64 CH2_MASK = 0; // EPT CAN
        public static UInt64 CH3_MASK = 0; // VCAN1 FD
        public static UInt64 CH4_MASK = 0; // VCAN2 FD

        private int ch1CI = 0;
        private int ch2CI = 0;
        private int ch3CI = 0;
        private int ch4CI = 0;

        private static EventWaitHandle xlEvWaitHandle = new EventWaitHandle(false, EventResetMode.AutoReset, null);

        private uint canFdModeNoIso = 0;      // Global CAN FD ISO (default) / no ISO mode flag

        // RX thread
        private Thread rxThread;
        private bool blockRxThread = false;

        private Thread txThread;

        #endregion

        #region Constructor

        public CVector()
        {
            
            CanMsgMap = new Dictionary<uint, string>();

            InitCANFD();
            InitCAN();

           

            StartReceiveThread();

            // Tx thread
        }
        #endregion

        #region Public Method

        public void InitCANFD()
        {
            XLDefine.XL_Status status;

            Console.WriteLine("-------------------------------------------------------------------");
            Console.WriteLine("                    xlCANFDdemo.NET C# V11.0                       ");
            Console.WriteLine("Copyright (c) 2019 by Vector Informatik GmbH.  All rights reserved.");
            Console.WriteLine("-------------------------------------------------------------------\n");

            // print .NET wrapper version
            Console.WriteLine("vxlapi_NET        : " + typeof(XLDriver).Assembly.GetName().Version);

            // Open XL Driver
            status = CANDemo.XL_OpenDriver();
            Console.WriteLine("Open Driver       : " + status);
            if (status != XLDefine.XL_Status.XL_SUCCESS)
                return;


            // Get XL Driver configuration
            status = CANDemo.XL_GetDriverConfig(ref driverConfig);
            Console.WriteLine("Get Driver Config : " + status);
            if (status != XLDefine.XL_Status.XL_SUCCESS)
                return;

            // Convert the dll version number into a readable string
            Console.WriteLine("DLL Version       : " + CANDemo.VersionToString(driverConfig.dllVersion));

            // Display channel count
            Console.WriteLine("Channels found    : " + driverConfig.channelCount);


           

            // If the application name cannot be found in VCANCONF...
            if (
                (CANDemo.XL_GetApplConfig(appName, 0, ref hwType, ref hwIndex, ref hwChannel, XLDefine.XL_BusTypes.XL_BUS_TYPE_CAN) != XLDefine.XL_Status.XL_SUCCESS) ||
                (CANDemo.XL_GetApplConfig(appName, 2, ref hwType, ref hwIndex, ref hwChannel, XLDefine.XL_BusTypes.XL_BUS_TYPE_CAN) != XLDefine.XL_Status.XL_SUCCESS) ||
                (CANDemo.XL_GetApplConfig(appName, 3, ref hwType, ref hwIndex, ref hwChannel, XLDefine.XL_BusTypes.XL_BUS_TYPE_CAN) != XLDefine.XL_Status.XL_SUCCESS)
                )
            {
                //...create the item with two CAN channels
                CANDemo.XL_SetApplConfig(appName, 0, XLDefine.XL_HardwareType.XL_HWTYPE_NONE, 0, 0, XLDefine.XL_BusTypes.XL_BUS_TYPE_CAN);
                CANDemo.XL_SetApplConfig(appName, 2, XLDefine.XL_HardwareType.XL_HWTYPE_NONE, 0, 0, XLDefine.XL_BusTypes.XL_BUS_TYPE_CAN);
                CANDemo.XL_SetApplConfig(appName, 3, XLDefine.XL_HardwareType.XL_HWTYPE_NONE, 0, 0, XLDefine.XL_BusTypes.XL_BUS_TYPE_CAN);
                PrintAssignErrorAndPopupHwConf();
            }

            // Request the user to assign channels until both CAN1 (Tx) and CAN2 (Rx) are assigned to usable channels
            while (!GetAppChannelAndTestIsOk(0, ref CH1_MASK, ref ch1CI) || !GetAppChannelAndTestIsOk(2, ref CH3_MASK, ref ch3CI) || !GetAppChannelAndTestIsOk(3, ref CH4_MASK, ref ch4CI))
            {
                PrintAssignErrorAndPopupHwConf();
                return;
            }

            PrintConfig(new int[] {ch1CI, ch3CI, ch4CI });

            accessMaskFD = CH1_MASK | CH3_MASK | CH4_MASK;
            permissionMask = accessMaskFD;

            // --------------------
            // Open port
            // --------------------
            status = CANDemo.XL_OpenPort(ref fdPortHandle, appName, accessMaskFD, ref permissionMask, 16000, XLDefine.XL_InterfaceVersion.XL_INTERFACE_VERSION_V4, XLDefine.XL_BusTypes.XL_BUS_TYPE_CAN);
            Console.WriteLine("\n\nOpen Port             : " + status);
            if (status != XLDefine.XL_Status.XL_SUCCESS)
            {
                return;
            }

            // --------------------
            // Set CAN FD config
            // --------------------
            XLClass.XLcanFdConf canFdConf = new XLClass.XLcanFdConf();

            // arbitration bitrate
            canFdConf.arbitrationBitRate = 1000000;
            canFdConf.tseg1Abr = 6;
            canFdConf.tseg2Abr = 3;
            canFdConf.sjwAbr = 2;

            // data bitrate
            canFdConf.dataBitRate = canFdConf.arbitrationBitRate * 2;
            canFdConf.tseg1Dbr = 6;
            canFdConf.tseg2Dbr = 3;
            canFdConf.sjwDbr = 2;

            if (canFdModeNoIso > 0)
            {
                canFdConf.options = (byte)XLDefine.XL_CANFD_ConfigOptions.XL_CANFD_CONFOPT_NO_ISO;
            }
            else
            {
                canFdConf.options = 0;
            }

            status = CANDemo.XL_CanFdSetConfiguration(fdPortHandle, accessMaskFD, canFdConf);
            Console.WriteLine("\n\nSet CAN FD Config     : " + status);
            if (status != XLDefine.XL_Status.XL_SUCCESS)
            {
                return;
            }


            // Get RX event handle
            status = CANDemo.XL_SetNotification(fdPortHandle, ref eventHandle, 1);
            Console.WriteLine("Set Notification      : " + status);
            if (status != XLDefine.XL_Status.XL_SUCCESS)
            {
                return;
            }

            // Activate channel - with reset clock
            status = CANDemo.XL_ActivateChannel(fdPortHandle, accessMaskFD, XLDefine.XL_BusTypes.XL_BUS_TYPE_CAN, XLDefine.XL_AC_Flags.XL_ACTIVATE_RESET_CLOCK);
            Console.WriteLine("Activate Channel      : " + status);
            if (status != XLDefine.XL_Status.XL_SUCCESS)
            {
                return;
            }

            // Get XL Driver configuration to get the actual setup parameter
            status = CANDemo.XL_GetDriverConfig(ref driverConfig);
            if (status != XLDefine.XL_Status.XL_SUCCESS)
            {
                return;
            }

            if (canFdModeNoIso > 0)
            {
                Console.WriteLine("CAN FD mode           : NO ISO");
            }
            else
            {
                Console.WriteLine("CAN FD mode           : ISO");
            }
            Console.WriteLine("TX Arb. BitRate       : " + driverConfig.channel[ch1CI].busParams.dataCanFd.arbitrationBitRate
                            + "Bd, Data Bitrate: " + driverConfig.channel[ch1CI].busParams.dataCanFd.dataBitRate + "Bd");

            
        }

        public void InitCAN()
        {
            XLDefine.XL_Status status;

            // If the application name cannot be found in VCANCONF...
            if ((CANDemo.XL_GetApplConfig(appName, 1, ref hwType, ref hwIndex, ref hwChannel, XLDefine.XL_BusTypes.XL_BUS_TYPE_CAN) != XLDefine.XL_Status.XL_SUCCESS) )
            {
                //...create the item with two CAN channels
                CANDemo.XL_SetApplConfig(appName, 1, XLDefine.XL_HardwareType.XL_HWTYPE_NONE, 0, 0, XLDefine.XL_BusTypes.XL_BUS_TYPE_CAN);
                PrintAssignErrorAndPopupHwConf();
            }

            // Request the user to assign channels until both CAN1 (Tx) and CAN2 (Rx) are assigned to usable channels
            while (!GetAppChannelAndTestIsOk(1, ref CH2_MASK, ref ch2CI))
            {
                PrintAssignErrorAndPopupHwConf();
                return;
            }

            PrintConfig(new int[] { ch2CI });

            accessMask = CH2_MASK;
            permissionMask = accessMask;

            // Open port
            status = CANDemo.XL_OpenPort(ref portHandle, appName, accessMask, ref permissionMask, 1024, XLDefine.XL_InterfaceVersion.XL_INTERFACE_VERSION, XLDefine.XL_BusTypes.XL_BUS_TYPE_CAN);
            Console.WriteLine("\n\nOpen Port             : " + status);
            if (status != XLDefine.XL_Status.XL_SUCCESS)
            {
                return;
            }

            // Check port
            status = CANDemo.XL_CanRequestChipState(portHandle, accessMask);
            Console.WriteLine("Can Request Chip State: " + status);
            if (status != XLDefine.XL_Status.XL_SUCCESS)
            {
                return;
            }

            // Activate channel
            status = CANDemo.XL_ActivateChannel(portHandle, accessMask, XLDefine.XL_BusTypes.XL_BUS_TYPE_CAN, XLDefine.XL_AC_Flags.XL_ACTIVATE_NONE);
            Console.WriteLine("Activate Channel      : " + status);
            if (status != XLDefine.XL_Status.XL_SUCCESS)
            {
                return;
            }

            // Initialize EventWaitHandle object with RX event handle provided by DLL
            int tempInt = -1;
            status = CANDemo.XL_SetNotification(portHandle, ref tempInt, 1);
            xlEvWaitHandle.SafeWaitHandle = new SafeWaitHandle(new IntPtr(tempInt), true);

            Console.WriteLine("Set Notification      : " + status);
            if (status != XLDefine.XL_Status.XL_SUCCESS)
            {
                return;
            }

            // Reset time stamp clock
            status = CANDemo.XL_ResetClock(portHandle);
            Console.WriteLine("Reset Clock           : " + status + "\n\n");
            if (status != XLDefine.XL_Status.XL_SUCCESS)
            {
                return;
            }

          
        }

        public void Dispose()
        {
            rxThread.Abort();
            txThread.Abort();

            rxThread = null;
            txThread = null;

            CanMsgMap.Clear();

            CANDemo.XL_ClosePort(fdPortHandle);
            CANDemo.XL_ClosePort(portHandle);
        }

        public void Reconnect()
        {
            InitCANFD();
            InitCAN();
            
        }

        public void CANTransmit(ulong channelMask, uint canID, byte[] data)
        {
            XLDefine.XL_Status txStatus;

            // Create an event collection with 2 messages (events)
            XLClass.xl_event_collection xlEventCollection = new XLClass.xl_event_collection(1);

            // event 1
            xlEventCollection.xlEvent[0].tagData.can_Msg.id = canID;
            xlEventCollection.xlEvent[0].tagData.can_Msg.dlc = 8;
            xlEventCollection.xlEvent[0].tagData.can_Msg.data = data;
            xlEventCollection.xlEvent[0].tag = XLDefine.XL_EventTags.XL_TRANSMIT_MSG;

            // Transmit events
            txStatus = CANDemo.XL_CanTransmit(portHandle, channelMask, xlEventCollection);
            Console.WriteLine("Transmit Message      : " + txStatus);
        }

        public void CANFDTransmit(ulong channelMask, uint canID, byte[] data)
        {
            XLDefine.XL_Status txStatus;

            // Create an event collection with 2 messages (events)
            XLClass.xl_canfd_event_collection xlEventCollection = new XLClass.xl_canfd_event_collection(1);


            // event 1
            xlEventCollection.xlCANFDEvent[0].tag = XLDefine.XL_CANFD_TX_EventTags.XL_CAN_EV_TAG_TX_MSG;
            xlEventCollection.xlCANFDEvent[0].tagData.canId = canID;
            xlEventCollection.xlCANFDEvent[0].tagData.dlc = GetLengthFromDLC(data.Length);
            xlEventCollection.xlCANFDEvent[0].tagData.msgFlags = XLDefine.XL_CANFD_TX_MessageFlags.XL_CAN_TXMSG_FLAG_BRS | XLDefine.XL_CANFD_TX_MessageFlags.XL_CAN_TXMSG_FLAG_EDL;
            xlEventCollection.xlCANFDEvent[0].tagData.data = data;

            // Transmit events
            uint messageCounterSent = 0;
            txStatus = CANDemo.XL_CanTransmitEx(fdPortHandle, channelMask, ref messageCounterSent, xlEventCollection);
            Console.WriteLine("Transmit Message ({0})     : " + txStatus, messageCounterSent);

        }

        // -----------------------------------------------------------------------------------------------
        /// <summary>
        /// EVENT THREAD (RX)
        /// 
        /// RX thread waits for Vector interface events and displays filtered CAN messages.
        /// </summary>
        // ----------------------------------------------------------------------------------------------- 
        public void RXThread()
        {
            // Create new object containing received data 
            XLClass.XLcanRxEvent receivedFDEvent = new XLClass.XLcanRxEvent();

            XLClass.xl_event receivedEvent = new XLClass.xl_event();

            // Result of XL Driver function calls
            XLDefine.XL_Status xlStatus = XLDefine.XL_Status.XL_SUCCESS;

            // Result values of WaitForSingleObject 
            XLDefine.WaitResults waitResult = new XLDefine.WaitResults();


            // Note: this thread will be destroyed by MAIN
            while (true)
            {
                // FD
                // Wait for hardware events
                waitResult = (XLDefine.WaitResults)WaitForSingleObject(eventHandle, 500);

                // If event occurred...
                if (waitResult != XLDefine.WaitResults.WAIT_TIMEOUT)
                {
                    // ...init xlStatus first
                    xlStatus = XLDefine.XL_Status.XL_SUCCESS;

                    // afterwards: while hw queue is not empty...
                    while (xlStatus != XLDefine.XL_Status.XL_ERR_QUEUE_IS_EMPTY)
                    {
                        // ...block RX thread to generate RX-Queue overflows
                        while (blockRxThread) Thread.Sleep(1000);

                        // ...receive data from hardware.
                        xlStatus = CANDemo.XL_CanReceive(fdPortHandle, ref receivedFDEvent);


                        //  If receiving succeed....
                        if (xlStatus == XLDefine.XL_Status.XL_SUCCESS )
                        {
                            var canMsg = receivedFDEvent.tagData.canRxOkMsg;
                            var canMsgString = BitConverter.ToString(canMsg.data).Replace("-", " ");
                             
                            Console.WriteLine(CANDemo.XL_CanGetEventString(receivedFDEvent));
                            //mw.LogState(LogType.Info, CANDemo.XL_CanGetEventString(receivedEvent));
                            if (CanMsgMap.ContainsKey(canMsg.canId))
                            {
                                CanMsgMap[canMsg.canId] = canMsgString;
                            }
                            else
                            {
                                CanMsgMap.Add(canMsg.canId, canMsgString);
                            }
                        }
                    }
                }
                // No event occurred

                // Normal
                if (xlEvWaitHandle.WaitOne(500))
                {
                    // ...init xlStatus first
                    xlStatus = XLDefine.XL_Status.XL_SUCCESS;

                    // afterwards: while hw queue is not empty...
                    while (xlStatus != XLDefine.XL_Status.XL_ERR_QUEUE_IS_EMPTY)
                    {
                        // ...block RX thread to generate RX-Queue overflows
                        while (blockRxThread) { Thread.Sleep(1000); }

                        // ...receive data from hardware.
                        xlStatus = CANDemo.XL_Receive(portHandle, ref receivedEvent);

                        //  If receiving succeed....
                        if (xlStatus == XLDefine.XL_Status.XL_SUCCESS)
                        {
                            if ((receivedEvent.flags & XLDefine.XL_MessageFlags.XL_EVENT_FLAG_OVERRUN) != 0)
                            {
                                Console.WriteLine("-- XL_EVENT_FLAG_OVERRUN --");
                            }

                            // ...and data is a Rx msg...
                            if (receivedEvent.tag == XLDefine.XL_EventTags.XL_RECEIVE_MSG)
                            {
                                if ((receivedEvent.tagData.can_Msg.flags & XLDefine.XL_MessageFlags.XL_CAN_MSG_FLAG_OVERRUN) != 0)
                                {
                                    Console.WriteLine("-- XL_CAN_MSG_FLAG_OVERRUN --");
                                }

                                // ...check various flags
                                if ((receivedEvent.tagData.can_Msg.flags & XLDefine.XL_MessageFlags.XL_CAN_MSG_FLAG_ERROR_FRAME)
                                    == XLDefine.XL_MessageFlags.XL_CAN_MSG_FLAG_ERROR_FRAME)
                                {
                                    Console.WriteLine("ERROR FRAME");
                                }

                                else if ((receivedEvent.tagData.can_Msg.flags & XLDefine.XL_MessageFlags.XL_CAN_MSG_FLAG_REMOTE_FRAME)
                                    == XLDefine.XL_MessageFlags.XL_CAN_MSG_FLAG_REMOTE_FRAME)
                                {
                                    Console.WriteLine("REMOTE FRAME");
                                }

                                else
                                {
                                    var canMsg = receivedEvent.tagData.can_Msg;
                                    var canMsgString = BitConverter.ToString(canMsg.data).Replace("-", " ");

                                    //Console.WriteLine(CANDemo.XL_CanGetEventString(receivedEvent));
                                    //mw.LogState(LogType.Info, CANDemo.XL_CanGetEventString(receivedEvent));
                                    if (CanMsgMap.ContainsKey(canMsg.id))
                                    {
                                        CanMsgMap[canMsg.id] = canMsgString;
                                    }
                                    else
                                    {
                                        CanMsgMap.Add(canMsg.id, canMsgString);
                                    }

                                   

                                    Console.WriteLine(CANDemo.XL_GetEventString(receivedEvent));
                                }
                            }
                        }
                    }
                }
            }
        }

        

    

        
        #endregion

        #region Private Helpers

        
        // -----------------------------------------------------------------------------------------------
        /// <summary>
        /// Retrieve the application channel assignment and test if this channel can be opened
        /// </summary>
        // -----------------------------------------------------------------------------------------------
        private bool GetAppChannelAndTestIsOk(uint appChIdx, ref UInt64 chMask, ref int chIdx)
        {
            XLDefine.XL_Status status = CANDemo.XL_GetApplConfig(appName, appChIdx, ref hwType, ref hwIndex, ref hwChannel, XLDefine.XL_BusTypes.XL_BUS_TYPE_CAN);
            if (status != XLDefine.XL_Status.XL_SUCCESS)
            {
                Console.WriteLine("XL_GetApplConfig      : " + status);
                return false;
            }

            chMask = CANDemo.XL_GetChannelMask(hwType, (int)hwIndex, (int)hwChannel);
            chIdx = CANDemo.XL_GetChannelIndex(hwType, (int)hwIndex, (int)hwChannel);
            if (chIdx < 0 || chIdx >= driverConfig.channelCount)
            {
                // the (hwType, hwIndex, hwChannel) triplet stored in the application configuration does not refer to any available channel.
                return false;
            }

            if ((driverConfig.channel[chIdx].channelBusCapabilities & XLDefine.XL_BusCapabilities.XL_BUS_ACTIVE_CAP_CAN) == 0)
            {
                // CAN is not available on this channel
                return false;
            }

            if (canFdModeNoIso > 0)
            {
                if ((driverConfig.channel[chIdx].channelCapabilities & XLDefine.XL_ChannelCapabilities.XL_CHANNEL_FLAG_CANFD_BOSCH_SUPPORT) == 0)
                {
                    Console.WriteLine("{0} ({1}) does not support CAN FD NO-ISO", driverConfig.channel[chIdx].name.TrimEnd(' ', '\0'),
                        driverConfig.channel[chIdx].transceiverName.TrimEnd(' ', '\0'));
                    return false;
                }
            }
            else
            {
                if ((driverConfig.channel[chIdx].channelCapabilities & XLDefine.XL_ChannelCapabilities.XL_CHANNEL_FLAG_CANFD_ISO_SUPPORT) == 0)
                {
                    Console.WriteLine("{0} ({1}) does not support CAN FD ISO", driverConfig.channel[chIdx].name.TrimEnd(' ', '\0'),
                        driverConfig.channel[chIdx].transceiverName.TrimEnd(' ', '\0'));
                    return false;
                }
            }

            return true;
        }

        // -----------------------------------------------------------------------------------------------
        /// <summary>
        /// Error message if channel assignment is not valid.
        /// </summary>
        // -----------------------------------------------------------------------------------------------
        private void PrintAssignErrorAndPopupHwConf()
        {
            Console.WriteLine("\nPlease check application settings of \"" + appName + " CAN1/CAN2\",\nassign it to available hardware channels and press enter.");
            CANDemo.XL_PopupHwConfig();
        }

        // -----------------------------------------------------------------------------------------------
        /// <summary>
        /// Displays the Vector Hardware Configuration.
        /// </summary>
        // -----------------------------------------------------------------------------------------------
        private void PrintConfig(int[] ciArr)
        {
            Console.WriteLine("\n\nAPPLICATION CONFIGURATION");

            foreach (int channelIndex in ciArr)
            {
                Console.WriteLine("-------------------------------------------------------------------");
                Console.WriteLine("Configured Hardware Channel : " + driverConfig.channel[channelIndex].name);
                Console.WriteLine("Hardware Driver Version     : " + CANDemo.VersionToString(driverConfig.channel[channelIndex].driverVersion));
                Console.WriteLine("Used Transceiver            : " + driverConfig.channel[channelIndex].transceiverName);
            }

            Console.WriteLine("-------------------------------------------------------------------\n");
        }

        private int ReverseDataAtIntelType(string data, int stbit, int length)
        {
            var ret = string.Join(String.Empty, data.Select(c => Convert.ToString(Convert.ToInt64(c.ToString(), 16), 2).PadLeft(4, '0')));

            //원본 64개
            var arr = ret.ToArray();
            //7654 3210


            var rtv = "";
            for (int j = 7; j < arr.Length; j = j + 8)
            {
                //7,15,23(8차이)
                int innerCnt = 0;
                for (int i = j; ; i--)
                {
                    if (innerCnt == 8)
                        break;

                    rtv += arr[i];
                    innerCnt++;
                }
            }

            var setStLenVal = "";
            var SetArr = rtv.ToArray();
            for (int i = stbit; i < stbit + length; i++)
            {
                setStLenVal += SetArr[i];
            }
            var rvsArr = setStLenVal.ToArray();
            Array.Reverse(rvsArr);

            var resultStr = "";
            foreach (char a in rvsArr)
            {
                resultStr += a;
            }

            //8개짤라서 뒤에다 옴기자
            return Convert.ToInt32(resultStr, 2);
        }

        private string IEEEtoFloat(string hex32Input)
        {
            int len = hex32Input.Length / 2;

            byte[] bytes = new byte[len];
            for (int i = 0; i < len; i++)
            {
                bytes[i] = Convert.ToByte(hex32Input.Substring(i * 2, 2), 16);
            }

            Array.Reverse(bytes);

            float data = BitConverter.ToSingle(bytes, 0);
            return data.ToString();
        }

        private XLDefine.XL_CANFD_DLC GetLengthFromDLC(int len)
        {
            if (len <= 8)
                return XLDefine.XL_CANFD_DLC.DLC_CAN_CANFD_8_BYTES;

            

            switch (len)
            {
                case 9: return XLDefine.XL_CANFD_DLC.DLC_CANFD_12_BYTES;
                case 10: return XLDefine.XL_CANFD_DLC.DLC_CANFD_16_BYTES;
                case 11: return XLDefine.XL_CANFD_DLC.DLC_CANFD_20_BYTES;
                case 12: return XLDefine.XL_CANFD_DLC.DLC_CANFD_24_BYTES;
                case 13: return XLDefine.XL_CANFD_DLC.DLC_CANFD_32_BYTES;
                case 14: return XLDefine.XL_CANFD_DLC.DLC_CANFD_48_BYTES;
                case 15: return XLDefine.XL_CANFD_DLC.DLC_CANFD_64_BYTES;
                default: return XLDefine.XL_CANFD_DLC.DLC_CANFD_64_BYTES; // dlc -> 64
            }
        }

     

        private void StartReceiveThread()
        {
            // Run Rx thread
            Console.WriteLine("Start Rx thread...");
            rxThread = new Thread(new ThreadStart(RXThread));
            rxThread.Start();
        }

        #endregion
    }
}
