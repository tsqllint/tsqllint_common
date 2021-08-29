#!/bin/bash

source /app/scripts/setup.sh

echoBlockMessage "packing project"

dotnet pack \
    ./TSQLLint.Common/TSQLLint.Common.csproj \
    -p:VERSION="$VERSION" \
    --configuration Release \
    --output /artifacts

if [ "$RELEASE" == "false" ]; then
    echoMessage "Untagged commits are not pushed to Nuget"
    exit 0
fi

echoBlockMessage "releasing project"

if [[ -z "$NUGET_API_KEY" ]]; then
    echoMessage "NUGET_API_KEY is not set in the environment."
    echoMessage "Artifacts will not be pushed to Nuget."
    exit 1
fi

echoBlockMessage "pushing to Nuget"

dotnet nuget push \
    "/artifacts/TSQLLint.Common.$VERSION.nupkg" \
    --api-key "$NUGET_API_KEY"  \
    --source https://api.nuget.org/v3/index.json

echoBlockMessage "creating release"

gh auth login  --hostname "github.com" --with-token < "$GITHUB_TOKEN_FILE"

gh release create "$VERSION" -d /artifacts/*.nupkg
