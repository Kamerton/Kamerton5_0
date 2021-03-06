using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.IO.Ports;
using System.Globalization;
using System.Threading;
using Kamerton5_0;
using System.Linq;
using FieldTalk.Modbus.Master;
//using System.Runtime.Serialization.Formatters.Binary;


// ������� ��������� MODBUS

//  res = myProtocol.readCoils(slave, startCoil, coilArr, numCoils);                  // 01 ������� ���  false ��� true �� ������ 00000 - 09999
//  res = myProtocol.writeCoil(slave, startCoil, true);                               // 05   �������� ��� false ��� true �� ������ 00000 - 09999
//  res = myProtocol.forceMultipleCoils(slave, startCoil, coilVals, numCoils);        // 15 (0F) �������� ��� false ��� true  �� ������ 0-9999 
//  res = myProtocol.readInputDiscretes(slave, startCoil, coilArr, numCoils);         // 02  ������� ���  0 ��� 1 �� ������ 10000 - 19999
//  res = myProtocol.readInputRegisters(slave, startRdReg, readVals, numRdRegs);      // 04  ������� ���  0 ��� 1 �� ������ 30000 - 39999
//  res = myProtocol.readMultipleRegisters(slave, startRdReg, readVals, numRdRegs);   // 03  ������� ����� �� ��������� �� ������  40000 -49999
//  res = myProtocol.writeMultipleRegisters(slave, startWrReg, writeVals, numWrRegs); // 16 (10Hex)�������� � �������� ����� �� ������ 40000 -49999
//  res = myProtocol.writeSingleRegister(int slaveAddr,int regAddr, ushort regVal)    // 06  �������� � ������� ����� �� ������ 40000 -49999


namespace KamertonTest
{

    public partial class Form1 : Form
    {

        private MbusMasterFunctions myProtocol;

        private int slave;
        private int startCoil;
        private int numCoils;
        private int startWrReg;
        private int numRdRegs;
        private int startRdReg;
        private int res;
        private int TestN;
       // private int TestSum;
        private int TestStep;
        private int TestRepeatCount;
        private int _SerialMonitor;
        static bool _All_Test_Stop = true;
        float temp_disp;
        ushort[] readVals_all = new ushort[200];
        ushort[] readVolt_all = new ushort[200];
        private int[] test_step = new int[20];

        bool[] coilArr_all = new bool[200];
        bool portFound = false;
        bool testAllRun = false;
        //bool power_radio1 = false;
        //bool power_radio2 = false;
        //bool power_ggs = false;
        SerialPort currentPort;

        public Form1()
        {
            InitializeComponent();
            LoadListboxes();
        }
 
        //SerialPort _serialPort = new SerialPort("COM4",
        //                                9600,
        //                                Parity.None,
        //                                8,
        //                                StopBits.One);

        const string fileName = "Kamerton log.txt";


        private void Form1_Load(object sender, EventArgs e)
        {
            ToolTip1.SetToolTip(txtPollDelay, "�������� � ������������� ����� ����� ����������������� ���������� Modbus, 0 ��� ����������");
            ToolTip1.SetToolTip(cmbRetry, "������� ��� ��������� ��������, ���� � ������ ��� �� ������?");
            ToolTip1.SetToolTip(cmbSerialProtocol, "����� ��������� COM: ASCII ��� RTU");
            ToolTip1.SetToolTip(cmbTcpProtocol, "����� ��������� Ethernet: MODBUS/TCP ��� Encapsulated RTU ��� TCP");
            //   ToolTip1.SetToolTip(textBox1, "����� �������� ��������� � �������������");
            cmbComPort.SelectedIndex = 0;
            cmbParity.SelectedIndex = 0;
            cmbStopBits.SelectedIndex = 0;
            cmbDataBits.SelectedIndex = 0;
            cmbBaudRate.SelectedIndex = 5;
            cmbSerialProtocol.SelectedIndex = 0;
            cmbTcpProtocol.SelectedIndex = 0;
            cmbRetry.SelectedIndex = 2;
            cmbCommand.SelectedIndex = 0;
            Polltimer1.Enabled = false;
            timer_byte_set.Enabled = false;
            timer_Mic_test.Enabled = false;
            timerCTS.Enabled = false;
            timerTestAll.Enabled = false;
            find_com_port.Enabled = false;
            radioButton1.Checked = true;
            serviceSet();
            _SerialMonitor = 0;
            // _serialPort.Handshake = Handshake.None;
            // _serialPort.DataReceived += new SerialDataReceivedEventHandler(sp_DataReceived);
            // _serialPort.ReadTimeout  = 500;
            // _serialPort.WriteTimeout = 500;
            // if (!(_serialPort.IsOpen))
            // _serialPort.Open();
          //  findComPort();
           // serial_connect();
            SetComPort();
            Polltimer1.Enabled = true;
        }

        private delegate void SetTextDeleg(string text);                  //             

        void sp_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            Thread.Sleep(500);
            //  string data = _serialPort.ReadLine();
            //  ����������� �������� �� ������ UI, � �������� ������, �������
            //  ���� ������� ������������ �������.
            //  ---- ����� "si_DataReceived" ����� �������� � ������ UI,
            //  ������� �������� ��������� ��������� ���� TextBox.
            //  this.BeginInvoke(new SetTextDeleg(si_DataReceived), new object[] { data });
        }

        private void serviceSet()
        {
            checkBoxSenAll.Checked = true;
        }

        private void si_DataReceived(string data)
        {
            switch (_SerialMonitor) // ������������� ������� ������ ���������� � Serial
            {
                default:
                case 0:
                    //textBox9.Text += (data.Trim() + "\r\n");
                    //textBox9.Refresh();
                    textBox11.Text += (data.Trim() + "\r\n");
                    textBox11.Refresh();
                    break;
                case 1:
                    //textBox9.Text += (data.Trim() + "\r\n");
                    //textBox9.Refresh();
                    break;
                case 2:
                    break;
                case 3:
                    textBox11.Text += (data.Trim() + "\r\n");
                    textBox11.Refresh();
                    break;
            }
        }

        private void cmdOpenSerial_Click(object sender, EventArgs e)
        {

            //
            // First we must instantiate class if we haven't done so already
            //
            if ((myProtocol == null))
            {
                try
                {
                    if ((cmbSerialProtocol.SelectedIndex == 0))
                        myProtocol = new MbusRtuMasterProtocol(); // RTU
                    else
                        myProtocol = new MbusAsciiMasterProtocol(); // ASCII
                }
                catch (OutOfMemoryException ex)
                {
                    lblResult.Text = (" ������ ����" + ex.Message);
                    label78.Text = ("�� ������� ������� ��������� ������ ��������� ���������!" + ex.Message);
                    return;
                }
            }
            else // already instantiated, close protocol, reinstantiate
            {
                if (myProtocol.isOpen())
                    myProtocol.closeProtocol();
                    myProtocol = null;
                try
                {
                    if ((cmbSerialProtocol.SelectedIndex == 0))
                        myProtocol = new MbusRtuMasterProtocol(); // RTU
                    else
                        myProtocol = new MbusAsciiMasterProtocol(); // ASCII
                }
                catch (OutOfMemoryException ex)
                {
                    lblResult.Text = (" ������ ����" + ex.Message);
                    label78.Text = ("�� ������� ������� ��������� ������ ��������� ���������!" + ex.Message);
                    return;
                }
            }
            //
            // Here we configure the protocol
            // ����� �� �������� ��������
            int retryCnt;
            int pollDelay;
            int timeOut;
            int baudRate;
            int parity;
            int dataBits;
            int stopBits;
            int res;
            try
            {
                retryCnt = int.Parse(cmbRetry.Text, CultureInfo.CurrentCulture);
            }
            catch (Exception)
            {
                retryCnt = 2;
            }
            try
            {
                pollDelay = int.Parse(txtPollDelay.Text, CultureInfo.CurrentCulture);
            }
            catch (Exception)
            {
                pollDelay = 10;
            }
            try
            {
                timeOut = int.Parse(txtTimeout.Text, CultureInfo.CurrentCulture);
            }
            catch (Exception)
            {
                timeOut = 1000;
            }
            try
            {
                baudRate = int.Parse(cmbBaudRate.Text, CultureInfo.CurrentCulture);
            }
            catch (Exception)
            {
                baudRate = 57600;
            }
            switch (cmbParity.SelectedIndex)
            {
                default:
                case 0:
                    parity = MbusSerialMasterProtocol.SER_PARITY_NONE;
                    break;
                case 1:
                    parity = MbusSerialMasterProtocol.SER_PARITY_EVEN;
                    break;
                case 2:
                    parity = MbusSerialMasterProtocol.SER_PARITY_ODD;
                    break;
            }
            switch (cmbDataBits.SelectedIndex)
            {
                default:
                case 0:
                    dataBits = MbusSerialMasterProtocol.SER_DATABITS_8;
                    break;
                case 1:
                    dataBits = MbusSerialMasterProtocol.SER_DATABITS_7;
                    break;
            }
            switch (cmbStopBits.SelectedIndex)
            {
                default:
                case 0:
                    stopBits = MbusSerialMasterProtocol.SER_STOPBITS_1;
                    break;
                case 1:
                    stopBits = MbusSerialMasterProtocol.SER_STOPBITS_2;
                    break;
            }
            myProtocol.timeout = timeOut;
            myProtocol.retryCnt = retryCnt;
            myProtocol.pollDelay = pollDelay;
            // Note: The following cast is required as the myProtocol object is declared
            // as the superclass of MbusSerialMasterProtocol. That way myProtocol can
            // represent different protocol types.
            // ����������: � ��������� �������� ��������� ��� ������ myProtocol ��������
            // ��� ���������� MbusSerialMasterProtocol. ����� ������� myProtocol �����
            // ������������ ��������� ���� ����������.

            res = ((MbusSerialMasterProtocol)(myProtocol)).openProtocol(cmbComPort.Text, baudRate, dataBits, stopBits, parity);
            if ((res == BusProtocolErrors.FTALK_SUCCESS))
            {
                label78.Text = ("���������������� ���� ������� ������ � �����������:  "
                            + (cmbComPort.Text + (", "
                            + (baudRate + (" baud, "
                            + (dataBits + (" data bits, "
                            + (stopBits + (" stop bits, parity " + parity)))))))));
                button5.Enabled = true;
                Polltimer1.Enabled = true;
            }
            else
            {
                lblResult.Text = (" ������ ����: " + BusProtocolErrors.getBusProtocolErrorText(res));
                label78.Text = ("�� ������� ������� ��������!");
            }
        }

        private void serial_connect()
        {
            //
            // First we must instantiate class if we haven't done so already
            //
            //  CloseButton.Enabled = true;
            //  cmdOpenSerial.Enabled = false;    // ������ "������� Serial"
            //if (portFound == true)
            //{
                if ((myProtocol == null))
                {
                    try
                    {
                        if ((cmbSerialProtocol.SelectedIndex == 0))
                            myProtocol = new MbusRtuMasterProtocol(); // RTU
                        else
                            myProtocol = new MbusAsciiMasterProtocol(); // ASCII
                    }
                    catch (OutOfMemoryException ex)
                    {
                        lblResult.Text = ("�� ������� ������� ��������� ������ ��������� ���������! ������ ���� " + ex.Message);
                        return;
                    }
                }
                else // already instantiated, close protocol, reinstantiate
                {
                    if (myProtocol.isOpen())
                        myProtocol.closeProtocol();
                    myProtocol = null;
                    try
                    {
                        if ((cmbSerialProtocol.SelectedIndex == 0))
                            myProtocol = new MbusRtuMasterProtocol(); // RTU
                        else
                            myProtocol = new MbusAsciiMasterProtocol(); // ASCII
                    }
                    catch (OutOfMemoryException ex)
                    {
                        lblResult.Text = ("�� ������� ������� ��������� ������ ��������� ���������! ������ ���� " + ex.Message);
                        return;
                    }
                }
                //
                // Here we configure the protocol
                //
                short[] readVals = new short[125];
                int retryCnt;
                int pollDelay;
                int timeOut;
                int baudRate;
                int parity;
                int dataBits;
                int stopBits;
                int res;
                int slave;
                int startRdReg;
                int numRdRegs;
                try
                {
                    retryCnt = int.Parse(cmbRetry.Text, CultureInfo.CurrentCulture);
                }
                catch (Exception)
                {
                    retryCnt = 0;
                }
                try
                {
                    pollDelay = int.Parse(txtPollDelay.Text, CultureInfo.CurrentCulture);
                }
                catch (Exception)
                {
                    pollDelay = 0;
                }
                try
                {
                    timeOut = int.Parse(txtTimeout.Text, CultureInfo.CurrentCulture);
                }
                catch (Exception)
                {
                    timeOut = 1000;
                }
                try
                {
                    baudRate = int.Parse(cmbBaudRate.Text, CultureInfo.CurrentCulture);
                }
                catch (Exception)
                {
                    baudRate = 57600;
                }
                switch (cmbParity.SelectedIndex)
                {
                    default:
                    case 0:
                        parity = MbusSerialMasterProtocol.SER_PARITY_NONE;
                        break;
                    case 1:
                        parity = MbusSerialMasterProtocol.SER_PARITY_EVEN;
                        break;
                    case 2:
                        parity = MbusSerialMasterProtocol.SER_PARITY_ODD;
                        break;
                }
                switch (cmbDataBits.SelectedIndex)
                {
                    default:
                    case 0:
                        dataBits = MbusSerialMasterProtocol.SER_DATABITS_8;
                        break;
                    case 1:
                        dataBits = MbusSerialMasterProtocol.SER_DATABITS_7;
                        break;
                }
                switch (cmbStopBits.SelectedIndex)
                {
                    default:
                    case 0:
                        stopBits = MbusSerialMasterProtocol.SER_STOPBITS_1;
                        break;
                    case 1:
                        stopBits = MbusSerialMasterProtocol.SER_STOPBITS_2;
                        break;
                }
                myProtocol.timeout = timeOut;
                myProtocol.retryCnt = retryCnt;
                myProtocol.pollDelay = pollDelay;
                if (portFound == true) cmbComPort.Text = currentPort.PortName;
                // Note: The following cast is required as the myProtocol object is declared 
                // as the superclass of MbusSerialMasterProtocol. That way myProtocol can
                // represent different protocol types.
                res = ((MbusSerialMasterProtocol)(myProtocol)).openProtocol(cmbComPort.Text, baudRate, dataBits, stopBits, parity);
                if ((res == BusProtocolErrors.FTALK_SUCCESS))
                {
                    lblResult1.Text = ("���������������� ���� ������� ������ � �����������: "
                                + (cmbComPort.Text + (", "
                                + (baudRate + (" baud, "
                                + (dataBits + (" data bits, "
                                + (stopBits + (" stop bits, parity " + parity)))))))));

                    toolStripStatusLabel3.Text =(cmbComPort.Text + (", " + (baudRate + (" baud"))));

                }
                else
                {
                    lblResult1.Text = ("�� ������� ������� ��������, ������ ����: " + BusProtocolErrors.getBusProtocolErrorText(res));
                    toolStripStatusLabel3.Text = ("��� ���� �� ���������");
                }

                slave = int.Parse(txtSlave.Text, CultureInfo.CurrentCulture);

                startRdReg = 47; // 40047 ����� ����/����� �����������
                numRdRegs = 8;
                res = myProtocol.readMultipleRegisters(slave, startRdReg, readVals, numRdRegs);
               // lblResult.Text += ("���������: " + (BusProtocolErrors.getBusProtocolErrorText(res) + "\r\n"));
                if ((res == BusProtocolErrors.FTALK_SUCCESS))
                {


                    label83.Text = "";
                    label83.Text = (label83.Text + readVals[0] + "." + readVals[1] + "." + readVals[2] + "   " + readVals[3] + ":" + readVals[4] + ":" + readVals[5]);


                }
                if ((res == BusProtocolErrors.FTALK_SUCCESS))
                {

                    toolStripStatusLabel1.Text = "    MODBUS ON    ";
                    toolStripStatusLabel1.BackColor = Color.Lime;

                }
                else
                {
                    toolStripStatusLabel1.Text = "    MODBUS ERROR (6) ";
                    toolStripStatusLabel1.BackColor = Color.Red;
                    Polltimer1.Enabled = false;
                    timer_Mic_test.Enabled = false;
                    portFound = false;
                    find_com_port.Enabled = true;
                   // SetComPort();
                }

          //  }
          //  Polltimer1.Enabled = true;
        }

        private void SetComPort()
        {
            //currentPort = new SerialPort();
            //if (currentPort.IsOpen) currentPort.Close();
            try
            {
                string[] ports = SerialPort.GetPortNames();
                foreach (string port in ports)
                {
                    currentPort = new SerialPort(port, 57600);
                    if (DetectKamerton())
                    {
                        label78.Text = ("��������� � " + currentPort.PortName);
                        portFound = true;
                        serial_connect();
                        lblResult1.Text = ("��������� � " + currentPort.PortName);
                        toolStripStatusLabel3.Text = ("��������� � " + currentPort.PortName);
                        find_com_port.Enabled = false;
                        break;
                    }
                    else
                    {
                        label78.Text = "COM ���� �� ������";
                        toolStripStatusLabel3.Text = ("��� ���� �� ���������");
                        lblResult1.Text = ("��� ���� �� ���������");
                        portFound = false;
                    }
                }
            }
            catch (Exception e)
            {

            }
        }
        private bool DetectKamerton()
        {
            try
            {
                //The below setting are for the Hello handshake
                byte[] buffer = new byte[5];
                buffer[0] = Convert.ToByte(16);
                buffer[1] = Convert.ToByte(128);
                buffer[2] = Convert.ToByte(0);
                buffer[3] = Convert.ToByte(0);
                buffer[4] = Convert.ToByte(4);
                int intReturnASCII = 0;
                char charReturnValue = (Char)intReturnASCII;
                currentPort.Open();
                currentPort.Write(buffer, 0, 5);
                Thread.Sleep(1000);
                int count = currentPort.BytesToRead;
                string returnMessage = "";
                while (count > 0)
                    {
                        intReturnASCII = currentPort.ReadByte();
                        returnMessage = returnMessage + Convert.ToChar(intReturnASCII);
                        count--;
                    }

                currentPort.Close();
                if (returnMessage.Contains("HELLO FROM KAMERTON "))
                {
                    label78.Text += returnMessage;
                    return true;
                }
                else
                {
                    label78.Text += " ���� �� ������ false ";
                    return false;
                }
            }
            catch (Exception e)
            {
                label78.Text += " ���� �� ������ ";
                return false;
            }
        }

        private void file_fakt_namber()
        {
            short[] readVals = new short[20];
            int slave;
            int startRdReg;
            int numRdRegs;
            slave = int.Parse(txtSlave.Text, CultureInfo.CurrentCulture);
            startRdReg = 112; // 40112 �����
            numRdRegs = 4;
            res = myProtocol.readMultipleRegisters(slave, startRdReg, readVals, numRdRegs);
            lblResult.Text = ("���������: " + (BusProtocolErrors.getBusProtocolErrorText(res) + "\r\n"));
            if ((res == BusProtocolErrors.FTALK_SUCCESS))
            {
                toolStripStatusLabel1.Text = "    MODBUS ON    ";
                toolStripStatusLabel1.BackColor = Color.Lime;

                // textBox9.Text = "\r\n";
                textBox9.Text += "������� ����� �����   -   ";
                textBox9.Text += (readVals[0]);
                if (readVals[1] < 10)
                {
                    textBox9.Text += ("0" + readVals[1]);
                }
                else
                {
                    textBox9.Text += (readVals[1]);
                }
                if (readVals[2] < 10)
                {
                    textBox9.Text += ("0" + readVals[2]);
                }
                else
                {
                    textBox9.Text += (readVals[2]);
                }
                if (readVals[3] < 10)
                {
                    textBox9.Text += ("0" + readVals[3] + ".TXT" + "\r\n");
                }
                else
                {
                    textBox9.Text += (readVals[3] + ".TXT" + "\r\n");
                }
            }

            else
            {
                toolStripStatusLabel1.Text = "    MODBUS ERROR (8) ";
                toolStripStatusLabel1.BackColor = Color.Red;
                portFound = false;
                find_com_port.Enabled = true;
               // SetComPort();
            }
           test_end1();
        }

        #region Load Listboxes
        private void LoadListboxes()
        {
            //Three to load - ports, baudrates, datetype.  Also set default textbox values:
            //1) Available Ports:
            string[] ports = SerialPort.GetPortNames();

            foreach (string port in ports)
            {
                cmbComPort.Items.Add(port);
            }

            cmbComPort.SelectedIndex = 0;

        }
        #endregion

        #region timer all
        private void Polltimer1_Tick(object sender, EventArgs e)           // ��������� �������� MODBUS � �����
        {
            if (portFound == true)
            {
                short[] readVals = new short[125];
                int slave;
                int startRdReg;
                int numRdRegs;
                int res;

                slave = int.Parse(txtSlave.Text, CultureInfo.CurrentCulture);
                startRdReg = 46; // 40046 ����� ����/����� �����������  
                numRdRegs = 8;
                res = myProtocol.readMultipleRegisters(slave, startRdReg, readVals, numRdRegs);
                lblResult.Text = ("���������: " + (BusProtocolErrors.getBusProtocolErrorText(res) + "\r\n"));
                if ((res == BusProtocolErrors.FTALK_SUCCESS))
                {
                    toolStripStatusLabel1.Text = "    MODBUS ON    ";
                    toolStripStatusLabel1.BackColor = Color.Lime;

                    label83.Text = "";
                    label83.Text = (label83.Text + readVals[0] + "." + readVals[1] + "." + readVals[2] + "   " + readVals[3] + ":" + readVals[4] + ":" + readVals[5]);

                    startRdReg = 112; // 40046 ����� ����/����� �����������  
                    numRdRegs = 4;
                    res = myProtocol.readMultipleRegisters(slave, startRdReg, readVals, numRdRegs);
                    lblResult.Text = ("���������: " + (BusProtocolErrors.getBusProtocolErrorText(res) + "\r\n"));
                    if ((res == BusProtocolErrors.FTALK_SUCCESS))
                    {
                        toolStripStatusLabel1.Text = "    MODBUS ON    ";
                        toolStripStatusLabel1.BackColor = Color.Lime;

                        label134.Text = "";
                        label134.Text = (label134.Text + readVals[0]);
                        if (readVals[1] < 10)
                        {
                            label134.Text += ("0" + readVals[1]);
                        }
                        else
                        {
                            label134.Text += (readVals[1]);
                        }
                        if (readVals[2] < 10)
                        {
                            label134.Text += ("0" + readVals[2]);
                        }
                        else
                        {
                            label134.Text += (readVals[2]);
                        }
                        if (readVals[3] < 10)
                        {
                            label134.Text += ("0" + readVals[3] + ".TXT");
                        }
                        else
                        {
                            label134.Text += (readVals[3] + ".TXT");
                        }

                    }
                }

                else
                {
                    toolStripStatusLabel1.Text = "    MODBUS ERROR (9) ";
                    toolStripStatusLabel1.BackColor = Color.Red;
                    timer_byte_set.Enabled = false;
                    timer_Mic_test.Enabled = false;
                    timerCTS.Enabled = false;
                    timerTestAll.Enabled = false;
                    portFound = false;
                    find_com_port.Enabled = true;
                    //SetComPort();
                 }

                label80.Text = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss", CultureInfo.CurrentCulture);
                toolStripStatusLabel2.Text = ("����� : " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.CurrentCulture));
                //  timer_Mic_test.Enabled = false;
            }
            else
            {
                portFound = false;
               // SetComPort();
                find_com_port.Enabled = true;
            }
        }

        private void timer_byte_set_Tick(object sender, EventArgs e)
        {
            short[] readVals = new short[124];
            int slave;
            int startRdReg;
            int numRdRegs;
            int startCoil;
            int numCoils;
            int i;
            int res;
            bool[] coilVals = new bool[64];
            bool[] coilArr = new bool[64];
            bool[] coilSensor = new bool[64];

            slave = int.Parse(txtSlave.Text, CultureInfo.CurrentCulture);

            //*************************  �������� ������ ��������� ������ �������� ************************************

            Int64 binaryHolder;
            string binaryResult = "";
            int decimalNum;
            bool[] Dec_bin = new bool[64];
            startRdReg = 1;
            numRdRegs = 7;
            res = myProtocol.readMultipleRegisters(slave, startRdReg, readVals, numRdRegs);    // 03  ������� ����� �� ��������� �� ������  40000 -49999
            label78.Text = ("���������: " + (BusProtocolErrors.getBusProtocolErrorText(res) + "\r\n"));
            if ((res == BusProtocolErrors.FTALK_SUCCESS))
            {
                toolStripStatusLabel1.Text = "    MODBUS ON    ";
                toolStripStatusLabel1.BackColor = Color.Lime;

                for (int bite_x = 0; bite_x < 7; bite_x++)
                {
                    // textBox11.Text += (bite_x + "  ");
                    decimalNum = readVals[bite_x];
                    //    textBox11.Text += (decimalNum +  "\r\n");
                    while (decimalNum > 0)
                    {
                        binaryHolder = decimalNum % 2;
                        binaryResult += binaryHolder;
                        decimalNum = decimalNum / 2;
                    }

                    int len_str = binaryResult.Length;

                    while (len_str < 8)
                    {
                        binaryResult += 0;
                        len_str++;
                    }

                    //****************** �������� ����� ***************************
                    //binaryArray = binaryResult.ToCharArray();
                    //Array.Reverse(binaryArray);
                    //binaryResult = new string(binaryArray);
                    //*************************************************************

                    //  textBox11.Text = (textBox11.Text + (binaryResult + "\r\n"));

                    for (i = 0; i < 8; i++)                         // 
                    {
                        if (binaryResult[i] == '1')
                        {
                            Dec_bin[i + (8 * bite_x)] = true;
                        }
                        else
                        {
                            Dec_bin[i + (8 * bite_x)] = false;
                        }
                        //   textBox11.Text = (textBox11.Text + (Dec_bin[i+(8*bite_x)] + " "));
                    }
                    binaryResult = "";
                    //textBox11.Text += ("\r\n");
                }

            }

            //*************************** ����� ��������� ����� ��������� *****************************************

            label30.Text = "";
            label31.Text = "";
            label32.Text = "";

            label33.Text = "";
            label34.Text = "";
            label35.Text = "";
            label36.Text = "";

            for (i = 7; i >= 0; i--)
            {
                if (Dec_bin[i] == true)
                {
                    label30.Text += ("1" + "  ");
                }
                else
                {
                    label30.Text += ("0" + "  ");
                }
                if (Dec_bin[i + 8] == true)
                {
                    label31.Text += ("1" + "  ");
                }
                else
                {
                    label31.Text += ("0" + "  ");
                }


                if (Dec_bin[i + 16] == true)
                {
                    label32.Text += ("1" + "  ");
                }
                else
                {
                    label32.Text += ("0" + "  ");
                }

                if (Dec_bin[i + 24] == true)
                {
                    label33.Text += ("1" + "  ");
                }
                else
                {
                    label33.Text += ("0" + "  ");
                }

                if (Dec_bin[i + 32] == true)
                {
                    label34.Text += ("1" + "  ");
                }
                else
                {
                    label34.Text += ("0" + "  ");
                }

                if (Dec_bin[i + 40] == true)
                {
                    label35.Text += ("1" + "  ");
                }
                else
                {
                    label35.Text += ("0" + "  ");
                }
                if (Dec_bin[i + 48] == true)
                {
                    label36.Text += ("1" + "  ");
                }
                else
                {
                    label36.Text += ("0" + "  ");
                }
            }

            //***********************************************************************************

            if (Dec_bin[24] == false) // 30024 ���� ����������� �� �����2
            {
                //label103.BackColor = Color.Red;
                //label103.Text = "0";
            }
            else
            {
                //label103.BackColor = Color.Lime;
                //label103.Text = "1";
            }
            if (Dec_bin[25] == false) // 30025 ���� ����������� �� �����1
            {
                //label104.BackColor = Color.Red;
                //label104.Text = "0";
            }
            else
            {
                //label104.BackColor = Color.Lime;
                //label104.Text = "1";
            }

            if (Dec_bin[26] == false) // 30026 ���� ����������� ������
            {
                label105.BackColor = Color.Red;
                label105.Text = "0";
            }
            else
            {
                label105.BackColor = Color.Lime;
                label105.Text = "1";
            }

            if (Dec_bin[27] == false)   // 30027 ���� ����������� ������ ��������
            {
                label106.BackColor = Color.Red;
                label106.Text = "0";
            }
            else
            {
                label106.BackColor = Color.Lime;
                label106.Text = "1";
            }

            if (Dec_bin[28] == false)  // 30028 ���� ����������� ������
            {
                label107.BackColor = Color.Red;
                label107.Text = "0";
            }
            else
            {
                label107.BackColor = Color.Lime;
                label107.Text = "1";
            }

            if (Dec_bin[40] == false) // 30040  ���� ����������� �����������
            {
                //label108.BackColor = Color.Red;
                //label108.Text = "0";
            }
            else
            {
                //label108.BackColor = Color.Lime;
                //label108.Text = "1";
            }

            if (Dec_bin[41] == false) // 30041  ���� ����������� ��������� ����������� 2 ����������
            {
                label109.BackColor = Color.Red;
                label109.Text = "0";
            }
            else
            {
                label109.BackColor = Color.Lime;
                label109.Text = "1";
            }

            if (Dec_bin[42] == false) // 30042  ���� ����������� ��������� �����������
            {
                label110.BackColor = Color.Red;
                label110.Text = "0";
            }
            else
            {
                label110.BackColor = Color.Lime;
                label110.Text = "1";
            }

            if (Dec_bin[43] == false) // 30043  ���� ����������� ��������� ���������� � 2 ����������
            {
                label111.BackColor = Color.Red;
                label111.Text = "0";
            }
            else
            {
                label111.BackColor = Color.Lime;
                label111.Text = "1";
            }

            if (Dec_bin[44] == false) // 30044  ���� ����������� ��������� ����������
            {
                label112.BackColor = Color.Red;
                label112.Text = "0";
            }
            else
            {
                label112.BackColor = Color.Lime;
                label112.Text = "1";
            }

            if (Dec_bin[45] == false) // 30045  ���� ����������� ��������� XS1 - 6 Sence
            {
                label113.BackColor = Color.Red;
                label113.Text = "0";
            }
            else
            {
                label113.BackColor = Color.Lime;
                label113.Text = "1";
            }

            if (Dec_bin[46] == false) //  30046  ���� ����������� ���
            {
                //label115.BackColor = Color.Red;
                //label115.Text = "0";
            }
            else
            {
                //label115.BackColor = Color.Lime;
                //label115.Text = "1";
            }


            if (Dec_bin[52] == false) // 30052   ���� ���������� ��� (Mute)
            {
                label144.BackColor = Color.Red;
                label144.Text = "0";
            }
            else
            {
                label144.BackColor = Color.Lime;
                label144.Text = "1";
            }

            if (Dec_bin[53] == false) // 30053   ���� �������������
            {
                label143.BackColor = Color.Red;
                label143.Text = "0";
            }
            else
            {
                label143.BackColor = Color.Lime;
                label143.Text = "1";
            }

            if (Dec_bin[54] == false) // 30054   ���� ���������� ����������� ��������
            {
                label142.BackColor = Color.Red;
                label142.Text = "0";
            }
            else
            {
                label142.BackColor = Color.Lime;
                label142.Text = "1";
            }

            //********************������ ������� ********************
            startCoil = 1;  //  regBank.add(00001-9);   ����������� ���������� ���� 0-7
            numCoils = 8;
            res = myProtocol.readCoils(slave, startCoil, coilArr, numCoils);
            lblResult2.Text = ("���������: " + (BusProtocolErrors.getBusProtocolErrorText(res) + "\r\n"));


            if ((res == BusProtocolErrors.FTALK_SUCCESS))
            {

                if (coilArr[0] == true)                              //   ���� RL0
                {
                    button37.BackColor = Color.Lime;
                    button48.BackColor = Color.White;
                }
                else
                {
                    button48.BackColor = Color.Red;
                    button37.BackColor = Color.White;
                }
                if (coilArr[1] == true)                              //   ���� RL1
                {
                    button40.BackColor = Color.Lime;
                    button53.BackColor = Color.White;
                }
                else
                {
                    button53.BackColor = Color.Red;
                    button40.BackColor = Color.White;
                }
                if (coilArr[2] == true)                              //   ���� RL2
                {
                    button44.BackColor = Color.Lime;
                    button79.BackColor = Color.White;
                }
                else
                {
                    button79.BackColor = Color.Red;
                    button44.BackColor = Color.White;
                }
                if (coilArr[3] == true)                              //   ���� RL3
                {
                    button49.BackColor = Color.Lime;
                    button66.BackColor = Color.White;
                }
                else
                {
                    button66.BackColor = Color.Red;
                    button49.BackColor = Color.White;
                }
                if (coilArr[4] == true)                              //   ���� RL4
                {
                    button38.BackColor = Color.Lime;
                    button52.BackColor = Color.White;
                }
                else
                {
                    button52.BackColor = Color.Red;
                    button38.BackColor = Color.White;
                }
                if (coilArr[5] == true)                              //   ���� RL5
                {
                    button71.BackColor = Color.Lime;
                    button47.BackColor = Color.White;
                }
                else
                {
                    button47.BackColor = Color.Red;
                    button71.BackColor = Color.White;
                }
                if (coilArr[6] == true)                              //   ���� RL6
                {
                    button69.BackColor = Color.Lime;
                    button42.BackColor = Color.White;
                }
                else
                {
                    button42.BackColor = Color.Red;
                    button69.BackColor = Color.White;
                }
                if (coilArr[7] == true)                              //   ���� RL7
                {
                    button51.BackColor = Color.Lime;
                    button45.BackColor = Color.White;
                }
                else
                {
                    button45.BackColor = Color.Red;
                    button51.BackColor = Color.White;
                }

            }

            startCoil = 9;  //  regBank.add(00009-16);   ����������� ���������� ���� 9-16
            numCoils = 8;
            res = myProtocol.readCoils(slave, startCoil, coilArr, numCoils);
            lblResult2.Text = ("���������: " + (BusProtocolErrors.getBusProtocolErrorText(res) + "\r\n"));
            if ((res == BusProtocolErrors.FTALK_SUCCESS))
            {

                if (coilArr[0] == true)                              //   ���� RL8 ���� �� �������� regBank.add(9)
                {
                    button46.BackColor = Color.Lime;
                    button50.BackColor = Color.White;
                }
                else
                {
                    button50.BackColor = Color.Red;
                    button46.BackColor = Color.White;
                }
                if (coilArr[1] == true)                              //   ���� RL9  XP1 10 regBank.add(10)
                {
                    button27.BackColor = Color.Lime;
                    button30.BackColor = Color.White;
                }
                else
                {
                    button30.BackColor = Color.Red;
                    button27.BackColor = Color.White;
                }

                if (coilArr[2] == true)                              //   �������� regBank.add(11)
                {
                    button2.BackColor = Color.Lime;
                    button81.BackColor = Color.White;
                }
                else
                {
                    button81.BackColor = Color.Red;
                    button2.BackColor = Color.White;
                }

                if (coilArr[3] == true)                                //   �������� regBank.add(12)
                {
                    //button27.BackColor = Color.Lime;
                    //button30.BackColor = Color.White;
                }
                else
                {
                    //button30.BackColor = Color.Red;
                    //button27.BackColor = Color.White;
                }


                if (coilArr[4] == true)                             // XP8 - 2  Sence ���� �. regBank.add(13)
                {
                    button59.BackColor = Color.Lime;
                    button74.BackColor = Color.White;
                }
                else
                {
                    button74.BackColor = Color.Red;
                    button59.BackColor = Color.White;
                }
                if (coilArr[5] == true)                             //XP8 - 1  PTT ���� �. regBank.add(14)
                {
                    button39.BackColor = Color.Lime;
                    button41.BackColor = Color.White;
                }
                else
                {
                    button41.BackColor = Color.Red;
                    button39.BackColor = Color.White;
                }
                if (coilArr[6] == true)                             // XS1 - 5   PTT ���  regBank.add(15)
                {
                    button7.BackColor = Color.Lime;
                    button18.BackColor = Color.White;
                }
                else
                {
                    button18.BackColor = Color.Red;
                    button7.BackColor = Color.White;
                }
                if (coilArr[7] == true)                             // XS1 - 6 Sence ���. regBank.add(16)
                {
                    button33.BackColor = Color.Lime;
                    button34.BackColor = Color.White;
                }
                else
                {
                    button34.BackColor = Color.Red;
                    button33.BackColor = Color.White;
                }
            }

            startCoil = 17;  //  regBank.add(00017-24);   ����������� ���������� ���� 17-24
            numCoils = 8;
            res = myProtocol.readCoils(slave, startCoil, coilArr, numCoils);
            lblResult2.Text = ("���������: " + (BusProtocolErrors.getBusProtocolErrorText(res) + "\r\n"));

            if ((res == BusProtocolErrors.FTALK_SUCCESS))
            {
                if (coilArr[0] == true)                             // XP7 4 PTT2 ����. �.  regBank.add(17)
                {
                    button14.BackColor = Color.Lime;
                    button29.BackColor = Color.White;
                }
                else
                {
                    button29.BackColor = Color.Red;
                    button14.BackColor = Color.White;
                }
                if (coilArr[1] == true)                             // XP1 - 20  HangUp  DCD regBank.add(18)
                {
                    button19.BackColor = Color.Lime;
                    button26.BackColor = Color.White;
                }
                else
                {
                    button26.BackColor = Color.Red;
                    button19.BackColor = Color.White;
                }
                if (coilArr[2] == true)                             // J8-11     XP7 2 Sence  ����. �. regBank.add(19)
                {
                    button58.BackColor = Color.Lime;
                    button72.BackColor = Color.White;
                }
                else
                {
                    button72.BackColor = Color.Red;
                    button58.BackColor = Color.White;
                }
                if (coilArr[3] == true)                             //  XP7 1 PTT1 ����. �.  regBank.add(20)
                {
                    button10.BackColor = Color.Lime;
                    button23.BackColor = Color.White;
                }
                else
                {
                    button23.BackColor = Color.Red;
                    button10.BackColor = Color.White;
                }
                if (coilArr[4] == true)                             // XP2-2    Sence "���." 
                {
                    //button60.BackColor = Color.Lime;
                    //button76.BackColor = Color.White;
                }
                else
                {
                    //button76.BackColor = Color.Red;
                    //button60.BackColor = Color.White;
                }
                if (coilArr[5] == true)                             // XP5-3    Sence "��C."
                {
                    button35.BackColor = Color.Lime;
                    button36.BackColor = Color.White;
                }
                else
                {
                    button36.BackColor = Color.Red;
                    button35.BackColor = Color.White;
                }
                if (coilArr[6] == true)                             // XP3-3    Sence "��-�����1."
                {
                    button56.BackColor = Color.Lime;
                    button68.BackColor = Color.White;
                }
                else
                {
                    button68.BackColor = Color.Red;
                    button56.BackColor = Color.White;
                }
                if (coilArr[7] == true)                             // XP4-3    Sence "��-�����2."
                {
                    button55.BackColor = Color.Lime;
                    button67.BackColor = Color.White;
                }
                else
                {
                    button67.BackColor = Color.Red;
                    button55.BackColor = Color.White;
                }

            }

            startCoil = 25;  //  regBank.add(00001-12);   ����������� ���������� ���� 25-32
            numCoils = 8;
            res = myProtocol.readCoils(slave, startCoil, coilArr, numCoils);
            lblResult2.Text = ("���������: " + (BusProtocolErrors.getBusProtocolErrorText(res) + "\r\n"));


            if ((res == BusProtocolErrors.FTALK_SUCCESS))
            {


                if (coilArr[0] == false)                            // XP1- 19 HaSs      ������  ����������� ������       
                {
                    button57.BackColor = Color.Lime;
                    button70.BackColor = Color.White;
                }
                else
                {
                    button70.BackColor = Color.Red;
                    button57.BackColor = Color.White;
                }

                if (coilArr[1] == true)                             // XP1- 17 HaSPTT    CTS DSR ���. 
                {
                    button16.BackColor = Color.Lime;
                    button20.BackColor = Color.White;
                }
                else
                {
                    button20.BackColor = Color.Red;
                    button16.BackColor = Color.White;
                }



                if (coilArr[2] == true)                             // XP1- 16 HeS2Rs    ���� ����������� ��������� ����������� � 2 ����������
                {
                    button61.BackColor = Color.Lime;
                    button78.BackColor = Color.White;
                }
                else
                {
                    button78.BackColor = Color.Red;
                    button61.BackColor = Color.White;
                }


                if (coilArr[3] == true)                             // XP1- 15 HeS2PTT   CTS ���
                {
                    button28.BackColor = Color.Lime;
                    button17.BackColor = Color.White;
                }
                else
                {
                    button17.BackColor = Color.Red;
                    button28.BackColor = Color.White;
                }

                if (coilArr[4] == true)                             //    XP1- 13 HeS2Ls    ���� ����������� ��������� ����������� 
                {
                    button62.BackColor = Color.Lime;
                    button77.BackColor = Color.White;
                }
                else
                {
                    button77.BackColor = Color.Red;
                    button62.BackColor = Color.White;
                }

                if (coilArr[5] == true)                             //    XP1- 6  HeS1PTT   CTS ���
                {
                    button8.BackColor = Color.Lime;
                    button22.BackColor = Color.White;
                }
                else
                {
                    button22.BackColor = Color.Red;
                    button8.BackColor = Color.White;
                }

                if (coilArr[6] == true)                             //   XP1- 5  HeS1Rs    ���� ���������� ��������� ���������� � 2 ����������
                {
                    button63.BackColor = Color.Lime;
                    button75.BackColor = Color.White;
                }
                else
                {
                    button75.BackColor = Color.Red;
                    button63.BackColor = Color.White;
                }

                if (coilArr[7] == true)                             //    XP1- 1  HeS1Ls    ���� ���������� ��������� ����������
                {
                    button64.BackColor = Color.Lime;
                    button73.BackColor = Color.White;
                }
                else
                {
                    button73.BackColor = Color.Red;
                    button64.BackColor = Color.White;
                }


            }

            startCoil = 81;  // ���� 
            numCoils = 4;
            res = myProtocol.readInputDiscretes(slave, startCoil, coilArr, numCoils);
            lblResult2.Text = ("���������: " + (BusProtocolErrors.getBusProtocolErrorText(res) + "\r\n"));

            if ((res == BusProtocolErrors.FTALK_SUCCESS))
            {
                if (coilArr[0] == false) // ��� CTS - 1x81 
                {
                    label156.BackColor = Color.Red;
                    label156.Text = "1";
                }
                else
                {
                    label156.BackColor = Color.Lime;
                    label156.Text = "0";
                }
                if (coilArr[1] == false) // ��� DSR - 1x82  
                {
                    label155.BackColor = Color.Red;
                    label155.Text = "1";
                }
                else
                {
                    label155.BackColor = Color.Lime;
                    label155.Text = "0";
                }
                if (coilArr[2] == false) // // ��� DCD -  1x83
                {
                    label152.BackColor = Color.Red;
                    label152.Text = "1";
                }
                else
                {
                    label152.BackColor = Color.Lime;
                    label152.Text = "0";
                }

            }

            progressBar1.Value += 1;
            label114.Text = ("" + progressBar1.Value);
            if (progressBar1.Value == progressBar1.Maximum)
            {
                progressBar1.Value = 0;
            }

            if ((res == BusProtocolErrors.FTALK_SUCCESS))
            {
                toolStripStatusLabel1.Text = "    MODBUS ON    ";
                toolStripStatusLabel1.BackColor = Color.Lime;

                label83.Text = "";
                label83.Text = (label83.Text + readVals[0] + "." + readVals[1] + "." + readVals[2] + "   " + readVals[3] + ":" + readVals[4] + ":" + readVals[5]);
            }
            else
            {
                toolStripStatusLabel1.Text = "    MODBUS ERROR (1)  ";
                toolStripStatusLabel1.BackColor = Color.Red;
                // Polltimer1.Enabled = false;
                timer_Mic_test.Enabled = false;
                portFound = false;
                find_com_port.Enabled = true;
               // SetComPort();
            }
            label80.Text = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss", CultureInfo.CurrentCulture);
            toolStripStatusLabel2.Text = ("����� : " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.CurrentCulture));
        }
        private void timerCTS_Tick(object sender, EventArgs e)
        {

            // short[] writeVals = new short[32];
            short[] readVals = new short[124];
            int slave;
            int startRdReg;
            int numRdRegs;
            int startCoil;
            int numCoils;
            int i;
            int res;
            bool[] coilVals = new bool[64];
            bool[] coilArr = new bool[34]
                ;
            bool[] coilSensor = new bool[64];

            slave = int.Parse(txtSlave.Text, CultureInfo.CurrentCulture);

            startRdReg = 1;
            numRdRegs = 56;

            res = myProtocol.readInputRegisters(slave, startRdReg, readVals, numRdRegs);
            label78.Text = ("���������: " + (BusProtocolErrors.getBusProtocolErrorText(res) + "\r\n"));
            if ((res == BusProtocolErrors.FTALK_SUCCESS))
            {

                toolStripStatusLabel1.Text = "    MODBUS ON    ";
                toolStripStatusLabel1.BackColor = Color.Lime;


                //  ������ ����

                label30.Text = "";
                label31.Text = "";
                label32.Text = "";

                label33.Text = "";
                label34.Text = "";
                label35.Text = "";
                label36.Text = "";


                for (i = 7; i >= 0; i--)
                {
                    label30.Text = (label30.Text + (readVals[i] + "  "));
                    label31.Text = (label31.Text + (readVals[i + 8] + "  "));
                    label32.Text = (label32.Text + (readVals[i + 16] + "  "));

                    label33.Text = (label33.Text + (readVals[i + 24] + "  "));
                    label34.Text = (label34.Text + (readVals[i + 32] + "  "));
                    label35.Text = (label35.Text + (readVals[i + 40] + "  "));
                    label36.Text = (label36.Text + (readVals[i + 48] + "  "));
                }
                if (readVals[24] == 0) // 30024 ���� ����������� �� �����2
                {
                    //label103.BackColor = Color.Red;
                    //label103.Text = "0";
                }
                else
                {
                    //label103.BackColor = Color.Lime;
                    //label103.Text = "1";
                }
                if (readVals[25] == 0) // 30025 ���� ����������� �� �����1
                {
                    //label104.BackColor = Color.Red;
                    //label104.Text = "0";
                }
                else
                {
                    //label104.BackColor = Color.Lime;
                    //label104.Text = "1";
                }

                if (readVals[26] == 0) // 30026 ���� ����������� ������
                {
                    label105.BackColor = Color.Red;
                    label105.Text = "0";
                }
                else
                {
                    label105.BackColor = Color.Lime;
                    label105.Text = "1";
                }

                if (readVals[27] == 0)   // 30027 ���� ����������� ������ ��������
                {
                    label106.BackColor = Color.Red;
                    label106.Text = "0";
                }
                else
                {
                    label106.BackColor = Color.Lime;
                    label106.Text = "1";
                }

                if (readVals[28] == 0)  // 30028 ���� ����������� ������
                {
                    label107.BackColor = Color.Red;
                    label107.Text = "0";
                }
                else
                {
                    label107.BackColor = Color.Lime;
                    label107.Text = "1";
                }

                if (readVals[40] == 0) // 30040  ���� ����������� �����������
                {
                    //label108.BackColor = Color.Red;
                    //label108.Text = "0";
                }
                else
                {
                    //label108.BackColor = Color.Lime;
                    //label108.Text = "1";
                }

                if (readVals[41] == 0) // 30041  ���� ����������� ��������� ����������� 2 ����������
                {
                    label109.BackColor = Color.Red;
                    label109.Text = "0";
                }
                else
                {
                    label109.BackColor = Color.Lime;
                    label109.Text = "1";
                }

                if (readVals[42] == 0) // 30042  ���� ����������� ��������� �����������
                {
                    label110.BackColor = Color.Red;
                    label110.Text = "0";
                }
                else
                {
                    label110.BackColor = Color.Lime;
                    label110.Text = "1";
                }

                if (readVals[43] == 0) // 30043  ���� ����������� ��������� ���������� � 2 ����������
                {
                    label111.BackColor = Color.Red;
                    label111.Text = "0";
                }
                else
                {
                    label111.BackColor = Color.Lime;
                    label111.Text = "1";
                }

                if (readVals[44] == 0) // 30044  ���� ����������� ��������� ����������
                {
                    label112.BackColor = Color.Red;
                    label112.Text = "0";
                }
                else
                {
                    label112.BackColor = Color.Lime;
                    label112.Text = "1";
                }

                if (readVals[45] == 0) // 30045  ���� ����������� ��������� XS1 - 6 Sence
                {
                    label113.BackColor = Color.Red;
                    label113.Text = "0";
                }
                else
                {
                    label113.BackColor = Color.Lime;
                    label113.Text = "1";
                }

                if (readVals[46] == 0) //  30046  ���� ����������� ���
                {
                    //label115.BackColor = Color.Red;
                    //label115.Text = "0";
                }
                else
                {
                    //label115.BackColor = Color.Lime;
                    //label115.Text = "1";
                }


                if (readVals[47] == 0) // 30047   ���� ���������� ��� (Mute)
                {
                    label144.BackColor = Color.Red;
                    label144.Text = "0";
                }
                else
                {
                    label144.BackColor = Color.Lime;
                    label144.Text = "1";
                }

                if (readVals[48] == 0) // 30048   ���� �������������
                {
                    label143.BackColor = Color.Red;
                    label143.Text = "0";
                }
                else
                {
                    label143.BackColor = Color.Lime;
                    label143.Text = "1";
                }

                if (readVals[49] == 0) // 30049   ���� ���������� ����������� ��������
                {
                    label142.BackColor = Color.Red;
                    label142.Text = "0";
                }
                else
                {
                    label142.BackColor = Color.Lime;
                    label142.Text = "1";
                }
            }





            startCoil = 81;  // ���� ���������� ������� �����
            numCoils = 4;
            res = myProtocol.readInputDiscretes(slave, startCoil, coilArr, numCoils);
            lblResult2.Text = ("���������: " + (BusProtocolErrors.getBusProtocolErrorText(res) + "\r\n"));

            if ((res == BusProtocolErrors.FTALK_SUCCESS))
            {
                if (coilArr[0] == false) // ��� CTS - 1x80   
                {
                    label156.BackColor = Color.Red;
                    label156.Text = "1";
                }
                else
                {
                    label156.BackColor = Color.Lime;
                    label156.Text = "0";
                }
                if (coilArr[1] == false) // ��� DSR - 1x81   
                {
                    label155.BackColor = Color.Red;
                    label155.Text = "1";
                }
                else
                {
                    label155.BackColor = Color.Lime;
                    label155.Text = "0";
                }
                if (coilArr[2] == false) // // ��� DCD -  1x82 
                {
                    label152.BackColor = Color.Red;
                    label152.Text = "1";
                }
                else
                {
                    label152.BackColor = Color.Lime;
                    label152.Text = "0";
                }

            }


            if ((res != BusProtocolErrors.FTALK_SUCCESS))
            {
                toolStripStatusLabel1.Text = "    MODBUS ERROR (2) ";
                toolStripStatusLabel1.BackColor = Color.Red;
                //Polltimer1.Enabled = false;
                timer_Mic_test.Enabled = false;
                portFound = false;
                find_com_port.Enabled = true;
                //SetComPort();
            }



            progressBar1.Value += 1;
            label114.Text = ("" + progressBar1.Value);
            if (progressBar1.Value == progressBar1.Maximum)
            {
                progressBar1.Value = 0;
            }


            if ((res == BusProtocolErrors.FTALK_SUCCESS))
            {
                toolStripStatusLabel1.Text = "    MODBUS ON    ";
                toolStripStatusLabel1.BackColor = Color.Lime;

                label83.Text = "";
                label83.Text = (label83.Text + readVals[0] + "." + readVals[1] + "." + readVals[2] + "   " + readVals[3] + ":" + readVals[4] + ":" + readVals[5]);
            }


            else
            {
                toolStripStatusLabel1.Text = "    MODBUS ERROR (3) ";
                toolStripStatusLabel1.BackColor = Color.Red;
                // Polltimer1.Enabled = false;
                timer_Mic_test.Enabled = false;
                timer_byte_set.Enabled = false;
                timerCTS.Enabled = false;
                timerTestAll.Enabled = false;
                portFound = false;
                find_com_port.Enabled = true;
               // SetComPort();
            }


            label80.Text = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss", CultureInfo.CurrentCulture);
            toolStripStatusLabel2.Text = ("����� : " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.CurrentCulture));
        }


        #endregion

        private void cmdOpenTCP_Click(object sender, EventArgs e)
        {
            //
            // First we must instantiate class if we haven't done so already
            //
            if ((myProtocol == null))
            {
                try
                {
                    if ((cmbTcpProtocol.SelectedIndex == 0))
                        myProtocol = new MbusTcpMasterProtocol();
                    else
                        myProtocol = new MbusRtuOverTcpMasterProtocol();
                }
                catch (OutOfMemoryException ex)
                {
                    label78.Text = ("�� ������� ������� ��������� ������ ��������� ���������! ������ ���� " + ex.Message);
                    return;
                }
            }
            else // already instantiated, close protocol and reinstantiate
            {
                if (myProtocol.isOpen())
                    myProtocol.closeProtocol();
                myProtocol = null;
                try
                {
                    if ((cmbTcpProtocol.SelectedIndex == 0))
                        myProtocol = new MbusTcpMasterProtocol();
                    else
                        myProtocol = new MbusRtuOverTcpMasterProtocol();
                }
                catch (OutOfMemoryException ex)
                {
                    label78.Text = ("�� ������� ������� ��������� ������ ��������� ���������! ������ ���� " + ex.Message);
                    return;
                }
            }
            //
            // Here we configure the protocol
            //
            int retryCnt;
            int pollDelay;
            int timeOut;
            int tcpPort;
            int res;
            try
            {
                retryCnt = int.Parse(cmbRetry.Text, CultureInfo.CurrentCulture);
            }
            catch (Exception)
            {
                retryCnt = 0;
            }
            try
            {
                pollDelay = int.Parse(txtPollDelay.Text, CultureInfo.CurrentCulture);
            }
            catch (Exception)
            {
                pollDelay = 0;
            }
            try
            {
                timeOut = int.Parse(txtTimeout.Text, CultureInfo.CurrentCulture);
            }
            catch (Exception)
            {
                timeOut = 1000;
            }
            try
            {
                tcpPort = int.Parse(txtTCPPort.Text, CultureInfo.CurrentCulture);
            }
            catch (Exception)
            {
                tcpPort = 502;
            }
            myProtocol.timeout = timeOut;
            myProtocol.retryCnt = retryCnt;
            myProtocol.pollDelay = pollDelay;
            // Note: The following cast is required as the myProtocol object is declared
            // as the superclass of MbusTcpMasterProtocol. That way myProtocol can
            // represent different protocol types.
            ((MbusTcpMasterProtocol)myProtocol).port = (short)tcpPort;
            res = ((MbusTcpMasterProtocol)myProtocol).openProtocol(txtHostName.Text);
            if ((res == BusProtocolErrors.FTALK_SUCCESS))
            {
                label78.Text = ("Modbus/TCP port opened successfully with parameters: " + (txtHostName.Text + (", TCP port " + tcpPort)));
                button6.Enabled = true;
                Polltimer1.Enabled = true;
            }
            else
            {
                label78.Text = ("Could not open protocol, error was: " + BusProtocolErrors.getBusProtocolErrorText(res));
            }
        }

        private void cmdExecute_Click(object sender, EventArgs e)    
        {  // ���� ������ � �������� MODBUS
            short[] writeVals = new short[125];
            short[] readVals = new short[125];
            int slave;
            int startWrReg;
            int numWrRegs;
            int startRdReg;
            int numRdRegs;
            int i;
            int res;
            int startCoil;
            int numCoils;
            bool[] coilVals = new bool[2000];
            try
            {
                try
                {
                    slave = int.Parse(txtSlave.Text, CultureInfo.CurrentCulture);
                }
                catch (Exception)
                {
                    slave = 1;
                }
                try
                {
                    startCoil = int.Parse(txtStartCoil.Text, CultureInfo.CurrentCulture);
                }
                catch (Exception)
                {
                    startCoil = 1;
                }
                try
                {
                    numCoils = int.Parse(txtNumCoils.Text, CultureInfo.CurrentCulture);
                }
                catch (Exception)
                {
                    numCoils = 1;
                }
                try
                {
                    startRdReg = int.Parse(txtStartRdReg.Text, CultureInfo.CurrentCulture);
                }
                catch (Exception)
                {
                    startRdReg = 1;
                }
                try
                {
                    startWrReg = int.Parse(txtStartWrReg.Text, CultureInfo.CurrentCulture);
                }
                catch (Exception)
                {
                    startWrReg = 1;
                }
                try
                {
                    numWrRegs = int.Parse(txtNumWrRegs.Text, CultureInfo.CurrentCulture);
                }
                catch (Exception)
                {
                    numWrRegs = 1;
                }
                try
                {
                    numRdRegs = int.Parse(txtNumRdRegs.Text, CultureInfo.CurrentCulture);
                }
                catch (Exception)
                {
                    numRdRegs = 1;
                }
                try
                {
                    writeVals[0] = Int16.Parse(txtWriteVal1.Text, CultureInfo.CurrentCulture);
                    writeVals[1] = Int16.Parse(txtWriteVal2.Text, CultureInfo.CurrentCulture);
                    writeVals[2] = Int16.Parse(txtWriteVal3.Text, CultureInfo.CurrentCulture);
                    writeVals[3] = Int16.Parse(txtWriteVal4.Text, CultureInfo.CurrentCulture);
                    writeVals[4] = Int16.Parse(txtWriteVal5.Text, CultureInfo.CurrentCulture);
                    writeVals[5] = Int16.Parse(txtWriteVal6.Text, CultureInfo.CurrentCulture);
                    writeVals[6] = Int16.Parse(txtWriteVal7.Text, CultureInfo.CurrentCulture);
                    writeVals[7] = Int16.Parse(txtWriteVal8.Text, CultureInfo.CurrentCulture);
                    coilVals[0] = (writeVals[0] != 0);
                    coilVals[1] = (writeVals[1] != 0);
                    coilVals[2] = (writeVals[2] != 0);
                    coilVals[3] = (writeVals[3] != 0);
                    coilVals[4] = (writeVals[4] != 0);
                    coilVals[5] = (writeVals[5] != 0);
                    coilVals[6] = (writeVals[6] != 0);
                    coilVals[7] = (writeVals[7] != 0);
                }
                catch (Exception)
                {
                }
                switch (cmbCommand.SelectedIndex)
                {
                    //
                    // Read Holding Registers
                    //
                    case 0:
                        res = myProtocol.readMultipleRegisters(slave, startRdReg, readVals, numRdRegs);
                        lblResult2.Text = ("���������: " + (BusProtocolErrors.getBusProtocolErrorText(res) + "\r\n"));
                        if ((res == BusProtocolErrors.FTALK_SUCCESS))
                        {
                            lblReadValues.Text = "";
                            for (i = 0; (i <= (numRdRegs - 1)); i++)
                            {
                                lblReadValues.Text = (lblReadValues.Text + (readVals[i] + "  "));
                            }
                        }
                        break;
                    //
                    // Read Input Registers
                    //
                    case 1:
                        res = myProtocol.readInputRegisters(slave, startRdReg, readVals, numRdRegs);
                        lblResult2.Text = ("���������: " + (BusProtocolErrors.getBusProtocolErrorText(res) + "\r\n"));
                        if ((res == BusProtocolErrors.FTALK_SUCCESS))
                        {
                            lblReadValues.Text = "";
                            for (i = 0; (i <= (numRdRegs - 1)); i++)
                            {
                                lblReadValues.Text = (lblReadValues.Text + (readVals[i] + "  "));
                            }
                        }
                        break;
                    //
                    // Read Coils
                    //
                    case 2:
                        res = myProtocol.readCoils(slave, startCoil, coilVals, numCoils);
                        lblResult2.Text = ("���������: " + (BusProtocolErrors.getBusProtocolErrorText(res) + "\r\n"));
                        if ((res == BusProtocolErrors.FTALK_SUCCESS))
                        {
                            lblReadValues.Text = "";
                            for (i = 0; (i <= (numCoils - 1)); i++)
                            {
                                if (coilVals[i])
                                    lblReadValues.Text = (lblReadValues.Text + "1  ");
                                else
                                    lblReadValues.Text = (lblReadValues.Text + "0  ");
                            }
                        }
                        break;
                    //
                    // Write Coils
                    //
                    case 3:
                        res = myProtocol.forceMultipleCoils(slave, startCoil, coilVals, numCoils);
                        lblResult2.Text = ("���������: " + (BusProtocolErrors.getBusProtocolErrorText(res) + "\r\n"));
                        break;
                    //
                    // Write Holding Registers
                    //
                    case 4:
                        res = myProtocol.writeMultipleRegisters(slave, startWrReg, writeVals, numWrRegs);
                        lblResult2.Text = ("���������: " + (BusProtocolErrors.getBusProtocolErrorText(res) + "\r\n"));
                        break;
                    //
                    // Write Single Registers
                    //
                    case 5:
                        res = myProtocol.writeSingleRegister(slave, startWrReg, writeVals[0]);
                        lblResult2.Text = ("���������: " + (BusProtocolErrors.getBusProtocolErrorText(res) + "\r\n"));
                        break;
                    //
                    // Read/Write Registers
                    //
                    case 6:
                        res = myProtocol.readWriteRegisters(slave, startRdReg, readVals, numRdRegs, startWrReg, writeVals, numWrRegs);
                        lblResult2.Text = ("���������: " + (BusProtocolErrors.getBusProtocolErrorText(res) + "\r\n"));
                        break;
                }
            }
            catch (Exception ex)
            {
                lblResult2.Text = ("Exception occured: " + ex.Message);
            }
        }

        private void label24_Click(object sender, EventArgs e)
        {

        }

        private void label25_Click(object sender, EventArgs e)
        {

        }

        private void cmbBaudRate_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void tabPage3_Click(object sender, EventArgs e)
        {

        }

        private void dateTimePicker2_ValueChanged(object sender, EventArgs e)
        {

        }

        private void label75_Click(object sender, EventArgs e)
        {

        }

        private void label76_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)                         // �������� ��������� �����
        {

            ushort[] writeVals = new ushort[20];
            bool[] coilVals = new bool[2];
            int slave;                           //
            int res;
            int startWrReg;
            int numWrRegs;   //

            slave = int.Parse(txtSlave.Text, CultureInfo.CurrentCulture);

            int numVal = -1;

            //    string command = DateTime.Parse(label80.Text, CultureInfo.CurrentCulture).ToString("ddMMyyyyHHmmss", CultureInfo.CurrentCulture);

            string command = label80.Text;
            numVal = Convert.ToInt32(command.Substring(0, 2), CultureInfo.CurrentCulture);
            writeVals[0] = (ushort)numVal;   // 
            numVal = Convert.ToInt32(command.Substring(3, 2), CultureInfo.CurrentCulture);
            writeVals[1] = (ushort)numVal;   // 
            numVal = Convert.ToInt32(command.Substring(6, 4), CultureInfo.CurrentCulture);
            writeVals[2] = (ushort)numVal;   // 
            numVal = Convert.ToInt32(command.Substring(11, 2), CultureInfo.CurrentCulture);
            writeVals[3] = (ushort)numVal;   // 
            numVal = Convert.ToInt32(command.Substring(14, 2), CultureInfo.CurrentCulture);
            writeVals[4] = (ushort)numVal;   // 

            startWrReg = 52;
            numWrRegs = 6;   //

            res = myProtocol.writeMultipleRegisters(slave, startWrReg, writeVals, numWrRegs);
            startWrReg = 120;
            res = myProtocol.writeSingleRegister(slave, startWrReg, 14);                          // �������� ��������� �����
        }

        private void button4_Click(object sender, EventArgs e)                         // �������� ���������������� �����
        {

            ushort[] writeVals = new ushort[20];
            bool[] coilVals = new bool[2];
            int slave;                           //
            int res;
            int startWrReg;
            int numWrRegs;   //

            slave = int.Parse(txtSlave.Text, CultureInfo.CurrentCulture);

            int numVal = -1;

            string command = dateTimePicker1.Value.ToString("ddMMyyyyHHmmss", CultureInfo.CurrentCulture);

            numVal = Convert.ToInt32(command.Substring(0, 2), CultureInfo.CurrentCulture);
            writeVals[0] = (ushort)numVal;   // 
            numVal = Convert.ToInt32(command.Substring(2, 2), CultureInfo.CurrentCulture);
            writeVals[1] = (ushort)numVal;   // 
            numVal = Convert.ToInt32(command.Substring(4, 4), CultureInfo.CurrentCulture);
            writeVals[2] = (ushort)numVal;   // 
            numVal = Convert.ToInt32(command.Substring(8, 2), CultureInfo.CurrentCulture);
            writeVals[3] = (ushort)numVal;   // 
            numVal = Convert.ToInt32(command.Substring(10, 2), CultureInfo.CurrentCulture);
            writeVals[4] = (ushort)numVal;   // 

            startWrReg = 52;
            numWrRegs = 6;   //
            res = myProtocol.writeMultipleRegisters(slave, startWrReg, writeVals, numWrRegs);
            startWrReg = 120;
            res = myProtocol.writeSingleRegister(slave, startWrReg, 14);                       // �������� ����� ����� ������������
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)                         // ������� ������ � ��������
        {
            if ((myProtocol != null))
            { 
                // Close protocol and serial port
                myProtocol.closeProtocol();
                //    // Indicate result on status line
                lblResult.Text = "�������� ������";
                //    // Disable button controls
                button5.Enabled = false;
                cmdOpenSerial.Enabled = true;
                Polltimer1.Enabled = false;
                toolStripStatusLabel1.Text = "  MODBUS ������   ";
                toolStripStatusLabel1.BackColor = Color.Red;
                toolStripStatusLabel3.Text = ("");
                portFound = false;
             }
          
        }

        private void button6_Click(object sender, EventArgs e)                        // ������� TCP � ��������
        {
            if ((myProtocol != null))
            {
                // Close protocol and serial port
                myProtocol.closeProtocol();
                //    // Indicate result on status line
                lblResult.Text = "�������� ������";
                //    // Disable button controls
                button6.Enabled = false;
                cmdOpenTCP.Enabled = true;
                Polltimer1.Enabled = false;
                toolStripStatusLabel1.Text = "  MODBUS ������   ";
                toolStripStatusLabel1.BackColor = Color.Red;
            }
        }
        #region label all
        private void label48_Click(object sender, EventArgs e)
        {

        }

        private void tabPage6_Click(object sender, EventArgs e)
        {

        }

        // Label
        private void label88_Click(object sender, EventArgs e)
        {

        }
        private void label92_Click_1(object sender, EventArgs e)
        {

        }
        private void label49_Click(object sender, EventArgs e)
        {

        }
        private void label104_Click(object sender, EventArgs e)
        {

        }
        private void label51_Click(object sender, EventArgs e)
        {

        }
        private void label118_Click(object sender, EventArgs e)
        {

        }

        private void label116_Click(object sender, EventArgs e)
        {

        }

        private void label52_Click(object sender, EventArgs e)
        {

        }

        private void label50_Click(object sender, EventArgs e)
        {

        }

        private void label59_Click(object sender, EventArgs e)
        {

        }

        private void label68_Click(object sender, EventArgs e)
        {

        }

        private void label62_Click(object sender, EventArgs e)
        {

        }

        private void label159_Click(object sender, EventArgs e)
        {

        }

        private void label144_Click(object sender, EventArgs e)
        {

        }

        private void label103_Click(object sender, EventArgs e)
        {

        }
        private void label73_Click(object sender, EventArgs e)
        {

        }

        private void label102_Click(object sender, EventArgs e)
        {

        }

        private void label90_Click(object sender, EventArgs e)
        {

        }

        private void label97_Click(object sender, EventArgs e)
        {

        }

        private void label142_Click(object sender, EventArgs e)
        {

        }

        private void label143_Click(object sender, EventArgs e)
        {

        }

        private void label147_Click(object sender, EventArgs e)
        {

        }

        private void label156_Click(object sender, EventArgs e)
        {

        }

        private void label155_Click(object sender, EventArgs e)
        {

        }

        private void label27_Click(object sender, EventArgs e)
        {

        }

        private void label133_Click(object sender, EventArgs e)
        {

        }

        private void label158_Click(object sender, EventArgs e)
        {

        }

        private void label153_Click(object sender, EventArgs e)
        {

        }

        private void label129_Click(object sender, EventArgs e)
        {

        }

        private void label130_Click(object sender, EventArgs e)
        {

        }

        private void label127_Click(object sender, EventArgs e)
        {

        }

        private void label105_Click(object sender, EventArgs e)
        {

        }

        private void label63_Click(object sender, EventArgs e)
        {

        }

        private void label45_Click(object sender, EventArgs e)
        {

        }

        private void label47_Click(object sender, EventArgs e)
        {

        }
        private void label30_Click(object sender, EventArgs e)
        {

        }

        private void label33_Click(object sender, EventArgs e)
        {

        }

        private void label83_Click(object sender, EventArgs e)
        {

        }

        private void label71_Click(object sender, EventArgs e)
        {

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }
        private void checkBoxSenGar2instr_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void label92_Click_2(object sender, EventArgs e)
        {

        }

        private void label95_Click(object sender, EventArgs e)
        {

        }



        private void label133_Click_1(object sender, EventArgs e)
        {

        }

        private void label80_Click(object sender, EventArgs e)
        {

        }

        private void label37_Click(object sender, EventArgs e)
        {

        }
        private void label101_Click(object sender, EventArgs e)
        {

        }

        private void label134_Click(object sender, EventArgs e)
        {

        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        #endregion
        #region Button test
        private void tabPage5_Click(object sender, EventArgs e)
        {
            if (TabControl1.SelectedIndex == 2)
            {
                // ���������� ��������   "����� ������ � ��������"

            }
        }

        private void button32_Click(object sender, EventArgs e)                        //����� ����� "����� ������ � ��������"
        {
            Polltimer1.Enabled = false;                                                // ��������� ����� ���������
            timer_Mic_test.Enabled = false;                                            // ��������� ���� ���������
            timerCTS.Enabled = false;
            timerTestAll.Enabled = false;


            bool[] coilVals = new bool[200];
            bool[] coilArr = new bool[2];
            slave = int.Parse(txtSlave.Text, CultureInfo.CurrentCulture);
            progressBar1.Value = 0;
            startCoil = 8;                                                             // ���������� �������� ����� "��������"
            res = myProtocol.writeCoil(slave, startCoil, true);                        // �������� ������� ����� "��������"
            Thread.Sleep(1700);
            button32.BackColor = Color.Lime;                                           // ��������� ����� ������
            button31.BackColor = Color.LightSalmon;
            label102.Text = "����������� �������� ��������� ��������";
            label102.ForeColor = Color.DarkOliveGreen;

            numRdRegs = 2;
            startCoil = 124;                                                                      // regBank.add(120);  ���� ��������� ������������� ����� ������
            numCoils = 2;
            res = myProtocol.readCoils(slave, startCoil, coilArr, numCoils);                       // ��������� ����� 120  ��������� ������������� ����� ������
            if (coilArr[0] == true) //���� ������
            {
                // ��������� ������.
                textBox11.Text = ("����� �� �������� ������ �������� �� ����������� !" + "\r\n" + "\r\n");
            }

            timer_byte_set.Enabled = true;                                           // �������� �������� ��������� ������ ��������            

        }

        private void button31_Click(object sender, EventArgs e)                       //������� ����� "����� ������ � ��������"
        {
            startCoil = 33; // ���������� ���� CTS

            res = myProtocol.writeCoil(slave, startCoil, false);
            button31.BackColor = Color.Red;
            button32.BackColor = Color.White;
            label102.Text = "�������� ��������� �������� ����������";
            label102.ForeColor = Color.Red;
            progressBar1.Value = 0;
            timer_byte_set.Enabled = false;
            Polltimer1.Enabled = true;
            textBox11.Text = "";
            startCoil = 8;                                                            // ���������� �������� ����� "��������"
            res = myProtocol.writeCoil(slave, startCoil, false);                      // ��������� ������� ����� "��������"
        }

        private void progressBar1_Click(object sender, EventArgs e)
        {

        }


        //****************  ��������� ���� ************************************************
        private void button37_Click(object sender, EventArgs e)                      // ��� ���� RL0
        {
            startCoil = 1; // ���������� ���������
            res = myProtocol.writeCoil(slave, startCoil, true);
        }
        private void button48_Click(object sender, EventArgs e)                      // ���� ���� RL0
        {

            startCoil = 1; // ���������� ���������
            res = myProtocol.writeCoil(slave, startCoil, false);
        }

        private void button40_Click(object sender, EventArgs e)                      // ��� ���� RL1
        {
            startCoil = 2; // ���������� ���������
            res = myProtocol.writeCoil(slave, startCoil, true);
        }
        private void button53_Click(object sender, EventArgs e)                      // ���� ���� RL1
        {
            startCoil = 2; // ���������� ���������
            res = myProtocol.writeCoil(slave, startCoil, false);
        }

        private void button44_Click(object sender, EventArgs e)                      // ��� ���� RL2
        {
            startCoil = 3; // ���������� ���������
            res = myProtocol.writeCoil(slave, startCoil, true);
        }
        private void button79_Click(object sender, EventArgs e)                      // ���� ���� RL2
        {
            startCoil = 3; // ���������� ���������
            res = myProtocol.writeCoil(slave, startCoil, false);
        }

        private void button49_Click(object sender, EventArgs e)                      // ��� ���� RL3
        {
            startCoil = 4; // ���������� ���������
            res = myProtocol.writeCoil(slave, startCoil, true);
        }
        private void button66_Click(object sender, EventArgs e)                      // ���� ���� RL3
        {
            startCoil = 4; // ���������� ���������
            res = myProtocol.writeCoil(slave, startCoil, false);
        }

        private void button38_Click(object sender, EventArgs e)                      // ��� ���� RL4
        {
            startCoil = 5; // �������� ����������� ��������
            res = myProtocol.writeCoil(slave, startCoil, true);
        }
        private void button52_Click(object sender, EventArgs e)                      // ���� ���� RL4
        {
            startCoil = 5; // ���������� ���������
            res = myProtocol.writeCoil(slave, startCoil, false);
        }

        private void button71_Click(object sender, EventArgs e)                      // ��� ���� RL5
        {
            startCoil = 6; // ���������� ���������
            res = myProtocol.writeCoil(slave, startCoil, true);
        }
        private void button47_Click(object sender, EventArgs e)                      // ���� ���� RL5
        {
            startCoil = 6; // ���������� ���������
            res = myProtocol.writeCoil(slave, startCoil, false);
        }

        private void button69_Click(object sender, EventArgs e)                      // ��� ���� RL6
        {
            startCoil = 7; // ���������� ���������
            res = myProtocol.writeCoil(slave, startCoil, true);
        }
        private void button42_Click(object sender, EventArgs e)                      // ���� ���� RL6
        {
            startCoil = 7; // ���������� ���������
            res = myProtocol.writeCoil(slave, startCoil, false);
        }

        private void button51_Click(object sender, EventArgs e)                      // ��� ���� RL7
        {
            startCoil = 8; // ���������� ���������
            res = myProtocol.writeCoil(slave, startCoil, true);
        }
        private void button45_Click(object sender, EventArgs e)                      // ���� ���� RL7
        {
            startCoil = 8; // ���������� ���������
            res = myProtocol.writeCoil(slave, startCoil, false);
        }

        private void button46_Click(object sender, EventArgs e)                      // ��� ���� RL8
        {
            startCoil = 9; // ���������� ���������
            res = myProtocol.writeCoil(slave, startCoil, true);
        }
        private void button50_Click(object sender, EventArgs e)                      // ���� ���� RL8
        {
            startCoil = 9; // ���������� ���������
            res = myProtocol.writeCoil(slave, startCoil, false);
        }

        private void button27_Click(object sender, EventArgs e)                      // ��� ���� RL9
        {
            startCoil = 10; // �������� ���������� ��������
            res = myProtocol.writeCoil(slave, startCoil, true);
        }
        private void button30_Click(object sender, EventArgs e)                      // ���� ���� RL9
        {
            startCoil = 10; // �������� ���������� ���������
            res = myProtocol.writeCoil(slave, startCoil, false);
        }
        private void button2_Click(object sender, EventArgs e)                       // ��� ���� RL10
        {
            startCoil = 11; // ������� �������������� ��������
            res = myProtocol.writeCoil(slave, startCoil, true);
        }

        private void button81_Click(object sender, EventArgs e)                      // ���� ���� RL10
        {
            startCoil = 11; // ������� �������������� ���������
            res = myProtocol.writeCoil(slave, startCoil, false);
        }

        //**********************************************************************************************




        private void button55_Click(object sender, EventArgs e)                      // ������  ��� ������� +12 ��-�����2
        {
            startCoil = 24;                                                          // ���������� ���������
            res = myProtocol.writeCoil(slave, startCoil, true);
        }
        private void button67_Click(object sender, EventArgs e)                      // ������  ���� ������� +12 ��-�����2
        {
            startCoil = 24;                                                           // ���������� ���������
            res = myProtocol.writeCoil(slave, startCoil, false);
        }

        private void button56_Click(object sender, EventArgs e)                      // ������  ��� ������� +12 ��-�����1
        {
             startCoil = 23; // ���������� ���������
            res = myProtocol.writeCoil(slave, startCoil, true);
        }
        private void button68_Click(object sender, EventArgs e)                      // ������  ���� ������� +12 � ��-�����1
        {
            startCoil = 23; // ���������� ���������
            res = myProtocol.writeCoil(slave, startCoil, false);
        }

        private void button57_Click(object sender, EventArgs e)                      // ������  ��� ����������� ������    XP1- 19 HaSs  
        {
            startCoil = 25; // ���������� ���������
            res = myProtocol.writeCoil(slave, startCoil, false);
        }
        private void button70_Click(object sender, EventArgs e)                      // ������  ���� ����������� ������    XP1- 19 HaSs  
        {
            startCoil = 25; // ���������� ���������
            res = myProtocol.writeCoil(slave, startCoil, true);
        }

        private void button58_Click(object sender, EventArgs e)                      // ������  ��� ������  ����.� 
        {
            startCoil = 19; // ���������� ���������
            res = myProtocol.writeCoil(slave, startCoil, true);
        }

        private void button72_Click(object sender, EventArgs e)                      // ������  ���� ������ ����.� 
        {
            startCoil = 19; // ���������� ���������
            res = myProtocol.writeCoil(slave, startCoil, false);
        }

        private void button59_Click(object sender, EventArgs e)                      // ������  ��� ������ ����.�
        {
            startCoil = 13; // ���������� ���������
            res = myProtocol.writeCoil(slave, startCoil, true);
        }

        private void button74_Click(object sender, EventArgs e)                      // ������  ���� ������ ����.�
        {
            startCoil = 13; // ���������� ���������
            res = myProtocol.writeCoil(slave, startCoil, false);
        }

        private void button60_Click(object sender, EventArgs e)                      // ������  ��� ������  ���.
        {
            //startCoil = 21; // ���������� ���������
            //res = myProtocol.writeCoil(slave, startCoil, true);
        }

        private void button76_Click(object sender, EventArgs e)                      // ������  ���� ������  ���.
        {
            //startCoil = 21; // ���������� ���������
            //res = myProtocol.writeCoil(slave, startCoil, false);
        }

        private void button61_Click(object sender, EventArgs e)                      // 7 ������  ���   XP1- 16 HeS2Rs ������ ����������� ��������� ����������� � 2 ����������
        {
            startCoil = 27; // ���������� ���������
            res = myProtocol.writeCoil(slave, startCoil, true);
        }

        private void button78_Click(object sender, EventArgs e)                      // 7 ������  ����  XP1- 16 HeS2Rs  ������ ����������� ��������� ����������� � 2 ����������
        {
            startCoil = 27; // ���������� ���������
            res = myProtocol.writeCoil(slave, startCoil, false);
        }

        private void button62_Click(object sender, EventArgs e)                      // ��� XP1- 13 HeS2Ls ������  ��� ���� ����������� ��������� ����������� 
        {
            startCoil = 29; // ���������� ���������
            res = myProtocol.writeCoil(slave, startCoil, true);
        }

        private void button77_Click(object sender, EventArgs e)                      // ���� XP1- 13 HeS2Ls ������  ���� ���� ����������� ��������� �����������  
        {
            startCoil = 29; // ���������� ���������
            res = myProtocol.writeCoil(slave, startCoil, false);
        }

        private void button63_Click(object sender, EventArgs e)                      //  XP1- 5  HeS1Rs ������  ��� ���� ����������� ��������� ���������� � 2 ����������
        {
            startCoil = 31; // ���������� ���������
            res = myProtocol.writeCoil(slave, startCoil, true);
        }

        private void button75_Click(object sender, EventArgs e)                      //  XP1- 5  HeS1Rs ������  ���� ���� ����������� ��������� ���������� � 2 ����������
        {
            startCoil = 31; // ���������� ���������
            res = myProtocol.writeCoil(slave, startCoil, false);
        }

        private void button64_Click(object sender, EventArgs e)                      // XP1- 1  HeS1Ls  ������  ���  ���� ����������� ��������� ����������
        {
            startCoil = 32; // ���������� ���������
            res = myProtocol.writeCoil(slave, startCoil, true);
        }

        private void button73_Click(object sender, EventArgs e)                      // XP1- 1  HeS1Ls  ������  ���� ���� ����������� ��������� ����������
        {
            startCoil = 32; // ���������� ���������
            res = myProtocol.writeCoil(slave, startCoil, false);
        }

        private void button33_Click(object sender, EventArgs e)                      //  ������  ��� ������ ���.
        {
            startCoil = 16; // ���������� ���������
            res = myProtocol.writeCoil(slave, startCoil, true);
        }

        private void button34_Click(object sender, EventArgs e)                      // ������  ���� ������ ���.
        {
            startCoil = 16; // ���������� ���������
            res = myProtocol.writeCoil(slave, startCoil, false);
        }

        private void button35_Click(object sender, EventArgs e)                      //  ������  ��� ������� +12 ���
        {
            startCoil = 22; // ���������� ���������
            res = myProtocol.writeCoil(slave, startCoil, true);
        }

        private void button36_Click(object sender, EventArgs e)                      //  ������  ���� ������� +12 ���
        {
            startCoil = 22; // ���������� ���������
            res = myProtocol.writeCoil(slave, startCoil, false);
        }


        //********************** ������ ������� ������*****************************
        private void label28_Click(object sender, EventArgs e) // 
        {
        }

        private void button7_Click(object sender, EventArgs e)                       // ��� . XP8 - 1  PTT ���
        {
            startCoil = 15; // ���������� ���������
            res = myProtocol.writeCoil(slave, startCoil, true);
        }

        private void button18_Click(object sender, EventArgs e)                      // ���� XP8 - 1  PTT ���
        {
            startCoil = 15; // ���������� ���������
            res = myProtocol.writeCoil(slave, startCoil, false);
        }

        private void button10_Click(object sender, EventArgs e)                      // ���   XP7 1 PTT1 ����. �.
        {
            startCoil = 20; // ���������� ���������
            res = myProtocol.writeCoil(slave, startCoil, true);
        }

        private void button23_Click(object sender, EventArgs e)                      // ����   XP7 1 PTT1 ����. �.
        {
            startCoil = 20; // ���������� ���������
            res = myProtocol.writeCoil(slave, startCoil, false);
        }

        private void button14_Click(object sender, EventArgs e)                      // ���  XP7 4 PTT2 ����. �.
        {
            startCoil = 17; // ���������� ���������
            res = myProtocol.writeCoil(slave, startCoil, true);
        }

        private void button29_Click(object sender, EventArgs e)                      // ����  XP7 4 PTT2 ����. �.
        {
            startCoil = 17; // ���������� ���������
            res = myProtocol.writeCoil(slave, startCoil, false);
        }

        private void button19_Click(object sender, EventArgs e)                      // ��� XP1 - 20  HangUp  DCD
        {
            startCoil = 18; // ���������� ���������
            res = myProtocol.writeCoil(slave, startCoil, true);
        }

        private void button26_Click(object sender, EventArgs e)                      // ���� XP1 - 20  HangUp  DCD
        {
            startCoil = 18; // ���������� ���������
            res = myProtocol.writeCoil(slave, startCoil, false);
        }

        private void button8_Click(object sender, EventArgs e)                       // ��� XP1- 6  HeS1PTT   CTS ���
        {
            startCoil = 30; // ���������� ���������
            res = myProtocol.writeCoil(slave, startCoil, true);
        }

        private void button22_Click(object sender, EventArgs e)                      // ����  XP1- 6  HeS1PTT   CTS ���
        {
            startCoil = 30; // ���������� ���������
            res = myProtocol.writeCoil(slave, startCoil, false);
        }

        private void button28_Click(object sender, EventArgs e)                      //���  XP1- 15 HeS2PTT   CTS ���
        {
            startCoil = 28; // ���������� ���������
            res = myProtocol.writeCoil(slave, startCoil, true);
        }

        private void button17_Click(object sender, EventArgs e)                      // ����  XP1- 15 HeS2PTT   CTS ���
        {
            startCoil = 28; // ���������� ���������
            res = myProtocol.writeCoil(slave, startCoil, false);
        }

        private void button16_Click(object sender, EventArgs e)                      //���  XP1- 17 HaSPTT    CTS DSR ���.
        {
            startCoil = 26; // ���������� ���������
            res = myProtocol.writeCoil(slave, startCoil, true);
        }

        private void button20_Click(object sender, EventArgs e)                      //����  XP1- 17 HaSPTT    CTS DSR ���.
        {
            startCoil = 26; // ���������� ���������
            res = myProtocol.writeCoil(slave, startCoil, false);
        }


        private void button39_Click(object sender, EventArgs e)                      // ��� XP8-1 ��� ����. �.
        {
            startCoil = 14; // ���������� ���������
            res = myProtocol.writeCoil(slave, startCoil, true);
        }

        private void button41_Click(object sender, EventArgs e)                      //  ���� XP8-1  ��� ����. �.
        {
            startCoil = 14; // ���������� ���������
            res = myProtocol.writeCoil(slave, startCoil, false);
        }

        private void button43_Click(object sender, EventArgs e)
        {

        }

        //**********************************************************************************************
        #endregion

        #region Test all
        // ��� ������ ������ ���������� ��������� �� ������ 120  � ���������� ����� �����  (1-12)
        // ����� ��������� ������� (true) ������  ����� �� ������ 38

        private void sensor_off()// 
        {
            ushort[] writeVals = new ushort[2];
            short[] readVals = new short[124];
            bool[] coilArr = new bool[64];
            slave = int.Parse(txtSlave.Text, CultureInfo.CurrentCulture);
            startWrReg = 120;
            res = myProtocol.writeSingleRegister(slave, startWrReg, 1); // ��������� ��� �������
            textBox7.Text += ("������� �� ���������� �������� ����������" + "\r\n");
            textBox7.Refresh();
            Thread.Sleep(700);

            // ����� �������� ������ ��������� 40001-40007

            int startRdReg;
            int numRdRegs;
            int i;
            bool[] coilVals = new bool[64];
            bool[] coilSensor = new bool[64];

            //*************************  �������� ������ ��������� ������ �������� ************************************

            Int64 binaryHolder;
            string binaryResult = "";
            int decimalNum;
            bool[] Dec_bin = new bool[64];
            startRdReg = 1;
            numRdRegs = 7;
            res = myProtocol.readMultipleRegisters(slave, startRdReg, readVals, numRdRegs);    // 03  ������� ����� �� ��������� �� ������  40000 -49999
            label78.Text = ("���������: " + (BusProtocolErrors.getBusProtocolErrorText(res) + "\r\n"));
            if ((res == BusProtocolErrors.FTALK_SUCCESS))
            {
                toolStripStatusLabel1.Text = "    MODBUS ON    ";
                toolStripStatusLabel1.BackColor = Color.Lime;

                for (int bite_x = 0; bite_x < 7; bite_x++)
                {
                    decimalNum = readVals[bite_x];
                    while (decimalNum > 0)
                    {
                        binaryHolder = decimalNum % 2;
                        binaryResult += binaryHolder;
                        decimalNum = decimalNum / 2;
                    }

                    int len_str = binaryResult.Length;

                    while (len_str < 8)
                    {
                        binaryResult += 0;
                        len_str++;
                    }

                    //****************** �������� ����� ***************************
                    //binaryArray = binaryResult.ToCharArray();
                    //Array.Reverse(binaryArray);
                    //binaryResult = new string(binaryArray);
                    //*************************************************************

                    for (i = 0; i < 8; i++)                         // 
                    {
                        if (binaryResult[i] == '1')
                        {
                            Dec_bin[i + (8 * bite_x)] = true;
                        }
                        else
                        {
                            Dec_bin[i + (8 * bite_x)] = false;
                        }
                    }
                    binaryResult = "";
                }

            }

            test_end();
        }
        private void sensor_on()
        {
            ushort[] writeVals = new ushort[2];

            short[] readVals = new short[124];
            bool[] coilArr = new bool[64];
            slave = int.Parse(txtSlave.Text, CultureInfo.CurrentCulture);
            startWrReg = 120;
            res = myProtocol.writeSingleRegister(slave, startWrReg, 2); // �������� ��� �������
            textBox7.Text += ("������� �� ��������� �������� ����������" + "\r\n");
            textBox7.Refresh();

            Thread.Sleep(600);

            //  �������� ������ ��������� 40001-40007 ��������� "��������"

            int startRdReg;
            int numRdRegs;
            int i;
            bool[] coilVals = new bool[64];
            bool[] coilSensor = new bool[64];

            //*************************  �������� ������ ��������� ������ �������� ************************************

            Int64 binaryHolder;
            string binaryResult = "";
            int decimalNum;
            bool[] Dec_bin = new bool[64];
            startRdReg = 1;
            numRdRegs = 7;
            res = myProtocol.readMultipleRegisters(slave, startRdReg, readVals, numRdRegs);    // 03  ������� ����� �� ��������� �� ������  40000 -49999
            label78.Text = ("���������: " + (BusProtocolErrors.getBusProtocolErrorText(res) + "\r\n"));
            if ((res == BusProtocolErrors.FTALK_SUCCESS))
            {
                toolStripStatusLabel1.Text = "    MODBUS ON    ";
                toolStripStatusLabel1.BackColor = Color.Lime;

                for (int bite_x = 0; bite_x < 7; bite_x++)
                {
                    decimalNum = readVals[bite_x];
                    while (decimalNum > 0)
                    {
                        binaryHolder = decimalNum % 2;
                        binaryResult += binaryHolder;
                        decimalNum = decimalNum / 2;
                    }

                    int len_str = binaryResult.Length;

                    while (len_str < 8)
                    {
                        binaryResult += 0;
                        len_str++;
                    }

                    //****************** �������� ����� ***************************
                    //binaryArray = binaryResult.ToCharArray();
                    //Array.Reverse(binaryArray);
                    //binaryResult = new string(binaryArray);
                    //*************************************************************

                    for (i = 0; i < 8; i++)                         // 
                    {
                        if (binaryResult[i] == '1')
                        {
                            Dec_bin[i + (8 * bite_x)] = true;
                        }
                        else
                        {
                            Dec_bin[i + (8 * bite_x)] = false;
                        }
                    }
                    binaryResult = "";
                }

            }
            //if (Dec_bin[24] == false) // ���� ����������� �� �����2
            //{
            //    textBox8.Text += ("������ �� �����2 �� ���������" + "\r\n");
            //    textBox8.Refresh();
            //}

            //if (Dec_bin[25] == false) // ���� ����������� �� �����1
            //{
            //    textBox8.Text += ("������ �� �����1 �� ���������" + "\r\n");
            //    textBox8.Refresh();
            //}
/*
            if (Dec_bin[26] == false) //  ���� ����������� ������
            {
                //    textBox8.Text += ("������ ����������� ������ �� ���������" + "\r\n");
                textBox8.Refresh();
            }



            if (Dec_bin[27] == false)   // ���� ����������� ������ ��������
            {
                //     textBox8.Text += ("������ ����������� ������ �������� �� ���������" + "\r\n");
                textBox8.Refresh();
            }


            if (Dec_bin[28] == false)  // ���� ����������� ������
            {
                //    textBox8.Text += ("������ ����������� ������ �� ���������" + "\r\n");
                textBox8.Refresh();
            }
            */

            //if (Dec_bin[40] == false) //  ���� ����������� �����������
            //{
            //    textBox8.Text += ("������ ����������� ����������� �� ���������" + "\r\n");
            //    textBox8.Refresh();
            //}

/*
            if (Dec_bin[41] == false) //  ���� ����������� ��������� ����������� 2 ����������
            {
                //    textBox8.Text += ("������ ���������� ��������� ����������� 2 ���������� �� ���������" + "\r\n");
                textBox8.Refresh();
            }

            if (Dec_bin[42] == false) //   ���� ����������� ��������� �����������
            {
                //    textBox8.Text += ("������ ����������� ��������� ����������� �� ���������" + "\r\n");
                textBox8.Refresh();
            }


            if (Dec_bin[43] == false) //  ���� ����������� ��������� ���������� � 2 ����������
            {
                //    textBox8.Text += ("������ ����������� ��������� ���������� � 2 ���������� �� ���������" + "\r\n");
                textBox8.Refresh();
            }


            if (Dec_bin[44] == false) //  ���� ����������� ��������� ����������
            {
                //    textBox8.Text += ("������ ����������� ��������� ���������� �� ���������" + "\r\n");
                textBox8.Refresh();
            }


            if (Dec_bin[45] == false) //  ���� ����������� ��������� XS1 - 6 Sence  
            {
                //    textBox8.Text += ("������ ����������� ��������� �� ���������" + "\r\n");
                textBox8.Refresh();
            }
            */
            //if (Dec_bin[46] == false) // ���� ����������� ���
            //{
            //    textBox8.Text += ("������ ����������� ��� �� ���������" + "\r\n");
            //    textBox8.Refresh();
            //}
            //if (Dec_bin[52] == false) //  ���� ����������� ��������� ����������
            //{
            //    textBox8.Text += ("�������� ��������� ����������� �� ���������" + "\r\n");
            //    textBox8.Refresh();
            //}


            //if (Dec_bin[54] == false) //  ���� ����������� ��������� XS1 - 6 Sence  
            //{
            //    textBox8.Text += ("�������� ��������� ���������� �� ���������" + "\r\n");
            //    textBox8.Refresh();
            //}


            test_end();
        }
        private void test_instruktora()
        {
            ushort[] writeVals = new ushort[2];
            bool[] coilArr = new bool[2];
            short[] readVals = new short[125];
            bool[] coilVals = new bool[120];
            slave = int.Parse(txtSlave.Text, CultureInfo.CurrentCulture);
            startWrReg = 120;  // � 40120 ������ �������� ����� �����. ��� ������ ��������� test_switch() Arduino
            res = myProtocol.writeSingleRegister(slave, startWrReg, 3); // ��������� ��� �������
            textBox7.Text += ("������� �� �������� '��������� �����������' ����������" + "\r\n");
            textBox7.Refresh();
            Thread.Sleep(250);

            test_end();
        }
        private void test_dispetchera()
        {
            ushort[] writeVals = new ushort[2];
            bool[] coilArr = new bool[4];
            startWrReg = 120;  // � 40120 ������ �������� ����� �����. ��� ������ ��������� test_switch() Arduino
            res = myProtocol.writeSingleRegister(slave, startWrReg, 4); // ��������� ��� �������
            startCoil = 38; // ��������� ������ ���� , ����� � ����������� 37
            res = myProtocol.writeCoil(slave, startCoil, true);
            textBox7.Text += ("������� �� �������� '��������� ����������' ����������" + "\r\n");
            textBox7.Refresh();
            Thread.Sleep(250);
            test_end();
        }
        private void test_MTT()
        {
            ushort[] writeVals = new ushort[2];
            bool[] coilArr = new bool[4];
            startWrReg = 120;
            res = myProtocol.writeSingleRegister(slave, startWrReg, 5); // ��������� ��� �������
            textBox7.Text += ("������� �� �������� '���' ����������" + "\r\n");
            textBox7.Refresh();
            Thread.Sleep(250);
            test_end();
        }
        private void test_tangR()
        {
            ushort[] writeVals = new ushort[2];
            bool[] coilArr = new bool[4];
            startWrReg = 120;
            res = myProtocol.writeSingleRegister(slave, startWrReg, 6); // ��������� ��� �������
            textBox7.Text += ("������� �� �������� '�������� ������' ����������" + "\r\n");
            textBox7.Refresh();
            Thread.Sleep(250);
            test_end();
        }
        private void test_tangN()
        {
            ushort[] writeVals = new ushort[2];
            bool[] coilArr = new bool[4];
            startWrReg = 120;
            res = myProtocol.writeSingleRegister(slave, startWrReg, 7); // ��������� ��� �������
            textBox7.Text += ("������� �� �������� '�������� ������' ����������" + "\r\n");
            textBox7.Refresh();
            Thread.Sleep(250);
            test_end();
        }
        private void testGGS()
        {
            ushort[] writeVals = new ushort[2];
            bool[] coilArr = new bool[4];
            startWrReg = 120;
            res = myProtocol.writeSingleRegister(slave, startWrReg, 8); // ��������� ��� �������
            textBox7.Text += ("������� �� �������� ��� ����������" + "\r\n");
            textBox7.Refresh();
            Thread.Sleep(250);
            test_end();
        }
        private void test_GG_Radio1()
        {
            ushort[] writeVals = new ushort[2];
            bool[] coilArr = new bool[4];
            startWrReg = 120;
            res = myProtocol.writeSingleRegister(slave, startWrReg, 9); // ��������� ��� �������
            textBox7.Text += ("������� �� �������� '��-�����1' ����������" + "\r\n");
            textBox7.Refresh();
            Thread.Sleep(250);
            test_end();
        }
        private void test_GG_Radio2()
        {
            ushort[] writeVals = new ushort[2];
            bool[] coilArr = new bool[4];
            startWrReg = 120;
            res = myProtocol.writeSingleRegister(slave, startWrReg, 10); // ��������� ��� �������
            textBox7.Text += ("������� �� �������� '��-�����2' ����������" + "\r\n");
            textBox7.Refresh();
            Thread.Sleep(250);
            test_end();
        }
        private void test_mikrophon()
        {
            ushort[] writeVals = new ushort[2];
            bool[] coilArr = new bool[4];
            startWrReg = 120;
            res = myProtocol.writeSingleRegister(slave, startWrReg, 11); // ��������� ��� �������
            textBox7.Text += ("������� �� �������� '��������' ����������" + "\r\n");
            textBox7.Refresh();
            Thread.Sleep(250);
            test_end();
        }
        private void test_power()
        {
            ushort[] writeVals = new ushort[2];
            bool[] coilArr = new bool[4];
            startWrReg = 120;
            res = myProtocol.writeSingleRegister(slave, startWrReg, 17); // ��������� ��� �������
            textBox7.Text += ("�������� ���������� �������" + "\r\n");
            textBox7.Refresh();
            Thread.Sleep(250);
            test_end();
            // ���������?
        }
        private void test_valueDisp()
        {
            ushort[] writeVals = new ushort[2];
            bool[] coilArr = new bool[4];
            startWrReg = 120;
            res = myProtocol.writeSingleRegister(slave, startWrReg, 19); // ��������� ��� �������
            textBox7.Text += ("�������� ����������� ������� �������" + "\r\n");
            textBox7.Refresh();
            Thread.Sleep(250);
            test_end();
         }
 
        private void test_end()
        {
            ushort[] readVals = new ushort[2];
            bool[] coilArr = new bool[2];
            startRdReg = 120;                                    //regBank.add(40120);  // adr_control_command ����� �������� ������� �� ����������
                                                                 //  0 � �������� �������� ���������� ���������� ��������� ��������
            numRdRegs = 2;
            startCoil = 120;                                     // regBank.add(120);   // ���� ��������� ������������� ����� ������
            numCoils = 2;
         //   Thread.Sleep(100);

            do
            {
                res = myProtocol.readMultipleRegisters(slave, 120, readVals, numRdRegs);  // �������� ���� ������������� ��������� ��������  ����� �������� ������������� 40120

                if ((res == BusProtocolErrors.FTALK_SUCCESS))
                {
                    toolStripStatusLabel1.Text = "    MODBUS ON    ";
                    toolStripStatusLabel1.BackColor = Color.Lime;
                }

                else
                {
                    toolStripStatusLabel1.Text = "    MODBUS ERROR (4) ";
                    toolStripStatusLabel1.BackColor = Color.Red;
                    Polltimer1.Enabled = false;
                    timer_byte_set.Enabled = false;
                    timer_Mic_test.Enabled = false;
                    timerCTS.Enabled = false;
                    timerTestAll.Enabled = false;
                    portFound = false;
                    find_com_port.Enabled = true;
                   // SetComPort();
                    return;
                }
                Thread.Sleep(50);
            } while (readVals[0] != 0);                                     // ���� readVals[0] == 0 , ���� ��������

            textBox7.Text += ("���������� ������� ���������" + "\r\n");
            textBox7.Refresh();

            res = myProtocol.readCoils(slave, startCoil, coilArr, numCoils);                       // ��������� ����� 120  ��������� ������������� ����� ������
            if (coilArr[0] == true) //���� ������
            {
                // ��������� ������.
                textBox8.Text += ("����� ��������� ��������� ������. " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.CurrentCulture) + "\r\n");
                textBox8.Refresh();
                error_list();
 
             }

       }
        private void test_end1()
        {
            ushort[] readVals = new ushort[2];
            bool[] coilArr = new bool[2];
            startRdReg = 120;                                    //regBank.add(40120);  // adr_control_command ����� �������� ������� �� ����������
            //  0 � �������� �������� ���������� ���������� ��������� ��������
            numRdRegs = 2;
            do
            {
                res = myProtocol.readMultipleRegisters(slave, 120, readVals, numRdRegs);  // �������� ���� ������������� ��������� ��������  ����� �������� ������������� 40120

                if ((res == BusProtocolErrors.FTALK_SUCCESS))
                {
                    toolStripStatusLabel1.Text = "    MODBUS ON    ";
                    toolStripStatusLabel1.BackColor = Color.Lime;
                }

                else
                {
                    toolStripStatusLabel1.Text = "    MODBUS ERROR (5) ";
                    toolStripStatusLabel1.BackColor = Color.Red;
                    Polltimer1.Enabled = false;
                    portFound = false;
                    find_com_port.Enabled = true;
                   // SetComPort();
                    return;
                }
                Thread.Sleep(50);
            } while (readVals[0] != 0);                                     // ���� readVals[0] == 0 , ���� ��������
        }

        private void error_list()
        {
            error_list1();                              // �������� ���������� ���������  40200 - 40330 ������� �������� ������  
            error_list2();                              // �������� ���������� ���������  40400 - 40530 ������� ���������� 
            error_list3();                              // �������� ���������� ���������  200 - 330 ����� ��������� �������������  ������

           
           if (coilArr_all[0] != false)
            {
                textBox8.Text += ("������  ������ �� ����������                                \t< = " + readVals_all[0] + ">\r\n");
                textBox8.Refresh();
                res = myProtocol.writeCoil(slave, 200, false);
            }

            if (coilArr_all[1] != false)
            {
                textBox8.Text += ("������ �������� ������ �� ����������                      \t< = " + readVals_all[1] + ">\r\n");
                textBox8.Refresh();
                res = myProtocol.writeCoil(slave, 201, false);
            }

            if (coilArr_all[2] != false)
            {
                textBox8.Text += ("������ �������� ������ �� ����������                      \t< = " + readVals_all[2] + ">\r\n");
                textBox8.Refresh();
                textBox8.Refresh(); res = myProtocol.writeCoil(slave, 202, false);
            }

            if (coilArr_all[3] != false)
            {
                textBox8.Text += ("������ ��������� ����������� � 2 ����������  �� ����������\t< = " + readVals_all[3] + ">\r\n");
                textBox8.Refresh();
                res = myProtocol.writeCoil(slave, 203, false);
            }
            if (coilArr_all[4] != false)
            {
                textBox8.Text += ("������ ��������� �����������  �� ����������               \t< = " + readVals_all[4] + ">\r\n");
                textBox8.Refresh();
                res = myProtocol.writeCoil(slave, 204, false);
            }
            if (coilArr_all[5] != false)
            {
                textBox8.Text += ("������ ���������� � 2 ���������� �� ����������            \t< = " + readVals_all[5] + ">\r\n");
                textBox8.Refresh();
                res = myProtocol.writeCoil(slave, 205, false);
            }
            if (coilArr_all[6] != false)
            {
                textBox8.Text += ("������ ���������� �� ����������                               \t< = " + readVals_all[6] + ">\r\n");
                textBox8.Refresh();
                res = myProtocol.writeCoil(slave, 206, false);
            }
            if (coilArr_all[7] != false)
            {
                textBox8.Text += ("������ ��������� �� ����������                                \t< = " + readVals_all[7] + ">\r\n");
                textBox8.Refresh();
                res = myProtocol.writeCoil(slave, 207, false);
            }
            if (coilArr_all[8] != false)
            {
                textBox8.Text += ("�������� ����������� �� ����������                           \t< = " + readVals_all[8] + ">\r\n");
                textBox8.Refresh();
                res = myProtocol.writeCoil(slave, 208, false);
            }
            if (coilArr_all[9] != false)
            {
                textBox8.Text += ("�������� ���������� �� ����������                            \t< = " + readVals_all[9] + ">\r\n");
                textBox8.Refresh();
                res = myProtocol.writeCoil(slave, 209, false);
            }

            if (coilArr_all[10] != false)
            {
                textBox8.Text += ("������  ������ �� ���������                                    \t< = " + readVals_all[10] + ">\r\n");
                textBox8.Refresh();
                res = myProtocol.writeCoil(slave, 210, false);
            }

            if (coilArr_all[11] != false)
            {
                textBox8.Text += ("������ �������� ������ �� ���������                       \t< = " + readVals_all[11] + ">\r\n");
                textBox8.Refresh();
                res = myProtocol.writeCoil(slave, 211, false);
            }

            if (coilArr_all[12] != false)
            {
                textBox8.Text += ("������ �������� ������ �� ���������                       \t< = " + readVals_all[12] + ">\r\n");
                textBox8.Refresh();
                res = myProtocol.writeCoil(slave, 212, false);
            }

            if (coilArr_all[13] != false)
            {
                textBox8.Text += ("������ ��������� ����������� � 2 ����������  �� ��������� \t< = " + readVals_all[13] + ">\r\n");
                textBox8.Refresh();
                res = myProtocol.writeCoil(slave, 213, false);
            }
            if (coilArr_all[14] != false)
            {
                textBox8.Text += ("������ ��������� �����������  �� ���������                \t< = " + readVals_all[14] + ">\r\n");
                textBox8.Refresh();
                res = myProtocol.writeCoil(slave, 214, false);
            }
            if (coilArr_all[15] != false)
            {
                textBox8.Text += ("������ ���������� � 2 ���������� �� ���������          \t< = " + readVals_all[15] + ">\r\n");
                textBox8.Refresh();
                res = myProtocol.writeCoil(slave, 215, false);
            }
            if (coilArr_all[16] != false)
            {
                textBox8.Text += ("������ ���������� �� ���������                               \t< = " + readVals_all[16] + ">\r\n");
                textBox8.Refresh();
                res = myProtocol.writeCoil(slave, 216, false);
            }
            if (coilArr_all[17] != false)
            {
                textBox8.Text += ("������ ��������� �� ���������                                \t< = " + readVals_all[17] + ">\r\n");
                textBox8.Refresh();
                res = myProtocol.writeCoil(slave, 217, false);
            }
            if (coilArr_all[18] != false)
            {
                textBox8.Text += ("�������� ����������� �� ���������                            \t< = " + readVals_all[18] + ">\r\n");
                textBox8.Refresh();
                res = myProtocol.writeCoil(slave, 218, false);
            }
            if (coilArr_all[19] != false)
            {
                textBox8.Text += ("�������� ���������� �� ���������                             \t< = " + readVals_all[19] + ">\r\n");
                textBox8.Refresh();
                res = myProtocol.writeCoil(slave, 219, false);
            }

            if (coilArr_all[20] != false)
            {
                textBox8.Text += ("PTT ����������� �� ����������                                \t< = " + readVals_all[20] + ">\r\n");
                textBox8.Refresh();
                res = myProtocol.writeCoil(slave, 220, false);
            }

            if (coilArr_all[21] != false)
            {
                textBox8.Text += ("PTT ����������� �� ���������                                 \t< = " + readVals_all[21] + ">\r\n");
                textBox8.Refresh();
                res = myProtocol.writeCoil(slave, 221, false);
            }

            if (coilArr_all[22] != false)
            {
                textBox8.Text += ("PTT ���������� �� ����������                                 \t< = " + readVals_all[22] + ">\r\n");
                textBox8.Refresh();
                res = myProtocol.writeCoil(slave, 222, false);
            }

            if (coilArr_all[23] != false)
            {
                textBox8.Text += ("PTT ���������� �� ���������                                 \t< = " + readVals_all[23] + ">\r\n");
                textBox8.Refresh();
                res = myProtocol.writeCoil(slave, 223, false);
            }
            if (coilArr_all[24] != false)
            {
                temp_disp = readVolt_all[24];
                textBox8.Text += ("������ ����������� LineL �����������                      \t< = " + readVals_all[24] + ">  " + temp_disp / 100 + " V \r\n");
                textBox8.Refresh();
                res = myProtocol.writeCoil(slave, 224, false);
            }
            if (coilArr_all[25] != false)
            {
                temp_disp = readVolt_all[25];
                textBox8.Text += ("������ ����������� LineR �����������                      \t< = " + readVals_all[25] + ">  " + temp_disp / 100 + " V \r\n");
                res = myProtocol.writeCoil(slave, 225, false);
            }
            if (coilArr_all[26] != false)
            {
                temp_disp = readVolt_all[26];
                textBox8.Text += ("������ ����������� �� ������� ��� phone ����������� \t< = " + readVals_all[26] + ">  " + temp_disp / 100 + " V \r\n");
                textBox8.Refresh();
                res = myProtocol.writeCoil(slave, 226, false);
            }
            if (coilArr_all[27] != false)
            {
                temp_disp = readVolt_all[27];
                textBox8.Text += ("������ ���������� LineL �����������                       \t< = " + readVals_all[27] + ">  " + temp_disp / 100 + " V\r\n");
                textBox8.Refresh();
                res = myProtocol.writeCoil(slave, 227, false);
            }
            if (coilArr_all[28] != false)
            {
                temp_disp = readVolt_all[28];
                textBox8.Text += ("������ ���������� LineR �����������                       \t< = " + readVals_all[28] + ">  " + temp_disp / 100 + " V\r\n");
                textBox8.Refresh();
                res = myProtocol.writeCoil(slave, 228, false);
            }
            if (coilArr_all[29] != false)
            {
                temp_disp = readVolt_all[29];
                textBox8.Text += ("������ ���������� �� ������� ��� phone �����������    \t< = " + readVals_all[29] + ">  " + temp_disp / 100 + " V\r\n");
                textBox8.Refresh();
                res = myProtocol.writeCoil(slave, 229, false);
            }

            if (coilArr_all[30] != false)
            {
                temp_disp = readVolt_all[30];
                textBox8.Text += ("���� ��������� ����������� ** ������ FrontL     \tOFF          \t< = " + readVals_all[30] + ">  " + temp_disp / 100 + " V\r\n");
                textBox8.Refresh();
                res = myProtocol.writeCoil(slave, 230, false);
            }

            if (coilArr_all[31] != false)
            {
                temp_disp = readVolt_all[31];
                textBox8.Text += ("���� ��������� ����������� ** ������ FrontR     \tOFF         \t< = " + readVals_all[31] + ">  " + temp_disp / 100 + " V\r\n");
                textBox8.Refresh();
                res = myProtocol.writeCoil(slave, 231, false);
            }

            if (coilArr_all[32] != false)
            {
                temp_disp = readVolt_all[32];
                textBox8.Text += ("���� ��������� ����������� ** ������ LineL        \tOFF        \t< = " + readVals_all[32] + ">  " + temp_disp / 100 + " V\r\n");
                textBox8.Refresh();
                res = myProtocol.writeCoil(slave, 232, false);
            }

            if (coilArr_all[33] != false)
            {
                temp_disp = readVolt_all[33];
                textBox8.Text += ("���� ��������� ����������� ** ������ LineR    \tOFF        \t< = " + readVals_all[33] + ">  " + temp_disp / 100 + " V\r\n");
                textBox8.Refresh();
                res = myProtocol.writeCoil(slave, 233, false);
            }
            if (coilArr_all[34] != false)
            {
                temp_disp = readVolt_all[34];
                textBox8.Text += ("���� ��������� ����������� ** ������ mag radio    \tOFF        \t< = " + readVals_all[34] + ">  " + temp_disp / 100 + " V\r\n");
                textBox8.Refresh();
                res = myProtocol.writeCoil(slave, 234, false);
            }
            if (coilArr_all[35] != false)
            {
                temp_disp = readVolt_all[35];
                textBox8.Text += ("���� ��������� ����������� ** ������ mag phone    \tOFF        \t< = " + readVals_all[35] + ">  " + temp_disp / 100 + " V\r\n");
                textBox8.Refresh();
                res = myProtocol.writeCoil(slave, 235, false);
            }
            if (coilArr_all[36] != false)
            {
                temp_disp = readVolt_all[36];
                textBox8.Text += ("���� ��������� ����������� ** ������ ���          \tOFF        \t< = " + readVals_all[36] + ">  " + temp_disp / 100 + " V\r\n");
                textBox8.Refresh();
                res = myProtocol.writeCoil(slave, 236, false);
            }
            if (coilArr_all[37] != false)
            {
                temp_disp = readVolt_all[37];
                textBox8.Text += ("���� ��������� ����������� ** ������ �� �����1    \tOFF        \t< = " + readVals_all[37] + ">  " + temp_disp / 100 + " V\r\n");
                textBox8.Refresh();
                res = myProtocol.writeCoil(slave, 237, false);
            }
            if (coilArr_all[38] != false)
            {
                temp_disp = readVolt_all[38];
                textBox8.Text += ("���� ��������� ����������� ** ������ �� �����2    \tOFF        \t< = " + readVals_all[38] + ">  " + temp_disp / 100 + " V\r\n");
                textBox8.Refresh();
                res = myProtocol.writeCoil(slave, 238, false);
            }
            if (coilArr_all[39] != false)
            {
                temp_disp = readVolt_all[39];
                textBox8.Text += ("������! ��� ���������� �� � �����  < = " + readVals_all[38] + ">  " + temp_disp / 100 + " V\r\n");
                textBox8.Refresh();
                res = myProtocol.writeCoil(slave, 239, false);
            }

            if (coilArr_all[40] != false)
            {
                temp_disp = readVolt_all[40];
                textBox8.Text += ("���� ��������� ���������� ** ������ FrontL     \tOFF          \t< = " + readVals_all[40] + ">  " + temp_disp / 100 + " V\r\n");
                textBox8.Refresh();
                res = myProtocol.writeCoil(slave, 240, false);
            }

            if (coilArr_all[41] != false)
            {
                temp_disp = readVolt_all[41];
                textBox8.Text += ("���� ��������� ���������� ** ������ FrontR      \tOFF         \t< = " + readVals_all[41] + ">  " + temp_disp / 100 + " V\r\n");
                textBox8.Refresh();
                res = myProtocol.writeCoil(slave, 241, false);
            }

            if (coilArr_all[42] != false)
            {
                temp_disp = readVolt_all[42];
                textBox8.Text += ("���� ��������� ���������� ** ������ LineL       \tOFF         \t< = " + readVals_all[42] + ">  " + temp_disp / 100 + " V\r\n");
                textBox8.Refresh();
                res = myProtocol.writeCoil(slave, 242, false);
            }

            if (coilArr_all[43] != false)
            {
                temp_disp = readVolt_all[43];
                textBox8.Text += ("���� ��������� ���������� ** ������ LineR       \tOFF         \t< = " + readVals_all[43] + ">  " + temp_disp / 100 + " V\r\n");
                textBox8.Refresh();
                res = myProtocol.writeCoil(slave, 243, false);
            }
            if (coilArr_all[44] != false)
            {
                temp_disp = readVolt_all[44];
                textBox8.Text += ("���� ��������� ���������� ** ������ mag radio    \tOFF        \t< = " + readVals_all[44] + ">  " + temp_disp / 100 + " V\r\n");
                textBox8.Refresh();
                res = myProtocol.writeCoil(slave, 244, false);
            }
            if (coilArr_all[45] != false)
            {
                temp_disp = readVolt_all[45];
                textBox8.Text += ("���� ��������� ���������� ** ������ mag phone    \tOFF        \t< = " + readVals_all[45] + ">  " + temp_disp / 100 + " V\r\n");
                res = myProtocol.writeCoil(slave, 245, false);
            }
            if (coilArr_all[46] != false)
            {
                temp_disp = readVolt_all[46];
                textBox8.Text += ("���� ��������� ���������� ** ������ ���          \tOFF        \t< = " + readVals_all[46] + ">  " + temp_disp / 100 + " V\r\n");
                textBox8.Refresh();
                res = myProtocol.writeCoil(slave, 246, false);
            }
            if (coilArr_all[47] != false)
            {
                temp_disp = readVolt_all[47];
                textBox8.Text += ("���� ��������� ���������� ** ������ �� �����1    \tOFF        \t< = " + readVals_all[47] + ">  " + temp_disp / 100 + " V\r\n");
                textBox8.Refresh();
                res = myProtocol.writeCoil(slave, 247, false);
            }
            if (coilArr_all[48] != false)
            {
                temp_disp = readVolt_all[48];
                textBox8.Text += ("���� ��������� ���������� ** ������ �� �����2    \tOFF        \t< = " + readVals_all[48] + ">  " + temp_disp / 100 + " V\r\n");
                textBox8.Refresh();
                res = myProtocol.writeCoil(slave, 248, false);
            }
            if (coilArr_all[49] != false)
            {
                temp_disp = readVolt_all[49];
                textBox8.Text += ("������! ��� ���������� �� � �����                             \t< = " + readVals_all[49] + ">  " + temp_disp / 100 + " V\r\n");
                textBox8.Refresh();
                res = myProtocol.writeCoil(slave, 249, false);
            }

            if (coilArr_all[50] != false)
            {
                temp_disp = readVolt_all[50];
                textBox8.Text += ("���� ��� (������) ** ������ FrontL                      \tOFF        \t< = " + readVals_all[50] + ">  " + temp_disp / 100 + " V\r\n");
                textBox8.Refresh();
                res = myProtocol.writeCoil(slave, 250, false);
            }

            if (coilArr_all[51] != false)
            {
                temp_disp = readVolt_all[51];
                textBox8.Text += ("���� ��� (������) ** ������ FrontR                      \tOFF        \t< = " + readVals_all[51] + ">  " + temp_disp / 100 + " V\r\n");
                textBox8.Refresh();
                res = myProtocol.writeCoil(slave, 251, false);
            }

            if (coilArr_all[52] != false)
            {
                temp_disp = readVolt_all[52];
                textBox8.Text += ("���� ��� (������) ** ������ LineL                       \tOFF        \t< = " + readVals_all[52] + ">  " + temp_disp / 100 + " V\r\n");
                textBox8.Refresh();
                res = myProtocol.writeCoil(slave, 252, false);
            }

            if (coilArr_all[53] != false)
            {
                temp_disp = readVolt_all[53];
                textBox8.Text += ("���� ��� (������) ** ������ LineR                       \tOFF        \t< = " + readVals_all[53] + ">  " + temp_disp / 100 + " V\r\n");
                textBox8.Refresh();
                res = myProtocol.writeCoil(slave, 253, false);
            }
            if (coilArr_all[54] != false)
            {
                temp_disp = readVolt_all[54];
                textBox8.Text += ("���� ��� (������) ** ������ mag radio                   \tOFF        \t< = " + readVals_all[54] + ">  " + temp_disp / 100 + " V\r\n");
                textBox8.Refresh();
                res = myProtocol.writeCoil(slave, 254, false);
            }
            if (coilArr_all[55] != false)
            {
                temp_disp = readVolt_all[55];
                textBox8.Text += ("���� ��� (������) ** ������ mag phone                   \tOFF        \t< = " + readVals_all[55] + ">  " + temp_disp / 100 + " V\r\n");
                textBox8.Refresh();
                res = myProtocol.writeCoil(slave, 255, false);
            }
            if (coilArr_all[56] != false)
            {
                temp_disp = readVolt_all[56];
                textBox8.Text += ("���� ��� (������) ** ������ ���                         \tOFF        \t< = " + readVals_all[56] + ">  " + temp_disp / 100 + " V\r\n");
                textBox8.Refresh();
                res = myProtocol.writeCoil(slave, 256, false);
            }
            if (coilArr_all[57] != false)
            {
                temp_disp = readVolt_all[57];
                textBox8.Text += ("���� ��� (������) ** ������ �� �����1                   \tOFF        \t< = " + readVals_all[57] + ">  " + temp_disp / 100 + " V\r\n");
                textBox8.Refresh();
                res = myProtocol.writeCoil(slave, 257, false);
            }
            if (coilArr_all[58] != false)
            {
                temp_disp = readVolt_all[58];
                textBox8.Text += ("���� ��� (������) ** ������ �� �����2                   \tOFF        \t< = " + readVals_all[58] + ">  " + temp_disp / 100 + " V\r\n");
                textBox8.Refresh();
                res = myProtocol.writeCoil(slave, 258, false);
            }
            if (coilArr_all[59] != false)
            {
                temp_disp = readVolt_all[59];
                textBox8.Text += ("���� ��� (������) ** ������ ���                         \tON         \t< = " + readVals_all[59] + ">  " + temp_disp / 100 + " V\r\n");
                textBox8.Refresh();
                res = myProtocol.writeCoil(slave, 259, false);
            }

            if (coilArr_all[60] != false)
            {
                temp_disp = readVolt_all[60];
                textBox8.Text += ("���� ��� (������) ** ������ LineL                       \tON         \t< = " + readVals_all[60] + ">  " + temp_disp / 100 + " V\r\n");
                textBox8.Refresh();
                res = myProtocol.writeCoil(slave, 260, false);
            }

            if (coilArr_all[61] != false)
            {
                temp_disp = readVolt_all[61];
                textBox8.Text += ("���� ��� (������) ** ������ LineR                       \tON         \t< = " + readVals_all[61] + ">  " + temp_disp / 100 + " V\r\n");
                textBox8.Refresh();
                res = myProtocol.writeCoil(slave, 261, false);
            }

            if (coilArr_all[62] != false)
            {
                temp_disp = readVolt_all[62];
                textBox8.Text += ("���� ��� (������) ** ������ Mag phone                 \tON         \t< = " + readVals_all[62] + ">  " + temp_disp / 100 + " V\r\n");
                textBox8.Refresh();
                res = myProtocol.writeCoil(slave, 262, false);
             }

            if (coilArr_all[63] != false)
            {
                temp_disp = readVolt_all[63];
                textBox8.Text += ("���� ��� (������) PTT    (CTS)                          \tOFF        \t< = " + readVals_all[63] + ">  " + temp_disp / 100 + " V\r\n");
                textBox8.Refresh();
                res = myProtocol.writeCoil(slave, 263, false);
            }
            if (coilArr_all[64] != false)
            {
                textBox8.Text += ("���� ��������� PTT  (CTS)                     \t OFF       \t< = " + readVals_all[64] + ">\r\n");
                textBox8.Refresh();
                res = myProtocol.writeCoil(slave, 264, false);
            }
            if (coilArr_all[65] != false)
            {
                textBox8.Text += ("���� ��� (������) PTT    (CTS)                          \tON         \t< = " + readVals_all[65] + ">\r\n");
                textBox8.Refresh();
                res = myProtocol.writeCoil(slave, 265, false);
            }
            if (coilArr_all[66] != false)
            {
                textBox8.Text += ("���� ��������� PTT  (CTS)                     \tON         \t< = " + readVals_all[66] + ">\r\n");
                textBox8.Refresh();
                res = myProtocol.writeCoil(slave, 266, false);
            }
            if (coilArr_all[67] != false)
            {
                textBox8.Text += ("���� ��� (������) HangUp (DCD)                          \tOFF        \t< = " + readVals_all[67] + ">\r\n");
                textBox8.Refresh();
                res = myProtocol.writeCoil(slave, 267, false);
            }
            if (coilArr_all[68] != false)
            {
                textBox8.Text += ("���� ��� (������) HangUp (DCD)                          \tON         \t< = " + readVals_all[68] + ">\r\n");
                textBox8.Refresh();
                res = myProtocol.writeCoil(slave, 268, false);
            }
            if (coilArr_all[69] != false)
            {
                temp_disp = readVolt_all[69];
                textBox8.Text += ("������������ �������� ����������� ������� ������� < = \t< = " + readVals_all[69] + ">  " + temp_disp + " ���\r\n");
                textBox8.Refresh();
                res = myProtocol.writeCoil(slave, 269, false);
            }

            if (coilArr_all[70] != false)
            {
                textBox8.Text += ("������� PTT1 �������� ������ (CTS)           \tOFF        \t< = " + readVals_all[70] + ">\r\n");
                textBox8.Refresh();
                res = myProtocol.writeCoil(slave, 270, false);
            }

            if (coilArr_all[71] != false)
            {
                textBox8.Text += ("������� PTT2 �������� ������ (DCR)           \tOFF        \t< = " + readVals_all[71] + ">\r\n");
                textBox8.Refresh();
                res = myProtocol.writeCoil(slave, 271, false);
            }

            if (coilArr_all[72] != false)
            {
                textBox8.Text += ("������� PTT1 �������� ������ (CTS)           \tON         \t< = " + readVals_all[72] + ">\r\n");
                textBox8.Refresh();
                res = myProtocol.writeCoil(slave, 272, false);
            }

            if (coilArr_all[73] != false)
            {
                textBox8.Text += ("������� PTT2 �������� ������ (DCR)           \tON         \t< = " + readVals_all[73] + ">\r\n");
                textBox8.Refresh();
                res = myProtocol.writeCoil(slave, 273, false);
            }
            if (coilArr_all[74] != false)
            {
                textBox8.Text += ("������� ������ �������� ������               \tOFF        \t< = " + readVals_all[74] + ">\r\n");
                textBox8.Refresh();
                res = myProtocol.writeCoil(slave, 274, false);
            }
            if (coilArr_all[75] != false)
            {
                textBox8.Text += ("������� ������ �������� ������               \tON         \t< = " + readVals_all[75] + ">\r\n");
                textBox8.Refresh();
                res = myProtocol.writeCoil(slave, 275, false);
            }
            if (coilArr_all[76] != false)
            {
                textBox8.Text += ("������� ������ �������� ������                \tOFF        \t< = " + readVals_all[76] + ">\r\n");
                textBox8.Refresh();
                res = myProtocol.writeCoil(slave, 276, false);
            }
            if (coilArr_all[77] != false)
            {
                textBox8.Text += ("������� ������ �������� ������                \tON         \t< = " + readVals_all[77] + ">\r\n");
                textBox8.Refresh();
                res = myProtocol.writeCoil(slave, 277, false);
            }
            if (coilArr_all[78] != false)
            {
                textBox8.Text += ("������� PTT �������� ������ (CTS)             \tOFF        \t< = " + readVals_all[78] + ">\r\n");
                textBox8.Refresh();
                res = myProtocol.writeCoil(slave, 278, false);
            }
            if (coilArr_all[79] != false)
            {
                textBox8.Text += ("������� PTT �������� ������ (CTS)             \tON         \t< = " + readVals_all[79] + ">\r\n");
                textBox8.Refresh();
                res = myProtocol.writeCoil(slave, 279, false);
            }

            if (coilArr_all[80] != false)
            {
                temp_disp = readVolt_all[80];
                textBox8.Text += ("���� ��� ** ������ FrontL                      \tOFF        \t< = " + readVals_all[80] + ">  " + temp_disp / 100 + " V\r\n");
                textBox8.Refresh();
                res = myProtocol.writeCoil(slave, 280, false);
            }

            if (coilArr_all[81] != false)
            {
                temp_disp = readVolt_all[81];
                textBox8.Text += ("���� ��� ** ������ FrontR                     \tOFF         \t< = " + readVals_all[81] + ">  " + temp_disp / 100 + " V\r\n");
                textBox8.Refresh();
                res = myProtocol.writeCoil(slave, 281, false);
            }

            if (coilArr_all[82] != false)
            {
                temp_disp = readVolt_all[82];
                textBox8.Text += ("���� ��� ** ������ LineL                       \tOFF        \t< = " + readVals_all[82] + ">  " + temp_disp / 100 + " V\r\n");
                textBox8.Refresh();
                res = myProtocol.writeCoil(slave, 282, false);
            }

            if (coilArr_all[83] != false)
            {
                temp_disp = readVolt_all[83];
                textBox8.Text += ("���� ��� ** ������ LineR                       \tOFF        \t< = " + readVals_all[83] + ">  " + temp_disp / 100 + " V\r\n");
                textBox8.Refresh();
                res = myProtocol.writeCoil(slave, 283, false);
            }
            if (coilArr_all[84] != false)
            {
                temp_disp = readVolt_all[84];
                textBox8.Text += ("���� ��� ** ������ mag radio               \tOFF        \t< = " + readVals_all[84] + ">  " + temp_disp / 100 + " V\r\n");
                textBox8.Refresh();
                res = myProtocol.writeCoil(slave, 284, false);
            }
            if (coilArr_all[85] != false)
            {
                temp_disp = readVolt_all[85];
                textBox8.Text += ("���� ��� ** ������ mag phone                   \tOFF        \t< = " + readVals_all[85] + ">  " + temp_disp / 100 + " V\r\n");
                textBox8.Refresh();
                res = myProtocol.writeCoil(slave, 285, false);
            }
            if (coilArr_all[86] != false)
            {
                temp_disp = readVolt_all[86];
                textBox8.Text += ("���� ��� ** ������ ���                         \tOFF        \t< = " + readVals_all[86] + ">  " + temp_disp / 100 + " V\r\n");
                textBox8.Refresh();
                res = myProtocol.writeCoil(slave, 286, false);
            }
            if (coilArr_all[87] != false)
            {
                temp_disp = readVolt_all[87];
                textBox8.Text += ("���� ��� ** ������ �� �����1                   \tOFF        \t< = " + readVals_all[87] + ">  " + temp_disp / 100 + " V\r\n");
                textBox8.Refresh();
                res = myProtocol.writeCoil(slave, 287, false);
            }
            if (coilArr_all[88] != false)
            {
                temp_disp = readVolt_all[88];
                textBox8.Text += ("���� ��� ** ������ �� �����2                   \tOFF        \t< = " + readVals_all[88] + ">  " + temp_disp / 100 + " V\r\n");
                textBox8.Refresh();
                res = myProtocol.writeCoil(slave, 288, false);
            }
            if (coilArr_all[89] != false)
            {
                temp_disp = readVolt_all[89];
                textBox8.Text += ("���� ��� ** ������ ���                         \tON         \t< = " + readVals_all[89] + ">  " + temp_disp / 100 + " V\r\n");
                textBox8.Refresh();
                res = myProtocol.writeCoil(slave, 289, false);
            }

            if (coilArr_all[90] != false)
            {
                temp_disp = readVolt_all[90];
                textBox8.Text += ("���� ��� ** ������ FrontL                      \tON         \t< = " + readVals_all[90] + ">  " + temp_disp / 100 + " V\r\n");
                textBox8.Refresh();
                res = myProtocol.writeCoil(slave, 290, false);
            }

            if (coilArr_all[91] != false)
            {
                temp_disp = readVolt_all[91];
                textBox8.Text += ("���� ��� ** ������ FrontR                     \tON          \t< = " + readVals_all[91] + ">  " + temp_disp / 100 + " V\r\n");
                textBox8.Refresh();
                res = myProtocol.writeCoil(slave, 291, false);
            }

            if (coilArr_all[92] != false)
            {
                temp_disp = readVolt_all[92];
                textBox8.Text += ("���� ��� ** ������ mag phone            \tON         \t < = " + readVals_all[92] + ">  " + temp_disp / 100 + " V\r\n");
                textBox8.Refresh();
                res = myProtocol.writeCoil(slave, 292, false);
            }

            if (coilArr_all[93] != false)
            {
                temp_disp = readVolt_all[93];
                textBox8.Text += ("���������� ������� ������ �� � �����                 \t< = " + readVals_all[93] + ">  " + temp_disp / 100 + " V\r\n");
                textBox8.Refresh();
                res = myProtocol.writeCoil(slave, 293, false);
            }
            if (coilArr_all[94] != false)
            {
                temp_disp = readVolt_all[94];
                textBox8.Text += ("���������� ������� �����1 �� � �����                 \t < = " + readVals_all[94] + ">  " + temp_disp / 100 + " V\r\n");
                textBox8.Refresh();
                res = myProtocol.writeCoil(slave, 294, false);
            }
            if (coilArr_all[95] != false)
            {
                temp_disp = readVolt_all[95];
                textBox8.Text += ("���������� ������� �����2 �� � �����                 \t  < = " + readVals_all[95] + ">  " + temp_disp / 100 + " V\r\n");
                textBox8.Refresh();
                res = myProtocol.writeCoil(slave, 295, false);
            }
            if (coilArr_all[96] != false)
            {
                temp_disp = readVolt_all[96];
                textBox8.Text += ("���������� ������� ���  �� � �����                   \t  < = " + readVals_all[96] + ">  " + temp_disp / 100 + " V\r\n");
                textBox8.Refresh();
                res = myProtocol.writeCoil(slave, 296, false);
            }
            if (coilArr_all[97] != false)
            {
                temp_disp = readVolt_all[97];
                textBox8.Text += ("���������� ������� ���������� ��������� �� � �����   \t  < = " + readVals_all[97] + ">  " + temp_disp / 100 + " V\r\n");
                textBox8.Refresh();
                res = myProtocol.writeCoil(slave, 297, false);
            }
            if (coilArr_all[98] != false)
            {
                temp_disp = readVolt_all[98];
                textBox8.Text += ("���� ��������� ** ������ mag phone            \tON         \t< = " + readVals_all[98] + ">  " + temp_disp / 100 + " V\r\n");
                textBox8.Refresh();
                res = myProtocol.writeCoil(slave, 298, false);
            }
            if (coilArr_all[99] != false)
            {
                temp_disp = readVolt_all[99];
                textBox8.Text += ("���� ��������� ** ������ LineL                \tON         \t< = " + readVals_all[99] + ">  " + temp_disp / 100 + " V\r\n");
                textBox8.Refresh();
                res = myProtocol.writeCoil(slave, 299, false);
            }

            if (coilArr_all[100] != false)
            {
                temp_disp = readVolt_all[100];
                textBox8.Text += ("���� �����1 ** ������ FrontL                  \tOFF        \t< = " + readVals_all[100] + ">  " + temp_disp / 100 + " V\r\n");
                textBox8.Refresh();
                res = myProtocol.writeCoil(slave, 300, false);
            }

            if (coilArr_all[101] != false)
            {
                temp_disp = readVolt_all[101];
                textBox8.Text += ("���� �����1 ** ������ FrontR                   \tOFF        \t< = " + readVals_all[101] + ">  " + temp_disp / 100 + " V\r\n");
                textBox8.Refresh();
                res = myProtocol.writeCoil(slave, 301, false);
            }

            if (coilArr_all[102] != false)
            {
                temp_disp = readVolt_all[102];
                textBox8.Text += ("���� �����1 ** ������ LineL                    \tOFF        \t< = " + readVals_all[102] + ">  " + temp_disp / 100 + " V\r\n");
                textBox8.Refresh();
                res = myProtocol.writeCoil(slave, 302, false);
            }

            if (coilArr_all[103] != false)
            {
                temp_disp = readVolt_all[103];
                textBox8.Text += ("���� �����1 ** ������ LineR                    \tOFF        \t< = " + readVals_all[103] + ">  " + temp_disp / 100 + " V\r\n");
                textBox8.Refresh();
                res = myProtocol.writeCoil(slave, 303, false);
            }
            if (coilArr_all[104] != false)
            {
                temp_disp = readVolt_all[104];
                textBox8.Text += ("���� �����1 ** ������ mag radio                \tOFF        \t< = " + readVals_all[104] + ">  " + temp_disp / 100 + " V\r\n");
                textBox8.Refresh();
                res = myProtocol.writeCoil(slave, 304, false);
            }
            if (coilArr_all[105] != false)
            {
                temp_disp = readVolt_all[105];
                textBox8.Text += ("���� �����1 ** ������ mag phone                \tOFF        \t< = " + readVals_all[105] + ">  " + temp_disp / 100 + " V\r\n");
                textBox8.Refresh();
                res = myProtocol.writeCoil(slave, 305, false);
            }
            if (coilArr_all[106] != false)
            {
                temp_disp = readVolt_all[6];
                textBox8.Text += ("���� �����1 ** ������ ���                      \tOFF        \t< = " + readVals_all[106] + ">  " + temp_disp / 100 + " V\r\n");
                textBox8.Refresh();
                res = myProtocol.writeCoil(slave, 306, false);
            }
            if (coilArr_all[107] != false)
            {
                temp_disp = readVolt_all[107];
                textBox8.Text += ("���� �����1 ** ������ �� �����1                \tOFF        \t< = " + readVals_all[107] + ">  " + temp_disp / 100 + " V\r\n");
                textBox8.Refresh();
                res = myProtocol.writeCoil(slave, 307, false);
            }
            if (coilArr_all[108] != false)
            {
                temp_disp = readVolt_all[108];
                textBox8.Text += ("���� �����1 ** ������ �� �����2                \tOFF        \t< = " + readVals_all[108] + ">  " + temp_disp / 100 + " V\r\n");
                textBox8.Refresh();
                res = myProtocol.writeCoil(slave, 308, false);
            }
            if (coilArr_all[109] != false)
            {
                temp_disp = readVolt_all[109];
                textBox8.Text += ("���� �����1 ** ������ Radio1                   \tON         \t< = " + readVals_all[109] + ">  " + temp_disp / 100 + " V\r\n");
                textBox8.Refresh();
                res = myProtocol.writeCoil(slave, 309, false);
            }
             
            if (coilArr_all[110] != false)
            {
                temp_disp = readVolt_all[110];
                textBox8.Text += ("���� �����2 ** ������ FrontL                   \tOFF        \t< = " + readVals_all[110] + ">  " + temp_disp / 100 + " V\r\n");
                textBox8.Refresh();
                res = myProtocol.writeCoil(slave, 310, false);
            }

            if (coilArr_all[111] != false)
            {
                temp_disp = readVolt_all[111];
                textBox8.Text += ("���� �����2 ** ������ FrontR                   \tOFF        \t< = " + readVals_all[111] + ">  " + temp_disp / 100 + " V\r\n");
                textBox8.Refresh();
                res = myProtocol.writeCoil(slave, 311, false);
            }

            if (coilArr_all[112] != false)
            {
                temp_disp = readVolt_all[112];
                textBox8.Text += ("���� �����2 ** ������ LineL                    \tOFF        \t< = " + readVals_all[112] + ">  " + temp_disp / 100 + " V\r\n");
                textBox8.Refresh();
                res = myProtocol.writeCoil(slave, 312, false);
            }

            if (coilArr_all[113] != false)
            {
                temp_disp = readVolt_all[113];
                textBox8.Text += ("���� �����2 ** ������ LineR                    \tOFF        \t< = " + readVals_all[113] + ">  " + temp_disp / 100 + " V\r\n");
                textBox8.Refresh();
                res = myProtocol.writeCoil(slave, 313, false);
            }
            if (coilArr_all[114] != false)
            {
                temp_disp = readVolt_all[114];
                textBox8.Text += ("���� �����2 ** ������ mag radio                \tOFF        \t< = " + readVals_all[114] + ">  " + temp_disp / 100 + " V\r\n");
                textBox8.Refresh();
                res = myProtocol.writeCoil(slave, 314, false);
            }
            if (coilArr_all[115] != false)
            {
                temp_disp = readVolt_all[115];
                textBox8.Text += ("���� �����2 ** ������ mag phone                \tOFF        \t< = " + readVals_all[115] + ">  " + temp_disp / 100 + " V\r\n");
                textBox8.Refresh();
                res = myProtocol.writeCoil(slave, 315, false);
            }
            if (coilArr_all[116] != false)
            {
                temp_disp = readVolt_all[116];
                textBox8.Text += ("���� �����2 ** ������ ���                      \tOFF        \t< = " + readVals_all[116] + ">  " + temp_disp / 100 + " V\r\n");
                textBox8.Refresh();
                res = myProtocol.writeCoil(slave, 316, false);
            }
            if (coilArr_all[117] != false)
            {
                temp_disp = readVolt_all[117];
                textBox8.Text += ("���� �����2 ** ������ �� �����1                \tOFF        \t< = " + readVals_all[117] + ">  " + temp_disp / 100 + " V\r\n");
                textBox8.Refresh();
                res = myProtocol.writeCoil(slave, 317, false);
            }
            if (coilArr_all[118] != false)
            {
                temp_disp = readVolt_all[118];
                textBox8.Text += ("���� �����2 ** ������ �� �����2                \tOFF        \t< = " + readVals_all[118] + ">  " + temp_disp / 100 + " V\r\n");
                textBox8.Refresh();
                res = myProtocol.writeCoil(slave, 318, false);
            }
            if (coilArr_all[119] != false)
            {
                temp_disp = readVolt_all[119];
                textBox8.Text += ("���� �����2 ** ������ �����2                   \tON         \t< = " + readVals_all[119] + ">  " + temp_disp / 100 + " V\r\n");
                textBox8.Refresh();
                res = myProtocol.writeCoil(slave, 319, false);
            }

            if (coilArr_all[120] != false)
            {
                temp_disp = readVolt_all[120];
                textBox8.Text += ("���� ��������� ** ������ FrontL               \tOFF        \t< = " + readVals_all[120] + ">  " + temp_disp / 100 + " V\r\n");
                textBox8.Refresh();
                res = myProtocol.writeCoil(slave, 320, false);
            }

            if (coilArr_all[121] != false)
            {
                temp_disp = readVolt_all[121];
                textBox8.Text += ("���� ��������� ** ������ FrontR               \tOFF        \t< = " + readVals_all[121] + ">  " + temp_disp / 100 + " V\r\n");
                textBox8.Refresh();
                res = myProtocol.writeCoil(slave, 321, false);
            }

            if (coilArr_all[122] != false)
            {
                temp_disp = readVolt_all[122];
                textBox8.Text += ("���� ��������� ** ������ LineL                \tOFF        \t< = " + readVals_all[122] + ">  " + temp_disp / 100 + " V\r\n");
                textBox8.Refresh();
                res = myProtocol.writeCoil(slave, 322, false);
            }

            if (coilArr_all[123] != false)
            {
                temp_disp = readVolt_all[123];
                textBox8.Text += ("���� ��������� ** ������ LineR                \tOFF        \t< = " + readVals_all[123] + ">  " + temp_disp / 100 + " V\r\n");
                textBox8.Refresh();
                res = myProtocol.writeCoil(slave, 323, false);
            }
            if (coilArr_all[124] != false)
            {
                temp_disp = readVolt_all[124];
                textBox8.Text += ("���� ��������� ** ������ mag radio            \tOFF        \t< = " + readVals_all[124] + ">  " + temp_disp / 100 + " V\r\n");
                textBox8.Refresh();
                res = myProtocol.writeCoil(slave, 324, false);
            }
            if (coilArr_all[125] != false)
            {
                temp_disp = readVolt_all[125];
                textBox8.Text += ("���� ��������� ** ������ mag phone            \tOFF        \t< = " + readVals_all[125] + ">  " + temp_disp / 100 + " V\r\n");
                textBox8.Refresh();
                res = myProtocol.writeCoil(slave, 325, false);
            }
            if (coilArr_all[126] != false)
            {
                temp_disp = readVolt_all[126];
                textBox8.Text += ("���� ��������� ** ������ ���                  \tOFF        \t< = " + readVals_all[126] + ">  " + temp_disp / 100 + " V\r\n");
                textBox8.Refresh();
                res = myProtocol.writeCoil(slave, 326, false);
            }
            if (coilArr_all[127] != false)
            {
                temp_disp = readVolt_all[127];
                textBox8.Text += ("���� ��������� ** ������ �� �����1            \tOFF        \t< = " + readVals_all[127] + ">  " + temp_disp / 100 + " V\r\n");
                textBox8.Refresh();
                res = myProtocol.writeCoil(slave, 327, false);
            }
            if (coilArr_all[128] != false)
            {
                temp_disp = readVolt_all[128];
                textBox8.Text += ("���� ��������� ** ������ �� �����2            \tOFF        \t< = " + readVals_all[128] + ">  " + temp_disp / 100 + " V\r\n");
                textBox8.Refresh();
                res = myProtocol.writeCoil(slave, 328, false);
            }
            if (coilArr_all[129] != false)
            {
                temp_disp = readVolt_all[129];
                textBox8.Text += ("��� ����������� ������� ������� �� ���������   \t< = " + readVals_all[128] + ">  " + temp_disp + " \r\n");
                textBox8.Refresh();
                res = myProtocol.writeCoil(slave, 329, false);
            }
            res = myProtocol.writeCoil(slave, 120, false);                      // ����� ���� ����� ������ �����
        }
        private void error_list1()
        {
            ushort[] readVals = new ushort[10];
            ushort[] readVolt = new ushort[10];
            bool[] coilArr = new bool[10];
            startRdReg = 200;
            numRdRegs = 5;

            for (int i_reg = 0; i_reg < 13; i_reg++)
            {
                res = myProtocol.readMultipleRegisters(slave, startRdReg, readVals, numRdRegs);     // 40200 ������� �������� ������  
                if ((res == BusProtocolErrors.FTALK_SUCCESS))
                {
                    toolStripStatusLabel1.Text = "    MODBUS ON    ";
                    toolStripStatusLabel1.BackColor = Color.Lime;

                    for (int i_temp = 0; i_temp < 5; i_temp++)
                    {
                        readVals_all[startRdReg + i_temp - 200] = readVals[i_temp];
                    }
                    startRdReg += 5;
                }

                res = myProtocol.readMultipleRegisters(slave, startRdReg, readVals, numRdRegs);     // 40200 ������� �������� ������  
                if ((res == BusProtocolErrors.FTALK_SUCCESS))
                {
                    toolStripStatusLabel1.Text = "    MODBUS ON    ";
                    toolStripStatusLabel1.BackColor = Color.Lime;

                    for (int i_temp = 0; i_temp < 5; i_temp++)
                    {
                        readVals_all[startRdReg + i_temp - 200] = readVals[i_temp];
                    }
                    startRdReg += 5;
                }
             }

        }
        private void error_list2()
        {
            ushort[] readVolt = new ushort[10];
            startRdReg = 200;
            numRdRegs = 5;
  
            for (int i_reg = 0; i_reg < 13; i_reg++)
            {
                res = myProtocol.readMultipleRegisters(slave, startRdReg + 200, readVolt, numRdRegs);
                if ((res == BusProtocolErrors.FTALK_SUCCESS))
                {
                    toolStripStatusLabel1.Text = "    MODBUS ON    ";
                    toolStripStatusLabel1.BackColor = Color.Lime;

                    for (int i_temp = 0; i_temp < 5; i_temp++)
                    {
                        readVolt_all[startRdReg + i_temp - 200] = readVolt[i_temp];
                    }
                    startRdReg += 5;
                 }

                res = myProtocol.readMultipleRegisters(slave, startRdReg + 200, readVolt, numRdRegs);
                if ((res == BusProtocolErrors.FTALK_SUCCESS))
                {
                    toolStripStatusLabel1.Text = "    MODBUS ON    ";
                    toolStripStatusLabel1.BackColor = Color.Lime;

                    for (int i_temp = 0; i_temp < 5; i_temp++)
                    {
                        readVolt_all[startRdReg + i_temp - 200] = readVolt[i_temp];
                    }
                    startRdReg += 5;
                 }
            }
        }
        private void error_list3()
        {
            //   Polltimer1.Enabled = true;
            ushort[] readVals = new ushort[10];
            ushort[] readVolt = new ushort[10];
            bool[] coilArr = new bool[10];
            startCoil = 200;                                                                    // ��������� ����� 200 ����� ��������� �������������  ������
            numCoils = 5;


            for (int i_reg = 0; i_reg < 13; i_reg++)
            {
                 res = myProtocol.readCoils(slave, startCoil, coilArr, numCoils);
                if ((res == BusProtocolErrors.FTALK_SUCCESS))
                {
                    toolStripStatusLabel1.Text = "    MODBUS ON    ";
                    toolStripStatusLabel1.BackColor = Color.Lime;
                

                    for (int i_temp = 0; i_temp < 5; i_temp++)
                    {
                        coilArr_all[startCoil + i_temp - 200] = coilArr[i_temp];
                    }
                    startCoil += 5;
               }
                res = myProtocol.readCoils(slave, startCoil, coilArr, numCoils);
                if ((res == BusProtocolErrors.FTALK_SUCCESS))
                {
                    toolStripStatusLabel1.Text = "    MODBUS ON    ";
                    toolStripStatusLabel1.BackColor = Color.Lime;

                    for (int i_temp = 0; i_temp < 5; i_temp++)
                    {
                        coilArr_all[startCoil + i_temp - 200] = coilArr[i_temp];
                    }
                    startCoil += 5;
                }

            }

        }
        private void error_list_print()
        {
            richTextBox1.Text = "";
            for (int i_reg = 0; i_reg < 130; i_reg++)
                {
                    richTextBox1.Text += ((i_reg + 200) + "   -  " + readVals_all[i_reg] + "  -  " + coilArr_all[i_reg] +  "  -  " + readVolt_all[i_reg] + "\r\n");
                }
            richTextBox1.Refresh();
            Polltimer1.Enabled = true;
        }

        //*******************************************
        private void TestAllNew()
        {
            short[] readVals = new short[125];
            int slave;
            int startRdReg;
            int numRdRegs;
            int res;
            testAllRun = true;

            slave = int.Parse(txtSlave.Text, CultureInfo.CurrentCulture);
            startRdReg = 46; // 40046 ����� ����/����� �����������  
            numRdRegs = 8;
            res = myProtocol.readMultipleRegisters(slave, startRdReg, readVals, numRdRegs);


            while (testAllRun)
            {
    

                if (checkBoxPower.Checked || radioButton1.Checked)
                {
                    test_power();
                    progressBar2.Value += 1;
                    label98.Text = ("" + progressBar2.Value);
                    label98.Refresh();
                   // if(button9.Click()){}

                }
                if (checkBoxSensors1.Checked || radioButton1.Checked)
                {
                    sensor_off();
                    progressBar2.Value += 1;
                    label98.Text = ("" + progressBar2.Value);
                    label98.Refresh();
                }
                if (checkBoxSensors2.Checked || radioButton1.Checked)
                {
                    sensor_on();
                    progressBar2.Value += 1;
                    label98.Text = ("" + progressBar2.Value);
                    label98.Refresh();
                }
                if (checkBoxSenGar1instr.Checked || radioButton1.Checked)
                {
                    test_instruktora();
                    progressBar2.Value += 1;
                    label98.Text = ("" + progressBar2.Value);
                    label98.Refresh();
                }
                if (checkBoxSenGar1disp.Checked || radioButton1.Checked)
                {
                    test_dispetchera();
                    progressBar2.Value += 1;
                    label98.Text = ("" + progressBar2.Value);
                    label98.Refresh();
                }
                if (checkBoxSenTrubka.Checked || radioButton1.Checked)
                {
                    test_MTT();
                    progressBar2.Value += 1;
                    label98.Text = ("" + progressBar2.Value);
                    label98.Refresh();
                }
                if (checkBoxSenTangRuch.Checked || radioButton1.Checked)
                {
                    test_tangR();
                    progressBar2.Value += 1;
                    label98.Text = ("" + progressBar2.Value);
                    label98.Refresh();
                }
                if (checkBoxSenTangN.Checked || radioButton1.Checked)
                {
                    test_tangN();
                    progressBar2.Value += 1;
                    label98.Text = ("" + progressBar2.Value);
                    label98.Refresh();
                }
                if (checkBoxSenGGS.Checked || radioButton1.Checked)
                {
                    testGGS();
                    progressBar2.Value += 1;
                    label98.Text = ("" + progressBar2.Value);
                    label98.Refresh();
                }
                if (checkBoxSenGGRadio1.Checked || radioButton1.Checked)
                {
                    test_GG_Radio1();
                    progressBar2.Value += 1;
                    label98.Text = ("" + progressBar2.Value);
                    label98.Refresh();
                }
                if (checkBoxSenGGRadio2.Checked || radioButton1.Checked)
                {
                    test_GG_Radio2();
                    progressBar2.Value += 1;
                    label98.Text = ("" + progressBar2.Value);
                    label98.Refresh();
                }
                if (checkBoxSenMicrophon.Checked || radioButton1.Checked)
                {
                    test_mikrophon();
                    progressBar2.Value += 1;
                    label98.Text = ("" + progressBar2.Value);
                    label98.Refresh();
                }
                if (checkBoxDisp.Checked || radioButton1.Checked)
                {
                    test_valueDisp();
                    progressBar2.Value += 1;
                    label98.Text = ("" + progressBar2.Value);
                    label98.Refresh();
                }

                if (radioButton2.Checked)
                //     if (radioButton2.Checked & TestN == 13)
                {
                    timerTestAll.Enabled = true;
                    TestN = 0;
                    TestRepeatCount++;
                    if (TestRepeatCount > 32767) TestRepeatCount = 1;
                    textBox7.Text += ("\r\n");
                    textBox7.Text += ("������ ����� " + TestRepeatCount + "\r\n");
                    textBox7.Text += ("\r\n");
                    Thread.Sleep(500);
                }

        }



        }





        private void timerTestAll_Tick(object sender, EventArgs e)        // ������������ ��������� ������ �����
        {
            short[] readVals = new short[125];
            int slave;
            int startRdReg;
            int numRdRegs;
            int res;
            int TestNst;

            slave = int.Parse(txtSlave.Text, CultureInfo.CurrentCulture);
            startRdReg = 46; // 40046 ����� ����/����� �����������  
            numRdRegs = 8;
            res = myProtocol.readMultipleRegisters(slave, startRdReg, readVals, numRdRegs);

            TestNst = test_step[TestN];

            switch (TestNst)  // ���������� � �����
            {
                default:
                case 0:
                    if (checkBoxPower.Checked || radioButton1.Checked)
                    {
                        test_power();
                        progressBar2.Value += 1;
                        label98.Text = ("" + progressBar2.Value);
                        label98.Refresh();
                    }
                    break;

                case 1:
                    if (checkBoxSensors1.Checked || radioButton1.Checked)
                    {
                        sensor_off();
                        progressBar2.Value += 1;
                        label98.Text = ("" + progressBar2.Value);
                        label98.Refresh();
                    }
                    break;

                case 2:
                    if (checkBoxSensors2.Checked || radioButton1.Checked)
                    {
                        sensor_on();
                        progressBar2.Value += 1;
                        label98.Text = ("" + progressBar2.Value);
                        label98.Refresh();
                    }
                    break;
                case 3:
                    if (checkBoxSenGar1instr.Checked || radioButton1.Checked)
                    {
                        test_instruktora();
                        progressBar2.Value += 1;
                        label98.Text = ("" + progressBar2.Value);
                        label98.Refresh();
                    }
                    break;
                case 4:
                    if (checkBoxSenGar1disp.Checked || radioButton1.Checked)
                    {
                        test_dispetchera();
                        progressBar2.Value += 1;
                        label98.Text = ("" + progressBar2.Value);
                        label98.Refresh();
                    }
                    break;
                case 5:
                    if (checkBoxSenTrubka.Checked || radioButton1.Checked)
                    {
                        test_MTT();
                        progressBar2.Value += 1;
                        label98.Text = ("" + progressBar2.Value);
                        label98.Refresh();
                    }
                    break;
                case 6:
                    if (checkBoxSenTangRuch.Checked || radioButton1.Checked)
                    {
                        test_tangR();
                        progressBar2.Value += 1;
                        label98.Text = ("" + progressBar2.Value);
                        label98.Refresh();
                    }
                    break;
                case 7:
                    if (checkBoxSenTangN.Checked || radioButton1.Checked)
                    {
                        test_tangN();
                        progressBar2.Value += 1;
                        label98.Text = ("" + progressBar2.Value);
                        label98.Refresh();
                    }
                    break;
                case 8:
                    if (checkBoxSenGGS.Checked || radioButton1.Checked)
                    {
                        testGGS();
                        progressBar2.Value += 1;
                        label98.Text = ("" + progressBar2.Value);
                        label98.Refresh();
                    }
                    break;
                case 9:
                    if (checkBoxSenGGRadio1.Checked || radioButton1.Checked)
                    {
                        test_GG_Radio1();
                        progressBar2.Value += 1;
                        label98.Text = ("" + progressBar2.Value);
                        label98.Refresh();
                    }
                    break;
                case 10:
                    if (checkBoxSenGGRadio2.Checked || radioButton1.Checked)
                    {
                        test_GG_Radio2();
                        progressBar2.Value += 1;
                        label98.Text = ("" + progressBar2.Value);
                        label98.Refresh();
                    }
                    break;
                case 11:
                    if (checkBoxSenMicrophon.Checked || radioButton1.Checked)
                    {
                        test_mikrophon();
                        progressBar2.Value += 1;
                        label98.Text = ("" + progressBar2.Value);
                        label98.Refresh();
                    }
                    break;
                case 12:
                    if (checkBoxDisp.Checked || radioButton1.Checked)
                    {
                        test_valueDisp();
                        progressBar2.Value += 1;
                        label98.Text = ("" + progressBar2.Value);
                        label98.Refresh();
                    }
                    break;

            }
            
            TestN++;                                                     // ��������� ������� ������ �����

            label98.Text = ("" + progressBar2.Value);
            if (progressBar2.Value == progressBar2.Maximum)
            {
                progressBar2.Value = 0;
            }
            label80.Text = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss", CultureInfo.CurrentCulture);
            toolStripStatusLabel2.Text = ("����� : " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.CurrentCulture));

            if (radioButton1.Checked & TestN == TestStep)
            {
                timerTestAll.Enabled = false;
                //textBox7.Text += ("���� ��������" + "\r\n");
                //textBox7.Text += ("\r\n");
                _All_Test_Stop = false;
                //-----
         //       timerTestAll.Enabled = false;

                ushort[] writeVals = new ushort[20];
                bool[] coilArr = new bool[34];

                slave = int.Parse(txtSlave.Text, CultureInfo.CurrentCulture);
                button9.BackColor = Color.Red;
                button11.BackColor = Color.White;
                label92.Text = ("");
       //         textBox7.Text += ("���� ����������" + "\r\n");
                progressBar2.Value = 0;
                startWrReg = 120;
                res = myProtocol.writeSingleRegister(slave, startWrReg, 13);                // ������� �� �������� ����� ����������
                textBox9.Text += ("������� �� �������� ����� ����������" + "\r\n");
                textBox9.Refresh();
                testAllRun = false;
                textBox7.Text += "���� �������!";
                textBox7.Refresh();
                _All_Test_Stop = true;
                Polltimer1.Enabled = true;
                startCoil = 8;                                                             // ���������� �������� ����� "��������"
                res = myProtocol.writeCoil(slave, startCoil, false);                       // ��������� ������� ����� "��������"

                //-----
                Polltimer1.Enabled = true;
            }
            if (radioButton2.Checked & TestN == TestStep)   // ������ ������������� �����
            {
                timerTestAll.Enabled = true;
                TestN = 0;
                TestRepeatCount++;
                if (TestRepeatCount > 32767) TestRepeatCount = 1;
                textBox7.Text += ("\r\n");
                textBox7.Text += ("������ ����� " + TestRepeatCount + "\r\n");
                textBox7.Text += ("\r\n");
                Thread.Sleep(100);
            }
        }

        private void button11_Click_1(object sender, EventArgs e)         //����� ������� �����
        {
            Polltimer1.Enabled = false;
            timer_Mic_test.Enabled = false;
            timer_byte_set.Enabled = false;
            ushort[] writeVals = new ushort[200];
            bool[] coilArr = new bool[200];
            progressBar2.Value = 0;
            slave = int.Parse(txtSlave.Text, CultureInfo.CurrentCulture);
            startCoil = 8;                                               // ���������� �������� ����� "��������"
            res = myProtocol.writeCoil(slave, startCoil, true);          // �������� ������� ����� "��������"
            Thread.Sleep(1700);
            button11.BackColor = Color.Lime;
            button11.Refresh();
    //        label92.Text = ("�� �������� ������ ������ [����] ����� ���������� �����!");
            button9.BackColor = Color.LightSalmon;
            button9.Refresh();
            textBox7.Text = ("����������� ������  �������� ��������� ������ �������� " + "\r\n");
            textBox7.Text += ("\r\n");
            textBox8.Text = ("");
            textBox9.Text = ("");
            textBox8.Refresh();
            textBox9.Refresh();
            //  0 � �������� �������� ���������� ���������� ��������� ��������
            numRdRegs = 2;
            startCoil = 124;                                                                       // regBank.add(124);  ���� ��������� ����� � ������� "�����"
            numCoils = 2;
            res = myProtocol.readCoils(slave, startCoil, coilArr, numCoils);                       // ��������� ����� 124 ���� ��������� ����� � ������� "�����"
           // coilArr[0] = false;                                                                    // !!! ������, ������ ��� ������������

            if (coilArr[0] == true)                                                                //���� ������
                {
                    // ��������� ������.
                    textBox7.Text += ("����� �� �������� ������ ����� �� ����������� !" + "\r\n" + "\r\n");
                    timerTestAll.Enabled = false;
                    button9.BackColor = Color.Red;
                    button11.BackColor = Color.White;
                    label92.Text = ("");
                    textBox7.Text += ("���� ����������" + "\r\n");
                    progressBar2.Value = 0;
                 }
            else
                {
                   textBox7.Text += ("����� �� �������� ������ ����� �����������." + "\r\n");
                   textBox7.Refresh();

                   if (radioButton2.Checked)                                                               // ������� ������������ ������� 
                    {
                        startCoil = 118;                                                                   // ������� ������������ ������� ����������. �������� � ����������
                        res = myProtocol.writeCoil(slave, startCoil, true);
                        //for (int i = 0; i < 25; i++)                // �������� ������ ������
                        //{
                        //    test_step[i] = 255;
                        //}

                        //---------------------------
                        TestStep = 0;
                       
                        //while (TestSum <= 13)
                        //   {

                               if (checkBoxPower.Checked)
                               {
                                    test_step[TestStep] = 0;
                                    TestStep++;
                               }

                               if (checkBoxSensors1.Checked)
                               {
                                   test_step[TestStep] = 1;
                                   TestStep++;
                               }

                               if (checkBoxSensors2.Checked)
                               {
                                    test_step[TestStep] = 2;
                                    TestStep++;
                               }

                               if (checkBoxSenGar1instr.Checked)
                               {
                                   test_step[TestStep] = 3;
                                   TestStep++;
                               }

                               if (checkBoxSenGar1disp.Checked)
                               {
                                    test_step[TestStep] = 4;
                                    TestStep++;
                               }

                               if (checkBoxSenTrubka.Checked)
                               {
                                    test_step[TestStep] = 5;
                                    TestStep++;
                               }

                               if (checkBoxSenTangRuch.Checked)
                               {
                                    test_step[TestStep] = 6;
                                    TestStep++;
                               }

                               if (checkBoxSenTangN.Checked)
                               {
                                    test_step[TestStep] = 7;
                                    TestStep++;
                               }

                               if (checkBoxSenGGS.Checked)
                               {
                                   test_step[TestStep] = 8;
                                   TestStep++;
                               }

                               if (checkBoxSenGGRadio1.Checked)
                               {
                                   test_step[TestStep] = 9;
                                   TestStep++;
                               }

                               if (checkBoxSenGGRadio2.Checked)
                               {
                                    test_step[TestStep] = 10;
                                    TestStep++;
                               }

                               if (checkBoxSenMicrophon.Checked)
                               {
                                    test_step[TestStep] = 11;
                                    TestStep++;
                               }

                               if (checkBoxDisp.Checked)
                               {
                                    test_step[TestStep] = 12;
                                    TestStep++;
                               }
  
                     }
                  else
                    {
                        startCoil = 118;                                                                    // ������� ������������ ������� ����.      �������� � ����������
                        res = myProtocol.writeCoil(slave, startCoil, false);
                        for (int i = 0; i < 13; i++)                // �������� ������ ������
                        {
                            test_step[i] = i;
                        }
                        TestStep = 13;
                    }
 

                    TestN = 0;                                                                              // �������� ������� ������ ����������� ������
                    TestRepeatCount = 1;                                                                    // ���������� ��������� �����  �������� �������� �����

                    if (_All_Test_Stop)                                                                     // ��������� ������� ���������� ���������� ������
                    {
                        startWrReg = 120;                                                                   // ������� �� 
                        res = myProtocol.writeSingleRegister(slave, startWrReg, 16);                        // ������� �� ����� ��������� ����������
                        test_end1();
                        startWrReg = 120;                                                                   // ������� �� �������� ����� ����������
                        res = myProtocol.writeSingleRegister(slave, startWrReg, 12);                        // ������� �� �������� ����� ����������
                        textBox9.Text += ("������� �� �������� ����� ����������" + "\r\n");
                        textBox7.Refresh();
                        test_end1();
                        file_fakt_namber();
                        //Thread.Sleep(400);
                        test_end1();
                        _All_Test_Stop = false;                                                             // ���������� ���� ������� �����
                    }
     
                timerTestAll.Enabled = true;
            
        }

            if ((res == BusProtocolErrors.FTALK_SUCCESS))
            {
                toolStripStatusLabel1.Text = "    MODBUS ON    ";
                toolStripStatusLabel1.BackColor = Color.Lime;
            }
            else
            {
                toolStripStatusLabel1.Text = "    MODBUS ERROR (7) ";
                toolStripStatusLabel1.BackColor = Color.Red;
                portFound = false;
                find_com_port.Enabled = true;
            }

            progressBar2.Value = 0;

            label80.Text = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss", CultureInfo.CurrentCulture);
            toolStripStatusLabel2.Text = ("����� : " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.CurrentCulture));
        }

        private void button9_Click(object sender, EventArgs e)            // ���� ������� �����
        {
            timerTestAll.Enabled = false;

            ushort[] writeVals = new ushort[20];
            bool[] coilArr = new bool[34];

            slave = int.Parse(txtSlave.Text, CultureInfo.CurrentCulture);
            button9.BackColor = Color.Red;
            button11.BackColor = Color.White;
            label92.Text = ("");
            textBox7.Text += ("���� ����������" + "\r\n");
            progressBar2.Value = 0;
            startWrReg = 120;
            res = myProtocol.writeSingleRegister(slave, startWrReg, 13); // ������� �� �������� ����� ����������
            textBox9.Text += ("������� �� �������� ����� ����������" + "\r\n");
            // textBox9.Text = ("���� �����");
            textBox7.Refresh();
            textBox9.Refresh();
            testAllRun = false;
            //    test_end();

            textBox7.Text += "���� �������!";

            _All_Test_Stop = true;
            Polltimer1.Enabled = true;
            startCoil = 8;                                               // ���������� �������� ����� "��������"
            res = myProtocol.writeCoil(slave, startCoil, false);         // ��������� ������� ����� "��������"
        }

        private void label92_Click(object sender, EventArgs e)
        {

        }

        private void button12_Click(object sender, EventArgs e)          // ������� ����
        {
            if (!File.Exists(fileName))
            {
                File.Create(fileName).Close();
            }
            else
            {
                MessageBox.Show("���� ��� ����������!", "������", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void button13_Click(object sender, EventArgs e)          // ������� ���� 
        {
            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }
            else
            {
                //MessageBox.Show("���� �� ����������!", "������", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                MessageBox.Show("���� �� ����������!", "������", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

        }

        private void button21_Click(object sender, EventArgs e)          // �������� ����� � ���� 
        {

            File.WriteAllText(fileName, textBox9.Text);
        }

        private void button15_Click(object sender, EventArgs e)          // ��������� ����
        {
            if (File.Exists(fileName))
            {
                textBox9.Text = File.ReadAllText(fileName);
            }
            else
            {
                MessageBox.Show("���� �� ����������!", "������", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

        }

        private void label145_Click(object sender, EventArgs e)
        {

        }

        private void label148_Click(object sender, EventArgs e)
        {

        }

        private void toolStripStatusLabel3_Click(object sender, EventArgs e)
        {

        }

        private void label78_Click(object sender, EventArgs e)
        {

        }

        private void lblResult_Click(object sender, EventArgs e)
        {

        }

        private void progressBar2_Click(object sender, EventArgs e)
        {

        }

        private void statusStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void label128_Click(object sender, EventArgs e)
        {

        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox7_TextChanged(object sender, EventArgs e)
        {
            textBox7.SelectionStart = textBox7.Text.Length;
            textBox7.ScrollToCaret();
            textBox7.Refresh();
        }

        private void textBox8_TextChanged(object sender, EventArgs e)
        {
            textBox8.SelectionStart = textBox8.Text.Length;
            textBox8.ScrollToCaret();
            textBox8.Refresh();
        }

        private void textBox9_TextChanged(object sender, EventArgs e)
        {
            textBox9.SelectionStart = textBox9.Text.Length;
            textBox9.ScrollToCaret();
            textBox9.Refresh();
        }

        private void groupBox21_Enter(object sender, EventArgs e)
        {

        }
        #endregion


        private void textBox11_TextChanged(object sender, EventArgs e)
        {
            textBox11.SelectionStart = textBox11.Text.Length;
            textBox11.ScrollToCaret();
            textBox11.Refresh();
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxSenAll.Checked == true)
            {
                // �������� �������� ���� ��������
                checkBoxSensors1.Checked = true;
                checkBoxSensors2.Checked = true;
                checkBoxSenGar1instr.Checked = true;
                checkBoxSenGar1disp.Checked = true;
                checkBoxSenTrubka.Checked = true;
                checkBoxSenTangN.Checked = true;
                checkBoxSenTangRuch.Checked = true;
                checkBoxSenMicrophon.Checked = true;
                checkBoxSenGGRadio1.Checked = true;
                checkBoxSenGGRadio2.Checked = true;
                checkBoxSenGGS.Checked = true;
                checkBoxDisp.Checked = true;
                checkBoxPower.Checked = true;
            }
            else
            {
                // ��������� �������� ���� ��������
                checkBoxSenGGRadio1.Checked = false;
                checkBoxSenGGRadio2.Checked = false;
                checkBoxSenTrubka.Checked = false;
                checkBoxSenTangN.Checked = false;
                checkBoxSenTangRuch.Checked = false;
                checkBoxSensors1.Checked = false;
                checkBoxSensors2.Checked = false;
                checkBoxSenGar1instr.Checked = false;
                checkBoxSenGar1disp.Checked = false;
                checkBoxSenMicrophon.Checked = false;
                checkBoxSenGGS.Checked = false;
                checkBoxDisp.Checked = false;
                checkBoxPower.Checked = false;
            }

        }

        private void checkBoxSenGGRadio1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void button24_Click(object sender, EventArgs e)                     // ��������� ������ �������� ������� �����������
        {
            short[] writeVals = new short[12];
            short[] MSK = new short[2];
            MSK[0] = 5;
            ushort[] readVals = new ushort[125];

            bool[] coilVals = new bool[200];
            bool[] coilArr = new bool[20];

            slave = int.Parse(txtSlave.Text, CultureInfo.CurrentCulture);

            textBox4.BackColor = Color.White;
            writeVals[1] = short.Parse(textBox4.Text, CultureInfo.CurrentCulture);   // ��������� ������ �������� �������
            int tempK = writeVals[1] * 5;                                            // ��������� ������ �������� �������
            if (tempK > 250)
            {
                label72.Text = "<";
                tempK = 250;
                textBox4.Text = "50";
                textBox4.BackColor = Color.Red;
            }
            else
            {
                label72.Text = "=";
                textBox4.BackColor = Color.White;
            }
            startWrReg = 60;                                                                   // 40060 ����� �������� �������� �������
            res = myProtocol.writeSingleRegister(slave, startWrReg, (short)tempK);
            startWrReg = 120;                                                                   // 
            res = myProtocol.writeSingleRegister(slave, startWrReg, 15);                        // 

        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {

        }

  
        private void checkBoxSensors1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            // �������� ������ ������
            error_list1();
            error_list2();
            error_list3();
            error_list_print();
        }

        private void label112_Click(object sender, EventArgs e)
        {

        }

        private void button25_Click(object sender, EventArgs e)                     // �������� ������� ������
        {
            short[] writeVals = new short[12];
            short[] readVals = new short[4];

            bool[] coilVals = new bool[200];
            bool[] coilArr = new bool[20];

            slave = int.Parse(txtSlave.Text, CultureInfo.CurrentCulture);

            textBox5.BackColor = Color.White;
            writeVals[1] = short.Parse(textBox5.Text, CultureInfo.CurrentCulture);   // ��������� ������ �������� �������
            int tempK = writeVals[1];                                                // ��������� ������ �������� �������
            if (tempK > 127)
            {
                label39.Text = "<";
                tempK = 127;
                textBox5.Text = "127";
                textBox5.BackColor = Color.Red;
            }
            else
            {
                label39.Text = "=";
                textBox5.BackColor = Color.White;
            }
            startWrReg = 61;                                                                       // 40060 ����� �������� �������� �������
            res = myProtocol.writeSingleRegister(slave, startWrReg, (short)tempK);
         
            startWrReg = 120;                                                                      // 
            res = myProtocol.writeSingleRegister(slave, startWrReg, 18);                        // 
            test_end1();
            startRdReg = 62;
            numRdRegs = 2;
            res = myProtocol.readMultipleRegisters(slave, startRdReg, readVals, numRdRegs);     // 40200 ������� �������� ������  
            if ((res == BusProtocolErrors.FTALK_SUCCESS))
            {
                toolStripStatusLabel1.Text = "    MODBUS ON    ";
                toolStripStatusLabel1.BackColor = Color.Lime;

                string s1 = readVals[0].ToString();                                             // �������������� ����� � ������
                label43.Text = (s1);

                if (readVals[1] != 0)
                {
                    string s2 = readVals[1].ToString();                                        // �������������� ����� � ������
                    label41.Text = (s2);
                    label42.Text = "���";
                }
                else
                {
                    label41.Text = "3,3";
                    label42.Text = "������";
                }
             }
        }

        private void label40_Click(object sender, EventArgs e)
        {

        }

        private void cmbComPort_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void FindSerial_Click(object sender, EventArgs e)
        {
 
                        label78.Text = "";
                        lblResult.Text = "";
                        toolStripStatusLabel3.Text = ("");
                        label78.Refresh();
                        lblResult.Refresh();
                       if (portFound != true)
                        {
                            myProtocol = new MbusRtuOverTcpMasterProtocol();
                            if (myProtocol.isOpen())
                            myProtocol.closeProtocol();
                            myProtocol = null;
                            SetComPort();
                            button5.Enabled = true;
                            Polltimer1.Enabled = true;
                        }
                        else
                        {
                            label78.Text = "COM ���� ��� ������";
                            label78.Refresh();
                        }
       // }
                        
        }

        private void label44_Click(object sender, EventArgs e)
        {

        }

        private void button65_Click(object sender, EventArgs e)
        {

        }

        private void button80_Click(object sender, EventArgs e)
        {

        }

        private void Test_power12v_Click(object sender, EventArgs e)
        {
            ushort[] readVolt = new ushort[10];
            numRdRegs = 2;
            ushort[] writeVals = new ushort[2];
            bool[] coilArr = new bool[4];
            startWrReg = 120;
            res = myProtocol.writeSingleRegister(slave, startWrReg, 17);                // �������� ��������� �������
            Thread.Sleep(250);
            test_end1();
            startRdReg = 494;
            res = myProtocol.readMultipleRegisters(slave, startRdReg, readVolt, numRdRegs);
            double s = readVolt[0] * 2.51 / 100;
            label45.Text = string.Format("{0:0.00}", s, CultureInfo.CurrentCulture);
            startRdReg = 495;
            res = myProtocol.readMultipleRegisters(slave, startRdReg, readVolt, numRdRegs);
            s = readVolt[0] * 2.51 / 100;
            label46.Text = string.Format("{0:0.00}", s, CultureInfo.CurrentCulture);
            startRdReg = 496;
            res = myProtocol.readMultipleRegisters(slave, startRdReg, readVolt, numRdRegs);
            s = readVolt[0] * 2.51 / 100;
            label47.Text = string.Format("{0:0.00}", s, CultureInfo.CurrentCulture);
            startRdReg = 493;
            res = myProtocol.readMultipleRegisters(slave, startRdReg, readVolt, numRdRegs);
            s = readVolt[0] * 2.51 / 100;
            label48.Text = string.Format("{0:0.00}", s, CultureInfo.CurrentCulture);
         }

        private void find_com_port_Tick(object sender, EventArgs e)
        {
            if ((myProtocol != null))
            {
                // Close protocol and serial port
                myProtocol.closeProtocol();
                //    // Indicate result on status line
                //lblResult.Text = "�������� ������";
                ////    // Disable button controls
                //button5.Enabled = false;
                //cmdOpenSerial.Enabled = true;
                //Polltimer1.Enabled = false;
                //toolStripStatusLabel1.Text = "  MODBUS ������   ";
                //toolStripStatusLabel1.BackColor = Color.Red;
                //toolStripStatusLabel3.Text = ("");
                //portFound = false;
            }


            if (portFound == false)
            {

                toolStripStatusLabel4.Text = "����� COM �����";
                SetComPort();
                if (portFound == true)
                {
                    toolStripStatusLabel4.Text = "";
                }
                Polltimer1.Enabled = true;
            }

        }

 
     }
}
