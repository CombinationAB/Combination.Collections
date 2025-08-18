#!/bin/sh

# Fail pipeline on any error
set -ex

# Build
${CMB_ROOT:-/src}/.commons/scripts/build.sh

dotnet nuget push -k $GITHUB_TOKEN -s github \
  src/*/bin/Release/*.${CMB_VERSION_PACKAGE_SEMVER}.nupkg
