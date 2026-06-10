Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd '$PSScriptRoot\src\Services\Identity\Identity.API'; dotnet run"
Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd '$PSScriptRoot\src\Services\Catalog\Catalog.API'; dotnet run"
Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd '$PSScriptRoot\src\Services\Enrollment\Enrollment.API'; dotnet run"
Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd '$PSScriptRoot\src\Services\Notification\Notification.Worker'; dotnet run"
