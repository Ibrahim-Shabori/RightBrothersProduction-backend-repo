using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RightBrothersProduction.Models
{
    public class ChartPointRange
    {
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public string Label { get; set; }
    }
    public static class PeriodModels
    {
        public enum QueryPeriod
        {
            All,
            LastDay,
            LastWeek,
            LastMonth,
            Last6Months,
            LastYear,
        }

        public static (DateTime From, DateTime To) GetQueryPeriodBounds(QueryPeriod period)
        {
            // Use DateTime.UtcNow if your DB stores UTC, otherwise DateTime.Now
            var now = DateTime.Now;

            // Normalize to start of today (00:00:00)
            var todayStart = now.Date;

            // End of today (23:59:59.9999999) - Useful for inclusive SQL 'BETWEEN'
            var endOfToday = todayStart.AddDays(1).AddTicks(-1);

            return period switch
            {
                QueryPeriod.All => (DateTime.MinValue, DateTime.MaxValue),

                QueryPeriod.LastDay => (todayStart, endOfToday),

                // Rolling periods (Start date -> End of Today)
                QueryPeriod.LastWeek => (todayStart.AddDays(-7), endOfToday),
                QueryPeriod.LastMonth => (todayStart.AddMonths(-1), endOfToday),
                QueryPeriod.Last6Months => (todayStart.AddMonths(-6), endOfToday),
                QueryPeriod.LastYear => (todayStart.AddYears(-1), endOfToday),

                // Default fallback
                _ => (DateTime.MinValue, DateTime.MaxValue)
            };
        }

        public static (DateTime From, DateTime To) GetPreviousBounds(QueryPeriod period, DateTime currentFrom)
        {
            // Calculate the timespan of the current period to shift back by the exact same amount
            // But for things like "Months", it's better to shift by logical months, not just days.
            if (period == QueryPeriod.All) return (DateTime.MinValue, DateTime.MaxValue);
            return period switch
            {
                QueryPeriod.LastDay => (currentFrom.AddDays(-1), currentFrom.AddTicks(-1)),
                QueryPeriod.LastWeek => (currentFrom.AddDays(-7), currentFrom.AddTicks(-1)),
                QueryPeriod.LastMonth => (currentFrom.AddMonths(-1), currentFrom.AddTicks(-1)),
                QueryPeriod.Last6Months => (currentFrom.AddMonths(-6), currentFrom.AddTicks(-1)),
                QueryPeriod.LastYear => (currentFrom.AddYears(-1), currentFrom.AddTicks(-1)),
                _ => (DateTime.MinValue, DateTime.MinValue) // 'All' has no previous
            };
        }

        public static List<ChartPointRange> GetChartRanges(QueryPeriod period, DateTime userNow)
        {
            var ranges = new List<ChartPointRange>();
            var todayStart = userNow.Date;

            switch (period)
            {
                case QueryPeriod.LastDay:
                    // Split into 24 Hours (00:00 to 23:59)
                    for (int i = 0; i < 24; i++)
                    {
                        var start = todayStart.AddHours(i);
                        // Only add hours that have already happened? 
                        // Usually for "Today" charts, we show all 24 hours or up to current hour.
                        // Let's show all 24 hours for the grid.
                        ranges.Add(new ChartPointRange
                        {
                            From = start,
                            To = start.AddHours(1).AddTicks(-1),
                            Label = start.ToString("h tt") // "1 AM", "2 PM"
                        });
                    }
                    break;

                case QueryPeriod.LastWeek:
                    // Last 7 Days (Rolling)
                    for (int i = 6; i >= 0; i--)
                    {
                        var date = todayStart.AddDays(-i);
                        ranges.Add(new ChartPointRange
                        {
                            From = date,
                            To = date.AddDays(1).AddTicks(-1),
                            Label = date.ToString("ddd") // "Mon", "Tue"
                        });
                    }
                    break;

                case QueryPeriod.LastMonth:
                    // Last 30 Days
                    for (int i = 29; i >= 0; i--)
                    {
                        var date = todayStart.AddDays(-i);
                        ranges.Add(new ChartPointRange
                        {
                            From = date,
                            To = date.AddDays(1).AddTicks(-1),
                            Label = date.ToString("dd MMM") // "29 Dec"
                        });
                    }
                    break;

                case QueryPeriod.Last6Months:
                    // Last 6 Months
                    // We start from 5 months ago to make 6 points total (0 to 5)
                    var currentMonthStart = new DateTime(todayStart.Year, todayStart.Month, 1);
                    for (int i = 5; i >= 0; i--)
                    {
                        var monthDate = currentMonthStart.AddMonths(-i);
                        var daysInMonth = DateTime.DaysInMonth(monthDate.Year, monthDate.Month);

                        ranges.Add(new ChartPointRange
                        {
                            From = monthDate,
                            To = monthDate.AddDays(daysInMonth).AddTicks(-1), // End of that month
                            Label = monthDate.ToString("MMM") // "Jan", "Feb"
                        });
                    }
                    break;

                case QueryPeriod.LastYear:
                    // Last 12 Months
                    var thisMonth = new DateTime(todayStart.Year, todayStart.Month, 1);
                    for (int i = 11; i >= 0; i--)
                    {
                        var monthDate = thisMonth.AddMonths(-i);
                        var daysInMonth = DateTime.DaysInMonth(monthDate.Year, monthDate.Month);

                        ranges.Add(new ChartPointRange
                        {
                            From = monthDate,
                            To = monthDate.AddDays(daysInMonth).AddTicks(-1),
                            Label = monthDate.ToString("MMM yyyy") // "Dec 2024"
                        });
                    }
                    break;
            }

            return ranges;
        }


    }
}
