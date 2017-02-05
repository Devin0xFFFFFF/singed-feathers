// Fill out your copyright notice in the Description page of Project Settings.

#include "SingedFeathers.h"
#include "ConstructorHelpers.h"
#include "Base_Tile.h"
#include "Game_Map.h"

// Sets default values
AGame_Map::AGame_Map(/*const FObjectInitializer&*/)
{
    height = 10;
    width = 10;
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
	GenerateTiles();
}

// Called every frame
void AGame_Map::Tick( float DeltaTime )
{
	Super::Tick( DeltaTime );

}

void AGame_Map::GenerateTiles() {
    UE_LOG(LogTemp, Warning, TEXT("Inside generate tiles"));
    /*static ConstructorHelpers::FClassFinder<UBlueprint> BaseTile(TEXT("Content/Blueprints/Base_Tile_Blueprint"));
    if (BaseTile.Succeeded()) {
        FVector Location(0.0f,0.0f,0.0f);
        FRotator Rotation(90.0f,0.0f,0.0f);
        FActorSpawnParameters SpawnInfo;
        GetWorld()->SpawnActor<ABase_Tile>(BaseTile.Class, Location, Rotation, SpawnInfo);
     } else {
        UE_LOG(LogTemp, Warning, TEXT("Failed to generate tiles"));
     }*/
}

void AGame_Map::ProcessTurn() {
    
}