# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/). To see an example from a mature product in the program [see the Complete products changelog that follows the same methodology](https://github.com/DFE-Digital/dfe-complete-conversions-transfers-and-changes/blob/main/CHANGELOG.md).


## Notes
### Releases
 - Unreleased: Developed and tested, has not yet deployed to prod
 - [TEST]/[PROD]: Feature/fix is considered ready for dual running in this environment

### Statuses
Added for new features.  
Changed for changes in existing functionality.  
Fixed for any bug fixes.  
Removed for now removed features.  
Deprecated for soon-to-be removed features.  
Security in case of vulnerabilities.  

---

## Unreleased  

### Added  
- Internal contacts page and edit pages `/projects/{projectId}/internal-contacts`
- Add notes repository
- Add `GetNotesByProjectId` query and handler
- Add `GetNoteById` query and handler
- Add `UpdateNote` command and handler
- Add `CreateNote` command and handler
- Add project notes page (`/projects/{projectId}/notes`)
- Add project notes editing page (`/projects/{projectId}/notes/{noteId}/edit`)
- Add project notes creation page (`/projects/{projectId}/notes/new`)
- Add ability to delete note (`/projects/{projectId}/notes/{noteId}/delete`)
- Attach user ID from DB as custom claim

### Changed  

### Fixed

See the [full commit history](https://github.com/DFE-Digital/complete-conversions-transfers-changes/compare/production-2025-06-27.463...main) for everything awaiting release

---

## [1.7.0](https://github.com/DFE-Digital/complete-conversions-transfers-changes/releases/tag/production-2025-06-27.463) - 2025-06-27

### Added
- Internal contacts page and edit pages `/projects/{projectId}/internal-contacts`
- app settings for test environment

### Fixed
- footer links for production
- privacy link
- show 'service not working' on unexpected error

See the [full commit history](https://github.com/DFE-Digital/complete-conversions-transfers-changes/compare/production-2025-06-24.451...production-2025-06-27.463) for everything in the release
---

## [1.6.2](https://github.com/DFE-Digital/complete-conversions-transfers-changes/releases/tag/production-2025-06-24.451) - 2025-06-24

### Fixed
- Notification banner was not showing as cookie banner was clearing TempData

See the [full commit history](https://github.com/DFE-Digital/complete-conversions-transfers-changes/compare/production-2025-06-23.444...production-2025-06-24.451) for everything in the release
---

## [1.6.1](https://github.com/DFE-Digital/complete-conversions-transfers-changes/releases/tag/production-2025-06-23.444) - 2025-06-23

### Fixed  
- Fixed incoming trust displaying as `None`
- update exports tab to point at correct page
- Fixed project creation path.

See the [full commit history](https://github.com/DFE-Digital/complete-conversions-transfers-changes/compare/production-2025-06-17.413...production-2025-06-23.444) for everything in the release
---

## [1.6.0](https://github.com/DFE-Digital/complete-conversions-transfers-changes/releases/tag/production-2025-06-17.413) - 2025-05-30

### Fixed  
- Updated pagination query parameter from `pageNumber` to `page` to match Ruby app
- Optimised queries behind the "By local authority" page
- Show a `Page Not Found` error if the requested page number exceeds the total number of available pages.
- Fixed the search functionality to return only projects with status values of 0, 1, or 3 (Active, Completed or DAO revoked)
- Fixed unable to set cookies issue if request is coming from ruby app

See the [full commit history](https://github.com/DFE-Digital/complete-conversions-transfers-changes/compare/production-2025-05-30.320...production-2025-06-17.413) for everything in the release

---

## [1.5.4](https://github.com/DFE-Digital/complete-conversions-transfers-changes/releases/tag/production-2025-05-30.320) - 2025-05-30
### Added  
- `PolicyCheckTagHelper` added to conditionally hide elements based on policy
- Query builder in the infrastructure layer to help support custom queries

### Changed  
- Navigation items previously hidden with `UserTabAccessHelper` now hide on policy

### Fixed  
- Unassigned projects should show "Not yet assigned" under "Assigned To" column for projects on the local authority/trust pages
- Optimised queries behind the "By month" and "For trust" listing pages

### Removed
- `UserTabAccessHelper` class is no longer required. Use policies instead

### Security
- Only correct user groups can now create projects

See the [full commit history](https://github.com/DFE-Digital/complete-conversions-transfers-changes/compare/production-2025-05-22.290...production-2025-05-30.320) for everything in the release

---

## [1.5.3](https://github.com/DFE-Digital/complete-conversions-transfers-changes/releases/tag/production-2025-05-22.290) - 2025-05-22
### Added  
 - Enabled error tracking via Application Insights.
 - New route `/projects/team/unassigned`
 - Your team projects "Unassigned" list (`/projects/team/unassigned`)

### Changed  
- Sort all projects by region list alphabetically

### Fixed  
- Note FK Ids are now required
- Separated created and assigned users in project creation

See the [full commit history](https://github.com/DFE-Digital/complete-conversions-transfers-changes/compare/production-2025-05-16.272...production-2025-05-22.290) for everything in the release

---

## [1.5.2](https://github.com/DFE-Digital/complete-conversions-transfers-changes/releases/tag/production-2025-05-16.272) - 2025-05-16

### Changed
- Optimise several project listing queries by implementing pagination before retrieving records

### Fixed  
- Fixed identifying "Form A MAT" projects logic
- Removed unnecessary `Assign To` filter while pulling projects from database.
- Resolve accessibility issue causing app header to appear blue instead of white
- Removed `Project Status` filter while pullling search results.  

See the [full commit history](https://github.com/DFE-Digital/complete-conversions-transfers-changes/compare/production-2025-05-14.254...production-2025-05-16.272) for everything in the release

---

## [1.5.1](https://github.com/DFE-Digital/complete-conversions-transfers-changes/releases/tag/production-2025-05-14.254) - 2025-05-14

### Changed
 - Change date format to "Month Year" string on local authority projects list
 - Change projects to sort by significant date on local authority projects list

### Fixed  
 - Resolve project pagination issue on "Team projects" > "By User" > User page

### Security
 - Authorization fixed on all API endpoints

See the [full commit history](https://github.com/DFE-Digital/complete-conversions-transfers-changes/compare/production-2025-05-13.244...production-2025-05-14.254) for everything in the release

---

## [1.5.0](https://github.com/DFE-Digital/complete-conversions-transfers-changes/releases/tag/production-2025-05-13.244) - 2025-05-13
### Added  
 - Added "order by field" argument to `GetUserWithProjectsQuery`
 - Added search bar to search projects with active status

### Changed  
 - Merged `ListAllUsersInTeamWithProjectsQuery` into `ListAllUsersWithProjectsQuery` with filter
 - Order "Team projects" > "By User" by significant date
 - Filter "Team projects" > "Handed over" to active projects only

### Fixed  
 - Routing for projects merged (`/conversion-project` and `/transfer-project` become `/project`)
 - "Team projects" > "Handed over" now shows unassigned projects again


See the [full commit history](https://github.com/DFE-Digital/complete-conversions-transfers-changes/compare/production-2025-05-08.217...production-2025-05-13.244) for everything in the release

---

## [1.4.0](https://github.com/DFE-Digital/complete-conversions-transfers-changes/releases/tag/production-2025-05-08.217) - 2025-05-08
### Added
 - New route `/projects/all/by-month/conversions/{month}/{year}`
 - New route `/projects/all/by-month/transfers/{month}/{year}`
 - New route `/projects/all/by-month/conversions/from/{fromMonth}/{fromYear}/to/{toMonth}/{toYear}`
 - New route `/projects/all/by-month/transfers/from/{fromMonth}/{fromYear}/to/{toMonth}/{toYear}`
 - New route `/projects/{projectId}/tasks`
 - New route `/projects/team/new`
 - New route `/projects/team/handed-over`
 - New route `/projects/team/users`
 - New route `/projects/team/users/{userId}`
 - Your team projects "New" list (`/projects/team/new`)
 - Your team projects "Handed over" list (`/projects/team/handed-over`)
 - Your team projects "By user" list (`/projects/team/users`)
 - Your team projects "By user" > "User" list (`/projects/team/users/{userId}`)
 - Add new `ProjectTeam` extension method `TeamIsRegionalCaseworkServices`, to identify RCS users 
 - Projects added by you (`/projects/yours/added-by`)
 - Projects completed by you (`/projects/yours/completed`)

### Changed
 - Merged ListAllProjectsByFilter into main ListAllProjects query
 - Add an "orderBy" argument to the `ListAllProjectsByFilter` query
 - Allow `ListAllProjectsByFilter` query to handle multiple filters
 - All transfer/conversion projects list use a partial
 - Projects will route to `/project/{projectId}/tasks` from all projects list

### Fixed
 - Project for user list should show month and year (not day)

See the [full commit history](https://github.com/DFE-Digital/complete-conversions-transfers-changes/compare/production-2025-04-24.175...production-2025-05-08.217) for everything in the release

---

## [1.3.0](https://github.com/DFE-Digital/complete-conversions-transfers-changes/releases/tag/production-2025-04-24.175) - 2025-04-25
### Added
 - New route `/projects/team/completed`
 - Your team projects completed list (`/projects/team/completed`)


### Changed
 - Filter out any local authorities with no projects in `ListAllProjectByLocalAuthorities`
 - Include unassigned projects in "All projects" > "By region"

See the [full commit history](https://github.com/DFE-Digital/complete-conversions-transfers-changes/compare/production-2025-04-17.164...production-2025-04-24.175) for everything in the release

---

## [1.2.0](https://github.com/DFE-Digital/complete-conversions-transfers-changes/releases/tag/production-2025-04-17.164) - 2025-04-17
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
 - New route `/projects/all/in-progress/form-a-multi-academy-trust`
 - New route `/form-a-multi-academy-trust/{reference}`
 - Form a MAT with projects in progress list (`/projects/all/in-progress/form-a-multi-academy-trust`)
 - MAT projects listing related establishments (`/form-a-multi-academy-trust/{reference}`)
 - Added missing "project for region" header

 - User redirection on app load based on their permissions
 - Add navigation items to be more consistent with ruby UI
 - New route `/projects/team/users`
 - Your team projects by user list (`/projects/team/users`)
 - Your team projects by user query `ListAllUsersInTeamWithProjectsQuery`

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
 - Show 404 page when get projects for region `/projects/all/regions/{region}` has a "bad" region in path param
 - null `AssignedTo` in `ListAllProjects` throws an unexpected error

See the [full commit history](https://github.com/DFE-Digital/complete-conversions-transfers-changes/compare/production-2025-04-01.120-manual...production-2025-04-17.164) for everything in the release

---

## [1.1.0](https://github.com/DFE-Digital/complete-conversions-transfers-changes/releases/tag/production-2025-04-01.120-manual) - 2025-04-01
### Added
 - Another 'sync' release to bring the changelog up to date

See the [full commit history](https://github.com/DFE-Digital/complete-conversions-transfers-changes/compare/development-2025-03-05.78...production-2025-04-01.120-manual) for everything in the release

---

## [1.0.0](https://github.com/DFE-Digital/complete-conversions-transfers-changes/compare/082ba69cfa1b5b098d5dd5e2c804e8f5c58c2a00...development-2025-03-05.78) - 2025-03-28

### Added
 - Initial changelog setup to match the current production state.
 - Captures prior production releases retroactively, for syncing purposes.

See the [full commit history](https://github.com/DFE-Digital/complete-conversions-transfers-changes/compare/082ba69cfa1b5b098d5dd5e2c804e8f5c58c2a00...development-2025-03-05.78) for everything in the release
