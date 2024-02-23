wait 10
:again
taskkill /F /IM %1
if errorlevel=0 goto end
if errorlevel=1 goto again
:end