namespace RetryAnalysis;

public enum Verdict
{
    NotRun,
    Passed,
    Flaky,
    Failed
}

public record Attempt(int Number, string Outcome);

public static class RetryAnalyzer
{
    /// <summary>
    /// Final verdict for one test case, from its attempts in the order they ran.
    /// Outcome is "Pass", "Fail" or "Skip", compared case-insensitively.
    /// Skipped attempts are ignored. Only the first (1 + maxRetries) remaining attempts count.
    /// NotRun: no counted attempts. Passed: the first counted attempt passed.
    /// Flaky: a later counted attempt passed. Failed: no counted attempt passed.
    /// </summary>
    public static Verdict Evaluate(IReadOnlyList<Attempt> attempts, int maxRetries)
    {
        int limit = 1 + maxRetries;
        int counted = 0;

        foreach (var attempt in attempts)
        {
            if (attempt.Outcome == "Skip")
                continue;
            if (counted >= limit)
                break;

            counted++;
            if (attempt.Outcome == "Pass")
                return counted == 1 ? Verdict.Passed : Verdict.Flaky;
        }

        return counted == 0 ? Verdict.NotRun : Verdict.Failed;
    }
}
