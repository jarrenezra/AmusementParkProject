using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;

//using System.Linq;
//using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace shiProject2
{
    public partial class Form1 : Form
    {
        SerialPort port = new SerialPort("COM4", 9600);
        bool isPortConnectToggled = true;
        bool isPortDisonnectToggled = false;
        bool isGateUnlocked = false;
        bool isMerryGoRoundOn = false;
        bool isMerryGoRoundDirectionToggled = false;
        bool isFerrisWheelOn = false;
        bool isFerrisWheenDirectionToggled = false;
        bool isDartBoardOn = false;
        bool isDartBoardDirectionToggled = false;
        bool isLightsOn = false;

        int ferrisWheelSpeed = 0; //0 is the lowest, can't get negative.
        int dartBoardSpeed = 0; //0 is the lowest, can't get negative.



        public Form1()
        {
            InitializeComponent();
            ProjectBackground();
            button1.Enabled = isPortConnectToggled;//At the start, the connect button is enabled and the disconnect button is disabled.
            button2.Enabled = isPortDisonnectToggled;
        }
        private async void ProjectBackground()
        {
            await Task.Run(() => AcquireParkingSpaces());
        }
        private void button1_Click(object sender, EventArgs e) //CONNECT button
        {

            try
            {

                if (!port.IsOpen)
                {
                    port.Open(); //Open the port
                    port.Write("0");//turn led on
                    GeneralSafeLog("Successfully connected to the port.\n");
                    label1.BackColor = Color.Green; //This is that label with "PORT" in it.
                    isPortConnectToggled = !isPortConnectToggled; //Keep track of clicking the button
                    isPortDisonnectToggled = !isPortDisonnectToggled; //Keep track of clicking the button
                    button1.Enabled = isPortConnectToggled; //Disable the connect button until the disconnect button is clicked.
                    button2.Enabled = isPortDisonnectToggled; //Enable the disconnect button until the connect button is clicked.
                }
            }
            catch
            {
                GeneralSafeLog("An error occurred while connecting to the port.\n");
            }
        }

        private void button2_Click(object sender, EventArgs e) //disconnect button
        {
            try
            {

                if (port.IsOpen)
                {
                    port.Write("1");//turn led off
                    port.Close(); //Open the port
                    GeneralSafeLog("Successfully disconnected from the port.\n");
                    label1.BackColor = Color.Red; //This is that label with "PORT" in it.
                    isPortDisonnectToggled = !isPortDisonnectToggled; //Keep track of clicking the button
                    isPortConnectToggled = !isPortConnectToggled; //Keep track of clicking the button
                    button1.Enabled = isPortConnectToggled; //Disable the connect button until the disconnect button is clicked.
                    button2.Enabled = isPortDisonnectToggled; //Enable the disconnect button until the connect button is clicked.
                }
                else
                {
                    GeneralSafeLog("Port is already disconnected.\n");
                    isPortDisonnectToggled = !isPortDisonnectToggled; //Keep track of clicking the button
                    isPortConnectToggled = !isPortConnectToggled; //Keep track of clicking the button
                    button1.Enabled = isPortConnectToggled; //Disable the connect button until the disconnect button is clicked.
                    button2.Enabled = isPortDisonnectToggled; //Enable the disconnect button until the connect button is clicked.

                }

            }
            catch
            {
                GeneralSafeLog("An error occurred while disconnecting from the port.\n");
            }

        }

        private void button3_Click(object sender, EventArgs e) //UNLOCK/LOCK button
        {
            if (port.IsOpen) //Make sure the port is open before trying to write to it
            {
                isGateUnlocked = !isGateUnlocked; //Keep track of clicking the button
                if (isGateUnlocked) //if gate is unlocked
                {
                    port.Write("2");//Unlock gate
                    button3.BackColor = Color.Red; //switch appearance of button to "LOCK"
                    button3.Text = "LOCK";
                    label4.Text = "UNLOCKED"; //Change the text of the label to "OPEN"
                    label4.BackColor = Color.Green; //This is that label with "GATE" in it.
                    GeneralSafeLog("Gate Unlocked. Sent 2\n");
                }
                else
                {
                    port.Write("3");//Lock gate
                    button3.BackColor = Color.Green; //switch appearance of button to "UNLOCK"
                    button3.Text = "UNLOCK";
                    label4.Text = "LOCKED"; //Change the text of the label to "LOCKED"
                    label4.BackColor = Color.Red; //This is that label with "GATE" in it.
                    GeneralSafeLog("Gate Locked. Sent 3\n");

                }
            }
            else
            {
                GeneralSafeLog("An error occured, check if port is connected.\n");
            }
        }

        private void button5_Click(object sender, EventArgs e)//Turn on merry go round!
        {
            if (port.IsOpen) //Make sure the port is open before trying to write to it
            {
                isMerryGoRoundOn = !isMerryGoRoundOn; //Keep track of clicking the button
                if (isMerryGoRoundOn)
                {
                    port.Write("4"); //turn on merry go round
                    label3.BackColor = Color.Green; //This is that label with "MERRY GO ROUND" in it.
                    button5.Text = "OFF"; //switch appearance of button to "OFF"
                    button5.BackColor = Color.Red;//switch appearance of button to red
                    GeneralSafeLog("Merry Go Round is now live! Sent 4\n");
                }
                else
                {
                    port.Write("5"); //turn off merry go round
                    label3.BackColor = Color.Red; //This is that label with "MERRY GO ROUND" in it.
                    button5.Text = "ON"; //switch appearance of button to "ON"
                    button5.BackColor = Color.Green;//switch appearance of button to green
                    GeneralSafeLog("Merry Go Round has shutdown. Sent 5\n");
                }
            }
            else
            {
                GeneralSafeLog("An error occured, check if port is connected.\n");
            }
        }

        private void button6_Click(object sender, EventArgs e) //slow merry go round
        {
            if (port.IsOpen) //Make sure the port is open before trying to write to it
            {
                if (isMerryGoRoundOn)
                {
                    port.Write("6"); //set speed of merry go round to slow
                    GeneralSafeLog("Merry Go Round speed set to SLOW\n");
                    label2.BackColor = Color.Yellow; //This is that label with "SPEED" in it.
                }
                else
                {
                    //if the merry go round is off, then we can't change the speed, so we just print an error message.
                    GeneralSafeLog("Merry Go Round is not live, speed change fail.\n");
                }
            }
            else
            {
                GeneralSafeLog("An error occured, check if port is connected.\n");
            }
        }

        private void button7_Click(object sender, EventArgs e) //make merry go round fast
        {
            if (port.IsOpen) //Make sure the port is open before trying to write to it
            {
                if (isMerryGoRoundOn)
                {
                    port.Write("7"); //set speed of merry go round to fast
                    GeneralSafeLog("Merry Go Round speed set to FAST\n");
                    label2.BackColor = Color.Orange; //This is that label with "SPEED" in it.
                }
                else
                {
                    //if the merry go round is off, then we can't change the speed, so we just print an error message.
                    GeneralSafeLog("Merry Go Round is not live, speed change fail.\n");
                }
            }
            else
            {
                GeneralSafeLog("An error occured, check if port is connected.\n");
            }

        }

        private void button8_Click(object sender, EventArgs e) //make merry go round faster
        {
            if (port.IsOpen) //Make sure the port is open before trying to write to it
            {
                if (isMerryGoRoundOn)
                {
                    port.Write("8"); //set speed of merry go round to faster
                    GeneralSafeLog("Merry Go Round speed set to FASTER\n");
                    label2.BackColor = Color.Violet; //This is that label with "SPEED" in it.
                }
                else
                {
                    //if the merry go round is off, then we can't change the speed, so we just print an error message.
                    GeneralSafeLog("Merry Go Round is not live, speed change fail.\n");
                }
            }
            else
            {
                GeneralSafeLog("An error occured, check if port is connected.\n");
            }

        }

        private void button15_Click(object sender, EventArgs e) //Merry go round direction toggle button
        {
            if (port.IsOpen)
            {
                if (isMerryGoRoundOn)
                {
                    isMerryGoRoundDirectionToggled = !isMerryGoRoundDirectionToggled; //Keep track of clicking the button
                    if (isMerryGoRoundDirectionToggled)
                    {
                        port.Write("9"); //set direction of merry go round to one direction
                    }
                    else
                    {
                        port.Write("a");//set direction of merry go round to the other direction
                    }
                    GeneralSafeLog("Merry Go Round changed direction!\n");
                }
                else //if the merry go round is off, then we won't know the direction it is going to turn, since rotation is relative to the observer.
                {
                    GeneralSafeLog("Merry Go Round is not live, direction change fail.\n");
                    //Move the cursor to the end of the text in the richTextBox1
                }
            }
            else
            {
                GeneralSafeLog("An error occured, check if port is connected.\n");
            }
        }

        private void button12_Click(object sender, EventArgs e) // turn on ferris wheel!
        {
            if (port.IsOpen) //Make sure the port is open before trying to write to it
            {
                isFerrisWheelOn = !isFerrisWheelOn; //Keep track of clicking the button
                if (isFerrisWheelOn)
                {
                    port.Write("b"); //turn on ferris wheel
                    label5.BackColor = Color.Green; //This is that label with "FERRIS WHEEL" in it.
                    button12.Text = "OFF"; //switch appearance of button to "OFF"
                    button12.BackColor = Color.Red;//switch appearance of button to red
                    GeneralSafeLog("Ferris Wheel is now live!\n");
                }
                else
                {
                    port.Write("c"); //turn off ferris wheel
                    label5.BackColor = Color.Red; //This is that label with "FERRIS WHEEL" in it.
                    button12.Text = "ON"; //switch appearance of button to "ON"
                    button12.BackColor = Color.Green;//switch appearance of button to green
                    GeneralSafeLog("Ferris Wheel has shutdown.\n");
                }
            }
            else
            {
                GeneralSafeLog("An error occured, check if port is connected.\n");
            }

        }

        private void button16_Click(object sender, EventArgs e) //Ferris wheel direction toggle button
        {
            if (port.IsOpen)
            {
                if (isFerrisWheelOn)
                {
                    isFerrisWheenDirectionToggled = !isFerrisWheenDirectionToggled; //Keep track of clicking the button
                    if (isFerrisWheenDirectionToggled)
                    {
                        port.Write("d"); //set direction of merry go round to one direction
                    }
                    else
                    {
                        port.Write("e");//set direction of merry go round to the other direction
                    }
                    GeneralSafeLog("Ferris Wheel changed direction!\n");
                }
                else //if the merry go round is off, then we won't know the direction it is going to turn, since rotation is relative to the observer.
                {
                    GeneralSafeLog("Ferris Wheel is not live, direction change fail.\n");
                }
            }
            else
            {
                GeneralSafeLog("An error occured, check if port is connected.\n");
            }

        }

        private void button10_Click(object sender, EventArgs e) //ferris wheel increase speed by an increment button
        {
            if (port.IsOpen) //Make sure the port is open before trying to write to it
            {
                if (isFerrisWheelOn)
                {
                    if (ferrisWheelSpeed >= 3) //3 is max, 0 for no speed, 1 for slow, 2 for fast, 3 for faster
                    {
                        GeneralSafeLog("Ferris wheel is at the max speed.\n");
                    }
                    else
                    {

                        port.Write("g"); //every time arduino receives "g", it will increment by n.
                        ++ferrisWheelSpeed; //this is for display
                        label6.Text = ferrisWheelSpeed.ToString(); //This is that label with "FERRIS WHEEL SPEED" in it
                        GeneralSafeLog("Ferris wheel increased its speed\n");

                    }

                }
                else
                {
                    //if the ferris wheel is off, then we can't change the speed, so we just print an error message.
                    GeneralSafeLog("Ferris wheel is not live, speed change fail.\n");
                }
            }
            else
            {
                GeneralSafeLog("An error occured, check if port is connected.\n");
            }

        }

        private void button9_Click(object sender, EventArgs e) //ferris wheel decrease speed by an decrement button
        {
            if (port.IsOpen) //Make sure the port is open before trying to write to it
            {
                if (isFerrisWheelOn)
                {
                    if (ferrisWheelSpeed > 0) //make sure we don't hit negative
                    {
                        port.Write("h"); //every time arduino receives "g", it will increment by n.
                        --ferrisWheelSpeed; //this is for display
                        label6.Text = ferrisWheelSpeed.ToString(); //This is that label with "FERRIS WHEEL SPEED" in it
                        GeneralSafeLog("Ferris wheel decreased its speed\n");
                    }
                    else
                    {
                        GeneralSafeLog("Ferris wheel is at the lowest speed.\n");
                    }
                }
                else
                {
                    //if the ferris wheel is off, then we can't change the speed, so we just print an error message.
                    GeneralSafeLog("Ferris wheel is not live, speed change fail.\n");
                }
            }
            else
            {
                GeneralSafeLog("An error occured, check if port is connected.\n");
            }

        }

        private void button17_Click(object sender, EventArgs e) //Dart board on off button
        {
            if (port.IsOpen) //Make sure the port is open before trying to write to it
            {
                isDartBoardOn = !isDartBoardOn; //Keep track of clicking the button
                if (isDartBoardOn)
                {
                    port.Write("i"); //turn on Dart Board
                    label8.BackColor = Color.Green; //This is that label with "DART BOARD" in it.
                    button17.Text = "OFF"; //switch appearance of button to "OFF"
                    button17.BackColor = Color.Red;//switch appearance of button to red
                    GeneralSafeLog("Dart board is now spinning!\n");
                }
                else
                {
                    port.Write("j"); //turn off Dart Board
                    label8.BackColor = Color.Red; //This is that label with "DART BOARD" in it.
                    button17.Text = "ON"; //switch appearance of button to "ON"
                    button17.BackColor = Color.Green;//switch appearance of button to green
                    GeneralSafeLog("Dart board has shutdown.\n");
                }
            }
            else
            {
                GeneralSafeLog("An error occured, check if port is connected.\n");
            }

        }

        private void button11_Click(object sender, EventArgs e) //Change dart board direction button
        {
            if (port.IsOpen)
            {
                if (isDartBoardOn)
                {
                    isDartBoardDirectionToggled = !isDartBoardDirectionToggled; //Keep track of clicking the button
                    if (isDartBoardDirectionToggled)
                    {
                        port.Write("k"); //set direction of dart board to one direction
                    }
                    else
                    {
                        port.Write("l");//set direction of dart board to the other direction
                    }
                    GeneralSafeLog("Dart Board changed direction!\n");
                }
                else //if the dart board is off, then we won't know the direction it is going to turn, since rotation is relative to the observer.
                {
                    GeneralSafeLog("Dart Board is not live, direction change fail.\n");
                }
            }
            else
            {
                GeneralSafeLog("An error occured, check if port is connected.\n");
            }

        }

        private void button13_Click(object sender, EventArgs e) //Dart board increase speed by an increment button
        {
            if (port.IsOpen) //Make sure the port is open before trying to write to it
            {
                if (isDartBoardOn)
                {
                    if (dartBoardSpeed >= 4) //slow,fast,faster,fastest, 0 for no speed
                    {
                        GeneralSafeLog("Dart board is at the max speed.\n");
                    }
                    else
                    {
                        port.Write("m"); //every time arduino receives "n", it will increase by n.
                        ++dartBoardSpeed; //this is for display
                        label7.Text = dartBoardSpeed.ToString(); //This is that label with "DART BOARD SPEED" in it
                        GeneralSafeLog("Dart board increased its speed\n");

                    }

                }
                else
                {
                    //if the ferris wheel is off, then we can't change the speed, so we just print an error message.
                    GeneralSafeLog("Dart Board is not live, speed change fail.\n");
                }
            }
            else
            {
                GeneralSafeLog("An error occured, check if port is connected.\n");
            }

        }

        private void button14_Click(object sender, EventArgs e) //Dart board decrease speed by an decrement button
        {
            if (port.IsOpen) //Make sure the port is open before trying to write to it
            {
                if (isDartBoardOn)
                {
                    if (dartBoardSpeed > 0) //make sure we don't hit negative
                    {
                        port.Write("n"); //every time arduino receives "n", it will decrement by n.
                        --dartBoardSpeed; //this is for display
                        label7.Text = dartBoardSpeed.ToString(); //This is that label with "DART BOARD SPEED" in it
                        GeneralSafeLog("Dart board decreased its speed\n");
                    }
                    else
                    {
                        GeneralSafeLog("Dart board is at the lowest speed.\n");
                    }
                }
                else
                {
                    //if the dart board is off, then we can't change the speed, so we just print an error message.
                    GeneralSafeLog("Dart board is not live, speed change fail.\n");
                }
            }
            else
            {
                GeneralSafeLog("An error occured, check if port is connected.\n");
            }

        }

        private void AcquireParkingSpaces()
        {
            while (true)
            {
                if (port.IsOpen)
                {

                    port.Write("o"); //request arduino for parking space status.
                    if (port.BytesToRead > 0)
                    {
                        string parkingSpaceStatus = port.ReadLine().Trim();
                        switch (parkingSpaceStatus)
                        {
                            case "000": //all occupied
                                label9.BackColor = Color.Red; //1
                                label10.BackColor = Color.Red;//2
                                label11.BackColor = Color.Red;//3
                                ParkingSafeLog("Received 000\n");
                                break;
                            case "001":
                                label9.BackColor = Color.Red;
                                label10.BackColor = Color.Red;
                                label11.BackColor = Color.Green;
                                ParkingSafeLog("Received 001\n");
                                break;
                            case "010":
                                label9.BackColor = Color.Red;
                                label10.BackColor = Color.Green;
                                label11.BackColor = Color.Red;
                                ParkingSafeLog("Received 010\n");
                                break;
                            case "011":
                                label9.BackColor = Color.Red;
                                label10.BackColor = Color.Green;
                                label11.BackColor = Color.Green;
                                ParkingSafeLog("Received 011\n");
                                break;
                            case "100":
                                label9.BackColor = Color.Green;
                                label10.BackColor = Color.Red;
                                label11.BackColor = Color.Red;
                                ParkingSafeLog("Received 100\n");
                                break;
                            case "101":
                                label9.BackColor = Color.Green;
                                label10.BackColor = Color.Red;
                                label11.BackColor = Color.Green;
                                ParkingSafeLog("Received 101\n");
                                break;
                            case "110":
                                label9.BackColor = Color.Green;
                                label10.BackColor = Color.Green;
                                label11.BackColor = Color.Red;
                                ParkingSafeLog("Received 110\n");
                                break;
                            case "111"://all free
                                label9.BackColor = Color.Green;
                                label10.BackColor = Color.Green;
                                label11.BackColor = Color.Green;
                                ParkingSafeLog("Received 111\n");
                                break;

                        }
                        if (richTextBox2.InvokeRequired) //richTextBox2 for parking space only since I don't want one console handle everything.
                        {
                            // We are on the wrong thread! Use Invoke to switch to the UI thread.
                            richTextBox2.Invoke(new Action(() =>
                            {
                                ParkingSafeLog("Aquiring parking space status.\n");
                            }));
                        }
                        else
                        {
                            // We are already on the UI thread, just update it normally.
                            ParkingSafeLog("Aquiring parking space status.\n");
                        }


                    }
                    else
                    {
                        if (richTextBox2.InvokeRequired)
                        {
                            // We are on the wrong thread! Use Invoke to switch to the UI thread.
                            richTextBox2.Invoke(new Action(() =>
                            {
                                ParkingSafeLog("No response from arduino, check if port is connected.\n");
                            }));
                        }
                        else
                        {
                            // We are already on the UI thread, just update it normally.
                            ParkingSafeLog("No response from arduino, check if port is connected.\n");
                        }
                    }

                }
                else
                {
                    if (richTextBox2.InvokeRequired)
                    {
                        // We are on the wrong thread! Use Invoke to switch to the UI thread.
                        richTextBox2.Invoke(new Action(() =>
                        {
                            ParkingSafeLog("An error occured, check if port is connected.\n");
                        }));
                    }
                    else
                    {
                        // We are already on the UI thread, just update it normally.
                        ParkingSafeLog("An error occured, check if port is connected.\n");
                    }

                }
                Task.Delay(2000).Wait();
            }
        }

        private void button18_Click(object sender, EventArgs e) //turn on lights
        {
            if (port.IsOpen)
            {
                isLightsOn = !isLightsOn; //Keep track of clicking the button
                if (isLightsOn)
                {
                    port.Write("p"); //turn on lights
                    label14.BackColor = Color.Green; //This is that label with "LIGHTS" in it.
                    label14.Text = "ON"; //switch appearance of label to "OFF"
                    button18.Text = "OFF"; //switch appearance of button to "OFF"
                    button18.BackColor = Color.Red;//switch appearance of button to red
                    GeneralSafeLog("Lights are now ON!\n");
                }
                else
                {
                    port.Write("q"); //turn off lights
                    label14.BackColor = Color.Red; //This is that label with "LIGHTS" in it.
                    label14.Text = "OFF"; //switch appearance of label to "OFF"
                    button18.Text = "ON"; //switch appearance of button to "ON"
                    button18.BackColor = Color.Green;//switch appearance of button to green
                    GeneralSafeLog("Lights are now OFF.\n");
                }
            }
            else
            {
                GeneralSafeLog("An error occured, check if port is connected.\n");
            }
        }
        private void GeneralSafeLog(string msg) //Set a limit to the number of lines in the rich text box to prevent memory overflow, and also make sure that we are on the right thread when updating the UI.
        {
            if (this.IsDisposed) return; //If the form is already disposed, we shouldn't try to update the UI, so we just return.

            if (richTextBox1.InvokeRequired) //Check if we are on the right thread to update the UI. If not, use Invoke to switch to the UI thread.
            {
                this.Invoke(new Action<string>(GeneralSafeLog), msg);//Invoke the same method on the UI thread with the same message.
            }
            else
            {
                int maxLines = 500; //set maximum lines to 500, if we exceed that, we will remove the oldest 100 lines to prevent memory overflow.

                richTextBox1.AppendText($"[{DateTime.Now:HH:mm:ss}] {msg}{Environment.NewLine}");//Append the new message to the rich text box with a timestamp.

                if (richTextBox1.Lines.Length > maxLines)//If we exceed the maximum number of lines, we will remove the oldest 100 lines.
                {
                    richTextBox1.ReadOnly = false; // Temporarily allow changes

                    string[] newLines = richTextBox1.Lines.Skip(richTextBox1.Lines.Length - 400).ToArray();//Keep the most recent 400 lines and remove the oldest 100 lines.
                    richTextBox1.Lines = newLines;//Update the lines in the rich text box with the new lines.

                    richTextBox1.ReadOnly = true;//Set it back to read only after updating the lines.
                }

                richTextBox1.SelectionStart = richTextBox1.Text.Length;//Move the cursor to the end of the text in the richTextBox1
                richTextBox1.ScrollToCaret();//Scroll to the cursor, which is at the end of the text, to show the most recent log message.
            }
        }
        private void ParkingSafeLog(string msg)//This is a separate log for parking space status updates, since they are more frequent and we might want to handle them differently in the future. It also has its own limit of lines to prevent memory overflow.
        {
            if (this.IsDisposed) return;//If the form is already disposed, we shouldn't try to update the UI, so we just return.

            if (richTextBox2.InvokeRequired)//Check if we are on the right thread to update the UI. If not, use Invoke to switch to the UI thread.
            {
                this.Invoke(new Action<string>(ParkingSafeLog), msg);//Invoke the same method on the UI thread with the same message.
            }
            else
            {
                int maxLines = 500;//set maximum lines to 500, if we exceed that, we will remove the oldest 100 lines to prevent memory overflow.

                richTextBox2.AppendText($"[{DateTime.Now:HH:mm:ss}] {msg}{Environment.NewLine}");//Append the new message to the rich text box with a timestamp.

                if (richTextBox2.Lines.Length > maxLines)//If we exceed the maximum number of lines, we will remove the oldest 100 lines.
                {
                    richTextBox2.ReadOnly = false; // Temporarily allow changes

                    string[] newLines = richTextBox2.Lines.Skip(richTextBox2.Lines.Length - 400).ToArray();//Keep the most recent 400 lines and remove the oldest 100 lines.
                    richTextBox2.Lines = newLines;//Update the lines in the rich text box with the new lines.

                    richTextBox2.ReadOnly = true;//Set it back to read only after updating the lines.
                }

                richTextBox2.SelectionStart = richTextBox2.Text.Length;//Move the cursor to the end of the text in the richTextBox2
                richTextBox2.ScrollToCaret();//Scroll to the cursor, which is at the end of the text, to show the most recent log message.
            }
        }
    }
}
