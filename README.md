## General info
The application for intercepting http requests uses the <a href="https://www.nuget.org/packages/FiddlerCore/">.NET FiddlerCore proxy library</a>.

## How to use
1. Start application and accept certificate (that allows you to capture https trafic). 
2. Go to any website/service and initialize POST request.
3. POST header and body of your request will be added in the window.
