---
name: Claude PR Review
description: Review newly opened pull requests with Claude and post actionable findings.
intent: Help maintainers identify substantiated bugs, security issues, and performance regressions in proposed changes.
on:
  pull_request:
    types: [opened]
permissions:
  contents: read
  issues: read
  pull-requests: read
engine: claude
strict: true
tools:
  github:
    mode: gh-proxy
    toolsets: [default]
safe-outputs:
  create-pull-request-review-comment:
    max: 10
  submit-pull-request-review:
    max: 1
    allowed-events: [COMMENT]
---

# Claude PR Review

Review the triggering pull request's changes and relevant surrounding code. Report only actionable issues introduced by this pull request; avoid style preferences and speculative concerns.

For each confirmed issue, add an inline review comment on a changed line with a concise explanation and impact. Submit one `COMMENT` review summarizing the findings. Do not approve, request changes, or modify repository files.

If you find no actionable issues after reviewing the changes, submit a `COMMENT` review stating that no actionable issues were found. If the diff or context is unavailable or insufficient to review reliably, call `noop` with a short reason.
