using Mentula.Engine;
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Net;
using Mentula.Server.GUI;
using NIMT = Lidgren.Network.NetIncomingMessageType;
using System.Runtime.InteropServices;

namespace Mentula.Server
{
    public class Server : Game
    {
        private Dictionary<IPAddress, int> players;
        private CPUUsage cpu;

        public Server()
        {
            InitializeComponent();
            WriteFirstLine("Console Created.");
            IsMouseVisible = true;
            players = new Dictionary<IPAddress, int>();
            cpu = new CPUUsage();
        }

        public void SetState(string state)
        {
            lbl_Status.Text = string.Format("Status: {0}", state);
        }

        public void UpdateStats(bool last = false)
        {
            short value = last ? (short)0 : cpu.GetUsage();

            InvokeIfRequired(lbl_CPU, () =>
                {
                    lbl_CPU.Text = string.Format("CPU usage: {0}", value);
                    proBarCPU.Value = (int)value;
                });
        }

        public void AddPlayer(IPAddress ip, string name)
        {
            InvokeIfRequired(dGrid_Connections, () => players.Add(ip, dGrid_Connections.Rows.Add(name, ip)));
        }

        public void RemovePlayer(IPAddress ip)
        {
            InvokeIfRequired(dGrid_Connections, () => dGrid_Connections.Rows.RemoveAt(players[ip]));
            players.Remove(ip);
        }

        public void ClearPlayers()
        {
            InvokeIfRequired(dGrid_Connections, () => dGrid_Connections.Rows.Clear());
        }

        public void WriteLine(NIMT nimt, string format, params object[] args)
        {
            if (string.IsNullOrWhiteSpace(format)) return;
            string mode = "";
            Color color = Color.White;

            switch (nimt)
            {
                case (NIMT.ConnectionApproval):
                    color = Color.Green;
                    mode = "Approval";
                    break;
                case (NIMT.ConnectionLatencyUpdated):
                    mode = "LatencyUpdate";
                    break;
                case (NIMT.Data):
                case (NIMT.UnconnectedData):
                    mode = "Data";
                    break;
                case (NIMT.DebugMessage):
                case (NIMT.VerboseDebugMessage):
                    mode = "Debug";
                    break;
                case (NIMT.DiscoveryRequest):
                case (NIMT.DiscoveryResponse):
                    mode = "Discovery";
                    break;
                case (NIMT.Error):
                case (NIMT.ErrorMessage):
                    color = Color.Red;
                    mode = "Error";
                    break;
                case (NIMT.NatIntroductionSuccess):
                    mode = "NAT";
                    break;
                case (NIMT.Receipt):
                    mode = "Receipt";
                    break;
                case (NIMT.StatusChanged):
                    color = Color.Green;
                    mode = "Status";
                    break;
                case (NIMT.WarningMessage):
                    color = Color.Yellow;
                    mode = "Warning";
                    break;
            }

            string line = string.Format("\n[{0}][{1}] {2}", string.Format("{0:H:mm:ss}", DateTime.Now), mode, string.Format(format, args));

            InvokeIfRequired(txt_Console, () =>
            {
                txt_Console.SelectionColor = color;
                txt_Console.AppendText(line);
                txt_Console.Find(line);
                txt_Console.ScrollToCaret();
            });
        }

        private void WriteFirstLine(string format, params object[] args)
        {
            string line = string.Format("[{0}][Info] {1}", string.Format("{0:H:mm:ss}", DateTime.Now), string.Format(format, args));

            txt_Console.SelectionColor = Color.DodgerBlue;
            txt_Console.AppendText(line);
            txt_Console.Find(line);
            txt_Console.ScrollToCaret();
        }

        private void InvokeIfRequired(Control control, MethodInvoker action)
        {
            try
            {
                if (control.InvokeRequired && !control.IsDisposed) control.Invoke(action);
                else action();
            }
            catch(Exception e)
            {
                if (!control.IsDisposed) WriteLine(NIMT.Error, "Cannot perform action on '{0}'. \nInnterException: {1}", control.Name, e);
            }
        }

        #region Controls

        private void InitializeComponent()
        {
            this.statusStrip =          new StatusStrip();
            this.lbl_Status =           new ToolStripStatusLabel();
            this.lbl_LastMessage =      new ToolStripStatusLabel();
            this.gBox_Info =            new GroupBox();
            this.lbl_CPU =              new Label();
            this.proBarCPU =            new ProgressBar();
            this.btn_Stop =             new Button();
            this.btn_Restart =          new Button();
            this.btn_Kill =             new Button();
            this.dGrid_Connections =    new DataGridView();
            this.coll_Name =            new DataGridViewTextBoxColumn();
            this.coll_Ip =              new DataGridViewTextBoxColumn();
            this.txt_Console =          new RichTextBox();
            this.splitContainer1 =      new SplitContainer();
            this.statusStrip.SuspendLayout();
            this.gBox_Info.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dGrid_Connections)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            Window.SuspendLayout();
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lbl_Status,
            this.lbl_LastMessage});
            this.statusStrip.Name = "statusStrip";
            // 
            // lbl_Status
            // 
            this.lbl_Status.Name = "lbl_Status";
            this.lbl_Status.Text = "Status: NULL";
            // 
            // lbl_LastMessage
            // 
            this.lbl_LastMessage.Name = "lbl_LastMessage";
            this.lbl_LastMessage.Text = "Last Info Message: NULL";
            this.lbl_LastMessage.Padding = new System.Windows.Forms.Padding(50, 0, 0, 0);
            // 
            // gBox_Info
            // 
            this.gBox_Info.Controls.Add(this.lbl_CPU);
            this.gBox_Info.Controls.Add(this.proBarCPU);
            this.gBox_Info.Controls.Add(this.btn_Stop);
            this.gBox_Info.Controls.Add(this.btn_Restart);
            this.gBox_Info.Controls.Add(this.btn_Kill);
            this.gBox_Info.Name = "gBox_Info";
            this.gBox_Info.Text = "Info";
            this.gBox_Info.TabStop = false;
            this.gBox_Info.Location = new Point(16, 258);
            this.gBox_Info.Size = new Size(752, 123);
            // 
            // lbl_CPU
            // 
            this.lbl_CPU.Name = "lbl_CPU";
            this.lbl_CPU.Text = "CPU Usage: 000%";
            this.lbl_CPU.Location = new Point(150, 33);
            // 
            // proBarCPU
            // 
            this.proBarCPU.Name = "proBarCPU";
            this.proBarCPU.Location = new Point(323, 33);
            // 
            // btn_Stop
            // 
            this.btn_Stop.Name = "btn_Stop";
            this.btn_Stop.Text = "Stop";
            this.btn_Stop.Location = new Point(7, 93);
            this.btn_Stop.UseVisualStyleBackColor = true;
            this.btn_Stop.Click += new System.EventHandler(this.btn_Stop_Click);
            // 
            // btn_Restart
            // 
            this.btn_Restart.Name = "btn_Restart";
            this.btn_Restart.Text = "Restart";
            this.btn_Restart.Location = new Point(7, 62);
            this.btn_Restart.UseVisualStyleBackColor = true;
            this.btn_Restart.Click += new System.EventHandler(this.btn_Restart_Click);
            // 
            // btn_Kill
            // 
            this.btn_Kill.Name = "btn_Kill";
            this.btn_Kill.Text = "Kill";
            this.btn_Kill.Location = new Point(7, 31);
            this.btn_Kill.UseVisualStyleBackColor = true;
            this.btn_Kill.Click += new System.EventHandler(this.btn_Kill_Click);
            // 
            // dGrid_Connections
            // 
            this.dGrid_Connections.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            this.dGrid_Connections.AllowUserToAddRows = false;
            this.dGrid_Connections.AllowUserToDeleteRows = false;
            this.dGrid_Connections.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dGrid_Connections.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.coll_Name,
            this.coll_Ip});
            this.dGrid_Connections.Name = "dGrid_Connections";
            this.dGrid_Connections.ReadOnly = true;
            this.dGrid_Connections.RowHeadersVisible = false;
            this.dGrid_Connections.Size = new Size(144, 233);
            // 
            // coll_Name
            // 
            this.coll_Name.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.coll_Name.Name = "coll_Name";
            this.coll_Name.HeaderText = "Name";
            this.coll_Name.ReadOnly = true;
            // 
            // coll_Ip
            // 
            this.coll_Ip.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.coll_Ip.Name = "coll_Ip";
            this.coll_Ip.HeaderText = "IP";
            this.coll_Ip.ReadOnly = true;
            // 
            // txt_Console
            // 
            this.txt_Console.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.txt_Console.ForeColor = System.Drawing.SystemColors.Menu;
            this.txt_Console.Name = "txt_Console";
            this.txt_Console.ReadOnly = true;
            this.txt_Console.Size = new Size(592, 236);
            this.txt_Console.MouseDown += new System.Windows.Forms.MouseEventHandler(this.txt_Console_MouseDown);
            // 
            // splitContainer
            // 
            this.splitContainer1.Name = "splitContainer";
            this.splitContainer1.Location = new Point(16, 12);
            this.splitContainer1.Size = new Size(752, 239);
            this.splitContainer1.SplitterDistance = 150;
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.dGrid_Connections);
            this.splitContainer1.Panel1MinSize = 25;
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.txt_Console);
            // 
            // form_GUI
            // 
            Window.ClientBounds = new Engine.Core.Rectangle(0, 0, 800, 450);
            Window.AddControl(this.splitContainer1);
            Window.AddControl(this.gBox_Info);
            Window.AddControl(this.statusStrip);
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.gBox_Info.ResumeLayout(false);
            this.gBox_Info.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dGrid_Connections)).EndInit();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            Window.ResumeLayout(false);
            Window.PerformLayout();
        }

        public Button btn_Kill;
        public Button btn_Stop;
        public Button btn_Restart;
        private StatusStrip statusStrip;
        private ToolStripStatusLabel lbl_Status;
        private ToolStripStatusLabel lbl_LastMessage;
        private GroupBox gBox_Info;
        private DataGridView dGrid_Connections;
        private RichTextBox txt_Console;
        private DataGridViewTextBoxColumn coll_Name;
        private DataGridViewTextBoxColumn coll_Ip;
        private SplitContainer splitContainer1;
        private Label lbl_CPU;
        private ProgressBar proBarCPU;

        #endregion

        [DllImport("user32.dll")]
        private static extern int HideCaret(IntPtr hwnd);

        private void btn_Kill_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void btn_Restart_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void btn_Stop_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void txt_Console_MouseDown(object sender, MouseEventArgs e)
        {
            HideCaret(txt_Console.Handle);
        }
    }
}
