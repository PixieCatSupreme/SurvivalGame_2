using Lidgren.Network;
using Lidgren.Network.Xna;
using Mentula.Content;
using Mentula.Engine;
using Mentula.Engine.Core;
using Mentula.Server.GUI;
using Mentula.Utilities;
using Mentula.Utilities.Net;
using Mentula.Utilities.Resources;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using Color = System.Drawing.Color;
using NIMT = Lidgren.Network.NetIncomingMessageType;
using NOM = Lidgren.Network.NetOutgoingMessage;
using NPConf = Lidgren.Network.NetPeerConfiguration;
using Point = System.Drawing.Point;
using Size = System.Drawing.Size;

namespace Mentula.Server
{
    public class Server : Game
    {
        private Dictionary<IPAddress, int> players_UI;
        private Dictionary<string, IPAddress> players_Banned;
        private Dictionary<long, string> players_Queue;

        private CPUUsage cpu;
        private NetServer server;
        private GameLogic logic;
        private float timeDiff;

        public Server()
        {
            InitializeComponent();
            logic = new GameLogic(this);
            WriteFirstLine("Console Created.");
            IsMouseVisible = true;

            Load += Server_Load;
            Initialize += Server_Initialize;
            Update += Server_Update;
            Draw += Server_Draw;
        }

        private void Server_Load()
        {
            Window.AllowAltF4 = true;
            WriteLine(NIMT.Data, "Loading Server.");
            server.Start();
        }

        private void Server_Initialize()
        {
            WriteLine(NIMT.Data, "Initializing server.");
            players_UI = new Dictionary<IPAddress, int>();
            players_Banned = new Dictionary<string, IPAddress>();
            players_Queue = new Dictionary<long, string>();
            cpu = new CPUUsage();

            NPConf config = new NPConf(Res.AppName) { Port = Ips.PORT, EnableUPnP = true };
            config.EnableMessageType(NIMT.DiscoveryRequest);
            config.EnableMessageType(NIMT.ConnectionApproval);
            server = new NetServer(config);
        }

        private unsafe void Server_Update(GameTime time)
        {
            SetState(server.Status.ToString());
            NetIncomingMessage msg;

            while ((msg = server.ReadMessage()) != null)
            {
                NOM nom;
                long id = msg.SenderConnection != null ? msg.SenderConnection.RemoteUniqueIdentifier : -1;

                switch (msg.MessageType)
                {
                    case (NIMT.VerboseDebugMessage):
                    case (NIMT.DebugMessage):
                    case (NIMT.WarningMessage):
                    case (NIMT.ErrorMessage):
                        WriteLine(msg.MessageType, msg.ReadString());
                        break;
                    case (NIMT.DiscoveryRequest):
                        server.SendDiscoveryResponse(null, msg.SenderEndPoint);
                        WriteLine(NIMT.DiscoveryRequest, "{0} discovered the service.", msg.SenderEndPoint.Address);
                        break;
                    case (NIMT.ConnectionApproval):
                        string name = msg.ReadString();

                        if (string.IsNullOrWhiteSpace(name) || name.Length > 16)
                        {
                            msg.SenderConnection.Deny("Your name must be between 1 and 16 characters!");
                            break;
                        }
                        else if (players_Banned.Values.Contains(msg.SenderEndPoint.Address))
                        {
                            msg.SenderConnection.Deny("You have been banned from this server!");
                            break;
                        }
                        else if (logic.PlayerExists(id, name) || players_Queue.ContainsKey(id))
                        {
                            msg.SenderConnection.Deny(string.Format("The name \"{0}\" is already in use!", name));
                            break;
                        }

                        players_Queue.Add(id, name);
                        msg.SenderConnection.Approve();
                        break;
                    case (NIMT.StatusChanged):
                        id = msg.SenderConnection.RemoteUniqueIdentifier;
                        NetConnectionStatus status = (NetConnectionStatus)msg.ReadByte();

                        switch (status)
                        {
                            case (NetConnectionStatus.Connected):
                                if (!players_Queue.ContainsKey(id))
                                {
                                    WriteLine(NIMT.WarningMessage, string.Format("Unknown acces by: {0}", id));
                                    break;
                                }

                                name = players_Queue[id];
                                players_Queue.Remove(id);
                                AddPlayer(msg.SenderEndPoint.Address, name);
                                logic.AddPlayer(id, name);
                                WriteLine(NIMT.StatusChanged, "{0}({1}) connected!", NetUtility.ToHexString(id), name);

                                logic.Update(0);
                                Chunk[] chunks = logic.Map.GetChunks(logic.GetPlayer(id).ChunkPos);
                                NPC[] npcs = logic.Map.GetNPC(logic.GetPlayer(id).ChunkPos);

                                nom = server.CreateMessage();
                                nom.Write((byte)NDT.InitialChunkRequest);
                                nom.Write(chunks);
                                nom.Write(npcs);
                                server.SendMessage(nom, msg.SenderConnection, NetDeliveryMethod.ReliableOrdered);
                                break;
                            case (NetConnectionStatus.Disconnected):
                                name = logic.GetPlayer(id).Name;
                                logic.RemovePlayer(id);
                                RemovePlayer(msg.SenderEndPoint.Address);
                                WriteLine(NIMT.StatusChanged, "{0}({1}) disconnected!", NetUtility.ToHexString(id), name);
                                break;
                        }
                        break;
                    case (NIMT.Data):
                        NDT type = (NDT)msg.ReadByte();
                        try
                        {
                            switch (type)
                            {
                                case (NDT.HeroUpdate):
                                    IntVector2 chunk = msg.ReadPoint();
                                    IntVector2 oldChunk = logic.GetPlayer(id).ChunkPos;
                                    Vector2 tile = msg.ReadVector2();
                                    float rot = msg.ReadHalfPrecisionSingle();

                                    if (chunk != oldChunk)
                                    {
                                        nom = server.CreateMessage();
                                        nom.Write((byte)NDT.ChunkRequest);
                                        Chunk[] chunkArr = logic.Map.GetChunks(oldChunk, chunk);
                                        NPC[] npcArr = logic.Map.GetNPC(oldChunk, chunk);
                                        nom.Write(chunkArr);
                                        nom.Write(npcArr);
                                        server.SendMessage(nom, msg.SenderConnection, NetDeliveryMethod.ReliableUnordered);
                                    }

                                    if (!logic.UpdatePlayer(msg.SenderConnection.RemoteUniqueIdentifier, &chunk, &tile, rot))
                                    {
                                        nom = server.CreateMessage();
                                        nom.Write((byte)NDT.HeroUpdate);
                                        nom.Write(&chunk);
                                        nom.Write(&tile);
                                        server.SendMessage(nom, msg.SenderConnection, NetDeliveryMethod.ReliableOrdered);
                                    }
                                    break;
                                case (NDT.Attack):
                                    logic.PlayerAttack(id);
                                    break;
                            }
                        }
                        catch (Exception e)
                        {
                            WriteLine(NIMT.Error, "An error occured wile reading a {0} message. Innerexception: {1}", type, e);
                        }
                        break;
                }
            }

            logic.Update(time.DeltaTime);

            if (timeDiff >= Res.FPS30 && logic.Index > 0)
            {
                for (int i = 0; i < server.Connections.Count; i++)
                {
                    NetConnection conn = server.Connections[i];
                    KeyValuePair<long, Creature> cur = logic.Players.First(p => p.Key == conn.RemoteUniqueIdentifier);
                    NOM nom = server.CreateMessage();

                    nom.Write((byte)NDT.Update);
                    nom.Write(ref logic.Players, logic.Index, cur.Key);
                    nom.Write(logic.Map.GetNPC(logic.GetPlayer(conn.RemoteUniqueIdentifier).ChunkPos));
                    server.SendMessage(nom, conn, NetDeliveryMethod.ReliableOrdered);
                }

                timeDiff = 0;
            }

            timeDiff += time.DeltaTime;
        }

        private void Server_Draw(GameTime time)
        {
            UpdateStats();
        }

        #region User Interface

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
            InvokeIfRequired(dGrid_Connections, () => players_UI.Add(ip, dGrid_Connections.Rows.Add(name, ip)));
        }

        public void RemovePlayer(IPAddress ip)
        {
            InvokeIfRequired(dGrid_Connections, () => dGrid_Connections.Rows.RemoveAt(players_UI[ip]));
            players_UI.Remove(ip);
        }

        public void ClearPlayers()
        {
            InvokeIfRequired(dGrid_Connections, () => dGrid_Connections.Rows.Clear());
            players_UI.Clear();
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
                    lbl_LastMessage.Text = "Last info message: '" + string.Format(format, args) + "'";
                    color = Color.LightBlue;
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
            string line = string.Format("[{0}][Data] {1}", string.Format("{0:H:mm:ss}", DateTime.Now), string.Format(format, args));

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
            catch (Exception e)
            {
                if (!control.IsDisposed) WriteLine(NIMT.Error, "Cannot perform action on '{0}'. \nInnterException: {1}", control.Name, e);
            }
        }

        #region Controls

        private void InitializeComponent()
        {
            this.statusStrip = new StatusStrip();
            this.lbl_Status = new ToolStripStatusLabel();
            this.lbl_LastMessage = new ToolStripStatusLabel();
            this.gBox_Info = new GroupBox();
            this.lbl_CPU = new Label();
            this.proBarCPU = new ProgressBar();
            this.btn_Stop = new Button();
            this.btn_Restart = new Button();
            this.dGrid_Connections = new DataGridView();
            this.coll_Name = new DataGridViewTextBoxColumn();
            this.coll_Ip = new DataGridViewTextBoxColumn();
            this.txt_Console = new RichTextBox();
            this.splitContainer1 = new SplitContainer();
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

        private void btn_Restart_Click(object sender, EventArgs e)
        {
            if (server.Status == NetPeerStatus.NotRunning)
            {
                server.Start();
                WriteLine(NIMT.Data, "Server restarting.");
            }
        }

        private void btn_Stop_Click(object sender, EventArgs e)
        {
            players_Queue = new Dictionary<long, string>();
            ClearPlayers();

            server.Shutdown(string.Format("The {0} server has shut down.", Res.AppName));
            WriteLine(NIMT.Data, "Server closing.");
        }

        private void txt_Console_MouseDown(object sender, MouseEventArgs e)
        {
            HideCaret(txt_Console.Handle);
        }

        #endregion
    }
}