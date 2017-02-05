// Fill out your copyright notice in the Description page of Project Settings.

#include "SingedFeathers.h"
#include <string>
#include "Tile_Info.cpp"
#include "ConstructorHelpers.h"
#include "Game_Map.h"

// Sets default values
AGame_Map::AGame_Map(/*const FObjectInitializer&*/)
{
    height = 4;
    width = 4;
    tileMap = vector<vector<int>>(height,vector<int>(width, tileType::ash));
    tileMap.at(0).assign(0,3);
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
    UE_LOG(LogTemp, Warning, TEXT("Made Base Tile x:%d y:%d"), x, y);
    FVector Location(x * tilePixels, y * tilePixels, 0.0f);
    FRotator Rotation(90.0f, 0.0f, 0.0f);
    ABase_Tile* baseTile = GetWorld()->SpawnActor<ABase_Tile>(Location, Rotation);
    //baseTile->SetTileType(tileMap.at(x).at(y));
    return baseTile;
}

FVector AGame_Map::GetMapLocation(int x, int y) {
    FVector result(x * tilePixels, y * tilePixels, 0.0f);
    UE_LOG(LogTemp, Warning, TEXT("Made Base Tile x:%f, y:%f, z:%f"), result.X, result.Y, result.Z);
    return FVector(x * tilePixels, y * tilePixels, 0.0f);
}

void AGame_Map::ProcessTurn() {
    
}