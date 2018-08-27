﻿using System;
using System.IO;
using Org.Unidal.Cat;
using System.Threading;
using CatClientTest.PerformanceTest;
using System.Collections.Generic;
using Org.Unidal.Cat.Message;
using System.Threading.Tasks;
using System.Linq;

namespace CatClientTest
{
    public class Program
    {
        static void Main()
        {
            try
            {
                SimpleTest().GetAwaiter().GetResult();
            }
            finally
            {
                //if (null != Cat.lastException)
                //{
                //    Console.WriteLine("Cat.lastException:\n" + Cat.lastException);
                //}
                Console.WriteLine("Test ends successfully. Press any key to continue");
                Console.Read();
            }
        }

        private static async Task<int> TestTaskResult()
        {
            await Task.Delay(100);

            return 10;
        }


        private static async Task SimpleTest()
        {

            Console.WriteLine("Start: " + DateTime.Now);
            Console.WriteLine($"Top ThreadId: {System.Threading.Thread.CurrentThread.ManagedThreadId}");
            ITransaction newOrderTransaction = null;

            try
            {
                newOrderTransaction = Cat.NewTransaction("SimpleTestAsync-4-" + DateTime.Now.Ticks, "NewTrainOrder");
                newOrderTransaction.AddData("I am a detailed message");
                newOrderTransaction.AddData("another message"); 
                //Cat.LogEvent("TrainNo", "123456");
                //Cat.LogError("MyException", new Exception("My Exception"));

                var tasks = Enumerable.Range(1, 2).Select(async (i) => await InvokePaymentWrap(i));

                await Task.WhenAll(tasks);

                for (int i = 100; i < 103; i++)
                    await InvokePayment(i);

                newOrderTransaction.Status = CatConstants.SUCCESS;
            }
            catch (Exception ex)
            {
                newOrderTransaction.SetStatus(ex);
            }
            finally
            {
                newOrderTransaction.Complete();
                Console.WriteLine("End: " + DateTime.Now);
            }
        }

        private static async Task InvokePaymentWrap(int i)
        {
            await InvokePayment(i);

            return;
            var forkedTran = Cat.NewForkedTransaction("remote", "InvokePaymentWrap");
            try
            {
                await InvokePayment(i);

                forkedTran.Status = CatConstants.SUCCESS;
            }
            catch (Exception ex)
            {
                forkedTran?.SetStatus(ex);
            }
            finally
            {
                forkedTran?.Complete();
            }
        }

        private static async Task InvokePayment(int i)
        {
            ITransaction paymentTransaction = null;
            try
            {
                Console.WriteLine($"InvokePayment 1 ThreadId: {System.Threading.Thread.CurrentThread.ManagedThreadId}");
                paymentTransaction = Cat.NewTransaction("NewPayment" + i, "PaymentDetail");
                paymentTransaction.Status = CatConstants.SUCCESS;
                await Task.Delay(100);
                Console.WriteLine($"InvokePayment 2 ThreadId: {System.Threading.Thread.CurrentThread.ManagedThreadId}");

                await InvokeInnerPayment();
            }
            catch (Exception ex)
            {
                paymentTransaction.SetStatus(ex);
            }
            finally
            {
                paymentTransaction.Complete();
            }
        }

        private static async Task InvokeInnerPayment()
        {
            ITransaction paymentTransaction = null;
            try
            {
                Console.WriteLine($"InnerPayment 1 ThreadId: {System.Threading.Thread.CurrentThread.ManagedThreadId}");
                paymentTransaction = Cat.NewTransaction("NewInnerPayment", "PaymentDetail");
                paymentTransaction.Status = CatConstants.SUCCESS;
                await Task.Delay(1000);
                Console.WriteLine($"InnerPayment 2 ThreadId: {System.Threading.Thread.CurrentThread.ManagedThreadId}");

            }
            catch (Exception ex)
            {
                paymentTransaction.SetStatus(ex);
            }
            finally
            {
                paymentTransaction.Complete();
            }
        }
    }
}