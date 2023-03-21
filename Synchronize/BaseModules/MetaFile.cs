using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.IO;

namespace Synchronize.BaseModule
{
    public class MetaFile
    {
        public class MetaFileInfo
        {
            public string filename;
            public string size;
            public string type;
            public string owner;
            public string createdate;
            public string modifieddate;
            public string md5;
            public string sha1;

            public bool is_newer;

            public MetaFileInfo(string n, string s, string t, string o, string c, string m, string md, string sh)
            {
                filename = n;
                size = s;
                type = t;
                owner = o;
                createdate = c;
                modifieddate = m;
                md5 = md;
                sha1 = sh;

                is_newer = true;
            }
        }
        //public string path; // relative path.

        public string sno;
        public MetaFileInfo file_info;

        public MetaFile(string order, MetaFileInfo info)
        {
            sno = order;
            file_info = info;
        }
    }

}
