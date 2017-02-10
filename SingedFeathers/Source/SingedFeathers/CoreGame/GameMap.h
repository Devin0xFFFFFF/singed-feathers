// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include <vector>
#include "GameFramework/Actor.h"
#include "TileInfo.h"
#include "BaseTile.h"
#include "GameMap.generated.h"

using std::vector;

UCLASS()
class SINGEDFEATHERS_API AGame_Map : public AActor {
	GENERATED_BODY()
	
public:
    // Sets default values for this actor's properties
    AGame_Map(/*const FObjectInitializer&*/);

	// Called when the game starts or when spawned
	virtual void BeginPlay() override;

    UFUNCTION(BlueprintCallable, Category = "SetUp")
    void Init();
	
	// Called every frame
	virtual void Tick( float DeltaSeconds ) override;

    UFUNCTION(BlueprintCallable, Category = "Map_Variables")
    ABase_Tile* GetBaseTile(int x, int y);

    UFUNCTION(BlueprintCallable, Category = "Map_Variables")
    int GetTileType(int x, int y);

    UFUNCTION(BlueprintCallable, Category = "Map_Variables")
    FVector GetMapLocation(int x, int y);

    UFUNCTION(BlueprintCallable, Category = "Map_Variables")
    int GetHeight();

    UFUNCTION(BlueprintCallable, Category = "Map_Variables")
    int GetWidth();

    UFUNCTION(BlueprintCallable, Category = "Map_Variables")
    TArray<ABase_Tile*> GetTilesToRender(); 

    UFUNCTION(BlueprintCallable, Category = "Fire")
    void SetFire(int x, int y);

private:

    int _height;

    int _width;

    TArray<ABase_Tile*> _tilesToRender;

    void MakeBaseTile(int x, int y);

    TArray<TArray<int>> _tileMap;

    TArray<TArray<ABase_Tile*>> _baseTileMap;

    void ProcessTurn();

    void LinkNearbyTiles(int x, int y);
};
