// Fill out your copyright notice in the Description page of Project Settings.

#include "SingedFeathers.h"
#include <string>
#include "TileInfo.cpp"
#include "ConstructorHelpers.h"
#include "GameMap.h"

// Sets default values
AGame_Map::AGame_Map(/*const FObjectInitializer&*/) {
    height = 5;
    width = 4;
    tileMap = vector< vector<int> >(width, vector<int>(height, tileType::grass));
    baseTileMap = vector< vector<ABase_Tile*> >(width, vector<ABase_Tile*>(height, NULL));
    tileMap[0][0] = tileType::stone;
    tileMap[0][3] = tileType::stone;
    tileMap[1][2] = tileType::wood;
    tileMap[2][1] = tileType::wood;
    UE_LOG(LogTemp, Warning, TEXT("Init map"));
    //load this in later
    //for now we are just to going use it as is
 	// Set this actor to call Tick() every frame.  You can turn this off to improve performance if you don't need it.
	PrimaryActorTick.bCanEverTick = true;
}

// Called when the game starts or when spawned
void AGame_Map::BeginPlay() {
    Super::BeginPlay();
}

// Call immediatly after blueprint begin play
void AGame_Map::Init()
{
    for (int x = 0; x < width; x++) {
        for (int y = 0; y < height; y++) {
            MakeBaseTile(x, y);
        }
    }
    for (int x = 0; x < width; x++) {
        for (int y = 0; y < height; y++) {
            LinkNearbyTiles(x, y);
        }
    }
    baseTileMap[2][2]->ApplyHeat(10);
}

void AGame_Map::LinkNearbyTiles(int x, int y) {
    //is this terrible? Yes
    //should I fix it when we decide if we want diagonals or not? Yes
    if (x > 0) {
        baseTileMap[x][y]->AddTileToNeighbours(baseTileMap[x - 1][y]);
    }
    if (y > 0) {
        baseTileMap[x][y]->AddTileToNeighbours(baseTileMap[x][y - 1]);
    }
    if (x < width - 1) {
        baseTileMap[x][y]->AddTileToNeighbours(baseTileMap[x + 1][y]);
    }
    if (y < height - 1) {
        baseTileMap[x][y]->AddTileToNeighbours(baseTileMap[x][y + 1]);
    }
}

// Called every frame
void AGame_Map::Tick( float DeltaTime ) {
	Super::Tick( DeltaTime );
    ProcessTurn();
}

ABase_Tile* AGame_Map::GetBaseTile(int x, int y) {
    return baseTileMap[x][y];
}

void AGame_Map::MakeBaseTile(int x, int y) {
    FVector Location(y * tilePixels, x * tilePixels, 0.0f);
    FRotator Rotation(0.0f, 0.0f, 0.0f);
    baseTileMap[x][y] = GetWorld()->SpawnActor<ABase_Tile>(Location, Rotation);
    baseTileMap[x][y]->SetTileType(tileMap[x][y]);
}

int AGame_Map::GetTileType(int x, int y) {
    return tileMap[x][y];
}

FVector AGame_Map::GetMapLocation(int x, int y) {
    return FVector(x * xMultiplier * tilePixels, y * yMultiplier * tilePixels, 0.0f);
}

void AGame_Map::ProcessTurn() {
    for (int x = 0; x < width; x++) {
        for (int y = 0; y < height; y++) {
           baseTileMap[x][y]->StartTurn();
        }
    }
    for (int x = 0; x < width; x++) {
        for (int y = 0; y < height; y++) {
            baseTileMap[x][y]->SpreadFire();
        }
    }
}
