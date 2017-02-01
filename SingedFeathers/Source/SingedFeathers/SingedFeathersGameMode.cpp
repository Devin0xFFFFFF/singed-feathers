// Copyright 1998-2016 Epic Games, Inc. All Rights Reserved.

#include "SingedFeathers.h"
#include "SingedFeathersGameMode.h"
#include "SingedFeathersCharacter.h"

ASingedFeathersGameMode::ASingedFeathersGameMode()
{
	// set default pawn class to our character
	DefaultPawnClass = ASingedFeathersCharacter::StaticClass();	
}
