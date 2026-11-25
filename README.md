# Exam: retry verdicts for test cases

This is a live session. While you code, explain out loud what you are doing and why. We evaluate your reasoning as much as the final result.

**You may use AI or documentation.** If you do, say what you asked, what answer you got, and what you did with it. Copying code without understanding what it does is not acceptable.

## Prerequisites

- Git and the .NET SDK 8 or newer. Check with `dotnet --version`.
- Check the state of this repository with `git status` and `git log --oneline`.

## The code you will work on

Our test runner calls `RetryAnalyzer.Evaluate` after a test case has run one or more times. It passes in the attempts for that test case, in the order they ran, and the number of retries allowed. The method returns the final verdict for that test case.

```csharp
public static Verdict Evaluate(IReadOnlyList<Attempt> attempts, int maxRetries)
```

- Each `Attempt` has an `Outcome`: `"Pass"`, `"Fail"` or `"Skip"`. Compare it ignoring case, so `"pass"` and `"PASS"` are also a pass.
- `Verdict` is one of `NotRun`, `Passed`, `Flaky` or `Failed`.

## What the method must return

Apply these rules in order:

1. **Remove the skipped attempts first.** A skipped attempt is as if it never ran. It does not count and does not use up a retry.
2. **Apply the retry budget.** With `maxRetries` retries, a test can run `1 + maxRetries` times. Only that many attempts count. Any attempt after them is ignored.
3. **Pick the verdict from the attempts that count:**
   - No attempts left: `NotRun`.
   - The first attempt passed: `Passed`.
   - The first attempt failed, but a later one passed: `Flaky`.
   - No attempt passed: `Failed`.

## Examples of the expected behavior

Each row is one call to `Evaluate`. The input is the attempts in order plus `maxRetries`; the expected output is the verdict.

| Attempts (in order) | maxRetries | Expected verdict | Why |
|---|---|---|---|
| `Skip`, `Pass`, `Fail` | 0 | `Passed` | The skip is removed. The budget is 1 attempt, and that attempt is the `Pass`. |
| `Fail`, `Fail`, `Pass`, `Pass` | 1 | `Failed` | The budget is 2 attempts. Both count and both fail; the passes come after the budget. |
| `Fail`, `Skip`, `FAIL`, `pass` | 2 | `Flaky` | The skip is removed. The budget is 3 attempts. `pass` is the third counted attempt, a later pass. |
| `Skip`, `skip` | 3 | `NotRun` | Nothing counts. |

Do not change the signature of `Evaluate`, nor the `Verdict` and `Attempt` types.

## Part A: prediction (5-10 min, out loud)

Without running any code, say what the current code returns for each example and why. Then run:

```powershell
dotnet run --project src
```

The examples are in `src/Program.cs`:

- **Example 1:** outcomes `Fail`, `Pass`; `maxRetries` = 1
- **Example 2:** outcomes `pass`; `maxRetries` = 0
- **Example 3:** outcomes `Skip`, `Fail`, `Pass`; `maxRetries` = 1

Write your prediction and the actual result in an `ANSWERS.md` file **outside** this repository, one row per example: the prediction with its reason, then the actual result.

## Part B: fix and push

1. Fix `src/RetryAnalyzer.cs` so it meets the rules above. Make commits with clear messages.
2. Before pushing, check the remote: `git fetch`, then `git log --oneline origin/main`. If a commit on the remote should not be on `main`, undo its effect with `git revert`, without rewriting history.
3. Push your work to `main`: `git push origin main`.
4. Do not use `push --force` and do not delete commits on the remote.

When you finish, share the output of `git log --oneline --graph --all` and `git reflog`.
