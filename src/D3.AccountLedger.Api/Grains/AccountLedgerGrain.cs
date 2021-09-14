using System;
using System.Threading.Tasks;
using Orleans;
using Orleans.EventSourcing;

namespace D3.AccountLedger.Api.Grains
{
    public class AccountLedgerGrain : JournaledGrain<AccountLedgerState, AccountLedgerTransaction>, IAccountLedgerGrain
    {
        public async Task<TransactionTicket> AddDebit(DebitTransaction transaction)
        {
            RaiseEvent(transaction);

            await ConfirmEvents();

            return new TransactionTicket(transaction.TransactionId, transaction.Amount);
        }

        public async Task<TransactionTicket> AddCredit(CreditTransaction transaction)
        {
            RaiseEvent(transaction);

            await ConfirmEvents();

            return new TransactionTicket(transaction.TransactionId, transaction.Amount);
        }

        public Task<decimal> GetCurrentBalance()
        {
            return Task.FromResult(State.AccountBalance);
        }
    }

    public class AccountLedgerState
    {
        public decimal AccountBalance { get; private set; }

        public void Apply(DebitTransaction debitTransaction)
        {
            AccountBalance -= debitTransaction.Amount;
        }

        public void Apply(CreditTransaction creditTransaction)
        {
            AccountBalance += creditTransaction.Amount;
        }
    }

    public abstract class AccountLedgerTransaction
    {
        public decimal Amount { get; set; }
        public string TransactionId { get; set; }
        public DateTimeOffset TransactionDate { get; set; }
    }

    public interface IAccountLedgerGrain : IGrainWithStringKey
    {
        Task<TransactionTicket> AddDebit(DebitTransaction transaction);
        Task<TransactionTicket> AddCredit(CreditTransaction transaction);
        Task<decimal> GetCurrentBalance();
    }

    public class CreditTransaction : AccountLedgerTransaction
    {
        
    }

    public class DebitTransaction : AccountLedgerTransaction
    {
    }

    public class TransactionTicket
    {
        public string TransactionId { get; }
        public decimal Amount { get; }
        public long Timestamp { get; }

        public TransactionTicket(string transactionId, decimal amount)
        {
            TransactionId = transactionId;
            Amount = amount;
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        }
    }
}
