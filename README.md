# AbyssFM

AbyssFM is a full-stack music discovery and recommendation app built around a mysterious purple-to-black visual identity.

The project recommends songs based on mood, genre, energy, darkness, tempo, and thematic similarity using a custom recommendation algorithm.

## Tech Stack

- React + TypeScript
- ASP.NET Core Web API
- C#
- Entity Framework Core
- PostgreSQL
- Docker
- DBeaver

## Project Goals

AbyssFM is designed to demonstrate:

- Full-stack development
- Relational database design
- Backend API development
- Entity modeling with C#
- Custom recommendation logic
- Professional documentation
- Clean GitHub project structure

## Current Status

- Project structure initialized
- PostgreSQL database running locally in Docker
- DBeaver connected to PostgreSQL
- Requirements and architecture documentation added

## Documentation

See the `docs/` folder for:

- Project requirements
- System architecture
- API design
- Data and recommendation requirements
- Recommendation algorithm plan
- Development plan

## Local Database Setup

This project uses PostgreSQL through Docker Compose.

1. Copy `.env.example` into a new `.env` file.
2. Fill in the PostgreSQL environment variables.
3. Run:

```bash
docker compose up -d