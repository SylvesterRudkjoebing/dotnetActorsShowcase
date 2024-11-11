# Project Overview

This repository demonstrates my proficiency in C# and .NET development.

The project is a **Blazor WebAssembly** application that implements a **breadth-first search (BFS)** algorithm to identify the shortest "connection path" between two actors based on movies they starred in together 

<img width="1083" alt="Skærmbillede 2024-11-11 kl  18 54 02" src="https://github.com/user-attachments/assets/9f88a387-779b-4c18-b2de-9b40f2683fd2">

## Highlights

1. **Database-First Approach**  
   The project follows a "Database First" approach, using migrations from an Azure-hosted `MoviesDB` that includes three tables: `Actors`, `Films`, and `Starrings`.
   
2. **BFS Algorithm Implementation**  
   The BFS algorithm is encapsulated as a scoped service, designed with interfaces and low coupling to ensure clean and modular code.
   
3. **Front-End**  
   The front-end is built in C# using **Razor pages**, with HTML and CSS for styling.
   
4. **Unit Testing**  
   Unit tests are written to validate functionality, using an in-memory mock database for reliable and isolated testing.

## Access to MoviesDB

If you would like access to `MoviesDB`, please feel free to message me to request Azure passthrough permissions.

