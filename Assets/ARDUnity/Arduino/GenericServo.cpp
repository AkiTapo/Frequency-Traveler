/*
  GenericServo.cpp - Ardunity Arduino library
  Copyright (C) 2015 ojh6t3k.  All rights reserved.
*/

//******************************************************************************
//* Includes
//******************************************************************************
#include "Ardunity.h"
#include "GenericServo.h"


//******************************************************************************
//* Constructors
//******************************************************************************

GenericServo::GenericServo(int id, int pin) : ArdunityController(id)
{
	_pin = pin;
    canFlush = false;
}

//******************************************************************************
//* Override Methods
//******************************************************************************
void GenericServo::OnSetup()
{
}

void GenericServo::OnStart()
{
	_servo.attach(_pin);
}

void GenericServo::OnStop()
{
	_servo.detach();
}

void GenericServo::OnProcess()
{
}

void GenericServo::OnUpdate()
{
	UINT8 newAngle;
	ArdunityApp.pop(&newAngle);
	if(_angle != newAngle)
	{
		_angle = newAngle;
		updated = true;
	}
}

void GenericServo::OnExecute()
{
	_servo.write(_angle);
}

void GenericServo::OnFlush()
{
}

//******************************************************************************
//* Private Methods
//******************************************************************************

