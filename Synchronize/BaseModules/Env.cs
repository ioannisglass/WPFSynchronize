using Synchronize.BaseModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Synchronize.BaseModules
{
    class Env
    {
        static public string server_url = "https://apps.chemoinformatics.com/crm/services/ls.php?path=./deploy";
        static public string server_file_url = "https://apps.chemoinformatics.com/crm/services/get.php?file=./deploy";

        static public string LOG_FILE_NAME = "log.txt";

        static public string LOG_ACTION_SYNC_START = "SYNC_START";
        static public string LOG_ACTION_SYNC_FINISH = "SYNC_FINISH";
        static public string LOG_ACTION_SYNC_FAILED = "SYNC_FAILED";
        static public string LOG_ACTION_SYNC_CANCELLED = "SYNC_CANCELLED";
        static public string LOG_ACTION_SYNC_FILE_OK = "SYNC_FILE_OK";
        static public string LOG_ACTION_SYNC_FILE_FAILED = "SYNC_FILE_FAILED";
        static public string LOG_ACTION_SYNC_FILE_PASS = "SYNC_FILE_PASS(Already Matched)";
        static public string LOG_ACTION_UNWANTED_FILE_DEL = "UNWANTED_FILE_DELETE";
    }
}
