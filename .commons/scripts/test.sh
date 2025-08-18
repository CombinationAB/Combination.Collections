#!/bin/sh

# Fail pipeline on any error
set -ex

# Build in Debug and run unit tests
dotnet test --configuration Debug --logger trx --results-directory ./.artifacts/reports/vstest/ --collect:"XPlat Code Coverage;Format=opencover" --filter "Category!=ComponentTest&Category!=IntegrationTest"
