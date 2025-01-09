<# :
@chcp 65001 > nul
@set args=%*
@powershell -c "iex((cat '%~f0' -Encoding UTF8) -join [Environment]::NewLine)"
@exit /b %ERRORLEVEL%
#>

$file = $env:args
$file = $file -replace '"(.*)"', '$1'
$filedata = @(Get-Content -Encoding UTF8 $file)
$filedir = Split-Path -Path $file

$assembles = @('System.Windows.Forms', 'System.Drawing')
$outputexename =  ($filedir + '\ScriptCSOutput.exe')

Add-Type ($filedata -join [Environment]::NewLine) -ReferencedAssemblies $assembles -OutputType ConsoleApplication -OutputAssembly $outputexename
if (Test-Path $outputexename) {
    Start-Process $outputexename -Wait -NoNewWindow -PassThru | Out-Null
    Remove-Item $outputexename
}
