# AbyssFM Project Requirements Document

## 1. Project Overview

AbyssFM is a full-stack music discovery and recommendation app built around a mysterious, atmospheric, purple-to-black visual identity. The app recommends songs based on mood, genre, energy, darkness, tempo, and thematic similarity.

The goal of AbyssFM is to create a project that demonstrates more than basic API usage. The project should show full-stack engineering ability, database design, backend architecture, custom recommendation logic, frontend design, documentation, and deployment readiness.

AbyssFM will begin with a locally controlled song database instead of depending on paid or restricted third-party music APIs. This gives the project full control over the data model, recommendation algorithm, and demo reliability.

## 2. Project Purpose

The purpose of AbyssFM is to build a polished, technically meaningful software engineering portfolio project that demonstrates:

* Relational database design
* Backend API development using ASP.NET Core
* Entity modeling with C# and Entity Framework Core
* Custom recommendation algorithm design
* React and TypeScript frontend development
* Strong UI/UX design with animation and visual identity
* Clean GitHub documentation
* A professional development workflow using issues, commits, and project planning

## 3. Target Users

The main target users are music listeners who want to discover songs based on a specific emotional or atmospheric vibe.

Example users include:

* A listener looking for late-night, dreamy, or mysterious music
* A student who wants a focus or study playlist
* A gamer who wants atmospheric background music
* A person looking for hiking, night-drive, heartbreak, or gym music
* A recruiter or developer viewing the project as a technical portfolio piece

## 4. Product Vision

AbyssFM should feel like a dark, immersive music discovery room rather than a standard dashboard. The experience should feel cinematic, mysterious, and emotional.

The user should feel like they are descending into a music space where songs are discovered by mood and atmosphere instead of only by artist or genre.

## 5. Core MVP Features

The first version of AbyssFM must include:

1. A visually polished landing or dashboard page
2. A local PostgreSQL song database
3. A mood and vibe selection interface
4. A recommendation endpoint that receives user preferences
5. A recommendation algorithm that ranks songs
6. Recommendation result cards
7. Explanation text showing why each song was recommended
8. A clean README with setup instructions
9. Project documentation describing requirements, architecture, database design, and algorithm logic

## 6. User Stories

As a user, I want to select a mood so that I can discover songs that match how I feel.

As a user, I want to select a vibe such as late-night, gaming, hiking, or heartbreak so that recommendations match the context I am listening in.

As a user, I want to see recommended songs ranked by relevance so that I can quickly find music that fits my selected mood.

As a user, I want each recommendation to explain why it was chosen so that the system feels understandable and not random.

As a user, I want the interface to feel immersive and visually memorable so that the app feels different from a basic music search tool.

As a developer or recruiter, I want the GitHub repository to clearly explain the architecture, database design, and algorithm so that I can understand the engineering behind the project.

## 7. In-Scope Requirements

The MVP will include:

* React and TypeScript frontend
* ASP.NET Core Web API backend
* PostgreSQL database running locally in Docker
* Entity Framework Core for database access
* DBeaver for database inspection and manual SQL testing
* Local seed data for songs, artists, genres, moods, and vibes
* REST API endpoints for retrieving songs, moods, vibes, and recommendations
* Custom weighted recommendation algorithm
* Recommendation result explanations
* Professional GitHub documentation

## 8. Out-of-Scope for MVP

The MVP will not include:

* Spotify login
* Paid music APIs
* Real-time music streaming
* Full user accounts
* Payment features
* Social media sharing
* Complex machine learning
* Mobile app version
* Real audio playback from copyrighted tracks
* Production-scale authentication

These features can be considered later after the MVP is complete.

## 9. Success Criteria

The MVP is considered successful when:

* The database runs locally through Docker
* DBeaver can connect to the database
* The backend can retrieve songs from PostgreSQL
* The backend can generate recommendations from selected moods and vibes
* The frontend can call the backend and display recommendation results
* Each recommendation includes a score and explanation
* The GitHub repository has clear setup instructions
* The project has screenshots or a demo video
* The project can be explained clearly on LinkedIn and in interviews

## 10. Public Project Description

AbyssFM is a full-stack music recommendation engine that helps users discover songs by mood, genre, energy, and atmosphere. The project uses a PostgreSQL database, ASP.NET Core backend, React frontend, and a custom weighted scoring algorithm to generate explainable music recommendations.

The project is designed to demonstrate database modeling, backend API development, frontend UI design, and algorithmic thinking in a polished, recruiter-readable portfolio project.
