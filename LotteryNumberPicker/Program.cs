using System;
using System.Threading;
using Microsoft.SPOT;
using SecretLabs.NETMF.Hardware.NetduinoPlus;

namespace LotteryNumberPicker2
{
    public class Program
    {
        private const int MaxLotteryNumber = 49;
        private static readonly SevenSegmentLedController Controller = new SevenSegmentLedController();
        private static readonly Random Random = new Random();

        public static void Main()
        {
            // A dedicated thread to render the multi-plexed LEDs
            var renderThread = new Thread(RenderWorker)
                                   {
                                       Priority = ThreadPriority.Lowest
                                   };
            renderThread.Start();

            // Prove we can render 0 --> 99
            CountCycle();

            // Set to 0 (Blanks the displays)
            Controller.SetNumber(0);

            // Configure the user-interface button
            var button = new Button(Pins.GPIO_PIN_D8);
            button.Pressed += OnButtonPressed;
            button.Released += OnButtonReleased;

            // Wait forever...
            Thread.Sleep(Timeout.Infinite);
        }

        private static void RenderWorker()
        {
            while (true)
                Controller.Render();
        }

        private static void OnButtonPressed()
        {
            int selectedNumber = Random.Next(MaxLotteryNumber);
            Debug.Print("OnButtonPressed: " + selectedNumber);
            Controller.SetNumber(selectedNumber);
        }

        private static void OnButtonReleased()
        {
            Debug.Print("OnButtonReleased");
        }

        private static void CountCycle()
        {
            for (int i = 0; i < 100; i++)
            {
                Controller.SetNumber(i);
                Thread.Sleep(30);
            }
        }
    }
}