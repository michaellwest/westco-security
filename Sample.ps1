$ErrorActionPreference = 'Stop'
$ScriptPath = Split-Path $MyInvocation.MyCommand.Path
$MicroCHAP = $ScriptPath + '\MicroCHAP.dll'
if(-not(Test-Path -Path $MicroCHAP)) {
    $MicroCHAP = Get-ChildItem -Path $ScriptPath -Filter "MicroCHAP.dll" | Select-Object -First 1 -ExpandProperty FullName
}

Add-Type -Path $MicroCHAP

function Get-Challenge {
	param(
		[Parameter(Mandatory=$true)]
		[string]$InstanceUrl
	)

	$url = "$($InstanceUrl)/sitecore/api/westco/auth/challenge"

	$result = Invoke-WebRequest -Uri $url -TimeoutSec 360 -UseBasicParsing

	$result.Content | ConvertFrom-Json | Select-Object -ExpandProperty challenge
}

function Get-Result {
	param(
		[Parameter(Mandatory=$true)]
		[string]$ApiUrl,

        [Parameter(Mandatory=$true)]
        [string]$Challenge
	)

    $signatureService = New-Object MicroCHAP.SignatureService -ArgumentList $SharedSecret
    $signature = $signatureService.CreateSignature($challenge, $query, $null)
    $result = Invoke-WebRequest -Uri $query -Headers @{ "X-MC-MAC" = $signature.SignatureHash; "X-MC-Nonce" = $challenge; } -TimeoutSec 10800 -UseBasicParsing

    $result.Content
}

$instanceUrl = "https://sc82u5.dev.local"
$sharedSecret = "cpfwrKUUcefNam36zwZQpmjk2342342342342432342fsgfsgd"

$challenge = Get-Challenge -InstanceUrl $instanceUrl
$query = $instanceUrl + "/sitecore/api/ssc/item/{110D559F-DEA5-42EA-9C1C-8A5DF7E70EF9}"
Get-Result -ApiUrl $query -Challenge $challenge
