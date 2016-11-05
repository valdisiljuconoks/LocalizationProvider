param($installPath, $toolsPath, $package, $project)

$projectFile = Get-Item $project.FullName

#locate the project configuration file
$webConfigPath = join-path $projectFile.Directory.FullName "web.config"

#load the configuration file for the project
$config = New-Object xml
$config.Load($webConfigPath)

$node = $xml.SelectSingleNode("/configuration/episerver.framework")

Write-Host "Node: " + $node.Name

# $config.Save($webConfigPath)

#$parentNode = $xml.SelectSingleNode("system.web/pages")
#$childNode = $xml.CreateElement("add")
#$childNode.SetAttribute("namespace", "SimpleCustomControl")
#$childNode.SetAttribute("assembly", "SimpleCustomControl")
#$childNode.SetAttribute("tagPrefix", "custom")
#$parentNode.AppendChild($childNode)


#$node.ParentNode.RemoveChild($node)


#if ($webConfig.configuration.connectionStrings.configSource -ne $null) 
#{
#	if ([System.IO.Path]::IsPathRooted($webConfig.configuration.connectionStrings.configSource))
#	{
#		return GetEPiServerConnectionString($webConfig.configuration.connectionStrings.configSource)
#	}
#	else
#	{
#		return GetEPiServerConnectionString (Join-path  ([System.IO.Path]::GetDirectoryName($WebConfigFile))  $webConfig.configuration.connectionStrings.configSource)
#	}
#}
