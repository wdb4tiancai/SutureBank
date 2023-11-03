
set LUBAN_DLL=.\Luban\Luban.dll
set CONF_ROOT=.\Datas
set UNITY_PATH=..\Assets

dotnet %LUBAN_DLL% ^
    -t client ^
    -c cs-bin   ^
    -d bin  ^
	--customTemplateDir  .\Luban\customTemplate ^
    --conf %CONF_ROOT%\..\luban.conf ^
    -x outputCodeDir=%UNITY_PATH%\HotScripts\Config\Gen^
    -x outputDataDir=%UNITY_PATH%\HotAssets\Config

pause