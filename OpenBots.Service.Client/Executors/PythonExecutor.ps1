Write-Output "[1/6] Setting parameter values."
$python = $args[0]
$projectDir = $args[1]
$scriptPath = $args[2]
$venvPath = $projectDir + "\.env3\\"
$activatePath = $projectDir + "\.env3\Scripts\activate.ps1"
$venvExp = "$python -m venv " + "`'$venvPath`'"

Write-Output "[2/6] Creating the virtual environment."
Invoke-Expression "$python -m pip install --upgrade pip"
Invoke-Expression "$python -m pip install --user virtualenv"
Invoke-Expression $venvExp
& $activatePath

Write-Output "[3/6] Retrieving dependencies."
pip install -r "$projectDir\requirements.txt"

Write-Output "[4/6] Executing the specified script."
Invoke-Expression "$python `'$scriptPath`'"

Write-Output "[5/6] Deactivating the virtual environment."
deactivate

Write-Output "[6/6] Execution completed."