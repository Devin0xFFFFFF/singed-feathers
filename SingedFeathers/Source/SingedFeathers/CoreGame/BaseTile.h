// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include <vector>
#include "TileInfo.h"
#include "GameFramework/Actor.h"
#include "BaseTile.generated.h"

using std::vector;

UCLASS()
class SINGEDFEATHERS_API ABase_Tile : public AActor {
	GENERATED_BODY()
	
public:	

	// Sets default values for this actor's properties
	ABase_Tile();

    void SetTileType(int tildType);

    UPROPERTY(EditAnywhere, BlueprintReadWrite, Category="Tile")
    int tileId;//I was running into issues when I named it textureCode

	// Called when the game starts or when spawned
	virtual void BeginPlay() override;
	
	// Called every frame
	virtual void Tick( float DeltaSeconds ) override;

    void ApplyHeat(int heat);

    void AddTileToNeighbours(ABase_Tile* tile);

    void StartTurn();

    void SpreadFire();

    void AddRenderList(TArray<ABase_Tile*>);

    void RenderTile();

private:
    bool isFlammable;
    int flashPoint;
    int durability;
    int burnDuration;
    int heatThisTurn;
    bool onFire;
    bool shouldSpreadFireThisTurn;
    vector<ABase_Tile*> neighbouringTiles;
    TArray<ABase_Tile*> renderList;

    void SetTileTypeInternal(const tileInfo*);

    void SetOnFire();
	
};
