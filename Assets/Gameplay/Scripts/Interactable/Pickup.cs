using System;
using System.Linq;
using IV.Gameplay.Actions;
using IV.Core.Actions;
using IV.Core.Interactions;
// using Unity.VisualScripting;
using UnityEngine;

namespace IV.Gameplay.Interactions
{
    [RequireComponent(typeof(BaseInteractable))]
    public partial class Pickup : MonoBehaviour
    {
        [SerializeField] private ActionQueue actionQueue;
        [SerializeField] private Backpack backpack;

        // [SerializeField] private ScriptGraphAsset graphAsset;
        // [SerializeField] private ScriptMachine graphMachine;

        public Data data;

        // private ControlOutput controlOutput;
        // private SubgraphUnit subgraphUnit;

        private void OnAwake()
        {
            // controlOutput = new ControlOutput("script_output");
            // controlOutput = graphAsset.graph.units.OfType<GraphInput>().First().controlOutputs.First();

            // subgraphUnit = new SubgraphUnit(graphAsset);
            // subgraphUnit.controlOutputs.Add(controlOutput);

            //graphAsset.graph.units.OfType<GraphInput>().First().controlOutputs.First();
            // var controlInput = graphAsset.graph.units.OfType<GraphOutput>().First().controlInputs.First();
            // controlInput.ConnectToValid(controlOutput);
        }

        // private void OnEnable()
        // {
        //     var graphPtr = graphAsset.GetReference();
        //     var graphRef = (GraphReference) graphPtr;// GraphReference.New(graphPtr.root, false);
        //     var flow = Flow.New(graphRef);
        //     flow.Run(controlOutput);
        // }

        public void OnRequest()
        {
            if (!baseInteractable.AssertInteraction())
            {
                baseInteractable.ShowHint();
                return;
            }

            var destination = baseInteractable.GetInteractionPoint();
            var requestMoveAction = actionQueue.GetAction<RequestMoveAction>().SetUp(destination);
            actionQueue.QueueAction(requestMoveAction);

            var interactAction = actionQueue.GetAction<InteractAction>();
            interactAction.interactable = baseInteractable;
            actionQueue.QueueAction(interactAction, false);
        }

        public void OnPickUp() => backpack.AddPickup(data);

        [Serializable]
        public struct Data
        {
            public string name;
            public Sprite icon;
            public int value;
        }
    }
}