# Implementation Plan: Swing Trading Journal App

**Branch**: `001-swing-trading-journal` | **Date**: 2025-10-03 | **Spec**: [spec.md](./spec.md)
**Input**: Feature specification from `/specs/001-swing-trading-journal/spec.md`

## Execution Flow (/plan command scope)
```
1. Load feature spec from Input path
   → If not found: ERROR "No feature spec at {path}"
2. Fill Technical Context (scan for NEEDS CLARIFICATION)
   → Detect Project Type from file system structure or context (web=frontend+backend, mobile=app+api)
   → Set Structure Decision based on project type
3. Fill the Constitution Check section based on the content of the constitution document.
4. Evaluate Constitution Check section below
   → If violations exist: Document in Complexity Tracking
   → If no justification possible: ERROR "Simplify approach first"
   → Update Progress Tracking: Initial Constitution Check
5. Execute Phase 0 → research.md
   → If NEEDS CLARIFICATION remain: ERROR "Resolve unknowns"
6. Execute Phase 1 → contracts, data-model.md, quickstart.md, agent-specific template file (e.g., `CLAUDE.md` for Claude Code, `.github/copilot-instructions.md` for GitHub Copilot, `GEMINI.md` for Gemini CLI, `QWEN.md` for Qwen Code, or `AGENTS.md` for all other agents).
7. Re-evaluate Constitution Check section
   → If new violations: Refactor design, return to Phase 1
   → Update Progress Tracking: Post-Design Constitution Check
8. Plan Phase 2 → Describe task generation approach (DO NOT create tasks.md)
9. STOP - Ready for /tasks command
```

**IMPORTANT**: The /plan command STOPS at step 7. Phases 2-4 are executed by other commands:
- Phase 2: /tasks command creates tasks.md
- Phase 3-4: Implementation execution (manual or via tools)

## Summary

A comprehensive swing trading journal application built with .NET MAUI for Windows desktop, enabling traders to record trades with full details (entry/exit, stop-loss, take-profit, position sizing), track psychology and discipline, perform post-trade reviews with quality ratings and lessons learned, analyze performance through statistics and charts (win rate, R/R ratios, equity curve), and export/backup data. The app operates offline-first with local SQLite storage, supports dark/light themes, handles 10,000+ trades efficiently, and provides desktop-optimized UI with keyboard shortcuts for rapid trade entry.

## Technical Context
**Language/Version**: C# / .NET 8.0 with .NET MAUI  
**Primary Dependencies**: .NET MAUI, Entity Framework Core, SQLite-net or EF Core SQLite provider, Syncfusion MAUI Charts (or Microcharts), Syncfusion PDF (or community PDF package)  
**Storage**: SQLite database in %LOCALAPPDATA%/TradingJournal/, screenshots in Documents/TradingJournal/Screenshots/  
**Testing**: xUnit or NUnit for unit tests, MAUI UITest or Appium for UI tests  
**Target Platform**: Windows Desktop (Windows 10/11), future extension to Android/iOS/macOS  
**Project Type**: Single desktop application (.NET MAUI project)  
**Performance Goals**: App load <3s, trade entry <2s, analytics generation <2s for 10K trades, maintain 60fps UI  
**Constraints**: Offline-first operation, <100MB memory per session, local data encryption, optional Windows Hello biometric auth  
**Scale/Scope**: Support 10,000+ trades, single-user desktop application, ~15-20 main screens/views

## Constitution Check
*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

- [x] **Code Quality Assurance**: Plan includes linting (StyleCop/Roslyn analyzers), formatting (.editorconfig), static analysis (SonarLint/Roslynator)
- [x] **Test-First Development**: Contract tests for data layer, integration tests for user workflows, unit tests for business logic (R/R calculations, statistics) planned before implementation  
- [x] **UX Consistency**: MAUI design system with consistent spacing, typography (Segoe UI Variable), colors (dark/light themes), Material Design-inspired controls
- [x] **Performance Requirements**: Performance benchmarks (3s app load, 2s trade entry, 2s analytics, 60fps UI) planned with profiling and optimization tasks
- [x] **Specification-Driven Development**: Feature specification complete with 39 functional requirements, 21 non-functional requirements, user scenarios, and acceptance criteria
- [x] **Milestone Testability**: Milestones include user-executable test procedures in quickstart.md for trade entry, psychology logging, analysis viewing, and export

*All constitutional requirements satisfied - no violations to document*

## Project Structure

### Documentation (this feature)
```
specs/[###-feature]/
├── plan.md              # This file (/plan command output)
├── research.md          # Phase 0 output (/plan command)
├── data-model.md        # Phase 1 output (/plan command)
├── quickstart.md        # Phase 1 output (/plan command)
├── contracts/           # Phase 1 output (/plan command)
└── tasks.md             # Phase 2 output (/tasks command - NOT created by /plan)
```

### Source Code (repository root)
```
TradingJournal/                          # .NET MAUI solution root
├── TradingJournal.sln                   # Visual Studio solution file
├── src/
│   ├── TradingJournal/                  # Main MAUI application project
│   │   ├── App.xaml                     # Application entry point
│   │   ├── AppShell.xaml                # Shell navigation structure
│   │   ├── MauiProgram.cs               # DI container and app configuration
│   │   ├── Models/                      # Entity classes (Trade, ManagementAdjustment, etc.)
│   │   ├── ViewModels/                  # MVVM ViewModels for each view
│   │   ├── Views/                       # XAML pages and views
│   │   │   ├── Dashboard/               # Dashboard and analytics views
│   │   │   ├── Trades/                  # Trade list, entry, edit views
│   │   │   ├── Psychology/              # Psychology logging views
│   │   │   ├── Review/                  # Trade review views
│   │   │   └── Settings/                # Settings and preferences
│   │   ├── Services/                    # Business logic services
│   │   │   ├── ITradeService.cs
│   │   │   ├── TradeService.cs
│   │   │   ├── IAnalyticsService.cs
│   │   │   ├── AnalyticsService.cs
│   │   │   ├── IExportService.cs
│   │   │   ├── ExportService.cs
│   │   │   └── IStorageService.cs
│   │   ├── Data/                        # EF Core DbContext and migrations
│   │   │   ├── TradingJournalContext.cs
│   │   │   └── Migrations/
│   │   ├── Helpers/                     # Utility classes and converters
│   │   ├── Resources/                   # Images, fonts, styles
│   │   │   ├── Styles/
│   │   │   ├── Images/
│   │   │   └── Fonts/
│   │   └── Platforms/                   # Platform-specific code
│   │       └── Windows/
│   └── TradingJournal.Core/             # Optional: Shared business logic library
│       ├── Calculations/                # R/R ratio, profit/loss calculations
│       └── Validators/                  # Input validation logic
└── tests/
    ├── TradingJournal.UnitTests/        # Unit tests for services and calculations
    ├── TradingJournal.IntegrationTests/ # Integration tests for workflows
    └── TradingJournal.UITests/          # UI automation tests
```

**Structure Decision**: Single .NET MAUI application project with MVVM architecture. The main TradingJournal project contains all UI (Views), presentation logic (ViewModels), business logic (Services), and data access (EF Core DbContext). Optional Core library for shared calculation and validation logic that might be reused. Tests are organized by type (unit, integration, UI) in separate test projects. This structure follows .NET MAUI best practices and supports desktop-first development with future mobile extension capability.

## Phase 0: Outline & Research
1. **Extract unknowns from Technical Context** above:
   - For each NEEDS CLARIFICATION → research task
   - For each dependency → best practices task
   - For each integration → patterns task

2. **Generate and dispatch research agents**:
   ```
   For each unknown in Technical Context:
     Task: "Research {unknown} for {feature context}"
   For each technology choice:
     Task: "Find best practices for {tech} in {domain}"
   ```

3. **Consolidate findings** in `research.md` using format:
   - Decision: [what was chosen]
   - Rationale: [why chosen]
   - Alternatives considered: [what else evaluated]

**Output**: research.md with all NEEDS CLARIFICATION resolved

## Phase 1: Design & Contracts
*Prerequisites: research.md complete*

1. **Extract entities from feature spec** → `data-model.md`:
   - Entity name, fields, relationships
   - Validation rules from requirements
   - State transitions if applicable

2. **Generate API contracts** from functional requirements:
   - For each user action → endpoint
   - Use standard REST/GraphQL patterns
   - Output OpenAPI/GraphQL schema to `/contracts/`

3. **Generate contract tests** from contracts:
   - One test file per endpoint
   - Assert request/response schemas
   - Tests must fail (no implementation yet)

4. **Extract test scenarios** from user stories:
   - Each story → integration test scenario
   - Quickstart test = story validation steps

5. **Update agent file incrementally** (O(1) operation):
   - Run `.specify/scripts/powershell/update-agent-context.ps1 -AgentType copilot`
     **IMPORTANT**: Execute it exactly as specified above. Do not add or remove any arguments.
   - If exists: Add only NEW tech from current plan
   - Preserve manual additions between markers
   - Update recent changes (keep last 3)
   - Keep under 150 lines for token efficiency
   - Output to repository root

**Output**: data-model.md, /contracts/*, failing tests, quickstart.md, agent-specific file

## Phase 2: Task Planning Approach
*This section describes what the /tasks command will do - DO NOT execute during /plan*

**Task Generation Strategy**:
- Load `.specify/templates/tasks-template.md` as base
- Generate tasks from Phase 1 design docs (3 service contracts, data model with 5 entities, 6 quickstart scenarios)
- Each contract method → contract test task [P] (ITradeService: 15 methods, IAnalyticsService: 12 methods, IExportService: 7 methods = ~35 test tasks)
- Each entity → model creation task [P] (Trade, ManagementAdjustment, Attachment, StrategyCategory, UserPreferences = 5 tasks)
- Each quickstart scenario → integration test task (6 scenarios = 6 integration test tasks)
- Implementation tasks: DbContext, Migrations, Service implementations, ViewModels, XAML Views
- UI tasks: Shell navigation, theme resources, charts integration
- Export tasks: CSV, Excel, PDF generation
- Polish tasks: Performance profiling, accessibility, keyboard shortcuts

**Ordering Strategy**:
- TDD order: Tests before implementation (contract tests → models → services → ViewModels → Views)
- Dependency order: Data layer → Business logic layer → Presentation layer
- Mark [P] for parallel execution (independent files, e.g., each ViewModel, each View, each test file)
- Infrastructure first: Project setup, EF Core configuration, DI registration
- Core features before advanced: CRUD before analytics, analytics before export

**Task Categories**:
1. **Setup (T001-T010)**: Solution structure, NuGet packages, linting, quality gates, performance monitoring
2. **Tests First - Data Layer (T011-T025)**: Entity tests, DbContext tests, repository pattern tests [MUST FAIL]
3. **Tests First - Service Layer (T026-T070)**: Contract tests for all 34 service methods [MUST FAIL]
4. **Tests First - Integration (T071-T080)**: User workflow tests from quickstart scenarios [MUST FAIL]
5. **Core Implementation - Data (T081-T095)**: Entities, DbContext, Migrations, seed data
6. **Core Implementation - Services (T096-T125)**: TradeService, AnalyticsService, ExportService implementations
7. **Core Implementation - ViewModels (T126-T145)**: MVVM ViewModels with commands and observables
8. **Core Implementation - Views (T146-T170)**: XAML pages for all screens (Dashboard, Trades, Psychology, Review, Settings)
9. **Integration (T171-T185)**: DI wiring, navigation, data binding, chart integration
10. **Polish & Quality Gates (T186-T200)**: Unit test gaps, performance tests, static analysis, UX review, accessibility, keyboard shortcuts, milestone validation procedures

**Estimated Output**: 200 numbered, ordered tasks in tasks.md

**Key Milestones**:
- Milestone 1: Data layer complete (entities, migrations, DbContext working)
- Milestone 2: Service layer complete (all business logic tested and working)
- Milestone 3: Basic UI complete (can create/view/edit trades)
- Milestone 4: Analytics complete (dashboard with charts)
- Milestone 5: Export/Backup complete (CSV, Excel, PDF, backup/restore)
- Milestone 6: Polish complete (performance targets met, all tests passing, quickstart validated)

**IMPORTANT**: This phase is executed by the /tasks command, NOT by /plan

## Phase 3+: Future Implementation
*These phases are beyond the scope of the /plan command*

**Phase 3**: Task execution (/tasks command creates tasks.md)  
**Phase 4**: Implementation (execute tasks.md following constitutional principles)  
**Phase 5**: Validation (run tests, execute quickstart.md, performance validation)

## Complexity Tracking
*Fill ONLY if Constitution Check has violations that must be justified*

| Violation | Why Needed | Simpler Alternative Rejected Because |
|-----------|------------|-------------------------------------|
| [e.g., 4th project] | [current need] | [why 3 projects insufficient] |
| [e.g., Repository pattern] | [specific problem] | [why direct DB access insufficient] |


## Progress Tracking
*This checklist is updated during execution flow*

**Phase Status**:
- [x] Phase 0: Research complete (/plan command)
- [x] Phase 1: Design complete (/plan command)
- [x] Phase 2: Task planning complete (/plan command - describe approach only)
- [ ] Phase 3: Tasks generated (/tasks command)
- [ ] Phase 4: Implementation complete
- [ ] Phase 5: Validation passed

**Gate Status**:
- [x] Initial Constitution Check: PASS
- [x] Post-Design Constitution Check: PASS
- [x] All NEEDS CLARIFICATION resolved (user provided implementation details)
- [x] Complexity deviations documented (none - all requirements satisfied)

**Artifacts Generated**:
- [x] `research.md` - Technology stack decisions and architectural patterns
- [x] `data-model.md` - 5 entities with relationships, validation rules, and calculations
- [x] `contracts/ITradeService.md` - 20+ method contracts with pre/post-conditions
- [x] `contracts/IAnalyticsService.md` - 12+ analytics method contracts
- [x] `contracts/IExportService.md` - 7+ export method contracts
- [x] `quickstart.md` - 6 user-testable validation scenarios
- [ ] `tasks.md` - To be generated by /tasks command
- [ ] `.github/copilot-instructions.md` - To be generated after Phase 1

**Next Command**: `/tasks` to generate task breakdown

---
*Based on Constitution v1.0.1 - See `/memory/constitution.md`*
