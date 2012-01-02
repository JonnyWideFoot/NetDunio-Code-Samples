using System;
using System.Threading;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware.NetduinoPlus;

namespace LotteryNumberPicker2
{
    /// <summary>
    /// Controls a pair of Multiplexed 7-Segment Displays
    /// </summary>
    public sealed class SevenSegmentLedController
    {
        private readonly bool[][] _pins = new[]
                                              {
                                                  new[] {true, true, true, true, true, true, false}, // 0
                                                  new[] {true, false, false, false, true, false, false}, // 1
                                                  new[] {false, true, true, true, true, false, true}, // 2
                                                  new[] {true, false, true, true, true, false, true}, // 3
                                                  new[] {true, false, false, false, true, true, true}, // 4
                                                  new[] {true, false, true, true, false, true, true}, // 5
                                                  new[] {true, true, true, true, false, true, true}, // 6
                                                  new[] {true, false, false, true, true, false, false}, // 7
                                                  new[] {true, true, true, true, true, true, true}, // 8
                                                  new[] {true, false, false, true, true, true, true}, // 9
                                              };

        private readonly OutputPort[] _outputPorts;
        private readonly OutputPort[] _controlPorts;

        private int _number;

        public SevenSegmentLedController()
        {
            _outputPorts = new OutputPort[7];
            _outputPorts[0] = new OutputPort(Pins.GPIO_PIN_D0, false);
            _outputPorts[1] = new OutputPort(Pins.GPIO_PIN_D1, false);
            _outputPorts[2] = new OutputPort(Pins.GPIO_PIN_D2, false);
            _outputPorts[3] = new OutputPort(Pins.GPIO_PIN_D3, false);
            _outputPorts[4] = new OutputPort(Pins.GPIO_PIN_D4, false);
            _outputPorts[5] = new OutputPort(Pins.GPIO_PIN_D5, false);
            _outputPorts[6] = new OutputPort(Pins.GPIO_PIN_D6, false);

            _controlPorts = new OutputPort[2];
            _controlPorts[0] = new OutputPort(Pins.GPIO_PIN_D12, false);
            _controlPorts[1] = new OutputPort(Pins.GPIO_PIN_D13, false);
        }

        public void SetNumber(int number)
        {
            if (number < 0 || number > 99)
            {
                throw new Exception();
            }
            _number = number;
        }

        public void Render()
        {
            int digitLeft = _number / 10;
            int digitRight = _number % 10;

            SetBankOff();
            if (digitLeft == 0)
            {
                SetPinsOff();
            }
            else
            {
                SetDigit(digitLeft);
            }
            SetBank(true);
            RenderSleep();

            SetBankOff();
            if (digitLeft == 0 && digitRight == 0)
            {
                SetPinsOff();
            }
            else
            {
                SetDigit(digitRight);
            }
            SetBank(false);
            RenderSleep();
        }

        private static void RenderSleep()
        {
            Thread.Sleep(3);
        }

        private void SetPinsOff()
        {
            for (int i = 0; i < 7; i++)
            {
                _outputPorts[i].Write(false);
            }
        }

        private void SetDigit(int number)
        {
            var pins = _pins[number];
            for (int i = 0; i < 7; i++)
            {
                _outputPorts[i].Write(pins[i]);
            }
        }

        private void SetBankOff()
        {
            _controlPorts[0].Write(false);
            _controlPorts[1].Write(false);
        }

        private void SetBank(bool left)
        {
            _controlPorts[0].Write(!left);
            _controlPorts[1].Write(left);
        }
    }
}