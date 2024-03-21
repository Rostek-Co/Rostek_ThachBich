using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Text;
using System.IO;
using System.IO.Ports;
using System.Threading;
using System.Windows.Forms;

using System.Runtime.InteropServices;
using static PreviewDemo.CHCNetSDK;


namespace PreviewDemo
{
    /// <summary>
    /// Form1 µÄÕªÒªËµÃ÷¡£
    /// </summary>
    public class Preview : System.Windows.Forms.Form
    {
        private uint iLastErr = 0;
        private Int32 m_lUserID = -1;
        private bool m_bInitSDK = false;
        private bool m_bRecord = false;
        private bool m_bTalk = false;
        private Int32 m_lRealHandle = -1;
        private int lVoiceComHandle = -1;
        private string str;

        private SerialPort serialPort;
        private bool isRunning = false;
        private string receivedData = "";
        private DateTime lastDataReceivedTime;

        CHCNetSDK.NET_DVR_PREVIEWINFO lpPreviewInfo;
        CHCNetSDK.REALDATACALLBACK RealData = null;
        public CHCNetSDK.NET_DVR_PTZPOS m_struPtzCfg;
        private System.Windows.Forms.PictureBox RealPlayWnd;
        private Button btn_Exit;
        private Panel BarcodeHistory;
        private Label label15;
        private TextBox txtResult;
        private Panel UserInfomation;
        private Label label18;
        private Button btnClose;
        private Button btnStart;
        private ComboBox cboPorts;
        private Label label24;
        private TextBox username_textbox;

        //private GroupBox groupBox1;

        private System.ComponentModel.Container components = null;

        public Preview()
        {
            InitializeComponent();
            m_bInitSDK = CHCNetSDK.NET_DVR_Init();
            if (m_bInitSDK == false)
            {
                MessageBox.Show("NET_DVR_Init error!");
                return;
            }
            else
            {
                //To save the SDK log
                CHCNetSDK.NET_DVR_SetLogToFile(3, "C:\\SdkLog\\", true);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (m_lRealHandle >= 0)
            {
                CHCNetSDK.NET_DVR_StopRealPlay(m_lRealHandle);
            }
            if (m_lUserID >= 0)
            {
                CHCNetSDK.NET_DVR_Logout(m_lUserID);
            }
            if (m_bInitSDK == true)
            {
                CHCNetSDK.NET_DVR_Cleanup();
            }
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        #region Windows ´°ÌåÉè¼ÆÆ÷Éú³ÉµÄ´úÂë

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Preview));
            this.RealPlayWnd = new System.Windows.Forms.PictureBox();
            this.btn_Exit = new System.Windows.Forms.Button();
            this.BarcodeHistory = new System.Windows.Forms.Panel();
            this.label15 = new System.Windows.Forms.Label();
            this.txtResult = new System.Windows.Forms.TextBox();
            this.UserInfomation = new System.Windows.Forms.Panel();
            this.label18 = new System.Windows.Forms.Label();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnStart = new System.Windows.Forms.Button();
            this.cboPorts = new System.Windows.Forms.ComboBox();
            this.label24 = new System.Windows.Forms.Label();
            this.username_textbox = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.RealPlayWnd)).BeginInit();
            this.BarcodeHistory.SuspendLayout();
            this.UserInfomation.SuspendLayout();
            this.SuspendLayout();
            // 
            // RealPlayWnd
            // 
            this.RealPlayWnd.BackColor = System.Drawing.SystemColors.WindowText;
            this.RealPlayWnd.Location = new System.Drawing.Point(785, 10);
            this.RealPlayWnd.Name = "RealPlayWnd";
            this.RealPlayWnd.Size = new System.Drawing.Size(413, 376);
            this.RealPlayWnd.TabIndex = 4;
            this.RealPlayWnd.TabStop = false;
            // 
            // btn_Exit
            // 
            this.btn_Exit.Font = new System.Drawing.Font("Segoe UI", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_Exit.Location = new System.Drawing.Point(390, 328);
            this.btn_Exit.Name = "btn_Exit";
            this.btn_Exit.Size = new System.Drawing.Size(62, 30);
            this.btn_Exit.TabIndex = 11;
            this.btn_Exit.Tag = "";
            this.btn_Exit.Text = "Thoát";
            this.btn_Exit.UseVisualStyleBackColor = true;
            this.btn_Exit.Click += new System.EventHandler(this.btn_Exit_Click);
            // 
            // BarcodeHistory
            // 
            this.BarcodeHistory.Controls.Add(this.label15);
            this.BarcodeHistory.Controls.Add(this.txtResult);
            this.BarcodeHistory.Location = new System.Drawing.Point(10, 10);
            this.BarcodeHistory.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.BarcodeHistory.Name = "BarcodeHistory";
            this.BarcodeHistory.Size = new System.Drawing.Size(770, 376);
            this.BarcodeHistory.TabIndex = 38;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Font = new System.Drawing.Font("Segoe UI", 16.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label15.Location = new System.Drawing.Point(8, 6);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(98, 30);
            this.label15.TabIndex = 17;
            this.label15.Text = "Barcode";
            // 
            // txtResult
            // 
            this.txtResult.Location = new System.Drawing.Point(14, 62);
            this.txtResult.Name = "txtResult";
            this.txtResult.Size = new System.Drawing.Size(742, 20);
            this.txtResult.TabIndex = 2;
            this.txtResult.TextChanged += new System.EventHandler(this.txtResult_TextChanged);
            // 
            // UserInfomation
            // 
            this.UserInfomation.Controls.Add(this.label18);
            this.UserInfomation.Controls.Add(this.btnClose);
            this.UserInfomation.Controls.Add(this.btn_Exit);
            this.UserInfomation.Controls.Add(this.btnStart);
            this.UserInfomation.Controls.Add(this.cboPorts);
            this.UserInfomation.Controls.Add(this.label24);
            this.UserInfomation.Controls.Add(this.username_textbox);
            this.UserInfomation.Location = new System.Drawing.Point(1202, 10);
            this.UserInfomation.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.UserInfomation.Name = "UserInfomation";
            this.UserInfomation.Size = new System.Drawing.Size(465, 376);
            this.UserInfomation.TabIndex = 39;
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Font = new System.Drawing.Font("Segoe UI", 16.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label18.Location = new System.Drawing.Point(37, 5);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(183, 30);
            this.label18.TabIndex = 18;
            this.label18.Text = "Tên Người Dùng";
            // 
            // btnClose
            // 
            this.btnClose.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnClose.Location = new System.Drawing.Point(176, 284);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(134, 64);
            this.btnClose.TabIndex = 10;
            this.btnClose.Text = "Dừng";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnStart
            // 
            this.btnStart.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnStart.Location = new System.Drawing.Point(176, 194);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(134, 63);
            this.btnStart.TabIndex = 9;
            this.btnStart.Text = "Khởi Động";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // cboPorts
            // 
            this.cboPorts.FormattingEnabled = true;
            this.cboPorts.Location = new System.Drawing.Point(176, 136);
            this.cboPorts.Name = "cboPorts";
            this.cboPorts.Size = new System.Drawing.Size(134, 21);
            this.cboPorts.TabIndex = 8;
            this.cboPorts.SelectedIndexChanged += new System.EventHandler(this.cboPorts_SelectedIndexChanged);
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.Font = new System.Drawing.Font("Segoe UI", 16.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label24.Location = new System.Drawing.Point(98, 124);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(58, 30);
            this.label24.TabIndex = 4;
            this.label24.Text = "Port";
            // 
            // username_textbox
            // 
            this.username_textbox.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.username_textbox.Location = new System.Drawing.Point(42, 62);
            this.username_textbox.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.username_textbox.Name = "username_textbox";
            this.username_textbox.Size = new System.Drawing.Size(371, 26);
            this.username_textbox.TabIndex = 1;
            // 
            // Preview
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(1924, 1055);
            this.Controls.Add(this.UserInfomation);
            this.Controls.Add(this.BarcodeHistory);
            this.Controls.Add(this.RealPlayWnd);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Preview";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "PalletWrapper_PC2";
            this.Load += new System.EventHandler(this.Preview_Load);
            ((System.ComponentModel.ISupportInitialize)(this.RealPlayWnd)).EndInit();
            this.BarcodeHistory.ResumeLayout(false);
            this.BarcodeHistory.PerformLayout();
            this.UserInfomation.ResumeLayout(false);
            this.UserInfomation.PerformLayout();
            this.ResumeLayout(false);

        }
        #endregion

        [STAThread]
        static void Main()
        {
            Application.Run(new Preview());
        }

        private void textBox1_TextChanged(object sender, System.EventArgs e)
        {

        }

        private void btn_Exit_Click(object sender, EventArgs e)
        {
            //Stop live view 
            if (m_lRealHandle >= 0)
            {
                CHCNetSDK.NET_DVR_StopRealPlay(m_lRealHandle);
                m_lRealHandle = -1;
            }

            //×¢ÏúµÇÂ¼ Logout the device
            if (m_lUserID >= 0)
            {
                CHCNetSDK.NET_DVR_Logout(m_lUserID);
                m_lUserID = -1;
            }

            CHCNetSDK.NET_DVR_Cleanup();
            serialPort.Close();

            Application.Exit();
        }

        private void Preview_Load(object sender, EventArgs e)
        {
            serialPort = new SerialPort();
            serialPort.DataReceived += SerialPort_DataReceived;
            string[] ports = SerialPort.GetPortNames();
            cboPorts.Items.AddRange(ports);

            cboPorts.SelectedIndex = -1;
            btnClose.Enabled = false;
            txtResult.Enabled = false;
            lastDataReceivedTime = DateTime.Now;

        }

        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            receivedData += serialPort.ReadExisting();
            lastDataReceivedTime = DateTime.Now;

            if (receivedData.Contains("!"))
            {
                ProcessReceivedData();
            }
        }

        private void ProcessReceivedData()
        {
            if (receivedData.StartsWith("01:"))
            {
                receivedData = "";
            }
            else if (receivedData.StartsWith("02:"))
            {
                string dataToShow = receivedData.Substring(3, receivedData.Length - 4);
                DisplayData(dataToShow);
                receivedData = "";
            }
        }

        private void DisplayData(string data)
        {
            if (txtResult.InvokeRequired)
            {
                txtResult.Invoke((MethodInvoker)delegate
                {
                    txtResult.Text = data;
                });
            }
            else
            {
                txtResult.Text = data;
            }
        }


        private void btnStart_Click(object sender, EventArgs e)
        {
            if (cboPorts.SelectedIndex == -1)
            {
                MessageBox.Show("Vui lòng chọn cổng COM trước khi bắt đầu.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                btnStart.Enabled = true;
                return;
            }

            string selectedPort = cboPorts.SelectedItem.ToString();
            serialPort.PortName = selectedPort;
            serialPort.BaudRate = 9600;
            serialPort.Open();
            btnStart.Enabled = false;
            btnClose.Enabled = true;

            MessageBox.Show("Kết nối thành công.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            txtResult.Enabled = true;

            isRunning = true;

            Thread dataCheckThread = new Thread(DataCheckThread);
            dataCheckThread.Start();

            if (Login.Default.IP == "" || Login.Default.Port.ToString() == "" ||
                Login.Default.UserName == "" || Login.Default.Password == "")
            {
                MessageBox.Show("Please input IP, Port, User name, and Password!");
                return;
            }

            if (m_lUserID < 0)
            {
                string DVRIPAddress = Login.Default.IP;
                Int16 DVRPortNumber = (short)Login.Default.Port;
                string DVRUserName = Login.Default.UserName;
                string DVRPassword = Login.Default.Password;

                CHCNetSDK.NET_DVR_DEVICEINFO_V30 DeviceInfo = new CHCNetSDK.NET_DVR_DEVICEINFO_V30();

                //Login the device
                m_lUserID = CHCNetSDK.NET_DVR_Login_V30(DVRIPAddress, DVRPortNumber, DVRUserName, DVRPassword, ref DeviceInfo);
                if (m_lUserID < 0)
                {
                    iLastErr = CHCNetSDK.NET_DVR_GetLastError();
                    str = "NET_DVR_Login_V30 failed, error code= " + iLastErr;
                    MessageBox.Show(str);
                    return;
                }
                else
                {
                    MessageBox.Show("Login Success!");
                }

            }

            CHCNetSDK.NET_DVR_PREVIEWINFO lpPreviewInfo = new CHCNetSDK.NET_DVR_PREVIEWINFO();
            lpPreviewInfo.hPlayWnd = RealPlayWnd.Handle;
            lpPreviewInfo.lChannel = Login.Default.lChannel;
            lpPreviewInfo.dwStreamType = 0;
            lpPreviewInfo.dwLinkMode = 0;
            lpPreviewInfo.bBlocked = true;
            lpPreviewInfo.dwDisplayBufNum = 1;
            lpPreviewInfo.byProtoType = 0;
            lpPreviewInfo.byPreviewMode = 0;


            if (Login.Default.Stream_id != "")
            {
                lpPreviewInfo.lChannel = -1;
                byte[] byStreamID = System.Text.Encoding.Default.GetBytes(Login.Default.Stream_id);
                lpPreviewInfo.byStreamID = new byte[32];
                byStreamID.CopyTo(lpPreviewInfo.byStreamID, 0);
            }

            if (RealData == null)
            {
                RealData = new CHCNetSDK.REALDATACALLBACK(RealDataCallBack);
            }
            IntPtr pUser = new IntPtr();
            m_lRealHandle = CHCNetSDK.NET_DVR_RealPlay_V40(m_lUserID, ref lpPreviewInfo, null, pUser);
            if (m_lRealHandle < 0)
            {
                var iLastErr = CHCNetSDK.NET_DVR_GetLastError();
                var str = "NET_DVR_RealPlay_V40 failed, error code= " + iLastErr;
                MessageBox.Show(str);
                return;
            }
        }

        public void RealDataCallBack(Int32 lRealHandle, UInt32 dwDataType, IntPtr pBuffer, UInt32 dwBufSize, IntPtr pUser)
        {
            if (dwBufSize > 0)
            {
                byte[] sData = new byte[dwBufSize];
                Marshal.Copy(pBuffer, sData, 0, (Int32)dwBufSize);

                string str = "ÊµÊ±Á÷Êý¾Ý.ps";
                FileStream fs = new FileStream(str, FileMode.Create);
                int iLen = (int)dwBufSize;
                fs.Write(sData, 0, iLen);
                fs.Close();
            }
        }


        private void DataCheckThread()
        {
            int timeoutSeconds = 10;

            while (isRunning)
            {
                TimeSpan elapsedTime = DateTime.Now - lastDataReceivedTime;

                if (elapsedTime.TotalSeconds > timeoutSeconds)
                {
                    Invoke((MethodInvoker)delegate
                    {
                        MessageBox.Show("Không nhận được dữ liệu.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    });
                }

                Thread.Sleep(1000);
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            btnStart.Enabled = true;
            btnClose.Enabled = false;
            isRunning = false;
            try
            {
                serialPort.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (serialPort.IsOpen)
            {
                serialPort.Close();
                isRunning = false;
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            serialPort.Close();
        }


        private void Ptz_Set_Click(object sender, EventArgs e)
        {

        }

        private void PreSet_Click(object sender, EventArgs e)
        {
            PreSet dlg = new PreSet();
            dlg.m_lUserID = m_lUserID;
            dlg.m_lChannel = 1;
            dlg.m_lRealHandle = m_lRealHandle;
            dlg.ShowDialog();
            
        }

        private void cboPorts_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void txtResult_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
