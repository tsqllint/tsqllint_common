#!/bin/bash

################################################################################
# a script to create GitHub releases from built artifacts
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

# Variables expected to be set
# ARTIFACTS_DIR, GITHUB_TOKEN, VERSION

[ -n "$ARTIFACTS_DIR" ] || error "ARTIFACTS_DIR is not set"
[ -n "$GITHUB_TOKEN" ] || error "GITHUB_TOKEN is required and not set"
[ -n "$VERSION" ] || error "VERSION is not set"

if [ "$RELEASE" == "false" ]; then
    info "Not a release build. Exiting now."
    exit 0
else
    info "Starting release process"
fi

info "Logging into GitHub CLI"

# Authenticating with GitHub CLI using token
echo "$GITHUB_TOKEN" | gh auth login --with-token

info "Configuring GitHub CLI to disable prompts"
gh config set prompt disabled

info "Creating GitHub release"
gh release create "$VERSION" --title "$VERSION" --prerelease --draft

info "Listing artifacts in $ARTIFACTS_DIR"
ls -lah "$ARTIFACTS_DIR"

PLATFORMS=("win-x86" "win-x64" "win-arm64" "osx-x64" "osx-arm64" "linux-x64" "linux-musl-x64" "linux-musl-arm64" "linux-arm" "linux-arm64")
for PLATFORM in "${PLATFORMS[@]}"
do
    ARTIFACT="${ARTIFACTS_DIR}/${PLATFORM}.tgz"
    if [ -f "$ARTIFACT" ]; then
        info "Uploading artifact '$ARTIFACT' to release $VERSION"
        gh release upload "$VERSION" "$ARTIFACT"
    else
        error "Artifact '$ARTIFACT' does not exist"
    fi
done

info "GitHub release process complete"

exit 0
