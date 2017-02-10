// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

/**
 * 
 */
struct tileInfo {
    const bool isFlammable;
    const int flashPoint;
    const int durability;
    const int burnDuration;
    const int textureCode;
};

const int TILE_PIXELS = 16;
const int BURN_HEAT = 5;
const int FIRE_TILE = 2;
const int X_MULTIPLIER = -1;
const int Y_MULTIPLIER = 1;
