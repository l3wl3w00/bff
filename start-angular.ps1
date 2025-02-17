Write-Host "`nStarting Angular Client1..."
Start-Process "cmd" -ArgumentList "/k npm start" -WorkingDirectory ".\BffDemo.Bff1\BffDemo.Client1" -PassThru

Write-Host "`nStarting Angular Client2..."
Start-Process "cmd" -ArgumentList "/k npm start" -WorkingDirectory ".\BffDemo.Bff2\BffDemo.Client2" -PassThru

Start-Process "cmd" -ArgumentList "/c start http://localhost:4200"
Start-Process "cmd" -ArgumentList "/c start http://localhost:4201"
