// Fill out your copyright notice in the Description page of Project Settings.

#include "SingedFeathers.h"
#include "BaseTile.h"
#include "TileInfo.cpp"

// Sets default values
ABase_Tile::ABase_Tile() {
 	// Set this actor to call Tick() every frame.
    RootComponent = CreateDefaultSubobject<USceneComponent>(TEXT("RootComponent"));
	PrimaryActorTick.bCanEverTick = false;
    _heatThisTurn = 0;
    _onFire = false;
    _shouldSpreadFireThisTurn = false;
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
    _isFlammable = tile->isFlammable;
    _flashPoint = tile->flashPoint;
    _durability = tile->durability;
    _burnDuration = tile->burnDuration;
    _tileId = tile->textureCode;
    _onFire = false;
    RenderTile();
}

void ABase_Tile::AddTileToNeighbours(ABase_Tile* tile) {
    _neighbouringTiles.Add(tile);
}

void ABase_Tile::SetOnFire() {
    _onFire = true;
    _tileId = FIRE_TILE;
    RenderTile();
}

void ABase_Tile::ApplyHeat(int heat) {
    if (_isFlammable && !_onFire) {
        _heatThisTurn += heat;
        if (_heatThisTurn >= _flashPoint) {
            SetOnFire();
        } else {
            _durability -= heat;
            if (_durability <= 0) {
                SetOnFire();
            }
        }
    }
}

void ABase_Tile::SpreadFire() {
    if (_shouldSpreadFireThisTurn) {
        for (ABase_Tile* neighbour : _neighbouringTiles) {
            neighbour->ApplyHeat(BURN_HEAT);
        }
        _burnDuration--;
        if (_burnDuration <= 0) {
            SetTileType(tileType::ash);
        }
    }
}

void ABase_Tile::StartTurn() {
    _heatThisTurn = 0;
    _shouldSpreadFireThisTurn = _onFire;
}

void ABase_Tile::AddRenderList(TArray<ABase_Tile*>* list ) {
    _renderList = list;
}

void ABase_Tile::RenderTile() {
    _renderList->Add(this);
}

int ABase_Tile::GetTileId() {
    return _tileId;
}

// Called when the game starts or when spawned
void ABase_Tile::BeginPlay() {
	Super::BeginPlay();
	
}

// Called every frame
void ABase_Tile::Tick( float DeltaTime ) {
	Super::Tick( DeltaTime );

}

