Overview
UserManagementAPI is an ASP.NET Core Web API project developed in Visual Studio 2026. It provides full CRUD functionality for managing user records and includes middleware for security headers, input sanitization, rate limiting, and global exception handling. This guide explains how peer reviewers can download, open, build, run, and test the project successfully. It includes instructions for both Visual Studio and VS Code users.
1. Prerequisites
Before running the project, install the following software:
Required Software
•	.NET 8 SDK: Download it from https://dotnet.microsoft.com/en-us/download/dotnet/8.0.
•	Git https://git-scm.com/downloads
•	IDE options: Use Visual Studio 2022/2026 or VS Code.
o	Visual Studio 2022/2026 (recommended)
	Install the ASP.NET and Web Development workload
o	VS Code
	Install the C# Dev Kit extension
	Install the .NET Install Tool extension (optional but helpful)
2. Download the Project from GitHub
Option A: Git Command Line
Open a terminal and run the following command:
git clone https://github.com/<your-repo-name>/UserManagementAPI.git
Option B: GitHub Desktop
1.	Open GitHub Desktop.
2.	Click Clone Repository.
3.	Select the UserManagementAPI repository.
4.	Choose the local folder where you want to save the project.
 
3. Open the Project
VS Code Instructions
1.	Open VS Code.
2.	Select File > Open Folder.
3.	Choose the cloned project folder.
4.	VS Code should detect the .csproj file.
5.	When prompted, click Restore to install the project dependencies.
Visual Studio 2026 Instructions
1.	Open Visual Studio 2026.
2.	Select Open a project or solution.
3.	Choose the project’s .csproj file.
4.	Visual Studio automatically restores the required NuGet packages.
4. Run the API
Using VS Code
Open the integrated terminal and run these commands:
bash
dotnet build
dotnet run
After the API starts, you should see a message like this:
Now listening on: https://localhost:5001
Using Visual Studio 2026
Press F5 or click Start Debugging.
Visual Studio will automatically:
•	Build the project.
•	Launch the API.
•	Open Swagger in your browser.
5. Test the API with Swagger
After the API is running, test it in Swagger by following these steps:
1.	Open your browser.
2.	Go to https://localhost:5001/swagger/index.html.
3.	Review the available API endpoints.
o	Users endpoints
o	(Auth endpoints appear in Phase 3 versions)
4.	Expand an endpoint, click Try It Out, enter the required JSON, and then click Execute.
6. Common Issues and Solutions
HTTPS Certificate Warning in VS Code
If VS Code prompts you to trust the development certificate, run this command:
dotnet dev-certs https --trust
Port Already in Use
If the default port is already in use, run the API on a different port:
dotnet run --urls https://localhost:6001
Missing Dependencies
If dependencies are missing, restore the project packages:
dotnet restore
SQLite Database Not Created
The API automatically creates the SQLite database file the first time it runs. If the database file is deleted, run the API again to recreate it.
7. Notes for VS Code Users
VS Code does not automatically perform the following tasks:
•	Generate launch profiles.
•	Attach debuggers automatically.
•	Open Swagger automatically.
To run the project in VS Code, start the API manually:
dotnet run
Then open Swagger manually in your browser. This behavior is normal and expected when using VS Code.
8. Notes for Visual Studio 2026 Users
Visual Studio provides the following built-in conveniences:
•	Automatic project builds.
•	Automatic API launch.
•	Integrated debugging tools.
•	Automatic Swagger launch.
These features help the project run more smoothly in Visual Studio 2026.
