// Fill out your copyright notice in the Description page of Project Settings.

#include "SingedFeathers.h"
#include <string>
#include "TileInfo.cpp"
#include "ConstructorHelpers.h"
#include "GameMap.h"

// Sets default values
AGame_Map::AGame_Map(/*const FObjectInitializer&*/) {
    _height = 10;
    _width = 8;

    TArray<int> collumns;
    collumns.Init(tileType::grass, _height);
    _tileMap.Init(collumns, _width);

    TArray<ABase_Tile*> tileCollumns;
    tileCollumns.Init(NULL, _height);
    _baseTileMap.Init(tileCollumns, _width);

    _tileMap[0][1] = tileType::stone;
    _tileMap[0][3] = tileType::stone;
    _tileMap[0][5] = tileType::stone;
    _tileMap[0][7] = tileType::stone;
    _tileMap[0][9] = tileType::stone;
    _tileMap[1][0] = tileType::stone;
    _tileMap[3][0] = tileType::stone;
    _tileMap[5][0] = tileType::stone;
    _tileMap[7][0] = tileType::stone;
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
    for (int x = 0; x < _width; x++) {
        for (int y = 0; y < _height; y++) {
            MakeBaseTile(x, y);
        }
    }
    for (int x = 0; x < _width; x++) {
        for (int y = 0; y < _height; y++) {
            LinkNearbyTiles(x, y);
        }
    }
}

void AGame_Map::LinkNearbyTiles(int x, int y) {
    //is this terrible? Yes
    //should I fix it when we decide if we want diagonals or not? Yes
    if (x > 0) {
        _baseTileMap[x][y]->AddTileToNeighbours(_baseTileMap[x - 1][y]);
    }
    if (y > 0) {
        _baseTileMap[x][y]->AddTileToNeighbours(_baseTileMap[x][y - 1]);
    }
    if (x < _width - 1) {
        _baseTileMap[x][y]->AddTileToNeighbours(_baseTileMap[x + 1][y]);
    }
    if (y < _height - 1) {
        _baseTileMap[x][y]->AddTileToNeighbours(_baseTileMap[x][y + 1]);
    }
}

// Called every frame
void AGame_Map::Tick( float DeltaTime ) {
	Super::Tick( DeltaTime );
    ProcessTurn();
}

ABase_Tile* AGame_Map::GetBaseTile(int x, int y) {
    return _baseTileMap[x][y];
}

void AGame_Map::MakeBaseTile(int x, int y) {
    static const FRotator rotation = FRotator(0.0f, 0.0f, 90.0f);//y,z,x
    _baseTileMap[x][y] = GetWorld()->SpawnActor<ABase_Tile>(GetMapLocation(x,y), rotation);
    _baseTileMap[x][y]->AddRenderList(&_tilesToRender);
    _baseTileMap[x][y]->SetTileType(_tileMap[x][y]);
}

int AGame_Map::GetTileType(int x, int y) {
    return _tileMap[x][y];
}

FVector AGame_Map::GetMapLocation(int x, int y) {
    return FVector(x * X_MULTIPLIER * TILE_PIXELS, y * Y_MULTIPLIER * TILE_PIXELS, 0.0f);
}

void AGame_Map::ProcessTurn() {
    for (int x = 0; x < _width; x++) {
        for (int y = 0; y < _height; y++) {
           _baseTileMap[x][y]->StartTurn();
        }
    }
    for (int x = 0; x < _width; x++) {
        for (int y = 0; y < _height; y++) {
            _baseTileMap[x][y]->SpreadFire();
        }
    }
}

TArray<ABase_Tile*> AGame_Map::GetTilesToRender() {
    TArray<ABase_Tile*> copy = TArray<ABase_Tile*> (_tilesToRender);
    _tilesToRender.Empty();
    return copy;
}

void AGame_Map::SetFire(int x, int y) {
    if (x >= 0 && y >= 0 && x < _width && y < _height) {
        _baseTileMap[x][y]->ApplyHeat(100);
    }
}

int AGame_Map::GetHeight() {
    return _height;
}

int AGame_Map::GetWidth() {
    return _width;
}
