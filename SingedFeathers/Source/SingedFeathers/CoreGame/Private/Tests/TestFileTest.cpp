// Fill out your copyright notice in the Description page of Project Settings.

#include "SingedFeathers.h"
#include "AutomationTest.h"

IMPLEMENT_SIMPLE_AUTOMATION_TEST( FMyTest, "Test.MyTest", EAutomationTestFlags::ApplicationContextMask | EAutomationTestFlags::SmokeFilter )

bool FMyTest::RunTest(const FString& Parameters) {
    return true;
}

