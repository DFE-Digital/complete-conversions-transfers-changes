# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/). To see an example from a mature product in the program [see the Complete products changelog that follows the same methodology](https://github.com/DFE-Digital/dfe-complete-conversions-transfers-and-changes/blob/main/CHANGELOG.md).

Added for new features.  
Changed for changes in existing functionality.  
Fixed for any bug fixes.  
Removed for now removed features.  
Deprecated for soon-to-be removed features.  
Security in case of vulnerabilities.  

---

## Unreleased  
### Added
 - New route `/projects/team/in-progress`
 - New route `/projects/team/completed`
 - Your team projects in progress list (`/projects/team/in-progress`)
 - Your team projects completed list (`/projects/team/completed`)
 - Added user (`ClaimsPrincipal`) extension to get users team `GetUserTeam`
 - Added endpoint to the projectsController `ListAllProjectsInTrust`-`/v1/Projects/List/Trust`

### Changed
 - Updated route `/accessibility-statement` to `/accessibility`
 - Updated route `/public/cookies` to `/cookies`
 - Updated route `/projects/transfer/new_mat` to `/projects/transfers/new_mat`
 - Updated route `/projects/transfer-projects/new` to `/projects/transfers/new`
 - Updated ListAllProjects in-progress and Count all projects in progress to filter out unassigned projects

### Fixed
 - Correctly identify test env based on environment name being "Test" (previously looking for "Staging")
 - Added WireMock support back

See the [full commit history](https://github.com/DFE-Digital/complete-conversions-transfers-changes/compare/main...production-2025-04-01.120-manual) for everything awaiting release

---

## [Release-1](https://github.com/DFE-Digital/complete-conversions-transfers-changes/releases/tag/production-2025-04-01.120-manual) - 2025-04-01
### Added
 - Another 'sync' release to bring the changelog up to date

See the [full commit history](https://github.com/DFE-Digital/complete-conversions-transfers-changes/compare/development-2025-03-05.78...production-2025-04-01.120-manual) for everything in the release

---

## [Release-0](https://github.com/DFE-Digital/complete-conversions-transfers-changes/compare/082ba69cfa1b5b098d5dd5e2c804e8f5c58c2a00...development-2025-03-05.78) - 2025-03-28

### Added
 - Initial changelog setup to match the current production state.
 - Captures prior production releases retroactively, for syncing purposes.

See the [full commit history](https://github.com/DFE-Digital/complete-conversions-transfers-changes/compare/082ba69cfa1b5b098d5dd5e2c804e8f5c58c2a00...development-2025-03-05.78) for everything in the release
