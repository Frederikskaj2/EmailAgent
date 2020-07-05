# Email Agent

This application will post messages to Teams when specific email messages are received.

## Notes

- The application is an _Azure Function App_ that is triggered by a timer.
- _IMAP_ is used to access the email account.
- The application can be extended to recognize and process different email types.
- Application state is used to ensure that emails that have been processed once wont be processed the next time the application executes.
- The Teams app _Incoming Webhook_ is used to post to Teams.

## Configuration

To test the app locally you need to add the file `local.settings.json` to the `Functions` project. This file is not commited to git and can contain secrets.

```
{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet",
    "Imap:HostName": "...",
    "Imap:UserName": "...",
    "Imap:Password": "...",
    "Storage:ConnectionString": "UseDevelopmentStorage=true",
    "Teams:IsPostingEnabled": "False",
    "Teams:Webhooks:Beboerhenvendelser": "..."
  }
}
```

The Azure Functions App application settings have to be configured with the same values:

| Name | Value |
| - | - |
| `Imap:HostName` | The host name of the IMAP server. |
| `Imap:UserName` | The user name providing access to the email account. |
| `Imap:Password` | The password providing access to the email account. |
| `Storage:ConnectionString` | Connection string for the Azure Storage used to store application state. |
| `Teams:IsPostingEnabled` | Setting used to turn on or off if anything will be posted to Teams. This is useful to turn off when testing locally. |
| `Teams:Webhooks:Beboerhenvendelser` | The Teams Incoming Webhook URL used to post to the channel. |
