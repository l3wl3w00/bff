Write-Host "`nStarting Angular Client1..."
Start-Process "cmd" -ArgumentList "/k npm start" -WorkingDirectory ".\BffDemo.Bff1\BffDemo.Client1" -PassThru

Write-Host "`nStarting Angular Client2..."
Start-Process "cmd" -ArgumentList "/k npm start" -WorkingDirectory ".\BffDemo.Bff2\BffDemo.Client2" -PassThru

Write-Host "`nStarting No Bff Angular Client..."
Start-Process "cmd" -ArgumentList "/c npm start" -WorkingDirectory ".\BffDemo.NoBffApplication\BffDemo.NoBffClient" -PassThru

Start-Process "cmd" -ArgumentList "/c start http://bff-client-1.test:4201"
Start-Process "cmd" -ArgumentList "/c start http://bff-client-2.test:4202"
Start-Process "cmd" -ArgumentList "/c start http://no-bff-client.test:4203"
