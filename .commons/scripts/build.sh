#!/bin/bash

# Fail pipeline on any error
set -ex

# Build in Release mode and publish to direcory
dotnet pack --configuration Release
