# AbyssFM System Architecture Requirements

## 1. Architecture Overview

AbyssFM will use a full-stack client-server architecture.

The frontend will be responsible for the user interface, animations, mood selection, vibe selection, and displaying recommendation results.

The backend will be responsible for handling API requests, retrieving data from the database, running the recommendation algorithm, and returning ranked recommendation results.

The database will store songs, artists, albums, genres, mood tags, vibe tags, and recommendation-related data.

## 2. Technology Stack

The project will use the following stack.

### Frontend

* React
* TypeScript
* Tailwind CSS
* Framer Motion for animation

### Backend

* ASP.NET Core Web API
* C#
* Entity Framework Core
* Npgsql PostgreSQL provider

### Database

* PostgreSQL
* Docker for local database environment
* DBeaver for database management and inspection

### Development Tools

* VS Code
* Git
* GitHub
* Postman or Thunder Client for API testing
* Docker Desktop

## 3. Local Development Architecture

During local development, the system will run as follows:

1. The React frontend runs locally in the browser.
2. The ASP.NET Core backend runs locally on the Mac.
3. PostgreSQL runs inside a Docker container.
4. DBeaver connects to PostgreSQL through localhost and the exposed PostgreSQL port.

The communication flow is:

```text
React frontend
    ↓ HTTP / JSON
ASP.NET Core backend
    ↓ Entity Framework Core / Npgsql
PostgreSQL database in Docker
```

## 4. Communication Requirements

### Frontend to Backend

* Communication type: HTTP
* API style: REST
* Data format: JSON
* Example: React sends a POST request to `/api/recommendations`

### Backend to Database

* Communication type: PostgreSQL database connection over TCP
* ORM: Entity Framework Core
* Database provider: Npgsql
* Data format: relational rows mapped to C# entity models

### Developer to Database

* Tool: DBeaver
* Purpose: inspect tables, run SQL queries, test data, and verify relationships
* Connection: localhost to Docker-exposed PostgreSQL port

## 5. Frontend Requirements

The frontend must include:

* A polished AbyssFM landing page
* Purple-to-black visual theme
* Mood and vibe selector
* Recommendation results section
* Recommendation cards with title, artist, score, tags, and reasons
* Responsive layout for desktop first
* Smooth UI transitions and animations
* Clear loading state while recommendations are being generated
* Error state if the backend request fails

The frontend should not contain the core recommendation logic. It should only collect user input, call the backend, and display results.

## 6. Backend Requirements

The backend must include:

* ASP.NET Core Web API project
* Entity Framework Core integration
* PostgreSQL connection string configuration
* Entity models for core database tables
* DbContext for database access
* Seed data loading process
* API controllers
* DTOs for request and response objects
* Recommendation service class
* Basic error handling
* CORS configuration for frontend communication

The backend should separate concerns clearly:

* Controllers handle HTTP requests.
* Services handle business logic.
* Models define database entities.
* DTOs define API request and response shapes.
* DbContext manages database access.

## 7. Database Requirements

The database must store structured music metadata.

The database should support:

* Artists
* Albums
* Songs
* Genres
* Mood tags
* Vibe tags
* Many-to-many relationships between songs and tags
* Recommendation sessions
* Recommendation results

The database should be normalized enough to avoid unnecessary duplication but simple enough for an MVP.

## 8. API Requirements

The MVP backend should include these endpoints.

### GET `/api/songs`

Returns a list of available songs.

### GET `/api/moods`

Returns available mood tags.

### GET `/api/vibes`

Returns available vibe tags.

### GET `/api/genres`

Returns available genres.

### POST `/api/recommendations`

Accepts user-selected moods, vibes, genres, and target attributes.

Returns ranked song recommendations with scores and explanation reasons.

## 9. Error Handling Requirements

The backend should return clear error responses when:

* Required input is missing
* Selected mood or vibe does not exist
* Database connection fails
* Recommendation request has invalid values
* No matching recommendations are found

The frontend should display user-friendly messages when:

* The backend is unavailable
* The recommendation request fails
* No recommendations are found
* Data is still loading

## 10. Testing Requirements

The project should include backend tests for:

* Recommendation scoring
* Mood tag matching
* Vibe tag matching
* Energy similarity scoring
* Excluding selected songs from results
* Returning results in correct ranked order

Frontend testing can be added later, but the MVP should prioritize backend algorithm tests because they demonstrate engineering logic.

## 11. Deployment Requirements

Local development must work before deployment.

Potential deployment plan:

* Frontend: Vercel
* Backend: Render, Railway, or Fly.io
* Database: Neon or Supabase PostgreSQL

The deployed version should use environment variables for database credentials instead of hardcoded local development credentials.

## 12. Architecture Success Criteria

The architecture is successful when:

* The frontend and backend are clearly separated
* The backend can connect to PostgreSQL
* Entity Framework Core can create and query the database
* API endpoints return JSON correctly
* The recommendation algorithm is isolated in a service class
* The project structure is easy to understand from GitHub
* A new developer can follow the README and run the project locally
