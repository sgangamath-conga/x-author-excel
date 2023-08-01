"C:\Program Files (x86)\Microsoft SDKs\Windows\v10.0A\bin\NETFX 4.6.2 Tools\svcutil" /language:cs /noConfig /n:*,Apttus.XAuthor.SalesforceAdapter.AppBuilderWS /out:AppBuilderWsProxy.cs AppBuilderWs.wsdl

"C:\Program Files (x86)\Microsoft SDKs\Windows\v10.0A\bin\NETFX 4.6.2 Tools\svcutil" /language:cs /noConfig  /useSerializerForFaults /n:*,Apttus.XAuthor.SalesforceAdapter.SForce /out:SForcePartnerServiceProxy.cs partner.wsdl

pause

@echo off
echo.
echo.
echo Hit enter and read the next message carefully
pause

cls
@echo off
echo.
echo *** IMPORTANT X-AUTHOR MESSAGE: Post Proxy Generation Manual Steps ***
echo.
echo After Proxy has been generated for API version 46:
echo 1. open SForcePartnerServiceProxy.cs.
echo 2. Replace all occurence of [][] with []. You will find 2 occurences.
echo 3. Recompile the Salesforce Adapter Project.
echo.
echo.
echo.
echo More details for this workaround can found at https://developer.salesforce.com/forums?id=906F0000000AiPEIA0"
echo.
echo.

pause