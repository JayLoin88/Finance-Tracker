| Finance Tracker |

Finance Tracker is a C# console application for managing users and tracking their financial transactions. The application uses SQL Server for persistent data storage and communicates with the database using ADO.NET and parameterized SQL queries.

The project began as a file-based application using JSON for persistence and was later migrated to a relational SQL Server database. As part of that migration, the application was refactored to separate database operations from the console user interface using repository classes.


| Features |

* Add and manage multiple users
* Add financial transactions for individual users
* Assign transactions to categories
* View all transactions for a selected user
* Filter transactions by category
* Delete transactions
* View financial summaries for users
* Calculate monthly net savings based on income and expenses
* Display total transaction count and largest recorded expense
* Validate transaction dates and user input
* Handle database errors without crashing the application
* Persist application data using SQL Server


| Technologies Used |

- C#
- .NET
- SQL Server
- ADO.NET (Microsoft.Data.SqlClient)
- SQL


| Database |

The application uses a relational SQL Server database containing three primary tables:

- Users — stores user information, balance, monthly income, and monthly expenses.
- Transactions — stores transaction amounts, descriptions, dates, and relationships to users and categories.
- Categories — stores the available transaction categories.

Foreign keys are used to associate transactions with their corresponding users and categories.

The application performs database operations using parameterized SQL queries to safely insert, retrieve, filter, and delete data.


| Application Structure |

The project separates application responsibilities between models, repositories, and the console interface.

- Models represent application data such as users, transactions, categories, and user summaries.

- Repositories handle communication with SQL Server, including SQL queries and mapping database results to C# objects.

- Program.cs handles the console interface, user input, validation, and application flow.

This structure keeps database access separate from the user interface and makes the application easier to maintain and extend.


| What I Learned |

This project gave me hands-on experience with:

- Designing and working with a relational database
- Connecting a C# application to SQL Server
- Writing SQL queries involving joins, filtering, aggregation, and CRUD operations
- Using ADO.NET with SqlConnection, SqlCommand, and SqlDataReader
- Using parameterized queries
- Working with primary keys, foreign keys, and database constraints
- Separating database logic from application/UI logic
- Handling database exceptions at the application level
- Validating and converting user input
- Refactoring an existing application from JSON persistence to SQL Server


| Getting Started |

- Prerequisites -

To run Finance Tracker locally, you will need:

- .NET 10
- SQL Server
- SQL Server Management Studio (SSMS)

| Setup |

1. Clone or download this repository.
2. Open Database/FinanceTracker.sql in SQL Server Management Studio.
3. Execute the script to create the FinanceTracker database, tables, relationships, constraints, indexes, and default transaction categories.
4. Verify that the connection string in Program.cs matches your local SQL Server configuration.
5. Build and run the application.

The default connection string is configured to use a local SQL Server instance with Windows Authentication:

Server=localhost;Database=FinanceTracker;Integrated Security=True;TrustServerCertificate=True;

Depending on your SQL Server configuration, you may need to modify the server portion of the connection string before running the application.


| Project Status |

Finance Tracker is a completed console application and serves as a portfolio project demonstrating foundational C#, SQL Server, ADO.NET, relational database, and application architecture skills.
