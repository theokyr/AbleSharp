# AbleSharp Cursor Rules

This directory contains Cursor Rules (AI coding assistant guidelines) for the AbleSharp project, organized by component.

## File Structure

```
.cursor/rules/
├── README.md       # This file
├── global.mdc      # Global project standards (always applied)
├── lib.mdc         # AbleSharp.Lib core library rules
├── sdk.mdc         # AbleSharp.SDK high-level API rules
├── cli.mdc         # AbleSharp.CLI command-line interface rules
└── gui.mdc         # AbleSharp.GUI desktop application rules
```

## Rules Overview

### global.mdc
**Scope**: All C# and project files  
**Always Applied**: Yes

Covers:
- General C# coding standards (.NET 9.0)
- Naming conventions
- Error handling patterns
- XML documentation requirements
- Testing approaches
- Project-specific conventions for Ableton Live format

### lib.mdc
**Scope**: `AbleSharp.Lib/**/*.cs`  
**Always Applied**: No (only when editing Lib files)

Covers:
- Core library architecture
- Data class design patterns
- XML schema serialization
- Schema generation
- Automation system
- Track and device systems
- Performance considerations

### sdk.mdc
**Scope**: `AbleSharp.SDK/**/*.cs`  
**Always Applied**: No (only when editing SDK files)

Covers:
- Facade pattern implementation
- Singleton pattern usage
- Options pattern for API methods
- Core handler classes
- API design principles
- Error handling strategies
- Documentation standards

### cli.mdc
**Scope**: `AbleSharp.CLI/**/*.cs`  
**Always Applied**: No (only when editing CLI files)

Covers:
- CLI design principles
- Command structure and routing
- Argument parsing
- Output formatting
- Error handling and exit codes
- SDK integration patterns

### gui.mdc
**Scope**: `AbleSharp.GUI/**/*.{cs,axaml}`  
**Always Applied**: No (only when editing GUI files)

Covers:
- Avalonia UI framework usage
- MVVM pattern implementation
- ReactiveUI integration
- Dependency injection
- Value converters
- Commands and data binding
- Cross-platform considerations

## Usage

Cursor will automatically apply these rules based on:
1. **File globs**: Rules apply to files matching their glob patterns
2. **Always Apply**: Global rules are always active
3. **Context**: Component-specific rules activate when editing those files

## Maintenance

When updating these rules:
1. Keep consistency across all rule files
2. Update this README when adding new rules
3. Test rules by editing files in each component
4. Ensure examples are current with codebase

## Version

Rules last updated: November 1, 2025  
AbleSharp Version: 0.0.6  
Framework: .NET 9.0

## References

- [Cursor Rules Documentation](https://docs.cursor.com/rules)
- [AbleSharp README](../../README.md)
- [Avalonia Documentation](https://docs.avaloniaui.net/)
- [ReactiveUI Documentation](https://www.reactiveui.net/)

