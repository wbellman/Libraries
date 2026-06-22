# AGENTS.md

Authoritative working standard for this repository. This file governs all
agent-assisted work here — coding, refactoring, review, and documentation.
Other agent config files (`.github/copilot-instructions.md`, `CLAUDE.md`)
point back to this file; keep the substance here and edit it in one place.

This is a **public portfolio repository** built around a C# solution
(`Libraries.sln`). It contains reusable libraries, a supporting tool, and
its own CI/build pipeline. Code is **C#**. `Library.Operations` is the
**centerpiece** and original work; the documentation should reflect that
(see "Centerpiece & provenance" below).

Project layout note: top-level solution-member directories are projects
(e.g. `Library.Authentication`, `Library.Operations`). `Tools/` is a
**category path, not a project** — document the tool inside it
(`Tools.Configuration.Reader`) as a project; do not create a README for the
`Tools/` folder itself beyond an optional one-line index if it aids navigation.

---

## Coding Guidelines

These are **guidelines, not laws**. Prefer clarity over cleverness, and
**make illegal states unrepresentable** in the domain. Allow messiness
**only at system boundaries** (EF/SQL/JSON/UI), then normalize into strict
domain types.

### Performance
- Prefer **clear code first**, then measure; optimize only where profiling shows pressure.
- Use **`Span<T>` / `ReadOnlySpan<T>` / `Memory<T>`** in hot paths that manipulate contiguous data to cut allocations and bounds checks.
- Leverage **`SearchValues`** for repeated membership/lookup checks over known char/byte sets.
- Consider **`CompositeFormat`** for heavily used format strings to reduce transient allocations.
- Use **collection expressions and spreads** to compose sequences clearly; avoid premature micro-optimizations that obscure intent.
- Favor **value objects as `readonly struct`** when small, immutable, and frequently allocated on hot paths (measure!).

#### Entity Framework Core
- **Prefer server-side evaluation** — push filters, projections, and joins into the database; avoid loading large sets for in-memory filtering.
- **Avoid `.Include()` unless necessary** for eager navigation loading; use **explicit projections** (`Select`) or **separate queries** when only partial data is needed.
- Keep **tracked entity graphs minimal**; prefer **`AsNoTracking()`** for read-only queries to reduce overhead.
- Validate generated SQL with logging or profiling when optimizing hot queries.

### Async Programming
- In **library code that may be used with a SynchronizationContext**, **`ConfigureAwait(false)`** to avoid deadlocks; in app/UI code, omit where context capture is desired.
- Avoid **`async void`** except for event handlers; return `Task`/`ValueTask` from everything else.
- Prefer **`ValueTask<T>`** for high-frequency, usually-synchronous operations; otherwise stick to `Task<T>` for simplicity.
- Use **`IAsyncEnumerable<T>`** for streaming scenarios; apply **cancellation tokens** and **bounded channels** where backpressure matters.
- Keep async **composable**: small, purpose-built methods; avoid mixing blocking (`.Result`, `.Wait()`) with async flows.
- Drop the `Async` suffix for methods that do not have a direct synchronous overload.

### Nullability & State Modeling
- **Enable** nullable reference types (`<Nullable>enable</Nullable>`) and **treat warnings as design feedback**.
- Use the null-forgiving operator (`!`) **sparingly** and only to document verified invariants (prefer refactoring to prove safety).
- **Ban sentinel values — including `null` — inside the domain model.**
  - Required values: use **non-nullable** properties and enforce invariants at construction (guarded factories).
  - Optional domain states: prefer **explicit sum types** (e.g., `Option<T>` or a small union like `Known | Unknown | NotApplicable`) over `T?`.
  - Failures of **operations** are **not** "absence": return a **Result/Railway** (following a pattern like `Outcome<T>`, `Result<T,E>`) rather than `null`.
- **At boundaries only** (DTOs, EF entities, external inputs), allow `null` to represent missing data; **normalize** to domain types immediately.
- Use **null-coalescing operators** (`??`, `??=`) for boundary defaults, not as a substitute for domain modeling.

#### Example (boundary → domain)
```csharp
public sealed record PrintJobDto(int? Copies);
public sealed class Copies { /* guarded factory enforcing >= 1 */ }

public static Outcome<PrintJob> ToDomain(PrintJobDto dto)
    => dto.Copies is int c
       ? Copies.Create(c).Map(cs => new PrintJob(cs))
       : Outcome.Fail("Copies is required.");
```

### Records & Value Types
- Use **`record`** (class) for immutable identity-by-value DTOs and lightweight domain messages.
- Use **`record struct`** or `readonly struct` for **small, immutable value objects** that are copied frequently (measure).
- Prefer **primary constructors** for brevity; push **invariant checks** into factories or constructors (don't rely on later mutation).
- For domain value objects, expose **behavior** (methods) near the data; avoid "primitive obsession".

#### Example (guarded value object)
```csharp
public sealed class Percentage
{
    public int Value { get; }
    private Percentage(int value) => Value = value;
    public static Outcome<Percentage> Create(int value)
        => value is >= 0 and <= 100
           ? Outcome.Ok(new Percentage(value))
           : Outcome.Fail("Percentage must be 0..100");
}
```

### Testing Standards

**Principle:** Tests should validate **behavior, contracts, and invariants**,
not internal mechanics or framework wiring.

Patterns:
- **AAA Structure** – Arrange, Act, Assert; one logical assertion per test.
- **Intent over Mechanics** – test observable outcomes, not private implementation details.
- **Hermetic Isolation** – no global state, shared fixtures, or hidden order dependencies.
- **Pure Functions Preferred** – deterministic seams simplify verification.
- **Table-Driven & Property-Based** – handle combinatorics and invariants efficiently.
- **Naming** – `Method_Scenario_Outcome`; keep unit tests sub-second.
- **Async Safety** – use `await` and `CancellationToken`, never block.
- **Inject Time** – replace `DateTime.UtcNow` with an injected clock or abstraction.

Constraints:
- **Mocks are exceptional, not default.**
  - Mock only dependencies you **do not own** (e.g., external APIs, third-party services).
  - Do **not** mock databases unless testing the **exact product and version** used in production.
  - REST controllers should remain **thin adapters** — their logic belongs in libraries tested directly.
  - Avoid mocking **internal REST calls**; strong library tests make them unnecessary.
- **Functional testing > simulated infrastructure.** Favor pure logic verification over environment emulation.
- **E2E and integration coverage** are provided by **Playwright** automation and the **QA team**; unit tests remain narrow in scope and fast.

#### Example (table-driven)
```csharp
[Theory]
[InlineData(0, true)]
[InlineData(50, true)]
[InlineData(101, false)]
public void Percentage_Create_ValidatesBounds(int input, bool ok)
{
    var result = Percentage.Create(input);
    Assert.Equal(ok, result.IsOk);
}
```

### Quick Decision Matrix
| Situation | Preferred Approach |
|------------|--------------------|
| Missing data from outside world | Allow `null` at boundary → normalize → domain type or failure (`Outcome`) |
| Optional domain concept | Use `Option<T>` pattern or explicit union, not `T?` |
| Operation can fail | Return `Result`/`Outcome` pattern, not `null` or exceptions for expected failures |
| Hot path allocations | Consider `Span<T>`, `SearchValues`, `ValueTask<T>`, `readonly struct` **after profiling** |

These standards maintain intent: **strict in the center, forgiving at the
edges, and measurable where performance matters**.

---

## Documentation Standard

Documentation exists to help a technical reviewer (recruiter, interviewer,
engineer) quickly assess competence and creativity. Treat docs as a
first-class deliverable, held to the same bar as code.

### Tone
- Professional, concise, confident. Not "silly," not self-deprecating.
- No marketing language, no emoji, no exclamation marks.
- Describe what the code does and what technique it demonstrates. Do not
  inflate scope or imply production/enterprise origin.

### Document tiers

**Root level:**
- `/README.md` — portfolio overview plus a Markdown table linking to each
  project's README with a one-line description. Lead with what the solution
  is and call out `Library.Operations` as the centerpiece. Include a short
  "Build & CI" subsection that points to `docs/BUILD.md`.
- `/docs/CONTENTS.md` — full index/map of the repo: every project and every
  doc, as a nested link tree.
- `/docs/BUILD.md` — documents the build and CI story: the GitHub Actions
  workflows under `.github/workflows`, `pipeline-build.ps1`, and how the
  pieces fit. Describe the *what* observable from those files; mark any
  intent/rationale that cannot be inferred with `TODO(wbellman)`.

**Per project** (each top-level project directory):
- `README.md` — overview only: what it is, the problem it explores, tech
  stack, how to build/run, and a link to its `docs/CORE_CONCEPTS.md`.
- `docs/CORE_CONCEPTS.md` — the notable techniques used and **why** they were
  chosen. Document the *what* (techniques observable in the code). For every
  *why* or design-rationale judgment that cannot be inferred from code,
  insert a placeholder:
  `> TODO(wbellman): why — <short prompt of what to fill in>`
- Create additional `docs/*.md` files only when a topic clearly does not fit
  CORE_CONCEPTS (e.g. `ARCHITECTURE.md`, an algorithm deep-dive). Link them
  from `CORE_CONCEPTS.md` and from `/docs/CONTENTS.md`.

### Rules
- Infer tech stack from `.sln`, `.csproj`, `package.json`, and source files.
- Never invent features, benchmarks, or rationale. If unknown, use a
  `TODO(wbellman)` marker rather than guessing.
- Use **relative links** between docs so they resolve on GitHub.
- Every README must link to its docs; `CONTENTS.md` must link to everything.
- Keep each README under ~40 lines; push depth into `docs/`.
- When documenting, work **one project at a time**; do not modify other
  projects in the same pass. List the files to be written before writing.

### Centerpiece & provenance
- `Library.Operations` is the **centerpiece** and original work. Give it the
  deepest treatment: its `docs/CORE_CONCEPTS.md` should walk through the
  notable design decisions and techniques in detail, with `TODO(wbellman)`
  markers for every design *rationale* that cannot be read from the code.
  The depth of this rationale is what demonstrates authorship — favor it over
  any bare "this is my own work" statement.
- Do not editorialize about originality in project READMEs. At most, the root
  `/README.md` may carry a single neutral line noting the work is the author's
  own; provenance is otherwise carried by the LICENSE and commit history.
- Never inflate `Library.Operations`' scope or imply production/enterprise
  origin; let the design reasoning speak for itself.
