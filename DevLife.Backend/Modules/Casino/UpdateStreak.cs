using DevLife.Backend.Domain;

namespace DevLife.Backend.Modules.Casino
{
    public static class UpdateStreak
    {
        public static void ApplyStreak(User user, bool isCorrect)
        {
            if (isCorrect)
            {
                if (user.LastCorrectGuessDate?.Date == DateTime.UtcNow.Date.AddDays(-1))
                    user.Streak += 1;
                else if (user.LastCorrectGuessDate?.Date != DateTime.UtcNow.Date)
                    user.Streak = 1;

                user.LastCorrectGuessDate = DateTime.UtcNow;
            }
            else
            {
                user.Streak = 0;
            }
        }
    }
}
