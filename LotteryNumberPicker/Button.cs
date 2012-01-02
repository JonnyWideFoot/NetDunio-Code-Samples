using System;
using Microsoft.SPOT.Hardware;

namespace LotteryNumberPicker2
{
    public sealed class Button
    {
        private readonly InterruptPort _port;

        public event NoParamEventHandler Pressed;
        public event NoParamEventHandler Released;

        public Button(Cpu.Pin pin)
        {
            _port = new InterruptPort(pin, true, Port.ResistorMode.Disabled, Port.InterruptMode.InterruptEdgeBoth);
            _port.OnInterrupt += port_OnInterrupt;
        }

        public bool IsPressed
        {
            get { return !_port.Read(); }
        }

        private void port_OnInterrupt(uint data1, uint data2, DateTime time)
        {
            _port.DisableInterrupt();

            if (data2 > 0)
            {
                OnPressed();
            }
            else
            {
                OnReleased();
            }

            _port.EnableInterrupt();
            _port.ClearInterrupt();
        }

        private void OnReleased()
        {
            if (Released != null)
            {
                Released();
            }
        }

        private void OnPressed()
        {
            if (Pressed != null)
            {
                Pressed();
            }
        }
    }
}