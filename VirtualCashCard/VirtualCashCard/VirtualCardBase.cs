using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace VirtualCashCard
{

    //Written By: Puneet Mahajan, 07886341389

    /// <summary>
    /// Base Virtualcard class with base functionality
    /// </summary>
    public abstract class VirtualCardBase
    {
        /// <summary>
        /// Semaphore to synchronize multiple threads updating balance
        /// </summary>
        private static SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1, 1);

        public decimal Balance { get; private set; }

        public int Pin { get; private set; }

        /// <summary>
        /// Set Balance in threadsafe way
        /// </summary>
        private async Task SetBalance(decimal balance)
        {
            await semaphoreSlim.WaitAsync();
            try
            {
                Balance = balance;
            }
            finally
            {
                semaphoreSlim.Release();
            }
        }

        protected VirtualCardBase(int pin, decimal balance)
        {
            SetBalance(balance);
            Pin = pin;
        }

        /// <summary>
        /// Validate Pin
        /// </summary>
        private bool IsValidPin(int pin)
        {
            return Pin == pin;
        }


        /// <summary>
        /// Withdraw money asynchronously after validating pin, if pin does not match then throw InvalidPinException
        /// </summary>
        public async Task WithdrawMoney(int pin, decimal amount)
        {
            if (IsValidPin(pin))
            {
                await SetBalance(Balance - amount);
            }
            else
            {
                throw new InvalidPinException("Invalid Pin");
            }
        }

        /// <summary>
        /// Topup balance asynchronously
        /// </summary>
        public async Task AddMoney(decimal amount)
        {
            await SetBalance(Balance + amount);
        }
    }
}

