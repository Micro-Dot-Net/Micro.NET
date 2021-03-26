# Micro.NET
An super easy light-weight microservice chassis for .NET!
***
This framework abstracts away boilerplate code for common microservice scenarios into easy-to-drop-in components! Customize as much or as little as you want: roll your own to fit onto the framework, or use our's out of the box.

Features:
 - HTTP Transport
 - Handler Routing
 - Sagas (in progress)
 - Routing Slips (in progress)
 
&nbsp;
 Coming Soon:
 - Service Host
 - Generic transport adapter
 - SQL Server Persistence
 - Apache Ignite Persistence
 - Message Deduplication
 - Message Authentication
- Service Discovery
- Caching
- Transactional Outboxing
- ... and more!

Easy to use, just call .UseMicroNet() on your IServiceCollection, and do a little configuration, either in code...

    Code Coming Soon!

Or in your favorite configuration source (like appsettings.json)!

    Code Coming Soon!
  
Or pull the Micro.Net.Host nuget package, and implement the IConfigureMicroservices interface in your project:

    Code Coming Soon!
  
With a small configuration change to your project to run the Micro.Net.Host executable, you should be up and running in no time flat!
