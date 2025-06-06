using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EventDrivenProgram
{
    /// <summary>
    ///  This Class represents an Account
    /// </summary>
    public class Account
    {
        
        // Properties to store Account details
        public int AccountNumber { get; set; }
        public decimal Balance { get; set; }
        public int PIN { get; set; }
        public string UserName { get; set; }


        // This method is called when the user attempts to deposi money into their account.
        // The method validates the user input the ensure it is acceptable.
        public void TransactionDeposit(decimal amount)
        {
            if (amount <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(amount), "The amount to deposit must be positive");
            }
            else
            {
                // Perform Transaction
                Balance = Balance + amount;
            }
        }


        // This method is called when the user attempts to withdraw money from their account.
        // The method validates the user input the ensure it is acceptable.
        public void TransactionWithdraw(decimal amount)
        {
            if (amount <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(amount), "Amount to withdraw must be a positive number");
            }
            else if (Balance < amount)
            {
                throw new InvalidOperationException("Insufficient funds");
            }
            else
            {
                // Perform Transaction
                Balance = Balance - amount;
            }
        }


        public Account()
        {

        }

        // Overload constructor
        public Account(string name, decimal intitialBalance, int pin, int accountNumber)
        {
            // Assign passed in variable values to the property values.
            UserName = name;
            Balance = intitialBalance;
            PIN = pin;
            AccountNumber = accountNumber;
          
        }
    }
}
