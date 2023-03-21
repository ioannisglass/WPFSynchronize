using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;

using System.Windows.Forms;
using System.Security;
using System.Security.Permissions;
using Microsoft.Win32;
using System.Diagnostics.Contracts;
using System.Runtime.InteropServices;
using Synchronize.BaseModules;
using Synchronize.BaseModule;
using System.ComponentModel;
using System.Net;

namespace Synchronize
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string varJava = "JAVA_HOME";
        private string varJre = "JRE_HOME";
        private string varLims = "LIMS_HOME";

        private List<string> log_list = new List<string>();

        private SyncServer sync_server;

        private readonly BackgroundWorker worker = new BackgroundWorker();
        //private DependencyProperty unwant_files;

        public MainWindow()
        {
            
            InitializeComponent();

            String varJava_Path = System.Environment
                .GetEnvironmentVariable(varJava, EnvironmentVariableTarget.Machine);
            tbxEnvP_1.AppendText(varJava_Path);

            String varJre_Path = System.Environment
                .GetEnvironmentVariable(varJre, EnvironmentVariableTarget.Machine);
            tbxEnvP_2.AppendText(varJre_Path);

            String varLims_Path = System.Environment
                .GetEnvironmentVariable(varLims, EnvironmentVariableTarget.Machine);
            tbxEnvP_3.AppendText(varLims_Path);

            worker.WorkerReportsProgress = true;
            worker.WorkerSupportsCancellation = true;
            worker.DoWork += worker_DoWork;
            worker.RunWorkerCompleted += worker_RunWorkerCompleted;
            worker.ProgressChanged += worker_ProgressChanged;
        }

        private void btnSet_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(tbxEnvP_1.Text) || !string.IsNullOrEmpty(tbxEnvP_2.Text) || !string.IsNullOrEmpty(tbxEnvP_3.Text))
            {
                if (!string.IsNullOrEmpty(tbxEnvP_1.Text))
                {
                    EnvironmentPermission permissions = new EnvironmentPermission(EnvironmentPermissionAccess.AllAccess, varJava);
                    permissions.Demand();
                    Environment.SetEnvironmentVariable(varJava, tbxEnvP_1.Text.ToString(),
                        EnvironmentVariableTarget.Machine);
                }
                if (!string.IsNullOrEmpty(tbxEnvP_2.Text))
                {
                    EnvironmentPermission permissions = new EnvironmentPermission(EnvironmentPermissionAccess.AllAccess, varJre);
                    permissions.Demand();
                    Environment.SetEnvironmentVariable(varJre, tbxEnvP_2.Text.ToString(),
                        EnvironmentVariableTarget.Machine);
                }
                if (!string.IsNullOrEmpty(tbxEnvP_3.Text))
                {
                    EnvironmentPermission permissions = new EnvironmentPermission(EnvironmentPermissionAccess.AllAccess, varLims);
                    permissions.Demand();
                    Environment.SetEnvironmentVariable(varLims, tbxEnvP_3.Text.ToString(),
                        EnvironmentVariableTarget.Machine);
                }
                System.Windows.Forms.MessageBox.Show("Successful.");
            }
            else
                System.Windows.Forms.MessageBox.Show("No Input.");

        }

        private void btnOpen_1_Click(object sender, RoutedEventArgs e)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                DialogResult result = fbd.ShowDialog();
                tbxEnvP_1.Clear();
                tbxEnvP_1.AppendText(fbd.SelectedPath);
            }
        }

        private void btnOpen_2_Click(object sender, RoutedEventArgs e)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                DialogResult result = fbd.ShowDialog();
                //System.Windows.Forms.MessageBox.Show("Files found: " + fbd.SelectedPath);
                tbxEnvP_2.Clear();
                tbxEnvP_2.AppendText(fbd.SelectedPath);
            }
        }

        private void btnOpen_3_Click(object sender, RoutedEventArgs e)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                DialogResult result = fbd.ShowDialog();
                tbxEnvP_3.Clear();
                tbxEnvP_3.AppendText(fbd.SelectedPath);
            }
        }
        private void btnIns_Click(object sender, RoutedEventArgs e)
        {
            /*
            RegistryKey key;
            RegistrySecurity regSecurity;
            RegistryKeyPermissionCheck permission_check = RegistryKeyPermissionCheck.ReadWriteSubTree;

            key = Registry.ClassesRoot.OpenSubKey("Directory\\shell\\cmd", permission_check, RegistryRights.FullControl);
            if (key == null)
            {

            }
            else
            {
                RegistrySecurity rs = new RegistrySecurity();
                rs = key.GetAccessControl();
                ShowSecurity(rs);

                string user = Environment.UserDomainName + "\\" + Environment.UserName;

                // Create a security object that grants no access.
                RegistrySecurity mSec = new RegistrySecurity();

                // Add a rule that grants the current user the right
                // to read and enumerate the name/value pairs in a key, 
                // to read its access and audit rules, to enumerate
                // its subkeys, to create subkeys, and to delete the key. 
                // The rule is inherited by all contained subkeys.
                //
                RegistryAccessRule rule = new RegistryAccessRule(user, RegistryRights.FullControl, InheritanceFlags.ContainerInherit, PropagationFlags.None, AccessControlType.Allow);
                mSec.AddAccessRule(rule);

                // Add a rule that allows the current user the right
                // right to set the name/value pairs in a key. 
                // This rule is inherited by contained subkeys, but
                // propagation flags limit it to immediate child 
                // subkeys.
                rule = new RegistryAccessRule(user,
                    RegistryRights.ChangePermissions,
                    InheritanceFlags.ContainerInherit,
                    PropagationFlags.InheritOnly |
                        PropagationFlags.NoPropagateInherit,
                    AccessControlType.Allow);
                mSec.AddAccessRule(rule);

                ShowSecurity(mSec);

                key = Registry.ClassesRoot.CreateSubKey("Directory\\shell\\cmd", RegistryKeyPermissionCheck.ReadWriteSubTree, mSec);

                ////////////////////////////////////////////////////////////////////////////////////

                object val = key.GetValue("ShowBasedOnVelocityId");

                key.SetValue("HideBasedOnVelocityId", val);
                key.DeleteValue("ShowBasedOnVelocityId");
            }*/

            try
            {
                RegPriv.SetCommandPromptIntoConetxMenu("Directory\\shell\\cmd", true);
                RegPriv.SetCommandPromptIntoConetxMenu("Directory\\Background\\shell\\cmd", true);
                lblCmdMenu.FontSize = 16;
                lblCmdMenu.Content = "\"Command Line\" context has been installed successfully!";
            }
            catch (Exception exception)
            {
                lblCmdMenu.Content = "Failed to install : " + exception.Message;
            }
        }

        private void btnUnins_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                RegPriv.SetCommandPromptIntoConetxMenu("Directory\\shell\\cmd", false);
                RegPriv.SetCommandPromptIntoConetxMenu("Directory\\Background\\shell\\cmd", false);
                lblCmdMenu.FontSize = 16;
                lblCmdMenu.Content = "\"Command Line\" context has been uninstalled successfully!";
            }
            catch (Exception exception)
            {
                lblCmdMenu.Content = "Failed to uninstall : " + exception.Message;
            }
        }
        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            string path = (string)e.Argument;
            sync_server = new SyncServer();
            sync_server.worker = worker;
            sync_server.main = this;
            sync_server.Sync(path);
            if (sync_server.is_cancelled)
                e.Cancel = true;
        }
        private void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            Console.WriteLine("Log : percent : {0}, msg = {1}", e.ProgressPercentage, (string)e.UserState);
            lbStatus.Content = (string)e.UserState;
            txtLog.Text += (txtLog.Text == "") ? (string)e.UserState : "\n" + (string)e.UserState;
            prgbUpdate.Value = e.ProgressPercentage;
            prgbUpdate.InvalidateVisual();
        }

        public class Unwanted : INotifyPropertyChanged
        {
            public Unwanted(string _name, string _status)
            {
                name = _name;
                status = _status;
            }

            string name;
            public string Name
            {
                get { return name; }
                set { name = value; OnPropertyChanged(); }
            }

            string status;
            public string Status
            {
                get { return status; }
                set { status = value; OnPropertyChanged(); }
            }
            public event PropertyChangedEventHandler PropertyChanged;
            protected virtual void OnPropertyChanged(string propertyName = null)
            {
                if (this.PropertyChanged != null)
                    this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }


        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled != true)
            {
                lbStatus.Visibility = Visibility.Hidden;
                txtLog.Visibility = Visibility.Hidden;
                prgbUpdate.Visibility = Visibility.Hidden;

                lbUpdate.Visibility = Visibility.Visible;
                lsvUpdate.Visibility = Visibility.Visible;
                lsvUpdate.Items.Clear();

                lbUpdate.Content = string.Format("Additional Local Files(unwanted) : {0}", sync_server.unwant_files.Count);

                btnUpdate.Content = "Update";

                foreach (string item in sync_server.unwant_files)
                {
                    lsvUpdate.Items.Add(new Unwanted(item, ""));
                }

                if (sync_server.unwant_files.Count > 0)
                    btnDelete.IsEnabled = true;

                add_log_to_list(Env.LOG_ACTION_SYNC_FINISH, "");
            }
            btnLog.IsEnabled = true;
        }
        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            btnDelete.IsEnabled = false;
            btnLog.IsEnabled = false;
            if (btnUpdate.Content.ToString() == "Update")
            {
                string path = Environment.GetEnvironmentVariable(varLims, EnvironmentVariableTarget.Machine);
                lbStatus.Content = "";
                txtLog.Clear();
                log_list.Clear();
                prgbUpdate.Value = 0;

                lbStatus.Visibility = Visibility.Visible;
                txtLog.Visibility = Visibility.Visible;
                prgbUpdate.Visibility = Visibility.Visible;

                lbUpdate.Visibility = Visibility.Hidden;
                lsvUpdate.Visibility = Visibility.Hidden;

                add_log_to_list(Env.LOG_ACTION_SYNC_START, string.Format("Server : {0}, Local : {1}", Env.server_url, path));

                worker.RunWorkerAsync(path);

                btnUpdate.Content = "Cancel";
            }
            else
            {
                lbStatus.Content = "Cancelled";

                worker.CancelAsync();

                btnUpdate.Content = "Update";

                add_log_to_list(Env.LOG_ACTION_SYNC_CANCELLED, "");
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (lsvUpdate.SelectedIndex == -1)
            {
                System.Windows.Forms.MessageBox.Show("Please select an Item first!");
                return;
            }


            for (int i = 0; i < lsvUpdate.SelectedItems.Count; i++)
            {
                Unwanted item = (Unwanted)lsvUpdate.SelectedItems[i];
                string str = item.Name;
                item.Status = "Delete";
                if (File.Exists(str))
                {
                    File.Delete(str);
                    add_log_to_list(Env.LOG_ACTION_UNWANTED_FILE_DEL, str);
                }
                if (Directory.Exists(str))
                {
                    try
                    {
                        Directory.Delete(str, true);
                        add_log_to_list(Env.LOG_ACTION_UNWANTED_FILE_DEL, str);
                    }
                    catch (Exception exception)
                    {
                        Console.WriteLine(exception.Message);
                    }
                }
            }
        }
        public void add_log_to_list(string status, string msg)
        {
            string log = string.Format("{0} {1} {2}", DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss"), status, msg);
            log_list.Add(log);
        }
        private void btnLog_Click(object sender, RoutedEventArgs e)
        {
            using (StreamWriter stream = File.AppendText(Env.LOG_FILE_NAME))
            {
                foreach (string log in log_list)
                {
                    stream.WriteLine(log);
                }
            }
            log_list.Clear();
            System.Windows.MessageBox.Show(string.Format("Append log to {0}", Env.LOG_FILE_NAME));
        }
    }
}
