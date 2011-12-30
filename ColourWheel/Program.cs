using System.Threading;
using SecretLabs.NETMF.Hardware;
using SecretLabs.NETMF.Hardware.NetduinoPlus;

namespace ColourWheel
{
    public class Program
    {
        public static void Main()
        {
            var ledR = new PWM(Pins.GPIO_PIN_D9);
            var ledG = new PWM(Pins.GPIO_PIN_D6);
            var ledB = new PWM(Pins.GPIO_PIN_D5);

            while (true)
            {
                for (double i = 0; i < 1; i += 0.003)
                {
                    var c = ColorRGB.Hsl2Rgb(i, 1.0, 0.5);

                    ledR.SetPulse(255, c.R);
                    ledG.SetPulse(255, c.G);
                    ledB.SetPulse(255, c.B);

                    Thread.Sleep(25);
                }
            }
        }
    }
}