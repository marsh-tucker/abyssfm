using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace AbyssFm.Api.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "artists",
                columns: table => new
                {
                    artist_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    slug = table.Column<string>(type: "character varying(180)", maxLength: 180, nullable: false),
                    image_url = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamptz", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamptz", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_artists", x => x.artist_id);
                });

            migrationBuilder.CreateTable(
                name: "genres",
                columns: table => new
                {
                    genre_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    slug = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamptz", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_genres", x => x.genre_id);
                });

            migrationBuilder.CreateTable(
                name: "mood_tags",
                columns: table => new
                {
                    mood_tag_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    slug = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    display_icon = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamptz", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mood_tags", x => x.mood_tag_id);
                });

            migrationBuilder.CreateTable(
                name: "recommendation_sessions",
                columns: table => new
                {
                    recommendation_session_id = table.Column<Guid>(type: "uuid", nullable: false),
                    selected_mood_tag_ids_json = table.Column<string>(type: "text", nullable: true),
                    selected_genre_ids_json = table.Column<string>(type: "text", nullable: true),
                    selected_vibe_tag_ids_json = table.Column<string>(type: "text", nullable: true),
                    target_energy = table.Column<short>(type: "smallint", nullable: true),
                    target_darkness = table.Column<short>(type: "smallint", nullable: true),
                    target_danceability = table.Column<short>(type: "smallint", nullable: true),
                    target_valence = table.Column<short>(type: "smallint", nullable: true),
                    target_instrumentalness = table.Column<short>(type: "smallint", nullable: true),
                    source = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamptz", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_recommendation_sessions", x => x.recommendation_session_id);
                    table.CheckConstraint("ck_recommendation_sessions_target_danceability_range", "target_danceability IS NULL OR target_danceability BETWEEN 1 AND 10");
                    table.CheckConstraint("ck_recommendation_sessions_target_darkness_range", "target_darkness IS NULL OR target_darkness BETWEEN 1 AND 10");
                    table.CheckConstraint("ck_recommendation_sessions_target_energy_range", "target_energy IS NULL OR target_energy BETWEEN 1 AND 10");
                    table.CheckConstraint("ck_recommendation_sessions_target_instrumentalness_range", "target_instrumentalness IS NULL OR target_instrumentalness BETWEEN 1 AND 10");
                    table.CheckConstraint("ck_recommendation_sessions_target_valence_range", "target_valence IS NULL OR target_valence BETWEEN 1 AND 10");
                });

            migrationBuilder.CreateTable(
                name: "vibe_tags",
                columns: table => new
                {
                    vibe_tag_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    slug = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    display_icon = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamptz", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_vibe_tags", x => x.vibe_tag_id);
                });

            migrationBuilder.CreateTable(
                name: "albums",
                columns: table => new
                {
                    album_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    artist_id = table.Column<int>(type: "integer", nullable: false),
                    release_year = table.Column<int>(type: "integer", nullable: true),
                    cover_image_url = table.Column<string>(type: "text", nullable: true),
                    cover_theme = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamptz", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamptz", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_albums", x => x.album_id);
                    table.ForeignKey(
                        name: "FK_albums_artists_artist_id",
                        column: x => x.artist_id,
                        principalTable: "artists",
                        principalColumn: "artist_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "songs",
                columns: table => new
                {
                    song_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    primary_artist_id = table.Column<int>(type: "integer", nullable: false),
                    album_id = table.Column<int>(type: "integer", nullable: true),
                    primary_genre_id = table.Column<int>(type: "integer", nullable: false),
                    duration_seconds = table.Column<int>(type: "integer", nullable: true),
                    tempo_bpm = table.Column<int>(type: "integer", nullable: true),
                    energy = table.Column<short>(type: "smallint", nullable: false),
                    darkness = table.Column<short>(type: "smallint", nullable: false),
                    danceability = table.Column<short>(type: "smallint", nullable: false),
                    valence = table.Column<short>(type: "smallint", nullable: false),
                    instrumentalness = table.Column<short>(type: "smallint", nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    preview_url = table.Column<string>(type: "text", nullable: true),
                    external_url = table.Column<string>(type: "text", nullable: true),
                    cover_theme = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamptz", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamptz", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_songs", x => x.song_id);
                    table.CheckConstraint("ck_songs_danceability_range", "danceability BETWEEN 1 AND 10");
                    table.CheckConstraint("ck_songs_darkness_range", "darkness BETWEEN 1 AND 10");
                    table.CheckConstraint("ck_songs_energy_range", "energy BETWEEN 1 AND 10");
                    table.CheckConstraint("ck_songs_instrumentalness_range", "instrumentalness BETWEEN 1 AND 10");
                    table.CheckConstraint("ck_songs_valence_range", "valence BETWEEN 1 AND 10");
                    table.ForeignKey(
                        name: "FK_songs_albums_album_id",
                        column: x => x.album_id,
                        principalTable: "albums",
                        principalColumn: "album_id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_songs_artists_primary_artist_id",
                        column: x => x.primary_artist_id,
                        principalTable: "artists",
                        principalColumn: "artist_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_songs_genres_primary_genre_id",
                        column: x => x.primary_genre_id,
                        principalTable: "genres",
                        principalColumn: "genre_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "recommendation_results",
                columns: table => new
                {
                    recommendation_result_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    recommendation_session_id = table.Column<Guid>(type: "uuid", nullable: false),
                    song_id = table.Column<int>(type: "integer", nullable: false),
                    rank = table.Column<int>(type: "integer", nullable: false),
                    score = table.Column<double>(type: "double precision", nullable: false),
                    reasons_json = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamptz", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_recommendation_results", x => x.recommendation_result_id);
                    table.CheckConstraint("ck_recommendation_results_rank_positive", "rank > 0");
                    table.CheckConstraint("ck_recommendation_results_score_non_negative", "score >= 0");
                    table.ForeignKey(
                        name: "FK_recommendation_results_recommendation_sessions_recommendati~",
                        column: x => x.recommendation_session_id,
                        principalTable: "recommendation_sessions",
                        principalColumn: "recommendation_session_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_recommendation_results_songs_song_id",
                        column: x => x.song_id,
                        principalTable: "songs",
                        principalColumn: "song_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "song_genres",
                columns: table => new
                {
                    song_id = table.Column<int>(type: "integer", nullable: false),
                    genre_id = table.Column<int>(type: "integer", nullable: false),
                    weight = table.Column<short>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_song_genres", x => new { x.song_id, x.genre_id });
                    table.CheckConstraint("ck_song_genres_weight_range", "weight BETWEEN 1 AND 10");
                    table.ForeignKey(
                        name: "FK_song_genres_genres_genre_id",
                        column: x => x.genre_id,
                        principalTable: "genres",
                        principalColumn: "genre_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_song_genres_songs_song_id",
                        column: x => x.song_id,
                        principalTable: "songs",
                        principalColumn: "song_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "song_mood_tags",
                columns: table => new
                {
                    song_id = table.Column<int>(type: "integer", nullable: false),
                    mood_tag_id = table.Column<int>(type: "integer", nullable: false),
                    weight = table.Column<short>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_song_mood_tags", x => new { x.song_id, x.mood_tag_id });
                    table.CheckConstraint("ck_song_mood_tags_weight_range", "weight BETWEEN 1 AND 10");
                    table.ForeignKey(
                        name: "FK_song_mood_tags_mood_tags_mood_tag_id",
                        column: x => x.mood_tag_id,
                        principalTable: "mood_tags",
                        principalColumn: "mood_tag_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_song_mood_tags_songs_song_id",
                        column: x => x.song_id,
                        principalTable: "songs",
                        principalColumn: "song_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "song_vibe_tags",
                columns: table => new
                {
                    song_id = table.Column<int>(type: "integer", nullable: false),
                    vibe_tag_id = table.Column<int>(type: "integer", nullable: false),
                    weight = table.Column<short>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_song_vibe_tags", x => new { x.song_id, x.vibe_tag_id });
                    table.CheckConstraint("ck_song_vibe_tags_weight_range", "weight BETWEEN 1 AND 10");
                    table.ForeignKey(
                        name: "FK_song_vibe_tags_songs_song_id",
                        column: x => x.song_id,
                        principalTable: "songs",
                        principalColumn: "song_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_song_vibe_tags_vibe_tags_vibe_tag_id",
                        column: x => x.vibe_tag_id,
                        principalTable: "vibe_tags",
                        principalColumn: "vibe_tag_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_albums_artist_id",
                table: "albums",
                column: "artist_id");

            migrationBuilder.CreateIndex(
                name: "IX_albums_title_artist_id",
                table: "albums",
                columns: new[] { "title", "artist_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_artists_name",
                table: "artists",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_artists_slug",
                table: "artists",
                column: "slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_genres_name",
                table: "genres",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_genres_slug",
                table: "genres",
                column: "slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_mood_tags_name",
                table: "mood_tags",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_mood_tags_slug",
                table: "mood_tags",
                column: "slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_recommendation_results_recommendation_session_id",
                table: "recommendation_results",
                column: "recommendation_session_id");

            migrationBuilder.CreateIndex(
                name: "IX_recommendation_results_recommendation_session_id_rank",
                table: "recommendation_results",
                columns: new[] { "recommendation_session_id", "rank" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_recommendation_results_recommendation_session_id_song_id",
                table: "recommendation_results",
                columns: new[] { "recommendation_session_id", "song_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_recommendation_results_song_id",
                table: "recommendation_results",
                column: "song_id");

            migrationBuilder.CreateIndex(
                name: "IX_song_genres_genre_id",
                table: "song_genres",
                column: "genre_id");

            migrationBuilder.CreateIndex(
                name: "IX_song_mood_tags_mood_tag_id",
                table: "song_mood_tags",
                column: "mood_tag_id");

            migrationBuilder.CreateIndex(
                name: "IX_song_vibe_tags_vibe_tag_id",
                table: "song_vibe_tags",
                column: "vibe_tag_id");

            migrationBuilder.CreateIndex(
                name: "IX_songs_album_id",
                table: "songs",
                column: "album_id");

            migrationBuilder.CreateIndex(
                name: "IX_songs_danceability",
                table: "songs",
                column: "danceability");

            migrationBuilder.CreateIndex(
                name: "IX_songs_darkness",
                table: "songs",
                column: "darkness");

            migrationBuilder.CreateIndex(
                name: "IX_songs_energy",
                table: "songs",
                column: "energy");

            migrationBuilder.CreateIndex(
                name: "IX_songs_instrumentalness",
                table: "songs",
                column: "instrumentalness");

            migrationBuilder.CreateIndex(
                name: "IX_songs_primary_artist_id",
                table: "songs",
                column: "primary_artist_id");

            migrationBuilder.CreateIndex(
                name: "IX_songs_primary_genre_id",
                table: "songs",
                column: "primary_genre_id");

            migrationBuilder.CreateIndex(
                name: "IX_songs_title_primary_artist_id_album_id",
                table: "songs",
                columns: new[] { "title", "primary_artist_id", "album_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_songs_valence",
                table: "songs",
                column: "valence");

            migrationBuilder.CreateIndex(
                name: "IX_vibe_tags_name",
                table: "vibe_tags",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_vibe_tags_slug",
                table: "vibe_tags",
                column: "slug",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "recommendation_results");

            migrationBuilder.DropTable(
                name: "song_genres");

            migrationBuilder.DropTable(
                name: "song_mood_tags");

            migrationBuilder.DropTable(
                name: "song_vibe_tags");

            migrationBuilder.DropTable(
                name: "recommendation_sessions");

            migrationBuilder.DropTable(
                name: "mood_tags");

            migrationBuilder.DropTable(
                name: "songs");

            migrationBuilder.DropTable(
                name: "vibe_tags");

            migrationBuilder.DropTable(
                name: "albums");

            migrationBuilder.DropTable(
                name: "genres");

            migrationBuilder.DropTable(
                name: "artists");
        }
    }
}
