// Fill out your copyright notice in the Description page of Project Settings.

#include "SingedFeathers.h"
#include "Base_Tile.h"
#include "Tile_Info.cpp"

// Sets default values
ABase_Tile::ABase_Tile()
{
 	// Set this actor to call Tick() every frame.  You can turn this off to improve performance if you don't need it.
	PrimaryActorTick.bCanEverTick = true;
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
}

// Called when the game starts or when spawned
void ABase_Tile::BeginPlay()
{
	Super::BeginPlay();
	
}

// Called every frame
void ABase_Tile::Tick( float DeltaTime )
{
	Super::Tick( DeltaTime );

}

