using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.IO;
using System.Net;
using Newtonsoft.Json;

namespace Synchronize.BaseModule
{
    class MetaServer
    {
        public Dictionary<string, MetaFile> meta_files = null;
		
		public MetaServer()
		{
			meta_files = new Dictionary<string, MetaFile>();
		}
		
		public bool is_exist(string rel_path)
		{
			try
			{
                if (rel_path == "")
                    rel_path = "***"; // it means root.

                MetaFile meta = meta_files[rel_path];
				if (meta != null)
					return true;
			}
			catch (Exception exception)
			{
                Console.WriteLine("Error occured in (is_exist)" + exception);
            }
			return false;
		}
        public MetaFile find_file(string rel_path)
        {
            try
            {
                MetaFile meta = meta_files[rel_path];
                if (meta != null)
                    return meta;
            }
            catch (Exception exception)
            {
                Console.WriteLine("Error occured in (find_file)" + exception);
            }
            return null;
        }

        public void get_json_from_server(string rel_path)
		{
            // download json.

            // parse json.
		}
    }
}
