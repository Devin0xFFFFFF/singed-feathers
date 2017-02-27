package main

import (
	"io"
	"net/http"
)

func GetHelloText() string {
	return "Hello World!"
}

func hello(w http.ResponseWriter, r *http.Request) {
	io.WriteString(w, GetHelloText())
}

func main() {
	http.HandleFunc("/", hello)
	http.ListenAndServe(":8000", nil)
}