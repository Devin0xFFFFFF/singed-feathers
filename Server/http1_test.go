package main

import (
	"testing"
)

func TestHttp1(t *testing.T) {
	const expected = "Hello World!"
	if x := GetHelloText(); x != expected {
		t.Errorf("GetHelloText() = %v, want %v", x, expected)
	}
}