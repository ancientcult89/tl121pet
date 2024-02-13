# TeamLead Helper

.env frontend config example:<br>
REACT_APP_API_URL='http://localhost:80/'

teamleadhelperconfig.json example:<br>
{
  "AppSettings": {
    "Token": "somesecretandhorrobletokenbooo"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "CorsAllowedHosts": "https://localhost:3060; ",
  "AllowedHosts": "*",
  "ConnectionStrings": { "TeamLead_Db": "Server=host.docker.internal;Database=TeamLeadDB;Port=49154;User Id=postgres;Password=postgrespw" },
  "MailSettings": {
    "Mail": "test@mail.com",
    "DisplayName": "My Displayed Name",
    "Password": "mypassword",
    "Host": "mailserver",
    "Port": 465
  }
}<br>

all example configs are located in the folder ./configs_example
<br>


then:<br>
`docker compose up` from directory with .yaml<br>

Default user: admin@example.com<br>
Default password: admin<br>
