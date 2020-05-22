using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System;
using VRage.Collections;
using VRage.Game.Components;
using VRage.Game.GUI.TextPanel;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ObjectBuilders.Definitions;
using VRage.Game;
using VRage;
using VRageMath;

namespace IngameScript
{
    partial class Program : MyGridProgram
    {
        FiniteStateMachine _fsm = new FiniteStateMachine();
        IMyInteriorLight _light1, _light2, _light3;

        public Program()
        {
            Runtime.UpdateFrequency = UpdateFrequency.Once | UpdateFrequency.Update100;

            Setup();

            string[] storedData = Storage.Split(';');
            if (storedData.Length >= 1)
            {
                _fsm.SetCurrentStateName(storedData[0]);
            }
        }

        public void Save()
        {
            Storage = string.Join(";", _fsm.GetCurrentStateName() ?? "ERROR");
        }

        public void Main(string argument, UpdateType updateSource)
        {
            _fsm.DoFirstPossibleStateTransition();

            Echo($"State: {_fsm.GetCurrentStateName()}");
            Echo($"LastRunTimeMs: {Runtime.LastRunTimeMs}");
            Echo($"InstructionCount: {Runtime.CurrentInstructionCount} / {Runtime.MaxInstructionCount}");
        }

        private void Setup()
        {
            _light1 = GridTerminalSystem.GetBlockWithName("Interior Light") as IMyInteriorLight;
            _light2 = GridTerminalSystem.GetBlockWithName("Interior Light 2") as IMyInteriorLight;
            _light3 = GridTerminalSystem.GetBlockWithName("Interior Light 3") as IMyInteriorLight;

            _fsm.AddState("CHASER1", () => SetLightColor(_light1, Color.Red), () => SetLightColor(_light1, Color.White));
            _fsm.AddState("CHASER2", () => SetLightColor(_light2, Color.Red), () => SetLightColor(_light2, Color.White));
            _fsm.AddState("CHASER3", () => SetLightColor(_light3, Color.Red), () => SetLightColor(_light3, Color.White));

            _fsm.AddStateTransition("CHASER1", "CHASER2", () => true);
            _fsm.AddStateTransition("CHASER2", "CHASER3", () => true);
            _fsm.AddStateTransition("CHASER3", "CHASER1", () => true);

            _fsm.SetCurrentStateName("CHASER1");
        }

        private void SetLightColor(IMyInteriorLight light, Color color)
        {
            light.Color = color;
        }
    }
}
