# ProjectOneMil

This project is configured to develop web applications using .NET 8.0. The project integrates with Entity Framework Core and PostgreSQL database.

## Features

- Web application developed with .NET 8.0
- Database operations using Entity Framework Core
- PostgreSQL database support
- User authentication and authorization

## Technologies and Packages Used

- **.NET 8.0**: The target framework of the project.
- **EPPlus**: Library used for working with Excel files.
- **Microsoft.AspNetCore.Identity.EntityFrameworkCore**: Library used for authentication and authorization.
- **Microsoft.EntityFrameworkCore**: ORM (Object-Relational Mapping) library.
- **Microsoft.EntityFrameworkCore.Design**: Design-time tools for Entity Framework Core.
- **Microsoft.EntityFrameworkCore.Tools**: Command-line tools for Entity Framework Core.
- **Npgsql.EntityFrameworkCore.PostgreSQL**: PostgreSQL database provider.
- **System.Linq.Dynamic.Core**: Library for dynamic LINQ queries.

## Setup

### Requirements

- .NET 8.0 SDK
- PostgreSQL

### Steps

1. Clone the repository:
    ```sh
    git clone https://github.com/username/ProjectOneMil.git
    cd ProjectOneMil
    ```
2. Restore the required packages:
    ```sh
    dotnet restore
    ```
3. Update the database connection string in the `appsettings.json` file.

4. Apply the database migrations:
    ```sh
    dotnet ef database update
    ```

5. Run the application:
    ```sh
    dotnet run
    ```

## Usage

- After starting the application, navigate to `http://localhost:5000` in your browser.
- Perform user authentication and authorization operations.

## Contributing

If you would like to contribute, please follow these steps:

1. Fork this repository.
2. Create a new branch (`git checkout -b feature-name`).
3. Commit your changes (`git commit -am 'Add new feature'`).
4. Push to the branch (`git push origin feature-name`).
5. Create a Pull Request.

## Contact

If you have any questions or feedback, please contact us at [emretrbas@gmail.com](mailto:emretrbas@gmail.com).
