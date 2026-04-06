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
            Notes = string.IsNullOrWhiteSpace(input.Notes) ? null : input.Notes.Trim()
        };

        foreach (var factor in input.SimpleFactors)
        {
            dailyLog.Factors.Add(new SimpleFactor
            {
                Name = factor.Name.Trim(),
                Detail = string.IsNullOrWhiteSpace(factor.Detail) ? null : factor.Detail.Trim(),
                Category = factor.Category,
                TimeSlot = factor.TimeSlot
            });
        }

        foreach (var meal in input.MealFactors)
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
                    TimeSlot = meal.TimeSlot  // gli ingredienti ereditano il time slot del pasto
                });
            }

            dailyLog.Factors.Add(mealFactor);
        }

        dbContext.DailyLogs.Add(dailyLog);
        await dbContext.SaveChangesAsync(cancellationToken);

        await hubContext.Clients.All.SendAsync("DailyLogSaved", cancellationToken);
    }

    public async Task<IReadOnlyList<DailyLog>> GetAllLogsAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.DailyLogs
            .AsNoTracking()
            .Include(log => log.Factors)
                .ThenInclude(factor => factor.Children)
            .OrderByDescending(log => log.Day)
            .ToListAsync(cancellationToken);
    }
}
