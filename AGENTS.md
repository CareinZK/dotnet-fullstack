## 1. Think Before Coding

**Don't assume. Don't hide confusion. Surface tradeoffs.**

Before implementing:
- State your assumptions explicitly. If uncertain, ask.
- If multiple interpretations exist, present them - don't pick silently.
- If a simpler approach exists, say so. Push back when warranted.
- If something is unclear, stop. Name what's confusing. Ask.

## 2. Simplicity First

**Minimum code that solves the problem. Nothing speculative.**

- No features beyond what was asked.
- No abstractions for single-use code.
- No "flexibility" or "configurability" that wasn't requested.
- No error handling for impossible scenarios.
- If you write 200 lines and it could be 50, rewrite it.

Ask yourself: "Would a senior engineer say this is overcomplicated?" If yes, simplify.

## 3. Surgical Changes

**Touch only what you must. Clean up only your own mess.**

When editing existing code:
- Don't "improve" adjacent code, comments, or formatting.
- Don't refactor things that aren't broken.
- Match existing style, even if you'd do it differently.
- If you notice unrelated dead code, mention it - don't delete it.

When your changes create orphans:
- Remove imports/variables/functions that YOUR changes made unused.
- Don't remove pre-existing dead code unless asked.

The test: Every changed line should trace directly to the user's request.

## 4. Goal-Driven Execution

**Define success criteria. Loop until verified.**

Transform tasks into verifiable goals:
- "Add validation" → "Write tests for invalid inputs, then make them pass"
- "Fix the bug" → "Write a test that reproduces it, then make it pass"
- "Refactor X" → "Ensure tests pass before and after"

For multi-step tasks, state a brief plan:
```
1. [Step] → verify: [check]
2. [Step] → verify: [check]
3. [Step] → verify: [check]
```

Strong success criteria let you loop independently. Weak criteria ("make it work") require constant clarification.

---

## 5. Documentation & Architecture Navigation

The repository uses a **3-Tier Documentation System** optimized for LLM readability and fast contextual retrieval in a **Modular Monolith** architecture:

- **Tier 1 (`docs/`)**: Granular, modular technical documentation separated into cross-cutting architecture (`docs/architecture/`), shared development guides (`docs/development/`), and module-specific bounded contexts (`docs/modules/<module-name>/`).
- **Tier 2 (`llms.txt`)**: High-signal entry point at the root containing non-negotiable architectural invariants, system overview, and an intent-based file routing matrix.
- **Tier 3 (`llms-full.txt`)**: Monolithic, bundled documentation corpus containing all Tier 1 documents with a master table of contents for single-pass context loading.

### When to Read Which Tier

| Scenario / Task | File to Read | Why |
| :--- | :--- | :--- |
| **New session / Quick orientation / Rules check** | `llms.txt` | Read this first to understand core invariants, architectural boundaries, and locate exact files via the Intent Routing Table without wasting context window tokens. |
| **Broad multi-layer work / Complex cross-cutting task / Subagent initialization** | `llms-full.txt` | Read this when you need holistic, deep context across all layers, entities, repositories, and API conventions in a single pass. |
| **Surgical task on a specific module (e.g., DirectoryService)** | `docs/modules/<module-name>/...` | Read only the specific topic file (e.g., `domain-model.md`, `cqrs-application.md`, `persistence.md`, or `api-presentation.md`) to minimize prompt pollution. |
| **Setting up local environment, docker, or running tests** | `docs/development/getting-started.md` & `docs/development/testing-strategy.md` | Contains exact commands, ports (PostgreSQL `5434`, Seq `5341`), and testing patterns. |
| **Adding a new module to the Modular Monolith** | `docs/architecture/overview.md` | Follow the modular monolith rules, project structure, and isolation guidelines. |

---

**These guidelines are working if:** fewer unnecessary changes in diffs, fewer rewrites due to overcomplication, and clarifying questions come before implementation rather than after mistakes.
