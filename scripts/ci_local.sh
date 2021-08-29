#!/bin/bash

set -e

docker run \
    -v "$(pwd)":/app \
    -v "$(pwd)/artifacts":/artifacts \
    --rm \
    nathanboyd/github_cli:0.0.2 \
    /app/scripts/ci_run.sh
