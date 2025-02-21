# Global arrays to track processes
$global:allProcesses = @()
$global:dotnetProcesses = @()

#--------------------------------------------------------------------
function Build-Solution {
    Write-Host "Building .NET solution..."
    dotnet build .\BffDemo.sln
}

#--------------------------------------------------------------------
function Start-DotNetProcesses {
    Write-Host "`nStarting IdentityServer..."
    $global:dotnetProcesses += Start-Process "dotnet" -ArgumentList "run --launch-profile SelfHost" -WorkingDirectory ".\BffDemo.IdentityServer" -PassThru

    Write-Host "`nStarting the .NET Back End..."
    $global:dotnetProcesses += Start-Process "dotnet" -ArgumentList "run --launch-profile default" -WorkingDirectory ".\BffDemo.Backend" -PassThru

    Write-Host "`nStarting Bff1..."
    $global:dotnetProcesses += Start-Process "dotnet" -ArgumentList "run --launch-profile default" -WorkingDirectory ".\BffDemo.Bff1" -PassThru

    Write-Host "`nStarting Bff2..."
    $global:dotnetProcesses += Start-Process "dotnet" -ArgumentList "run --launch-profile default" -WorkingDirectory ".\BffDemo.Bff2" -PassThru
    
    Write-Host "`nStarting NoBffApp..."
    $global:dotnetProcesses += Start-Process "dotnet" -ArgumentList "run --launch-profile default" -WorkingDirectory ".\BffDemo.NoBffApplication" -PassThru

    # Add .NET processes to the global process list
    $global:allProcesses += $global:dotnetProcesses
}

#--------------------------------------------------------------------
function Start-AngularClients {
    Write-Host "`nStarting Bff Angular Client1..."
    $global:allProcesses += Start-Process "cmd" -ArgumentList "/k npm start" -WorkingDirectory ".\BffDemo.Bff1\BffDemo.Client1" -PassThru

    Write-Host "`nStarting Bff Angular Client2..."
    $global:allProcesses += Start-Process "cmd" -ArgumentList "/k npm start" -WorkingDirectory ".\BffDemo.Bff2\BffDemo.Client2" -PassThru

    Write-Host "`nStarting No Bff Angular Client..."
    $global:allProcesses += Start-Process "cmd" -ArgumentList "/k npm start" -WorkingDirectory ".\BffDemo.NoBffApplication\BffDemo.NoBffClient" -PassThru
}

#--------------------------------------------------------------------
function Open-BrowserApps {
    Write-Host "`nWaiting 3 seconds before opening the browser..."
    Start-Sleep -Seconds 3

    Write-Host "Opening Angular apps in the browser..."
    Start-Process "cmd" -ArgumentList "/c start http://bff-client-1.test:4201"
    Start-Process "cmd" -ArgumentList "/c start http://bff-client-2.test:4202"
}

#--------------------------------------------------------------------
function Stop-AllProcesses {
    Write-Host "Stopping all processes..."
    $global:allProcesses | ForEach-Object {
        try {
            Stop-Process -Id $_.Id -ErrorAction SilentlyContinue
        } catch {
            # Ignore errors if process has already exited
        }
    }
    Write-Host "All processes stopped."
}

#--------------------------------------------------------------------
function Restart-DotNetProcesses {
    Write-Host "Restarting .NET projects..."
    # Stop current .NET processes
    $global:dotnetProcesses | ForEach-Object {
        try {
            Stop-Process -Id $_.Id -ErrorAction SilentlyContinue
        } catch {
            # Ignore errors if process already exited
        }
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
