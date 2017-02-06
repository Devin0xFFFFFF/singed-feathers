// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

/**
 * 
 */
const struct tileInfo {
    const bool isFlammable;
    const int flashPoint;
    const int durability;
    const int burnDuration;
    const int textureCode;
};

const int tilePixels = 16;
const int burnHeat = 5;
const int fireTile = 2;
const int xMultiplier = -1;
const int yMultiplier = 1;
