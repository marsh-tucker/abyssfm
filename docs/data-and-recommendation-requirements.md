# AbyssFM Data and Recommendation Requirements

## 1. Data Strategy Overview

AbyssFM will begin with a local, controlled song dataset instead of relying on paid or restricted third-party APIs. The first version of the app will use manually curated seed data stored in PostgreSQL.

This strategy keeps the project free, reliable, and easier to explain. It also demonstrates database design and custom recommendation logic instead of depending on an external recommendation API.

The data flow will be:

1. Seed data file is created.
2. Seed data is loaded into PostgreSQL.
3. ASP.NET Core backend retrieves song data using Entity Framework Core.
4. Recommendation service compares user preferences against stored song metadata.
5. Backend returns ranked recommendations as JSON.
6. React frontend displays the results.

## 2. Data Source Requirements

The MVP must use local seed data.

The seed data should include enough songs to make recommendations meaningful. The first target should be 50 to 100 songs.

Each song should contain metadata that supports recommendation scoring.

The song data should not require paid APIs.

The app should remain functional even without internet access once the local development environment is running.

## 3. Song Metadata Requirements

Each song should have metadata that supports both display and recommendation logic.

A song should include:

* Title
* Artist
* Album
* Main genre
* Duration
* Tempo or tempo category
* Energy level
* Darkness level
* Danceability level
* Valence or emotional brightness level
* Instrumentalness level
* Mood tags
* Vibe tags
* Optional description
* Optional cover image theme

The numeric attributes should use a simple 1 to 10 scale.

## 4. Attribute Definitions

### Energy

Energy describes how intense or active the song feels.

* 1 means very calm
* 10 means very intense

### Darkness

Darkness describes how mysterious, heavy, or shadowy the song feels.

* 1 means bright or light
* 10 means very dark or mysterious

### Danceability

Danceability describes how easy the song is to move to.

* 1 means not danceable
* 10 means highly danceable

### Valence

Valence describes emotional brightness.

* 1 means sad, dark, or emotionally heavy
* 10 means happy, bright, or uplifting

### Instrumentalness

Instrumentalness describes how much of the song is instrumental instead of vocal.

* 1 means mostly vocal
* 10 means mostly instrumental

## 5. Mood and Vibe Requirements

Mood tags describe emotional feeling.

Example mood tags:

* dreamy
* ethereal
* haunting
* melancholic
* euphoric
* romantic
* aggressive
* nostalgic
* focused
* late-night

Vibe tags describe listening context or atmosphere.

Example vibe tags:

* abyss
* night-drive
* gaming
* hiking
* study
* heartbreak
* gym
* rainy-night
* floating
* cinematic

A song can have multiple mood tags and multiple vibe tags.

This means the database needs many-to-many relationships between songs and mood tags, and between songs and vibe tags.

## 6. Recommendation Input Requirements

The user should be able to request recommendations using selected preferences.

The recommendation request may include:

* Selected mood tags
* Selected vibe tags
* Selected genres
* Target energy level
* Target darkness level
* Target danceability level
* Target valence level
* Optional selected favorite songs to compare against

Example request:

```json
{
  "moodTags": ["ethereal", "late-night"],
  "vibeTags": ["abyss", "night-drive"],
  "genreIds": [1, 4],
  "targetEnergy": 6,
  "targetDarkness": 8
}
```

The backend should return songs that best match these preferences.

## 7. Recommendation Output Requirements

Each recommendation result should include:

* Song ID
* Song title
* Artist name
* Album title if available
* Genre
* Score
* Rank
* Mood tags
* Vibe tags
* Explanation reasons

Example result:

```json
{
  "songId": 14,
  "title": "Dark Beach",
  "artist": "Pastel Ghost",
  "score": 91,
  "rank": 1,
  "reasons": [
    "Matches late-night mood",
    "Matches abyss vibe",
    "Similar darkness level",
    "Same genre"
  ]
}
```

The explanation system is important because it makes the recommendation algorithm understandable.

## 8. Recommendation Algorithm Requirements

The first version of the recommendation algorithm should use weighted scoring.

The algorithm should compare user-selected preferences against each song in the database.

Possible scoring logic:

| Attribute               |                           Points |
| ----------------------- | -------------------------------: |
| Mood tag match          | +25 points per matching mood tag |
| Vibe tag match          | +20 points per matching vibe tag |
| Genre match             |                       +20 points |
| Energy similarity       |                 Up to +15 points |
| Darkness similarity     |                 Up to +15 points |
| Danceability similarity |                 Up to +10 points |
| Valence similarity      |                 Up to +10 points |
| Tempo similarity        |                 Up to +10 points |

The algorithm should rank songs by total score from highest to lowest.

The algorithm should return explanation reasons based on which attributes contributed to the score.

## 9. Data Retrieval Requirements

The backend must retrieve song data from PostgreSQL using Entity Framework Core.

The backend should load related data when needed, including:

* Artist
* Album
* Genre
* Mood tags
* Vibe tags

The recommendation service should receive a complete list of candidate songs and their related metadata.

The backend should not rely on the frontend for recommendation calculations.

The frontend should only send user preferences and display returned results.

## 10. Data Parsing Requirements

Seed data should begin as a structured JSON file or C# seed method.

The seed data must be converted into database records.

The system should prevent duplicate artists, albums, genres, mood tags, and vibe tags during seeding.

The data parser or seeder should create relationships between songs and tags correctly.

Example seed data shape:

```json
{
  "title": "Example Song",
  "artist": "Example Artist",
  "album": "Example Album",
  "genre": "Darkwave",
  "durationSeconds": 210,
  "tempoBpm": 120,
  "energy": 6,
  "darkness": 8,
  "danceability": 5,
  "valence": 3,
  "instrumentalness": 4,
  "moodTags": ["ethereal", "late-night"],
  "vibeTags": ["abyss", "night-drive"]
}
```

## 11. Data Quality Requirements

The seed data should follow consistent rules.

All songs must have:

* Title
* Artist
* Genre
* Energy
* Darkness
* Danceability
* Valence
* Instrumentalness
* At least one mood tag
* At least one vibe tag

Optional fields include:

* Album
* Release year
* Duration
* Tempo
* Description
* Cover image URL or generated visual theme

Numeric values must stay between 1 and 10.

Duplicate song entries should be avoided.

Tag names should use consistent lowercase formatting.

## 12. Future Data Enhancements

After the MVP works, the project may add external data enrichment.

Possible future sources:

* Last.fm for tags and similar tracks
* MusicBrainz for structured artist and album metadata
* Cover Art Archive for album artwork metadata
* Spotify links for opening songs externally

These should remain optional enhancements.

The core app should continue working without any third-party API.

## 13. Data and Recommendation Success Criteria

This part of the project is successful when:

* Song seed data exists in a structured format
* Data can be loaded into PostgreSQL
* Songs are connected to artists, genres, moods, and vibes
* The backend can retrieve songs with related metadata
* The recommendation service can score and rank songs
* Each recommendation includes explanation reasons
* The algorithm can be tested with predictable results
* The data model is documented clearly enough to explain in an interview
