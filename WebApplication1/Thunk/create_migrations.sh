# dotnet ef migrations add AddExceptionsJournalFacade -c ExceptionsJournalFacade
# dotnet ef migrations add AddTreeFacade -c TreeFacade
dotnet ef database update -c ExceptionsJournalFacade
dotnet ef database update -c TreeFacade