#!/bin/bash

# Fail pipeline on any error
set -ex

# Format C# code according to conventions
dotnet format -v d
