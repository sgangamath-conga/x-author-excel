(Get-Content AppBuilderWsProxy.cs) | 
Foreach-Object {
$_ -replace ", Order=.", "" `
       -replace "Order=.", ""
} | 
Set-Content AppBuilderWsProxy.cs