//PIN for trigger 
int ledPin = 13; 
int buzzPin = 12;
//for incoming byte data
byte Bdata;

void setup() {
  // put your setup code here, to run once:
  pinMode(ledPin, OUTPUT);   
  pinMode(buzzPin, OUTPUT);   
  //Begin serial on baudrate 9600   
  //Refer to system for the assigned COM port number   
  Serial.begin(9600); 
}

void loop() {
  // put your main code here, to run repeatedly:
  //Check serial availability   
  if (Serial.available() > 0) {       
    //Read initial incoming data in Byte
    Bdata = Serial.read();
    if (Bdata == 48){
      digitalWrite(ledPin, 0);       
      digitalWrite(buzzPin, 0);
    }else if (Bdata == 49){
      digitalWrite(ledPin, 1);       
      digitalWrite(buzzPin, 1);
    } 
  }else{
    digitalWrite(ledPin, 1);       
    digitalWrite(buzzPin, 1);
  }
  delay(500);
}

