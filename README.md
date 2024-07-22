## Onboarding

1. Install Visual Studio using the `.vsconfig` [file in the repo](./.vsconfig).
2. In Visual Studio, start the portal by selecting "Debug" -> "Start Debugging"
3. When you first run the portal project in Visual Studio, it will prompt you to trust the ASP.NET Core SSL certificate; you should trust this certificate
4. You will then be prompted to install the certificate; also install the certificate
5. A firewall prompt will appear requesting access; give whatever access is appropriate for your testing cases
6. When the Edge browser opens, it will inform you that the connection isn't private; there is no obvious way to allow this. What you must do is click in the margin of the page (to put the "focus" there) and type "thisisunsafe"; this will allow the connection and take you to the page
7. Close the browser window.
8. In Visual Studio, start the portal by selecting "Debug" -> "Start Debugging"
9. In this second run, you will be asked to trust the IIS Express SSL certificate; trust this certificate; you will also need to install this certificate
