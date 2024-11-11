# Project Overview

This repository demonstrates my proficiency in C# and .NET development.

The project is a **Blazor WebAssembly** application that implements a **breadth-first search (BFS)** algorithm to identify the shortest "connection path" between two actors based on movies they starred in together.

<img width="1083" alt="Skærmbillede 2024-11-11 kl  18 54 02" src="https://github.com/user-attachments/assets/dd0029af-9b95-4f93-a036-0f813cd3e029">

## Highlights

1. **Database-First Approach**  
   The project follows a "Database First" approach, using migrations from an Azure-hosted `MoviesDB` that includes three tables: `Actors`, `Films`, and `Starrings`.
   
2. **BFS Algorithm Implementation**  
   The BFS algorithm is encapsulated as a scoped service, designed using interfacing and low coupling to ensure clean and modular code.
   
3. **Back-End**  
   The back-end is built in C# using Entity Framework, including data access layer and controllers handling endpoints and http requests.
   
4. **Front-End**  
   The front-end is built in C# using **Razor pages**, with HTML and CSS for styling.
   
5. **Testing**  
   Unit tests are written to validate functionality, using an in-memory mock database for reliable and isolated testing.

## Access to MoviesDB

If you would like access to `MoviesDB`, please feel free to message me to request connection strings and Azure passthrough permissions.

