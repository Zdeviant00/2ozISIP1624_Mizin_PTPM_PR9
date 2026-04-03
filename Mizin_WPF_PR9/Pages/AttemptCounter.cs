using System;
using System.Collections.Generic;

namespace Mizin_WPF_PR9.Pages
{
    public static class AttemptCounter
    {
        private static Dictionary<string, int> _failedAttempts = new Dictionary<string, int>();
        private static Dictionary<string, DateTime> _lockTimes = new Dictionary<string, DateTime>();

        private const int MaxAttempts = 3;
        private const int LockDurationMinutes = 5;

        public static int GetFailedAttempts(string login)
        {
            if (_failedAttempts.ContainsKey(login))
                return _failedAttempts[login];
            return 0;
        }

        public static void IncrementFailedAttempts(string login)
        {
            if (_failedAttempts.ContainsKey(login))
                _failedAttempts[login]++;
            else
                _failedAttempts[login] = 1;
        }

        public static void ResetAttempts(string login)
        {
            if (_failedAttempts.ContainsKey(login))
                _failedAttempts[login] = 0;
        }

        public static bool IsLockedOut(string login)
        {
            if (_lockTimes.ContainsKey(login))
            {
                if (DateTime.Now < _lockTimes[login])
                    return true;
                else
                {
                    _lockTimes.Remove(login);
                    ResetAttempts(login);
                    return false;
                }
            }
            return false;
        }

        public static bool ShouldShowCaptcha(string login)
        {
            return GetFailedAttempts(login) >= MaxAttempts;
        }

        public static void LockAccount(string login)
        {
            _lockTimes[login] = DateTime.Now.AddMinutes(LockDurationMinutes);
        }
    }
}