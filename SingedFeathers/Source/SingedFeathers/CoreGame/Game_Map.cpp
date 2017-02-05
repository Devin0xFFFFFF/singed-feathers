// Fill out your copyright notice in the Description page of Project Settings.

#include "SingedFeathers.h"
#include <string>
#include "ConstructorHelpers.h"
#include "Game_Map.h"

// Sets default values
AGame_Map::AGame_Map(/*const FObjectInitializer&*/)
{
    height = 2;
    width = 2;
    tileMap = vector<vector<int>>(height,vector<int>(width,2));
    UE_LOG(LogTemp, Warning, TEXT("Init map"));
    //load this in later
    //for now we are just to going use it as is
 	// Set this actor to call Tick() every frame.  You can turn this off to improve performance if you don't need it.
	PrimaryActorTick.bCanEverTick = true;
}

// Called when the game starts or when spawned
void AGame_Map::BeginPlay()
{
	Super::BeginPlay();
    UE_LOG(LogTemp, Warning, TEXT("Starting to generate tiles"));
}

// Called every frame
void AGame_Map::Tick( float DeltaTime )
{
	Super::Tick( DeltaTime );

}

void AGame_Map::AddBaseTile(UBlueprint* tile) {

}

ABase_Tile* AGame_Map::GetBaseTile(int x, int y) {
    UE_LOG(LogTemp, Warning, TEXT("Made Base Tile"));
    FVector Location(x * tilePixels, y * tilePixels, 0.0f);
    FRotator Rotation(90.0f, 0.0f, 0.0f);
    return GetWorld()->SpawnActor<ABase_Tile>(Location, Rotation);
}

void AGame_Map::ProcessTurn() {
    
}