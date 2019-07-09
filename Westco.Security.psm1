$ErrorActionPreference = 'Stop'
$ScriptPath = Split-Path $MyInvocation.MyCommand.Path
$MicroCHAP = $ScriptPath + '\MicroCHAP.dll'
if(-not(Test-Path -Path $MicroCHAP)) {
    $MicroCHAP = Get-ChildItem -Path $ScriptPath -Filter "MicroCHAP.dll" -Recurse | Select-Object -First 1 -ExpandProperty FullName
}

Add-Type -Path $MicroCHAP

function Get-SscChallenge {
	param(
		[Parameter(Mandatory=$true)]
		[string]$InstanceUrl
	)

	$url = "$($InstanceUrl)/sitecore/api/ssc/chapauth/challenge"

	$result = Invoke-WebRequest -Uri $url -TimeoutSec 360 -UseBasicParsing

	$result.Content | ConvertFrom-Json | Select-Object -ExpandProperty challenge
}

function Get-SscResult {
	param(
		[Parameter(Mandatory=$true)]
		[string]$Url,

        [Parameter(Mandatory=$true)]
		[string]$SharedSecret,

        [Parameter(Mandatory=$true)]
        [string]$Challenge
	)

    $signatureService = New-Object MicroCHAP.SignatureService -ArgumentList $SharedSecret
    $signature = $signatureService.CreateSignature($challenge, $Url, $null)
    $headers = @{ "X-MC-MAC" = $signature.SignatureHash; "X-MC-Nonce" = $challenge; }
    $result = Invoke-WebRequest -Uri $Url -Headers $headers -TimeoutSec 10800 -UseBasicParsing

    $result.Content
}

function Invoke-SscRequest {
    param(
        [Parameter(Mandatory=$true)]
        [string]$Url,

        [Parameter(Mandatory=$true)]
	[string]$SharedSecret,

        [Parameter(Mandatory=$true)]
        [string]$Challenge,

        [string]$Payload,

        [string]$ContentType = "application/json",

        [string]$Method = "Post"
    )

    $signatureService = New-Object MicroCHAP.SignatureService -ArgumentList $SharedSecret
    $signature = $signatureService.CreateSignature($challenge, $Url, $null)
    $headers = @{ "X-MC-MAC" = $signature.SignatureHash; "X-MC-Nonce" = $challenge; }

    $requestProps = @{
        Uri = $Url
        Headers = $headers
        ContentType = $ContentType
        TimeoutSec = 10800
        UseBasicParsing = $true
        Method = $Method
    }

    if($Method -eq "Post") {
        $requestProps["Body"] = $Payload
    }

    $result = Invoke-WebRequest @requestProps

    $result.Content
}
