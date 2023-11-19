# TeamLead Helper

.env frontend config example:
REACT_APP_API_URL='http://localhost:80/'

teamleadhelperconfig.json example:
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
}


then:
`docker compose up` from directory with .yaml