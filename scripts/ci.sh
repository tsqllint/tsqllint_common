#!/bin/bash

set -e

docker run \
    -v "$(pwd)":/app \
    -v "$(pwd)/artifacts":/artifacts \
    --rm \
    mcr.microsoft.com/dotnet/sdk:5.0 \
    /app/scripts/build.sh
