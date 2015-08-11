# [Java / .NET interop - Windows Service hosting a Java .jar](http://www.phils-osophy.co.za/)

I recently received a requirement to integrate with a Java API. 
I've worked with Java before and [natch](http://www.urbandictionary.com/define.php?term=natch), the requirement came my way.

## Considerations

* This is a .NET shop and [Maven](https://maven.apache.org/) / [Gradle](https://gradle.org/) / [Spring](https://spring.io/) / [IntelliJ](https://www.jetbrains.com/idea/) are foreign concepts (the company firewall wouldn't let me trough to the [Maven](https://maven.apache.org/) repo, nor was there an internal repo that I could find).  Thus, in order to ensure the solution is maintainable by anyone in the team, most of the logic needed to reside in the .NET space and the least amount of Java should be written.

## Requirements

* The interaction with Java should be seamless.
* Minimal to no coupling between the Java API and the .NET code. This meant no [jni4net](http://jni4net.com/) that would require intricate knowledge of the API's internals as well as will require the Java API to emit to our .NET application, and vice versa.
	<sup>(The implementation of [jni4net](http://jni4net.com/) in the Java API was not an option anyway as it would require additional support from the writers of the API for my specific situation.)</sup>
* The Java API needs to be hosted regardless of any user being logged in, it needs to run as a background process (like a [Daemon](https://en.wikipedia.org/wiki/Daemon_(computing)) in a Unix environment).[^1]

[^1]:java.exe -jar on a console was thus not an option.
 
## The result of ruminating on the constraints

* **Seamless, loosely coupled integration between the .NET application and the Java API** by _**wrap the Java API in a web service**_ that can then just be consumed like any other SOAP based web service.

* **Write as little Java as possible** by _**exposing the required Java API functionality as vanilla as possible in the web service**_. The implemebntations code on the .NET side is then automatically through the service proxy class generation tools in Visual Studio OR [Svcutil.exe](https://msdn.microsoft.com/en-us/library/aa347733(v=vs.110).aspx). _**Any orchestration logic such as polling should then reside in the .NET application code.**_

* **Run the Web Service (that wraps the Java API**[^2]**) as a background application** by wrapping it in a _**Windows Service**_. _The windows service hosts the web service as a background operation which means it is not tied to any user sessions._ A windows service has the additional capability of being configured to starting automatically on the occasional server reboot as well as run under credentials specific to the operation.

[^2]:[..in the house that Jack built.](https://en.wikipedia.org/wiki/This_Is_the_House_That_Jack_Built)
