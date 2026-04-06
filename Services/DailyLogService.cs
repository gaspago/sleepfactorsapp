using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.SignalR;
using SleepFactorsApp.Data;
using SleepFactorsApp.Domain;
using SleepFactorsApp.Hubs;

namespace SleepFactorsApp.Services;

public sealed class DailyLogService(SleepFactorsDbContext dbContext, IHubContext<SleepFactorsHub> hubContext)
{
    public async Task AddDailyLogAsync(DailyLogInput input, CancellationToken cancellationToken = default)
    {
        var dailyLog = new DailyLog
        {
            Day = input.Day,
            SleepQuality = input.SleepQuality,
            IsCommitted = true,
            Notes = string.IsNullOrWhiteSpace(input.Notes) ? null : input.Notes.Trim()
        };

        AddFactors(dailyLog, input.SimpleFactors, input.MealFactors);

        dbContext.DailyLogs.Add(dailyLog);
        await dbContext.SaveChangesAsync(cancellationToken);
        await hubContext.Clients.All.SendAsync("DailyLogSaved", cancellationToken);
    }

    public async Task SaveDraftAsync(DailyDraftInput input, CancellationToken cancellationToken = default)
    {
        var draft = await dbContext.DailyLogs
            .Include(log => log.Factors)
                .ThenInclude(factor => factor.Children)
            .FirstOrDefaultAsync(log => log.Day == input.Day && !log.IsCommitted, cancellationToken);

        if (draft is null)
        {
            draft = new DailyLog
            {
                Day = input.Day,
                SleepQuality = SleepQuality.SoSo,
                IsCommitted = false,
                Notes = string.IsNullOrWhiteSpace(input.Notes) ? null : input.Notes.Trim()
            };
            dbContext.DailyLogs.Add(draft);
        }
        else
        {
            draft.Factors.Clear();
            draft.Notes = string.IsNullOrWhiteSpace(input.Notes) ? null : input.Notes.Trim();
        }

        AddFactors(draft, input.SimpleFactors, input.MealFactors);

        await dbContext.SaveChangesAsync(cancellationToken);
        await hubContext.Clients.All.SendAsync("DailyLogSaved", cancellationToken);
    }

    public async Task CommitSleepAsync(CommitSleepInput input, CancellationToken cancellationToken = default)
    {
        var draft = await dbContext.DailyLogs
            .FirstOrDefaultAsync(log => log.Day == input.Day && !log.IsCommitted, cancellationToken);

        if (draft is null)
        {
            throw new InvalidOperationException("Nessun pre-salvataggio trovato per il giorno selezionato.");
        }

        draft.SleepQuality = input.SleepQuality;
        draft.IsCommitted = true;

        if (!string.IsNullOrWhiteSpace(input.Notes))
        {
            draft.Notes = input.Notes.Trim();
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        await hubContext.Clients.All.SendAsync("DailyLogSaved", cancellationToken);
    }

    public async Task<IReadOnlyList<DateOnly>> GetPendingDraftDaysAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.DailyLogs
            .AsNoTracking()
            .Where(log => !log.IsCommitted)
            .OrderBy(log => log.Day)
            .Select(log => log.Day)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<DailyLog>> GetAllLogsAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.DailyLogs
            .AsNoTracking()
            .Where(log => log.IsCommitted)
            .Include(log => log.Factors)
                .ThenInclude(factor => factor.Children)
            .OrderByDescending(log => log.Day)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<string>> GetHistoricalMealTypesAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.DailyLogs
            .AsNoTracking()
            .SelectMany(log => log.Factors)
            .OfType<MealFactor>()
            .Select(f => f.MealType.ToLower())
            .Where(x => x != "")
            .Distinct()
            .OrderBy(x => x)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<string>> GetHistoricalIngredientsAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.DailyLogs
            .AsNoTracking()
            .SelectMany(log => log.Factors)
            .OfType<MealFactor>()
            .SelectMany(meal => meal.Children)
            .Where(f => f.Category == FactorCategory.Ingredient)
            .Select(f => f.Name.ToLower())
            .Where(x => x != "")
            .Distinct()
            .OrderBy(x => x)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<string>> GetHistoricalSimpleFactorNamesAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.DailyLogs
            .AsNoTracking()
            .SelectMany(log => log.Factors)
            .OfType<SimpleFactor>()
            .Select(f => f.Name.ToLower())
            .Where(x => x != "")
            .Distinct()
            .OrderBy(x => x)
            .ToListAsync(cancellationToken);
    }

    private static void AddFactors(DailyLog dailyLog, IReadOnlyList<SimpleFactorInput> simpleFactors, IReadOnlyList<MealFactorInput> mealFactors)
    {
        foreach (var factor in simpleFactors)
        {
            dailyLog.Factors.Add(new SimpleFactor
            {
                Name = factor.Name.Trim(),
                Detail = string.IsNullOrWhiteSpace(factor.Detail) ? null : factor.Detail.Trim(),
                Category = factor.Category,
                TimeSlot = factor.TimeSlot
            });
        }

        foreach (var meal in mealFactors)
        {
            var mealFactor = new MealFactor
            {
                Name = meal.MealType.Trim().ToLowerInvariant(),
                MealType = meal.MealType.Trim().ToLowerInvariant(),
                Detail = string.IsNullOrWhiteSpace(meal.Detail) ? null : meal.Detail.Trim(),
                Category = FactorCategory.Meal,
                TimeSlot = meal.TimeSlot
            };

            foreach (var ingredient in meal.Ingredients.Where(value => !string.IsNullOrWhiteSpace(value)))
            {
                mealFactor.Children.Add(new SimpleFactor
                {
                    Name = ingredient.Trim().ToLowerInvariant(),
                    Category = FactorCategory.Ingredient,
                    TimeSlot = meal.TimeSlot
                });
            }

            dailyLog.Factors.Add(mealFactor);
        }
    }
}
