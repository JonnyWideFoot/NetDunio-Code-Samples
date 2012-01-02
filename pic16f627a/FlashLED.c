#include <pic.h>

__CONFIG(UNPROTECT & LVPDIS & BORDIS & MCLRDIS & PWRTEN & WDTDIS & INTCLK);

#ifndef _XTAL_FREQ
//  Unless already defined assume 4MHz system frequency
//  This definition is required to calibrate __delay_us() and __delay_ms()
 #define _XTAL_FREQ 4000000
#endif

void main(void)
{
	TRISB = 0;
	
	/* Reset the LEDs */
	PORTB = 0;
	
	/* Light the LEDs */
	PORTB = 0x5A;
	
	while(1)
	{
		PORTB=0XFF; 
		__delay_ms(100);//		 delay for 100 milliseconds
		PORTB=0X00;
		__delay_ms(100);//		 delay for 100 milliseconds
	}
}