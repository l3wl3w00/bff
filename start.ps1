$processes = @()

Write-Host "Building .NET solution..."
dotnet build .\BffDemo.sln

Write-Host "`nStarting IdentityServer..."
$processes += Start-Process "dotnet" -ArgumentList "run --launch-profile SelfHost" -WorkingDirectory ".\BffDemo.IdentityServer" -PassThru

Write-Host "`nStarting the .NET Back End..."
$processes += Start-Process "dotnet" -ArgumentList "run --launch-profile https" -WorkingDirectory ".\BffDemo.Backend" -PassThru

Write-Host "`nStarting Bff1..."
$processes += Start-Process "dotnet" -ArgumentList "run --launch-profile https" -WorkingDirectory ".\BffDemo.Bff1" -PassThru

Write-Host "`nStarting Bff2..."
$processes += Start-Process "dotnet" -ArgumentList "run --launch-profile https" -WorkingDirectory ".\BffDemo.Bff2" -PassThru

Write-Host "`nStarting Angular Client1..."
$processes += Start-Process "cmd" -ArgumentList "/k npm start" -WorkingDirectory ".\BffDemo.Bff1\BffDemo.Client1" -PassThru

Write-Host "`nStarting Angular Client2..."
$processes += Start-Process "cmd" -ArgumentList "/k npm start" -WorkingDirectory ".\BffDemo.Bff2\BffDemo.Client2" -PassThru

Write-Host "`nAll processes have been started."

Write-Host "Waiting 3 seconds before opening the browser..."
Start-Sleep -Seconds 3

Write-Host "Opening Angular apps in the browser..."
Start-Process "cmd" -ArgumentList "/c start http://localhost:4200"
Start-Process "cmd" -ArgumentList "/c start http://localhost:4201"

Write-Host "Type 'q' then press Enter to stop all processes."

while ($true) {
    $userInput = Read-Host
    if ($userInput -eq 'q') {
        Write-Host "Stopping all processes..."
        $processes | ForEach-Object {
            try {
                Stop-Process -Id $_.Id -ErrorAction SilentlyContinue
            } catch {
                # Ignore errors if a process has already exited
            }
        }
        Write-Host "All processes stopped."
        break
    } else {
        Write-Host "Type 'q' to stop all processes, or press Ctrl+C to cancel."
    }
}
