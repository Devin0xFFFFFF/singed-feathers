// Fill out your copyright notice in the Description page of Project Settings.

#include "SingedFeathers.h"
#include "TileInfo.h"

const tileInfo grassTileInfo = {
    true, //isFlammable
    10, //flashPoint
    15, //durability;
    3, //burnDuration;
    3 //textureCode;
};

const tileInfo stoneTileInfo = {
    false, //isFlammable
    0, //flashPoint
    0, //durability;
    1, //burnDuration;
    5 //textureCode;
};

const tileInfo ashTileInfo = {
    false, //isFlammable
    0, //flashPoint
    0, //durability;
    1, //burnDuration;
    1 //textureCode;
};

const tileInfo errorTileInfo = {
    false, //isFlammable
    0, //flashPoint
    0, //durability;
    1, //burnDuration;
    0 //textureCode;
};

enum tileType { error=0, ash, grass, stone };
