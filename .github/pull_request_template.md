## What and why

## Checks

- [ ] `dotnet format --verify-no-changes`, `dotnet build -c Release` and `dotnet test` pass
- [ ] `scripts/run-examples`, `scripts/check-dependencies` and `scripts/audit-dependencies` pass
- [ ] Public API changes have XML docs, a CHANGELOG entry and, where useful, an example
- [ ] No client-side statistics (the server computes every probability and total)
- [ ] No new runtime dependency (or its justification is in CONTRIBUTING.md and approved)
