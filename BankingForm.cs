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
    public partial class BankingForm : Form
    {
        private TextBox CurrentlyFocussedTextBox;

        private Account userInfo;


        // Property to hold the Root Directory value
        public string RootDir { get; set; }

        static string fileName = "TransactionList.txt";

        static string filePath;


        public BankingForm(Account userAccount)
        {
            InitializeComponent();

            userInfo = userAccount;

            // Set the tbxAmount the first text box to have focus
            CurrentlyFocussedTextBox = tbxAmount;

            // Give the textbox focus using the .Focus() method
            CurrentlyFocussedTextBox.Focus();
        }

       

       private void BankingForm_Load(object sender, EventArgs e)
        {
            GetRootDirecotory();

            filePath = RootDir + fileName;

            lblAccNo.Text = userInfo.AccountNumber.ToString();

            lblBalance.Text = userInfo.Balance.ToString();

            lblName.Text = userInfo.UserName;

        }


        // number pad click event
        private void numBtns_Click(object sender, EventArgs e)
        {
            // Append the currently focused textbox with the value stored in the .text
            // Property of the button raising the event
            CurrentlyFocussedTextBox.Text += ((Button)sender).Text;
        }


        // Clears the text box for the amount
        private void btnClear_Click(object sender, EventArgs e)
        {
            tbxAmount.Clear();
        }


        // Logs out the user and opens a new Login form
        private void btnLogout_Click(object sender, EventArgs e)
        {

            MessageBox.Show("You have been logged out successfully", "Logged Out", MessageBoxButtons.OK, MessageBoxIcon.Information);

            Close();
        }

      
        private void Deposit()
        {
            decimal amount;
         
            amount = Convert.ToDecimal(tbxAmount.Text);

            userInfo.TransactionDeposit(amount);
        }


        private void Withdraw()
        {
            decimal amount;
            amount = Convert.ToDecimal(tbxAmount.Text);

            userInfo.TransactionWithdraw(amount);
        }


        private void btnSubmit_Click(object sender, EventArgs e)
        {
            bool transactionType;

            // Used to verify the amount given by the user
            bool amountVerified = false;
            decimal inputAmount = 0.00m;

            // To make sure the input given by the user is correct 
            if (decimal.TryParse(tbxAmount.Text, out inputAmount))
            {
                amountVerified = true;
            }

            if (amountVerified)
            {
                try
                {
                    // Deposits money if vlaue is correct and the rbDeposit is checked
                    if (rbDeposit.Checked)
                    {
                        Deposit();

                        MessageBox.Show("Money has been successfully depositted", "Thank You", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        transactionType = true;

                        SendToTransactionFile(transactionType);

                        tbxAmount.Clear();

                        lblBalance.Text = userInfo.Balance.ToString();
                    }
                    // Withdraws money if vlaue is correct and the rbDeposit isnt checked
                    else
                    {
                        Withdraw();

                        MessageBox.Show("Money has been successfully withdrawn", "Thank You", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        transactionType = false;

                        SendToTransactionFile(transactionType);

                        tbxAmount.Clear();

                        lblBalance.Text = userInfo.Balance.ToString();
                    }
                }
                // exception catchers, that will show a message when an exception happens
                catch (ArgumentOutOfRangeException a)
                {
                    MessageBox.Show(a.Message);
                }
                catch (InvalidOperationException i)
                {
                    MessageBox.Show(i.Message);
                }
                catch (FormatException)
                {
                    MessageBox.Show("Format Exception", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            // If a unwanted value is entered this message pops up
            else
            {
                MessageBox.Show("Unknown number for transaction \n Try again", "Ivalid Input", MessageBoxButtons.OK, MessageBoxIcon.Error);

                tbxAmount.Clear();
            }
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


        // Sends the transaction information to the transaction log file
        private void SendToTransactionFile(bool transactionType)
        {
            string tranTyp;

            if (transactionType == true)
                tranTyp = "Deposit";
            else
                tranTyp = "Withdrawl";


            string transactionDetails = $"Date: {DateTime.Now} \t Account Number: {userInfo.AccountNumber} \t Transaction Type: {tranTyp} \t Amount: {tbxAmount.Text} \t New Balance: {userInfo.Balance} \n";


            // First parameter is the path and the 2nd is for the contents 
            File.AppendAllText(filePath, transactionDetails);
        }


        // Function that closes the Banking form window when clicking the 'X' button, and opens a new Login Form 
        private void BankingForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            MessageBox.Show("Leaving this page will log you out", "Close", MessageBoxButtons.OK, MessageBoxIcon.Information);

            new LoginForm().Show();
        }
    }
}
