$ScriptPath = Split-Path $MyInvocation.MyCommand.Path
$MicroCHAP = $ScriptPath + '\MicroCHAP.dll'

$publishPayload = @{
    "ItemId" = ""
    "IncludeDescendantItems" = $true
    "IncludeRelatedItems" = $true
    "Languages" = @(@{"Code" = "en"; "DisplayName" = "English"})
    "Targets" = @(@{"Id" = "Internet"; "Name" = "Internet"})
    "sourceDatabase" = "master"
} | ConvertTo-Json

$publishPayloadAll = @{
    "ItemId" = "{11111111-1111-1111-1111-111111111111}"
    "IncludeDescendantItems" = $true
    "SyncWithTarget" = $false
    "Languages" = @(@{"Code" = "en"; "DisplayName" = "English"})
    "Targets" = @(@{"Id" = "Internet"; "Name" = "Internet"})
    "sourceDatabase" = "master"
    "metadata" = @{
        "Publish.Options.ClearAllCaches" = $false
        "Publish.Options.Republish" = $false
    }
} | ConvertTo-Json

$secret = "Patch-This-Value-With-Something-Other-Than-This"

$instanceUrl = "https://scms.dev.sc.local" # Replace with your site Url

Write-Host "Publishing the site" -ForegroundColor Yellow
$publishUrl = "$($instanceUrl)/sitecore/api/ssc/publishing/jobs/0/FullPublish"
$challenge = Get-SscChallenge -InstanceUrl $instanceUrl
Invoke-SscRequest -Url $publishUrl -SharedSecret $secret -Challenge $challenge -Payload $publishPayload | ConvertFrom-Json

Write-Host "Getting the home item" -ForegroundColor Yellow
$challenge = Get-SscChallenge -InstanceUrl $instanceUrl
$query = $instanceUrl + "/sitecore/api/ssc/item/{110D559F-DEA5-42EA-9C1C-8A5DF7E70EF9}"
Get-SscResult -Url $query -SharedSecret $secret -Challenge $challenge | ConvertFrom-Json