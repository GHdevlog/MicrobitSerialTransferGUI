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

namespace MicrobitSerialTransferGUI
{
    public partial class Form1 : Form
    {
        string dataOutPut;
        string sendwith;


        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            string[] ports = SerialPort.GetPortNames();
            cBoxCOMPORT.Items.AddRange(ports);

            btnOpen.Enabled = true;
            btnClose.Enabled = false;

            chBoxDtrEnable.Checked = false;
            serialPort1.DtrEnable = false;

            chBoxRtsEnable.Checked = false;
            serialPort1.RtsEnable = false ;

            btnSendData.Enabled = false;

            chBoxWriteLine.Checked = false;
            chBoxWrite.Checked = true;
            sendwith = "Write";

        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            try
            {
                serialPort1.PortName = cBoxCOMPORT.Text;
                serialPort1.BaudRate = Convert.ToInt32(cBoxBaudRate.Text);
                serialPort1.DataBits = Convert.ToInt32(cBoxDataBits.Text);
                serialPort1.StopBits = (StopBits)Enum.Parse(typeof(StopBits), cBoxStopBits.Text);
                serialPort1.Parity =  (Parity)Enum.Parse(typeof(Parity), cBoxParityBits.Text);

                serialPort1.Open();
                progressBar1.Value = 100;

                lblSatausCom.Text = "ON";

                btnOpen.Enabled = false;
                btnClose.Enabled=true;
            } 
            catch (Exception err)
            {
                MessageBox.Show(err.Message,"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                btnOpen.Enabled = true;
                btnClose.Enabled = false;
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            if (serialPort1.IsOpen)
            {
                serialPort1.Close();
                progressBar1.Value=0;

                lblSatausCom.Text = "OFF";

                btnOpen.Enabled = true;
                btnClose.Enabled = false;

            }
        }

        private void btnSendData_Click(object sender, EventArgs e)
        {
            if (serialPort1.IsOpen)
            {
                dataOutPut = tBoxDataOut.Text;

                if (sendwith == "Write")
                {
                    serialPort1.Write(dataOutPut);
                }else if (sendwith == "WriteLine"){
                    serialPort1.WriteLine(dataOutPut);
                }
            }
        }

        private void btnClearDataOut_Click(object sender, EventArgs e)
        {
            if(tBoxDataOut.Text != "")
            {
                tBoxDataOut.Text = "";
            }
        }

        private void chBoxDtrEnable_CheckedChanged(object sender, EventArgs e)
        {
            if(chBoxDtrEnable.Checked)
            {
                serialPort1.DtrEnable = true;
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

            if (chBoxUsingEnter.Checked)
            {
                tBoxDataOut.Text = tBoxDataOut.Text.Replace(Environment.NewLine, "");
            }
        }

        private void chBoxUsingButton_CheckedChanged(object sender, EventArgs e)
        {
            if(chBoxUsingButton.Checked) { 
                btnSendData.Enabled = true;
            }
            else
            {
                btnSendData.Enabled= false;
            }
        }

        private void tBoxDataOut_KeyDown(object sender, KeyEventArgs e)
        {
            if (chBoxUsingEnter.Checked)
            {
                if(e.KeyCode == Keys.Enter)
                {
                    dataOutPut = tBoxDataOut.Text;

                    if (sendwith == "Write")
                    {
                        serialPort1.Write(dataOutPut);
                    }
                    else if (sendwith == "WriteLine")
                    {
                        serialPort1.WriteLine(dataOutPut);
                    }
                }
            }
        }

        private void chBoxWriteLine_CheckedChanged(object sender, EventArgs e)
        {
            if (chBoxUsingEnter.Checked) {
                sendwith = "WriteLine";
                chBoxWrite.Enabled = false;
                chBoxWriteLine.Enabled = true;
            }
        }

        private void chBoxWrite_CheckedChanged(object sender, EventArgs e)
        {
            if (chBoxUsingEnter.Checked)
            {
                sendwith = "Write";
                chBoxWrite.Enabled = true;
                chBoxWriteLine.Enabled = false;
            }
        }
    }
}
