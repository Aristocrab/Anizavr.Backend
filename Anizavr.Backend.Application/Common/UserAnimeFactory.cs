using Anizavr.Backend.Domain.Entities;
using ShikimoriSharp.Classes;

namespace Anizavr.Backend.Application.Common;

public static class UserAnimeFactory
{
    public static UserWatchingAnime CreateUserWatchingAnime(AnimeID anime, int currentEpisode, float secondsTotal)
    {
        if (anime.Id == DeathNoteFixHelper.DeathNoteId)
        {
            DeathNoteFixHelper.FixDeathNotePoster(anime);
        }

        return new UserWatchingAnime
        {
            AnimeId = anime.Id,
            EpisodesTotal = (int)anime.Episodes,
            CurrentEpisode = currentEpisode,
            Title = anime.Russian,
            PosterUrl = anime.Image.Original,
            Rating = anime.Score,
            SecondsWatched = 0,
            SecondsTotal = secondsTotal,
            Kind = anime.Kind
        };
    }

    public static UserWatchedAnime CreateUserWatchedAnime(AnimeID anime, int? userScore, int? currentEpisode)
    {
        if (anime.Id == DeathNoteFixHelper.DeathNoteId)
        {
            DeathNoteFixHelper.FixDeathNotePoster(anime);
        }
        
        return new UserWatchedAnime
        {
            AnimeId = anime.Id,
            EpisodesTotal = (int)anime.Episodes,
            CurrentEpisode = currentEpisode ?? (int)anime.Episodes,
            Title = anime.Russian,
            PosterUrl = anime.Image.Original,
            Rating = anime.Score,
            UserScore = userScore
        };
    }

    public static WishlistAnime CreateWishlistAnime(AnimeID anime)
    {
        if (anime.Id == DeathNoteFixHelper.DeathNoteId)
        {
            DeathNoteFixHelper.FixDeathNotePoster(anime);
        }
        
        return new WishlistAnime
        {
            AnimeId = anime.Id,
            EpisodesTotal = (int)anime.Episodes,
            Title = anime.Russian,
            PosterUrl = anime.Image.Original,
            Rating = anime.Score,
            Kind = anime.Kind
        };
    }
    
    public static TierlistAnime CreateTierlistAnime(AnimeID anime, int position)
    {
        if (anime.Id == DeathNoteFixHelper.DeathNoteId)
        {
            DeathNoteFixHelper.FixDeathNotePoster(anime);
        }
        
        return new TierlistAnime
        {
            AnimeId = anime.Id,
            EpisodesTotal = (int)anime.Episodes,
            Title = anime.Russian,
            PosterUrl = anime.Image.Original,
            Rating = anime.Score,
            Position = position
        };
    }
}