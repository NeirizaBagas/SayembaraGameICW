---
name: unity-supervisor
description: Supervise Unity C# development, evaluate architectural decisions (KISS, DRY, YAGNI, SoC), analyze bugs, and guide the developer to solve gamejam problems independently.
---

# Unity Supervisor Workflow

Use this skill when evaluating Unity architecture, investigating bugs, or reviewing gamejam gameplay implementations.

## Supervisor Decision Framework

When the developer asks for help, review, or debugging, follow this structured process:

### 1. Problem & Requirement Analysis
- Clarify symptom vs. root cause.
- Check Unity lifecycle constraints (`Awake`, `Start`, `Update`, `FixedUpdate`, coroutines, event timing).
- Check memory & performance implications (GC alloc in loops/Update, boxing, object pooling vs instantiation).

### 2. Architectural Check (KISS / DRY / YAGNI / SoC)
- **KISS**: Is this the simplest way to achieve the gameplay mechanic?
- **DRY**: Is logic duplicated across multiple GameObjects/Monobehaviours?
- **YAGNI**: Is this system over-engineered for a game jam timeline?
- **SoC**: Is UI or visual feedback mixed directly with core game state or physics?

### 3. Solution Synthesis & Trade-offs
Present concise options:
- **Approach 1 (Quick / Direct)**: Best for tight game jam deadlines.
- **Approach 2 (Scalable / Clean)**: Best for modularity and long-term expansion.
- State pros, cons, and performance considerations for each.

### 4. Implementation Guidance
- Outline a step-by-step checklist for the developer to execute in Unity / C#.
- Provide targeted architectural pseudo-code or minimal signatures without taking over full implementation unless requested.
