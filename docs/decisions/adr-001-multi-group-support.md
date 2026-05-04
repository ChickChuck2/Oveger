# ADR 001: Multi-Group Support Implementation

## Status
Accepted

## Context
Users want to categorize a single file shortcut into multiple groups. The initial implementation only supported a single group per item and had a logic bug in the group lookup.

## Decision
1. Change the internal API to return a list of groups for any given path.
2. In the UI, represent an item in multiple groups by creating a separate `StackPanel` (card) instance for each group.
3. Use the `Tag` property of the `StackPanel` to maintain context (which group that specific card belongs to).

## Consequences
- **Pros**: Clear visual representation of items in their respective categories. Actions like "Remove from Group" are context-aware.
- **Cons**: UI elements are duplicated in memory for items in multiple groups. Any state change (like renaming) requires a full UI reload (`Reload()`) to ensure consistency across all instances.
