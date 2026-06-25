Finance Tracker

A console application built using c#, .NET framework, and JSON format. Users can add multiple users to the finance tracker, then add their transactions. They can then view
a summary of their transactions which will show their total number of transactions, the highest transaction recorded, and will even calculate the users net savings after inputting
their monthly income and expenses. This application does feature data persistence through the use of JSON serialization with formatting, and if the .json file gets corrupted, the
user has the option to procede or exit. If the user procedes, a backup of the corrupted file will be created, and a new save file will overwrite the original.

- Features -

- Add users
- Add transactions
- Delete transactions
- View summaries
- Filter by category
- Date validation
- JSON persistence
- Backup corrupted save files
