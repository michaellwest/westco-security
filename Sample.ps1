$ErrorActionPreference = 'Stop'
$ScriptPath = Split-Path $MyInvocation.MyCommand.Path
$MicroCHAP = $ScriptPath + '\MicroCHAP.dll'
Add-Type -Path $MicroCHAP

function Get-Challenge {
	param(
		[Parameter(Mandatory=$true)]
		[string]$InstanceUrl
	)

	$url = "$($InstanceUrl)/sitecore/api/westco/auth/challengetoken"

	$result = Invoke-WebRequest -Uri $url -TimeoutSec 360 -UseBasicParsing -Method Post

	$result.Content | ConvertFrom-Json | Select-Object -ExpandProperty token
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
    $result = Invoke-WebRequest -Uri $query -Headers @{ "X-MC-MAC" = $signature.SignatureHash; "X-MC-Nonce" = $challenge; "token" = "jjj" } -TimeoutSec 10800 -UseBasicParsing

    $result.Content
}

$instanceUrl = "http://sc82u5.local"
$sharedSecret = "cpfwrKUUcefNam36zwZQpmjk2342342342342432342fsgfsgd"

$challenge = Get-Challenge -InstanceUrl $instanceUrl
$query = $instanceUrl + "/sitecore/api/ssc/item/{110D559F-DEA5-42EA-9C1C-8A5DF7E70EF9}"
Get-Result -ApiUrl $query -Challenge $challenge
