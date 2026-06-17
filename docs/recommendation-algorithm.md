# AbyssFM Recommendation Algorithm

## 1. Algorithm Overview

AbyssFM will use a custom weighted scoring algorithm for the MVP.

The goal of the algorithm is to recommend songs based on how closely each song matches the user's selected moods, vibes, genres, and target audio attributes.

The algorithm should be simple enough to explain clearly but meaningful enough to demonstrate backend logic and data-driven decision-making.

## 2. Recommendation Inputs

The algorithm may receive the following inputs:

- Selected mood tags
- Selected vibe tags
- Selected genre IDs
- Target energy level
- Target darkness level
- Target danceability level
- Target valence level
- Optional favorite song IDs

Example input:

```json
{
  "moodTags": ["ethereal", "late-night"],
  "vibeTags": ["abyss", "night-drive"],
  "genreIds": [1, 2],
  "targetEnergy": 6,
  "targetDarkness": 8,
  "targetDanceability": 5,
  "targetValence": 3
}