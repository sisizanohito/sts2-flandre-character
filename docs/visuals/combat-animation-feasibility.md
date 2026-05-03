# Flandre 戦闘アニメ化の可否メモ

## 判定
- 結論: 可能。
- 根拠:
  - 現在の Flandre は `Sprite2D` 1枚表示の静止構成。
  - 既存キャラは `SpineSprite` + `*_skel_data.tres` + `*.atlas` + `*.skel` で戦闘アニメを再生している。
  - `NCreatureVisuals` は `SpineSprite` を検出するとアニメ制御を有効化する。

## 参照したアセット/コード
- `flandremod/Characters/FlandreCharacter/flandre_character.tscn`
- `recovered-game/scenes/creature_visuals/ironclad.tscn`
- `recovered-game/animations/characters/ironclad/ironclad_skel_data.tres`
- `Code/Characters/FlandreCharacter.cs`
- `Code/Characters/FlandreVisualTextureFixPatch.cs`

## 必要素材 (本番)
- 必須:
  - `flandre.atlas` (Spine atlas)
  - `flandre.skel` (Spine binary)
  - `flandre_skel_data.tres` (Godot の SpineSkeletonDataResource)
  - テクスチャPNG (atlas参照先)
- 推奨アニメ名:
  - `idle_loop`
  - `attack`
  - `cast`
  - `hurt`
  - `die`

## Meshy で先行生成した素材タスク
- Tポーズ基準立ち絵: `019d7c77-093e-7568-be80-313a0ac41968`
- 攻撃ポーズ参考: `019d7c77-1f4c-7ad9-a716-a3d9e3205adf`
- 被弾ポーズ参考: `019d7c77-1fcc-7c9b-9c0d-1adfd5acb353`

## 実装方針
- 1. Meshy画像をベースに、Spine Editorでパーツ分け/ボーン/アニメを作成。
- 2. `flandre.atlas` と `flandre.skel` を `flandremod/animations/characters/flandre/` に配置。
- 3. `flandre_character.tscn` の `Visuals` を `Sprite2D` から `SpineSprite` に置換。
- 4. `FlandreCharacter.CustomVisualPath` はそのまま利用可能。
- 5. 必要に応じて `FlandreVisualTextureFixPatch` を簡略化。

## 画像編集バックアップ運用
- 画像変更前は必ずバックアップすること。
- 追加済みスクリプト:
  - `tools/Backup-ImageAsset.ps1`
- 例:
  - `.\tools\Backup-ImageAsset.ps1 -SourcePath flandremod\Characters\FlandreCharacter\flandre_character.png`
- 初回バックアップ保存先:
  - `assets_tmp/backups/images/20260411-211559/`
