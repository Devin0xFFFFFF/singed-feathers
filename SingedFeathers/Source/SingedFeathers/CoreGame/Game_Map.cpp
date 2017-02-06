// Fill out your copyright notice in the Description page of Project Settings.

#include "SingedFeathers.h"
#include <string>
#include "Tile_Info.cpp"
#include "ConstructorHelpers.h"
#include "Game_Map.h"

// Sets default values
AGame_Map::AGame_Map(/*const FObjectInitializer&*/)
{
    height = 5;
    width = 4;
    tileMap = vector< vector<int> >(width, vector<int>(height, 2));
    baseTileMap = vector< vector<ABase_Tile*> >(width, vector<ABase_Tile*>(height, NULL));
    tileMap[0][0] = 3;
    tileMap[1][2] = 1;
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
}

// Called every frame
void AGame_Map::Tick( float DeltaTime )
{
	Super::Tick( DeltaTime );

}

void AGame_Map::AddBaseTile(UBlueprint* tile) {

}

ABase_Tile* AGame_Map::GetBaseTile(int x, int y) {
    FVector Location(y * tilePixels, x * tilePixels, 0.0f);
    FRotator Rotation(90.0f, 0.0f, 0.0f);
    ABase_Tile* baseTile = GetWorld()->SpawnActor<ABase_Tile>(Location, Rotation);
    //baseTileMap[x][y] = baseTile;
    return baseTile;
}

int AGame_Map::GetTileType(int x, int y) {
    return tileMap[x][y];
}

FVector AGame_Map::GetMapLocation(int x, int y) {
    return FVector(x * -1 * tilePixels, y * tilePixels, 0.0f);
}

void AGame_Map::ProcessTurn() {
    
}