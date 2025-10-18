# Research and Design Architecture
<!-- Template Version: 13 | ContextKit: 0.1.0 | Updated: 2025-10-02 -->

> [!WARNING]
> **👩‍💻 FOR DEVELOPERS**: Do not edit the content above the developer customization section - changes will be overwritten during ContextKit updates.
>
> For project-specific customizations, use the designated section at the bottom of this file.
>
> Found a bug or improvement for everyone? Please report it: https://github.com/FlineDev/ContextKit/issues

## Description
**Two-phase workflow with review checkpoint**: First run creates Research.md and halts for user review. Second run (after review) creates Tech.md architecture plan informed by approved research findings.

## User Input Format

```
═══════════════════════════════════════════════════
║ ❓ USER INPUT REQUIRED - [Topic]
═══════════════════════════════════════════════════
║
║ [Question and context]
║
║ [Response instruction]
```

## Execution Flow (main)

**TWO-PHASE WORKFLOW**: This command operates in two stages with a review checkpoint:
- **First run**: Creates Research.md → HALTS for user review
- **Second run**: Creates Tech.md (after research approval)

### Phase 0: Check Customization

0. **Read the "👩‍💻 DEVELOPER CUSTOMIZATIONS" section**
   - Use `Grep` tool to find the start of the section
   - Read everything below that line contained in this document til the end of the file
   - Make sure to consider what was said there with high priority
   - If anything conflicts with the rest of the workflow, prioritize the "developer customizations"

### Phase 1: Setup Validation & Prerequisites

1. **Check Project Setup**
   - Use `Glob` tool to verify Context.md exists: `Glob . Context.md`
   - If Context.md missing:
     ```
     ❌ ContextKit not initialized in this project!

     Run /ctxk:proj:init first to setup ContextKit in this project.
     This command requires project context to detect tech stack and apply
     appropriate development guidelines.
     ```
     → END (exit with error)

### Phase 2: Feature Detection & Validation

2. **Detect Current Feature**
   - Use `Bash` tool to check current git branch: `git branch --show-current`
   - If on feature/[prefix]-[name] branch: Extract feature name from branch
   - If not on feature branch: Ask user which feature to work on using consistent format
   - Use `Glob` tool to find numbered feature directory: `Glob Context/Features/???-[FeatureName]`
   - Store the found directory path for use in subsequent steps

3. **Validate Prerequisites & Determine Workflow Stage**
   - Use `Read` tool to check Spec.md exists in the found numbered directory: `Read [numbered-feature-directory]/Spec.md`
   - If Spec.md missing:
     ```
     ❌ Feature specification not found!

     Run /ctxk:plan:1-spec first to create the business requirements.
     Technical planning requires completed specification as input.
     ```
     → END (exit with error)
   - Check for 🚨 [NEEDS CLARIFICATION] markers in specification
   - If clarifications exist: WARN user to resolve them first

   - **Determine Workflow Stage**: Use `Glob` tool to check if Research.md exists: `Glob [numbered-feature-directory]/Research.md`
   - If Research.md NOT found:
     - **WORKFLOW STAGE**: Research Phase (proceed to Phase 3)
   - If Research.md found:
     - Use `Read` tool to check if it contains system instructions: `Read [numbered-feature-directory]/Research.md`
     - Search for "🤖 EXECUTION FLOW" or boxed instruction sections
     - If system instructions found: ERROR "Research.md incomplete - contains template instructions"
     - If no system instructions (clean research document):
       - **WORKFLOW STAGE**: Tech Architecture Phase (skip Phase 3, proceed to Phase 4)

### Phase 3: Research Phase Execution

4. **Copy Research Template**
   ```bash
   cp ~/.ContextKit/Templates/Features/Research.md [numbered-feature-directory]/Research.md
   echo "✅ Copied technical research template"
   ```

5. **Execute Research Template System Instructions**
   - Use `Read` tool to read the **ENTIRE** copied Research.md template: `Read [numbered-feature-directory]/Research.md`
   - **CRITICAL**: The template contains 300+ lines with detailed system instructions - read it completely to understand all phases
   - **CRITICAL**: Follow the Research.md template's **🤖 EXECUTION FLOW** instructions step by step:

   **Phase 1: Knowledge Gap Identification** (Steps 1-4 in Research.md)
   - Load feature specification and project context
   - Identify all research targets from specification
   - Launch comprehensive codebase integration agent if needed

   **Phase 2: Active Knowledge Acquisition** (Steps 5-7 in Research.md)
   - Launch technology research agents using `Task` tool for each mentioned technology
   - Launch API research agents for external services using `Task` tool
   - Launch architecture pattern research agents using `Task` tool
   - **CRITICAL**: Instruct ALL agents to RETURN findings as text responses, NOT create markdown files
   - **Wait for ALL agents to complete** before proceeding

   **Phase 3: Research Consolidation** (Steps 8-12 in Research.md)
   - Use `Edit` tool to replace template header with specific feature information
   - Use `Edit` tool to document ALL technology research results from completed agents
   - Use `Edit` tool to document ALL API & service research results from completed agents
   - Use `Edit` tool to document codebase integration analysis from completed agents
   - Use `Edit` tool to document ALL architectural pattern research from completed agents

   **Phase 4: Completion** (Steps 13-15 in Research.md)
   - Validate research completeness against specification
   - Generate research summary with key decisions
   - Use `Edit` tool to **remove the entire boxed system instructions section** from Research.md

   **Research execution**: You must populate the Research.md file with actual findings, not leave it as a template

6. **Clean Up Research Template**
   - Use `Read` tool to check if Research.md still contains system instructions: `Read [numbered-feature-directory]/Research.md`
   - Search for "🤖 EXECUTION FLOW" or "VALIDATION & EXECUTION STATUS" sections
   - If system instructions remain: Use `Edit` tool to remove all boxed instruction sections
   - Ensure final Research.md contains only clean research results

7. **Validate Research Completion**
   - Use `Read` tool to verify research completion: `Read [numbered-feature-directory]/Research.md`
   - Ensure all technologies mentioned in specification have been researched
   - Verify research findings are documented with decisions and rationale
   - If research incomplete: WARN user to complete research before proceeding

8. **HALT for User Review** (see Research Phase Completed message)
   - Display success message with review instructions
   - **END EXECUTION** - DO NOT proceed to Phase 4
   - User must review Research.md and run command again to continue

### Phase 4: Technical Architecture Planning (Only Executes on Second Run)

**SKIP THIS PHASE if Research.md doesn't exist** (handled by Phase 2 workflow stage detection)

9. **Copy Technical Architecture Template**
   ```bash
   cp ~/.ContextKit/Templates/Features/Tech.md [numbered-feature-directory]/Tech.md
   echo "✅ Copied technical architecture template"
   ```

10. **Execute Technical Architecture Template**
    - Use `Read` tool to read the copied Tech.md: `Read [numbered-feature-directory]/Tech.md`
    - Follow the **system instructions** section (boxed area) step by step
    - The template contains technical architecture generation logic informed by research results
    - Use tools (`Read`, `Edit`) as directed by the template instructions
    - **Template execution**: The copied Tech.md handles architecture decisions, Context/Guidelines compliance, and complexity assessment
    - **Progress tracking**: User can see architectural planning checkboxes being completed in the copied file

11. **Clean Up Technical Architecture Template**
    - Use `Read` tool to check if Tech.md still contains system instructions: `Read [numbered-feature-directory]/Tech.md`
    - Search for "🤖 EXECUTION FLOW" or "VALIDATION & EXECUTION STATUS" sections
    - If system instructions remain: Use `Edit` tool to remove all boxed instruction sections
    - Ensure final Tech.md contains only clean technical architecture content

12. **Extract and Resolve Clarification Points Interactively**
    - Use `Grep` tool to find clarification markers in Research.md: `Grep "🚨 \\[NEEDS CLARIFICATION:" [numbered-feature-directory]/Research.md`
    - Use `Grep` tool to find clarification markers in Tech.md: `Grep "🚨 \\[NEEDS CLARIFICATION:" [numbered-feature-directory]/Tech.md`
    - If clarification points found in either file:
      - Parse each clarification point to extract the specific question, file location, and line context
      - **FOR EACH CLARIFICATION (one at a time, from both files)**:
        - Present the specific question to user using User Input Format:
          ```
          ═══════════════════════════════════════════════════
          ║ ❓ TECHNICAL CLARIFICATION NEEDED - [Research.md | Tech.md]
          ═══════════════════════════════════════════════════
          ║
          ║ [Specific extracted question from 🚨 [NEEDS CLARIFICATION: ...]]
          ║
          ║ Please provide your answer to resolve this technical requirement:
          ```
        - **WAIT for user response** (execution MUST stop until user answers)
        - Use `Edit` tool to replace the 🚨 [NEEDS CLARIFICATION: ...] marker with the user's answer in the appropriate file
        - Continue to next clarification point only after current one is resolved
      - After all clarifications resolved: confirm all markers removed from both Research.md and Tech.md

13. **Display Success Message** (see Tech Architecture Completed message)

## Error Conditions

- **"Context.md not found"** → User must run `/ctxk:proj:init` to initialize ContextKit
- **"Feature specification not found"** → Must run `/ctxk:plan:1-spec` first
- **"Research.md incomplete - contains template instructions"** → Research phase was started but not completed - system instructions still present
- **"Technical template not found"** → Ensure template files are available
- **"Specification has unresolved clarifications"** → Resolve [NEEDS CLARIFICATION] markers in Spec.md first
- **"Template execution failed"** → Verify Research.md and Tech.md templates contain system instructions sections
- **"Research.md not populated"** → Research agents completed but findings not documented in Research.md - must execute template's Phase 3 consolidation steps
- **"Cleanup validation failed"** → System instruction sections remain after template execution - manual cleanup required

## Validation Gates

### First Run (Research Phase)
- Project Context.md exists (ContextKit project setup complete)?
- Feature specification exists and is complete?
- No unresolved [NEEDS CLARIFICATION] markers in specification?
- Research.md doesn't exist yet (not second run)?
- Research template copied and executed successfully?
- Research.md system instructions cleaned up immediately after completion?
- User informed to review Research.md before continuing?

### Second Run (Tech Architecture Phase)
- Research.md exists and is complete (no system instructions)?
- Tech.md doesn't exist yet (avoiding duplicate creation)?
- Technical template copied and executed successfully?
- Tech.md system instructions cleaned up immediately after completion?
- Clarification points resolved interactively one at a time from Research.md and Tech.md?
- User informed to review and commit technical plan before proceeding?

## Integration Points

- **Global ContextKit**: Uses Templates/Features/Tech.md template for technical architecture generation
- **Project Setup**: Requires Context.md created by /ctxk:proj:init for project detection and context
- **Template Execution**: Delegates all technical logic to copied Tech.md template (follows init-workspace pattern)
- **Development Workflow**: Creates foundation for /ctxk:plan:3-steps implementation planning phase
- **Team Collaboration**: Creates committed technical plan for team review and development guidance
- **Git Integration**: Works within existing feature branch for systematic development workflow
- **Workspace Integration**: Template inherits coding standards and constitutional overrides from workspace Context.md

## Success Messages

### Research Phase Completed (First Run - HALT)
```
🎉 Technical research completed!

✅ Created: Context/Features/[Name]/Research.md

📋 What to do next:

1️⃣  **Review** the research findings in Context/Features/[Name]/Research.md
    - Make any corrections or additions if needed

2️⃣  **Continue** by running this command again:
    /ctxk:plan:2-research-tech

    This will create the Tech.md architecture plan based on your research.

💡 Tips:
   - You can review and edit Research.md first, or continue immediately if it looks good
   - If context usage is above 60%, consider running /compact or starting a new chat before continuing
```

### Tech Architecture Completed (Second Run - Complete)
```
🎉 Technical architecture planning completed successfully!

✅ Existing: Context/Features/[Name]/Research.md (from previous run)
✅ Created: Context/Features/[Name]/Tech.md
✅ Applied Context/Guidelines/Swift.md and SwiftUI.md standards
✅ All mandatory sections completed with research-informed decisions
✅ Technical template system instructions cleaned up

✅ All technical clarifications resolved interactively during generation

🔗 Next Steps:
1. Review Context/Features/[Name]/Tech.md to ensure technical decisions are sound
2. [If clarifications needed:] Edit the file to resolve marked questions
3. When satisfied with the technical plan: commit your changes with git
4. Run /ctxk:plan:3-steps to proceed with implementation task breakdown

💡 Research-informed technical architecture ready for implementation planning!
```

════════════════════════════════════════════════════════════════════════════════
👩‍💻 DEVELOPER CUSTOMIZATIONS - EDITABLE SECTION
════════════════════════════════════════════════════════════════════════════════

This section is preserved during ContextKit migrations and updates.
Add project-specific instructions, examples, and overrides below.

## Project-Specific Instructions

<!-- Add any project-specific guidance for technical architecture planning here -->

## Additional Examples

<!-- Add examples of architectural patterns that work well with your project -->

## Override Behaviors

<!-- Document any project-specific architectural requirement overrides here -->