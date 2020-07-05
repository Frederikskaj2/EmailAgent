# Email Agent

This application will post messages to Teams when specific email messages are received.

Notes:

- The application is an _Azure Function App_ that runs on a timer.
- _IMAP_ is used to access the email inbox.
- The application can be extended to recognize and process different email types.
- Application state is used to ensure that emails that have been processed once wont be processed the next time the application executes.
- The Teams app _Incoming Webhook_ is used to post to Teams.
