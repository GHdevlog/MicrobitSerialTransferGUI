using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;
using System.Security.Policy;

namespace MicrobitSerialTransferGUI
{
    public partial class Form1 : Form
    {
        string dataOutPut;
        string sendwith;
        string dataIN;
        string dataBuffer = string.Empty;


        public Form1()
        {
            InitializeComponent();
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            // 초기 설정 값 지정
            string[] ports = SerialPort.GetPortNames();
            cBoxCOMPORT.Items.AddRange(ports);

            chBoxDtrEnable.Checked = false;
            serialPort1.DtrEnable = false;

            chBoxRtsEnable.Checked = false;
            serialPort1.RtsEnable = false ;

            btnSendData.Enabled = true;

            sendwith = "Both";

            toolStripComboBox1.Text = "Add To Old Data";
            toolStripComboBox2.Text = "Both";
            toolStripComboBox3.Text = "BOTTOM";

        }
        private void oPENToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                serialPort1.PortName = cBoxCOMPORT.Text;
                serialPort1.BaudRate = Convert.ToInt32(cBoxBaudRate.Text);
                serialPort1.DataBits = Convert.ToInt32(cBoxDataBits.Text);
                serialPort1.StopBits = (StopBits)Enum.Parse(typeof(StopBits), cBoxStopBits.Text);
                serialPort1.Parity = (Parity)Enum.Parse(typeof(Parity), cBoxParityBits.Text);

                serialPort1.Open();
                progressBar1.Value = 100;

            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void cLOSEToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (serialPort1.IsOpen)
            {
                serialPort1.Close();
                progressBar1.Value = 0;

            }
        }


        private void btnSendData_Click(object sender, EventArgs e)
        {
            if (serialPort1.IsOpen)
            {
                dataOutPut = tBoxDataOut.Text;

                if (sendwith == "None")
                {
                    serialPort1.Write(dataOutPut);
                }
                else if (sendwith == "Both")
                {
                    serialPort1.WriteLine(dataOutPut + "\r\n");
                }
                else if (sendwith == "New Line")
                {
                    serialPort1.WriteLine(dataOutPut + "\n");
                }
                else if (sendwith == "Carriage Return")
                {
                    serialPort1.WriteLine(dataOutPut + "\r");
                }
            }
        }

        private void toolStripComboBox2_DropDownClosed(object sender, EventArgs e)
        {
            if (toolStripComboBox2.Text == "None")
            {
                sendwith = "None";
            }
            else if (toolStripComboBox2.Text == "Both")
            {
                sendwith = "Both";
            }
            else if (toolStripComboBox2.Text == "New Line")
            {
                sendwith = "New Line";
            }
            else if (toolStripComboBox2.Text == "Carriage Return")
            {
                sendwith = "Carriage Return";
            }
        }

        private void clearToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (tBoxDataIN.Text != "")
            {
                tBoxDataIN.Text = "";
            }
        }

        private void chBoxDtrEnable_CheckedChanged(object sender, EventArgs e)
        {
            if(chBoxDtrEnable.Checked)
            {
                serialPort1.DtrEnable = true;
                MessageBox.Show("DTR Enalbed", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                serialPort1.DtrEnable = false;
            }
        }

        private void chBoxRtsEnable_CheckedChanged(object sender, EventArgs e)
        {
            if (chBoxRtsEnable.Checked)
            {
                serialPort1.RtsEnable = true;
                MessageBox.Show("RTS Enalbed", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                serialPort1.RtsEnable = false;
            }
        }

        private void tBoxDataOut_TextChanged(object sender, EventArgs e)
        {
            int dataOUTLength = tBoxDataOut.TextLength;
            lblDataOutLength.Text = string.Format("{0:00}", dataOUTLength);

        }

        private void tBoxDataOut_KeyDown(object sender, KeyEventArgs e)
        {
            
            if(e.KeyCode == Keys.Enter)
            {
                this.doSomething();
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
            
        }

        private void doSomething()
        {
            dataOutPut = tBoxDataOut.Text;
            if (toolStripComboBox2.Text == "None")
            {
                sendwith = "None";
            }
            else if (toolStripComboBox2.Text == "Both")
            {
                sendwith = "Both";
            }
            else if (toolStripComboBox2.Text == "New Line")
            {
                sendwith = "New Line";
            }
            else if (toolStripComboBox2.Text == "Carriage Return")
            {
                sendwith = "Carriage Return";
            }
        }

        private void serialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            dataIN = serialPort1.ReadExisting();
            dataBuffer += dataIN;
            this.Invoke(new EventHandler(ProcessData));
        }

        private void ProcessData(object sender, EventArgs e)
        {
            while (dataBuffer.Contains("\r\n"))
            {
                int endOfPacket = dataBuffer.IndexOf("\r\n");
                string completePacket = dataBuffer.Substring(0, endOfPacket+2);
                dataBuffer = dataBuffer.Substring(endOfPacket + 2);

                ShowData(completePacket);
            }
        }

        private void ShowData(string data)
        {
            int dataINLength = data.Length;
            lblDataInLength.Text = string.Format("{0:00}", dataINLength);

            if (toolStripComboBox1.Text == "Always Update")
            {
                tBoxDataIN.Text = data;
            }
            else if (toolStripComboBox1.Text == "Add To Old Data")
            {
                if (toolStripComboBox3.Text == "TOP")
                {
                    tBoxDataIN.Text = tBoxDataIN.Text.Insert(0, data);
                }
                else if (toolStripComboBox3.Text == "BOTTOM")
                {
                    tBoxDataIN.AppendText(data);
                    tBoxDataIN.SelectionStart = tBoxDataIN.Text.Length;
                    tBoxDataIN.ScrollToCaret();
                }
            }
        }

        private void btnClearDataIN_Click(object sender, EventArgs e)
        {
            if (tBoxDataIN.Text != "")
            {
                tBoxDataIN.Text = "";
            }
        }

        private void clearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (tBoxDataOut.Text != "")
            {
                tBoxDataOut.Text = "";
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
