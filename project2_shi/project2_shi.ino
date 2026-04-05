unsigned long prevTimeGate = 0;
unsigned long prevTimeMerryGoRound = 0;
unsigned long prevTimeFerrisWheel = 0;
unsigned long prevTimeDartBoard = 0;
unsigned long prevTimeBannerMotors = 0;

byte oddPorts = 0b10101010;
byte evenPorts = 0b01010101;

int baudRate = 9600;
int pinPortConnectionLED = 53;
int mode = 0;

const int gateOutsidePIN = 41;  //check cars coming in, let them in if the gate is unlocked and at least one parking space is available (if parking space is 111 then no cars can come in (gate is always close from the outside) but cars can always come out)
const int gateEntranceLightsPIN = 43;
const int gateInsidePIN = 39;  //check cars from inside when leaving the park, it will let cars out except when it is locked.
const int gateSensorObstacleAbsent = 1;
const int gateSensorObstaclePresent = 0;
int gateInterval = 10;       //ms up the steps
int gateSteps = 4096 / 4;    //90 degrees, open gate 90 degrees
int gateCurrentSteps = 0;    //track the step
int gateState = 0;           //0 locked, 1 for unlocked, by default, start at locked state
int gateOpenCloseState = 0;  //another state for actually telling arduino to open or close the gate.
int gateDetectInState = 1;   //default: senses nothing
int gateDetectOutState = 1;  //default: senses nothing
bool gateUnlocked = false;   //for street light logic


const int bannerMicrostepSelectPIN1 = 31;
const int bannerMicrostepSelectPIN2 = 33;
const int bannerMicrostepSelectPIN3 = 35;
const int bannerMotorStepPin = 50;
const int bannerMotorDirPin = 52;
const int bannerEnableMotorPin = 37;
const int bannerNumRotations = 6;
int bannerOnOffState = 0;   //turn on banner 1, off 0
int bannerSteps = 200 * 4;  //QUARTER Steps
int bannerMotorTURN_ON = LOW;
int bannerMotorTURN_OFF = HIGH;
int bannerCurrentSteps = 0;
int bannerInterval = 2;  //Speed
unsigned long bannerCurrentInterval = 0;
bool bannerMotorStepValue = true;
bool bannerChangeDir = false;  //change direction of motors

//when banner is off, it needs to remember the previous count
//also when gate is locked, before turning off banner, return to default state, this ensure that it won't repeat the default movement or it may keep moving the banner

const int merryGoRoundLED_ON = 38;
const int merryGoRoundMaxInterval = 13;              //13 ms
const int merryGoRoundMinInterval = 3;               //3 ms
const int merryGoRoundSteps = 4096 - 1;              //full 360
int merryGoRoundInterval = merryGoRoundMaxInterval;  //ms up the steps
int merryGoRoundState = 0;                           //0 off, 1 on, 2 slow, 3 fast, 4 faster, 5 reverse direction.
int merryGoRoundCurrentSteps = 0;
bool merryGoRoundReverseDir = false;  //0 clockwise, 1 other way

const int ferrisWheelLED_ON = 29;
const int ferrisWheelMaxInterval = 13;             //13 ms
const int ferrisWheelMinInterval = 3;              //3 ms
const int ferrisWheelSteps = 4096 - 1;             //full 360
int ferrisWheelInterval = ferrisWheelMaxInterval;  // start at slow speed
int ferrisWheelState = 0;                          //0 off, 1 on, 2 lower speed, 3 increase speed, 4 reverse direction
int ferrisWheelCurrentSteps = 0;
bool ferrisWheelReverseDir = false;  //0 clock wise, 1 other way

const int dartBoardPWM_PIN = 2;  //using DC motor for Dart Board since I'm out of stepper motor lol.
const int dartBoardLED_PIN = 47;
const int dartBoardIN1 = 51;
const int dartBoardIN2 = 49;
const int dartBoardDutyCycle3 = 255;             //100%
const int dartBoardDutyCycle2 = 255 * 80 / 100;  //70%
const int dartBoardDutyCycle1 = 255 * 60 / 100;  //40%
const int dartBoardDutyCycle0 = 255 * 40 / 100;  //10%
int dartBoardState = 0;                          //0 off, 1 on, 2 lower speed, 3 increase speed, 4 reverse direction
int dartBoardDutyCycle[] = { dartBoardDutyCycle0, dartBoardDutyCycle1, dartBoardDutyCycle2, dartBoardDutyCycle3 };
int dartBoardDutyCycleIndex = 0;
int dartBoardCurrentDutyCycle = 0;
bool dartBoardReverseDir = false;  //0 clock wise, 1 other way

const int parkingPIN1 = 23;
const int parkingPIN2 = 25;
const int parkingPIN3 = 27;
int parkingSensor = 0;
char parkingState[4] = { 0 };

const int streetLightLED_PIN = 45;

//for a half cycle of stepper motor
//I will use port A and port C, that's a total of 3 stepper motors
//and one dc motor
unsigned char oddPortsClockwise[] = { 0b00001000,    // PA3
                                      0b00001010,    // PA3 + PA1
                                      0b00000010,    // PA1
                                      0b10000010,    // PA7 + PA1
                                      0b10000000,    // PA7
                                      0b10100000,    // PA7 + PA5
                                      0b00100000,    // PA5
                                      0b00101000 };  // PA5 + PA3
unsigned char oddPortsCounterClockwise[] = { 0b00101000,
                                             0b00100000,
                                             0b10100000,
                                             0b10000000,
                                             0b10000010,
                                             0b00000010,
                                             0b00001010,
                                             0b00001000 };

// unsigned char evenPortsClockwise[] = { 0b00000101,
//                                        0b00000001,
//                                        0b00010100,
//                                        0b00000100,
//                                        0b01010000,
//                                        0b00010000,
//                                        0b01000001,
//                                        0b01000000 };
// unsigned char evenPortsCounterClockwise[] = { 0b01000000,
//                                               0b01000001,
//                                               0b00010000,
//                                               0b01010000,
//                                               0b00000100,
//                                               0b00010100,
//                                               0b00000001,
//                                               0b00000101 };
unsigned char evenPortsClockwise[] = { 0b00000001,    // Bit 0
                                       0b00000101,    // Bit 0 + 2
                                       0b00000100,    // Bit 2
                                       0b00010100,    // Bit 2 + 4
                                       0b00010000,    // Bit 4
                                       0b01010000,    // Bit 4 + 6
                                       0b01000000,    // Bit 6
                                       0b01000001 };  // Bit 6 + 0

unsigned char evenPortsCounterClockwise[] = { 0b01000000,
                                              0b01010000,
                                              0b00010000,
                                              0b00010100,
                                              0b00000100,
                                              0b00000101,
                                              0b00000001,
                                              0b01000001 };


void setup() {
  // put your setup code here, to run once:
  Serial.begin(baudRate);

  pinMode(pinPortConnectionLED, OUTPUT);  //When port connection is establish turn a green LED on.
  pinMode(merryGoRoundLED_ON, OUTPUT);
  pinMode(ferrisWheelLED_ON, OUTPUT);

  pinMode(gateOutsidePIN, INPUT);
  pinMode(gateInsidePIN, INPUT);
  pinMode(gateEntranceLightsPIN, OUTPUT);

  pinMode(bannerMicrostepSelectPIN1, OUTPUT);
  pinMode(bannerMicrostepSelectPIN2, OUTPUT);
  pinMode(bannerMicrostepSelectPIN3, OUTPUT);
  pinMode(bannerEnableMotorPin, OUTPUT);
  pinMode(bannerMotorStepPin, OUTPUT);
  pinMode(bannerMotorDirPin, OUTPUT);

  pinMode(dartBoardIN1, OUTPUT);
  pinMode(dartBoardIN2, OUTPUT);
  pinMode(dartBoardLED_PIN, OUTPUT);

  pinMode(parkingPIN1, INPUT);
  pinMode(parkingPIN2, INPUT);
  pinMode(parkingPIN3, INPUT);

  pinMode(streetLightLED_PIN, OUTPUT);

  digitalWrite(bannerMicrostepSelectPIN1, LOW);  //quarter step
  digitalWrite(bannerMicrostepSelectPIN2, HIGH);
  digitalWrite(bannerMicrostepSelectPIN3, LOW);
  digitalWrite(bannerMotorDirPin, LOW);   //start clockwise
  digitalWrite(streetLightLED_PIN, LOW);  //make sure it's off at the start of the program lol

  dartBoardCurrentDutyCycle = dartBoardDutyCycle0;  //start dart bord slow
  bannerCurrentInterval = bannerInterval;
  prevTimeGate = millis();
  prevTimeMerryGoRound = millis();
  prevTimeFerrisWheel = millis();
  prevTimeDartBoard = millis();
  prevTimeBannerMotors = millis();
  DDRL |= oddPorts;
}

void loop() {

  // put your main code here, to run repeatedly:
  unsigned long currentTime = millis();

  if (Serial.available()) mode = Serial.read();
  //Controller logic
  switch (mode) {
      //******************************PORT STATES***********************************
    case '0':                                    //port is connected
      digitalWrite(pinPortConnectionLED, HIGH);  //turn LED on
      mode = 0;
      break;
    case '1':                                   //port is disconnected
      digitalWrite(pinPortConnectionLED, LOW);  //turn LED off
      mode = 0;
      break;

      //******************************GATE STATES***********************************

    case '2':                //Unlock gate
      bannerOnOffState = 2;  //turn on the banner motors
      gateState = 1;         //gate is now Unlocked, automatically open for cars approaching if there is at least 1 parking space open, else if it is full, it will only let cars inside to go out but not let car outside to go in
                             // DDRL |= oddPorts;      //check parking spaces here
      digitalWrite(gateEntranceLightsPIN, HIGH);
      digitalWrite(streetLightLED_PIN, HIGH);  //turn on street lights
      gateUnlocked = true;
      mode = 0;
      break;
    case '3':                //Lock gate
      bannerOnOffState = 1;  //turn off the banner motors
      gateState = 0;         //Lock gate
      //DDRL &= ~oddPorts;
      digitalWrite(gateEntranceLightsPIN, LOW);
      digitalWrite(streetLightLED_PIN, LOW);  //turn off street lights
      gateUnlocked = false;                   //ok,
      mode = 0;
      break;

      //******************************CAROUSEL STATES********************************

    case '4':                 //speed up Carousel
      merryGoRoundState = 2;  //carousel's speed up state
      mode = 0;

      break;
    case '5':                 //Slow down carousel
      merryGoRoundState = 3;  //carousel's slow down state
      mode = 0;

      break;
    case '6':  //turn on carousel
      merryGoRoundState = 1;
      DDRA |= evenPorts;
      mode = 0;
      break;
    case '7':  //turn of carousel
      merryGoRoundState = 0;
      DDRA &= ~evenPorts;
      mode = 0;
      break;
    case '8':                 //change direction
      merryGoRoundState = 4;  //change direction state
      mode = 0;
      break;

      //******************************FERRIS WHEEL STATES*****************************
    case '9':  // turn on ferris wheel
      ferrisWheelState = 1;
      DDRC |= oddPorts;
      mode = 0;
      break;
    case 'a':  //turn off ferris wheel
      ferrisWheelState = 0;
      DDRC &= ~oddPorts;
      mode = 0;
      break;
    case 'b':                //change direction
      ferrisWheelState = 4;  //change direction state
      mode = 0;
      break;
    case 'c':  //increase speed, 3 presets
      ferrisWheelState = 3;
      mode = 0;
      break;
    case 'd':  //decrease speed
      ferrisWheelState = 2;
      mode = 0;
      break;
      //******************************DART BOARD STATES*******************************

    case 'e':              //Dart board on 0 off, 1 on, 2 lower speed, 3 increase speed, 4 reverse direction
      dartBoardState = 1;  //turn on dart board
      digitalWrite(dartBoardLED_PIN, HIGH);
      mode = 0;
      break;
    case 'f':              //Dart board off
      dartBoardState = 0;  //turn off dart board
      digitalWrite(dartBoardLED_PIN, LOW);
      mode = 0;
      break;
    case 'g':  //change direction
      dartBoardReverseDir = !dartBoardReverseDir;
      mode = 0;
      break;
    case 'h':  //increase speed
      dartBoardState = 3;
      mode = 0;
      break;
    case 'i':  //decrease speed
      dartBoardState = 2;
      mode = 0;
      break;

      //******************************PARKING LOGIC*************************************
    case 'j':  //check the status of the parking spaces, are there cars?
      parkingSensor = digitalRead(parkingPIN1);
      parkingState[0] = parkingSensor + '0';  //create the string, 1 for available, 0 for taken
      parkingSensor = digitalRead(parkingPIN2);
      parkingState[1] = parkingSensor + '0';
      parkingSensor = digitalRead(parkingPIN3);
      parkingState[2] = parkingSensor + '0';
      //if parking space full and gate close -> open gate
      parkingState[3] = '\0';

      Serial.write(parkingState);  //this tells C# GUI what the color of the label would be.
      Serial.write('\n');
      mode = 0;
      break;

      //******************************STREET LIGHTS STATES*******************************

    case 'k':  //turn on street lights, will use breakers for this
      if (gateUnlocked) digitalWrite(streetLightLED_PIN, HIGH);
      mode = 0;
      break;
    case 'l':  //turn off street lights
      if (gateUnlocked) digitalWrite(streetLightLED_PIN, LOW);
      mode = 0;
      break;
    default:
      mode = 0;
      break;
  }

  //Gate logic part one
  switch (gateState) {         //gateState is either locked or unlocked
    case 0:                    //locked
      gateOpenCloseState = 2;  //close the gate not matter if the sensor detects anything.
      break;

    case 1:  //gate is unlocked, let cars outside to come in if there's at least one parking spot available
      // else, only open for cars in the inside wanting to leave.
      gateDetectInState = digitalRead(gateInsidePIN);    //check cars leaving the park!
      gateDetectOutState = digitalRead(gateOutsidePIN);  //check cars entering the park, remember, if parksing space is not available, they cannot come in!

      if (gateDetectInState == gateSensorObstaclePresent && gateDetectOutState == gateSensorObstaclePresent) {
        gateOpenCloseState = 1;  //don't care since both ways have cars
      } else if (gateDetectInState == gateSensorObstaclePresent && gateDetectOutState == gateSensorObstacleAbsent) {
        gateOpenCloseState = 1;  //always let inside cars out.
      } else if (gateDetectInState == gateSensorObstacleAbsent && gateDetectOutState == gateSensorObstaclePresent) {
        if (strcmp(parkingState, "000") == 0) {  //if occupied don't open!
          gateOpenCloseState = 2;                //close
        } else {
          gateOpenCloseState = 1;  //always open for at least one parking space available
        }
      } else {                   //11
        gateOpenCloseState = 2;  //close it since there's nothing there.
      }

      break;
  }
  //Gate logic part two
  switch (gateOpenCloseState) {
    case 0:
      PORTL = PORTL & evenPorts;  //Close gate
      break;
    case 1:  //open Gate
      if (currentTime - prevTimeGate >= gateInterval) {
        //OPEN GATE
        PORTL = (PORTL & evenPorts) | oddPortsClockwise[gateCurrentSteps % 8];

        if (gateCurrentSteps >= gateSteps) {
          gateOpenCloseState = 0;  //gate has stopped
          break;                   //go out of scope to preseve gateCurrentSteps
        }
        gateCurrentSteps++;
        prevTimeGate = currentTime;
      }
      break;
    case 2:  //close gate
      if (currentTime - prevTimeGate >= gateInterval) {
        prevTimeGate = currentTime;

        PORTL = (PORTL & evenPorts) | oddPortsClockwise[gateCurrentSteps % 8];  //this will count backwards, so I kept the oddPortClockwise things

        if (gateCurrentSteps > 0) {
          gateCurrentSteps--;  //count backwards, because I want the previous position

          break;
        }  //need to go back to previous position
        gateOpenCloseState = 0;
      }
      break;
  }
  //Banner logic
  switch (bannerOnOffState) {
    case 0:                                                     //default, at the start of the program, make sure it is off.
      digitalWrite(bannerEnableMotorPin, bannerMotorTURN_OFF);  //Turn off stepper motors
      break;

    case 1:  //turn off both stepper motor

      //before we turn on off, make sure it finished its cycle, why? well if the memory gets
      //wiped, it will reset the count, so the banner will get destroyed, so for safety, ensure
      //that it's where it started after sending a command from C#
      if (bannerCurrentSteps == 0 && bannerChangeDir == false) {  //if it's at zero and banner's direction is at initial, then just turn off
        digitalWrite(bannerEnableMotorPin, bannerMotorTURN_OFF);  //Turn off stepper motors
        bannerOnOffState = 0;
        break;
      }
      digitalWrite(bannerEnableMotorPin, bannerMotorTURN_ON);  //Turn on stepper motors

      if (currentTime - prevTimeBannerMotors >= bannerInterval) {

        prevTimeBannerMotors = currentTime;
        if (bannerChangeDir == false) {  //this is at the start of the spinning, since it senses that it's close to initial pos, it will go back there.
          bannerChangeDir = true;        //switch direction immediatley, to the initial point
          digitalWrite(bannerMotorDirPin, bannerChangeDir);
          bannerCurrentSteps = (bannerSteps * bannerNumRotations) - bannerCurrentSteps;  //need to reach zero which is the initial position.
        }
        //continue with code:
        bannerMotorStepValue = !bannerMotorStepValue;
        digitalWrite(bannerMotorStepPin, bannerMotorStepValue);
        if (bannerMotorStepValue == HIGH) bannerCurrentSteps++;

        if (bannerCurrentSteps >= (bannerSteps * bannerNumRotations)) {  //after n rotations
          bannerCurrentSteps = 0;
          bannerChangeDir = false;  //reset direction
          digitalWrite(bannerMotorDirPin, LOW);
          digitalWrite(bannerEnableMotorPin, bannerMotorTURN_OFF);  //Turn off stepper motors
          bannerOnOffState = 0;
        }
      }

      break;

    case 2:                                                    //turn on both Stepper Motors
      digitalWrite(bannerEnableMotorPin, bannerMotorTURN_ON);  //Turn off stepper motors

      if (currentTime - prevTimeBannerMotors >= bannerInterval) {

        prevTimeBannerMotors = currentTime;
        bannerMotorStepValue = !bannerMotorStepValue;
        digitalWrite(bannerMotorStepPin, bannerMotorStepValue);
        if (bannerMotorStepValue == HIGH) bannerCurrentSteps++;

        if (bannerCurrentSteps >= (bannerSteps * bannerNumRotations)) {  //after 3 rotations
          bannerCurrentSteps = 0;
          bannerChangeDir = !bannerChangeDir;
          digitalWrite(bannerMotorDirPin, bannerChangeDir ? HIGH : LOW);
        }
      }
      break;
  }

  //Carousel logic
  switch (merryGoRoundState) {
    case 0:  //turn off
      digitalWrite(merryGoRoundLED_ON, LOW);

      PORTA = PORTA & oddPorts;  //bitwise and with the odd ports
      break;

    case 1:  //turn merry go round on
      if (currentTime - prevTimeMerryGoRound >= merryGoRoundInterval) {

        if (merryGoRoundCurrentSteps >= merryGoRoundSteps) {  //true if n = 4096, 4096 = 0
          merryGoRoundCurrentSteps = 0;                       //don't let it overflow
        } else if (merryGoRoundCurrentSteps < 0) {            //if it hits negative
          merryGoRoundCurrentSteps = merryGoRoundSteps;
        }

        if (merryGoRoundReverseDir) {
          PORTA = (PORTA & oddPorts) | evenPortsClockwise[merryGoRoundCurrentSteps % 8];
          merryGoRoundCurrentSteps++;
        } else {
          PORTA = (PORTA & oddPorts) | evenPortsClockwise[merryGoRoundCurrentSteps % 8];
          merryGoRoundCurrentSteps--;
        }
        //PORTA = evenPortsClockwise[merryGoRoundCurrentSteps % 8];

        digitalWrite(merryGoRoundLED_ON, HIGH);

        prevTimeMerryGoRound = currentTime;
      }
      break;
    case 2:                                                  //increase speed
      if (merryGoRoundInterval > merryGoRoundMinInterval) {  //15 is minimum speed
        merryGoRoundInterval -= 5;
      }                       //else do nothing
      merryGoRoundState = 1;  //Switch state to on again.
      break;
    case 3:                                                  //decrease speed
      if (merryGoRoundInterval < merryGoRoundMaxInterval) {  //15 is minimum speed
        merryGoRoundInterval += 5;
      }                       //else do nothing
      merryGoRoundState = 1;  //Switch state to on again.

      break;
    case 4:
      merryGoRoundReverseDir = !merryGoRoundReverseDir;  //reverse the direction
      merryGoRoundState = 1;                             //run merry go round again.
      break;
  }
  //Ferris wheel logic
  switch (ferrisWheelState) {
    case 0:  //turn off
      digitalWrite(ferrisWheelLED_ON, LOW);
      PORTC = PORTC & evenPorts;
      break;
    case 1:  //turn on
      if (currentTime - prevTimeFerrisWheel >= ferrisWheelInterval) {
        if (ferrisWheelCurrentSteps >= ferrisWheelSteps) {  //don't let it overflow
          ferrisWheelCurrentSteps = 0;
        } else if (ferrisWheelCurrentSteps < 0) {  //no negatives!
          ferrisWheelCurrentSteps = ferrisWheelSteps;
        }
        if (ferrisWheelReverseDir) {
          PORTC = (PORTC & evenPorts) | oddPortsCounterClockwise[ferrisWheelCurrentSteps % 8];
          ferrisWheelCurrentSteps++;
        } else {
          PORTC = (PORTC & evenPorts) | oddPortsCounterClockwise[ferrisWheelCurrentSteps % 8];
          ferrisWheelCurrentSteps--;
        }
        digitalWrite(ferrisWheelLED_ON, HIGH);
        prevTimeFerrisWheel = currentTime;
      }
      break;
    case 2:                                                //decrease speed
      if (ferrisWheelInterval < ferrisWheelMaxInterval) {  //15 is minimum speed
        ferrisWheelInterval += 5;
      }                      //else do nothing
      ferrisWheelState = 1;  //Switch state to on again.
      break;
    case 3:                                                //increase speed, while interval not equal to 1 keep decreasing
      if (ferrisWheelInterval > ferrisWheelMinInterval) {  //to increase we keep decreasing the interval, 1 is the lowest (fastest)
        ferrisWheelInterval -= 5;
      }                      //else do nothing.
      ferrisWheelState = 1;  //Switch state to on again.
      break;
    case 4:  //change direction
      ferrisWheelReverseDir = !ferrisWheelReverseDir;
      ferrisWheelState = 1;  //Switch state to on again.
      break;
  }
  //Dart board logic
  switch (dartBoardState) {
    case 0:  //dart board off
      analogWrite(dartBoardPWM_PIN, 0);
      digitalWrite(dartBoardIN1, LOW);
      digitalWrite(dartBoardIN2, LOW);

      break;
    case 1:  //dart board on
      analogWrite(dartBoardPWM_PIN, dartBoardCurrentDutyCycle);
      digitalWrite(dartBoardIN1, dartBoardReverseDir);
      digitalWrite(dartBoardIN2, !dartBoardReverseDir);
      break;
    case 2:  //slow down
      dartBoardDutyCycleIndex--;

      if (dartBoardDutyCycleIndex < 0) dartBoardDutyCycleIndex = 0;  //stay at index zero since that's the minimum
      dartBoardCurrentDutyCycle = dartBoardDutyCycle[dartBoardDutyCycleIndex % 4];

      dartBoardState = 1;  //go to one again
      break;
    case 3:  //speed up
      dartBoardDutyCycleIndex++;
      if (dartBoardDutyCycleIndex > 3) dartBoardDutyCycleIndex = 3;  //3 is max
      dartBoardCurrentDutyCycle = dartBoardDutyCycle[dartBoardDutyCycleIndex % 4];

      dartBoardState = 1;  //Stop it from running state 3
      break;
  }
}
