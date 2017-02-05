// Fill out your copyright notice in the Description page of Project Settings.

#include "SingedFeathers.h"
#include "Tile_Info.h"

const tileInfo grassTileInfo = {
    true,
    5,
    5,
    3,
    3
};

const tileInfo stoneTileInfo = {
    false,
    0,
    0,
    1,
    5
};

const tileInfo ashTileInfo = {
    false,
    0,
    0,
    1,
    1
};

const tileInfo errorTileInfo = {
    false,
    0,
    0,
    1,
    0
};

enum tileType { error=0, ash, grass, stone };
