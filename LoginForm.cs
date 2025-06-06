using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EventDrivenProgram
{
    public partial class LoginForm : Form
    {
        // A boolean property that stores the TestMode setting value
        // To change this, go to Properties -> Settings.Settings -> TestMode
        private readonly bool testMode = Properties.Settings.Default.TestMode;


        // A reference to the currently focused text box
        private TextBox CurrentlyFocussedTextBox;


        // Declare list of accounts
        public static List<Account> accounts = new List<Account>();


        // Property to hold the Root Directory value
        public string RootDir { get; set; }

        static string fileName = "loginAttempts.txt";

        static string filePath;



        public LoginForm()
        {
            InitializeComponent();

            // Set the txtAccNo to be the first text box to have focus
            CurrentlyFocussedTextBox = txtbAccNo;

            // Give the textbox focus using the .Focus() method
            CurrentlyFocussedTextBox.Focus();

        }


        private void numBtns_Click(object sender, EventArgs e)
        {
            // Append the currently focused textbox with the value stored in the .text
            // Property of the button raising the event
            CurrentlyFocussedTextBox.Text += ((Button)sender).Text;
        }


        private void LoginForm_Load(object sender, EventArgs e)
        {
            GetRootDirecotory();

            filePath = RootDir + fileName;

        }



        private void txtbAccNo_Enter(object sender, EventArgs e)
        {
            CurrentlyFocussedTextBox = txtbAccNo;
        }

        private void txtbPIN_Enter(object sender, EventArgs e)
        {
            CurrentlyFocussedTextBox = txtbPIN;
        }



        // To prevent pin and account number numbers to exceed the length limit
        private void TxtBoxes_TextChanged(object sender, EventArgs e)
        {
            TextBox tbx = sender as TextBox;
            if (tbx.TextLength >= tbx.MaxLength)
            {
                tbx.Text = tbx.Text.ToString().Substring(0, tbx.MaxLength);
            }
        }

        private void txtbAccNo_TextChanged(object sender, EventArgs e)
        {
            TxtBoxes_TextChanged(sender, e);

            //bankingForm.ModifyTextBoxValue = txtbAccNo.Text;
        }

        private void txtbPIN_TextChanged(object sender, EventArgs e)
        {
            TxtBoxes_TextChanged(sender, e);
        }



        // Login button
        private void btnLogin_Click(object sender, EventArgs e)
        {
            //// Verifies the user
            //Account userAccount = authenticateUser();

            bool loginSuccessful;

            int loginNum = Int32.Parse(txtbAccNo.Text);
            int loginPIN = Int32.Parse(txtbPIN.Text);

            string sql = "Select * From Accounts Where AccountNumber = "+ loginNum + "";
            var loginInfo = AccountsData.GetData<Account>(sql, CommandType.Text);

            if (loginInfo.Count > 0)
            {

                if (testMode)
                { 
                    Publisher pub = new Publisher();

                    pub.RaiseTestEvent += HandleTestEvent;

                    pub.DoTestLogin();
                }

                Account userAcc = new Account();

                foreach (var objAcc in loginInfo)
                {
                    var a1 = loginInfo[0];

                    userAcc.UserName = objAcc.UserName;
                    userAcc.Balance = objAcc.Balance;
                    userAcc.AccountNumber = objAcc.AccountNumber;
                    userAcc.PIN = objAcc.PIN;
                }

                if (userAcc.PIN == loginPIN)
                {
                    loginSuccessful = true;

                    // Property we created for noting each login attempt
                    // Sends the attempt to be written in the file
                    SendToLoginFile(loginSuccessful);

                    BankingForm bankAcc = new BankingForm(userAcc);
                    bankAcc.Show();

                    this.Hide();
                }
                else
                {
                    loginSuccessful = false;

                    // Property we created for noting each login attempt
                    // Sends the attempt to be written in the file
                    SendToLoginFile(loginSuccessful);

                    MessageBox.Show("Please Enter Correct Account Details \n Please Try Again", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    ClearTextBoxes();
                }

            }
            else
            {
                loginSuccessful = false;

                // Property we created for noting each login attempt
                // Sends the attempt to be written in the file
                SendToLoginFile(loginSuccessful);

                MessageBox.Show("Please Enter Correct Account Details \n Please Try Again", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Information);

                ClearTextBoxes();
            }
            
        }

        void HandleTestEvent(object sender, TestEventArgs e)
        {
            string testLogDetails = $"\n Test Account: {txtbAccNo.Text} {e.Message}";
            File.AppendAllText(filePath, testLogDetails);
        }

        // Re-usable method to allow for clearing text boxes
        private void ClearTextBoxes()
        {
            txtbAccNo.Clear();
            txtbPIN.Clear();

            txtbAccNo.Focus();
        }



        // Closes the whole application
        private void button12_Click(object sender, EventArgs e)
        {
            MessageBox.Show("You are closing the program", "Exit", MessageBoxButtons.OK, MessageBoxIcon.Information);
            
            Application.Exit();

            //MessageBox.Show("You are closing the program", "Exit", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }



        // Clears text boxes when clicked 
        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearTextBoxes();
        }


        // Finds the root directory of the project
        private void GetRootDirecotory()
        {
            // Gets the location of the applications exe file
            string path = System.Reflection.Assembly.GetExecutingAssembly().Location;

            // We can now change to another folder once we have the executable's directory location
            var d = System.IO.Path.GetDirectoryName(path);

            var directory = d.Replace("bin\\Debug", "");

            // Assign the directory value to the RootDir property
            RootDir = directory;
        }



        // Creates a login attempt and appends it to the file we created for storing the data
        private void SendToLoginFile(bool successfulLogin)
        {
            string logDetails = $"Date: {DateTime.Now} \t Account Number: {txtbAccNo.Text} \t Successful: {successfulLogin} \n";

            // First parameter is the path and the 2nd is for the contents 
            File.AppendAllText(filePath, logDetails);
        }
    }
}
