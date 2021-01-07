using System;
using System.Collections.Generic;
using System.Text;

namespace CommunicationCAN.Model.CAN
{
    public enum ECANType
    {
        Peak,
        Vector
    }

    public class ManagerCAN
    {

        #region Public Properties

        public ECANType ModuleType
        {
            get;
            private set;
        }

        #endregion


        #region Constructor

        public ManagerCAN(ECANType type )
        {
            ModuleType = type;
        }
        #endregion


        #region Public Methods

        public void Connect()
        {

        }

        public void Dispose()
        {

        }
        #endregion

    }
}
