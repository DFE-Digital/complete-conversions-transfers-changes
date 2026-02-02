#!/bin/bash

# exit on failures
set -euo pipefail

if [[ "${RUN_DB_MIGRATIONS:-false}" = "true" ]]; then
  echo "Running EF migrations..."
  /app/migratedb
  echo "Migrations complete."
else
  echo "Skipping migrations (RUN_DB_MIGRATIONS != true)"
fi

exec dotnet Dfe.Complete.dll
