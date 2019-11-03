using System.Collections.Generic;
using uTinyRipper.Classes;
using uTinyRipper.Classes.AnimatorControllers;
using uTinyRipper.Converters;
using uTinyRipper.SerializedFiles;
using uTinyRipper.Classes.Misc;
using uTinyRipper.Classes.BlendTrees;

namespace uTinyRipper.Project
{
	public class AnimatorControllerExportCollection : AssetsExportCollection
	{
		public AnimatorControllerExportCollection(IAssetExporter assetExporter, VirtualSerializedFile virtualFile, Object asset) :
			this(assetExporter, virtualFile, (AnimatorController)asset)
		{
		}

		public AnimatorControllerExportCollection(IAssetExporter assetExporter, VirtualSerializedFile virtualFile, AnimatorController asset) :
			base(assetExporter, asset)
		{
			ControllerConstant controller = asset.Controller;
			IReadOnlyList<OffsetPtr<StateMachineConstant>> stateMachinesConst = controller.StateMachineArray;
			StateMachines = new AnimatorStateMachine[stateMachinesConst.Count];
			for (int i = 0; i < stateMachinesConst.Count; i++)
			{
				AnimatorStateMachine stateMachine = AnimatorStateMachine.CreateVirtualInstance(virtualFile, asset, i);
				StateMachines[i] = stateMachine;
			}

#warning TODO: export MonoBehaviours
			for (int i = 0; i < StateMachines.Length; i++)
			{
				AnimatorStateMachine stateMachine = StateMachines[i];
				StateMachineConstant stateMachineConstant = asset.Controller.StateMachineArray[i].Instance;
				AddAsset(stateMachine);

				foreach (PPtr<AnimatorStateTransition> transitionPtr in stateMachine.AnyStateTransitions)
				{
					AnimatorStateTransition transition = transitionPtr.GetAsset(virtualFile);
					AddAsset(transition);
				}
				foreach (PPtr<AnimatorTransition> transitionPtr in stateMachine.EntryTransitions)
				{
					AnimatorTransition transition = transitionPtr.GetAsset(virtualFile);
					AddAsset(transition);
				}

				for (int j = 0; j < stateMachine.ChildStates.Length; j++)
				{
					PPtr<AnimatorState> statePtr = stateMachine.ChildStates[j].State;
					AnimatorState state = statePtr.GetAsset(virtualFile);
					StateConstant stateConstant = stateMachineConstant.StateConstantArray[j].Instance;
					AddAsset(state);

					if (state.Motion.IsVirtual)
					{
						Motion motion = state.Motion.GetAsset(virtualFile);
						AddBlendTree(virtualFile, (BlendTree)motion);
					}

					for (int k = 0; k < state.Transitions.Length; k++)
					{
						PPtr<AnimatorStateTransition> transitionPtr = state.Transitions[k];
						AnimatorStateTransition transition = transitionPtr.GetAsset(virtualFile);
						TransitionConstant transitionConstant = stateConstant.TransitionConstantArray[k].Instance;

						AddAsset(transition);
					}
				}
			}
		}

		private void AddBlendTree(VirtualSerializedFile virtualFile, BlendTree blendTree)
		{
			AddAsset(blendTree);
			foreach (ChildMotion childMotion in blendTree.Childs)
			{
				if (childMotion.Motion.IsVirtual)
				{
					Motion motion = childMotion.Motion.GetAsset(virtualFile);
					AddBlendTree(virtualFile, (BlendTree)motion);
				}
			}
		}

		public AnimatorStateMachine[] StateMachines { get; set; }
	}
}
