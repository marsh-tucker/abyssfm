# AbyssFM Development Plan

## 1. Development Philosophy

AbyssFM should be developed in small, safe milestones.

The goal is to avoid building too much at once. Each phase should produce a working checkpoint that can be committed to GitHub.

The project should prioritize:

- Clear documentation
- Small commits
- Working backend before complex frontend polish
- Testable recommendation logic
- Clean project structure
- Reproducible local setup

## 2. Phase 0: Project Setup

Goals:

- Create project root folder
- Add Docker Compose file for PostgreSQL
- Connect DBeaver to PostgreSQL
- Create Git repository
- Add README
- Add documentation files
- Add `.gitignore`

Success criteria:

- PostgreSQL runs in Docker
- DBeaver connects successfully
- Project structure is visible in VS Code
- Initial documentation exists
- First Git commit is created

## 3. Phase 1: Database Design

Goals:

- Define database tables
- Define columns
- Define primary keys
- Define foreign keys
- Define many-to-many relationships
- Document normalization decisions

Expected tables:

- Artists
- Albums
- Genres
- Songs
- MoodTags
- VibeTags
- SongMoodTags
- SongVibeTags
- RecommendationSessions
- RecommendationResults

Success criteria:

- `docs/database-design.md` explains every table
- Relationships are clearly defined
- Database design can be translated into C# entity models

## 4. Phase 2: Backend Setup

Goals:

- Create ASP.NET Core Web API project
- Add Entity Framework Core
- Add Npgsql PostgreSQL provider
- Configure PostgreSQL connection string
- Create project folders for Models, Data, DTOs, Services, and Controllers

Success criteria:

- Backend runs locally
- Backend can connect to PostgreSQL
- Swagger or API testing tool can access backend endpoints

## 5. Phase 3: Entity Models and Migrations

Goals:

- Create C# entity models
- Create DbContext
- Configure relationships
- Create initial EF Core migration
- Apply migration to PostgreSQL

Success criteria:

- Database tables are created through EF Core migrations
- DBeaver can inspect the generated tables
- Tables match the documented database design

## 6. Phase 4: Seed Data

Goals:

- Create seed data for songs, artists, albums, genres, moods, and vibes
- Load seed data into PostgreSQL
- Prevent duplicate artists, albums, genres, and tags
- Verify seeded data in DBeaver

Success criteria:

- Database contains enough songs for basic recommendation testing
- Songs are correctly connected to artists, genres, moods, and vibes
- Seed data can be recreated from the repo

## 7. Phase 5: Recommendation Engine

Goals:

- Create recommendation request DTO
- Create recommendation result DTO
- Create recommendation service
- Implement weighted scoring algorithm
- Generate explanation reasons
- Return ranked recommendations

Success criteria:

- Backend can return ranked song recommendations
- Each result includes score and reasons
- Algorithm has unit tests
- Recommendation endpoint works in Postman, Thunder Client, or Swagger

## 8. Phase 6: Frontend Setup

Goals:

- Create React and TypeScript frontend
- Add Tailwind CSS
- Add Framer Motion
- Create layout and visual theme
- Build mood and vibe selector
- Build recommendation result cards

Success criteria:

- Frontend runs locally
- UI matches the AbyssFM visual direction
- User can select preferences
- Frontend can call backend API

## 9. Phase 7: Full Integration

Goals:

- Connect React frontend to ASP.NET Core backend
- Send recommendation requests from frontend
- Display backend recommendation results
- Handle loading and error states

Success criteria:

- User can complete the full recommendation flow
- Frontend displays real data from PostgreSQL through the backend
- App works locally end to end

## 10. Phase 8: Polish and Documentation

Goals:

- Improve UI animations
- Add screenshots
- Add demo GIF or video
- Improve README
- Document setup instructions
- Document architecture and algorithm clearly

Success criteria:

- GitHub repo looks professional
- README explains the project clearly
- Project can be shared on LinkedIn
- Project can be discussed in interviews

## 11. Suggested Git Branches

Possible branches:

```text
docs/project-setup
docs/database-design
feature/backend-setup
feature/entity-models
feature/seed-data
feature/recommendation-service
feature/frontend-dashboard
feature/full-integration
feature/ui-polish