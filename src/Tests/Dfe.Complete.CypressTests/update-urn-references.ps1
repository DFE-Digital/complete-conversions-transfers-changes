# PowerShell script to update URN references in Cypress tests
# This script removes the .value wrapper from URN properties

$cypressPath = ".\cypress"

Write-Host "Updating URN references in Cypress tests..." -ForegroundColor Green

# Get all TypeScript files in the cypress directory
$files = Get-ChildItem -Path $cypressPath -Filter "*.ts" -Recurse

$totalUpdated = 0

foreach ($file in $files) {
    Write-Host "Processing: $($file.Name)" -ForegroundColor Yellow
    
    $content = Get-Content $file.FullName -Raw
    $originalContent = $content
    
    # Replace URN creation patterns - all ProjectBuilder create methods
    # urn: { value: urnPool.something } -> urn: urnPool.something
    $content = $content -replace 'urn: \{ value: (urnPool\.[^}]+) \}', 'urn: $1'
    
    # Replace UKPRN creation patterns  
    # incomingTrustUkprn: { value: number } -> incomingTrustUkprn: number
    $content = $content -replace 'incomingTrustUkprn: \{ value: ([^}]+) \}', 'incomingTrustUkprn: $1'
    $content = $content -replace 'outgoingTrustUkprn: \{ value: ([^}]+) \}', 'outgoingTrustUkprn: $1'
    
    # Replace URN access patterns  
    # project.urn.value -> project.urn
    # transferProject.urn.value -> transferProject.urn
    # etc. (any variable name + .urn.value)
    $content = $content -replace '(\w+)\.urn\.value', '$1.urn'
    
    # Replace access patterns for UKPRN
    $content = $content -replace '(\w+)\.incomingTrustUkprn\.value', '$1.incomingTrustUkprn'
    $content = $content -replace '(\w+)\.outgoingTrustUkprn\.value', '$1.outgoingTrustUkprn'
    
    # Replace other common property access patterns that might exist
    $content = $content -replace '(\w+)\.newTrustReferenceNumber\.value', '$1.newTrustReferenceNumber'
    $content = $content -replace '(\w+)\.newTrustName\.value', '$1.newTrustName'
    
    # Remove obsolete properties that no longer exist in the new interface
    # Remove userAdId lines completely (this property doesn't exist in new interfaces)
    $content = $content -replace '    userAdId: [^,\n]+,?\n?', ''
    $content = $content -replace ',\s*userAdId: [^,\n}]+', ''
    
    # Remove significantDate and replace with appropriate date fields
    # This is complex because we need to know the context (conversion vs transfer)
    # For now, we'll comment these out and let developers fix manually
    $content = $content -replace 'significantDate:', '// TODO: Replace with provisionalConversionDate or provisionalTransferDate - significantDate:'
    
    # Remove other obsolete properties
    $content = $content -replace '    isSignificantDateProvisional: [^,\n]+,?\n?', ''
    $content = $content -replace '    isDueTo2Ri: [^,\n]+,?\n?', ''
    $content = $content -replace '    hasAcademyOrderBeenIssued: [^,\n]+,?\n?', ''
    $content = $content -replace '    establishmentSharepointLink: [^,\n]+,?\n?', ''
    $content = $content -replace '    incomingTrustSharepointLink: [^,\n]+,?\n?', ''
    $content = $content -replace '    outgoingTrustSharepointLink: [^,\n]+,?\n?', ''
    $content = $content -replace '    handingOverToRegionalCaseworkService: [^,\n]+,?\n?', ''
    $content = $content -replace '    handoverComments: [^,\n]+,?\n?', ''
    $content = $content -replace '    isDueToInedaquateOfstedRating: [^,\n]+,?\n?', ''
    $content = $content -replace '    isDueToIssues: [^,\n]+,?\n?', ''
    $content = $content -replace '    outGoingTrustWillClose: [^,\n]+,?\n?', ''
    $content = $content -replace '    groupReferenceNumber: [^,\n]+,?\n?', ''
    
    # Clean up trailing commas and extra whitespace
    $content = $content -replace ',(\s*\})', '$1'
    $content = $content -replace ',(\s*\n\s*\})', '$1'
    
    # Write back to file if content changed
    if ($content -ne $originalContent) {
        Set-Content -Path $file.FullName -Value $content -NoNewline
        Write-Host "  ✓ Updated" -ForegroundColor Green
        $totalUpdated++
    } else {
        Write-Host "  - No changes needed" -ForegroundColor Gray
    }
}

Write-Host "`nUpdate complete!" -ForegroundColor Green
Write-Host "Updated $totalUpdated files." -ForegroundColor Green
Write-Host "`nIMPORTANT: Please review the following:" -ForegroundColor Yellow
Write-Host "1. Search for '// TODO:' comments for significantDate replacements" -ForegroundColor Yellow  
Write-Host "2. Convert significantDate to provisionalConversionDate or provisionalTransferDate as appropriate" -ForegroundColor Yellow
Write-Host "3. Add missing required properties like createdByEmail, createdByFirstName, etc." -ForegroundColor Yellow
Write-Host "4. Run 'npx tsc --noEmit' to check for TypeScript compilation errors" -ForegroundColor Yellow
Write-Host "5. Test key Cypress tests to ensure they work correctly" -ForegroundColor Yellow