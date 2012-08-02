@echo off
@echo.
@echo -----------------------------
@echo Roar SDK - Unity (building...)
@echo -----------------------------
set "API_FUNCTION_JSON=../api_functions.json"
@echo.
@echo -----------------------------
@echo Checking config json
@echo -----------------------------
@echo.
REM TODO: fix execution of jsonlint, uncomment and it will not complete execution of script
REM jsonlint "%API_FUNCTION_JSON%" -c > NUL
@echo.
@echo -----------------------------
@echo Building unity from template
@echo -----------------------------
@echo.
node template.js
@echo.
@echo -----------------------------
@echo Done
@echo -----------------------------
@echo.