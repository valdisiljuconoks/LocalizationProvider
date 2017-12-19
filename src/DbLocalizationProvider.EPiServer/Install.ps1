param($installPath, $toolsPath, $package, $project)

$projectFile = Get-Item $project.FullName

#locate the project configuration file
$webConfigPath = join-path $projectFile.Directory.FullName "web.config"

#load the configuration file for the project
$config = New-Object xml
$config.Load($webConfigPath)

$node = $xml.SelectSingleNode("/configuration/episerver.framework")
