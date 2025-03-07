# Global arrays to track processes
$global:angularProcesses = @()
$global:dotnetProcesses = @()

#--------------------------------------------------------------------
# Global arrays to track processes
$global:allProcesses = @()
$global:dotnetProcesses = @()

#--------------------------------------------------------------------
function Stop-Process {
    param(
        [Parameter(Mandatory=$true)]
        [Object]$Process,
        [int]$WaitTimeMs = 0
    )

    try {
        # First attempt to stop gracefully
        Microsoft.PowerShell.Management\Stop-Process -Id $Process.Id -ErrorAction SilentlyContinue
        Start-Sleep -Milliseconds $WaitTimeMs
        $stillRunning = Get-Process -Id $Process.Id -ErrorAction SilentlyContinue
        if ($stillRunning) {
            Write-Host "Process with Id $($Process.Id) did not stop gracefully. Forcing termination..."
            Microsoft.PowerShell.Management\Stop-Process -Id $Process.Id -Force -ErrorAction SilentlyContinue
        }
    } catch {
        # Ignore if it's already exited
    }

    # Double-check; if it's still running, kill the entire process tree
    $stillRunning = Get-Process -Id $Process.Id -ErrorAction SilentlyContinue
    if ($stillRunning) {
        Write-Host "Killing process tree for Id $($Process.Id) using taskkill..."
        Start-Process -FilePath "taskkill.exe" -ArgumentList "/PID $($Process.Id) /T /F" -NoNewWindow -Wait | Out-Null
    }
}

#--------------------------------------------------------------------
function Build-Solution {
    Write-Host "Building .NET solution..."
    dotnet build .\BffDemo.sln
}

#--------------------------------------------------------------------
function Start-DotNetProcesses {
    Write-Host "`nStarting IdentityServer..."
    $global:dotnetProcesses += Start-Process "dotnet" -ArgumentList "run --launch-profile default" -WorkingDirectory ".\BffDemo.IdentityServer" -PassThru

    Write-Host "`nStarting the .NET Back End..."
    $global:dotnetProcesses += Start-Process "dotnet" -ArgumentList "run --launch-profile default" -WorkingDirectory ".\BffDemo.Backend" -PassThru

    Write-Host "`nStarting Bff1..."
    $global:dotnetProcesses += Start-Process "dotnet" -ArgumentList "run --launch-profile default" -WorkingDirectory ".\BffDemo.Bff1" -PassThru

    Write-Host "`nStarting Bff2..."
    $global:dotnetProcesses += Start-Process "dotnet" -ArgumentList "run --launch-profile default" -WorkingDirectory ".\BffDemo.Bff2" -PassThru

    Write-Host "`nStarting NoBffApp..."
    $global:dotnetProcesses += Start-Process "dotnet" -ArgumentList "run --launch-profile default" -WorkingDirectory ".\BffDemo.NoBffApplication" -PassThru
}

#--------------------------------------------------------------------
function Start-AngularClients {
    Write-Host "`nStarting Bff Angular Client1..."
    $global:angularProcesses += Start-Process "cmd" -ArgumentList "/c npm start" -WorkingDirectory ".\BffDemo.Bff1\BffDemo.Client1" -PassThru

    Write-Host "`nStarting Bff Angular Client2..."
    $global:angularProcesses += Start-Process "cmd" -ArgumentList "/c npm start" -WorkingDirectory ".\BffDemo.Bff2\BffDemo.Client2" -PassThru

    Write-Host "`nStarting No Bff Angular Client..."
    $global:angularProcesses += Start-Process "cmd" -ArgumentList "/c npm start" -WorkingDirectory ".\BffDemo.NoBffApplication\BffDemo.NoBffClient" -PassThru
}

#--------------------------------------------------------------------
function Open-BrowserApps {
    Write-Host "`nWaiting 3 seconds before opening the browser..."
    Start-Sleep -Seconds 3

    Write-Host "Opening Angular apps in the browser..."
    Start-Process "cmd" -ArgumentList "/c start http://bff-client-1.test:4201"
    Start-Process "cmd" -ArgumentList "/c start http://bff-client-2.test:4202"
    Start-Process "cmd" -ArgumentList "/c start http://localhost:4203"
}

#--------------------------------------------------------------------
function Stop-AllProcesses {
    Write-Host "Stopping all processes..."
    $global:angularProcesses | ForEach-Object {
        Stop-Process $_ -WaitTimeMs 500
    }
    $global:dotnetProcesses | ForEach-Object {
        Stop-Process $_
    }
    Write-Host "All processes stopped."
}

#--------------------------------------------------------------------
function Restart-DotNetProcesses {
    Write-Host "Restarting .NET projects..."
    # Stop current .NET processes
    $global:dotnetProcesses | ForEach-Object {
        Stop-Process $_
    }
    # Clear the .NET process list
    $global:dotnetProcesses = @()

    Start-DotNetProcesses
    Write-Host "Restarted all .NET projects."
}

#--------------------------------------------------------------------
# Main Script Execution
Build-Solution
Start-DotNetProcesses
Start-AngularClients

Write-Host "`nAll processes have been started."
Open-BrowserApps

Write-Host "`nType 'q' then press Enter to stop all processes, or type 'r' to restart the .NET projects."
$running = $true
while ($running) {
    $userInput = Read-Host "Enter command (q to quit, r to restart .NET)"
    switch ($userInput) {
        'q' {
            Stop-AllProcesses
            $running = $false
        }
        'r' {
            Restart-DotNetProcesses
        }
        default {
            Write-Host "Invalid input. Type 'q' to quit or 'r' to restart the .NET projects."
        }
    }
}
