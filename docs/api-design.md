# AbyssFM API Design

## 1. API Overview

AbyssFM will use an internal REST API built with ASP.NET Core Web API.

The API will allow the React frontend to retrieve songs, genres, mood tags, vibe tags, and recommendation results from the backend.

The API will not depend on Spotify or any paid third-party music API for the MVP.

## 2. Purpose of the API

The purpose of the API is to separate frontend presentation from backend logic.

The frontend is responsible for:

- Displaying the user interface
- Collecting user mood and vibe preferences
- Sending recommendation requests
- Displaying returned recommendations

The backend API is responsible for:

- Retrieving data from PostgreSQL
- Running the recommendation algorithm
- Returning structured JSON responses
- Handling validation and errors

## 3. Base URL

During local development, the backend API will run locally.

Example local URL:

```text
https://localhost:5001