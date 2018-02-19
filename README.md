# Westco Security

The module provides a mechanism for replacing the existing Sitecore Services Client token security with one that depends on the [MicroCHAP](https://github.com/kamsar/MicroCHAP) library.

## How does the module work?

A new `TokenDelegatingHandler` is included to replace the OOTB handler provided by Sitecore. 

Requests which require an authentication token are processed by the handler. The existence of the `X-MC-MAC` and `X-MC-Nonce` headers are checked and then verified against the server **SharedSecret**. If the signatures match then the request is authenticated. 

As noted in the **MicroCHAP** library:
> Each challenge is valid for only a single request and multiple authenticated requests require multiple handshakes. In other words this is designed for low volume scenarios like authenticating deployment tools as opposed to authenticating multi-user human requests.

If you wish to authenticate from client-side JavaScript, then it would be better for you to use the mechanism provided by Sitecore.

## Getting Started

* Clone the repository and restore the NuGet packages.
* Build the solution.
* Deploy the files.
  * Patch the **SharedSecret** in your own config. Check out the example.
* Run the PowerShell script with the updated **SharedSecret**.

Files:

* `\App_Config\Include\Westco.Services.Infrastructure\Westco.Services.Infrastructure.config`
* `\bin\Westco.Services.Infrastructure.dll`
* `\bin\MicroCHAP.dll` - if using Unicorn the file may already be deployed.