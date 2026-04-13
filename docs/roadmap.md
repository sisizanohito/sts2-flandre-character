# Roadmap

## 目的

Flandre mod の今後の作業を、実装・検証・文書化の三本柱で見通せる形に整理する。

この文書は、いま何を優先すべきか、どこに未確定要素があるか、次回どこから入ればよいかを短時間で共有するための実務用メモとする。

関連文書:
- [agent-knowledge.md](C:/Users/isiis/Documents/flandre-mod/docs/agent-knowledge.md)
- [build-install-workflow.md](C:/Users/isiis/Documents/flandre-mod/docs/build-install-workflow.md)
- [combat-animation-progress.md](C:/Users/isiis/Documents/flandre-mod/docs/combat-animation-progress.md)
- [card-pool-draft.md](C:/Users/isiis/Documents/flandre-mod/docs/card-pool-draft.md)

## 現在地

- build、PCK 再構築、install の基本手順は整理されている。
- ローカライズ崩れの主要因だった stale PCK 問題は、原因と確認手順まで文書化済み。
- tooltip / keyword 系は一度修正と確認が進んでおり、今後は再発防止のための導入手順固定が重要。
- Flandre の戦闘見た目は現状 `Sprite2D` ベースで、簡易 idle アニメは導入済み。
- Spine 移行は準備段階で、素材投入と scene 差し替えが次の節目になっている。
- card pool まわりは草案があり、テーマと主要カード案は見えているが、実装順の確定はこれから。
- 運用面では hook ベース監視を採用せず、越権監視は heartbeat ベースに統一している。

## 直近優先タスク

1. 導入手順の固定化
build、PCK、install を毎回同じ流れで回せるようにし、表示不具合と asset 未反映を先に潰す。

2. パチュリー優先の設計整理
`card-pool-draft.md` と既存コードを照らし合わせ、`破壊の目`、`狂気`、連鎖処理、対象変更まわりの仕様を先に固める。
特に「どこまでを共通層で吸うか」「カード個別実装に寄せるか」を曖昧なまま進めない。

3. 咲夜優先の最小実装単位の確定
設計整理の結果を受けて、コモン帯から着手するか、先にアンコモンの中核札を作るかを決める。
実装は一度に広げず、挙動確認しやすい単位で切る。

4. 表示確認ポイントの定型化
hover tip、keyword、pool 名、戦闘中表示など、壊れやすい箇所を確認項目としてまとめる。
ゲームログ、PCK 内容、画面表示の対応を揃えて、再確認を短くする。

5. Spine 移行の入口整備
`Enable-FlandreSpineVisuals.ps1` を使う前提で、必要素材、配置先、差し替え対象を明文化する。

## 中期タスク

- Flandre の戦闘ビジュアルを `SpineSprite` ベースへ移行し、最低限の `idle / attack / cast / hurt / die` を揃える。
- `破壊の目` を軸にしたカード群を、コモンから順に検証可能な形で実装していく。
- `狂気` の対象変更処理について、共通層パッチ依存をどこまで許容するか整理し、保守しやすい形へ寄せる。
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
- 厳密な STS2 検証は [agent-knowledge.md](C:/Users/isiis/Documents/flandre-mod/docs/agent-knowledge.md) の手順と判定基準に従う。
- 越権監視は heartbeat ベースで行い、hook 実装は運用前提にしない。
- assets や localization を触った変更は、repo 上の修正だけで完了扱いにせず、install 後の実体確認まで含める。

### Git

- commit 対象は現在の issue に属する変更だけに絞る。
- 調査用の一時変更は、恒常的な価値がない限り残さない。
- 実装と docs 更新を同時に行う場合も、差分の意味が追える粒度を保つ。

## 次回作業の入口

1. まず [agent-knowledge.md](C:/Users/isiis/Documents/flandre-mod/docs/agent-knowledge.md) を確認し、運用前提を揃える。
2. build / install を含む作業なら、先に [build-install-workflow.md](C:/Users/isiis/Documents/flandre-mod/docs/build-install-workflow.md) を確認する。
3. gameplay 側を進めるなら、[card-pool-draft.md](C:/Users/isiis/Documents/flandre-mod/docs/card-pool-draft.md) を起点に、パチュリー観点で未確定仕様を洗い出す。
4. 実装へ入る場合は、咲夜観点で「一度に確認できる最小単位」を決めてから着手する。
5. 戦闘演出を進める場合は、[combat-animation-progress.md](C:/Users/isiis/Documents/flandre-mod/docs/combat-animation-progress.md) を起点に素材と scene の前提を確認する。
6. 作業後は、再利用できる判断と手順だけを docs に戻す。

