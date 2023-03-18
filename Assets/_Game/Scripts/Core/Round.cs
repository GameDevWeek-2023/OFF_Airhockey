using Airhockey.Events;
using Airhockey.Utils.FSM;
using DG.Tweening;
using UnityEngine;

namespace Airhockey.Core {
    public class Round : StateMachine<Round, Match> {
        private class CountdownState : State<Round, Match> {
            public override void OnEnter() {
                var sequence = DOTween.Sequence();
                sequence.AppendInterval(Parent.Countdown).AppendCallback(() => { StateMachine.SwitchState("enable"); });
            }

            public override void OnExit() { }

            public override void OnUpdate() { }
        }

        private class EnableState : State<Round, Match> {
            public override void OnEnter() {
                Parent.Arena.LockPlayer = false;

                StateMachine.SwitchState("waitForGoal");
            }

            public override void OnExit() { }

            public override void OnUpdate() { }
        }

        private class WaitForGoalState : State<Round, Match> {
            public override void OnEnter() {
                Signals.Subscribe(GameSignal.OnGoalScored, OnGoalScored);
                Signals.Publish(GameSignal.OnStartSpawnPortals);
            }

            public override void OnExit() {
                Signals.Unsubscribe(GameSignal.OnGoalScored, OnGoalScored);
                Signals.Publish(GameSignal.OnStopSpawnPortals);
            }

            private void OnGoalScored(Signals.Args obj) {
                if (!obj.Read(out int index)) return;

                StateMachine.goalScoredIn = index;
                StateMachine.SwitchState("disable");
            }

            public override void OnUpdate() { }
        }

        private class DisableState : State<Round, Match> {
            public override void OnEnter() {
                Parent.Arena.LockPlayer = true;
                
                Parent.Arena.ResetArena(StateMachine.goalScoredIn, () => {
                    Signals.Publish(GameSignal.OnRoundEnd);

                    if (!Parent.DidPlayerWon()) {
                        StateMachine.SwitchState("countdown");
                        return;
                    }

                    Signals.Publish(GameSignal.OnMatchEnd);
                });
            }

            public override void OnExit() { }

            public override void OnUpdate() { }
        }

        public int goalScoredIn = -1;

        public Round(Match parent) : base(parent) {
            AddState("countdown", new CountdownState());
            AddState("enable", new EnableState());
            AddState("waitForGoal", new WaitForGoalState());
            AddState("disable", new DisableState());
        }

        public void Start() {
            Entry("countdown");
        }

        public void Reset() {
            goalScoredIn = -1;
        }
    }
}