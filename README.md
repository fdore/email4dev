email4dev
==============

email4dev is a set of Http Handler and Http Listener to expose eml files as html.
This is particularly usefull when running Test Automation around emails. No need to login to your email inbox to retrieve and parse your emails anymore. Simply access them via a direct Http call.

You can configure the built-in .Net SmtpClient to drop emails into a location on disk.
email4dev enables you to search and retrieve emails via http.
Simply enter http://<your server>:8887?Subject=my email subject, and the html body of the email will be returned.
email4dev allows you to search emails by subject, body, recipient and by dates.


Configure Smtp to drop emails on drive
--------------------------------------

First of all, you will need to configure your smtp client to drop emails on drive.
To do so, put the following section into your App.config or Web.config

<system.net>
	<mailSettings>
		<smtp deliveryMethod="SpecifiedPickupDirectory">
			<specifiedPickupDirectory pickupDirectoryLocation="c:\temp\"/>
		</smtp>
	</mailSettings>
</system.net>

Now, everytime you call the Send method on your SmtpClient, the message will be dropped on drive.

Install email4dev
-----------------

email4dev comes as a HttpHandler and HttpListener depending on the type of the hosting application.

### Http Listener


When installing email4dev.HttpListener, the following sections will be added to your App.config
	
      <configuration>
            <configSections>
                  <sectionGroup name="backupMailSettings">
		        <section name="smtp" type="System.Net.Configuration.SmtpSection" />
                  </sectionGroup>
            </configSections>
	  
            <appSettings>
                  <add key="EmailHttpListenerPort" value="8887" />
            </appSettings>
	  
            <backupMailSettings>
                  <smtp deliveryMethod="SpecifiedPickupDirectory">
                        <specifiedPickupDirectory pickupDirectoryLocation="C:\temp" />
                  </smtp>
            </backupMailSettings>
      </configuration>
	
Configure the pickupDirectoryLocation parameter to point to the folder where emails are now dropped.
You can also change the EmailHttpListenerPort to whatever port to want the handler to listen to. 

_Note that if the handler is running in a Windows Service, the user must have enough privilege to open the socket._

### Http Handler

When installing email4dev.HttpHandler, the following sections will be added to your Web.config

      <configuration>
            <configSections>
                  <sectionGroup name="backupMailSettings">
                        <section name="smtp" type="System.Net.Configuration.SmtpSection"/>
                  </sectionGroup>
            </configSections>
            <system.web>
                  <httpHandlers>
                        <add verb="*" path="*.axd" type="email4dev.HttpHandler.EmailHttpHandler, email4dev.HttpHandler" />
                  </httpHandlers>
            </system.web>
            <backupMailSettings>
                  <smtp deliveryMethod="SpecifiedPickupDirectory">
                        <specifiedPickupDirectory pickupDirectoryLocation="C:\temp" />
                  </smtp>
            </backupMailSettings>
      </configuration>

Configure the pickupDirectoryLocation parameter to point to the folder where emails are now dropped.

## What if I want to be able to both be able to send emails, and drop a copy on drive?

If you configure your Smtp Client to drop emails on drive, then, the emails wont be sent anymore. 
In the case you still want your email to be sent, and have a copy of it on drive, email4dev provides a SmtpClient which reads its configuration from the backupMailSettings.

You can now use the StmpClient as usual, and use the backup smtp client to save a copy of your emails on drive.

      var smtp = BackupSmtpClientBuilder.Build();

Your configuration will now look like 

     <configuration>
            <configSections>
                  <sectionGroup name="backupMailSettings">
		        <section name="smtp" type="System.Net.Configuration.SmtpSection" />
                  </sectionGroup>
            </configSections>
	  
            <appSettings>
                  <add key="EmailHttpListenerPort" value="8887" />
            </appSettings>
            <system.net>
                  <mailSettings>
                        <smtp deliveryMethod="SpecifiedPickupDirectory">
                              <specifiedPickupDirectory pickupDirectoryLocation="c:\temp\"/>
                        </smtp>
                  </mailSettings>
            </system.net>
            <backupMailSettings>
                  <smtp deliveryMethod="SpecifiedPickupDirectory">
                        <specifiedPickupDirectory pickupDirectoryLocation="C:\temp" />
                  </smtp>
            </backupMailSettings>
      </configuration>


## Available filters

### Subject
http://localhost:8887?Subject="email subject"

### Recipient
http://localhost:8887?Recipient="email address"

### Body content

http://localhost:8887?Body="text to search for"

### Time sent
[http://localhost:8887?Body="text to search for"&TimeSent="time to search from"

email4dev always return the latest email matching the filter.