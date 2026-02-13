#!/usr/bin/env bash
set -euo pipefail

log() { echo "[init-migrations] $*"; }

log "Starting migration container"
log "UTC time: $(date -u +"%Y-%m-%dT%H:%M:%SZ")"
log "Hostname: $(hostname)"

if [[ -z "${CONNECTION_STRING:-}" ]]; then
  log "ERROR: CONNECTION_STRING is not set"
  exit 1
fi

# Safe-ish debug: print only non-sensitive fields if present
log "CONNECTION_STRING present: YES"
echo "$CONNECTION_STRING" | grep -oE 'Server=[^;]+' 2>/dev/null | sed 's/^/[init-migrations] /' || true
echo "$CONNECTION_STRING" | grep -oE 'Database=[^;]+' 2>/dev/null | sed 's/^/[init-migrations] /' || true

BUNDLE="/sql/migratedb"
if [[ ! -f "$BUNDLE" ]]; then
  log "ERROR: migration bundle not found at $BUNDLE"
  log "Listing /sql:"
  ls -la /sql | sed 's/^/[init-migrations] /' || true
  exit 1
fi

chmod +x "$BUNDLE" || true

log "Running migration bundle: $BUNDLE"
"$BUNDLE" --connection "$CONNECTION_STRING"

log "Migration completed successfully"
