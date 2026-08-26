#!/bin/bash

################################################################################
# a script to build and test the project locally
################################################################################

# enable for bash debugging
#set -x

# fail script if a cmd fails
set -e

# fail script if piped command fails
set -o pipefail

SCRIPT_DIR=$( cd -- "$( dirname -- "${BASH_SOURCE[0]}" )" &> /dev/null && pwd )
PROJECT_ROOT="$(dirname "$SCRIPT_DIR")"
PATH="$PATH:$HOME/.dotnet/tools"
cd "$PROJECT_ROOT"

source "$SCRIPT_DIR/utils.sh"

if [ -f /.dockerenv ]; then
    confirmRunningInDocker
fi

info "restoring project"

dotnet restore \
    "$PROJECT_ROOT/TSQLLint.Common.sln" \
    --verbosity m \
    --packages "$DOTNET_PACKAGE_DIR"

info "building project"

dotnet build \
    "$PROJECT_ROOT/TSQLLint.Common.sln" \
    /p:Version="$VERSION" \
    --configuration Release \
    --no-restore

info "running test project"

dotnet test \
    --no-restore \
    --collect:"XPlat Code Coverage" \
    --settings "$PROJECT_ROOT/coverlet.runsettings" \
    --results-directory "$COVERAGE_DIR" \
    "$PROJECT_ROOT/TSQLLint.Common.sln"

info "done"

exit 0
