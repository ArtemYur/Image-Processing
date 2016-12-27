using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics;
using System.Threading;
using Examples.Tutorial;
using System.Globalization;
using System.Diagnostics;
using System.ComponentModel.DataAnnotations;

namespace Image_Processing
{
    class A
    {
        protected internal void Func()
        {

        }
        
        [CustomAttr()]
        [Required(ErrorMessage = "You must provide not empty 'Prop' argument!")]
        public string Prop { get; set; }
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    class CustomAttr: Attribute
    {
        public CustomAttr ()
        {

        }
    }

    class Program
    {
        public static Task<string> show()
        {
            //Console.WriteLine($"Async method task: {Task.CurrentId}, " + $"thread: {Thread.CurrentThread.ManagedThreadId}");
            
            return Task.Factory.StartNew(() => 
            {
                return someMethod();
            });
        }

        static string someMethod()
        {
            //for (var i = 0; i < 99999999; i++)
            //{
            //    for (var j = 0; j < 10; j++)
            //    {
            //    }
            //}
            Thread.Sleep(2000);
            //Console.WriteLine($"Async method task: {Task.CurrentId}, " + $"thread: {Thread.CurrentThread.ManagedThreadId}");
            return $"Async method task: {Task.CurrentId}, " + $"thread: {Thread.CurrentThread.ManagedThreadId}";
            //return $"Async method task: {Task.CurrentId}, " + $"thread: {Thread.CurrentThread.ManagedThreadId}";
        }

        static async void some()
        {
            Console.WriteLine($"Async method 2 Start task: {Task.CurrentId}, " + $"thread: {Thread.CurrentThread.ManagedThreadId}");
            var task = show();

            var res = await task;

            Console.WriteLine(res);
            Console.WriteLine($"Async method 2 task: {Task.CurrentId}, " + $"thread: {Thread.CurrentThread.ManagedThreadId}");

            IsSynced = true;
        }
        
        static bool IsSynced = false;

        static void Main(string[] args)
        {
            A a = new A();

            a.Prop = "sdf";

            var attrs = a.GetType().GetProperty("Prop").GetCustomAttributes(false);
            var attr = Attribute.GetCustomAttribute(a.GetType().GetProperty("Prop"), typeof(CustomAttr)) as CustomAttr;
//            dynamic expObj = new { };
//            expObj.FirstName = "Daffy";
//            expObj.LastName = "Duck";
//            Console.WriteLine($"{expObj.FirstName} {expObj.LastName}");
//            Func<DateTime, string> GetTomorrow = today => today.AddDays(1).ToShortDateString();
//            expObj.GetTomorrowDate = GetTomorrow;
//            Console.WriteLine($"Tomorrow is {expObj.GetTomorrowDate(DateTime.Now)}");
            //expObj.Friends = new List<Person>();
            //expObj.Friends.Add(new Person() { FirstName = "Bob", LastName = "Jones" });
            //expObj.Friends.Add(new Person() { FirstName = "Robert", LastName = "Jones" });
            //expObj.Friends.Add(new Person() { FirstName = "Bobby", LastName = "Jones" });
            //foreach (Person friend in expObj.Friends)
            //{
            //    WriteLine($"{friend.FirstName} {friend.LastName}");
            //}
            //Task.Run(() =>
            //{
            //    some();
            //});

            //some();

            //Task.Factory.StartNew(() =>
            //{
            //    for (var i = 0; i < 99999999; i++)
            //    {
            //        for (var j = 0; j < 10; j++)
            //        {
            //        }
            //    }


            //    //return $"Async method task: {Task.CurrentId}, " + $"thread: {Thread.CurrentThread.ManagedThreadId}";
            //} //, TaskCreationOptions.AttachedToParent //TaskScheduler.FromCurrentSynchronizationContext() 
            //).ContinueWith((t) => 
            //{
            //    t.Wait();
            //    Console.WriteLine($"Async method task: {Task.CurrentId}, " + $"thread: {Thread.CurrentThread.ManagedThreadId}");
            //}, TaskScheduler.FromCurrentSynchronizationContext());

            //Console.WriteLine($"task: {Task.CurrentId}, " + $"thread: {Thread.CurrentThread.ManagedThreadId}");

            //while(!IsSynced)
            //{
            //    //Console.WriteLine(".");
            //}
            //var res = task.Wait();
            //show();
            //Task.Run(() =>
            //{
            //    Console.WriteLine($"Async method task: {Task.CurrentId}, " + $"thread: {Thread.CurrentThread.ManagedThreadId}");
            //});


            //var df = new DiscoverFeatures();
            //GLControl control = new GLControl(new GraphicsMode(32, 24, 8, 4), 3, 0, GraphicsContextFlags.Default);
            ////var availableDevices = df.DiscoverAvailableDevices();
            //control.MakeCurrent();
            //var list = new List<int>() { 1, 2, 3, 4 };

            //var res = list.Where(l => l == 2);

            //ImmediateModeCube.Execute();
            //T09_VBO_Dynamic.Execute();
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");

            var input = Console.ReadLine();

            switch(Convert.ToInt32(input))
            {
                case 1:
                    Lab_1.Worker.Execute();
                    break;
                case 3:
                    Image_Processing.Lab_3.Worker.Execute();
                    //Examples.Tutorial.Textures.Execute();
                    break;
                default:
                    T09_VBO_Dynamic.Execute();
                    break;
            }

            Console.ReadKey();
        }
    }
}