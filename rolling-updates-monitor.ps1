
while ($true) {
    $Response = Invoke-WebRequest 'http://localhost:5000/api/version/' -Method 'GET'

    Write-Host 'Version: '$Response

    Start-Sleep -s 1
}