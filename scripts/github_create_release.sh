#!/bin/bash

################################################################################
# Script to create GitHub releases from built artifacts
################################################################################

# Enable for bash debugging
# set -x

# Fail script if a command fails
set -e

# Fail script if a piped command fails
set -o pipefail

# Obtain script and project directories
SCRIPT_DIR=$( cd -- "$( dirname -- "${BASH_SOURCE[0]}" )" &> /dev/null && pwd )
PROJECT_ROOT="$(dirname "$SCRIPT_DIR")"
cd "$PROJECT_ROOT"

# Source utility script
source "$SCRIPT_DIR/utils.sh"

# Ensure this is a release build
if [ "$RELEASE" == "false" ]; then
    info "Not a release build, exiting now."
    exit 0
else
    info "Starting release process."
fi

# Check for GitHub token file
[ -n "$GITHUB_TOKEN_FILE" ] || { error "GITHUB_TOKEN_FILE is required and not set, aborting."; exit 1; }

info "Logging into GitHub CLI..."
ls -lah "$ARTIFACTS_DIR"

# Read the GitHub token from the file and login
echo "${GITHUB_TOKEN_FILE}" > .githubtoken
gh auth login --with-token < .githubtoken
rm .githubtoken

info "Configuring gh CLI..."
gh config set prompt disabled

info "Creating GitHub release..."
# Create the release if it doesn't already exist
if ! gh release view "$VERSION" > /dev/null 2>&1; then
    gh release create "$VERSION" --title "$VERSION" ${IS_PRERELEASE:+--prerelease} ${IS_DRAFT:+--draft}
else
    info "Release $VERSION already exists, skipping creation."
fi

# Upload artifacts for each platform
PLATFORMS=("win-x86" "win-x64" "win-arm64" "osx-x64" "osx-arm64" "linux-x64" "linux-musl-x64" "linux-musl-arm64" "linux-arm" "linux-arm64")
for PLATFORM in "${PLATFORMS[@]}"
do
    ARTIFACT="$ARTIFACTS_DIR/${PLATFORM}.tgz"
    info "Uploading artifact '$ARTIFACT' to release $VERSION..."
    if [ -f "$ARTIFACT" ]; then
        gh release upload "$VERSION" "$ARTIFACT"
    else
        error "Artifact '$ARTIFACT' does not exist, skipping upload."
    fi
done

info "Release process completed."

exit 0
