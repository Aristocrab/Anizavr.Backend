using Anizavr.Backend.Application.KodikApi.Entities;
using ShikimoriSharp.Classes;

namespace Anizavr.Backend.Application.Dtos;

public class AnimeDto
{
    public required AnimeID ShikimoriDetails { get; init; }
    public required KodikResults KodikDetails { get; init; }
}