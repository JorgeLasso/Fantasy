﻿using Fantasy.Backend.Data;
using Fantasy.Backend.Helpers;
using Fantasy.Backend.Repositories.Interfaces;
using Fantasy.Shared.DTOs;
using Fantasy.Shared.Entities;
using Fantasy.Shared.Responses;
using Microsoft.EntityFrameworkCore;

namespace Fantasy.Backend.Repositories.Implementations;

public class TournamentsRepository : GenericRepository<Tournament>, ITournamentsRepository
{
    private readonly DataContext _context;
    private readonly IFileStorage _fileStorage;

    public TournamentsRepository(DataContext context, IFileStorage fileStorage) : base(context)
    {
        _context = context;
        _fileStorage = fileStorage;
    }

    public override async Task<ActionResponse<IEnumerable<Tournament>>> GetAsync(PaginationDTO pagination)
    {
        var queryable = _context.Tournaments
            //.Include(x => x.Matches)
            .Include(x => x.TournamentTeams)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(pagination.Filter))
        {
            queryable = queryable.Where(x => x.Name.ToLower().Contains(pagination.Filter.ToLower()));
        }

        return new ActionResponse<IEnumerable<Tournament>>
        {
            WasSuccess = true,
            Result = await queryable
                .OrderBy(x => x.Name)
                .Paginate(pagination)
                .ToListAsync()
        };
    }

    public override async Task<ActionResponse<Tournament>> GetAsync(int id)
    {
        var tournament = await _context.Tournaments
             .Include(x => x.TournamentTeams!)
             .ThenInclude(x => x.Team)
             .FirstOrDefaultAsync(c => c.Id == id);

        if (tournament == null)
        {
            return new ActionResponse<Tournament>
            {
                WasSuccess = false,
                Message = "ERR001"
            };
        }

        return new ActionResponse<Tournament>
        {
            WasSuccess = true,
            Result = tournament
        };
    }

    public async Task<ActionResponse<Tournament>> AddAsync(TournamentDTO tournamentDTO)
    {
        var tournament = new Tournament
        {
            IsActive = false,
            Name = tournamentDTO.Name,
            Remarks = tournamentDTO.Remarks,
            TournamentTeams = new List<TournamentTeam>()
        };

        if (!string.IsNullOrEmpty(tournamentDTO.Image))
        {
            var imageBase64 = Convert.FromBase64String(tournamentDTO.Image!);
            tournament.Image = await _fileStorage.SaveFileAsync(imageBase64, ".jpg", "tournaments");
        }

        _context.Add(tournament);
        try
        {
            await _context.SaveChangesAsync();
            return new ActionResponse<Tournament>
            {
                WasSuccess = true,
                Result = tournament
            };
        }
        catch (DbUpdateException)
        {
            return new ActionResponse<Tournament>
            {
                WasSuccess = false,
                Message = "ERR003"
            };
        }
        catch (Exception exception)
        {
            return new ActionResponse<Tournament>
            {
                WasSuccess = false,
                Message = exception.Message
            };
        }
    }

    public async Task<IEnumerable<Tournament>> GetComboAsync()
    {
        return await _context.Tournaments
            .Where(x => x.IsActive)
            .OrderBy(x => x.Name)
            .ToListAsync();
    }

    public async Task<ActionResponse<int>> GetTotalRecordsAsync(PaginationDTO pagination)
    {
        var queryable = _context.Tournaments.AsQueryable();

        if (!string.IsNullOrWhiteSpace(pagination.Filter))
        {
            queryable = queryable.Where(x => x.Name.ToLower().Contains(pagination.Filter.ToLower()));
        }

        double count = await queryable.CountAsync();
        return new ActionResponse<int>
        {
            WasSuccess = true,
            Result = (int)count
        };
    }

    public async Task<ActionResponse<Tournament>> UpdateAsync(TournamentDTO tournamentDTO)
    {
        var currentTournament = await _context.Tournaments.FindAsync(tournamentDTO.Id);
        if (currentTournament == null)
        {
            return new ActionResponse<Tournament>
            {
                WasSuccess = false,
                Message = "ERR009"
            };
        }

        if (!string.IsNullOrEmpty(tournamentDTO.Image))
        {
            var imageBase64 = Convert.FromBase64String(tournamentDTO.Image!);
            currentTournament.Image = await _fileStorage.SaveFileAsync(imageBase64, ".jpg", "tournaments");
        }

        currentTournament.Name = tournamentDTO.Name;
        currentTournament.IsActive = tournamentDTO.IsActive;
        currentTournament.Remarks = tournamentDTO.Remarks;

        _context.Update(currentTournament);
        try
        {
            await _context.SaveChangesAsync();
            return new ActionResponse<Tournament>
            {
                WasSuccess = true,
                Result = currentTournament
            };
        }
        catch (DbUpdateException)
        {
            return new ActionResponse<Tournament>
            {
                WasSuccess = false,
                Message = "ERR003"
            };
        }
        catch (Exception exception)
        {
            return new ActionResponse<Tournament>
            {
                WasSuccess = false,
                Message = exception.Message
            };
        }
    }
}