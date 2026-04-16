# Roadmap

## 目的

Flandre mod の今後の作業を、実装・検証・文書化の三本柱で見通せる形に整理する。

この文書は、いま何を優先すべきか、どこに未確定要素があるか、次回どこから入ればよいかを短時間で共有するための実務用メモとする。

関連文書:
- [agent-knowledge.md](./agent-knowledge.md)
- [build-install-workflow.md](./build-install-workflow.md)
- [combat-animation-progress.md](./combat-animation-progress.md)
- [card-pool-draft.md](./card-pool-draft.md)

## 現在地

- build、PCK 再構築、install の基本手順は整理されている。
- ローカライズ崩れの主要因だった stale PCK 問題は、原因と確認手順まで文書化済み。
- tooltip / keyword 系は一度修正と確認が進んでおり、今後は再発防止のための導入手順固定が重要。
- Flandre の戦闘見た目は現状 `Sprite2D` ベースで、簡易 idle アニメは導入済み。
- Spine 移行は準備段階で、素材投入と scene 差し替えが次の節目になっている。
- card pool まわりは草案があり、テーマと主要カード案は見えているが、実装順の確定はこれから。
- 運用面では hook ベース監視を採用せず、越権監視は heartbeat ベースに統一している。

## 直近優先タスク

1. DokaanCard / RandomReflectionCard に限定した最小実装単位
完了: `狂気` の共通 Harmony パッチを維持したまま、`DokaanCard` / `RandomReflectionCard` の対象差分を確認した。
確認: `狂気なし` では `DokaanCard` が `AnyEnemy`、`RandomReflectionCard` が `RandomEnemy` のまま出ることを確認した。
確認: `狂気あり` では両方とも `AllEnemies` として表示され、target なしでプレイでき、`DokaanCard` -> `RandomReflectionCard` の順で `MadnessPower` が `2 -> 1 -> 消滅` と消費されることを確認した。

2. 表示確認ポイントの定型化
hover tip、keyword、pool 名、戦闘中表示など、壊れやすい箇所を確認項目としてまとめる。
ゲームログ、PCK 内容、画面表示の対応を揃えて、再確認を短くする。
完了: [install-gate-checklist.md](./install-gate-checklist.md) に install gate 後段の短い表示確認手順を追加した。

3. 導入手順の固定化
build、PCK、install の基本フローと stale PCK 再発防止の確認手順は暫定的に整理できたため、自己検証サマリと checklist を運用に定着させる。
完了: [install-gate-checklist.md](./install-gate-checklist.md) を追加し、導入完了判定と追加確認条件を分離した。

4. パチュリー優先の設計整理
完了: `狂気` と対象変更処理の共通層依存の許容範囲を整理し、実装受け渡し用メモを [madness-design-note.md](./madness-design-note.md) にまとめた。

5. 咲夜優先の最小実装単位の確定
完了: `madness-design-note.md` の前提に沿って、`DokaanCard` と `RandomReflectionCard` に限定した小修正を次の実装単位として整理した。
検証観点は `狂気なし` / `狂気あり` の対象数差を 2 枚で見る形に絞った。

6. Spine 移行の入口整備
`Enable-FlandreSpineVisuals.ps1` を使う前提で、必要素材、配置先、差し替え対象を明文化する。
完了: [`flandre-spine-visual-entry.md`](./flandre-spine-visual-entry.md) を追加し、必須 3 素材、配置先 `flandremod/animations/characters/flandre/`、生成される `flandre_skel_data.tres`、差し替え対象 `flandremod/Characters/FlandreCharacter/flandre_character.tscn`、`Visuals` の `Sprite2D` -> `SpineSprite` 置換内容を固定した。

7. `Gaze` の starter / common 整合
`card-pool-draft.md` では starter `Gaze` x2 と common 配置が前提なので、`EchoLinkCard` の rarity をその前提に揃える。
完了: [`Code/Cards/EchoLinkCard.cs`](../Code/Cards/EchoLinkCard.cs) の rarity を `Uncommon` -> `Common` に修正し、[`card-pool-draft.md`](./card-pool-draft.md) に現況メモを追加した。

## 次の直近着手

- 次は画像・アニメーション作業を除く安全枠として、[card-pool-draft.md](./card-pool-draft.md) を起点に `MadGazeCard` か `SparkScatterCard` のどちらか 1 件を次の common 検証単位として固定する。

## 中期タスク

- Flandre の戦闘ビジュアルを `SpineSprite` ベースへ移行し、最低限の `idle / attack / cast / hurt / die` を揃える。
- `破壊の目` を軸にしたカード群を、コモンから順に検証可能な形で実装していく。
- `狂気` の対象変更処理について、共通層パッチ依存をどこまで許容するか整理し、保守しやすい形へ寄せる。
- カード実装と deck build の自由度を広げるため、`破壊の目` / `狂気` 以外にも将来の主軸になりうる新ギミックをあと 2 つ程度検討し、既存テーマと競合しすぎない形で拡張余地を確保する。
- Reflection 系やランダム生成カードを含む gameplay 検証経路を、ローカライズ込みで安定させる。
- docs を調査メモ中心から、再実行可能な作業手順と判断記録中心へ寄せる。

## 保留中・リスク

- 一部 docs に文字化けが残っており、過去判断や役割分担の読み取り精度を落としている。
- sub-agent の `sts2_moddding` 実行可否は seat 状態に影響されるため、検証担当の再現性に揺れがある。
- Meshy 経由の素材取得は手動工程が残りやすく、戦闘アニメ移行の詰まりどころになりやすい。
- `狂気` や対象変更系は共通層の効き方次第で副作用が広がるため、実装を急ぐと差分管理が難しくなる。
- card pool 実装を先に広げすぎると、仕様未確定部分の巻き戻しコストが大きくなる。

## 運用ルール

### 役割分担

- パチュリーは API / 内部設計レビューを優先し、仕様の前提と危険箇所を先に揃える。
- 咲夜はその整理結果を受けて、コード変更案と実装を最小単位で進める。
- 小悪魔は docs / ナレッジ整理を担当し、手順、教訓、確認観点、保留事項を共有文書へ戻す。
- フランドールが `sts2_moddding` を使う検証は別担当の役割として扱い、この文書では前提化しない。

### 検証

- confirmed fact と inference を混ぜない。
- 厳密な STS2 検証は [agent-knowledge.md](./agent-knowledge.md) の手順と判定基準に従う。
- 越権監視は heartbeat ベースで行い、hook 実装は運用前提にしない。
- assets や localization を触った変更は、repo 上の修正だけで完了扱いにせず、install 後の実体確認まで含める。

### Git

- commit 対象は現在の issue に属する変更だけに絞る。
- 調査用の一時変更は、恒常的な価値がない限り残さない。
- 実装と docs 更新を同時に行う場合も、差分の意味が追える粒度を保つ。

## 次回作業の入口

1. まず [agent-knowledge.md](./agent-knowledge.md) を確認し、運用前提を揃える。
2. build / install を含む作業なら、先に [build-install-workflow.md](./build-install-workflow.md) を確認する。
3. gameplay 側を進めるなら、[card-pool-draft.md](./card-pool-draft.md) を起点に、パチュリー観点で未確定仕様を洗い出す。
4. 実装へ入る場合は、咲夜観点で「一度に確認できる最小単位」を決めてから着手する。
5. 戦闘演出を進める場合は、[combat-animation-progress.md](./combat-animation-progress.md) を起点に素材と scene の前提を確認する。
6. 作業後は、再利用できる判断と手順だけを docs に戻す。

