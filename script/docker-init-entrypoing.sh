#!/usr/bin/env bash
set -euo pipefail

echo "=== Complete DB Migration (init image) ==="
echo "Time: $(date -u +"%Y-%m-%dT%H:%M:%SZ")"

if [[ -z "${CONNECTION_STRING:-}" ]]; then
  echo "ERROR: CONNECTION_STRING environment variable is not set"
  exit 1
fi

BUNDLE_PATH="/sql/migratedb"

if [[ ! -f "$BUNDLE_PATH" ]]; then
  echo "ERROR: Migration bundle not found at $BUNDLE_PATH"
  echo "Contents of /sql:"
  ls -la /sql || true
  exit 1
fi

chmod +x "$BUNDLE_PATH" || true

echo "Running migration bundle..."
"$BUNDLE_PATH" --connection "$CONNECTION_STRING"

echo "Migration completed successfully."
