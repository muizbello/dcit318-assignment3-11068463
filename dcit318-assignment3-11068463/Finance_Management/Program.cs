using System;
using System.Collections.Generic;


namespace Finance_Management
{
    public record Transaction(int Id, DateTime Date, decimal Amount, string Category);

    public interface ITransactionProcessor
    {
        void Process(Transaction transaction);
    }

    public class BankTransferProcessor : ITransactionProcessor
    {
        public void Process(Transaction transaction)
        {
            Console.WriteLine($"Bank Transfer: {transaction.Amount} for {transaction.Category}");
        }
    }

    public class MobileMoneyProcessor : ITransactionProcessor
    {
        public void Process(Transaction transaction)
        {
            Console.WriteLine($"Mobile Money: {transaction.Amount} for {transaction.Category}");
        }
    }

    public class CryptoWalletProcessor : ITransactionProcessor
    {
        public void Process(Transaction transaction)
        {
            Console.WriteLine($"Crypto Wallet: {transaction.Amount} for {transaction.Category}");
        }
    }

    public class Account
    {
        public string AccountNumber { get; }
        public decimal Balance { get; protected set; }
        private readonly List<Transaction> _transactions = new();

        public Account(string accountNumber, decimal initialBalance)
        {
            AccountNumber = accountNumber;
            Balance = initialBalance;
        }

        public virtual void ApplyTransaction(Transaction transaction)
        {
            Balance -= transaction.Amount;
            _transactions.Add(transaction);
        }
    }

    public sealed class SavingsAccount : Account
    {
        public SavingsAccount(string accountNumber, decimal initialBalance)
            : base(accountNumber, initialBalance) { }

        public override void ApplyTransaction(Transaction transaction)
        {
            if (Balance - transaction.Amount >= 0)
                base.ApplyTransaction(transaction);
            else
                Console.WriteLine("Insufficient funds for savings account transaction.");
        }
    }

    public class FinanceApp
    {
        public void Run()
        {
            ITransactionProcessor bankProcessor = new BankTransferProcessor();
            ITransactionProcessor momoProcessor = new MobileMoneyProcessor();
            ITransactionProcessor cryptoProcessor = new CryptoWalletProcessor();

            var savings = new SavingsAccount("MB11068463", 10000m);

            var t1 = new Transaction(1, DateTime.Now, 2573m, "School Fees");
            var t2 = new Transaction(2, DateTime.Now, 35m, "Waakye");
            var t3 = new Transaction(3, DateTime.Now, 300m, "Coursera Subscription");

            bankProcessor.Process(t1);
            savings.ApplyTransaction(t1);

            momoProcessor.Process(t2);
            savings.ApplyTransaction(t2);

            cryptoProcessor.Process(t3);
            savings.ApplyTransaction(t3);

            Console.WriteLine($"Final Balance: {savings.Balance}");
        }
    }

    public class Program
    {
        public static void Main()
        {
            new FinanceApp().Run();
        }
    }

}
