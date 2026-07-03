# AbyssFM Database Design Document

## 1. Database Design Overview

AbyssFM will use a relational PostgreSQL database to store song metadata, artists, albums, genres, mood tags, vibe tags, and recommendation results.

The database will be created through Entity Framework Core migrations from the ASP.NET Core backend. This means the final database schema should not be manually created in DBeaver. DBeaver will be used to inspect tables, run SQL queries, and verify data, but the official database structure should come from the C# entity models and EF Core migrations.

The database design supports the following goals:

* Store structured song metadata.
* Avoid duplicated artist, genre, mood, and vibe data.
* Support many-to-many relationships between songs and weighted classifications.
* Support a custom recommendation algorithm.
* Store recommendation sessions and results for future analysis.
* Map clearly to C# entity models in the backend.

---

## 2. Database System

| Area                       | Tool                                  |
| -------------------------- | ------------------------------------- |
| Database system            | PostgreSQL                            |
| Local database environment | PostgreSQL running inside Docker      |
| Database management tool   | DBeaver                               |
| Backend ORM                | Entity Framework Core                 |
| PostgreSQL .NET provider   | Npgsql.EntityFrameworkCore.PostgreSQL |

---

## 3. Naming Conventions

The PostgreSQL database will use lowercase `snake_case` names for tables and columns.

Examples:

```text
artists
albums
songs
song_genres
mood_tags
song_mood_tags
recommendation_sessions
```

The C# backend will use PascalCase names for entities and properties.

Examples:

```text
Artist
Album
Song
SongGenre
MoodTag
SongMoodTag
RecommendationSession
```

Entity Framework Core will map C# model names to PostgreSQL table and column names. This allows the backend code to stay readable while keeping the database schema clean and easy to query manually.

---

## 4. Final MVP Tables

The MVP database will include these tables:

```text
artists
albums
genres
songs
song_genres
mood_tags
vibe_tags
song_mood_tags
song_vibe_tags
recommendation_sessions
recommendation_results
```

The `songs` table stores the central song record.

The `genres`, `mood_tags`, and `vibe_tags` tables store reusable classification options.

The `song_genres`, `song_mood_tags`, and `song_vibe_tags` tables connect songs to those classifications with weights from 1 to 10.

---

## 5. Entity Relationship Summary

The core relationships are:

* One artist can have many albums.
* One artist can have many songs.
* One album can have many songs.
* One genre can be the primary genre for many songs.
* One song can have many weighted genres.
* One genre can apply to many songs through `song_genres`.
* One song can have many mood tags.
* One mood tag can apply to many songs through `song_mood_tags`.
* One song can have many vibe tags.
* One vibe tag can apply to many songs through `song_vibe_tags`.
* One recommendation session can have many recommendation results.
* One song can appear in many recommendation results.

Simplified relationship map:

```text
artists 1 ─── many albums
artists 1 ─── many songs
albums 1 ─── many songs

genres 1 ─── many songs as primary genre
songs many ─── many genres through song_genres

songs many ─── many mood_tags through song_mood_tags
songs many ─── many vibe_tags through song_vibe_tags

recommendation_sessions 1 ─── many recommendation_results
songs 1 ─── many recommendation_results
```

In plain English:

* `genres` stores the master list of genre options.
* `songs.primary_genre_id` stores the main genre displayed for a song.
* `song_genres` stores all weighted genres that apply to a song.
* `song_mood_tags` stores all weighted moods that apply to a song.
* `song_vibe_tags` stores all weighted vibes that apply to a song.

This lets the app display a simple primary genre while still supporting richer similarity scoring through weighted genre relationships.

---

# Core Metadata Tables

## 6. Table: `artists`

### Purpose

The `artists` table stores unique artists so that artist names are not duplicated across songs and albums.

### Columns

| Column       | Type         | Required | Description                                      |
| ------------ | ------------ | -------: | ------------------------------------------------ |
| `artist_id`  | integer      |      Yes | Primary key                                      |
| `name`       | varchar(150) |      Yes | Artist name                                      |
| `slug`       | varchar(180) |      Yes | URL/database-friendly version of the artist name |
| `image_url`  | text         |       No | Optional artist image URL                        |
| `created_at` | timestamptz  |      Yes | Record creation timestamp                        |
| `updated_at` | timestamptz  |       No | Last update timestamp                            |

### Primary Key

```text
artist_id
```

### Constraints

* `name` should be unique.
* `slug` should be unique.

### Example Data

| name         | slug         |
| ------------ | ------------ |
| Pastel Ghost | pastel-ghost |
| Frank Ocean  | frank-ocean  |
| Travis Scott | travis-scott |

---

## 7. Table: `albums`

### Purpose

The `albums` table stores album or project information for songs. Albums are optional because some songs in the MVP seed data may not have a known album.

### Columns

| Column            | Type         | Required | Description                                       |
| ----------------- | ------------ | -------: | ------------------------------------------------- |
| `album_id`        | integer      |      Yes | Primary key                                       |
| `title`           | varchar(200) |      Yes | Album title                                       |
| `artist_id`       | integer      |      Yes | Foreign key to `artists`                          |
| `release_year`    | integer      |       No | Year the album was released                       |
| `cover_image_url` | text         |       No | Optional album cover URL                          |
| `cover_theme`     | varchar(100) |       No | Visual theme used for generated or mock cover art |
| `created_at`      | timestamptz  |      Yes | Record creation timestamp                         |
| `updated_at`      | timestamptz  |       No | Last update timestamp                             |

### Primary Key

```text
album_id
```

### Foreign Keys

```text
artist_id references artists.artist_id
```

### Constraints

* The combination of `title` and `artist_id` should be unique when possible.

### Example Data

| title  | artist       | release_year | cover_theme    |
| ------ | ------------ | -----------: | -------------- |
| Abyss  | Pastel Ghost |         2015 | purple-roses   |
| Blonde | Frank Ocean  |         2016 | golden-minimal |

---

## 8. Table: `genres`

### Purpose

The `genres` table stores unique music genres so that genre names are not duplicated across songs.

Genres are reusable. A genre can be used as a song's primary display genre and can also appear inside `song_genres` as a weighted genre match.

### Columns

| Column       | Type         | Required | Description                                |
| ------------ | ------------ | -------: | ------------------------------------------ |
| `genre_id`   | integer      |      Yes | Primary key                                |
| `name`       | varchar(80)  |      Yes | Genre name                                 |
| `slug`       | varchar(100) |      Yes | URL/database-friendly version of the genre |
| `created_at` | timestamptz  |      Yes | Record creation timestamp                  |

### Primary Key

```text
genre_id
```

### Constraints

* `name` should be unique.
* `slug` should be unique.

### Example Data

| name         | slug         |
| ------------ | ------------ |
| Darkwave     | darkwave     |
| Dream Pop    | dream-pop    |
| R&B          | r-and-b      |
| Hip-Hop      | hip-hop      |
| Ambient      | ambient      |
| Shoegaze     | shoegaze     |
| Synthpop     | synthpop     |
| Alternative  | alternative  |
| Experimental | experimental |

---

## 9. Table: `songs`

### Purpose

The `songs` table stores the main song records used by the recommendation system.

Each song stores basic display metadata, one primary display genre, and numeric attributes used by the recommendation algorithm.

Genres used for similarity scoring are stored separately in the `song_genres` join table.

### Columns

| Column              | Type         | Required | Description                                              |
| ------------------- | ------------ | -------: | -------------------------------------------------------- |
| `song_id`           | integer      |      Yes | Primary key                                              |
| `title`             | varchar(200) |      Yes | Song title                                               |
| `primary_artist_id` | integer      |      Yes | Foreign key to `artists`                                 |
| `album_id`          | integer      |       No | Optional foreign key to `albums`                         |
| `primary_genre_id`  | integer      |      Yes | Foreign key to `genres`; main display genre for the song |
| `duration_seconds`  | integer      |       No | Song duration in seconds                                 |
| `tempo_bpm`         | integer      |       No | Tempo in beats per minute                                |
| `energy`            | smallint     |      Yes | Intensity level from 1 to 10                             |
| `darkness`          | smallint     |      Yes | Darkness or mystery level from 1 to 10                   |
| `danceability`      | smallint     |      Yes | Danceability level from 1 to 10                          |
| `valence`           | smallint     |      Yes | Emotional brightness level from 1 to 10                  |
| `instrumentalness`  | smallint     |      Yes | Instrumentalness level from 1 to 10                      |
| `description`       | text         |       No | Optional short song description                          |
| `preview_url`       | text         |       No | Optional preview or audio URL for future use             |
| `external_url`      | text         |       No | Optional external song link                              |
| `cover_theme`       | varchar(100) |       No | Visual theme for generated or mock cover art             |
| `created_at`        | timestamptz  |      Yes | Record creation timestamp                                |
| `updated_at`        | timestamptz  |       No | Last update timestamp                                    |

### Primary Key

```text
song_id
```

### Foreign Keys

```text
primary_artist_id references artists.artist_id
album_id references albums.album_id
primary_genre_id references genres.genre_id
```

### Constraints

* `energy` must be between 1 and 10.
* `darkness` must be between 1 and 10.
* `danceability` must be between 1 and 10.
* `valence` must be between 1 and 10.
* `instrumentalness` must be between 1 and 10.
* The combination of `title`, `primary_artist_id`, and `album_id` should be unique when possible.

### Attribute Meanings

#### Energy

Describes how intense or active the song feels.

```text
1 = very calm
10 = very intense
```

#### Darkness

Describes how mysterious, shadowy, heavy, or abyss-like the song feels.

```text
1 = bright/light
10 = very dark/mysterious
```

#### Danceability

Describes how easy the song is to move to.

```text
1 = not danceable
10 = highly danceable
```

#### Valence

Describes emotional brightness.

```text
1 = sad/dark/emotionally heavy
10 = happy/bright/uplifting
```

#### Instrumentalness

Describes how much the song feels instrumental instead of vocal-driven.

```text
1 = mostly vocal
10 = mostly instrumental
```

### Example Data

| title      | artist       | primary_genre | energy | darkness | danceability | valence | instrumentalness |
| ---------- | ------------ | ------------- | -----: | -------: | -----------: | ------: | ---------------: |
| Prism      | Pastel Ghost | Darkwave      |      6 |        8 |            6 |       3 |                4 |
| Dark Beach | Pastel Ghost | Darkwave      |      7 |        9 |            7 |       2 |                5 |
| Nights     | Frank Ocean  | R&B           |      5 |        6 |            5 |       4 |                3 |

---

## 10. Table: `song_genres`

### Purpose

The `song_genres` table connects songs to all genres that apply to them.

A song can have many genres, and a genre can apply to many songs.

This table supports weighted genre similarity for the recommendation algorithm.

The `songs.primary_genre_id` column stores the main display genre, while `song_genres` stores all genres used for scoring.

Example:

```text
Song: Nights

Primary genre:
R&B

Weighted genres:
R&B, weight 10
Alternative, weight 7
Experimental, weight 5
```

### Columns

| Column     | Type     | Required | Description                                            |
| ---------- | -------- | -------: | ------------------------------------------------------ |
| `song_id`  | integer  |      Yes | Foreign key to `songs`                                 |
| `genre_id` | integer  |      Yes | Foreign key to `genres`                                |
| `weight`   | smallint |      Yes | Strength of the genre match for this song from 1 to 10 |

### Primary Key

Composite primary key:

```text
song_id + genre_id
```

### Foreign Keys

```text
song_id references songs.song_id
genre_id references genres.genre_id
```

### Constraints

* `weight` must be between 1 and 10.
* Each song should have at least one `song_genres` record.
* The primary genre should also appear in `song_genres` with a high weight, usually 10.

### Example Data

| song       | genre        | weight |
| ---------- | ------------ | -----: |
| Nights     | R&B          |     10 |
| Nights     | Alternative  |      7 |
| Nights     | Experimental |      5 |
| Dark Beach | Darkwave     |     10 |
| Dark Beach | Synthpop     |      7 |
| Dark Beach | Dream Pop    |      4 |

---

# Tag Tables

## 11. Table: `mood_tags`

### Purpose

The `mood_tags` table stores emotional tags used to describe how a song feels.

Mood tags describe emotional state.

### Example Mood Tags

```text
dreamy
ethereal
haunting
melancholic
euphoric
romantic
aggressive
nostalgic
focused
late-night
```

### Columns

| Column         | Type         | Required | Description                                   |
| -------------- | ------------ | -------: | --------------------------------------------- |
| `mood_tag_id`  | integer      |      Yes | Primary key                                   |
| `name`         | varchar(80)  |      Yes | Mood tag name                                 |
| `slug`         | varchar(100) |      Yes | URL/database-friendly version of the mood tag |
| `description`  | text         |       No | Optional mood description                     |
| `display_icon` | varchar(30)  |       No | Optional emoji or icon label                  |
| `created_at`   | timestamptz  |      Yes | Record creation timestamp                     |

### Primary Key

```text
mood_tag_id
```

### Constraints

* `name` should be unique.
* `slug` should be unique.

### Example Data

| name       | slug       | display_icon |
| ---------- | ---------- | ------------ |
| dreamy     | dreamy     | moon         |
| ethereal   | ethereal   | sparkle      |
| haunting   | haunting   | ghost        |
| late-night | late-night | crescent     |

---

## 12. Table: `vibe_tags`

### Purpose

The `vibe_tags` table stores contextual or atmospheric tags used to describe where or when a song fits.

Vibe tags describe listening context or atmosphere.

Vibes are optional during recommendation search. They act as a refinement layer, not a required matching category.

### Example Vibe Tags

```text
abyss
night-drive
gaming
hiking
study
heartbreak
gym
rainy-night
floating
cinematic
```

### Columns

| Column         | Type         | Required | Description                                   |
| -------------- | ------------ | -------: | --------------------------------------------- |
| `vibe_tag_id`  | integer      |      Yes | Primary key                                   |
| `name`         | varchar(80)  |      Yes | Vibe tag name                                 |
| `slug`         | varchar(100) |      Yes | URL/database-friendly version of the vibe tag |
| `description`  | text         |       No | Optional vibe description                     |
| `display_icon` | varchar(30)  |       No | Optional emoji or icon label                  |
| `created_at`   | timestamptz  |      Yes | Record creation timestamp                     |

### Primary Key

```text
vibe_tag_id
```

### Constraints

* `name` should be unique.
* `slug` should be unique.

### Example Data

| name        | slug        | display_icon |
| ----------- | ----------- | ------------ |
| abyss       | abyss       | void         |
| night-drive | night-drive | car          |
| gaming      | gaming      | controller   |
| hiking      | hiking      | mountain     |

---

# Join Tables

## 13. Table: `song_mood_tags`

### Purpose

The `song_mood_tags` table connects songs to mood tags.

A song can have many mood tags, and a mood tag can belong to many songs.

This table supports the many-to-many relationship between `songs` and `mood_tags`.

### Columns

| Column        | Type     | Required | Description                                         |
| ------------- | -------- | -------: | --------------------------------------------------- |
| `song_id`     | integer  |      Yes | Foreign key to `songs`                              |
| `mood_tag_id` | integer  |      Yes | Foreign key to `mood_tags`                          |
| `weight`      | smallint |      Yes | Strength of the mood tag for this song from 1 to 10 |

### Primary Key

Composite primary key:

```text
song_id + mood_tag_id
```

### Foreign Keys

```text
song_id references songs.song_id
mood_tag_id references mood_tags.mood_tag_id
```

### Constraints

* `weight` must be between 1 and 10.
* Each song should have at least one `song_mood_tags` record.

### Example Data

| song       | mood_tag   | weight |
| ---------- | ---------- | -----: |
| Prism      | ethereal   |      9 |
| Prism      | late-night |      8 |
| Dark Beach | haunting   |      9 |

---

## 14. Table: `song_vibe_tags`

### Purpose

The `song_vibe_tags` table connects songs to vibe tags.

A song can have many vibe tags, and a vibe tag can belong to many songs.

This table supports the many-to-many relationship between `songs` and `vibe_tags`.

### Columns

| Column        | Type     | Required | Description                                         |
| ------------- | -------- | -------: | --------------------------------------------------- |
| `song_id`     | integer  |      Yes | Foreign key to `songs`                              |
| `vibe_tag_id` | integer  |      Yes | Foreign key to `vibe_tags`                          |
| `weight`      | smallint |      Yes | Strength of the vibe tag for this song from 1 to 10 |

### Primary Key

Composite primary key:

```text
song_id + vibe_tag_id
```

### Foreign Keys

```text
song_id references songs.song_id
vibe_tag_id references vibe_tags.vibe_tag_id
```

### Constraints

* `weight` must be between 1 and 10.
* Each song should have at least one `song_vibe_tags` record.

### Example Data

| song   | vibe_tag    | weight |
| ------ | ----------- | -----: |
| Prism  | abyss       |      9 |
| Prism  | night-drive |      7 |
| Nights | study       |      8 |

---

# Recommendation Tables

## 15. Table: `recommendation_sessions`

### Purpose

The `recommendation_sessions` table stores each recommendation request made by the user.

This table is optional for a basic MVP, but it makes the project more technically meaningful because the app can save what the user selected and what the system returned.

There will be no user accounts in the MVP, so a recommendation session represents one anonymous recommendation request.

### Columns

| Column                       | Type        | Required | Description                                             |
| ---------------------------- | ----------- | -------: | ------------------------------------------------------- |
| `recommendation_session_id`  | uuid        |      Yes | Primary key                                             |
| `selected_mood_tag_ids_json` | text        |       No | JSON string of selected mood tag IDs                    |
| `selected_genre_ids_json`    | text        |       No | JSON string of selected genre IDs                       |
| `selected_vibe_tag_ids_json` | text        |       No | JSON string of selected vibe tag IDs                    |
| `target_energy`              | smallint    |       No | User-selected target energy                             |
| `target_darkness`            | smallint    |       No | User-selected target darkness                           |
| `target_danceability`        | smallint    |       No | User-selected target danceability                       |
| `target_valence`             | smallint    |       No | User-selected target valence                            |
| `target_instrumentalness`    | smallint    |       No | User-selected target instrumentalness                   |
| `source`                     | varchar(50) |      Yes | Source of request, such as `mvp`, `frontend`, or `test` |
| `created_at`                 | timestamptz |      Yes | Session creation timestamp                              |

### Primary Key

```text
recommendation_session_id
```

### Constraints

* `target_energy` must be null or between 1 and 10.
* `target_darkness` must be null or between 1 and 10.
* `target_danceability` must be null or between 1 and 10.
* `target_valence` must be null or between 1 and 10.
* `target_instrumentalness` must be null or between 1 and 10.

### Request Rules

* The user must select at least one mood.
* The user must select at least one genre.
* Vibe selection is optional.
* Numeric profile is optional.
* If one numeric target is provided, all numeric targets must be provided.

### Example Data

```text
recommendation_session_id: generated UUID
selected mood tag IDs: 1, 4
selected genre IDs: 1, 7
selected vibe tag IDs: 1, 2
target energy: 6
target darkness: 8
target danceability: 5
target valence: 3
target instrumentalness: 4
source: frontend
```

---

## 16. Table: `recommendation_results`

### Purpose

The `recommendation_results` table stores the ranked songs returned for a recommendation session.

Each recommendation result belongs to one session and one song.

### Columns

| Column                      | Type             | Required | Description                                |
| --------------------------- | ---------------- | -------: | ------------------------------------------ |
| `recommendation_result_id`  | integer          |      Yes | Primary key                                |
| `recommendation_session_id` | uuid             |      Yes | Foreign key to `recommendation_sessions`   |
| `song_id`                   | integer          |      Yes | Foreign key to `songs`                     |
| `rank`                      | integer          |      Yes | Result order, starting at 1                |
| `score`                     | double precision |      Yes | Final recommendation score                 |
| `reasons_json`              | text             |      Yes | JSON string containing explanation reasons |
| `created_at`                | timestamptz      |      Yes | Result creation timestamp                  |

### Primary Key

```text
recommendation_result_id
```

### Foreign Keys

```text
recommendation_session_id references recommendation_sessions.recommendation_session_id
song_id references songs.song_id
```

### Constraints

* `rank` must be greater than 0.
* `score` must be greater than or equal to 0.
* The combination of `recommendation_session_id` and `song_id` should be unique.
* The combination of `recommendation_session_id` and `rank` should be unique.

### Example Result

```text
Rank 1: Dark Beach, score 83.25
Reasons: matches ethereal mood, matches abyss vibe, strong darkwave genre match, similar darkness level
```

---

# Indexes and Delete Behavior

## 17. Recommended Indexes

Indexes improve lookup performance and make the schema more realistic.

### `artists`

* Unique index on `name`.
* Unique index on `slug`.

### `albums`

* Index on `artist_id`.
* Unique index on `title` and `artist_id`.

### `genres`

* Unique index on `name`.
* Unique index on `slug`.

### `songs`

* Index on `primary_artist_id`.
* Index on `album_id`.
* Index on `primary_genre_id`.
* Index on `energy`.
* Index on `darkness`.
* Index on `danceability`.
* Index on `valence`.
* Index on `instrumentalness`.

### `song_genres`

* Composite primary key on `song_id` and `genre_id`.
* Index on `genre_id`.

### `mood_tags`

* Unique index on `name`.
* Unique index on `slug`.

### `vibe_tags`

* Unique index on `name`.
* Unique index on `slug`.

### `song_mood_tags`

* Composite primary key on `song_id` and `mood_tag_id`.
* Index on `mood_tag_id`.

### `song_vibe_tags`

* Composite primary key on `song_id` and `vibe_tag_id`.
* Index on `vibe_tag_id`.

### `recommendation_results`

* Index on `recommendation_session_id`.
* Index on `song_id`.
* Unique index on `recommendation_session_id` and `song_id`.
* Unique index on `recommendation_session_id` and `rank`.

---

## 18. Delete Behavior

### Artists

Artists should not be deleted automatically if songs or albums still reference them.

Recommended behavior:

```text
Restrict delete if songs or albums exist.
```

### Albums

If an album is deleted, songs should not automatically be deleted. Songs can remain with a null `album_id`.

Recommended behavior:

```text
Set album_id to null on songs.
```

### Genres

Genres should not be deleted if songs still reference them as a primary genre or through `song_genres`.

Recommended behavior:

```text
Restrict delete if songs or song_genres records exist.
```

### Songs

If a song is deleted, related join table records should also be deleted.

Recommended behavior:

```text
Cascade delete song_genres.
Cascade delete song_mood_tags.
Cascade delete song_vibe_tags.
Cascade delete recommendation_results.
```

### Mood Tags and Vibe Tags

Mood tags and vibe tags should not be deleted if songs still reference them through join table records.

Recommended behavior:

```text
Restrict delete if join table records exist.
```

### Recommendation Sessions

If a recommendation session is deleted, its related recommendation results should be deleted.

Recommended behavior:

```text
Cascade delete recommendation_results.
```

---

# Design Decisions

## 19. Normalization Decisions

### Artists are separated

Instead of storing the artist name directly in every song row, songs reference the `artists` table.

This prevents duplicate artist names such as:

```text
Pastel Ghost
pastel ghost
PastelGhost
```

### Genres are separated

Instead of storing raw genre strings in songs, songs reference the `genres` table.

This keeps genre names consistent.

The `songs` table stores a `primary_genre_id` for the main display genre.

The `song_genres` table stores all weighted genres that apply to a song.

This allows a song to have one clear display genre while still supporting hybrid genre similarity.

Example:

```text
Song: Dark Beach

Primary genre:
Darkwave

Weighted genres:
Darkwave, weight 10
Synthpop, weight 7
Dream Pop, weight 4
```

This is better than storing only one `genre_id`, because music often blends several genres.

### Mood tags and vibe tags are separated

Mood tags and vibe tags are reusable across many songs.

Because songs can have multiple moods and vibes, the database uses join tables:

```text
song_mood_tags
song_vibe_tags
```

### Numeric attributes stay on songs

Attributes like `energy`, `darkness`, `danceability`, `valence`, and `instrumentalness` belong directly to a song because they describe the song itself.

### Recommendation results are persisted

Recommendation sessions and results are stored so the app can later support analytics, debugging, and recommendation history.

For the MVP, the app can still work even if recommendation history is not displayed in the frontend.

---

# Seed Data

## 20. Seed Data Strategy

The MVP will use local seed data.

Seed data should live in:

```text
database/seed-data/songs.seed.json
```

The backend seeder should read the seed file and insert records into PostgreSQL.

The seed process should:

1. Read each song from the JSON file.
2. Create the artist if it does not already exist.
3. Create the album if provided and if it does not already exist.
4. Create the primary genre if it does not already exist.
5. Create all weighted genres if they do not already exist.
6. Create mood tags if they do not already exist.
7. Create vibe tags if they do not already exist.
8. Create the song.
9. Create `song_genres` records.
10. Create `song_mood_tags` records.
11. Create `song_vibe_tags` records.
12. Avoid duplicate artists, albums, genres, tags, and songs.

---

## 21. Seed Data Shape

Each seed data object should include:

* `title`
* `artist`
* `album`
* `releaseYear`
* `primaryGenre`
* `genres`
* `durationSeconds`
* `tempoBpm`
* `energy`
* `darkness`
* `danceability`
* `valence`
* `instrumentalness`
* `description`
* `coverTheme`
* `moodTags`
* `vibeTags`

### Example Seed Data Object

```json
{
  "title": "Prism",
  "artist": "Pastel Ghost",
  "album": "Abyss",
  "releaseYear": 2015,
  "primaryGenre": "Darkwave",
  "genres": [
    {
      "name": "Darkwave",
      "weight": 10
    },
    {
      "name": "Synthpop",
      "weight": 7
    },
    {
      "name": "Dream Pop",
      "weight": 4
    }
  ],
  "durationSeconds": 209,
  "tempoBpm": 120,
  "energy": 6,
  "darkness": 8,
  "danceability": 6,
  "valence": 3,
  "instrumentalness": 4,
  "description": "A dreamy darkwave track with an ethereal late-night atmosphere.",
  "coverTheme": "purple-roses",
  "moodTags": [
    {
      "name": "ethereal",
      "weight": 9
    },
    {
      "name": "late-night",
      "weight": 8
    }
  ],
  "vibeTags": [
    {
      "name": "abyss",
      "weight": 9
    },
    {
      "name": "night-drive",
      "weight": 7
    }
  ]
}
```

---

# Recommendation Flow and API Mapping

## 22. Recommendation Data Flow

The recommendation flow will work like this:

```text
React frontend collects user preferences
        ↓
Frontend sends a POST request to /api/recommendations
        ↓
ASP.NET Core controller receives the request DTO
        ↓
RecommendationService loads songs and related metadata from PostgreSQL
        ↓
RecommendationService scores each candidate song
        ↓
RecommendationService generates explanation reasons
        ↓
Backend saves the recommendation session and results
        ↓
Backend returns ranked results as JSON
        ↓
React frontend displays recommendations
```

---

## 23. Recommendation Query Requirements

The backend recommendation service must load songs with related metadata.

Required related data:

* Primary artist
* Album
* Primary genre
* Weighted genres
* Mood tags
* Vibe tags

The recommendation algorithm should not depend on frontend-only data.

The frontend sends user preferences. The backend is responsible for calculating results.

The backend should compare selected IDs against weighted song relationships.

Example:

```text
User selected genre IDs:
1, 2

Song weighted genres:
genre_id 1, weight 10
genre_id 2, weight 7
genre_id 4, weight 4
```

The backend should score the selected genre matches using the stored weights.

---

## 24. API DTO Mapping

The backend should not return raw Entity Framework Core entity models directly to the frontend.

Instead, the backend should use DTOs.

### Song Response DTO Recommended Fields

* `songId`
* `title`
* `artist`
* `album`
* `primaryGenre`
* `durationSeconds`
* `tempoBpm`
* `energy`
* `darkness`
* `danceability`
* `valence`
* `instrumentalness`
* `description`
* `coverTheme`
* `genres`
* `moodTags`
* `vibeTags`

### Recommendation Request DTO Recommended Fields

* `selectedMoodTagIds`
* `selectedGenreIds`
* `selectedVibeTagIds`
* `targetEnergy`
* `targetDarkness`
* `targetDanceability`
* `targetValence`
* `targetInstrumentalness`

Request rules:

* The user must select at least one mood.
* The user must select at least one genre.
* Vibe selection is optional.
* Numeric profile is optional.
* If one numeric target is provided, all numeric targets must be provided.

### Recommendation Result DTO Recommended Fields

* `rank`
* `songId`
* `title`
* `artist`
* `album`
* `primaryGenre`
* `score`
* `genres`
* `moodTags`
* `vibeTags`
* `reasons`

---

# C# Backend Mapping

## 25. C# Entity Model Mapping

The following C# entities should be created in the backend:

```text
Artist
Album
Genre
Song
SongGenre
MoodTag
VibeTag
SongMoodTag
SongVibeTag
RecommendationSession
RecommendationResult
```

The following relationships should be configured in `AbyssFmDbContext`:

* `Artist` has many `Albums`.
* `Artist` has many `Songs`.
* `Album` has many `Songs`.
* `Genre` has many primary genre `Songs`.
* `Song` has one primary `Genre`.
* `Song` has many `SongGenres`.
* `Genre` has many `SongGenres`.
* `Song` has many `SongMoodTags`.
* `MoodTag` has many `SongMoodTags`.
* `Song` has many `SongVibeTags`.
* `VibeTag` has many `SongVibeTags`.
* `RecommendationSession` has many `RecommendationResults`.
* `Song` has many `RecommendationResults`.

The join tables should use composite keys:

* `SongGenre` uses `SongId` and `GenreId`.
* `SongMoodTag` uses `SongId` and `MoodTagId`.
* `SongVibeTag` uses `SongId` and `VibeTagId`.

---

## 26. Validation Requirements

The backend should validate database-bound values before saving.

### A song must have:

* Title
* Primary artist
* Primary genre
* Energy
* Darkness
* Danceability
* Valence
* Instrumentalness
* At least one weighted genre
* At least one mood tag
* At least one vibe tag

### The following numeric values must be between 1 and 10:

* `energy`
* `darkness`
* `danceability`
* `valence`
* `instrumentalness`
* `song genre weight`
* `song mood tag weight`
* `song vibe tag weight`
* `target energy`
* `target darkness`
* `target danceability`
* `target valence`
* `target instrumentalness`

### Recommendation request validation

The recommendation request must follow these rules:

* The user must select at least one mood.
* The user must select at least one genre.
* Vibe selection is optional.
* Numeric profile is optional.
* If one numeric target is provided, all numeric targets must be provided.
* Numeric target values must be between 1 and 10.

### Text validation rules

* Names should be trimmed before saving.
* Tag names should be lowercase.
* Slugs should be generated consistently.

Example slug conversions:

```text
Late Night -> late-night
Dream Pop -> dream-pop
R&B -> r-and-b
```

---

# MVP Scope and Future Enhancements

## 27. MVP Database Scope

The MVP database includes anonymous song recommendation data only.

The MVP does not include:

* User accounts
* Passwords
* OAuth login
* Spotify authentication
* Payment information
* Private user data
* Social features
* Comments
* Likes
* User-owned playlists

These can be added later if needed.

---

## 28. Future Database Enhancements

Future versions may add:

```text
users
saved_playlists
playlist_songs
favorite_songs
listening_history
song_similarity_edges
external_music_metadata
artist_aliases
song_audio_features
```

### Possible Future Table: `users`

Purpose:

Stores authenticated users and saved preferences.

### Possible Future Table: `saved_playlists`

Purpose:

Stores user-generated or system-generated playlists.

### Possible Future Table: `playlist_songs`

Purpose:

Supports many-to-many relationships between playlists and songs.

### Possible Future Table: `favorite_songs`

Purpose:

Stores user feedback and personalization data.

### Possible Future Table: `song_similarity_edges`

Purpose:

Supports graph-based recommendations.

Potential columns:

* `song_a_id`
* `song_b_id`
* `similarity_score`
* `reason`

---

# Success Criteria

## 29. Database Success Criteria

The database design is successful when:

* EF Core migrations can create all tables.
* DBeaver can inspect all generated tables.
* Songs can be connected to artists, albums, genres, moods, and vibes.
* Seed data can be loaded without duplicates.
* The backend can retrieve songs with related metadata.
* The recommendation service can score and rank songs.
* Recommendation sessions and results can be saved.
* The schema is understandable from GitHub documentation.
* The database design can be clearly explained in interviews.

---

## 30. Final MVP Schema Summary

Final MVP tables:

```text
artists
albums
genres
songs
song_genres
mood_tags
vibe_tags
song_mood_tags
song_vibe_tags
recommendation_sessions
recommendation_results
```

Core idea:

* Songs are the central table.
* Artists and albums describe the song.
* `primary_genre_id` gives each song one main display genre.
* `song_genres` gives each song multiple weighted genres for similarity scoring.
* Mood tags and vibe tags classify the emotional and contextual feel of the song.
* Recommendation sessions store user requests.
* Recommendation results store ranked song matches.

This design is normalized enough to show real database understanding while still being manageable for the first version of AbyssFM.

The recommendation system works because each song becomes a structured feature profile:

```text
metadata
numeric attributes
weighted genres
weighted mood tags
weighted vibe tags
```

The backend can compare that song profile against the user's selected mood, genre, optional vibe, and optional numeric profile to calculate a ranked recommendation score.
