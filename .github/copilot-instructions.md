# Copilot Instructions

The authoritative working standard for this repository lives in
[`/AGENTS.md`](../AGENTS.md). Read it and treat it as binding for every task
in this repo — coding, refactoring, review, and documentation.

Always reference `AGENTS.md` before acting. If anything here appears to
conflict with it, `AGENTS.md` wins.

Quick reminders (full detail in `AGENTS.md`):
- Stack is **C#** (solution: `Libraries.sln`). `Library.Operations` is the
  centerpiece and original work — give it the deepest documentation.
- This repo has its own **CI/build** (`.github/workflows`, `pipeline-build.ps1`);
  document it in `docs/BUILD.md` per the standard.
- Strict in the domain center, forgiving at boundaries; **no `null` sentinels
  in the domain** — normalize at the edge, return `Outcome`/`Result` for
  expected failures.
- Clear code first; optimize only after profiling.
- For documentation work, follow the **Documentation Standard** section:
  README = overview, depth goes in `docs/`, and use `TODO(wbellman):` markers
  for any "why" that cannot be inferred from code. Document one project per pass.
