#!/bin/bash

# exit on failures
echo "ENTRYPOINT STARTED: $(date -u +"%Y-%m-%dT%H:%M:%SZ")"
echo "Args: $*"
env | sort | sed -n '1,40p'


set -e
set -o pipefail

exec "$@"