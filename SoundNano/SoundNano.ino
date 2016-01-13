
/*
Прграмма управления звуковым генератором 
Микроконтроллер Atmega328 (Arduino Nano)
Применяется в модуле проверки звуковой платы "Камертон"
Версия тестера платы "Камертон 5.0"
*/
#include <Arduino.h>
#include <AH_AD9850.h>


//Настройка звукового генератора
//#define CLK     8      // Назначение выводов генератора сигналов
//#define FQUP    9      // Назначение выводов генератора сигналов
//#define BitData 10     // Назначение выводов генератора сигналов
//#define RESET   11     // Назначение выводов генератора сигналов

#define CLK     5      // Назначение выводов генератора сигналов
#define FQUP    6      // Назначение выводов генератора сигналов
#define BitData 7     // Назначение выводов генератора сигналов
#define RESET   8     // Назначение выводов генератора сигналов


#define led13    13    // 

#define Kn1      9     // 
#define Kn2     10     // 
#define Kn3     11     // 
#define Out1    12     // 

int var = 0;
int _var = 0;
bool sound_run = true;

AH_AD9850 AD9850(CLK, FQUP, BitData, RESET);// настройка звукового генератора

int frequency_start = 500;
int frequency_max = 3500;
int frequency_step = 50;
int frequency_start2 = 100;
int frequency_max2 = 5500;
int frequency_step2 = 50;


void step_sound()
{
	  do{
			digitalWrite(Out1, HIGH);
			digitalWrite(led13, HIGH); 
			if ((Kn1 != 0) & (Kn2 != 0) & (Kn3 != 0)) var = 0;
			if ((Kn1 == 0) & (Kn2 != 0) & (Kn3 != 0)) var = 1;
			if ((Kn1 != 0) & (Kn2 == 0) & (Kn3 != 0)) var = 2;
			if ((Kn1 != 0) & (Kn2 != 0) & (Kn3 == 0)) var = 3;
			if (var != _var) sound_run = false;
			digitalWrite(Out1, HIGH);
			digitalWrite(led13, HIGH); 
 
			for (int i=frequency_start;i<= frequency_max; i=i+frequency_step)
				{
						AD9850.set_frequency(0,0,i);    //set power=UP, phase=0, i= frequency
						delay(5); 
				}
			delay(10); 
			digitalWrite(led13, LOW); 
			for (int i=frequency_max;i >  frequency_start; i=i-frequency_step)
				{
			
						AD9850.set_frequency(0,0,i);    //set power=UP, phase=0, i= frequency
						delay(5); 
				}
		}while (sound_run == true);
}
void fix_sound1()
{
	AD9850.reset();                                  //reset module
	delay(500);
	AD9850.powerDown();                              //set signal output to LOW
	delay(100);
	AD9850.set_frequency(0,0,1000);                   //set power=UP, phase=0, 1kHz frequency
	delay(500); 
	digitalWrite(Out1, LOW); 
}
void fix_sound2()
{
	AD9850.reset();                                  //reset module
	delay(500);
	AD9850.powerDown();                              //set signal output to LOW
	delay(100);
	AD9850.set_frequency(0,0,2000);                   //set power=UP, phase=0, 1kHz frequency
	delay(500); 
	digitalWrite(Out1, LOW); 
}

void step_sound2()
{
  do{
		digitalWrite(Out1, HIGH);
		digitalWrite(led13, HIGH); 
		if ((Kn1 != 0) & (Kn2 != 0) & (Kn3 != 0)) var = 0;
		if ((Kn1 == 0) & (Kn2 != 0) & (Kn3 != 0)) var = 1;
		if ((Kn1 != 0) & (Kn2 == 0) & (Kn3 != 0)) var = 2;
		if ((Kn1 != 0) & (Kn2 != 0) & (Kn3 == 0)) var = 3;
		if (var != _var) sound_run = false;

		for (int i=frequency_start2;i<= frequency_max2; i=i+frequency_step2)
				{
						AD9850.set_frequency(0,0,i);    //set power=UP, phase=0, i= frequency
						delay(5); 
				}
		delay(10); 
		digitalWrite(led13, LOW); 
		for (int i=frequency_max2;i >  frequency_start2; i=i-frequency_step2)
				{
						AD9850.set_frequency(0,0,i);    //set power=UP, phase=0, i= frequency
						delay(5); 
				}
   }while (sound_run == true);
}

void menu()
{
	//if ((Kn1 != 0) & (Kn2 != 0) & (Kn3 != 0)) var = 0;
	//if ((Kn1 == 0) & (Kn2 != 0) & (Kn3 != 0)) var = 1;
	//if ((Kn1 != 0) & (Kn2 == 0) & (Kn3 != 0)) var = 2;
	//if ((Kn1 != 0) & (Kn2 != 0) & (Kn3 == 0)) var = 3;
//	if (Kn1 == 0)  var = 1;
	//if (Kn2 != 0)  var = 2;
	//if (Kn3 != 0)  var = 3;
	//if ((Kn1 != 0) & (Kn2 != 0) & (Kn3 == 0)) var = 3;


//	Serial.println(var);
  if (Kn1 == 0)
	{
		//_var = var;
		Serial.println("ok");
		/*
		switch (_var) 
		{
		case 1:
			step_sound();
			break;
		case 2:
			fix_sound1();
			break;
		case 3:
			step_sound2();
			break;
		default: 
		step_sound();
//		fix_sound2();
		}
		*/
	}
	else
	{
	//var = 0;
	
	}

}



void setup()
{
  //reset device
  AD9850.reset();                   //reset module
  delay(1000);
  AD9850.powerDown();               //set signal output to LOW
  pinMode(led13, OUTPUT); 
  pinMode(Kn1, INPUT);
  pinMode(Kn2, INPUT);
  pinMode(Kn3, INPUT);
  pinMode(Out1,OUTPUT);

  digitalWrite(Kn1, HIGH);
  digitalWrite(Kn2, HIGH);
  digitalWrite(Kn3, HIGH);
  digitalWrite(Out1, HIGH);
  // initialize serial communication
  AD9850.set_frequency(0,0,2000);    //set power=UP, phase=0, 2kHz frequency 
  Serial.begin(9600);
  Serial.println(_var);
}

void loop()
{
	//step_sound();
	menu();
    delay(100);

}
