// Fill out your copyright notice in the Description page of Project Settings.

#include "SingedFeathers.h"
#include "BaseTile.h"
#include "TileInfo.cpp"

// Sets default values
ABase_Tile::ABase_Tile() {
 	// Set this actor to call Tick() every frame.
	PrimaryActorTick.bCanEverTick = false;
    heatThisTurn = 0;
    onFire = false;
    shouldSpreadFireThisTurn = false;
    neighbouringTiles = vector<ABase_Tile*>();
}

void ABase_Tile::SetTileType(int type) {
    switch (type) {
        case tileType::ash :
            SetTileTypeInternal(&ashTileInfo);
            break;
        case tileType::grass:
            SetTileTypeInternal(&grassTileInfo);
            break;
        case tileType::stone:
            SetTileTypeInternal(&stoneTileInfo);
            break;
        case tileType::wood:
            SetTileTypeInternal(&woodTileInfo);
            break;
        default :
            SetTileTypeInternal(&errorTileInfo);
    }
}

void ABase_Tile::SetTileTypeInternal(const tileInfo* tile) {
    isFlammable = tile->isFlammable;
    flashPoint = tile->flashPoint;
    durability = tile->durability;
    burnDuration = tile->burnDuration;
    tileId = tile->textureCode;
    onFire = false;
}

void ABase_Tile::AddTileToNeighbours(ABase_Tile* tile) {
    neighbouringTiles.emplace_back(tile);
}

void ABase_Tile::SetOnFire() {
    onFire = true;
    tileId = fireTile;
}

void ABase_Tile::ApplyHeat(int heat) {
    if (isFlammable && !onFire) {
        heatThisTurn += heat;
        if (heatThisTurn >= flashPoint) {
            SetOnFire();
        } else {
            durability -= heat;
            if (durability <= 0) {
                SetOnFire();
            }
        }
    }
}

void ABase_Tile::SpreadFire() {
    if (shouldSpreadFireThisTurn) {
        for (ABase_Tile* neighbour : neighbouringTiles) {
            neighbour->ApplyHeat(burnHeat);
        }
        burnDuration--;
        if (burnDuration <= 0) {
            SetTileType(tileType::ash);
        }
    }
}

void ABase_Tile::StartTurn() {
    heatThisTurn = 0;
    shouldSpreadFireThisTurn = onFire;
}

// Called when the game starts or when spawned
void ABase_Tile::BeginPlay() {
	Super::BeginPlay();
	
}

// Called every frame
void ABase_Tile::Tick( float DeltaTime ) {
	Super::Tick( DeltaTime );

}

