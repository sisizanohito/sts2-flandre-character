using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;

namespace FlandreMod.Monsters;

// Echo Link カードで召喚されるパッシブダミー敵。
// 何もしない（HiddenIntent + no-op move）。
// ダメージを受けるとLinkPowerを通じてリンク先の敵に50%転送する。
public sealed class LinkedDummyMonsterModel : MonsterModel
{
    public override LocString Title => MonsterModel.L10NMonsterLookup("ECHO.name");

    protected override string VisualsPath => SceneHelper.GetScenePath("creature_visuals/defect");

    public override int MinInitialHp => 5;
    public override int MaxInitialHp => 5;

    protected override MonsterMoveStateMachine GenerateMoveStateMachine()
    {
        var states = new List<MonsterState>();
        var idleMove = new MoveState("NOTHING", NothingMove, new HiddenIntent());
        idleMove.FollowUpState = idleMove;
        states.Add(idleMove);
        return new MonsterMoveStateMachine(states, idleMove);
    }

    private Task NothingMove(IReadOnlyList<Creature> targets) => Task.CompletedTask;
}
