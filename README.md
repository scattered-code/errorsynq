errorsynq
=========

ErrorSynq API client


Configuring your application in .NET
1. First, add it as a reference to your application, or install it from NuGet

2. Add the API key provided in your account to your web.config file

3. Finally, tell the client when an exception ocurred, so it can be recorded. The best place to do this is Application_Error in your Global.asax

That's it, any errors your application passes to the client library will be available for viewing in your http://errorsynq.net account.
