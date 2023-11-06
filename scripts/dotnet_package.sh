#!/bin/bash

################################################################################
# a script to package, publish and push nuget packages
################################################################################

# enable for bash debugging
#set -x

# fail script if a cmd fails
set -e

# fail script if piped command fails
set -o pipefail

SCRIPT_DIR=$( cd -- "$( dirname -- "${BASH_SOURCE[0]}" )" &> /dev/null && pwd )
PROJECT_ROOT="$(dirname "$SCRIPT_DIR")"
cd "$PROJECT_ROOT"

source "$SCRIPT_DIR/utils.sh"

confirmRunningInDocker # Ensures the script is running in Docker

info "Initializing"

# Variables expected to be set from utils.sh or elsewhere
# VERSION, ARTIFACTS_DIR, NUGET_API_KEY

[ -n "$VERSION" ] || error "VERSION is not set"
[ -n "$ARTIFACTS_DIR" ] || error "ARTIFACTS_DIR is not set"

if [ "$RELEASE" == "false" ]; then
    info "Not a release build. Exiting now."
    exit 0
else
    info "Starting release"
fi

info "Packing project"

dotnet pack \
    "$PROJECT_ROOT/TSQLLint.Common.sln" \
    -p:PackageVersion="$VERSION" \
    --configuration Release \
    --output "$ARTIFACTS_DIR"

info "Build and archive assemblies"

PLATFORMS=("win-x86" "win-x64" "win-arm64" "osx-x64" "osx-arm64" "linux-x64" "linux-musl-x64" "linux-musl-arm64" "linux-arm" "linux-arm64")
for PLATFORM in "${PLATFORMS[@]}"
do
    info "Building assemblies for platform $PLATFORM"

    OUT_DIR="$ARTIFACTS_DIR/$PLATFORM"
    mkdir -p "$OUT_DIR"

    info "Creating assemblies directory $OUT_DIR"

    dotnet publish \
        "$PROJECT_ROOT/TSQLLint.Common/TSQLLint.Common.csproj" \
        -c Release \
        -r "$PLATFORM" \
        --self-contained true \
        -p:PublishSingleFile=true \
        -p:PublishTrimmed=true \
        -p:Version="$VERSION" \
        -o "$OUT_DIR"

    info "Archiving assemblies for platform $PLATFORM"

    # change directory to reduce directory depth in archive file
    info "Changing directory to $ARTIFACTS_DIR"
    cd "$ARTIFACTS_DIR"

    tar -zcf "$PLATFORM.tgz" "$PLATFORM"
    rm -rf "$PLATFORM"

    info "Changing directory to $PROJECT_ROOT"
    cd "$PROJECT_ROOT"
done

[ -n "$NUGET_API_KEY" ] || { error "NUGET_API_KEY is required and not set, aborting"; }

info "Pushing to Nuget"

dotnet nuget push \
    "${ARTIFACTS_DIR}/TSQLLint.Common.${VERSION}.nupkg" \
    --api-key "${NUGET_API_KEY}" \
    --source "https://api.nuget.org/v3/index.json"

info "Done"

exit 0
