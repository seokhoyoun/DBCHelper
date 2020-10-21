using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DBCHelper
{

    public class DBCParser
    {
        #region Private Field

        private string mPath;

        #endregion

        public DBCParser()
        {
            mPath = $"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}";
        }

        public void LoadFile()
        {
            using FileStream fileStream = new FileStream(mPath, FileMode.Append, FileAccess.Read, FileShare.ReadWrite);
            using StreamReader streamReader = new StreamReader(fileStream);

            streamReader.ReadToEnd();

            

        }


    }
}
