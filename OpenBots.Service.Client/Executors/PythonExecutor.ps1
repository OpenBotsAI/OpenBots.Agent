Write-Output "[1/6] Deactivating the virtual environment."
$python = $args[0]
$projectDir = $args[1]
$scriptPath = $args[2]

#Debug statements######
$python = "C:\Users\AccelirateAdmin\AppData\Local\Programs\Python\Python39\python.exe"
$projectDir = "C:\Users\AccelirateAdmin\Desktop\TestPythonProject2"
$scriptPath = "C:\Users\AccelirateAdmin\Desktop\TestPythonProject2\__main__.py"
#######################

Write-Output "[2/6] Creating the virtual environment."
Invoke-Expression "$python -m pip install --upgrade pip"
Invoke-Expression "$python -m pip install --user virtualenv"
Invoke-Expression "$python -m venv $projectDir\.env3\"
Invoke-Expression "$projectDir\.env3\Scripts\activate.ps1"

Write-Output "[3/6] Retrieving dependencies."
pip install --upgrade pip
pip install -r "$projectDir\requirements.txt"

Write-Output "[4/6] Executing the specified script."
Invoke-Expression "$python $scriptPath"

Write-Output "[5/6] Deactivating the virtual environment."
deactivate

Write-Output "[6/6] Execution completed."