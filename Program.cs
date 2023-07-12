using System;
using System.Collections.Generic;

namespace BankAccountSystem
{
    class Transaction
    {
        public string Id { get; }
        public DateTime Date { get; }
        public string Account { get; }
        public TransactionType Type { get; }
        public decimal Amount { get; }

        public Transaction(string id, DateTime date, string account, TransactionType type, decimal amount)
        {
            Id = id;
            Date = date;
            Account = account;
            Type = type;
            Amount = amount;
        }

        public override string ToString()
        {
            return $"{Date:yyyyMMdd} | {Id} | {Type} | {Amount:C}";
        }
    }

    enum TransactionType
    {
        Deposit,
        Withdrawal,
        Interest
    }

    class BankAccount
    {
        public string AccountNumber { get; }
        public List<Transaction> Transactions { get; }
        public decimal Balance { get; private set; }

        public BankAccount(string accountNumber)
        {
            AccountNumber = accountNumber;
            Transactions = new List<Transaction>();
            Balance = 0;
        }

        public void AddTransaction(Transaction transaction)
        {
            Transactions.Add(transaction);
            if (transaction.Type == TransactionType.Deposit || transaction.Type == TransactionType.Interest)
            {
                Balance += transaction.Amount;
            }
            else if (transaction.Type == TransactionType.Withdrawal)
            {
                Balance -= transaction.Amount;
            }
        }

        public override string ToString()
        {
            var statement = $"Account: {AccountNumber}\n";
            statement += "Date     | Txn Id      | Type | Amount | Balance |\n";
            foreach (var transaction in Transactions)
            {
                statement += $"{transaction} | {Balance:C}\n";
            }
            return statement;
        }
    }

    class InterestRule
    {
        public DateTime Date { get; }
        public string RuleId { get; }
        public decimal Rate { get; }

        public InterestRule(DateTime date, string ruleId, decimal rate)
        {
            Date = date;
            RuleId = ruleId;
            Rate = rate;
        }

        public override string ToString()
        {
            return $"{Date:yyyyMMdd} | {RuleId} | {Rate:P}";
        }
    }

    class Bank
    {
        private static Dictionary<string, BankAccount> accounts = new Dictionary<string, BankAccount>();
        private static List<InterestRule> interestRules = new List<InterestRule>();
        private static int transactionCounter = 1;

        public static void Main(string[] args)
        {
            Console.WriteLine("Welcome to AwesomeGIC Bank! What would you like to do?");
            //while (true)
            //{
            //    Console.WriteLine("[I]nput transactions");
            //    Console.WriteLine("[D]efine interest rules");
            //    Console.WriteLine("[P]rint statement");
            //    Console.WriteLine("[Q]uit");
            //    Console.Write("> ");
            //    var choice = Console.ReadLine().ToUpper();

            //    switch (choice)
            //    {
            //        case "I":
            //            InputTransactions();
            //            break;
            //        case "D":
            //            DefineInterestRules();
            //            break;
            //        case "P":
            //            PrintStatement();
            //            break;
            //        case "Q":
            //            Quit();
            //            return;
            //        default:
            //            Console.WriteLine("Invalid choice. Please try again.");
            //            break;
            //    }
            //}
            string Userinput;
            do
            {
                
                Console.WriteLine("[I]nput transactions");
                Console.WriteLine("[D]efine interest rules");
                Console.WriteLine("[P]rint statement");
                Console.WriteLine("[Q]uit");
                Console.Write("> ");
                Userinput = Console.ReadLine()?.Trim().ToUpper();

                switch (Userinput)
                {
                    case "I":
                        InputTransactions();
                        break;
                    case "D":
                        DefineInterestRules();
                        break;
                    case "P":
                        PrintStatement();
                        break;
                    case "Q":
                        Console.WriteLine("Thank you for banking with AwesomeGIC Bank.");
                        Console.WriteLine("Have a nice day!");
                        break;
                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        break;
                }

                Console.WriteLine();
            } while (Userinput != "Q");
        }
   

        private static void InputTransactions()
        {
            Console.WriteLine("Please enter transaction details in <Date>|<Account>|<Type>|<Amount> format");
            Console.WriteLine("(or enter blank to go back to the main menu):");
            while (true)
            {
                Console.Write("> ");
                var input = Console.ReadLine();
                
                if (string.IsNullOrWhiteSpace(input))
                {
                    return;
                }
                //Checking the user input (yyyyMMDdd|Acnumb|W/D|amount)
                var parts = input.Split('|');
                if (parts.Length != 4)
                {
                    Console.WriteLine("Invalid input format. Please try again.");
                    continue;
                }
                //Checking date format
                if (!DateTime.TryParseExact(parts[0], "yyyyMMdd", null, System.Globalization.DateTimeStyles.None, out var date))
                {
                    Console.WriteLine("Invalid date format. Please try again.");
                    continue;
                }
                //Check Deposit or withdrwal
                var account = parts[1].Trim();
                var type = parts[2].ToUpper();
                if (type != "D" && type != "W")
                {
                    Console.WriteLine("Invalid transaction type. Please enter D for deposit or W for withdrawal.");
                    continue;
                }

                if (!decimal.TryParse(parts[3], out var amount) || amount <= 0)
                {
                    Console.WriteLine("Invalid amount. Please enter a positive decimal value.");
                    continue;
                }

                if (!accounts.ContainsKey(account))
                {
                    if (type == "W")
                    {
                        Console.WriteLine("The first transaction on a new account must be a deposit.");
                        continue;
                    }

                    accounts[account] = new BankAccount(account);
                }
                else if (type == "W" && accounts[account].Balance - amount < 0)
                {
                    Console.WriteLine("The account balance cannot go below 0. Please enter a different amount.");
                    continue;
                }

                var transactionId = $"{date:yyyyMMdd}-{transactionCounter:D2}";
                var transactionType = type == "D" ? TransactionType.Deposit : TransactionType.Withdrawal;
                var transaction = new Transaction(transactionId, date, account, transactionType, amount);
                accounts[account].AddTransaction(transaction);
                transactionCounter++;

                Console.WriteLine("Transaction added successfully!");
                Console.WriteLine(accounts[account]);
                //comments
              
                //comments
            }
        }

        private static void DefineInterestRules()
        {
            Console.WriteLine("Please enter interest rules details in <Date>|<RuleId>|<Rate in %> format");
            Console.WriteLine("(or enter blank to go back to the main menu):");
            while (true)
            {
                Console.Write("> ");
                var input = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(input))
                {
                    return;
                }
                //check user input (20230615|RULE03|2.20)
                var parts = input.Split('|');
                if (parts.Length != 3)
                {
                    Console.WriteLine("Invalid input format. Please try again.");
                    continue;
                }

                if (!DateTime.TryParseExact(parts[0], "yyyyMMdd", null, System.Globalization.DateTimeStyles.None, out var date))
                {
                    Console.WriteLine("Invalid date format. Please try again.");
                    continue;
                }
                //Ruleid
                var ruleId = parts[1].Trim();
                if (string.IsNullOrWhiteSpace(ruleId))
                {
                    Console.WriteLine("Invalid rule ID. Please enter a non-empty value.");
                    continue;
                }
                //interest rate (rate>0 and <100)
                if (!decimal.TryParse(parts[2], out var rate) || rate <= 0 || rate >= 100)
                {
                    Console.WriteLine("Invalid interest rate. Please enter a value greater than 0 and less than 100.");
                    continue;
                }

                var rule = new InterestRule(date, ruleId, rate);
                // Remove any existing rule for the same date
                interestRules.RemoveAll(r => r.Date == date);
                interestRules.Add(rule);
                interestRules.Sort((r1, r2) => r1.Date.CompareTo(r2.Date));

                Console.WriteLine("Interest rule added successfully!");
                Console.WriteLine("Interest rules:");
                foreach (var interestRule in interestRules)
                {
                    Console.WriteLine(interestRule);
                }
            }
        }

        private static void PrintStatement()
        {
            Console.WriteLine("Please enter account andmonth to generate the statement <Account>|<Month>");
            Console.WriteLine("(or enter blank to go back to the main menu):");
            while (true)
            {
                Console.Write("> ");
                var input = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(input))
                {
                    return;
                }

                var parts = input.Split('|');
                if (parts.Length != 2)
                {
                    Console.WriteLine("Invalid input format. Please try again.");
                    continue;
                }

                var account = parts[0].Trim();
                var month = parts[1].Trim();
                if (string.IsNullOrWhiteSpace(account) || string.IsNullOrWhiteSpace(month))
                {
                    Console.WriteLine("Invalid account or month. Please enter valid values.");
                    continue;
                }

                if (!accounts.ContainsKey(account))
                {
                    Console.WriteLine("Account not found.");
                    continue;
                }

                Console.WriteLine($"Account: {account}");
                Console.WriteLine("Date     | Txn Id      | Type | Amount | Balance |");
                var accountTransactions = accounts[account].Transactions;
                foreach (var transaction in accountTransactions)
                {
                    if (transaction.Date.ToString("MM") == month)
                    {
                        Console.WriteLine($"{transaction.Date:yyyyMMdd} | {transaction.Id} | {transaction.Type} | {transaction.Amount:C} | {accounts[account].Balance:C}");
                    }
                }

                // display interest for the month
                var interestRate = GetInterestRateForMonth(month);
                var interestAmount = CalculateInterest(accounts[account], month, interestRate);
                Console.WriteLine($"{DateTime.DaysInMonth(DateTime.Now.Year, int.Parse(month)):yyyyMMdd} | | I | {interestAmount:C} | {accounts[account].Balance:C}");

                break;
            }
        }

        private static decimal GetInterestRateForMonth(string month)
        {
            foreach (var interestRule in interestRules)
            {
                if (interestRule.Date.ToString("MM") == month)
                {
                    return interestRule.Rate;
                }
            }
            return 0;
        }

        private static decimal CalculateInterest(BankAccount account, string month, decimal interestRate)
        {
            var startDate = new DateTime(DateTime.Now.Year, int.Parse(month), 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);
            var balance = account.Balance;
            decimal totalInterest = 0;

            foreach (var transaction in account.Transactions)
            {
                if (transaction.Date >= startDate && transaction.Date <= endDate)
                {
                    if (transaction.Type == TransactionType.Deposit || transaction.Type == TransactionType.Interest)
                    {
                        balance += transaction.Amount;
                    }
                    else if (transaction.Type == TransactionType.Withdrawal)
                    {
                        balance -= transaction.Amount;
                    }
                }
            }

            var daysInMonth = (endDate - startDate).Days + 1;
            var dailyRate = interestRate / 100 / 365;
            var interest = balance * dailyRate * daysInMonth;

            totalInterest += Math.Round(interest, 2);
            account.AddTransaction(new Transaction("", endDate, account.AccountNumber, TransactionType.Interest, totalInterest));

            return totalInterest;
        }

        private static void Quit()
        {
            Console.WriteLine("Thank you for banking with AwesomeGIC Bank.");
            Console.WriteLine("Have a nice day!");
        }
    }
}
