using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VirtualCashCard;

namespace VirtualCashCard.Tests
{
    [TestClass]
    public class VirtualCardTests
    {
        [TestMethod]
        public void VirtualCard_Contructor_Sets_Pin()
        {
            //arrange
            var pin = 1234;
            var vc = new VirtualCard(pin, 0);

            //act
            var result = vc.Pin;

            //assert
            Assert.AreEqual(pin, result);
        }

        [TestMethod]
        public void VirtualCard_Contructor_Sets_Balance()
        {
            //arrange
            var balance = 100.10M;
            var vc = new VirtualCard(0, balance);

            //act
            var result = vc.Balance;

            //assert
            Assert.AreEqual(balance, result);
        }

        [TestMethod]
        public void VirtualCard_Can_TopBalance()
        {
            //arrange
            var balance = 100M;
            var topup_amt = 10M;
            var vc = new VirtualCard(0, balance);

            //act
            vc.AddMoney(topup_amt).Wait(new TimeSpan(0, 0, 0, 1));
            var result = vc.Balance;

            //assert
            Assert.AreEqual(balance + topup_amt, result);
        }

        [TestMethod]
        public void VirtualCard_Can_WithdrawMoney()
        {
            //arrange
            var pin = 1234;
            var balance = 100M;
            var withdraw_amt = 10M;
            var vc = new VirtualCard(pin, balance);

            //act
            vc.WithdrawMoney(pin, withdraw_amt).Wait(new TimeSpan(0, 0, 0, 1));
            var result = vc.Balance;

            //assert
            Assert.AreEqual(balance - withdraw_amt, result);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidPinException))]
        public void VirtualCard_WithdrawMoney_Throws_InvalidPinException_When_Pin_Does_Not_Match()
        {
            //arrange
            var pin = 1234;
            var invalid_pin = 0000;
            var balance = 100M;
            var withdraw_amt = 10M;
            var vc = new VirtualCard(pin, balance);

            //act
            try
            {
                vc.WithdrawMoney(invalid_pin, withdraw_amt).Wait(new TimeSpan(0, 0, 0, 1));
            }
            catch (Exception e)
            {
                throw e.InnerException;
            }

            //assert
            //ASSERT EXCEPTION AS PIN DOES NOT MATCH
        }

        [TestMethod]
        public void VirtualCard_WithdrawMoney_Updates_Balance_When_Pin_Is_Valid()
        {
            //arrange
            var pin = 1234;
            var valid_pin = 1234;
            var balance = 100M;
            var withdraw_amt = 10M;
            var vc = new VirtualCard(pin, balance);

            //act
            vc.WithdrawMoney(valid_pin, withdraw_amt).Wait(new TimeSpan(0, 0, 0, 1));
            var result = vc.Balance;

            //assert
            Assert.AreEqual(90, result);
        }

        [TestMethod]
        public void VirtualCard_Updates_Balance_In_ThreadSafe()
        {
            //arrange
            var pin = 1234;
            var balance = 100M;
            var vc = new VirtualCard(pin, balance);

            var taskList = new List<Task>();

            //act
            taskList.Add(Task.Run(async () =>
            {
                await Task.Delay(99);
                await vc.WithdrawMoney(pin, 20);
            }));

            taskList.Add(Task.Run(async () =>
            {
                await Task.Delay(150);
                await vc.WithdrawMoney(pin, 10);
            }));

            taskList.Add(Task.Run(async () =>
            {
                await Task.Delay(33);
                await vc.AddMoney(10);
            }));

            taskList.Add(Task.Run(async () =>
            {
                await Task.Delay(77);
                await vc.AddMoney(10);
            }));

            Task.WaitAll(taskList.ToArray());
            var result = vc.Balance;

            //assert
            Assert.AreEqual(90, result);
        }

    }
}
