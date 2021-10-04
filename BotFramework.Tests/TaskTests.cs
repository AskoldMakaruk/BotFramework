using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using BotFramework.Middleware;
using FluentAssertions;
using NUnit.Framework;

namespace BotFramework.Tests
{
    // this 
    public class TaskTests
    {
        private static Task _task;

        [Test]
        public async Task Test()
        {
            Func<Task> task1 = async () =>
            {
                await Task.Delay(100);
                Console.WriteLine("Delay");
                PrintTaskState();
            };

            Func<Task> task2 = async () =>
            {
                await Task.Delay(100);
                Console.WriteLine("Delay");
                PrintTaskState();
            };
            _task = Task.Run(async () =>
            {
                PrintTaskState();
                var someState = 0;
                await task1.Invoke();


                await task2.Invoke();
                someState++;
                Console.WriteLine("Done");
            });

            await _task;
        }

        private static int counter = 0;


        private static void ResetTaskState()
        {
            counter++;
            if (counter == 5)
            {
                throw new Exception();
            }

            var a = _task.GetPrivateValue<Task>(5);
            var b = a.GetPrivateValue(1);
            b.SetPublicValue(0, 0);
        }

        private static void PrintTaskState()
        {
            var a = _task.GetPrivateValue<Task>(5);
            var b = a.GetPrivateValue(1);
            Console.WriteLine($"Task state is: {b.GetPrivateValue(0)}");
        }
    }

    public static class ObjectHelpers
    {
        public static object GetPrivateValue<T>(this object o, int index)
        {
            return typeof(T).GetFields(BindingFlags.Instance | BindingFlags.NonPublic)[index].GetValue(o);
        }

        public static object GetPrivateValue(this object o, int index)
        {
            return o.GetType().GetRuntimeFields().ToArray()[index].GetValue(o);
        }

        public static void SetPublicValue(this object o, int index, object value)
        {
            o.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public)[index].SetValue(o, value);
        }
    }
}