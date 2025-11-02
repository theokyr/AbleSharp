using System;
using System.Diagnostics;
using AbleSharp.Lib.Schema;
using NUnit.Framework;

namespace AbleSharp.CLI.Tests;

[TestFixture]
public class CliSchemasCommandTests
{
    [Test]
    public void SchemasListOutputsCanonicalVersion()
    {
        var canonicalId = SchemaCatalog.Canonical.VersionId;

        var output = RunCli("schemas", "list", "--version", canonicalId, "--path");

        Assert.Multiple(() =>
        {
            Assert.That(output, Does.Contain("Found 1 schema"));
            Assert.That(output, Does.Contain(canonicalId));
        });
    }

    [Test]
    public void SchemasListWarnsWhenAdditionalRootMissing()
    {
        var canonicalId = SchemaCatalog.Canonical.VersionId;
        var missingRoot = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));

        var output = RunCli("schemas", "list", "--version", canonicalId, "--schema-root", missingRoot);

        Assert.That(output, Does.Contain("Warning: unable to register schema root"));
    }

    private static string RunCli(params string[] args)
    {
        var cliAssembly = ResolveCliAssembly();
        if (!File.Exists(cliAssembly))
        {
            Assert.Fail($"AbleSharp.CLI assembly was not found at '{cliAssembly}'. Build may have failed.");
        }

        var argumentText = string.Join(' ', args.Select(EscapeArgument));
        var startInfo = new ProcessStartInfo("dotnet", $"\"{cliAssembly}\" {argumentText}")
        {
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        var startedProcess = Process.Start(startInfo);
        if (startedProcess is null)
        {
            throw new InvalidOperationException("Unable to start AbleSharp CLI process.");
        }

        using var process = startedProcess;

        var stdout = process.StandardOutput.ReadToEnd();
        var stderr = process.StandardError.ReadToEnd();

        if (!process.WaitForExit(10000))
        {
            try
            {
                process.Kill(entireProcessTree: true);
            }
            catch
            {
                // Ignored - best effort cleanup.
            }

            Assert.Fail("AbleSharp CLI process timed out.");
        }

        Assert.That(process.ExitCode, Is.EqualTo(0), $"CLI exited with code {process.ExitCode}.{Environment.NewLine}{stderr}");
        return string.Concat(stdout, stderr);
    }

    private static string ResolveCliAssembly()
    {
        var testDirectory = new DirectoryInfo(TestContext.CurrentContext.TestDirectory);
        var targetFramework = testDirectory.Name;
        var configuration = testDirectory.Parent?.Name ?? "Debug";
        var solutionRoot = Path.GetFullPath(Path.Combine(testDirectory.FullName, "..", "..", "..", ".."));

        return Path.Combine(solutionRoot, "AbleSharp.CLI", "bin", configuration, targetFramework, "AbleSharp.CLI.dll");
    }

    private static string EscapeArgument(string arg)
    {
        if (string.IsNullOrEmpty(arg))
        {
            return "\"\"";
        }

        return arg.Contains(' ') ? $"\"{arg}\"" : arg;
    }
}
