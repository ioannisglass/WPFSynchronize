using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.IO;
using Synchronize.BaseModule;
using System.Net;
using static Synchronize.BaseModule.MetaFile;
using Newtonsoft.Json;
using System.ComponentModel;

namespace Synchronize.BaseModules
{
    class SyncServer
    {
        private string server_url;
        private string root_dir;
        private int total_file_num;
        private int done_file_num;
        private Dictionary<string, Dictionary<string, MetaFileInfo>> json_files;

        public List<string> unwant_files;
        public BackgroundWorker worker;
        public bool is_cancelled;

        public MainWindow main;

        public SyncServer()
		{
            server_url = Env.server_url;
            unwant_files = new List<string>();
            root_dir = "";
            worker = null;
            total_file_num = 0;
            done_file_num = 0;
            is_cancelled = false;
        }
        private Dictionary<string, MetaFileInfo> get_json_from_server(string rel_path)
        {
            Dictionary<string, MetaFileInfo> ret = new Dictionary<string, MetaFileInfo>();

            try
            {
                if (worker.CancellationPending)
                {
                    is_cancelled = true;
                    return null;
                }

                string url = Env.server_url + rel_path.Replace("\\", "/");

                // download json.

                var w = new WebClient();
                string json_data = w.DownloadString(url);

                if (worker.CancellationPending)
                {
                    is_cancelled = true;
                    return null;
                }

                // parse json.

                MetaFile[] meta_files = JsonConvert.DeserializeObject<MetaFile[]>(json_data);

                foreach (MetaFile meta_file in meta_files)
                {
                    string file_rel_path = (rel_path == "") ? "\\" + meta_file.file_info.filename : rel_path + "\\" + meta_file.file_info.filename;

                    ret.Add(file_rel_path, meta_file.file_info);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine("Error occured in (get_json_from_server)" + exception.Message);
                return null;
            }
            return ret;
        }
        public void Sync(string local_dir_path)
		{
            try
            {
                done_file_num = 0;
                is_cancelled = false;
                unwant_files.Clear();
                json_files = new Dictionary<string, Dictionary<string, MetaFileInfo>>();

                root_dir = local_dir_path;

                worker.ReportProgress(0, "Enumerating server files....");

                total_file_num = GetTotalFileNum("");
                if (is_cancelled)
                    return;

                worker.ReportProgress(0, "Synchronizing server files....");

                SyncDirectory("");
            }
            catch (Exception exception)
            {
                Console.WriteLine("Error occured in (Sync)" + exception.Message);
                main.add_log_to_list(Env.LOG_ACTION_SYNC_FAILED, exception.Message);
            }
        }
        private int GetTotalFileNum(string local_path)
        {
            int num = 0;

            if (worker.CancellationPending)
            {
                is_cancelled = true;
                return 0;
            }

            Dictionary<string, MetaFileInfo> meta_files = get_json_from_server(local_path);
            if (meta_files == null)
                return num;

            if (worker.CancellationPending)
            {
                is_cancelled = true;
                return num;
            }

            json_files.Add((local_path == "") ? "***" : local_path, meta_files);

            foreach (KeyValuePair<string, MetaFileInfo> x in meta_files)
            {
                if (worker.CancellationPending)
                    return num;

                num++;

                if (x.Value.type == "dir")
                {
                    string full_path = local_path + "\\" + x.Value.filename;
                    num += GetTotalFileNum(full_path);
                }
            }
            return num;
        }
        private string get_rel_path(string abs_path)
        {
            string path = abs_path.Substring(root_dir.Length);
            return path;
        }
        private MetaFileInfo find_meta_info(Dictionary<string, MetaFileInfo> dic, string rel_path)
        {
            MetaFileInfo ret = null;
            try
            {
                ret = dic[rel_path];
            }
            catch (Exception exception)
            {
            }
            return ret;
        }
        private void update_file(string local_rel_path)
        {
            try
            {
                if (worker.CancellationPending)
                {
                    is_cancelled = true;
                    return;
                }

                string local_full_path = root_dir + local_rel_path;
                string file_url = Env.server_file_url + local_rel_path.Replace("\\", "/");

                string temp_path = get_temp_file_path(local_full_path);

                WebClient wc = new WebClient();
                wc.DownloadFile(file_url, temp_path);
                if (File.Exists(local_full_path))
                    File.Delete(local_full_path);
                File.Move(temp_path, local_full_path);

                main.add_log_to_list(Env.LOG_ACTION_SYNC_FILE_OK, local_full_path);
            }
            catch (Exception exception)
            {
                main.add_log_to_list(Env.LOG_ACTION_SYNC_FILE_FAILED, string.Format("{0} : {1}", root_dir + local_rel_path, exception.Message));
            }
        }
        private int get_progress_percentage()
        {
            done_file_num++;
            if (total_file_num == 0)
                return 100;

            return (int)(((double)done_file_num / (double)total_file_num) * 100.0);
        }
        private void SyncDirectory(string local_rel_dir)
        {
            string abs_path = (local_rel_dir == "") ? root_dir : root_dir + local_rel_dir;

            if (worker.CancellationPending)
            {
                is_cancelled = true;
                return;
            }

            string key_path = local_rel_dir == "" ? "***" : local_rel_dir;
            /*if (key_path != "***" && key_path[0] != '/')
                key_path = "\\" + key_path;*/
            Dictionary<string, MetaFileInfo> meta_files = json_files[key_path];
            if (meta_files == null)
            {
                // unwanted dir. must save log.
                add_unwant_files(abs_path);
                return;
            }

            if (worker.CancellationPending)
            {
                is_cancelled = true;
                return;
            }

            // Recurse into subdirectories of this directory.
            string[] subdirectoryEntries = Directory.GetDirectories(abs_path);
            foreach (string subdirectory in subdirectoryEntries)
            {
                if (worker.CancellationPending)
                {
                    is_cancelled = true;
                    return;
                }

                string sub_rel_path = get_rel_path(subdirectory);
                MetaFileInfo meta_info = find_meta_info(meta_files, sub_rel_path);
                if (meta_info == null)
                {
                    // unwanted dir. must save log.
                    add_unwant_files(subdirectory);
                    continue;
                }

                worker.ReportProgress(get_progress_percentage(), string.Format("Synchronizing sub directory : {0}", sub_rel_path));

                meta_info.is_newer = false;
                SyncDirectory(sub_rel_path);
            }

            // create a folder which is existed in server, but not existed in local.
            foreach (KeyValuePair<string, MetaFileInfo> x in meta_files)
            {
                if (worker.CancellationPending)
                {
                    is_cancelled = true;
                    return;
                }

                if (x.Value.type == "dir" && x.Value.is_newer)
                {
                    string full_path = abs_path + "\\" + x.Value.filename;
                    string sub_rel_path = get_rel_path(full_path);

                    Directory.CreateDirectory(full_path);
                    SyncDirectory(sub_rel_path);

                    worker.ReportProgress(get_progress_percentage(), string.Format("Synchronizing sub directory : {0}", sub_rel_path));
                }
            }

            string[] fileEntries = Directory.GetFiles(abs_path);
            foreach (string fileName in fileEntries)
            {
                if (worker.CancellationPending)
                {
                    is_cancelled = true;
                    return;
                }
                string sub_rel_path = get_rel_path(fileName);
                MetaFileInfo meta_info = find_meta_info(meta_files, sub_rel_path);
                if (meta_info == null)
                {
                    // unwanted dir. must save log.
                    add_unwant_files(fileName);
                    continue;
                }

                worker.ReportProgress(get_progress_percentage(), string.Format("Synchronizing file : {0}", sub_rel_path));

                meta_info.is_newer = false;
                SyncFile(root_dir, sub_rel_path, meta_info);
            }

            // create a file which is existed in server, but not existed in local.
            foreach (KeyValuePair<string, MetaFileInfo> x in meta_files)
            {
                if (worker.CancellationPending)
                {
                    is_cancelled = true;
                    return;
                }

                if (x.Value.type != "dir" && x.Value.is_newer)
                {
                    string full_path = abs_path + "\\" + x.Value.filename;
                    string sub_rel_path = get_rel_path(full_path);

                    worker.ReportProgress(get_progress_percentage(), string.Format("Synchronizing file : {0}", sub_rel_path));

                    update_file(sub_rel_path);
                }
            }
        }

        private void SyncFile(string root_dir, string local_rel_path, MetaFileInfo meta_file_info)
        {
            if (worker.CancellationPending)
            {
                is_cancelled = true;
                return;
            }

            //Console.WriteLine("Processed file '{0}'.", path);
            string local_full_path = root_dir + "\\" + local_rel_path;
			
			// get file hash.
			string hash_sha1 = FileHash.checkSHA1(local_full_path);
            hash_sha1 = hash_sha1.ToLower();

            // compare hash with server.
            // If dismatched, download file.
            if (meta_file_info.sha1 != hash_sha1)
            {
                if (worker.CancellationPending)
                {
                    is_cancelled = true;
                    return;
                }

                update_file(local_rel_path);
            }
            else
            {
                // save log.
                main.add_log_to_list(Env.LOG_ACTION_SYNC_FILE_PASS, local_full_path);
            }
        }
        private string get_temp_file_path(string file_full_path)
        {
            int i = 0;
            string temp_path;

            while (true)
            {
                temp_path = string.Format("{0}.downloading{1}", file_full_path, i + 1);
                if (!File.Exists(temp_path))
                    break;
                i++;
            }
            return temp_path;
        }
        private void SaveLog()
        {

        }
        private void add_unwant_files(string full_path)
        {
            unwant_files.Add(full_path);
        }
    }
}
