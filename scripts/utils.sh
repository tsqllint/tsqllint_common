#!/bin/bash

# Setup for script debugging
# set -x

# Fail script if a command fails or if a piped command fails
set -eo pipefail

# Terminal color codes
NO_COLOR='\033[0m'
BLUE='\033[0;34m'
RED='\033[0;31m'

info() {
    printf "${BLUE}INFO:${NO_COLOR} %s\n" "$1"
}

error() {
    printf "${RED}ERROR:${NO_COLOR} %s\n" "$1" >&2
    exit 1
}

# Ensure this script runs in Docker
confirmRunningInDocker() {
    if [ ! -f /.dockerenv ]; then
        error "This script must be run from within a Docker container."
    else
        info "Script is running in a container."
    fi
}

info "Initializing"

# Determine the GIT state
GIT_STATE="clean"
if [[ $(git diff --stat) != '' ]]; then
  GIT_STATE="dirty"
fi

# Process the branch name
BRANCH_NAME="$(git rev-parse --abbrev-ref HEAD)"
BRANCH_NAME="${BRANCH_NAME//[_]/-}" # replace underscores with -
BRANCH_NAME="${BRANCH_NAME//[\/]/-}" # replace slashes with -

# Process versioning information
TAG_COMMIT="$(git rev-list --abbrev-commit --tags --max-count=1)"
TAG="$(git describe --abbrev=0 --tags "${TAG_COMMIT}" 2>/dev/null || true)"

HEAD_COMMIT="$(git rev-parse --short HEAD)"
HEAD_COMMIT_DATE=$(git log -1 --format=%cd --date=format:'%Y%m%d')

# Define directories
ARTIFACTS_DIR="$PROJECT_ROOT/artifacts"
COVERAGE_DIR="$ARTIFACTS_DIR/coverage"

# Ensure directories exist
for dir in "$ARTIFACTS_DIR" "$COVERAGE_DIR"; do
  mkdir -p "$dir"
done

# Gather commit info
COMMIT_AUTHOR=$(git show -s --pretty=format:"%cn")
COMMIT_AUTHOR_EMAIL=$(git show -s --pretty=format:"%ce")
COMMIT_MESSAGE=$(git show -s --pretty=format:"%s")

DOTNET_PACKAGE_DIR="$PROJECT_ROOT/packages"

# Determine if this is a release
RELEASE="false"
if [ "$HEAD_COMMIT" == "$TAG_COMMIT" ] && [ "$GIT_STATE" == "clean" ]; then
    VERSION="$TAG"
    RELEASE="true"
else
    VERSION="${TAG}-${BRANCH_NAME}-${HEAD_COMMIT}-${HEAD_COMMIT_DATE}-${GIT_STATE}"
fi

# Printing environment information
printf "###############################################################\n"
printf "# Environment variables:\n"
printf "# BRANCH_NAME:          %s\n" "$BRANCH_NAME"
printf "# GIT_STATE:            %s\n" "$GIT_STATE"
printf "# RELEASE:              %s\n" "$RELEASE"
printf "# TAG:                  %s\n" "$TAG"
printf "# TAG_COMMIT:           %s\n" "$TAG_COMMIT"
printf "# HEAD_COMMIT:          %s\n" "$HEAD_COMMIT"
printf "# HEAD_COMMIT_DATE:     %s\n" "$HEAD_COMMIT_DATE"
printf "# VERSION:              %s\n" "$VERSION"
printf "# COMMIT_AUTHOR:        %s\n" "$COMMIT_AUTHOR"
printf "# COMMIT_AUTHOR_EMAIL:  %s\n" "$COMMIT_AUTHOR_EMAIL"
printf "# COMMIT_MESSAGE:       %s\n" "$COMMIT_MESSAGE"
printf "# PROJECT_ROOT:         %s\n" "$PROJECT_ROOT"
printf "# SCRIPT_DIR:           %s\n" "$SCRIPT_DIR"
printf "# ARTIFACTS_DIR:        %s\n" "$ARTIFACTS_DIR"
printf "# COVERAGE_DIR:         %s\n" "$COVERAGE_DIR"
printf "# DOTNET_PACKAGE_DIR:   %s\n" "$DOTNET_PACKAGE_DIR"
printf "###############################################################\n"

# Verification of variables
declare -a vars_to_check=("BRANCH_NAME" "GIT_STATE" "RELEASE" "TAG" "TAG_COMMIT" "HEAD_COMMIT" "HEAD_COMMIT_DATE" "VERSION" "PROJECT_ROOT" "SCRIPT_DIR" "COVERAGE_DIR" "DOTNET_PACKAGE_DIR")
for var in "${vars_to_check[@]}"; do
  [ -n "${!var}" ] || error "$var is required and not set."
done

info "Verifying version number"
if ! [[ $VERSION =~ ^[0-9]+\.[0-9]+ ]]; then
    error "Version number failed validation: '$VERSION'"
fi

# Exporting variables
export BRANCH_NAME GIT_STATE RELEASE TAG TAG_COMMIT HEAD_COMMIT HEAD_COMMIT_DATE VERSION PROJECT_ROOT SCRIPT_DIR COVERAGE_DIR DOTNET_PACKAGE_DIR

# Verification of directories
declare -a dirs_to_verify=("$PROJECT_ROOT" "$SCRIPT_DIR" "$ARTIFACTS_DIR" "$COVERAGE_DIR")
for dir in "${dirs_to_verify[@]}"; do
  [ -d "$dir" ] || error "$dir does not exist: $dir"
done

# DOTNET_PACKAGE_DIR should not exist before dotnet restore

info "Initialization complete."
