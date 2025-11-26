# Contributing to Simple Observability

Thank you for your interest in contributing to Simple Observability! We welcome contributions from the community.

## Important: Contributor Agreement

**Before contributing, please understand:**

### No Payment or Royalties
Contributors to this project **do not receive**:
- ❌ Upfront payment from the project owner
- ❌ Royalties from commercial licensing
- ❌ Licensing fees or revenue sharing
- ❌ Any form of monetary compensation

### Rights Assignment
By contributing to this project, you agree to:
- ✅ Assign all rights to your contributions to Justin Adler (project owner)
- ✅ Allow your contributions to be used under the project's dual-license model
- ✅ Waive any claims to future compensation related to your contributions

### Why This Model?
This project uses a dual-license model to:
- Support free use by open source, educational, non-profit, and small business users
- Generate sustainable funding through commercial licensing
- Maintain a single copyright holder for licensing clarity

**If you're uncomfortable with these terms, please do not contribute code.** You can still support the project by:
- 📝 Reporting bugs
- 💡 Suggesting features
- 📖 Improving documentation (contributors still assign rights)
- 🌟 Starring the repository
- 📣 Spreading the word

---

## How to Contribute

### Reporting Bugs

1. Check if the bug has already been reported in [GitHub Issues](https://github.com/PureKrome/SimpleObservability/issues)
2. Create a new issue with:
   - Clear title and description
   - Steps to reproduce
   - Expected vs actual behavior
   - Environment details (.NET version, OS, etc.)
   - Screenshots if applicable

### Suggesting Features

1. Check existing issues and discussions
2. Create a new issue with the `enhancement` label
3. Describe:
   - The problem you're trying to solve
   - Your proposed solution
   - Alternative solutions you've considered
   - How it benefits other users

### Code Contributions

#### Before You Start

1. **Read the Contributor Agreement above** and ensure you agree
2. Check existing issues or create one to discuss your proposed change
3. Fork the repository
4. Create a feature branch from `main`

#### Coding Standards

Follow the guidelines in [`.github/copilot-instructions.md`](.github/copilot-instructions.md):

- Use modern C# features (pattern matching, records, nullable reference types, file-scoped namespaces)
- Follow Microsoft's C# coding conventions
- Use `async`/`await` for I/O operations
- Write meaningful, descriptive names
- Add XML documentation comments for public APIs
- Include unit tests for new functionality
- Use xUnit for tests with AAA pattern (Arrange, Act, Assert)
- All comments in English, ending with a period

#### Development Setup

```bash
# Clone your fork
git clone https://github.com/YOUR-USERNAME/SimpleObservability.git
cd SimpleObservability

# Create a feature branch
git checkout -b feature/your-feature-name

# Restore dependencies
dotnet restore

# Build the solution
dotnet build

# Run tests
dotnet test
```

#### Making Changes

1. **Write tests first** (Test-Driven Development preferred)
2. **Implement your changes**
3. **Ensure all tests pass**
4. **Update documentation** if needed (README, XML comments, etc.)
5. **Follow the coding standards** in the instructions file

#### Commit Messages

- Use clear, descriptive commit messages
- Start with a verb (Add, Fix, Update, Remove, etc.)
- Reference issue numbers when applicable

Examples:
```
Add support for custom health check intervals
Fix timeout bug in HealthCheckService
Update README with Docker deployment instructions
```

#### Pull Request Process

1. **Push your branch** to your fork
2. **Create a Pull Request** against the `main` branch
3. **Fill out the PR template** with:
   - Description of changes
   - Related issue numbers
   - Testing performed
   - Screenshots (if UI changes)
4. **Address review feedback** promptly
5. **Ensure CI/CD checks pass**

#### Pull Request Review

- Maintainers will review your PR as time permits
- Be open to feedback and discussion
- Changes may be requested for code quality, design, or alignment with project goals
- Not all PRs will be merged - maintainers have final say on what enters the codebase

---

## Code of Conduct

### Be Respectful
- Treat all contributors with respect
- Welcome newcomers and help them learn
- Provide constructive feedback
- Assume good intentions

### Be Professional
- Keep discussions focused and on-topic
- Avoid personal attacks or inflammatory language
- Respect differing viewpoints and experiences

### Unacceptable Behavior
- Harassment, discrimination, or offensive comments
- Trolling, insulting/derogatory comments
- Publishing others' private information
- Other conduct inappropriate in a professional setting

**Violations** may result in removal from the project community.

---

## Testing

### Running Tests

```bash
# Run all tests
dotnet test

# Run tests for a specific project
dotnet test code/Core/WorldDomination.SimpleObservability.Tests

# Run with code coverage
dotnet test /p:CollectCoverage=true
```

### Writing Tests

- Use xUnit v3 with Microsoft Test Platform
- One test class per method being tested (for parallel execution)
- At least one success test and one failure test per method
- Comment sections: `// Arrange.`, `// Act.`, `// Assert.`
- Use meaningful test names: `MethodName_Scenario_ExpectedResult`

Example:
```csharp
public class AddMessageAsyncTests
{
    [Fact]
    public async Task AddMessageAsync_WithValidContent_ReturnsSuccess()
    {
        // Arrange.
        var service = new MyService();
        var message = "Test message";

        // Act.
        var result = await service.AddMessageAsync(message);

        // Assert.
        Assert.True(result.IsSuccess);
    }
}
```

---

## Documentation

When adding features or making changes:
- ✅ Update XML documentation comments
- ✅ Update README.md if user-facing
- ✅ Update docs/ files if applicable
- ✅ Include code examples for new APIs

---

## Questions?

- **General questions:** [GitHub Discussions](https://github.com/PureKrome/SimpleObservability/discussions)
- **Bugs/Features:** [GitHub Issues](https://github.com/PureKrome/SimpleObservability/issues)
- **Licensing questions:** licensing@world-domination.com.au

---

## Acknowledgments

Thank you for considering contributing to Simple Observability! Your contributions help make service health monitoring more accessible to everyone.

**Remember:** By contributing, you agree to the Contributor Agreement outlined above. If you have questions about this, please ask before contributing.
