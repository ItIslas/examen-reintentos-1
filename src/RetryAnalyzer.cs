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
        var window = attempts.Take(maxRetries).ToList();
        if (window.Count == 0) return Verdict.NotRun;
        if (window[0].Outcome == "Pass") return Verdict.Passed;
        return window.Any(a => a.Outcome == "Pass") ? Verdict.Flaky : Verdict.Failed;
    }
}
