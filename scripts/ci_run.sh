#!/bin/bash

source /app/scripts/script.sh

echoBlockMessage "restoring project"

dotnet restore \
    ./TSQLLint.Common/TSQLLint.Common.csproj \
    --verbosity m

echoBlockMessage "building project"

dotnet build \
    ./TSQLLint.Common/TSQLLint.Common.csproj \
    /p:Version="$VERSION" \
    --configuration Release \
    --no-restore

echoBlockMessage "restoring test project"

dotnet restore \
    ./TSQLLint.Common.Tests/TSQLLint.Common.Tests.csproj \
    --verbosity m

echoBlockMessage "running test project"

dotnet test \
    --no-restore \
    ./TSQLLint.Common.Tests/TSQLLint.Common.Tests.csproj
