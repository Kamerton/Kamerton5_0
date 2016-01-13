
/*
�������� ���������� �������� ����������� 
��������������� Atmega328 (Arduino Nano)
����������� � ������ �������� �������� ����� "��������"
������ ������� ����� "�������� 5.0"
*/
#include <Arduino.h>
#include <AH_AD9850.h>


//��������� ��������� ����������
//#define CLK     8      // ���������� ������� ���������� ��������
//#define FQUP    9      // ���������� ������� ���������� ��������
//#define BitData 10     // ���������� ������� ���������� ��������
//#define RESET   11     // ���������� ������� ���������� ��������

#define CLK     5      // ���������� ������� ���������� ��������
#define FQUP    6      // ���������� ������� ���������� ��������
#define BitData 7     // ���������� ������� ���������� ��������
#define RESET   8     // ���������� ������� ���������� ��������


#define led13    13    // 

#define Kn1      9     // 
#define Kn2     10     // 
#define Kn3     11     // 
#define Out1    12     // 

int var = 0;


AH_AD9850 AD9850(CLK, FQUP, BitData, RESET);// ��������� ��������� ����������

int frequency_start = 500;
int frequency_max = 3500;
int frequency_step = 50;
int frequency_start2 = 100;
int frequency_max2 = 5500;
int frequency_step2 = 50;


void step_sound()
{
  digitalWrite(Out1, HIGH);
  digitalWrite(led13, HIGH); 
   //set_frequency(boolean PowerDown, byte Phase, double Freq); 
  // AD9850.set_frequency(0,0,1000);    //set power=UP, phase=0, 1kHz frequency
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
}
void fix_sound1()
{
	AD9850.reset();                                  //reset module
	delay(500);
	AD9850.powerDown();                              //set signal output to LOW
	delay(100);
	AD9850.set_frequency(0,0,1000);                   //set power=UP, phase=0, 1kHz frequency
	delay(1000); 
    digitalWrite(Out1, LOW); 
}

void fix_sound2()
{
  digitalWrite(Out1, HIGH);
  digitalWrite(led13, HIGH); 
   //set_frequency(boolean PowerDown, byte Phase, double Freq); 
  // AD9850.set_frequency(0,0,1000);    //set power=UP, phase=0, 1kHz frequency
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
}

void menu()
{
  if (Kn1 == 0) var = 1;
  if (Kn2 == 0) var = 2;
  if (Kn3 == 0) var = 3;

  switch (var) {
	case 1:
	  step_sound();
	  break;
	case 2:
	  fix_sound1();
	  break;
	case 3:
	  fix_sound2();
	  break;
	default: 
	step_sound();
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

  // initialize serial communication
  AD9850.set_frequency(0,0,2000);    //set power=UP, phase=0, 2kHz frequency 
  Serial.begin(9600);
}

void loop()
{
	//step_sound();
	menu();
 //   delay(10);

}
