﻿using Fantasy.Shared.DTOs;
using Fantasy.Shared.Entities;
using Fantasy.Shared.Responses;

namespace Fantasy.Backend.Repositories.Interfaces;

public interface ITournamentsRepository
{
    Task<IEnumerable<Tournament>> GetComboAsync();

    Task<ActionResponse<Tournament>> AddAsync(TournamentDTO tournamentDTO);

    Task<ActionResponse<Tournament>> UpdateAsync(TournamentDTO tournamentDTO);

    Task<ActionResponse<Tournament>> GetAsync(int id);

    Task<ActionResponse<IEnumerable<Tournament>>> GetAsync(PaginationDTO pagination);

    Task<ActionResponse<int>> GetTotalRecordsAsync(PaginationDTO pagination);
}