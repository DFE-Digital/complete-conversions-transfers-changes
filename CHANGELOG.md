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
 - New route `/projects/team/completed`
 - Your team projects completed list (`/projects/team/completed`)

### Changed
 - Filter out any local authorities with no projects in `ListAllProjectByLocalAuthorities`

 
See the [full commit history](https://github.com/DFE-Digital/complete-conversions-transfers-changes/compare/main...production-2025-04-17.164) for everything awaiting release

---

## [Release-2](https://github.com/DFE-Digital/complete-conversions-transfers-changes/releases/tag/production-2025-04-17.164) - 2025-04-17
### Added
 - New route `/projects/team/in-progress`
 - Your team projects in progress list (`/projects/team/in-progress`)
 - Added user (`ClaimsPrincipal`) extension to get users team `GetUserTeam`
 - Added API endpoint `/v1/Projects/List/All/LocalAuthority` for fetching projects for local authority
 - Added API endpoint `/v1/Projects/List/All/Region` for fetching projects for region
 - Added API endpoint `/v1/Projects/List/All/Team` for fetching projects for team
 - Added API endpoint `/v1/Projects/List/All/User` for fetching projects for user
 - Added endpoint to the projectsController `ListAllProjectsInTrust`-`/v1/Projects/List/Trust`
 - Added missing "project for region" header
 - User redirection on app load based on their permissions
 - Add navigation items to be more consistent with ruby UI
 - New route `/projects/all/by-month/conversions/{month}/{year}`
 - New route `/projects/all/by-month/transfers/{month}/{year}`
 - New route `/projects/all/by-month/conversions/from/{fromMonth}/{fromYear}/to/{toMonth}/{toYear}`
 - New route `/projects/all/by-month/transfers/from/{fromMonth}/{fromYear}/to/{toMonth}/{toYear}`

### Changed
 - Updated route `/accessibility-statement` to `/accessibility`
 - Updated route `/public/cookies` to `/cookies`
 - Updated route `/projects/transfer/new_mat` to `/projects/transfers/new_mat`
 - Updated route `/projects/transfer-projects/new` to `/projects/transfers/new`
 - Updated ListAllProjects in-progress and Count all projects in progress to filter out unassigned projects
 - Don't filter unassigned projects for "All projects by region" -> Region
 - Move tab access logic to a helper `UserTabAccessHelper`

### Fixed
 - Correctly identify test env based on environment name being "Test" (previously looking for "Staging")
 - Added WireMock support back
 - Show 404 page when get projects for region `/projects/all/regions/{region}` has a "bad" region in path param
 - null `AssignedTo` in `ListAllProjects` throws an unexpected error

See the [full commit history](https://github.com/DFE-Digital/complete-conversions-transfers-changes/compare/production-2025-04-01.120-manual...production-2025-04-17.164) for everything in the release

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
