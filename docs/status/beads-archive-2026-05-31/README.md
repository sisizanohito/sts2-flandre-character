# Beads Archive 2026-05-31

このディレクトリは Beads から Agent Teams `koumakan` board へ移行した後の履歴保全 snapshot です。

## Source

- `pre-close-*` は local commit `8d27e4a chore: remove beads task data` の直前、`HEAD~1` の tracked `.beads/issues.jsonl` から一時 worktree で Beads を復元して取得した。
- root `.beads/**` と `.agents/skills/beads/**` は、この archive 作成前に local commit `8d27e4a` で既に削除済みだった。
- `refs-dolt-data-check.txt` は cleanup 実行時点の local / remote `refs/dolt/data` 再確認結果。
- `local-git-hooks-cleanup.txt` は local Git config の `core.hooksPath` を外した記録。cleanup 前は root `.beads/hooks` を指していた。

## Important Files

- `beads-to-agent-teams-map.json`: Beads open 15 件と Agent Teams task ID の対応表。
- `pre-close-issues.jsonl`: close 前の tracked `.beads/issues.jsonl`。
- `pre-close-open.json`, `pre-close-in-progress.json`, `pre-close-closed.json`: close 前の status 別 snapshot。
- `post-close-issues.jsonl`, `post-close-open.json`, `post-close-in-progress.json`, `post-close-closed.json`: Agent Teams task ID 入り close reason を反映した close 後 snapshot。
- `close-commands-log.txt`: 実行した `bd close` コマンド、exit code、出力。最初の epic close 失敗と、子 issue から閉じ直した retry を含む。
- `local-git-hooks-cleanup.txt`: `git config --unset core.hooksPath` と検証結果。
- `summary.json`: close 前後の件数 summary。

## Counts

- close 前: open 15 / in_progress 0 / closed 6
- close 後: open 0 / in_progress 0 / closed 21
