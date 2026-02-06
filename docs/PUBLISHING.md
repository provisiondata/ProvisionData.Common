# Quick Reference: Publishing Packages

## Everyday Workflow

### Make a change and publish automatically

```bash
# 1. Make your code changes
git add .
git commit -m "Add new feature to Dapper mapper"

# 2. Push to main
git push origin main

# 3. That's it! 
# GitHub Actions will:
# - Build & test
# - Create packages
# - Check if version exists on NuGet
# - Publish new versions automatically
```

**Result:** New package version published to NuGet.org within minutes.

---

## Bump Minor/Major Version

### When to bump

- **Patch** (1.0.x): Bug fixes, small improvements → **Automatic**
- **Minor** (1.x.0): New features, backward compatible → **Edit version.json**
- **Major** (x.0.0): Breaking changes → **Edit version.json**

### How to bump

**Example:** Bump ProvisionData.Dapper from 1.0.x to 1.1.0

```bash
# Edit src/ProvisionData.Dapper/version.json
# Change: "version": "1.0"
# To:     "version": "1.1"

git add src/ProvisionData.Dapper/version.json
git commit -m "Bump ProvisionData.Dapper to v1.1 - Add new column mapping features"
git push origin main
```

**Next version published:** `1.1.0`

---

## Check What Version Will Be Published

```bash
# Check current version
nbgv get-version -p src/ProvisionData.Dapper

# Output shows:
# NuGetPackageVersion: 1.0.42  ← This is what gets published
```

---

## Package-Specific Versioning

Each package versions independently:

```bash
# Bump only ProvisionData.Dapper
# Edit: src/ProvisionData.Dapper/version.json

# Bump only ProvisionData.Testing.Integration  
# Edit: src/ProvisionData.Testing.Integration/version.json

# Bump ProvisionData.Common (main package)
# Edit: version.json (root)
```

---

## Common Questions

**Q: What if I push multiple commits quickly?**  
A: Each commit gets a unique version. All new versions publish automatically.

**Q: What if the build fails?**  
A: Nothing publishes. Fix the build, push again.

**Q: Can I re-run a failed GitHub Actions workflow?**  
A: Yes! The workflow checks for existing versions and skips duplicates.

**Q: Do I need to create Git tags?**  
A: No! Tags are optional. NBGV uses commit count instead.

**Q: How do I publish a pre-release?**  
A: Push to a feature branch (not main). NBGV will add a prerelease suffix automatically.

---

## Emergency: Unpublish a Package

**Important:** You cannot delete or overwrite NuGet packages.

If you published a bad version:

```bash
# 1. Fix the issue
git add .
git commit -m "Fix critical bug"
git push origin main

# 2. New version publishes automatically
# Users can upgrade to the fixed version

# 3. (Optional) Unlist the bad version on NuGet.org
# Go to: https://www.nuget.org/packages/ProvisionData.Dapper
# Click "Manage Package" → "Unlist"
```

---

## View Published Versions

**On NuGet.org:**

- ProvisionData.Common: <https://www.nuget.org/packages/ProvisionData.Common>
- ProvisionData.Dapper: <https://www.nuget.org/packages/ProvisionData.Dapper>
- ProvisionData.Testing.Integration: <https://www.nuget.org/packages/ProvisionData.Testing.Integration>

**Via Command Line:**

```bash
dotnet package search ProvisionData.Dapper --exact-match
```

---

## Workflow Summary

```

┌─────────────────────────────────────────────────┐
│ Push to main                                    │
└────────────────┬────────────────────────────────┘
                 │
                 ▼
┌─────────────────────────────────────────────────┐
│ GitHub Actions                                  │
│ • Build & Test                                  │
│ • Pack all packages                             │
│ • Check each version on NuGet.org               │
│   ├─ Exists? Skip                               │
│   └─ New? Publish                               │
└────────────────┬────────────────────────────────┘
                 │
                 ▼
┌─────────────────────────────────────────────────┐
│ NuGet.org updated with new packages             │
└─────────────────────────────────────────────────┘
```

**Time from push to published:** ~5-10 minutes

---

## Getting Help

- NBGV docs: <https://github.com/dotnet/Nerdbank.GitVersioning>
- Issues: Create a GitHub issue in this repository
