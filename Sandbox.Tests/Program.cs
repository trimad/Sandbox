using System.Text.RegularExpressions;

string repoRoot = LocateRepoRoot();
string programPath = Path.Combine(repoRoot, "Program.cs");
string fractalPath = Path.Combine(repoRoot, "Fractals", "Fractal.cs");
string dailyPipelinePath = Path.Combine(repoRoot, "scripts", "daily_fractal.py");
string auditPath = Path.Combine(repoRoot, "scripts", "fractal_hunt_audit.py");
string source = File.ReadAllText(programPath);
string fractalSource = File.ReadAllText(fractalPath);
string dailyPipelineSource = File.ReadAllText(dailyPipelinePath);
string auditSource = File.ReadAllText(auditPath);

AssertCaseUsesConfiguredBounds(source, "mandelbrot");
AssertNoHardCodedExternalFractalDirectory(fractalSource);
AssertNoHardCodedExternalFractalDirectory(dailyPipelineSource);
AssertRendererUsesSandboxOutputDirectory(fractalSource);
AssertDailyPipelineSkipsCatalogedFractals(dailyPipelineSource);
AssertAuditReportsSelfImprovementOpportunities(auditSource);

Console.WriteLine("All Sandbox behavior checks passed.");

static string LocateRepoRoot()
{
    DirectoryInfo? directory = new(AppContext.BaseDirectory);
    while (directory is not null)
    {
        string candidate = Path.Combine(directory.FullName, "Sandbox.csproj");
        if (File.Exists(candidate))
        {
            return directory.FullName;
        }

        directory = directory.Parent;
    }

    throw new InvalidOperationException("Could not locate Sandbox repo root containing Sandbox.csproj.");
}

static void AssertCaseUsesConfiguredBounds(string source, string keyword)
{
    Match match = Regex.Match(
        source,
        $@"case \""{Regex.Escape(keyword)}\"":.*?break;",
        RegexOptions.Singleline);

    if (!match.Success)
    {
        throw new InvalidOperationException($"Could not find Program.cs case for '{keyword}'.");
    }

    string caseBody = match.Value;
    string expected = ".Init(settings.MinReal, settings.MaxReal, settings.MinImaginary, settings.MaxImaginary)";
    if (!caseBody.Contains(expected, StringComparison.Ordinal))
    {
        throw new InvalidOperationException(
            $"The '{keyword}' command must initialize the fractal with configured real/imaginary bounds. " +
            $"Expected to find: {expected}");
    }
}

static void AssertNoHardCodedExternalFractalDirectory(string source)
{
    if (source.Contains(@"C:\Fractals", StringComparison.OrdinalIgnoreCase))
    {
        throw new InvalidOperationException("Fractal file activity must stay inside the Sandbox project; do not hard-code C:\\Fractals.");
    }
}

static void AssertRendererUsesSandboxOutputDirectory(string source)
{
    string expected = "Path.Combine(ResolveSandboxRoot(), \"output\", \"fractals\")";
    if (!source.Contains(expected, StringComparison.Ordinal))
    {
        throw new InvalidOperationException($"Fractal saves must resolve under the Sandbox output directory. Expected to find: {expected}");
    }
}

static void AssertDailyPipelineSkipsCatalogedFractals(string source)
{
    string[] requiredSnippets =
    {
        "def cataloged_keywords",
        "def select_uncataloged_entry",
        "Skipped already cataloged queue entries",
        "No uncataloged fractals remain",
        "--dry-run"
    };

    foreach (string snippet in requiredSnippets)
    {
        if (!source.Contains(snippet, StringComparison.Ordinal))
        {
            throw new InvalidOperationException($"daily_fractal.py is missing duplicate-guard snippet: {snippet}");
        }
    }
}

static void AssertAuditReportsSelfImprovementOpportunities(string source)
{
    string[] requiredSnippets =
    {
        "Fractal Hunt Audit",
        "Already cataloged queue entries to skip",
        "Uncataloged queue entries ready for daily runs",
        "Implemented but neither queued nor cataloged opportunities",
        "Self-improvement opportunity"
    };

    foreach (string snippet in requiredSnippets)
    {
        if (!source.Contains(snippet, StringComparison.Ordinal))
        {
            throw new InvalidOperationException($"fractal_hunt_audit.py is missing audit snippet: {snippet}");
        }
    }
}
