// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include <vector>
#include "GameFramework/Actor.h"
#include "Tile_Info.h"
#include "Game_Map.generated.h"

using std::vector;

UCLASS()
class SINGEDFEATHERS_API AGame_Map : public AActor
{
	GENERATED_BODY()
	
public:	

    // Sets default values for this actor's properties
    AGame_Map(/*const FObjectInitializer&*/);

	// Called when the game starts or when spawned
	virtual void BeginPlay() override;
	
	// Called every frame
	virtual void Tick( float DeltaSeconds ) override;

    int height;

    int width;

private:
    vector<vector<int>> tileMap;

    void ProcessTurn();

    void GenerateTiles();
};
