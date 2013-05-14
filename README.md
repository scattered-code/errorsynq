errorsynq
=========

ErrorSynq API client


Configuring your application in .NET

1. First, download or build the library, and add it as a reference to your application

2. Add the API key provided in your account to your web.config file
<pre>
&lt;appSettings>
        &lt;add key="ErrorTrackingKey" value="MY API KEY GOES HERE" />
&lt;/appSettings>
</pre>
3. Finally, tell the client when an exception ocurred, so it can be recorded. The best place to do this is Application_Error in your Global.asax
<pre>
void Application_Error(object sender, EventArgs e)
{
        ErrorSynq.Client.SubmitError();
}
</pre>


That's it, any errors your application passes to the client library will be available for viewing in your http://errorsynq.net account.
