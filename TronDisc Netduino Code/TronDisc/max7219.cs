using System;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware;
using SecretLabs.NETMF.Hardware.Netduino;
using System.Collections;

namespace TronDisc
{
    public class max7219
    {
        // Pin ports for spi
        private OutputPort loadPin;
        private OutputPort dataPin;
        private OutputPort clkPin;

        // Command reference
        private byte max7219_reg_noop           = 0x00;
        private byte max7219_reg_digit0         = 0x01;
        private byte max7219_reg_digit1         = 0x02;
        private byte max7219_reg_digit2         = 0x03;
        private byte max7219_reg_digit3         = 0x04;
        private byte max7219_reg_digit4         = 0x05;
        private byte max7219_reg_digit5         = 0x06;
        private byte max7219_reg_digit6         = 0x07;
        private byte max7219_reg_digit7         = 0x08;
        private byte max7219_reg_decodeMode     = 0x09;
        private byte max7219_reg_intensity      = 0x0a;
        private byte max7219_reg_scanLimit      = 0x0b;
        private byte max7219_reg_shutdown       = 0x0c;
        private byte max7219_reg_displayTest    = 0x0f;

        // Constructor, pass pin definitions
        public max7219(OutputPort in_dataPin, OutputPort in_clockPin, OutputPort in_loadPin)
        {
            // Assign local port pins to ports passed from constructor
            dataPin = in_dataPin;
            clkPin = in_clockPin;
            loadPin = in_loadPin;

            maxSingle(max7219_reg_scanLimit, 0x07);     // set scan limit
            maxSingle(max7219_reg_decodeMode, 0x00);    // using an led matrix mode (not digits)
            maxSingle(max7219_reg_shutdown, 0x01);      // not in shutdown mode
            maxSingle(max7219_reg_displayTest, 0x00);   // no display test
            maxSingle(max7219_reg_intensity, 0x0f);     // set max intensity  (range 00-0f)
        }

        // Transmits 1 byte over SPI, bitbang method
        public void putByte(byte data)
        {
            byte i = 8;
            int mask;

            while (i > 0)
            {
                mask = (1 << i - 1);
                clkPin.Write(false);
                if (((int)data & mask) == 0)
                    dataPin.Write(false);
                else
                    dataPin.Write(true);
                clkPin.Write(true);
                --i;
            }
        }

        // Sends 1 Command / Data pair to a single driver chip
        public void maxSingle(byte reg, byte col)
        {
            // LOAD low
            loadPin.Write(false);

            // Transmit Register
            putByte(reg);
            // Transmit Column
            putByte(col);

            // LOAD high latches data sent
            loadPin.Write(true);
        }
    }
}
